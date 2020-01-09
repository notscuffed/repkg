namespace RePKG.Core.Texture
{
    public class TexFrameInfoContainer
    {
        public string Magic { get; set; }
        public int FrameCount { get; set; }
        public TexFrameInfo[] Frames { get; set; }
        public int GifWidth { get; set; }
        public int GifHeight { get; set; }
    }
}