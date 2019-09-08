using System.Collections.Generic;

namespace RePKG.Core.Package
{
    public class Package
    {
        public string Magic { get; set; }
        public int HeaderSize { get; set; }

        public readonly List<PackageEntry> Entries;

        public Package()
        {
            Entries = new List<PackageEntry>();
        }
    }
}