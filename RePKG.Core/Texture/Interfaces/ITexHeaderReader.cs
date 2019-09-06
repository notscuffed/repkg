using System.IO;

namespace RePKG.Core.Texture
{
    public interface ITexHeaderReader
    {
        TexHeader ReadFromStream(Stream stream);
    }
}