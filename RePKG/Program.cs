using System;
using System.Collections.Generic;
using CommandLine;
using RePKG.Command;

namespace RePKG
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length > 0 && args[0] == "interactive")
            {
                InteractiveConsole();
                return;
            }

            Parser.Default.ParseArguments<ExtractOptions, InfoOptions>(args)
                .WithParsed<ExtractOptions>(Extract.Action)
                .WithParsed<InfoOptions>(Info.Action)
                .WithNotParsed(NotParsedAction);
        }

        private static void InteractiveConsole()
        {
            Console.WriteLine(@"RePKG started in interactive mode. You can now type commands");
            Console.WriteLine(@"Type ""help"" for commands");
            
            string line;

            while ((line = Console.ReadLine()) != string.Empty)
            {
                var debugArgs = line.SplitArguments();

                Parser.Default.ParseArguments<ExtractOptions, InfoOptions>(debugArgs)
                    .WithParsed<ExtractOptions>(Extract.Action)
                    .WithParsed<InfoOptions>(Info.Action)
                    .WithNotParsed(NotParsedAction);
            }
        }


        private static void NotParsedAction(IEnumerable<Error> errors)
        {
            /*foreach (var error in errors)
            {
                if (error.Tag == ErrorType.HelpRequestedError)
                    continue;

                Console.WriteLine(error.Tag);
            }*/
        }
    }
}
