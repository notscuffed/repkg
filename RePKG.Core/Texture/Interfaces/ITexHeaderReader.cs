using System.IO;

namespace RePKG.Core.Texture
{
    public interface ITexHeaderReader
    {
        TexHeader ReadFrom(BinaryReader reader);
    }
}