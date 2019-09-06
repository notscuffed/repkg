using System.Collections.Generic;

namespace RePKG.Core.Texture
{
    public class TexMipmapContainer
    {
        public string Magic { get; set; }
        public FreeImageFormat ImageFormat { get; set; } = FreeImageFormat.FIF_UNKNOWN;
        public int UnkIntCont0 { get; set; }
        public int MipmapCount { get; set; }
        public List<TexMipmap> Mipmaps { get; }
        
        public TexMipmapContainerVersion MipmapContainerVersion { get; set; }

        public TexMipmapContainer()
        {
            Mipmaps = new List<TexMipmap>();
        }
    }
}