using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimilarityAnalyzer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimilarityAnalyzer.Logic
{
    //TODO consider using F# code here
    static class SyntaxCompare
    {
        public static IEnumerable<NodePair> FindCommonSuperTree(SyntaxNode left, SyntaxNode right)
        {
            List<NodePair> commonPath = new List<NodePair>();
            return FindCommonSuperTree(left, right, ref commonPath);
        }

        private static IEnumerable<NodePair> FindCommonSuperTree(SyntaxNode left, SyntaxNode right, ref List<NodePair> acc)
        {
            //TODO there should be not just methods here
            if (left is MethodDeclarationSyntax || right is MethodDeclarationSyntax)
                return acc;


            if (Compare(left, right))
            {
                acc.Add(new NodePair(left, right));
                return FindCommonSuperTree(left.Parent, right.Parent, ref acc);
            }
            else
                return acc;

        }

        private static bool Compare(SyntaxNode left, SyntaxNode right)
        {
            return left.IsEquivalentTo(right);
        }
    }
}
