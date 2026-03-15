using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mindscape.WpfElements.Charting
{
  /// <summary>
  /// Holds information about the chart zoom event.
  /// </summary>
  public class ChartZoomEventArgs : EventArgs
  {
    internal ChartZoomEventArgs(double xZoom, double yZoom)
    {
      XZoom = xZoom;
      YZoom = yZoom;
      AverageZoom = (xZoom + yZoom) / 2.0;
    }

    /// <summary>
    /// Gets the current zoom percentage of the X axis. This will be a value between 0 and 1.
    /// </summary>
    public double XZoom { get; private set; }

    /// <summary>
    /// Gets the current zoom percentage of the Y axis. This will be a value between 0 and 1.
    /// </summary>
    public double YZoom { get; private set; }

    /// <summary>
    /// Gets the current average zoom percentage of the chart control. This will be a value between 0 and 1.
    /// </summary>
    public double AverageZoom { get; private set; }
  }
}
