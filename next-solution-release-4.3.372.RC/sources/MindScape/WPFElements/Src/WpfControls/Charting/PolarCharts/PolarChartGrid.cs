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
using System.Collections.ObjectModel;

namespace Mindscape.WpfElements.Charting
{
  /// <summary>
  /// Renders radial and angular grid lines on a <see cref="PolarChart"/>.
  /// </summary>
  public class PolarChartGrid : Canvas
  {
    private RhoAxis _rhoAxis;
    private ThetaAxis _thetaAxis;

    private ObservableCollection<Brush> _angularStripeBrushes = new ObservableCollection<Brush>();
    private ObservableCollection<Brush> _radialStripeBrushes = new ObservableCollection<Brush>();

    private ObservableCollection<Polyline> _angularGridLines = new ObservableCollection<Polyline>();
    private ObservableCollection<Path> _radialGridLines = new ObservableCollection<Path>();

    private ObservableCollection<Path> _angularStripeLines = new ObservableCollection<Path>();
    private ObservableCollection<Path> _radialStripeLines = new ObservableCollection<Path>();

    private static Color DefaultGridLineColor = new Color() { A = 255, R = 100, G = 100, B = 100 };

    /// <summary>
    /// Initializes a new instance of the <see cref="PolarChartGrid"/> class.
    /// </summary>
    public PolarChartGrid()
    {
      Loaded += new RoutedEventHandler(PolarChartGrid_Loaded);
    }

    private void PolarChartGrid_Loaded(object sender, RoutedEventArgs e)
    {
      PolarChart chart = VisualTreeUtils.FindAncestor<PolarChart>(this);
      if (chart != null)
      {
        if (RhoAxis == null)
        {
          RhoAxis = chart.RhoAxis;
        }
        if (ThetaAxis == null)
        {
          ThetaAxis = chart.ThetaAxis;
        }
      }
    }

    private void BuildRadialLines()
    {
      if (RhoAxis != null)
      {
        int index = 0;
        double lastRadius = 0;
        int gridLineCount = 0;
        int stripeCount = 0;
        double centerX = ActualWidth / 2.0;
        double centerY = ActualHeight / 2.0;
        //double axisRadius = (Math.Min(ActualWidth, ActualHeight) / 2.0);
        for (int rhoIndex = 0; rhoIndex < RhoAxis.MajorTickMarks.Count; rhoIndex++)
        {
          double radius = RhoAxis.MajorTickMarks[rhoIndex];
          if (RadialStripeBrushes.Count > 0)
          {
            if (-lastRadius < ActualHeight)
            {
              AddRadialStripeLine(_radialStripeLines, stripeCount,centerX, centerY, index, radius, lastRadius);
              stripeCount++;
            }
            index = ++index % _radialStripeBrushes.Count;
          }
          AddRadialGridLine(_radialGridLines, rhoIndex, centerX, centerY, radius, RadialGridLineBrush, RadialDashArray);

          lastRadius = radius;
          gridLineCount = rhoIndex + 1;
        }
        while (_radialGridLines.Count > gridLineCount)
        {
          UpdateRadialGridLine(_radialGridLines[gridLineCount], centerX, centerY, 0);
          gridLineCount++;
        }
        while (_radialStripeLines.Count > stripeCount)
        {
          UpdateRadialStripeLine(_radialStripeLines[stripeCount], centerX, centerY, 0, 0, 0);
          stripeCount++;
        }
      }
    }

    private Path AddRadialGridLine(ObservableCollection<Path> cache, int lineIndex, double centerX, double centerY, double radius, Brush brush, DoubleCollection dashArray)
    {
      Path line = null;
      if (lineIndex < cache.Count)
      {
        line = cache[lineIndex];
        UpdateRadialGridLine(line, centerX, centerY, radius);
      }
      else
      {
        line = BuildRadialGridLine(centerX, centerY, radius, brush, dashArray);
        cache.Add(line);
      }
      return line;
    }

