using System;
using System.Windows.Data;
using System.Windows;

namespace Mindscape.WpfElements.WpfPropertyGrid
{
  /// <summary>
  /// Calculates margins and padding required by editors with non-default background
  /// colors implemented using Border elements.
  /// </summary>
  [ValueConversion(typeof(Thickness), typeof(Thickness))]
  public class GridSpacingCompensationConverter : IValueConverter
  {
    private const double LeftGridSpacing = 12;  // 2 * 6 (baked into the WPF GridViewRowPresenter class)
    private const double RightGridSpacing = 10;

    /// <summary>
    /// Gets or sets whether the thickness is for use as a margin (expanding a
    /// Border over the space reserved by the grid) or as padding (realigning content
    /// to unbordered layouts).
    /// </summary>
    public ThicknessUsage Usage { get; set; }

    /// <summary>
    /// Calculates the margin or padding that should be set on a Border element to
    /// align the border with the column separator and the border content with the
    /// value column.
    /// </summary>
    /// <param name="value">The editor margin imposed by the BuiltInEditor class.
    /// This will be the <see cref="PropertyGrid.DefaultMargin"/> of the <see cref="PropertyGrid"/> class.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>The margin or padding to set on the Border element.</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      int scale = (Usage == ThicknessUsage.Padding ? 1 : -1);
      Thickness defaultMargin = (Thickness)value;
      double left = (LeftGridSpacing + defaultMargin.Left) * scale;
      double right = (RightGridSpacing + defaultMargin.Right) * scale;
      return new Thickness(left, scale * defaultMargin.Top, right, scale * defaultMargin.Bottom);
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

  /// <summary>
  /// Defines whether a <see cref="GridSpacingCompensationConverter"/> should calculate
  /// margin or padding values.
  /// </summary>
  public enum ThicknessUsage
  {
    /// <summary>
    /// The converter should calculate margin values.
    /// </summary>
    Margin,

    /// <summary>
    /// The converter should calculate padding values.
    /// </summary>
    Padding
  }


}
