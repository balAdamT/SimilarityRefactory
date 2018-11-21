using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using SimilarityAnalyzer.Helpers;
using SimilarityTreeExplorer.SubTree;
using SimilarityTreeExplorer.SuperTree;
using SyntaxComparision.Algorithm;
using SyntaxComparision.Data;
using SyntaxComparision.Information;
using SyntaxComparision.Interfaces;
using SyntaxVectors;
using SyntaxVectors.Masking;

namespace SimilarityAnalyzerManualExecutor
{
    internal class Executor
    {
        private bool list = false;

        NodeToNode identity = new NodeToNode();
        NodeToVector withVector = new NodeToVector(SyntaxMasks.AllNodes);
        ISyntaxComparator<SyntaxLeafPair<NodeAsRepresentation>, NodeAsRepresentation, OncePerTreeInformation> superComparator = new SuperTreeComparator<SyntaxLeafPair<NodeAsRepresentation>, NodeAsRepresentation, OncePerTreeInformation>();
        ISyntaxComparator<SyntaxPair<NodeAsRepresentation>, NodeAsRepresentation, OncePerTreeInformation> subComparator = new StructuralComparator<SyntaxPair<NodeAsRepresentation>, NodeAsRepresentation, OncePerTreeInformation>();
        ISyntaxComparator<SyntaxPair<NodeWithVector>, NodeWithVector, OncePerTreeInformation> subComparatorWithVector = new StructuralComparator<SyntaxPair<NodeWithVector>, NodeWithVector, OncePerTreeInformation>();
        ISyntaxComparator<SyntaxPair<NodeWithVector>, NodeWithVector, OncePerTreeInformation> vectorComparator = new VectorComparator<SyntaxPair<NodeWithVector>, NodeWithVector, OncePerTreeInformation>();
        ISyntaxComparator<SyntaxPair<NodeAsRepresentation>, NodeAsRepresentation, OncePerTreeInformation> dfComparataor = new DataflowComparator<SyntaxPair<NodeAsRepresentation>, NodeAsRepresentation, OncePerTreeInformation>();
        ISyntaxComparator<SyntaxPair<NodeAsRepresentation>, NodeAsRepresentation, OncePerTreeInformation> semanticComparator = new SemanticComparator<SyntaxPair<NodeAsRepresentation>, NodeAsRepresentation, OncePerTreeInformation>();
        ISyntaxComparator<SyntaxPair<NodeAsRepresentation>, NodeAsRepresentation, OncePerTreeInformation> compatibleComparator = new CompatibleComparator<SyntaxPair<NodeAsRepresentation>, NodeAsRepresentation, OncePerTreeInformation>();



        public Executor(bool list)
        {
            this.list = list;
        }

        public void Execute(string path)
        {
            MSBuildWorkspace workspace = MSBuildWorkspace.Create();
            Solution solution = workspace.OpenSolutionAsync(path).Result;
            var projects = solution.Projects.Where(p => p.Name.EndsWith("-4.5")).ToList();

            Parallel.ForEach(projects, (p) => ExecuteForProject(p, null));

            Console.WriteLine("Done!");
            Console.ReadKey();
        }

        public void ExecuteForProject(string path, HashSet<string> enabled)
        {
            MSBuildWorkspace workspace = MSBuildWorkspace.Create();
            Project project = workspace.OpenProjectAsync(path).Result;
            Console.WriteLine("Loaded project");
            while (true)
            {
                ExecuteForProject(project, enabled);
                Console.WriteLine("Done!");
                Console.ReadKey();
            }
        }

        private void ExecuteForProject(Project project, HashSet<string> enabled)
        {
            var projectName = project.Name;
            var compilation = project.GetCompilationAsync().Result;
            var key = "";

            key = "sub";
            if (enabled == null || enabled.Contains(key))
            {
                var m1 = Config1(compilation);
                Log(projectName, key, m1);
            }

            key = "sup";
            if (enabled == null || enabled.Contains(key))
            {
                var m2 = Config2(compilation);
                Log(projectName, key, m2);
            }

            key = "vec";
            if (enabled == null || enabled.Contains(key))
            {
                var m3 = Config3(compilation);
                Log(projectName, key, m3);
            }

            key = "sub+df";
            if (enabled == null || enabled.Contains(key))
            {
                var m4 = Config4(compilation);
                Log(projectName, key, m4);
            }

            key = "sub+df+seman";
            if (enabled == null || enabled.Contains(key))
            {
                var m4 = Config5(compilation);
                Log(projectName, key, m4);
            }

            key = "sub+com";
            if (enabled == null || enabled.Contains(key))
            {
                var m4 = Config6(compilation);
                Log(projectName, key, m4);
            }
        }

        private void Log(string projectName, string configName, SimilarityMeasure measure)
        {
            Console.WriteLine($@"{projectName}:{configName.ToUpper()}:{measure}");
        }

