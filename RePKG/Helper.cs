using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RePKG.Texture;

namespace RePKG
{
    public static class Helper
    {
        public static IEnumerable<string> GetPropertyKeysForDynamic(dynamic dynamicToGetPropertiesFor)
        {
            JObject attributesAsJObject = dynamicToGetPropertiesFor;
            var values = attributesAsJObject.ToObject<Dictionary<string, object>>();
            var toReturn = new List<string>();
            foreach (var key in values.Keys)
            {
                toReturn.Add(key);
            }

            return toReturn;
        }

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
        public static unsafe void CopyRawPixelsIntoBitmap(byte[] data, int textureWidth, Bitmap processedBitmap,
            bool invertedColorOrder)
        {
            var dataStride = textureWidth * 4;

            var bitmapData = processedBitmap.LockBits(
                new Rectangle(0, 0, processedBitmap.Width, processedBitmap.Height), ImageLockMode.ReadWrite,
                processedBitmap.PixelFormat);

            var bytesPerPixel = Image.GetPixelFormatSize(processedBitmap.PixelFormat) / 8;
            var heightInPixels = bitmapData.Height;
            var widthInBytes = bitmapData.Width * bytesPerPixel;
            var ptrFirstPixel = (byte*) bitmapData.Scan0;

            if (invertedColorOrder)
            {
                // BGRA
                Parallel.For(0, heightInPixels, y =>
                {
                    var currentLine = ptrFirstPixel + y * bitmapData.Stride;
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
                    var currentLine = ptrFirstPixel + y * bitmapData.Stride;
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

        public static string GetExtension(FreeImageFormat format)
        {
            var str = format.ToString().Split('_').Last();

            if (str == null)
                return string.Empty;

            if (format == FreeImageFormat.FIF_JPEG)
                str = str.Replace("JPEG", "JPG");

            return $".{str.ToLower()}";
        }
    }
}