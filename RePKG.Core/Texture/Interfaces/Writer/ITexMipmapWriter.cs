using System.IO;

namespace RePKG.Core.Texture
{
    public interface ITexMipmapWriter
    {
        void WriteToStream(Tex tex, TexMipmap mipmap, Stream stream);
    }
}