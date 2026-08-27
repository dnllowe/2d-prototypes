import {
  DIR_DELTA,
  DIRECTIONS,
  applyBlockPlacement,
  applyMove,
  colOf,
  countPieces,
  defendStrength,
  inBounds,
  idx,
  initLeaveBoardAt,
  initTurnStartAt,
  isBlock,
  legalBlockPlacements,
  legalMoves,
  legalPlaceIndices,
  legalTurnActions,
  other,
  rowOf,
  type AiStyle,
  type GameState,
  type Move,
  type Player,
  type TurnAction,
} from "./game";

type Weights = {
  knock: number;
  approach: number;
  edge: number;
  block: number;
  hazard: number;
  mobility: number;
  material: number;
};

const THOUGHTFUL: Weights = {
  knock: 40,
  approach: 8,
  edge: 25,
  block: 12,
  hazard: 30,
  mobility: 3,
  material: 200,
};

const AGGRESSIVE: Weights = {
  knock: 90,
  approach: 22,
  edge: 8,
  block: 4,
  hazard: 5,
  mobility: 2,
  material: 200,
};

const PATIENT: Weights = {
  knock: 18,
  approach: 3,
  edge: 42,
  block: 26,
  hazard: 45,
  mobility: 4,
  material: 220,
};

function weightsFor(style: AiStyle): Weights {
  if (style === "aggressive") return AGGRESSIVE;
  if (style === "patient") return PATIENT;
  return THOUGHTFUL;
}

function applyAction(state: GameState, action: TurnAction): GameState {
  return action.kind === "slide"
    ? applyMove(state, action.move)
    : applyBlockPlacement(state, action.at);
}

function stillOurTurn(before: GameState, after: GameState): boolean {
  return (
    after.phase === "play" &&
    after.current === before.current &&
    after.actionsLeft > 0
  );
}

function manhattan(n: number, a: number, b: number): number {
  return Math.abs(rowOf(n, a) - rowOf(n, b)) + Math.abs(colOf(n, a) - colOf(n, b));
}

function openEdgeDirs(state: GameState, from: number): number {
  const { n, cells } = state;
  let count = 0;
  for (const dir of DIRECTIONS) {
    const [dr, dc] = DIR_DELTA[dir];
    let r = rowOf(n, from) + dr;
    let c = colOf(n, from) + dc;
    let hit = false;
    while (inBounds(n, r, c)) {
      const cell = cells[idx(n, r, c)];
      if (cell !== null) {
        hit = true;
        break;
      }
      r += dr;
      c += dc;
    }
    if (!hit) count++;
  }
  return count;
}

function mobilityFor(state: GameState, player: Player): number {
  if (state.phase !== "play") return 0;
  const probe: GameState = {
    ...state,
    current: player,
    actionsLeft: Math.max(1, state.actionsLeft),
    turnStartAt: initTurnStartAt(state.cells, player),
    leaveBoardAt: initLeaveBoardAt(state.cells.length),
  };
  return legalMoves(probe).length;
}

