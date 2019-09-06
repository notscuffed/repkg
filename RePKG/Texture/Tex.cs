using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using RePKG.Core.Texture;

namespace RePKG.Texture
{
    public class Tex
    {
        public string Magic; // always: TEXV0005
        public string Magic2; // always: TEXI0001
        public int FormatId;
        public TexFormat Format;
        public TexFlags Flags;
        public int TextureWidth;
        public int TextureHeight;
        public int ImageWidth;
        public int ImageHeight;
        public uint UnkInt0;
        public string TextureContainerMagic;
        public TexMipmapVersion TextureContainerVersion;
        public int UnkIntCont0;
        public FreeImageFormat ImageFormat;
        public int MipmapCount;

        public readonly List<TexMipmap> Mipmaps;

        public bool IsGif => (Flags & TexFlags.IsGif) == TexFlags.IsGif;

        public Tex()
        {
            Format = TexFormat.ARGB8888;
            ImageFormat = FreeImageFormat.FIF_UNKNOWN;
            Mipmaps = new List<TexMipmap>();
        }

        public byte[] Decompile()
        {
            var bytes = Mipmaps[0].Bytes;

            if (Mipmaps[0].Lz4Compressed == 1)
                bytes = TexLoader.DecompressLz4(bytes, Mipmaps[0].PixelCount);

            if (ImageFormat != FreeImageFormat.FIF_UNKNOWN)
                return bytes;

            var width = ImageWidth;
            var textureWidth = Mipmaps[0].Width;
            var height = ImageHeight;
            var bitmap = new Bitmap(width, height);

            switch (Format)
            {
                case TexFormat.DXT5:
                    bytes = DXT.DecompressImage(Mipmaps[0].Width, Mipmaps[0].Height, bytes, DXT.DXTFlags.DXT5);
                    Helper.CopyRawPixelsIntoBitmap(bytes, textureWidth, bitmap, true);
                    break;

                case TexFormat.DXT3:
                    bytes = DXT.DecompressImage(Mipmaps[0].Width, Mipmaps[0].Height, bytes, DXT.DXTFlags.DXT3);
                    Helper.CopyRawPixelsIntoBitmap(bytes, textureWidth, bitmap, true);
                    break;

                case TexFormat.DXT1:
                    bytes = DXT.DecompressImage(Mipmaps[0].Width, Mipmaps[0].Height, bytes, DXT.DXTFlags.DXT1);
                    Helper.CopyRawPixelsIntoBitmap(bytes, textureWidth, bitmap, true);
                    break;

                case TexFormat.R8:
                    Helper.CopyRawR8PixelsIntoBitmap(bytes, textureWidth, bitmap);
                    break;
                
                case TexFormat.RG88:
                    Helper.CopyRawRG88PixelsIntoBitmap(bytes, textureWidth, bitmap);
                    break;

                case TexFormat.ARGB8888:
                    Helper.CopyRawPixelsIntoBitmap(bytes, textureWidth, bitmap, true);
                    break;
                default:
                    throw new NotImplementedException($"Format: \"{Format.ToString()}\" ({(int) Format})");
            }

            var stream = new MemoryStream();
            bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            bitmap.Dispose();
            bytes = stream.GetBuffer();
            stream.Close();

            return bytes;
        }

        public void DecompileAndSave(string path, bool overwrite)
        {
            if (!overwrite && File.Exists(path + Helper.GetExtension(
                                              ImageFormat == FreeImageFormat.FIF_UNKNOWN
                                                  ? FreeImageFormat.FIF_PNG
                                                  : ImageFormat)))
                return;

            var bytes = Decompile();

            if (ImageFormat == FreeImageFormat.FIF_UNKNOWN)
                ImageFormat = FreeImageFormat.FIF_PNG;

            File.WriteAllBytes(path + Helper.GetExtension(ImageFormat), bytes);
        }

        public void SaveFormatInfo(string path, bool overwrite)
        {
            path = path + ".tex-json";

            if (!overwrite && File.Exists(path))
                return;

            var format = Format.ToString().Split('.').Last().ToLower();

            // ReSharper disable LocalizableElement
            File.WriteAllText(path, $"{{\r\n\t\"format\" : \"{format}\"\r\n}}");
        }
    }

    public enum TexMipmapVersion
    {
        Version3,
        Version2,
        Version1
    }

    [Flags]
    public enum TexFlags
    {
        NoInterpolation = 1,
        ClampUVs = 2,
        IsGif = 4,

        // Placeholders
        Unk3 = 8,
        Unk4 = 16,
        Unk5 = 32,
        Unk6 = 64,
        Unk7 = 128,
    }
}