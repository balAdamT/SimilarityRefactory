using Microsoft.CodeAnalysis;
using SimilarityAnalyzer.SyntaxComparision.Interfaces;
using System.Collections.Generic;

namespace SimilarityAnalyzer.SyntaxComparision.Information
{
    public class OncePerTreeInformation : ISyntaxInformation
    {
        private readonly Compilation compilation;
        private readonly Dictionary<long, SemanticModel> cache = new Dictionary<long, SemanticModel>();

        public OncePerTreeInformation()
        {

        }

        public OncePerTreeInformation(Compilation compilation)
        {
            this.compilation = compilation;
        }

        public SemanticModel Provide(SyntaxTree tree)
        {
            int hash = tree.GetHashCode();

            if (!cache.ContainsKey(hash))
                cache[hash] = compilation.GetSemanticModel(tree);

            return cache[hash];
        }
    }
}
