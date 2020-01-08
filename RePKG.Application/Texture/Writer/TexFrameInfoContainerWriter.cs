using System;
using System.IO;
using System.Text;
using RePKG.Application.Exceptions;
using RePKG.Core.Texture;

namespace RePKG.Application.Texture
{
    public class TexFrameInfoContainerWriter : ITexFrameInfoContainerWriter
    {
        public void WriteToStream(TexFrameInfoContainer frameInfoContainer, Stream stream)
        {
            if (frameInfoContainer == null) throw new ArgumentNullException(nameof(frameInfoContainer));
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            using (var writer = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                writer.WriteNString(frameInfoContainer.Magic);
                writer.Write(frameInfoContainer.Frames.Length);
                
                switch (frameInfoContainer.Magic)
                {
                    case "TEXS0002":
                        WriteV2(frameInfoContainer, writer);
                        break;

                    case "TEXS0003":
                        WriteV3(frameInfoContainer, writer);
                        break;

                    default:
                        throw new UnknownTexFrameInfoContainerMagicException(frameInfoContainer.Magic);
                }
            }
        }

        private static void WriteV2(TexFrameInfoContainer container, BinaryWriter writer)
        {
            WriteFrames(container, writer);
        }

        private static void WriteV3(TexFrameInfoContainer container, BinaryWriter writer)
        {
            writer.Write(container.Unk0);
            writer.Write(container.Unk1);

            WriteFrames(container, writer);
        }

        private static void WriteFrames(TexFrameInfoContainer container, BinaryWriter writer)
        {
            foreach (var frame in container.Frames)
            {
                writer.Write(frame.ImageId);
                writer.Write(frame.Frametime);
                writer.Write(frame.X);
                writer.Write(frame.Y);
                writer.Write(frame.Width);
                writer.Write(frame.Unk0);
                writer.Write(frame.Unk1);
                writer.Write(frame.Height);
            }
        }
    }
}