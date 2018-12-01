using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using SimilarityAnalyzer.SimilarityTree.SubTree;
using SimilarityAnalyzer.SimilarityTree.SuperTree;
using SimilarityAnalyzer.SimilarityTree.SuperTree.Algorithm;
using SimilarityAnalyzer.SyntaxComparision.Algorithm;
using SimilarityAnalyzer.SyntaxComparision.Data;
using SimilarityAnalyzer.SyntaxComparision.Information;
using SimilarityAnalyzer.SyntaxComparision.Interfaces;
using SimilarityAnalyzer.SyntaxVectors;
using SimilarityAnalyzer.SyntaxVectors.Masking;
using SyntaxComparision.Algorithm;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace SimilarityAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class InClassMethodSimilarityAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "InClassSimilarity";
        internal static readonly LocalizableString Title = "Code may be unnecessary duplicate.";
        internal static readonly LocalizableString MessageFormat = "This code can be found on '{0}'";
        internal const string Category = "DuplicatedCode";

        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            //context.RegisterSyntaxNodeAction(AnalyzeClassSuperTrees, SyntaxKind.ClassDeclaration);
            //context.RegisterSyntaxNodeAction(AnalyzeClassSubTrees, SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeClassSubTreesWithDataflow, SyntaxKind.ClassDeclaration);
            //context.RegisterSyntaxNodeAction(AnalyzeClassVectors, SyntaxKind.ClassDeclaration);
        }

        private void AnalyzeClassSuperTrees(SyntaxNodeAnalysisContext context)
        {
            ClassDeclarationSyntax @class = context.Node as ClassDeclarationSyntax;

            LeavesInClass source = new LeavesInClass(@class);
            NodeToNode pre = new NodeToNode();
            SingleTreeInformation information = new SingleTreeInformation(context.SemanticModel);
            ISyntaxComparator<SyntaxLeafPair<NodeAsRepresentation>, NodeAsRepresentation, SingleTreeInformation> comparator = new SuperTreeComparator<SyntaxLeafPair<NodeAsRepresentation>, NodeAsRepresentation, SingleTreeInformation>();

            SimilarityFinder<SyntaxLeafPair<NodeAsRepresentation>, NodeAsRepresentation, SingleTreeInformation> analyzer = new SimilarityFinder<SyntaxLeafPair<NodeAsRepresentation>, NodeAsRepresentation, SingleTreeInformation>(source, pre, Enumerable.Repeat(comparator, 1).ToList(), information);
            IEnumerable<SyntaxLeafPair<NodeAsRepresentation>> similarities = analyzer.FindAll();
        }

        private void AnalyzeClassSubTrees(SyntaxNodeAnalysisContext context)
        {
            ClassDeclarationSyntax @class = context.Node as ClassDeclarationSyntax;

            MethodFragmentsInClass source = new MethodFragmentsInClass(@class);
            NodeToNode pre = new NodeToNode();
            ISyntaxComparator<SyntaxPair<NodeAsRepresentation>, NodeAsRepresentation, SingleTreeInformation> comparator = new StructuralComparator<SyntaxPair<NodeAsRepresentation>, NodeAsRepresentation, SingleTreeInformation>();
            SingleTreeInformation information = new SingleTreeInformation(context.SemanticModel);

            SimilarityFinder<SyntaxPair<NodeAsRepresentation>, NodeAsRepresentation, SingleTreeInformation> analyzer = new SimilarityFinder<SyntaxPair<NodeAsRepresentation>, NodeAsRepresentation, SingleTreeInformation>(source, pre, Enumerable.Repeat(comparator, 1).ToList(), information);
            IEnumerable<SyntaxPair<NodeAsRepresentation>> similarities = analyzer.FindAll();

            SyntaxPair<NodeAsRepresentation> similarity = similarities.OrderByDescending(s => s.Left.Node.DescendantNodes().Count()).Take(1).Single();


            Report(context, similarity.Left.Node, similarity.Right.Node);
        }

        private void AnalyzeClassSubTreesWithDataflow(SyntaxNodeAnalysisContext context)
        {
            ClassDeclarationSyntax @class = context.Node as ClassDeclarationSyntax;

            MethodFragmentsInClass source = new MethodFragmentsInClass(@class);
            NodeToNode pre = new NodeToNode();
            ISyntaxComparator<SyntaxPair<NodeAsRepresentation>, NodeAsRepresentation, SingleTreeInformation> comparator = new StructuralComparator<SyntaxPair<NodeAsRepresentation>, NodeAsRepresentation, SingleTreeInformation>();
            ISyntaxComparator<SyntaxPair<NodeAsRepresentation>, NodeAsRepresentation, SingleTreeInformation> dfComparator = new DataflowComparator<SyntaxPair<NodeAsRepresentation>, NodeAsRepresentation, SingleTreeInformation>();
            ISyntaxComparator<SyntaxPair<NodeAsRepresentation>, NodeAsRepresentation, SingleTreeInformation> semanticComparator = new CompatibleComparator<SyntaxPair<NodeAsRepresentation>, NodeAsRepresentation, SingleTreeInformation>();
            ISyntaxComparator<SyntaxPair<NodeAsRepresentation>, NodeAsRepresentation, SingleTreeInformation> refactorComparator = new RefactorableMemberAccessComparator<SyntaxPair<NodeAsRepresentation>, NodeAsRepresentation, SingleTreeInformation>();
            SingleTreeInformation information = new SingleTreeInformation(context.SemanticModel);

            SimilarityFinder<SyntaxPair<NodeAsRepresentation>, NodeAsRepresentation, SingleTreeInformation> analyzer = new SimilarityFinder<SyntaxPair<NodeAsRepresentation>, NodeAsRepresentation, SingleTreeInformation>(source, pre, new List<ISyntaxComparator<SyntaxPair<NodeAsRepresentation>, NodeAsRepresentation, SingleTreeInformation>>() { comparator, dfComparator, semanticComparator, refactorComparator }, information);
            IEnumerable<SyntaxPair<NodeAsRepresentation>> similarities = analyzer.FindAll();

            similarities = similarities.OrderByDescending(s => s.Left.Node.DescendantNodes().Count());
            SyntaxPair<NodeAsRepresentation> similarity = similarities.First();

            Report(context, similarity.Left.Node, similarity.Right.Node);
        }

        private void AnalyzeClassVectors(SyntaxNodeAnalysisContext context)
        {
            ClassDeclarationSyntax @class = context.Node as ClassDeclarationSyntax;

            MethodFragmentsInClass source = new MethodFragmentsInClass(@class);
            NodeToVector pre = new NodeToVector(SyntaxMasks.AllNodes);
            ISyntaxComparator<SyntaxPair<NodeWithVector>, NodeWithVector, SingleTreeInformation> comparator = new VectorComparator<SyntaxPair<NodeWithVector>, NodeWithVector, SingleTreeInformation>();
            SingleTreeInformation information = new SingleTreeInformation(context.SemanticModel);

            SimilarityFinder<SyntaxPair<NodeWithVector>, NodeWithVector, SingleTreeInformation> analyzer = new SimilarityFinder<SyntaxPair<NodeWithVector>, NodeWithVector, SingleTreeInformation>(source, pre, Enumerable.Repeat(comparator, 1).ToList(), information);
            IEnumerable<SyntaxPair<NodeWithVector>> similarities = analyzer.FindAll();
        }

        private void Report(SyntaxNodeAnalysisContext context, SyntaxNode left, SyntaxNode right)
        {
            context.ReportDiagnostic(Diagnostic.Create(Rule, left.GetLocation(), new[] { right.GetLocation() }, GetVisualStudioInfo(right.GetLocation())));
        }

        private string GetVisualStudioInfo(Location location)
        {
            string result = "";
            FileLinePositionSpan pos = location.GetLineSpan();
            if (pos.Path != null)
            {
                // user-visible line and column counts are 1-based, but internally are 0-based.
                result += "(" 
                    + pos.Path 
                    + "@" 
                    + (pos.StartLinePosition.Line + 1) + ":" + (pos.StartLinePosition.Character + 1) 
                    + "-"
                    + (pos.EndLinePosition.Line + 1) + ":" + (pos.EndLinePosition.Character + 1) 
                    + ")";
            }

            return result;
        }
    }
}