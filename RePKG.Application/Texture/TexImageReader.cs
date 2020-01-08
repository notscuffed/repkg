using System;
using System.IO;
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

        public TexImage ReadFrom(BinaryReader reader, Tex tex)
        {
            var version = tex.ImagesContainer.ImageContainerVersion;

            var image = new TexImage(reader.ReadInt32());

            Func<BinaryReader, TexMipmap> mipmapReader;
            switch (version)
            {
                case TexImageContainerVersion.Version1:
                    mipmapReader = ReadMipmapV1;
                    break;
                case TexImageContainerVersion.Version2:
                case TexImageContainerVersion.Version3:
                    mipmapReader = ReadMipmapV2And3;
                    break;
                default:
                    throw new NotImplementedException($"Tex image container version: {version} is not supported!");
            }

            var format = TexMipmapFormatGetter.GetFormatForTex(tex);
            for (var i = 0; i < image.MipmapCount; i++)
            {
                var mipmap = mipmapReader(reader);
                mipmap.Format = format;
                ReadBytes(reader, mipmap);

                image.Mipmaps[i] = mipmap;
            }

            return image;
        }

        private static TexMipmap ReadMipmapV1(BinaryReader reader)
        {
            return new TexMipmap()
            {
                Width = reader.ReadInt32(),
                Height = reader.ReadInt32(),
                BytesCount = reader.ReadInt32(),
            };
        }

        private static TexMipmap ReadMipmapV2And3(BinaryReader reader)
        {
            return new TexMipmap()
            {
                Width = reader.ReadInt32(),
                Height = reader.ReadInt32(),
                IsLZ4Compressed = reader.ReadInt32() == 1,
                DecompressedBytesCount = reader.ReadInt32(),
                BytesCount = reader.ReadInt32()
            };
        }

        private void ReadBytes(BinaryReader reader, TexMipmap mipmap)
        {
            if (!ReadMipmapBytes)
            {
                reader.BaseStream.Seek(mipmap.BytesCount, SeekOrigin.Current);
                return;
            }

            mipmap.Bytes = new byte[mipmap.BytesCount];

            var bytesRead = reader.Read(mipmap.Bytes, 0, mipmap.BytesCount);

            if (bytesRead != mipmap.BytesCount)
                throw new Exception("Failed to read bytes from stream while reading mipmap");

            if (DecompressMipmapBytes)
                _texMipmapDecompressor.DecompressMipmap(mipmap);
        }
    }
}