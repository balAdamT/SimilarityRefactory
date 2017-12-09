using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyntaxComparision.Interfaces
{
  public interface ISyntaxComparator<in TPair, TRepresentation>
    where TRepresentation : ISyntaxRepresentation
    where TPair : ISyntaxPair<TRepresentation>
  {
    bool Equals(TPair pair);
  }
}
