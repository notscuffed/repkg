using System.IO;

namespace RePKG.Core.Texture
{
    public interface ITexWriter
    {
        void WriteTo(BinaryWriter writer, ITex tex);
    }
}