using Microsoft.CodeAnalysis;

namespace SimilarityAnalyzer.SyntaxComparision.Interfaces
{
    public interface ISyntaxRepresentation
    {
        SyntaxNode Node { get; }
    }
}
