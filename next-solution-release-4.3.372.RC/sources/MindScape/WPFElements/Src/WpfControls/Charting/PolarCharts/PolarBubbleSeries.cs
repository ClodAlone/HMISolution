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

namespace Mindscape.WpfElements.Charting
{
  // TODO: provide an option to change if the size affects the radius or the diameter.
  // TODO: provide an option to specify if the size data of the bubble is used directly or if it is modified by an axis.

  /// <summary>
  /// Plots a bubble series on a <see cref="PolarChart"/> control.
  /// </summary>
  public class PolarBubbleSeries : PolarSeries
  {
    static PolarBubbleSeries()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(PolarBubbleSeries),
        new FrameworkPropertyMetadata(typeof(PolarBubbleSeries)));
    }

    /// <summary>
    /// Gets or sets the binding used to extract the bubble size from each data point.
    /// </summary>
    public Binding SizeBinding { get; set; }

    private double GetSize(PolarBubble bubble)
    {
      if (SizeBinding != null)
      {
        bubble.SetBinding(PolarBubble.SizeProperty, SizeBinding);
      }
      return bubble.Size;
    }

    #region BubbleStyle property

    /// <summary>
    /// Gets or sets the <see cref="Style"/> to be applied to the <see cref="PolarBubble"/> data points.
    /// This is a dependency property.
    /// </summary>
    public Style BubbleStyle
    {
      get { return (Style)GetValue(BubbleStyleProperty); }
      set { SetValue(BubbleStyleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="BubbleStyle"/> property.
    /// </summary>
    public static readonly DependencyProperty BubbleStyleProperty =
      DependencyProperty.Register("BubbleStyle", typeof(Style), typeof(PolarBubbleSeries),
      new PropertyMetadata(new PropertyChangedCallback(OnBubbleStyleChanged)));

    private static void OnBubbleStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((PolarBubbleSeries)d).OnBubbleStyleChanged();
    }

    private void OnBubbleStyleChanged()
    {
      // TODO: apply the new style to all the bubbles.
    }

    #endregion // BubbleStyle property
    
    /// <summary>
    /// Plots the <see cref="PolarBubbleSeries"/> on the chart canvas.
    /// </summary>
    protected override void BuildChartCore()
    {
      int index = 0;
      foreach (object o in ItemsSource)
      {
        PolarPoint point;
        PolarBubble bubble = GetBubble(o, index, out point);
        PrepareDataPoint(bubble, index);
        Point normalPoint = ConvertLogicalToPhysicalPoint(point);

        double logicalSize = GetSize(bubble);
        // TODO: provide options for choosing which axis to use for getting the size.
        double size = Math.Abs(RhoAxis.ConvertLogicalToPhysicalSize(logicalSize));

        bubble.IsNegative = logicalSize < 0;
        bubble.Width = size;
        bubble.Height = size;
        if (SeriesBrush != null)
        {
          bubble.Background = SeriesBrush;
        }
        if (BubbleStyle != null)
        {
          bubble.Style = BubbleStyle;
        }
        Canvas.SetLeft(bubble, normalPoint.X - size / 2);
        Canvas.SetTop(bubble, normalPoint.Y - size / 2);
        Canvas.Children.Add(bubble);
        index++;
      }
    }

    private PolarBubble GetBubble(object o, int index, out PolarPoint point)
    {
      PolarBubble bubble = GetDataPoint(o, index) as PolarBubble;
      if (bubble == null)
      {
        bubble = new PolarBubble(o);
        point = GetPoint(bubble);
      }
      else
      {
        point = bubble.LogicalPoint;
      }
      return bubble;
    }
  }
}
