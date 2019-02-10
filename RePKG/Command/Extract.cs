using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using CommandLine;
using RePKG.Package;
using RePKG.Properties;
using RePKG.Texture;

namespace RePKG.Command
{
    public static class Extract
    {
        private static ExtractOptions _options;
        private static string[] _skipExtArray;
        private static string[] _onlyExtArray;

        public static void Action(ExtractOptions options)
        {
            _options = options;

            if (string.IsNullOrEmpty(options.OutputDirectory))
            {
                options.OutputDirectory = Directory.GetCurrentDirectory();
            }

            if (!string.IsNullOrEmpty(_options.SkipExts))
                _skipExtArray = NormalizeExtensions(_options.SkipExts.Split(','));

            if (!string.IsNullOrEmpty(_options.OnlyExts))
                _onlyExtArray = NormalizeExtensions(_options.OnlyExts.Split(','));

            var fileInfo = new FileInfo(options.Input);
            var directoryInfo = new DirectoryInfo(options.Input);

            if (!fileInfo.Exists)
            {
                if (directoryInfo.Exists)
                {
                    if (_options.TexDirectory)
                        ExtractTexDirectory(directoryInfo);
                    else
                        ExtractPkgDirectory(directoryInfo);

                    Console.WriteLine(Resources.Done);
                    return;
                }

                Console.WriteLine(Resources.InputNotFound);
                Console.WriteLine(options.Input);
                return;
            }
            
            ExtractFile(fileInfo);
            Console.WriteLine(Resources.Done);
        }

        private static string[] NormalizeExtensions(string[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].StartsWith("."))
                    continue;
                array[i] = '.' + array[i];
            }

            return array;
        }

        private static void ExtractTexDirectory(DirectoryInfo directoryInfo)
        {
            var flags = SearchOption.TopDirectoryOnly;

            if (_options.Recursive)
                flags = SearchOption.AllDirectories;

            foreach (var file in directoryInfo.EnumerateFiles("*.tex", flags))
                ExtractTEX(file);
        }

        private static void ExtractPkgDirectory(DirectoryInfo directoryInfo)
        {
            if (_options.Recursive)
            {
                foreach (var file in directoryInfo.EnumerateFiles("*.pkg", SearchOption.AllDirectories))
                {
                    ExtractPKG(file);
                }

                return;
            }

            foreach (var directory in directoryInfo.EnumerateDirectories())
            {
                foreach (var file in directory.EnumerateFiles("*.pkg"))
                {
                    ExtractPKG(file);
                }
            }
        }

        private static void ExtractFile(FileInfo fileInfo)
        {
            var opt = _options;

            Console.WriteLine(Resources.OutputDirectory, opt.OutputDirectory);

            if (fileInfo.Extension.Equals(".pkg", StringComparison.OrdinalIgnoreCase))
                ExtractPKG(fileInfo);
            else if (fileInfo.Extension.Equals(".tex", StringComparison.OrdinalIgnoreCase))
                ExtractTEX(fileInfo);
            else
                Console.WriteLine(Resources.UnrecognizedFileExtension, fileInfo.Extension);
        }

        private static void ExtractPKG(FileInfo file)
        {
            var loader = new PackageLoader(true);
            var package = loader.Load(file);

            Console.WriteLine(Resources.ExtractingPackage, file.FullName);
            IEnumerable<Entry> entries = package.Entries;

            if (!string.IsNullOrEmpty(_options.SkipExts))
            {
                entries = from entry in package.Entries
                    where !_skipExtArray.Any(s => entry.Name.EndsWith(s, StringComparison.OrdinalIgnoreCase))
                    select entry;
            }
            else if (!string.IsNullOrEmpty(_options.OnlyExts))
            {
                entries = from entry in package.Entries
                    where _onlyExtArray.Any(s => entry.Name.EndsWith(s, StringComparison.OrdinalIgnoreCase))
                    select entry;
            }

            foreach (var entry in entries)
            {
                Console.WriteLine(Resources.ExtractingFileName, entry.Name);
                entry.WriteTo(_options.OutputDirectory, _options.SingleDir);
            }
        }

        private static void ExtractTEX(FileInfo file)
        {
            Console.WriteLine(Resources.ExtractingTexture, file.FullName);

            var bytes = File.ReadAllBytes(file.FullName);
            var tex = TexLoader.LoadTex(bytes);

            if (_options.DebugInfo)
                tex.DebugInfo();

            var bitmap = TexBitmapExtractor.Extract(tex);
            var path = Path.Combine(_options.OutputDirectory, Path.GetFileNameWithoutExtension(file.Name));
            bitmap.Save($"{path}.png", ImageFormat.Png);
        }
    }


    [Verb("extract", HelpText = "Extract pkg/tex.")]
    public class ExtractOptions
    {
        [Option('o', "output", Required = false, HelpText = "Path to output directory", Default = "./output")]
        public string OutputDirectory { get; set; }

        [Option('i', "ignoreexts", HelpText = "Ignore files with specified extension (split extensions using ,)")]
        public string SkipExts { get; set; }

        [Option('e', "onlyexts", HelpText = "Only extract files with specified extension (split extensions using ,)")]
        public string OnlyExts { get; set; }

        [Option('d', "debuginfo", HelpText = "Show debug info about extracted items", Default = false)]
        public bool DebugInfo { get; set; }

        [Option('t', "tex", HelpText = "Extract directory of tex files", Default = false)]
        public bool TexDirectory { get; set; }

        [Option('s', "singledir", HelpText = "Should all extracted files be put in one directory instead of their specific subfolders", Default = false)]
        public bool SingleDir { get; set; }

        [Option('r', "recursive", HelpText = "Recursive search in all subfolders of specified directory", Default = false)]
        public bool Recursive { get; set; }

        [Value(0, Required = true, HelpText = "Path to file/directory which you want to extract", MetaName = "Input")]
        public string Input { get; set; }
    }
}
