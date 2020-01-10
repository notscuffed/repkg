using System;

namespace RePKG.Core.Texture
{
    public static class TexMipmapFormatGetter
    {
        public static MipmapFormat GetFormatForTex(Tex tex)
        {
            return GetFormatForTex(tex.ImagesContainer.ImageFormat, tex.Header.Format);
        }
        
        public static MipmapFormat GetFormatForTex(FreeImageFormat imageFormat, TexFormat texFormat)
        {
            if (imageFormat != FreeImageFormat.FIF_UNKNOWN)
                return FreeImageFormatToMipmapFormat(imageFormat);
            
            switch (texFormat)
            {
                case TexFormat.RGBA8888:
                    return MipmapFormat.RGBA8888;
                case TexFormat.DXT5:
                    return MipmapFormat.CompressedDXT5;
                case TexFormat.DXT3:
                    return MipmapFormat.CompressedDXT3;
                case TexFormat.DXT1:
                    return MipmapFormat.CompressedDXT1;
                case TexFormat.R8:
                    return MipmapFormat.R8;
                case TexFormat.RG88:
                    return MipmapFormat.RG88;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static MipmapFormat FreeImageFormatToMipmapFormat(FreeImageFormat freeImageFormat)
        {
            switch (freeImageFormat)
            {
                case FreeImageFormat.FIF_UNKNOWN:
                    throw new Exception($"Can't convert '{freeImageFormat}' to '{typeof(MipmapFormat)}'");
                
                case FreeImageFormat.FIF_BMP:
                    return MipmapFormat.ImageBMP;
                
                case FreeImageFormat.FIF_ICO:
                    return MipmapFormat.ImageICO;
                
                case FreeImageFormat.FIF_JPEG:
                    return MipmapFormat.ImageJPEG;
                
                case FreeImageFormat.FIF_JNG:
                    return MipmapFormat.ImageJNG;
                
                case FreeImageFormat.FIF_KOALA:
                    return MipmapFormat.ImageKOALA;
                
                case FreeImageFormat.FIF_LBM:
                    return MipmapFormat.ImageLBM;
                
                case FreeImageFormat.FIF_MNG:
                    return MipmapFormat.ImageMNG;
                
                case FreeImageFormat.FIF_PBM:
                    return MipmapFormat.ImagePBM;
                
                case FreeImageFormat.FIF_PBMRAW:
                    return MipmapFormat.ImagePBMRAW;
                
                case FreeImageFormat.FIF_PCD:
                    return MipmapFormat.ImagePCD;
                
                case FreeImageFormat.FIF_PCX:
                    return MipmapFormat.ImagePCX;
                
                case FreeImageFormat.FIF_PGM:
                    return MipmapFormat.ImagePGM;
                
                case FreeImageFormat.FIF_PGMRAW:
                    return MipmapFormat.ImagePGMRAW;
                
                case FreeImageFormat.FIF_PNG:
                    return MipmapFormat.ImagePNG;
                
                case FreeImageFormat.FIF_PPM:
                    return MipmapFormat.ImagePPM;
                
                case FreeImageFormat.FIF_PPMRAW:
                    return MipmapFormat.ImagePPMRAW;
                
                case FreeImageFormat.FIF_RAS:
                    return MipmapFormat.ImageRAS;
                
                case FreeImageFormat.FIF_TARGA:
                    return MipmapFormat.ImageTARGA;
                
                case FreeImageFormat.FIF_TIFF:
                    return MipmapFormat.ImageTIFF;
                
                case FreeImageFormat.FIF_WBMP:
                    return MipmapFormat.ImageWBMP;
                
                case FreeImageFormat.FIF_PSD:
                    return MipmapFormat.ImagePSD;
                
                case FreeImageFormat.FIF_CUT:
                    return MipmapFormat.ImageCUT;
                
                case FreeImageFormat.FIF_XBM:
                    return MipmapFormat.ImageXBM;
                
                case FreeImageFormat.FIF_XPM:
                    return MipmapFormat.ImageXPM;
                
                case FreeImageFormat.FIF_DDS:
                    return MipmapFormat.ImageDDS;
                
                case FreeImageFormat.FIF_GIF:
                    return MipmapFormat.ImageGIF;
                
                case FreeImageFormat.FIF_HDR:
                    return MipmapFormat.ImageHDR;
                
                case FreeImageFormat.FIF_FAXG3:
                    return MipmapFormat.ImageFAXG3;
                
                case FreeImageFormat.FIF_SGI:
                    return MipmapFormat.ImageSGI;
                
                case FreeImageFormat.FIF_EXR:
                    return MipmapFormat.ImageEXR;
                
                case FreeImageFormat.FIF_J2K:
                    return MipmapFormat.ImageJ2K;
                
                case FreeImageFormat.FIF_JP2:
                    return MipmapFormat.ImageJP2;
                
                case FreeImageFormat.FIF_PFM:
                    return MipmapFormat.ImagePFM;
                
                case FreeImageFormat.FIF_PICT:
                    return MipmapFormat.ImagePICT;
                
                case FreeImageFormat.FIF_RAW:
                    return MipmapFormat.ImageRAW;
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(freeImageFormat), freeImageFormat, null);
            }
        }
    }
}