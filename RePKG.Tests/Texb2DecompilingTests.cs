using NUnit.Framework;

namespace RePKG.Tests
{
    public class Texb2DecompilingTests : TexDecompilingTestsBase
    {
        [Test]
        public void V2_DXT5() => Test(nameof(V2_DXT5));

        [Test]
        public void V2_ARGB8888() => Test(nameof(V2_ARGB8888));
    }
}