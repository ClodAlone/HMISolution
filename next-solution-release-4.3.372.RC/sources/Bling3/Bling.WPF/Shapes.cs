
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Media.Media3D;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using System.Linq;
using Bling.DSL;
using Bling.WPF;
using Bling.Core;
using Bling.Util;

namespace Bling.Shapes {
  public class FunnyRectangle : Shape {
    private readonly QuadraticBezierSegment[] segments = new QuadraticBezierSegment[4];
    private readonly PathGeometry geometry = new PathGeometry();
    private readonly PathFigure figure = new PathFigure();
    protected override Geometry DefiningGeometry {
      get { return geometry; }
    }
    public PointBl Corner(int i) {
      if (i == 0)
        return figure.Bl().StartPoint;
      else return segments[i - 1].Bl().Point2;
    }
    public PointBl MidPoint(int i) {
      return segments[i].Bl().MidPoint[Corner(i), .5];
    }
    public PointBl MidPoint(int i, DoubleBl t) {
      return segments[i].Bl().MidPoint[Corner(i), t];
    }
    public PointBl MidPointX(int i, PointBl p) {
      var c0 = Corner(i);
      var c1 = Corner((i + 1) % segments.Length);

      var l0 = (p - c0).Length;
      var l1 = (p - c1).Length;
      return MidPoint(i, l0 / (l0 + l1));
    }

    public FunnyRectangle() {
      for (int i = 0; i < segments.Length; i++) {
        segments[i] = new QuadraticBezierSegment();
        figure.Segments.Add(segments[i]);
      }
      segments[segments.Length - 1].Bl().Point2.Bind = figure.Bl().StartPoint;
      for (int i = 0; i < segments.Length; i++) {
        MidPoint(i).Bind = (Corner(i) + Corner((i + 1) % segments.Length)) / 2;

      }
      geometry.Figures.Add(figure);
      ClipToBounds = false;
    }
  }
  public class CurvedPolygon : PathShape {
    public readonly static DependencyProperty SmoothnessProperty = "Smoothness".NewProperty<CurvedPolygon, double>(0.8).Property;
    internal readonly static DependencyProperty[] PointProperties = new DependencyProperty[10];
    private int UsePointCount = 0;

    static CurvedPolygon() {
      for (int i = 0; i < PointProperties.Length; i++)
        PointProperties[i] = ("Point" + i).NewProperty<CurvedPolygon, Point>().Property;
    }
    public double Smoothness { 
      get { return (double) GetValue(SmoothnessProperty); } 
      set { SetValue(SmoothnessProperty, value); } 
    }
    public CurvedPolygon(params PointBl[] Points) {
      Init(Points);
    }
    public CurvedPolygon(int Count) {
      UsePointCount = Count;
      var Points = new PointBl[Count];
      CurvedPolygonBl Self = this;
      for (int i = 0; i < Count; i++) {
        Points[i] = Self.Points[i];
      }
      Init(Points);
    }
    internal void Init(params PointBl[] Points) {
      CurvedPolygonBl Self = this;
      Self.Start = (Points[(0 + 1) % Points.Length]);
      for (int i = 0; i < Points.Length; i++) {
        var points = new PointBl[4];
        for (int j = 0; j < points.Length; j++)
          points[j] = Points[(i + j) % Points.Length];
        Self.Segments.Add(Self.Smoothness.PolygonSegment(points.Portable()));
      }
    }
  }
  public abstract partial class CurvedPolygonBl<T, BRAND> : PathShapeBl<T, BRAND>
    where T : CurvedPolygon
    where BRAND : CurvedPolygonBl<T, BRAND> {
    public CurvedPolygonBl(Expr<T> Provider) : base(Provider) { }
    public CurvedPolygonBl(CanvasBl canvas, T shape) : base(canvas, shape) { }
    public DoubleBl Smoothness {
      get { return Underlying.Property<double>(CurvedPolygon.SmoothnessProperty); }
      set { Smoothness.Bind = value; }
    }
    public class PointProperties {
      internal BRAND Target;
      public PointBl this[int Idx] {
        get {
          return Target.Underlying.Property<Point>(CurvedPolygon.PointProperties[Idx]); 
        }
        set { this[Idx].Bind = value; }
      }
    }
    public PointProperties Points { get { return new PointProperties() { Target = (BRAND)this }; } }

  }
  public class CurvedPolygonBl : CurvedPolygonBl<CurvedPolygon, CurvedPolygonBl> {
    public CurvedPolygonBl(Expr<CurvedPolygon> v) : base(v) { }
    public CurvedPolygonBl(Canvas canvas, params PointBl[] Points)
      : base(canvas, new CurvedPolygon(Points)) { }

    public CurvedPolygonBl(Canvas canvas, int Count)
      : base(canvas, new CurvedPolygon(Count)) { }

