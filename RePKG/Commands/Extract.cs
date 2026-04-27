using RePKG.Core.Package;
using RePKG.Core.Package.Enums;
using RePKG.Core.Package.Interfaces;
using RePKG.Core.Texture;
using System.CommandLine;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json.Nodes;

namespace RePKG.Commands
{
    public static class Extract
    {
        private static readonly string[] ProjectFiles = ["project.json"];
        private static readonly ITexReader _texReader = TexReader.Default;
        private static readonly ITexJsonInfoGenerator _texJsonInfoGenerator = new TexJsonInfoGenerator();
        private static readonly IPackageReader _packageReader = new PackageReader();
        private static readonly TexToImageConverter _texToImageConverter = new();

        public static void Action(ExtractOptions o)
        {
            if (string.IsNullOrEmpty(o.OutputDirectory))
                o.OutputDirectory = Directory.GetCurrentDirectory();

            var fileInfo = new FileInfo(o.Input);
            var directoryInfo = new DirectoryInfo(o.Input);

            if (!fileInfo.Exists)
            {
                if (directoryInfo.Exists)
                {
                    if (o.TexDirectory)
                        ExtractTexDirectory(o, directoryInfo);
                    else
                        ExtractPkgDirectory(o, directoryInfo);

                    Console.WriteLine("Done");
                    return;
                }

                Console.WriteLine("Input file not found");
                Console.WriteLine(o.Input);
                return;
            }

            ExtractFile(o, fileInfo);
            Console.WriteLine("Done");
        }

        private static void ExtractTexDirectory(ExtractOptions o, DirectoryInfo directoryInfo)
        {
            var flags = o.Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

            Directory.CreateDirectory(o.OutputDirectory);

            foreach (var fileInfo in directoryInfo.EnumerateFiles("*.tex", flags))
            {
                if (!fileInfo.Extension.Equals(".tex", StringComparison.OrdinalIgnoreCase))
                    continue;

                try
                {
                    var tex = LoadTex(o, File.ReadAllBytes(fileInfo.FullName), fileInfo.FullName);
                    if (tex == null)
                        continue;

                    var filePath = Path.Combine(o.OutputDirectory, Path.GetFileNameWithoutExtension(fileInfo.Name));
                    ConvertToImageAndSave(o, tex, filePath);
                    File.WriteAllText($"{filePath}.tex-json", _texJsonInfoGenerator.GenerateInfo(tex));
                }
                catch (Exception e)
                {
                    Console.WriteLine("Failed to write texture");
                    Console.WriteLine(e);
                }
            }
        }

        private static void ExtractPkgDirectory(ExtractOptions o, DirectoryInfo directoryInfo)
        {
            var rootLen = directoryInfo.FullName.Length + 1;

            if (o.Recursive)
            {
                foreach (var file in directoryInfo.EnumerateFiles("*.pkg", SearchOption.AllDirectories))
                {
                    if (file.Directory == null || file.Directory.FullName.Length < rootLen)
                        ExtractPkg(o, file);
                    else
                        ExtractPkg(o, file, true, file.Directory.FullName[rootLen..]);
                }
                return;
            }

            foreach (var directory in directoryInfo.EnumerateDirectories())
            {
                foreach (var file in directory.EnumerateFiles("*.pkg"))
                    ExtractPkg(o, file, true, directory.FullName[rootLen..]);
            }
        }

