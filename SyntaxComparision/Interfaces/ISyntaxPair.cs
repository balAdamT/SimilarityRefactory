using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyntaxComparision.Interfaces
{
  public interface ISyntaxPair<out TRepresentation> where TRepresentation : ISyntaxRepresentation
  {
    TRepresentation Left { get; }
    TRepresentation Right { get; }
  }
}
