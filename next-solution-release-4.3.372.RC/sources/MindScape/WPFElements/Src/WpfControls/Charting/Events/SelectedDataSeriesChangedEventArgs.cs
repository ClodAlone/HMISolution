using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mindscape.WpfElements.Charting
{
  /// <summary>
  /// Holds information about the DataSeries that was selected
  /// </summary>
  public class SelectedDataSeriesChangeddEventArgs : EventArgs
  {
    /// <summary>
    /// Gets the selected <see cref="DataSeries"/>.
    /// </summary>
    public DataSeries SelectedDataSeries { get; private set; }

    /// <summary>
    /// Gets the previously selected <see cref="DataSeries"/>.
    /// </summary>
    public DataSeries PreviousSelectedDataSeries { get; private set; }

    internal SelectedDataSeriesChangeddEventArgs(DataSeries selectedDataSeries, DataSeries previousSelectedDataSeries)
    {
      SelectedDataSeries = selectedDataSeries;
      PreviousSelectedDataSeries = previousSelectedDataSeries;
    }
  }
}
