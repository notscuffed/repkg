using System.Runtime.InteropServices;

namespace RePKG.Native.Package
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct CPackage
    {
        public void* magic;
        public int header_size;
        public int entry_count;
        public CPackageEntry* entries;
    }
}