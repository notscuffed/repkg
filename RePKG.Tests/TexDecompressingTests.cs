using System;
using System.IO;
using NUnit.Framework;
using RePKG.Application.Texture;
using RePKG.Core.Texture;

namespace RePKG.Tests
{
    public class TexDecompressingTests
    {
        public const string ValidatedDirectoryName = "TestTexturesValidated";
        public const string OutputDirectoryName = "Output";
        public const string InputDirectoryName = "TestTextures";
        protected TexReader _reader;

        [SetUp]
        public void SetUp()
        {
            Directory.CreateDirectory($"{TestHelper.BasePath}\\{OutputDirectoryName}\\");
            Directory.CreateDirectory($"{TestHelper.BasePath}\\{ValidatedDirectoryName}\\");

            var headerReader = new TexHeaderReader();
            var mipmapDecompressor = new TexMipmapDecompressor();
            var mipmapReader = new TexMipmapReader(mipmapDecompressor);
            var containerReader = new TexMipmapContainerReader(mipmapReader);
            var frameInfoReader = new TexFrameInfoReader();

            _reader = new TexReader(headerReader, containerReader, frameInfoReader);
        }

        [Test]
        [TestCase("V1_DXT5", true, null)]
        [TestCase("V1_RGBA8888", true, null)]
        [TestCase("V2_DXT5", true, null)]
        [TestCase("V2_RGBA8888", true, null)]
        [TestCase("V2_R8", true, null)]
        [TestCase("V2_RG88", true, null)]
        [TestCase("V2_RGBA8888N", true, null)]
        [TestCase("V3_RGBA8888_JPEG", true, null)]
        [TestCase("V3_DXT1", true, null)]
        [TestCase("V3_DXT3", true, null)]
        [TestCase("V3_DXT5", true, null)]
        [TestCase("V3_RGBA8888_GIF_TEXS0003", true, TexFlags.IsGif)]
        public void TestTexDecompressing(
            string name,
            bool validateBytes = true,
            TexFlags? validateFlags = TexFlags.None)
        {
            var texture = _reader.ReadFromStream(LoadTestFile(name));

            var firstMipmap = texture.FirstMipmap;
            var bytes = firstMipmap.Bytes;

            if (validateFlags.HasValue)
                Assert.IsTrue(texture.Header.Flags.HasFlag(validateFlags));

            if (validateBytes)
            {
                ValidateBytes(bytes, name);
            }
            else
            {
                SaveValidatedBytes(bytes, name);
                TexPreviewWriter.WriteTexture(texture, $"{TestHelper.BasePath}\\{OutputDirectoryName}\\{name}");
            }
        }

        public static Stream LoadTestFile(string name)
        {
            return File.Open(
                $"{TestHelper.BasePath}\\{InputDirectoryName}\\{name}.tex",
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read);
        }

        public static void SaveValidatedBytes(byte[] bytes, string name)
        {
            using (var stream = File.Open($"{TestHelper.BasePath}\\{ValidatedDirectoryName}\\{name}.bytes",
                FileMode.Create,
                FileAccess.Write,
                FileShare.Read))
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write(bytes, 0, bytes.Length);
            }
        }

        public static void ValidateBytes(byte[] bytes, string name)
        {
            var validatedBytes = File.ReadAllBytes($"{TestHelper.BasePath}\\{ValidatedDirectoryName}\\{name}.bytes");

            Assert.AreEqual(bytes.Length, validatedBytes.Length);

            for (var i = 0; i < validatedBytes.Length; i++)
            {
                if (validatedBytes[i] == bytes[i])
                    continue;

                throw new Exception(
                    $"Decompresssed tex bytes are not the same at index: {i}\r\n" +
                    $"Expected: {validatedBytes[i]}\r\n" +
                    $"Actual: {bytes[i]}");
            }
        }
    }
}