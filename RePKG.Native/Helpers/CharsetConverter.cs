using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace RePKG.Native
{
    public static unsafe class CharsetConverter
    {
        private const int CP_ACP = 0;
        private const int CP_UTF8 = 65001;
        private const int WC_NO_BEST_FIT_CHARS = 0x400;
        
        [ThreadStatic]
        private static readonly byte[] buffer;

        static CharsetConverter()
        {
            buffer = new byte[4096];
        }

        // TODO: implement for platforms other than windows
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int UTF16ToUTF8Buffer(string input, byte* outputBuffer, int bufferSize)
        {
            int size;

            fixed (char* inputAddress = input)
            {
                size = WideCharToMultiByte(CP_UTF8, WC_NO_BEST_FIT_CHARS, inputAddress, input.Length, outputBuffer,
                    bufferSize, null, null);
            }

            outputBuffer[size] = 0;
            return size;
        }

        // TODO: implement for platforms other than windows
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string UTF8BufferToString(byte* inputBuffer, int bufferSize)
        {
            int size;

            fixed (byte* outputBuffer = buffer)
            {
                size = MultiByteToWideChar(CP_UTF8, 0, inputBuffer, bufferSize, outputBuffer, bufferSize) * 2;
            }

            return Encoding.Unicode.GetString(buffer, 0, size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int UTF8Length(byte* inputBuffer)
        {
            var length = 0;
            while (*inputBuffer++ != 0) length++;
            return length;
        }

        [DllImport("kernel32.dll", SetLastError = false)]
        private static extern int WideCharToMultiByte(
            uint codePage,
            uint dwFlags,
            char* lpWideCharStr,
            int cchWideChar,
            byte* lpMultiByteStr,
            int cbMultiByte,
            void* lpDefaultChar,
            void* lpUsedDefaultChar
        );

        [DllImport("kernel32.dll", SetLastError = false)]
        private static extern int MultiByteToWideChar(
            uint codePage,
            uint dwFlags,
            byte* lpMultiByteStr,
            int cbMultiByte,
            byte* lpWideCharStr,
            int cchWideChar);
    }
}