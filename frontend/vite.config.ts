import path from 'path'
import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import tailwindcss from '@tailwindcss/vite'
import svgr from 'vite-plugin-svgr'
import { reactClickToComponent } from 'vite-plugin-react-click-to-component'
import { TanStackRouterVite } from '@tanstack/router-plugin/vite'

process.env.LAUNCH_EDITOR = 'code'

// https://vite.dev/config/
export default defineConfig({
  plugins: [
    TanStackRouterVite({ autoCodeSplitting: true }),
    react(),
    tailwindcss(),
    svgr({
      include: '**/*.svg',
      svgrOptions: {
        exportType: 'default',
      },
    }),
    reactClickToComponent(),
  ],
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './src'),
    },
  },
  server: {
    proxy: {
      '/api': {
        target:
          process.env.services__server__https__0 ||
          process.env.services__server__http__0 ||
          'http://localhost:5000',
        changeOrigin: true,
        secure: false,
      },
    },
  },
})
