using Microsoft.CodeAnalysis.CSharp;
using SimilarityAnalyzer.SyntaxVectors.Masking;
using System;
using System.Collections.Generic;

namespace SimilarityAnalyzer.SyntaxVectors.Representation
{
    public class SyntaxVector
    {
        private Dictionary<SyntaxKind, int> count = new Dictionary<SyntaxKind, int>();
        public SyntaxVector(ISyntaxMask mask)
        {
            SyntaxKind[] allKinds = (SyntaxKind[])Enum.GetValues(typeof(SyntaxKind));
            foreach (SyntaxKind kind in allKinds)
            {
                if (mask.Skip(kind))
                    continue;

                count[kind] = 0;
            }
        }

        public void Count(SyntaxKind kind)
        {
            //Throws exception
            count[kind]++;
        }

        public int this[SyntaxKind key]
        {
            get
            {
                return count[key];
            }
        }

        public override bool Equals(object obj)
        {
            SyntaxVector other = obj as SyntaxVector;

            if (other == null)
                return false;

            if (this.count.Keys.Count != other.count.Keys.Count)
                return false;

            try
            {
                foreach (SyntaxKind key in this.count.Keys)
                {
                    if (this[key] != other[key])
                        return false;
                }
            }
            catch (KeyNotFoundException)
            {
                return false;
            }

            return true;
        }

        public ulong GetLongHash()
        {
            ulong power(int degree)
            {
                ulong result = 10;
                for (int i = 0; i < degree - 1; i++)
                    result *= 10;

                return result;
            }

            ulong spacing = power(32);
            foreach (KeyValuePair<SyntaxKind, int> entry in count)
            {
                // hash += (ulong)entry.Key * spacing;
            }

            return 0;
        }
    }
}
