import "./style.css";
import { pickAiPlacement, pickAiTurnAction } from "./ai";
import {
  copyGameLog,
  hasGameLog,
  logBlock,
  logEndTurn,
  logGameOver,
  logPhase,
  logPlace,
  persistGameLog,
  logSkip,
  logSlide,
  logUnplace,
  startGameLog,
} from "./log";
import {
  DIR_DELTA,
  applyBlockPlacement,
  applyMove,
  applyPresetFormation,
  blocksAvailable,
  canPlace,
  endTurn,
  endTurnIsForfeit,
  colOf,
  countPieces,
  idx,
  isBlock,
  knockTravel,
  legalBlockPlacements,
  legalMoves,
  moveForClick,
  moveForKey,
  newGame,
  nextMarkTurn,
  placePiece,
  rowOf,
  squareOwner,
  validateConfig,
  visibleCells,
  warningRoundsLeft,
  unplacePiece,
  canUnplace,
  type BlockCell,
  type Config,
  type Direction,
  type FormationSource,
  type GameState,
  type Move,
  type OverlapMode,
  type Player,
  type PushLanding,
  type SetupMode,
  type ActionsMode,
  type WinMode,
  type KnockStrength,
  type AiStyle,
} from "./game";

const PALETTE = [
  { name: "Ember", fill: "#c45c26" },
  { name: "Ink", fill: "#2a5a72" },
] as const;

const KEY_DIR: Record<string, Direction> = {
  arrowup: "up",
  w: "up",
  arrowdown: "down",
  s: "down",
  arrowleft: "left",
  a: "left",
  arrowright: "right",
  d: "right",
};

const configScreen = document.querySelector("#config-screen")!;
const colorScreen = document.querySelector("#color-screen")!;
const gameScreen = document.querySelector("#game-screen")!;
const colorPicks = document.querySelector("#color-picks")!;
const boardOuter = document.querySelector("#board-outer") as HTMLElement;
const boardWrap = document.querySelector("#board-wrap") as HTMLElement;
const boardLabels = document.querySelector("#board-labels") as HTMLElement;
const boardActions = document.querySelector("#board-actions") as HTMLElement;
const hudEl = document.querySelector("#hud")!;
const skipBanner = document.querySelector("#skip-banner") as HTMLElement;
const hintEl = document.querySelector("#hint")!;
const overlay = document.querySelector("#overlay") as HTMLElement;
const overlayTitle = document.querySelector("#overlay-title")!;
const overlayBody = document.querySelector("#overlay-body")!;
const configError = document.querySelector("#config-error") as HTMLElement;
const freeOpts = document.querySelector("#cfg-free-opts") as HTMLElement;
const formationOpts = document.querySelector("#cfg-formation-opts") as HTMLElement;
const winTargetWrap = document.querySelector("#cfg-win-target-wrap") as HTMLElement;

let state: GameState | null = null;
let selected: number | null = null;
let blockMode = false;
let busy = false;
let gameOverLogged = false;

function afterStateChange(before: GameState | null, after: GameState): void {
  if (before && before.phase !== after.phase) {
    if (after.phase === "play") logPhase("play");
    else if (after.phase === "replace") logPhase("replace");
  }
  if (
    after.skipNotice &&
    (!before?.skipNotice ||
      before.skipNotice.player !== after.skipNotice.player)
  ) {
    logSkip(after, after.skipNotice.player);
  }
  if (after.phase === "over" && after.winner !== null && !gameOverLogged) {
    logGameOver(after);
    gameOverLogged = true;
    void persistGameLog();
  }
}

function radioValue(name: string): string {
  return (document.querySelector(`input[name="${name}"]:checked`) as HTMLInputElement)
    .value;
}

