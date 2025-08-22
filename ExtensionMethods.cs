using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

public static class ExtensionMethods
{
  public static int Remove<T>(
    this ObservableCollection<T> coll, Func<T, bool> condition)
  {
    var itemsToRemove = coll.Where(condition).ToList();

    foreach (var itemToRemove in itemsToRemove) coll.Remove(itemToRemove);

    return itemsToRemove.Count;
  }
}

public static class LinqExtensions
{
  public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> linqResult)
  {
    return new ObservableCollection<T>(linqResult);
  }
}