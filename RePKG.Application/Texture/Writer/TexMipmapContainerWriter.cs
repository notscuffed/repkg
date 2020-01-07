using System;
using System.IO;
using System.Text;
using RePKG.Application.Exceptions;
using RePKG.Core.Texture;

namespace RePKG.Application.Texture
{
    public class TexMipmapContainerWriter : ITexMipmapContainerWriter
    {
        private readonly ITexMipmapWriter _texMipmapWriter;

        public TexMipmapContainerWriter(ITexMipmapWriter texMipmapWriter)
        {
            _texMipmapWriter = texMipmapWriter;
        }

        public void WriteToStream(TexMipmapContainer mipmapContainer, Stream stream)
        {
            if (mipmapContainer == null) throw new ArgumentNullException(nameof(mipmapContainer));
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            
            using (var writer = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                writer.WriteNString(mipmapContainer.Magic);

                switch (mipmapContainer.Magic)
                {
                    case "TEXB0001":
                        WriteV1(mipmapContainer, writer);
                        break;

                    case "TEXB0002":
                        WriteV2(mipmapContainer, writer);
                        break;

                    case "TEXB0003":
                        WriteV3(mipmapContainer, writer);
                        break;

                    default:
                        throw new UnknownTexMipmapContainerMagicException(mipmapContainer.Magic);
                }
            }
        }

        public void WriteMipmapsToStream(Tex tex, Stream stream)
        {
            foreach (var mipmap in tex.MipmapsContainer.Mipmaps)
            {
                _texMipmapWriter.WriteToStream(tex, mipmap, stream);
            }
        }

        private static void WriteV1(TexMipmapContainer container, BinaryWriter writer)
        {
            writer.Write(container.UnkIntCont0);
            writer.Write(container.Mipmaps.Count);
        }

        private static void WriteV2(TexMipmapContainer container, BinaryWriter writer)
        {
            writer.Write(container.UnkIntCont0);
            writer.Write(container.Mipmaps.Count);
        }

        private static void WriteV3(TexMipmapContainer container, BinaryWriter writer)
        {
            writer.Write(container.UnkIntCont0);
            writer.Write((int) container.ImageFormat);
            writer.Write(container.Mipmaps.Count);
        }
    }
}