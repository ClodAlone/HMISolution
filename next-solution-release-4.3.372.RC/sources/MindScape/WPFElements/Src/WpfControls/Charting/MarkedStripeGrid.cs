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
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Markup;
using System.Collections.Specialized;

namespace Mindscape.WpfElements.Charting
{
  /// <summary>
  /// Renders colored axis zones based on a pair of <see cref="ChartAxis"/> objects.
  /// </summary>
  public class MarkedStripeGrid : Canvas
  {
    private readonly ObservableCollection<MarkedStripe> _markedStripes = new ObservableCollection<MarkedStripe>();
    
    /// <summary>
    /// Initializes a new instance of the <see cref="MarkedStripeGrid"/> class.
    /// </summary>
    public MarkedStripeGrid()
    {
      _markedStripes.CollectionChanged += new NotifyCollectionChangedEventHandler(MarkedStripes_CollectionChanged);
      Loaded += new RoutedEventHandler(MarkedStripeGrid_Loaded);
    }

    private void MarkedStripes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      if (e.OldItems != null)
      {
        foreach (MarkedStripe stripe in e.OldItems)
        {
          stripe.ValueChanged -= new EventHandler(Stripe_ValueChanged);
          stripe.BackgroundChanged -= new EventHandler(Stripe_BackgroundChanged);
        }
      }
      if (e.NewItems != null)
      {
        foreach (MarkedStripe stripe in e.NewItems)
        {
          stripe.ValueChanged += new EventHandler(Stripe_ValueChanged);
          stripe.BackgroundChanged += new EventHandler(Stripe_BackgroundChanged);
        }
      }
      BuildZones();
    }

    private void Stripe_BackgroundChanged(object sender, EventArgs e)
    {
      BuildZones();
    }

    private void Stripe_ValueChanged(object sender, EventArgs e)
    {
      BuildZones();
    }

    private void MarkedStripeGrid_Loaded(object sender, RoutedEventArgs e)
    {
      Chart chart = VisualTreeUtils.FindAncestor<Chart>(this);
      if (chart != null)
      {
        if (XAxis == null)
        {
          XAxis = chart.XAxis;
        }
        if (YAxis == null)
        {
          YAxis = chart.YAxis;
        }
      }
    }

    #region YAxis property

