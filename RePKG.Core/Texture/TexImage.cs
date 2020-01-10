using System.Collections.Generic;
using System.Linq;

namespace RePKG.Core.Texture
{
    public class TexImage
    {
        public List<TexMipmap> Mipmaps { get; } = new List<TexMipmap>();

        public TexMipmap FirstMipmap => Mipmaps.FirstOrDefault();
    }
}