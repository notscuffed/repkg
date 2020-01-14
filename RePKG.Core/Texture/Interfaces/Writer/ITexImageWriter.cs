using System.IO;

namespace RePKG.Core.Texture
{
    public interface ITexImageWriter
    {
        void WriteTo(BinaryWriter writer, TexImageContainerVersion containerVersion, ITexImage image);
    }
}