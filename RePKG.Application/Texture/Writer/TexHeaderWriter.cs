using System;
using System.IO;
using RePKG.Core.Texture;

namespace RePKG.Application.Texture
{
    public class TexHeaderWriter : ITexHeaderWriter
    {
        public void WriteTo(BinaryWriter writer, ITexHeader header)
        {
            if (writer == null) throw new ArgumentNullException(nameof(writer));
            if (header == null) throw new ArgumentNullException(nameof(header));

            writer.Write((int) header.Format);
            writer.Write((int) header.Flags);
            writer.Write(header.TextureWidth);
            writer.Write(header.TextureHeight);
            writer.Write(header.ImageWidth);
            writer.Write(header.ImageHeight);
            writer.Write(header.UnkInt0);
        }
    }
}