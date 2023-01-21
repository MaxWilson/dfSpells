import { defineConfig } from 'vite'

export default defineConfig({
  root: "src",
  base: "dfSpells",
  build: {
    outDir: "../docs",
    emptyOutDir: true,
    sourcemap: true
  }
});
