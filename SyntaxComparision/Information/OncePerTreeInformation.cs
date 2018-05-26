using Microsoft.CodeAnalysis;
using SyntaxComparision.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyntaxComparision.Information
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
            var hash = tree.GetHashCode();

            if (!cache.ContainsKey(hash))
                cache[hash] = compilation.GetSemanticModel(tree);

            return cache[hash];
        }
    }   
}
