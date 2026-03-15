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
using System.Collections.ObjectModel;
using System.Windows.Markup;

namespace Mindscape.WpfElements.Charting
{
  /// <summary>
  /// A type of <see cref="IAxisValueConverter"/> used for creating a DateTime axis scale.
  /// </summary>
  [ContentProperty("DateTimeIntervalDefinitions")]
  public class DateTimeAxisValueConverter : IAxisValueConverter, IAdvancedAxisValueConverter
  {
    private readonly ObservableCollection<DateTimeIntervalDefinition> _dateTimeIntervalDefinitions = new ObservableCollection<DateTimeIntervalDefinition>();

    /// <summary>
    /// Gets the logical axis position for a DateTime.
    /// </summary>
    /// <param name="o">The DateTime whose plot position is required.</param>
    /// <returns>The logical axis position of the given DateTime.</returns>
    public double GetAxisPlotPosition(object o)
    {
      if (o is DateTime)
      {
        DateTime date = (DateTime)o;
        date = date.RoundNearest(new TimeSpan(0, 0, 0, 0, 1));
        double ticks = date.Ticks;
        return ticks;
      }
      if (o == null)
      {
        return Double.NaN;
      }
      return 0;
    }

    /// <summary>
    /// Gets the DateTime corresponding to a logical axis position.
    /// </summary>
    /// <param name="axisPosition">The logical axis position.</param>
    /// <returns>The DateTime represented on the axis at that position.</returns>
    public object GetDataObjectAt(double axisPosition)
    {
      return new DateTime(Math.Max(0, (long)axisPosition)).RoundNearest(new TimeSpan(0, 0, 0, 0, 1));
    }

    /// <summary>
    /// Returns the next axis plot position starting from the given position.
    /// </summary>
    /// <param name="axisMinimum">The minimum axis value.</param>
    /// <param name="axisMaximum">The maximum axis value.</param>
    /// <param name="currentPlotPosition">The current axis plot position.</param>
    /// <returns>The next axis plot position.</returns>
    public double GetNextAxisPlotPosition(double axisMinimum, double axisMaximum, double currentPlotPosition)
    {
      int unitCount;
      DateTimeUnit unit;
      GetInterval(axisMinimum, axisMaximum, out unitCount, out unit);
      DateTime dateTime = new DateTime(Math.Max(0, (long)currentPlotPosition));
      switch (unit)
      {
        case DateTimeUnit.Year:
          dateTime = dateTime.AddYears(unitCount);
          break;
        case DateTimeUnit.Month:
          dateTime = dateTime.AddMonths(unitCount);
          break;
        case DateTimeUnit.Week:
          dateTime = dateTime.AddDays(7 * unitCount);
          break;
        case DateTimeUnit.Day:
          dateTime = dateTime.AddDays(unitCount);
          break;
        case DateTimeUnit.Hour:
          dateTime = dateTime.AddHours(unitCount);
          break;
        case DateTimeUnit.Minute:
          dateTime = dateTime.AddMinutes(unitCount);
          break;
        case DateTimeUnit.Second:
          dateTime = dateTime.AddSeconds(unitCount);
          break;
        case DateTimeUnit.Millisecond:
          dateTime = dateTime.AddMilliseconds(unitCount);
          break;
      }
      return dateTime.Ticks;
    }

