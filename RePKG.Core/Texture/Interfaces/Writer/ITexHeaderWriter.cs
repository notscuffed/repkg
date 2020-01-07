using System.IO;

namespace RePKG.Core.Texture
{
    public interface ITexHeaderWriter
    {
        void WriteToStream(TexHeader header, Stream stream);
    }
}