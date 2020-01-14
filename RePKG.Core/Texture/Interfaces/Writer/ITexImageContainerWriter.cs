using System.IO;

namespace RePKG.Core.Texture
{
    public interface ITexImageContainerWriter
    {
        void WriteTo(BinaryWriter writer, ITexImageContainer imageContainer);
    }
}