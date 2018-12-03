using Microsoft.CodeAnalysis;

namespace SimilarityAnalyzer.SyntaxComparision.Helpers
{
    public static class LocationExtensions
    {
        public static string ToVisualStudioString(this Location location)
        {
            var result = "";
            var pos = location.GetLineSpan();
            if (pos.Path != null)
            {
                // user-visible line and column counts are 1-based, but internally are 0-based.
                result += "("
                    + pos.Path
                    + "@"
                    + (pos.StartLinePosition.Line + 1) + ":" + (pos.StartLinePosition.Character + 1)
                    + "-"
                    + (pos.EndLinePosition.Line + 1) + ":" + (pos.EndLinePosition.Character + 1)
                    + ")";
            }

            return result;
        }
    }
}
