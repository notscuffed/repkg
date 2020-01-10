using System;

namespace RePKG.Core.Texture
{
    [Flags]
    public enum DXTFlags
    {
        DXT1 = 1,
        DXT3 = 1 << 1,
        DXT5 = 1 << 2,
    }
}