    /// <summary>
    /// Returns the previous axis plot position starting from the given position.
    /// </summary>
    /// <param name="axisMinimum">The minimum axis value.</param>
    /// <param name="axisMaximum">The maximum axis value.</param>
    /// <param name="currentPlotPosition">The current axis plot position.</param>
    /// <returns>The previous axis plot position.</returns>
    public double GetPreviousAxisPlotPosition(double axisMinimum, double axisMaximum, double currentPlotPosition)
    {
      int unitCount;
      DateTimeUnit unit;
      GetInterval(axisMinimum, axisMaximum, out unitCount, out unit);
      DateTime dateTime = new DateTime(Math.Max(0, (long)currentPlotPosition));
      switch (unit)
      {
        case DateTimeUnit.Year:
          dateTime = dateTime.AddYears(-unitCount);
          break;
        case DateTimeUnit.Month:
          if (dateTime.Year > 1 || dateTime.Month > 1)
          {
            dateTime = dateTime.AddMonths(-unitCount);
          }
          else
          {
            dateTime = DateTime.MinValue;
          }
          break;
        case DateTimeUnit.Week:
          dateTime = dateTime.AddDays(7 * -unitCount);
          break;
        case DateTimeUnit.Day:
          dateTime = dateTime.AddDays(-unitCount);
          break;
        case DateTimeUnit.Hour:
          dateTime = dateTime.AddHours(-unitCount);
          break;
        case DateTimeUnit.Minute:
          if (dateTime > new DateTime(1, 1, 1, 0, 1, 0))
          {
            dateTime = dateTime.AddMinutes(-unitCount);
          }
          break;
        case DateTimeUnit.Second:
          if (dateTime > new DateTime(1, 1, 1, 0, 0, 1))
          {
            dateTime = dateTime.AddSeconds(-unitCount);
          }
          break;
        case DateTimeUnit.Millisecond:
          if (dateTime > new DateTime(1, 1, 1, 0, 0, 0, 1))
          {
            dateTime = dateTime.AddMilliseconds(-unitCount);
          }
          break;
      }
      return dateTime.Ticks;
    }

    /// <summary>
    /// Rounds the given axis plot position up to the start of the closest date time unit.
    /// </summary>
    /// <param name="axisMinimum">The minimum axis value.</param>
    /// <param name="axisMaximum">The maximum axis value.</param>
    /// <param name="currentPlotPosition">The current axis plot position to normalize.</param>
    /// <returns>The normalized axis plot position.</returns>
    public double NormalizeAxisPlotPosition(double axisMinimum, double axisMaximum, double currentPlotPosition)
    {
      int unitCount;
      DateTimeUnit unit;
      GetInterval(axisMinimum, axisMaximum, out unitCount, out unit);
      DateTime dateTime = new DateTime(Math.Max(0, (long)currentPlotPosition));
      switch (unit)
      {
        case DateTimeUnit.Year:
          int totalYears = dateTime.TotalYears();
          totalYears = Math.Max(1, totalYears - (totalYears % unitCount));
          dateTime = new DateTime(totalYears, 1, 1);
          break;
        case DateTimeUnit.Month:
          int totalMonths = dateTime.TotalMonths();
          totalMonths = totalMonths - (totalMonths % unitCount);
          dateTime = DateTimeUtils.DateTimeFromMonths(totalMonths); // new DateTime(dateTime.Year, dateTime.Month, 1);
          break;
        case DateTimeUnit.Week:
          dateTime = dateTime.StartOfWeek(DayOfWeek.Monday);
          dateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
          break;
        case DateTimeUnit.Day:
          int totalDays = dateTime.TotalDays();
          totalDays = totalDays - (totalDays % unitCount);
          dateTime = DateTimeUtils.DateTimeFromDays(totalDays); // new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
          break;
        case DateTimeUnit.Hour:
          int totalHours = dateTime.TotalHours();
          totalHours = totalHours - (totalHours % unitCount);
          dateTime = DateTimeUtils.DateTimeFromHours(totalHours); // new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 0, 0);
          break;
        case DateTimeUnit.Minute:
          int totalMinutes = dateTime.TotalMinutes();
          totalMinutes = totalMinutes - (totalMinutes % unitCount);
          dateTime = DateTimeUtils.DateTimeFromMinutes(totalMinutes); // new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0);
          break;
        case DateTimeUnit.Second:
          long totalSeconds = dateTime.TotalSeconds();
          totalSeconds = totalSeconds - (totalSeconds % unitCount);
          dateTime = DateTimeUtils.DateTimeFromSeconds(totalSeconds);
          break;
        case DateTimeUnit.Millisecond:
          long totalMilliseconds = dateTime.TotalMilliseconds();
          totalMilliseconds = totalMilliseconds - (totalMilliseconds % unitCount);
          dateTime = DateTimeUtils.DateTimeFromMilliseconds(totalMilliseconds);
          break;
      }
      return dateTime.Ticks;
    }