    private void UpdateRadialGridLine(Path path, double centerX, double centerY, double radius)
    {
      PathGeometry geo = path.Data as PathGeometry;
      PathFigure figure = geo.Figures[0];
      figure.StartPoint = new Point(centerX, centerY - radius);

      if (RadialLineType == RadialType.Curved)
      {
        ArcSegment arc = figure.Segments[0] as ArcSegment;
        arc.Size = new Size(radius, radius);
        arc.Point = new Point(centerX - 0.1, centerY - radius);
      }
      else if (ThetaAxis != null)
      {
        figure.Segments.Clear();
        foreach (double angle in ThetaAxis.MajorTickMarks)
        {
          Point vector = GeometryUtils.GetPosition(angle, radius);
          LineSegment lineSegmenet = new LineSegment();
          lineSegmenet.Point = new Point(centerX + vector.X, centerY - vector.Y);
          figure.Segments.Add(lineSegmenet);
        }
      }
    }

    private Path BuildRadialGridLine(double centerX, double centerY, double radius, Brush brush, DoubleCollection dashArray)
    {
      Path path = new Path();
      PathGeometry geo = new PathGeometry();
      PathFigure figure = new PathFigure();
      figure.IsClosed = true;
      figure.IsFilled = false;
      figure.StartPoint = new Point(centerX, centerY - radius);

      if (RadialLineType == RadialType.Curved)
      {
        ArcSegment arc = new ArcSegment();
        arc.Size = new Size(radius, radius);
        arc.Point = new Point(centerX - 0.1, centerY - radius);
        arc.RotationAngle = 359.9;
        arc.IsLargeArc = true;
        arc.SweepDirection = SweepDirection.Clockwise;
        figure.Segments.Add(arc);
      }
      else if (ThetaAxis != null)
      {
        foreach (double angle in ThetaAxis.MajorTickMarks)
        {
          Point vector = GeometryUtils.GetPosition(angle, radius);
          LineSegment lineSegmenet = new LineSegment();
          lineSegmenet.Point = new Point(centerX + vector.X, centerY - vector.Y);
          figure.Segments.Add(lineSegmenet);
        }
      }

      geo.Figures.Add(figure);
      path.Data = geo;

      ApplyShapeStyle(path, brush, dashArray);

      Children.Add(path);
      return path;
    }

    private void AddRadialStripeLine(ObservableCollection<Path> cache, int stripeIndex, double centerX, double centerY, int brushIndex, double position, double lastPosition)
    {
      if (stripeIndex < cache.Count)
      {
        Path stripe = cache[stripeIndex];
        UpdateRadialStripeLine(stripe, centerX, centerY, brushIndex, position, lastPosition);
      }
      else
      {
        BuildRadialStripeLine(centerX, centerY, brushIndex, position, lastPosition);
      }
    }

    private void UpdateRadialStripeLine(Path stripe, double centerX, double centerY, int brushIndex, double position, double lastPosition)
    {
      PathGeometry geo = stripe.Data as PathGeometry;
      PathFigure figure = geo.Figures[0];
      double middle = (position + lastPosition) / 2.0;
      double thickness = position - lastPosition;
      figure.StartPoint = new Point(centerX, centerY - middle);

      if (RadialLineType == RadialType.Curved)
      {
        ArcSegment arc = figure.Segments[0] as ArcSegment;
        arc.Size = new Size(middle, middle);
        arc.Point = new Point(centerX - 0.1, centerY - middle);
      }
      else if (ThetaAxis != null)
      {
        figure.Segments.Clear();
        foreach (double angle in ThetaAxis.MajorTickMarks)
        {
          Point vector = GeometryUtils.GetPosition(angle, middle);
          LineSegment lineSegmenet = new LineSegment();
          lineSegmenet.Point = new Point(centerX + vector.X, centerY - vector.Y);
          figure.Segments.Add(lineSegmenet);
        }
      }

      stripe.StrokeThickness = thickness;
      stripe.Stroke = _radialStripeBrushes[brushIndex];
    }

