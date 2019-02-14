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

        // source: http://csharpexamples.com/fast-image-processing-c/
        public static unsafe void CopyRawPixelsIntoBitmap(byte[] data, int dataStride, Bitmap processedBitmap, bool invertedColorOrder)
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

        public static ImageFormat ConvertFIFFormat(FreeImageFormat format)
        {
            switch (format)
            {
                case FreeImageFormat.FIF_BMP:
                    return ImageFormat.Bmp;
                case FreeImageFormat.FIF_ICO:
                    return ImageFormat.Icon;
                case FreeImageFormat.FIF_JPEG:
                case FreeImageFormat.FIF_JNG:
                case FreeImageFormat.FIF_J2K:
                case FreeImageFormat.FIF_JP2:
                    return ImageFormat.Jpeg;
                case FreeImageFormat.FIF_PNG:
                    return ImageFormat.Png;
                case FreeImageFormat.FIF_TIFF:
                    return ImageFormat.Tiff;
                case FreeImageFormat.FIF_GIF:
                    return ImageFormat.Gif;
                default:
                    return null;
            }
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
