using NUnit.Framework;

namespace RePKG.Tests
{
    [TestFixture]
    public class Texb3DecompilingTests : TexDecompilingTestsBase
    {
        [Test]
        public void V3_RGBA8888_JPEG() => Test(nameof(V3_RGBA8888_JPEG));

        [Test]
        public void V3_RGBA8888_GIF() => Test(nameof(V3_RGBA8888_GIF),
            validateTex: (tex, _) => Assert.IsTrue(tex.IsGif));

        [Test]
        public void V3_DXT1() => Test(nameof(V3_DXT1));

        [Test]
        public void V3_DXT3() => Test(nameof(V3_DXT3));

        [Test]
        public void V3_DXT5() => Test(nameof(V3_DXT5));
    }
}