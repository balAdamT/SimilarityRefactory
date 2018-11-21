using SimilarityAnalyzer.SyntaxComparision.Interfaces;

namespace SimilarityAnalyzer.SimilarityTree.SuperTree
{
    public class SuperTreeComparator<TPair, TRepresentation, TInformation> : ISyntaxComparator<TPair, TRepresentation, TInformation>
      where TRepresentation : ISyntaxRepresentation
      where TPair : SyntaxLeafPair<TRepresentation>
      where TInformation : ISyntaxInformation
    {
        public bool SyntaxEquals(TPair pair, TInformation information)
        {
            int height = SyntaxCompare.FindCommonSuperTree(pair.Left.Node, pair.Right.Node);
            pair.EquivalenceHeight = height;

            return height > 0;
        }
    }
}