    static CurvedPolygonBl() {
      Register(v => v);
    }
    public static implicit operator CurvedPolygonBl(Expr<CurvedPolygon> v) { return new CurvedPolygonBl(v); }
    public static implicit operator CurvedPolygonBl(CurvedPolygon v) { return new Constant<CurvedPolygon>(v); }
  }


  public class Triangle : Shape {
    private readonly PathGeometry geometry = new PathGeometry();
    //private readonly PathFigure figure = new PathFigure();
    protected override Geometry DefiningGeometry {
      get { return geometry; }
    }
    public static GetProperty<Triangle,Point> PointAProperty = "PointA".NewProperty<Triangle, Point>();
    public static GetProperty<Triangle,Point> PointBProperty = "PointB".NewProperty<Triangle, Point>();
    public static GetProperty<Triangle,Point> PointCProperty = "PointC".NewProperty<Triangle, Point>();

    public PointBl A { get { return PointAProperty[this]; } set { A.Bind = value; } }
    public PointBl B { get { return PointBProperty[this]; } set { B.Bind = value; } }
    public PointBl C { get { return PointCProperty[this]; } set { C.Bind = value; } }

    public Triangle() {
      this.geometry.Bl().Figures.Add(new PathFigureBl() {
        StartPoint = A,
        Segments = { AddMany = new PathSegmentBl[] { (B).Segment(), C.Segment() }, },
      });
    }
  }
  public class PathShape : Shape {
    public static readonly DependencyProperty StartProperty = "Start".NewProperty<PathShape, Point>().Property;

    public Point Start {
      get { return (Point)GetValue(StartProperty); }
      set { SetValue(StartProperty, value); }
    }

    public readonly PathGeometry Geometry = new PathGeometry();
    public readonly PathFigure Figure = new PathFigure();
    public PathShape() {
      Geometry.Figures.Add(Figure);
      Figure.Bl().StartPoint = (new Constant<PathShape>(this)).Property<Point>(StartProperty).Bl();
    }
    protected override Geometry DefiningGeometry {
      get { return Geometry; }
    }
  }
  public abstract partial class PathShapeBl<T, BRAND> : ShapeBl<T, BRAND>
    where T : PathShape
    where BRAND : PathShapeBl<T, BRAND> {
     public PathShapeBl(Expr<T> Provider) : base(Provider) { }
    public PathShapeBl(CanvasBl canvas, T shape) : base(canvas, shape) {}

    public PointBl Start {
      get { return Underlying.Property<Point>(PathShape.StartProperty); }
      set { Start.Bind = value; }
    }
    public CollectionBl<PathSegment, PathSegmentBl, PathSegmentCollection> Segments {
      get {
        return Map<PathFigure,PathFigureBl>(shape => shape.Figure).Underlying.Property<PathSegmentCollection>(PathFigure.SegmentsProperty);
      }
      set {
        Segments.Bind = value;
      }
    }
  }

  public partial class PathShapeBl : PathShapeBl<PathShape, PathShapeBl>, ICoreBrandT {
    public PathShapeBl(Expr<PathShape> v) : base(v) { }
    public PathShapeBl(Canvas canvas)
      : base(canvas, new PathShape()) {
      Stroke.Brush = Brushes.Black;
      Stroke.Thickness = 1;
    }
    static PathShapeBl() {
      Register(v => v);
    }
    public static implicit operator PathShapeBl(Expr<PathShape> v) { return new PathShapeBl(v); }
    public static implicit operator PathShapeBl(PathShape v) { return new Constant<PathShape>(v); }

  }
  public partial class LineShapeBl : PathShapeBl<PathShape, LineShapeBl>, ICoreBrandT {
    public LineShapeBl(Expr<PathShape> v) : base(v) { }
    public LineShapeBl(Canvas canvas, int SegmentCount) : base(canvas, new PathShape()) {
      Stroke.Brush = Brushes.Black;
      Stroke.Thickness = 1;

      for (int i = 0; i < SegmentCount; i++) {
        var segment = new LineSegmentBl() { Point = Start /* new PointBl(100, 100) * i */, };
        this.Segments.AddOne = segment;
      }
    }
    public PointBl this[int Idx] {
      get {
        return (Idx == 0) ? (Start) : (this.Segments[Idx - 1].Coerce<LineSegmentBl>().Point);
      }
      set { this[Idx].Bind = value; }
    }

    static LineShapeBl() {
      Register(v => new LineShapeBl(v));
    }


    //public static implicit operator LineShapeBl(Expr<PathShape> v) { return new LineShapeBl(v); }
    //public static implicit operator LineShapeBl(PathShape v) { return new Constant<PathShape>(v); }

  }

