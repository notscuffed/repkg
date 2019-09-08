using NUnit.Framework;

namespace RePKG.Tests
{
    public class Texb1DecompilingTests : TexDecompilingTestsBase
    {
        [Test]
        public void V1_DXT5() => Test(nameof(V1_DXT5));

        [Test]
        public void V1_RGBA8888() => Test(nameof(V1_RGBA8888));
        
    }
}