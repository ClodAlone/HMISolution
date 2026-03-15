using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mindscape.WpfElements.Charting
{
  /// <summary>
  /// Represents a data object containing a <see cref="DateTime"/> and a double.
  /// </summary>
  public class DateTimeDouble : ViewModelBase
  {
    private DateTime _dateTime;
    private double _double;

    /// <summary>
    /// Initializes a new instance of the <see cref="DateTimeDouble"/> class.
    /// </summary>
    /// <param name="dateTime">The <see cref="DateTime"/> value.</param>
    /// <param name="d">The double value.</param>
    public DateTimeDouble(DateTime dateTime, double d)
    {
      DateTime = dateTime;
      Double = d;
    }

    /// <summary>
    /// Gets or sets the <see cref="DateTime"/> value.
    /// </summary>
    public DateTime DateTime
    {
      get { return _dateTime; }
      set { Set<DateTime>(ref _dateTime, value, "DateTime"); }
    }

    /// <summary>
    /// Gets or sets the double value.
    /// </summary>
    public double Double
    {
      get { return _double; }
      set { Set<double>(ref _double, value, "Double"); }
    }
  }
}
