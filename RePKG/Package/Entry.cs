using System;
using System.IO;
using RePKG.Properties;

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
                Type = EntryType.Tex;
        }

        public void WriteTo(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException($"{nameof(filePath)} is null or empty");

            if (filePath.Length > 255)
            {
                Console.WriteLine(Resources.PathTooLong, filePath);
                return;
            }

            var directoryName = Path.GetDirectoryName(filePath);
            if (directoryName == null)
            {
                Console.WriteLine(Resources.SkippingBecauseNull, filePath, nameof(directoryName));
                return;
            }

            Directory.CreateDirectory(directoryName);

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
