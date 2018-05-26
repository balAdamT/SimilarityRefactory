using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyntaxComparision.Interfaces;

namespace SyntaxVectors
{
  public class VectorComparator<TPair, TRepresentation, TInformation> : ISyntaxComparator<TPair, TRepresentation, TInformation>
    where TRepresentation : NodeWithVector
    where TPair : ISyntaxPair<TRepresentation>
    where TInformation : ISyntaxInformation
  {
    public bool SyntaxEquals(TPair pair, TInformation information)
    {
      return pair.Left.Vector.Equals(pair.Right.Vector);
    }
  }
}
