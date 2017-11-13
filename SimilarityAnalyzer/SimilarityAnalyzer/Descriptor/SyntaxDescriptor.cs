using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimilarityAnalyzer
{
    public class SyntaxDescriptor
    {
        Dictionary<SyntaxKind, int> count = new Dictionary<SyntaxKind, int>();
        public SyntaxDescriptor(ISyntaxMask mask)
        {
            var allKinds = (SyntaxKind[])Enum.GetValues(typeof(SyntaxKind));
            foreach (var kind in allKinds)
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
            var other = obj as SyntaxDescriptor;

            if (other == null)
                return false;
            
            if (this.count.Keys.Count != other.count.Keys.Count)
                return false;

            try
            {
                foreach (var key in this.count.Keys)
                {
                    if (this[key] != other[key])
                        return false;
                }
            }
            catch(KeyNotFoundException)
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

            ulong hash = 0;
            foreach(var entry in count)
            {
               // hash += (ulong)entry.Key * spacing;
            }

            return 0;
        }
    }
}
