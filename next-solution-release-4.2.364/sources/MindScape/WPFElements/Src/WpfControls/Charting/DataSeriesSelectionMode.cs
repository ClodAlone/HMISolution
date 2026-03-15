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
  /// Specifies the selection mode of <see cref="DataSeries"/> within a <see cref="Chart"/>.
  /// </summary>
  public enum DataSeriesSelectionMode
  {
    /// <summary>
    /// Only <see cref="DataPoint"/> objects from a single <see cref="DataSeries"/> can be selected at a time.
    /// Selecting a <see cref="DataPoint"/> within a <see cref="DataSeries"/> will deselect any data points in the
    /// previously selected <see cref="DataSeries"/>.
    /// </summary>
    Single,

    /// <summary>
    /// Any number of <see cref="DataSeries"/> can have selected <see cref="DataPoint"/> objects.
    /// </summary>
    Multiple
  }
}
