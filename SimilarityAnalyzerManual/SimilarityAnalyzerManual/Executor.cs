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
using System.Diagnostics;
using System.Linq;

namespace SimilarityAnalyzerManualExecutor
{
    internal class Executor
    {
        private readonly NodeToNode identity = new NodeToNode();
        private readonly NodeToVector withVector = new NodeToVector();

        private readonly ISyntaxComparator<SyntaxLeafPair<NodeAsRepresentation>, NodeAsRepresentation, OncePerTreeInformation> superComparator = new SuperTreeComparator<SyntaxLeafPair<NodeAsRepresentation>, NodeAsRepresentation, OncePerTreeInformation>();
        private readonly ISyntaxComparator<SyntaxPair<NodeAsRepresentation>, NodeAsRepresentation, OncePerTreeInformation> subComparator = new StructuralComparator<SyntaxPair<NodeAsRepresentation>, NodeAsRepresentation, OncePerTreeInformation>();

        private readonly ISyntaxComparator<SyntaxPair<NodeWithVector>, NodeWithVector, OncePerTreeInformation> subComparatorWithVector = new StructuralComparator<SyntaxPair<NodeWithVector>, NodeWithVector, OncePerTreeInformation>();
        private readonly ISyntaxComparator<SyntaxPair<NodeWithVector>, NodeWithVector, OncePerTreeInformation> vectorComparator = new VectorComparator<SyntaxPair<NodeWithVector>, NodeWithVector, OncePerTreeInformation>();

        private readonly ISyntaxComparator<SyntaxPair<NodeWithVector>, NodeWithVector, OncePerTreeInformation> semanticComparator = new SemanticComparator<SyntaxPair<NodeWithVector>, NodeWithVector, OncePerTreeInformation>();
        private readonly ISyntaxComparator<SyntaxPair<NodeWithVector>, NodeWithVector, OncePerTreeInformation> identifierComparator = new IdentifierComparator<SyntaxPair<NodeWithVector>, NodeWithVector, OncePerTreeInformation>();
        private readonly ISyntaxComparator<SyntaxPair<NodeWithVector>, NodeWithVector, OncePerTreeInformation> literalComparator = new LiteralComparator<SyntaxPair<NodeWithVector>, NodeWithVector, OncePerTreeInformation>();

        private readonly ISyntaxComparator<SyntaxPair<NodeWithVector>, NodeWithVector, OncePerTreeInformation> compatibleComparator = new CompatibleComparator<SyntaxPair<NodeWithVector>, NodeWithVector, OncePerTreeInformation>();
        private readonly ISyntaxComparator<SyntaxPair<NodeWithVector>, NodeWithVector, OncePerTreeInformation> dataflowComparator = new DataflowComparator<SyntaxPair<NodeWithVector>, NodeWithVector, OncePerTreeInformation>();
        private readonly ISyntaxComparator<SyntaxPair<NodeWithVector>, NodeWithVector, OncePerTreeInformation> refactorInvocationsComparator = new RefactorableInvocationsComparator<SyntaxPair<NodeWithVector>, NodeWithVector, OncePerTreeInformation>();
        private readonly ISyntaxComparator<SyntaxPair<NodeWithVector>, NodeWithVector, OncePerTreeInformation> refactorMembersComparator = new RefactorableMemberAccessComparator<SyntaxPair<NodeWithVector>, NodeWithVector, OncePerTreeInformation>();

        private Logger logger;

        public Executor(Logger logger)
        {
            this.logger = logger;
        }

        public void ExecuteForProject(string path, int minDepth, HashSet<string> enabled)
        {
            MSBuildLocator.RegisterDefaults();
            var workspace = MSBuildWorkspace.Create();
            var project = workspace.OpenProjectAsync(path).Result;
            Console.WriteLine("Loaded project");
            logger.SetProjectName(project.Name);
            logger.SetMinSize(minDepth);
            ExecuteForProject(project, minDepth, enabled);
            Console.WriteLine("Finished analysing project");
        }

