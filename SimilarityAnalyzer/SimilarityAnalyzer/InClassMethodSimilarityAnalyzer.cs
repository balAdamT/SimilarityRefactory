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
    public const string DiagnosticId = "InClassMethodSimilarityAnalyzer";
    internal static readonly LocalizableString Title = "InClassMethodSimilarityAnalyzer Title";
    internal static readonly LocalizableString MessageFormat = "InClassMethodSimilarityAnalyzer '{0}'";
    internal const string Category = "InClassMethodSimilarityAnalyzer Category";

    internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

    public override void Initialize(AnalysisContext context)
    {
      context.RegisterSyntaxNodeAction(AnalyzeClassSuperTrees, SyntaxKind.ClassDeclaration);
      //context.RegisterSyntaxNodeAction(AnalyzeClassSubTrees, SyntaxKind.ClassDeclaration);
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