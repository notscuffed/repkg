namespace RePKG.Core.Texture
{
    public interface ITexHeader
    {
        TexFormat Format { get; set; }
        TexFlags Flags { get; set; }
        int TextureWidth { get; set; }
        int TextureHeight { get; set; }
        int ImageWidth { get; set; }
        int ImageHeight { get; set; }
        uint UnkInt0 { get; set; }
    }
}