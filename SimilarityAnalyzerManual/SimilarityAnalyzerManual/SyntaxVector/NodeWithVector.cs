using Microsoft.CodeAnalysis;
using SimilarityAnalyzer.SyntaxComparision.Interfaces;
using SimilarityAnalyzer.SyntaxVectors.Representation;

namespace SimilarityAnalyzer.SyntaxVectors
{
    public class NodeWithVector : ISyntaxRepresentation
    {
        public NodeWithVector() { }

        public NodeWithVector(SyntaxNode node, SyntaxVector vector)
        {
            Node = node;
            Vector = vector;
        }

        public SyntaxNode Node { get; private set; }
        public SyntaxVector Vector { get; private set; }
    }
}
