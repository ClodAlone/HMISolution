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
  /// Represents a data point of a polar data series involving plotted points.
  /// </summary>
  public class PolarChartSymbol : PolarDataPoint
  {
    static PolarChartSymbol()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(PolarChartSymbol),
        new FrameworkPropertyMetadata(typeof(PolarChartSymbol)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PolarChartSymbol"/> class.
    /// </summary>
    public PolarChartSymbol() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="PolarChartSymbol"/> class.
    /// </summary>
    /// <param name="data">The data object that the <see cref="PolarChartSymbol"/> plots.</param>
    internal PolarChartSymbol(object data)
    {
      DataContext = data;
    }
  }
}
