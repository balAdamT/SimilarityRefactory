using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace SyntaxComparision.Interfaces
{
  public interface ISyntaxRepresentation
  {
    SyntaxNode Node { get; }
  }
}
