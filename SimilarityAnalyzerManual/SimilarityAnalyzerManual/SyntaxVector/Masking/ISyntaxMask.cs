using Microsoft.CodeAnalysis.CSharp;

namespace SimilarityAnalyzer.SyntaxVectors.Masking
{
    public interface ISyntaxMask
    {
        bool Skip(SyntaxKind kind);
        bool Terminal(SyntaxKind kind);
    }
}
