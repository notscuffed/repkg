using System;
using System.Collections.Concurrent;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using RePKG.Application.Package;
using RePKG.Application.Texture;
using RePKG.Native.Package;
using RePKG.Native.Texture;

namespace RePKG.Native
{
    public static unsafe class RePKG
    {
        private const int ErrorBufferSize = 1024;
        
        private static readonly ConcurrentDictionary<long, NativeEnvironment> _environments;
        private static readonly byte* _errorBuffer;

        private static readonly TexReader _texReader;
        
        static RePKG()
        {
            _environments = new ConcurrentDictionary<long, NativeEnvironment>();
            _errorBuffer = (byte*) Marshal.AllocHGlobal(ErrorBufferSize).ToPointer();

            for (var i = 0; i < ErrorBufferSize; i++)
            {
                _errorBuffer[i] = 0;
            }
            
            var headerReader = new TexHeaderReader();
            var mipmapDecompressor = new TexMipmapDecompressor();
            var mipmapReader = new TexImageReader(mipmapDecompressor);
            var containerReader = new TexImageContainerReader(mipmapReader);
            var frameInfoReader = new TexFrameInfoContainerReader();
            
            _texReader = new TexReader(headerReader, containerReader, frameInfoReader);
        }
        
        [NativeCallable(EntryPoint = "pkg_load")]
        public static CPackage* pkg_load(IntPtr pathPtr, bool readEntryBytes)
        {
            var e = new NativeEnvironment();
            var environmentId = 0;

            Interlocked.Increment(ref environmentId);

            try
            {
                var path = Marshal.PtrToStringAnsi(pathPtr);

                if (string.IsNullOrWhiteSpace(path))
                {
                    SetError("Path is null or empty");
                    return null;
                }

                if (!File.Exists(path))
                {
                    SetError("File doesn't exist");
                    return null;
                }

                var reader = new PackageReader {ReadEntryBytes = readEntryBytes};

 
                using (var stream = File.OpenRead(path))
                using (var binaryReader = new BinaryReader(stream, Encoding.UTF8))
                {
                    reader.ReadEntryBytes = readEntryBytes;
                    var package = reader.ReadFrom(binaryReader);

                    var cpackage = e.ConvertToCPackage(package);

                    _environments[(long) cpackage] = e;
                    
                    return cpackage;
                }
            }
            catch (Exception exception)
            {
                e.Dispose();
                SetError(exception.ToString());
                return null;
            }
        }

        [NativeCallable(EntryPoint = "pkg_destroy")]
        public static bool pkg_destroy(IntPtr pkg)
        {
            try
            {
                if (!_environments.TryRemove(pkg.ToInt64(), out var e))
                {
                    SetError("There is no package at this address");
                    return false;
                }

                e.Dispose();
                return true;
            }
            catch (Exception exception)
            {
                SetError(exception.ToString());
                return false;
            }
        }
        
        [NativeCallable(EntryPoint = "tex_load_file")]
        public static CTex* tex_load_file(IntPtr pathPtr)
        {
            var path = Marshal.PtrToStringAnsi(pathPtr);

            if (string.IsNullOrWhiteSpace(path))
            {
                SetError("Path is null or empty");
                return null;
            }

            if (!File.Exists(path))
            {
                SetError($"File doesn't exist: {path}");
                return null;
            }

            try
            {
                using (var fileStream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    return tex_load_impl(fileStream);
                }
            }
            catch (Exception exception)
            {
                SetError(exception.ToString());
                return null;
            }
        }
        
        [NativeCallable(EntryPoint = "tex_load")]
        public static CTex* tex_load(byte* memoryPointer, long size)
        {
            if (memoryPointer == null)
            {
                SetError("Memory pointer is a null pointer");
                return null;
            }

            if (size <= 0)
            {
                SetError($"Invalid size: {size}");
                return null;
            }
            
            return tex_load_impl(new UnmanagedMemoryStream(memoryPointer, size));
        }

        private static CTex* tex_load_impl(Stream stream)
        {
            var e = new NativeEnvironment();
            var environmentId = 0;

            Interlocked.Increment(ref environmentId);

            try
            {
                using (var binaryReader = new BinaryReader(stream, Encoding.UTF8))
                {
                    var tex = _texReader.ReadFrom(binaryReader);

                    var ctex = e.ConvertToCTex(tex);

                    _environments[(long) ctex] = e;
                    
                    return ctex;
                }
            }
            catch (Exception exception)
            {
                e.Dispose();
                SetError(exception.ToString());
                return null;
            }
        }

        [NativeCallable(EntryPoint = "get_last_error")]
        public static byte* get_last_error()
        {
            return _errorBuffer;
        }

        private static void SetError(string error)
        {
            if (string.IsNullOrEmpty(error))
                return;

            var bytes = Encoding.ASCII.GetBytes(error.Substring(0, Math.Min(error.Length, ErrorBufferSize)));

            for (var i = 0; i < bytes.Length; i++)
            {
                _errorBuffer[i] = bytes[i];
            }

            _errorBuffer[bytes.Length + 1] = 0;
        }
    }
}