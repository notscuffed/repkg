using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RePKG.Texture
{
    public class Tex
    {
        public string Magic; // always: TEXV0005
        public string Magic2; // always: TEXI0001
        public int DxtCompression;
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

            //extract -d -t -o . .
        }

        private static readonly string[] _membersToDebug = {"DxtCompression", "SecondByte", "DxtDecompress", "TextureContainerMagic", "ImageFormat"};
    }

    public enum TexMipmapVersion
    {
        Version3,
        Version2
    }
}
