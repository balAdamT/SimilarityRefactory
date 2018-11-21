using Microsoft.CodeAnalysis;

namespace SimilarityAnalyzer.SyntaxComparision.Interfaces
{
    public interface ISyntaxPreprocessor<out TRepresentation> where TRepresentation : ISyntaxRepresentation
    {
        TRepresentation Process(SyntaxNode node);
    }
}
