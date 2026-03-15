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
  /// Calculates the corner radius for displaying a <see cref="SchedulerElement"/> in a view.
  /// The element is given rounded corners on the left or top if it includes the
  /// start of the schedule item, and on the right or bottom if it includes the end of the schedule
  /// item; otherwise, the item is given non-rounded corners.
  /// </summary>
  public class SchedulerElementDisplayStateToCornerRadiusConverter : IValueConverter
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="SchedulerElementDisplayStateToCornerRadiusConverter"/> class.
    /// </summary>
    public SchedulerElementDisplayStateToCornerRadiusConverter()
    {
      Orientation = Orientation.Vertical;
    }

    /// <summary>
    /// Gets or sets the corner rounding to be applied at each end of the <see cref="SchedulerElement"/>
    /// if that end is included in the <see cref="SchedulerElementDisplayState"/>.
    /// </summary>
    public double ScheduleElementEndRadius { get; set; }

    /// <summary>
    /// Gets or sets the orientation of the item.  If Vertical, the rounding will be applied to the top
    /// and/or bottom pairs of corners; if Horizontal, to the left and/or right pairs of corners.
    /// The default is Orientation.Vertical.
    /// </summary>
    public Orientation Orientation { get; set; }

    /// <summary>
    /// Calculates corner rounding for displaying a <see cref="SchedulerElement"/> in a summary view.
    /// </summary>
    /// <param name="value">The <see cref="SchedulerElementDisplayState"/> value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>A CornerRadius with left/top and right/bottom values set to the <see cref="ScheduleElementEndRadius"/>
    /// if the SchedulerElement includes the relevant end of the schedule item, otherwise unrounded.</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if(value != null)
      {
        SchedulerElementDisplayState state = (SchedulerElementDisplayState)value;
        if (Orientation == Orientation.Vertical)
        {
          double top = state.IsScheduleItemStartVisible ? ScheduleElementEndRadius : 0;
          double bottom = state.IsScheduleItemEndVisible ? ScheduleElementEndRadius : 0;
          return new CornerRadius(top, top, bottom, bottom);
        }
        else
        {
          double left = state.IsScheduleItemStartVisible ? ScheduleElementEndRadius : 0;
          double right = state.IsScheduleItemEndVisible ? ScheduleElementEndRadius : 0;
          return new CornerRadius(left, right, right, left);
        }
      }
      return new CornerRadius(ScheduleElementEndRadius);
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
