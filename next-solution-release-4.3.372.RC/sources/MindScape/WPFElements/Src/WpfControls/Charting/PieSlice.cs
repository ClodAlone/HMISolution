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

namespace Mindscape.WpfElements.Charting
{
  /// <summary>
  /// Represents a data point on a <see cref="PieChart"/>.
  /// </summary>
  public class PieSlice : DataPoint, INotifyPropertyChanged
  {
    private PathGeometry _data;
    private Point _centerPoint;

    static PieSlice()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(PieSlice),
        new FrameworkPropertyMetadata(typeof(PieSlice)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PieSlice"/> class.
    /// <param name="data">The data object that the <see cref="PieSlice"/> plots.</param>
    /// <param name="doughnutScale">The doughnut scale of the <see cref="PieSlice"/>.</param>
    /// </summary>
    internal PieSlice(object data, double doughnutScale)
    {
      DataContext = data;
      DoughnutScale = doughnutScale;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PieSlice"/> class.
    /// </summary>
    /// <param name="data">The data that the <see cref="PieSlice"/> holds.</param>
    internal PieSlice(object data)
    {
      DataContext = data;
    }

    internal double StartAngle { get; set; }

    internal double EndAngle { get; set; }

    #region Size property

    /// <summary>
    /// Gets or sets the radius factor of the <see cref="PieSlice"/>. This is generally a value between 0 and 1.
    /// This property can be set using the RadiusBinding property of a <see cref="PieSeries"/>.
    /// This is a dependency property.
    /// </summary>
    public double RadiusFactor
    {
      get { return (double)GetValue(RadiusFactorProperty); }
      set { SetValue(RadiusFactorProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="RadiusFactor"/> property.
    /// </summary>
    public static readonly DependencyProperty RadiusFactorProperty =
      DependencyProperty.Register("RadiusFactor", typeof(double), typeof(PieSlice),
      new PropertyMetadata(new PropertyChangedCallback(OnRadiusFactorChanged)));

    private static void OnRadiusFactorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((PieSlice)d).OnRadiusFactorChanged();
    }

    private void OnRadiusFactorChanged()
    {
    }

    #endregion // Size property

    #region DataValue property

    /// <summary>
    /// Gets or sets the numerical data value that the <see cref="PieSlice"/> is plotting.
    /// This is a dependency property.
    /// </summary>
    public double DataValue
    {
      get { return (double)GetValue(DataValueProperty); }
      set { SetValue(DataValueProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="DataValue"/> property.
    /// </summary>
    public static readonly DependencyProperty DataValueProperty =
      DependencyProperty.Register("DataValue", typeof(double), typeof(PieSlice),
      new PropertyMetadata(new PropertyChangedCallback(OnDataValueChanged)));

    private static void OnDataValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((PieSlice)d).OnDataValueChanged();
    }

    // This is internal for now because we may want to change the event handler type to include other info such as the old data value.
    internal event EventHandler DataValueChanged;

    private void OnDataValueChanged()
    {
      EventHandler handler = DataValueChanged;
      if (handler != null)
      {
        handler(this, new EventArgs());
      }
    }

    #endregion // DataValue property

    #region PieSeries Property

    /// <summary>
    /// Gets the <see cref="PieSeries"/> that created this <see cref="PieSlice"/>.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="PieSeriesProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public PieSeries PieSeries
    {
      get { return (PieSeries)GetValue(PieSeriesProperty); }
      internal set { SetValue(PieSeriesPropertyKey, value); }
    }

    private static readonly DependencyPropertyKey PieSeriesPropertyKey =
        DependencyProperty.RegisterReadOnly("PieSeries", typeof(PieSeries), typeof(PieSlice), new UIPropertyMetadata(null));

    /// <summary>
    /// Identifies the <see cref="PieSeries"/> property.
    /// </summary>
    public static readonly DependencyProperty PieSeriesProperty =
        PieSeriesPropertyKey.DependencyProperty;

    #endregion // PieSeries Property

    /// <summary>
    /// Gets the percentage of the <see cref="PieSlice"/>.
    /// </summary>
    public double Percentage { get; internal set; }

    /// <summary>
    /// Gets a <see cref="PathGeometry"/> that specifies the shape and position of the <see cref="PieSlice"/>.
    /// </summary>
    public PathGeometry PathData
    {
      get
      {
        //TODO: the performance of this could be slightly improved by returning the _data itself the first time this getter is used.
        return ClonePathGeometry(_data);
      }
      internal set
      {
        _data = value;
        OnPropertyChanged("PathData");
      }
    }

    /// <summary>
    /// Gets the diameter of the pie chart.
    /// </summary>
    public double Diameter { get; internal set; }

    /// <summary>
    /// Gets the radius of the <see cref="PieSlice"/>.
    /// </summary>
    public double Radius { get; internal set; }

    /// <summary>
    /// Gets the position of the center corner of the <see cref="PieSlice"/> relative to the
    /// <see cref="Canvas"/> that the pie chart is being hosted in.
    /// </summary>
    public Point CenterPoint
    {
      get { return _centerPoint; }
      internal set
      {
        _centerPoint = value;
        OnPropertyChanged("CenterPoint");
        OnPropertyChanged("Self");
      }
    }

    /// <summary>
    /// Gets this pie slice. This is useful for binding to this pie slice when the center point changes.
    /// </summary>
    public PieSlice Self { get { return this; } }

    /// <summary>
    /// Gets the doughnut scale of the <see cref="PieSlice"/>.
    /// By multiplying this value with the radius, you get the thickness of the douughnut shape.
    /// </summary>
    public double DoughnutScale { get; internal set; }

    #region ExplodedDistance property

    /// <summary>
    /// Gets or sets the distance of the <see cref="PieSlice"/> from the center of the pie chart.
    /// This is a dependency property.
    /// </summary>
    public double ExplodedDistance
    {
      get { return (double)GetValue(ExplodedDistanceProperty); }
      set { SetValue(ExplodedDistanceProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ExplodedDistance"/> property.
    /// </summary>
    public static readonly DependencyProperty ExplodedDistanceProperty =
      DependencyProperty.Register("ExplodedDistance", typeof(double), typeof(PieSlice),
      new PropertyMetadata(new PropertyChangedCallback(OnExplodedDistanceChanged)));

    private static void OnExplodedDistanceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((PieSlice)d).OnExplodedDistanceChanged();
    }

    /// <summary>
    /// Raised when the exploded distance changes.
    /// </summary>
    public event EventHandler ExplodedDistanceChanged;

    private void OnExplodedDistanceChanged()
    {
      EventHandler handler = ExplodedDistanceChanged;
      if (handler != null)
      {
        handler(this, new EventArgs());
      }
    }

    #endregion // ExplodedDistance property

    #region Title property

    /// <summary>
    /// Gets or sets the title of the <see cref="PieSlice"/>.
    /// This is a dependency property.
    /// </summary>
    public string Title
    {
      get { return (string)GetValue(TitleProperty); }
      set { SetValue(TitleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Title"/> property.
    /// </summary>
    public static readonly DependencyProperty TitleProperty =
      DependencyProperty.Register("Title", typeof(string), typeof(PieSlice),
      new PropertyMetadata(new PropertyChangedCallback(OnTitleChanged)));

    private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((PieSlice)d).OnTitleChanged();
    }

    private void OnTitleChanged()
    {
    }

    #endregion // Title property

    private PathGeometry ClonePathGeometry(PathGeometry geo)
    {
      PathFigure figure = geo.Figures[0];

      PathGeometry geometryClone = new PathGeometry();
      PathFigure figureClone = new PathFigure();
      figureClone.IsClosed = figure.IsClosed;
      figureClone.IsFilled = figure.IsFilled;
      figureClone.StartPoint = figure.StartPoint;

      foreach (PathSegment segment in figure.Segments)
      {
        figureClone.Segments.Add(ClonePathSegment(segment));
      }
      geometryClone.Figures.Add(figureClone);
      return geometryClone;
    }

    private PathSegment ClonePathSegment(PathSegment segment)
    {
      if (segment is LineSegment)
      {
        return CloneLineSegment(segment as LineSegment);
      }
      if (segment is ArcSegment)
      {
        return CloneArcSegment(segment as ArcSegment);
      }
      return null;
    }

    private LineSegment CloneLineSegment(LineSegment segment)
    {
      LineSegment lineSegmentClone = new LineSegment();
      lineSegmentClone.Point = segment.Point;
      return lineSegmentClone;
    }

    private ArcSegment CloneArcSegment(ArcSegment segment)
    {
      ArcSegment arcSegmentClone = new ArcSegment();
      arcSegmentClone.IsLargeArc = segment.IsLargeArc;
      arcSegmentClone.Point = segment.Point;
      arcSegmentClone.RotationAngle = segment.RotationAngle;
      arcSegmentClone.Size = segment.Size;
      arcSegmentClone.SweepDirection = segment.SweepDirection;
      return arcSegmentClone;
    }

    /// <summary>
    /// Raised when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
      PropertyChangedEventHandler handler = PropertyChanged;
      if (handler != null)
      {
        handler(this, new PropertyChangedEventArgs(propertyName));
      }
    }
  }
}
