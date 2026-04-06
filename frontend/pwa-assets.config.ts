import {
  defineConfig,
  minimal2023Preset as preset,
} from '@vite-pwa/assets-generator/config'

export default defineConfig({
  headLinkOptions: {
    preset: '2023',
  },
  preset: {
    ...preset,
    transparent: {
      sizes: [64, 192, 512],
      favicons: [[48, 'favicon-48x48.ico']],
      padding: 0.1,
      resizeOptions: {
        background: '#2563eb',
      },
    },
    maskable: {
      sizes: [512],
      padding: 0.2,
      resizeOptions: {
        background: '#2563eb',
      },
    },
    apple: {
      sizes: [180],
      padding: 0.15,
      resizeOptions: {
        background: '#2563eb',
      },
    },
  },
  images: ['public/pwa-source.svg'],
})
