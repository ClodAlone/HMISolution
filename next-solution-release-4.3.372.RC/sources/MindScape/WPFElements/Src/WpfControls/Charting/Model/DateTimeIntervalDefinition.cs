using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Mindscape.WpfElements.Charting
{
  /// <summary>
  /// Defines a date time interval to be used within the specified time spans.
  /// </summary>
  public class DateTimeIntervalDefinition
  {
    private TimeSpan _minimumTimeSpan;
    private TimeSpan _maximumTimeSpan;
    private DateTimeUnit _intervalUnit;
    private int _intervalMagnitude;
    private string _intervalFormat;

    /// <summary>
    /// Initializes a new instance of the <see cref="DateTimeIntervalDefinition"/> class.
    /// </summary>
    public DateTimeIntervalDefinition()
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="DateTimeIntervalDefinition"/> class.
    /// </summary>
    /// <param name="minimumTimeSpan">The minimim <see cref="TimeSpan"/> that can use the date time interval.</param>
    /// <param name="maximumTimeSpan">The maximum <see cref="TimeSpan"/> that can use the date time interval.</param>
    /// <param name="intervalUnit">The <see cref="DateTimeUnit"/> of the interval.</param>
    /// <param name="intervalMagnitude">The number of date time units that make up the interval.</param>
    public DateTimeIntervalDefinition(TimeSpan minimumTimeSpan, TimeSpan maximumTimeSpan, DateTimeUnit intervalUnit, int intervalMagnitude)
    {
      MinimumTimeSpan = minimumTimeSpan;
      MaximumTimeSpan = maximumTimeSpan;
      IntervalUnit = intervalUnit;
      InternalMagnitude = intervalMagnitude;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DateTimeIntervalDefinition"/> class.
    /// </summary>
    /// <param name="minimumTimeSpan">The minimim <see cref="TimeSpan"/> that can use the date time interval.</param>
    /// <param name="maximumTimeSpan">The maximum <see cref="TimeSpan"/> that can use the date time interval.</param>
    /// <param name="intervalUnit">The <see cref="DateTimeUnit"/> of the interval.</param>
    /// <param name="intervalMagnitude">The number of date time units that make up the interval.</param>
    /// <param name="intervalFormat">The string format used to display <see cref="DateTime"/> </param>
    public DateTimeIntervalDefinition(TimeSpan minimumTimeSpan, TimeSpan maximumTimeSpan, DateTimeUnit intervalUnit, int intervalMagnitude, string intervalFormat)
    {
      MinimumTimeSpan = minimumTimeSpan;
      MaximumTimeSpan = maximumTimeSpan;
      IntervalUnit = intervalUnit;
      InternalMagnitude = intervalMagnitude;
      IntervalFormat = intervalFormat;
    }

    /// <summary>
    /// Gets or sets the minimum <see cref="TimeSpan"/> that can use the date time interval.
    /// </summary>
    [TypeConverter(typeof(TimeSpanConverter))]
    public TimeSpan MinimumTimeSpan
    {
      get { return _minimumTimeSpan; }
      set
      {
        _minimumTimeSpan = value;
      }
    }

    /// <summary>
    /// gets or sets the maximum <see cref="TimeSpan"/> that can use the date time interval.
    /// </summary>
    [TypeConverter(typeof(TimeSpanConverter))]
    public TimeSpan MaximumTimeSpan
    {
      get { return _maximumTimeSpan; }
      set
      {
        _maximumTimeSpan = value;
      }
    }

    /// <summary>
    /// Gets or sets the <see cref="DateTimeUnit"/> of the interval.
    /// </summary>
    public DateTimeUnit IntervalUnit
    {
      get { return _intervalUnit; }
      set
      {
        _intervalUnit = value;
      }
    }

    /// <summary>
    /// Gets or sets the number of date time units that make up the interval.
    /// </summary>
    public int InternalMagnitude
    {
      get { return _intervalMagnitude; }
      set
      {
        _intervalMagnitude = value;
      }
    }

    /// <summary>
    /// Gets or sets the string format used to display <see cref="DateTime"/> values when using this interval.
    /// </summary>
    public string IntervalFormat
    {
      get { return _intervalFormat; }
      set
      {
        _intervalFormat = value;
      }
    }
  }
}
