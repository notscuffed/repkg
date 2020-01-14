using System.IO;

namespace RePKG.Core.Texture
{
    public interface ITexMipmap
    {
        byte[] Bytes { get; set; }
        int Width { get; set; }
        int Height { get; set; }
        int DecompressedBytesCount { get; set; }
        bool IsLZ4Compressed { get; set; }
        MipmapFormat Format { get; set; }

        Stream GetBytesStream();
    }
}