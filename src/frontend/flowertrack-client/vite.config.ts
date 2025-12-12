import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react-swc';

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  // Base path dla GitHub Pages: https://daniel-kazarnowicz-baumalog.github.io/flowertrack/
  // W development używa '/', w production '/flowertrack/'
  base: process.env.NODE_ENV === 'production' ? '/flowertrack/' : '/',
});