    /// <summary>
    /// Returns a format string for a <see cref="DateTime"/> within the given interval.
    /// </summary>
    /// <param name="axisMinimum">The minimum plot position of the axis.</param>
    /// <param name="axisMaximum">The maximum plot position of the axis.</param>
    /// <returns>A format string for a <see cref="DateTime"/> within the axis range.</returns>
    public string GetFormat(double axisMinimum, double axisMaximum)
    {
      DateTime startDate = new DateTime(Math.Max(0, (long)axisMinimum));
      DateTime endDate = new DateTime(Math.Max(0, (long)axisMaximum));
      TimeSpan range = endDate - startDate;

      DateTimeIntervalDefinition definition = GetIntervalDefinition(range);

      string format = "{0:d}";

      if (definition == null || definition.IntervalFormat == null)
      {
        int multiplier = 3;
        if (range > new TimeSpan(365 * multiplier, 0, 0, 0))
        {
          format = "{0:yyyy}";
        }
        else if (range > new TimeSpan(30 * multiplier, 0, 0, 0))
        {
          format = "{0:MMM:yyyy}";
        }
        else if (range > new TimeSpan(7 * multiplier, 0, 0, 0))
        {
          format = "{0:d}";
        }
        else if (range > new TimeSpan(1 * multiplier, 0, 0, 0))
        {
          format = "{0:d}";
        }
        else if (range > new TimeSpan(1 * multiplier, 0, 0))
        {
          format = "{0:t}";
        }
        else if (range > new TimeSpan(0, multiplier, 0))
        {
          format = "{0:t}";
        }
        else if (range > new TimeSpan(0, 0, multiplier))
        {
          format = "{0:T}";
        }
        else
        {
          format = "{0:hh:mm:ss:ffff}";
        }
      }
      else
      {
        format = definition.IntervalFormat;
      }

      return format;
    }

    private void GetInterval(double start, double end, out int unitCount, out DateTimeUnit unit)
    {
      DateTime startDate = new DateTime(Math.Max(0, (long)start));
      DateTime endDate = new DateTime(Math.Max(0, (long)end));
      TimeSpan range = endDate - startDate;

      DateTimeIntervalDefinition definition = GetIntervalDefinition(range);

      if (IntervalUnit == null)
      {
        if (definition != null)
        {
          unit = definition.IntervalUnit;
        }
        else
        {
          int multiplier = 3;
          if (range > new TimeSpan(365 * multiplier, 0, 0, 0))
          {
            unit = DateTimeUnit.Year;
          }
          else if (range > new TimeSpan(30 * multiplier, 0, 0, 0))
          {
            unit = DateTimeUnit.Month;
          }
          else if (range > new TimeSpan(7 * multiplier, 0, 0, 0))
          {
            unit = DateTimeUnit.Week;
          }
          else if (range > new TimeSpan(1 * multiplier, 0, 0, 0))
          {
            unit = DateTimeUnit.Day;
          }
          else if (range > new TimeSpan(1 * multiplier, 0, 0))
          {
            unit = DateTimeUnit.Hour;
          }
          else if (range > new TimeSpan(0, multiplier, 0))
          {
            unit = DateTimeUnit.Minute;
          }
          else if(range > new TimeSpan(0, 0, multiplier))
          {
            unit = DateTimeUnit.Second;
          }
          else
          {
            unit = DateTimeUnit.Millisecond;
          }
        }
      }
      else
      {
        unit = IntervalUnit.Value;
      }

      if (IntervalMagnitude <= 0)
      {
        if (definition != null && definition.InternalMagnitude >= 0)
        {
          unitCount = definition.InternalMagnitude;
        }
        else
        {
          TimeSpan approximateUnitLength;
          switch (unit)
          {
            case DateTimeUnit.Year:
              approximateUnitLength = new TimeSpan(365, 6, 0, 0);
              break;
            case DateTimeUnit.Month:
              approximateUnitLength = new TimeSpan(730, 30, 0);
              break;
            case DateTimeUnit.Week:
              approximateUnitLength = new TimeSpan(7, 0, 0, 0);
              break;
            case DateTimeUnit.Day:
              approximateUnitLength = new TimeSpan(1, 0, 0, 0);
              break;
            case DateTimeUnit.Hour:
              approximateUnitLength = new TimeSpan(1, 0, 0);
              break;
            case DateTimeUnit.Minute:
              approximateUnitLength = new TimeSpan(0, 1, 0);
              break;
            case DateTimeUnit.Second:
              approximateUnitLength = new TimeSpan(0, 0, 1);
              break;
            default:
              approximateUnitLength = new TimeSpan(0, 0, 0, 0, 1);
              break;
          }

          double approximateTotalUnitCount = (end - start) / approximateUnitLength.Ticks;
          unitCount = GetBestUnitCount(unit, approximateTotalUnitCount); //(int)Math.Max(1, approximateTotalUnitCount / 7.0);
        }
      }
      else
      {
        unitCount = IntervalMagnitude;
      }
    }