function readConfig(): Config {
  const n = Number((document.querySelector("#cfg-n") as HTMLInputElement).value);
  const k = Number((document.querySelector("#cfg-k") as HTMLInputElement).value);
  const placeOnColor = (document.querySelector("#cfg-color-place") as HTMLInputElement)
    .checked;
  const opp = radioValue("opp");
  const vsAi = opp !== "hotseat";
  const aiStyle: AiStyle = vsAi ? (opp as AiStyle) : "simple";
  const setupMode = radioValue("setup") as SetupMode;
  const revealPlacement = (document.querySelector("#cfg-reveal") as HTMLInputElement)
    .checked;
  const formationSource = radioValue("formation-src") as FormationSource;
  const overlapMode = radioValue("overlap") as OverlapMode;
  const winMode = radioValue("win") as WinMode;
  const winTarget = Number(
    (document.querySelector("#cfg-win-target") as HTMLInputElement).value,
  );
  const drawOnOneEach = (document.querySelector("#cfg-draw-one") as HTMLInputElement)
    .checked;
  const actionsMode = radioValue("actions") as ActionsMode;
  const knockStrength = radioValue("knock") as KnockStrength;
  const requireBoardChange = (
    document.querySelector("#cfg-board-change") as HTMLInputElement
  ).checked;
  const push = (document.querySelector("#cfg-push") as HTMLInputElement).checked;
  const pushLanding = radioValue("push-landing") as PushLanding;
  const spawnBlocks = (document.querySelector("#cfg-spawn") as HTMLInputElement)
    .checked;
  const spawnEveryRounds = Number(
    (document.querySelector("#cfg-spawn-every") as HTMLInputElement).value,
  );
  const spawnWarningRounds = Number(
    (document.querySelector("#cfg-spawn-warn") as HTMLInputElement).value,
  );
  const playerBlocks = Number(
    (document.querySelector("#cfg-player-blocks-n") as HTMLInputElement).value,
  );
  const firstPlacerFirst = (
    document.querySelector("#cfg-first-placer-first") as HTMLInputElement
  ).checked;
  return {
    n,
    k,
    placeOnColor,
    vsAi,
    aiStyle,
    revealPlacement,
    setupMode,
    formationSource,
    overlapMode,
    winMode,
    winTarget,
    drawOnOneEach,
    actionsMode,
    knockStrength,
    requireBoardChange,
    push,
    pushLanding,
    spawnBlocks,
    spawnEveryRounds,
    spawnWarningRounds,
    playerBlocks,
    firstPlacerFirst,
  };
}

function syncConfigUi() {
  const setup = radioValue("setup");
  freeOpts.hidden = setup !== "free";
  formationOpts.hidden = setup !== "formation";
  const win = radioValue("win");
  winTargetWrap.hidden = win === "elimination";
  const spawnOn = (document.querySelector("#cfg-spawn") as HTMLInputElement).checked;
  (document.querySelector("#cfg-spawn-every") as HTMLInputElement).disabled = !spawnOn;
  (document.querySelector("#cfg-spawn-warn") as HTMLInputElement).disabled = !spawnOn;
  const pushOn = (document.querySelector("#cfg-push") as HTMLInputElement).checked;
  const landing = document.querySelector("#cfg-push-landing") as HTMLElement;
  landing.hidden = !pushOn;
  for (const el of landing.querySelectorAll("input")) {
    (el as HTMLInputElement).disabled = !pushOn;
  }
}

function show(el: Element, on: boolean) {
  (el as HTMLElement).hidden = !on;
}

function paintScreens(which: "config" | "color" | "game") {
  show(configScreen, which === "config");
  show(colorScreen, which === "color");
  show(gameScreen, which === "game");
}

function remainingToPlace(s: GameState): number {
  if (s.phase === "replace") return s.replaceLeft[s.current];
  if (s.setupMode === "formation") return s.k - s.drafts[0].length;
  return s.k - s.drafts[s.current].length;
}

function playerLabel(s: GameState, p: Player): string {
  return s.names[p];
}

function setHint(s: GameState) {
  if (s.phase === "replace") {
    hintEl.textContent =
      "Overlaps cleared — place your displaced piece on an empty square.";
    return;
  }
  if (s.phase === "place") {
    if (s.setupMode === "formation") {
      hintEl.textContent =
        "Pick squares on one half of the board. The other side mirrors automatically. Click a placed square to undo.";
      return;
    }
    if (!s.revealPlacement) {
      hintEl.textContent =
        "Blind placement — your opponent cannot see these pieces until both sides are done. Click a placed square to undo.";
      return;
    }
    hintEl.textContent = s.placeOnColor
      ? "Place on your checker color. Click a placed square to undo."
      : "Click an empty square to place, or a placed square to undo.";
    return;
  }
  hintEl.textContent =
    s.actionsMode === "fixed"
      ? "3 actions per turn — slide a piece or place a block on an empty square."
      : "Up to 3 actions per turn (≤ pieces) — slide or place a block.";
  if (s.spawnBlocks && s.phase === "play") {
    hintEl.textContent +=
      " Marked squares become blocks after a warning — leave them or get eliminated.";
  }
  if (blockMode && blocksAvailable(s)) {
    hintEl.textContent = "Block mode — click an empty square. Press B or Block to cancel.";
  }
}

