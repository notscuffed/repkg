using System.IO;

namespace RePKG.Core.Texture
{
    public interface ITexImageContainerReader
    {
        TexImageContainer ReadFromStream(Stream stream);
        void ReadImagesFromStream(Stream stream, Tex tex);
    }
}