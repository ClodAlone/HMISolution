using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mindscape.WpfElements.Charting
{
  /// <summary>
  /// A data point made up of open-high-low-close values used by stock charts.
  /// </summary>
  public class StockDataPoint : ViewModelBase
  {
    private double _low;
    private double _high;
    private double _open;
    private double _close;
    private DateTime _date;

    // TODO: this class should probably have constraints such as open and close values must be within low and high values etc.

    /// <summary>
    /// Initializes a new instance of the <see cref="StockDataPoint"/> class.
    /// </summary>
    /// <param name="date">The <see cref="DateTime"/> value.</param>
    /// <param name="open">The open value.</param>
    /// <param name="high">The high value.</param>
    /// <param name="low">The low value.</param>
    /// <param name="close">The close value.</param>
    public StockDataPoint(DateTime date, double open, double high, double low, double close)
    {
      Low = low;
      High = high;
      Open = open;
      Close = close;
      DateTime = date;
    }

    /// <summary>
    /// Gets or sets the low value.
    /// </summary>
    public double Low
    {
      get { return _low; }
      set { Set<double>(ref _low, value, "Low"); }
    }

    /// <summary>
    /// Gets or sets the high value.
    /// </summary>
    public double High
    {
      get { return _high; }
      set { Set<double>(ref _high, value, "High"); }
    }

    /// <summary>
    /// Gets or sets the open value.
    /// </summary>
    public double Open
    {
      get { return _open; }
      set { Set<double>(ref _open, value, "Open"); }
    }

    /// <summary>
    /// Gets or sets the close value.
    /// </summary>
    public double Close
    {
      get { return _close; }
      set { Set<double>(ref _close, value, "Close"); }
    }

    /// <summary>
    /// Gets or sets the <see cref="DateTime"/> value.
    /// </summary>
    public DateTime DateTime
    {
      get { return _date; }
      set { Set<DateTime>(ref _date, value, "Date"); }
    }
  }
}
