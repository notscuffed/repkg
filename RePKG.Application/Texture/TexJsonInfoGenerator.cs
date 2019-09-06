using Newtonsoft.Json;
using RePKG.Core.Texture;

namespace RePKG.Application.Texture
{
    public class TexJsonInfoGenerator : ITexJsonInfoGenerator
    {
        public string GenerateInfo(Tex tex)
        {
            return JsonConvert.SerializeObject(new
            {
                bleedtransparentcolors = true,
                clampuvs = tex.HasFlag(TexFlags.ClampUVs),
                format = tex.Header.Format.ToString().ToLower(),
                nomip = (tex.MipmapsContainer.Mipmaps.Count == 1).ToString().ToLower(),
                nointerpolation = tex.HasFlag(TexFlags.NoInterpolation).ToString().ToLower(),
                nonpoweroftwo = (!NumberIsPowerOfTwo(tex.Header.ImageWidth) ||
                                 !NumberIsPowerOfTwo(tex.Header.ImageHeight)).ToString().ToLower()
            });
        }

        private static bool NumberIsPowerOfTwo(int n)
        {
            if (n == 0)
                return false;

            while (n != 1)
            {
                if (n % 2 != 0)
                    return false;

                n /= 2;
            }

            return true;
        }
    }
}