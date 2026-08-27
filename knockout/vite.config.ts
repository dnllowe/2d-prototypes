import { defineConfig } from "vite";
import { gameLogPlugin } from "./vite-log-plugin";

export default defineConfig({
  plugins: [gameLogPlugin()],
  server: {
    port: 5173,
    strictPort: true,
  },
});
