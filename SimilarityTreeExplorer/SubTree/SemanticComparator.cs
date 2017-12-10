using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyntaxComparision.Interfaces;

namespace SimilarityTreeExplorer.SubTree
{
  public class SemanticComparator<TPair, TRepresentation> : ISyntaxComparator<TPair, TRepresentation>
    where TRepresentation : ISyntaxRepresentation
    where TPair : ISyntaxPair<TRepresentation>
  {
    public bool Equals(TPair pair)
    {
      return pair.Left.Node.IsEquivalentTo(pair.Right.Node);
    }
  }
}