        private static void ExtractFile(ExtractOptions o, FileInfo fileInfo)
        {
            Directory.CreateDirectory(o.OutputDirectory);

            if (fileInfo.Extension.Equals(".pkg", StringComparison.OrdinalIgnoreCase))
            {
                ExtractPkg(o, fileInfo);
            }
            else if (fileInfo.Extension.Equals(".tex", StringComparison.OrdinalIgnoreCase))
            {
                var tex = LoadTex(o, File.ReadAllBytes(fileInfo.FullName), fileInfo.FullName);
                if (tex == null)
                    return;

                try
                {
                    var filePath = Path.Combine(o.OutputDirectory, Path.GetFileNameWithoutExtension(fileInfo.Name));
                    ConvertToImageAndSave(o, tex, filePath);
                    File.WriteAllText($"{filePath}.tex-json", _texJsonInfoGenerator.GenerateInfo(tex));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            else
            {
                Console.WriteLine($"Unrecognized file extension: {fileInfo.Extension}");
            }
        }

        private static void ExtractPkg(ExtractOptions o, FileInfo file, bool appendFolderName = false, string defaultProjectName = "")
        {
            Console.WriteLine($"\r\n### Extracting package: {file.FullName}");

            Core.Package.Package package;
            using (var reader = new BinaryReader(file.Open(FileMode.Open, FileAccess.Read, FileShare.Read)))
                package = _packageReader.ReadFrom(reader);

            string outputDirectory;
            var preview = string.Empty;
            if (appendFolderName)
                GetProjectFolderNameAndPreviewImage(o, file, defaultProjectName, out outputDirectory, out preview);
            else
                outputDirectory = o.OutputDirectory;

            foreach (var entry in FilterEntries(o, package.Entries))
                ExtractEntry(o, entry, ref outputDirectory);

            if (!o.CopyProject || o.SingleDir || file.Directory == null)
                return;

            var files = file.Directory.GetFiles().Where(x =>
                x.Name.Equals(preview, StringComparison.OrdinalIgnoreCase) ||
                ProjectFiles.Contains(x.Name, StringComparer.OrdinalIgnoreCase));

            CopyFiles(o, files, outputDirectory);
        }

        private static void CopyFiles(ExtractOptions o, IEnumerable<FileInfo> files, string outputDirectory)
        {
            foreach (var file in files)
            {
                var outputPath = Path.Combine(outputDirectory, file.Name);
                if (!o.Overwrite && File.Exists(outputPath))
                    Console.WriteLine($"* Skipping, already exists: {outputPath}");
                else
                {
                    File.Copy(file.FullName, outputPath, true);
                    Console.WriteLine($"* Copying: {file.FullName}");
                }
            }
        }

        private static IEnumerable<PackageEntry> FilterEntries(ExtractOptions o, IEnumerable<PackageEntry> entries)
        {
            if (!string.IsNullOrEmpty(o.IgnoreExts))
            {
                var skip = NormalizeExtensions(o.IgnoreExts.Split(','));
                return entries.Where(e => !skip.Any(s => e.FullPath.EndsWith(s, StringComparison.OrdinalIgnoreCase)));
            }

            if (!string.IsNullOrEmpty(o.OnlyExts))
            {
                var only = NormalizeExtensions(o.OnlyExts.Split(','));
                return entries.Where(e => only.Any(s => e.FullPath.EndsWith(s, StringComparison.OrdinalIgnoreCase)));
            }

            return entries;
        }

        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        private static void ExtractEntry(ExtractOptions o, PackageEntry entry, ref string outputDirectory)
        {
            if (Program.Closing)
                Environment.Exit(0);

            var filePathNoExt = o.SingleDir
                ? Path.Combine(outputDirectory, entry.Name)
                : Path.Combine(outputDirectory, entry.DirectoryPath, entry.Name);

            var filePath = filePathNoExt + entry.Extension;

            Directory.CreateDirectory(Path.GetDirectoryName(filePathNoExt));

            if (!o.Overwrite && File.Exists(filePath))
                Console.WriteLine($"* Skipping, already exists: {filePath}");
            else
            {
                Console.WriteLine($"* Extracting: {entry.FullPath}");
                File.WriteAllBytes(filePath, entry.Bytes);
            }

            if (o.NoTexConvert || entry.Type != EntryType.Tex)
                return;

            var tex = LoadTex(o, entry.Bytes, entry.FullPath);
            if (tex == null)
                return;

            try
            {
                ConvertToImageAndSave(o, tex, filePathNoExt);
                File.WriteAllText($"{filePathNoExt}.tex-json", _texJsonInfoGenerator.GenerateInfo(tex));
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to write texture");
                Console.WriteLine(e);
            }
        }

        private static void GetProjectFolderNameAndPreviewImage(ExtractOptions o, FileInfo packageFile,
            string defaultProjectName, out string outputDirectory, out string preview)
        {
            preview = string.Empty;

            if (o.SingleDir)
            {
                outputDirectory = o.OutputDirectory;
                return;
            }

            if (o.UseName)
            {
                var name = defaultProjectName;
                GetProjectInfo(packageFile, ref name, ref preview);
                outputDirectory = Path.Combine(o.OutputDirectory, name.GetSafeFilename());
                return;
            }

            outputDirectory = Path.Combine(o.OutputDirectory, defaultProjectName);
        }

        private static void GetProjectInfo(FileInfo packageFile, ref string title, ref string preview)
        {
            var directory = packageFile.Directory;
            if (directory == null)
                return;

            var projectJson = directory.GetFiles("project.json");
            if (projectJson.Length == 0 || !projectJson[0].Exists)
                return;

            var json = JsonNode.Parse(File.ReadAllText(projectJson[0].FullName));
            if (json is JsonObject obj)
            {
                title = obj["title"]?.GetValue<string>() ?? title;
                preview = obj["preview"]?.GetValue<string>() ?? preview;
            }
        }

        private static ITex? LoadTex(ExtractOptions o, byte[] bytes, string name)
        {
            if (Program.Closing)
                Environment.Exit(0);

            Console.WriteLine("* Reading: {0}", name);

            try
            {
                using var reader = new BinaryReader(new MemoryStream(bytes), Encoding.UTF8);
                return _texReader.ReadFrom(reader);
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to read texture");
                Console.WriteLine(e);
            }

            return null;
        }

        private static void ConvertToImageAndSave(ExtractOptions o, ITex tex, string path)
        {
            var format = _texToImageConverter.GetConvertedFormat(tex);
            var outputPath = $"{path}.{format.GetFileExtension()}";

            if (!o.Overwrite && File.Exists(outputPath))
                return;

            File.WriteAllBytes(outputPath, _texToImageConverter.ConvertToImage(tex).Bytes);
        }

        private static string[] NormalizeExtensions(string[] array)
        {
            ArgumentNullException.ThrowIfNull(array);

            for (int i = 0; i < array.Length; i++)
                if (!array[i].StartsWith("."))
                    array[i] = '.' + array[i];
            return array;
        }

        public static Command BuildCommand()
        {
            var inputArg = new Argument<string>("input") { Description = "Path to file/directory" };
            var outputOpt = new Option<string>("--output", ["-o"]) { Description = "Output directory", DefaultValueFactory = _ => "./output" };
            var ignoreOpt = new Option<string?>("--ignoreexts", ["-i"]) { Description = "Don't extract files with these extensions (comma-separated)" };
            var onlyOpt = new Option<string?>("--onlyexts", ["-e"]) { Description = "Only extract files with these extensions (comma-separated)" };
            var texOpt = new Option<bool>("--tex", ["-t"]) { Description = "Convert all tex files into images from specified directory" };
            var sdirOpt = new Option<bool>("--singledir", ["-s"]) { Description = "Put all extracted files in one directory" };
            var recurOpt = new Option<bool>("--recursive", ["-r"]) { Description = "Recursive search in all subfolders" };
            var copyOpt = new Option<bool>("--copyproject", ["-c"]) { Description = "Copy project.json and preview.jpg into output directory" };
            var nameOpt = new Option<bool>("--usename", ["-n"]) { Description = "Use project.json title as subfolder name instead of id" };
            var noTexOpt = new Option<bool>("--no-tex-convert") { Description = "Don't convert TEX files into images while extracting PKG" };
            var overOpt = new Option<bool>("--overwrite") { Description = "Overwrite all existing files" };

            var cmd = new Command("extract", "Extract PKG / Convert TEX into image")
            {
                inputArg,
                outputOpt,
                ignoreOpt,
                onlyOpt,
                texOpt,
                sdirOpt,
                recurOpt,
                copyOpt,
                nameOpt,
                noTexOpt,
                overOpt
            };

            cmd.SetAction(pr => Action(new ExtractOptions
            {
                Input = pr.GetValue(inputArg)!,
                OutputDirectory = pr.GetValue(outputOpt) ?? "./output",
                IgnoreExts = pr.GetValue(ignoreOpt),
                OnlyExts = pr.GetValue(onlyOpt),
                TexDirectory = pr.GetValue(texOpt),
                SingleDir = pr.GetValue(sdirOpt),
                Recursive = pr.GetValue(recurOpt),
                CopyProject = pr.GetValue(copyOpt),
                UseName = pr.GetValue(nameOpt),
                NoTexConvert = pr.GetValue(noTexOpt),
                Overwrite = pr.GetValue(overOpt),
            }));

            return cmd;
        }
    }

    public class ExtractOptions
    {
        public string Input { get; set; } = string.Empty;
        public string OutputDirectory { get; set; } = string.Empty;
        public string? IgnoreExts { get; set; }
        public string? OnlyExts { get; set; }
        public bool TexDirectory { get; set; }
        public bool SingleDir { get; set; }
        public bool Recursive { get; set; }
        public bool CopyProject { get; set; }
        public bool UseName { get; set; }
        public bool NoTexConvert { get; set; }
        public bool Overwrite { get; set; }
    }
}