using SimilarityAnalyzer.Helpers;
using SimilarityAnalyzer.SyntaxComparision.Helpers;
using SimilarityAnalyzer.SyntaxComparision.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimilarityAnalyzer.SyntaxComparision.Algorithm
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
            IEnumerable<Microsoft.CodeAnalysis.SyntaxNode> nodes = source.Fetch().Select((x, i) =>
            {
                measure.Sources = i;
                return x;
            });
            IEnumerable<TRepresentation> representations = nodes.Select(node => preprocessor.Process(node));

            IEnumerable<TPair> pairs = representations.InnerPairs<TPair, TRepresentation>().Select((x, i) =>
            {
                measure.Pairs = i;
                return x;
            });

            measure.Similarities = Enumerable.Repeat(0, comparators.Count()).ToList();
            foreach (TPair element in pairs)
            {
                int cIndex = 0;
                foreach (ISyntaxComparator<TPair, TRepresentation, TInformation> comparator in comparators)
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
            StringBuilder sb = new StringBuilder();
            sb.Append(Sources);
            sb.Append('>');
            sb.Append(Pairs);

            Similarities.ForEachNow(s => sb.Append($">{s}"));

            return sb.ToString();
        }
    }
}
