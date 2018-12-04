using System;
using System.Collections.Generic;
using System.Linq;

namespace SimilarityAnalyzerManualExecutor
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Started!");

            if (args.Length == 1 && (args[0] == "-h" || args[0] == "-H" || args[0] == "\\h" || args[0] == "--help"))
                WriteHelp();
            else if (args.Length == 1 && args[0] == "-disambiguation")
                WriteDisambiguation();
            else if (args.Length >= 3)
            {
                var target = args[0];
                if (!int.TryParse(args[1], out var minDepth))
                    goto Failed;
                var keys = new HashSet<string>(args.Skip(2));
                Execute(target, minDepth, keys);
            }
#if DEBUG
            else
            {
                var pathToPorject = @"C:\Git Repos\NUnit\nunit\src\NUnitFramework\nunitlite\nunitlite-4.5.csproj";
                //string pathToPorject = @"C:\Git Repos\NUnit\nunit\src\NUnitFramework\framework\nunit.framework-4.5.csproj";

                Execute(pathToPorject, 111, new HashSet<string> {
                    "sub",
                    //"sup",
                    "sub+vec",
                    //"sub+vec+seman",
                    //"sub+vec+seman",
                    //"sub+vec+seman+lit+id",
                    //"sub+vec+seman+df",
                    //"sub+vec+seman+df+refactInvoc+refactMember",
                    //"sub+vec+com+df",
                    //"sub+vec+com+df+refactInvoc+refactMember"
                });
            }
#else
            else
                goto Failed;
#endif

            Console.WriteLine("Finished");
#if DEBUG
            Console.ReadKey();
#endif
            // Successful
            return;

            Failed:
            Console.WriteLine("Incorrect arguments!");
            WriteHelp();
            WriteDisambiguation();
        }

        private static void WriteHelp()
        {
            Console.WriteLine("Please pass the following arguments:\n" +
                "1st: absolute path to target project's csproj file (only support .NET Framework)\n" +
                "2nd: minimum size of code fragments to be analyzer, RECOMMENDED: 25\n" +
                "3rd and further (OPTIONAL): any number of analyzer configuration keys\n" +
                "" +
                "Valid configuration keys: sub, sup, sub+vec, sub+vec+seman, sub+vec+seman+lit+id, sub+vec+seman+df, sub+vec+seman+df+refactInvoc+refactMember, sub+vec+com+df, sub+vec+com+df+refactInvoc+refactMember\n" +
                "The order of the 3rd and further arguments are ignored, the order of execution is preset" +
                "For disambiguation, run the program with the only command line argument -disambiguation");
        }

        private static void WriteDisambiguation()
        {
            Console.WriteLine("sub: subtree strategy\n" +
                "sup: supertree strategy\n" +
                "vec: using syntax vectors to prune duplicates\n" +
                "seman: semantical analysis of type matchings\n" +
                "comp: sematnical analysis of type matching allowing compatible types" +
                "lit: comparision of the literal values\n" +
                "id: value comparision of the identifiers\n" +
                "df: dataflow comparision of the identifiers\n" +
                "refactMember: comparision of identifiers for refactorability: only first member access differs\n" +
                "refactInvoc: comparision of identifiers for refactorability: methods are equal");
        }

        private static void Execute(string target, int minDepth, HashSet<string> keys)
        {
            var logger = new Logger(target, minDepth);
            var executor = new Executor(logger);

            executor.ExecuteForProject(target, minDepth, keys);
        }
    }
}
