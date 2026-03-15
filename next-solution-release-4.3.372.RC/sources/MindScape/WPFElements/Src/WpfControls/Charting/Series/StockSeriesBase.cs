using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;

namespace Mindscape.WpfElements.Charting
{
  /// <summary>
  /// The base class for stock series involving open, high, low and close values.
  /// </summary>
  public abstract class StockSeriesBase : DataSeries
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="StockSeriesBase"/> class.
    /// </summary>
    public StockSeriesBase()
    {
      // TODO: this should probabaly override the metadata rather than setting property directly. (Is that possible?)
      DataSampler = new FixedSampleCountSampler() { MaxDataPointCount = 100 }; // TODO: this value should be based on the number of BarSeries in the Chart.
    }

    internal override void GetDataPointDimensions(object dataPoint, Point point, out double dependentLow, out double dependentHigh)
    {
      Stick stick = new Stick(dataPoint); // TODO: this is a little bit slow.
      dependentLow = GetLow(stick);
      dependentHigh = GetHigh(stick);
    }

    internal Brush GetPositiveBrush()
    {
      if (PositiveStyle != null)
      {
        foreach (Setter setter in PositiveStyle.Setters)
        {
          if (setter.Property == Path.StrokeProperty)
          {
            return setter.Value as Brush;
          }
        }
      }
      return SeriesBrush;
    }

    internal Brush GetPositiveFillBrush()
    {
      if (PositiveStyle != null)
      {
        foreach (Setter setter in PositiveStyle.Setters)
        {
          if (setter.Property == Path.FillProperty)
          {
            return setter.Value as Brush;
          }
        }
      }
      return null;
    }

    internal Brush GetNegativeBrush()
    {
      if (NegativeStyle != null)
      {
        foreach (Setter setter in NegativeStyle.Setters)
        {
          if (setter.Property == Path.StrokeProperty)
          {
            return setter.Value as Brush;
          }
        }
      }
      return NegativeSeriesBrush;
    }

    internal Brush GetNegativeFillBrush()
    {
      if (NegativeStyle != null)
      {
        foreach (Setter setter in NegativeStyle.Setters)
        {
          if (setter.Property == Path.FillProperty)
          {
            return setter.Value as Brush;
          }
        }
      }
      return NegativeSeriesBrush;
    }

    internal override void OnXAxisChanged()
    {
      base.OnXAxisChanged();

      if (XAxis != null && XAxis.IsAuto)
      {
        XAxis.SetLabelLayoutInternal(AxisLabelLayout.Inside);
        XAxis.TickLayout = AxisTickLayout.Inside;
      }
    }

    /// <summary>
    /// Gets or set the binding used to extract the high value from each data point.
    /// </summary>
    public Binding HighBinding { get; set; }

    /// <summary>
    /// Gets or set the binding used to extract the low value from each data point.
    /// </summary>
    public Binding LowBinding { get; set; }

    /// <summary>
    /// Gets or set the binding used to extract the open value from each data point.
    /// </summary>
    public Binding OpenBinding { get; set; }

    /// <summary>
    /// Gets or set the binding used to extract the close value from each data point.
    /// </summary>
    public Binding CloseBinding { get; set; }

    #region NegativeSeriesBrush Property

    /// <summary>
    /// Gets or sets the <see cref="Brush"/> used to render negative data points.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="NegativeSeriesBrushProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public Brush NegativeSeriesBrush
    {
      get { return (Brush)GetValue(NegativeSeriesBrushProperty); }
      set { SetValue(NegativeSeriesBrushProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="NegativeSeriesBrush"/> property.
    /// </summary>
    public static readonly DependencyProperty NegativeSeriesBrushProperty =
      DependencyProperty.Register("NegativeSeriesBrush", typeof(Brush), typeof(StockSeriesBase),
      new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Red), OnNegativeSeriesBrushChanged));

    private static void OnNegativeSeriesBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((StockSeriesBase)d).OnNegativeSeriesBrushChanged();
    }

    private void OnNegativeSeriesBrushChanged()
    {
    }

    #endregion // NegativeSeriesBrush Property

    #region PositiveStyle Property

    /// <summary>
    /// Gets or sets the <see cref="Style"/> used when the close value is higher than the open value of a data point.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="PositiveStyleProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public Style PositiveStyle
    {
      get { return (Style)GetValue(PositiveStyleProperty); }
      set { SetValue(PositiveStyleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="PositiveStyle"/> property.
    /// </summary>
    public static readonly DependencyProperty PositiveStyleProperty =
      DependencyProperty.Register("PositiveStyle", typeof(Style), typeof(StockSeriesBase),
      new FrameworkPropertyMetadata(OnPositiveStyleChanged));

    private static void OnPositiveStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((StockSeriesBase)d).OnPositiveStyleChanged();
    }

    private void OnPositiveStyleChanged()
    {
      // TODO
    }

    #endregion // PositiveStyle Property

    #region NegativeStyle Property

    /// <summary>
    /// Gets or sets the <see cref="Style"/> used when the close value is lower than the open value of a data point.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="NegativeStyleProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public Style NegativeStyle
    {
      get { return (Style)GetValue(NegativeStyleProperty); }
      set { SetValue(NegativeStyleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="NegativeStyle"/> property.
    /// </summary>
    public static readonly DependencyProperty NegativeStyleProperty =
      DependencyProperty.Register("NegativeStyle", typeof(Style), typeof(StockSeriesBase),
      new FrameworkPropertyMetadata(OnNegativeStyleChanged));

    private static void OnNegativeStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((StockSeriesBase)d).OnNegativeStyleChanged();
    }

    private void OnNegativeStyleChanged()
    {
      // TODO
    }

    #endregion // NegativeStyle Property

    // TODO: use the extractor technique to improve the performance of using the bindings.
    //       Remember that when rendering, a binding should be set on each data point to listen for property changes. But when analysing the series we just need the property value.

    internal double GetLow(StockDataPointBase stick)
    {
      if (LowBinding != null)
      {
        stick.SetBinding(StockDataPointBase.LowProperty, LowBinding);
      }
      else if (stick.DataContext is StockDataPoint)
      {
        StockDataPoint stockDataPoint = stick.DataContext as StockDataPoint;
        return stockDataPoint.Low;
      }
      return stick.Low;
    }

    internal double GetHigh(StockDataPointBase stick)
    {
      if (HighBinding != null)
      {
        stick.SetBinding(StockDataPointBase.HighProperty, HighBinding);
      }
      else if (stick.DataContext is StockDataPoint)
      {
        StockDataPoint stockDataPoint = stick.DataContext as StockDataPoint;
        return stockDataPoint.High;
      }
      return stick.High;
    }

    internal double GetOpen(StockDataPointBase stick)
    {
      if (OpenBinding != null)
      {
        stick.SetBinding(StockDataPointBase.OpenProperty, OpenBinding);
      }
      else if (stick.DataContext is StockDataPoint)
      {
        StockDataPoint stockDataPoint = stick.DataContext as StockDataPoint;
        return stockDataPoint.Open;
      }
      return stick.Open;
    }

    internal double GetClose(StockDataPointBase stick)
    {
      if (CloseBinding != null)
      {
        stick.SetBinding(StockDataPointBase.CloseProperty, CloseBinding);
      }
      else if (stick.DataContext is StockDataPoint)
      {
        StockDataPoint stockDataPoint = stick.DataContext as StockDataPoint;
        return stockDataPoint.Close;
      }
      return stick.Close;
    }
  }
}