    private void BuildRadialStripeLine(double centerX, double centerY, int brushIndex, double position, double lastPosition)
    {
      Path stripe = new Path();
      stripe.StrokeLineJoin = PenLineJoin.Round;
      PathGeometry geo = new PathGeometry();
      PathFigure figure = new PathFigure();
      figure.IsClosed = true;
      figure.IsFilled = false;
      double middle = (position + lastPosition) / 2.0;
      double thickness = position - lastPosition;
      figure.StartPoint = new Point(centerX, centerY - middle);

      if (RadialLineType == RadialType.Curved)
      {
        ArcSegment arc = new ArcSegment();
        arc.Size = new Size(middle, middle);
        arc.Point = new Point(centerX - 0.1, centerY - middle);
        arc.RotationAngle = 359.9;
        arc.IsLargeArc = true;
        arc.SweepDirection = SweepDirection.Clockwise;
        figure.Segments.Add(arc);
      }
      else if (ThetaAxis != null)
      {
        foreach (double angle in ThetaAxis.MajorTickMarks)
        {
          Point vector = GeometryUtils.GetPosition(angle, middle);
          LineSegment lineSegmenet = new LineSegment();
          lineSegmenet.Point = new Point(centerX + vector.X, centerY - vector.Y);
          figure.Segments.Add(lineSegmenet);
        }
      }

      geo.Figures.Add(figure);
      stripe.Data = geo;

      stripe.StrokeThickness = thickness;
      stripe.Stroke = _radialStripeBrushes[brushIndex];
      _radialStripeLines.Add(stripe);
      Children.Add(stripe);
    }

    private void BuildAngularLines()
    {
      if (ThetaAxis != null)
      {
        int index = 0;
        int gridLineCount = 0;
        int stripeCount = 0;

        double centerX = ActualWidth / 2.0;
        double centerY = ActualHeight / 2.0;
        Point pt1 = new Point(centerX, centerY);
        double axisRadius = (Math.Min(ActualWidth, ActualHeight) / 2.0);

        Point previousPoint = new Point(centerX, centerY - axisRadius);
        double previousAngle = 0;

        for (int thetaIndex = 0; thetaIndex < ThetaAxis.MajorTickMarks.Count + 1; thetaIndex++)
        {
          double angle = thetaIndex < ThetaAxis.MajorTickMarks.Count ? ThetaAxis.MajorTickMarks[thetaIndex] : 359.9;

          Point vector = GeometryUtils.GetPosition(angle, axisRadius);
          Point pt2 = new Point(centerX + vector.X, centerY - vector.Y);

          if (_angularStripeBrushes.Count > 0)
          {
            if (angle > 0)
            {
              AddAngularStripeLine(_angularStripeLines, stripeCount, pt1, pt2, previousPoint, angle - previousAngle, axisRadius, index);
              stripeCount++;
            }
            index = ++index % _angularStripeBrushes.Count;
          }
          if (thetaIndex < ThetaAxis.MajorTickMarks.Count)
          {
            AddAngularGridLine(_angularGridLines, thetaIndex, pt1, pt2, AngularGridLineBrush, AngularDashArray);
            gridLineCount = thetaIndex + 1;
          }

          previousPoint = pt2;
          previousAngle = angle;
        }
        while (_angularGridLines.Count > gridLineCount)
        {
          UpdateAngularGridLine(_angularGridLines[gridLineCount], pt1, pt1);
          gridLineCount++;
        }
        while (_angularStripeLines.Count > stripeCount)
        {
          UpdateAngularStripeLine(_angularStripeLines[stripeCount], pt1, pt1, pt1, 0, 0, 0);
          stripeCount++;
        }
      }
    }

