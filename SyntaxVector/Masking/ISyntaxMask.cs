using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace SyntaxVectors.Masking
{
    public interface ISyntaxMask
    {
        bool Skip(SyntaxKind kind);
        bool Terminal(SyntaxKind kind);
    }
}
