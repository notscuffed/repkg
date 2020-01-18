using System.Runtime.InteropServices;

namespace RePKG.Native
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct CBytesResult
    {
        public void* pointer;
        public int length;

        public CBytesResult(void* pointer, int length)
        {
            this.pointer = pointer;
            this.length = length;
        }
    }
}