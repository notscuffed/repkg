using System.IO;

namespace RePKG.Core.Texture
{
    public interface ITexImageContainerReader
    {
        TexImageContainer ReadFrom(BinaryReader reader);
        void ReadImagesFrom(BinaryReader reader, Tex tex);
    }
}