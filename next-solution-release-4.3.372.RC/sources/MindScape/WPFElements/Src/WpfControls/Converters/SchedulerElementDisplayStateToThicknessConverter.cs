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
  /// Calculates the margin for displaying a <see cref="SchedulerElement"/> in a view.
  /// The element is offset from the left if includes the start of the schedule item, and
  /// from the right if includes the end of the schedule item; otherwise, the element bleeds 
  /// to the edge of the view.
  /// </summary>
  public class SchedulerElementDisplayStateToThicknessConverter : IValueConverter
  {
    /// <summary>
    /// Gets or sets the offset to be applied at each end of the <see cref="SchedulerElement"/>
    /// if that end is included in the <see cref="SchedulerElementDisplayState"/>.
    /// </summary>
    public double ScheduleElementEndOffset { get; set; }

    /// <summary>
    /// Calculates the margin for displaying a <see cref="SchedulerElement"/>
    /// in a view.
    /// </summary>
    /// <param name="value">The <see cref="SchedulerElementDisplayState"/> value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>A Thickness with left and right values set to the <see cref="ScheduleElementEndOffset"/>
    /// if the SchedulerElement includes the relevant end of the schedule item, otherwise 0.</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value != null)
      {
        SchedulerElementDisplayState state = (SchedulerElementDisplayState)value;
        double left = state.IsScheduleItemStartVisible ? ScheduleElementEndOffset : 0;
        double right = state.IsScheduleItemEndVisible ? ScheduleElementEndOffset : 0;
        return new Thickness(left, 0, right, 0);
      }
      return new Thickness(0);
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
