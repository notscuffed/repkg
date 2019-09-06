using System.IO;

namespace RePKG.Core.Texture
{
    public interface ITexMipmapReader
    {
        TexMipmap ReadFromStream(Stream stream, Tex tex);
    }
}