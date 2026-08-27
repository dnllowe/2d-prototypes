import { pickAiTurnAction, pickAiPlacement } from "./ai";
import {
  applyBlockPlacement,
  applyMove,
  applyPresetFormation,
  attackForce,
  blocksAvailable,
  canKnock,
  canPlace,
  countPieces,
  endTurn,
  hasLegalActions,
  idx,
  initBlocksLeft,
  initLeaveBoardAt,
  initTurnStartAt,
  legalBlockPlacements,
  legalMoves,
  mirrorIndex,
  newGame,
  placePiece,
  unplacePiece,
  stalemateOutcome,
  tickAfterTurn,
  tickRound,
  nextMarkTurn,
  validateConfig,
  warningRoundsLeft,
  type Cell,
  type Config,
  type GameState,
  type Player,
} from "./game";

function fail(msg: string): never {
  throw new Error(msg);
}

function assert(cond: unknown, msg: string): asserts cond {
  if (!cond) fail(msg);
}

function base(over: Partial<Config> = {}): Config {
  return {
    n: 6,
    k: 5,
    placeOnColor: false,
    vsAi: false,
    revealPlacement: true,
    setupMode: "free",
    formationSource: "player",
    overlapMode: "block",
    winMode: "elimination",
    winTarget: 3,
    drawOnOneEach: true,
    actionsMode: "fixed",
    knockStrength: "simple",
    requireBoardChange: true,
    push: true,
    pushLanding: "impact",
    spawnBlocks: false,
    spawnEveryRounds: 2,
    spawnWarningRounds: 1,
    playerBlocks: 3,
    firstPlacerFirst: true,
    aiStyle: "simple",
    ...over,
  };
}

function parseBoard(
  rows: string[],
  current: Player = 0,
  over: Partial<Config> = {},
): GameState {
  const n = rows.length;
  const cells: Cell[] = [];
  for (const row of rows) {
    assert(row.length === n, "board must be square");
    for (const ch of row) {
      if (ch === ".") cells.push(null);
      else if (ch === "0") cells.push(0);
      else if (ch === "1") cells.push(1);
      else if (ch === "B") cells.push("block");
      else if (ch === "D") cells.push("block0");
      else if (ch === "E") cells.push("block1");
      else fail(`bad cell ${ch}`);
    }
  }
  const cfg = base({ n, ...over });
  return {
    ...cfg,
    phase: "play",
    current,
    cells,
    drafts: [[], []],
    placed: 0,
    actionsLeft: (() => {
      const pieces = countPieces(cells, current);
      if (pieces === 0) return 0;
      const mode = over.actionsMode ?? "fixed";
      return mode === "perPiece" ? Math.min(3, pieces) : 3;
    })(),
    knockouts: [0, 0],
    replaceLeft: [0, 0],
    colors: ["#a", "#b"],
    names: ["A", "B"],
    winner: null,
    endReason: null,
    skipNotice: null,
    turnStartAt: initTurnStartAt(cells, current),
    leaveBoardAt: initLeaveBoardAt(cells.length),
    turnsFinished: 0,
    spawnMarksDone: 0,
    warnings: [],
    blocksLeft: initBlocksLeft(cfg.playerBlocks),
    emptyPasses: [0, 0],
    turnChangedBoard: false,
  };
}

assert(validateConfig(base()) === null, "default config ok");
assert(validateConfig(base({ k: 5, winMode: "knockouts", winTarget: 5 })) !== null, "KO < k");
assert(validateConfig(base({ placeOnColor: true })) === null, "color place ok");
assert(
  validateConfig(base({ spawnBlocks: true, spawnEveryRounds: 0 })) !== null,
  "spawn every ≥ 1",
);
assert(
  validateConfig(
    base({ spawnBlocks: true, spawnEveryRounds: 2, spawnWarningRounds: 1 }),
  ) === null,
  "spawn config ok",
);

// Always 3 actions even with one piece
{
  const one = parseBoard(["01..", "....", "....", "...."], 0, { actionsMode: "fixed" });
  assert(one.actionsLeft === 3, "fixed mode always 3");
  const capped = parseBoard(["01..", "....", "....", "...."], 0, { actionsMode: "perPiece" });
  assert(capped.actionsLeft === 1, "perPiece caps to pieces");
}

