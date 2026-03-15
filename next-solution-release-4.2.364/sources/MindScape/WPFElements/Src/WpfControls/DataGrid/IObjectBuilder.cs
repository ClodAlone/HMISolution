using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// Provides a way to build a data object for a row in datagrid. This is useful for building objects that do not have public default constructors.
  /// </summary>
  public interface IObjectBuilder
  {
    /// <summary>
    /// Builds an object.
    /// </summary>
    /// <returns>An object.</returns>
    object Build();
  }
}
