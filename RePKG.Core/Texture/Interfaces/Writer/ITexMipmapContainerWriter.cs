using System.IO;

namespace RePKG.Core.Texture
{
    public interface ITexMipmapContainerWriter
    {
        void WriteToStream(TexMipmapContainer mipmapContainer, Stream stream);
        void WriteMipmapsToStream(Tex tex, Stream stream);
    }
}