let g = newGame(base({ n: 4, k: 2 }), ["#a", "#b"], ["A", "B"]);
assert(canPlace(g, idx(4, 0, 0)), "can place");
g = placePiece(g, idx(4, 0, 0));
g = unplacePiece(g, idx(4, 0, 0));
assert(g.placed === 0 && g.cells[idx(4, 0, 0)] === null, "unplace clears");
g = placePiece(g, idx(4, 0, 0));
g = placePiece(g, idx(4, 0, 1));
assert(g.current === 1, "P2 places");
g = placePiece(g, idx(4, 1, 0));
g = placePiece(g, idx(4, 1, 1));
assert(g.phase === "play", "enter play revealed");
assert(g.current === 0, "first placer goes first by default");

// Formation unplace
{
  let f = newGame(base({ n: 4, k: 2, setupMode: "formation" }), ["#a", "#b"], ["A", "B"]);
  f = placePiece(f, 0);
  assert(f.placed === 1 && f.cells[0] === 0, "formation placed");
  f = unplacePiece(f, 0);
  assert(f.placed === 0 && f.cells[0] === null, "formation unplace");
}

g = newGame(base({ n: 4, k: 2, firstPlacerFirst: false }), ["#a", "#b"], ["A", "B"]);
g = placePiece(g, idx(4, 0, 0));
g = placePiece(g, idx(4, 0, 1));
g = placePiece(g, idx(4, 1, 0));
g = placePiece(g, idx(4, 1, 1));
assert(g.phase === "play" && g.current === 1, "second placer first when option off");

// Blind overlap → block
g = newGame(base({ n: 4, k: 1, revealPlacement: false, overlapMode: "block" }), ["#a", "#b"], ["A", "B"]);
g = placePiece(g, 0);
assert(g.current === 1, "blind handoff");
g = placePiece(g, 0);
assert(g.cells[0] === "block", "overlap block");
assert(
  g.phase === "play" || (g.phase === "over" && g.endReason === "mutual"),
  "blind resolve",
);

// Blind overlap → eliminate
g = newGame(
  base({ n: 4, k: 1, revealPlacement: false, overlapMode: "eliminate" }),
  ["#a", "#b"],
  ["A", "B"],
);
g = placePiece(g, 0);
g = placePiece(g, 0);
assert(g.cells[0] === null, "overlap cleared");
assert(g.phase === "over" && g.winner === "draw", "mutual empty draw");

// Blind overlap → replace
g = newGame(
  base({ n: 4, k: 1, revealPlacement: false, overlapMode: "replace" }),
  ["#a", "#b"],
  ["A", "B"],
);
g = placePiece(g, 0);
g = placePiece(g, 0);
assert(g.phase === "replace", "replace phase");
assert(g.replaceLeft[0] === 1 && g.replaceLeft[1] === 1, "both re-place");
g = placePiece(g, 1);
assert(g.current === 1, "P2 re-places");
g = placePiece(g, 2);
assert(
  g.phase === "play" || (g.phase === "over" && g.endReason === "one_each"),
  "replace done",
);

// Formation mirror
g = newGame(base({ n: 4, k: 2, setupMode: "formation" }), ["#a", "#b"], ["A", "B"]);
assert(canPlace(g, 0), "formation half");
assert(!canPlace(g, mirrorIndex(4, 0)), "not mirror half");
g = placePiece(g, 0);
g = placePiece(g, 1);
assert(g.cells[0] === 0 && g.cells[mirrorIndex(4, 0)] === 1, "mirrored");
assert(g.cells[1] === 0 && g.cells[mirrorIndex(4, 1)] === 1, "mirrored pair");
assert(g.phase === "play" || g.phase === "over", "formation resolved");

g = newGame(
  base({ n: 6, k: 5, setupMode: "formation", formationSource: "preset" }),
  ["#a", "#b"],
  ["A", "B"],
);
g = applyPresetFormation(g);
assert(g.phase === "play" || g.phase === "replace" || g.phase === "over", "preset applied");
assert(countPieces(g.cells, 0) === countPieces(g.cells, 1), "fair mirror counts");

