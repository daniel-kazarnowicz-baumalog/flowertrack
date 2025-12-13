import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react-swc';

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  // Base path for GitHub Pages - root URL
  // https://daniel-kazarnowicz-baumalog.github.io/
  base: '/',
});
