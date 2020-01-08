using System.IO;
using System.Text;
using RePKG.Application.Exceptions;
using RePKG.Core.Texture;

namespace RePKG.Application.Texture
{
    public class TexFrameInfoReader : ITexFrameInfoReader
    {
        public TexFrameInfoContainer ReadFromStream(Stream stream)
        {
            using (var reader = new BinaryReader(stream, Encoding.UTF8, true))
            {
                var container = new TexFrameInfoContainer
                {
                    Magic = reader.ReadNString(maxLength: 16),
                    FrameCount = reader.ReadInt32()
                };
                
                container.Frames = new TexFrameInfo[container.FrameCount];

                switch (container.Magic)
                {
                    case "TEXS0002":
                        ReadV2(container, reader);
                        break;
                    
                    case "TEXS0003":
                        ReadV3(container, reader);
                        break;
                    
                    default:
                        throw new UnknownTexFrameInfoContainerMagicException(container.Magic);
                }
                
                return container;
            }
        }

        private static void ReadV2(TexFrameInfoContainer container, BinaryReader reader)
        {
            
        }

        private static void ReadV3(TexFrameInfoContainer container, BinaryReader reader)
        {
            container.Unk0 = reader.ReadInt32();
            container.Unk1 = reader.ReadInt32();

            
            for (var i = 0; i < container.FrameCount; i++)
            {
                var frame = new TexFrameInfo
                {
                    Unk0 = reader.ReadInt32(),
                    Frametime = reader.ReadSingle(),
                    FrameInMilliseconds = reader.ReadSingle(),
                    Unk1 = reader.ReadSingle(),
                    Width = reader.ReadSingle(),
                    Unk2 = reader.ReadSingle(),
                    Unk3 = reader.ReadSingle(),
                    Height = reader.ReadSingle(),
                };
            }
        }
    }
}