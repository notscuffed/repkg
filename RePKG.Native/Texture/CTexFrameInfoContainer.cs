using System.Runtime.InteropServices;

namespace RePKG.Native.Texture
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct CTexFrameInfoContainer
    {
        public void* magic;
        public int frame_count;
        public CTexFrameInfo* frames;
        public int gif_width;
        public int gif_height;
    }
}