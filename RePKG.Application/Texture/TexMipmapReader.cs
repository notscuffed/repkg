using System;
using System.IO;
using System.Text;
using RePKG.Core.Texture;

namespace RePKG.Application.Texture
{
    public class TexMipmapReader : ITexMipmapReader
    {
        protected readonly ITexMipmapDecompressor _texMipmapDecompressor;
        public bool ReadMipmapBytes { get; set; } = true;
        public bool DecompressMipmapBytes { get; set; } = true;

        public TexMipmapReader(ITexMipmapDecompressor texMipmapDecompressor)
        {
            _texMipmapDecompressor = texMipmapDecompressor;
        }

        public TexMipmap ReadFromStream(Stream stream, Tex tex)
        {
            using (var reader = new BinaryReader(stream, Encoding.UTF8, true))
            {
                var version = tex.MipmapsContainer.MipmapContainerVersion;
                TexMipmap mipmap;

                switch (version)
                {
                    case TexMipmapContainerVersion.Version1:
                        mipmap = new TexMipmap()
                        {
                            Width = reader.ReadInt32(),
                            Height = reader.ReadInt32(),
                            BytesCount = reader.ReadInt32(),
                        };
                        break;
                    case TexMipmapContainerVersion.Version2:
                    case TexMipmapContainerVersion.Version3:
                        mipmap = new TexMipmap()
                        {
                            Width = reader.ReadInt32(),
                            Height = reader.ReadInt32(),
                            IsLZ4Compressed = reader.ReadInt32() == 1,
                            DecompressedBytesCount = reader.ReadInt32(),
                            BytesCount = reader.ReadInt32()
                        };

                        break;
                    default:
                        throw new NotImplementedException($"Tex mipmap container version: {version} is not supported!");
                }

                mipmap.Format = TexMipmapFormatGetter.GetFormatForTex(tex);

                ReadBytes(reader, mipmap);
                
                if (tex.IsGif)
                    mipmap.Unk0 = reader.ReadInt32();

                return mipmap;
            }
        }

        protected void ReadBytes(BinaryReader reader, TexMipmap mipmap)
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