function renderHud(s: GameState) {
  const p0 = countPieces(s.cells, 0);
  const p1 = countPieces(s.cells, 1);
  let status = "";
  if (s.phase === "place") {
    status = `${playerLabel(s, s.current)} places · ${remainingToPlace(s)} left`;
    if (s.setupMode === "formation") status = `Formation · ${remainingToPlace(s)} left`;
  } else if (s.phase === "replace") {
    status = `${playerLabel(s, s.current)} re-places · ${s.replaceLeft[s.current]} left`;
  } else if (s.phase === "play") {
    const actions =
      s.actionsLeft === 1 ? "1 action left" : `${s.actionsLeft} actions left`;
    let extra = "";
    if (s.winMode === "knockouts") {
      extra = ` · KO ${s.knockouts[0]}–${s.knockouts[1]} (to ${s.winTarget})`;
    } else if (s.winMode === "reduceTo") {
      extra = ` · leave opp at ${s.winTarget}`;
    }
    if (s.playerBlocks !== 0) {
      const left = s.playerBlocks === -1 ? "∞" : String(s.blocksLeft[s.current]);
      extra += ` · blocks ${left}`;
    }
    if (s.spawnBlocks) {
      const target = nextMarkTurn(s.spawnMarksDone, s.spawnEveryRounds);
      const turnsLeft = Math.max(0, target - s.turnsFinished);
      const roundsLeft = Math.max(1, Math.ceil(turnsLeft / 2));
      extra +=
        s.warnings.length > 0
          ? ` · ${s.warnings.length} hazard${s.warnings.length === 1 ? "" : "s"}`
          : ` · block in ${roundsLeft} round${roundsLeft === 1 ? "" : "s"}`;
    }
    status = `${playerLabel(s, s.current)} · ${actions}${extra}`;
  } else if (s.winner === "draw") {
    status = "Draw";
  } else if (s.winner !== null) {
    status = `${playerLabel(s, s.winner)} wins`;
  }

  hudEl.innerHTML = `
    <div class="hud-side">
      <span class="chip" style="background:${s.colors[0]}"></span>
      <span>${s.names[0]} · ${p0}${
        s.phase === "play" && s.playerBlocks !== 0
          ? ` · ${s.playerBlocks === -1 ? "∞" : s.blocksLeft[0]} blk`
          : ""
      }</span>
    </div>
    <div class="status"><strong>${status}</strong></div>
    <div class="hud-side right">
      <span>${s.names[1]} · ${p1}${
        s.phase === "play" && s.playerBlocks !== 0
          ? ` · ${s.playerBlocks === -1 ? "∞" : s.blocksLeft[1]} blk`
          : ""
      }</span>
      <span class="chip" style="background:${s.colors[1]}"></span>
      <button type="button" class="ghost" id="copy-log" title="Copy game log">Copy log</button>
      <button type="button" class="ghost" id="new-game">New game</button>
    </div>
  `;
  hudEl.querySelector("#new-game")?.addEventListener("click", backToConfig);
  hudEl.querySelector("#copy-log")?.addEventListener("click", () => {
    void (async () => {
      const btn = hudEl.querySelector("#copy-log") as HTMLButtonElement | null;
      if (!hasGameLog() || !btn) return;
      const ok = await copyGameLog();
      const prev = btn.textContent;
      btn.textContent = ok ? "Copied" : "Copy failed";
      window.setTimeout(() => {
        if (btn.textContent === "Copied" || btn.textContent === "Copy failed") {
          btn.textContent = prev;
        }
      }, 1200);
    })();
  });

  if (s.phase === "play" && s.skipNotice) {
    const skipped = playerLabel(s, s.skipNotice.player);
    skipBanner.hidden = false;
    skipBanner.textContent = `${skipped} skipped — no legal moves (slides or blocks).`;
  } else {
    skipBanner.hidden = true;
    skipBanner.textContent = "";
  }
}

