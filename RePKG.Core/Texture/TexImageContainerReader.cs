using RePKG.Core.Exceptions;

namespace RePKG.Core.Texture
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
            ArgumentNullException.ThrowIfNull(reader);

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
                    container.ImageFormat = (FreeImageFormat)reader.ReadInt32();
                    break;

                case "TEXB0004":
                    var format = (FreeImageFormat)reader.ReadInt32();
                    var isVideoMp4 = reader.ReadInt32() == 1;
                    if (format == FreeImageFormat.FIF_UNKNOWN)
                    {
                        if (isVideoMp4)
                        {
                            format = FreeImageFormat.FIF_MP4;
                        }
                    }
                    container.ImageFormat = format;
                    break;

                default:
                    throw new UnknownMagicException(nameof(TexImageContainerReader), container.Magic);
            }

            int version = Convert.ToInt32(container.Magic[4..]);
            container.ImageContainerVersion = (TexImageContainerVersion)version;

            if (container.ImageContainerVersion == TexImageContainerVersion.Version4
                && container.ImageFormat != FreeImageFormat.FIF_MP4)
            {
                container.ImageContainerVersion = TexImageContainerVersion.Version3;
            }

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