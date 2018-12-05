using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using SimilarityAnalyzer.SimilarityTree.SubTree;
using SimilarityAnalyzer.SyntaxComparision.Algorithm;
using SimilarityAnalyzer.SyntaxComparision.Data;
using SimilarityAnalyzer.SyntaxComparision.Helpers;
using SimilarityAnalyzer.SyntaxComparision.Information;
using SimilarityAnalyzer.SyntaxComparision.Interfaces;
using SimilarityAnalyzer.SyntaxVectors;
using SyntaxComparision.Algorithm;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace SimilarityAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class Stage2CodeDuplicatesAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "DUPS00002";
        internal static readonly LocalizableString Title = "Code may be unnecessary duplicate. (Stage 2)";
        internal static readonly LocalizableString MessageFormat = "This code can be found on '{0}'";
        internal const string Category = "DuplicatedCode";

        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(FindDuplicates, SyntaxKind.ClassDeclaration);
        }

        private void FindDuplicates(SyntaxNodeAnalysisContext context)
        {
            var @class = context.Node as ClassDeclarationSyntax;

            var source = new MethodFragmentsInClass(@class, 20);
            var pre = new NodeToVector();
            ISyntaxComparator<SyntaxPair<NodeWithVector>, NodeWithVector, SingleTreeInformation> vectorComparator = new VectorComparator<SyntaxPair<NodeWithVector>, NodeWithVector, SingleTreeInformation>();
            ISyntaxComparator<SyntaxPair<NodeWithVector>, NodeWithVector, SingleTreeInformation> comparator = new StructuralComparator<SyntaxPair<NodeWithVector>, NodeWithVector, SingleTreeInformation>();
            ISyntaxComparator<SyntaxPair<NodeWithVector>, NodeWithVector, SingleTreeInformation> semanticComparator = new SemanticComparator<SyntaxPair<NodeWithVector>, NodeWithVector, SingleTreeInformation>();
            ISyntaxComparator<SyntaxPair<NodeWithVector>, NodeWithVector, SingleTreeInformation> literalComparator = new LiteralComparator<SyntaxPair<NodeWithVector>, NodeWithVector, SingleTreeInformation>();
            ISyntaxComparator<SyntaxPair<NodeWithVector>, NodeWithVector, SingleTreeInformation> idComparator = new IdentifierComparator<SyntaxPair<NodeWithVector>, NodeWithVector, SingleTreeInformation>();
            var information = new SingleTreeInformation(context.SemanticModel);

            var analyzer = new SimilarityFinder<SyntaxPair<NodeWithVector>, NodeWithVector, SingleTreeInformation>(
                source,
                pre,
                new List<ISyntaxComparator<SyntaxPair<NodeWithVector>, NodeWithVector, SingleTreeInformation>>()
                {
                    vectorComparator,
                    comparator,
                    semanticComparator,
                    literalComparator,
                    idComparator
                },
                information);
            var similarities = analyzer.FindAll();

            similarities = similarities.OrderByDescending(s => s.Left.Node.DescendantNodes().Count());

            var reportedSimilarities = new List<SyntaxNode>();
            foreach (var similarity in similarities)
            {
                if (!reportedSimilarities.Any(node => node.Contains(similarity.Left.Node)))
                {
                    reportedSimilarities.Add(similarity.Left.Node);
                    Report(context, similarity.Left.Node, similarity.Right.Node);
                }
            }
        }

        private void Report(SyntaxNodeAnalysisContext context, SyntaxNode left, SyntaxNode right)
        {
            context.ReportDiagnostic(Diagnostic.Create(Rule, left.GetLocation(), new[] { right.GetLocation() }, right.GetLocation().ToVisualStudioString()));
        }
    }
}