function renderOverlay(s: GameState) {
  if (s.phase !== "over" || s.winner === null) {
    overlay.hidden = true;
    return;
  }
  overlay.hidden = false;
  if (s.winner === "draw") {
    overlayTitle.textContent = "Draw";
    if (s.endReason === "one_each") {
      overlayBody.textContent =
        "One apiece left — you can evade forever, so this one is called.";
    } else if (s.endReason === "mutual") {
      overlayBody.textContent = "Both sides went over the edge.";
    } else if (s.endReason === "reduceTo") {
      overlayBody.textContent = "Both sides hit the piece threshold together.";
    } else if (s.endReason === "stalemate") {
      overlayBody.textContent =
        "No legal slides left, and both sides have the same number of pieces.";
    } else {
      overlayBody.textContent = "No legal slides left.";
    }
  } else {
    overlayTitle.textContent = `${playerLabel(s, s.winner)} wins`;
    if (s.endReason === "knockouts") {
      overlayBody.textContent = `Reached ${s.winTarget} knock-offs.`;
    } else if (s.endReason === "reduceTo") {
      overlayBody.textContent = `Opponent down to ${s.winTarget} piece(s).`;
    } else if (s.endReason === "stalemate") {
      overlayBody.textContent =
        "No legal slides left — most pieces remaining wins.";
    } else if (s.endReason === "forfeit") {
      overlayBody.textContent = "Opponent passed without changing the board twice.";
    } else {
      overlayBody.textContent = "Every opposing piece is off the board.";
    }
  }
  // Ensure over is logged even if we reached overlay without afterStateChange.
  if (!gameOverLogged) {
    logGameOver(s);
    gameOverLogged = true;
    void persistGameLog();
  }
}

function cellEl(i: number): HTMLElement | null {
  return boardWrap.querySelector(`.cell[data-index="${i}"]`);
}

function pieceInCell(i: number): HTMLElement | null {
  return cellEl(i)?.querySelector(".piece") ?? null;
}

function cellSize(): number {
  const cell = boardWrap.querySelector(".cell");
  return cell ? cell.getBoundingClientRect().width : 40;
}

function colLabel(c: number): string {
  return String.fromCharCode(65 + c);
}

function renderBoardActions(s: GameState) {
  const humanPlay =
    s.phase === "play" && !(s.vsAi && s.current === 1);
  if (!humanPlay) {
    boardActions.replaceChildren();
    return;
  }

  const buttons: HTMLButtonElement[] = [];

  if (blocksAvailable(s)) {
    const blockBtn = document.createElement("button");
    blockBtn.type = "button";
    blockBtn.className = `ghost${blockMode ? " active" : ""}`;
    blockBtn.id = "block-mode";
    blockBtn.textContent = blockMode ? "Cancel block" : "Place block";
    blockBtn.addEventListener("click", () => {
      if (!state || !blocksAvailable(state)) return;
      blockMode = !blockMode;
      if (blockMode) selected = null;
      render();
    });
    buttons.push(blockBtn);
  }

  const endBtn = document.createElement("button");
  endBtn.type = "button";
  endBtn.className = "ghost";
  endBtn.id = "end-turn";
  endBtn.textContent = endTurnIsForfeit(s) ? "Forfeit" : "End turn";
  endBtn.addEventListener("click", () => {
    void commitEndTurn();
  });
  buttons.push(endBtn);

  boardActions.replaceChildren(...buttons);
}

function appendBlockEl(
  cell: HTMLElement,
  occupant: BlockCell,
  colors: [string, string],
): void {
  const block = document.createElement("span");
  block.className = "block";
  if (occupant === "block0" || occupant === "block1") {
    block.classList.add("block-owned");
    block.style.setProperty(
      "--block-tint",
      colors[occupant === "block0" ? 0 : 1],
    );
  } else {
    block.classList.add("block-neutral");
  }
  cell.append(block);
}

function wait(ms: number): Promise<void> {
  return new Promise((resolve) => window.setTimeout(resolve, ms));
}

function animateEl(
  el: HTMLElement,
  keyframes: Keyframe[],
  options: KeyframeAnimationOptions,
): Promise<void> {
  return new Promise((resolve) => {
    const anim = el.animate(keyframes, options);
    anim.onfinish = () => resolve();
    anim.oncancel = () => resolve();
  });
}

