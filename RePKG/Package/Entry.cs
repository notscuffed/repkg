using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using RePKG.Texture;

namespace RePKG.Package
{
    public class Entry
    {
        public string Name;
        public string Extension;
        public int Offset;
        public int Length;
        public byte[] Data;
        public EntryType Type;

        public Entry(string name)
        {
            Name = name;
            Extension = Path.GetExtension(name);
        }

        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        public void WriteTo(string directoryPath, bool stripDirectory = false)
        {
            if (string.IsNullOrEmpty(directoryPath))
                throw new ArgumentException($"{nameof(directoryPath)} is null or empty");

            var name = Path.GetFileNameWithoutExtension(Name);
            var directoryName = Path.GetDirectoryName(Name);

            if (!stripDirectory && directoryName != null)
                name = Path.Combine(directoryName, name);
            
            var path = Path.Combine(directoryPath, name);
            var directory = Path.GetDirectoryName(path);

            if (string.IsNullOrEmpty(directory))
                throw new Exception($"Invalid directory: {directoryPath}");

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            var stream = File.Open(path + Extension, FileMode.Create, FileAccess.Write, FileShare.None);
            stream.Write(Data, 0, Data.Length);
            stream.Flush();
            stream.Close();

            if (Type == EntryType.TEX)
            {
                var bitmap = GetBitmap();
                bitmap.Save(path + ".png", ImageFormat.Png);
                bitmap.Dispose();
            }
        }

        public Tex GetTex() => TexLoader.LoadTex(Data);

        public Bitmap GetBitmap() => TexBitmapExtractor.Extract(GetTex());

        public override string ToString()
        {
            return $"0x{Length:X} {Name}";
        }
    }
}
