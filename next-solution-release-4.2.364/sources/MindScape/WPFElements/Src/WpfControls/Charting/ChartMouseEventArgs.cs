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
  /// Holds information about mouse events that occur on a chart canvas.
  /// </summary>
  public class ChartMouseEventArgs : EventArgs
  {
    internal ChartMouseEventArgs(Point physical, Point logical, Point constrainedLogicalPoint, bool isMouseDown, bool isMouseOver)
    {
      PhysicalPoint = physical;
      LogicalPoint = logical;
      ConstrainedLogicalPoint = constrainedLogicalPoint;
      IsMouseDown = isMouseDown;
      IsMouseOver = isMouseOver;
    }

    /// <summary>
    /// Gets the physical position of the mouse pointer over the chart canvas.
    /// </summary>
    public Point PhysicalPoint { get; private set; }

    /// <summary>
    /// Gets the logical position of the mouse pointer over the chart canvas.
    /// </summary>
    public Point LogicalPoint { get; private set; }

    /// <summary>
    /// Gets the logical postion of the mouse pointer constrained by the dimensions of the chart canvas.
    /// </summary>
    public Point ConstrainedLogicalPoint { get; private set; }

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
