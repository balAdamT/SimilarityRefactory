using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.CSharp;

namespace SimilarityAnalyzer
{
    public interface ISyntaxMask
    {
        bool Skip(SyntaxKind kind);
        bool Terminal(SyntaxKind kind);
    }
}
