using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimilarityAnalyzer.SyntaxComparision.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace SimilarityAnalyzer.SimilarityTree.SuperTree.Algorithm
{
    public class LeavesInClass : ISyntaxSource
    {
        private readonly ClassDeclarationSyntax @class;

        public LeavesInClass(ClassDeclarationSyntax @class)
        {
            this.@class = @class;
        }

        public IEnumerable<SyntaxNode> Fetch()
        {
            return @class.Members.Where(member => member.Kind() == SyntaxKind.MethodDeclaration).
              SelectMany(method => method.DescendantNodes().
              Where(node => node.ChildNodes().Any() == false));
        }
    }
}
