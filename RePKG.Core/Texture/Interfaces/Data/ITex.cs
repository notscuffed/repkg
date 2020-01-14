namespace RePKG.Core.Texture
{
    public interface ITex
    {
        string Magic1 { get; set; }
        string Magic2 { get; set; }
        ITexHeader Header { get; set; }
        ITexImageContainer ImagesContainer { get; set; }
        ITexFrameInfoContainer FrameInfoContainer { get; set; }

        bool IsGif { get; }
        ITexImage FirstImage { get; }

        bool HasFlag(TexFlags flag);
    }
}