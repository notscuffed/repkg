using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using CommandLine;
using Newtonsoft.Json;
using RePKG.Package;
using RePKG.Properties;

namespace RePKG.Command
{
    public class Info
    {
        private static InfoOptions _options;
        private static string[] _projectInfoToPrint;

        public static void Action(InfoOptions options)
        {
            _options = options;

            if (string.IsNullOrEmpty(_options.ProjectInfo))
                _projectInfoToPrint = null;
            else
                _projectInfoToPrint = _options.ProjectInfo.Split(',');

            var fileInfo = new FileInfo(options.Input);
            var directoryInfo = new DirectoryInfo(options.Input);

            if (!fileInfo.Exists)
            {
                if (directoryInfo.Exists)
                {
                    if (_options.TexDirectory)
                        InfoTexDirectory(directoryInfo);
                    else
                        InfoPkgDirectory(directoryInfo);

                    Console.WriteLine(Resources.Done);
                    return;
                }

                Console.WriteLine(Resources.InputNotFound);
                Console.WriteLine(options.Input);
                return;
            }
            
            InfoFile(fileInfo);
            Console.WriteLine(Resources.Done);
        }

        private static void InfoPkgDirectory(DirectoryInfo directoryInfo)
        {
            var rootDirectoryLength = directoryInfo.FullName.Length;

            foreach (var directory in directoryInfo.EnumerateDirectories())
            {
                foreach (var file in directory.EnumerateFiles("*.pkg"))
                {
                    InfoPkg(file, file.FullName.Substring(rootDirectoryLength));
                }
            }
        }

        private static void InfoTexDirectory(DirectoryInfo directoryInfo)
        {

        }

        private static void InfoFile(FileInfo file)
        {
            if (file.Extension.Equals(".pkg", StringComparison.OrdinalIgnoreCase))
                InfoPkg(file, Path.GetFullPath(file.Name));
            else if (file.Extension.Equals(".tex", StringComparison.OrdinalIgnoreCase))
                InfoTex(file);
            else
                Console.WriteLine(Resources.UnrecognizedFileExtension, file.Extension);
        }

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        private static void InfoPkg(FileInfo file, string name)
        {
            var projectInfo = GetProjectInfo(file);

            if (!MatchesFilter(projectInfo))
                return;

            Console.WriteLine(Resources.InfoAboutPackage, name);

            if (projectInfo != null && _projectInfoToPrint != null && _projectInfoToPrint.Length > 0)
            {
                IEnumerable<string> projectInfoEnumerator;

                if (_projectInfoToPrint.Length == 1 && _projectInfoToPrint[0] == "*")
                    projectInfoEnumerator = Helper.GetPropertyKeysForDynamic(projectInfo);
                else
                {
                    projectInfoEnumerator = Helper.GetPropertyKeysForDynamic(projectInfo);
                    projectInfoEnumerator = projectInfoEnumerator.Where(x =>
                        _projectInfoToPrint.Contains(x, StringComparer.OrdinalIgnoreCase));
                }

                foreach (var key in projectInfoEnumerator)
                {
                    if (projectInfo[key] == null)
                        Console.WriteLine(key + @": null");
                    else
                        Console.WriteLine(key + @": " + projectInfo[key].ToString());
                }
            }

            if (_options.PrintEntries)
            {
                Console.WriteLine(Resources.PackageEntries);

                var loader = new PackageLoader(false);
                var package = loader.Load(file);

                var entries = package.Entries;

                if (_options.Sort)
                {
                    if (_options.SortBy == "extension")
                        entries.Sort((a, b) =>
                            String.Compare(a.EntryPath, b.EntryPath, StringComparison.OrdinalIgnoreCase));
                    else if (_options.SortBy == "size")
                        entries.Sort((a, b) => a.Length.CompareTo(b.Length));
                    else
                        entries.Sort((a, b) =>
                            String.Compare(a.EntryPath, b.EntryPath, StringComparison.OrdinalIgnoreCase));
                }

                foreach (var entry in entries)
                {
                    Console.WriteLine(@"* " + entry.FullName + $@" - {entry.Length} bytes");
                }
            }
        }

        private static void InfoTex(FileInfo file)
        {

        }

        private static dynamic GetProjectInfo(FileInfo packageFile)
        {
            var directory = packageFile.Directory;
            if (directory == null)
                return null;

            var projectJson = directory.GetFiles("project.json");
            if (projectJson.Length == 0 || !projectJson[0].Exists)
                return null;

            return JsonConvert.DeserializeObject(File.ReadAllText(projectJson[0].FullName));
        }

        private static bool MatchesFilter(dynamic project)
        {
            if (project == null)
                return true;

            if (!string.IsNullOrEmpty(_options.TitleFilter))
            {
                var title = (string) project.title;
                if (!title.Contains(_options.TitleFilter, StringComparison.OrdinalIgnoreCase))
                    return false;
            }

            return true;
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

        [Option('t', "tex", HelpText = "Get info about all tex files from specified directory in input")]
        public bool TexDirectory { get; set; }

        [Option('p', "projectinfo", HelpText = "Select info from project.json to print (delimit using comma)")]
        public string ProjectInfo { get; set; }

        [Option('e', "printentries", HelpText = "Print entries in packages")]
        public bool PrintEntries { get; set; }

        [Option("title-filter", HelpText = "Title filter")]
        public string TitleFilter { get; set; }
    }
}
