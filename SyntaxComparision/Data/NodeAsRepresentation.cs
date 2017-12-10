using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using SyntaxComparision.Interfaces;

namespace SyntaxComparision.Data
{
  public class NodeAsRepresentation : ISyntaxRepresentation
  {
    public NodeAsRepresentation()
    {
      
    }
    public NodeAsRepresentation(SyntaxNode node)
    {
      Node = node;
    }

    public SyntaxNode Node { get; set; }
  }
}
