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
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;
using SimilarityTreeExplorer.SubTree;
using SimilarityTreeExplorer.SuperTree;
using SyntaxComparision.Algorithm;
using SyntaxComparision.Data;
using SyntaxComparision.Interfaces;
using SyntaxVectors;
using SyntaxVectors.Masking;

namespace SimilarityAnalyzerManualExecutor
{
    class Program
    {
        static void Main(string[] args)
        {
            TestNUnit();
            //TestLocal();

        }

        private static void TestNUnit()
        {
            var path = @"C:\Git Repos\NUnit\nunit\nunit.sln";
            //var pathToPorject = @"C:\Git Repos\NUnit\nunit\src\NUnitFramework\nunitlite-runner\nunitlite-runner-4.5.csproj";
            var pathToPorject = @"C:\Git Repos\NUnit\nunit\src\NUnitFramework\nunitlite\nunitlite-4.5.csproj";

            var exec = new Executor(true);
            //exec.Execute(path);
            exec.ExecuteForProject(pathToPorject, new HashSet<string> {"sub", "sup" ,"sub+df", "sub+df+seman", "sub+com" });
        }

        private static void TestLocal()
        {
            var path = @"C:\Git Repos\Roslyn\SimilarityRefactory\SimilarityAnalyzer.sln";
            //var pathToPorject = @"C:\Git Repos\NUnit\nunit\src\NUnitFramework\nunitlite-runner\nunitlite-runner-4.5.csproj";
            var pathToPorject = @"C:\Git Repos\Roslyn\SimilarityRefactory\SimilarityExample\SimilarityExample.csproj";

            var exec = new Executor(true);
            //exec.Execute(path);
            exec.ExecuteForProject(pathToPorject, new HashSet<string> {"sub+df", "sub+df+seman", "sub+com" });
        }
    }
}
