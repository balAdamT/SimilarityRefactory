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
            context.RegisterSyntaxNodeAction(AnalyzeClass, SyntaxKind.ClassDeclaration);
        }

        private void AnalyzeClass(SyntaxNodeAnalysisContext context)
        {
            var @class = context.Node as ClassDeclarationSyntax;

            var classMap = new MethodFragmentMap();

            foreach(MethodDeclarationSyntax method 
                in @class.Members.Where(member => member.Kind() == SyntaxKind.MethodDeclaration))
            {
                FragmentExplorer.ForEachFragment(method, fragment => classMap.AddFragmentOfMethod(method, fragment));
            }

            var pairs = classMap.GetOuterPairs();
        }
    }
}