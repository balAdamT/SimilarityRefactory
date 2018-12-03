using Microsoft.CodeAnalysis;
using SimilarityAnalyzer.SyntaxComparision.Interfaces;
using SimilarityAnalyzer.SyntaxVectors.Representation;

namespace SimilarityAnalyzer.SyntaxVectors
{
    public class NodeToVector : ISyntaxPreprocessor<NodeWithVector>
    {
        public NodeWithVector Process(SyntaxNode node)
        {
            return new NodeWithVector(node, SyntaxVector.Calculate(node));
        }
    }
}
