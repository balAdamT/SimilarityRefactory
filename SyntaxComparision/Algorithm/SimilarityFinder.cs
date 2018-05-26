using SimilarityAnalyzer.Helpers;
using SyntaxComparision.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace SyntaxComparision.Algorithm
{
    public class SimilarityFinder<TPair, TRepresentation, TInformation>
      where TRepresentation : ISyntaxRepresentation
      where TPair : ISyntaxPair<TRepresentation>, new()
      where TInformation : ISyntaxInformation
    {
        private readonly ISyntaxSource source;
        private readonly ISyntaxPreprocessor<TRepresentation> preprocessor;
        private readonly List<ISyntaxComparator<TPair, TRepresentation, TInformation>> comparators;
        private readonly TInformation information;

        //TODO DI
        public SimilarityFinder(ISyntaxSource source, ISyntaxPreprocessor<TRepresentation> preprocessor, List<ISyntaxComparator<TPair, TRepresentation, TInformation>> comparators, TInformation information)
        {
            this.source = source;
            this.preprocessor = preprocessor;
            this.comparators = comparators;
            this.information = information;
        }

        public IEnumerable<TPair> FindAll()
        {
            var nodes = source.Fetch();
            var representations = nodes.Select(node => preprocessor.Process(node));
            var pairs = representations.InnerPairs<TPair, TRepresentation>();
            var found = pairs.Where(comparators, information);

            return found;
        }
    }
}
