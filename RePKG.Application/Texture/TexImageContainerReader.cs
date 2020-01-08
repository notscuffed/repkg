using System.IO;
using System.Text;
using RePKG.Application.Exceptions;
using RePKG.Core.Texture;

namespace RePKG.Application.Texture
{
    public class TexImageContainerReader : ITexImageContainerReader
    {
        private readonly ITexImageReader _texImageReader;

        public TexImageContainerReader(ITexImageReader texImageReader)
        {
            _texImageReader = texImageReader;
        }

        public TexImageContainer ReadFromStream(Stream stream)
        {
            using (var reader = new BinaryReader(stream, Encoding.UTF8, true))
            {
                var magic = reader.ReadNString(16);

                TexImageContainer container;

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
                        throw new UnknownTexImageContainerMagicException(magic);
                }

                container.ImageFormat.AssertValid();

                return container;
            }
        }

        public void ReadImagesFromStream(Stream stream, Tex tex)
        {
            for (var i = 0; i < tex.ImagesContainer.ImageCount; i++)
            {
                var image = _texImageReader.ReadFromStream(stream, tex);
                tex.ImagesContainer.Images.Add(image);
            }
        }

        private static TexImageContainer ReadV1(BinaryReader reader)
        {
            return new TexImageContainer
            {
                ImageContainerVersion = TexImageContainerVersion.Version1,
                Magic = "TEXB0001",
                ImageCount = reader.ReadInt32()
            };
        }

        private static TexImageContainer ReadV2(BinaryReader reader)
        {
            return new TexImageContainer
            {
                ImageContainerVersion = TexImageContainerVersion.Version2,
                Magic = "TEXB0002",
                ImageCount = reader.ReadInt32()
            };
        }

        private static TexImageContainer ReadV3(BinaryReader reader)
        {
            return new TexImageContainer
            {
                ImageContainerVersion = TexImageContainerVersion.Version3,
                Magic = "TEXB0003",
                ImageCount = reader.ReadInt32(),
                ImageFormat = (FreeImageFormat) reader.ReadInt32()
            };
        }
    }
}