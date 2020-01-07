using System;
using System.IO;
using NUnit.Framework;
using RePKG.Application.Texture;
using RePKG.Core.Texture;

namespace RePKG.Tests
{
    public class TexDecompilingTestsBase
    {
        protected const string ValidatedDirectoryName = "TestTexturesValidated";
        protected const string OutputDirectoryName = "Output";
        protected const string InputDirectoryName = "TestTextures";
        protected string BasePath;
        protected TexReader _reader;

        [SetUp]
        protected void SetUp()
        {
            BasePath = TestHelper.BasePath;

            Directory.CreateDirectory($"{BasePath}\\{OutputDirectoryName}\\");
            Directory.CreateDirectory($"{BasePath}\\{ValidatedDirectoryName}\\");

            var headerReader = new TexHeaderReader();
            var mipmapDecompressor = new TexMipmapDecompressor();
            var mipmapReader = new TexMipmapReader(mipmapDecompressor);
            var containerReader = new TexMipmapContainerReader(mipmapReader);

            _reader = new TexReader(headerReader, containerReader);
        }

        protected void Test(string name, bool validateBytes = true, Action<Tex, byte[]> validateTex = null)
        {
            var texture = _reader.ReadFromStream(LoadTestFile(name));

            var firstMipmap = texture.FirstMipmap;
            var bytes = firstMipmap.Bytes;

            validateTex?.Invoke(texture, bytes);

            if (validateBytes)
            {
                ValidateBytes(bytes, name);
            }
            else
            {
                SaveValidatedBytes(bytes, name);
                TexPreviewWriter.WriteTexture(texture, $"{BasePath}\\{OutputDirectoryName}\\{name}");
            }
        }

        protected Stream LoadTestFile(string name)
        {
            return File.Open(
                $"{BasePath}\\{InputDirectoryName}\\{name}.tex",
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read);
        }

        protected void SaveValidatedBytes(byte[] bytes, string name)
        {
            using (var stream = File.Open($"{BasePath}\\{ValidatedDirectoryName}\\{name}.bytes",
                FileMode.Create,
                FileAccess.Write,
                FileShare.Read))
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write(bytes, 0, bytes.Length);
            }
        }

        protected void ValidateBytes(byte[] bytes, string name)
        {
            var validatedBytes = File.ReadAllBytes($"{BasePath}\\{ValidatedDirectoryName}\\{name}.bytes");

            Assert.AreEqual(bytes.Length, validatedBytes.Length);
            
            for (var i = 0; i < validatedBytes.Length; i++)
            {
                if (validatedBytes[i] == bytes[i])
                    continue;
                
                throw new Exception(
                    $"Decompiled tex bytes are not the same at index: {i}\r\n" +
                    $"Expected: {validatedBytes[i]}\r\n" +
                    $"Actual: {bytes[i]}");
            }
            
        }
    }
}