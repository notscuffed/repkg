using System.Collections.Generic;

namespace RePKG.Core.Texture
{
    public class TexImageContainer
    {
        public string Magic { get; set; }
        public FreeImageFormat ImageFormat { get; set; } = FreeImageFormat.FIF_UNKNOWN;
        public List<TexImage> Images { get; } = new List<TexImage>();
        
        public TexImageContainerVersion ImageContainerVersion { get; set; }
    }
}