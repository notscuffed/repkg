using System;
using System.IO;
using RePKG.Application.Exceptions;
using RePKG.Core.Texture;

namespace RePKG.Application.Texture
{
    public class TexFrameInfoContainerReader : ITexFrameInfoContainerReader
    {
        public ITexFrameInfoContainer ReadFrom(BinaryReader reader)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            
            var container = new TexFrameInfoContainer
            {
                Magic = reader.ReadNString(maxLength: 16)
            };

            var frameCount = reader.ReadInt32();

            if (frameCount > Constants.MaximumFrameCount)
                throw new UnsafeTexException($"Frame count exceeds limit: {frameCount}/{Constants.MaximumFrameCount}");

            switch (container.Magic)
            {
                case "TEXS0002":
                    break;

                case "TEXS0003":
                    container.GifWidth = reader.ReadInt32();
                    container.GifHeight = reader.ReadInt32();
                    break;

                default:
                    throw new UnknownMagicException(nameof(TexFrameInfoContainerReader), container.Magic);
            }

            for (var i = 0; i < frameCount; i++)
            {
                container.Frames.Add(new TexFrameInfo
                {
                    ImageId = reader.ReadInt32(),
                    Frametime = reader.ReadSingle(),
                    X = reader.ReadSingle(),
                    Y = reader.ReadSingle(),
                    Width = reader.ReadSingle(),
                    Unk0 = reader.ReadSingle(),
                    Unk1 = reader.ReadSingle(),
                    Height = reader.ReadSingle(),
                });
            }

            // TEXS0002 doesn't save gif width/height so we will get it from first frame
            // Because we use those values in TexToImageConverter
            if (container.GifWidth == 0 ||
                container.GifHeight == 0)
            {
                container.GifWidth = (int) container.Frames[0].Width;
                container.GifHeight = (int) container.Frames[0].Height;
            }

            return container;
        }
    }
}