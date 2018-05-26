using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimilarityAnalyzer.Logic;
using SyntaxComparision.Interfaces;

namespace SimilarityTreeExplorer.SuperTree
{
    public class SuperTreeComparator<TPair, TRepresentation, TInformation> : ISyntaxComparator<TPair, TRepresentation, TInformation>
      where TRepresentation : ISyntaxRepresentation
      where TPair : SyntaxLeafPair<TRepresentation>
      where TInformation : ISyntaxInformation
    {
        public bool SyntaxEquals(TPair pair, TInformation information)
        {
            var height = SyntaxCompare.FindCommonSuperTree(pair.Left.Node, pair.Right.Node);
            pair.EquivalenceHeight = height;

            return height > 0;
        }
    }
}
