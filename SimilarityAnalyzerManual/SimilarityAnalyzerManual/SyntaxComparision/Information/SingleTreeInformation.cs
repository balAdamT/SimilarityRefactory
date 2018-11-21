using Microsoft.CodeAnalysis;
using SimilarityAnalyzer.SyntaxComparision.Interfaces;

namespace SimilarityAnalyzer.SyntaxComparision.Information
{
    public class SingleTreeInformation : ISyntaxInformation
    {
        private SemanticModel model;

        public SingleTreeInformation()
        {

        }

        public SingleTreeInformation(SemanticModel model)
        {
            this.model = model;
        }

        public SemanticModel Provide(SyntaxTree tree)
        {
            return model;
        }
    }
}
