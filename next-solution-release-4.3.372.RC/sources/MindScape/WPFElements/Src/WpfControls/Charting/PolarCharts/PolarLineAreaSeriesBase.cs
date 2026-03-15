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

namespace Mindscape.WpfElements.Charting
{
  /// <summary>
  /// A base class for <see cref="PolarSeries"/> that use lines to plot data, such as <see cref="RadarSeries"/>.
  /// </summary>
  public abstract class PolarLineAreaSeriesBase : PolarPointSeriesBase
  {
    #region LineStyle property

    /// <summary>
    /// Gets or sets the LineStyle.
    /// This is a dependency property.
    /// </summary>
    public Style LineStyle
    {
      get { return (Style)GetValue(LineStyleProperty); }
      set { SetValue(LineStyleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="LineStyle"/> property.
    /// </summary>
    public static readonly DependencyProperty LineStyleProperty =
      DependencyProperty.Register("LineStyle", typeof(Style), typeof(PolarLineAreaSeriesBase),
      new PropertyMetadata(BuildDefaultLineStyle(), new PropertyChangedCallback(OnLineStyleChanged)));

    private static void OnLineStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((PolarLineAreaSeriesBase)d).OnLineStyleChanged();
    }

    private void OnLineStyleChanged()
    {
      // TODO: apply new style to the path
    }

    private static Style BuildDefaultLineStyle()
    {
      // TODO: might be able to refactor this out with cartesian charts.
      Style style = new Style(typeof(Path));
      style.Setters.Add(new Setter(Path.StrokeThicknessProperty, 2.0));
      return style;
    }

    #endregion // LineStyle property

    #region DashArray property

    /// <summary>
    /// Gets or sets the DashArray.
    /// This is a dependency property.
    /// </summary>
    public DoubleCollection DashArray
    {
      get { return (DoubleCollection)GetValue(DashArrayProperty); }
      set { SetValue(DashArrayProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="DashArray"/> property.
    /// </summary>
    public static readonly DependencyProperty DashArrayProperty =
      DependencyProperty.Register("DashArray", typeof(DoubleCollection), typeof(PolarLineAreaSeriesBase),
      new PropertyMetadata(new PropertyChangedCallback(OnDashArrayChanged)));

    private static void OnDashArrayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((PolarLineAreaSeriesBase)d).OnDashArrayChanged();
    }

    private void OnDashArrayChanged()
    {
      // TODO: apply new dash array to line.
    }

    #endregion // DashArray property
    
    internal Brush GetAreaBrush()
    {
      SolidColorBrush seriesBrush = SeriesBrush as SolidColorBrush;
      if (seriesBrush != null)
      {
        SolidColorBrush background = new SolidColorBrush();
        byte r = seriesBrush.Color.R;
        byte g = seriesBrush.Color.G;
        byte b = seriesBrush.Color.B;
        background.Color = new Color() { A = 119, R = r, G = g, B = b };
        return background;
      }
      return SeriesBrush;
    }
  }
}
