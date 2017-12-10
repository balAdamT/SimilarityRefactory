using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using SyntaxComparision.Interfaces;
using SyntaxVectors.Masking;
using SyntaxVectors.Representation;

namespace SyntaxVectors
{
  public class NodeWithVector : ISyntaxRepresentation
  {
    public NodeWithVector() {}

    public NodeWithVector(SyntaxNode node, SyntaxVector vector)
    {
      Node = node;
      Vector = vector;
    }

    public SyntaxNode Node { get; private set; }
    public SyntaxVector Vector { get; private set; }
  }
}
