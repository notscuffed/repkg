using System.Runtime.InteropServices;
using RePKG.Core.Texture;

namespace RePKG.Native.Texture
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct CTexMipmap
    {
        public void* bytes;
        public int bytes_count;
        public int width;
        public int height;
        public int decompressed_bytes_count;
        public bool is_lz4_compressed;
        public MipmapFormat format;
    }
}