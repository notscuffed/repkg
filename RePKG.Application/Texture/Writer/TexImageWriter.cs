using System;
using System.IO;
using RePKG.Core.Texture;

namespace RePKG.Application.Texture
{
    public class TexImageWriter : ITexImageWriter
    {
        public void WriteTo(BinaryWriter writer, TexImageContainerVersion containerVersion, ITexImage image)
        {
            if (writer == null) throw new ArgumentNullException(nameof(writer));
            if (image == null) throw new ArgumentNullException(nameof(image));

            var mipmapWriter = PickMipmapWriter(containerVersion);

            writer.Write(image.Mipmaps.Count);

            foreach (var mipmap in image.Mipmaps)
            {
                mipmapWriter(writer, mipmap);
            }
        }

        private static void WriteMipmapV1(BinaryWriter writer, ITexMipmap mipmap)
        {
            if (mipmap.IsLZ4Compressed)
                throw new InvalidOperationException(
                    $"Cannot write lz4 compressed mipmap when using tex container version: {TexImageContainerVersion.Version1}");

            writer.Write(mipmap.Width);
            writer.Write(mipmap.Height);

            using (var stream = mipmap.GetBytesStream())
            {
                writer.Write((int) stream.Length);
                writer.Flush();
                stream.CopyTo(writer.BaseStream);
            }
        }

        private static void WriteMipmapV2And3(BinaryWriter writer, ITexMipmap mipmap)
        {
            writer.Write(mipmap.Width);
            writer.Write(mipmap.Height);
            writer.Write(mipmap.IsLZ4Compressed ? 1 : 0);
            writer.Write(mipmap.DecompressedBytesCount);

            using (var stream = mipmap.GetBytesStream())
            {
                writer.Write((int) stream.Length);
                writer.Flush();
                stream.CopyTo(writer.BaseStream);
            }
        }

        private static Action<BinaryWriter, ITexMipmap> PickMipmapWriter(TexImageContainerVersion containerVersion)
        {
            switch (containerVersion)
            {
                case TexImageContainerVersion.Version1:
                    return WriteMipmapV1;

                case TexImageContainerVersion.Version2:
                case TexImageContainerVersion.Version3:
                    return WriteMipmapV2And3;

                default:
                    throw new ArgumentOutOfRangeException(nameof(containerVersion));
            }
        }
    }
}