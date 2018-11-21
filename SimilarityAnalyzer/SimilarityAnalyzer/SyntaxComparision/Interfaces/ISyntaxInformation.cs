using Microsoft.CodeAnalysis;

namespace SimilarityAnalyzer.SyntaxComparision.Interfaces
{
    public interface ISyntaxInformation
    {
        SemanticModel Provide(SyntaxTree tree);
    }
}
