using System.IO;

namespace RePKG.Core.Texture
{
    public interface ITexFrameInfoReader
    {
        TexFrameInfoContainer ReadFromStream(Stream stream);
    }
}