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
                        var id = $"{projectName} {@class.Identifier} {Interlocked.Increment(ref classNumber)}";
                        var data = subFinder.MatchData;
                        Console.WriteLine($"{id},{@class.SyntaxTree.FilePath},sub,{data.RunTimeInMs},{data.InnerPairs+data.OuterPairs},{data.Matches},{data.MatchNodeLengths.DefaultIfEmpty(0).Max()}");
                        data = superFinder.MatchData;
                        Console.WriteLine($"{id},{@class.SyntaxTree.FilePath},sup,{data.RunTimeInMs},{data.InnerPairs + data.OuterPairs},{data.Matches},{data.MatchNodeLengths.DefaultIfEmpty(0).Max()}");
                    }
                });
            });
            Console.ReadKey();
        }
    }
}
