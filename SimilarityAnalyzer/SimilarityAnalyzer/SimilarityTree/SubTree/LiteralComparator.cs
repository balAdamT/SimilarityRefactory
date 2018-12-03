using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimilarityAnalyzer.SyntaxComparision.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;


namespace SimilarityAnalyzer.SimilarityTree.SubTree
{
    public class LiteralComparator<TPair, TRepresentation, TInformation> : ISyntaxComparator<TPair, TRepresentation, TInformation>
      where TRepresentation : ISyntaxRepresentation
      where TPair : ISyntaxPair<TRepresentation>
      where TInformation : ISyntaxInformation
    {
        public bool SyntaxEquals(TPair pair, TInformation information)
        {
            return Equals(pair.Left.Node, pair.Right.Node);
        }

        private bool Equals(
            SyntaxNode left,
            SyntaxNode right,
            SemanticModel leftModel,
            SemanticModel rightModel)
        {
            if (left.Kind() != right.Kind())
                    return false;

            if (left.Kind() == SyntaxKind.NumericLiteralExpression 
                || left.Kind() == SyntaxKind.StringLiteralExpression
                || left.Kind() == SyntaxKind.CharacterLiteralExpression)
            {
                var leftLiteral = left as LiteralExpressionSyntax;
                var rightLiteral = right as LiteralExpressionSyntax;

                return LiteralIsTheSame(leftLiteral, rightLiteral);
            }

            return ChildrenEquals(left.ChildNodes(), right.ChildNodes());
        }

        private bool LiteralIsTheSame(LiteralExpressionSyntax leftLiteral, LiteralExpressionSyntax rightLiteral)
        {
            return leftLiteral.Token.Text == rightLiteral.Token.Text;
        }

        private bool ChildrenEquals(IEnumerable<SyntaxNode> leftChildren, IEnumerable<SyntaxNode> rightChildren)
        {
            var parallelEnumerator = leftChildren.Zip(rightChildren, (x, y) => new { Left = x, Right = y });

            if (parallelEnumerator.Any(pair => !Equals(pair.Left, pair.Right)))
                return false;

            //No inequalities, check if children are balanced
            return leftChildren.Count() == rightChildren.Count();
        }
    }
}
