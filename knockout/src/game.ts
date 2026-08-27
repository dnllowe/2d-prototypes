export type Player = 0 | 1;
export type NeutralBlock = "block";
export type PlayerBlock = "block0" | "block1";
export type BlockCell = NeutralBlock | PlayerBlock;
export type Cell = Player | BlockCell | null;
export type Phase = "place" | "replace" | "play" | "over";
export type Direction = "up" | "down" | "left" | "right";
export type SetupMode = "free" | "formation";
export type FormationSource = "player" | "preset";
export type OverlapMode = "block" | "eliminate" | "replace";
export type WinMode = "elimination" | "knockouts" | "reduceTo";
export type ActionsMode = "fixed" | "perPiece";
export type KnockStrength = "simple" | "cumulative";
export type AiStyle = "simple" | "thoughtful" | "complex" | "aggressive" | "patient";
/** After a successful stack push: stay at impact (gap) or follow into the vacated square. */
export type PushLanding = "impact" | "follow";

export const DIRECTIONS: Direction[] = ["up", "down", "left", "right"];
export const ACTIONS_CAP = 3;

export const DIR_DELTA: Record<Direction, readonly [number, number]> = {
  up: [-1, 0],
  down: [1, 0],
  left: [0, -1],
  right: [0, 1],
};

export type Config = {
  n: number;
  k: number;
  placeOnColor: boolean;
  vsAi: boolean;
  /** Opponent temperament when vsAi is true. */
  aiStyle: AiStyle;
  /** When true, P2 sees P1's pieces while placing. */
  revealPlacement: boolean;
  setupMode: SetupMode;
  formationSource: FormationSource;
  overlapMode: OverlapMode;
  winMode: WinMode;
  /** Knockouts needed, or opponent pieces remaining (reduceTo). Ignored for elimination. */
  winTarget: number;
  /** Auto-draw when both have exactly one piece. */
  drawOnOneEach: boolean;
  /** fixed = always 3 slides; perPiece = min(3, pieces left). */
  actionsMode: ActionsMode;
  /** simple = one attacker; cumulative = stack force behind the slider. */
  knockStrength: KnockStrength;
  /** When true, moves that would not change the board are illegal. */
  requireBoardChange: boolean;
  /**
   * When true, a slide that crosses ≥1 empty into a 2+ stack can shove that
   * stack one square (if it has room). Adjacent contact never pushes.
   */
  push: boolean;
  /** Where the slider stops after a successful stack push. */
  pushLanding: PushLanding;
  /** Periodically mark squares that become permanent neutral blocks. */
  spawnBlocks: boolean;
  /** Full rounds between new warnings (both players acted = 1 round). */
  spawnEveryRounds: number;
  /** Full rounds a marked square stays warned before becoming a block. */
  spawnWarningRounds: number;
  /** Blocks each player may place during play (-1 = infinite, 0 = off). */
  playerBlocks: number;
  /** When true, player 1 (first to place) opens play; otherwise player 2. */
  firstPlacerFirst: boolean;
};

export type TurnAction =
  | { kind: "slide"; move: Move }
  | { kind: "block"; at: number };

export type SpawnWarning = {
  i: number;
  /** Materialize when turnsFinished reaches this (1 round = 2 turns). */
  expiresAtTurn: number;
};

export type Move = {
  from: number;
  to: number;
  dir: Direction;
  knock: number | null;
  /** False when a knock was attempted but the defending stack held. */
  knocks: boolean;
};

export type EndReason =
  | "wipeout"
  | "one_each"
  | "mutual"
  | "stalemate"
  | "knockouts"
  | "reduceTo"
  | "forfeit";

export type GameState = Config & {
  phase: Phase;
  current: Player;
  cells: Cell[];
  /** Free/blind: draft indices per player. Formation: chosen half-board indices. */
  drafts: [number[], number[]];
  placed: number;
  actionsLeft: number;
  knockouts: [number, number];
  /** Pieces each player must still re-place after overlap. */
  replaceLeft: [number, number];
  colors: [string, string];
  names: [string, string];
  winner: Player | "draw" | null;
  endReason: EndReason | null;
  /** Set when a player’s turn is skipped for having no legal slides. */
  skipNotice: { player: Player; reason: "no_moves" } | null;
  /** Per-cell: where that piece started the current turn (current player only). */
  turnStartAt: (number | null)[];
  /** Board snapshot when a piece left its turn-start square (indexed by home cell). */
  leaveBoardAt: (Cell[] | null)[];
  /** Finished player-turns during play (used for fair alternating spawns). */
  turnsFinished: number;
  /** How many spawn marks have been issued. */
  spawnMarksDone: number;
  /** Squares marked to become neutral blocks. */
  warnings: SpawnWarning[];
  /** Blocks remaining to place (-1 slot = infinite). */
  blocksLeft: [number, number];
  /** Consecutive empty passes (end turn with no board change) per player — independent. */
  emptyPasses: [number, number];
  /** Whether the current player's turn has changed the board. */
  turnChangedBoard: boolean;
};

export function other(p: Player): Player {
  return p === 0 ? 1 : 0;
}

export function isBlock(cell: Cell | undefined): cell is BlockCell {
  return cell === "block" || cell === "block0" || cell === "block1";
}

export function playerBlock(p: Player): PlayerBlock {
  return p === 0 ? "block0" : "block1";
}

export function idx(n: number, r: number, c: number): number {
  return r * n + c;
}

export function rowOf(n: number, i: number): number {
  return Math.floor(i / n);
}

export function colOf(n: number, i: number): number {
  return i % n;
}

export function inBounds(n: number, r: number, c: number): boolean {
  return r >= 0 && c >= 0 && r < n && c < n;
}

/** 180° rotation — fair mirror for formations. */
export function mirrorIndex(n: number, i: number): number {
  return n * n - 1 - i;
}

/** Cells that are their own mirror (odd boards: the center). */
export function isMirrorFixed(n: number, i: number): boolean {
  return mirrorIndex(n, i) === i;
}

