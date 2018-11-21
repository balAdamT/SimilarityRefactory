using Microsoft.CodeAnalysis;
using SimilarityAnalyzer.SyntaxComparision.Interfaces;

namespace SimilarityAnalyzer.SyntaxComparision.Data
{
    public class NodeAsRepresentation : ISyntaxRepresentation
    {
        public NodeAsRepresentation()
        {

        }
        public NodeAsRepresentation(SyntaxNode node)
        {
            Node = node;
        }

        public SyntaxNode Node { get; set; }
    }
}
