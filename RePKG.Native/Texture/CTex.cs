using System;
using System.Runtime.InteropServices;
using RePKG.Core.Texture;

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

    public unsafe class WCTex : ITex
    {
        public readonly CTex* Self;
        private readonly NativeEnvironment _environment;
        
        private readonly WCString _magic1;
        private readonly WCString _magic2;
        private readonly WCTexHeader _wcTexHeader;
        private readonly WCTexImageContainer _wcTexImageContainer;
        private WCTexFrameInfoContainer _wcTexFrameInfoContainer;
        
        public WCTex(CTex* self, NativeEnvironment environment)
        {
            Self = self;
            _environment = environment;

            _wcTexHeader = new WCTexHeader(&self->header);
            _wcTexImageContainer = new WCTexImageContainer(&self->images_container, _environment);

            _magic1 = new WCString(&self->magic1, environment);
            _magic2 = new WCString(&self->magic2, environment);
            
            if (Self->frameinfo_container != null)
            {
                _wcTexFrameInfoContainer = new WCTexFrameInfoContainer(Self->frameinfo_container, environment);
            }
        }

        public string Magic1
        {
            get => _magic1.Value;
            set => _magic1.Value = value;
        }

        public string Magic2
        {
            get => _magic2.Value;
            set => _magic2.Value = value;
        }

        public ITexHeader Header
        {
            get => _wcTexHeader;
            set => throw new NotImplementedException();
        }

        public ITexImageContainer ImagesContainer
        {
            get => _wcTexImageContainer;
            set => throw new NotImplementedException();
        }
        
        public ITexFrameInfoContainer FrameInfoContainer
        {
            get
            {
                if (Self->frameinfo_container == null)
                {
                    _wcTexFrameInfoContainer = null;
                    return null;
                }

                if (_wcTexFrameInfoContainer == null ||
                    Self->frameinfo_container != _wcTexFrameInfoContainer.Self)
                {
                    _wcTexFrameInfoContainer = new WCTexFrameInfoContainer(Self->frameinfo_container, _environment);
                }
                
                return _wcTexFrameInfoContainer;
            }
            set => throw new NotImplementedException();
        }

        public bool IsGif => HasFlag(TexFlags.IsGif);
        public ITexImage FirstImage
        {
            get
            {
                if (_wcTexImageContainer.Images.Count == 0)
                    return null;
                
                return _wcTexImageContainer.Images[0];
            }   
        }

        public bool HasFlag(TexFlags flag)
        {
            return (_wcTexHeader.Flags & flag) == flag;
        }
    }
}