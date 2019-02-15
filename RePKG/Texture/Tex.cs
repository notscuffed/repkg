using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;


namespace RePKG.Texture
{
    public class Tex
    {
        public string Magic; // always: TEXV0005
        public string Magic2; // always: TEXI0001
        public TexFormat Format;
        public int _unkInt_1; 
        public int TextureWidth;
        public int TextureHeight;
        public int ImageWidth;
        public int ImageHeight;
        public uint _unkInt_2;

        //public byte _unkByte_0;
        //public byte _unkByte_1;
        //public ushort _unkShort_0;

        public string TextureContainerMagic;
        public TexMipmapVersion TextureContainerVersion;
        public int _unkIntCont_0;
        public FreeImageFormat ImageFormat;
        public int MipmapCount;

        public List<TexMipmap> Mipmaps;

        public Tex()
        {
            Format = TexFormat.ARGB8888;
            ImageFormat = FreeImageFormat.FIF_UNKNOWN;
            Mipmaps = new List<TexMipmap>();
        }

        public void DebugInfo()
        {
            var type = typeof(Tex);
            var flags = BindingFlags.Instance | BindingFlags.Public;

            foreach (var field in type.GetFields(flags).Where(
                f => _membersToDebug.Contains(f.Name) || f.Name.StartsWith("_unk")))
                Console.WriteLine($@"{field.Name}: {field.GetValue(this)}");

            foreach (var property in type.GetProperties(flags).Where(
                f => _membersToDebug.Contains(f.Name) || f.Name.StartsWith("_unk")))
                Console.WriteLine($@"{property.Name}: {property.GetValue(this)}");
        }

        public byte[] Decompile()
        {
            var bytes = Mipmaps[0].Bytes;

            if (Mipmaps[0].LZ4Compressed == 1)
                bytes = TexLoader.DecompressLZ4(bytes, Mipmaps[0].PixelCount);

            if (ImageFormat != FreeImageFormat.FIF_UNKNOWN)
                return bytes;

            var invertedColorOrder = false;

            switch (Format)
            {
                case TexFormat.DXT5:
                    bytes = DXT.DecompressImage(Mipmaps[0].Width, Mipmaps[0].Height, bytes, DXT.DXTFlags.DXT5);
                    break;
                case TexFormat.DXT3:
                    bytes = DXT.DecompressImage(Mipmaps[0].Width, Mipmaps[0].Height, bytes, DXT.DXTFlags.DXT3);
                    break;
                case TexFormat.DXT1:
                    bytes = DXT.DecompressImage(Mipmaps[0].Width, Mipmaps[0].Height, bytes, DXT.DXTFlags.DXT1);
                    break;
                case TexFormat.ARGB8888:
                    invertedColorOrder = true;
                    break;
                default:
                    throw new NotImplementedException($"Format: \"{Format.ToString()}\" ({(int)Format})");
            }

            var width = ImageWidth;
            var textureWidth = Mipmaps[0].Width;
            var height = ImageHeight;
            var bitmap = new Bitmap(width, height);


            Helper.CopyRawPixelsIntoBitmap(bytes, textureWidth * 4, bitmap, invertedColorOrder);

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
                ImageFormat == FreeImageFormat.FIF_UNKNOWN ? FreeImageFormat.FIF_PNG : ImageFormat)))
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

        private static readonly string[] _membersToDebug = {"Format", "ImageFormat"};
    }

    public enum TexMipmapVersion
    {
        Version3,
        Version2
    }

    public enum TexFormat
    {
        ARGB8888,
        RA88,
        A8,
        DXT5,
        DXT3,
        DXT1
    }
}
