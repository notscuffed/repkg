using System.IO;

namespace RePKG.Core.Texture
{
    public interface ITexImageContainerReader
    {
        ITexImageContainer ReadFrom(BinaryReader reader, TexFormat texFormat);
    }
}