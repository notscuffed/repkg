using System.Linq;

namespace RePKG.Core.Texture
{
    public class TexImage
    {
        public TexImage(int mipmapCount)
        {
            MipmapCount = mipmapCount;
            Mipmaps = new TexMipmap[mipmapCount];
        }

        public TexImage()
        {
        }

        public int MipmapCount { get; set; }
        public TexMipmap[] Mipmaps { get; set; }
        
        public TexMipmap FirstMipmap => Mipmaps?.FirstOrDefault();
    }
}