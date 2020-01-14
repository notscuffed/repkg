using System.IO;

namespace RePKG.Core.Texture
{
    public interface ITexImageReader
    {
        ITexImage ReadFrom(
            BinaryReader reader,
            ITexImageContainer container,
            TexFormat texFormat);
    }
}