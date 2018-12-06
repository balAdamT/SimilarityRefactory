using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimilarityAnalyzer.SyntaxComparision.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace SimilarityAnalyzer.SimilarityTree.SubTree
{
    public class CompatibleComparator<TPair, TRepresentation, TInformation> : ISyntaxComparator<TPair, TRepresentation, TInformation>
      where TRepresentation : ISyntaxRepresentation
      where TPair : ISyntaxPair<TRepresentation>
      where TInformation : ISyntaxInformation
    {
        public bool SyntaxEquals(TPair pair, TInformation information)
        {
            var leftModel = information.Provide(pair.Left.Node.SyntaxTree);
            var rightModel = information.Provide(pair.Right.Node.SyntaxTree);

            return Compatible(pair.Left.Node, pair.Right.Node, leftModel, rightModel, new Dictionary<string, ITypeSymbol>());
        }

        private bool Compatible(SyntaxNode left, SyntaxNode right, SemanticModel leftModel, SemanticModel rightModel, Dictionary<string, ITypeSymbol> compatibility)
        {
            if (left.Kind() != SyntaxKind.IdentifierName)
                return ChildrenCompatible(left.ChildNodes(), right.ChildNodes(), leftModel, rightModel, compatibility);

            if (right.Kind() != SyntaxKind.IdentifierName)
                return false;

            if (TypeCompatible((IdentifierNameSyntax)left, (IdentifierNameSyntax)right, leftModel, rightModel, compatibility))
                return ChildrenCompatible(left.ChildNodes(), right.ChildNodes(), leftModel, rightModel, compatibility);

            return false;

        }

        private bool TypeCompatible(IdentifierNameSyntax left, IdentifierNameSyntax right, SemanticModel leftModel, SemanticModel rightModel, Dictionary<string, ITypeSymbol> compatibility)
        {
            var leftType = leftModel.GetTypeInfo(left).Type;
            var rightType = rightModel.GetTypeInfo(right).Type;

            if (leftType == null)
                return true; //Not a variable

            if (rightType == null)
                return false; //Different types of identifiers

            //Both are variables, check if they have a common base
            var leftTypes = new List<ITypeSymbol>();
            var current = leftType;
            while (current != null && current.Name != "Object")
            {
                leftTypes.Add(current);
                current = current.BaseType;
            }

            current = rightType;
            while (current != null && current.Name != "Object")
            {
                foreach (var candidate in leftTypes)
                {
                    if (current == candidate)
                    {
                        compatibility[$"{left}+{right}"] = current;
                        return true;
                    }
                }

                current = current.BaseType;
            }

            return false;
        }


        private bool ChildrenCompatible(IEnumerable<SyntaxNode> leftChildren, IEnumerable<SyntaxNode> rightChildren, SemanticModel leftModel, SemanticModel rightModel, Dictionary<string, ITypeSymbol> compatibility)
        {
            var parallelEnumerator = leftChildren.Zip(rightChildren, (x, y) => new { Left = x, Right = y });

            if (parallelEnumerator.Any(pair => !Compatible(pair.Left, pair.Right, leftModel, rightModel, compatibility)))
                return false;

            //No inequalities, check if children are balanced
            return leftChildren.Count() == rightChildren.Count();
        }
    }
}

