using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using SimilarityAnalyzer.SyntaxVectors.Masking;

namespace SimilarityAnalyzer.SyntaxVectors.Representation
{
    public static class SyntaxCounter
    {
        public static SyntaxVector Calculate(SyntaxNode root, ISyntaxMask mask)
        {
            SyntaxVector desc = new SyntaxVector(mask);

            //This is safe, IsKind() uses the same logic to comapre it
            desc.Count((SyntaxKind)root.RawKind);

            //TODO: Consider mutlithreading with Interlocked
            foreach (SyntaxNode node in root.DescendantNodes())
            {
                desc.Count((SyntaxKind)root.RawKind);
            }

            return desc;
        }
    }
}
