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
  /// Represents a data point of a data series involving plotted points.
  /// </summary>
  public class ChartSymbol : CartesianDataPoint
  {
    static ChartSymbol()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(ChartSymbol),
        new FrameworkPropertyMetadata(typeof(ChartSymbol)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ChartSymbol"/> class.
    /// </summary>
    public ChartSymbol() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ChartSymbol"/> class.
    /// </summary>
    /// <param name="data">The data object that the <see cref="ChartSymbol"/> plots.</param>
    internal ChartSymbol(object data)
    {
      DataContext = data;
    }
  }
}
