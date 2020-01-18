using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using RePKG.Application.Texture;
using RePKG.Core.Texture;
using RePKG.Native.Texture;

namespace RePKG.Native
{
    public static unsafe partial class RePKG
    {
        private static ITexReader _texReader = TexReader.Default;
        private static ITexWriter _texWriter = TexWriter.Default;
        private static readonly TexToImageConverter _texToImageConverter = new TexToImageConverter();
        //private static readonly ITexJsonInfoGenerator _jsonInfoGenerator = new TexJsonInfoGenerator();

        public static void SetTexReader(ITexReader reader) => _texReader = reader;
        public static void SetTexWriter(ITexWriter writer) => _texWriter = writer;
        
        /// <summary>
        /// Loads tex from file path
        /// </summary>
        /// <param name="pathPtr">File path pointer</param>
        [NativeCallable(EntryPoint = nameof(tex_load_file))]
        public static CTex* tex_load_file(byte* pathPtr)
        {
            if (!TryGetExistingFilePath(pathPtr, out var path))
                return null;

            try
            {
                using (var fileStream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    return tex_load_internal(fileStream);
                }
            }
            catch (Exception exception)
            {
                Error(exception.ToString());
                return null;
            }
        }

        /// <summary>
        /// Loads tex from memory pointer
        /// </summary>
        /// <param name="memoryPointer">Pointer to tex bytes</param>
        /// <param name="size">Size of tex</param>
        /// <returns></returns>
        [NativeCallable(EntryPoint = nameof(tex_load))]
        public static CTex* tex_load(byte* memoryPointer, long size)
        {
            if (memoryPointer == null)
            {
                Error("Pointer is a null pointer");
                return null;
            }

            if (size <= 0)
            {
                Error($"Invalid size: {size}");
                return null;
            }

            using (var stream = new UnmanagedMemoryStream(memoryPointer, size))
            {
                return tex_load_internal(stream);
            }
        }

        /*
        *** Including this causes the library to be 4-5x larger because of json parsing library ***
         
        /// <summary>
        /// Generates .tex-json for tex
        /// </summary>
        /// <param name="tex">Pointer to tex</param>
        /// <returns></returns>
        [NativeCallable(EntryPoint = nameof(tex_generate_json_info))]
        public static void* tex_generate_json_info(CTex* tex)
        {
            try
            {
                if (!TryGetEnvironment(tex, out var environment))
                    return null;

                var wrappedCTex = new WCTex(tex, environment);
                var info = _jsonInfoGenerator.GenerateInfo(wrappedCTex);

                return environment.ToCString(info);
            }
            catch (Exception exception)
            {
                HandleException(exception);
                return null;
            }
        }*/

        /// <summary>
        /// Saves tex to file
        /// </summary>
        /// <param name="tex">Pointer to tex</param>
        /// <param name="pathPtr">File path pointer</param>
        /// <returns></returns>
        [NativeCallable(EntryPoint = nameof(tex_save_file))]
        public static bool tex_save_file(CTex* tex, byte* pathPtr)
        {
            try
            {
                if (!ValidFilePath(pathPtr, out var path))
                    return false;
                
                if (!TryGetEnvironment(tex, out var environment))
                    return false;

                using (var stream = File.OpenWrite(path))
                using (var writer = new BinaryWriter(stream))
                {
                    var wrappedCTex = new WCTex(tex, environment);
                    _texWriter.WriteTo(writer, wrappedCTex);
                    return true;
                }
            }
            catch (Exception exception)
            {
                return HandleException(exception);
            }
        }

        /// <summary>
        /// Saves tex to memory, returns it pointer and sets length out parameter 
        /// </summary>
        /// <param name="tex">Pointer to tex</param>
        /// <returns>Bytes result containing pointer to bytes and it's length</returns>
        [NativeCallable(EntryPoint = nameof(tex_save))]
        public static CBytesResult tex_save(CTex* tex)
        {
            try
            {
                if (!TryGetEnvironment(tex, out var environment))
                    return new CBytesResult();
                
                using (var stream = new MemoryStream())
                using (var writer = new BinaryWriter(stream))
                {
                    var wrappedCTex = new WCTex(tex, environment);
                    
                    _texWriter.WriteTo(writer, wrappedCTex);
                    writer.Flush();
                    
                    var buffer = stream.GetBuffer();
                    return new CBytesResult(environment.Pin(buffer), buffer.Length);
                }
            }
            catch (Exception exception)
            {
                HandleException(exception);
                return new CBytesResult();
            }
        }

        /// <summary>
        /// Converts tex to image and saves it to file
        /// </summary>
        /// <param name="tex">Pointer to tex</param>
        /// <param name="pathPtr">File path pointer</param>
        [NativeCallable(EntryPoint = nameof(tex_to_image_file))]
        public static bool tex_to_image_file(CTex* tex, byte* pathPtr)
        {
            try
            {
                if (!ValidFilePath(pathPtr, out var path))
                    return false;
                
                if (!TryGetEnvironment(tex, out var environment))
                    return false;
                
                var wrappedCTex = new WCTex(tex, environment);
                var image = _texToImageConverter.ConvertToImage(wrappedCTex);
                var pathWithExtension = path + '.' + image.Format.GetFileExtension();
                File.WriteAllBytes(pathWithExtension, image.Bytes);

                return true;
            }
            catch (Exception exception)
            {
                return HandleException(exception);
            }
        }

        /// <summary>
        /// Converts tex to image and returns pointer to it
        /// </summary>
        /// <param name="tex">Pointer to tex</param>
        /// <returns>Bytes result containing pointer to bytes and it's length</returns>
        [NativeCallable(EntryPoint = nameof(tex_to_image))]
        public static CBytesResult tex_to_image(CTex* tex)
        {
            try
            {
                if (!TryGetEnvironment(tex, out var environment))
                    return new CBytesResult();

                var wrappedCTex = new WCTex(tex, environment);
                var image = _texToImageConverter.ConvertToImage(wrappedCTex);
                
                return new CBytesResult(environment.Pin(image.Bytes), image.Bytes.Length);
            }
            catch (Exception exception)
            {
                HandleException(exception);
                return new CBytesResult();
            }
        }
        
        /// <summary>
        /// Frees tex and all resources associated with it
        /// </summary>
        /// <param name="tex"></param>
        /// <returns></returns>
        [NativeCallable(EntryPoint = nameof(tex_destroy))]
        public static bool tex_destroy(CTex* tex)
        {
            try
            {
                if (!TryGetEnvironment(tex, out var environment))
                    return false;

                _environments.Remove(new IntPtr(tex));
                environment.Dispose();

                return true;
            }
            catch (Exception exception)
            {
                return HandleException(exception);
            }
        }
        
        /* Internals */
        
        private static CTex* tex_load_internal(Stream stream)
        {
            var environment = new NativeEnvironment();

            try
            {
                using (var binaryReader = new BinaryReader(stream, Encoding.UTF8))
                {
                    var tex = _texReader.ReadFrom(binaryReader);
                    var ctex = environment.ConvertToCTex(tex);
                    _environments[new IntPtr(ctex)] = environment;
                    return ctex;
                }
            }
            catch (Exception exception)
            {
                HandleException(exception);
                environment.Dispose();
                return null;
            }
        }
    }
}