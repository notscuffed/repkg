namespace RePKG.Core.Texture
{
    public class TexHeader : ITexHeader
    {
        public TexFormat Format { get; set; }
        public TexFlags Flags { get; set; }
        public int TextureWidth { get; set; }
        public int TextureHeight { get; set; }
        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }
        public uint UnkInt0 { get; set; }
    }
}