/** Prefer the "first" half of each mirrored pair for formation picking. */
export function isFormationHalf(n: number, i: number): boolean {
  return i <= mirrorIndex(n, i);
}

export function squareOwner(n: number, i: number): Player {
  return (rowOf(n, i) + colOf(n, i)) % 2 === 0 ? 0 : 1;
}

export function colorSquareCount(n: number, owner: Player): number {
  let count = 0;
  for (let i = 0; i < n * n; i++) {
    if (squareOwner(n, i) === owner) count++;
  }
  return count;
}

export function countPieces(cells: Cell[], player: Player): number {
  let count = 0;
  for (const cell of cells) {
    if (cell === player) count++;
  }
  return count;
}

export function actionsFor(
  cells: Cell[],
  player: Player,
  mode: ActionsMode = "fixed",
): number {
  const pieces = countPieces(cells, player);
  if (pieces === 0) return 0;
  if (mode === "perPiece") return Math.min(ACTIONS_CAP, pieces);
  return ACTIONS_CAP;
}

export function validateConfig(config: Config): string | null {
  if (!Number.isInteger(config.n) || config.n < 3 || config.n > 16) {
    return "Grid size must be a whole number from 3 to 16.";
  }
  if (!Number.isInteger(config.k) || config.k < 1) {
    return "Each player needs at least 1 piece.";
  }
  if (2 * config.k > config.n * config.n) {
    return "Not enough squares for both players.";
  }
  if (config.placeOnColor) {
    const dark = colorSquareCount(config.n, 0);
    const light = colorSquareCount(config.n, 1);
    if (config.k > dark || config.k > light) {
      return "Not enough squares of each color for that many pieces.";
    }
  }
  if (config.setupMode === "formation") {
    const half = Math.ceil((config.n * config.n) / 2);
    if (config.k > half) {
      return "Formation needs at most half the board (rounded up) for one side.";
    }
  }
  if (config.winMode === "knockouts" || config.winMode === "reduceTo") {
    if (!Number.isInteger(config.winTarget) || config.winTarget < 0) {
      return "Win target must be a whole number ≥ 0.";
    }
    if (config.winTarget >= config.k) {
      return "Win target must be less than pieces each.";
    }
    if (config.winMode === "knockouts" && config.winTarget < 1) {
      return "Knockouts needed must be at least 1.";
    }
  }
  if (config.spawnBlocks) {
    if (
      !Number.isInteger(config.spawnEveryRounds) ||
      config.spawnEveryRounds < 1
    ) {
      return "Spawn interval must be a whole number ≥ 1.";
    }
    if (
      !Number.isInteger(config.spawnWarningRounds) ||
      config.spawnWarningRounds < 0
    ) {
      return "Spawn warning duration must be a whole number ≥ 0.";
    }
  }
  if (
    !Number.isInteger(config.playerBlocks) ||
    config.playerBlocks < -1 ||
    config.playerBlocks > 99
  ) {
    return "Player blocks must be -1 (infinite), 0 (off), or 1–99.";
  }
  return null;
}

export function initBlocksLeft(playerBlocks: number): [number, number] {
  if (playerBlocks <= 0) return [0, 0];
  return [playerBlocks, playerBlocks];
}

export function blocksAvailable(
  state: GameState,
  player: Player = state.current,
): boolean {
  if (state.playerBlocks <= 0) return false;
  if (state.playerBlocks === -1) return true;
  return state.blocksLeft[player] > 0;
}

export function outcome(state: GameState): {
  winner: Player | "draw";
  endReason: EndReason;
} | null {
  const a = countPieces(state.cells, 0);
  const b = countPieces(state.cells, 1);
  if (a === 0 && b === 0) return { winner: "draw", endReason: "mutual" };
  if (a === 0) return { winner: 1, endReason: "wipeout" };
  if (b === 0) return { winner: 0, endReason: "wipeout" };

  if (state.winMode === "knockouts") {
    if (state.knockouts[0] >= state.winTarget) {
      return { winner: 0, endReason: "knockouts" };
    }
    if (state.knockouts[1] >= state.winTarget) {
      return { winner: 1, endReason: "knockouts" };
    }
  }

  if (state.winMode === "reduceTo") {
    if (b <= state.winTarget && a > state.winTarget) {
      return { winner: 0, endReason: "reduceTo" };
    }
    if (a <= state.winTarget && b > state.winTarget) {
      return { winner: 1, endReason: "reduceTo" };
    }
    if (a <= state.winTarget && b <= state.winTarget) {
      return { winner: "draw", endReason: "reduceTo" };
    }
  }

  if (state.drawOnOneEach && a === 1 && b === 1) {
    return { winner: "draw", endReason: "one_each" };
  }
  return null;
}

export function newGame(
  config: Config,
  colors: [string, string],
  names: [string, string],
): GameState {
  return {
    ...config,
    phase: "place",
    current: 0,
    cells: Array<Cell>(config.n * config.n).fill(null),
    drafts: [[], []],
    placed: 0,
    actionsLeft: 0,
    knockouts: [0, 0],
    replaceLeft: [0, 0],
    colors,
    names,
    winner: null,
    endReason: null,
    skipNotice: null,
    turnStartAt: [],
    leaveBoardAt: [],
    turnsFinished: 0,
    spawnMarksDone: 0,
    warnings: [],
    blocksLeft: initBlocksLeft(config.playerBlocks),
    emptyPasses: [0, 0],
    turnChangedBoard: false,
  };
}

/** Where each of the current player's pieces began this turn (indexed by cell). */
export function initTurnStartAt(cells: Cell[], player: Player): (number | null)[] {
  return cells.map((c, i) => (c === player ? i : null));
}

export function initLeaveBoardAt(length: number): (Cell[] | null)[] {
  return Array<Cell[] | null>(length).fill(null);
}

function boardsEqual(a: Cell[], b: Cell[]): boolean {
  if (a.length !== b.length) return false;
  for (let i = 0; i < a.length; i++) {
    if (a[i] !== b[i]) return false;
  }
  return true;
}

function transferTurnOrigin(
  turnStartAt: (number | null)[],
  from: number,
  to: number,
): void {
  turnStartAt[to] = turnStartAt[from] ?? null;
  turnStartAt[from] = null;
}

