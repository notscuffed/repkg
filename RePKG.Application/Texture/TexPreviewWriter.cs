using System;
using System.Drawing;
using System.IO;
using RePKG.Core.Texture;

namespace RePKG.Application.Texture
{
    public static class TexPreviewWriter
    {
        public static void WriteTexture(Tex texture, string fileNameWithoutExtension, bool overwrite = true)
        {
            var mipmap = texture.FirstImage.FirstMipmap;
            var outputBytes = mipmap.Bytes;

            var extension = "png";
            if (mipmap.IsImage)
                extension = mipmap.Format.GetFileExtension();

            var filePath = $"{fileNameWithoutExtension}.{extension}";

            if (!overwrite && File.Exists(filePath))
                return;
            
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
                        BitmapHelper.CopyRawPixelsIntoBitmap(outputBytes, textureWidth, bitmap);
                        break;
                    case MipmapFormat.RG88:
                        BitmapHelper.CopyRawRG88PixelsIntoBitmap(outputBytes, textureWidth, bitmap);
                        break;
                    case MipmapFormat.R8:
                        BitmapHelper.CopyRawR8PixelsIntoBitmap(outputBytes, textureWidth, bitmap);
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
            
            
            using (var stream = File.Open(filePath,
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