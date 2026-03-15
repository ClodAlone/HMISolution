using System;
using System.Collections.Generic;
using Bling.DSL;
using Bling.Util;
using Bling.Core;
using linq = Microsoft.Linq.Expressions;
using Bling.Matrices;

namespace Bling.Core {
  public abstract partial class BaseNumericBl<T, K, BRAND, BOOL, MONO> : BaseNumericBl<T, K, BRAND, MONO>, INumericBl<BRAND, BOOL>
    where BRAND : BaseNumericBl<T, K, BRAND, BOOL, MONO>
    where BOOL : IBoolBl<BOOL> 
    where MONO : BaseNumericBl<K, K, MONO, BoolBl, MONO> {
    /// <summary>
    /// Determine if this value is between min and max.
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns>True iff this is greater than min and less than max.</returns>
    public BOOL Between(BRAND min, BRAND max) {
      return (this >= min).And(this <= max).Or(this >= max).And(this <= min);
    }
  }
  static class BezierFactory<BRAND> where BRAND : IDoubleBl<BRAND> {
    public static Func<DoubleBl, IntBl, BRAND[], BRAND> F;
    public static Func<DoubleBl, BRAND[], BRAND> G;
  }

  public abstract partial class BaseDoubleBl<T, BRAND, BOOL> :
    BaseDoubleLikeBl<T, BRAND, BOOL>, IDoubleBl<BRAND, BOOL>, IBaseDoubleBl<T, BRAND>
    where BRAND : BaseDoubleBl<T, BRAND, BOOL>
    where BOOL : IBoolBl<BOOL> {
    public BRAND BezierTerm(DoubleBl t, int n, int i) {
      return this * t.Bernstein(n, i);
    }
    private static int InitGeometry() {
      BezierFactory<BRAND>.F = Bezier;
      BezierFactory<BRAND>.G = Bezier;
      return 0;
    }
    private static readonly int InitGeometry0 = InitGeometry();

    /// <summary>
    /// Calculate the value along a Bezier path at percent t.
    /// </summary>
    /// <param name="t">Perect progress along the Bezier path.</param>
    /// <param name="first">What term should occur first (object affects inversion).</param>
    /// <param name="p">the control points</param>
    /// <returns></returns>
    public static BRAND Bezier(DoubleBl t, IntBl first, params BRAND[] p) {
      BRAND result = ToBrand(new Constant<T>(default(T)));
      for (int i = 0; i < p.Length; i++) {
        var j = (i + first) % p.Length;
        result += p.Table(j) * t.Bernstein(p.Length - 1, j);
      }
      return result;
    }
    public static BRAND Bezier(DoubleBl t, params BRAND[] ps) {
      var qs = new BRAND[ps.Length];
      for (int i = 0; i < ps.Length; i++)
        qs[i] = ps[i];
      ps = qs;
      for (int i = 1; i < ps.Length; i++)
        for (int j = 0; j < ps.Length - i; j++)
          ps[j] = ps[j] * (1 - t) + ps[j + 1] * t;

      return ps[0];
    }
    /// <summary>
    /// Simulate momentum using Verlet. 
    /// </summary>
    /// <param name="loss">Percent of momentum that is lost per step; e.g., .01 causes 1% loss in momentum per time step.</param>
    /// <param name="OldValue">Previous computed value.</param>
    /// <returns>New now value.</returns>
    public BRAND VerletStep(DoubleBl loss, BRAND OldValue) {
      return this * (2d - loss) - OldValue * (1d - loss);
    }
    public void InstallVerletStep(DoubleBl loss) {
      this.Step = (newV, oldV) => newV.VerletStep(loss, oldV);
    }



    private partial class SpringOp : Ops.StaticCallOperator<T, T, double, T, SpringOp> {
      private SpringOp() { }
      public new static readonly SpringOp Instance = new SpringOp();
      protected override string CallName { get { return "Spring"; } }
      protected override Type TargetType { get { return typeof(Math); } }
      public override Expr<T> Expand(Expr<T> argA, Expr<T> argB, Expr<double> argC) {
        BRAND At = ToBrand(argA);
        BRAND Center = ToBrand(argB);
        DoubleBl RestLength = argC;
        BRAND v = (At) - (Center);
        var l = v.Length;
        v = (l == 0).Condition(Unit, v);
        l = l.Max(1d);
        v = v / l * RestLength;
        return v + (Center);
      }
      public override Expr<double> InverseC(Expr<T> argA, Expr<T> argB, Expr<double> argC, Expr<T> result) {
        BRAND At = ToBrand(argA);
        BRAND Center = ToBrand(argB);
        DoubleBl RestLength = argC;
        if (At.Underlying.Equals(result)) 
          return At.Distance(Center);
        else return base.InverseC(argA, argB, argC, result);
      }
    }
    private partial class DistanceOp : Ops.StaticCallOperator<T, T, double, DistanceOp> {
      private DistanceOp() { }
      public new static readonly DistanceOp Instance = new DistanceOp();
      protected override string CallName { get { return "Distance"; } }
      protected override Type TargetType { get { return typeof(Math); } }
      public override Expr<double> Expand(Expr<T> argA, Expr<T> argB) {
        var a = ToBrand(argA);
        var b = ToBrand(argB);
        return (a - b).Length;
      }
      public override Expr<T> InverseA(Expr<T> argA, Expr<T> argB, Expr<double> result) {
        var a = ToBrand(argA);
        var b = ToBrand(argB);
        return a.Spring(b, result.Bl());
      }
      public override Expr<T> InverseB(Expr<T> argA, Expr<T> argB, Expr<double> result) {
        var a = ToBrand(argA);
        var b = ToBrand(argB);
        return b.Spring(a, result.Bl());
      }
    }
    public DoubleBl Distance(BRAND other) { return DistanceOp.Instance.Make(this, other); }
    public BRAND Spring(BRAND Center, DoubleBl RestLength) {
      return ToBrand(SpringOp.Instance.Make(this, Center, RestLength));
    }
  }
  public abstract partial class PointBl<T, BRAND, ARITY> : MultiDoubleBl<T, BRAND, Vecs.Vec2<bool>, Bool2Bl, ARITY>, HasXY<DoubleBl>
    where BRAND : PointBl<T, BRAND, ARITY>
    where ARITY : Vecs.Arity2<double, T, ARITY>, new() {
    public BRAND ClockwisePerp {
      get { return ToBrand(Vecs.Arity2<double,T,ARITY>.ArityI.Make(-Y, X)); }
    }
    /// <summary>
    /// To determine where line(P1,P2) and line(P3,P4) intersect
    /// </summary>
    /// <param name="P1">the start point of the first line</param>
    /// <param name="P2">the end point of the first line</param>
    /// <param name="P3">the start point of the second line</param>
    /// <param name="P4">the end point of the second line</param>
    /// <returns></returns>
    public static BRAND Intersect(BRAND P1, BRAND P2, BRAND P3, BRAND P4) {
      var ua = (P4.X - P3.X) * (P1.Y - P3.Y) - (P4.Y - P3.Y) * (P1.X - P3.X);
      var div = ((P4.Y - P3.Y) * (P2.X - P1.X) - (P4.X - P3.X) * (P2.Y - P1.Y));
      ua = ua / div;
      var ret = P1 + ((P2 - P1) * ua);
      ret = (ua < 0 | ua > 1 | ua.IsNaN | !ret.Between(P3, P4)).Condition(NaN, ret);
      return ret;
    }
    public static BRAND IntersectB(BRAND P1, BRAND P2, BRAND P3, BRAND P4) {
      var ua = (P4.X - P3.X) * (P1.Y - P3.Y) - (P4.Y - P3.Y) * (P1.X - P3.X);
      var div = ((P4.Y - P3.Y) * (P2.X - P1.X) - (P4.X - P3.X) * (P2.Y - P1.Y));
      ua = ua / div;
      var ret = P1 + ((P2 - P1) * ua);
      return ret;
    }


    /// <summary>
    /// to determine if this point and the other one are at the same side of line(a,b)
    /// </summary>
    /// <param name="other">to decide if the other point is at the same side of this point</param>
    /// <param name="a">the start point of the line</param>
    /// <param name="b">the end point of the line</param>
    /// <returns></returns>
    public BoolBl SameSide(BRAND other, BRAND a, BRAND b) {
      var ba = (b - a);
      var s0 = ba.Cross(this - a).Sign;
      var s1 = ba.Cross(other - a).Sign;
      return s0 == 0 | s1 == 0 | s0 == s1;
    }
    /// <summary>
    /// determine if this point within the triangle ABC
    /// </summary>
    /// <param name="A">the first vertex of triangle</param>
    /// <param name="B">the second vertex of triangle</param>
    /// <param name="C">the third vertex of triangle</param>
    /// <returns></returns>
    public BoolBl Within(BRAND A, BRAND B, BRAND C) {
      return this.SameSide(A, B, C) & (this.SameSide(B, A, C)) & (this.SameSide(C, A, B));
    }
    /// <summary>
    /// to determine if this point is inside the convex consists of P1, P2, P3 and P4 in 
    /// clockwise order or in conter-clockwise order    
    /// </summary>
    /// <param name="P1"></param>
    /// <param name="P2"></param>
    /// <param name="P3"></param>
    /// <param name="P4"></param>
    /// <returns></returns>        
    public BoolBl Within(BRAND P1, BRAND P2, BRAND P3, BRAND P4) {
      return (this.SameSide(P4, P1, P2) &
        this.SameSide(P1, P2, P3) &
        this.SameSide(P2, P3, P4) &
        this.SameSide(P3, P4, P1));
    }

    /// <summary>
    /// to rotate this point with a specified angle around the specified center point
    /// </summary>
    /// <param name="center">the center point of rotation</param>
    /// <param name="angle">the angle to rotate</param>
    /// <returns></returns>
    public BRAND Rotate(BRAND center, Angle2DBl angle) {
      var v = this - center;
      var a = v.Angle + angle;
      return ((PointBl<T,BRAND,ARITY>) (center.Portable + a.ToPoint * v.Length));
    }
    /// <summary>
    /// return the intersecting point of the height of the triangle (this,B,C) from this point to the base (B,C) and the base (B,C) 
    /// </summary>
    /// <param name="B">the start point of the base of the triangle</param>
    /// <param name="C">the end point of the base of the triangle</param>
    /// <returns></returns>
    public BRAND TriangleBase(BRAND B, BRAND C) {
      var A = this.Portable;
      var angle = (C - B).Angle.Inverse; 
      var Bx = B.Portable.Rotate(A, angle);
      var Hx = new PointBl(A.X, Bx.Y);
      var H = Hx.Rotate(A, angle.Inverse);
      return ((PointBl<T,BRAND,ARITY>) H);
    }
    public DoubleBl TriangleRatio(BRAND Start, BRAND Mid, BRAND End) {
      var areaA = this.TriangleArea(Mid, Start).Sqrt;
      var areaB = this.TriangleArea(Mid, End).Sqrt;
      var sum = areaA + areaB;
      return areaA / (areaA + areaB);
    }

    /// <summary>
    /// return the area of triangle ABC
    /// </summary>
    /// <param name="A">the first vertex of triangle</param>
    /// <param name="B">the second vertex of triangle</param>
    /// <param name="C">the third vertex of triangle</param>
    /// <returns></returns>
    public DoubleBl TriangleArea(BRAND B, BRAND C) {
      BRAND A = this;
      var t0 = (C.X - A.X) * (B.Y - A.Y);
      var t1 = (C.Y - A.Y) * (B.X - A.X);

      var t3 = (C - A) * (B - A);


      return .5 * (t0 - t1).Abs;

    }

    /// <summary>
    /// return the distance of this point to the line denoted by point start to end
    /// </summary>
    /// <param name="start">the start point of line</param>
    /// <param name="end">the end point of line</param>    
    /// <returns></returns>    
    public DoubleBl Distance(BRAND start, BRAND end) {
      var lineDir = (end - start).Normalize;

      var lineDirP = Make(-lineDir.Y, lineDir.X); //90 degree rotation with clockwise
      var lineC = -lineDirP.Dot(start);

      var distance = (this.Dot(lineDirP) + lineC);

      return distance;
    }

    public BRAND MirrorPoint(BRAND start, BRAND end) {
      var lineDir = (end - start).Normalize;

      var lineDirP = Make(-lineDir.Y, lineDir.X);
      var lineC = -lineDirP.Dot(start);

      var distance = (this.Dot(lineDirP) + lineC);

      var mirrorPoint = this - (2 * distance) * lineDirP;

      return mirrorPoint;
    }

    /// <summary>
    /// Translate this point based on four points of a quadrilateral as if the quadrilateral was a distorted rectangle.
    /// </summary>
    /// <param name="area">Total area of quadrilateral, defined as TriangleArea(points[0], points[1], points[2]) + TriangleArea(points[2], points[3], points[0]).</param>
    /// <param name="isIn">Whether or not this point is in the quadrilateral.</param>
    /// <param name="points">Four points that define the north, east, south, and west corner of the quadrilateral</param>
    /// <returns>A translated point as if the quadrilateral was a distorted rectangle.</returns>
    public BRAND Distort(DoubleBl area, out BoolBl isIn, params BRAND[] points) {
      var uv = this;
      var north = uv.TriangleArea(points[0], points[1]);
      var east = uv.TriangleArea(points[1], points[2]);
      var south = uv.TriangleArea(points[2], points[3]);
      var west = uv.TriangleArea(points[3], points[0]);
      var y = north / (north + south);
      var x = west / (west + east);
      var size = north + south + west + east;
      isIn = size <= area + .0000001;
      return Make(x, y);
    }
    public BRAND Distort(DoubleBl area, out BoolBl isIn, DoubleBl ctl, params BRAND[] points) {
      var uv = this;
      var north = uv.TriangleArea(points[0], points[1]);
      var east = uv.TriangleArea(points[1], points[2]);
      var south = uv.TriangleArea(points[2], points[3]);
      var west = uv.TriangleArea(points[3], points[0]);
      var y = north / (north + south);
      var x = west / (west + east);
      var size = north + south + west + east;
      isIn = size <= area + .0000001 * ctl;
      return Make(x, y);
    }
    public BRAND Distort(params BRAND[] points) {
      //var uv = this;
      var north = this.TriangleArea(points[0], points[1]);
      var east = this.TriangleArea(points[1], points[2]);
      var south = this.TriangleArea(points[2], points[3]);
      var west = this.TriangleArea(points[3], points[0]);
      var y = north / (north + south);
      var x = west / (west + east);
      return Make(x, y);
    }

    /// <summary>
    /// return the length of the quadBezier curve from this to p2
    /// </summary>
    /// <param name="p1">the control point</param>
    /// <param name="p2">the end point</param>
    /// <returns></returns>
    public DoubleBl QuadBezierLength(BRAND p1, BRAND p2) {
      //var p0 = startPoint;
      //var p1 = source.Point1();
      //var p2 = source.Point2();
      var p0 = this;
      var a = p0 - (p1 * 2) + p2;
      var b = (p1 * 2) - (p0 * 2);

      var A = 4d * ((a.X).Square + (a.Y).Square);
      var B = 4d * ((a.X * b.X) + (a.Y * b.Y));
      var C = (b.X.Square) + (b.Y.Square);

      var Sabc = 2d * (A + B + C).Sqrt;
      var A_2 = A.Sqrt;
      var A_32 = A * A_2 * 2d;
      var C_2 = C.Sqrt * 2d;
      var BA = B / A_2;
      return (A_32 * Sabc + A_2 * B * (Sabc - C_2) + (4 * C * A - B * B) * ((2d * A_2 + BA + Sabc) / (BA + C_2)).Log) / (4 * A_32);
    }

  }
  public partial class DoubleBl : BaseDoubleBl<DoubleBl>, IMonoNumericBl<double, DoubleBl> {
    /// <summary>
    /// Calculate the Bernstein coefficients of degree n.
    /// </summary>
    /// <param name="n"></param>
    /// <param name="i">argument</param>
    /// <returns></returns>
    public DoubleBl Bernstein(IntBl n, IntBl i) {
      var ret = this.Pow((DoubleBl) i) * (1d - this).Pow(n - i) * IntBl.BinomialCoefficient(n, i);
      return (i == 0).Condition((1d - this).Pow(n), ret);
    }
    /// <summary>
    /// calculate the Bezier value with control points p[] in type DoubleBl
    /// </summary>
    public BRAND Bezier<BRAND>(IntBl first, params BRAND[] p) where BRAND : IDoubleBl<BRAND> {
      return BezierFactory<BRAND>.F(this, first, p);
    }
    /// <summary>
    /// calculate the Bezier value with control points p[] in type DoubleBl
    /// </summary>
    public BRAND Bezier<BRAND>(params BRAND[] p) where BRAND : IDoubleBl<BRAND> {
      return BezierFactory<BRAND>.G(this, p);
    }


