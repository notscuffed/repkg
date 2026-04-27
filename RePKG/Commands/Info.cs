using RePKG.Core.Package;
using RePKG.Core.Package.Interfaces;
using System.CommandLine;
using System.Text.Json.Nodes;

namespace RePKG.Commands
{
    public static class Info
    {
        private static readonly IPackageReader _reader = new PackageReader();

        public static void Action(InfoOptions o)
        {
            var projectInfoToPrint = string.IsNullOrEmpty(o.ProjectInfo) ? null : o.ProjectInfo.Split(',');

            var fileInfo = new FileInfo(o.Input);
            var directoryInfo = new DirectoryInfo(o.Input);

            if (!fileInfo.Exists)
            {
                if (directoryInfo.Exists)
                {
                    if (o.TexDirectory)
                        InfoTexDirectory(o, directoryInfo);
                    else
                        InfoPkgDirectory(o, directoryInfo, projectInfoToPrint);

                    Console.WriteLine("Done");
                    return;
                }

                Console.WriteLine("Input file/directory doesn''t exist!");
                Console.WriteLine(o.Input);
                return;
            }

            InfoFile(o, fileInfo, projectInfoToPrint);
            Console.WriteLine("Done");
        }

        private static void InfoPkgDirectory(InfoOptions o, DirectoryInfo directoryInfo, string[]? projectInfoToPrint)
        {
            var rootLen = directoryInfo.FullName.Length;

            foreach (var directory in directoryInfo.EnumerateDirectories())
            {
                foreach (var file in directory.EnumerateFiles("*.pkg"))
                    InfoPkg(o, file, file.FullName.Substring(rootLen), projectInfoToPrint);
            }
        }

        private static void InfoTexDirectory(InfoOptions o, DirectoryInfo directoryInfo)
        {
        }

        private static void InfoFile(InfoOptions o, FileInfo file, string[]? projectInfoToPrint)
        {
            if (file.Extension.Equals(".pkg", StringComparison.OrdinalIgnoreCase))
                InfoPkg(o, file, Path.GetFullPath(file.Name), projectInfoToPrint);
            else if (file.Extension.Equals(".tex", StringComparison.OrdinalIgnoreCase))
                InfoTex(o, file);
            else
                Console.WriteLine($"Unrecognized file extension: {file.Extension}");
        }

        private static void InfoPkg(InfoOptions o, FileInfo file, string name, string[]? projectInfoToPrint)
        {
            var projectInfo = GetProjectInfo(file);

            if (!MatchesFilter(o, projectInfo))
                return;

            Console.WriteLine($"\r\n### Package info: {name}");

            if (projectInfo is JsonObject projectJson && projectInfoToPrint?.Length > 0)
            {
                var keys = Helper.GetPropertyKeysForDynamic(projectJson);

                if (!(projectInfoToPrint.Length == 1 && projectInfoToPrint[0] == "*"))
                    keys = keys.Where(x => projectInfoToPrint.Contains(x, StringComparer.OrdinalIgnoreCase));

                foreach (var key in keys)
                {
                    var value = projectJson[key];
                    Console.WriteLine(value == null ? $"{key}: null" : $"{key}: {value}");
                }
            }

            if (!o.PrintEntries)
                return;

            Console.WriteLine("Package entries:");

            Core.Package.Package package;
            using (var reader = new BinaryReader(file.Open(FileMode.Open, FileAccess.Read, FileShare.Read)))
                package = _reader.ReadFrom(reader);

            var entries = package.Entries;

            if (o.Sort)
            {
                entries.Sort(o.SortBy == "size"
                    ? (a, b) => a.Length.CompareTo(b.Length)
                    : (a, b) => String.Compare(a.FullPath, b.FullPath, StringComparison.OrdinalIgnoreCase));
            }

            foreach (var entry in entries)
                Console.WriteLine($"* {entry.FullPath} - {entry.Length} bytes");
        }

        private static void InfoTex(InfoOptions o, FileInfo file)
        {
        }

        private static JsonNode? GetProjectInfo(FileInfo packageFile)
        {
            var directory = packageFile.Directory;
            if (directory == null)
                return null;

            var projectJson = directory.GetFiles("project.json");
            if (projectJson.Length == 0 || !projectJson[0].Exists)
                return null;

            return JsonNode.Parse(File.ReadAllText(projectJson[0].FullName));
        }

        private static bool MatchesFilter(InfoOptions o, JsonNode? project)
        {
            if (project == null || string.IsNullOrEmpty(o.TitleFilter))
                return true;

            var title = project["title"]?.GetValue<string>();
            return title == null || title.Contains(o.TitleFilter, StringComparison.OrdinalIgnoreCase);
        }

        public static Command BuildCommand()
        {
            var inputArg = new Argument<string>("input") { Description = "Path to file/directory" };
            var sortOpt = new Option<bool>("--sort", ["-s"]) { Description = "Sort entries a-z" };
            var sortByOpt = new Option<string>("--sortby", ["-b"]) { Description = "Sort by name/extension/size", DefaultValueFactory = _ => "name" };
            var texOpt = new Option<bool>("--tex", ["-t"]) { Description = "Dump info about all tex files from specified directory" };
            var projOpt = new Option<string?>("--projectinfo", ["-p"]) { Description = "Keys from project.json (comma-separated, * for all)" };
            var printOpt = new Option<bool>("--printentries", ["-e"]) { Description = "Print entries in packages" };
            var filterOpt = new Option<string?>("--title-filter") { Description = "Title filter" };

            var cmd = new Command("info", "Dumps PKG/TEX info");
            cmd.Add(inputArg);
            cmd.Add(sortOpt); cmd.Add(sortByOpt); cmd.Add(texOpt);
            cmd.Add(projOpt); cmd.Add(printOpt); cmd.Add(filterOpt);

            cmd.SetAction((ParseResult pr) => Action(new InfoOptions
            {
                Input = pr.GetValue(inputArg)!,
                Sort = pr.GetValue(sortOpt),
                SortBy = pr.GetValue(sortByOpt)!,
                TexDirectory = pr.GetValue(texOpt),
                ProjectInfo = pr.GetValue(projOpt),
                PrintEntries = pr.GetValue(printOpt),
                TitleFilter = pr.GetValue(filterOpt),
            }));

            return cmd;
        }
    }

    public class InfoOptions
    {
        public string Input { get; set; } = string.Empty;
        public bool Sort { get; set; }
        public string SortBy { get; set; } = "name";
        public bool TexDirectory { get; set; }
        public string? ProjectInfo { get; set; }
        public bool PrintEntries { get; set; }
        public string? TitleFilter { get; set; }
    }
}