/** Block return-home when the board matches the snapshot from when the piece left.
 *  Knocking/pushing returns are allowed — they change the board. */
function wouldFlipFlop(
  state: GameState,
  from: number,
  to: number,
  knocks = false,
): boolean {
  if (knocks) return false;
  if (from === to) return false;
  const home = state.turnStartAt[from];
  if (home === null || to !== home) return false;
  const snap = state.leaveBoardAt[home];
  if (snap == null) return false;
  return boardsEqual(state.cells, snap);
}

function recordLeaveSnapshot(
  home: number | null,
  from: number,
  to: number,
  cells: Cell[],
  leaveBoardAt: (Cell[] | null)[],
): void {
  if (home !== null && from === home && to !== home) {
    leaveBoardAt[home] = cells.slice();
  }
}

function occupiedDuringPlace(state: GameState, i: number): boolean {
  if (state.setupMode === "formation") {
    return state.drafts[0].includes(i);
  }
  if (state.revealPlacement) {
    return state.cells[i] !== null;
  }
  // Blind: only block squares this player already picked.
  return state.drafts[state.current].includes(i);
}

export function canPlace(state: GameState, i: number): boolean {
  if (state.phase === "replace") {
    if (i < 0 || i >= state.cells.length) return false;
    if (state.cells[i] !== null) return false;
    if (state.replaceLeft[state.current] <= 0) return false;
    if (state.placeOnColor && squareOwner(state.n, i) !== state.current) return false;
    return true;
  }
  if (state.phase !== "place") return false;
  if (i < 0 || i >= state.cells.length) return false;
  if (occupiedDuringPlace(state, i)) return false;

  if (state.setupMode === "formation") {
    if (!isFormationHalf(state.n, i)) return false;
    return true;
  }

  if (state.placeOnColor && squareOwner(state.n, i) !== state.current) return false;
  return true;
}

export function legalPlaceIndices(state: GameState): number[] {
  const spots: number[] = [];
  if (state.phase !== "place" && state.phase !== "replace") return spots;
  for (let i = 0; i < state.n * state.n; i++) {
    if (canPlace(state, i)) spots.push(i);
  }
  return spots;
}

/** Squares that can receive a new spawn warning. */
export function spawnCandidates(state: GameState): number[] {
  const warned = new Set(state.warnings.map((w) => w.i));
  const out: number[] = [];
  for (let i = 0; i < state.cells.length; i++) {
    if (isBlock(state.cells[i])) continue;
    if (warned.has(i)) continue;
    out.push(i);
  }
  return out;
}

/** Full rounds completed (both players acted). */
export function roundsCompleted(state: GameState): number {
  return Math.floor(state.turnsFinished / 2);
}

/**
 * Turn index (1-based) when the next spawn mark should fire.
 * Alternates which player just finished: after P0, then P1, then P0, ...
 * when player 1 opens the game (turns 1=P1, 2=P0, 3=P1, 4=P0, ...).
 */
export function nextMarkTurn(
  marksDone: number,
  everyRounds: number,
): number {
  return (marksDone + 1) * everyRounds * 2 - (marksDone % 2);
}

/** Rounds of warning left (for HUD), ceil of remaining half-rounds. */
export function warningRoundsLeft(
  w: SpawnWarning,
  turnsFinished: number,
): number {
  return Math.max(0, Math.ceil((w.expiresAtTurn - turnsFinished) / 2));
}

function materializeAt(
  cells: Cell[],
  turnStartAt: (number | null)[],
  i: number,
): void {
  const occ = cells[i];
  if (occ === 0 || occ === 1) {
    cells[i] = null;
    turnStartAt[i] = null;
  }
  if (!isBlock(cells[i])) {
    cells[i] = "block";
  }
}

/**
 * After a player finishes their turn: maybe materialize warnings and/or mark.
 * Mark turns alternate so neither side always reacts first.
 * Eliminations from materialize grant no knockouts.
 */
export function tickAfterTurn(
  state: GameState,
  rng: () => number = Math.random,
): GameState {
  const turnsFinished = state.turnsFinished + 1;

  if (state.phase !== "play" || !state.spawnBlocks) {
    return { ...state, turnsFinished };
  }

  const cells = state.cells.slice();
  const turnStartAt = state.turnStartAt.slice();
  let spawnMarksDone = state.spawnMarksDone;
  let boardChanged = false;

  const warnings: SpawnWarning[] = [];
  for (const w of state.warnings) {
    if (turnsFinished >= w.expiresAtTurn) {
      materializeAt(cells, turnStartAt, w.i);
      boardChanged = true;
    } else {
      warnings.push(w);
    }
  }

  if (turnsFinished === nextMarkTurn(spawnMarksDone, state.spawnEveryRounds)) {
    const warned = new Set(warnings.map((w) => w.i));
    const candidates: number[] = [];
    for (let i = 0; i < cells.length; i++) {
      if (isBlock(cells[i])) continue;
      if (warned.has(i)) continue;
      candidates.push(i);
    }
    if (candidates.length > 0) {
      const pick = candidates[Math.floor(rng() * candidates.length)]!;
      if (state.spawnWarningRounds <= 0) {
        materializeAt(cells, turnStartAt, pick);
        boardChanged = true;
      } else {
        warnings.push({
          i: pick,
          expiresAtTurn: turnsFinished + state.spawnWarningRounds * 2,
        });
      }
      spawnMarksDone += 1;
    }
  }

  const mid: GameState = {
    ...state,
    cells,
    turnStartAt,
    warnings,
    turnsFinished,
    spawnMarksDone,
  };

  if (!boardChanged) return mid;

  const result = outcome(mid);
  if (result !== null) {
    return {
      ...mid,
      phase: "over",
      actionsLeft: 0,
      winner: result.winner,
      endReason: result.endReason,
      skipNotice: null,
    };
  }
  return mid;
}

/** @deprecated alias — one round = two turn ticks. Prefer tickAfterTurn. */
export function tickRound(
  state: GameState,
  rng: () => number = Math.random,
): GameState {
  return tickAfterTurn(tickAfterTurn(state, rng), rng);
}

