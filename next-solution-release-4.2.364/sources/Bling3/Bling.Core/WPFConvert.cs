
#if USE_WPF

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

#endif

using Bling.Ops;
using Bling.Util;
using Bling.DSL;
using Bling.Vecs;

namespace Bling.Core {
#if USE_WPF
  public partial interface ICanWrapWPFPoint : ICanWrap<PointBl,Point>, ICanWrap<PointBl,Vector> {}
  public partial interface ICanWrapWPFPoint3D : ICanWrap<Point3DBl, Point3D>, ICanWrap<Point3DBl, Vector3D> { }
  public partial interface ICanWrapWPFPoint4D : ICanWrap<Point4DBl, Point4D> { }
  public partial interface ICanWrapWPFRect : ICanWrap<Point4DBl, Rect> { }
#else
  public partial interface ICanWrapWPFPoint {}
  public partial interface ICanWrapWPFPoint3D {}
  public partial interface ICanWrapWPFPoint4D {}
  public partial interface ICanWrapWPFRect {}
#endif

}

#if USE_WPF

namespace Bling.WPF {
  public partial class PointArity : Arity2<double, Point, PointArity> {
    public override double Access(Point Value, int Idx) {
      return (Idx == 0) ? Value.X : Value.Y;
    }
    public override Point Make(params double[] Params) {
      return new Point(Params[0], Params[1]); ;
    }
    public override string[] PropertyNames {
      get { return new string[] { "X", "Y" }; }
    }
    public static int CheckInit() { Extensions.Trace("CheckInit: " + typeof(PointArity) + " " + ArityI); return 0; }
  }
  public partial class VectorArity : Arity2<double, Vector, VectorArity> {
    public override double Access(Vector Value, int Idx) {
      return (Idx == 0) ? Value.X : Value.Y;
    }
    public override Vector Make(params double[] Params) {
      return new Vector(Params[0], Params[1]); ;
    }
    public override string[] PropertyNames {
      get { return new string[] { "X", "Y" }; }
    }
    public static int CheckInit() { Extensions.Trace("CheckInit: " + typeof(VectorArity) + " " + ArityI); return 0; }
  }
  public partial class SizeArity : Arity2<double, Size, SizeArity> {
    public override double Access(Size Value, int Idx) {
      return (Idx == 0) ? Value.Width : Value.Height;
    }
    public override Size Make(params double[] Params) {
      return new Size(Params[0], Params[1]); ;
    }
    public override string[] PropertyNames {
      get { return new string[] { "Width", "Height" }; }
    }
    public static int CheckInit() { Extensions.Trace("CheckInit: " + typeof(SizeArity) + " " + ArityI); return 0; }
  }
  public partial class Point3DArity : Arity3<double, Point3D, Point3DArity> {
    public static int CheckInit() { Extensions.Trace("CheckInit: " + typeof(Point3DArity) + " " + ArityI); return 0; }
    public override double Access(Point3D Value, int Idx) {
      return (Idx == 0) ? Value.X : (Idx == 1) ? Value.Y : Value.Z;
    }
    public override Point3D Make(params double[] Params) {
      return new Point3D(Params[0], Params[1], Params[2]); ;
    }
    public override string[] PropertyNames {
      get { return new string[] { "X", "Y", "Z" }; }
    }
  }
  public partial class Vector3DArity : Arity3<double, Vector3D, Vector3DArity> {
    public static int CheckInit() { Extensions.Trace("CheckInit: " + typeof(Vector3DArity) + " " + ArityI); return 0; }
    public override double Access(Vector3D Value, int Idx) {
      return (Idx == 0) ? Value.X : (Idx == 1) ? Value.Y : Value.Z;
    }
    public override Vector3D Make(params double[] Params) {
      return new Vector3D(Params[0], Params[1], Params[2]); ;
    }
    public override string[] PropertyNames {
      get { return new string[] { "X", "Y", "Z" }; }
    }
  }
  public partial class Point4DArity : Arity4<double, Point4D, Point4DArity> {
    public static int CheckInit() { Extensions.Trace("CheckInit: " + typeof(Point4DArity) + " " + ArityI); return 0; }
    public override double Access(Point4D Value, int Idx) {
      return (Idx == 0) ? Value.X : (Idx == 1) ? Value.Y : (Idx == 2) ? Value.Z : Value.W;
    }
    public override Point4D Make(params double[] Params) {
      return new Point4D(Params[0], Params[1], Params[2], Params[3]); ;
    }
    public override string[] PropertyNames {
      get { return new string[] { "X", "Y", "Z", "W" }; }
    }
  }
  public partial class RectArity : Arity4<double, Rect, RectArity> {
    public static int CheckInit() { Extensions.Trace("CheckInit: " + typeof(RectArity) + " " + ArityI); return 0; }
    public override double Access(Rect Value, int Idx) {
      return (Idx == 0) ? Value.X : (Idx == 1) ? Value.Y : (Idx == 2) ? Value.Width : Value.Height;
    }
    public override Rect Make(params double[] Params) {
      return new Rect(Params[0], Params[1], Params[2], Params[3]); ;
    }
    public override string[] PropertyNames {
      get { return new string[] { "X", "Y", "Width", "Height" }; }
    }
  }
  public partial class ColorArity : Arity4<double, Color, ColorArity> {
    public static int CheckInit() { Extensions.Trace("CheckInit: " + typeof(ColorArity) + " " + ArityI); return 0; }
    public override double Access(Color Value, int Idx) {
      return (Idx == 0) ? Value.ScR : (Idx == 1) ? Value.ScG : (Idx == 2) ? Value.ScB : Value.ScA;
    }
    public override Color Make(params double[] Params) {
      return Color.FromScRgb((float)Params[3], (float)Params[0], (float)Params[1], (float)Params[2]); ;
    }
    public override string[] PropertyNames {
      get { return new string[] { "ScR", "ScG", "ScB", "ScA" }; }
    }
  }
  public partial class QuaternionArity : Arity4<double, Quaternion, QuaternionArity> {
    public static int CheckInit() { Extensions.Trace("CheckInit: " + typeof(QuaternionArity) + " " + ArityI); return 0; }
    public override double Access(Quaternion Value, int Idx) {
      return (Idx == 0) ? Value.X : (Idx == 1) ? Value.Y : (Idx == 2) ? Value.Z : Value.W;
    }
    public override Quaternion Make(params double[] Params) {
      return new Quaternion(Params[0], Params[1], Params[2], Params[3]); ;
    }
    public override string[] PropertyNames {
      get { return new string[] { "X", "Y", "Z", "W" }; }
    }
  }
}

