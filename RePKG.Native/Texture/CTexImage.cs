using System.Collections.Generic;
using System.Runtime.InteropServices;
using RePKG.Core.Texture;

namespace RePKG.Native.Texture
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct CTexImage
    {
        public int mipmap_count;
        public int mipmaps_length;
        public CTexMipmap* mipmaps;
    }

    public unsafe class WCTexImage : ITexImage
    {
        public readonly CTexImage* Self;
        private readonly WCList<CTexMipmap, ITexMipmap> _mipmaps;

        public WCTexImage(CTexImage* self, NativeEnvironment environment)
        {
            Self = self;

            _mipmaps = new WCList<CTexMipmap, ITexMipmap>(
                &Self->mipmap_count,
                &Self->mipmaps,
                environment,
                (x, e) => new WCTexMipmap(x, e));
        }

        public IList<ITexMipmap> Mipmaps => _mipmaps;
        public ITexMipmap FirstMipmap => _mipmaps?[0];
    }
}