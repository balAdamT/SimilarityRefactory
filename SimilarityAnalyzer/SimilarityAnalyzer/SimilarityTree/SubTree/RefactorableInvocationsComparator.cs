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
            var leftModel = information.Provide(pair.Left.Node.SyntaxTree);
            var rightModel = information.Provide(pair.Right.Node.SyntaxTree);

            return Equals(pair.Left.Node, pair.Right.Node, leftModel, rightModel);
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


                return MethodNameIsSame(leftIdentifier, rightIdentifier, leftModel, rightModel);
            }

            if (right.Kind() == SyntaxKind.IdentifierName)
                return false;

            return ChildrenEquals(left.ChildNodes(), right.ChildNodes(), leftModel, rightModel);
        }

        private bool MethodNameIsSame(IdentifierNameSyntax left, IdentifierNameSyntax right, SemanticModel leftModel, SemanticModel rightModel)
        {
            var leftSymbol = leftModel.GetSymbolInfo(left).Symbol;
            var rightSymbol = rightModel.GetSymbolInfo(right).Symbol;

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
