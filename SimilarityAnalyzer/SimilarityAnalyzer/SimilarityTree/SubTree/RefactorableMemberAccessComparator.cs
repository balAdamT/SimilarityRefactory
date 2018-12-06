using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimilarityAnalyzer.SyntaxComparision.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace SimilarityAnalyzer.SimilarityTree.SubTree
{
    public class RefactorableMemberAccessComparator<TPair, TRepresentation, TInformation> : ISyntaxComparator<TPair, TRepresentation, TInformation>
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
            SyntaxNode right)
        {
            if (left.Kind() == SyntaxKind.ElementAccessExpression
                || left.Kind() == SyntaxKind.SimpleMemberAccessExpression
                || left.Kind() == SyntaxKind.ConditionalAccessExpression
                || left.Kind() == SyntaxKind.InvocationExpression)
            {
                if (left.Kind() != right.Kind())
                    return false;
                else return AccessIsRefactorable(left, right);
            }
            else
                return ChildrenEquals(left.ChildNodes(), right.ChildNodes());
        }

        private bool AccessIsRefactorable(SyntaxNode left, SyntaxNode right)
        {
            var leftIdentifiers = GetMemberIdentifiers(left);
            var rightIdentifiers = GetMemberIdentifiers(right);

            var identifierPairs = leftIdentifiers.Zip(rightIdentifiers, (x, y) => new { Left = x, Right = y });

            // First identifier can be unified
            foreach (var pair in identifierPairs.Skip(1))
            {
                if (pair.Left.Identifier.Value != pair.Left.Identifier.Value)
                    return false;
            }

            return true;
        }

        private IEnumerable<IdentifierNameSyntax> GetMemberIdentifiers(SyntaxNode node)
        {
            return node.DescendantNodes().OfType<IdentifierNameSyntax>()
                .Where(n => n.FirstAncestorOrSelf<ArgumentListSyntax>() == null)
                .Where(n => n.FirstAncestorOrSelf<BracketedArgumentListSyntax>() == null);
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