/** Count a finished turn and advance spawn/hazard clock. */
function resolveRoundProgress(
  state: GameState,
  rng?: () => number,
): GameState {
  return tickAfterTurn(state, rng);
}

function beginTurn(state: GameState, player: Player): GameState {
  const next: GameState = {
    ...state,
    current: player,
    actionsLeft: actionsFor(state.cells, player, state.actionsMode),
    turnStartAt: initTurnStartAt(state.cells, player),
    leaveBoardAt: initLeaveBoardAt(state.cells.length),
    turnChangedBoard: false,
    winner: null,
    endReason: null,
    skipNotice: null,
  };
  if (hasLegalActions(next)) return next;

  // Skip — this player's turn slot is consumed.
  let afterSkip = resolveRoundProgress({
    ...next,
    skipNotice: { player, reason: "no_moves" },
  });
  if (afterSkip.phase === "over") return afterSkip;

  const otherPlayer = other(player);
  const skipped: GameState = {
    ...afterSkip,
    current: otherPlayer,
    actionsLeft: actionsFor(afterSkip.cells, otherPlayer, afterSkip.actionsMode),
    turnStartAt: initTurnStartAt(afterSkip.cells, otherPlayer),
    leaveBoardAt: initLeaveBoardAt(afterSkip.cells.length),
    turnChangedBoard: false,
    skipNotice: { player, reason: "no_moves" },
  };
  if (hasLegalActions(skipped)) return skipped;

  return {
    ...afterSkip,
    phase: "over",
    ...stalemateOutcome(afterSkip.cells),
    actionsLeft: 0,
    skipNotice: null,
  };
}

/** When nobody can slide, most pieces win (tie → draw). */
export function stalemateOutcome(cells: Cell[]): {
  winner: Player | "draw";
  endReason: "stalemate";
} {
  const a = countPieces(cells, 0);
  const b = countPieces(cells, 1);
  if (a === b) return { winner: "draw", endReason: "stalemate" };
  return { winner: a > b ? 0 : 1, endReason: "stalemate" };
}

function enterPlay(state: GameState): GameState {
  const result = outcome(state);
  if (result !== null) {
    return {
      ...state,
      phase: "over",
      current: 0,
      placed: 0,
      actionsLeft: 0,
      winner: result.winner,
      endReason: result.endReason,
    };
  }
  return beginTurn(
    {
      ...state,
      phase: "play",
      placed: 0,
      winner: null,
      endReason: null,
      blocksLeft: initBlocksLeft(state.playerBlocks),
    },
    state.firstPlacerFirst ? 0 : 1,
  );
}

function materializeDrafts(
  n: number,
  d0: number[],
  d1: number[],
  overlapMode: OverlapMode,
): { cells: Cell[]; replaceLeft: [number, number] } {
  const set0 = new Set(d0);
  const set1 = new Set(d1);
  const cells: Cell[] = Array(n * n).fill(null);
  let overlaps = 0;

  for (let i = 0; i < n * n; i++) {
    const a = set0.has(i);
    const b = set1.has(i);
    if (a && b) {
      overlaps++;
      if (overlapMode === "block") cells[i] = "block";
      // eliminate / replace: leave empty for now
    } else if (a) cells[i] = 0;
    else if (b) cells[i] = 1;
  }

  const replaceLeft: [number, number] =
    overlapMode === "replace" ? [overlaps, overlaps] : [0, 0];
  return { cells, replaceLeft };
}

function finishFreePlacement(state: GameState): GameState {
  const { cells, replaceLeft } = materializeDrafts(
    state.n,
    state.drafts[0],
    state.drafts[1],
    state.overlapMode,
  );
  const next: GameState = { ...state, cells, replaceLeft, placed: 0 };
  if (replaceLeft[0] > 0 || replaceLeft[1] > 0) {
    const first: Player = replaceLeft[0] > 0 ? 0 : 1;
    return { ...next, phase: "replace", current: first };
  }
  return enterPlay(next);
}

function finishFormation(state: GameState, half: number[]): GameState {
  const d0: number[] = [];
  const d1: number[] = [];
  for (const i of half) {
    d0.push(i);
    d1.push(mirrorIndex(state.n, i));
  }
  const { cells, replaceLeft } = materializeDrafts(
    state.n,
    d0,
    d1,
    state.overlapMode,
  );
  const next: GameState = {
    ...state,
    drafts: [d0, d1],
    cells,
    replaceLeft,
    placed: 0,
  };
  if (replaceLeft[0] > 0 || replaceLeft[1] > 0) {
    const first: Player = replaceLeft[0] > 0 ? 0 : 1;
    return { ...next, phase: "replace", current: first };
  }
  return enterPlay(next);
}

/** Built-in formations on the formation half, then mirrored. */
export function presetFormation(n: number, k: number): number[] {
  const half = Array.from({ length: n * n }, (_, i) => i).filter((i) =>
    isFormationHalf(n, i),
  );
  // Prefer back ranks (top rows), then edges — deterministic “game-picked” look.
  half.sort((a, b) => {
    const ra = rowOf(n, a);
    const rb = rowOf(n, b);
    if (ra !== rb) return ra - rb;
    return colOf(n, a) - colOf(n, b);
  });
  const picks = half.slice(0, k);
  if (picks.length < k) {
    // fallback: first k half cells
    return half.slice(0, k);
  }
  return picks;
}

export function applyPresetFormation(state: GameState): GameState {
  if (state.setupMode !== "formation" || state.formationSource !== "preset") {
    return state;
  }
  return finishFormation(state, presetFormation(state.n, state.k));
}

export function canUnplace(state: GameState, i: number): boolean {
  if (state.phase !== "place") return false;
  if (i < 0 || i >= state.cells.length) return false;
  if (state.setupMode === "formation") {
    return state.drafts[0].includes(i);
  }
  return state.drafts[state.current].includes(i);
}

