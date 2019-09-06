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

        [SetUp]
        protected void SetUp()
        {
            BasePath =
                AppContext.BaseDirectory.Split(new[] {"RePKG.Tests"}, StringSplitOptions.RemoveEmptyEntries)[0] +
                "RePKG.Tests";

            Directory.CreateDirectory($"{BasePath}\\{OutputDirectoryName}\\");
            Directory.CreateDirectory($"{BasePath}\\{ValidatedDirectoryName}\\");
        }

        protected void Test(string name, bool validateBytes = true, Action<Tex, byte[]> validateTex = null)
        {
            var headerReader = new TexHeaderReader();
            var mipmapDecompressor = new TexMipmapDecompressor();
            var mipmapReader = new TexMipmapReader(mipmapDecompressor);
            var containerReader = new TexMipmapContainerReader(mipmapReader);
            var reader = new TexReader(headerReader, containerReader);

            var texture = reader.ReadFromStream(LoadTestFile(name));

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
                PreviewWriter.WriteTexture(texture, $"{BasePath}\\{OutputDirectoryName}\\{name}");
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

            var areEqual = true;
            var index = 0;

            for (var i = 0; i < validatedBytes.Length; i++)
            {
                if (validatedBytes[i] != bytes[i])
                {
                    areEqual = false;
                    index = i;
                    break;
                }
            }

            if (!areEqual)
                throw new Exception(
                    $"Decompiled tex bytes are not the same at index: {index}\r\n" +
                    $"Expected: {validatedBytes[index]}\r\n" +
                    $"Actual: {bytes[index]}");
        }
    }
}