    private Polyline AddAngularGridLine(ObservableCollection<Polyline> cache, int lineIndex, Point center, Point edge, Brush brush, DoubleCollection dashArray)
    {
      Polyline line = null;
      if (lineIndex < cache.Count)
      {
        line = cache[lineIndex];
        UpdateAngularGridLine(line, center, edge);
      }
      else
      {
        line = BuildAngularGridLine(center, edge, brush, dashArray);
        cache.Add(line);
      }
      return line;
    }

    private void UpdateAngularGridLine(Polyline line, Point center, Point edge)
    {
      line.Points.Clear();
      line.Points.Add(center);
      line.Points.Add(edge);
    }

    private Polyline BuildAngularGridLine(Point center, Point edge, Brush brush, DoubleCollection dashArray)
    {
      Polyline line = new Polyline();
      line.Points.Add(center);
      line.Points.Add(edge);
      line.Stroke = AngularGridLineBrush;
      line.StrokeThickness = 1;

      ApplyShapeStyle(line, brush, dashArray);

      Children.Add(line);
      return line;
    }

    private void AddAngularStripeLine(ObservableCollection<Path> cache, int stripeIndex, Point center, Point edgePoint, Point previousEdgePoint, double angle, double radius, int brushIndex)
    {
      if (stripeIndex < cache.Count)
      {
        Path stripe = cache[stripeIndex];
        UpdateAngularStripeLine(stripe, center, edgePoint, previousEdgePoint, angle, radius, brushIndex);
      }
      else
      {
        BuildAngularStripeLine(center, edgePoint, previousEdgePoint, angle, radius, brushIndex);
      }
    }

    private void UpdateAngularStripeLine(Path stripe, Point center, Point edgePoint, Point previousEdgePoint, double angle, double radius, int brushIndex)
    {
      PathGeometry geo = stripe.Data as PathGeometry;
      PathFigure figure = geo.Figures[0];

      figure.Segments.Clear();
      figure.StartPoint = center;

      LineSegment segment1 = new LineSegment();
      segment1.Point = previousEdgePoint;
      figure.Segments.Add(segment1);

      if (RadialLineType == RadialType.Curved)
      {
        ArcSegment arc = new ArcSegment();
        arc.Size = new Size(radius, radius);
        arc.Point = edgePoint;
        arc.RotationAngle = angle;
        arc.IsLargeArc = false;
        arc.SweepDirection = SweepDirection.Clockwise;
        figure.Segments.Add(arc);
      }
      else
      {
        LineSegment segment2 = new LineSegment();
        segment2.Point = edgePoint;
        figure.Segments.Add(segment2);
      }
      
      stripe.Fill = _angularStripeBrushes[brushIndex];
    }

    private void BuildAngularStripeLine(Point center, Point edgePoint, Point previousEdgePoint, double angle, double radius, int brushIndex)
    {
      Path stripe = new Path();
      PathGeometry geo = new PathGeometry();
      PathFigure figure = new PathFigure();

      figure.StartPoint = center;
      figure.IsClosed = true;
      figure.IsFilled = true;

      LineSegment segment1 = new LineSegment();
      segment1.Point = previousEdgePoint;
      figure.Segments.Add(segment1);

      if (RadialLineType == RadialType.Curved)
      {
        ArcSegment arc = new ArcSegment();
        arc.Size = new Size(radius, radius);
        arc.Point = edgePoint;
        arc.RotationAngle = angle;
        arc.IsLargeArc = false;
        arc.SweepDirection = SweepDirection.Clockwise;
        figure.Segments.Add(arc);
      }
      else
      {
        LineSegment segment2 = new LineSegment();
        segment2.Point = edgePoint;
        figure.Segments.Add(segment2);
      }
      
      geo.Figures.Add(figure);
      stripe.Data = geo;

      stripe.Fill = _angularStripeBrushes[brushIndex];
      _angularStripeLines.Add(stripe);
      Children.Add(stripe);
    }

