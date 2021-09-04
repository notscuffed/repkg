using System;
using System.IO;
using RePKG.Application.Texture.Helpers;
using RePKG.Core.Texture;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;

namespace RePKG.Application.Texture
{
    public class TexToImageConverter
    {
        public ImageResult ConvertToImage(ITex tex)
        {
            if (tex == null) throw new ArgumentNullException(nameof(tex));
            
            if (tex.IsGif)
                return ConvertToGif(tex);

            var sourceMipmap = tex.FirstImage.FirstMipmap;
            var format = sourceMipmap.Format;

            if (format.IsCompressed())
                throw new InvalidOperationException("Raw mipmap format must be uncompressed");

            if (format.IsRawFormat())
            {
                var image = ImageFromRawFormat(format, sourceMipmap.Bytes, sourceMipmap.Width, sourceMipmap.Height);

                if (sourceMipmap.Width != tex.Header.ImageWidth ||
                    sourceMipmap.Height != tex.Header.ImageHeight)
                    image.Mutate(x => x.Crop(tex.Header.ImageWidth, tex.Header.ImageHeight));

                using (var memoryStream = new MemoryStream())
                {
                    image.SaveAsPng(memoryStream);

                    return new ImageResult
                    {
                        Bytes = memoryStream.ToArray(),
                        Format = MipmapFormat.ImagePNG
                    };
                }
            }

            return new ImageResult
            {
                Bytes = sourceMipmap.Bytes,
                Format = format
            };
        }

        public MipmapFormat GetConvertedFormat(ITex tex)
        {
            if (tex == null) throw new ArgumentNullException(nameof(tex));
            
            if (tex.IsGif)
                return MipmapFormat.ImageGIF;

            var format = tex.FirstImage.FirstMipmap.Format;

            if (format.IsCompressed())
                throw new InvalidOperationException("Raw mipmap format must be uncompressed");

            return format.IsRawFormat() ? MipmapFormat.ImagePNG : format;
        }

        private static ImageResult ConvertToGif(ITex tex)
        {
            var frameFormat = tex.FirstImage.FirstMipmap.Format;

            if (!frameFormat.IsRawFormat())
                throw new InvalidOperationException(
                    "Only raw mipmap formats are supported right now while converting gif");

            var image = ImageFromRawFormat(frameFormat, null,
                tex.FrameInfoContainer.GifWidth,
                tex.FrameInfoContainer.GifHeight);

            var sequenceImages = new Image[tex.ImagesContainer.Images.Count];

            for (var i = 0; i < sequenceImages.Length; i++)
            {
                var mipmap = tex.ImagesContainer.Images[i].FirstMipmap;
                sequenceImages[i] = ImageFromRawFormat(frameFormat, mipmap.Bytes, mipmap.Width, mipmap.Height);
            }

            foreach (var frameInfo in tex.FrameInfoContainer.Frames)
            {
                // Frames can be turned to fit into the map so we need to compute cropping coordinates first
                // We're keeping width and height signed for the rotation angle calculation
                var width = frameInfo.Width != 0 ? frameInfo.Width : frameInfo.HeightX;
                var height = frameInfo.Height != 0 ? frameInfo.Height : frameInfo.WidthY;
                var x = Math.Min(frameInfo.X, frameInfo.X + width);
                var y = Math.Min(frameInfo.Y, frameInfo.Y + height);
                
                // This formula gives us the angle for which we need to turn the frame,
                // assuming that either Width or HeightX is 0 (same with Height and WidthY)
                var rotationAngle = -(Math.Atan2(Math.Sign(height), Math.Sign(width)) - Math.PI / 4);
                
                var frame = sequenceImages[frameInfo.ImageId].Clone(
                    context => context.Crop(new Rectangle(
                        (int) x,
                        (int) y,
                        (int) Math.Abs(width),
                        (int) Math.Abs(height))
                    ).Rotate((float) Math.Round(rotationAngle * 180 / Math.PI)));

                var metadata = frame.Frames.RootFrame.Metadata.GetFormatMetadata(GifFormat.Instance);
                metadata.FrameDelay = (int) Math.Round(frameInfo.Frametime * 100.0f);

                image.Frames.AddFrame(frame.Frames[0]);
            }

            // Remove first black frame
            image.Frames.RemoveFrame(0);

            using (var memoryStream = new MemoryStream())
            {
                image.SaveAsGif(memoryStream, new GifEncoder {ColorTableMode = GifColorTableMode.Local});

                return new ImageResult
                {
                    Bytes = memoryStream.ToArray(),
                    Format = MipmapFormat.ImageGIF
                };
            }
        }

        private static Image ImageFromRawFormat(MipmapFormat format, byte[] bytes, int width, int height)
        {
            switch (format)
            {
                case MipmapFormat.R8:
                    return bytes == null
                        ? new Image<Gray8>(width, height)
                        : Image.LoadPixelData<Gray8>(bytes, width, height);

                case MipmapFormat.RG88:
                    return bytes == null
                        ? new Image<RG88>(width, height)
                        : Image.LoadPixelData<RG88>(bytes, width, height);

                case MipmapFormat.RGBA8888:
                    return bytes == null
                        ? new Image<Rgba32>(width, height)
                        : Image.LoadPixelData<Rgba32>(bytes, width, height);

                default:
                    throw new InvalidOperationException($"Mipmap format: {format} is not supported");
            }
        }
    }

    public class ImageResult
    {
        public byte[] Bytes { get; set; }
        public MipmapFormat Format { get; set; }
    }
}