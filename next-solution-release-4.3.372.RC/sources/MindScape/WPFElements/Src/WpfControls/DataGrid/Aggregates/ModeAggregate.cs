using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// An aggregate that calculates the mode (most occurring value) of a collection.
  /// </summary>
  public class ModeAggregate : AggregateBase
  {
    /// <summary>
    /// Calculates the mode (most occurring value) of the collection.
    /// </summary>
    /// <param name="items">The collection of items.</param>
    /// <returns>The mode (most occurring value) of the collection.</returns>
    protected override object CalculateCore(IEnumerable items)
    {
      Dictionary<object, int> _counts = new Dictionary<object, int>();
      int totalCount = 0;
      foreach (object item in items)
      {
        totalCount++;
        object value = GetValue(item);
        if (_counts.ContainsKey(value))
        {
          int count = _counts[value];
          _counts[value] = count + 1;
        }
        else
        {
          _counts[value] = 1;
        }
      }

      object mode = null;
      int modeCount = 0;
      foreach (object value in _counts.Keys)
      {
        int count = _counts[value];
        if (count > modeCount)
        {
          mode = value;
          modeCount = count;
        }
      }

      return new Mode(mode, modeCount / (double)totalCount);
    }
  }
}
