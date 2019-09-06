using System;
using System.IO;
using System.Text;
using K4os.Compression.LZ4;
using RePKG.Application.Exceptions;
using RePKG.Core.Texture;

namespace RePKG.Texture
{
    public static class TexLoader
    {
        public static byte[] DecompressLz4(byte[] input, int knownLength)
        {
            var buffer = new byte[knownLength];

            LZ4Codec.Decode(
                input, 0, input.Length,
                buffer, 0, knownLength);

            return buffer;
        }

        public static Tex LoadTex(byte[] bytes, int maxMipmapsToLoad = -1)
        {
            var stream = new MemoryStream(bytes);
            var reader = new BinaryReader(stream, Encoding.ASCII);
            Tex tex;

            try
            {
                tex = new Tex {Magic = reader.ReadNString()};

                if (tex.Magic != "TEXV0005")
                    throw new UnknownTexHeaderMagicException("TEXV0005", tex.Magic);

                tex.Magic2 = reader.ReadNString();

                if (tex.Magic2 != "TEXI0001")
                    throw new UnknownTexHeaderMagicException("TEXI0001", tex.Magic2);

                tex.FormatId = reader.ReadInt32();
                switch (tex.FormatId)
                {
                    case 0:
                        tex.Format = TexFormat.ARGB8888;
                        break;
                    case 4:
                        tex.Format = TexFormat.DXT5;
                        break;
                    case 6:
                        tex.Format = TexFormat.DXT3;
                        break;
                    case 7:
                        tex.Format = TexFormat.DXT1;
                        break;
                    case 8:
                        tex.Format = TexFormat.RG88;
                        break;
                    case 9:
                        tex.Format = TexFormat.R8;
                        break;
                    default:
                        throw new Exception(
                            $"Unknown tex format id: {tex.FormatId} for {tex.TextureContainerMagic}");
                }
                
                tex.Flags = (TexFlags) reader.ReadInt32();
                tex.TextureWidth = reader.ReadInt32();
                tex.TextureHeight = reader.ReadInt32();
                tex.ImageWidth = reader.ReadInt32();
                tex.ImageHeight = reader.ReadInt32();
                tex.UnkInt0 = reader.ReadUInt32();

                // mipmap header
                tex.TextureContainerMagic = reader.ReadNString();
                
                if (tex.TextureContainerMagic == "TEXB0003")
                {
                    tex.TextureContainerVersion = TexMipmapVersion.Version3;
                    tex.UnkIntCont0 = reader.ReadInt32();
                    tex.ImageFormat = (FreeImageFormat) reader.ReadInt32();
                }
                else if (tex.TextureContainerMagic == "TEXB0002")
                {
                    tex.TextureContainerVersion = TexMipmapVersion.Version2;
                    tex.UnkIntCont0 = reader.ReadInt32();
                }
                else if (tex.TextureContainerMagic == "TEXB0001")
                {
                    tex.TextureContainerVersion = TexMipmapVersion.Version1;
                    tex.UnkIntCont0 = reader.ReadInt32();
                }
                else
                    throw new UnknownTexHeaderMagicException("TEXB0001/TEXB0002/TEXB0003", tex.TextureContainerMagic);

                tex.MipmapCount = reader.ReadInt32();

                if (maxMipmapsToLoad == 0)
                    return tex;

                var mipmapCount = tex.MipmapCount;
                if (maxMipmapsToLoad > 0)
                    mipmapCount = Math.Min(maxMipmapsToLoad, mipmapCount);

                for (var i = 0; i < mipmapCount; i++)
                {
                    tex.Mipmaps.Add(ReadMipmap(reader, tex.TextureContainerVersion));
                }
            }
            finally
            {
                reader.Close();
            }

            return tex;
        }

        private static TexMipmap ReadMipmap(BinaryReader reader, TexMipmapVersion version)
        {
            var mipmap = new TexMipmap();

            if (version == TexMipmapVersion.Version1)
            {
                mipmap.Width = reader.ReadInt32();
                mipmap.Height = reader.ReadInt32();
                mipmap.BytesCount = reader.ReadInt32();
            }
            else
            {
                mipmap.Width = reader.ReadInt32();
                mipmap.Height = reader.ReadInt32();
                mipmap.Lz4Compressed = reader.ReadInt32();
                mipmap.PixelCount = reader.ReadInt32();
                mipmap.BytesCount = reader.ReadInt32();
            }

            mipmap.Bytes = new byte[mipmap.BytesCount];
            reader.Read(mipmap.Bytes, 0, mipmap.BytesCount);

            return mipmap;
        }
    }
}