namespace Bling.Core {
  using Bling.Semantics;
  using Bling.WPF;
  public abstract partial class BasePointBl<BRAND> : PointBl<Vec2<double>, BRAND, Vec2Arity<double>> where BRAND : BasePointBl<BRAND> {
    private static readonly int x01 = PointArity.CheckInit() + VectorArity.CheckInit() + SizeArity.CheckInit();  
    public static implicit operator BasePointBl<BRAND>(Point p) {
      return new Constant<Point>(p);
    }
    public static implicit operator BasePointBl<BRAND>(Vector p) {
      return new Constant<Vector>(p);
    }
    public static implicit operator BasePointBl<BRAND>(Size p) {
      return new Constant<Size>(p);
    }

    public static implicit operator BasePointBl<BRAND>(Expr<Size> p) {
      return ToBrand(VecConvert<Size, double, Vec2<double>>.ToVec.Instance.Make(p));
    }
    public static implicit operator Expr<Size>(BasePointBl<BRAND> p) {
      return (VecConvert<Size, double, Vec2<double>>.FromVec.Instance.Make(p));
    }
    public static implicit operator BasePointBl<BRAND>(Expr<Point> p) {
      return ToBrand(VecConvert<Point, double, Vec2<double>>.ToVec.Instance.Make(p));
    }
    public static implicit operator Expr<Point>(BasePointBl<BRAND> p) {
      return (VecConvert<Point, double, Vec2<double>>.FromVec.Instance.Make(p));
    }
    public static implicit operator BasePointBl<BRAND>(Expr<Vector> p) {
      return ToBrand(VecConvert<Vector, double, Vec2<double>>.ToVec.Instance.Make(p));
    }
    public static implicit operator Expr<Vector>(BasePointBl<BRAND> p) {
      return (VecConvert<Vector, double, Vec2<double>>.FromVec.Instance.Make(p));
    }
  }
  public partial class PointBl : BasePointBl<PointBl>, ICoreBrandT, ICanWrapWPFPoint {
    private static int InitWPF() {
      CanWrap<PointBl, Point>.To = p => 
        VecConvert<Point, double, Vec2<double>>.ToVec.Instance.Make(p);
      CanWrap<PointBl, Point>.From = p =>
        VecConvert<Point, double, Vec2<double>>.FromVec.Instance.Make(p);

      CanWrap<PointBl, Vector>.To = p =>
        VecConvert<Vector, double, Vec2<double>>.ToVec.Instance.Make(p);
      CanWrap<PointBl, Vector>.From = p =>
        VecConvert<Vector, double, Vec2<double>>.FromVec.Instance.Make(p);

      return 0;
    }
    private static readonly int x01 = InitWPF() + PointArity.CheckInit() + VectorArity.CheckInit() + SizeArity.CheckInit();


