using System.Runtime.InteropServices;
using RePKG.Core.Texture;

namespace RePKG.Native.Texture
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct CTexImageContainer
    {
        public void* magic;
        public FreeImageFormat image_format;
        public int image_count;
        public CTexImage* images;
        
        public TexImageContainerVersion container_version;
    }
}