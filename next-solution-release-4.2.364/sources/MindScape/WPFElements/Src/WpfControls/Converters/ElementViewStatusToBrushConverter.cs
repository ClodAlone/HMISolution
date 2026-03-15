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
  /// Selects a brush according to an <see cref="ElementViewStatus"/>.
  /// </summary>
  public class ElementViewStatusToBrushConverter : IValueConverter
  {
    /// <summary>
    /// The brush to use if the <see cref="ElementViewStatus"/> is Normal.
    /// </summary>
    public Brush NormalBrush { get; set; }

    /// <summary>
    /// The brush to use if the <see cref="ElementViewStatus"/> is MouseOver.
    /// </summary>
    public Brush MouseOverBrush { get; set; }

    /// <summary>
    /// The brush to use if the <see cref="ElementViewStatus"/> is Padding.
    /// </summary>
    public Brush PaddingBrush { get; set; }

    /// <summary>
    /// The brush to use if the <see cref="ElementViewStatus"/> is Selected.
    /// </summary>
    public Brush SelectedBrush { get; set; }

    /// <summary>
    /// The brush to use if the <see cref="ElementViewStatus"/> is MouseOverPadding.
    /// </summary>
    public Brush MouseOverPaddingBrush { get; set; }

    /// <summary>
    /// Selects a brush according to the input <see cref="ElementViewStatus"/>.
    /// </summary>
    /// <param name="value">The <see cref="ElementViewStatus"/> value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>The selected brush.</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      ElementViewStatus viewStatus = (ElementViewStatus)value;
      switch (viewStatus)
      {
        case ElementViewStatus.Selected: return SelectedBrush;
        case ElementViewStatus.MouseOver: return MouseOverBrush;
        case ElementViewStatus.MouseOverPadding: return MouseOverPaddingBrush ?? PaddingBrush;
        case ElementViewStatus.Padding: return PaddingBrush;
        default: return NormalBrush;
      }
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