    public static implicit operator PointBl(Point p) {
      return ((BasePointBl<PointBl>)p);
    }
    public static implicit operator PointBl(Size p) {
      return ((BasePointBl<PointBl>)p);
    }
    public static implicit operator PointBl(Vector p) {
      return ((BasePointBl<PointBl>)p);
    }
    public static implicit operator PointBl(Expr<Point> p) {
      return ((BasePointBl<PointBl>)p);
    }
    public static implicit operator PointBl(Expr<Vector> p) {
      return ((BasePointBl<PointBl>)p);
    }
    public static implicit operator PointBl(Expr<Size> p) {
      return ((BasePointBl<PointBl>)p);
    }
    public static explicit operator Vector(PointBl p) {
      return CanWrap<PointBl, Vector>.From(p).CurrentValue;
    }
    public static explicit operator Point(PointBl p) {
      return CanWrap<PointBl, Point>.From(p).CurrentValue;
    }

  }
  public abstract partial class BasePoint3DBl<BRAND> : Point3DBl<Vec3<double>, BRAND, Vec3Arity<double>> where BRAND : BasePoint3DBl<BRAND> {
    private static readonly int x01 = Point3DArity.CheckInit() + Vector3DArity.CheckInit();
    public static implicit operator BasePoint3DBl<BRAND>(Point3D p) {
      return new Constant<Point3D>(p);
    }
    public static implicit operator BasePoint3DBl<BRAND>(Vector3D p) {
      return new Constant<Vector3D>(p);
    }
    public static implicit operator BasePoint3DBl<BRAND>(Expr<Point3D> p) {
      return ToBrand(VecConvert<Point3D, double, Vec3<double>>.ToVec.Instance.Make(p));
    }
    public static implicit operator Expr<Point3D>(BasePoint3DBl<BRAND> p) {
      return (VecConvert<Point3D, double, Vec3<double>>.FromVec.Instance.Make(p));
    }
    public static implicit operator BasePoint3DBl<BRAND>(Expr<Vector3D> p) {
      return ToBrand(VecConvert<Vector3D, double, Vec3<double>>.ToVec.Instance.Make(p));
    }
    public static implicit operator Expr<Vector3D>(BasePoint3DBl<BRAND> p) {
      return (VecConvert<Vector3D, double, Vec3<double>>.FromVec.Instance.Make(p));
    }
  }
  public partial class Point3DBl : BasePoint3DBl<Point3DBl>, ICoreBrandT, ICanWrapWPFPoint3D {
    private static int InitWPF() {
      CanWrap<Point3DBl, Point3D>.To = p =>
        VecConvert<Point3D, double, Vec3<double>>.ToVec.Instance.Make(p);
      CanWrap<Point3DBl, Point3D>.From = p =>
        VecConvert<Point3D, double, Vec3<double>>.FromVec.Instance.Make(p);

      CanWrap<Point3DBl, Vector3D>.To = p =>
        VecConvert<Vector3D, double, Vec3<double>>.ToVec.Instance.Make(p);
      CanWrap<Point3DBl, Vector3D>.From = p =>
        VecConvert<Vector3D, double, Vec3<double>>.FromVec.Instance.Make(p);

      return Point3DArity.CheckInit() + Vector3DArity.CheckInit();
    }
    private static readonly int x543 = InitWPF();