/** Remove a draft placement so the player can rethink before finishing. */
export function unplacePiece(state: GameState, i: number): GameState {
  if (!canUnplace(state, i)) return state;

  if (state.setupMode === "formation") {
    const draft = state.drafts[0].filter((d) => d !== i);
    return {
      ...state,
      drafts: [draft, state.drafts[1]],
      placed: draft.length,
      cells: previewFormation(state.n, draft),
    };
  }

  const mine = state.drafts[state.current].filter((d) => d !== i);
  const drafts: [number[], number[]] =
    state.current === 0 ? [mine, state.drafts[1]] : [state.drafts[0], mine];

  const cells = state.cells.slice();
  if (state.revealPlacement) {
    cells[i] = null;
  } else {
    for (let c = 0; c < cells.length; c++) cells[c] = null;
    for (const d of mine) cells[d] = state.current;
  }

  return { ...state, drafts, cells, placed: mine.length };
}

export function placePiece(state: GameState, i: number): GameState {
  if (state.phase === "replace") {
    if (!canPlace(state, i)) return state;
    const cells = state.cells.slice();
    cells[i] = state.current;
    const replaceLeft: [number, number] = [...state.replaceLeft];
    replaceLeft[state.current] -= 1;
    let next: GameState = { ...state, cells, replaceLeft };
    if (replaceLeft[0] <= 0 && replaceLeft[1] <= 0) {
      return enterPlay(next);
    }
    const nxt: Player =
      replaceLeft[other(state.current)] > 0 ? other(state.current) : state.current;
    return { ...next, current: nxt };
  }

  if (!canPlace(state, i)) return state;

  if (state.setupMode === "formation") {
    const draft = state.drafts[0].concat(i);
    const placed = draft.length;
    const next: GameState = {
      ...state,
      drafts: [draft, state.drafts[1]],
      placed,
      // Live board preview: own side + mirrored ghosts.
      cells: previewFormation(state.n, draft),
    };
    if (placed < state.k) return next;
    return finishFormation(next, draft);
  }

  // Free placement (revealed or blind) via drafts.
  const mine = state.drafts[state.current].concat(i);
  const drafts: [number[], number[]] =
    state.current === 0 ? [mine, state.drafts[1]] : [state.drafts[0], mine];
  const cells = state.cells.slice();
  if (state.revealPlacement) {
    cells[i] = state.current;
  } else {
    // Blind: only show the active player's drafts on the board.
    for (let c = 0; c < cells.length; c++) cells[c] = null;
    for (const d of mine) cells[d] = state.current;
  }

  const placed = mine.length;
  if (placed < state.k) {
    return { ...state, drafts, cells, placed };
  }

  if (state.current === 0) {
    // Hand off to P2. If blind, clear board to only show P2 as they place.
    const handoffCells = state.revealPlacement
      ? cells
      : Array<Cell>(state.n * state.n).fill(null);
    return {
      ...state,
      drafts,
      cells: handoffCells,
      placed: 0,
      current: 1,
    };
  }

  return finishFreePlacement({ ...state, drafts, cells, placed: 0 });
}

function previewFormation(n: number, half: number[]): Cell[] {
  const cells: Cell[] = Array(n * n).fill(null);
  for (const i of half) {
    const m = mirrorIndex(n, i);
    if (i === m) {
      cells[i] = "block"; // fixed point preview as contested
    } else {
      cells[i] = 0;
      cells[m] = 1;
    }
  }
  return cells;
}

type RayBlocker =
  | "edge"
  | "block"
  | { i: number; player: Player };

type Ray = {
  empties: number[];
  blocker: RayBlocker;
};

  /** True if applying the move would alter piece/block placement. */
function moveWouldChangeBoard(state: GameState, move: Move): boolean {
  const cells = state.cells.slice();
  const piece = cells[move.from];
  if (piece !== 0 && piece !== 1) return false;

  if (move.knock !== null && !move.knocks) {
    if (move.from !== move.to) {
      cells[move.to] = piece;
      cells[move.from] = null;
    }
    return !boardsEqual(cells, state.cells);
  }

  if (move.knock !== null && move.knocks) {
    applyKnock(cells, state.n, move.knock, move.dir);
    const friend = friendlyBeforeVictim(
      state.cells,
      state.n,
      move.from,
      move.dir,
      move.knock,
      state.current,
    );
    if (friend !== null && cells[friend] === state.current) {
      shoveStackOne(cells, state.n, friend, move.dir);
    }
  }
  if (move.from !== move.to) {
    cells[move.to] = piece;
    cells[move.from] = null;
  }
  return !boardsEqual(cells, state.cells);
}

function inspectRay(n: number, cells: Cell[], from: number, dir: Direction): Ray {
  const [dr, dc] = DIR_DELTA[dir];
  let r = rowOf(n, from) + dr;
  let c = colOf(n, from) + dc;
  const empties: number[] = [];
  while (inBounds(n, r, c)) {
    const i = idx(n, r, c);
    const cell = cells[i] ?? null;
    if (cell === null) {
      empties.push(i);
      r += dr;
      c += dc;
      continue;
    }
    if (isBlock(cell)) {
      return { empties, blocker: "block" };
    }
    return { empties, blocker: { i, player: cell } };
  }
  return { empties, blocker: "edge" };
}

