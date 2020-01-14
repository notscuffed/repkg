using System.IO;

namespace RePKG.Core.Texture
{
    public interface ITexFrameInfoContainerWriter
    {
        void WriteTo(BinaryWriter writer, ITexFrameInfoContainer frameInfoContainer);
    }
}