using System.IO;

namespace RePKG.Core.Texture
{
    public interface ITexReader
    {
        Tex ReadFrom(BinaryReader reader);
    }
}