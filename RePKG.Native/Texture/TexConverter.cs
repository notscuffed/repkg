using System;
using RePKG.Core.Texture;

namespace RePKG.Native.Texture
{
    public static unsafe class TexConverter
    {
        public static CTex* ConvertToCTex(
            this NativeEnvironment e,
            Tex tex)
        {
            // tex
            var ctex = e.AllocateStruct<CTex>();
            ctex->magic1 = e.ToCString(tex.Magic1);
            ctex->magic2 = e.ToCString(tex.Magic2);

            // header
            var header = tex.Header;
            ctex->header.format = header.Format;
            ctex->header.flags = header.Flags;
            ctex->header.texture_width = header.TextureWidth;
            ctex->header.texture_height = header.TextureHeight;
            ctex->header.image_width = header.ImageWidth;
            ctex->header.image_height = header.ImageHeight;
            ctex->header.unk_int0 = header.UnkInt0;

            // images container
            var imgContainer = tex.ImagesContainer;
            var images = imgContainer.Images;
            ctex->images_container.magic = e.ToCString(imgContainer.Magic);
            ctex->images_container.image_format = imgContainer.ImageFormat;
            ctex->images_container.image_count = images.Count;
            ctex->images_container.container_version = imgContainer.ImageContainerVersion;

            if (images.Count == 0)
            {
                ctex->images_container.images = null;
            }
            else
            {
                var cimages = e.AllocateStructArray<CTexImage>(images.Count);
                ctex->images_container.images = cimages;

                for (var i = 0; i < images.Count; i++)
                {
                    ConvertCTexImage(e, images[i], &cimages[i]);
                }
            }

            // frame info container
            var frameContainer = tex.FrameInfoContainer;
            if (frameContainer == null)
            {
                ctex->frameinfo_container = null;
                return ctex;
            }

            var frames = frameContainer.Frames;
            ctex->frameinfo_container = e.AllocateStruct<CTexFrameInfoContainer>();
            ctex->frameinfo_container->magic = e.ToCString(frameContainer.Magic);
            ctex->frameinfo_container->frame_count = frames.Count;
            ctex->frameinfo_container->gif_width = frameContainer.GifWidth;
            ctex->frameinfo_container->gif_height = frameContainer.GifHeight;

            if (frames.Count == 0)
            {
                ctex->frameinfo_container->frames = null;
                return ctex;
            }

            var cframes = e.AllocateStructArray<CTexFrameInfo>(frames.Count);
            ctex->frameinfo_container->frames = cframes;

            for (var i = 0; i < frames.Count; i++)
            {
                ConvertCTexFrameInfo(e, frames[i], &cframes[i]);
            }

            return ctex;
        }

        private static void ConvertCTexImage(
            NativeEnvironment e,
            TexImage src,
            CTexImage* dst)
        {
            var mipmaps = src.Mipmaps;
            dst->mipmap_count = mipmaps.Count;

            if (mipmaps.Count == 0)
            {
                dst->mipmaps = null;
                return;
            }

            var cmipmaps = e.AllocateStructArray<CTexMipmap>(mipmaps.Count);
            dst->mipmaps = cmipmaps;

            for (var i = 0; i < mipmaps.Count; i++)
            {
                ConvertCTexMipmap(e, mipmaps[i], &cmipmaps[i]);
            }
        }

        private static void ConvertCTexMipmap(
            NativeEnvironment e,
            TexMipmap src,
            CTexMipmap* dst)
        {
            dst->bytes = e.Pin(src.Bytes);
            dst->bytes_count = src.Bytes.Length;
            dst->width = src.Width;
            dst->height = src.Height;
            dst->decompressed_bytes_count = src.DecompressedBytesCount;
            dst->is_lz4_compressed = src.IsLZ4Compressed;
            dst->format = src.Format;
        }

        private static void ConvertCTexFrameInfo(
            NativeEnvironment e,
            TexFrameInfo src,
            CTexFrameInfo* dst)
        {
            dst->image_id = src.ImageId;
            dst->frametime = src.Frametime;
            dst->x = src.X;
            dst->y = src.Y;
            dst->width = src.Width;
            dst->unk0 = src.Unk0;
            dst->unk1 = src.Unk1;
            dst->height = src.Height;
        }
    }
}