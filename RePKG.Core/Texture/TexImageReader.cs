using RePKG.Core.Exceptions;
using RePKG.Core.Texture.Interfaces;

namespace RePKG.Core.Texture
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
            ArgumentNullException.ThrowIfNull(reader);
            ArgumentNullException.ThrowIfNull(container);

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

        private TexMipmap ReadMipmapV4(BinaryReader reader)
        {
            /**FIXME
             * The role of the following param* parameters cannot be confirmed,
             * it may be a parameter used in the built-in display of the wallpaper editor and does not need to be processed
             */
            var param1 = reader.ReadInt32();
            if (param1 != 1)
            {
                throw new UnsafeTexException($"ReadMipmapV4 unknow param1 :{param1}");
            }
            var param2 = reader.ReadInt32();
            if (param2 != 2)
            {
                throw new UnsafeTexException($"ReadMipmapV4 unknow param2 :{param2}");
            }
            var conditionJson = reader.ReadNString();

            var param3 = reader.ReadInt32();
            if (param3 != 1)
            {
                throw new UnsafeTexException($"ReadMipmapV4 unknow param3 :{param3}");
            }
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
            reader.BaseStream.ReadExactly(bytes);
            return bytes;
        }

        private Func<BinaryReader, TexMipmap> PickMipmapReader(TexImageContainerVersion containerVersion)
        {
            return containerVersion switch
            {
                TexImageContainerVersion.Version1 => ReadMipmapV1,
                TexImageContainerVersion.Version2 or TexImageContainerVersion.Version3 => ReadMipmapV2And3,
                TexImageContainerVersion.Version4 => ReadMipmapV4,
                _ => throw new InvalidOperationException(
                                        $"Tex image container version: {containerVersion} is not supported!"),
            };
        }
    }
}