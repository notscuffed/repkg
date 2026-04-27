using RePKG.Commands;
using System.CommandLine;
using System.CommandLine.Help;

namespace RePKG
{
    internal class Program
    {
        public static bool Closing;

        private static int Main(string[] args)
        {
            Console.CancelKeyPress += Cancel;

            var root = new RootCommand("RePKG - Wallpaper Engine package tool")
            {
                Extract.BuildCommand(),
                Info.BuildCommand()
            };
            root.Action = new HelpAction();
            return root.Parse(args).Invoke();
        }

        private static void Cancel(object? sender, ConsoleCancelEventArgs e)
        {
            Closing = true;
            e.Cancel = true;
            Console.WriteLine("Terminating...");
        }
    }
}