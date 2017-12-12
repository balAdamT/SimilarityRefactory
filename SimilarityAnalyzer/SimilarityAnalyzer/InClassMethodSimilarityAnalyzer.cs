using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using SimilarityTreeExplorer.SubTree;
using SimilarityTreeExplorer.SuperTree;
using SyntaxComparision.Algorithm;
using SyntaxComparision.Data;
using SyntaxComparision.Interfaces;
using SyntaxVectors;
using SyntaxVectors.Masking;

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
      var @class = context.Node as ClassDeclarationSyntax;

      var source = new LeavesInClass(@class);
      var pre = new NodeToNode();
      var comparator = (ISyntaxComparator<SyntaxLeafPair<NodeAsRepresentation>, NodeAsRepresentation>)new SuperTreeComparator<SyntaxLeafPair<NodeAsRepresentation>, NodeAsRepresentation>();

      var analyzer = new SimilarityFinder<SyntaxLeafPair<NodeAsRepresentation>, NodeAsRepresentation>(source, pre, Enumerable.Repeat(comparator, 1).ToList());
      var similarities = analyzer.FindAll();
    }

    private void AnalyzeClassSubTrees(SyntaxNodeAnalysisContext context)
    {
      var @class = context.Node as ClassDeclarationSyntax;

      var source = new MethodFragmentsInClass(@class);
      var pre = new NodeToNode();
      var comparator = (ISyntaxComparator<SyntaxPair<NodeAsRepresentation>, NodeAsRepresentation>) new SemanticComparator<SyntaxPair<NodeAsRepresentation>, NodeAsRepresentation>();


      var analyzer = new SimilarityFinder<SyntaxPair<NodeAsRepresentation>,NodeAsRepresentation>(source, pre,  Enumerable.Repeat(comparator, 1).ToList());
      var similarities = analyzer.FindAll();

      var similarity = similarities.OrderByDescending(s => s.Left.Node.DescendantNodes().Count()).Take(1).Single();

      context.ReportDiagnostic(Diagnostic.Create(Rule, similarity.Left.Node.GetLocation(), similarity.Right.Node.GetLocation().GetLineSpan()));
      context.ReportDiagnostic(Diagnostic.Create(Rule, similarity.Right.Node.GetLocation(), similarity.Left.Node.GetLocation().GetLineSpan()));
    }

    private void AnalyzeClassSubTreesWithDataflow(SyntaxNodeAnalysisContext context)
    {
      var @class = context.Node as ClassDeclarationSyntax;

      var source = new MethodFragmentsInClass(@class);
      var pre = new NodeToNode();
      var comparator = (ISyntaxComparator<SyntaxPair<NodeAsRepresentation>, NodeAsRepresentation>)new SemanticComparator<SyntaxPair<NodeAsRepresentation>, NodeAsRepresentation>();
      var dfComparator = (ISyntaxComparator<SyntaxPair<NodeAsRepresentation>, NodeAsRepresentation>)new DataflowComparator<SyntaxPair<NodeAsRepresentation>, NodeAsRepresentation>();


      var analyzer = new SimilarityFinder<SyntaxPair<NodeAsRepresentation>, NodeAsRepresentation>(source, pre, new List<ISyntaxComparator<SyntaxPair<NodeAsRepresentation>, NodeAsRepresentation>>() {comparator, dfComparator});
      var similarities = analyzer.FindAll();

      similarities = similarities.OrderByDescending(s => s.Left.Node.DescendantNodes().Count());
      var similarity = similarities.First();

      context.ReportDiagnostic(Diagnostic.Create(Rule, similarity.Left.Node.GetLocation(), similarity.Right.Node.GetLocation().GetLineSpan()));
      context.ReportDiagnostic(Diagnostic.Create(Rule, similarity.Right.Node.GetLocation(), similarity.Left.Node.GetLocation().GetLineSpan()));
    }

    private void AnalyzeClassVectors(SyntaxNodeAnalysisContext context)
    {
      var @class = context.Node as ClassDeclarationSyntax;

      var source = new MethodFragmentsInClass(@class);
      var pre = new NodeToVector(SyntaxMasks.AllNodes);
      var comparator = (ISyntaxComparator<SyntaxPair<NodeWithVector>, NodeWithVector>) new VectorComparator<SyntaxPair<NodeWithVector>, NodeWithVector>();

      var analyzer = new SimilarityFinder<SyntaxPair<NodeWithVector>, NodeWithVector> (source, pre, Enumerable.Repeat(comparator, 1).ToList());
      var similarities = analyzer.FindAll();
    }
  }
}