using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Mindscape.WpfElements.Charting
{
  /// <summary>
  /// Contains commands used with the <see cref="Chart"/> control.
  /// </summary>
  public static class ChartCommands
  {
    /// <summary>
    /// A command for resetting the zoom level of a chart control back to 100 percent.
    /// </summary>
    public static readonly RoutedCommand ResetZoom = new RoutedCommand("ResetZoom", typeof(Chart));
  }
}
