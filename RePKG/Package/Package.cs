using System.Collections.Generic;
using System.IO;

namespace RePKG.Package
{
    public class Package
    {
        public string Version;
        public long HeaderSize;
        public readonly string Path;
        public readonly List<Entry> Entries;

        public int EntryCount => Entries.Count;

        public Package(string path)
        {
            Path = path;
            Entries = new List<Entry>();
        }

        public void Dump(string path)
        {
            var directoryInfo = new DirectoryInfo(path);

            if (!directoryInfo.Exists)
                Directory.CreateDirectory(path);

            var directory = directoryInfo.FullName;

            foreach (var entry in Entries)
            {
                entry.WriteTo(directory);
            }
        }
    }
}