// Open edge illegal
g = parseBoard(["0...", "....", "....", "...."]);
assert(!legalMoves(g).some((m) => m.dir === "right"), "no open edge slide");

// Block stops
g = parseBoard(["0B.1", "....", "....", "...."]);
assert(!legalMoves(g).some((m) => m.from === 0 && m.dir === "right"), "blocked adjacent");

g = parseBoard(["0.B1", "....", "....", "...."]);
const stopBefore = legalMoves(g).find((m) => m.from === 0 && m.dir === "right");
assert(stopBefore && stopBefore.to === idx(4, 0, 1) && stopBefore.knock === null, "stop before block");

// Knockouts win
g = parseBoard([".01.", "...1", "....", "...."], 0, {
  winMode: "knockouts",
  winTarget: 1,
  k: 2,
  drawOnOneEach: false,
});
const ko = legalMoves(g).find(
  (m) => m.from === idx(4, 0, 1) && m.dir === "right" && m.knock !== null,
);
assert(ko, "knock");
g = applyMove(g, ko);
assert(g.winner === 0 && g.endReason === "knockouts", `KO win got ${g.endReason}`);
assert(g.knockouts[0] === 1, "KO counted");
assert(countPieces(g.cells, 1) === 1, "opp still has a piece");

// reduceTo win
g = parseBoard([".01.", "0..1", "....", "...."], 0, {
  winMode: "reduceTo",
  winTarget: 1,
  k: 2,
  drawOnOneEach: false,
});
const red = legalMoves(g).find(
  (m) => m.from === idx(4, 0, 1) && m.dir === "right" && m.knock !== null,
);
assert(red, "reduce knock");
g = applyMove(g, red!);
assert(g.winner === 0 && g.endReason === "reduceTo", "reduceTo win");

