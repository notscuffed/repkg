using System.Runtime.InteropServices;

namespace RePKG.Native.Texture
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct CTexImage
    {
        public int mipmap_count;
        public CTexMipmap* mipmaps;
    }
}