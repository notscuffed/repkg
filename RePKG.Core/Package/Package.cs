using System.Collections.Generic;

namespace RePKG.Core.Package
{
    public class Package
    {
        public string Magic { get; set; }
        public int HeaderSize { get; set; }

        public List<PackageEntry> Entries { get; } = new List<PackageEntry>();
    }
}