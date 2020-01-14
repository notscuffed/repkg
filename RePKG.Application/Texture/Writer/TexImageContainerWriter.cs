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

        public void WriteTo(BinaryWriter writer, ITexImageContainer imageContainer)
        {
            if (writer == null) throw new ArgumentNullException(nameof(writer));
            if (imageContainer == null) throw new ArgumentNullException(nameof(imageContainer));
            
            writer.WriteNString(imageContainer.Magic);
            writer.Write(imageContainer.Images.Count);

            switch (imageContainer.ImageContainerVersion)
            {
                case TexImageContainerVersion.Version1:
                case TexImageContainerVersion.Version2:
                    break;

                case TexImageContainerVersion.Version3:
                    writer.Write((int) imageContainer.ImageFormat);
                    break;

                default:
                    throw new UnknownMagicException(nameof(TexImageContainerWriter), imageContainer.Magic);
            }
            
            foreach (var image in imageContainer.Images)
            {
                _texImageWriter.WriteTo(writer, imageContainer.ImageContainerVersion, image);
            }
        }
    }
}