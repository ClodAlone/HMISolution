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
  /// Plots a bar series on a <see cref="PolarChart"/> control.
  /// </summary>
  public class RoseSeries : PolarSeries
  {
    static RoseSeries()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(RoseSeries),
        new FrameworkPropertyMetadata(typeof(RoseSeries)));
    }

    #region BarStyle property

    /// <summary>
    /// Gets or sets the BarStyle.
    /// This is a dependency property.
    /// </summary>
    public Style BarStyle
    {
      get { return (Style)GetValue(BarStyleProperty); }
      set { SetValue(BarStyleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="BarStyle"/> property.
    /// </summary>
    public static readonly DependencyProperty BarStyleProperty =
      DependencyProperty.Register("BarStyle", typeof(Style), typeof(RoseSeries),
      new PropertyMetadata(new PropertyChangedCallback(OnBarStyleChanged)));

    private static void OnBarStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((RoseSeries)d).OnBarStyleChanged();
    }

    private void OnBarStyleChanged()
    {
      //TODO: apply the new style to all the bars.
    }

    #endregion // BarStyle property
    
    #region BarSizeFactor property

    /// <summary>
    /// Gets or sets the size factor used to calculate the thickness of the bars. This will typically be a value between 0 and 1.
    /// This value is the factor between a single axis unit and the thickness of a bar. A value of 1 will cause all the bars to
    /// be adjacent with no gaps between them. Smaller values put spaces between the bars.
    /// This is a dependency property. The default is 0.8.
    /// </summary>
    public double BarSizeFactor
    {
      get { return (double)GetValue(BarSizeFactorProperty); }
      set { SetValue(BarSizeFactorProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="BarSizeFactor"/> property.
    /// </summary>
    public static readonly DependencyProperty BarSizeFactorProperty =
      DependencyProperty.Register("BarSizeFactor", typeof(double), typeof(RoseSeries),
      new PropertyMetadata(0.8, new PropertyChangedCallback(OnBarSizeFactorChanged)));

    private static void OnBarSizeFactorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((RoseSeries)d).OnBarSizeFactorChanged();
    }

    private void OnBarSizeFactorChanged()
    {
    }

    #endregion // BarSizeFactor property
    
    /// <summary>
    /// Plots the <see cref="RoseSeries"/> on the chart canvas.
    /// </summary>
    protected override void BuildChartCore()
    {
      double logicalSpacing = BarSizeFactor;
      if (ItemsSource.Count > 1)
      {
        PolarPoint p1, p2;
        GetBar(ItemsSource[0], 0, out p1);
        GetBar(ItemsSource[1], 1, out p2);
        logicalSpacing *= (p2.Theta - p1.Theta);
        if (logicalSpacing < 0)
        {
          logicalSpacing = BarSizeFactor;
        }
      }
      double logicalRange = ThetaAxis.Maximum - ThetaAxis.Minimum;
      double ratio = logicalSpacing / logicalRange;
      double barWidthAngle = 360 * ratio;

      int index = 0;
      foreach (object o in ItemsSource)
      {
        PolarPoint point;
        PolarBar bar = GetBar(o, index, out point);
        PrepareDataPoint(bar, index);
        Point normalPoint = ConvertLogicalToPhysicalPoint(point);

        bar.Style = BarStyle;
        if (bar.Background == null && SeriesBrush != null)
        {
          bar.Background = SeriesBrush;
        }
        if (bar.BorderBrush == null && SeriesBrush != null)
        {
          bar.BorderBrush = SeriesBrush;
        }

        double height = RhoAxis.ConvertLogicalToPhysical(point.Rho);
        bar.Height = height + 1;
        double angle = ThetaAxis.ConvertLogicalToPhysical(point.Theta);
        RotateTransform rotation = new RotateTransform();
        rotation.Angle = angle;
        bar.RenderTransform = rotation;

        bar.UpdatePathData(height, 0, barWidthAngle);

        Canvas.SetLeft(bar, Math.Round(normalPoint.X));
        Canvas.SetTop(bar, Math.Round(normalPoint.Y));
        Canvas.SetZIndex(bar, 1);
        Canvas.Children.Add(bar);

        // Data Label:
        //AddDataLabel(index, symbol.Background);
        index++;
      }
    }

    /// <summary>
    /// Gets the <see cref="PolarBar"/> for the given data object.
    /// </summary>
    /// <param name="o">The data object that the <see cref="PolarBar"/> displays.</param>
    /// <param name="index">The index of the data.</param>
    /// <param name="point">The logical plot position of the data.</param>
    /// <returns>The <see cref="PolarBar"/> to display the given data object.</returns>
    protected PolarBar GetBar(object o, int index, out PolarPoint point)
    {
      PolarBar bar = GetDataPoint(o, index) as PolarBar;
      if (bar == null)
      {
        bar = new PolarBar(o);
        point = GetPoint(bar, ReverseAxes);
      }
      else
      {
        point = bar.LogicalPoint;
      }
      return bar;
    }
  }
}
