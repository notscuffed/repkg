using System;
using System.IO;
using System.Text;
using RePKG.Core.Texture;

namespace RePKG.Application.Texture
{
    public class TexMipmapWriter : ITexMipmapWriter
    {
        public void WriteToStream(Tex tex, TexMipmap mipmap, Stream stream)
        {
            if (tex == null) throw new ArgumentNullException(nameof(tex));
            if (mipmap == null) throw new ArgumentNullException(nameof(mipmap));
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            
            using (var writer = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                switch (tex.MipmapsContainer.MipmapContainerVersion)
                {
                    case TexMipmapContainerVersion.Version1:
                        if (mipmap.IsLZ4Compressed)
                            throw new InvalidOperationException(
                                $"Cannot write lz4 compressed mipmap when using tex container version: {tex.MipmapsContainer.MipmapContainerVersion}");
                        
                        writer.Write(mipmap.Width);
                        writer.Write(mipmap.Height);
                        writer.Write(mipmap.BytesCount);
                        break;

                    case TexMipmapContainerVersion.Version2:
                    case TexMipmapContainerVersion.Version3:
                        writer.Write(mipmap.Width);
                        writer.Write(mipmap.Height);
                        writer.Write(mipmap.IsLZ4Compressed ? 1 : 0);
                        writer.Write(mipmap.DecompressedBytesCount);
                        writer.Write(mipmap.BytesCount);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                writer.Write(mipmap.Bytes);
            }
        }
    }
}