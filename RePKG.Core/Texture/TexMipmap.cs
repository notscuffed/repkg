namespace RePKG.Core.Texture
{
    public class TexMipmap
    {
        public MipmapFormat Format { get; set; }
        public byte[] Bytes { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int PixelCount { get; set; }
        public int BytesCount { get; set; }
        public bool IsLZ4Compressed { get; set; }
        public bool IsImage => Format.IsImage();
    }
}