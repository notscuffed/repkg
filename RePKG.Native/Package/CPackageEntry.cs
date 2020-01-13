using System.Runtime.InteropServices;
using RePKG.Core.Package.Enums;

namespace RePKG.Native.Package
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct CPackageEntry
    {
        public void* full_path;
        public int offset;
        public int length;
        public void* bytes;
        public EntryType type;
    }
}