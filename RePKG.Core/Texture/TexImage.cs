namespace RePKG.Core.Texture
{
    public class TexImage : ITexImage
    {
        public IList<ITexMipmap> Mipmaps { get; } = new List<ITexMipmap>();

        public ITexMipmap FirstMipmap => Mipmaps.FirstOrDefault();
    }
}