// Cumulative knock force / Push
{
  // EEI — force 2 vs lone I → knock
  let c = parseBoard(["001.", "....", "....", "...."], 0, {
    knockStrength: "cumulative",
    requireBoardChange: true,
    drawOnOneEach: false,
  });
  const from = idx(4, 0, 1); // rightmost 0 is the slider into 1
  assert(attackForce(c, from, "right") === 2, "force 2");
  assert(canKnock(c, from, "right", idx(4, 0, 2)), "2 vs lone");
  let mv = legalMoves(c).find((m) => m.from === from && m.dir === "right" && m.knocks);
  assert(mv, "EEI legal knock");
  c = applyMove(c, mv!);
  assert(countPieces(c.cells, 1) === 0, "lone defended knocked off");

  // Adjacent into braced stack: never a push (no run-up)
  c = parseBoard(["0011", "....", "....", "...."], 0, {
    knockStrength: "cumulative",
    requireBoardChange: true,
    push: true,
  });
  assert(
    !legalMoves(c).some((m) => m.from === idx(4, 0, 1) && m.dir === "right"),
    "adjacent stack omitted",
  );
  c = parseBoard(["0011", "....", "....", "...."], 0, {
    knockStrength: "simple",
    requireBoardChange: true,
    push: true,
  });
  assert(
    !legalMoves(c).some((m) => m.from === idx(4, 0, 1) && m.dir === "right"),
    "adjacent never pushes even in simple",
  );

  // Push on + gap: shove stack one, impact landing leaves a gap
  c = parseBoard(
    ["0.11.", ".....", ".....", ".....", "....."],
    0,
    {
      push: true,
      pushLanding: "impact",
      requireBoardChange: true,
    },
  );
  mv = legalMoves(c).find(
    (m) =>
      m.from === idx(5, 0, 0) &&
      m.dir === "right" &&
      m.knocks &&
      m.to === idx(5, 0, 1),
  );
  assert(mv, "push with momentum");
  c = applyMove(c, mv!);
  assert(c.cells[idx(5, 0, 1)] === 0, "slider at impact");
  assert(c.cells[idx(5, 0, 2)] === null, "gap after shove");
  assert(c.cells[idx(5, 0, 3)] === 1 && c.cells[idx(5, 0, 4)] === 1, "stack shifted");

  // Follow landing: slider ends next to pushed stack
  c = parseBoard(
    ["0.11.", ".....", ".....", ".....", "....."],
    0,
    {
      push: true,
      pushLanding: "follow",
      requireBoardChange: true,
    },
  );
  mv = legalMoves(c).find(
    (m) =>
      m.from === idx(5, 0, 0) &&
      m.dir === "right" &&
      m.knocks &&
      m.to === idx(5, 0, 2),
  );
  assert(mv, "follow push lands on vacated front");
  c = applyMove(c, mv!);
  assert(c.cells[idx(5, 0, 2)] === 0, "slider followed");
  assert(c.cells[idx(5, 0, 3)] === 1 && c.cells[idx(5, 0, 4)] === 1, "stack shifted");

  // Push off: approach stack but do not move it
  c = parseBoard(
    ["0.11.", ".....", ".....", ".....", "....."],
    0,
    {
      push: false,
      requireBoardChange: true,
    },
  );
  mv = legalMoves(c).find(
    (m) =>
      m.from === idx(5, 0, 0) &&
      m.dir === "right" &&
      m.to === idx(5, 0, 1) &&
      m.knock !== null &&
      !m.knocks,
  );
  assert(mv, "slide into braced without push");
  c = applyMove(c, mv!);
  assert(c.cells[idx(5, 0, 1)] === 0, "slider moved");
  assert(c.cells[idx(5, 0, 2)] === 1 && c.cells[idx(5, 0, 3)] === 1, "stack unmoved");

  // Same adjacent but requireBoardChange off → wasted action allowed
  c = parseBoard(["0011", "....", "....", "...."], 0, {
    knockStrength: "cumulative",
    requireBoardChange: false,
    push: true,
  });
  mv = legalMoves(c).find(
    (m) => m.from === idx(4, 0, 1) && m.dir === "right" && m.knock !== null && !m.knocks,
  );
  assert(mv && mv.to === idx(4, 0, 1), "failed knock legal when board change not required");
  const before = c.cells.join(",");
  c = applyMove(c, mv!);
  assert(c.cells.join(",") === before, "failed adjacent knock no board change");
  assert(c.actionsLeft === 2, "action spent");

  // Push off the edge eliminates the rear piece
  c = parseBoard(["0.11", "....", "....", "...."], 0, {
    push: true,
    pushLanding: "impact",
    requireBoardChange: true,
    drawOnOneEach: false,
  });
  mv = legalMoves(c).find((m) => m.from === 0 && m.dir === "right" && m.knocks);
  assert(mv, "push toward edge");
  c = applyMove(c, mv!);
  assert(countPieces(c.cells, 1) === 1, "rear eliminated off edge");
  assert(c.cells[idx(4, 0, 3)] === 1, "front shifted to edge");
  assert(c.knockouts[0] === 1, "KO credited");

  // Braced contact with no room to displace is illegal when board change required
  c = parseBoard(["0.11B", ".....", ".....", ".....", "....."], 0, {
    push: true,
    requireBoardChange: true,
  });
  assert(
    !legalMoves(c).some((m) => m.from === idx(5, 0, 0) && m.dir === "right" && m.knocks),
    "blocked stack push omitted",
  );
  // Still legal to slide up without pushing when push can't displace
  mv = legalMoves(c).find(
    (m) => m.from === idx(5, 0, 0) && m.dir === "right" && !m.knocks,
  );
  assert(mv && mv.to === idx(5, 0, 1), "approach without push when jammed");

  // Victim flush against a block cannot be knocked into it
  c = parseBoard([".01B.", ".....", ".....", ".....", "....."], 0, {
    knockStrength: "simple",
    requireBoardChange: true,
  });
  assert(
    !legalMoves(c).some(
      (m) => m.from === idx(5, 0, 1) && m.dir === "right" && m.knocks,
    ),
    "no knock into adjacent block",
  );
  c = parseBoard([".0.1.B", "......", "......", "......", "......", "......"], 0, {
    knockStrength: "simple",
    requireBoardChange: true,
  });
  mv = legalMoves(c).find(
    (m) => m.from === idx(6, 0, 1) && m.dir === "right" && m.knocks,
  );
  assert(mv, "knock with room before block");
  c = applyMove(c, mv!);
  assert(c.cells[idx(6, 0, 4)] === 1, "victim stops before block");
  assert(c.cells[idx(6, 0, 5)] === "block", "block unmoved");
}

