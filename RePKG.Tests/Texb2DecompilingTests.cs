using NUnit.Framework;

namespace RePKG.Tests
{
    public class Texb2DecompilingTests : TexDecompilingTestsBase
    {
        [Test]
        public void V2_DXT5() => Test(nameof(V2_DXT5));

        [Test]
        public void V2_RGBA8888() => Test(nameof(V2_RGBA8888));
        
        [Test]
        public void V2_R8() => Test(nameof(V2_R8));
        
        [Test]
        public void V2_RG88() => Test(nameof(V2_RG88));
        
        [Test]
        public void V2_RGBA8888N() => Test(nameof(V2_RGBA8888N));
    }
}