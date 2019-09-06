using System.IO;

namespace RePKG.Core.Texture
{
    public interface ITexReader
    {
        Tex ReadFromStream(Stream stream);
    }
}