namespace RePKG.Core.Texture
{
    public class TexFrameInfoContainer
    {
        public string Magic { get; set; }
        public int FrameCount { get; set; }
        public TexFrameInfo[] Frames { get; set; }
        public int Unk0 { get; set; }
        public int Unk1 { get; set; }
    }
}