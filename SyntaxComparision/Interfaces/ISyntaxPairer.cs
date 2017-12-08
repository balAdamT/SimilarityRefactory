using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace SyntaxComparision.Interfaces
{
  interface IComparisonSource<out TPair, TRepresentation> 
    where TRepresentation : ISyntaxRepresentation
    where TPair : ISyntaxPair<TRepresentation>
  {
    IEnumerable<TPair> All(IEnumerable<ISyntaxRepresentation> source);
  }
}
