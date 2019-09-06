using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace RePKG.Application.Texture
{
    public static class BitmapHelper
    {
        public static unsafe void CopyRawR8PixelsIntoBitmap(byte[] data, int textureWidth, Bitmap processedBitmap)
        {
            var dataStride = textureWidth;

            var bitmapData = processedBitmap.LockBits(
                new Rectangle(0, 0, processedBitmap.Width, processedBitmap.Height), ImageLockMode.ReadWrite,
                processedBitmap.PixelFormat);

            var bytesPerPixel = Image.GetPixelFormatSize(processedBitmap.PixelFormat) / 8;
            var heightInPixels = bitmapData.Height;
            var widthInBytes = bitmapData.Width * bytesPerPixel;
            var ptrFirstPixel = (byte*) bitmapData.Scan0;

            Parallel.For(0, heightInPixels, y =>
            {
                var currentLine = ptrFirstPixel + y * bitmapData.Stride;
                var currentLineData = y * dataStride;

                for (var x = 0; x < widthInBytes; x += bytesPerPixel)
                {
                    var dataX = x / bytesPerPixel;
                    var p = data[currentLineData + dataX];

                    currentLine[x] = p;
                    currentLine[x + 1] = p;
                    currentLine[x + 2] = p;

                    currentLine[x + 3] = 0xFF;
                }
            });

            processedBitmap.UnlockBits(bitmapData);
        }

        public static unsafe void CopyRawRG88PixelsIntoBitmap(byte[] data, int textureWidth, Bitmap processedBitmap)
        {
            var dataStride = textureWidth * 2;

            var bitmapData = processedBitmap.LockBits(
                new Rectangle(0, 0, processedBitmap.Width, processedBitmap.Height), ImageLockMode.ReadWrite,
                processedBitmap.PixelFormat);

            var bytesPerPixel = Image.GetPixelFormatSize(processedBitmap.PixelFormat) / 8;
            var heightInPixels = bitmapData.Height;
            var widthInBytes = bitmapData.Width * bytesPerPixel;
            var ptrFirstPixel = (byte*) bitmapData.Scan0;

            Parallel.For(0, heightInPixels, y =>
            {
                var currentLine = ptrFirstPixel + y * bitmapData.Stride;
                var currentLineData = y * dataStride;

                for (var x = 0; x < widthInBytes; x += bytesPerPixel)
                {
                    var dataX = (x / bytesPerPixel) * 2;
                    var p = data[currentLineData + dataX + 1];

                    currentLine[x] = p;
                    currentLine[x + 1] = p;
                    currentLine[x + 2] = p;

                    currentLine[x + 3] = 0xFF;
                }
            });

            processedBitmap.UnlockBits(bitmapData);
        }

        // source: http://csharpexamples.com/fast-image-processing-c/
        public static unsafe void CopyRawPixelsIntoBitmap(byte[] data, int textureWidth, Bitmap processedBitmap)
        {
            var dataStride = textureWidth * 4;

            var bitmapData = processedBitmap.LockBits(
                new Rectangle(0, 0, processedBitmap.Width, processedBitmap.Height), ImageLockMode.ReadWrite,
                processedBitmap.PixelFormat);

            var bytesPerPixel = Image.GetPixelFormatSize(processedBitmap.PixelFormat) / 8;
            var heightInPixels = bitmapData.Height;
            var widthInBytes = bitmapData.Width * bytesPerPixel;
            var ptrFirstPixel = (byte*) bitmapData.Scan0;

            // BGRA
            Parallel.For(0, heightInPixels, y =>
            {
                var currentLine = ptrFirstPixel + y * bitmapData.Stride;
                var currentLineData = y * dataStride;
                for (var x = 0; x < widthInBytes; x = x + bytesPerPixel)
                {
                    // R
                    currentLine[x] = data[currentLineData + x + 2]; 
                        
                    // G
                    currentLine[x + 1] = data[currentLineData + x + 1];
                        
                    // B
                    currentLine[x + 2] = data[currentLineData + x]; 
                        
                    // A
                    currentLine[x + 3] = data[currentLineData + x + 3]; 
                }
            });

            processedBitmap.UnlockBits(bitmapData);
        }
    }
}