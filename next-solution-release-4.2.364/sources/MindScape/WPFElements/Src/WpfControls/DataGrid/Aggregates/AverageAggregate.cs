using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// An aggregate that calculates the average of all values in the collection.
  /// </summary>
  public class AverageAggregate : AggregateBase
  {
    /// <summary>
    /// Calculates the average of all the items in the collection.
    /// </summary>
    /// <param name="items">The collection of items.</param>
    /// <returns>The avergae of the items in the collection.</returns>
    protected override object CalculateCore(System.Collections.IEnumerable items)
    {
      double total = 0;
      int count = 0;
      foreach (object item in items)
      {
        count++;
        object value = GetValue(item);
        double? number = NumericalUtils.ConvertToDouble(value);
        if (number != null)
        {
          total += number.Value;
        }
      }
      return count == 0 ? 0 : total / (double)count;
    }
  }
}
