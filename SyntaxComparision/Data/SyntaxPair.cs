using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyntaxComparision.Interfaces;

namespace SyntaxComparision.Data
{
  public class SyntaxPair<TRepresentation> : ISyntaxPair<TRepresentation> where TRepresentation : ISyntaxRepresentation
  {
    public TRepresentation Left { get; set; }

    public TRepresentation Right { get; set; }

    public SyntaxPair(TRepresentation left, TRepresentation right)
    {
      Left = left;
      Right = right;
    }

    public static implicit operator SyntaxPair<TRepresentation>(Tuple<TRepresentation, TRepresentation> tuple)
    {
      return new SyntaxPair<TRepresentation>(tuple.Item1, tuple.Item2);
    }
  }
}