async function animateMove(s: GameState, move: Move): Promise<void> {
  const size = cellSize();
  const [dr, dc] = DIR_DELTA[move.dir];
  const windX = -dc * size * 0.28;
  const windY = -dr * size * 0.28;

  const fromR = rowOf(s.n, move.from);
  const fromC = colOf(s.n, move.from);

  if (move.knock !== null && !move.knocks) {
    const aimR = rowOf(s.n, move.knock);
    const aimC = colOf(s.n, move.knock);
    const slider = pieceInCell(move.from);
    if (!slider) return;

    if (move.from !== move.to) {
      // Slide up to a braced stack, bump, stay at the landing square.
      const travelX = (colOf(s.n, move.to) - fromC) * size;
      const travelY = (rowOf(s.n, move.to) - fromR) * size;
      const bumpX = (aimC - fromC) * size * 0.35;
      const bumpY = (aimR - fromR) * size * 0.35;
      const dist = Math.max(1, Math.abs(aimR - fromR) + Math.abs(aimC - fromC));
      slider.classList.add("moving");
      await animateEl(
        slider,
        [
          { transform: "translate(0, 0)" },
          { transform: `translate(${windX}px, ${windY}px)` },
        ],
        { duration: 100, easing: "ease-out", fill: "forwards" },
      );
      await animateEl(
        slider,
        [
          { transform: `translate(${windX}px, ${windY}px)` },
          { transform: `translate(${travelX}px, ${travelY}px)` },
        ],
        {
          duration: 90 + dist * 70,
          easing: "cubic-bezier(0.2, 0.8, 0.2, 1)",
          fill: "forwards",
        },
      );
      await animateEl(
        slider,
        [
          { transform: `translate(${travelX}px, ${travelY}px)` },
          { transform: `translate(${bumpX}px, ${bumpY}px)` },
        ],
        { duration: 80, easing: "ease-in", fill: "forwards" },
      );
      await animateEl(
        slider,
        [
          { transform: `translate(${bumpX}px, ${bumpY}px)` },
          { transform: `translate(${travelX}px, ${travelY}px)` },
        ],
        { duration: 100, easing: "ease-out", fill: "forwards" },
      );
      await wait(40);
      return;
    }

    const bumpX = (aimC - fromC) * size * 0.45;
    const bumpY = (aimR - fromR) * size * 0.45;
    slider.classList.add("moving");
    await animateEl(
      slider,
      [
        { transform: "translate(0, 0)" },
        { transform: `translate(${windX}px, ${windY}px)` },
      ],
      { duration: 100, easing: "ease-out", fill: "forwards" },
    );
    await animateEl(
      slider,
      [
        { transform: `translate(${windX}px, ${windY}px)` },
        { transform: `translate(${bumpX}px, ${bumpY}px)` },
      ],
      { duration: 120, easing: "ease-in", fill: "forwards" },
    );
    await animateEl(
      slider,
      [
        { transform: `translate(${bumpX}px, ${bumpY}px)` },
        { transform: "translate(0, 0)" },
      ],
      { duration: 140, easing: "ease-out", fill: "forwards" },
    );
    await wait(40);
    return;
  }

  // Follow landings aim at the impact square first, then settle onto the vacated front.
  const followPush =
    move.knock !== null && move.knocks && move.to === move.knock;
  const impact =
    followPush && move.knock !== null
      ? idx(s.n, rowOf(s.n, move.knock) - dr, colOf(s.n, move.knock) - dc)
      : null;
  const aim =
    move.knock !== null && move.to === move.from
      ? move.knock
      : impact !== null
        ? impact
        : move.to;
  const aimR = rowOf(s.n, aim);
  const aimC = colOf(s.n, aim);
  const travelX = (aimC - fromC) * size;
  const travelY = (aimR - fromR) * size;
  const settleX = (colOf(s.n, move.to) - fromC) * size;
  const settleY = (rowOf(s.n, move.to) - fromR) * size;
  const dist = Math.max(1, Math.abs(aimR - fromR) + Math.abs(aimC - fromC));

  const slider = pieceInCell(move.from);
  if (!slider) return;

  slider.classList.add("moving");
  await animateEl(
    slider,
    [
      { transform: "translate(0, 0)" },
      { transform: `translate(${windX}px, ${windY}px)` },
    ],
    { duration: 120, easing: "ease-out", fill: "forwards" },
  );

  await animateEl(
    slider,
    [
      { transform: `translate(${windX}px, ${windY}px)` },
      { transform: `translate(${travelX}px, ${travelY}px)` },
    ],
    {
      duration: 90 + dist * 70,
      easing: "cubic-bezier(0.2, 0.8, 0.2, 1)",
      fill: "forwards",
    },
  );

  if (move.to === move.from && move.knock !== null) {
    await animateEl(
      slider,
      [
        { transform: `translate(${travelX}px, ${travelY}px)` },
        { transform: `translate(${settleX}px, ${settleY}px)` },
      ],
      { duration: 100, easing: "ease-out", fill: "forwards" },
    );
  }

  if (move.knock !== null && move.knocks) {
    const victim = pieceInCell(move.knock);
    if (victim) {
      // Travel is computed before the slider occupies its landing square.
      const travel = knockTravel(s.cells, s.n, move.knock, move.dir);
      victim.classList.add("moving");

      if (travel.path.length === 0 && travel.eliminated) {
        const offX = dc * size * 1.15;
        const offY = dr * size * 1.15;
        await animateEl(
          victim,
          [
            { transform: "translate(0, 0)", opacity: 1 },
            { transform: `translate(${offX}px, ${offY}px)`, opacity: 0 },
          ],
          { duration: 220, easing: "ease-in", fill: "forwards" },
        );
      } else if (travel.path.length > 0) {
        const end = travel.path[travel.path.length - 1]!;
        const vx = (colOf(s.n, end) - colOf(s.n, move.knock)) * size;
        const vy = (rowOf(s.n, end) - rowOf(s.n, move.knock)) * size;
        const vDist = travel.path.length;
        await animateEl(
          victim,
          [
            { transform: "translate(0, 0)", opacity: 1 },
            {
              transform: `translate(${vx}px, ${vy}px)`,
              opacity: travel.eliminated ? 0.35 : 1,
            },
          ],
          {
            duration: 90 + vDist * 70,
            easing: "cubic-bezier(0.2, 0.8, 0.2, 1)",
            fill: "forwards",
          },
        );
        if (travel.eliminated) {
          const offX = vx + dc * size * 0.9;
          const offY = vy + dr * size * 0.9;
          await animateEl(
            victim,
            [
              { transform: `translate(${vx}px, ${vy}px)`, opacity: 0.35 },
              { transform: `translate(${offX}px, ${offY}px)`, opacity: 0 },
            ],
            { duration: 160, easing: "ease-in", fill: "forwards" },
          );
        }
      }
    }
  }

  if (followPush) {
    await animateEl(
      slider,
      [
        { transform: `translate(${travelX}px, ${travelY}px)` },
        { transform: `translate(${settleX}px, ${settleY}px)` },
      ],
      { duration: 120, easing: "ease-out", fill: "forwards" },
    );
  }

  await wait(40);
}

