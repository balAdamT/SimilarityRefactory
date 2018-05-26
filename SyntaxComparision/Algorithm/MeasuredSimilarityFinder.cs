using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimilarityAnalyzer.Helpers;
using SyntaxComparision.Interfaces;

namespace SyntaxComparision.Algorithm
{
    public class MeasuredSimilarityFinder<TPair, TRepresentation, TInformation>
        where TRepresentation : ISyntaxRepresentation
        where TPair : ISyntaxPair<TRepresentation>, new()
        where TInformation : ISyntaxInformation
    {
        private readonly ISyntaxSource source;
        private readonly ISyntaxPreprocessor<TRepresentation> preprocessor;
        private readonly List<ISyntaxComparator<TPair, TRepresentation, TInformation>> comparators;
        private readonly TInformation information;

        private SimilarityMeasure measure;
        public SimilarityMeasure Measure => measure;

        public MeasuredSimilarityFinder(ISyntaxSource source, ISyntaxPreprocessor<TRepresentation> preprocessor,
          List<ISyntaxComparator<TPair, TRepresentation, TInformation>> comparators, TInformation information)
        {
            this.source = source;
            this.preprocessor = preprocessor;
            this.comparators = comparators;
            this.information = information;

            measure = new SimilarityMeasure();
        }

        public IEnumerable<TPair> FindAll()
        {
            var nodes = source.Fetch().Select((x, i) =>
            {
                measure.Sources = i;
                return x;
            });
            var representations = nodes.Select(node => preprocessor.Process(node));

            var pairs = representations.InnerPairs<TPair, TRepresentation>().Select((x, i) =>
            {
                measure.Pairs = i;
                return x;
            });

            measure.Similarities = Enumerable.Repeat(0, comparators.Count()).ToList();
            foreach (var element in pairs)
            {
                var cIndex = 0;
                foreach (var comparator in comparators)
                {
                    if (!comparator.SyntaxEquals(element, information))
                        goto SkipThisElement;

                    measure.Similarities[cIndex++]++;
                }

                yield return element;
                SkipThisElement:
                ;
            }

        }
    }

    public struct SimilarityMeasure
    {
        public int Sources;
        public int Pairs;
        public List<int> Similarities;

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(Sources);
            sb.Append('>');
            sb.Append(Pairs);

            Similarities.ForEachNow(s => sb.Append($">{s}"));

            return sb.ToString();
        }
    }
}
