using System;
using System.IO;
using System.Linq;
using CommandLine;
using RePKG.Package;
using RePKG.Properties;

namespace RePKG.Command
{
    public class Info
    {
        private static InfoOptions _options;

        public static void Action(InfoOptions options)
        {
            _options = options;

            var fileInfo = new FileInfo(options.Input);

            if (fileInfo.Extension.Equals(".pkg", StringComparison.OrdinalIgnoreCase))
                InfoPKG(fileInfo);
            else if (fileInfo.Extension.Equals(".tex", StringComparison.OrdinalIgnoreCase))
                InfoTEX(fileInfo);
            else
                Console.WriteLine(Resources.UnrecognizedFileExtension, fileInfo.Extension);
        }

        private static void InfoPKG(FileInfo file)
        {
            var loader = new PackageLoader(false);
            var package = loader.Load(file);

            Console.WriteLine(Resources.InfoAboutPackage, Path.GetFileNameWithoutExtension(package.Path));
            Console.WriteLine(Resources.PackageEntries);

            var entries = package.Entries;

            if (_options.Sort)
            {
                var newEntries = entries.ToList();
                if (_options.SortBy == "extension")
                    newEntries.Sort((a, b) => String.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase));
                else if (_options.SortBy == "size")
                    newEntries.Sort((a, b) => a.Length.CompareTo(b.Length));
                else 
                    newEntries.Sort((a, b) => String.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase));
            }

            foreach (var entry in entries)
            {
                Console.WriteLine(@"* " + entry.Name + $@" - {entry.Length} bytes");
            }
        }

        private static void InfoTEX(FileInfo file)
        {

        }
    }

    [Verb("info", HelpText = "Show info about PKG/TEX")]
    public class InfoOptions
    {
        [Value(0, Required = true, HelpText = "Path to file which you want to get info about", MetaName = "Input file")]
        public string Input { get; set; }

        [Option('s', "sort", HelpText = "Sort entries a-z", Default = false)]
        public bool Sort { get; set; }

        [Option('b', "sortby", HelpText = "Sort by ... (available options: name, extension, size)", Default = "name")]
        public string SortBy { get; set; }
    }
}
