using System.Linq;

namespace RePKG.Core.Texture
{
    public class Tex
    {
        public string Magic1 { get; set; } // always: TEXV0005
        public string Magic2 { get; set; } // always: TEXI0001
        public TexHeader Header { get; set; }
        public TexMipmapContainer MipmapsContainer { get; set; }
        
        public bool IsGif => HasFlag(TexFlags.IsGif);
        public TexMipmap FirstMipmap => MipmapsContainer?.Mipmaps.FirstOrDefault();
        
        public bool HasFlag(TexFlags flag)
        {
            if (Header == null)
                return false;

            return (Header.Flags & flag) == flag;
        }
    }
}