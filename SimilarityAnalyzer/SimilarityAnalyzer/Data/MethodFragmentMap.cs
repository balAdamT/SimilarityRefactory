using Microsoft.CodeAnalysis;
using SimilarityAnalyzer.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimilarityAnalyzer.Data
{
    class MethodFragmentMap
    {
        public Dictionary<SyntaxNode, List<SyntaxNode>> map { get; private set; } = new Dictionary<SyntaxNode, List<SyntaxNode>>();

        public void AddFragmentOfMethod(SyntaxNode method, SyntaxNode fragment)
        {
            List<SyntaxNode> existingFragments = null;
            if (!map.TryGetValue(method, out existingFragments))
            {
                existingFragments = new List<SyntaxNode>();
                map.Add(method, existingFragments);
            }

            existingFragments.Add(fragment);
        }

        public IEnumerable<NodePair> GetOuterPairs()
        {
            var methodPairs = EnumerablePairHelpers.InnerPairs(map.Keys);

            var nodeListPairs = methodPairs.Select(pair => new Tuple<IEnumerable<SyntaxNode>, IEnumerable<SyntaxNode>>(map[pair.Item1], map[pair.Item2]));

            var nodePairs = nodeListPairs.Select(listPair => EnumerablePairHelpers.OuterPairs(listPair.Item1, listPair.Item2)).SelectMany(x => x);

            return nodePairs.Select(pair => new NodePair(pair));
        }

        public IEnumerable<NodePair> GetInnerPairs()
        {
            List<NodePair> nodePairs = new List<NodePair>();

            foreach(var nodes in map.Values)
            {
                EnumerablePairHelpers.InnerPairs(nodes).ForEach(tuple => nodePairs.Add(new NodePair(tuple)));
            }

            return nodePairs;
        }
    }
}
