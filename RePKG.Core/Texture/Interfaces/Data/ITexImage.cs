using System.Collections.Generic;

namespace RePKG.Core.Texture
{
    public interface ITexImage
    {
        IList<ITexMipmap> Mipmaps { get; }
        ITexMipmap FirstMipmap { get; }
    };
}