    private void ApplyShapeStyle(Shape shape, Brush brush, DoubleCollection dashArray)
    {
      shape.Stroke = brush;
      shape.StrokeThickness = 1;
      Canvas.SetZIndex(shape, 100);
      if (dashArray != null)
      {
        DoubleCollection collection = new DoubleCollection();
        foreach (double d in dashArray)
        {
          collection.Add(d);
        }
        shape.StrokeDashArray = collection;
      }
    }

    /// <summary>
    /// Gets the collection of angular stripe brushes.  These are used in rotation
    /// to fill the angular sectors of the grid.
    /// </summary>
    /// <remarks>Radial stripes are drawn over the top of angular stripes.  Therefore,
    /// an opaque radial brush will obscure the angular stripe color where the stripes
    /// overlap.</remarks>
    public Collection<Brush> AngularStripeBrushes
    {
      get { return _angularStripeBrushes; }
    }

    /// <summary>
    /// Gets the collection of radial stripe brushes.  These are used in rotation
    /// to fill the radial stripes of the grid.
    /// </summary>
    /// <remarks>Radial stripes are drawn over the top of angular stripes.  Therefore,
    /// an opaque radial brush will obscure the angular stripe color where the stripes
    /// overlap.</remarks>
    public Collection<Brush> RadialStripeBrushes
    {
      get { return _radialStripeBrushes; }
    }

    private RhoAxis RhoAxis
    {
      get { return _rhoAxis; }
      set
      {
        if (_rhoAxis != value)
        {
          if (_rhoAxis != null)
          {
            _rhoAxis.AxisUpdated -= new EventHandler(RhoAxis_AxisUpdated);
          }
          _rhoAxis = value;
          if (_rhoAxis != null)
          {
            _rhoAxis.AxisUpdated += new EventHandler(RhoAxis_AxisUpdated);
            BuildRadialLines();
          }
        }
      }
    }

    private void RhoAxis_AxisUpdated(object sender, EventArgs e)
    {
      BuildRadialLines();
    }

    private ThetaAxis ThetaAxis
    {
      get { return _thetaAxis; }
      set
      {
        if (_thetaAxis != value)
        {
          if (_thetaAxis != null)
          {
            _thetaAxis.AxisUpdated -= new EventHandler(ThetaAxis_AxisUpdated);
          }
          _thetaAxis = value;
          if (_thetaAxis != null)
          {
            _thetaAxis.AxisUpdated += new EventHandler(ThetaAxis_AxisUpdated);
            BuildAngularLines();
            if (RadialLineType == RadialType.Straight)
            {
              BuildRadialLines();
            }
          }
        }
      }
    }

    private void ThetaAxis_AxisUpdated(object sender, EventArgs e)
    {
      BuildAngularLines();
      if (RadialLineType == RadialType.Straight)
      {
        BuildRadialLines();
      }
    }

    #region RadialLineType property

