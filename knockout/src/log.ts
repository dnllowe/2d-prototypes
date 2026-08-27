import {
  colOf,
  rowOf,
  type Config,
  type GameState,
  type Move,
  type Player,
} from "./game";

let lines: string[] = [];
let startedAt = "";

function stamp(): string {
  return new Date().toISOString().replace(/\.\d{3}Z$/, "Z");
}

export function square(n: number, i: number): string {
  const col = String.fromCharCode(65 + colOf(n, i));
  const row = rowOf(n, i) + 1;
  return `${col}${row}`;
}

export function startGameLog(
  config: Config,
  names: [string, string],
): void {
  startedAt = stamp();
  lines = [
    "Knock Out — game log",
    `started ${startedAt}`,
    "",
    "Rules:",
    ...formatRules(config),
    "",
    `Players: ${names[0]} (0) vs ${names[1]} (1)`,
    config.vsAi ? `Opponent: AI (${config.aiStyle})` : "Opponent: hotseat",
    "",
    "--- actions ---",
  ];
}

function formatRules(c: Config): string[] {
  return [
    `  grid=${c.n} pieces=${c.k}`,
    `  setup=${c.setupMode}` +
      (c.setupMode === "formation" ? `/${c.formationSource}` : "") +
      ` reveal=${c.revealPlacement} placeOnColor=${c.placeOnColor}`,
    `  overlap=${c.overlapMode} firstPlacerFirst=${c.firstPlacerFirst}`,
    `  win=${c.winMode}` +
      (c.winMode === "elimination" ? "" : ` target=${c.winTarget}`) +
      ` drawOnOneEach=${c.drawOnOneEach}`,
    `  actions=${c.actionsMode} knock=${c.knockStrength} requireBoardChange=${c.requireBoardChange}`,
    `  push=${c.push}` + (c.push ? ` landing=${c.pushLanding}` : ""),
    `  playerBlocks=${c.playerBlocks}`,
    `  spawnBlocks=${c.spawnBlocks}` +
      (c.spawnBlocks
        ? ` every=${c.spawnEveryRounds} warn=${c.spawnWarningRounds}`
        : ""),
  ];
}

export function logPlayerAction(
  state: GameState,
  who: Player | "system",
  text: string,
): void {
  const name =
    who === "system" ? "system" : `${state.names[who]}(${who})`;
  lines.push(`${name} ${text}`);
}

export function logPlace(state: GameState, at: number): void {
  logPlayerAction(state, state.current, `place ${square(state.n, at)}`);
}

export function logUnplace(state: GameState, at: number): void {
  logPlayerAction(state, state.current, `unplace ${square(state.n, at)}`);
}

export function logSlide(state: GameState, move: Move, who: Player): void {
  const n = state.n;
  let text = `slide ${square(n, move.from)}→${square(n, move.to)} ${move.dir}`;
  if (move.knock !== null) {
    text += move.knocks
      ? ` knock ${square(n, move.knock)}`
      : ` bump ${square(n, move.knock)}`;
  }
  logPlayerAction(state, who, text);
}

export function logBlock(state: GameState, at: number, who: Player): void {
  logPlayerAction(state, who, `block ${square(state.n, at)}`);
}

export function logEndTurn(
  state: GameState,
  who: Player,
  forfeit: boolean,
): void {
  logPlayerAction(state, who, forfeit ? "forfeit" : "end-turn");
}

export function logSkip(state: GameState, who: Player): void {
  logPlayerAction(state, who, "skip (no legal moves)");
}

export function logPhase(label: string): void {
  lines.push(`--- ${label} ---`);
}

export function logGameOver(state: GameState): void {
  const winner =
    state.winner === "draw"
      ? "draw"
      : state.winner === null
        ? "?"
        : `${state.names[state.winner]}(${state.winner})`;
  lines.push("");
  lines.push("--- over ---");
  lines.push(`winner=${winner} reason=${state.endReason ?? "?"}`);
  lines.push(`ended ${stamp()}`);
}

export function getGameLogText(): string {
  return lines.join("\n") + (lines.length ? "\n" : "");
}

export function hasGameLog(): boolean {
  return lines.length > 0;
}

export function logFilename(): string {
  const safe = startedAt.replace(/[:.]/g, "-");
  return `knockout-${safe || "game"}.txt`;
}

/** Save to logs/ via dev server (silent if unavailable). */
export async function persistGameLog(): Promise<void> {
  const text = getGameLogText();
  if (!text) return;
  try {
    await fetch("/api/log", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ filename: logFilename(), text }),
    });
  } catch {
    // Not running under Vite dev server — copy log still works.
  }
}

export async function copyGameLog(): Promise<boolean> {
  const text = getGameLogText();
  if (!text) return false;
  try {
    await navigator.clipboard.writeText(text);
    return true;
  } catch {
    return false;
  }
}
