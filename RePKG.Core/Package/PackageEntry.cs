using System.IO;
using RePKG.Core.Package.Enums;

namespace RePKG.Core.Package
{
    public class PackageEntry
    {
        public string FullPath { get; set; }
        public int Offset { get; set; }
        public int Length { get; set; }
        public byte[] Bytes { get; set; }
        public EntryType Type { get; set; }
        
        public string Name => Path.GetFileNameWithoutExtension(FullPath);
        public string Extension => Path.GetExtension(FullPath);
        public string DirectoryPath => Path.GetDirectoryName(FullPath);
    }
}