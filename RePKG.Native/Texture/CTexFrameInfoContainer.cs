using System.Collections.Generic;
using System.Runtime.InteropServices;
using RePKG.Core.Texture;

namespace RePKG.Native.Texture
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct CTexFrameInfoContainer
    {
        public void* magic;
        public int frame_count;
        public int frames_length;
        public CTexFrameInfo* frames;
        public int gif_width;
        public int gif_height;
    }

    public unsafe class WCTexFrameInfoContainer : ITexFrameInfoContainer
    {
        public readonly CTexFrameInfoContainer* Self;

        private readonly WCString _magic;
        private readonly WCList<CTexFrameInfo, ITexFrameInfo> _frames;

        public WCTexFrameInfoContainer(CTexFrameInfoContainer* self, NativeEnvironment environment)
        {
            Self = self;

            _magic = new WCString(&self->magic, environment);

            _frames = new WCList<CTexFrameInfo, ITexFrameInfo>(
                &Self->frame_count,
                &Self->frames,
                environment,
                (x, _) => new WCTexFrameInfo(x));
        }

        public string Magic
        {
            get => _magic.Value;
            set => _magic.Value = value;
        }

        public IList<ITexFrameInfo> Frames => _frames;

        public int GifWidth
        {
            get => Self->gif_width;
            set => Self->gif_width = value;
        }

        public int GifHeight
        {
            get => Self->gif_height;
            set => Self->gif_height = value;
        }
    }
}