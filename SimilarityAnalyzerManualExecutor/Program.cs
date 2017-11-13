using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.MSBuild;
using SimilarityAnalyzer.SimilarityFinders;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;
using SimilarityAnalyzer.Data;
using MoreLinq;

namespace SimilarityAnalyzerManualExecutor
{
    class Program
    {
        static String Path = @"C:\Git Repos\NUnit\nunit\nunit.sln";

        static void Main(string[] args)
        {
            if (args.Length > 0)
                Path = args[0];

            MSBuildWorkspace workspace = MSBuildWorkspace.Create();

            Solution solution = workspace.OpenSolutionAsync(Path).Result;

            var projects = solution.Projects.Where(p => p.Name.EndsWith("-4.5"));

            //Write a header
            Console.WriteLine("id,path,type,runtimeMS,#pairs,matches,longestmatch,linespanleft,linespanright,longestmatchnodeifsup");

            Parallel.ForEach(projects, project =>
            {
                var projectName = project.Name;
                Compilation compilation = project.GetCompilationAsync().Result;
                var classNumber = 0;
                Parallel.ForEach(compilation.SyntaxTrees, tree => {
                    foreach (var @class in tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>())
                    {
                        var subFinder = new CommonSubTreeFinder(@class);
                        var superFinder = new CommonSuperTreeFinder(@class);
                        var hashFinder = new CommonHashFinder(@class);
                        var id = $"{projectName} {@class.Identifier} {Interlocked.Increment(ref classNumber)}";
                        DataToConsole(id, @class.SyntaxTree.FilePath, "sub", subFinder.MatchData, @class.SyntaxTree);
                        DataToConsole(id, @class.SyntaxTree.FilePath, "sup", superFinder.MatchData, @class.SyntaxTree);
                        DataToConsole(id, @class.SyntaxTree.FilePath, "hash", hashFinder.MatchData, @class.SyntaxTree);
                    }
                });
            });
            Console.ReadKey();
        }

        static void DataToConsole(string id, string path, string type, SimilarityData data, SyntaxTree containingTree)
        {
            var maxMatchSpanByLength = data.MatchSpans.DefaultIfEmpty(new Tuple<TextSpan,TextSpan>(new TextSpan(), new TextSpan())).MaxBy(s => s.Item1.Length);
            var line1 = containingTree.GetLineSpan(maxMatchSpanByLength.Item1);
            var line2 = containingTree.GetLineSpan(maxMatchSpanByLength.Item2);
            Console.WriteLine($"{id},{path},{type},{data.RunTimeInMs},{data.InnerPairs + data.OuterPairs},{data.Matches},{maxMatchSpanByLength.Item1.Length},{line1.StartLinePosition.Line}-{line1.EndLinePosition.Line},{line2.StartLinePosition.Line}-{line2.EndLinePosition.Line},{data.MatchNodeLengths.DefaultIfEmpty(0).Max()}");
        }
    }
}
