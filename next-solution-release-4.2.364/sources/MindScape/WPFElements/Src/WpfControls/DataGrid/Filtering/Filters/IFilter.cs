using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// Provides filtering logic.
  /// </summary>
  public interface IFilter
  {
    /// <summary>
    /// Returns whether or not the given object matches the filtering logic.
    /// </summary>
    /// <param name="o">The object to check.</param>
    /// <returns>Whether or not the given object passes the filtering logic.</returns>
    bool IsMatch(object o);
  }
}
