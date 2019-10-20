using System.IO;
using RePKG.Core.Package.Enums;

namespace RePKG.Core.Package
{
    public static class PackageEntryTypeGetter
    {
        public static EntryType GetFromFileName(string path)
        {
            var extension = Path.GetExtension(path);

            if (string.IsNullOrWhiteSpace(extension))
                return EntryType.Binary;

            switch (extension.ToLower())
            {
                case ".tex":
                    return EntryType.Tex;

                default:
                    return EntryType.Binary;
            }
        }
    }
}