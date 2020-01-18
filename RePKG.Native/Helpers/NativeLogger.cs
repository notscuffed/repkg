using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RePKG.Native
{
    public static unsafe class NativeLogger
    {

        private const int LogBufferSize = 4096;
        private static readonly byte* _logBuffer;
        private static readonly IntPtr _logBufferPtr;
        private static LogDelegate _logAction;

        static NativeLogger()
        {
            _logBuffer = (byte*) Marshal.AllocHGlobal(LogBufferSize).ToPointer();
            _logBufferPtr = new IntPtr(_logBuffer);
            
            for (var i = 0; i < LogBufferSize; i++)
            {
                _logBuffer[i] = 0;
            }
        }

        public static void SetLogFunction(void* ptr)
        {
            _logAction = Marshal.GetDelegateForFunctionPointer<LogDelegate>(new IntPtr(ptr));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Verbose(string text) => Log(text, LogLevel.Verbose);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Debug(string text) => Log(text, LogLevel.Debug);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Info(string text) => Log(text, LogLevel.Information);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Warn(string text) => Log(text, LogLevel.Warning);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Error(string text) => Log(text, LogLevel.Error);
        
        public static void Log(string text, LogLevel level)
        {
            if (string.IsNullOrEmpty(text))
            {
                _logBuffer[0] = 0;
                LogInternal(level);
                return;
            }

            CharsetConverter.UTF16ToUTF8Buffer(text, _logBuffer, LogBufferSize);

            LogInternal(level);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void LogInternal(LogLevel level)
        {
            _logAction?.Invoke(_logBufferPtr, level);
        }

        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
        private delegate void LogDelegate(IntPtr log, LogLevel log_level);
    }

    public enum LogLevel
    {
        Verbose = -2,
        Debug = -1,
        Information = 0,
        Warning = 1,
        Error = 2
    }
}