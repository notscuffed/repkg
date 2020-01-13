using System.Runtime.InteropServices;
using RePKG.Core.Texture;

namespace RePKG.Native.Texture
{
    [StructLayout(LayoutKind.Sequential)]
    public struct CTexHeader
    {
        public TexFormat format;
        public TexFlags flags;
        public int texture_width;
        public int texture_height;
        public int image_width;
        public int image_height;
        public uint unk_int0;
    }
}