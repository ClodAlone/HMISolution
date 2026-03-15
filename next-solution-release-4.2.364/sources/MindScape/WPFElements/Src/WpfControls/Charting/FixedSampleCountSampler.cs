using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Mindscape.WpfElements.Charting
{
  /// <summary>
  /// An <see cref="IChartDataSampler"/> that calculates an index interval based on a desired maximum number of data points
  /// to render.
  /// </summary>
  public class FixedSampleCountSampler : IChartDataSampler
  {
    /// <summary>
    /// Gets or sets the maximum number of data points that this <see cref="FixedSampleCountSampler"/> will allow to be rendered.
    /// </summary>
    public int MaxDataPointCount { get; set; }

    /// <summary>
    /// Calculates the index interval of the data sampling. The data series will skip this number of data points when rendering the chart.
    /// </summary>
    /// <param name="dataCount">The number of data points available to display in the current viewport.</param>
    /// <param name="canvasSize">The dimensions of the charting canvas.</param>
    /// <returns>The index interval of the data sampling.</returns>
    public int CalculateIndexStep(int dataCount, Size canvasSize)
    {
      if (MaxDataPointCount > 0 && dataCount > MaxDataPointCount)
      {
        double step = Math.Max(1, (dataCount) / MaxDataPointCount);
        int count = 0;
        while (step > 1)
        {
          step /= 2.0;
          count++;
        }
        return (int)Math.Pow(2, count);
      }
      return 1;
    }
  }
}
