using Microsoft.CodeAnalysis;
using SimilarityAnalyzer.Masking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimilarityAnalyzer.Descriptor
{
    public class SyntaxNodeManager
    {
        private SyntaxNode node;
        private SyntaxDescriptor desc;

        public SyntaxNodeManager(SyntaxNode node)
        {
            this.node = node;
            desc = SyntaxCounter.Calculate(node, SyntaxMasks.AllNodes);
        }

        public SyntaxNode ToNode()
        {
            return node;
        }

        public override bool Equals(object obj)
        {
            var other = obj as SyntaxNodeManager;

            if (other == null)
                return false;

            return this.desc == other.desc;
        }
    }
}