    public static implicit operator Point3DBl(Point3D p) {
      return ((BasePoint3DBl<Point3DBl>)p);
    }
    public static implicit operator Point3DBl(Vector3D p) {
      return ((BasePoint3DBl<Point3DBl>)p);
    }
    public static implicit operator Point3DBl(Expr<Point3D> p) {
      return ((BasePoint3DBl<Point3DBl>)p);
    }
    public static implicit operator Point3DBl(Expr<Vector3D> p) {
      return ((BasePoint3DBl<Point3DBl>)p);
    }
  }
  public abstract partial class BasePoint4DBl<BRAND> : Point4DBl<Vec4<double>, BRAND, Vec4Arity<double>> where BRAND : BasePoint4DBl<BRAND> {
    private static readonly int x01 = Point4DArity.CheckInit() + RectArity.CheckInit();

    public static implicit operator BasePoint4DBl<BRAND>(Point4D p) {
      return new Constant<Point4D>(p);
    }
    public static implicit operator BasePoint4DBl<BRAND>(Expr<Point4D> p) {
      return ToBrand(VecConvert<Point4D, double, Vec4<double>>.ToVec.Instance.Make(p));
    }
    public static implicit operator Expr<Point4D>(BasePoint4DBl<BRAND> p) {
      return (VecConvert<Point4D, double, Vec4<double>>.FromVec.Instance.Make(p));
    }
    public static implicit operator BasePoint4DBl<BRAND>(Rect p) {
      return new Constant<Rect>(p);
    }
    public static implicit operator BasePoint4DBl<BRAND>(Expr<Rect> p) {
      return ToBrand(VecConvert<Rect, double, Vec4<double>>.ToVec.Instance.Make(p));
    }
    public static implicit operator Expr<Rect>(BasePoint4DBl<BRAND> p) {
      return (VecConvert<Rect, double, Vec4<double>>.FromVec.Instance.Make(p));
    }
  }
  public partial class Point4DBl : BasePoint4DBl<Point4DBl>, ICoreBrandT, ICanWrapWPFPoint4D, ICanWrapWPFRect {
    private static readonly int w01 = Point4DArity.CheckInit() + RectArity.CheckInit();

    private static int InitWPF() {
      CanWrap<Point4DBl, Point4D>.To = p =>
        VecConvert<Point4D, double, Vec4<double>>.ToVec.Instance.Make(p);
      CanWrap<Point4DBl, Point4D>.From = p =>
        VecConvert<Point4D, double, Vec4<double>>.FromVec.Instance.Make(p);

      CanWrap<Point4DBl, Rect>.To = p =>
        VecConvert<Rect, double, Vec4<double>>.ToVec.Instance.Make(p);
      CanWrap<Point4DBl, Rect>.From = p =>
        VecConvert<Rect, double, Vec4<double>>.FromVec.Instance.Make(p);

      return 0;
    }