        private SimilarityMeasure Config6(Compilation compilation)
        {
            var source = new MethodFragmentsInCompilation(compilation, 8);
            var information = new OncePerTreeInformation(compilation);
            var analyzer = new MeasuredSimilarityFinder<SyntaxPair<NodeAsRepresentation>, NodeAsRepresentation, OncePerTreeInformation>(source, identity, new List<ISyntaxComparator<SyntaxPair<NodeAsRepresentation>, NodeAsRepresentation, OncePerTreeInformation>>() { subComparator, compatibleComparator }, information);
            var similarities = analyzer.FindAll().ToList();

            if (list)
                ListSimilarities(similarities);

            return analyzer.Measure;
        }

        private SimilarityMeasure Config5(Compilation compilation)
        {
            var source = new MethodFragmentsInCompilation(compilation, 8);
            var information = new OncePerTreeInformation(compilation);
            var analyzer = new MeasuredSimilarityFinder<SyntaxPair<NodeAsRepresentation>, NodeAsRepresentation, OncePerTreeInformation>(source, identity, new List<ISyntaxComparator<SyntaxPair<NodeAsRepresentation>, NodeAsRepresentation, OncePerTreeInformation>>() { subComparator, dfComparataor, semanticComparator }, information);
            var similarities = analyzer.FindAll().ToList();

            if (list)
                ListSimilarities(similarities);

            return analyzer.Measure;
        }

        private SimilarityMeasure Config4(Compilation compilation)
        {
            var source = new MethodFragmentsInCompilation(compilation, 8);
            var information = new OncePerTreeInformation(compilation);
            var analyzer = new MeasuredSimilarityFinder<SyntaxPair<NodeAsRepresentation>, NodeAsRepresentation, OncePerTreeInformation>(source, identity, new List<ISyntaxComparator<SyntaxPair<NodeAsRepresentation>, NodeAsRepresentation, OncePerTreeInformation>>() { subComparator, dfComparataor }, information);
            var similarities = analyzer.FindAll().ToList();

            if (list)
                ListSimilarities(similarities);

            return analyzer.Measure;
        }

        private SimilarityMeasure Config3(Compilation compilation)
        {
            var source = new MethodFragmentsInCompilation(compilation: compilation, minDepth: 8);
            var information = new OncePerTreeInformation(compilation);
            var analyzer = new MeasuredSimilarityFinder<SyntaxPair<NodeWithVector>, NodeWithVector, OncePerTreeInformation>(source, withVector, new List<ISyntaxComparator<SyntaxPair<NodeWithVector>, NodeWithVector, OncePerTreeInformation>>() { vectorComparator, subComparatorWithVector }, information);
            var similarities = analyzer.FindAll().ToList();

            if (list)
                ListSimilarities(similarities);

            return analyzer.Measure;
        }

        private SimilarityMeasure Config2(Compilation compilation)
        {
            var source = new LeavesInCompilation(compilation);
            //TODO in this case we don't know the semantic model...
            var information = new OncePerTreeInformation(compilation);
            var analyzer = new MeasuredSimilarityFinder<SyntaxLeafPair<NodeAsRepresentation>, NodeAsRepresentation, OncePerTreeInformation>(source, identity, Enumerable.Repeat(superComparator, 1).ToList(), information);
            var similarities = analyzer.FindAll().ToList();

            if (list)
                ListSimilarities(similarities);

            return analyzer.Measure;
        }

        private SimilarityMeasure Config1(Compilation compilation)
        {
            var source = new MethodFragmentsInCompilation(compilation, 8);
            var information = new OncePerTreeInformation(compilation);
            var analyzer = new MeasuredSimilarityFinder<SyntaxPair<NodeAsRepresentation>, NodeAsRepresentation, OncePerTreeInformation>(source, identity, Enumerable.Repeat(subComparator, 1).ToList(), information);
            var similarities = analyzer.FindAll().ToList();

            if (list)
                ListSimilarities(similarities);

            return analyzer.Measure;
        }

        private void ListSimilarities(List<SyntaxPair<NodeAsRepresentation>> similarities)
        {
            foreach (var similiarity in similarities)
            {
                Console.WriteLine(
                  $@"{similiarity.Left.Node.GetLocation().GetLineSpan()} vs {similiarity.Right.Node.GetLocation().GetLineSpan()}"
                );
            }
        }
        private void ListSimilarities(List<SyntaxLeafPair<NodeAsRepresentation>> similarities)
        {
            foreach (var similiarity in similarities)
            {
                Console.WriteLine(
                    $@"{similiarity.Left.Node.GetLocation().GetLineSpan()} vs {similiarity.Right.Node.GetLocation().GetLineSpan()}"
                );
            }
        }


        private void ListSimilarities(List<SyntaxPair<NodeWithVector>> similarities)
        {
            foreach (var similiarity in similarities)
            {
                Console.WriteLine(
                    $@"{similiarity.Left.Node.GetLocation().GetLineSpan()} vs {similiarity.Right.Node.GetLocation().GetLineSpan()}"
                );
            }
        }
    }

}
