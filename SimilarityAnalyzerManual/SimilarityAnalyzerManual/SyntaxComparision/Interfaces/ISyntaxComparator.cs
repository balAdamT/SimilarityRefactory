namespace SimilarityAnalyzer.SyntaxComparision.Interfaces
{
    public interface ISyntaxComparator<in TPair, in TRepresentation, in TInformation>
    where TRepresentation : ISyntaxRepresentation
    where TPair : ISyntaxPair<TRepresentation>
    where TInformation : ISyntaxInformation
    {
        bool SyntaxEquals(TPair pair, TInformation information);
    }
}
