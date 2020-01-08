using System.IO;

namespace RePKG.Core.Texture
{
    public interface ITexFrameInfoContainerReader
    {
        TexFrameInfoContainer ReadFrom(BinaryReader reader);
    }
}