// Flip-flop: blocked only when board unchanged since leaving home
{
  let c = parseBoard(["00.B", "....", "....", "1..."], 0, {
    drawOnOneEach: false,
  });
  const out = legalMoves(c).find((m) => m.from === 1 && m.dir === "right");
  assert(out && out.to === 2, "slide away");
  c = applyMove(c, out!);
  assert(
    !legalMoves(c).some((m) => m.from === 2 && m.to === 1),
    "cannot return home if board unchanged",
  );

  // Block after leaving home — return is legal
  c = applyBlockPlacement(c, idx(4, 1, 0));
  assert(
    legalMoves(c).some((m) => m.from === 2 && m.to === 1),
    "return home after board changed",
  );

  // Block before leaving — return still illegal
  c = parseBoard(["00.B", "....", "....", "1..."], 0, {
    drawOnOneEach: false,
    playerBlocks: 3,
  });
  c = applyBlockPlacement(c, idx(4, 1, 0));
  c = applyMove(
    c,
    legalMoves(c).find((m) => m.from === 1 && m.dir === "right")!,
  );
  assert(
    !legalMoves(c).some((m) => m.from === 2 && m.to === 1),
    "block before leave does not enable flip-flop",
  );

  c = parseBoard(["00.B", "....", "....", "1..."], 0, { drawOnOneEach: false });
  c = applyMove(c, legalMoves(c).find((m) => m.from === 1 && m.dir === "right")!);
  c = {
    ...c,
    turnStartAt: initTurnStartAt(c.cells, 0),
    leaveBoardAt: initLeaveBoardAt(c.cells.length),
    actionsLeft: 3,
  };
  assert(
    legalMoves(c).some((m) => m.from === 2 && m.to === 1),
    "return home legal on fresh turn",
  );
}

// Ram: momentum into own piece shoves it into an enemy beyond (and clears flip-flop)
{
  // Column: Ink, Ember, Ember-home, empty, Ember-slider, block (so leaving home is legal)
  let c = parseBoard(
    ["1.....", "0.....", "0.....", "......", "......", "B....."],
    0,
    { push: true, pushLanding: "impact", drawOnOneEach: false, playerBlocks: 0 },
  );
  // Leave home at row2 → row4 (stops before block at row5)
  const leave = legalMoves(c).find(
    (m) => m.from === idx(6, 2, 0) && m.dir === "down" && m.to === idx(6, 4, 0),
  );
  assert(leave, "slide away down");
  c = applyMove(c, leave!);
  assert(c.turnStartAt[idx(6, 4, 0)] === idx(6, 2, 0), "home remembered");

  const ram = legalMoves(c).find(
    (m) =>
      m.from === idx(6, 4, 0) &&
      m.dir === "up" &&
      m.knocks &&
      m.knock === idx(6, 0, 0) &&
      m.to === idx(6, 2, 0),
  );
  assert(ram, "return home legal when it rams/pushes");
  c = applyMove(c, ram!);
  assert(c.cells[idx(6, 2, 0)] === 0, "slider back at impact/home");
  assert(c.cells[idx(6, 1, 0)] === null, "friendly vacated");
  assert(c.cells[idx(6, 0, 0)] === 0, "friendly shoved into enemy square");
  assert(countPieces(c.cells, 1) === 0, "enemy eliminated off edge");
  assert(c.knockouts[0] === 1, "KO credited for ram");
}

