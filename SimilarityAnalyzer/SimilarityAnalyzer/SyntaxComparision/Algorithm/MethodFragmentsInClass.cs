using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimilarityAnalyzer.SyntaxComparision.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace SimilarityAnalyzer.SyntaxComparision.Algorithm
{
    public class MethodFragmentsInClass : ISyntaxSource
    {
        private readonly ClassDeclarationSyntax @class;

        public MethodFragmentsInClass(ClassDeclarationSyntax @class)
        {
            this.@class = @class;
        }

        public IEnumerable<SyntaxNode> Fetch()
        {
            foreach (MethodDeclarationSyntax method
              in @class.Members.Where(member => member.Kind() == SyntaxKind.MethodDeclaration))
                foreach (SyntaxNode node in method.DescendantNodes().Where(node => node.DescendantNodes().Any()))
                    yield return node;
        }
    }
}
