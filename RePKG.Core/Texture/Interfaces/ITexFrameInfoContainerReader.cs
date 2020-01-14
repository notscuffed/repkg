using System.IO;

namespace RePKG.Core.Texture
{
    public interface ITexFrameInfoContainerReader
    {
        ITexFrameInfoContainer ReadFrom(BinaryReader reader);
    }
}