// Dynamic neutral block spawns
{
  let c = parseBoard(["0.1.", "....", "....", "...."], 0, {
    spawnBlocks: true,
    spawnEveryRounds: 2,
    spawnWarningRounds: 1,
    drawOnOneEach: false,
  });
  // Round 1 (2 turns): no mark yet
  c = tickRound(c, () => 0);
  assert(c.turnsFinished === 2 && c.warnings.length === 0, "no mark after round 1");
  // Round 2 ends on turn 4 → mark (after P0 if P1 opened)
  c = tickRound(c, () => 0);
  assert(c.turnsFinished === 4, "turn 4");
  assert(c.warnings.length === 1 && c.warnings[0]!.i === 0, "warned first candidate");
  assert(warningRoundsLeft(c.warnings[0]!, c.turnsFinished) === 1, "warning lasts 1");
  assert(c.cells[0] === 0, "piece still there during warning");
  // After one more full round (turns 5–6): materialize
  const koBefore = c.knockouts[0] + c.knockouts[1];
  c = tickRound(c, () => 0);
  assert(c.warnings.length === 0, "warning cleared");
  assert(c.cells[0] === "block", "became block");
  assert(countPieces(c.cells, 0) === 0, "piece eliminated");
  assert(c.knockouts[0] + c.knockouts[1] === koBefore, "no KO credit");
  assert(c.winner === 1 && c.endReason === "wipeout", "implicit wipeout");

  // Occupied warn under a piece; other pieces keep the game going
  c = parseBoard(["01..", "0...", "....", ".1.."], 0, {
    spawnBlocks: true,
    spawnEveryRounds: 2,
    spawnWarningRounds: 1,
    drawOnOneEach: false,
  });
  c = tickRound(c, () => 0); // turns 1–2
  c = tickRound(c, () => 0); // turns 3–4 mark
  assert(c.warnings[0]!.i === 0, "warn on piece");
  c = tickRound(c, () => 0); // turns 5–6 materialize
  assert(c.cells[0] === "block", "materialized");
  assert(c.phase === "play", "still playing with remaining pieces");
  assert(countPieces(c.cells, 0) === 1, "one P0 piece gone");
  assert(countPieces(c.cells, 1) === 2, "P1 intact");

  // Instant materialize when warning rounds = 0
  c = parseBoard(["....", "....", "....", "...."], 0, {
    spawnBlocks: true,
    spawnEveryRounds: 1,
    spawnWarningRounds: 0,
  });
  c = tickRound(c, () => 0);
  assert(c.warnings.length === 0, "no lingering warn");
  assert(c.cells[0] === "block", "immediate block");

  // Alternating mark turns: 4 (even), 7 (odd), 12 (even), ...
  assert(nextMarkTurn(0, 2) === 4, "first mark turn 4");
  assert(nextMarkTurn(1, 2) === 7, "second mark turn 7");
  assert(nextMarkTurn(2, 2) === 12, "third mark turn 12");
  c = parseBoard(["0.1.", "0.1.", "....", "...."], 0, {
    spawnBlocks: true,
    spawnEveryRounds: 2,
    spawnWarningRounds: 1,
    drawOnOneEach: false,
  });
  for (let t = 0; t < 4; t++) c = tickAfterTurn(c, () => 0);
  assert(c.spawnMarksDone === 1 && c.turnsFinished === 4, "mark after even turn");
  for (let t = 0; t < 3; t++) c = tickAfterTurn(c, () => 0);
  assert(c.spawnMarksDone === 2 && c.turnsFinished === 7, "next mark after odd turn");
}

// Player-placed blocks
{
  let c = parseBoard(["0...", "....", "....", "1..."], 0, {
    playerBlocks: 3,
    drawOnOneEach: false,
  });
  assert(c.blocksLeft[0] === 3, "starts with 3 blocks");
  assert(legalBlockPlacements(c).includes(1), "empty spot");
  assert(!legalBlockPlacements(c).includes(0), "occupied");
  c = applyBlockPlacement(c, 1);
  assert(c.cells[1] === "block0", "block placed");
  assert(c.blocksLeft[0] === 2, "block inventory drained");
  assert(c.actionsLeft === 2, "uses one action");
  assert(!legalMoves(c).some((m) => m.from === 0 && m.dir === "right"), "adjacent block stops slide");

  // Block-only turn continuation
  c = parseBoard(["0...", "....", "....", ".1.."], 0, {
    playerBlocks: 3,
    drawOnOneEach: false,
  });
  assert(legalMoves(c).length === 0, "no slides open edge");
  assert(legalBlockPlacements(c).length > 0, "can still block");
  assert(hasLegalActions(c), "block counts as legal action");
  c = applyBlockPlacement(c, 1);
  assert(c.actionsLeft === 2 && c.phase === "play", "continues turn");

  // Disabled when playerBlocks = 0
  c = parseBoard(["0...", "....", "....", "...."], 0, { playerBlocks: 0 });
  assert(!blocksAvailable(c), "blocks off");
  assert(legalBlockPlacements(c).length === 0, "no placements");
}

