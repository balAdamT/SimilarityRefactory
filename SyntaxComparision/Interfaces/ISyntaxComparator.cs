using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyntaxComparision.Interfaces
{
  interface ISyntaxComparator<TPair, TRepresentation>
    where TRepresentation : ISyntaxRepresentation
    where TPair : ISyntaxPair<TRepresentation>
  {
    IEnumerable<TPair> Filter(IEnumerable<TPair> all);
  }
}
