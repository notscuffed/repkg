using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace RePKG.Native
{
    public unsafe class NativeEnvironment : IDisposable
    {
        private readonly Dictionary<IntPtr, (string Label, GCHandle Handle)> _pinned = 
            new Dictionary<IntPtr, (string, GCHandle)>();
        private readonly Dictionary<IntPtr, string> _allocated = new Dictionary<IntPtr, string>();

        public void* Pin(object objectToPin)
        {
            if (objectToPin == null)
                throw new ArgumentNullException(nameof(objectToPin));

            NativeLogger.Verbose($"Pinning: {TinyReflection.GetTypeName(objectToPin)}");
            
            var pin = GCHandle.Alloc(objectToPin, GCHandleType.Pinned);
            var intPtr = pin.AddrOfPinnedObject();
            _pinned[intPtr] = (TinyReflection.GetTypeName(objectToPin), pin);
            
            return intPtr.ToPointer();
        }

        public bool TryFree(IntPtr intPtr)
        {
            if (_allocated.TryGetValue(intPtr, out var value))
            {
                NativeLogger.Verbose($"Deallocating: {value}");
                
                Marshal.FreeHGlobal(intPtr);
                _allocated.Remove(intPtr);
                
                return true;
            }

            if (_pinned.TryGetValue(intPtr, out var pin))
            {
                NativeLogger.Verbose($"Unpinning: {pin.Label}");
                
                pin.Handle.Free();
                _pinned.Remove(intPtr);
                
                return true;
            }

            return false;
        }

        public bool TryFree(void* address) => TryFree(new IntPtr(address));

        public T* AllocateStruct<T>() where T : unmanaged
        {
            NativeLogger.Verbose($"Allocating: {TinyReflection.GetTypeName<T>()}");
            
            var intPtr = Marshal.AllocHGlobal(sizeof(T));
            _allocated.Add(intPtr, TinyReflection.GetTypeName<T>());
            return (T*) intPtr.ToPointer();
        }

        public T* AllocateStructArray<T>(int count = 1) where T : unmanaged
        {
            NativeLogger.Verbose($"Allocating: {TinyReflection.GetTypeName<T>()}[{count}]");

            var intPtr = Marshal.AllocHGlobal(sizeof(T) * count);
            _allocated.Add(intPtr, $"{TinyReflection.GetTypeName<T>()}[{count}]");
            return (T*) intPtr.ToPointer();
        }

        public void* ToCString(string input)
        {
            NativeLogger.Verbose($"Allocating: string '{input}'");

            var pointer = Marshal.StringToHGlobalAnsi(input);

            try
            {
                _allocated.Add(pointer, $"string: '{input}'");
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
            foreach (var pair in _allocated)
            {
                Marshal.FreeHGlobal(pair.Key);
                NativeLogger.Verbose($"Deallocating: {pair.Value}");
            }

            _allocated.Clear();

            foreach (var pin in _pinned)
            {
                pin.Value.Handle.Free();
                NativeLogger.Verbose($"Unpinning: {pin.Value.Label}");
            }

            _pinned.Clear();
        }

        ~NativeEnvironment()
        {
            Dispose(false);
        }
    }
}