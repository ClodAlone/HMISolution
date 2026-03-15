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

namespace Mindscape.WpfElements.Charting
{
  /// <summary>
  /// A type of <see cref="IAxisValueConverter"/> used to create a month axis scale.
  /// </summary>
  public class MonthAxisValueConverter : IAxisValueConverter
  {
    /// <summary>
    /// Gets the logical axis position for a DateTime based on the year and month only.
    /// </summary>
    /// <param name="o">The DateTime whose plot position is required.</param>
    /// <returns>The logical axis position of the given DateTime.</returns>
    public double GetAxisPlotPosition(object o)
    {
      if (o is DateTime)
      {
        DateTime date = (DateTime)o;
        return (date.Year - 1) * 12 + date.Month - 1;
      }
      return 0;
    }

    /// <summary>
    /// Gets the date corresponding to a logical axis position.
    /// </summary>
    /// <param name="axisPosition">The logical axis position.</param>
    /// <returns>A DateTime representing the month on the axis at that position.</returns>
    public object GetDataObjectAt(double axisPosition)
    {
      int year = (int)(axisPosition / 12) + 1;
      int month = (int)(axisPosition % 12) + 1;
      if (year <= 0 || month <= 0 || month > 12)
      {
        return DateTime.MinValue;
      }
      return new DateTime(year, month, 1);
    }
  }
}
