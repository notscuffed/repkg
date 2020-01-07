namespace RePKG.Core.Texture
{
    public interface ITexMipmapCompressor
    {
        void CompressMipmap(TexMipmap mipmap, MipmapFormat targetCompressFormat, bool lz4Compress);
    }
}