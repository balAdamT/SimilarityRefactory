using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyntaxComparision.Interfaces;

namespace SimilarityTreeExplorer.SubTree
{
    public class StructuralComparator<TPair, TRepresentation, TInformation> : ISyntaxComparator<TPair, TRepresentation, TInformation>
      where TRepresentation : ISyntaxRepresentation
      where TPair : ISyntaxPair<TRepresentation>
      where TInformation : ISyntaxInformation
    {
        public bool SyntaxEquals(TPair pair, TInformation information)
        {
            return pair.Left.Node.IsEquivalentTo(pair.Right.Node);
        }
    }
}

