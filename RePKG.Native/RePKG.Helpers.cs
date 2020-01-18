using System;
using System.IO;

namespace RePKG.Native
{
    public static unsafe partial class RePKG
    {
        private static bool TryGetExistingFilePath(byte* pathPtr, out string outPath)
        {
            try
            {
                var length = CharsetConverter.UTF8Length(pathPtr);

                if (length == 0)
                    Error("Path is null or empty");
                
                outPath = CharsetConverter.UTF8BufferToString(pathPtr, length);

                if (string.IsNullOrWhiteSpace(outPath))
                    return Error("Path is empty");

                if (!File.Exists(outPath))
                    return Error($"File doesn't exist: '{outPath}'");

                return true;
            }
            catch (Exception exception)
            {
                outPath = null;
                return HandleException(exception);
            }
        }
        
        private static bool ValidFilePath(byte* pathPointer, out string outPath)
        {
            var length = CharsetConverter.UTF8Length(pathPointer);

            if (length == 0)
                Error("Path is null or empty");
                
            outPath = CharsetConverter.UTF8BufferToString(pathPointer, length);

            if (string.IsNullOrWhiteSpace(outPath))
                return Error("Path is empty");
            
            return true;
        }

        public static bool TryGetEnvironment<T>(T* pointer, out NativeEnvironment outEnvironment) where T : unmanaged
        {
            outEnvironment = null;

            if (pointer == null)
                return Error("Argument is a null pointer");

            if (!_environments.TryGetValue(new IntPtr(pointer), out var environment)) 
                return Error("Failed to get environment for tex");
            
            outEnvironment = environment;
            return true;
        }
        
        public static bool TryGetEnvironment(void* pointer, out NativeEnvironment outEnvironment) 
        {
            outEnvironment = null;

            if (pointer == null)
                return Error("Argument is a null pointer");

            if (!_environments.TryGetValue(new IntPtr(pointer), out var environment)) 
                return Error("Failed to get environment for tex");
            
            outEnvironment = environment;
            return true;
        }
        
        public static NativeEnvironment GetEnvironmentFor<T>(T* pointer) where T : unmanaged
        {
            if (pointer == null)
                return null;

            return _environments[new IntPtr(pointer)];
        }

        private static bool Error(string error)
        {
            if (string.IsNullOrEmpty(error))
            {
                _errorBuffer[0] = 0;
                return false;
            }

            CharsetConverter.UTF16ToUTF8Buffer(error, _errorBuffer, ErrorBufferSize);

            return false;
        }

        private static bool HandleException(Exception exception)
        {
            var message = exception.ToString();
            NativeLogger.Error(message);
            return Error(message);
        }
    }
}