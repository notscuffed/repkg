using System;
using System.IO;
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

        public ITexImageContainer ReadFrom(BinaryReader reader, TexFormat texFormat)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));

            if (!texFormat.IsValid())
                throw new EnumNotValidException<TexFormat>(texFormat);

            var container = new TexImageContainer
            {
                Magic = reader.ReadNString(maxLength: 16)
            };

            var imageCount = reader.ReadInt32();

            if (imageCount > Constants.MaximumImageCount)
                throw new UnsafeTexException(
                    $"Image count exceeds limit: {imageCount}/{Constants.MaximumImageCount}");

            switch (container.Magic)
            {
                case "TEXB0001":
                case "TEXB0002":
                    break;

                case "TEXB0003":
                    container.ImageFormat = (FreeImageFormat) reader.ReadInt32();
                    break;

                default:
                    throw new UnknownMagicException(nameof(TexImageContainerReader), container.Magic);
            }

            container.ImageContainerVersion = (TexImageContainerVersion) Convert.ToInt32(container.Magic.Substring(4));
            
            if (!container.ImageFormat.IsValid())
                throw new EnumNotValidException<FreeImageFormat>(container.ImageFormat);

            for (var i = 0; i < imageCount; i++)
            {
                container.Images.Add(_texImageReader.ReadFrom(reader, container, texFormat));
            }

            return container;
        }
    }
}