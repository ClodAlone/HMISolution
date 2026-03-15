using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Mindscape.WpfElements.Charting
{
  /// <summary>
  /// An <see cref="IChartDataSampler"/> that calculates an index interval based on the pixel dimensions of the viewport and a desired
  /// number of pixels between each data point.
  /// </summary>
  public class PixelDensitySampler : IChartDataSampler
  {
    /// <summary>
    /// Gets or sets the desired number of pixels between each rendered data point.
    /// </summary>
    public double PixelSpacing { get; set; }

    /// <summary>
    /// Gets or sets whether or not zoom adjustment is enabled. Zoom adjustment forces the index interval to be rounded to the
    /// nearest exponential of base 2 which allows data to look consistent at different zoom levels. The default is false.
    /// </summary>
    public bool IsZoomAdjustmentEnabled { get; set; }

    /// <summary>
    /// Calculates the index interval of the data sampling. The data series will skip this number of data points when rendering the chart.
    /// </summary>
    /// <param name="dataCount">The number of data points available to display in the current viewport.</param>
    /// <param name="canvasSize">The dimensions of the charting canvas.</param>
    /// <returns>The index interval of the data sampling.</returns>
    public int CalculateIndexStep(int dataCount, Size canvasSize)
    {
      if (PixelSpacing > 0)
      {
        double maxDataPointCount = canvasSize.Width / PixelSpacing;
        if (!Double.IsNaN(canvasSize.Width) && maxDataPointCount > 0)
        {
          double step = Math.Max(1, (dataCount) / maxDataPointCount);
          if (IsZoomAdjustmentEnabled)
          {
            int count = 0;
            while (step > 1)
            {
              step /= 2.0;
              count++;
            }
            return (int)Math.Pow(2, count);
          }
          return (int)step;
        }
      }
      FixedSampleCountSampler fallBack = new FixedSampleCountSampler() { MaxDataPointCount = 400 };
      return fallBack.CalculateIndexStep(dataCount, canvasSize);
    }
  }
}