export function legalMoves(state: GameState): Move[] {
  if (state.phase !== "play") return [];
  const moves: Move[] = [];
  const { n, cells, current } = state;

  for (let from = 0; from < cells.length; from++) {
    if (cells[from] !== current) continue;
    for (const dir of DIRECTIONS) {
      const { empties, blocker } = inspectRay(n, cells, from, dir);
      if (blocker === "edge") continue;

      if (blocker === "block") {
        if (empties.length > 0) {
          const to = empties[empties.length - 1]!;
          if (wouldFlipFlop(state, from, to)) continue;
          moves.push({
            from,
            to,
            dir,
            knock: null,
            knocks: false,
          });
        }
        continue;
      }

      const enemy = blocker.player !== current ? blocker.i : null;
      const last = empties.length > 0 ? empties[empties.length - 1]! : from;
      const momentum = empties.length > 0;

      if (enemy === null) {
        // Own piece ahead: with Push + momentum, may ram friendlies into an enemy beyond.
        if (state.push && momentum) {
          const friend = blocker.i;
          const beyond = enemyBeyondFriends(cells, n, friend, dir, current);
          if (
            beyond !== null &&
            canKnock(state, from, dir, beyond, true)
          ) {
            const to =
              state.pushLanding === "follow" ? friend : last;
            moves.push({
              from,
              to,
              dir,
              knock: beyond,
              knocks: true,
            });
            continue;
          }
        }
        // Otherwise only legal if we actually slide somewhere (no flip-flop).
        if (empties.length > 0 && !wouldFlipFlop(state, from, last)) {
          moves.push({ from, to: last, dir, knock: null, knocks: false });
        }
        continue;
      }

      const knocks = canKnock(state, from, dir, enemy, momentum);

      if (knocks) {
        const defend = defendStrength(cells, n, enemy, dir, blocker.player);
        const to =
          defend >= 2 && state.push && state.pushLanding === "follow"
            ? enemy
            : last;
        moves.push({
          from,
          to,
          dir,
          knock: enemy,
          knocks: true,
        });
        continue;
      }

      // Failed knock into braced stack: legal if the slider actually travels.
      if (empties.length > 0 && !wouldFlipFlop(state, from, last)) {
        moves.push({
          from,
          to: last,
          dir,
          knock: enemy,
          knocks: false,
        });
        continue;
      }

      // Adjacent braced contact — board unchanged. Only legal if no-op slides allowed.
      if (!state.requireBoardChange) {
        moves.push({
          from,
          to: from,
          dir,
          knock: enemy,
          knocks: false,
        });
      }
    }
  }
  if (!state.requireBoardChange) return moves;
  return moves.filter((m) => moveWouldChangeBoard(state, m));
}

/** Empty squares where the current player may place a block. */
export function legalBlockPlacements(state: GameState): number[] {
  if (state.phase !== "play" || !blocksAvailable(state)) return [];
  const out: number[] = [];
  for (let i = 0; i < state.cells.length; i++) {
    if (state.cells[i] === null) out.push(i);
  }
  return out;
}

export function hasLegalActions(state: GameState): boolean {
  return legalMoves(state).length > 0 || legalBlockPlacements(state).length > 0;
}

export function legalTurnActions(state: GameState): TurnAction[] {
  const actions: TurnAction[] = legalMoves(state).map((move) => ({
    kind: "slide",
    move,
  }));
  for (const at of legalBlockPlacements(state)) {
    actions.push({ kind: "block", at });
  }
  return actions;
}

function finishPlayAction(state: GameState, mid: GameState): GameState {
  const result = outcome(mid);
  if (result !== null) {
    return {
      ...mid,
      phase: "over",
      actionsLeft: 0,
      winner: result.winner,
      endReason: result.endReason,
    };
  }
  if (mid.actionsLeft > 0 && hasLegalActions(mid)) {
    return mid;
  }
  // Natural turn end (actions spent or no moves left) — not an empty pass.
  return beginTurn(resolveRoundProgress(mid), other(state.current));
}

/** True when End turn would forfeit (second empty pass in a row). */
export function endTurnIsForfeit(state: GameState): boolean {
  return (
    state.phase === "play" &&
    !state.turnChangedBoard &&
    state.emptyPasses[state.current] >= 1
  );
}

/**
 * Voluntarily end the current player's turn.
 * Each player has their own empty-pass count. One empty pass (no board change)
 * is allowed per player; a second consecutive empty pass by that same player
 * is a forfeit. The other player's count is unaffected. Any board change during
 * a player's turn resets only that player's empty-pass count.
 * Ending after the board already changed does not increment.
 */
export function endTurn(state: GameState): GameState {
  if (state.phase !== "play") return state;

  if (!state.turnChangedBoard && state.emptyPasses[state.current] >= 1) {
    return {
      ...state,
      phase: "over",
      actionsLeft: 0,
      winner: other(state.current),
      endReason: "forfeit",
    };
  }

  const emptyPasses: [number, number] = [...state.emptyPasses];
  if (!state.turnChangedBoard) {
    // First empty pass — mark it. (Board-changing turns leave the count at 0.)
    emptyPasses[state.current] = 1;
  }

  const mid = { ...state, actionsLeft: 0, emptyPasses };
  const result = outcome(mid);
  if (result !== null) {
    return {
      ...mid,
      phase: "over",
      winner: result.winner,
      endReason: result.endReason,
    };
  }
  return beginTurn(resolveRoundProgress(mid), other(state.current));
}

function withBoardChange(before: GameState, mid: GameState): GameState {
  if (boardsEqual(before.cells, mid.cells)) return mid;
  const emptyPasses: [number, number] = [...before.emptyPasses];
  emptyPasses[before.current] = 0;
  return {
    ...mid,
    emptyPasses,
    turnChangedBoard: true,
  };
}

function opposite(dir: Direction): Direction {
  if (dir === "up") return "down";
  if (dir === "down") return "up";
  if (dir === "left") return "right";
  return "left";
}

/** Contiguous pieces of `player` starting at `start`, stepping in `dir`. */
export function stackLength(
  cells: Cell[],
  n: number,
  start: number,
  dir: Direction,
  player: Player,
): number {
  const [dr, dc] = DIR_DELTA[dir];
  let count = 0;
  let r = rowOf(n, start);
  let c = colOf(n, start);
  while (inBounds(n, r, c)) {
    const i = idx(n, r, c);
    if (cells[i] !== player) break;
    count++;
    r += dr;
    c += dc;
  }
  return count;
}

export function attackForce(
  state: GameState,
  from: number,
  dir: Direction,
): number {
  if (state.knockStrength === "simple") return 1;
  // Attacker + contiguous friendlies directly behind (opposite travel).
  return stackLength(state.cells, state.n, from, opposite(dir), state.current);
}

export function defendStrength(
  cells: Cell[],
  n: number,
  victim: number,
  dir: Direction,
  defender: Player,
): number {
  return stackLength(cells, n, victim, dir, defender);
}

