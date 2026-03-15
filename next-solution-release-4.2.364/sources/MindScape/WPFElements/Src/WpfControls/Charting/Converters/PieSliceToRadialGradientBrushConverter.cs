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
using System.Windows.Markup;
using System.Collections.ObjectModel;

namespace Mindscape.WpfElements.Charting
{
  /// <summary>
  /// Creates a <see cref="RadialGradientBrush"/> for a <see cref="PieSlice"/>.
  /// </summary>
  public class PieSliceToRadialGradientBrushConverter : IValueConverter
  {
    private ObservableCollection<GradientStop> _gradientStops = new ObservableCollection<GradientStop>();
    private ObservableCollection<GradientStop> _doughnutGradientStops = new ObservableCollection<GradientStop>();

    /// <summary>
    /// Initializes a new instance of the <see cref="PieSliceToRadialGradientBrushConverter"/> class.
    /// </summary>
    public PieSliceToRadialGradientBrushConverter()
    {
      GradientCenter = new Point(0.5, 0.5);
    }

    /// <summary>
    /// Gets the collection of <see cref="GradientStop"/> objects for the <see cref="RadialGradientBrush"/>.
    /// </summary>
    public ObservableCollection<GradientStop> GradientStops { get { return _gradientStops; } }

    /// <summary>
    /// Gets the collection of <see cref="GradientStop"/> objects to use if the pie slice has a doughnut shape.
    /// </summary>
    public ObservableCollection<GradientStop> DoughnutGradientStops { get { return _doughnutGradientStops; } }

    /// <summary>
    /// Gets or sets the center point of the <see cref="RadialGradientBrush"/>. This point will typically be made up of values between
    /// 0 and 1 which are factors relative to the bounding box of all the pie slices in the chart. The default is (0.5, 0.5).
    /// </summary>
    public Point GradientCenter { get; set; }

    /// <summary>
    /// Creates a <see cref="RadialGradientBrush"/> for the given <see cref="PieSlice"/>.
    /// </summary>
    /// <param name="value">The <see cref="PieSlice"/> value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>The created <see cref="RadialGradientBrush"/>.</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      PieSlice slice = value as PieSlice;
      RadialGradientBrush brush = new RadialGradientBrush();

      if (slice != null && slice.Radius != 0)
      {
        //Canvas canvas = VisualTreeUtils.FindAncestor<Canvas>(slice);
        //if (canvas != null && canvas.ActualWidth != 0 && canvas.ActualHeight != 0)
        if (slice.Width != 0 && slice.Height != 0)
        {
          double startAngle = slice.StartAngle;
          double endAngle = slice.EndAngle;
          double sweepAngle = endAngle - startAngle;
          double dx = slice.ExplodedDistance * Math.Cos(startAngle + sweepAngle / 2);
          double dy = slice.ExplodedDistance * Math.Sin(startAngle + sweepAngle / 2);

          double gradientCenterX = (slice.Width / 2) - slice.Radius + (slice.Diameter * GradientCenter.X) + dx;
          double gradientCenterY = (slice.Height / 2) - slice.Radius + (slice.Diameter * GradientCenter.Y) + dy;
          double centerX = gradientCenterX / slice.Width; // 0.5;// slice.CenterPoint.X / canvas.ActualWidth;
          double centerY = gradientCenterY / slice.Height; // 0.5;// slice.CenterPoint.Y / canvas.ActualHeight;
          Point centerPoint = new Point(centerX, centerY);
          double radiusX = slice.Radius / slice.Width;
          double radiusY = slice.Radius / slice.Height;

          brush.Center = centerPoint;
          brush.GradientOrigin = centerPoint;
          brush.RadiusX = radiusX;
          brush.RadiusY = radiusY;

          double distanceFromCenter = Math.Sqrt(GeometryUtils.DistanceMagnitude(new Point(gradientCenterX, gradientCenterY), slice.CenterPoint));
          double radiusFactorInverse = (distanceFromCenter / slice.Radius);
          double radiusFactor = 1 - radiusFactorInverse;

          foreach (GradientStop gradientStop in slice.DoughnutScale == 1 ? _gradientStops : _doughnutGradientStops)
          {
            double offset = gradientStop.Offset * radiusFactor + radiusFactorInverse;
            GradientStop gs = new GradientStop() { Offset = offset, Color = gradientStop.Color };
            brush.GradientStops.Add(gs);
          }
        }
        //else
        {
          //return brush;
        }
      }

      return brush;
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
