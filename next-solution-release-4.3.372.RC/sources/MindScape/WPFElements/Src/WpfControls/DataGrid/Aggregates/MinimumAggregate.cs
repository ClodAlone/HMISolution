using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// An aggregate the finds the smallest value in the collection.
  /// </summary>
  public class MinimumAggregate : AggregateBase
  {
    /// <summary>
    /// Finds the smallest value in the collection.
    /// </summary>
    /// <param name="items">The collection of items.</param>
    /// <returns>The smallest value in the collection.</returns>
    protected override object CalculateCore(IEnumerable items)
    {
      IComparable min = null;
      foreach (object item in items)
      {
        object value = GetValue(item);
        if (value is IComparable)
        {
          IComparable comparable = (IComparable)value;
          if (comparable != null)
          {
            if (min == null)
            {
              min = comparable;
            }
            if (min.GetType() == comparable.GetType())
            {
              if (comparable.CompareTo(min) < 0)
              {
                min = comparable;
              }
            }
          }
        }
      }
      return min;
    }
  }
}
