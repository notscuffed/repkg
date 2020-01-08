using System;
using System.IO;
using System.Text;
using RePKG.Application.Exceptions;
using RePKG.Core.Texture;

namespace RePKG.Application.Texture
{
    public class TexImageContainerWriter : ITexImageContainerWriter
    {
        private readonly ITexImageWriter _texImageWriter;

        public TexImageContainerWriter(ITexImageWriter texImageWriter)
        {
            _texImageWriter = texImageWriter;
        }

        public void WriteToStream(TexImageContainer imageContainer, Stream stream)
        {
            if (imageContainer == null) throw new ArgumentNullException(nameof(imageContainer));
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            
            using (var writer = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                writer.WriteNString(imageContainer.Magic);

                switch (imageContainer.Magic)
                {
                    case "TEXB0001":
                        WriteV1(imageContainer, writer);
                        break;

                    case "TEXB0002":
                        WriteV2(imageContainer, writer);
                        break;

                    case "TEXB0003":
                        WriteV3(imageContainer, writer);
                        break;

                    default:
                        throw new UnknownTexImageContainerMagicException(imageContainer.Magic);
                }
            }
        }

        public void WriteImagesToStream(Tex tex, Stream stream)
        {
            foreach (var image in tex.ImagesContainer.Images)
            {
                _texImageWriter.WriteToStream(tex, image, stream);
            }
        }

        private static void WriteV1(TexImageContainer container, BinaryWriter writer)
        {
            writer.Write(container.Images.Count);
        }

        private static void WriteV2(TexImageContainer container, BinaryWriter writer)
        {
            writer.Write(container.Images.Count);
        }

        private static void WriteV3(TexImageContainer container, BinaryWriter writer)
        {
            writer.Write(container.Images.Count);
            writer.Write((int) container.ImageFormat);
        }
    }
}