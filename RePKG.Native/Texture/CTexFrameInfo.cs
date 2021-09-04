using System.Runtime.InteropServices;
using RePKG.Core.Texture;

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

    public unsafe class WCTexFrameInfo : ITexFrameInfo
    {
        public readonly CTexFrameInfo* Self;

        public WCTexFrameInfo(CTexFrameInfo* self)
        {
            Self = self;
        }

        public int ImageId
        {
            get => Self->image_id; 
            set => Self->image_id = value;
        }

        public float Frametime
        {
            get => Self->frametime;
            set => Self->frametime = value;
        }

        public float X
        {
            get => Self->x;
            set => Self->x = value;
        }

        public float Y
        {
            get => Self->y;
            set => Self->y = value;
        }

        public float Width
        {
            get => Self->width;
            set => Self->width = value;
        }

        public float WidthY
        {
            get => Self->unk0;
            set => Self->unk0 = value;
        }

        public float HeightX
        {
            get => Self->unk1;
            set => Self->unk1 = value;
        }

        public float Height
        {
            get => Self->height;
            set => Self->height = value;
        }
    }
}