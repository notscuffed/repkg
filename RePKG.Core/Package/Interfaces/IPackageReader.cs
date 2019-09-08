using System.IO;

namespace RePKG.Core.Package.Interfaces
{
    public interface IPackageReader
    {
        Package ReadFromStream(Stream stream);
    }
}