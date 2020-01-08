using System.IO;

namespace RePKG.Core.Texture
{
    public interface ITexImageContainerWriter
    {
        void WriteToStream(TexImageContainer imageContainer, Stream stream);
        void WriteImagesToStream(Tex tex, Stream stream);
    }
}