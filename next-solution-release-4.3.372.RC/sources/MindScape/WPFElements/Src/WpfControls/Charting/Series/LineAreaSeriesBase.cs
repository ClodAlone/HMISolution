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
  /// <summary>
  /// A base class for <see cref="DataSeries"/> that use lines to plot data, such as <see cref="LineSeries"/>.
  /// </summary>
  public abstract class LineAreaSeriesBase : PointSeriesBase
  {
    #region LineStyle property

    /// <summary>
    /// Gets or sets the style to be applied to the line.
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
      DependencyProperty.Register("LineStyle", typeof(Style), typeof(LineAreaSeriesBase),
      new PropertyMetadata(BuildDefaultLineStyle(), new PropertyChangedCallback(OnLineStyleChanged)));

    private static void OnLineStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((LineAreaSeriesBase)d).OnLineStyleChanged();
    }

    private void OnLineStyleChanged()
    {
      // TODO: A better way would be to just apply the new style to the line rather than rebuilding the whole chart.
      RequestRebuild();
    }

    private static Style BuildDefaultLineStyle()
    {
      Style style = new Style(typeof(Path));
      style.Setters.Add(new Setter(Path.StrokeThicknessProperty, 2.0));
      return style;
    }

    #endregion // LineStyle property

    #region DashArray property

    /// <summary>
    /// Gets or sets the dash array for the line.
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
      DependencyProperty.Register("DashArray", typeof(DoubleCollection), typeof(LineAreaSeriesBase),
      new PropertyMetadata(null));

    #endregion // DashArray property

    /// <summary>
    /// Gets whether the <see cref="DataSeries"/> supports sampling for large data sets.
    /// </summary>
    protected override bool SupportsDataSampling
    {
      get { return true; }
    }

    /// <summary>
    /// Returns the logical Y axis value for the given X axis value limited to the currently plotted data.
    /// </summary>
    /// <param name="x">The logical X axis value.</param>
    /// <returns>The logical Y axis value.</returns>
    public virtual double GetY(double x)
    {
      if (LinePath != null)
      {
        PathGeometry geo = LinePath.Data as PathGeometry;
        foreach (PathFigure figure in geo.Figures)
        {
          PolyLineSegment lineSegment = figure.Segments[0] as PolyLineSegment;

          if (lineSegment != null)
          {
            Point lowPoint, highPoint;
            //GetLowAndHigh(0, ItemsSource.Count - 1, x, out lowPoint, out highPoint);
            double physicalX = XAxis.ConvertLogicalToPhysical(x);
            if (lineSegment.Points.Count > 0)
            {
              bool isReversed = false;
              if (lineSegment.Points.Count > 0 && lineSegment.Points[lineSegment.Points.Count - 1].X < figure.StartPoint.X)
              {
                isReversed = true;
              }
              if (isReversed)
              {
                GetLowAndHigh_OnlyVisibleData_Reverse(figure, lineSegment, 0, lineSegment.Points.Count - 1, physicalX, out lowPoint, out highPoint);
              }
              else
              {
                GetLowAndHigh_OnlyVisibleData(figure, lineSegment, 0, lineSegment.Points.Count - 1, physicalX, out lowPoint, out highPoint);
              }
              lowPoint.Y -= 1;
              highPoint.Y -= 1;
              double rise = highPoint.Y - lowPoint.Y;
              double run = highPoint.X - lowPoint.X;
              double value = (physicalX - lowPoint.X) * (rise / run) + lowPoint.Y;
              if (isReversed && physicalX <= lowPoint.X && physicalX >= highPoint.X)
              {
                return YAxis.ConvertPhysicalToLogical(value);
              }
              else if (physicalX >= lowPoint.X && physicalX <= highPoint.X)
              {
                return YAxis.ConvertPhysicalToLogical(value);
              }
            }
          }
        }
      }
      return Double.NaN;
    }

    // Binary search algorithm:
    private static void GetLowAndHigh_OnlyVisibleData(PathFigure figure, PolyLineSegment segment, int startIndex, int endIndex, double target, out Point low, out Point high)
    {
      low = segment.Points[startIndex];
      high = segment.Points[endIndex];
      if (endIndex == startIndex)
      {
        if (target < low.X && startIndex == 0)
        {
          high = low;
          low = figure.StartPoint;
        }
        return;
      }
      if (endIndex - startIndex == 1)
      {
        if (target < low.X && startIndex == 0)
        {
          high = low;
          low = figure.StartPoint;
        }
        return;
      }
      int midIndex = (startIndex + endIndex) / 2;
      Point p = segment.Points[midIndex];
      if (p.X >= target)
      {
        endIndex = midIndex;
      }
      else if (p.X <= target)
      {
        startIndex = midIndex;
      }
      GetLowAndHigh_OnlyVisibleData(figure, segment, startIndex, endIndex, target, out low, out high);
    }

    private static void GetLowAndHigh_OnlyVisibleData_Reverse(PathFigure figure, PolyLineSegment segment, int startIndex, int endIndex, double target, out Point low, out Point high)
    {
      low = segment.Points[startIndex];
      high = segment.Points[endIndex];
      if (endIndex == startIndex)
      {
        if (target > low.X && startIndex == 0)
        {
          high = low;
          low = figure.StartPoint;
        }
        return;
      }
      if (endIndex - startIndex == 1)
      {
        if (target > low.X && startIndex == 0)
        {
          high = low;
          low = figure.StartPoint;
        }
        return;
      }
      int midIndex = (startIndex + endIndex) / 2;
      Point p = segment.Points[midIndex];
      if (p.X <= target)
      {
        endIndex = midIndex;
      }
      else if (p.X >= target)
      {
        startIndex = midIndex;
      }
      GetLowAndHigh_OnlyVisibleData_Reverse(figure, segment, startIndex, endIndex, target, out low, out high);
    }

    // Do not delete this method.
    // Binary search algorithm: This one will be used for tracing all the line data - including anything not being plotted.
    /*private void GetLowAndHigh(int startIndex, int endIndex, double target, out Point low, out Point high)
    {
      low = GetPoint(startIndex);
      high = GetPoint(endIndex);
      if (endIndex - startIndex == 1)
      {
        return;
      }
      int midIndex = (startIndex + endIndex) / 2;
      Point p = GetPoint(midIndex);
      if (p.X >= target)
      {
        endIndex = midIndex;
      }
      else if (p.X <= target)
      {
        startIndex = midIndex;
      }
      GetLowAndHigh(startIndex, endIndex, target, out low, out high);
    }*/

    // TODO: might be able to refactor so we don't need to expose this anymore.
    // Can be overriden to return the PolyLineSegment that renders the data line.
    // This is used by the GetY method.
    internal virtual Path LinePath
    {
      get { return null; }
    }

    // Constructs the main line of this series.
    // Sets up the style of the main line of this series.
    // Attaches event handlers to the main line of this series.
    internal Path BuildLine(Path oldPath)
    {
      // Remove handler
      if (oldPath != null)
      {
        oldPath.MouseLeftButtonDown -= new MouseButtonEventHandler(Path_MouseLeftButtonDown);
        oldPath.MouseRightButtonDown -= new MouseButtonEventHandler(Path_MouseLeftButtonDown);
      }
      // TODO: most of this stuff probably only needs to be set up once, and then modified whenever certain properties change.
      //       At the moment this is called every time the series is rendered.
      Path path = new Path();
      path.DataContext = this;
      path.ToolTip = ToolTip;
      if (LineStyle != null)
      {
        path.Style = LineStyle;
      }
      if (DashArray != null)
      {
        DoubleCollection collection = new DoubleCollection();
        foreach (double d in DashArray)
        {
          collection.Add(d);
        }
        path.StrokeDashArray = collection;
      }
      if (path.Stroke == null && SeriesBrush != null)
      {
        path.Stroke = SeriesBrush;
      }
      // Add handler
      path.MouseLeftButtonDown += new MouseButtonEventHandler(Path_MouseLeftButtonDown);
      path.MouseRightButtonDown += new MouseButtonEventHandler(Path_MouseLeftButtonDown);
      // Add to canvas
      Canvas.Children.Add(path);
      return path;
    }

    internal void UpdatePath(Path path)
    {
      path.ToolTip = ToolTip;
      if (LineStyle != null)
      {
        path.Style = LineStyle;
      }
      if (DashArray != null)
      {
        DoubleCollection collection = new DoubleCollection();
        foreach (double d in DashArray)
        {
          collection.Add(d);
        }
        path.StrokeDashArray = collection;
      }
      else
      {
        path.SetValue(Path.StrokeDashArrayProperty, DependencyProperty.UnsetValue);
        //path.StrokeDashArray = null;
      }
      if (path.Stroke != SeriesBrush && SeriesBrush != null)
      {
        path.Stroke = SeriesBrush;
      }
      // Add to canvas
      Canvas.Children.Add(path);
    }

    internal Path BuildSelectionLine(Path oldLine)
    {
      if (oldLine != null)
      {
        oldLine.MouseLeftButtonDown -= new MouseButtonEventHandler(Path_MouseLeftButtonDown);
        oldLine.MouseRightButtonDown -= new MouseButtonEventHandler(Path_MouseLeftButtonDown);
      }

      Path path = new Path();
      path.DataContext = this;
      path.MouseLeftButtonDown += new MouseButtonEventHandler(Path_MouseLeftButtonDown);
      path.MouseRightButtonDown += new MouseButtonEventHandler(Path_MouseLeftButtonDown);
      path.Stroke = Brushes.Transparent;
      path.StrokeThickness = 7;
      Canvas.Children.Add(path);
      return path;
    }

    internal void Path_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      if (e.ChangedButton == MouseButton.Left || (Chart != null && Chart.IsRightClickSelectionEnabled))
      {
        if (Chart == null || Chart.CanToggleSelection)
        {
          IsSelected = !IsSelected;
        }
        else
        {
          IsSelected = true;
        }
      }
    }
  }
}
