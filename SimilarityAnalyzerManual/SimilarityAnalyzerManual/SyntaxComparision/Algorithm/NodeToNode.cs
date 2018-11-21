using Microsoft.CodeAnalysis;
using SimilarityAnalyzer.SyntaxComparision.Data;
using SimilarityAnalyzer.SyntaxComparision.Interfaces;

namespace SimilarityAnalyzer.SyntaxComparision.Algorithm
{
    public class NodeToNode : ISyntaxPreprocessor<NodeAsRepresentation>
    {
        public NodeAsRepresentation Process(SyntaxNode node)
        {
            return new NodeAsRepresentation(node);
        }
    }
}
