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
using System.Windows.Data;
using System.Threading;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Formats a binding source value using a format string.
  /// </summary>
  public class FormattingConverter : IValueConverter
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="FormattingConverter"/> class.
    /// </summary>
    public FormattingConverter()
    {
      FormatString = "{0:0.0}";
    }

    /// <summary>
    /// Gets or sets the format string.
    /// </summary>
    public string FormatString { get; set; }

    /// <summary>
    /// Formats the binding source value using a format string.
    /// </summary>
    /// <param name="value">The object value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>The formatted object string.</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      try
      {
        string str = String.Format(Thread.CurrentThread.CurrentCulture, FormatString, value);
        return str;
      }
      catch (Exception)
      {
      }
      return value.ToString();
    }

    /// <summary>
    /// Converts a value from a binding target for writing to a binding source.
    /// </summary>
    /// <param name="value">The value produced by the binding target.</param>
    /// <param name="targetType">The type to convert to.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>This conversion direction is not implemented by this converter.</returns>
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
