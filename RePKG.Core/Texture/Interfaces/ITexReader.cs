using System.IO;

namespace RePKG.Core.Texture
{
    public interface ITexReader
    {
        ITex ReadFrom(BinaryReader reader);
    }
}