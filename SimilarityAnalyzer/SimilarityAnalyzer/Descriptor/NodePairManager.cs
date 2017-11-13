using SimilarityAnalyzer.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimilarityAnalyzer.Descriptor
{
    public struct NodePairManager
    {
        public SyntaxNodeManager Left;
        public SyntaxNodeManager Right;

        public NodePairManager(SyntaxNodeManager left, SyntaxNodeManager right)
        {
            Left = left;
            Right = right;
        }

        public NodePairManager(Tuple<SyntaxNodeManager, SyntaxNodeManager> pair)
        {
            Left = pair.Item1;
            Right = pair.Item2;
        }

        public NodePair ToNodePair()
        {
            return new NodePair(Left.ToNode(), Right.ToNode());
        }

        internal bool DescriptionMatches()
        {
            return Left == Right;
        }

        internal bool EqualPair()
        {
            return Left.ToNode().IsEquivalentTo(Right.ToNode());
        }
    }
}
