using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyntaxComparision.Interfaces;

namespace SyntaxComparision.Data
{
  public class SyntaxPair<Representation> : ISyntaxPair<Representation> where Representation : ISyntaxRepresentation
  {
    public Representation Left { get; set; }

    public Representation Right { get; set; }

    public SyntaxPair(Representation left, Representation right)
    {
      Left = left;
      Right = right;
    }

    public static implicit operator SyntaxPair<Representation>(Tuple<Representation, Representation> tuple)
    {
      return new SyntaxPair<Representation>(tuple.Item1, tuple.Item2);
    }
  }
}
