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
  /// Holds information about mouse events that occur on a polar chart canvas.
  /// </summary>
  public class PolarChartMouseEventArgs : EventArgs
  {
    internal PolarChartMouseEventArgs(Point physical, PolarPoint physicalPolar, PolarPoint logical, PolarPoint constrainedLogicalPoint, bool isMouseDown, bool isMouseOver)
    {
      PhysicalPoint = physical;
      PhysicalPolarPoint = physicalPolar;
      LogicalPoint = logical;
      ConstrainedLogicalPoint = constrainedLogicalPoint;
      IsMouseDown = isMouseDown;
      IsMouseOver = isMouseOver;
    }

    /// <summary>
    /// Gets the physical position of the mouse pointer over the polar chart canvas.
    /// This point is relative to the top left corner of the canvas.
    /// </summary>
    public Point PhysicalPoint { get; private set; }

    /// <summary>
    /// Gets a <see cref="PolarPoint"/> containing the actual angle and distance of the mouse point from the center of the polar chart.
    /// </summary>
    public PolarPoint PhysicalPolarPoint { get; private set; }

    /// <summary>
    /// Gets the logical position of the mouse pointer over the polar chart canvas.
    /// The logical values are based on the range of the axes.
    /// </summary>
    public PolarPoint LogicalPoint { get; private set; }

    /// <summary>
    /// Gets the logical postion of the mouse pointer constrained by the range of the axes.
    /// </summary>
    public PolarPoint ConstrainedLogicalPoint { get; private set; }

    /// <summary>
    /// Gets whether or not the left mouse button is down.
    /// </summary>
    public bool IsMouseDown { get; private set; }

    /// <summary>
    /// Gets whether or not the mouse pointer is within the bounds of the chart canvas.
    /// </summary>
    public bool IsMouseOver { get; private set; }
  }
}
