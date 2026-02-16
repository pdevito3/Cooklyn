import type { Area } from 'react-easy-crop'

export async function cropImage(
  imageSrc: string,
  cropArea: Area,
): Promise<File> {
  const image = await loadImage(imageSrc)
  const canvas = document.createElement('canvas')
  const ctx = canvas.getContext('2d')!

  canvas.width = cropArea.width
  canvas.height = cropArea.height

  ctx.drawImage(
    image,
    cropArea.x,
    cropArea.y,
    cropArea.width,
    cropArea.height,
    0,
    0,
    cropArea.width,
    cropArea.height,
  )

  return new Promise((resolve, reject) => {
    canvas.toBlob(
      (blob) => {
        if (!blob) {
          reject(new Error('Failed to create image blob'))
          return
        }
        resolve(new File([blob], 'cropped-image.jpg', { type: 'image/jpeg' }))
      },
      'image/jpeg',
      0.92,
    )
  })
}

function loadImage(src: string): Promise<HTMLImageElement> {
  return new Promise((resolve, reject) => {
    const img = new Image()
    img.crossOrigin = 'anonymous'
    img.onload = () => resolve(img)
    img.onerror = reject
    img.src = src
  })
}
