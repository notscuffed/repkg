using System.Collections.Generic;
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
        public int images_length;
        public CTexImage* images;

        public TexImageContainerVersion container_version;
    }

    public unsafe class WCTexImageContainer : ITexImageContainer
    {
        public readonly CTexImageContainer* Self;

        private readonly WCString _magic;
        private readonly WCList<CTexImage, ITexImage> _images;

        public WCTexImageContainer(CTexImageContainer* self, NativeEnvironment environment)
        {
            Self = self;
            
            _magic = new WCString(&self->magic, environment);
            _images = new WCList<CTexImage, ITexImage>(
                &Self->image_count,
                &Self->images,
                environment,
                (x, e) => new WCTexImage(x, e));
        }
        
        public string Magic
        {
            get => _magic.Value;
            set => _magic.Value = value;
        }

        public FreeImageFormat ImageFormat
        {
            get => Self->image_format;
            set => Self->image_format = value;
        }

        public IList<ITexImage> Images => _images;

        public TexImageContainerVersion ImageContainerVersion
        {
            get => Self->container_version;
            set => Self->container_version = value;
        }
    }
}