    /// <summary>
    /// Gets or sets the Y axis.
    /// This is a dependency property.
    /// </summary>
    public ChartAxis YAxis
    {
      get { return (ChartAxis)GetValue(YAxisProperty); }
      set { SetValue(YAxisProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="YAxis"/> property.
    /// </summary>
    public static readonly DependencyProperty YAxisProperty =
      DependencyProperty.Register("YAxis", typeof(ChartAxis), typeof(MarkedStripeGrid),
      new PropertyMetadata(new PropertyChangedCallback(OnYAxisChanged)));

    private static void OnYAxisChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((MarkedStripeGrid)d).OnYAxisChanged(e);
    }

    private void OnYAxisChanged(DependencyPropertyChangedEventArgs e)
    {
      ChartAxis oldAxis = e.OldValue as ChartAxis;
      if (oldAxis != null)
      {
        oldAxis.AxisRendered -= new EventHandler(Axis_AxisUpdated);
      }
      if (YAxis != null)
      {
        YAxis.AxisRendered += new EventHandler(Axis_AxisUpdated);
        BuildZones();
      }
    }

    #endregion // YAxis property

    #region XAxis property

    /// <summary>
    /// Gets or sets the X axis.
    /// This is a dependency property.
    /// </summary>
    public ChartAxis XAxis
    {
      get { return (ChartAxis)GetValue(XAxisProperty); }
      set { SetValue(XAxisProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="XAxis"/> property.
    /// </summary>
    public static readonly DependencyProperty XAxisProperty =
      DependencyProperty.Register("XAxis", typeof(ChartAxis), typeof(MarkedStripeGrid),
      new PropertyMetadata(new PropertyChangedCallback(OnXAxisChanged)));

    private static void OnXAxisChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((MarkedStripeGrid)d).OnXAxisChanged(e);
    }

    private void OnXAxisChanged(DependencyPropertyChangedEventArgs e)
    {
      ChartAxis oldAxis = e.OldValue as ChartAxis;
      if (oldAxis != null)
      {
        oldAxis.AxisRendered -= new EventHandler(Axis_AxisUpdated);
      }
      if (XAxis != null)
      {
        XAxis.AxisRendered += new EventHandler(Axis_AxisUpdated);
        BuildZones();
      }
    }

    #endregion // XAxis property

    #region Orientation Property

    /// <summary>
    /// Gets or sets the orientation of the marked stripes. The default is horizontal.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="OrientationProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public Orientation Orientation
    {
      get { return (Orientation)GetValue(OrientationProperty); }
      set { SetValue(OrientationProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Orientation"/> property.
    /// </summary>
    public static readonly DependencyProperty OrientationProperty =
      DependencyProperty.Register("Orientation", typeof(Orientation), typeof(MarkedStripeGrid),
      new FrameworkPropertyMetadata(Orientation.Horizontal, OnOrientationChanged));

    private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((MarkedStripeGrid)d).OnOrientationChanged();
    }

    private void OnOrientationChanged()
    {
      BuildZones();
    }

    #endregion // Orientation Property

    /// <summary>
    /// Gets the observable collection of <see cref="MarkedStripe"/> objects used to render the <see cref="MarkedStripeGrid"/>.
    /// </summary>
    public ObservableCollection<MarkedStripe> MarkedStripes { get { return _markedStripes; } }

    private void Axis_AxisUpdated(object sender, EventArgs e)
    {
      BuildZones(); // TODO: improve the performance here rather than building the zones no matter what axis property changed.
    }

    private void BuildZones()
    {
      Children.Clear();
      ChartAxis parallelAxis = Orientation == Orientation.Horizontal ? XAxis : YAxis;
      ChartAxis tangentAxis = Orientation == Orientation.Horizontal ? YAxis : XAxis;
      if (tangentAxis != null && parallelAxis != null)
      {
        double parallelAxisSize = Orientation == Orientation.Horizontal ? parallelAxis.ActualWidth : parallelAxis.ActualHeight;
        double tangentAxisSize = Orientation == Orientation.Horizontal ? tangentAxis.ActualHeight : tangentAxis.ActualWidth;
        double y = tangentAxis.ActualMinimumValue;
        MarkedStripe stripe = GetNextMarkedStripe(y);
        while (stripe != null && y < tangentAxis.ActualMaximumValue)
        {
          double physicalStart = Math.Round(tangentAxis.ConvertLogicalToPhysical(y));
          double physicalEnd = Math.Round(tangentAxis.ConvertLogicalToPhysical(stripe.Value));

          Border border = new Border();
          border.Background = stripe.Background;
          border.SnapsToDevicePixels = true;

          if (Orientation == Orientation.Horizontal)
          {
            border.Height = physicalEnd - physicalStart;
            border.Width = parallelAxisSize;
            Canvas.SetTop(border, tangentAxisSize - physicalEnd);
          }
          else
          {
            border.Width = physicalEnd - physicalStart;
            border.Height = parallelAxisSize;
            Canvas.SetLeft(border, physicalStart);
          }

          Children.Add(border);

          y = stripe.Value;
          stripe = GetNextMarkedStripe(y);
        }
      }
    }

    private MarkedStripe GetNextMarkedStripe(double current)
    {
      double next = Double.MaxValue;
      MarkedStripe result = null;
      foreach (MarkedStripe stripe in _markedStripes)
      {
        if (stripe.Value < next && stripe.Value > current)
        {
          next = stripe.Value;
          result = stripe;
        }
      }
      return result;
    }
  }
}
