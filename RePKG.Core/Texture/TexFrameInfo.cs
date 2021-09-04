namespace RePKG.Core.Texture
{
    public class TexFrameInfo : ITexFrameInfo
    {
        public int ImageId { get; set; }
        public float Frametime { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float WidthY { get; set; }
        public float HeightX { get; set; }
        public float Height { get; set; }
    }
}