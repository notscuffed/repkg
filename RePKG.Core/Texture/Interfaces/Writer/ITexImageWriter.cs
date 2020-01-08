using System.IO;

namespace RePKG.Core.Texture
{
    public interface ITexImageWriter
    {
        void WriteToStream(Tex tex, TexImage mipmap, Stream stream);
    }
}