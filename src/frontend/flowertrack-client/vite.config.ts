import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react-swc';

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  // Base path for GitHub Pages project site
  // https://daniel-kazarnowicz-baumalog.github.io/flowertrack/
  base: '/flowertrack/',
});
