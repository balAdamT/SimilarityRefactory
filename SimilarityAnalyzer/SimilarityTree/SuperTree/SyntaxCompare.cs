using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SimilarityAnalyzer.SimilarityTree.SuperTree
{
    internal static class SyntaxCompare
    {
        public static int FindCommonSuperTree(SyntaxNode left, SyntaxNode right, int i = 0)
        {
            while (true)
            {
                //Do not look for equivalence beyond method scope
                if (left is MethodDeclarationSyntax || right is MethodDeclarationSyntax)
                    return i;

                //If they run into the same block, that is not a similarity
                if (left.SpanStart == right.SpanStart)
                    return i;

                //If parent is no longer similar
                if (!Compare(left, right))
                    return i;

                left = left.Parent;
                right = right.Parent;
                i++;
            }
        }

        private static bool Compare(SyntaxNode left, SyntaxNode right)
        {
            return left.IsEquivalentTo(right);
        }
    }
}
