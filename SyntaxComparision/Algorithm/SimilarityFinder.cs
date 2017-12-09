using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimilarityAnalyzer.Helpers;
using SyntaxComparision.Interfaces;

namespace SyntaxComparision.Algorithm
{
  public class SimilarityFinder<TPair, TRepresentation>
    where TRepresentation : ISyntaxRepresentation
    where TPair : ISyntaxPair<TRepresentation>, new()
  {
    private readonly ISyntaxSource source;
    private readonly ISyntaxPreprocessor<TRepresentation> preprocessor;
    private readonly List<ISyntaxComparator<TPair, TRepresentation>> comparators;

    public SimilarityFinder(ISyntaxSource source, ISyntaxPreprocessor<TRepresentation> preprocessor, List<ISyntaxComparator<TPair, TRepresentation>> comparators)
    {
      this.source = source;
      this.preprocessor = preprocessor;
      this.comparators = comparators;
    }

    public IEnumerable<TPair> FindAll()
    {
      var nodes = source.Fetch();
      var representations = nodes.Select(node => preprocessor.Process(node));
      var pairs = representations.InnerPairs<TPair, TRepresentation>();
      var found = pairs.Where(comparators);

      return found;
    }
  }
}
