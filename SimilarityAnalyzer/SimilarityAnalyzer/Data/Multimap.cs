using Microsoft.CodeAnalysis;
using SimilarityAnalyzer.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimilarityAnalyzer.Data
{
    class Multimap<Item, Pair> where Pair : new()
    {
        public Dictionary<Item, List<Item>> map { get; private set; } = new Dictionary<Item, List<Item>>();

        public void AddFragmentOfMethod(Item method, Item fragment)
        {
            List<Item> existingFragments = null;
            if (!map.TryGetValue(method, out existingFragments))
            {
                existingFragments = new List<Item>();
                map.Add(method, existingFragments);
            }

            existingFragments.Add(fragment);
        }

        public IEnumerable<Pair> GetOuterPairs()
        {
            var methodPairs = EnumerablePairHelpers.InnerPairs(map.Keys);

            var nodeListPairs = methodPairs.Select(pair => new Tuple<IEnumerable<Item>, IEnumerable<Item>>(map[pair.Item1], map[pair.Item2]));

            var nodePairs = nodeListPairs.Select(listPair => EnumerablePairHelpers.OuterPairs(listPair.Item1, listPair.Item2)).SelectMany(x => x);

            return nodePairs.Select(pair => (Pair)Activator.CreateInstance(typeof(Pair), pair));
        }

        public IEnumerable<Pair> GetInnerPairs()
        {
            List<Pair> nodePairs = new List<Pair>();

            foreach(var nodes in map.Values)
            {
                //EnumerablePairHelpers.InnerPairs(nodes).ForEach(tuple => nodePairs.Add((Pair)Activator.CreateInstance(typeof(Pair), tuple)));
            }

            return nodePairs;
        }
    }
}
