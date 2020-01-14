using System.Collections.Generic;

namespace RePKG.Core.Texture
{
    public class TexFrameInfoContainer : ITexFrameInfoContainer
    {
        public string Magic { get; set; }
        public IList<ITexFrameInfo> Frames { get; } = new List<ITexFrameInfo>();
        public int GifWidth { get; set; }
        public int GifHeight { get; set; }
    }
}