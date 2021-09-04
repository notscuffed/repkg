using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using RePKG.Application.Texture;
using RePKG.Core.Texture;

namespace RePKG.Tests
{
    public class TexWriterTests
    {
        private ITexReader _reader;
        private ITexWriter _writer;

        [SetUp]
        public void Setup()
        {
            // Reader
            var headerReader = new TexHeaderReader();
            var mipmapDecompressor = new TexMipmapDecompressor();
            var mipmapReader = new TexImageReader(mipmapDecompressor);
            var containerReader = new TexImageContainerReader(mipmapReader);
            var frameInfoReader = new TexFrameInfoContainerReader();

            mipmapReader.DecompressMipmapBytes = false;
            mipmapReader.ReadMipmapBytes = true;

            _reader = new TexReader(headerReader, containerReader, frameInfoReader);

            // Writer
            _writer = TexWriter.Default;
        }


        // V%i - The number is TexImageContainer.ImageContainerVersion
        [Test]
        [TestCase("V1_DXT5")]
        [TestCase("V1_RGBA8888")]
        [TestCase("V2_DXT5")]
        [TestCase("V2_RGBA8888")]
        [TestCase("V2_R8")]
        [TestCase("V2_RG88")]
        [TestCase("V2_RGBA8888N")]
        [TestCase("V2_GIF_ROTATED_FRAMES_TEXS0001")]
        [TestCase("V3_RGBA8888_JPEG")]
        [TestCase("V3_DXT1")]
        [TestCase("V3_DXT3")]
        [TestCase("V3_DXT5")]
        [TestCase("V3_RGBA8888_GIF_TEXS0003")]
        public void TestWriteAndRead(string name)
        {
            // Load file
            var inputFileReader = TexDecompressingTests.LoadTestFile(name);
            var inputBytes = new byte[inputFileReader.BaseStream.Length];
            var bytesRead = inputFileReader.Read(inputBytes, 0, (int) inputFileReader.BaseStream.Length);
            Assert.AreEqual(inputFileReader.BaseStream.Length, bytesRead, "Failed to read input file");
            inputFileReader.Close();

            // Read tex
            var reader = new BinaryReader(new MemoryStream(inputBytes), Encoding.UTF8);
            var tex = _reader.ReadFrom(reader);

            // Write tex
            var memoryStream = new MemoryStream(inputBytes.Length);
            var writer = new BinaryWriter(memoryStream, Encoding.UTF8);
            _writer.WriteTo(writer, tex);
            var outputBytes = memoryStream.ToArray();

            // Verify
            Assert.AreEqual(inputBytes.Length, outputBytes.Length, "Written tex size doesn't match input size");

            for (var i = 0; i < inputBytes.Length; i++)
            {
                if (inputBytes[i] == outputBytes[i])
                    continue;

                throw new Exception(
                    $"Rewritten tex bytes are not the same at index: {i}\r\n" +
                    $"Expected: {inputBytes[i]}\r\n" +
                    $"Actual: {outputBytes[i]}");
            }
        }
    }
}