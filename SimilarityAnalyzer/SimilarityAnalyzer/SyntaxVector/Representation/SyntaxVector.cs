using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;

namespace SimilarityAnalyzer.SyntaxVectors.Representation
{
    public class SyntaxVector
    {
        private ulong hash;
        private SyntaxVector(ulong hash)
        {
            this.hash = hash;
        }

        public static SyntaxVector Calculate(SyntaxNode root)
        {
            ulong result = 0;

            result += CalculatePartialHash(root);

            foreach (var node in root.DescendantNodes())
            {
                result += CalculatePartialHash(node);
            }

            return new SyntaxVector(result);
        }

        private static ulong CalculatePartialHash(SyntaxNode node)
        {
            var kindValue = (ulong)node.RawKind;

            return kindValue * kindValue * kindValue;
        }

        public override bool Equals(object obj)
        {
            var other = obj as SyntaxVector;

            if (other == null)
                return false;

            return hash == other.hash;
        }

        public ulong GetLongHash()
        {
            return hash;
        }

        public override int GetHashCode()
        {
            return 734893433 + hash.GetHashCode();
        }
    }
}
