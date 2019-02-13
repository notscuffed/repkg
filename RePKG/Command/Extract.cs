using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
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
        private static readonly string[] _projectFiles = {"project.json", "preview.jpg"};

        public static void Action(ExtractOptions options)
        {
            _options = options;

            if (string.IsNullOrEmpty(options.OutputDirectory))
            {
                options.OutputDirectory = Directory.GetCurrentDirectory();
            }

            if (!string.IsNullOrEmpty(_options.IgnoreExts))
                _skipExtArray = NormalizeExtensions(_options.IgnoreExts.Split(','));

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
            {
                var bitmap = DecompileTex(File.ReadAllBytes(file.FullName), file.FullName);

                if (bitmap == null)
                    continue;

                var name = Path.GetFileNameWithoutExtension(file.Name) + ".png"; 
                bitmap.Save(Path.Combine(_options.OutputDirectory, name));
                bitmap.Dispose();
            }
        }

        private static void ExtractPkgDirectory(DirectoryInfo directoryInfo)
        {
            var rootDirectoryLength = directoryInfo.FullName.Length + 1;

            if (_options.Recursive)
            {
                foreach (var file in directoryInfo.EnumerateFiles("*.pkg", SearchOption.AllDirectories))
                {
                    var directoryName = file.DirectoryName.Substring(rootDirectoryLength);
                    ExtractPKG(file, Path.Combine(_options.OutputDirectory, directoryName));
                }

                return;
            }

            foreach (var directory in directoryInfo.EnumerateDirectories())
            {
                foreach (var file in directory.EnumerateFiles("*.pkg"))
                {
                    var directoryName = file.DirectoryName.Substring(rootDirectoryLength);
                    ExtractPKG(file, Path.Combine(_options.OutputDirectory, directoryName));
                }
            }
        }

        private static void ExtractFile(FileInfo fileInfo)
        {
            var opt = _options;

            if (fileInfo.Extension.Equals(".pkg", StringComparison.OrdinalIgnoreCase))
                ExtractPKG(fileInfo, _options.OutputDirectory);
            else if (fileInfo.Extension.Equals(".tex", StringComparison.OrdinalIgnoreCase))
            {
                var output = Path.GetFileNameWithoutExtension(fileInfo.Name) + ".png";

                if (!_options.Overwrite && File.Exists(output))
                {
                    Console.WriteLine(Resources.SkippingAlreadyExists, output);
                    return;
                }

                var bitmap = DecompileTex(File.ReadAllBytes(fileInfo.FullName), fileInfo.FullName);

                if (bitmap == null)
                    return;

                bitmap.Save(Path.Combine(_options.OutputDirectory, output));
                bitmap.Dispose();
            }
            else
                Console.WriteLine(Resources.UnrecognizedFileExtension, fileInfo.Extension);
        }

        private static void ExtractPKG(FileInfo file, string outputDirectory)
        {
            var loader = new PackageLoader(true);
            var package = loader.Load(file);

            Console.WriteLine(Resources.ExtractingPackage, file.FullName);
            IEnumerable<Entry> entries = package.Entries;

            if (!string.IsNullOrEmpty(_options.IgnoreExts))
            {
                entries = from entry in package.Entries
                    where !_skipExtArray.Any(s => entry.EntryPath.EndsWith(s, StringComparison.OrdinalIgnoreCase))
                    select entry;
            }
            else if (!string.IsNullOrEmpty(_options.OnlyExts))
            {
                entries = from entry in package.Entries
                    where _onlyExtArray.Any(s => entry.EntryPath.EndsWith(s, StringComparison.OrdinalIgnoreCase))
                    select entry;
            }

            foreach (var entry in entries)
            {
                ExtractEntry(entry, ref outputDirectory);
            }

            if (!_options.CopyProject || _options.SingleDir || file.Directory == null)
                return;

            var files = file.Directory.GetFiles().Where(x => _projectFiles.Contains(x.Name, StringComparer.OrdinalIgnoreCase));

            foreach (var fileToCopy in files)
            {
                var outputPath = Path.Combine(outputDirectory, fileToCopy.Name);

                if (_options.Overwrite || !File.Exists(outputPath))
                    File.Copy(fileToCopy.FullName, outputPath, true);
                else
                    Console.WriteLine(Resources.SkippingAlreadyExists, outputPath);
            }
        }
        
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        private static void ExtractEntry(Entry entry, ref string outputDirectory)
        {
            if (Program.Closing)
                Environment.Exit(0);

            // save raw
            string filePath;
            if (_options.SingleDir)
                filePath = Path.Combine(outputDirectory, entry.Name);
            else
                filePath = Path.Combine(outputDirectory, entry.EntryPath, entry.Name);

            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            var outputPath = filePath + entry.Extension;

            if (!_options.Overwrite && !File.Exists(outputPath))
            {
                Console.WriteLine(Resources.ExtractingName, entry.FullName);
                entry.WriteTo(outputPath);
            }
            else
                Console.WriteLine(Resources.SkippingAlreadyExists, outputPath);
            
            // decompile and save
            if (_options.NoTexDecompile || entry.Type != EntryType.TEX)
                return;

            var imageOutputPath = filePath + ".png";

            if (!_options.Overwrite && File.Exists(imageOutputPath))
            {
                Console.WriteLine(Resources.SkippingAlreadyExists, imageOutputPath);
                return;
            }

            var bitmap = DecompileTex(entry.Data, entry.FullName);

            if (bitmap == null)
                return;

            bitmap.Save(imageOutputPath, ImageFormat.Png);
            bitmap.Dispose();
        }

        private static Bitmap DecompileTex(byte[] bytes, string name)
        {
            if (Program.Closing)
                Environment.Exit(0);

            Console.WriteLine(Resources.DecompilingName, name);

            try
            {
                var tex = TexLoader.LoadTex(bytes, 1);

                if (_options.DebugInfo)
                    tex.DebugInfo();

                return TexDecompiler.Decompile(tex);
            }
            catch (Exception e)
            {
                Console.WriteLine(Resources.FailedToDecompile);
                Console.WriteLine(e);
            }

            return null;
        }
    }


    [Verb("extract", HelpText = "Extract .pkg/Decompile .tex")]
    public class ExtractOptions
    {
        [Option('o', "output", Required = false, HelpText = "Output directory", Default = "./output")]
        public string OutputDirectory { get; set; }

        [Option('i', "ignoreexts", HelpText = "Don't extract files with specified extensions (delimited by comma \",\")")]
        public string IgnoreExts { get; set; }

        [Option('e', "onlyexts", HelpText = "Only extract files with specified extensions (delimited by comma \",\")")]
        public string OnlyExts { get; set; }

        [Option('d', "debuginfo", HelpText = "Print debug info while extracting/decompiling")]
        public bool DebugInfo { get; set; }

        [Option('t', "tex", HelpText = "Decompile all tex files from specified directory in input")]
        public bool TexDirectory { get; set; }

        [Option('s', "singledir", HelpText = "Should all extracted files be put in one directory instead of their entry path")]
        public bool SingleDir { get; set; }

        [Option('r', "recursive", HelpText = "Recursive search in all subfolders of specified directory")]
        public bool Recursive { get; set; }

        [Option('c', "copyproject", HelpText = "Copy project.json and preview.jpg from beside .pkg into output directory")]
        public bool CopyProject { get; set; }

        [Option("no-tex-decompile", HelpText = "Don't decompile .tex files while extracting .pkg")]
        public bool NoTexDecompile { get; set; }

        [Option("overwrite", HelpText = "Overwrite all existing files")]
        public bool Overwrite { get; set; }

        [Value(0, Required = true, HelpText = "Path to file/directory", MetaName = "Input")]
        public string Input { get; set; }
    }
}
