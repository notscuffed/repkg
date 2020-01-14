using System;
using K4os.Compression.LZ4;
using RePKG.Application.Texture.Helpers;
using RePKG.Core.Texture;

namespace RePKG.Application.Texture
{
    public class TexMipmapDecompressor : ITexMipmapDecompressor
    {
        public void DecompressMipmap(ITexMipmap mipmap)
        {
            if (mipmap == null) throw new ArgumentNullException(nameof(mipmap));

            if (mipmap.IsLZ4Compressed)
            {
                mipmap.Bytes = Lz4Decompress(mipmap.Bytes, mipmap.DecompressedBytesCount);
                mipmap.IsLZ4Compressed = false;
            }

            if (mipmap.Format.IsImage())
                return;

            switch (mipmap.Format)
            {
                case MipmapFormat.CompressedDXT5:
                    mipmap.Bytes = DXT.DecompressImage(mipmap.Width, mipmap.Height, mipmap.Bytes, DXTFlags.DXT5);
                    mipmap.Format = MipmapFormat.RGBA8888;
                    break;
                case MipmapFormat.CompressedDXT3:
                    mipmap.Bytes = DXT.DecompressImage(mipmap.Width, mipmap.Height, mipmap.Bytes, DXTFlags.DXT3);
                    mipmap.Format = MipmapFormat.RGBA8888;
                    break;
                case MipmapFormat.CompressedDXT1:
                    mipmap.Bytes = DXT.DecompressImage(mipmap.Width, mipmap.Height, mipmap.Bytes, DXTFlags.DXT1);
                    mipmap.Format = MipmapFormat.RGBA8888;
                    break;
            }
        }

        private byte[] Lz4Decompress(byte[] bytes, int knownLength)
        {
            var buffer = new byte[knownLength];

            LZ4Codec.Decode(
                bytes, 0, bytes.Length,
                buffer, 0, buffer.Length);

            return buffer;
        }
    }
}