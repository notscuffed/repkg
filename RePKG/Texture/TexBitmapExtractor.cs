using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace RePKG.Texture
{
    public static class TexBitmapExtractor
    {
        // source: http://csharpexamples.com/fast-image-processing-c/
        private static unsafe void CopyRawPixelsIntoBitmap(byte[] data, int dataStride, Bitmap processedBitmap, bool invertedColorOrder)
        {
            var bitmapData = processedBitmap.LockBits(new Rectangle(0, 0, processedBitmap.Width, processedBitmap.Height), ImageLockMode.ReadWrite, processedBitmap.PixelFormat);
 
            var bytesPerPixel = Image.GetPixelFormatSize(processedBitmap.PixelFormat) / 8;
            var heightInPixels = bitmapData.Height;
            var widthInBytes = bitmapData.Width * bytesPerPixel;
            var PtrFirstPixel = (byte*)bitmapData.Scan0;

            if (invertedColorOrder)
            {
                // BGRA
                Parallel.For(0, heightInPixels, y =>
                {
                    var currentLine = PtrFirstPixel + y * bitmapData.Stride;
                    var currentLineData = y * dataStride;
                    for (var x = 0; x < widthInBytes; x = x + bytesPerPixel)
                    {
                        currentLine[x] = data[currentLineData + x + 2];
                        currentLine[x + 1] = data[currentLineData + x + 1];
                        currentLine[x + 2] = data[currentLineData + x];
                        currentLine[x + 3] = data[currentLineData + x + 3];
                    }
                });
            }
            else
            {
                // RGBA
                Parallel.For(0, heightInPixels, y =>
                {
                    var currentLine = PtrFirstPixel + y * bitmapData.Stride;
                    var currentLineData = y * dataStride;
                    for (var x = 0; x < widthInBytes; x = x + bytesPerPixel)
                    {
                        currentLine[x] = data[currentLineData + x];
                        currentLine[x + 1] = data[currentLineData + x + 1];
                        currentLine[x + 2] = data[currentLineData + x + 2];
                        currentLine[x + 3] = data[currentLineData + x + 3];
                    }
                });
            }

            processedBitmap.UnlockBits(bitmapData);
        }

        public static Bitmap Extract(Tex tex)
        {
            var bytes = tex.Mipmaps[0].Bytes;

            if (tex.Mipmaps[0].LZ4Compressed == 1)
                bytes = TexLoader.DecompressLZ4(bytes, tex.Mipmaps[0].PixelCount);

            if (tex.TextureContainerVersion == TexMipmapVersion.Version3)
            {
                if (tex.ImageFormat != FreeImageFormat.FIF_UNKNOWN)
                    return new Bitmap(new MemoryStream(bytes));
            }

            if (tex.DxtCompression > 0)
                bytes = DXT.DecompressImage(tex.Mipmaps[0].Width, tex.Mipmaps[0].Height, bytes, DXT.DXTFlags.DXT5);

            var width = tex.ImageWidth;
            var textureWidth = tex.Mipmaps[0].Width;
            var height = tex.ImageHeight;
            var bitmap = new Bitmap(width, height);

            if (tex.TextureContainerVersion == TexMipmapVersion.Version3)
                CopyRawPixelsIntoBitmap(bytes, textureWidth * 4, bitmap, false);
            else // V2 has inverted color order
                CopyRawPixelsIntoBitmap(bytes, textureWidth * 4, bitmap, true);

            return bitmap;
        }
    }
}
