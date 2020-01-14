using System.IO;

namespace RePKG.Core.Texture
{
    public class TexMipmap : ITexMipmap
    {
        public byte[] Bytes { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int DecompressedBytesCount { get; set; }
        public bool IsLZ4Compressed { get; set; }
        public MipmapFormat Format { get; set; }
        
        public Stream GetBytesStream()
        {
            return new MemoryStream(Bytes);
        }
    }
}