function evaluate(state: GameState, ai: Player, w: Weights): number {
  const opp = other(ai);
  const mine = countPieces(state.cells, ai);
  const theirs = countPieces(state.cells, opp);

  if (state.winner === ai) return 10_000;
  if (state.winner === opp) return -10_000;
  if (state.winner === "draw") {
    if (mine < theirs) return 5_000;
    if (mine > theirs) return -5_000;
    return 0;
  }

  let score = (mine - theirs) * w.material;

  if (state.winMode === "knockouts") {
    score += (state.knockouts[ai] - state.knockouts[opp]) * 180;
    if (state.knockouts[ai] >= state.winTarget - 1) score += 80;
  }
  if (state.winMode === "reduceTo") {
    if (theirs <= state.winTarget && mine > theirs) score += 400;
    if (mine <= state.winTarget && theirs > mine) score -= 400;
  }

  const myPieces: number[] = [];
  const oppPieces: number[] = [];
  for (let i = 0; i < state.cells.length; i++) {
    if (state.cells[i] === ai) myPieces.push(i);
    else if (state.cells[i] === opp) oppPieces.push(i);
  }

  let edge = 0;
  for (const p of myPieces) edge += openEdgeDirs(state, p);
  let oppEdge = 0;
  for (const p of oppPieces) oppEdge += openEdgeDirs(state, p);
  score -= edge * w.edge;
  score += oppEdge * (w.edge * 0.45);

  if (oppPieces.length > 0 && myPieces.length > 0) {
    let dist = 0;
    for (const p of myPieces) {
      let best = Infinity;
      for (const o of oppPieces) best = Math.min(best, manhattan(state.n, p, o));
      dist += best;
    }
    score -= (dist / myPieces.length) * w.approach;
  }

  const warned = new Set(state.warnings.map((x) => x.i));
  for (const p of myPieces) {
    if (warned.has(p)) score -= w.hazard;
  }
  for (const p of oppPieces) {
    if (warned.has(p)) score += w.hazard * 0.7;
  }

  let ownedBlocks = 0;
  for (const cell of state.cells) {
    if (cell === "block0" && ai === 0) ownedBlocks++;
    if (cell === "block1" && ai === 1) ownedBlocks++;
  }
  score += ownedBlocks * w.block;
  if (state.playerBlocks !== 0) {
    const left = state.playerBlocks === -1 ? 3 : state.blocksLeft[ai];
    score += left * (w.block * 0.35);
  }

  score += mobilityFor(state, ai) * w.mobility;
  score -= mobilityFor(state, opp) * (w.mobility * 0.6);

  return score;
}

function usefulBlockSquares(state: GameState): Set<number> {
  const { n, cells } = state;
  const useful = new Set<number>();
  const neigh: Array<readonly [number, number]> = [
    [-1, 0],
    [1, 0],
    [0, -1],
    [0, 1],
  ];
  for (let i = 0; i < cells.length; i++) {
    const cell = cells[i];
    if (cell !== 0 && cell !== 1) continue;
    for (const [dr, dc] of neigh) {
      const r = rowOf(n, i) + dr;
      const c = colOf(n, i) + dc;
      if (!inBounds(n, r, c)) continue;
      const j = idx(n, r, c);
      if (cells[j] === null) useful.add(j);
    }
  }
  for (let from = 0; from < cells.length; from++) {
    if (cells[from] !== 0 && cells[from] !== 1) continue;
    for (const dir of DIRECTIONS) {
      const [dr, dc] = DIR_DELTA[dir];
      let r = rowOf(n, from) + dr;
      let c = colOf(n, from) + dc;
      while (inBounds(n, r, c)) {
        const j = idx(n, r, c);
        if (cells[j] !== null) break;
        useful.add(j);
        r += dr;
        c += dc;
      }
    }
  }
  return useful;
}

function searchActions(state: GameState): TurnAction[] {
  const slides: TurnAction[] = legalMoves(state).map((move) => ({
    kind: "slide",
    move,
  }));
  const useful = usefulBlockSquares(state);
  const blocks: TurnAction[] = [];
  for (const at of legalBlockPlacements(state)) {
    if (useful.has(at)) blocks.push({ kind: "block", at });
  }
  return slides.concat(blocks);
}

function immediateBonus(
  before: GameState,
  action: TurnAction,
  after: GameState,
  ai: Player,
  w: Weights,
): number {
  const opp = other(ai);
  const mineLost = countPieces(before.cells, ai) - countPieces(after.cells, ai);
  const oppLost = countPieces(before.cells, opp) - countPieces(after.cells, opp);
  let bonus = 0;
  if (oppLost > 0) {
    const free = mineLost <= 0;
    bonus += w.knock * oppLost * (free ? 1.4 : 0.7);
  }
  if (action.kind === "slide" && action.move.knocks) {
    bonus += w.knock * 0.35;
    // Prefer momentum pushes that shove or cull stacks when Push is enabled.
    if (before.push && action.move.knock !== null) {
      const victim = action.move.knock;
      const who = before.cells[victim];
      if (who === 0 || who === 1) {
        const depth = defendStrength(
          before.cells,
          before.n,
          victim,
          action.move.dir,
          who,
        );
        if (depth >= 2) bonus += w.knock * 0.45;
      }
    }
  }
  if (action.kind === "block") {
    bonus += w.block * 0.8;
  }
  return bonus;
}

