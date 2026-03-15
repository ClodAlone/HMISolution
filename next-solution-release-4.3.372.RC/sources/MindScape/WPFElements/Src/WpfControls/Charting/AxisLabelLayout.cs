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
  /// Specifies how to position axis labels along a <see cref="ChartAxis"/>.
  /// </summary>
  public enum AxisLabelLayout
  {
    /// <summary>
    /// Axis labels are positioned from the start to the end of the <see cref="ChartAxis"/>.
    /// </summary>
    Normal,

    /// <summary>
    /// Axis labels start and end between a buffer placed at each end of the <see cref="ChartAxis"/>. This is useful for bar charts.
    /// </summary>
    Inside
  }
}
