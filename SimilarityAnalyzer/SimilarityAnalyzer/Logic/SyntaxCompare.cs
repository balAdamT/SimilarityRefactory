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
                return FinalizeAnswer(acc);


            //If they are not equal, stop comparision
            if (!Compare(left, right))
                return FinalizeAnswer(acc);

            //If it is the same node, it's not a valid similarity
            if (left.SpanStart == right.SpanStart)
                return FinalizeAnswer(acc);

            acc.Add(new NodePair(left, right));
            return FindCommonSuperTree(left.Parent, right.Parent, ref acc);
        }

        private static IEnumerable<NodePair> FinalizeAnswer(List<NodePair> acc)
        {
            while(acc.Count > 0)
            {
                var last = acc[acc.Count - 1];
                if (last.Right.IsEquivalentTo(last.Left))
                    return acc;
                else
                    acc.RemoveAt(acc.Count - 1);
            }

            return acc;
        }

        private static bool Compare(SyntaxNode left, SyntaxNode right)
        {
            return left.IsEquivalentTo(right);
        }
    }
}
