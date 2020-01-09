using System.Collections.Generic;
using System.IO;
using System.Text;
using RePKG.Core.Package;
using RePKG.Core.Package.Interfaces;

namespace RePKG.Application.Package
{
    public class PackageReader : IPackageReader
    {
        public bool ReadEntryBytes { get; set; } = true;

        public Core.Package.Package ReadFromStream(Stream stream)
        {
            using (var reader = new BinaryReader(stream, Encoding.UTF8, true))
            {
                var packageStart = stream.Position;
                var package = new Core.Package.Package
                {
                    Magic = reader.ReadStringI32Size(maxLength: 32)
                };

                ReadEntries(package.Entries, reader);

                var dataStart = (int) stream.Position;
                package.HeaderSize = (int) (dataStart - packageStart);

                if (!ReadEntryBytes)
                    return package;

                PopulateEntriesWithData(dataStart, package.Entries, reader);

                return package;
            }
        }

        private void ReadEntries(List<PackageEntry> list, BinaryReader reader)
        {
            var entryCount = reader.ReadInt32();

            for (var i = 1; i <= entryCount; i++)
            {
                var fullPath = reader.ReadStringI32Size(maxLength: 255);

                list.Add(new PackageEntry
                {
                    FullPath = fullPath,
                    Offset = reader.ReadInt32(),
                    Length = reader.ReadInt32(),
                    Type = PackageEntryTypeGetter.GetFromFileName(fullPath)
                });
            }
        }

        private void PopulateEntriesWithData(int dataStart, List<PackageEntry> entries, BinaryReader reader)
        {
            foreach (var entry in entries)
            {
                reader.BaseStream.Seek(entry.Offset + dataStart, SeekOrigin.Begin);
                entry.Bytes = reader.ReadBytes(entry.Length);
            }
        }
    }
}