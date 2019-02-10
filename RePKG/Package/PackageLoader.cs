using System;
using System.IO;

namespace RePKG.Package
{
    public class PackageLoader
    {
        private Stream _stream;
        private BinaryReader _reader;
        private Package _package;
        public bool LoadEntryDataInMemory;

        public PackageLoader(bool loadEntryDataInMemory)
        {
            LoadEntryDataInMemory = loadEntryDataInMemory;
        }

        public Package Load(FileInfo fileInfo)
        {
            if (!fileInfo.Exists)
                throw new FileNotFoundException($"File not found: {fileInfo.FullName}");

            Package package;

            try
            {
                _stream = File.Open(fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.Read);
                _reader = new BinaryReader(_stream);

                _package = new Package(fileInfo.FullName);
                
                ReadHeader();

                if (LoadEntryDataInMemory)
                    ReadEntriesData();
            }
            finally
            {
                package = _package;
                Close();
            }

            return package;
        }

        private void ReadHeader()
        {
            _package.Version = _reader.ReadStringI32Size();
            var entryCount = _reader.ReadInt32();

            for (int i = 0; i < entryCount; i++)
            {
                _package.Entries.Add(ReadEntry());
            }

            _package.HeaderSize = _stream.Position;
        }

        private void ReadEntriesData()
        {
            foreach (var entry in _package.Entries)
            {
                _stream.Seek(_package.HeaderSize + entry.Offset, SeekOrigin.Begin);
                entry.Data = new byte[entry.Length];
                _stream.Read(entry.Data, 0, entry.Length);
            }
        }

        private Entry ReadEntry()
        {
            var name = _reader.ReadStringI32Size();
            var entry = new Entry(name);

            entry.Offset = _reader.ReadInt32();
            entry.Length = _reader.ReadInt32();
            entry.Type = GetEntryType(entry);

            return entry;
        }

        private EntryType GetEntryType(Entry entry)
        {
            var name = entry.Name;

            if (name.EndsWith(".tex", StringComparison.OrdinalIgnoreCase))
                return EntryType.TEX;

            return EntryType.Binary;
        }

        private void Close()
        {
            _reader?.Close();
            _stream?.Close();

            _stream = null;
            _reader = null;
            _package = null;
        }
    }
}
