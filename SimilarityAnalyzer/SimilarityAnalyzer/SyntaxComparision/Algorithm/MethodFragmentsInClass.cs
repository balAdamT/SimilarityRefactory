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
        private readonly int minDepth;

        public MethodFragmentsInClass(ClassDeclarationSyntax @class, int minDepth = 0)
        {
            this.@class = @class;
            this.minDepth = minDepth;
        }

        public IEnumerable<SyntaxNode> Fetch() 
        {
             foreach (MethodDeclarationSyntax method
              in @class.Members.Where(member => member.Kind() == SyntaxKind.MethodDeclaration))
                foreach (var node in method.DescendantNodes().Where(node => node.DescendantNodes().Count() > minDepth))
                    yield return node;
        }
    }
}
