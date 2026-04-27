namespace RePKG.Core.Package.Interfaces
{
    public interface IPackageWriter
    {
        void WriteTo(BinaryWriter writer, Package package);
    }
}