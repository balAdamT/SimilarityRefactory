using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using SyntaxComparision.Data;
using SyntaxComparision.Interfaces;

namespace SyntaxComparision.Algorithm
{
  public class NodeToNode : ISyntaxPreprocessor<NodeAsRepresentation>
  {
    public NodeAsRepresentation Process(SyntaxNode node)
    {
      return new NodeAsRepresentation(node);
    }
  }
}
