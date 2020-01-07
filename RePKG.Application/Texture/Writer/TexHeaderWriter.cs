using System;
using System.IO;
using System.Text;
using RePKG.Core.Texture;

namespace RePKG.Application.Texture
{
    public class TexHeaderWriter : ITexHeaderWriter
    {
        public void WriteToStream(TexHeader header, Stream stream)
        {
            if (header == null) throw new ArgumentNullException(nameof(header));
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            
            using (var writer = new BinaryWriter(stream, Encoding.UTF8, true))
            {
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
}