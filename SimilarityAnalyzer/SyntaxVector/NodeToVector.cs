using Microsoft.CodeAnalysis;
using SimilarityAnalyzer.SyntaxComparision.Interfaces;
using SimilarityAnalyzer.SyntaxVectors.Masking;
using SimilarityAnalyzer.SyntaxVectors.Representation;

namespace SimilarityAnalyzer.SyntaxVectors
{
    public class NodeToVector : ISyntaxPreprocessor<NodeWithVector>
    {
        private readonly ISyntaxMask mask;

        public NodeToVector(ISyntaxMask mask)
        {
            this.mask = mask;
        }

        public NodeWithVector Process(SyntaxNode node)
        {
            return new NodeWithVector(node, SyntaxCounter.Calculate(node, mask));
        }
    }
}
