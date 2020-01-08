using System;
using System.IO;
using NUnit.Framework;
using RePKG.Application.Texture;

namespace RePKG.Tests
{
    public class TexWriterTests
    {
        private TexReader _reader;
        private TexWriter _writer;
        
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
            var headerWriter = new TexHeaderWriter();
            var mipmapWriter = new TexImageWriter();
            var containerWriter = new TexImageContainerWriter(mipmapWriter);
            var frameInfoWriter = new TexFrameInfoContainerWriter();
            
            _writer = new TexWriter(headerWriter, containerWriter, frameInfoWriter);
        }
        
        
        [Test]
        [TestCase("V1_DXT5")]
        [TestCase("V1_RGBA8888")]
        [TestCase("V2_DXT5")]
        [TestCase("V2_RGBA8888")]
        [TestCase("V2_R8")]
        [TestCase("V2_RG88")]
        [TestCase("V2_RGBA8888N")]
        [TestCase("V3_RGBA8888_JPEG")]
        [TestCase("V3_DXT1")]
        [TestCase("V3_DXT3")]
        [TestCase("V3_DXT5")]
        [TestCase("V3_RGBA8888_GIF_TEXS0003")]
        public void TestWriteAndRead(string name)
        {
            // Load file
            var file = TexDecompressingTests.LoadTestFile(name);
            var inputBytes = new byte[file.Length];
            var bytesRead = file.Read(inputBytes, 0, (int) file.Length);
            Assert.AreEqual(file.Length, bytesRead, "Failed to read input file");
            file.Close();
            
            // Read tex
            var memoryStream = new MemoryStream(inputBytes);
            var tex = _reader.ReadFromStream(memoryStream);

            // Write tex
            var outputMemoryStream = new MemoryStream(inputBytes.Length);
            _writer.WriteToStream(tex, outputMemoryStream);
            var outputBytes = outputMemoryStream.ToArray();
            
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