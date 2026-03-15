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

#if SILVERLIGHT
namespace Mindscape.SilverlightElements
#else
namespace Mindscape.WpfElements
#endif
{
  /// <summary>
  /// Converts a Boolean value to a visibility.
  /// </summary>
  public class BooleanToVisibilityConverter : IValueConverter
  {
    private Visibility _invisibility = Visibility.Collapsed;

    /// <summary>
    /// Gets or sets whether the normal visibility logic should be inverted.
    /// If IsInverted is false (the default), true is converted to Visible and false to Collapsed.
    /// If IsInverted is true, true is converted to Collapsed and false to Visible.
    /// </summary>
    public bool IsInverted { get; set; }

    /// <summary>
    /// Gets or sets what value to use for invisibility, either Hidden or Collapsed.
    /// </summary>
    public Visibility Invisibility
    {
      get { return _invisibility; }
      set { _invisibility = value; }
    }

    /// <summary>
    /// Converts a Boolean value from a binding source for use as a Visibility by a binding target.
    /// </summary>
    /// <param name="value">The value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>A value suitable for use by the binding target.</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      bool b = (bool)value ^ IsInverted;
      return b ? Visibility.Visible : Invisibility;
    }

    /// <summary>
    /// Converts a Visibility value from a binding target for writing to a Boolean binding source.
    /// </summary>
    /// <param name="value">The value produced by the binding target.</param>
    /// <param name="targetType">The type to convert to.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>True if the given value is Visible. False if the given value is Collapsed or Hidden.</returns>
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      Visibility visibility = (Visibility)value;
      if (IsInverted)
      {
        return visibility != Visibility.Visible;
      }
      return visibility == Visibility.Visible;
    }
  }
}
