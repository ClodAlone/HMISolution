using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Data;
using Mindscape.WpfElements.PropertyEditing;

namespace Mindscape.WpfElements.WpfPropertyGrid
{
  // TODO: should this be in the PropertyEditing namespace? The DataGrid doesn't use it.

  /// <summary>
  /// Contains standard property grouping strategies.
  /// </summary>
  public static class PropertyGrouping
  {
    private static GroupDescription _byCategoryGroupDescription = 
      new PropertyGroupDescription("Node", new NodeToCategoryConverter(), StringComparison.CurrentCultureIgnoreCase);

    /// <summary>
    /// Gets a GroupDescription which groups properties by their Category attribute.
    /// </summary>
    public static GroupDescription ByCategory
    {
      get { return _byCategoryGroupDescription; }
    }
  }
}
