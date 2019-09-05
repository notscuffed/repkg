using System;
using System.IO;
using NUnit.Framework;
using RePKG.Texture;

namespace RePKG.Tests
{
    public class TexDecompilingTestsBase
    {
        protected const int TestBytesCount = 128 * 128;
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
            var texture = TexLoader.LoadTex(LoadTestFile(name));
            var bytes = texture.Decompile();

            validateTex?.Invoke(texture, bytes);

            if (validateBytes)
                ValidateBytes(bytes, name);
            else
            {
                SaveValidatedBytes(bytes, name);
                SaveOutput(texture, name);
            }
        }

        protected byte[] LoadTestFile(string name)
        {
            return File.ReadAllBytes($"{BasePath}\\{InputDirectoryName}\\{name}.tex");
        }

        protected void SaveOutput(Tex tex, string name)
        {
            tex.DecompileAndSave($"{BasePath}\\{OutputDirectoryName}\\{name}.tex", true);
        }

        protected void SaveValidatedBytes(byte[] bytes, string name)
        {
            using (var stream = File.Open($"{BasePath}\\{ValidatedDirectoryName}\\{name}.bytes",
                FileMode.Create,
                FileAccess.Write,
                FileShare.Read))
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write(bytes, 0, TestBytesCount);
            }
        }

        protected void ValidateBytes(byte[] bytes, string name)
        {
            var goodBytes = File.ReadAllBytes($"{BasePath}\\{ValidatedDirectoryName}\\{name}.bytes");

            for (var i = 0; i < goodBytes.Length; i++)
            {
                Assert.AreEqual(goodBytes[i], bytes[i], "Decompiled tex bytes are not the same");
            }
        }
    }
}