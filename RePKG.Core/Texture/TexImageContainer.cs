using System.Collections.Generic;

namespace RePKG.Core.Texture
{
    public class TexImageContainer
    {
        public TexImageContainer()
        {
            Images = new List<TexImage>();
        }
        
        public string Magic { get; set; }
        public FreeImageFormat ImageFormat { get; set; } = FreeImageFormat.FIF_UNKNOWN;
        public int ImageCount { get; set; }
        public List<TexImage> Images { get; }
        
        public TexImageContainerVersion ImageContainerVersion { get; set; }
    }
}