/** True if a successful knock would move or eliminate the victim. */
function knockWouldDisplace(
  cells: Cell[],
  n: number,
  victim: number,
  dir: Direction,
): boolean {
  const sim = cells.slice();
  applyKnock(sim, n, victim, dir);
  return !boardsEqual(sim, cells);
}

export function canKnock(
  state: GameState,
  _from: number,
  dir: Direction,
  victim: number,
  momentum = true,
): boolean {
  const occ = state.cells[victim];
  if (occ !== 0 && occ !== 1) return false;
  if (occ === state.current) return false;
  const defend = defendStrength(state.cells, state.n, victim, dir, occ);

  // Lone defenders: always knockable if they can be displaced (no run-up required).
  if (defend <= 1) {
    return knockWouldDisplace(state.cells, state.n, victim, dir);
  }

  // Stacks of 2+: only Push with momentum can shove them.
  if (!state.push || !momentum) return false;
  return knockWouldDisplace(state.cells, state.n, victim, dir);
}

/** Contiguous same-player cells from victim along knock direction. */
function knockStack(
  cells: Cell[],
  n: number,
  victim: number,
  dir: Direction,
): number[] {
  const who = cells[victim];
  if (who !== 0 && who !== 1) return [];
  const [dr, dc] = DIR_DELTA[dir];
  const stack: number[] = [];
  let r = rowOf(n, victim);
  let c = colOf(n, victim);
  while (inBounds(n, r, c)) {
    const i = idx(n, r, c);
    if (cells[i] !== who) break;
    stack.push(i);
    r += dr;
    c += dc;
  }
  return stack;
}

/** Shift a same-color stack one step in `dir`. Returns whether someone left the board. */
function shoveStackOne(
  cells: Cell[],
  n: number,
  start: number,
  dir: Direction,
  turnStartAt?: (number | null)[] | null,
): boolean {
  const who = cells[start];
  if (who !== 0 && who !== 1) return false;
  const stack = knockStack(cells, n, start, dir);
  if (stack.length === 0) return false;

  const [dr, dc] = DIR_DELTA[dir];
  const rear = stack[stack.length - 1]!;
  const rr = rowOf(n, rear) + dr;
  const cc = colOf(n, rear) + dc;

  let eliminated = false;
  if (!inBounds(n, rr, cc)) {
    cells[rear] = null;
    if (turnStartAt) turnStartAt[rear] = null;
    eliminated = true;
  } else {
    const nextIdx = idx(n, rr, cc);
    if (cells[nextIdx] !== null) return false;
    cells[nextIdx] = who;
    cells[rear] = null;
    if (turnStartAt) transferTurnOrigin(turnStartAt, rear, nextIdx);
  }

  for (let s = stack.length - 2; s >= 0; s--) {
    const from = stack[s]!;
    const to = stack[s + 1]!;
    cells[to] = who;
    cells[from] = null;
    if (turnStartAt) transferTurnOrigin(turnStartAt, from, to);
  }

  return eliminated;
}

/** First friendly on the ray from `from` toward `victim` (exclusive). */
function friendlyBeforeVictim(
  cells: Cell[],
  n: number,
  from: number,
  dir: Direction,
  victim: number,
  player: Player,
): number | null {
  const [dr, dc] = DIR_DELTA[dir];
  let r = rowOf(n, from) + dr;
  let c = colOf(n, from) + dc;
  while (inBounds(n, r, c)) {
    const i = idx(n, r, c);
    if (i === victim) return null;
    const cell = cells[i] ?? null;
    if (cell === player) return i;
    if (cell !== null) return null;
    r += dr;
    c += dc;
  }
  return null;
}

/** Enemy immediately beyond a contiguous friendly run starting at `friend`. */
function enemyBeyondFriends(
  cells: Cell[],
  n: number,
  friend: number,
  dir: Direction,
  current: Player,
): number | null {
  const stack = knockStack(cells, n, friend, dir);
  if (stack.length === 0) return null;
  const [dr, dc] = DIR_DELTA[dir];
  const rear = stack[stack.length - 1]!;
  const rr = rowOf(n, rear) + dr;
  const cc = colOf(n, rear) + dc;
  if (!inBounds(n, rr, cc)) return null;
  const beyond = idx(n, rr, cc);
  const occ = cells[beyond];
  if (occ !== 0 && occ !== 1) return null;
  if (occ === current) return null;
  return beyond;
}

/** Returns true if the victim left the board. */
export function applyKnock(
  cells: Cell[],
  n: number,
  victim: number,
  dir: Direction,
  turnStartAt?: (number | null)[] | null,
): boolean {
  const who = cells[victim];
  if (who !== 0 && who !== 1) return false;

  const stack = knockStack(cells, n, victim, dir);
  if (stack.length <= 1) {
    const [dr, dc] = DIR_DELTA[dir];
    let pos = victim;
    for (;;) {
      const r = rowOf(n, pos) + dr;
      const c = colOf(n, pos) + dc;
      if (!inBounds(n, r, c)) {
        cells[pos] = null;
        if (turnStartAt) turnStartAt[pos] = null;
        return true;
      }
      const next = idx(n, r, c);
      const occ = cells[next] ?? null;
      if (occ === null) {
        cells[next] = who;
        cells[pos] = null;
        if (turnStartAt) transferTurnOrigin(turnStartAt, pos, next);
        pos = next;
        continue;
      }
      // Block, piece, or other obstruction — stop before it.
      return false;
    }
  }

  return shoveStackOne(cells, n, victim, dir, turnStartAt);
}

