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
  public class NodeToVector : ISyntaxPreprocessor<NodeWithVector>
  {
    private readonly ISyntaxMask mask;

    public NodeToVector(ISyntaxMask mask)
    {
      this.mask = mask;
    }

    public NodeWithVector Process(SyntaxNode node)
    {
      return new NodeWithVector(node, SyntaxCounter.Calculate(node, mask));
    }
  }
}
