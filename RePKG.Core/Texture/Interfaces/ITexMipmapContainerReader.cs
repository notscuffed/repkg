using System.IO;

namespace RePKG.Core.Texture
{
    public interface ITexMipmapContainerReader
    {
        TexMipmapContainer ReadFromStream(Stream stream);
        void ReadMipmapsFromStream(Stream stream, Tex tex);
    }
}