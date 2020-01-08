using System.IO;

namespace RePKG.Core.Texture
{
    public interface ITexImageContainerWriter
    {
        void WriteTo(BinaryWriter writer, TexImageContainer imageContainer);
        void WriteImagesTo(BinaryWriter writer, Tex tex);
    }
}