using SimilarityAnalyzer.SyntaxComparision.Interfaces;
using System;

namespace SimilarityAnalyzer.SimilarityTree.SuperTree
{
    public class SyntaxLeafPair<TRepresentation> : ISyntaxPair<TRepresentation> where TRepresentation : ISyntaxRepresentation
    {
        public TRepresentation Left { get; set; }
        public TRepresentation Right { get; set; }
        public int? EquivalenceHeight { get; set; }

        public SyntaxLeafPair() { }
        public SyntaxLeafPair(TRepresentation left, TRepresentation right)
        {
            Left = left;
            Right = right;
        }
        public static implicit operator SyntaxLeafPair<TRepresentation>(Tuple<TRepresentation, TRepresentation> tuple)
        {
            return new SyntaxLeafPair<TRepresentation>(tuple.Item1, tuple.Item2);
        }
    }
}
