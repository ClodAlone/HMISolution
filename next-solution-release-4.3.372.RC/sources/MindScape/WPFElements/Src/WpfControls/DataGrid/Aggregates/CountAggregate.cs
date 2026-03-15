using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// An aggregate that calculates the number of items in the collection.
  /// </summary>
  public class CountAggregate : AggregateBase
  {
    /// <summary>
    /// Calculates the number of items in the collection.
    /// </summary>
    /// <param name="items">The collection of items.</param>
    /// <returns>The number of items in the collection.</returns>
    protected override object CalculateCore(IEnumerable items)
    {
      int count = 0;
      foreach (object item in items)
      {
        count++;
      }
      return count;
    }
  }
}
