namespace SimilarityAnalyzer.SyntaxComparision.Interfaces
{
    public interface ISyntaxPair<out TRepresentation> where TRepresentation : ISyntaxRepresentation
    {
        TRepresentation Left { get; }
        TRepresentation Right { get; }
    }
}
