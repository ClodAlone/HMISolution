using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Mindscape.WpfElements.Charting
{
  /// <summary>
  /// Specifies the mouse mode of a <see cref="Chart"/>.
  /// </summary>
  public enum ChartMouseMode
  {
    /// <summary>
    /// The mouse is used for zooming.
    /// </summary>
    Zoom,

    /// <summary>
    /// The mouse is used for panning the chart.
    /// </summary>
    Pan,

    /// <summary>
    /// The mouse is pressed over the chart, but is not interacting with the chart.
    /// </summary>
    None
  }
}
