using System.Runtime.InteropServices;

namespace RePKG.Native.Texture
{
    [StructLayout(LayoutKind.Sequential)]
    public struct CTexFrameInfo
    {
        public int image_id;
        public float frametime;
        public float x;
        public float y;
        public float width;
        public float unk0;
        public float unk1;
        public float height;
    }
}