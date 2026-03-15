using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// Provides information about how to build a filter for a data type.
  /// </summary>
  public interface IFilterDescription
  {
    /// <summary>
    /// Gets the current filter created by this filter description.
    /// </summary>
    IFilter Filter { get; }

    /// <summary>
    /// Raised when the <see cref="Filter"/> changes.
    /// </summary>
    event EventHandler FilterChanged;

    /// <summary>
    /// Sets the filter description to best match the given <see cref="IFilter"/>.
    /// </summary>
    /// <param name="filter">The <see cref="IFilter"/> to mimic.</param>
    void SetAs(IFilter filter);
  }
}