function renderBoard(s: GameState) {
  const moves = selected !== null ? legalMoves(s).filter((m) => m.from === selected) : [];
  const blockSpots =
    blockMode && s.phase === "play" ? new Set(legalBlockPlacements(s)) : new Set<number>();
  const legalTos = new Set(
    moves
      .filter((m) => m.knock === null || (!m.knocks && m.to !== m.from))
      .map((m) => m.to),
  );
  const forcedTos = new Set(
    moves
      .filter((m) => m.knocks && m.knock !== null && m.to !== m.from && !legalTos.has(m.to))
      .map((m) => m.to),
  );
  const knocks = new Set(
    moves
      .filter((m) => m.knock !== null && (m.knocks || m.to === m.from))
      .map((m) => m.knock!)
  );
  const warned = new Set(s.warnings.map((w) => w.i));
  const view = visibleCells(s);
  const aiPlacing =
    s.vsAi &&
    s.current === 1 &&
    (s.phase === "place" || s.phase === "replace") &&
    s.setupMode === "free";

  boardOuter.style.setProperty("--n", String(s.n));
  boardOuter.style.setProperty("--p0", s.colors[0]);
  boardOuter.style.setProperty("--p1", s.colors[1]);
  boardOuter.style.setProperty("--turn", s.colors[s.current]);
  boardWrap.replaceChildren();
  boardLabels.replaceChildren();

  for (let c = 0; c < s.n; c++) {
    const label = document.createElement("span");
    label.className = "board-label col";
    label.textContent = colLabel(c);
    label.style.setProperty("--c", String(c));
    boardLabels.append(label);
  }

  for (let r = 0; r < s.n; r++) {
    const rowLabel = document.createElement("span");
    rowLabel.className = "board-label row";
    rowLabel.textContent = String(r + 1);
    rowLabel.style.setProperty("--r", String(r));
    boardLabels.append(rowLabel);

    for (let c = 0; c < s.n; c++) {
      const i = r * s.n + c;
      const cell = document.createElement("button");
      cell.type = "button";
      cell.className = `cell ${squareOwner(s.n, i) === 0 ? "dark" : "light"}`;
      cell.dataset.index = String(i);
      if (
        (s.phase === "place" || s.phase === "replace") &&
        canPlace(s, i) &&
        !aiPlacing
      ) {
        cell.classList.add("place-ok");
      }
      if (selected === i) cell.classList.add("selected");
      if (legalTos.has(i) || forcedTos.has(i)) cell.classList.add("legal");
      if (blockSpots.has(i)) cell.classList.add("block-ok");
      if (knocks.has(i)) cell.classList.add("knockable");
      if (warned.has(i)) cell.classList.add("warned");

      const occupant = view[i];
      if (warned.has(i) && !isBlock(occupant)) {
        const warn = document.createElement("span");
        warn.className = "warn";
        const w = s.warnings.find((x) => x.i === i);
        const left = w ? warningRoundsLeft(w, s.turnsFinished) : 1;
        warn.title =
          left <= 1
            ? "Becomes a block after both players act"
            : `Becomes a block in about ${left} rounds`;
        cell.append(warn);
      }
      if (isBlock(occupant)) {
        appendBlockEl(cell, occupant, s.colors);
      } else if (occupant === 0 || occupant === 1) {
        const piece = document.createElement("span");
        piece.className = "piece";
        piece.style.setProperty("--piece", s.colors[occupant]);
        cell.append(piece);
      }
      cell.addEventListener("click", () => onCellClick(i));
      boardWrap.append(cell);
    }
  }

  renderBoardActions(s);
}

