using System.IO;

namespace RePKG.Core.Texture
{
    public interface ITexHeaderWriter
    {
        void WriteTo(BinaryWriter writer, ITexHeader header);
    }
}