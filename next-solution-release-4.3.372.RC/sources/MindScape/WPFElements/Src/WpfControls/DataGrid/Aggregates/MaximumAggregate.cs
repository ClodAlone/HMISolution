using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// An aggregate that finds the largest value in the collection.
  /// </summary>
  public class MaximumAggregate : AggregateBase
  {
    /// <summary>
    /// Finds the largest value in the collection.
    /// </summary>
    /// <param name="items">The collection of items.</param>
    /// <returns>The largest value in the collection</returns>
    protected override object CalculateCore(IEnumerable items)
    {
      IComparable max = null;
      foreach (object item in items)
      {
        object value = GetValue(item);
        if (value is IComparable)
        {
          IComparable comparable = (IComparable)value;
          if (comparable != null)
          {
            if (max == null)
            {
              max = comparable;
            }
            if (max.GetType() == comparable.GetType())
            {
              if (comparable.CompareTo(max) > 0)
              {
                max = comparable;
              }
            }
          }
        }
      }
      return max;
    }
  }
}
