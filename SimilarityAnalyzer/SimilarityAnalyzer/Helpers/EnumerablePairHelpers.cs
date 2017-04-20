using SimilarityAnalyzer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimilarityAnalyzer.Helpers
{
    public static class EnumerablePairHelpers
    {
        public static IEnumerable<Tuple<T, T>> InnerPairs<T>(IEnumerable<T> list)
        {
            return from item1 in list
                   from item2 in list
                   where item1.GetHashCode() < item2.GetHashCode()
                   select new Tuple<T, T>(item1, item2);
        }

        public static IEnumerable<Tuple<T, T>> OuterPairs<T>(IEnumerable<T> list1, IEnumerable<T> list2)
        {
            return from item1 in list1
                   from item2 in list2
                   select new Tuple<T, T>(item1, item2);
        }
    }
}
