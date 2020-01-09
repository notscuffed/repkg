namespace RePKG.Core.Texture
{
    public enum MipmapFormat
    {
        Invalid = 0,
        
        /// <summary>
        /// Raw pixels (4 bytes per pixel) (RGBA8888)
        /// </summary>
        RGBA8888 = 1,

        /// <summary>
        /// Raw pixels (1 byte per pixel) (R8)
        /// </summary>
        R8 = 2,

        /// <summary>
        /// Raw pixels (2 bytes per pixel) (RG88)
        /// </summary>
        RG88 = 3,

        /// <summary>
        /// Raw pixels compressed using DXT5
        /// </summary>
        CompressedDXT5,

        /// <summary>
        /// Raw pixels compressed using DXT3
        /// </summary>
        CompressedDXT3,

        /// <summary>
        /// Raw pixels compressed using DXT1
        /// </summary>
        CompressedDXT1,

        /// <summary>
        /// Windows or OS/2 Bitmap File (*.BMP)
        /// </summary>
        /// Keep '= 1000' because MipmapFormatExtensions.IsImage uses this to check if format is an image format
        ImageBMP = 1000,

        /// <summary>
        /// Windows Icon (*.ICO)
        /// </summary>
        ImageICO,

        /// <summary>
        /// Independent JPEG Group (*.JPG, *.JIF, *.JPEG, *.JPE)
        /// </summary>
        ImageJPEG,

        /// <summary>
        /// JPEG Network Graphics (*.JNG)
        /// </summary>
        ImageJNG,

        /// <summary>
        /// Commodore 64 Koala format (*.KOA)
        /// </summary>
        ImageKOALA,

        /// <summary>
        /// Amiga IFF (*.IFF, *.LBM)
        /// </summary>
        ImageLBM,

        /// <summary>
        /// Amiga IFF (*.IFF, *.LBM)
        /// </summary>
        ImageIFF,

        /// <summary>
        /// Multiple Network Graphics (*.MNG)
        /// </summary>
        ImageMNG,

        /// <summary>
        /// Portable Bitmap (ASCII) (*.PBM)
        /// </summary>
        ImagePBM,

        /// <summary>
        /// Portable Bitmap (BINARY) (*.PBM)
        /// </summary>
        ImagePBMRAW,

        /// <summary>
        /// Kodak PhotoCD (*.PCD)
        /// </summary>
        ImagePCD,

        /// <summary>
        /// Zsoft Paintbrush PCX bitmap format (*.PCX)
        /// </summary>
        ImagePCX,

        /// <summary>
        /// Portable Graymap (ASCII) (*.PGM)
        /// </summary>
        ImagePGM,

        /// <summary>
        /// Portable Graymap (BINARY) (*.PGM)
        /// </summary>
        ImagePGMRAW,

        /// <summary>
        /// Portable Network Graphics (*.PNG)
        /// </summary>
        ImagePNG,

        /// <summary>
        /// Portable Pixelmap (ASCII) (*.PPM)
        /// </summary>
        ImagePPM,

        /// <summary>
        /// Portable Pixelmap (BINARY) (*.PPM)
        /// </summary>
        ImagePPMRAW,

        /// <summary>
        /// Sun Rasterfile (*.RAS)
        /// </summary>
        ImageRAS,

        /// <summary>
        /// truevision Targa files (*.TGA, *.TARGA)
        /// </summary>
        ImageTARGA,

        /// <summary>
        /// Tagged Image File Format (*.TIF, *.TIFF)
        /// </summary>
        ImageTIFF,

        /// <summary>
        /// Wireless Bitmap (*.WBMP)
        /// </summary>
        ImageWBMP,

        /// <summary>
        /// Adobe Photoshop (*.PSD)
        /// </summary>
        ImagePSD,

        /// <summary>
        /// Dr. Halo (*.CUT)
        /// </summary>
        ImageCUT,

        /// <summary>
        /// X11 Bitmap Format (*.XBM)
        /// </summary>
        ImageXBM,

        /// <summary>
        /// X11 Pixmap Format (*.XPM)
        /// </summary>
        ImageXPM,

        /// <summary>
        /// DirectDraw Surface (*.DDS)
        /// </summary>
        ImageDDS,

        /// <summary>
        /// Graphics Interchange Format (*.GIF)
        /// </summary>
        ImageGIF,

        /// <summary>
        /// High Dynamic Range (*.HDR)
        /// </summary>
        ImageHDR,

        /// <summary>
        /// Raw Fax format CCITT G3 (*.G3)
        /// </summary>
        ImageFAXG3,

        /// <summary>
        /// Silicon Graphics SGI image format (*.SGI)
        /// </summary>
        ImageSGI,

        /// <summary>
        /// OpenEXR format (*.EXR)
        /// </summary>
        ImageEXR,

        /// <summary>
        /// JPEG-2000 format (*.J2K, *.J2C)
        /// </summary>
        ImageJ2K,

        /// <summary>
        /// JPEG-2000 format (*.JP2)
        /// </summary>
        ImageJP2,

        /// <summary>
        /// Portable FloatMap (*.PFM)
        /// </summary>
        ImagePFM,

        /// <summary>
        /// Macintosh PICT (*.PICT)
        /// </summary>
        ImagePICT,

        /// <summary>
        /// RAW camera image (*.*)
        /// </summary>
        ImageRAW,
    }
}