    private int GetBestUnitCount(DateTimeUnit unit, double approximateTotalUnitCount)
    {
      double unitCount = Math.Max(1, approximateTotalUnitCount / 7.0);
      int[] unitCounts = new[] { 1, 2, 5 };

      switch (unit)
      {
        case DateTimeUnit.Month:
          unitCounts = new[] { 1, 2, 3, 6 };
          break;
        case DateTimeUnit.Week:
          unitCounts = new[] { 1, 2 };
          break;
        case DateTimeUnit.Hour:
          unitCounts = new[] { 1, 2, 3, 4, 6, 12 };
          break;
        case DateTimeUnit.Minute:
        case DateTimeUnit.Second:
          unitCounts = new[] { 1, 2, 5, 10, 15, 30 };
          break;
        case DateTimeUnit.Millisecond:
          unitCounts = new[] { 1, 2, 5, 10, 20, 25, 100, 200, 500 };
          break;
      }

      for (int i = 1; i < unitCounts.Length; i++)
      {
        int low = unitCounts[i - 1];
        int high = unitCounts[i];
        double mid = (low + high) / 2.0;
        if (unitCount >= low && unitCount <= mid)
        {
          unitCount = low;
          break;
        }
        if (unitCount > mid && unitCount <= high)
        {
          unitCount = high;
          break;
        }
        if (i == unitCounts.Length - 1)
        {
          int count = unitCounts[unitCounts.Length - 1];
          unitCount = ((int)(unitCount / count)) * count;
        }
      }
      return (int)unitCount;
    }

    private DateTimeIntervalDefinition GetIntervalDefinition(TimeSpan currentTimeSpan)
    {
      foreach (DateTimeIntervalDefinition d in DateTimeIntervalDefinitions)
      {
        TimeSpan minimum = d.MinimumTimeSpan;
        TimeSpan maximum = d.MaximumTimeSpan;
        if (minimum > maximum)
        {
          TimeSpan temp = minimum;
          minimum = maximum;
          maximum = temp;
        }
        if (currentTimeSpan >= minimum && currentTimeSpan <= maximum)
        {
          return d;
        }
      }
      return null;
    }

    /// <summary>
    /// Gets or sets the <see cref="DateTimeUnit"/> to for calculating axis intervals.
    /// Setting this will force this converter to use the specified interval and override the zooming intervals.
    /// </summary>
    public DateTimeUnit? IntervalUnit { get; set; }

    /// <summary>
    /// Gets or sets the interval magnitude to use. If this value is 0, the interval magnitude will be calculated automatically.
    /// </summary>
    public int IntervalMagnitude { get; set; }

    /// <summary>
    /// Gets the collection of <see cref="DateTimeIntervalDefinition"/> objects used for customizing the date time interval zooming logic.
    /// </summary>
    public ObservableCollection<DateTimeIntervalDefinition> DateTimeIntervalDefinitions
    {
      get { return _dateTimeIntervalDefinitions; }
    }
  }
}
