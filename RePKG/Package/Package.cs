using System;
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

        public Entry GetEntry(string name)
        {
            foreach (var entry in Entries)
            {
                if (entry.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    return entry;
            }

            return null;
        }
    }
}