function render() {
  if (!state) {
    paintScreens("config");
    overlay.hidden = true;
    skipBanner.hidden = true;
    return;
  }
  paintScreens("game");
  renderHud(state);
  renderBoard(state);
  renderOverlay(state);
  setHint(state);
}

function backToConfig() {
  state = null;
  selected = null;
  blockMode = false;
  busy = false;
  overlay.hidden = true;
  paintScreens("config");
}

async function commitBlock(at: number): Promise<void> {
  if (!state || busy) return;
  const who = state.current;
  logBlock(state, at, who);
  busy = true;
  selected = null;
  blockMode = false;
  renderBoard(state);
  try {
    await wait(80);
    const before = state;
    state = applyBlockPlacement(state, at);
    afterStateChange(before, state);
  } finally {
    busy = false;
  }
  render();
  maybeAi();
}

async function commitEndTurn(): Promise<void> {
  if (!state || busy || state.phase !== "play") return;
  if (state.vsAi && state.current === 1) return;
  const who = state.current;
  logEndTurn(state, who, endTurnIsForfeit(state));
  busy = true;
  selected = null;
  blockMode = false;
  try {
    const before = state;
    state = endTurn(state);
    afterStateChange(before, state);
  } finally {
    busy = false;
  }
  render();
  maybeAi();
}

async function commitMove(move: Move): Promise<void> {
  if (!state || busy) return;
  const who = state.current;
  logSlide(state, move, who);
  busy = true;
  selected = null;
  blockMode = false;
  renderBoard(state);
  try {
    await animateMove(state, move);
    const before = state;
    state = applyMove(state, move);
    afterStateChange(before, state);
  } finally {
    busy = false;
  }
  render();
  maybeAi();
}

function maybeAi() {
  if (!state || busy) return;
  if (!state.vsAi) return;
  if (state.phase === "over") return;
  // Formation is always human-authored (or preset). AI only plays as P1's opponent.
  if (state.setupMode === "formation" && state.phase === "place") return;
  if (state.current !== 1) return;

  busy = true;
  const delay = state.phase === "play" ? 220 : 200;
  window.setTimeout(async () => {
    if (!state || !state.vsAi || state.current !== 1 || state.phase === "over") {
      busy = false;
      return;
    }
    if (state.phase === "place" || state.phase === "replace") {
      const spot = pickAiPlacement(state);
      if (spot !== null) {
        logPlace(state, spot);
        const before = state;
        state = placePiece(state, spot);
        afterStateChange(before, state);
      }
      selected = null;
      busy = false;
      render();
      maybeAi();
      return;
    }

    const action = pickAiTurnAction(state);
    if (!action) {
      busy = false;
      render();
      return;
    }
    selected = null;
    blockMode = false;
    renderBoard(state);
    try {
      const before = state;
      if (action.kind === "slide") {
        logSlide(state, action.move, 1);
        await animateMove(state, action.move);
        state = applyMove(state, action.move);
      } else {
        logBlock(state, action.at, 1);
        await wait(80);
        state = applyBlockPlacement(state, action.at);
      }
      afterStateChange(before, state);
    } finally {
      busy = false;
    }
    render();
    maybeAi();
  }, delay);
}

