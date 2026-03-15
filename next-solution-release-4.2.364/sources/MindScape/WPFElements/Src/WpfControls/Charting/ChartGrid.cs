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
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Mindscape.WpfElements.Charting
{
  /// <summary>
  /// Renders grid lines and stripe lines based on a pair of <see cref="ChartAxis"/> objects.
  /// </summary>
  public class ChartGrid : Canvas
  {
    private ObservableCollection<Brush> _verticalStripeBrushes = new ObservableCollection<Brush>();
    private ObservableCollection<Brush> _horizontalStripeBrushes = new ObservableCollection<Brush>();

    private ObservableCollection<Polyline> _verticalGridLines = new ObservableCollection<Polyline>();
    private ObservableCollection<Polyline> _horizontalGridLines = new ObservableCollection<Polyline>();

    private ObservableCollection<Polyline> _verticalMinorGridLines = new ObservableCollection<Polyline>();
    private ObservableCollection<Polyline> _horizontalMinorGridLines = new ObservableCollection<Polyline>();

    private ObservableCollection<Border> _verticalStripeLines = new ObservableCollection<Border>();
    private ObservableCollection<Border> _horizontalStripeLines = new ObservableCollection<Border>();

    private static Color DefaultGridLineColor = new Color() { A = 255, R = 178, G = 178, B = 178 };

    /// <summary>
    /// Initializes a new instance of the <see cref="ChartGrid"/> class.
    /// </summary>
    public ChartGrid()
    {
      SnapsToDevicePixels = true;
      Loaded += new RoutedEventHandler(ChartGrid_Loaded);
    }

    private void ChartGrid_Loaded(object sender, RoutedEventArgs e)
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
        chart.XAxisChanged += new EventHandler(Chart_XAxisChanged);
        chart.YAxisChanged += new EventHandler(Chart_YAxisChanged);
      }
    }

    private void Chart_YAxisChanged(object sender, EventArgs e)
    {
      Chart chart = sender as Chart;
      YAxis = chart.YAxis;
    }

    private void Chart_XAxisChanged(object sender, EventArgs e)
    {
      Chart chart = sender as Chart;
      XAxis = chart.XAxis;
    }

    /*private void BuildGridLines()
    {
      Children.Clear();

      double width = Math.Round(ActualWidth);
      double height = Math.Round(ActualHeight);

      int index = 0;
      if (XAxis != null)
      {
        double lastX = 0;
        if (_verticalStripeBrushes.Count > 0)
        {
          index = Math.Abs((int)((XAxis.ActualMinimum / XAxis.GetSpacing()) % _verticalStripeBrushes.Count));
          index = (index - 1) % _verticalStripeBrushes.Count;
        }
        for(int xIndex = 0; xIndex < XAxis.MajorTickMarks.Count; xIndex++)
        {
          double x = 0;// width;
          if (xIndex != XAxis.MajorTickMarks.Count)
          {
            x = XAxis.MajorTickMarks[xIndex].Margin.Left;
          }
          double xTick = x;
          if (_verticalStripeBrushes.Count > 0 && xTick > 0)
          {
            Border stripe = new Border();
            stripe.SnapsToDevicePixels = true;
            stripe.Background = _verticalStripeBrushes[index];
            stripe.Height = height;
            stripe.Width = Math.Max(xTick - lastX + 0.5, 0);
            Canvas.SetLeft(stripe, lastX);
            Children.Add(stripe);
          }
          if (_verticalStripeBrushes.Count > 0)
          {
            index = ++index % _verticalStripeBrushes.Count;
          }

          Polyline line = new Polyline();
          line.Points.Add(new Point(xTick + 0.5, 0 - 0.5));
          line.Points.Add(new Point(xTick + 0.5, height));
          line.Stroke = VerticalGridLineBrush;
          line.StrokeThickness = 1;
          //line.SnapsToDevicePixels = true;
          RenderOptions.SetEdgeMode(line, EdgeMode.Aliased);
          Canvas.SetZIndex(line, 100);
          if (VerticalDashArray != null)
          {
            DoubleCollection collection = new DoubleCollection();
            foreach (double d in VerticalDashArray)
            {
              collection.Add(d);
            }
            line.StrokeDashArray = collection;
          }
          Children.Add(line);

          lastX = xTick;
        }
        RenderMinorVerticalLines();
      }
      if (YAxis != null)
      {
        double lastY = 0;
        index = 0;
        if (_horizontalStripeBrushes.Count > 0 && YAxis.MajorTickMarks.Count > 0)
        {
          index = Math.Abs((int)((YAxis.ActualMinimum / YAxis.GetSpacing()) % _horizontalStripeBrushes.Count));
          index = (index + (_horizontalStripeBrushes.Count - 1)) % _horizontalStripeBrushes.Count;
        }
        for(int yIndex = 0; yIndex < YAxis.MajorTickMarks.Count; yIndex++)
        {
          double y = 0;// height;
          if (yIndex != YAxis.MajorTickMarks.Count)
          {
            y = YAxis.MajorTickMarks[yIndex].Margin.Bottom;
          }
          double yTick = y;
          if (HorizontalStripeBrushes.Count > 0 && yTick > 0)
          {
            Border stripe = new Border();
            stripe.SnapsToDevicePixels = true;
            stripe.Background = _horizontalStripeBrushes[index];
            stripe.Width = width + 0.4;
            stripe.Height = Math.Max(yTick - lastY - 0.4, 1);
            Canvas.SetTop(stripe, height - lastY - stripe.Height);
            Children.Add(stripe);
          }
          if (HorizontalStripeBrushes.Count > 0)
          {
            index = ++index % _horizontalStripeBrushes.Count;
          }

          Polyline line = new Polyline();
          line.Points.Add(new Point(0, height - yTick - 0.4));
          line.Points.Add(new Point(width + 0.4, height - yTick - 0.4));
          line.Stroke = HorizontalGridLineBrush;
          line.StrokeThickness = 1;
          //line.SnapsToDevicePixels = true;
          RenderOptions.SetEdgeMode(line, EdgeMode.Aliased);
          Canvas.SetZIndex(line, 100);
          if (HorizontalDashArray != null)
          {
            DoubleCollection collection = new DoubleCollection();
            foreach (double d in HorizontalDashArray)
            {
              collection.Add(d);
            }
            line.StrokeDashArray = collection;
          }
          Children.Add(line);

          lastY = yTick;
        }
        RenderMinorHorizontalLines();
      }
    }*/

    private void BuildHorizontalLines()
    {
      if (YAxis != null)
      {
        int index = 0;
        double lastY = 0;
        int gridLineCount = 0;
        int stripeCount = 0;
        if (_horizontalStripeBrushes.Count > 0)
        {
          double firstLogicalPosition = YAxis.ConvertPhysicalToLogical(YAxis.ActualHeight - 1);
          if (YAxis.MajorTickMarks.Count > 0 && (YAxis.MajorTickMarks[0].RenderTransform as TranslateTransform).Y == 0)
          {
            firstLogicalPosition = YAxis.LogicalMajorTickMarkPositions[0];
          }
          if (YAxis.IsTickAndLabelLayoutDifferent)
          {
            firstLogicalPosition += YAxis.ActualMajorTickMarkSpacing / 2.0;
          }
          index = (int)Math.Abs(Math.Floor(firstLogicalPosition / YAxis.ActualMajorTickMarkSpacing) % _horizontalStripeBrushes.Count);
          //index = Math.Abs((int)((YAxis.ActualMinimum / YAxis.ActualMajorTickMarkSpacing) % _horizontalStripeBrushes.Count));
          //index = (index + (_horizontalStripeBrushes.Count - 1)) % _horizontalStripeBrushes.Count;
        }
        for (int yIndex = 0; yIndex <= YAxis.MajorTickMarks.Count; yIndex++)
        {
          if (yIndex == YAxis.MajorTickMarks.Count || YAxis.MajorTickMarks[yIndex].Visibility == Visibility.Visible)
          {
            double y = ActualHeight;
            if (yIndex != YAxis.MajorTickMarks.Count)
            {
              TranslateTransform transform = YAxis.MajorTickMarks[yIndex].RenderTransform as TranslateTransform;
              y = transform.Y;
            }
            else
            {
              y = -ActualHeight;
            }
            if (HorizontalStripeBrushes.Count > 0)
            {
              if (y < 0)
              {
                AddStripeLine(_horizontalStripeLines, stripeCount, Orientation.Horizontal, index, ActualHeight + y, ActualHeight + lastY);
                stripeCount++;
                index = ++index % _horizontalStripeBrushes.Count;
              }
            }
            AddGridLine(_horizontalGridLines, yIndex, Orientation.Horizontal, ActualHeight + y, HorizontalGridLineBrush, HorizontalDashArray);

            lastY = y;
            gridLineCount = yIndex;
          }
        }
        RenderMinorHorizontalLines();
        while (_horizontalGridLines.Count > gridLineCount)
        {
          UpdateGridLine(_horizontalGridLines[gridLineCount], Orientation.Horizontal, -10);
          gridLineCount++;
        }
        while (_horizontalStripeLines.Count > stripeCount)
        {
          UpdateStripeLine(_horizontalStripeLines[stripeCount], Orientation.Horizontal, 0, -10, -11);
          stripeCount++;
        }
      }
    }

    private void BuildVerticalLines()
    {
      if (XAxis != null)
      {
        int index = 0;
        double lastX = 0;
        int gridLineCount = 0;
        int stripeCount = 0;
        if (_verticalStripeBrushes.Count > 0)
        {
          double firstLogicalPosition = XAxis.ConvertPhysicalToLogical(0);
          if (XAxis.MajorTickMarks.Count > 0 && (XAxis.MajorTickMarks[0].RenderTransform as TranslateTransform).X == 0)
          {
            firstLogicalPosition = XAxis.LogicalMajorTickMarkPositions[0];
          }
          if (XAxis.IsTickAndLabelLayoutDifferent)
          {
            firstLogicalPosition += XAxis.ActualMajorTickMarkSpacing / 2.0;
          }
          index = (int)Math.Abs(Math.Floor(firstLogicalPosition / XAxis.ActualMajorTickMarkSpacing) % _verticalStripeBrushes.Count);
        }
        for (int xIndex = 0; xIndex <= XAxis.MajorTickMarks.Count; xIndex++)
        {
          if (xIndex == XAxis.MajorTickMarks.Count || XAxis.MajorTickMarks[xIndex].Visibility == Visibility.Visible)
          {
            double x = 0;
            if (xIndex != XAxis.MajorTickMarks.Count)
            {
              TranslateTransform transform = XAxis.MajorTickMarks[xIndex].RenderTransform as TranslateTransform;
              x = transform.X;
            }
            else
            {
              x = ActualWidth;
            }
            if (_verticalStripeBrushes.Count > 0)
            {
              if (x > 0)
              {
                AddStripeLine(_verticalStripeLines, stripeCount, Orientation.Vertical, index, x, lastX);
                stripeCount++;
                index = ++index % _verticalStripeBrushes.Count;
              }
            }
            AddGridLine(_verticalGridLines, xIndex, Orientation.Vertical, x, VerticalGridLineBrush, VerticalDashArray);

            lastX = x;
            gridLineCount = xIndex;
          }
        }
        RenderMinorVerticalLines();
        while (_verticalGridLines.Count > gridLineCount)
        {
          UpdateGridLine(_verticalGridLines[gridLineCount], Orientation.Vertical, -10);
          gridLineCount++;
        }
        while (_verticalStripeLines.Count > stripeCount)
        {
          UpdateStripeLine(_verticalStripeLines[stripeCount], Orientation.Vertical, 0, -10, -11);
          stripeCount++;
        }
      }
    }

    private Polyline AddGridLine(ObservableCollection<Polyline> cache, int lineIndex, Orientation orientation, double position, Brush brush, DoubleCollection dashArray)
    {
      Polyline line = null;
      if (lineIndex < cache.Count)
      {
        line = cache[lineIndex];
        UpdateGridLine(line, orientation, position);
      }
      else
      {
        line = BuildGridLine(orientation, position, brush, dashArray);
        cache.Add(line);
      }
      return line;
    }

    private void UpdateGridLine(Polyline line, Orientation orientation, double position)
    {
      TranslateTransform translation = line.RenderTransform as TranslateTransform;
      if (orientation == Orientation.Horizontal)
      {
        translation.Y = position - (YAxis.AxisSize * YAxis.StackIndex);
        //translation.X = 0.4;
      }
      else
      {
        translation.X = position;
        //translation.Y = 0.5;
      }
    }

    private Polyline BuildGridLine(Orientation orientation, double position, Brush brush, DoubleCollection dashArray)
    {
      Polyline line = new Polyline();
      if (orientation == Orientation.Horizontal)
      {
        line.Points.Add(new Point(0, -0.5));
        line.Points.Add(new Point(5000, -0.5));
        line.RenderTransform = new TranslateTransform() { Y = position - (YAxis.AxisSize * YAxis.StackIndex) };
      }
      else
      {
        line.Points.Add(new Point(0.5, -0.5));
        line.Points.Add(new Point(0.5, 5000));
        line.RenderTransform = new TranslateTransform() { X = position };
      }
      RenderOptions.SetEdgeMode(line, EdgeMode.Aliased);
      line.Stroke = brush;
      line.StrokeThickness = 1;
      Canvas.SetZIndex(line, 100);
      if (dashArray != null)
      {
        DoubleCollection collection = new DoubleCollection();
        foreach (double d in dashArray)
        {
          collection.Add(d);
        }
        line.StrokeDashArray = collection;
      }
      Children.Add(line);
      return line;
    }

    private void AddStripeLine(ObservableCollection<Border> cache, int stripeIndex, Orientation orientation, int brushIndex, double position, double lastPosition)
    {
      if (stripeIndex < cache.Count)
      {
        Border stripe = cache[stripeIndex];
        UpdateStripeLine(stripe, orientation, brushIndex, position, lastPosition);
      }
      else
      {
        BuildStripeLine(orientation, brushIndex, position, lastPosition);
      }
    }

    private void UpdateStripeLine(Border stripe, Orientation orientation, int brushIndex, double position, double lastPosition)
    {
      if (orientation == Orientation.Horizontal)
      {
        stripe.Background = _horizontalStripeBrushes[brushIndex];
        stripe.Width = ActualWidth;
        stripe.Height = Math.Max(lastPosition - Math.Round(position), 1);
        Canvas.SetTop(stripe, position);
      }
      else
      {
        stripe.Background = _verticalStripeBrushes[brushIndex];
        stripe.Height = ActualHeight;
        stripe.Width = Math.Max(Math.Round(position) - lastPosition, 0);
        Canvas.SetLeft(stripe, lastPosition);
      }
    }

    private void BuildStripeLine(Orientation orientation, int brushIndex, double position, double lastPosition)
    {
      Border stripe = new Border();
      if (orientation == Orientation.Horizontal)
      {
        stripe.Background = _horizontalStripeBrushes[brushIndex];
        stripe.Width = ActualWidth;
        stripe.Height = Math.Max(lastPosition - Math.Round(position), 1);
        Canvas.SetTop(stripe, position);
        _horizontalStripeLines.Add(stripe);
      }
      else
      {
        stripe.Background = _verticalStripeBrushes[brushIndex];
        stripe.Height = ActualHeight;
        stripe.Width = Math.Max(Math.Round(position) - lastPosition, 0);
        Canvas.SetLeft(stripe, lastPosition);
        _verticalStripeLines.Add(stripe);
      }
      stripe.SnapsToDevicePixels = true;
      Children.Add(stripe);
    }

    private void RenderMinorHorizontalLines()
    {
      /*if (MinorHorizontalGridLineBrush != null && MinorHorizontalGridLineBrush.Opacity != 0)
      {
        double width = Math.Round(ActualWidth);
        double height = Math.Round(ActualHeight);
        for (int yIndex = 0; yIndex < YAxis.MinorTickMarks.Count; yIndex++)
        {
          double y = ActualHeight;
          if (yIndex != YAxis.MinorTickMarks.Count)
          {
            y = YAxis.MinorTickMarks[yIndex].Margin.Bottom;
          }
          double yTick = y;

          Polyline line = new Polyline();
          line.Points.Add(new Point(0, height - yTick - 0.4));
          line.Points.Add(new Point(width, height - yTick - 0.4));
          line.Stroke = MinorHorizontalGridLineBrush;
          line.StrokeThickness = 1;
          //line.SnapsToDevicePixels = true;
          RenderOptions.SetEdgeMode(line, EdgeMode.Aliased);
          Canvas.SetZIndex(line, 50);
          if (MinorHorizontalDashArray != null)
          {
            DoubleCollection collection = new DoubleCollection();
            foreach (double d in MinorHorizontalDashArray)
            {
              collection.Add(d);
            }
            line.StrokeDashArray = collection;
          }
          Children.Add(line);
        }
      }*/
      if (MinorHorizontalGridLineBrush != null && MinorHorizontalGridLineBrush.Opacity != 0)
      {
        int count = 0;
        for (int yIndex = 0; yIndex <= YAxis.MinorTickMarks.Count; yIndex++)
        {
          //if (yIndex == YAxis.MinorTickMarks.Count || YAxis.MinorTickMarks[yIndex].Visibility == Visibility.Visible)
          {
            double y = ActualHeight;
            if (yIndex != YAxis.MinorTickMarks.Count)
            {
              TranslateTransform transform = YAxis.MinorTickMarks[yIndex].RenderTransform as TranslateTransform;
              y = transform.Y;
            }
            AddGridLine(_horizontalMinorGridLines, yIndex, Orientation.Horizontal, ActualHeight + y, MinorHorizontalGridLineBrush, MinorHorizontalDashArray);
            count++;
          }
        }
        while (_horizontalMinorGridLines.Count > count)
        {
          Children.Remove(_horizontalMinorGridLines[_horizontalMinorGridLines.Count - 1]);
          _horizontalMinorGridLines.RemoveAt(_horizontalMinorGridLines.Count - 1);
        }
      }
    }

    private void RenderMinorVerticalLines()
    {
      /*if (MinorVerticalGridLineBrush != null && MinorVerticalGridLineBrush.Opacity != 0)
      {
        double height = Math.Round(ActualHeight);
        for (int xIndex = 0; xIndex < XAxis.MinorTickMarks.Count; xIndex++)
        {
          double x = ActualWidth;
          if (xIndex != XAxis.MinorTickMarks.Count)
          {
            x = XAxis.MinorTickMarks[xIndex].Margin.Left;
          }
          double xTick = x;

          Polyline line = new Polyline();
          line.Points.Add(new Point(xTick + 0.5, 0));
          line.Points.Add(new Point(xTick + 0.5, height));
          line.Stroke = MinorVerticalGridLineBrush;
          line.StrokeThickness = 1;
          //line.SnapsToDevicePixels = true;
          RenderOptions.SetEdgeMode(line, EdgeMode.Aliased);
          Canvas.SetZIndex(line, 50);
          if (MinorVerticalDashArray != null)
          {
            DoubleCollection collection = new DoubleCollection();
            foreach (double d in MinorVerticalDashArray)
            {
              collection.Add(d);
            }
            line.StrokeDashArray = collection;
          }
          Children.Add(line);
        }
      }*/
      if (MinorVerticalGridLineBrush != null && MinorVerticalGridLineBrush.Opacity != 0)
      {
        int count = 0;
        for (int xIndex = 0; xIndex <= XAxis.MinorTickMarks.Count; xIndex++)
        {
          //if (xIndex == XAxis.MinorTickMarks.Count || XAxis.MinorTickMarks[xIndex].Visibility == Visibility.Visible)
          {
            double x = 0;
            if (xIndex != XAxis.MinorTickMarks.Count)
            {
              TranslateTransform transform = XAxis.MinorTickMarks[xIndex].RenderTransform as TranslateTransform;
              x = transform.X;
            }
            AddGridLine(_verticalMinorGridLines, xIndex, Orientation.Vertical, x, MinorVerticalGridLineBrush, MinorVerticalDashArray);
            count++;
          }
        }
        while (_verticalMinorGridLines.Count > count)
        {
          Children.Remove(_verticalMinorGridLines[_verticalMinorGridLines.Count - 1]);
          _verticalMinorGridLines.RemoveAt(_verticalMinorGridLines.Count - 1);
        }
      }
    }

    /// <summary>
    /// Gets the collection of vertical stripe brushes.  These are used in rotation
    /// to fill the vertical stripes of the grid, beginning with the leftmost column.
    /// </summary>
    /// <remarks>Horizontal stripes are drawn over the top of vertical stripes.  Therefore,
    /// an opaque horizontal brush will obscure the vertical stripe color where the stripes
    /// overlap.</remarks>
    public Collection<Brush> VerticalStripeBrushes
    {
      get { return _verticalStripeBrushes; }
    }

    /// <summary>
    /// Gets the collection of horizontal stripe brushes.  These are used in rotation
    /// to fill the horizontal stripes of the grid, beginning with the bottom row.
    /// </summary>
    /// <remarks>Horizontal stripes are drawn over the top of vertical stripes.  Therefore,
    /// an opaque horizontal brush will obscure the vertical stripe color where the stripes
    /// overlap.</remarks>
    public Collection<Brush> HorizontalStripeBrushes
    {
      get { return _horizontalStripeBrushes; }
    }

    #region XAxis property

    /// <summary>
    /// Gets or sets the X axis used by the <see cref="ChartGrid"/>.
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
      DependencyProperty.Register("XAxis", typeof(ChartAxis), typeof(ChartGrid),
      new PropertyMetadata(new PropertyChangedCallback(OnXAxisChanged)));

    private static void OnXAxisChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ChartGrid)d).OnXAxisChanged(e);
    }

    private void OnXAxisChanged(DependencyPropertyChangedEventArgs e)
    {
      ChartAxis oldAxis = e.OldValue as ChartAxis;
      if (oldAxis != null)
      {
        oldAxis.AxisRendered -= new EventHandler(XAxis_AxisUpdated);
      }
      if (XAxis != null)
      {
        XAxis.AxisRendered += new EventHandler(XAxis_AxisUpdated);
        BuildVerticalLines();
      }
    }

    /*private void XAxis_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      // TODO: doing this for both X and Y axis is going to build the grid lines twice every time the size changes...
      if (e.PropertyName.Equals("MajorTickMarks") || e.PropertyName.Equals("MinorTickMarks"))
      {
        BuildGridLines();
      }
    }*/

    private void XAxis_AxisUpdated(object sender, EventArgs e)
    {
      BuildVerticalLines();
      foreach (Border stripe in _horizontalStripeLines)
      {
        stripe.Width = ActualWidth;
      }
    }

    #endregion // XAxis property

    #region YAxis property

    /// <summary>
    /// Gets or sets the Y axis used by the <see cref="ChartGrid"/>.
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
      DependencyProperty.Register("YAxis", typeof(ChartAxis), typeof(ChartGrid),
      new PropertyMetadata(new PropertyChangedCallback(OnYAxisChanged)));

    private static void OnYAxisChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ChartGrid)d).OnYAxisChanged(e);
    }

    private void OnYAxisChanged(DependencyPropertyChangedEventArgs e)
    {
      ChartAxis oldAxis = e.OldValue as ChartAxis;
      if (oldAxis != null)
      {
        oldAxis.AxisRendered -= new EventHandler(YAxis_AxisUpdated);
      }
      if (YAxis != null)
      {
        YAxis.AxisRendered += new EventHandler(YAxis_AxisUpdated);
        BuildHorizontalLines();
      }
    }

    /*private void YAxis_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      // TODO: doing this for both X and Y axis is going to build the grid lines twice every time the size changes...
      if (e.PropertyName.Equals("MajorTickMarks") || e.PropertyName.Equals("MinorTickMarks"))
      {
        BuildGridLines();
      }
    }*/

    private void YAxis_AxisUpdated(object sender, EventArgs e)
    {
      BuildHorizontalLines();
      foreach (Border stripe in _verticalStripeLines)
      {
        stripe.Height = ActualHeight;
      }
    }

    #endregion // YAxis property

    #region VerticalGridLineBrush property

    /// <summary>
    /// Gets or sets the brush applied to vertical grid lines.
    /// This is a dependency property.
    /// </summary>
    public Brush VerticalGridLineBrush
    {
      get { return (Brush)GetValue(VerticalGridLineBrushProperty); }
      set { SetValue(VerticalGridLineBrushProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="VerticalGridLineBrush"/> property.
    /// </summary>
    public static readonly DependencyProperty VerticalGridLineBrushProperty =
      DependencyProperty.Register("VerticalGridLineBrush", typeof(Brush), typeof(ChartGrid),
      new PropertyMetadata(new SolidColorBrush(DefaultGridLineColor), new PropertyChangedCallback(OnVerticalGridLineBrushChanged)));

    private static void OnVerticalGridLineBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ChartGrid)d).OnVerticalGridLineBrushChanged();
    }

    private void OnVerticalGridLineBrushChanged()
    {
      foreach (Polyline line in _verticalGridLines)
      {
        line.Stroke = VerticalGridLineBrush;
      }
    }

    #endregion // VerticalGridLineBrush property

    #region HorizontalGridLineBrush property

    /// <summary>
    /// Gets or sets the brush applied to horizontal grid lines.
    /// This is a dependency property.
    /// </summary>
    public Brush HorizontalGridLineBrush
    {
      get { return (Brush)GetValue(HorizontalGridLineBrushProperty); }
      set { SetValue(HorizontalGridLineBrushProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="HorizontalGridLineBrush"/> property.
    /// </summary>
    public static readonly DependencyProperty HorizontalGridLineBrushProperty =
      DependencyProperty.Register("HorizontalGridLineBrush", typeof(Brush), typeof(ChartGrid),
      new PropertyMetadata(new SolidColorBrush(DefaultGridLineColor), new PropertyChangedCallback(OnHorizontalGridLineBrushChanged)));

    private static void OnHorizontalGridLineBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ChartGrid)d).OnHorizontalGridLineBrushChanged();
    }

    private void OnHorizontalGridLineBrushChanged()
    {
      foreach (Polyline line in _horizontalGridLines)
      {
        line.Stroke = HorizontalGridLineBrush;
      }
    }

    #endregion // HorizontalGridLineBrush property

    #region VerticalDashArray property

    /// <summary>
    /// Gets or sets the dash array used by vertical grid lines.
    /// This is a dependency property.
    /// </summary>
    public DoubleCollection VerticalDashArray
    {
      get { return (DoubleCollection)GetValue(VerticalDashArrayProperty); }
      set { SetValue(VerticalDashArrayProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="VerticalDashArray"/> property.
    /// </summary>
    public static readonly DependencyProperty VerticalDashArrayProperty =
      DependencyProperty.Register("VerticalDashArray", typeof(DoubleCollection), typeof(ChartGrid),
      new PropertyMetadata(new PropertyChangedCallback(OnVerticalDashArrayChanged)));

    private static void OnVerticalDashArrayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ChartGrid)d).OnVerticalDashArrayChanged();
    }

    private void OnVerticalDashArrayChanged()
    {
      if (VerticalDashArray != null)
      {
        foreach (Polyline line in _verticalGridLines)
        {
          DoubleCollection collection = new DoubleCollection();
          foreach (double d in VerticalDashArray)
          {
            collection.Add(d);
          }
          line.StrokeDashArray = collection;
        }
      }
    }

    #endregion // VerticalDashArray property

    #region HorizontalDashArray property

    /// <summary>
    /// Gets or sets the dash array used by horizontal grid lines.
    /// This is a dependency property.
    /// </summary>
    public DoubleCollection HorizontalDashArray
    {
      get { return (DoubleCollection)GetValue(HorizontalDashArrayProperty); }
      set { SetValue(HorizontalDashArrayProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="HorizontalDashArray"/> property.
    /// </summary>
    public static readonly DependencyProperty HorizontalDashArrayProperty =
      DependencyProperty.Register("HorizontalDashArray", typeof(DoubleCollection), typeof(ChartGrid),
      new PropertyMetadata(new PropertyChangedCallback(OnHorizontalDashArrayChanged)));

    private static void OnHorizontalDashArrayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ChartGrid)d).OnHorizontalDashArrayChanged();
    }

    private void OnHorizontalDashArrayChanged()
    {
      if (HorizontalDashArray != null)
      {
        foreach (Polyline line in _horizontalGridLines)
        {
          DoubleCollection collection = new DoubleCollection();
          foreach (double d in HorizontalDashArray)
          {
            collection.Add(d);
          }
          line.StrokeDashArray = collection;
        }
      }
    }

    #endregion // HorizontalDashArray property

    #region MinorVerticalGridLineBrush property

    /// <summary>
    /// Gets or sets the MinorVerticalGridLineBrush.
    /// This is a dependency property.
    /// </summary>
    public Brush MinorVerticalGridLineBrush
    {
      get { return (Brush)GetValue(MinorVerticalGridLineBrushProperty); }
      set { SetValue(MinorVerticalGridLineBrushProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="MinorVerticalGridLineBrush"/> property.
    /// </summary>
    public static readonly DependencyProperty MinorVerticalGridLineBrushProperty =
      DependencyProperty.Register("MinorVerticalGridLineBrush", typeof(Brush), typeof(ChartGrid),
      new PropertyMetadata(new SolidColorBrush(Colors.Transparent) { Opacity = 0 }, new PropertyChangedCallback(OnMinorVerticalGridLineBrushChanged)));

    private static void OnMinorVerticalGridLineBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ChartGrid)d).OnMinorVerticalGridLineBrushChanged();
    }

    private void OnMinorVerticalGridLineBrushChanged()
    {
      foreach (Polyline line in _verticalMinorGridLines)
      {
        line.Stroke = MinorVerticalGridLineBrush;
      }
    }

    #endregion // MinorVerticalGridLineBrush property

    #region MinorHorizontalGridLineBrush property

    /// <summary>
    /// Gets or sets the MinorHorizontalGridLineBrush.
    /// This is a dependency property.
    /// </summary>
    public Brush MinorHorizontalGridLineBrush
    {
      get { return (Brush)GetValue(MinorHorizontalGridLineBrushProperty); }
      set { SetValue(MinorHorizontalGridLineBrushProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="MinorHorizontalGridLineBrush"/> property.
    /// </summary>
    public static readonly DependencyProperty MinorHorizontalGridLineBrushProperty =
      DependencyProperty.Register("MinorHorizontalGridLineBrush", typeof(Brush), typeof(ChartGrid),
      new PropertyMetadata(new SolidColorBrush(Colors.Transparent) { Opacity = 0 }, new PropertyChangedCallback(OnMinorHorizontalGridLineBrushChanged)));

    private static void OnMinorHorizontalGridLineBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ChartGrid)d).OnMinorHorizontalGridLineBrushChanged();
    }

    private void OnMinorHorizontalGridLineBrushChanged()
    {
      foreach (Polyline line in _horizontalMinorGridLines)
      {
        line.Stroke = MinorHorizontalGridLineBrush;
      }
    }

    #endregion // MinorHorizontalGridLineBrush property

    #region MinorVerticalDashArray property

    /// <summary>
    /// Gets or sets the MinorVerticalDashArray.
    /// This is a dependency property.
    /// </summary>
    public DoubleCollection MinorVerticalDashArray
    {
      get { return (DoubleCollection)GetValue(MinorVerticalDashArrayProperty); }
      set { SetValue(MinorVerticalDashArrayProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="MinorVerticalDashArray"/> property.
    /// </summary>
    public static readonly DependencyProperty MinorVerticalDashArrayProperty =
      DependencyProperty.Register("MinorVerticalDashArray", typeof(DoubleCollection), typeof(ChartGrid),
      new PropertyMetadata(new PropertyChangedCallback(OnMinorVerticalDashArrayChanged)));

    private static void OnMinorVerticalDashArrayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ChartGrid)d).OnMinorVerticalDashArrayChanged();
    }

    private void OnMinorVerticalDashArrayChanged()
    {
      if (MinorVerticalDashArray != null)
      {
        foreach (Polyline line in _verticalMinorGridLines)
        {
          DoubleCollection collection = new DoubleCollection();
          foreach (double d in MinorVerticalDashArray)
          {
            collection.Add(d);
          }
          line.StrokeDashArray = collection;
        }
      }
    }

    #endregion // MinorVerticalDashArray property

    #region MinorHorizontalDashArray property

    /// <summary>
    /// Gets or sets the MinorHorizontalDashArray.
    /// This is a dependency property.
    /// </summary>
    public DoubleCollection MinorHorizontalDashArray
    {
      get { return (DoubleCollection)GetValue(MinorHorizontalDashArrayProperty); }
      set { SetValue(MinorHorizontalDashArrayProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="MinorHorizontalDashArray"/> property.
    /// </summary>
    public static readonly DependencyProperty MinorHorizontalDashArrayProperty =
      DependencyProperty.Register("MinorHorizontalDashArray", typeof(DoubleCollection), typeof(ChartGrid),
      new PropertyMetadata(new PropertyChangedCallback(OnMinorHorizontalDashArrayChanged)));

    private static void OnMinorHorizontalDashArrayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ChartGrid)d).OnMinorHorizontalDashArrayChanged();
    }

    private void OnMinorHorizontalDashArrayChanged()
    {
      if (MinorHorizontalDashArray != null)
      {
        foreach (Polyline line in _horizontalMinorGridLines)
        {
          DoubleCollection collection = new DoubleCollection();
          foreach (double d in MinorHorizontalDashArray)
          {
            collection.Add(d);
          }
          line.StrokeDashArray = collection;
        }
      }
    }

    #endregion // MinorHorizontalDashArray property
  }
}
