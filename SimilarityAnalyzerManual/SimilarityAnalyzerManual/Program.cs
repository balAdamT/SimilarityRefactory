using System.Collections.Generic;

namespace SimilarityAnalyzerManualExecutor
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            TestNUnit();
            //TestLocal();

        }

        private static void TestNUnit()
        {
            //var pathToPorject = @"C:\Git Repos\NUnit\nunit\src\NUnitFramework\nunitlite-runner\nunitlite-runner-4.5.csproj";
            string pathToPorject = @"C:\Git Repos\NUnit\nunit\src\NUnitFramework\nunitlite\nunitlite-4.5.csproj";

            Executor exec = new Executor(true);
            //exec.Execute(path);
            exec.ExecuteForProject(pathToPorject, new HashSet<string> { "sub", "sup", "sub+df", "sub+df+seman", "sub+com" });
        }

        private static void TestLocal()
        {
            //var pathToPorject = @"C:\Git Repos\NUnit\nunit\src\NUnitFramework\nunitlite-runner\nunitlite-runner-4.5.csproj";
            string pathToPorject = @"C:\Git Repos\Roslyn\SimilarityRefactory\SimilarityExample\SimilarityExample.csproj";

            Executor exec = new Executor(true);
            //exec.Execute(path);
            exec.ExecuteForProject(pathToPorject, new HashSet<string> { "sub", "sub+df", "sub+df+seman", "sub+com" });
        }
    }
}
