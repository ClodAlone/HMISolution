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
  /// Selects a brush according to whether a minutes value is on the hour or not.
  /// </summary>
  public class MinuteOfHourBrushConverter : IValueConverter
  {
    private Brush _onTheHourBrush = new SolidColorBrush(Color.FromArgb(255, 0, 255, 0));
    private Brush _defaultBrush = new SolidColorBrush(Color.FromArgb(255, 0, 0, 255));

    /// <summary>
    /// Gets or sets the brush to use if the minutes value is 0.
    /// </summary>
    public Brush OnTheHourBrush
    {
      get { return _onTheHourBrush; }
      set { _onTheHourBrush = value; }
    }

    /// <summary>
    /// Gets or sets the brush to use if the minutes value is not 0.
    /// </summary>
    public Brush DefaultBrush
    {
      get { return _defaultBrush; }
      set { _defaultBrush = value; }
    }

    /// <summary>
    /// Selects a brush according to whether the input value is on the hour or not.
    /// </summary>
    /// <param name="value">The integer value, representing a number of minutes, produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>The selected brush.</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      int minutes = (int)value;
      return minutes != 0 ? OnTheHourBrush : DefaultBrush;
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
