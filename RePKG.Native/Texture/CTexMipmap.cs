using System.IO;
using System.Runtime.InteropServices;
using RePKG.Core.Texture;

namespace RePKG.Native.Texture
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct CTexMipmap
    {
        public void* bytes;
        public int bytes_count;
        public int width;
        public int height;
        public int decompressed_bytes_count;
        public bool is_lz4_compressed;
        public MipmapFormat format;
    }

    public unsafe class WCTexMipmap : ITexMipmap
    {
        public readonly CTexMipmap* Self;
        private readonly NativeEnvironment _environment;

        private byte[] _bytes;
        private void* _lastBytesAddress;

        public WCTexMipmap(CTexMipmap* self, NativeEnvironment environment)
        {
            Self = self;
            _environment = environment;
        }

        public byte[] Bytes
        {
            get
            {
                RefreshBytes();
                return _bytes;
            }
            set
            {
                _environment.TryFree(Self->bytes);
                var address = _environment.Pin(value);
                Self->bytes = address;
            }
        }

        public int Width
        {
            get => Self->width;
            set => Self->width = value;
        }

        public int Height
        {
            get => Self->height;
            set => Self->height = value;
        }

        public int DecompressedBytesCount
        {
            get => Self->decompressed_bytes_count;
            set => Self->decompressed_bytes_count = value;
        }

        public bool IsLZ4Compressed
        {
            get => Self->is_lz4_compressed;
            set => Self->is_lz4_compressed = value;
        }

        public MipmapFormat Format
        {
            get => Self->format;
            set => Self->format = value;
        }

        public Stream GetBytesStream()
        {
            return new UnmanagedMemoryStream((byte*) Self->bytes, Self->bytes_count);
        }

        private void RefreshBytes()
        {
            var address = Self->bytes;

            if (_lastBytesAddress == address)
                return;

            _lastBytesAddress = address;
            _bytes = null;

            if (address == null)
                return;

            using (var stream = new UnmanagedMemoryStream((byte*) Self->bytes, Self->bytes_count))
            using (var memoryStream = new MemoryStream(Self->bytes_count))
            {
                stream.CopyTo(memoryStream);
                _bytes = memoryStream.GetBuffer();
            }
        }
    }
}