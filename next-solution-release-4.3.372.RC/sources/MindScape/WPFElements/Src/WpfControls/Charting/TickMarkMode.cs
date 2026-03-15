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
  /// Specifies how tick marks respond to panning the chart canvas.
  /// </summary>
  public enum TickMarkMode
  {
    /// <summary>
    /// Tick marks are stationary. The first tick mark will always start at the very start of the axis and
    /// adopt whatever axis value they land on.
    /// </summary>
    Stationary,

    /// <summary>
    /// Tick marks will slide as the chart canvas is panned. This will allow tick marks to be lined up with number series such as
    /// 0,1,2,3... or 0,0.5,1,1.5... or 0,10,20,30...
    /// </summary>
    Movable
  }
}
