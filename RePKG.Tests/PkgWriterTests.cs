using System.IO;
using System.Text;
using NUnit.Framework;
using RePKG.Application.Package;
using RePKG.Core.Package;
using RePKG.Core.Package.Interfaces;

namespace RePKG.Tests
{
    [TestFixture]
    public class PkgWriterTests
    {
        [Test]
        public void TestWriteAndRead()
        {
            var package = new Package {Magic = "PKGV0005"};

            package.Entries.Add(new PackageEntry
            {
                Bytes = Encoding.ASCII.GetBytes("Hello world!"),
                FullPath = "hello_world.txt",
            });

            package.Entries.Add(new PackageEntry
            {
                Bytes = Encoding.ASCII.GetBytes("Test"),
                FullPath = "test.txt",
            });

            // Write
            IPackageWriter writer = new PackageWriter();
            var stream = new MemoryStream();
            using (var binaryWriter = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                writer.WriteTo(binaryWriter, package);
            }

            // Read
            stream.Position = 0;
            var packageReader = new PackageReader {ReadEntryBytes = true};
            
            Package readPackage;
            using (var binaryReader = new BinaryReader(stream, Encoding.UTF8, true))
            {
                readPackage = packageReader.ReadFrom(binaryReader);
            }

            // Verify
            Assert.AreEqual(package.Magic, readPackage.Magic);
            Assert.AreEqual(package.Entries.Count, readPackage.Entries.Count);

            for (var i = 0; i < package.Entries.Count; i++)
            {
                var entry = package.Entries[i];
                var readEntry = readPackage.Entries[i];

                Assert.AreEqual(entry.Bytes, readEntry.Bytes);
                Assert.AreEqual(entry.Extension, readEntry.Extension);
                Assert.AreEqual(entry.Length, readEntry.Length);
                Assert.AreEqual(entry.Offset, readEntry.Offset);
            }
        }
    }
}