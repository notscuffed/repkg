using System;
using System.IO;
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

        public void WriteTo(BinaryWriter writer, TexImageContainer imageContainer)
        {
            if (writer == null) throw new ArgumentNullException(nameof(writer));
            if (imageContainer == null) throw new ArgumentNullException(nameof(imageContainer));
            
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
                    throw new UnknownMagicException(nameof(TexImageContainerWriter), imageContainer.Magic);
            }
        }

        public void WriteImagesTo(BinaryWriter writer, Tex tex)
        {
            foreach (var image in tex.ImagesContainer.Images)
            {
                _texImageWriter.WriteTo(writer, tex, image);
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