        private void ExecuteForProject(Project project, int minDepth, HashSet<string> enabled)
        {
            var projectName = project.Name;
            var compilation = project.GetCompilationAsync().Result;
            var key = "";

            key = "sub";
            if (enabled == null || enabled.Contains(key))
            {
                logger.SetCurrentKey(key);
                sub(compilation, minDepth, logger);
            }

            key = "sup";
            if (enabled == null || enabled.Contains(key))
            {
                logger.SetCurrentKey(key);
                super(compilation, minDepth, logger);
            }

            key = "sub+vec";
            if (enabled == null || enabled.Contains(key))
            {
                logger.SetCurrentKey(key);
                sub_vec(compilation, minDepth, logger);
            }

            key = "sub+vec+seman";
            if (enabled == null || enabled.Contains(key))
            {
                logger.SetCurrentKey(key);
                sub_vec_seman(compilation, minDepth, logger);
            }

            key = "sub+vec+seman+lit+id";
            if (enabled == null || enabled.Contains(key))
            {
                logger.SetCurrentKey(key);
                sub_vec_seman_lit_id(compilation, minDepth, logger);
            }

            key = "sub+vec+seman+df";
            if (enabled == null || enabled.Contains(key))
            {
                logger.SetCurrentKey(key);
                sub_vec_seman_df(compilation, minDepth, logger);
            }

            key = "sub+vec+seman+df+refactInvoc+refactMember";
            if (enabled == null || enabled.Contains(key))
            {
                logger.SetCurrentKey(key);
                sub_vec_seman_df_refact(compilation, minDepth, logger);
            }

            key = "sub+vec+com+df";
            if (enabled == null || enabled.Contains(key))
            {
                logger.SetCurrentKey(key);
                sub_vec_comp_df(compilation, minDepth, logger);
            }

            key = "sub+vec+com+df+refactInvoc+refactMember";
            if (enabled == null || enabled.Contains(key))
            {
                logger.SetCurrentKey(key);
                sub_vec_comp_df_refact(compilation, minDepth, logger);
            }

        }

        private void sub(Compilation compilation, int minDepth, Logger logger)
        {
            var source = new MethodFragmentsInCompilation(compilation, minDepth);
            var information = new OncePerTreeInformation(compilation);
            var analyzer = new MeasuredSimilarityFinder<SyntaxPair<NodeAsRepresentation>, NodeAsRepresentation, OncePerTreeInformation>(source, identity, Enumerable.Repeat(subComparator, 1).ToList(), information);

            var sv = new Stopwatch();
            sv.Start();
            var similarities = analyzer.FindAll().ToList();
            sv.Stop();

            logger.SetRuntime(sv.ElapsedMilliseconds);
            logger.SetCompletionDate(DateTime.Now);
            logger.LogMeasures(analyzer.Measure);
            logger.LogSimilarities(similarities);
        }

        private void super(Compilation compilation, int minDepth, Logger logger)
        {
            var source = new LeavesInCompilation(compilation);
            var information = new OncePerTreeInformation(compilation);
            var analyzer = new MeasuredSimilarityFinder<SyntaxLeafPair<NodeAsRepresentation>, NodeAsRepresentation, OncePerTreeInformation>(source, identity, Enumerable.Repeat(superComparator, 1).ToList(), information);

            var sv = new Stopwatch();
            sv.Start();
            var similarities = analyzer.FindAll().ToList();
            sv.Stop();

            logger.SetCompletionDate(DateTime.Now);
            logger.SetRuntime(sv.ElapsedMilliseconds);
            logger.LogMeasures(analyzer.Measure);
            logger.LogSimilarities(similarities);
        }

        private void sub_vec(Compilation compilation, int minDepth, Logger logger)
        {
            var source = new MethodFragmentsInCompilation(compilation, minDepth);
            var information = new OncePerTreeInformation(compilation);
            var analyzer = new MeasuredSimilarityFinder<SyntaxPair<NodeWithVector>, NodeWithVector, OncePerTreeInformation>(source, withVector, new List<ISyntaxComparator<SyntaxPair<NodeWithVector>, NodeWithVector, OncePerTreeInformation>>() { vectorComparator, subComparatorWithVector }, information);

            var sv = new Stopwatch();
            sv.Start();
            var similarities = analyzer.FindAll().ToList();
            sv.Stop();

            logger.SetRuntime(sv.ElapsedMilliseconds);
            logger.SetCompletionDate(DateTime.Now);
            logger.LogMeasures(analyzer.Measure);
            logger.LogSimilarities(similarities);
        }

        private void sub_vec_seman(Compilation compilation, int minDepth, Logger logger)
        {
            var source = new MethodFragmentsInCompilation(compilation, minDepth);
            var information = new OncePerTreeInformation(compilation);
            var analyzer = new MeasuredSimilarityFinder<SyntaxPair<NodeWithVector>, NodeWithVector, OncePerTreeInformation>(
                source,
                withVector, new List<ISyntaxComparator<SyntaxPair<NodeWithVector>, NodeWithVector, OncePerTreeInformation>>()
                {
                    vectorComparator,
                    subComparatorWithVector,
                    semanticComparator,
                },
                information);

            var sv = new Stopwatch();
            sv.Start();
            var similarities = analyzer.FindAll().ToList();
            sv.Stop();

            logger.SetRuntime(sv.ElapsedMilliseconds);
            logger.SetCompletionDate(DateTime.Now);
            logger.LogMeasures(analyzer.Measure);
            logger.LogSimilarities(similarities);
        }

