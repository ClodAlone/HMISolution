using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// Provides a way to build IFilter objects.
  /// </summary>
  public interface IFilterBuilder
  {
    /// <summary>
    /// Gets the display name of the filter.
    /// </summary>
    string Name { get; }
  }
}
