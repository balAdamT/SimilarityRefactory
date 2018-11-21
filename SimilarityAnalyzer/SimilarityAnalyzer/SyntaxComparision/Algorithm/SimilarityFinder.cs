using SimilarityAnalyzer.Helpers;
using SimilarityAnalyzer.SyntaxComparision.Interfaces;
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
            IEnumerable<Microsoft.CodeAnalysis.SyntaxNode> nodes = source.Fetch();
            IEnumerable<TRepresentation> representations = nodes.Select(node => preprocessor.Process(node));
            IEnumerable<TPair> pairs = representations.InnerPairs<TPair, TRepresentation>();
            IEnumerable<TPair> found = pairs.Where(comparators, information);

            return found;
        }
    }
}
