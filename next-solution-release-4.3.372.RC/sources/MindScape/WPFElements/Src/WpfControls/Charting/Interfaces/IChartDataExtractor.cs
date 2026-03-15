using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mindscape.WpfElements.Charting
{
  /// <summary>
  /// Provides logic for extracting X and Y values from custom data point model objects. This is a high performance alternative to using XBinding and YBinding.
  /// </summary>
  public interface IChartDataExtractor
  {
    /// <summary>
    /// Gets the X object from the given data point model object.
    /// </summary>
    /// <param name="o">The data point model object.</param>
    /// <returns>The X object.</returns>
    object GetX(object o);

    /// <summary>
    /// Gets the Y object from the given data point model object.
    /// </summary>
    /// <param name="o">The data point model object.</param>
    /// <returns>The Y object.</returns>
    object GetY(object o);
  }
}