// End turn / empty pass / forfeit (per player)
{
  let c = parseBoard(["0..1", "....", "....", "...."], 0, {
    drawOnOneEach: false,
  });
  assert(c.current === 0 && c.actionsLeft === 3, "start of turn");
  assert(c.emptyPasses[0] === 0 && c.emptyPasses[1] === 0, "both at zero");
  c = endTurn(c);
  assert(c.current === 1, "hand off after end turn");
  assert(c.emptyPasses[0] === 1 && c.emptyPasses[1] === 0, "only p0 marked");
  assert(c.phase === "play", "still playing");

  // P1's first empty pass — independent of P0
  c = endTurn(c);
  assert(
    c.current === 0 && c.emptyPasses[0] === 1 && c.emptyPasses[1] === 1,
    "each player has their own empty pass",
  );

  // Second empty pass for P0 is a forfeit; P1's count is untouched
  assert(c.emptyPasses[0] === 1 && !c.turnChangedBoard, "p0 still on empty streak");
  c = endTurn(c);
  assert(c.phase === "over" && c.endReason === "forfeit", "second empty pass forfeits");
  assert(c.winner === 1, "opponent wins forfeit");
  assert(c.emptyPasses[1] === 1, "p1 counter unchanged by p0 forfeit");

  // Board change resets only that player's empty-pass streak
  c = parseBoard(["0..B", "....", "....", "1..."], 0, { drawOnOneEach: false });
  c = endTurn(c); // p0 empty → [1, 0]
  c = endTurn(c); // p1 empty → [1, 1]
  assert(c.emptyPasses[0] === 1 && c.emptyPasses[1] === 1, "both marked once");
  const slide = legalMoves(c).find((m) => m.from === 0 && m.dir === "right");
  assert(slide, "has slide");
  c = applyMove(c, slide!);
  assert(c.phase === "play", "still in play after quiet slide");
  assert(
    c.turnChangedBoard && c.emptyPasses[0] === 0 && c.emptyPasses[1] === 1,
    "move resets only current player's empty pass",
  );
  c = endTurn(c);
  assert(c.emptyPasses[0] === 0 && c.emptyPasses[1] === 1 && c.current === 1, "end after move");
  // P1 can still safely empty-pass once more? No — they already have 1, so next is forfeit
  c = endTurn(c);
  assert(c.phase === "over" && c.winner === 0 && c.endReason === "forfeit", "p1 second empty forfeits");
}

// Stalemate: most pieces win (tie → draw)
{
  const more: Cell[] = [0, 0, 1, null];
  assert(stalemateOutcome(more).winner === 0, "stalemate more pieces");
  const tie: Cell[] = [0, 1, null, null];
  assert(stalemateOutcome(tie).winner === "draw", "stalemate tie");
}

// Isolated armies end via enterPlay stalemate
g = newGame(
  base({ n: 5, k: 2, drawOnOneEach: false, playerBlocks: 0 }),
  ["#a", "#b"],
  ["A", "B"],
);
g = placePiece(g, idx(5, 0, 0));
g = placePiece(g, idx(5, 0, 1));
g = placePiece(g, idx(5, 4, 3));
g = placePiece(g, idx(5, 4, 4));
assert(legalMoves({ ...g, phase: "play", current: 0, actionsLeft: 3 }).length === 0, "no P0 slides");
assert(legalMoves({ ...g, phase: "play", current: 1, actionsLeft: 3 }).length === 0, "no P1 slides");
assert(g.phase === "over" && g.endReason === "stalemate", `enterPlay stalemate got ${g.phase} ${g.endReason}`);
assert(g.winner === "draw", "equal pieces stalemate draw");

