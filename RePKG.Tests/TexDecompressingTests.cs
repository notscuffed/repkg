using System;
using System.IO;
using System.Text;
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
        private TexReader _reader;
        private TexToImageConverter _texToImageConverter;

        [SetUp]
        public void SetUp()
        {
            Directory.CreateDirectory($"{TestHelper.BasePath}\\{OutputDirectoryName}\\");
            Directory.CreateDirectory($"{TestHelper.BasePath}\\{ValidatedDirectoryName}\\");

            _reader = TexReader.Default;
            _texToImageConverter = new TexToImageConverter();
        }

        // V%i - The number is TexImageContainer.ImageContainerVersion
        [Test]
        [TestCase("V1_DXT5", true, null)]
        [TestCase("V1_RGBA8888", true, null)]
        [TestCase("V2_DXT5", true, null)]
        [TestCase("V2_RGBA8888", true, null)]
        [TestCase("V2_R8", true, null)]
        [TestCase("V2_RG88", true, null)]
        [TestCase("V2_RGBA8888N", true, null)]
        [TestCase("V2_GIF_ROTATED_FRAMES_TEXS0001", true, TexFlags.IsGif)]
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
            var texture = _reader.ReadFrom(LoadTestFile(name));

            var firstMipmap = texture.FirstImage.FirstMipmap;
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
                ConvertToImageAndSave(texture, name);
            }
        }

        private void ConvertToImageAndSave(ITex tex, string name)
        {
            var resultImage = _texToImageConverter.ConvertToImage(tex);
            
            var path = $"{TestHelper.BasePath}\\{OutputDirectoryName}\\{name}.{resultImage.Format.GetFileExtension()}";
            
            File.WriteAllBytes(path, resultImage.Bytes);
        }

        public static BinaryReader LoadTestFile(string name)
        {
            return new BinaryReader(File.Open(
                $"{TestHelper.BasePath}\\{InputDirectoryName}\\{name}.tex",
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read), Encoding.UTF8);
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