using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using SimilarityAnalyzer.SimilarityTree.SubTree;
using SimilarityAnalyzer.SimilarityTree.SuperTree;
using SimilarityAnalyzer.SyntaxComparision.Algorithm;
using SimilarityAnalyzer.SyntaxComparision.Data;
using SimilarityAnalyzer.SyntaxComparision.Information;
using SimilarityAnalyzer.SyntaxComparision.Interfaces;
using SimilarityAnalyzer.SyntaxVectors;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimilarityAnalyzerManualExecutor
{
    internal class Executor
    {
        private bool list = false;
        private NodeToNode identity = new NodeToNode();
        private NodeToVector withVector = new NodeToVector();
        private ISyntaxComparator<SyntaxLeafPair<NodeAsRepresentation>, NodeAsRepresentation, OncePerTreeInformation> superComparator = new SuperTreeComparator<SyntaxLeafPair<NodeAsRepresentation>, NodeAsRepresentation, OncePerTreeInformation>();
        private ISyntaxComparator<SyntaxPair<NodeAsRepresentation>, NodeAsRepresentation, OncePerTreeInformation> subComparator = new StructuralComparator<SyntaxPair<NodeAsRepresentation>, NodeAsRepresentation, OncePerTreeInformation>();

        private ISyntaxComparator<SyntaxPair<NodeWithVector>, NodeWithVector, OncePerTreeInformation> subComparatorWithVector = new StructuralComparator<SyntaxPair<NodeWithVector>, NodeWithVector, OncePerTreeInformation>();
        private ISyntaxComparator<SyntaxPair<NodeWithVector>, NodeWithVector, OncePerTreeInformation> vectorComparator = new VectorComparator<SyntaxPair<NodeWithVector>, NodeWithVector, OncePerTreeInformation>();

        private ISyntaxComparator<SyntaxPair<NodeAsRepresentation>, NodeAsRepresentation, OncePerTreeInformation> semanticComparator = new SemanticComparator<SyntaxPair<NodeAsRepresentation>, NodeAsRepresentation, OncePerTreeInformation>();
        private ISyntaxComparator<SyntaxPair<NodeAsRepresentation>, NodeAsRepresentation, OncePerTreeInformation> identifierComparator = new IdentifierComparator<SyntaxPair<NodeAsRepresentation>, NodeAsRepresentation, OncePerTreeInformation>();
        private ISyntaxComparator<SyntaxPair<NodeAsRepresentation>, NodeAsRepresentation, OncePerTreeInformation> literalComparator = new LiteralComparator<SyntaxPair<NodeAsRepresentation>, NodeAsRepresentation, OncePerTreeInformation>();


        private ISyntaxComparator<SyntaxPair<NodeAsRepresentation>, NodeAsRepresentation, OncePerTreeInformation> compatibleComparator = new CompatibleComparator<SyntaxPair<NodeAsRepresentation>, NodeAsRepresentation, OncePerTreeInformation>();
        private ISyntaxComparator<SyntaxPair<NodeAsRepresentation>, NodeAsRepresentation, OncePerTreeInformation> dataflowComparataor = new DataflowComparator<SyntaxPair<NodeAsRepresentation>, NodeAsRepresentation, OncePerTreeInformation>();
        private ISyntaxComparator<SyntaxPair<NodeAsRepresentation>, NodeAsRepresentation, OncePerTreeInformation> refactorInvocationsComparataor = new RefactorableInvocationsComparator<SyntaxPair<NodeAsRepresentation>, NodeAsRepresentation, OncePerTreeInformation>();
        private ISyntaxComparator<SyntaxPair<NodeAsRepresentation>, NodeAsRepresentation, OncePerTreeInformation> refactorMembersComparataor = new RefactorableMemberAccessComparator<SyntaxPair<NodeAsRepresentation>, NodeAsRepresentation, OncePerTreeInformation>();



        public Executor(bool list)
        {
            this.list = list;
        }

        public void ExecuteForProject(string path, HashSet<string> enabled)
        {
            MSBuildLocator.RegisterDefaults();
            var workspace = MSBuildWorkspace.Create();
            var project = workspace.OpenProjectAsync(path).Result;
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

        private const int minDepth = 30;

        private SimilarityMeasure Config6(Compilation compilation)
        {
            var source = new MethodFragmentsInCompilation(compilation, minDepth);
            var information = new OncePerTreeInformation(compilation);
            var analyzer = new MeasuredSimilarityFinder<SyntaxPair<NodeAsRepresentation>, NodeAsRepresentation, OncePerTreeInformation>(source, identity, new List<ISyntaxComparator<SyntaxPair<NodeAsRepresentation>, NodeAsRepresentation, OncePerTreeInformation>>() { subComparator, compatibleComparator }, information);
            var similarities = analyzer.FindAll().ToList();

            if (list)
                ListSimilarities(similarities);

            return analyzer.Measure;
        }

        private SimilarityMeasure Config5(Compilation compilation)
        {
            var source = new MethodFragmentsInCompilation(compilation, minDepth);
            var information = new OncePerTreeInformation(compilation);
            var analyzer = new MeasuredSimilarityFinder<SyntaxPair<NodeAsRepresentation>, NodeAsRepresentation, OncePerTreeInformation>(source, identity, new List<ISyntaxComparator<SyntaxPair<NodeAsRepresentation>, NodeAsRepresentation, OncePerTreeInformation>>() { subComparator, refactorInvocationsComparataor, semanticComparator }, information);
            var similarities = analyzer.FindAll().ToList();

            if (list)
                ListSimilarities(similarities);

            return analyzer.Measure;
        }

        private SimilarityMeasure Config4(Compilation compilation)
        {
            var source = new MethodFragmentsInCompilation(compilation, minDepth);
            var information = new OncePerTreeInformation(compilation);
            var analyzer = new MeasuredSimilarityFinder<SyntaxPair<NodeAsRepresentation>, NodeAsRepresentation, OncePerTreeInformation>(source, identity, new List<ISyntaxComparator<SyntaxPair<NodeAsRepresentation>, NodeAsRepresentation, OncePerTreeInformation>>() { subComparator, refactorInvocationsComparataor }, information);
            var similarities = analyzer.FindAll().ToList();

            if (list)
                ListSimilarities(similarities);

            return analyzer.Measure;
        }

        private SimilarityMeasure Config3(Compilation compilation)
        {
            var source = new MethodFragmentsInCompilation(compilation, minDepth);
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
            var source = new MethodFragmentsInCompilation(compilation, minDepth);
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
