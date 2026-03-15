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
  /// Converts between data values and logical axis positions to create custom axis scales.
  /// </summary>
  public interface IAxisValueConverter
  {
    /// <summary>
    /// Gets the logical axis plot position for a data object.
    /// </summary>
    /// <param name="o">The data object whose plot position is required.</param>
    /// <returns>The logical axis position of the given data object.</returns>
    double GetAxisPlotPosition(object o);

    /// <summary>
    /// Gets the data object corresponding to a logical axis position.
    /// </summary>
    /// <param name="axisPosition">The logical axis position.</param>
    /// <returns>The object represented on the axis at that position.</returns>
    object GetDataObjectAt(double axisPosition);
  }
}
