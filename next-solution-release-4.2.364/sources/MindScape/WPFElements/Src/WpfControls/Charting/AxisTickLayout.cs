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
  /// Specifies the layout of the tick marks along a <see cref="ChartAxis"/>.
  /// </summary>
  public enum AxisTickLayout
  {
    /// <summary>
    /// Tick marks are positioned from the start to the end of the <see cref="ChartAxis"/>.
    /// </summary>
    Normal,

    /// <summary>
    /// Tick marks start and end between a buffer placed at each end of the <see cref="ChartAxis"/>.
    /// </summary>
    Inside
  }
}
