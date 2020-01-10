using System;

namespace RePKG.Core.Texture
{
    public static class MipmapFormatExtensions
    {
        /// <summary>
        /// Checks if the mipmap format is an image format
        /// </summary>
        public static bool IsImage(this MipmapFormat format)
        {
            return (int) format >= 1000;
        }
        
        /// <summary>
        /// Checks if the mipmap format is an raw uncompressed format
        /// </summary>
        public static bool IsRawFormat(this MipmapFormat format)
        {
            var formatId = (int) format;
            return formatId >= 1 && formatId <= 3;
        }
        
        /// <summary>
        /// Checks if the mipmap format is an raw compressed format
        /// </summary>
        public static bool IsCompressed(this MipmapFormat format)
        {
            switch (format)
            {
                case MipmapFormat.CompressedDXT5:
                case MipmapFormat.CompressedDXT3:
                case MipmapFormat.CompressedDXT1:
                    return true;
                
                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns file extension for an image format
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">When mipmap format isn't an image format'</exception>
        public static string GetFileExtension(this MipmapFormat format)
        {
            switch (format)
            {
                case MipmapFormat.ImageBMP:
                    return "bmp";
                case MipmapFormat.ImageICO:
                    return "ico";
                case MipmapFormat.ImageJPEG:
                    return "jpg";
                case MipmapFormat.ImageJNG:
                    return "jng";
                case MipmapFormat.ImageKOALA:
                    return "koa";
                case MipmapFormat.ImageLBM:
                    return "lbm";
                case MipmapFormat.ImageIFF:
                    return "iff";
                case MipmapFormat.ImageMNG:
                    return "mng";
                case MipmapFormat.ImagePBM:
                case MipmapFormat.ImagePBMRAW:
                    return "pbm";
                case MipmapFormat.ImagePCD:
                    return "pcd";
                case MipmapFormat.ImagePCX:
                    return "pcx";
                case MipmapFormat.ImagePGM:
                case MipmapFormat.ImagePGMRAW:
                    return "pgm";
                case MipmapFormat.ImagePNG:
                    return "png";
                case MipmapFormat.ImagePPM:
                case MipmapFormat.ImagePPMRAW:
                    return "ppm";
                case MipmapFormat.ImageRAS:
                    return "ras";
                case MipmapFormat.ImageTARGA:
                    return "tga";
                case MipmapFormat.ImageTIFF:
                    return "tif";
                case MipmapFormat.ImageWBMP:
                    return "wbmp";
                case MipmapFormat.ImagePSD:
                    return "psd";
                case MipmapFormat.ImageCUT:
                    return "cut";
                case MipmapFormat.ImageXBM:
                    return "xbm";
                case MipmapFormat.ImageXPM:
                    return "xpm";
                case MipmapFormat.ImageDDS:
                    return "dds";
                case MipmapFormat.ImageGIF:
                    return "gif";
                case MipmapFormat.ImageHDR:
                    return "hdr";
                case MipmapFormat.ImageFAXG3:
                    return "g3";
                case MipmapFormat.ImageSGI:
                    return "sgi";
                case MipmapFormat.ImageEXR:
                    return "exr";
                case MipmapFormat.ImageJ2K:
                    return "j2k";
                case MipmapFormat.ImageJP2:
                    return "jp2";
                case MipmapFormat.ImagePFM:
                    return "pfm";
                case MipmapFormat.ImagePICT:
                    return "pict";
                case MipmapFormat.ImageRAW:
                    return "raw";
                default:
                    throw new ArgumentOutOfRangeException(nameof(format), format, null);
            }
        }
    }
}