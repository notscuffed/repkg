using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace RePKG.Native
{
    public static unsafe partial class RePKG
    {
        private const int ErrorBufferSize = 4096;
        private static readonly byte* _errorBuffer;

        private static readonly Dictionary<IntPtr, NativeEnvironment> _environments;

        static RePKG()
        {
            _environments = new Dictionary<IntPtr, NativeEnvironment>();
            _errorBuffer = (byte*) Marshal.AllocHGlobal(ErrorBufferSize).ToPointer();

            for (var i = 0; i < ErrorBufferSize; i++)
            {
                _errorBuffer[i] = 0;
            }
        }

        /// <summary>
        /// Returns last error string pointer
        /// </summary>
        /// <returns></returns>
        [NativeCallable(EntryPoint = nameof(get_last_error))]
        public static void* get_last_error()
        {
            return _errorBuffer;
        }

        /// <summary>
        /// Set log callback function
        /// </summary>
        /// <param name="log">Log callback function pointer</param>
        [NativeCallable(EntryPoint = nameof(set_logger))]
        public static bool set_logger(void* log)
        {
            try
            {
                NativeLogger.SetLogFunction(log);
                return true;
            }
            catch (Exception exception)
            {
                Error(exception.ToString());
                return false;
            }
        }

        /// <summary>
        /// Frees resource associated with owner
        /// </summary>
        /// <param name="owner">Either tex or pkg pointer</param>
        /// <param name="resource">Pointer to resource to free</param>
        /// <returns></returns>
        [NativeCallable(EntryPoint = nameof(free_resource))]
        public static bool free_resource(void* owner, void* resource)
        {
            if (owner == null) return Error("Owner is a null pointer");
            if (resource == null) return Error("Resource is a null pointer");

            if (!TryGetEnvironment(owner, out var environment))
                return false;

            if (!environment.TryFree(resource)) return Error("Resource not found in environment");

            return true;
        }
    }
}