using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimilarityAnalyzer.SyntaxComparision.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace SimilarityAnalyzer.SimilarityTree.SubTree
{
    public class RefactorableInvocationsComparator<TPair, TRepresentation, TInformation> : ISyntaxComparator<TPair, TRepresentation, TInformation>
      where TRepresentation : ISyntaxRepresentation
      where TPair : ISyntaxPair<TRepresentation>
      where TInformation : ISyntaxInformation
    {
        public bool SyntaxEquals(TPair pair, TInformation information)
        {
            SemanticModel leftModel = information.Provide(pair.Left.Node.SyntaxTree);
            SemanticModel rightModel = information.Provide(pair.Right.Node.SyntaxTree);

            return Equals(pair.Left.Node, pair.Right.Node, leftModel, rightModel);
        }

        private bool Equals(
            SyntaxNode left,
            SyntaxNode right,
            SemanticModel leftModel,
            SemanticModel rightModel)
        {
            if (left.Kind() == SyntaxKind.ElementAccessExpression 
                || left.Kind() == SyntaxKind.SimpleMemberAccessExpression 
                || left.Kind() == SyntaxKind.ConditionalAccessExpression 
                || left.Kind() == SyntaxKind.InvocationExpression)
            {
                if (left.Kind() != right.Kind())
                    return false;
                else return AccessIsRefactorable(left, right, leftModel, rightModel);
            }

            if (left.Kind() != SyntaxKind.IdentifierName || left.Kind() != SyntaxKind.ElementAccessExpression)
                return ChildrenEquals(left.ChildNodes(), right.ChildNodes(), leftModel, rightModel);

            if (right.Kind() != SyntaxKind.IdentifierName)
                return false;

            if (MethodNameIsSame((IdentifierNameSyntax)left, (IdentifierNameSyntax)right, leftModel, rightModel))
                return ChildrenEquals(left.ChildNodes(), right.ChildNodes(), leftModel, rightModel);

            return false;
        }

        private bool AccessIsRefactorable(SyntaxNode left, SyntaxNode right, SemanticModel leftModel, SemanticModel rightModel)
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
            return node.DescendantNodes().OfType<IdentifierNameSyntax>().Where(n => n.FirstAncestorOrSelf<ArgumentListSyntax>() == null);
        }
      
        private bool MethodNameIsSame(IdentifierNameSyntax left, IdentifierNameSyntax right, SemanticModel leftModel, SemanticModel rightModel)
        {
            ISymbol leftSymbol = leftModel.GetSymbolInfo(left).Symbol;
            ISymbol rightSymbol = rightModel.GetSymbolInfo(right).Symbol;

            if (leftSymbol is IMethodSymbol)
            {
                if (rightSymbol is IMethodSymbol)
                    return leftSymbol == rightSymbol;
                else
                    return false;
            }

            // Do not filter nonMethods
            return true;
        }

        private bool ChildrenEquals(IEnumerable<SyntaxNode> leftChildren, IEnumerable<SyntaxNode> rightChildren, SemanticModel leftModel, SemanticModel rightModel)
        {
            var parallelEnumerator = leftChildren.Zip(rightChildren, (x, y) => new { Left = x, Right = y });

            if (parallelEnumerator.Any(pair => !Equals(pair.Left, pair.Right, leftModel, rightModel)))
                return false;

            //No inequalities, check if children are balanced
            return leftChildren.Count() == rightChildren.Count();
        }
    }
}
