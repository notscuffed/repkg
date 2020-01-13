namespace RePKG.Core.Texture
{
    public static class EnumExtensions
    {
        // Not using reflection to do this because reflection data is stripped away for RePKG.Native and it would fail
        public static bool IsValid(this TexFormat format)
        {
            switch (format)
            {
                case TexFormat.RGBA8888:
                case TexFormat.DXT5:
                case TexFormat.DXT3:
                case TexFormat.DXT1:
                case TexFormat.RG88:
                case TexFormat.R8:
                    return true;
                default:
                    return false;
            }
        }
        
        public static bool IsValid(this FreeImageFormat format)
        {
            var formatId = (int) format;

            return formatId >= -1 && formatId <= 34;
        }
    }
}