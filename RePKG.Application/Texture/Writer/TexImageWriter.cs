using System;
using System.IO;
using RePKG.Core.Texture;

namespace RePKG.Application.Texture
{
    public class TexImageWriter : ITexImageWriter
    {
        public void WriteTo(BinaryWriter writer, Tex tex, TexImage image)
        {
            if (writer == null) throw new ArgumentNullException(nameof(writer));
            if (tex == null) throw new ArgumentNullException(nameof(tex));
            if (image == null) throw new ArgumentNullException(nameof(image));

            Action<BinaryWriter, TexMipmap> mipmapWriter;

            switch (tex.ImagesContainer.ImageContainerVersion)
            {
                case TexImageContainerVersion.Version1:
                    mipmapWriter = WriteMipmapV1;
                    break;
                case TexImageContainerVersion.Version2:
                case TexImageContainerVersion.Version3:
                    mipmapWriter = WriteMipmapV2And3;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            writer.Write(image.Mipmaps.Length);

            foreach (var mipmap in image.Mipmaps)
            {
                mipmapWriter(writer, mipmap);
            }
        }

        private static void WriteMipmapV1(BinaryWriter writer, TexMipmap mipmap)
        {
            if (mipmap.IsLZ4Compressed)
                throw new InvalidOperationException(
                    $"Cannot write lz4 compressed mipmap when using tex container version: {TexImageContainerVersion.Version1}");

            writer.Write(mipmap.Width);
            writer.Write(mipmap.Height);
            writer.Write(mipmap.BytesCount);
            writer.Write(mipmap.Bytes);
        }

        private static void WriteMipmapV2And3(BinaryWriter writer, TexMipmap mipmap)
        {
            writer.Write(mipmap.Width);
            writer.Write(mipmap.Height);
            writer.Write(mipmap.IsLZ4Compressed ? 1 : 0);
            writer.Write(mipmap.DecompressedBytesCount);
            writer.Write(mipmap.BytesCount);
            writer.Write(mipmap.Bytes);
        }
    }
}