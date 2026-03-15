using System;
using System.Windows.Data;
using System.Windows;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Converts a pair of dates to a boolean value indicating whether they are in the same month.
  /// </summary>
  public class SameMonthConverter : IMultiValueConverter
  {
    /// <summary>
    /// Converts a value from a binding source for use by a binding target.
    /// </summary>
    /// <param name="values">The dates to check.  This array must contain exactly two dates.  If
    /// the array contains fewer than two dates, an <see cref="IndexOutOfRangeException"/> occurs.
    /// If the array contains more than two dates, the extra dates are ignored.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>true if the dates are in the same month; otherwise false.</returns>
    public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (values[0] == DependencyProperty.UnsetValue || values[1] == DependencyProperty.UnsetValue)
      {
        return false;
      }

      try
      {
        DateTime date1 = (DateTime)(values[0]);
        DateTime date2 = (DateTime)(values[1]);
        return DateTimeUtils.SameMonth(date1, date2);
      }
      catch (InvalidCastException)
      {
        return false;
      }
    }

    /// <summary>
    /// Converts a value from a binding target for writing to a binding source.
    /// </summary>
    /// <param name="value">The value produced by the binding target.</param>
    /// <param name="targetTypes">The types to convert to.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>This conversion direction is not implemented by this converter.</returns>
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
