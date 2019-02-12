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
    }
}