    /// <summary>
    /// Gets or sets whether radial grid lines and stripes lines are curved or straight.
    /// This is a dependency property.
    /// </summary>
    public RadialType RadialLineType
    {
      get { return (RadialType)GetValue(RadialLineTypeProperty); }
      set { SetValue(RadialLineTypeProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="RadialLineType"/> property.
    /// </summary>
    public static readonly DependencyProperty RadialLineTypeProperty =
      DependencyProperty.Register("RadialLineType", typeof(RadialType), typeof(PolarChartGrid),
      new PropertyMetadata(RadialType.Curved, new PropertyChangedCallback(OnRadialLineTypeChanged)));

    private static void OnRadialLineTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((PolarChartGrid)d).OnRadialLineTypeChanged();
    }

    private void OnRadialLineTypeChanged()
    {
      // TODO: update grid
    }

    #endregion // RadialLineType property
    
    #region AngularGridLineBrush property

    /// <summary>
    /// Gets or sets the AngularGridLineBrush.
    /// This is a dependency property.
    /// </summary>
    public Brush AngularGridLineBrush
    {
      get { return (Brush)GetValue(AngularGridLineBrushProperty); }
      set { SetValue(AngularGridLineBrushProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="AngularGridLineBrush"/> property.
    /// </summary>
    public static readonly DependencyProperty AngularGridLineBrushProperty =
      DependencyProperty.Register("AngularGridLineBrush", typeof(Brush), typeof(PolarChartGrid),
      new PropertyMetadata(new SolidColorBrush(DefaultGridLineColor), new PropertyChangedCallback(OnAngularGridLineBrushChanged)));

    private static void OnAngularGridLineBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((PolarChartGrid)d).OnAngularGridLineBrushChanged();
    }

    private void OnAngularGridLineBrushChanged()
    {
      // TODO: apply new brush to all angular grid lines.
    }
    
    #endregion // AngularGridLineBrush property
    
    #region RadialGridLineBrush property
    
    /// <summary>
    /// Gets or sets the RadialGridLineBrush.
    /// This is a dependency property.
    /// </summary>
    public Brush RadialGridLineBrush
    {
      get { return (Brush)GetValue(RadialGridLineBrushProperty); }
      set { SetValue(RadialGridLineBrushProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="RadialGridLineBrush"/> property.
    /// </summary>
    public static readonly DependencyProperty RadialGridLineBrushProperty = 
      DependencyProperty.Register("RadialGridLineBrush", typeof(Brush), typeof(PolarChartGrid),
      new PropertyMetadata(new SolidColorBrush(DefaultGridLineColor), new PropertyChangedCallback(OnRadialGridLineBrushChanged)));
    
    private static void OnRadialGridLineBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((PolarChartGrid)d).OnRadialGridLineBrushChanged();
    }
    
    private void OnRadialGridLineBrushChanged()
    {
      // TODO: apply new brush to all radial grid lines.
    }

    #endregion // RadialGridLineBrush property
    
    #region AngularDashArray property

    /// <summary>
    /// Gets or sets the AngularDashArray.
    /// This is a dependency property.
    /// </summary>
    public DoubleCollection AngularDashArray
    {
      get { return (DoubleCollection)GetValue(AngularDashArrayProperty); }
      set { SetValue(AngularDashArrayProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="AngularDashArray"/> property.
    /// </summary>
    public static readonly DependencyProperty AngularDashArrayProperty =
      DependencyProperty.Register("AngularDashArray", typeof(DoubleCollection), typeof(PolarChartGrid),
      new PropertyMetadata(new PropertyChangedCallback(OnAngularDashArrayChanged)));

    private static void OnAngularDashArrayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((PolarChartGrid)d).OnAngularDashArrayChanged();
    }

    private void OnAngularDashArrayChanged()
    {
      // TODO: apply new dash array to all angular grid lines.
    }

    #endregion // AngularDashArray property

    #region RadialDashArray property

    /// <summary>
    /// Gets or sets the RadialDashArray.
    /// This is a dependency property.
    /// </summary>
    public DoubleCollection RadialDashArray
    {
      get { return (DoubleCollection)GetValue(RadialDashArrayProperty); }
      set { SetValue(RadialDashArrayProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="RadialDashArray"/> property.
    /// </summary>
    public static readonly DependencyProperty RadialDashArrayProperty =
      DependencyProperty.Register("RadialDashArray", typeof(DoubleCollection), typeof(PolarChartGrid),
      new PropertyMetadata(new PropertyChangedCallback(OnRadialDashArrayChanged)));

    private static void OnRadialDashArrayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((PolarChartGrid)d).OnRadialDashArrayChanged();
    }

    private void OnRadialDashArrayChanged()
    {
      // TODO: apply new dash array to all radial grid lines.
    }

    #endregion // RadialDashArray property
  }
}