    private static void Test() {
      ColorBl[] ds = null;
      DoubleBl t = null;

      t.Bezier(0, ds);
      t.Bezier(ds);

    }
  }

  public abstract partial class BaseIntBl<T, BRAND, BOOL> : BaseNumericBl<T, int, BRAND, BOOL, IntBl>, IIntBl<BRAND, BOOL>, IToDoubleBl<BOOL>
    where BRAND : BaseIntBl<T, BRAND, BOOL>
    where BOOL : IBoolBl<BOOL> {
  }
  public partial class IntBl : BaseIntBl<int, IntBl, BoolBl> {
    /// <summary>
    /// Calculate the Binomial coefficients of  n and k:C(n,k).
    /// </summary>
    public static DoubleBl BinomialCoefficient(IntBl n, IntBl k) {
      return n.Factorial / (k.Factorial * (n - k).Factorial);
    }
    public DoubleBl Factorial { get { return Map<double, DoubleBl>(i => Factorial0(i)); } }
    private static double Factorial0(int n) {
      if (n == 0 || n == 1) return 1d;
      return n * Factorial0(n - 1);
    }
  }

  public static partial class GeometryExtensions {
    /// <summary>
    /// Translate 2D point into 3D point based on sphere.
    /// </summary>
    /// <param name="Point"></param>
    /// <param name="Center"></param>
    /// <param name="Radius"></param>
    /// <returns></returns>