export function knockTravel(
  cells: Cell[],
  n: number,
  victim: number,
  dir: Direction,
): { path: number[]; eliminated: boolean } {
  const who = cells[victim];
  if (who !== 0 && who !== 1) return { path: [], eliminated: false };

  const stack = knockStack(cells, n, victim, dir);
  if (stack.length <= 1) {
    const [dr, dc] = DIR_DELTA[dir];
    const path: number[] = [];
    let pos = victim;
    for (;;) {
      const r = rowOf(n, pos) + dr;
      const c = colOf(n, pos) + dc;
      if (!inBounds(n, r, c)) {
        return { path, eliminated: true };
      }
      const next = idx(n, r, c);
      if (cells[next] === null) {
        path.push(next);
        pos = next;
        continue;
      }
      return { path, eliminated: false };
    }
  }

  const [dr, dc] = DIR_DELTA[dir];
  const rear = stack[stack.length - 1]!;
  const rr = rowOf(n, rear) + dr;
  const cc = colOf(n, rear) + dc;

  if (!inBounds(n, rr, cc)) {
    return { path: [stack[1]!], eliminated: false };
  }

  const nextIdx = idx(n, rr, cc);
  if (cells[nextIdx] !== null) return { path: [], eliminated: false };
  return { path: [stack[1]!], eliminated: false };
}

export function applyMove(state: GameState, move: Move): GameState {
  if (state.phase !== "play") return state;
  if (state.actionsLeft <= 0) return state;
  const legal = legalMoves(state).some(
    (m) =>
      m.from === move.from &&
      m.to === move.to &&
      m.dir === move.dir &&
      m.knock === move.knock &&
      m.knocks === move.knocks,
  );
  if (!legal) return state;

  // Failed knock: slide if we traveled, otherwise board unchanged.
  if (move.knock !== null && !move.knocks) {
    const cells = state.cells.slice();
    const turnStartAt = state.turnStartAt.slice();
    const leaveBoardAt = state.leaveBoardAt.slice();
    const home = state.turnStartAt[move.from] ?? null;
    if (move.from !== move.to) {
      const piece = cells[move.from];
      if (piece === 0 || piece === 1) {
        cells[move.to] = piece;
        cells[move.from] = null;
        transferTurnOrigin(turnStartAt, move.from, move.to);
      }
    }
    recordLeaveSnapshot(home, move.from, move.to, cells, leaveBoardAt);
    const actionsLeft = state.actionsLeft - 1;
    return finishPlayAction(
      state,
      withBoardChange(state, {
        ...state,
        cells,
        turnStartAt,
        leaveBoardAt,
        actionsLeft,
        winner: null,
        endReason: null,
      }),
    );
  }

  const cells = state.cells.slice();
  const turnStartAt = state.turnStartAt.slice();
  const leaveBoardAt = state.leaveBoardAt.slice();
  const home = state.turnStartAt[move.from] ?? null;
  const piece = cells[move.from];
  if (piece !== 0 && piece !== 1) return state;

  const knockouts: [number, number] = [...state.knockouts];
  // Knock/push first so follow-landing can occupy the vacated front square.
  if (move.knock !== null && move.knocks) {
    const friend = friendlyBeforeVictim(
      cells,
      state.n,
      move.from,
      move.dir,
      move.knock,
      state.current,
    );
    const eliminated = applyKnock(
      cells,
      state.n,
      move.knock,
      move.dir,
      turnStartAt,
    );
    if (eliminated) knockouts[state.current] += 1;
    // Ram: shove intervening friendlies one step into the space the knock opened.
    if (friend !== null && cells[friend] === state.current) {
      const friendGone = shoveStackOne(
        cells,
        state.n,
        friend,
        move.dir,
        turnStartAt,
      );
      if (friendGone) {
        // Own piece shoved off the edge — no KO credit for yourself.
      }
    }
  }

  if (move.from !== move.to) {
    if (cells[move.to] !== null) return state;
    cells[move.to] = piece;
    cells[move.from] = null;
    transferTurnOrigin(turnStartAt, move.from, move.to);
  }

  recordLeaveSnapshot(home, move.from, move.to, cells, leaveBoardAt);

  const actionsLeft = state.actionsLeft - 1;
  return finishPlayAction(
    state,
    withBoardChange(state, {
      ...state,
      cells,
      turnStartAt,
      leaveBoardAt,
      knockouts,
      actionsLeft,
      winner: null,
      endReason: null,
    }),
  );
}

export function applyBlockPlacement(state: GameState, at: number): GameState {
  if (state.phase !== "play") return state;
  if (state.actionsLeft <= 0) return state;
  if (!legalBlockPlacements(state).includes(at)) return state;

  const cells = state.cells.slice();
  cells[at] = playerBlock(state.current);

  const warnings = state.warnings.filter((w) => w.i !== at);
  const blocksLeft: [number, number] = [...state.blocksLeft];
  if (state.playerBlocks !== -1) {
    blocksLeft[state.current] -= 1;
  }

  const actionsLeft = state.actionsLeft - 1;
  return finishPlayAction(
    state,
    withBoardChange(state, {
      ...state,
      cells,
      warnings,
      blocksLeft,
      actionsLeft,
      winner: null,
      endReason: null,
    }),
  );
}

export function moveForClick(
  moves: Move[],
  selected: number,
  clicked: number,
): Move | undefined {
  const knock = moves.find(
    (m) => m.from === selected && m.knock === clicked && m.knocks,
  );
  if (knock) return knock;
  const failed = moves.find(
    (m) => m.from === selected && m.knock === clicked && !m.knocks,
  );
  if (failed) return failed;
  const slideIntoBraced = moves.find(
    (m) =>
      m.from === selected &&
      m.to === clicked &&
      m.knock !== null &&
      !m.knocks,
  );
  if (slideIntoBraced) return slideIntoBraced;
  const quiet = moves.find(
    (m) => m.from === selected && m.to === clicked && m.knock === null,
  );
  if (quiet) return quiet;
  return moves.find((m) => m.from === selected && m.to === clicked);
}

export function moveForKey(
  state: GameState,
  selected: number,
  dir: Direction,
): Move | undefined {
  return legalMoves(state).find((m) => m.from === selected && m.dir === dir);
}

/** Visible board for rendering during blind placement. */
export function visibleCells(state: GameState): Cell[] {
  if (state.phase !== "place" || state.setupMode === "formation") {
    return state.cells;
  }
  if (state.revealPlacement) return state.cells;
  const cells: Cell[] = Array(state.n * state.n).fill(null);
  for (const i of state.drafts[state.current]) {
    cells[i] = state.current;
  }
  return cells;
}
