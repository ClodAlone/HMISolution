using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Mindscape.WpfElements.Charting
{
  /// <summary>
  /// Implementations of this interface can be used for providing data sampling logic for a <see cref="DataSeries"/>.
  /// </summary>
  public interface IChartDataSampler
  {
    /// <summary>
    /// Calculates the index interval of the data sampling. The data series will skip this number of data points when rendering the chart.
    /// </summary>
    /// <param name="dataCount">The number of data points available to display in the current viewport.</param>
    /// <param name="canvasSize">The dimensions of the charting canvas.</param>
    /// <returns>The index interval of the data sampling.</returns>
    int CalculateIndexStep(int dataCount, Size canvasSize);
  }
}
