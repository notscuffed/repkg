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

    public unsafe class WCTexHeader : ITexHeader
    {
        public readonly CTexHeader* Self;

        public WCTexHeader(CTexHeader* self)
        {
            Self = self;
        }

        public TexFormat Format
        {
            get => Self->format;
            set => Self->format = value;
        }

        public TexFlags Flags
        {
            get => Self->flags;
            set => Self->flags = value;
        }

        public int TextureWidth
        {
            get => Self->texture_width;
            set => Self->texture_width = value;
        }

        public int TextureHeight
        {
            get => Self->texture_height;
            set => Self->texture_height = value;
        }

        public int ImageWidth
        {
            get => Self->image_width;
            set => Self->image_width = value;
        }

        public int ImageHeight
        {
            get => Self->image_height; 
            set => Self->image_height = value;
        }

        public uint UnkInt0
        {
            get => Self->unk_int0; 
            set => Self->unk_int0 = value;
        }
    }
}