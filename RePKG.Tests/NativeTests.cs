using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using NUnit.Framework;
using RePKG.Application.Texture;
using RePKG.Core.Texture;
using RePKG.Native;
using RePKG.Native.Texture;

namespace RePKG.Tests
{
    [TestFixture]
    public class NativeTests
    {
        [Test]
        [TestCase("V1_DXT5", null)]
        [TestCase("V1_RGBA8888", null)]
        [TestCase("V2_DXT5", null)]
        [TestCase("V2_RGBA8888", null)]
        [TestCase("V2_R8", null)]
        [TestCase("V2_RG88", null)]
        [TestCase("V2_RGBA8888N", null)]
        [TestCase("V3_RGBA8888_JPEG", null)]
        [TestCase("V3_DXT1", null)]
        [TestCase("V3_DXT3", null)]
        [TestCase("V3_DXT5", null)]
        [TestCase("V3_RGBA8888_GIF_TEXS0003", TexFlags.IsGif)]
        public unsafe void Test(
            string name,
            TexFlags? validateFlags = TexFlags.None)
        {
            var path = $"{TestHelper.BasePath}\\{TexDecompressingTests.InputDirectoryName}\\{name}.tex";
            var ctex = Native.RePKG.tex_load_file((byte*) Marshal.StringToHGlobalAnsi(path).ToPointer());

            using (var environment = Native.RePKG.GetEnvironmentFor(ctex))
            {
                ITex texture = new WCTex(ctex, environment);

                var firstMipmap = texture.FirstImage.FirstMipmap;
                var bytes = firstMipmap.Bytes;

                if (validateFlags.HasValue)
                    Assert.IsTrue(texture.Header.Flags.HasFlag(validateFlags));

                TexDecompressingTests.ValidateBytes(bytes, name);
            }
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
        public unsafe void TestWriteAndRead(string name)
        {
            var headerReader = new TexHeaderReader();
            var mipmapDecompressor = new TexMipmapDecompressor();
            var mipmapReader = new TexImageReader(mipmapDecompressor);
            var containerReader = new TexImageContainerReader(mipmapReader);
            var frameInfoReader = new TexFrameInfoContainerReader();

            mipmapReader.DecompressMipmapBytes = false;

            Native.RePKG.SetTexReader(new TexReader(headerReader, containerReader, frameInfoReader));

            // Load file
            var path = $"{TestHelper.BasePath}\\{TexDecompressingTests.InputDirectoryName}\\{name}.tex";
            var inputBytes = File.ReadAllBytes(path);
            var pin = GCHandle.Alloc(inputBytes, GCHandleType.Pinned);
            var bytesPointer = (byte*) pin.AddrOfPinnedObject().ToPointer();

            // Read tex
            var ctex = Native.RePKG.tex_load(bytesPointer, inputBytes.Length);

            if (ctex == null)
                throw new Exception(Marshal.PtrToStringAnsi(new IntPtr(Native.RePKG.get_last_error())));

            using (var environment = Native.RePKG.GetEnvironmentFor(ctex))
            {
                var wrappedCTex = new WCTex(ctex, environment);

                // Write tex
                var memoryStream = new MemoryStream(inputBytes.Length);
                var writer = new BinaryWriter(memoryStream, Encoding.UTF8);
                TexWriter.Default.WriteTo(writer, wrappedCTex);
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

        [Test]
        [TestCase("aęćół€☺☻♥♣♦♠")]
        public unsafe void TestCharsetConverter(string input)
        {
            var buffer = new byte[4096];

            fixed (byte* p = buffer)
            {
                var size = CharsetConverter.UTF16ToUTF8Buffer(input, p, 4096);
                var output = CharsetConverter.UTF8BufferToString(p, size);

                Assert.AreEqual(input, output);
            }
        }
    }
}