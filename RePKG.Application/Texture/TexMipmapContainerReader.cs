using System.IO;
using System.Text;
using RePKG.Application.Exceptions;
using RePKG.Core.Texture;

namespace RePKG.Application.Texture
{
    public class TexMipmapContainerReader : ITexMipmapContainerReader
    {
        private readonly ITexMipmapReader _texMipmapReader;

        public TexMipmapContainerReader(ITexMipmapReader texMipmapReader)
        {
            _texMipmapReader = texMipmapReader;
        }

        public TexMipmapContainer ReadFromStream(Stream stream)
        {
            using (var reader = new BinaryReader(stream, Encoding.UTF8, true))
            {
                var magic = reader.ReadNString(16);

                TexMipmapContainer container;

                switch (magic)
                {
                    case "TEXB0001":
                        container = ReadV1(reader);
                        break;

                    case "TEXB0002":
                        container = ReadV2(reader);
                        break;

                    case "TEXB0003":
                        container = ReadV3(reader);
                        break;

                    default:
                        throw new UnknownTexMipmapContainerMagicException(magic);
                }

                container.ImageFormat.AssertValid();

                return container;
            }
        }

        public void ReadMipmapsFromStream(Stream stream, Tex tex)
        {
            for (var i = 0; i < tex.MipmapsContainer.MipmapCount; i++)
            {
                var mipmap = _texMipmapReader.ReadFromStream(stream, tex);
                tex.MipmapsContainer.Mipmaps.Add(mipmap);
            }
        }

        private static TexMipmapContainer ReadV1(BinaryReader reader)
        {
            return new TexMipmapContainer
            {
                MipmapContainerVersion = TexMipmapContainerVersion.Version1,
                Magic = "TEXB0001",
                UnkIntCont0 = reader.ReadInt32(),
                MipmapCount = reader.ReadInt32()
            };
        }

        private static TexMipmapContainer ReadV2(BinaryReader reader)
        {
            return new TexMipmapContainer
            {
                MipmapContainerVersion = TexMipmapContainerVersion.Version2,
                Magic = "TEXB0002",
                UnkIntCont0 = reader.ReadInt32(),
                MipmapCount = reader.ReadInt32()
            };
        }

        private static TexMipmapContainer ReadV3(BinaryReader reader)
        {
            return new TexMipmapContainer
            {
                MipmapContainerVersion = TexMipmapContainerVersion.Version3,
                Magic = "TEXB0003",
                UnkIntCont0 = reader.ReadInt32(),
                ImageFormat = (FreeImageFormat) reader.ReadInt32(),
                MipmapCount = reader.ReadInt32()
            };
        }
    }
}