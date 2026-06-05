using System;
using System.IO;
using RePKG.Core.Texture;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace RePKG.Application.Texture.Helpers
{
    public static class ImageToTexConverter
    {
        public static Tex Convert(string imagePath, TexFormat format = TexFormat.RGBA8888, bool lz4 = false)
        {
            using (var image = Image.Load<Rgba32>(imagePath))
            {
                return Convert(image, format, lz4);
            }
        }

        public static Tex Convert(Image<Rgba32> image, TexFormat format = TexFormat.RGBA8888, bool lz4 = false)
        {
            var texWidth = NextPowerOfTwo(image.Width);
            var texHeight = NextPowerOfTwo(image.Height);

            if (image.Width != texWidth || image.Height != texHeight)
                image.Mutate(x => x.Resize(texWidth, texHeight));

            var pixels = new Rgba32[texWidth * texHeight];
            image.CopyPixelDataTo(pixels);

            byte[] pixelData;
            var mipmapFormat = GetPixelData(pixels, texWidth, texHeight, format, out pixelData);

            var mipmap = new TexMipmap
            {
                Width = texWidth,
                Height = texHeight,
                Bytes = pixelData,
                Format = mipmapFormat,
                IsLZ4Compressed = false,
                DecompressedBytesCount = pixelData.Length
            };

            var texImage = new TexImage();
            texImage.Mipmaps.Add(mipmap);

            var imageContainer = new TexImageContainer
            {
                Magic = "TEXB0002",
                ImageContainerVersion = TexImageContainerVersion.Version2,
                ImageFormat = FreeImageFormat.FIF_UNKNOWN
            };
            imageContainer.Images.Add(texImage);

            var header = new TexHeader
            {
                Format = format,
                Flags = TexFlags.None,
                TextureWidth = texWidth,
                TextureHeight = texHeight,
                ImageWidth = image.Width,
                ImageHeight = image.Height,
                UnkInt0 = 0
            };

            var tex = new Tex
            {
                Magic1 = "TEXV0005",
                Magic2 = "TEXI0001",
                Header = header,
                ImagesContainer = imageContainer
            };

            if (lz4)
            {
                var compressor = new TexMipmapCompressor();
                compressor.CompressMipmap(mipmap, mipmapFormat, true);
            }

            return tex;
        }

        public static Tex ConvertFromGif(string gifPath, bool lz4 = false)
        {
            using (var gif = Image.Load<Rgba32>(gifPath))
            {
                var frames = gif.Frames.Count;
                if (frames == 0)
                    return Convert(gif, TexFormat.RGBA8888, lz4);

                var texWidth = NextPowerOfTwo(gif.Width);
                var texHeight = NextPowerOfTwo(gif.Height);

                var header = new TexHeader
                {
                    Format = TexFormat.RGBA8888,
                    Flags = TexFlags.IsGif,
                    TextureWidth = texWidth,
                    TextureHeight = texHeight,
                    ImageWidth = gif.Width,
                    ImageHeight = gif.Height,
                    UnkInt0 = 0
                };

                var imageContainer = new TexImageContainer
                {
                    Magic = "TEXB0003",
                    ImageContainerVersion = TexImageContainerVersion.Version3,
                    ImageFormat = FreeImageFormat.FIF_GIF
                };

                var frameInfos = new TexFrameInfoContainer
                {
                    Magic = "TEXS0003",
                    GifWidth = gif.Width,
                    GifHeight = gif.Height
                };

                for (int i = 0; i < frames; i++)
                {
                    using (var frame = gif.Frames.CloneFrame(i))
                    {
                        if (frame.Width != texWidth || frame.Height != texHeight)
                            frame.Mutate(x => x.Resize(texWidth, texHeight));

                        var pixels = new Rgba32[texWidth * texHeight];
                        frame.CopyPixelDataTo(pixels);
                        var pixelData = PixelDataFromRgba32(pixels, texWidth, texHeight);

                        var mipmap = new TexMipmap
                        {
                            Width = texWidth,
                            Height = texHeight,
                            Bytes = pixelData,
                            Format = MipmapFormat.RGBA8888,
                            IsLZ4Compressed = false,
                            DecompressedBytesCount = pixelData.Length
                        };

                        if (lz4)
                        {
                            var compressor = new TexMipmapCompressor();
                            compressor.CompressMipmap(mipmap, MipmapFormat.RGBA8888, true);
                        }

                        var texImage = new TexImage();
                        texImage.Mipmaps.Add(mipmap);
                        imageContainer.Images.Add(texImage);

                        var delay = gif.Frames[i].Metadata.GetFormatMetadata(SixLabors.ImageSharp.Formats.Gif.GifFormat.Instance).FrameDelay;
                        frameInfos.Frames.Add(new TexFrameInfo
                        {
                            ImageId = i,
                            Frametime = delay / 100f,
                            X = 0,
                            Y = 0,
                            Width = gif.Width,
                            Height = gif.Height,
                            WidthY = gif.Width,
                            HeightX = gif.Height
                        });
                    }
                }

                var tex = new Tex
                {
                    Magic1 = "TEXV0005",
                    Magic2 = "TEXI0001",
                    Header = header,
                    ImagesContainer = imageContainer,
                    FrameInfoContainer = frameInfos
                };

                return tex;
            }
        }

        private static MipmapFormat GetPixelData(Rgba32[] pixels, int width, int height, TexFormat format, out byte[] pixelData)
        {
            switch (format)
            {
                case TexFormat.RGBA8888:
                    pixelData = PixelDataFromRgba32(pixels, width, height);
                    return MipmapFormat.RGBA8888;

                case TexFormat.R8:
                    pixelData = new byte[width * height];
                    for (int i = 0; i < width * height; i++)
                        pixelData[i] = (byte)((pixels[i].R * 299 + pixels[i].G * 587 + pixels[i].B * 114) / 1000);
                    return MipmapFormat.R8;

                case TexFormat.RG88:
                    pixelData = new byte[width * height * 2];
                    for (int i = 0; i < width * height; i++)
                    {
                        pixelData[i * 2] = pixels[i].R;
                        pixelData[i * 2 + 1] = pixels[i].G;
                    }
                    return MipmapFormat.RG88;

                default:
                    throw new NotSupportedException($"TexFormat {format} not supported");
            }
        }

        private static byte[] PixelDataFromRgba32(Rgba32[] pixels, int width, int height)
        {
            var data = new byte[width * height * 4];
            for (int i = 0; i < width * height; i++)
            {
                data[i * 4] = pixels[i].R;
                data[i * 4 + 1] = pixels[i].G;
                data[i * 4 + 2] = pixels[i].B;
                data[i * 4 + 3] = pixels[i].A;
            }
            return data;
        }

        public static Tex ConvertFromVideo(string videoPath, int width = 0, int height = 0, bool lz4 = false)
        {
            var videoBytes = File.ReadAllBytes(videoPath);

            if (width <= 0 || height <= 0)
                ProbeVideoDimensions(videoPath, ref width, ref height);

            if (width <= 0) width = 1920;
            if (height <= 0) height = 1080;

            var texWidth = NextPowerOfTwo(width);
            var texHeight = NextPowerOfTwo(height);

            var mipmap = new TexMipmap
            {
                Width = texWidth,
                Height = texHeight,
                Bytes = videoBytes,
                Format = MipmapFormat.VideoMp4,
                IsLZ4Compressed = false,
                DecompressedBytesCount = videoBytes.Length
            };

            var texImage = new TexImage();
            texImage.Mipmaps.Add(mipmap);

            var imageContainer = new TexImageContainer
            {
                Magic = "TEXB0004",
                ImageContainerVersion = TexImageContainerVersion.Version4,
                ImageFormat = FreeImageFormat.FIF_UNKNOWN
            };
            imageContainer.Images.Add(texImage);

            var header = new TexHeader
            {
                Format = TexFormat.RGBA8888,
                Flags = TexFlags.IsVideoTexture,
                TextureWidth = texWidth,
                TextureHeight = texHeight,
                ImageWidth = width,
                ImageHeight = height,
                UnkInt0 = 0
            };

            var tex = new Tex
            {
                Magic1 = "TEXV0005",
                Magic2 = "TEXI0001",
                Header = header,
                ImagesContainer = imageContainer
            };

            return tex;
        }

        private static void ProbeVideoDimensions(string videoPath, ref int width, ref int height)
        {
            try
            {
                var psi = new System.Diagnostics.ProcessStartInfo("ffprobe")
                {
                    Arguments = $"-v error -select_streams v:0 -show_entries stream=width,height -of csv=p=0 \"{videoPath}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                using (var proc = System.Diagnostics.Process.Start(psi))
                {
                    if (proc != null)
                    {
                        var output = proc.StandardOutput.ReadToEnd().Trim();
                        proc.WaitForExit(3000);
                        if (proc.ExitCode == 0 && !string.IsNullOrEmpty(output))
                        {
                            var parts = output.Split(',');
                            if (parts.Length == 2)
                            {
                                int.TryParse(parts[0], out width);
                                int.TryParse(parts[1], out height);
                            }
                        }
                    }
                }
            }
            catch
            {
                // ffprobe not available, user-provided or default dimensions used
            }
        }

        public static bool IsVideoFile(string path)
        {
            var ext = Path.GetExtension(path)?.ToLowerInvariant();
            switch (ext)
            {
                case ".mp4":
                case ".webm":
                case ".avi":
                case ".mov":
                case ".mkv":
                case ".flv":
                case ".wmv":
                    return true;
                default:
                    return false;
            }
        }

        private static int NextPowerOfTwo(int n)
        {
            if (n <= 0) return 1;
            n--;
            n |= n >> 1;
            n |= n >> 2;
            n |= n >> 4;
            n |= n >> 8;
            n |= n >> 16;
            return n + 1;
        }
    }
}
