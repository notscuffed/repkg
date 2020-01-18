using RePKG.Core.Package;

namespace RePKG.Native.Package
{
    public static unsafe class PackageConverter
    {
        public static CPackage* ConvertToCPackage(
            this NativeEnvironment e,
            Core.Package.Package package)
        {
            var pkg = e.AllocateStruct<CPackage>();

            pkg->magic = e.ToCString(package.Magic);
            pkg->header_size = package.HeaderSize;
            pkg->entry_count = package.Entries.Count;

            if (package.Entries.Count == 0)
            {
                pkg->entries = null;
                return pkg;
            }

            var entryArray = e.AllocateStructArray<CPackageEntry>(package.Entries.Count);

            for (var i = 0; i < package.Entries.Count; i++)
            {
                ConvertCPackageEntry(e, package.Entries[i], &entryArray[i]);
            }

            pkg->entries = entryArray;

            return pkg;
        }

        private static void ConvertCPackageEntry(
            NativeEnvironment e,
            PackageEntry src,
            CPackageEntry* dst)
        {
            dst->offset = src.Offset;
            dst->length = src.Length;
            dst->bytes = src.Bytes == null ? null : e.Pin(src.Bytes);
            dst->type = src.Type;
            dst->full_path = e.ToCString(src.FullPath);
        }
    }
}