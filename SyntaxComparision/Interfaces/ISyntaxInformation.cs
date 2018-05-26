using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyntaxComparision.Interfaces
{
    public interface ISyntaxInformation
    {
        SemanticModel Provide(SyntaxTree tree);
    }
}
