import fs from "node:fs";
import path from "node:path";
import type { Plugin } from "vite";

/** Dev-only: POST /api/log writes game logs to ./logs/ beside index.html. */
export function gameLogPlugin(): Plugin {
  return {
    name: "game-log",
    configureServer(server) {
      server.middlewares.use("/api/log", (req, res, next) => {
        if (req.method !== "POST") {
          next();
          return;
        }

        const chunks: Buffer[] = [];
        req.on("data", (chunk: Buffer) => chunks.push(chunk));
        req.on("end", () => {
          try {
            const body = JSON.parse(Buffer.concat(chunks).toString("utf8")) as {
              filename?: string;
              text?: string;
            };
            const text = body.text ?? "";
            const rawName = body.filename ?? "game.txt";
            const safeName = path.basename(rawName).replace(/[^\w.-]/g, "_");
            const dir = path.resolve(server.config.root, "logs");
            fs.mkdirSync(dir, { recursive: true });
            fs.writeFileSync(path.join(dir, safeName), text, "utf8");
            res.statusCode = 200;
            res.setHeader("Content-Type", "application/json");
            res.end(JSON.stringify({ ok: true, path: `logs/${safeName}` }));
          } catch {
            res.statusCode = 400;
            res.end(JSON.stringify({ ok: false }));
          }
        });
        req.on("error", () => {
          res.statusCode = 500;
          res.end(JSON.stringify({ ok: false }));
        });
      });
    },
  };
}
