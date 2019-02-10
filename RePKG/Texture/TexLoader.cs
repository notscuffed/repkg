using System;
using System.IO;
using System.Text;
using K4os.Compression.LZ4;

namespace RePKG.Texture
{
    public static class TexLoader
    {
        public static byte[] DecompressLZ4(byte[] input, int knownLength)
        {
            var stream = new MemoryStream(input);
            var buffer = new byte[knownLength];

            var result = LZ4Codec.Decode(
                input, 0, input.Length,
                buffer, 0, knownLength);

            return buffer;
        }

        public static Tex LoadTex(byte[] bytes)
        {
            var stream = new MemoryStream(bytes);
            var reader = new BinaryReader(stream, Encoding.ASCII);
            Tex tex;

            try
            {
                tex = new Tex();
                tex.Magic = reader.ReadNString();

                if (tex.Magic != "TEXV0005")
                    throw new InvalidTexHeaderMagic("TEXV0005", tex.Magic);

                tex.Magic2 = reader.ReadNString();

                if (tex.Magic2 != "TEXI0001")
                    throw new InvalidTexHeaderMagic("TEXI0001", tex.Magic2);

                tex.DxtCompression = reader.ReadInt32();
                tex._unkInt_1 = reader.ReadInt32();
                tex.TextureWidth = reader.ReadInt32();
                tex.TextureHeight = reader.ReadInt32();
                tex.ImageWidth = reader.ReadInt32();
                tex.ImageHeight = reader.ReadInt32();
                tex._unkInt_2 = reader.ReadUInt32(); // some checksum or something

                // mipmap header
                tex.TextureContainerMagic = reader.ReadNString();

                if (tex.TextureContainerMagic == "TEXB0003")
                {
                    tex.TextureContainerVersion = TexMipmapVersion.Version3;
                    tex._unkIntCont_0 = reader.ReadInt32();
                    tex.ImageFormat = (FreeImageFormat) reader.ReadInt32();
                }
                else if (tex.TextureContainerMagic == "TEXB0002")
                {
                    tex.TextureContainerVersion = TexMipmapVersion.Version2;
                    tex._unkIntCont_0 = reader.ReadInt32();
                }
                else
                    throw new InvalidTexHeaderMagic("TEXB0003/TEXB0002", tex.TextureContainerMagic);

                tex.MipmapCount = reader.ReadInt32();

                for (var i = 0; i < tex.MipmapCount; i++)
                {
                    tex.Mipmaps.Add(ReadMipmap(reader));
                }
            }
            finally
            {
                reader.Close();
            }

            return tex;
        }

        private static TexMipmap ReadMipmap(BinaryReader reader)
        {
            var mipmap = new TexMipmap();
            mipmap.Width = reader.ReadInt32();
            mipmap.Height = reader.ReadInt32();
            mipmap.LZ4Compressed = reader.ReadInt32();
            mipmap.PixelCount = reader.ReadInt32();
            mipmap.BytesCount = reader.ReadInt32();
            mipmap.Bytes = new byte[mipmap.BytesCount];
            reader.Read(mipmap.Bytes, 0, mipmap.BytesCount);

            return mipmap;
        }
    }
}
