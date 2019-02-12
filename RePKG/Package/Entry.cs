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
        public string FullName;
        public int Offset;
        public int Length;
        public byte[] Data;
        public EntryType Type;
        
        public string EntryPath => Path.GetDirectoryName(FullName);
        public string Name => Path.GetFileNameWithoutExtension(FullName);
        public string Extension => Path.GetExtension(FullName);

        public Entry(string fullName)
        {
            FullName = fullName;

            if (Extension.Equals(".tex", StringComparison.OrdinalIgnoreCase))
                Type = EntryType.TEX;
        }

        public void WriteTo(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException($"{nameof(filePath)} is null or empty");

            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            var stream = File.Open(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
            stream.Write(Data, 0, Data.Length);
            stream.Flush();
            stream.Close();
        }

        public override string ToString()
        {
            return $"0x{Length:X} {EntryPath}";
        }
    }
}