// 1v1 draw optional
g = newGame(base({ n: 4, k: 1, drawOnOneEach: true }), ["#a", "#b"], ["A", "B"]);
g = placePiece(g, 0);
g = placePiece(g, 1);
assert(g.phase === "over" && g.endReason === "one_each", "1v1 draw on");

g = newGame(base({ n: 4, k: 1, drawOnOneEach: false }), ["#a", "#b"], ["A", "B"]);
g = placePiece(g, 0);
g = placePiece(g, 1);
assert(g.phase === "play", "1v1 draw off stays in play");

// Thoughtful prefers a knock over a quiet slide
{
  const board = parseBoard(["01.B", "0...", "....", ".1.."], 0, {
    vsAi: true,
    aiStyle: "thoughtful",
    playerBlocks: 0,
    drawOnOneEach: false,
  });
  const action = pickAiTurnAction(board, () => 0);
  assert(action && action.kind === "slide" && action.move.knocks, "thoughtful knocks");
}

// Placement: revealed vs blind
{
  let g = newGame(
    base({
      n: 4,
      k: 2,
      vsAi: true,
      aiStyle: "aggressive",
      revealPlacement: true,
      playerBlocks: 0,
    }),
    ["#a", "#b"],
    ["A", "B"],
  );
  g = placePiece(g, 0);
  g = placePiece(g, 1);
  assert(g.current === 1 && g.phase === "place", "AI places second");
  const ag = pickAiPlacement({ ...g, aiStyle: "aggressive" }, () => 0);
  const pa = pickAiPlacement({ ...g, aiStyle: "patient" }, () => 0);
  assert(ag !== null && pa !== null, "heuristic placement");
  const minDist = (spot: number) => {
    let d = 99;
    for (const i of [0, 1]) {
      const a = Math.abs(Math.floor(spot / 4) - Math.floor(i / 4)) + Math.abs((spot % 4) - (i % 4));
      d = Math.min(d, a);
    }
    return d;
  };
  assert(minDist(ag!) <= minDist(pa!), "aggressive closer when pieces visible");

  g = newGame(
    base({
      n: 4,
      k: 2,
      vsAi: true,
      aiStyle: "thoughtful",
      revealPlacement: false,
      playerBlocks: 0,
    }),
    ["#a", "#b"],
    ["A", "B"],
  );
  g = placePiece(g, 0);
  g = placePiece(g, 1);
  assert(countPieces(g.cells, 0) === 0, "blind hides P1");
  const blind = pickAiPlacement(g, () => 0);
  assert(blind !== null, "blind placement still picks");
}

// Complex does not hang
{
  const board = parseBoard(["0.1.", "0.1.", "....", "...."], 0, {
    vsAi: true,
    aiStyle: "complex",
    playerBlocks: 3,
    drawOnOneEach: false,
  });
  const t0 = Date.now();
  const action = pickAiTurnAction(board, () => 0);
  assert(action, "complex picks");
  assert(Date.now() - t0 < 2000, "complex bounded");
}

// AI playout
let s = newGame(
  base({ n: 6, k: 4, vsAi: true, aiStyle: "simple", playerBlocks: 0 }),
  ["#a", "#b"],
  ["A", "B"],
);
let guard = 0;
while (s.phase === "place" || s.phase === "replace") {
  const spot = pickAiPlacement(s);
  assert(spot !== null, "placement");
  s = placePiece(s, spot);
  assert(++guard < 80, "place loop");
}
guard = 0;
while (s.phase === "play") {
  const action = pickAiTurnAction(s);
  if (!action) break;
  s =
    action.kind === "slide"
      ? applyMove(s, action.move)
      : applyBlockPlacement(s, action.at);
  assert(++guard < 1000, "play loop");
}
assert(s.phase === "over", "finished");

console.log("verify ok");