        private void sub_vec_seman_lit_id(Compilation compilation, int minDepth, Logger logger)
        {
            var source = new MethodFragmentsInCompilation(compilation, minDepth);
            var information = new OncePerTreeInformation(compilation);
            var analyzer = new MeasuredSimilarityFinder<SyntaxPair<NodeWithVector>, NodeWithVector, OncePerTreeInformation>(
                source,
                withVector, new List<ISyntaxComparator<SyntaxPair<NodeWithVector>, NodeWithVector, OncePerTreeInformation>>()
                {
                    vectorComparator,
                    subComparatorWithVector,
                    semanticComparator,
                    literalComparator,
                    identifierComparator,
                },
                information);

            var sv = new Stopwatch();
            sv.Start();
            var similarities = analyzer.FindAll().ToList();
            sv.Stop();

            logger.SetRuntime(sv.ElapsedMilliseconds);
            logger.SetCompletionDate(DateTime.Now);
            logger.LogMeasures(analyzer.Measure);
            logger.LogSimilarities(similarities);
        }

        private void sub_vec_seman_df(Compilation compilation, int minDepth, Logger logger)
        {
            var source = new MethodFragmentsInCompilation(compilation, minDepth);
            var information = new OncePerTreeInformation(compilation);
            var analyzer = new MeasuredSimilarityFinder<SyntaxPair<NodeWithVector>, NodeWithVector, OncePerTreeInformation>(
                source,
                withVector, new List<ISyntaxComparator<SyntaxPair<NodeWithVector>, NodeWithVector, OncePerTreeInformation>>()
                {
                    vectorComparator,
                    subComparatorWithVector,
                    semanticComparator,
                    dataflowComparator
                },
                information);

            var sv = new Stopwatch();
            sv.Start();
            var similarities = analyzer.FindAll().ToList();
            sv.Stop();

            logger.SetRuntime(sv.ElapsedMilliseconds);
            logger.SetCompletionDate(DateTime.Now);
            logger.LogMeasures(analyzer.Measure);
            logger.LogSimilarities(similarities);
        }

        private void sub_vec_seman_df_refact(Compilation compilation, int minDepth, Logger logger)
        {
            var source = new MethodFragmentsInCompilation(compilation, minDepth);
            var information = new OncePerTreeInformation(compilation);
            var analyzer = new MeasuredSimilarityFinder<SyntaxPair<NodeWithVector>, NodeWithVector, OncePerTreeInformation>(
                source,
                withVector, new List<ISyntaxComparator<SyntaxPair<NodeWithVector>, NodeWithVector, OncePerTreeInformation>>()
                {
                    vectorComparator,
                    subComparatorWithVector,
                    semanticComparator,
                    dataflowComparator,
                    refactorInvocationsComparator,
                    refactorMembersComparator
                },
                information);

            var sv = new Stopwatch();
            sv.Start();
            var similarities = analyzer.FindAll().ToList();
            sv.Stop();

            logger.SetRuntime(sv.ElapsedMilliseconds);
            logger.SetCompletionDate(DateTime.Now);
            logger.LogMeasures(analyzer.Measure);
            logger.LogSimilarities(similarities);
        }

        private void sub_vec_comp_df(Compilation compilation, int minDepth, Logger logger)
        {
            var source = new MethodFragmentsInCompilation(compilation, minDepth);
            var information = new OncePerTreeInformation(compilation);
            var analyzer = new MeasuredSimilarityFinder<SyntaxPair<NodeWithVector>, NodeWithVector, OncePerTreeInformation>(
                source,
                withVector, new List<ISyntaxComparator<SyntaxPair<NodeWithVector>, NodeWithVector, OncePerTreeInformation>>()
                {
                    vectorComparator,
                    subComparatorWithVector,
                    compatibleComparator,
                    dataflowComparator
                },
                information);

            var sv = new Stopwatch();
            sv.Start();
            var similarities = analyzer.FindAll().ToList();
            sv.Stop();

            logger.SetRuntime(sv.ElapsedMilliseconds);
            logger.SetCompletionDate(DateTime.Now);
            logger.LogMeasures(analyzer.Measure);
            logger.LogSimilarities(similarities);
        }

        private void sub_vec_comp_df_refact(Compilation compilation, int minDepth, Logger logger)
        {
            var source = new MethodFragmentsInCompilation(compilation, minDepth);
            var information = new OncePerTreeInformation(compilation);
            var analyzer = new MeasuredSimilarityFinder<SyntaxPair<NodeWithVector>, NodeWithVector, OncePerTreeInformation>(
                source,
                withVector, new List<ISyntaxComparator<SyntaxPair<NodeWithVector>, NodeWithVector, OncePerTreeInformation>>()
                {
                    vectorComparator,
                    subComparatorWithVector,
                    compatibleComparator,
                    dataflowComparator,
                    refactorInvocationsComparator,
                    refactorMembersComparator
                },
                information);

            var sv = new Stopwatch();
            sv.Start();
            var similarities = analyzer.FindAll().ToList();
            sv.Stop();

            logger.SetRuntime(sv.ElapsedMilliseconds);
            logger.SetCompletionDate(DateTime.Now);
            logger.LogMeasures(analyzer.Measure);
            logger.LogSimilarities(similarities);
        }
    }
}


