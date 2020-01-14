namespace RePKG.Core.Texture
{
    public interface ITexFrameInfo
    {
        int ImageId { get; set; }
        float Frametime { get; set; }
        float X { get; set; }
        float Y { get; set; }
        float Width { get; set; }
        float Unk0 { get; set; }
        float Unk1 { get; set; }
        float Height { get; set; }
    }
}