using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimilarityAnalyzer.SyntaxComparision.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace SimilarityAnalyzer.SimilarityTree.SubTree
{
    public class IdentifierComparator<TPair, TRepresentation, TInformation> : ISyntaxComparator<TPair, TRepresentation, TInformation>
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
            if (left.Kind() == SyntaxKind.IdentifierName)
            {
                if (left.Kind() != right.Kind())
                    return false;

                var leftIdentifier = left as IdentifierNameSyntax;
                var rightIdentifier = right as IdentifierNameSyntax;


                return IdentifierIsTheSame(leftIdentifier, rightIdentifier);
            }

            if (right.Kind() == SyntaxKind.IdentifierName)
                return false;

            return ChildrenEquals(left.ChildNodes(), right.ChildNodes());
        }

        private bool IdentifierIsTheSame(IdentifierNameSyntax left, IdentifierNameSyntax right)
        {
            return left.Identifier.Text == right.Identifier.Text;
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