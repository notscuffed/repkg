using System;
using System.Text.Json;

using System.Text.Json.Nodes;
using RePKG.Core.Texture;

namespace RePKG.Application.Texture
{
    public class TexJsonInfoGenerator : ITexJsonInfoGenerator
    {
        public string GenerateInfo(ITex tex)
        {
            if (tex == null) throw new ArgumentNullException(nameof(tex));

            var json = new JsonObject
            {
                ["bleedtransparentcolors"] = true,
                ["clampuvs"] = tex.HasFlag(TexFlags.ClampUVs),
                ["format"] = tex.Header.Format.ToString().ToLower(),
                ["nomip"] = (tex.FirstImage.Mipmaps.Count == 1).ToString().ToLower(),
                ["nointerpolation"] = tex.HasFlag(TexFlags.NoInterpolation).ToString().ToLower(),
                ["nonpoweroftwo"] = (!NumberIsPowerOfTwo(tex.Header.ImageWidth) ||
                                     !NumberIsPowerOfTwo(tex.Header.ImageHeight)).ToString().ToLower()
            };

            if (tex.IsGif)
            {
                if (tex.FrameInfoContainer == null)
                    throw new InvalidOperationException("TEX is animated but doesn't have frame info container");
                
                json["spritesheetsequences"] = new JsonArray
                {
                    new JsonObject
                    {
                        ["duration"] = 1, // not sure what this value is used for
                        ["frames"] = tex.FrameInfoContainer.Frames.Count,
                        ["width"] = tex.FrameInfoContainer.GifWidth,
                        ["height"] = tex.FrameInfoContainer.GifHeight
                    }
                };
            }
            return JsonSerializer.Serialize(json, new JsonSerializerOptions() { WriteIndented = true});
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