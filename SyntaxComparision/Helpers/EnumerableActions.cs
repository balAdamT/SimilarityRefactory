using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimilarityAnalyzer.Helpers
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
