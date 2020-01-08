using System.IO;

namespace RePKG.Core.Texture
{
    public interface ITexImageReader
    {
        TexImage ReadFromStream(Stream stream, Tex tex);
    }
}