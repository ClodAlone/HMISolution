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

#if SILVERLIGHT
namespace Mindscape.SilverlightElements
#else
namespace Mindscape.WpfElements
#endif
{
  /// <summary>
  /// Provides display information for a date range.
  /// </summary>
  public struct DateRangeDisplayInfo
  {
    private readonly DateTime _startDate;
    private readonly DateTime _endDate;
    private readonly DateDisplayMode _displayMode;

    /// <summary>
    /// Initializes a new instance of the <see cref="DateRangeDisplayInfo"/> struct.
    /// </summary>
    /// <param name="startDate">The start date of the range.</param>
    /// <param name="endDate">The end date of the range.</param>
    /// <param name="displayMode">The display mode.</param>
    public DateRangeDisplayInfo(DateTime startDate, DateTime endDate, DateDisplayMode displayMode)
    {
      _startDate = startDate;
      _endDate = endDate;
      _displayMode = displayMode;
    }

    /// <summary>
    /// Gets the start of the date range.
    /// </summary>
    public DateTime StartDate
    {
      get { return _startDate; }
    }

    /// <summary>
    /// Gets the end of the date range.
    /// </summary>
    public DateTime EndDate
    {
      get { return _endDate; }
    }

    /// <summary>
    /// Gets whether the date range should be formatted as days or as a month name.
    /// </summary>
    public DateDisplayMode DisplayMode
    {
      get { return _displayMode; }
    }

    /// <summary>
    /// Gets whether the range represents a single date.
    /// </summary>
    public bool IsSingleDate
    {
      get
      {
        return _startDate.Date == _endDate.Date
          || (_startDate.TimeOfDay == TimeSpan.Zero && _endDate.TimeOfDay == TimeSpan.Zero && (_endDate.AddSeconds(-1).Date == _startDate.Date));
      }
    }
  }
}
