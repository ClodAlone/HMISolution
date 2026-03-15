using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// An aggregate that calculates the sum of all values in the collection.
  /// </summary>
  public class SumAggregate : AggregateBase
  {
    /// <summary>
    /// Calculates the sum of the items in the collection.
    /// </summary>
    /// <param name="items">The collection of items.</param>
    /// <returns>The sum of the items.</returns>
    protected override object CalculateCore(IEnumerable items)
    {
      double total = 0;
      foreach (object item in items)
      {
        object value = GetValue(item);
        double? number = NumericalUtils.ConvertToDouble(value);
        if (number != null)
        {
          total += number.Value;
        }
      }
      return total;
    }
  }
}
