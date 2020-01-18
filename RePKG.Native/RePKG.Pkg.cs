using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using RePKG.Application.Package;
using RePKG.Core.Package.Interfaces;
using RePKG.Native.Package;

namespace RePKG.Native
{
    public static unsafe partial class RePKG
    {
        private static readonly IPackageReader _packageReader = new PackageReader();

        /// <summary>
        /// Loads pkg from file
        /// </summary>
        /// <param name="pathPtr">File path pointer</param>
        /// <returns>Pointer to pkg/null if fails</returns>
        [NativeCallable(EntryPoint = nameof(pkg_load_file))]
        public static CPackage* pkg_load_file(byte* pathPtr)
        {
            var environment = new NativeEnvironment();

            try
            {
                if (!TryGetExistingFilePath(pathPtr, out var path))
                    return null;
                
                using (var stream = File.OpenRead(path))
                using (var binaryReader = new BinaryReader(stream, Encoding.UTF8))
                {
                    var package = _packageReader.ReadFrom(binaryReader);
                    var cpackage = environment.ConvertToCPackage(package);

                    _environments[new IntPtr(cpackage)] = environment;

                    return cpackage;
                }
            }
            catch (Exception exception)
            {
                HandleException(exception);
                environment.Dispose();
                return null;
            }
        }

        /// <summary>
        /// Frees pkg and all resources associated with it
        /// </summary>
        /// <param name="pkg">Pointer to pkg</param>
        [NativeCallable(EntryPoint = nameof(pkg_destroy))]
        public static bool pkg_destroy(CPackage* pkg)
        {
            try
            {
                if (!TryGetEnvironment(pkg, out var environment))
                    return false;

                _environments.Remove(new IntPtr(pkg));
                environment.Dispose();

                return true;
            }
            catch (Exception exception)
            {
                return HandleException(exception);
            }
        }
    }
}