function scoreApplied(
  before: GameState,
  action: TurnAction,
  ai: Player,
  w: Weights,
): { after: GameState; score: number } {
  const after = applyAction(before, action);
  const score = evaluate(after, ai, w) + immediateBonus(before, action, after, ai, w);
  return { after, score };
}

function pickBest(
  state: GameState,
  actions: TurnAction[],
  ai: Player,
  w: Weights,
  rng: () => number,
): TurnAction | null {
  if (actions.length === 0) return null;
  let bestScore = -Infinity;
  let best: TurnAction[] = [];
  for (const action of actions) {
    const { score } = scoreApplied(state, action, ai, w);
    if (score > bestScore + 0.01) {
      bestScore = score;
      best = [action];
    } else if (Math.abs(score - bestScore) <= 0.01) {
      best.push(action);
    }
  }
  return best[Math.floor(rng() * best.length)]!;
}

function scoreSlideSimple(
  before: GameState,
  move: Move,
  after: GameState,
  ai: Player,
): number {
  const opp: Player = other(ai);
  const myBefore = countPieces(before.cells, ai);
  const oppBefore = countPieces(before.cells, opp);

  if (after.winner === ai) return 10_000;
  if (after.winner === opp) return -10_000;
  if (after.winner === "draw") {
    if (myBefore < oppBefore) return 5_000;
    if (myBefore > oppBefore) return -5_000;
    return 0;
  }

  const oppAfter = countPieces(after.cells, opp);
  if (oppAfter < oppBefore) return 1_000 + (oppBefore - oppAfter);
  if (move.knock !== null) return 100;
  return 0;
}

function pickSimpleAction(state: GameState, rng: () => number): TurnAction | null {
  const actions = legalTurnActions(state);
  if (actions.length === 0) return null;
  const ai = state.current;
  let bestScore = -Infinity;
  let best: TurnAction[] = [];
  for (const action of actions) {
    const after = applyAction(state, action);
    let s: number;
    if (action.kind === "slide") {
      s = scoreSlideSimple(state, action.move, after, ai);
    } else {
      s = scoreSlideSimple(
        state,
        { from: action.at, to: action.at, dir: "up", knock: null, knocks: false },
        after,
        ai,
      );
      if (s === 0) {
        let near = 0;
        const size = state.n;
        for (const [dr, dc] of [
          [-1, 0],
          [1, 0],
          [0, -1],
          [0, 1],
        ] as const) {
          const r = rowOf(size, action.at) + dr;
          const c = colOf(size, action.at) + dc;
          if (!inBounds(size, r, c)) continue;
          const cell = state.cells[idx(size, r, c)];
          if (cell !== null && !isBlock(cell) && cell !== ai) near++;
        }
        s = 10 + near * 5;
      }
    }
    if (s > bestScore) {
      bestScore = s;
      best = [action];
    } else if (s === bestScore) {
      best.push(action);
    }
  }
  return best[Math.floor(rng() * best.length)]!;
}

const COMPLEX_BEAM = 6;
const COMPLEX_BUDGET_MS = 80;

function pickComplexAction(state: GameState, rng: () => number): TurnAction | null {
  const w = THOUGHTFUL;
  const ai = state.current;
  const first = searchActions(state);
  if (first.length === 0) return null;

  const start = Date.now();
  type Node = { first: TurnAction; state: GameState; score: number };
  let beam: Node[] = [];

  for (const action of first) {
    const { after, score } = scoreApplied(state, action, ai, w);
    beam.push({ first: action, state: after, score });
  }
  beam.sort((a, b) => b.score - a.score);
  beam = beam.slice(0, COMPLEX_BEAM);

  const remaining = Math.max(0, state.actionsLeft - 1);
  for (let depth = 0; depth < remaining; depth++) {
    if (Date.now() - start > COMPLEX_BUDGET_MS) break;
    const next: Node[] = [];
    for (const node of beam) {
      if (!stillOurTurn(state, node.state)) {
        next.push(node);
        continue;
      }
      const acts = searchActions(node.state);
      if (acts.length === 0) {
        next.push(node);
        continue;
      }
      for (const action of acts) {
        const { after, score } = scoreApplied(node.state, action, ai, w);
        next.push({ first: node.first, state: after, score });
      }
    }
    next.sort((a, b) => b.score - a.score);
    beam = next.slice(0, COMPLEX_BEAM);
  }

  for (const node of beam) {
    if (node.state.phase !== "play" || node.state.current === ai) continue;
    const reply = pickBest(
      node.state,
      searchActions(node.state),
      node.state.current,
      THOUGHTFUL,
      rng,
    );
    if (!reply) continue;
    const after = applyAction(node.state, reply);
    node.score = evaluate(after, ai, w);
  }

  beam.sort((a, b) => b.score - a.score);
  const top = beam[0]!.score;
  const tied = beam.filter((n) => Math.abs(n.score - top) <= 0.01);
  return tied[Math.floor(rng() * tied.length)]!.first;
}

