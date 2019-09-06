using System;
using System.Drawing;
using System.IO;
using RePKG.Core.Texture;

namespace RePKG.Tests
{
    public static class PreviewWriter
    {
        public static void WriteTexture(Tex texture, string fileNameWithoutExtension)
        {
            var mipmap = texture.FirstMipmap;
            var outputBytes = mipmap.Bytes;
            
            // If mipmap is in raw pixels format, then convert into png
            if (!mipmap.IsImage)
            {
                var width = texture.Header.ImageWidth;
                var textureWidth = mipmap.Width;
                var height = texture.Header.ImageHeight;

                var bitmap = new Bitmap(width, height);

                switch (mipmap.Format)
                {
                    case MipmapFormat.RGBA8888:
                        Helper.CopyRawPixelsIntoBitmap(outputBytes, textureWidth, bitmap, true);
                        break;
                    case MipmapFormat.RG88:
                        Helper.CopyRawRG88PixelsIntoBitmap(outputBytes, textureWidth, bitmap);
                        break;
                    case MipmapFormat.R8:
                        Helper.CopyRawR8PixelsIntoBitmap(outputBytes, textureWidth, bitmap);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(
                            nameof(mipmap.Format),
                            mipmap.Format,
                            "This format is not valid here"
                        );
                }

                // set output bytes to png image
                var stream = new MemoryStream();
                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                bitmap.Dispose();
                outputBytes = stream.GetBuffer();
                stream.Close();
            }
            
            // save output
            var extension = "png";
            if (mipmap.IsImage)
                extension = mipmap.Format.GetFileExtension();
            
            using (var stream = File.Open($"{fileNameWithoutExtension}.{extension}",
                FileMode.Create,
                FileAccess.Write,
                FileShare.Read))
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write(outputBytes, 0, outputBytes.Length);
            }
        }
    }
}