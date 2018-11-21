using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace SimilarityAnalyzer.SyntaxComparision.Interfaces
{
    public interface ISyntaxSource
    {
        IEnumerable<SyntaxNode> Fetch();
    }
}
