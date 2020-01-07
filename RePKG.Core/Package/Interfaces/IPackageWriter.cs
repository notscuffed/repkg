using System.IO;

namespace RePKG.Core.Package.Interfaces
{
    public interface IPackageWriter
    {
        void WriteToStream(Package package, Stream stream);
    }
}