using System;

namespace RePKG.Core.Texture
{
    [Flags]
    public enum TexFlags
    {
        None = 0,
        NoInterpolation = 1,
        ClampUVs = 2,
        IsGif = 4,
        // Placeholders
        Unk3 = 8,
        Unk4 = 16,
        Unk5 = 32,
        Unk6 = 64,
        Unk7 = 128,
    }
}