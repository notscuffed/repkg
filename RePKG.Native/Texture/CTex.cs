using System.Runtime.InteropServices;

namespace RePKG.Native.Texture
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct CTex
    {
        public void* magic1;
        public void* magic2;
        public CTexHeader header;
        public CTexImageContainer images_container;
        public CTexFrameInfoContainer* frameinfo_container;
    }
}