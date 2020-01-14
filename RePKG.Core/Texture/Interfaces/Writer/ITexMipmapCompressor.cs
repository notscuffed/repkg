namespace RePKG.Core.Texture
{
    public interface ITexMipmapCompressor
    {
        void CompressMipmap(ITexMipmap mipmap, MipmapFormat targetCompressFormat, bool lz4Compress);
    }
}