function opponentVisible(state: GameState, ai: Player): boolean {
  if (!state.revealPlacement) return false;
  return countPieces(state.cells, other(ai)) > 0;
}

function scorePlacementSpot(
  state: GameState,
  spot: number,
  style: AiStyle,
): number {
  const ai = state.current;
  const opp = other(ai);
  const n = state.n;
  const w = weightsFor(style);
  let score = 0;

  const open = openEdgeDirs(state, spot);
  score -= open * w.edge;

  const center = (n - 1) / 2;
  const cr = Math.abs(rowOf(n, spot) - center);
  const cc = Math.abs(colOf(n, spot) - center);
  score -= (cr + cc) * 2;

  let ownNear = 0;
  for (let i = 0; i < state.cells.length; i++) {
    if (state.cells[i] !== ai) continue;
    const d = manhattan(n, spot, i);
    if (d === 1) ownNear += 6;
    else if (d === 2) ownNear += 2;
  }
  score += ownNear;

  if (opponentVisible(state, ai)) {
    let nearest = Infinity;
    for (let i = 0; i < state.cells.length; i++) {
      if (state.cells[i] !== opp) continue;
      nearest = Math.min(nearest, manhattan(n, spot, i));
    }
    if (nearest < Infinity) {
      if (style === "aggressive") score -= nearest * 14;
      else if (style === "patient") score += nearest * 8;
      else score -= nearest * 5;
      if (nearest === 1) score -= style === "patient" ? 20 : -8;
    }
  } else {
    score += rngJitter(spot);
  }

  return score;
}

function rngJitter(spot: number): number {
  return (spot % 7) - 3;
}

function pickHeuristicPlacement(
  state: GameState,
  style: AiStyle,
  rng: () => number,
): number | null {
  const spots = legalPlaceIndices(state);
  if (spots.length === 0) return null;
  let bestScore = -Infinity;
  let best: number[] = [];
  for (const spot of spots) {
    const s = scorePlacementSpot(state, spot, style);
    if (s > bestScore + 0.01) {
      bestScore = s;
      best = [spot];
    } else if (Math.abs(s - bestScore) <= 0.01) {
      best.push(spot);
    }
  }
  return best[Math.floor(rng() * best.length)]!;
}

export function pickAiPlacement(
  state: GameState,
  rng: () => number = Math.random,
): number | null {
  const style = state.aiStyle ?? "simple";
  if (style === "simple") {
    const spots = legalPlaceIndices(state);
    if (spots.length === 0) return null;
    return spots[Math.floor(rng() * spots.length)]!;
  }
  return pickHeuristicPlacement(state, style, rng);
}

export function pickAiTurnAction(
  state: GameState,
  rng: () => number = Math.random,
): TurnAction | null {
  const style = state.aiStyle ?? "simple";
  if (style === "simple") return pickSimpleAction(state, rng);
  if (style === "complex") return pickComplexAction(state, rng);
  return pickBest(state, searchActions(state), state.current, weightsFor(style), rng);
}

/** @deprecated use pickAiTurnAction */
export function pickAiMove(state: GameState): Move | null {
  const action = pickAiTurnAction(state);
  if (!action) return null;
  return action.kind === "slide" ? action.move : null;
}