    public static implicit operator Point4DBl(Point4D p) {
      return ((BasePoint4DBl<Point4DBl>)p);
    }
    public static implicit operator Point4DBl(Expr<Point4D> p) {
      return ((BasePoint4DBl<Point4DBl>)p);
    }
    public static implicit operator Point4DBl(Rect p) {
      return ((BasePoint4DBl<Point4DBl>)p);
    }
    public static implicit operator Point4DBl(Expr<Rect> p) {
      return ((BasePoint4DBl<Point4DBl>)p);
    }
  }

}
namespace Bling.Core {
  using Bling.WPF;
  public partial class ColorBl : ColorBl<Vec4<double>, ColorBl, Vec4Arity<double>> {
    private static readonly int x01 = ColorArity.CheckInit();
    public static implicit operator ColorBl(Color p) {
      return new Constant<Color>(p);
    }
    public static implicit operator ColorBl(Expr<Color> p) {
      return ToBrand(VecConvert<Color, double, Vec4<double>>.ToVec.Instance.Make(p));
    }
    public static implicit operator Expr<Color>(ColorBl p) {
      return (VecConvert<Color, double, Vec4<double>>.FromVec.Instance.Make(p));
    }
  }
  public partial class QuaternionBl : QuaternionBl<Vec4<double>, QuaternionBl, Vec4Arity<double>> {
    private static readonly int x01 = QuaternionArity.CheckInit();
    public static implicit operator QuaternionBl(Quaternion p) {
      return new Constant<Quaternion>(p);
    }
    public static implicit operator QuaternionBl(Expr<Quaternion> p) {
      return ToBrand(VecConvert<Quaternion, double, Vec4<double>>.ToVec.Instance.Make(p));
    }
    public static implicit operator Expr<Quaternion>(QuaternionBl p) {
      return (VecConvert<Quaternion, double, Vec4<double>>.FromVec.Instance.Make(p));
    }
  }
  public static partial class CoreExtensions {
    public static PointBl Bl(this Expr<Point> p) { return p; }
    public static PointBl Bl(this Expr<Vector> p) { return p; }
    public static PointBl Bl(this Expr<Size> p) { return p; }
    public static Point3DBl Bl(this Expr<Point3D> p) { return p; }
    public static Point3DBl Bl(this Expr<Vector3D> p) { return p; }
    public static Point4DBl Bl(this Expr<Point4D> p) { return p; }
    public static Point4DBl Bl(this Expr<Rect> p) { return p; }
    public static ColorBl Bl(this Expr<Color> p) { return p; }

    public static PointBl Bl(this Point p) { return p; }
    public static PointBl Bl(this Vector p) { return p; }
    //public static PointBl Bl(this Size p) { return p; }
    public static Point3DBl Bl(this Point3D p) { return p; }
    //public static Point3DBl Bl(this Vector3D p) { return p; }
    public static Point4DBl Bl(this Point4D p) { return p; }
    //public static Point4DBl Bl(this Rect p) { return p; }
    public static ColorBl Bl(this Color p) { return p; }
    public static ColorBl Cl(this Color p) { return p.Bl(); }


  }
}
namespace Bling.Graphics {
  using Bling.Core;
  using Bling.Vecs;
  using Bling.Shaders;
  public partial class Tex2D : BaseTex<PointBl, Tex2D> {
    public static Tex2D Make<T>(Expr<T> e) {
      return new Tex2D(p => Tex2DOp<T, Vecs.Vec2<double>, Vecs.Vec4<double>>.Instance.Make(ToTexture<T>.Instance.Make(e), p));
    }
    public static Tex2D Input {
      get {
        return new Tex2D(p => Tex2DOp<object, Vecs.Vec2<double>, Vecs.Vec4<double>>.Instance.Make(InputTexture.Instance, p));
      }
    }
    public static PointBl UV {
      get { return CurrentPixel.Instance; }
    }
    public static implicit operator Tex2D(Expr<Brush> Brush) { return Tex2D.Make(Brush); }
    public static implicit operator Tex2D(Brush Brush) { return new Constant<Brush>(Brush); }
    public static explicit operator Tex2D(Color Clr) { return (Tex2D) ((ColorBl) Clr); }
  }
}

#endif