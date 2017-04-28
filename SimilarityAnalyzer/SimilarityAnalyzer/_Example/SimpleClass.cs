using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimilarityAnalyzer._Example
{
    class SimpleClass
    {
        public int Method1(int z, int x)
        {
            int a = z + x;
            a *= 2;

            if (a >= 20)
            {
                z = a;
            }

            return z - x;
        }

        public int Method2(int z, int c)
        {
            int a = z + c;
            a *= 4;

            if (a >= 20)
            {
                z = a;
            }

            return z - c;
        }
    }
}
