using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyntaxComparision.Interfaces;

namespace SyntaxVectors
{
  public class VectorComparator<TPair, TRepresentation> : ISyntaxComparator<TPair, TRepresentation>
    where TRepresentation : NodeWithVector
    where TPair : ISyntaxPair<TRepresentation>
  {
    public bool Equals(TPair pair)
    {
      return pair.Left.Vector.Equals(pair.Right.Vector);
    }
  }
}
