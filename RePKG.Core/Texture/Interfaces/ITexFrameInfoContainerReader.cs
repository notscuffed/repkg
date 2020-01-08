using System.IO;

namespace RePKG.Core.Texture
{
    public interface ITexFrameInfoContainerReader
    {
        TexFrameInfoContainer ReadFromStream(Stream stream);
    }
}