    public static Point3DBl TurnToVirtualSphere(this PointBl Point, DoubleBl Radius) {
      Point = new PointBl(Point.X, Radius * 2d - Point.Y);
      var Center = new PointBl(Radius, Radius);
      var xy = (Point - Center) / Radius;
      var l = xy.LengthSquared;
      var z = (l > 1).Condition(0, (1d - l).Sqrt);
      xy = (l > 1).Condition(xy / l.Sqrt, xy);
      return new Point3DBl(xy, z);
    }
    public static QuaternionBl TurnToVirtualSphere(this PointBl oldP, PointBl newP, DoubleBl Radius) {
      var p0 = oldP.TurnToVirtualSphere(Radius);
      var p1 = newP.TurnToVirtualSphere(Radius);
      var axis = p0.Cross(p1);
      var q2 = new QuaternionBl(axis, p0.Dot(p1).Acos);
      return q2;
    }
    public static Point3DBl To3D(this MatrixBl<D4, D4> Transform, Point3DBl p) {
      var p0 = p.XY();
      var Z = p.Z;
      var q = -1d + 2d * (new PointBl(p0.X, 1 - p0.Y));
      var t0 = (Transform).Transform(new Point3DBl(q, Z));//-1
      var t = t0.XY();
      t = (t + 1d) / 2d;
      return new Point3DBl(t.X, (1 - t.Y), t0.Z);
    }
    /// <summary>
    /// Use transform to convert 3D point into screen-like 2D point.
    /// </summary>
    /// <param name="Transform">Transform used to convert...</param>
    /// <param name="p"></param>
    /// <returns></returns>
    public static PointBl To2D(this MatrixBl<D4, D4> Transform, Point3DBl p) {
      var q0 = new PointBl(p.X, 1 - p.Y);
      var q = q0 * 2d - 1d;
      // XXX: weird.
      var t0 = (Transform.Inverse() /*.Transpose */).Transform(new Point3DBl(q, p.Z));
      var t = t0.XY();
      t = (t + 1d) / 2d;
      var p3D = new Point3DBl(t.X, (1 - t.Y), t0.Z);
      return p3D.XY();
    }
    public static PointBl To2D0(this MatrixBl<D4, D4> TransformI, Point3DBl p) {
      var q0 = new PointBl(p.X, 1 - p.Y);
      var q = q0 * 2 - 1;
      var t0 = (TransformI).Transform(new Point3DBl(q, p.Z));
      var t = t0.XY();
      t = (t + 1d) / 2d;
      var p3D = new Point3DBl(t.X, (1 - t.Y), t0.Z);
      return p3D.XY();
    }
    public static PointBl MidPoint(PointBl startPoint, PointBl controlPoint, PointBl endPoint, DoubleBl t) {
      var tA = startPoint * (1d - t).Pow(2d);
      var tX = 2d * t * (1d - t);
      var tB = controlPoint * tX;
      var tC = endPoint * t.Pow(2);
      var ret = (tB + (tA + (tC)));
      return ret;
    }
    /// <summary>
    /// Create a bezier polygon segment with four points starting at the second point but curved for the first, and ending at the third point but curved for the fourth. 
    /// </summary>
    /// <param name="smoothValue">Sharpness of the polygon edge (0.8 is a good default)</param>
    /// <param name="Points">Four points that define the polygon segment.</param>
    /// <returns>A bezier segment that is curved through points 2 and 3 with tangents according to points 1 and 4.</returns>
    public static PointBl[] PolygonSegment(DoubleBl smoothValue, params PointBl[] Points) {
      (Points.Length == 4).Assert();
      return PolygonSegment(Points[0], Points[1], Points[2], Points[3], smoothValue);
    }

