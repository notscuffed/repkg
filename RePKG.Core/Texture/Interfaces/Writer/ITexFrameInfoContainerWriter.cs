using System.IO;

namespace RePKG.Core.Texture
{
    public interface ITexFrameInfoContainerWriter
    {
        void WriteToStream(TexFrameInfoContainer frameInfoContainer, Stream stream);
    }
}