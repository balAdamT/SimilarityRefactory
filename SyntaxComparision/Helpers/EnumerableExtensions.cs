﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using SyntaxComparision.Interfaces;

namespace SimilarityAnalyzer.Helpers
{
  public static class EnumerablePairHelpers
  {
    public static IEnumerable<TPair> InnerPairs<TPair, TRepresentation>(this IEnumerable<TRepresentation> list)
      where TRepresentation : ISyntaxRepresentation
      where TPair : ISyntaxPair<TRepresentation>, new()
    {
      return from item1 in list
             from item2 in list
             where item1.Node.GetHashCode() < item2.Node.GetHashCode()
             select (TPair)Activator.CreateInstance(typeof(TPair), item1, item2);
    }

    public static IEnumerable<TPair> OuterPairs<TPair, TRepresentation>(this IEnumerable<TRepresentation> list1, IEnumerable<TRepresentation> list2)
      where TRepresentation : ISyntaxRepresentation
      where TPair : ISyntaxPair<TRepresentation>, new()
    {
      return from item1 in list1
             from item2 in list2
             select (TPair)Activator.CreateInstance(typeof(TPair), item1, item2);
    }

    public static IEnumerable<TPair> Where<TPair, TRepresentation, TInformation>(this IEnumerable<TPair> source, ICollection<ISyntaxComparator<TPair, TRepresentation, TInformation>> comparators, TInformation information)
      where TRepresentation : ISyntaxRepresentation
      where TPair : ISyntaxPair<TRepresentation>, new()
      where TInformation : ISyntaxInformation
    {
      foreach(var element in source)
      {
        foreach (var comparator in comparators)
        {
          if (!comparator.SyntaxEquals(element, information))
            goto SkipThisElement;
        }

        yield return element;
        SkipThisElement:
        ;
      }
    }
  }
}