function onCellClick(i: number) {
  if (!state || busy || state.phase === "over") return;
  if (state.vsAi && state.current === 1 && state.setupMode === "free") return;
  if (state.vsAi && state.current === 1 && state.phase === "replace") return;

  if (state.phase === "place" || state.phase === "replace") {
    if (state.phase === "place" && canUnplace(state, i)) {
      logUnplace(state, i);
      state = unplacePiece(state, i);
      render();
      return;
    }
    const before = state;
    const next = placePiece(state, i);
    if (next === state) return;
    logPlace(before, i);
    state = next;
    afterStateChange(before, state);
    render();
    maybeAi();
    return;
  }

  if (blockMode && legalBlockPlacements(state).includes(i)) {
    void commitBlock(i);
    return;
  }

  const occupant = state.cells[i];
  if (occupant === state.current) {
    blockMode = false;
    selected = selected === i ? null : i;
    render();
    return;
  }

  if (selected === null) return;

  const move = moveForClick(legalMoves(state), selected, i);
  if (!move) {
    selected = null;
    render();
    return;
  }

  void commitMove(move);
}

function onKey(e: KeyboardEvent) {
  if (!state || busy || state.phase !== "play") return;
  if (state.vsAi && state.current === 1) return;
  const target = e.target as HTMLElement | null;
  if (target && (target.tagName === "INPUT" || target.tagName === "TEXTAREA")) return;

  if (e.key.toLowerCase() === "b" && blocksAvailable(state)) {
    e.preventDefault();
    blockMode = !blockMode;
    if (blockMode) selected = null;
    render();
    return;
  }

  const dir = KEY_DIR[e.key.toLowerCase()];
  if (!dir) return;
  e.preventDefault();
  blockMode = false;

  if (selected === null) {
    const mine = state.cells
      .map((cell, i) => (cell === state!.current ? i : -1))
      .filter((i) => i >= 0);
    if (mine.length === 1) selected = mine[0]!;
    else return;
  }

  const move = moveForKey(state, selected, dir);
  if (!move) return;
  void commitMove(move);
}

document.querySelector("#config-form")!.addEventListener("submit", (e) => {
  e.preventDefault();
  const config = readConfig();
  const error = validateConfig(config);
  if (error) {
    configError.hidden = false;
    configError.textContent = error;
    return;
  }
  configError.hidden = true;
  paintScreens("color");
});

for (const el of document.querySelectorAll(
  'input[name="setup"], input[name="win"], #cfg-spawn, #cfg-push',
)) {
  el.addEventListener("change", syncConfigUi);
}
syncConfigUi();

PALETTE.forEach((swatch, i) => {
  const btn = document.createElement("button");
  btn.type = "button";
  btn.className = "color-pick";
  btn.style.background = swatch.fill;
  btn.textContent = swatch.name;
  btn.addEventListener("click", () => {
    const config = readConfig();
    const otherSwatch = PALETTE[1 - i]!;
    const colors: [string, string] = [swatch.fill, otherSwatch.fill];
    const names: [string, string] = [swatch.name, otherSwatch.name];
    startGameLog(config, names);
    gameOverLogged = false;
    logPhase("place");
    state = newGame(config, colors, names);
    if (config.setupMode === "formation" && config.formationSource === "preset") {
      const before = state;
      state = applyPresetFormation(state);
      afterStateChange(before, state);
    }
    selected = null;
    busy = false;
    render();
    boardOuter.focus();
    maybeAi();
  });
  colorPicks.append(btn);
});

document.querySelector("#overlay-again")!.addEventListener("click", backToConfig);
window.addEventListener("keydown", onKey);

paintScreens("config");
