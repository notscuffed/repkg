using System;
using System.Runtime.InteropServices;

namespace RePKG.Native
{
    public unsafe class WCString
    {
        private readonly NativeEnvironment _environment;
        private string _cached;
        private readonly void** _pointer;
        private void* _lastPointer;
        
        public WCString(void** pointerToCStringPointer, NativeEnvironment environment)
        {
            _pointer = pointerToCStringPointer;
            _environment = environment;
        }

        public string Value
        {
            get
            {
                var newPointer = *_pointer;

                if (newPointer == _lastPointer)
                    return _cached;
                
                _lastPointer = newPointer;
                _cached = newPointer == null ? null : Marshal.PtrToStringAnsi(new IntPtr(_lastPointer));

                return _cached;
            }
            set
            {
                _environment.TryFree(_lastPointer);
                *_pointer = _environment.ToCString(value);
                _lastPointer = _pointer;
                _cached = value;
            }
        }
    }
}