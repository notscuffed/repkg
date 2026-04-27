namespace RePKG.Core.Texture
{
    public interface ITexHeaderReader
    {
        ITexHeader ReadFrom(BinaryReader reader);
    }
}