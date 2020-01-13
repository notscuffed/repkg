using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace RePKG.Native
{
    public unsafe class NativeEnvironment : IDisposable
    {
        private readonly List<GCHandle> _pinned = new List<GCHandle>();
        private readonly List<IntPtr> _allocated = new List<IntPtr>();

        public void* Pin(object o)
        {
            var pin = GCHandle.Alloc(o, GCHandleType.Pinned);
            _pinned.Add(pin);
            return pin.AddrOfPinnedObject().ToPointer();
        }

        public T* AllocateStruct<T>() where T : unmanaged
        {
            var intPtr = Marshal.AllocHGlobal(sizeof(T));
            _allocated.Add(intPtr);
            return (T*) intPtr.ToPointer();
        }

        public T* AllocateStructArray<T>(int count = 1) where T : unmanaged
        {
            var intPtr = Marshal.AllocHGlobal(sizeof(T) * count);
            _allocated.Add(intPtr);
            return (T*) intPtr.ToPointer();
        }

        public void* ToCString(string input)
        {
            var pointer = Marshal.StringToHGlobalAnsi(input);

            try
            {
                _allocated.Add(pointer);
            }
            catch
            {
                Marshal.FreeHGlobal(pointer);
                throw;
            }

            return pointer.ToPointer();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            foreach (var ptr in _allocated)
            {
                Marshal.FreeHGlobal(ptr);
            }

            _allocated.Clear();

            foreach (var pin in _pinned)
            {
                pin.Free();
            }

            _pinned.Clear();
        }

        ~NativeEnvironment()
        {
            Dispose(false);
        }
    }
}