using System;
using K4os.Compression.LZ4;
using RePKG.Core.Texture;

namespace RePKG.Application.Texture
{
    public class TexMipmapCompressor : ITexMipmapCompressor
    {
        public void CompressMipmap(
            ITexMipmap mipmap,
            MipmapFormat targetCompressFormat,
            bool lz4Compress)
        {
            if (mipmap == null) throw new ArgumentNullException(nameof(mipmap));
            if (mipmap.IsLZ4Compressed) throw new InvalidOperationException("Mipmap is already compressed using LZ4");

            if (targetCompressFormat != mipmap.Format)
                throw new NotSupportedException("Changing mipmap format is not yet supported");
            
            if (lz4Compress) LZ4Compress(mipmap);
        }

        private static void LZ4Compress(ITexMipmap mipmap)
        {
            var bytes = mipmap.Bytes;
            var maximumSize = LZ4Codec.MaximumOutputSize(bytes.Length);
            var buffer = new byte[maximumSize];

            var compressedSize = LZ4Codec.Encode(
                bytes, 0, bytes.Length,
                buffer, 0, buffer.Length);
            
            if (compressedSize < maximumSize)
                Array.Resize(ref buffer, compressedSize);
            
            mipmap.DecompressedBytesCount = bytes.Length;
            mipmap.Bytes = buffer;
            mipmap.IsLZ4Compressed = true;
        }
    }
}