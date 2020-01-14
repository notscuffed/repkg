using System.Linq;

namespace RePKG.Core.Texture
{
    public class Tex : ITex
    {
        public string Magic1 { get; set; } // always: TEXV0005
        public string Magic2 { get; set; } // always: TEXI0001
        public ITexHeader Header { get; set; }
        public ITexImageContainer ImagesContainer { get; set; }
        public ITexFrameInfoContainer FrameInfoContainer { get; set; }
        
        public bool IsGif => HasFlag(TexFlags.IsGif);
        public ITexImage FirstImage => ImagesContainer?.Images.FirstOrDefault();
        
        public bool HasFlag(TexFlags flag)
        {
            if (Header == null)
                return false;

            return (Header.Flags & flag) == flag;
        }
    }
}