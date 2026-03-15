using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.Primitives;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// A popup for data grid filtering.
  /// </summary>
  public class FilterPopup : Popup
  {
    /// <summary>
    /// Called when a mouse button is pressed over the <see cref="FilterPopup"/>.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnMouseDown(System.Windows.Input.MouseButtonEventArgs e)
    {
      e.Handled = true;
    }
  }
}
