using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using CommandLine;
using Newtonsoft.Json;
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
                try
                {
                    var tex = LoadTex(File.ReadAllBytes(file.FullName), file.FullName);

                    if (tex == null)
                        continue;

                    var name = Path.GetFileNameWithoutExtension(file.Name);
                    tex.DecompileAndSave(Path.Combine(_options.OutputDirectory, name), _options.Overwrite);
                    tex.SaveFormatInfo(Path.Combine(_options.OutputDirectory, name), _options.Overwrite);
                }
                catch (Exception e)
                {
                    Console.WriteLine(Resources.FailedToDecompile);
                    Console.WriteLine(e);
                }
            }
        }

        private static void ExtractPkgDirectory(DirectoryInfo directoryInfo)
        {
            var rootDirectoryLength = directoryInfo.FullName.Length + 1;

            if (_options.Recursive)
            {
                foreach (var file in directoryInfo.EnumerateFiles("*.pkg", SearchOption.AllDirectories))
                {
                    if (file.Directory == null || file.Directory.FullName.Length < rootDirectoryLength)
                        ExtractPKG(file);
                    else
                        ExtractPKG(file, true, file.Directory.FullName.Substring(rootDirectoryLength));
                }

                return;
            }

            foreach (var directory in directoryInfo.EnumerateDirectories())
            {
                foreach (var file in directory.EnumerateFiles("*.pkg"))
                {
                    ExtractPKG(file, true, directory.FullName.Substring(rootDirectoryLength));
                }
            }
        }

        private static void ExtractFile(FileInfo fileInfo)
        {
            if (fileInfo.Extension.Equals(".pkg", StringComparison.OrdinalIgnoreCase))
                ExtractPKG(fileInfo);
            else if (fileInfo.Extension.Equals(".tex", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    var tex = LoadTex(File.ReadAllBytes(fileInfo.FullName), fileInfo.FullName);

                    if (tex == null)
                        return;

                    var name = Path.GetFileNameWithoutExtension(fileInfo.Name);
                    tex.DecompileAndSave(Path.Combine(_options.OutputDirectory, name), _options.Overwrite);
                    tex.SaveFormatInfo(Path.Combine(_options.OutputDirectory, name), _options.Overwrite);
                }
                catch (Exception e)
                {
                    Console.WriteLine(Resources.FailedToDecompile);
                    Console.WriteLine(e);
                }
            }
            else
                Console.WriteLine(Resources.UnrecognizedFileExtension, fileInfo.Extension);
        }

        private static string GetProjectName(FileInfo packageFile, string defaultProjectName)
        {
            var directory = packageFile.Directory;
            if (directory == null)
                return defaultProjectName;

            var projectJson = directory.GetFiles("project.json");
            if (projectJson.Length == 0 || !projectJson[0].Exists)
                return defaultProjectName;

            dynamic json = JsonConvert.DeserializeObject(File.ReadAllText(projectJson[0].FullName));
            return json.title;
        }

        private static void ExtractPKG(FileInfo file, bool appendFolderName = false, string defaultProjectName = "")
        {
            var loader = new PackageLoader(true);
            var package = loader.Load(file);
            string outputDirectory;

            Console.WriteLine(Resources.ExtractingPackage, file.FullName);
            IEnumerable<Entry> entries = package.Entries;

            if (!_options.SingleDir && appendFolderName)
            {
                if (_options.UseName)
                {
                    var name = GetProjectName(file, defaultProjectName).GetSafeFilename();
                    outputDirectory = Path.Combine(_options.OutputDirectory, name);
                }
                else
                {
                    outputDirectory = Path.Combine(_options.OutputDirectory, defaultProjectName);
                }
            }
            else
            {
                outputDirectory = _options.OutputDirectory;                
            }

            if (!string.IsNullOrEmpty(_options.IgnoreExts))
            {
                entries = from entry in package.Entries
                    where !_skipExtArray.Any(s => entry.FullName.EndsWith(s, StringComparison.OrdinalIgnoreCase))
                    select entry;
            }
            else if (!string.IsNullOrEmpty(_options.OnlyExts))
            {
                entries = from entry in package.Entries
                    where _onlyExtArray.Any(s => entry.FullName.EndsWith(s, StringComparison.OrdinalIgnoreCase))
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

                if (!_options.Overwrite || File.Exists(outputPath))
                    Console.WriteLine(Resources.SkippingAlreadyExists, outputPath);
                else
                    File.Copy(fileToCopy.FullName, outputPath, true);            }
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

            if (!_options.Overwrite && File.Exists(outputPath))
                Console.WriteLine(Resources.SkippingAlreadyExists, outputPath);
            else
            {
                Console.WriteLine(Resources.ExtractingName, entry.FullName);
                entry.WriteTo(outputPath);
            }
            
            // decompile and save
            if (_options.NoTexDecompile || entry.Type != EntryType.TEX)
                return;

            try
            {
                var tex = LoadTex(entry.Data, entry.FullName);

                if (tex == null)
                    return;

                tex.DecompileAndSave(filePath, _options.Overwrite);
                tex.SaveFormatInfo(filePath, _options.Overwrite);
            }
            catch (Exception e)
            {
                Console.WriteLine(Resources.FailedToDecompile);
                Console.WriteLine(e);
            }
        }

        private static Tex LoadTex(byte[] bytes, string name)
        {
            if (Program.Closing)
                Environment.Exit(0);

            Console.WriteLine(Resources.DecompilingName, name);

            try
            {
                var tex = TexLoader.LoadTex(bytes, 1);

                if (_options.DebugInfo)
                    tex.DebugInfo();

                return tex;
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

        [Option('n', "usename", HelpText = "Use name from project.json as project subfolder name instead of id")]
        public bool UseName { get; set; }

        [Option("no-tex-decompile", HelpText = "Don't decompile .tex files while extracting .pkg")]
        public bool NoTexDecompile { get; set; }

        [Option("overwrite", HelpText = "Overwrite all existing files")]
        public bool Overwrite { get; set; }

        [Value(0, Required = true, HelpText = "Path to file/directory", MetaName = "Input")]
        public string Input { get; set; }
    }
}
