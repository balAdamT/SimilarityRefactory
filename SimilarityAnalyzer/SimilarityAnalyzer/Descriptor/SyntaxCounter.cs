using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimilarityAnalyzer.Descriptor
{
    public static class SyntaxCounter
    {
        public static SyntaxDescriptor Calculate(SyntaxNode root, ISyntaxMask mask)
        {
            SyntaxDescriptor desc = new SyntaxDescriptor(mask);

            //TODO: This is safe, IsKind() uses the same logic to comapre it
            desc.Count((SyntaxKind)root.RawKind);

            //TODO: Consider mutlithreading with iNterlocked
            foreach(var node in root.DescendantNodes())
            {
                desc.Count((SyntaxKind)root.RawKind);
            }

            return desc;
        }
    }
}
