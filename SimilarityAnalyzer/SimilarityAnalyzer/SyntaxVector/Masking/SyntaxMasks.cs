using SimilarityAnalyzer.SyntaxVectors.Masking;

namespace SimilarityAnalyzer.SyntaxVectors.Masking
{
    public static class SyntaxMasks
    {
        public static ISyntaxMask AllNodes => new AllNodesMask();
    }
}
