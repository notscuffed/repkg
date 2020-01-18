enum MipmapFormat {
    MF_Invalid = 0,
    
    /// <summary>
    /// Raw pixels (4 bytes per pixel) (RGBA8888)
    /// </summary>
    MF_RGBA8888 = 1,

    /// <summary>
    /// Raw pixels (1 byte per pixel) (R8)
    /// </summary>
    MF_R8 = 2,

    /// <summary>
    /// Raw pixels (2 bytes per pixel) (RG88)
    /// </summary>
    MF_RG88 = 3,

    /// <summary>
    /// Raw pixels compressed using DXT5
    /// </summary>
    MF_CompressedDXT5,

    /// <summary>
    /// Raw pixels compressed using DXT3
    /// </summary>
    MF_CompressedDXT3,

    /// <summary>
    /// Raw pixels compressed using DXT1
    /// </summary>
    MF_CompressedDXT1,

    /// <summary>
    /// Windows or OS/2 Bitmap File (*.BMP)
    /// </summary>
    /// Keep '= 1000' because MipmapFormatExtensions.IsImage uses this to check if format is an image format
    MF_ImageBMP = 1000,

    /// <summary>
    /// Windows Icon (*.ICO)
    /// </summary>
    MF_ImageICO,

    /// <summary>
    /// Independent JPEG Group (*.JPG, *.JIF, *.JPEG, *.JPE)
    /// </summary>
    MF_ImageJPEG,

    /// <summary>
    /// JPEG Network Graphics (*.JNG)
    /// </summary>
    MF_ImageJNG,

    /// <summary>
    /// Commodore 64 Koala format (*.KOA)
    /// </summary>
    MF_ImageKOALA,

    /// <summary>
    /// Amiga IFF (*.IFF, *.LBM)
    /// </summary>
    MF_ImageLBM,

    /// <summary>
    /// Amiga IFF (*.IFF, *.LBM)
    /// </summary>
    MF_ImageIFF,

    /// <summary>
    /// Multiple Network Graphics (*.MNG)
    /// </summary>
    MF_ImageMNG,

    /// <summary>
    /// Portable Bitmap (ASCII) (*.PBM)
    /// </summary>
    MF_ImagePBM,

    /// <summary>
    /// Portable Bitmap (BINARY) (*.PBM)
    /// </summary>
    MF_ImagePBMRAW,

    /// <summary>
    /// Kodak PhotoCD (*.PCD)
    /// </summary>
    MF_ImagePCD,

    /// <summary>
    /// Zsoft Paintbrush PCX bitmap format (*.PCX)
    /// </summary>
    MF_ImagePCX,

    /// <summary>
    /// Portable Graymap (ASCII) (*.PGM)
    /// </summary>
    MF_ImagePGM,

    /// <summary>
    /// Portable Graymap (BINARY) (*.PGM)
    /// </summary>
    MF_ImagePGMRAW,

    /// <summary>
    /// Portable Network Graphics (*.PNG)
    /// </summary>
    MF_ImagePNG,

    /// <summary>
    /// Portable Pixelmap (ASCII) (*.PPM)
    /// </summary>
    MF_ImagePPM,

    /// <summary>
    /// Portable Pixelmap (BINARY) (*.PPM)
    /// </summary>
    MF_ImagePPMRAW,

    /// <summary>
    /// Sun Rasterfile (*.RAS)
    /// </summary>
    MF_ImageRAS,

    /// <summary>
    /// truevision Targa files (*.TGA, *.TARGA)
    /// </summary>
    MF_ImageTARGA,

    /// <summary>
    /// Tagged Image File Format (*.TIF, *.TIFF)
    /// </summary>
    MF_ImageTIFF,

    /// <summary>
    /// Wireless Bitmap (*.WBMP)
    /// </summary>
    MF_ImageWBMP,

    /// <summary>
    /// Adobe Photoshop (*.PSD)
    /// </summary>
    MF_ImagePSD,

    /// <summary>
    /// Dr. Halo (*.CUT)
    /// </summary>
    MF_ImageCUT,

    /// <summary>
    /// X11 Bitmap Format (*.XBM)
    /// </summary>
    MF_ImageXBM,

    /// <summary>
    /// X11 Pixmap Format (*.XPM)
    /// </summary>
    MF_ImageXPM,

    /// <summary>
    /// DirectDraw Surface (*.DDS)
    /// </summary>
    MF_ImageDDS,

    /// <summary>
    /// Graphics Interchange Format (*.GIF)
    /// </summary>
    MF_ImageGIF,

    /// <summary>
    /// High Dynamic Range (*.HDR)
    /// </summary>
    MF_ImageHDR,

    /// <summary>
    /// Raw Fax format CCITT G3 (*.G3)
    /// </summary>
    MF_ImageFAXG3,

    /// <summary>
    /// Silicon Graphics SGI image format (*.SGI)
    /// </summary>
    MF_ImageSGI,

    /// <summary>
    /// OpenEXR format (*.EXR)
    /// </summary>
    MF_ImageEXR,

    /// <summary>
    /// JPEG-2000 format (*.J2K, *.J2C)
    /// </summary>
    MF_ImageJ2K,

    /// <summary>
    /// JPEG-2000 format (*.JP2)
    /// </summary>
    MF_ImageJP2,

    /// <summary>
    /// Portable FloatMap (*.PFM)
    /// </summary>
    MF_ImagePFM,

    /// <summary>
    /// Macintosh PICT (*.PICT)
    /// </summary>
    MF_ImagePICT,

    /// <summary>
    /// RAW camera image (*.*)
    /// </summary>
    MF_ImageRAW,
};