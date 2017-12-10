using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyntaxVectors.Masking;

namespace SyntaxVectors.Representation
{
    public static class SyntaxCounter
    {
        public static SyntaxVector Calculate(SyntaxNode root, ISyntaxMask mask)
        {
            SyntaxVector desc = new SyntaxVector(mask);

            //This is safe, IsKind() uses the same logic to comapre it
            desc.Count((SyntaxKind)root.RawKind);

            //TODO: Consider mutlithreading with Interlocked
            foreach(var node in root.DescendantNodes())
            {
                desc.Count((SyntaxKind)root.RawKind);
            }

            return desc;
        }
    }
}
