using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// Implementations of this interface calculate various aggregate values for a list of objects.
  /// </summary>
  public interface IAggregate
  {
    /// <summary>
    /// Calculates the aggregate for the given collection of items.
    /// </summary>
    /// <param name="items">The collection of items.</param>
    /// <returns>The aggregate value.</returns>
    object Calculate(IEnumerable items);

    /// <summary>
    /// Gets the result of the aggregate.
    /// </summary>
    object Result { get; }
  }
}