    /// <summary>
    /// Create a bezier polygon segment with four points starting at the second point but curved for the first, and ending at the third point but curved for the fourth. 
    /// </summary>
    /// <param name="Points">Four points that define the polygon segment.</param>
    /// <returns>A bezier segment that is curved through points 2 and 3 with tangents according to points 1 and 4.</returns>
    //public static BezierSegmentBl PolygonSegment(params PointBl[] Points) {
    //  return PolygonSegment(0.8, Points);
    //}
    private static PointBl[] PolygonSegment(PointBl point0, PointBl point1, PointBl point2, PointBl point3, DoubleBl smooth) {
      var c1 = (point0 + point1) / 2d;
      var c2 = (point1 + point2) / 2d;
      var c3 = (point2 + point3) / 2d;

      var len1 = (point1 - point0).Length;
      var len2 = (point2 - point1).Length;
      var len3 = (point3 - point2).Length;

      var k1 = len1 / (len1 + len2);
      var k2 = len2 / (len2 + len3);

      var m1 = c1 + (c2 - c1) * k1;
      var m2 = c2 + (c3 - c2) * k2;

      var ctrl1 = m1 + (c2 - m1) * smooth + point1 - m1;
      var ctrl2 = m2 + (c2 - m2) * smooth + point2 - m2;
      return new PointBl[] {
        ctrl1, ctrl2, point2,
      };
    }
  }
}