  /// <summary>
  /// Pie pieces useful in pie charts. 
  /// </summary>
  public class PiePiece : Shape {
    public static readonly DependencyProperty RadiusProperty = "Radius".NewProperty<PiePiece, double>(100d).Property;
    public static readonly DependencyProperty InnerRadiusProperty = "InnerRadius".NewProperty<PiePiece, double>(0d).Property;
    // Wedge angle is set in radians
    public static readonly DependencyProperty WedgeAngleProperty = "WedgeAngle".NewProperty<PiePiece, double>(Math.PI / 2d).Property;
    /// <summary>
    /// Radius of pie piece (100 by default).
    /// </summary>
    public double Radius {
      get { return (double)this.GetValue(RadiusProperty); }
      set { this.SetValue(RadiusProperty, value); }
    }
    /// <summary>
    /// Inner radius of pie piece (0 by default).
    /// </summary>
    public double InnerRadius {
      get { return (double)this.GetValue(InnerRadiusProperty); }
      set { this.SetValue(InnerRadiusProperty, value); }
    }
    /// <summary>
    /// Angle of pie piece's wedge in radians (.5 PI by default).
    /// </summary>
    public double WedgeAngle {
      get { return (double)this.GetValue(WedgeAngleProperty); }
      set { this.SetValue(WedgeAngleProperty, value); }
    }
    private readonly PathGeometry Geometry = new PathGeometry();
    private readonly PathFigure Figure = new PathFigure();
    public PiePiece() {
      PathFigureBl Figure = this.Figure.Bl();
      Geometry.Figures.Add(this.Figure);
      PointBl Center = new PointBl(0, 0);
      Angle2DBl Rotation = 0.PI();
      var self = new Constant<PiePiece>(this);


      var innerArcStartPoint = Center + Rotation.ToPoint * self.Property<double>(InnerRadiusProperty).Bl();
      var innerArcEndPoint = Center + (Rotation + this.Bl().WedgeAngle).ToPoint * self.Property<double>(InnerRadiusProperty).Bl();

      var outerArcStartPoint = Center + Rotation.ToPoint * self.Property<double>(RadiusProperty).Bl();
      var outerArcEndPoint = Center + (Rotation + this.Bl().WedgeAngle).ToPoint * self.Property<double>(RadiusProperty).Bl();

      BoolBl LargeArc = this.Bl().WedgeAngle > 1d.PI();
      PointBl outerArcSize = new PointBl(this.Bl().Radius, this.Bl().Radius);
      PointBl innerArcSize = new PointBl(this.Bl().InnerRadius, this.Bl().InnerRadius);
      Figure.StartPoint = innerArcStartPoint;
      Figure.Segments.AddMany = (new PathSegmentBl[] {
        outerArcStartPoint.Segment(),
        new ArcSegmentBl() {
          Point = outerArcEndPoint,
          Size = outerArcSize, SweepDirection = SweepDirection.Clockwise,
          //IsClockwise = false,
          IsLargeArc = LargeArc,
          RotationAngle = 0d.ToDegrees(),
        }, innerArcEndPoint.Segment(),
      new ArcSegmentBl() {
        Point = innerArcStartPoint,
        Size = innerArcSize, SweepDirection = SweepDirection.Counterclockwise, 
        //IsClockwise = true,
        IsLargeArc = LargeArc,
        RotationAngle = 0d.ToDegrees(),
      }});
    }
    protected override Geometry DefiningGeometry {
      get { return Geometry; }
    }
  }
  /// <summary>
  /// Bling type for pie pieces that are useful in pie charts.
  /// </summary>
  public partial class PiePieceBl : ShapeBl<PiePiece, PiePieceBl> {
    public PiePieceBl(Expr<PiePiece> v) : base(v) { }
    public PiePieceBl(CanvasBl canvas) : base(canvas, new PiePiece()) { }
    static PiePieceBl() {
      Register(v => v);
    }
    public static implicit operator PiePieceBl(Expr<PiePiece> v) { return new PiePieceBl(v); }
    public static implicit operator PiePieceBl(PiePiece v) { return new Constant<PiePiece>(v); }
    /// <summary>
    /// Radius of pie piece (100 by default).
    /// </summary>
    public DoubleBl Radius {
      get { return Underlying.Property<double>(PiePiece.RadiusProperty); }
      set { Radius.Bind = value; }
    }
    /// <summary>
    /// Inner radius of pie piece (0 by default).
    /// </summary>
    public DoubleBl InnerRadius {
      get { return Underlying.Property<double>(PiePiece.InnerRadiusProperty); }
      set { InnerRadius.Bind = value; }
    }
    /// <summary>
    /// Angle of pie piece's wedge in radians (.5 PI by default).
    /// </summary>
    public Angle2DBl WedgeAngle {
      get { return Underlying.Property<double>(PiePiece.WedgeAngleProperty).ToAngle(); }
      set { WedgeAngle.Bind = value; }
    }
  }
  public static class ShapeExtensions {
    public static PathShapeBl Bl(this PathShape element) {
      return (element);
    }
    public static PiePieceBl Bl(this PiePiece element) {
      return (element);
    }
  }


}