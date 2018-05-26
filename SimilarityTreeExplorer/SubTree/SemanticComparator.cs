using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SyntaxComparision.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimilarityTreeExplorer.SubTree
{
    public class SemanticComparator<TPair, TRepresentation, TInformation> : ISyntaxComparator<TPair, TRepresentation, TInformation>
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

        private bool Equals(SyntaxNode left, SyntaxNode right, SemanticModel leftModel, SemanticModel rightModel)
        {
            if (left.Kind() != SyntaxKind.IdentifierName)
                return ChildrenEquals(left.ChildNodes(), right.ChildNodes(), leftModel, rightModel);

            if (right.Kind() != SyntaxKind.IdentifierName)
                return false;

            if (TypeEquals((IdentifierNameSyntax)left, (IdentifierNameSyntax)right, leftModel, rightModel))
                return ChildrenEquals(left.ChildNodes(), right.ChildNodes(), leftModel, rightModel);

            return false;

        }

        private bool TypeEquals(IdentifierNameSyntax left, IdentifierNameSyntax right, SemanticModel leftModel, SemanticModel rightModel)
        {
            var leftType = leftModel.GetTypeInfo(left);
            var rightType = rightModel.GetTypeInfo(right);

            return leftType.Type == rightType.Type;
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
