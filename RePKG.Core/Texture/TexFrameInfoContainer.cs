using System.Collections.Generic;

namespace RePKG.Core.Texture
{
    public class TexFrameInfoContainer
    {
        public string Magic { get; set; }
        public List<TexFrameInfo> Frames { get; } = new List<TexFrameInfo>();
        public int GifWidth { get; set; }
        public int GifHeight { get; set; }
    }
}