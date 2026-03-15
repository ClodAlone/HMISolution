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
  /// Selects a brush according to an <see cref="TimeSlotState"/>.
  /// </summary>
  public class TimeSlotStateToBrushConverter : IValueConverter
  {
    private Brush _workTimeBrush = new SolidColorBrush(Color.FromArgb(255, 0, 0, 100));
    private Brush _notWorkTimeBrush = new SolidColorBrush(Color.FromArgb(255, 0, 100, 0));
    private Brush _selectedWorkTimeBrush = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
    private Brush _selectedNotWorkkTimeBrush = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));

    /// <summary>
    /// The brush to use if the <see cref="TimeSlotState"/> is WorkTime.
    /// </summary>
    public Brush WorkTimeBrush
    {
      get { return _workTimeBrush; }
      set { _workTimeBrush = value; }
    }

    /// <summary>
    /// The brush to use if the <see cref="TimeSlotState"/> is NotWorkTime.
    /// </summary>
    public Brush NotWorkTimeBrush
    {
      get { return _notWorkTimeBrush; }
      set { _notWorkTimeBrush = value; }
    }

    /// <summary>
    /// The brush to use if the <see cref="TimeSlotState"/> is SelectedWorkTime.
    /// </summary>
    public Brush SelectedWorkTimeBrush
    {
      get { return _selectedWorkTimeBrush; }
      set { _selectedWorkTimeBrush = value; }
    }

    /// <summary>
    /// The brush to use if the <see cref="TimeSlotState"/> is SelectedNotWorkTime.
    /// </summary>
    public Brush SelectedNotWorkTimeBrush
    {
      get { return _selectedNotWorkkTimeBrush; }
      set { _selectedNotWorkkTimeBrush = value; }
    }

    /// <summary>
    /// Selects a brush according to the input <see cref="TimeSlotState"/>.
    /// </summary>
    /// <param name="value">The <see cref="TimeSlotState"/> value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>The selected brush.</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      TimeSlotState state = (TimeSlotState)value;
      switch (state)
      {
        case TimeSlotState.WorkTime: return WorkTimeBrush;
        case TimeSlotState.NotWorkTime: return NotWorkTimeBrush;
        case TimeSlotState.SelectedWorkTime: return SelectedWorkTimeBrush;
        case TimeSlotState.SelectedNotWorkTime: return SelectedNotWorkTimeBrush;
      }
      throw new ArgumentException("Unknown TimeSlotState", "value");
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
