using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using SimilarityAnalyzer.SyntaxComparision.Interfaces;
using System.Linq;

namespace SimilarityAnalyzer.SimilarityTree.SubTree
{
    public class StructuralComparator<TPair, TRepresentation, TInformation> : ISyntaxComparator<TPair, TRepresentation, TInformation>
      where TRepresentation : ISyntaxRepresentation
      where TPair : ISyntaxPair<TRepresentation>
      where TInformation : ISyntaxInformation
    {
        public bool SyntaxEquals(TPair pair, TInformation information)
        {
            //return pair.Left.Node.IsEquivalentTo(pair.Right.Node);

            return Compare(pair.Left.Node, pair.Right.Node);
        }

        private bool Compare(SyntaxNode node1, SyntaxNode node2)
        {
            if (node1.Kind() != node2.Kind())
                return false;

            System.Collections.Generic.IEnumerable<SyntaxNode> leftChildren = node1.ChildNodes();
            System.Collections.Generic.IEnumerable<SyntaxNode> rightChildren = node2.ChildNodes();

            var parallelEnumerator = leftChildren.Zip(rightChildren, (x, y) => new { Left = x, Right = y });

            if (parallelEnumerator.Any(p => !Compare(p.Left, p.Right)))
                return false;

            //No inequalities, check if children are balanced
            return leftChildren.Count() == rightChildren.Count();
        }
    }
}

