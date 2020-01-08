using System.IO;

namespace RePKG.Core.Texture
{
    public interface ITexImageReader
    {
        TexImage ReadFrom(BinaryReader reader, Tex tex);
    }
}