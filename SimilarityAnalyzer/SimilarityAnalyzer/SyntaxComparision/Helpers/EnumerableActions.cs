using System;
using System.Collections.Generic;
using System.Linq;

namespace SimilarityAnalyzer.SyntaxComparision.Helpers
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> sequence, Action<T> action)
        {
            foreach (T item in sequence)
            {
                action(item);
                yield return item;
            }
        }

        public static IEnumerable<T> ForEachNow<T>(this IEnumerable<T> sequence, Action<T> action)
        {
            return sequence.ForEach<T>(action).ToList();
        }
    }
}
