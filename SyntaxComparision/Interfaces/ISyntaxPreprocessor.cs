using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace SyntaxComparision.Interfaces
{
  interface ISyntaxPreprocessor<out TRepresentation> where TRepresentation : ISyntaxRepresentation
  {
    TRepresentation Process(SyntaxNode node);
    IEnumerable<TRepresentation> Process(IEnumerable<SyntaxNode> node);
  }
}
