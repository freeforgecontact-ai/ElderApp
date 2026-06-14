import { defineConfig } from 'vite'

// PWA offline-first : tout est statique, aucun backend requis.
export default defineConfig({
  root: '.',
  publicDir: 'public',
  build: {
    outDir: 'dist',
    emptyOutDir: true,
    target: 'es2020'
  },
  server: { port: 5173, open: false }
})
