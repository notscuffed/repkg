using System;
using System.IO;
using RePKG.Application.Exceptions;
using RePKG.Core.Texture;

namespace RePKG.Application.Texture
{
    public class TexImageReader : ITexImageReader
    {
        protected readonly ITexMipmapDecompressor _texMipmapDecompressor;
        public bool ReadMipmapBytes { get; set; } = true;
        public bool DecompressMipmapBytes { get; set; } = true;

        public TexImageReader(ITexMipmapDecompressor texMipmapDecompressor)
        {
            _texMipmapDecompressor = texMipmapDecompressor;
        }

        public ITexImage ReadFrom(
            BinaryReader reader,
            ITexImageContainer container,
            TexFormat texFormat)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            if (container == null) throw new ArgumentNullException(nameof(container));
            
            if (!texFormat.IsValid())
                throw new EnumNotValidException<TexFormat>(texFormat);

            var mipmapCount = reader.ReadInt32();

            if (mipmapCount > Constants.MaximumMipmapCount)
                throw new UnsafeTexException(
                    $"Mipmap count exceeds limit: {mipmapCount}/{Constants.MaximumMipmapCount}");
            
            var readFunction = PickMipmapReader(container.ImageContainerVersion);
            var format = TexMipmapFormatGetter.GetFormatForTex(container.ImageFormat, texFormat);
            var image = new TexImage();
            
            for (var i = 0; i < mipmapCount; i++)
            {
                var mipmap = readFunction(reader);
                mipmap.Format = format;

                if (DecompressMipmapBytes)
                    _texMipmapDecompressor.DecompressMipmap(mipmap);

                image.Mipmaps.Add(mipmap);
            }

            return image;
        }

        private TexMipmap ReadMipmapV1(BinaryReader reader)
        {
            return new TexMipmap
            {
                Width = reader.ReadInt32(),
                Height = reader.ReadInt32(),
                Bytes = ReadBytes(reader)
            };
        }

        private TexMipmap ReadMipmapV2And3(BinaryReader reader)
        {
            return new TexMipmap
            {
                Width = reader.ReadInt32(),
                Height = reader.ReadInt32(),
                IsLZ4Compressed = reader.ReadInt32() == 1,
                DecompressedBytesCount = reader.ReadInt32(),
                Bytes = ReadBytes(reader)
            };
        }

        private byte[] ReadBytes(BinaryReader reader)
        {
            var byteCount = reader.ReadInt32();
            
            if (reader.BaseStream.Position + byteCount > reader.BaseStream.Length)
                throw new UnsafeTexException("Detected invalid mipmap byte count - exceeds stream length");

            if (byteCount > Constants.MaximumMipmapByteCount)
                throw new UnsafeTexException(
                    $"Mipmap byte count exceeds maximum size: {byteCount}/{Constants.MaximumMipmapByteCount}");

            if (!ReadMipmapBytes)
            {
                reader.BaseStream.Seek(byteCount, SeekOrigin.Current);
                return null;
            }

            var bytes = new byte[byteCount];
            var bytesRead = reader.Read(bytes, 0, byteCount);

            if (bytesRead != byteCount)
                throw new Exception("Failed to read bytes from stream while reading mipmap");

            return bytes;
        }

        private Func<BinaryReader, TexMipmap> PickMipmapReader(TexImageContainerVersion containerVersion)
        {
            switch (containerVersion)
            {
                case TexImageContainerVersion.Version1:
                    return ReadMipmapV1;

                case TexImageContainerVersion.Version2:
                case TexImageContainerVersion.Version3:
                    return ReadMipmapV2And3;

                default:
                    throw new InvalidOperationException(
                        $"Tex image container version: {containerVersion} is not supported!");
            }
        }
    }
}