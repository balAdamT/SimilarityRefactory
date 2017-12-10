using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using SimilarityAnalyzer.Logic;
using SimilarityAnalyzer.Data;
using SimilarityAnalyzer.SimilarityFinders;
using SimilarityTreeExplorer.SubTree;
using SyntaxComparision.Algorithm;
using SyntaxComparision.Data;
using SyntaxComparision.Interfaces;

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
      //context.RegisterSyntaxNodeAction(AnalyzeClass, SyntaxKind.ClassDeclaration);
      context.RegisterSyntaxNodeAction(AnalyzeClassSubTrees, SyntaxKind.ClassDeclaration);
    }

    private void AnalyzeClassSuperTrees(SyntaxNodeAnalysisContext context)
    {
      var @class = context.Node as ClassDeclarationSyntax;

      var finder = new CommonSuperTreeFinder(@class);
      var similarities = finder.Similarities;
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
  }
}