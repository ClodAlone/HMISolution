using System;
using System.Collections.Generic;
using linq = Microsoft.Linq.Expressions;
using Bling.Util;
using Bling.Matrices;
using Bling.Core;

namespace Bling.Ops {
  using Bling.DSL;
  using Bling.Vecs;
  public static partial class TrigOperators<T> {
    public abstract partial class MathOperator<CNT> : DoubleOperators0<T>.MathOperator<CNT> where CNT : MathOperator<CNT> { }
    public abstract partial class Math2Operator<CNT> : DoubleOperators0<T>.Math2Operator<CNT> where CNT : Math2Operator<CNT> { }

    public partial class Sin : MathOperator<Sin> {
      public new static readonly Sin Instance = new Sin();
      private Sin() { }
      public override IOperatorX<double, double> Uniform { get { return TrigOperators<double>.Sin.Instance; } }
      protected override string CallName { get { return "Sin"; } }
      public override Expr<T> Inverse(Expr<T> argA, Expr<T> result) {
        return Asin.Instance.Make(result);
      }
    }
    public partial class Asin : MathOperator<Asin> {
      public new static readonly Asin Instance = new Asin();
      private Asin() { }
      public override IOperatorX<double, double> Uniform { get { return TrigOperators<double>.Asin.Instance; } }
      protected override string CallName { get { return "Asin"; } }
      public override Expr<T> Inverse(Expr<T> argA, Expr<T> result) {
        return Sin.Instance.Make(result);
      }
    }
    public partial class Sinh : MathOperator<Sinh> {
      public new static readonly Sinh Instance = new Sinh();
      private Sinh() { }
      public override IOperatorX<double, double> Uniform { get { return TrigOperators<double>.Sinh.Instance; } }
      protected override string CallName { get { return "Sinh"; } }
    }
    public partial class Cos : MathOperator<Cos> {
      public new static readonly Cos Instance = new Cos();
      private Cos() { }
      public override IOperatorX<double, double> Uniform { get { return TrigOperators<double>.Cos.Instance; } }
      protected override string CallName { get { return "Cos"; } }
      public override Expr<T> Inverse(Expr<T> argA, Expr<T> result) {
        return Acos.Instance.Make(result);
      }
    }
    public partial class Acos : MathOperator<Acos> {
      public new static readonly Acos Instance = new Acos();
      private Acos() { }
      public override IOperatorX<double, double> Uniform { get { return TrigOperators<double>.Acos.Instance; } }
      protected override string CallName { get { return "Acos"; } }
      public override Expr<T> Inverse(Expr<T> argA, Expr<T> result) {
        return Cos.Instance.Make(result);
      }
    }
    public partial class Cosh : MathOperator<Cosh> {
      public new static readonly Cosh Instance = new Cosh();
      private Cosh() { }
      public override IOperatorX<double, double> Uniform { get { return TrigOperators<double>.Cosh.Instance; } }
      protected override string CallName { get { return "Cosh"; } }
    }
    public partial class Tan : MathOperator<Tan> {
      public new static readonly Tan Instance = new Tan();
      private Tan() { }
      public override IOperatorX<double, double> Uniform { get { return TrigOperators<double>.Tan.Instance; } }
      protected override string CallName { get { return "Tan"; } }
      public override Expr<T> Inverse(Expr<T> argA, Expr<T> result) {
        return Atan.Instance.Make(result);
      }
    }
    public partial class Atan : MathOperator<Atan> {
      public new static readonly Atan Instance = new Atan();
      private Atan() { }
      public override IOperatorX<double, double> Uniform { get { return TrigOperators<double>.Atan.Instance; } }
      protected override string CallName { get { return "Atan"; } }
      public override Expr<T> Inverse(Expr<T> argA, Expr<T> result) {
        return Tan.Instance.Make(result);
      }
    }
    public partial class Tanh : MathOperator<Tanh> {
      public new static readonly Tanh Instance = new Tanh();
      private Tanh() { }
      public override IOperatorX<double, double> Uniform { get { return TrigOperators<double>.Tanh.Instance; } }
      protected override string CallName { get { return "Tanh"; } }
    }
    public partial class Atan2 : Math2Operator<Atan2> {
      public new static readonly Atan2 Instance = new Atan2();
      private Atan2() { }
      public override OperatorX<double, double, double> Uniform { get { return TrigOperators<double>.Atan2.Instance; } }
      protected override string CallName { get { return "Atan2"; } }

      public override Expr<T> InverseA(Expr<T> argA, Expr<T> argB, Expr<T> result) {
        return base.InverseA(argA, argB, result);
      }

    }
  }
  public static partial class Trig1Operators<T> {
    public partial class SinCos : StaticCallOperator<double, T, SinCos> {
      protected override Type TargetType { get { return typeof(Math); } }
      public new static readonly SinCos Instance = new SinCos();
      private SinCos() { }
      protected override string CallName { get { return "SinCos"; } }
      public override Expr<T> Expand(Expr<double> argA) {
        var sin = TrigOperators<double>.Sin.Instance.Make(argA);
        var cos = TrigOperators<double>.Cos.Instance.Make(argA);
        return Arity<double, T>.ArityI.Make(sin, cos);
      }
      public override Expr<double> Inverse(Expr<double> argA, Expr<T> result) {
        return AntiSinCos.Instance.Make(result);
      }
    }
    public partial class AntiSinCos : Operator<T, double, AntiSinCos> {
      //protected override Type TargetType { get { return typeof(Math); } }
      public new static readonly AntiSinCos Instance = new AntiSinCos();
      private AntiSinCos() { }
      //protected override string CallName { get { return "Atan2"; } }
      public override Expr<double> Expand(Expr<T> argA) {
        var result = TrigOperators<double>.Atan2.Instance.Make(
          Arity<double, T>.ArityI.Access(argA, 1),
          Arity<double, T>.ArityI.Access(argA, 0));
        return result;
      }
      public override Expr<T> Inverse(Expr<T> argA, Expr<double> result) {
        return SinCos.Instance.Make(result);
      }
    }
  }
}
namespace Bling.Core {
  using Bling.DSL;
  using Bling.Vecs;
  using Bling.Ops;
  public partial interface IDoubleBl<BRAND> : INumericBl<BRAND> where BRAND : IDoubleBl<BRAND> {
    BRAND Slerp0(BRAND other, DoubleBl t);
  }
  public abstract partial class BaseDoubleBl<T, BRAND, BOOL> :
    BaseDoubleLikeBl<T, BRAND, BOOL>, IDoubleBl<BRAND, BOOL>, IBaseDoubleBl<T, BRAND>
    where BRAND : BaseDoubleBl<T, BRAND, BOOL>
    where BOOL : IBoolBl<BOOL> {
    public virtual BRAND Slerp0(BRAND other, DoubleBl t) { // XXX: make an operator.
      var from = this;
      var to = other;

      var theta = from.Dot(to).Acos;
      return (((1d - t) * theta).Sin / theta.Sin) * from + ((t * theta).Sin / theta.Sin) * to;
    }
    public virtual BRAND SinU {
      get {
        // swizzle sin!
        var q = this * (Math.PI * 2d);
        return ToBrand(TrigOperators<T>.Sin.Instance.Make(q.Underlying));
      }
    }
    public virtual BRAND CosU {
      get {
        // swizzle sin!
        var q = this * (Math.PI * 2d);
        return ToBrand(TrigOperators<T>.Cos.Instance.Make(q.Underlying));
      }
    }
    public virtual BRAND SinN {
      get {
        // swizzle sin!
        var q = this * (Math.PI * 1d);
        return ToBrand(TrigOperators<T>.Sin.Instance.Make(q.Underlying));
      }
    }
    public virtual BRAND CosN {
      get {
        // swizzle sin!
        var q = this * (Math.PI * 1d);
        return ToBrand(TrigOperators<T>.Cos.Instance.Make(q.Underlying));
      }
    }
  }
  public partial class DoubleBl : BaseDoubleBl<DoubleBl>, IMonoNumericBl<double, DoubleBl> {
    public Angle2DBl Asin { get { return Angle2DBl.ToBrand(TrigOperators<double>.Asin.Instance.Make(this)); } }
    public Angle2DBl Acos { get { return Angle2DBl.ToBrand(TrigOperators<double>.Acos.Instance.Make(this)); } }
    public Angle2DBl Atan { get { return Angle2DBl.ToBrand(TrigOperators<double>.Atan.Instance.Make(this)); } }
    /*
    public Angle2DBl Atan2(DoubleBl Y) {
      return Angle2DBl.ToBrand(
        
        Trig1Operators<double>.Atan2.Instance.Make(this, Y)); 
    }*/

    public Angle2DBl PI() { return new Angle2DBl(this * Math.PI); }
    public Angle2DBl PI2() { return (this * 2d).PI(); }
    public DegreeBl Degrees() { return new DegreeBl(this); }
    public BRAND Slerp<BRAND>(BRAND from, BRAND to) where BRAND : IDoubleBl<BRAND> { return from.Slerp0(to, this); }

  }
  public abstract partial class PointBl<T, BRAND, ARITY> : MultiDoubleBl<T, BRAND, Vec2<bool>, Bool2Bl, ARITY>, HasXY<DoubleBl>
    where BRAND : PointBl<T, BRAND, ARITY>
    where ARITY : Arity2<double, T, ARITY>, new() {

    public Angle2DBl Angle { get { return Angle2DBl.Angle(Portable); } }
    public Angle2DBl AngleBetween(PointBl p) { return Angle2DBl.AngleBetween(Portable, p); }
    public static BRAND FromAngle(Angle2DBl a) { return ((PointBl<T, BRAND, ARITY>)a.ToPoint); }

    public virtual BRAND SinCosU {
      get {
        return Make(this.SinU[0], this.CosU[1]);
        //var q = this * (Math.PI * 2d);
        //return Make(q.X.ToAngle().Sin, q.Y.ToAngle().Cos);
      }
    }

    /// <summary>
    /// Low-overhead angle conversion for percentage-relative points (useful in pixel shaders). 
    /// </summary>
    /// 
    /*
    public Angle2DBl SimpleAngle {
      get { return new Angle2DBl(X.Atan2(Y)); }
    }*/

    public Angle2DBl Atan2 {
      get { return new Angle2DBl(Trig1Operators<T>.AntiSinCos.Instance.Make(Underlying)); }
    }

  }
  public partial interface IAngle2DBl<BRAND> : INumericBl<BRAND> where BRAND : IAngle2DBl<BRAND> {
    DoubleBl Sin { get; }
    DoubleBl Cos { get; }
    DoubleBl Tan { get; }
    DoubleBl Sinh { get; }
    DoubleBl Cosh { get; }
    DoubleBl Tanh { get; }
    PointBl ToPoint { get; set; }
    PointBl Rotate(PointBl Around, PointBl Target);
    BRAND Inverse { get; set; }
    Angle2DBl AsRadians { get; set; }
    DegreeBl AsDegrees { get; set; }
    VecAngle2DBl AsVec { get; set; }

  }
  public partial interface IAngle2DBl<BRAND, BOOL> : IAngle2DBl<BRAND>, INumericBl<BRAND, BOOL> where BRAND : IAngle2DBl<BRAND, BOOL> { }
  public abstract partial class BaseAngle2DBl<T, BRAND, BOOL> :
    BaseDoubleLikeBl<T, BRAND, BOOL>, IAngle2DBl<BRAND, BOOL>
    where BRAND : BaseAngle2DBl<T, BRAND, BOOL>
    where BOOL : IBoolBl<BOOL> {

    public override BRAND Lerp0(BRAND Other, DoubleBl t) {
      throw new NotImplementedException();
    }

    public MatrixBl<D2, D2> AsMatrix(DoubleBl Sign) {
      return new MatrixBl<D2, D2>((x, y) => {
        if (x == y) return Cos;
        if (x == 0) return Sin * Sign;
        return Sin * Sign * -1;
      });
    }
    public abstract VecAngle2DBl AsVec { get; set; }
    public abstract BRAND Inverse { get; set; }

    private static BRAND Convert(Expr<double> d) {
      return ToBrand(Arity<double, T>.ArityI.Repeat(d));
    }
    public static BRAND operator *(DoubleBl bA, BaseAngle2DBl<T, BRAND, BOOL> bB) { return Convert(bA).Multiply(bB); }
    public static BRAND operator /(DoubleBl bA, BaseAngle2DBl<T, BRAND, BOOL> bB) { return Convert(bA).Divide(bB); }

    public static BRAND Average(params BRAND[] Args) {
      PointBl p = PointBl.Default;
      foreach (var a in Args) p += a.ToPoint;
      return ForPoint(p);
    }
    protected static Func<PointBl, BRAND> ForPoint { get; set; }


    public BaseAngle2DBl(Expr<T> Underlying) : base(Underlying) { }
    public BaseAngle2DBl() : base() { }
    /// <summary>
    /// Convert into a radian-based angle.
    /// </summary>
    public abstract Angle2DBl AsRadians { get; set; }
    /// <summary>
    /// Convert into a degree-based angle.
    /// </summary>
    public abstract DegreeBl AsDegrees { get; set; }
    /// <summary>
    /// Radians as scalar for angle.
    /// </summary>
    public virtual DoubleBl Radians { get { return AsRadians.Radians; } }
    /// <summary>
    /// Degrees as scalar for angle.
    /// </summary>
    public virtual DoubleBl Degrees { get { return AsDegrees.Degrees; } }
    public abstract PointBl ToPoint { get; set; }
    public virtual DoubleBl Sin { get { return (TrigOperators<double>.Sin.Instance.Make(Radians)); } }
    public virtual DoubleBl Cos { get { return (TrigOperators<double>.Cos.Instance.Make(Radians)); } }
    public virtual DoubleBl Tan { get { return (TrigOperators<double>.Tan.Instance.Make(Radians)); } }
    public virtual DoubleBl Sinh { get { return (TrigOperators<double>.Sinh.Instance.Make(Radians)); } }
    public virtual DoubleBl Cosh { get { return (TrigOperators<double>.Cosh.Instance.Make(Radians)); } }
    public virtual DoubleBl Tanh { get { return (TrigOperators<double>.Tanh.Instance.Make(Radians)); } }
    public PointBl Rotate(PointBl Center, PointBl Target) {
      return Center + (Target - Center).Rotate(ToPoint);
    }
    public MatrixBl<D3, D3> RotateAt(PointBl Center) {
      var p = Center.Portable;
      var delta = (p * (1.0 - this.Cos)) + (p.YX() * this.Sin * new PointBl(+1, -1));

      var Table = new DoubleBl[3, 3];
      Table[0, 0] = +Cos; Table[1, 0] = +Sin;
      Table[0, 1] = -Sin; Table[1, 1] = +Cos;
      Table[0, 2] = delta.X; Table[1, 2] = delta.Y;

      Table[2, 0] = 0;
      Table[2, 1] = 0;
      Table[2, 2] = 1;
      return new MatrixBl<D3, D3>(Table);
    }
    /// <summary>
    /// Create 3D axis rotation angle from this angle and axis.
    /// </summary>
    public AxisAngle3DBl RotateAround(Point3DBl Axis) { return new AxisAngle3DBl(Axis, this.AsRadians); }

  }
  public partial class VecAngle2DBl : BaseAngle2DBl<Vec2<double>, VecAngle2DBl, BoolBl> {
    protected VecAngle2DBl(Expr<Vec2<double>> Underlying) : base(Underlying) { }
    public VecAngle2DBl() : base() { }

    public new static VecAngle2DBl Default { get { return new VecAngle2DBl(new PointBl(1, 0)); } }

    public VecAngle2DBl(PointBl Vector) : this(Vector.Normalize.Underlying) { }
    //public VecAngle2DBl() : base() { }
    public override PointBl ToPoint { get { return (Underlying); } set { ToPoint.Bind = value.Normalize; } }
    public DoubleBl X { get { return ToPoint.X; } set { X.Bind = value; } }
    public DoubleBl Y { get { return ToPoint.Y; } set { Y.Bind = value; } }

    public override Angle2DBl AsRadians {
      get { return Angle2DBl.Angle(ToPoint); }
      set { this.AsRadians.Bind = value; }
    }
    public override VecAngle2DBl AsVec {
      get { return this; }
      set { AsVec.Bind = value; }
    }

    public override DoubleBl Sin { get { return Y; } }
    public override DoubleBl Cos { get { return X; } }
    public override DoubleBl Tan { get { return Sin / Cos; } }

    static VecAngle2DBl() {
      ForPoint = p => new VecAngle2DBl(p);
      Register(p => new VecAngle2DBl(p));

      Brand<VecAngle2DBl>.Default = Default;
    }

    public override DegreeBl AsDegrees { get { return AsRadians.AsDegrees; } set { AsDegrees.Bind = value; } }

    public override VecAngle2DBl Inverse {
      get { return new VecAngle2DBl((PointBl)(ToPoint * new PointBl(+1, -1))); }
      set { Inverse.Bind = value; }
    }
    public override BoolBl GreaterThan(VecAngle2DBl other) {
      // clockwise from (1,0)
      BoolBl ret = false;
      ret = (Y <= 0 & other.Y <= 0).Condition(X < other.X, ret); // X < other.X
      ret = (X <= 0 & other.X <= 0).Condition(Y > other.Y, ret);
      ret = (Y >= 0 & other.Y >= 0).Condition(X > other.X, ret);
      ret = (X >= 0 & other.X >= 0).Condition(Y >= 0, ret);
      ret = (Y > other.Y).Condition(true, ret);
      return ret;
    }
    public override BoolBl LessThan(VecAngle2DBl other) {
      // clockwise from (1,0)
      BoolBl ret = false;
      ret = (Y <= 0 & other.Y <= 0).Condition(X < other.X, ret); // X < other.X
      ret = (X <= 0 & other.X <= 0).Condition(Y < other.Y, ret);
      ret = (Y >= 0 & other.Y >= 0).Condition(X < other.X, ret);
      ret = (X >= 0 & other.X >= 0).Condition(Y <= 0, ret);
      ret = (Y < other.Y).Condition(true, ret);
      return ret;
    }
    public override BoolBl GreaterThanOrEqual(VecAngle2DBl other) {
      return !LessThan(other);
    }
    public override BoolBl LessThanOrEqual(VecAngle2DBl other) {
      return !GreaterThan(other);
    }
    public override VecAngle2DBl Add(VecAngle2DBl other) {
      return new VecAngle2DBl(other.Rotate(new PointBl(0, 0), ToPoint));
    }
    public override VecAngle2DBl Subtract(VecAngle2DBl other) {
      return Add(other.Inverse);
    }

  }


  public abstract partial class Angle2DBl<BRAND> : BaseAngle2DBl<double, BRAND, BoolBl> where BRAND : Angle2DBl<BRAND> {
    public Angle2DBl(Expr<double> Underlying) : base(Underlying) { }
    public Angle2DBl() : base() { }
    public static DoubleBl operator /(Angle2DBl<BRAND> opA, Angle2DBl<BRAND> opB) {
      return opA.Divide(opB).Underlying;
    }
    public override BRAND Lerp0(BRAND Other, DoubleBl t) {
      return ToBrand(t.Lerp((DoubleBl)Underlying, (DoubleBl)Other.Underlying));
    }

    public override BoolBl GreaterThan(BRAND other) {
      return NumericOperators<double, double, bool>.GreaterThan.Instance.Make(this, other);
    }
    public override BoolBl LessThan(BRAND other) {
      return NumericOperators<double, double, bool>.LessThan.Instance.Make(this, other);
    }
    public override BoolBl GreaterThanOrEqual(BRAND other) {
      return NumericOperators<double, double, bool>.GreaterThanOrEqual.Instance.Make(this, other);
    }
    public override BoolBl LessThanOrEqual(BRAND other) {
      return NumericOperators<double, double, bool>.LessThanOrEqual.Instance.Make(this, other);
    }
    public static explicit operator Angle2DBl(Angle2DBl<BRAND> angle) { return angle.AsRadians; }
    public static explicit operator DegreeBl(Angle2DBl<BRAND> angle) { return angle.AsDegrees; }
    public abstract BRAND Normalize { get; }
    public override PointBl ToPoint { // don't ask.
      get { return Trig1Operators<Vec2<double>>.SinCos.Instance.Make((2d * Math.PI - Radians) + .5d * Math.PI); }
      set { ToPoint.Bind = value; }
    }
    public PointBl SinCos {
      get { return Trig1Operators<Vec2<double>>.SinCos.Instance.Make(Radians); }
      set { SinCos.Bind = value; }
    }
    public override VecAngle2DBl AsVec {
      get { return new VecAngle2DBl(ToPoint); }
      set { AsVec.Bind = value; }
    }
    /// <summary>
    /// lerp in the smaller interval of the annular region [0, max angle] between angle "from" and "to"
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    public static BRAND SafeLerp(BRAND from, BRAND to, DoubleBl t) {
      var diff = (to.Underlying.Bl() - from.Underlying.Bl());
      diff = (diff.Abs > MaxAngle.Underlying.Bl() / 2d).Condition(-1d * (MaxAngle.Underlying.Bl() - (diff.Abs % MaxAngle.Underlying.Bl())) * diff.Sign, diff);
      return ToBrand(from.Underlying.Bl() + diff * t);
    }
    public static BRAND MaxAngle { get; protected set; }
  }
  public partial class Angle2DBl : Angle2DBl<Angle2DBl> {
    public Angle2DBl() : base() { }
    public Angle2DBl(Expr<double> Underlying) : base(Underlying) { }

    static Angle2DBl() {
      ForPoint = p => p.Angle;
      Register(r => new Angle2DBl(r));
      Angle2DBl<Angle2DBl>.MaxAngle = MaxAngle;
    }

    public new static Angle2DBl MaxAngle { get { return 2d.PI(); } }

    public override Angle2DBl AsRadians { get { return this; } set { AsRadians.Bind = value; } }
    public override DegreeBl AsDegrees { get { return new DegreeBl(Degrees); } set { AsDegrees.Bind = value; } }
    public override DoubleBl Radians { get { return Underlying; } }
    public override DoubleBl Degrees { get { return (Radians) * (180.0 / Math.PI); } }
    public DoubleBl PI { get { return Radians / Math.PI; } }
    public override StringBl ToStringBl() {
      return (Radians / Math.PI).ToStringBl() + "PI";
    }
    public override Angle2DBl Inverse { get { return 2d.PI() - this; } set { Inverse.Bind = value; } }

    public override Angle2DBl Normalize {
      get {
        var r = Radians;
        r = (r < 0d).Condition(2d * Math.PI + r, r);
        r = r % (2d * Math.PI);
        return new Angle2DBl(r);
      }
    }
    public static Angle2DBl AngleBetween(PointBl p0, PointBl p1) {
      var a0 = p1.Atan2 - p0.Atan2;
      return (a0);
    }
    public static Angle2DBl Angle(PointBl p1) {
      //var xxx = Math.Atan2(0, 1);
      //PointBl p0 = new PointBl(1, 0);
      var a0 = p1.Atan2; // -p0.Y.Atan2(p0.X);
      return (a0);
    }

  }
  public partial class DegreeBl : Angle2DBl<DegreeBl> {
    public DegreeBl(Expr<double> Underlying) : base(Underlying) { }
    public override DegreeBl AsDegrees { get { return this; } set { AsDegrees.Bind = value; } }
    public override Angle2DBl AsRadians { get { return new Angle2DBl(Radians); } set { AsRadians.Bind = value; } }
    public override DoubleBl Degrees { get { return Underlying; } }
    public override DoubleBl Radians { get { return (Degrees / (180.0 / Math.PI)); } }
    public override StringBl ToStringBl() {
      return Degrees + "_deg";
    }
    public override DegreeBl Inverse {
      get { return new DegreeBl(360d - Degrees); }
      set { Inverse.Bind = value; }
    }
    static DegreeBl() {
      ForPoint = p => p.Angle.AsDegrees;
      Register(r => new DegreeBl(r));
      Angle2DBl<DegreeBl>.MaxAngle = MaxAngle;
    }

    public new static DegreeBl MaxAngle { get { return 360d.ToDegrees(); } }


    public override DegreeBl Normalize {
      get {
        var r = Degrees;
        r = (r < 0d).Condition(360d + r, r);
        r = r % (360d);
        return new DegreeBl(r);
      }
    }
  }
  public partial class DegreePointBl : BaseNumericBl<Vec2<double>, double, DegreePointBl, Bool2Bl, DoubleBl> {
    public DegreePointBl(DegreeBl X, DegreeBl Y) : base(Vec2Arity<double>.ArityI.Make(X, Y)) { }
    public DegreeBl X { get { return Vec2Arity<double>.ArityI.Access(Underlying, 0).Bl().ToDegrees(); } set { X.Bind = value; } }
    public DegreeBl Y { get { return Vec2Arity<double>.ArityI.Access(Underlying, 1).Bl().ToDegrees(); } set { Y.Bind = value; } }
    public override Bool2Bl GreaterThan(DegreePointBl other) {
      return NumericOperators<Vec2<double>, Vec2<double>, Vec2<bool>>.GreaterThan.Instance.Make(this, other);
    }
    public override Bool2Bl GreaterThanOrEqual(DegreePointBl other) {
      return NumericOperators<Vec2<double>, Vec2<double>, Vec2<bool>>.GreaterThanOrEqual.Instance.Make(this, other);
    }
    public override Bool2Bl LessThan(DegreePointBl other) {
      return NumericOperators<Vec2<double>, Vec2<double>, Vec2<bool>>.LessThan.Instance.Make(this, other);
    }
    public override Bool2Bl LessThanOrEqual(DegreePointBl other) {
      return NumericOperators<Vec2<double>, Vec2<double>, Vec2<bool>>.LessThanOrEqual.Instance.Make(this, other);
    }
    public override DegreePointBl Lerp0(DegreePointBl Other, DoubleBl t) {
      return new DegreePointBl(t.Lerp(X, Other.X), t.Lerp(Y, Other.Y));
    }
  }
  public partial class DegreePoint3DBl : BaseNumericBl<Vec3<double>, double, DegreePoint3DBl, Bool3Bl, DoubleBl> {
    public DegreePoint3DBl(DegreeBl X, DegreeBl Y, DegreeBl Z) : base(Vec3Arity<double>.ArityI.Make(X, Y, Z)) { }
    public DegreeBl X { get { return Vec3Arity<double>.ArityI.Access(Underlying, 0).Bl().ToDegrees(); } set { X.Bind = value; } }
    public DegreeBl Y { get { return Vec3Arity<double>.ArityI.Access(Underlying, 1).Bl().ToDegrees(); } set { Y.Bind = value; } }
    public DegreeBl Z { get { return Vec3Arity<double>.ArityI.Access(Underlying, 2).Bl().ToDegrees(); } set { Z.Bind = value; } }
    public override Bool3Bl GreaterThan(DegreePoint3DBl other) {
      return NumericOperators<Vec3<double>, Vec3<double>, Vec3<bool>>.GreaterThan.Instance.Make(this, other);
    }
    public override DegreePoint3DBl Lerp0(DegreePoint3DBl Other, DoubleBl t) {
      return new DegreePoint3DBl(t.Lerp(X, Other.X), t.Lerp(Y, Other.Y), t.Lerp(Z, Other.Z));
    }

    public override Bool3Bl GreaterThanOrEqual(DegreePoint3DBl other) {
      return NumericOperators<Vec3<double>, Vec3<double>, Vec3<bool>>.GreaterThanOrEqual.Instance.Make(this, other);
    }
    public override Bool3Bl LessThan(DegreePoint3DBl other) {
      return NumericOperators<Vec3<double>, Vec3<double>, Vec3<bool>>.LessThan.Instance.Make(this, other);
    }
    public override Bool3Bl LessThanOrEqual(DegreePoint3DBl other) {
      return NumericOperators<Vec3<double>, Vec3<double>, Vec3<bool>>.LessThanOrEqual.Instance.Make(this, other);
    }
  }
  public abstract partial class AxisAngle3DBl<T, BRAND, ARITY> : MultiDoubleBl<T, BRAND, Vec4<bool>, Bool4Bl, ARITY>, ICanBeMatrixBl<D4, D4>
    where BRAND : AxisAngle3DBl<T, BRAND, ARITY>
    where ARITY : Arity4<double, T, ARITY>, new() {

    private static Expr<T> Make(Point3DBl Axis, Angle2DBl Angle) {
      Axis = Axis.Normalize;
      return Arity<double, T, ARITY>.ArityI.Make(Axis[0], Axis[1], Axis[2], Angle.Radians);
    }

    public AxisAngle3DBl(Point3DBl Axis, Angle2DBl Angle) : base(Make(Axis, Angle)) { }
    protected AxisAngle3DBl(Expr<T> E) : base(E) { }
    public Point3DBl Axis {
      get {
        return new Swizzle<double, T, Vec3<double>>(0, 1, 2).Make(Underlying);
      }
    }
    public Angle2DBl Angle { get { return new Angle2DBl(this[3]); } }
    /// <summary>
    /// Convert 3D axis angle into a 4x4 matrix.
    /// </summary>
    public MatrixBl<D4, D4> Matrix { get { return ((MatrixBl<D4, D4>)this); } }
    public static explicit operator BaseMatrixBl<D4, D4>(AxisAngle3DBl<T, BRAND, ARITY> Angle) {
      var u = Angle.Axis;
      var c = Angle.Angle.Cos;
      var s = Angle.Angle.Sin;
      var n = new DoubleBl[4, 4];


      n[0, 0] = u.X.Square + (1d - u.X.Square) * c;
      n[1, 0] = u.X * u.Y * (1 - c) - u.Z * s;
      n[2, 0] = u.X * u.Z * (1 - c) + u.Y * s;

      n[0, 1] = u.X * u.Y * (1 - c) + u.Z * s;
      n[1, 1] = u.Y.Square + (1d - u.Y.Square) * c;
      n[2, 1] = u.Y * u.Z * (1 - c) - u.X * s;

      n[0, 2] = u.X * u.Z * (1 - c) - u.Y * s;
      n[1, 2] = u.Y * u.Z * (1 - c) + u.X * s;
      n[2, 2] = u.Z.Square + (1d - u.Z.Square) * c;
      n[3, 3] = 1;
      for (int i = 0; i < 3; i++) {
        n[3, i] = 0;
        n[i, 3] = 0;
      }
      return new MatrixBl<D4, D4>(n);

    }
  }
  public partial class AxisAngle3DBl : AxisAngle3DBl<Vec4<double>, AxisAngle3DBl, Vec4Arity<double>> {
    public AxisAngle3DBl(Point3DBl Axis, Angle2DBl Angle) : base(Axis, Angle) { }
    public AxisAngle3DBl(Expr<Vec4<double>> E) : base(E) { }
    static AxisAngle3DBl() { Register(v => v); }

    public static implicit operator AxisAngle3DBl(Expr<Vec4<double>> v) { return new AxisAngle3DBl(v); }
  }

  public abstract partial class QuaternionBl<T, BRAND, ARITY> : Point4DBl<T, BRAND, ARITY>
    where BRAND : QuaternionBl<T, BRAND, ARITY>
    where ARITY : Arity4<double, T, ARITY>, new() {
    public QuaternionBl(Expr<T> Underlying) : base(Underlying) { }
    public QuaternionBl(DoubleBl X, DoubleBl Y, DoubleBl Z, DoubleBl W) : base(X, Y, Z, W) { }
    public QuaternionBl(Point3DBl XYZ, DoubleBl W) : base(XYZ, W) { }
    public QuaternionBl(Point3DBl Axis, Angle2DBl Angle) : base(FromAngle(Axis, Angle, false)) { IsNormalized = true; }
    public QuaternionBl(Point3DBl Axis, Angle2DBl Angle, bool IsSafe) : base(FromAngle(Axis, Angle, IsSafe)) { IsNormalized = true; }


    private static Expr<T> FromAngle(Point3DBl Axis, Angle2DBl Angle, bool IsSafe) {
      DoubleBl m = Axis.Length;
      PointBl a = (Angle / 2d).SinCos;
      Point3DBl XYZ = a.X * (Axis / m); // axis * sin(theta / 2)
      DoubleBl W = a.Y; // cos(theta / 2)
      if (IsSafe) return Make(XYZ, W);
      else return (m > 0.0001).Condition(Make(XYZ, W), Identity);
    }

    public static implicit operator QuaternionBl(QuaternionBl<T, BRAND, ARITY> p) { return QuaternionBl.ToBrand(VecConvert<T, double, Vec4<double>>.ToVec.Instance.Make(p)); }
    public static implicit operator QuaternionBl<T, BRAND, ARITY>(QuaternionBl p) { return ToBrand(VecConvert<T, double, Vec4<double>>.FromVec.Instance.Make(p)); }

    public static BRAND operator /(QuaternionBl<T,BRAND,ARITY> bA, DoubleBl bB) {
      Point4DBl p = (Point4DBl) bA;
      p = p/bB;
      return Make(p.XYZ(), p.W);
    }

    public new BRAND Normalize {
      get {
        if (IsNormalized) return this;
        DoubleBl m = Length;
        var ret = (m > 0.001).Condition(this / m, Identity);
        ret.IsNormalized = true;
        return ret;
      }
    }
    public MatrixBl<D3, D3> AsMatrix3D {
      get {
        var Self = this.Normalize;
        var mS = new DoubleBl[3, 3];

        mS[0, 0] = 1 - 2 * Y.Square - 2 * Z.Square;
        mS[1, 0] = 2 * X * Y - 2 * W * Z;
        mS[2, 0] = 2 * X * Z + 2 * W * Z;

        mS[0, 1] = 2 * X * Y + 2 * W * Z;
        mS[1, 1] = 1 - 2 * X.Square - 2 * Z.Square;
        mS[2, 1] = 2 * Y * Z - 2 * W * X;

        mS[0, 2] = 2 * X * Z - 2 * W * Y;
        mS[1, 2] = 2 * Y * Z + 2 * W * X;
        mS[2, 2] = 1 - 2 * X.Square - 2 * Y.Square;
        return new MatrixBl<D3, D3>(mS);
      }
    }

    public class ToMatrixOp : Ops.Operator<T, Matrix<double, D4, D4>, ToMatrixOp>, IMemoOperator<Matrix<double, D4, D4>> {
      public new static readonly ToMatrixOp Instance = new ToMatrixOp();
      private ToMatrixOp() { }
      public override string Format(string argA) { return argA + ".matrix"; }
      public override Expr<Matrix<double, D4, D4>> Expand(Expr<T> argA) {
        var Self = ToBrand(argA);

        var mA = new DoubleBl[4, 4];
        var mB = new DoubleBl[4, 4];
        Func<int, int> Ck = i => (i < 0) ? 4 + i : i % 4;

        for (int i = 0; i < 4; i++) {
          for (int j = 0; j < 4; j++) {
            if (i == j) {
              (((object)mA[i, j]) == null).Assert();
              mA[i, j] = Self.W;
              mB[i, j] = Self.W;
            }
            if (Ck(4 - (i - 1)) == j) {
              (((object)mA[i, j]) == null).Assert();
              mA[i, j] = Self.Z * (j == 0 || j == 3 ? -1 : +1);
              mB[i, j] = Self.Z * (j % 2 == 0 ? -1 : +1);
            }
            if (Ck(i - 2) == j) {
              (((object)mA[i, j]) == null).Assert();
              mA[i, j] = Self.Y * (j <= 1 ? +1 : -1);
              mB[i, j] = Self.Y * (j == 0 || j == 3 ? +1 : -1);
            }
            if (3 - i == j) {
              (((object)mA[i, j]) == null).Assert();
              mA[i, j] = Self.X * (j % 2 == 0 ? +1 : -1);
              mB[i, j] = Self.X * (j <= 1 ? -1 : +1);
            }
            (((object)mA[i, j]) != null).Assert();
          }
        }
        return new MatrixBl<D4, D4>(mA) * new MatrixBl<D4, D4>(mB);
      }

    }
    public MatrixBl<D4, D4> Matrix {
      get {
        return ToMatrixOp.Instance.Make(Underlying);
      }
    }
    public DoubleBl S { get { return W; } }
    public Point3DBl V { get { return this.XYZ(); } }

    public override BRAND Lerp0(BRAND other, DoubleBl t) {
      return Slerp0(other, t);
    }
    public override BRAND Slerp0(BRAND Other, DoubleBl t) {
      BRAND p = this;
      BRAND q = Other;
      DoubleBl eps = 1e-7;
      q = (p.Distance(q) > p.Distance(q)).Condition(q * -1d, q);

      BRAND qtAB, qtC;
      var cosom = p.Dot(q);
      {
        var omega = cosom.Acos; var sinom = omega.Sin;
        DoubleBl sclpA = (omega * (1.0 - t)).Sin / sinom;
        DoubleBl sclqA = (omega * (0.0 + t)).Sin / sinom;
        DoubleBl sclpB = 1.0 - t;
        DoubleBl sclqB = t;

        DoubleBl sclpAB = ((1.0d - cosom) > eps).Condition(sclpA, sclpB);
        DoubleBl sclqAB = ((1.0d - cosom) > eps).Condition(sclqA, sclqB);
        qtAB = sclpAB * p + sclqAB * q;
      }
      {
        DoubleBl sclpC = (.5.PI() * (1.0 - t)).Sin;
        DoubleBl sclqC = (.5.PI() * (0.0 + t)).Sin;
        qtC = sclpC * p + sclqC * q;
      }
      return ((1.0 + cosom) > eps).Condition(qtAB, qtC);
    }

    public class MultiplyOp : Ops.Operator<T, T, T, MultiplyOp>, IMemoOperator<T> {
      public new static readonly MultiplyOp Instance = new MultiplyOp();
      private MultiplyOp() { }
      public override string Format(string argA, string argB) { return argA + " * " + argB; }
      public override Expr<T> Expand(Expr<T> argA, Expr<T> argB) {
        BRAND q = ToBrand(argA);
        BRAND r = ToBrand(argB);
        var W = q.W * r.W - q.XYZ().Dot(r.XYZ());
        var XYZ = q.W * r.XYZ() +
                  r.W * q.XYZ() +
                  q.YZX() * r.ZXY() -
                  r.YZX() * q.ZXY();
        return ToBrand(QuaternionBl<T,BRAND,ARITY>.Make(XYZ, W)).Underlying;
      }
      public override Expr<T> InverseA(Expr<T> argA, Expr<T> argB, Expr<T> result) {
        return DivideOp.Instance.Make(result, argB);
      }
    }
    public class DivideOp : Ops.Operator<T, T, T, DivideOp>, IMemoOperator<T> {
      public new static readonly DivideOp Instance = new DivideOp();
      private DivideOp() { }
      public override string Format(string argA, string argB) { return argA + " / " + argB; }
      public override Expr<T> Expand(Expr<T> argA, Expr<T> argB) {
        BRAND q = ToBrand(argA);
        BRAND r = ToBrand(argB);
        var W = q.XYZ().Dot(r.XYZ());
        var XYZ = r.W * q.XYZ() -
                  q.W * r.XYZ() -
                  r.YZX() * q.ZXY() +
                  q.YZX() * r.ZXY();

        var L2 = 1d; // r.LengthSquared;
        return ToBrand(QuaternionBl<T, BRAND, ARITY>.Make(XYZ / L2, W / L2)).Underlying;
      }
      public override Expr<T> InverseA(Expr<T> argA, Expr<T> argB, Expr<T> result) {
        return MultiplyOp.Instance.Make(result, argB);
      }
    }

    public virtual new BRAND Multiply(BRAND OpB) {
      return ToBrand(MultiplyOp.Instance.Make(this, OpB));
    }
    public virtual new BRAND Divide(BRAND OpB) {
      return ToBrand(DivideOp.Instance.Make(this, OpB));
    }
    public static BRAND operator /(QuaternionBl<T, BRAND, ARITY> opA, BRAND opB) {
      return opA.Divide(opB);
    }
    public static BRAND operator *(QuaternionBl<T, BRAND, ARITY> opA, BRAND opB) {
      return opA.Multiply(opB);
    }

    public static BRAND Make(DoubleBl X, DoubleBl Y, DoubleBl Z, DoubleBl W) {
      return ToBrand(MultiArity<double, T, ARITY>.ArityI.Make(X, Y, Z, W));
    }
    public static BRAND Make(Point3DBl XYZ, DoubleBl W) {
      var e = TupleConstruct<double, Vec3<double>, double, T>.Instance.Make(XYZ.Underlying, W.Underlying);
      return ToBrand(e);
    }
    public static BRAND Identity {
      get { var ret = Make(0, 0, 0, 1); ret.IsNormalized = true; return ret; }
    }
    public static BRAND operator +(QuaternionBl<T, BRAND, ARITY> op0, DoubleBl d) {
      return Make(op0.XYZ(), op0.W + d);
    }
    public static BRAND operator +(DoubleBl d, QuaternionBl<T, BRAND, ARITY> op0) {
      return Make(op0.XYZ(), op0.W + d);
    }
    public static BRAND operator -(QuaternionBl<T, BRAND, ARITY> op0, DoubleBl d) {
      return Make(op0.XYZ(), op0.W - d);
    }
    public static BRAND operator -(DoubleBl d, QuaternionBl<T, BRAND, ARITY> op0) {
      return Make(op0.XYZ() * -1d, d - op0.W);
    }


    public override BRAND Add(BRAND other) { return base.Add(other); }


    // Axis nromalized
    public Point3DBl Axis { get { return this.XYZ().Normalize; } }
    public Angle2DBl Angle {
      get {
        var msin = this.XYZ().Length;
        var mcos = this.W;
        var maxcoeff = this.XYZ().Abs.Max();
        var msin0 = (this.XYZ() / maxcoeff).Length;
        var mcos0 = this.W / maxcoeff;
        msin = msin.IsInfinity.Condition(msin0, msin);
        mcos = msin.IsInfinity.Condition(mcos0, mcos);
        return
          2d * new PointBl(mcos, msin).Atan2;
        //msin.Atan2(mcos);
      }
    }
    public BRAND Conjugate { get { return Make(this.XYZ() * -1d, W); } }
    public BRAND Invert {
      get {
        var ret = Normalize.Conjugate;
        return ret;
      }
    }
    public MatrixBl<D4, D4> Rotate() { return RotateAt(0d); }

    public class RotateOp : Ops.Operator<T, Vec3<double>, Matrix<double, D4, D4>, RotateOp>, IMemoOperator<Matrix<double, D4, D4>> {
      public new static readonly RotateOp Instance = new RotateOp();
      private RotateOp() { }
      public override string Format(string argA, string argB) { return argA + ".rot[" + argB + "]"; }
      public override Expr<Matrix<double, D4, D4>> Expand(Expr<T> argA, Expr<Vec3<double>> argB) {
        var Self = ToBrand(argA);
        Point3DBl Center = argB;

        var xyz2 = Self.XYZ() * 2d;
        var x_xyz = Self.X * xyz2;
        var xx = x_xyz.X;
        var xy = x_xyz.Y;
        var xz = x_xyz.Z;

        var y_yz = Self.Y * xyz2.YZ();
        var yy = y_yz.X;
        var yz = y_yz.Y;
        var z_z = Self.Z * xyz2.Z;
        var zz = z_z;
        var w_xyz = Self.W * xyz2;
        var wx = w_xyz.X;
        var wy = w_xyz.Y;
        var wz = w_xyz.Z;


        var Table = new DoubleBl[4, 4];

        Table[0, 0] = 1.0 - (yy + zz);
        Table[1, 0] = xy + wz;
        Table[2, 0] = xz - wy;

        Table[0, 1] = xy - wz;
        Table[1, 1] = 1.0 - (xx + zz);
        Table[2, 1] = yz + wx;

        Table[0, 2] = xz + wy;
        Table[1, 2] = yz - wx;
        Table[2, 2] = 1.0 - (xx + yy);

        for (int i = 0; i < 3; i++) {
          Table[i, 3] = -Center[0] * Table[i, 0] - Center[1] * Table[i, 1] - Center[2] * Table[i, 2] + Center[i];
        }
        for (int i = 0; i < 4; i++)
          Table[3, i] = 0;
        return new MatrixBl<D4, D4>(Table).Underlying;
      }

    }


    public MatrixBl<D4, D4> RotateAt(Point3DBl Center) {
      return RotateOp.Instance.Make(Underlying, Center.Underlying);
    }

    public new bool IsNormalized { get; private set; }
    public BoolBl IsIdentity { get { throw new NotImplementedException(); } }

    public class TransformOp : Ops.Operator<T, Vec3<double>, Vec3<double>, TransformOp>, IMemoOperator<Vec3<double>> {
      public new static readonly TransformOp Instance = new TransformOp();
      private TransformOp() { }
      public override string Format(string argA, string argB) { return argA + ".transform[" + argB + "]"; }
      public override Expr<Vec3<double>> Expand(Expr<T> argA, Expr<Vec3<double>> argB) {
        var q = ToBrand(argA);
        
        var m = -q.XYZ().Square;
        m = m.YXX() + m.ZZY();

        var n = q.W * q.XYZ();
        var r = q.XXY() * q.YZZ();
        var t = r.XZY() - n.ZXY();
        var u = n.YZX() + r.YXZ();

        Point3DBl p = argB;

        var v = m * p.XYZ() + t * p.YZX() + u * p.ZXY();
        return 2d * v + p.XYZ();
      }

    }

    public Point3DBl Transform(Point3DBl p) {
      if (false) {
        // this path doesn't work for some reason. 
        var q = this;
        q = q.Normalize;
        var q2 = q.Conjugate;
        var qNode = Make(p, 0);
        qNode = q * qNode * q2;
        return qNode.Axis;
      } else if (true) {
        return TransformOp.Instance.Make(Normalize, p);
      }
    }

    /// <summary>
    /// Real dimension
    /// </summary>
    public new DoubleBl W { get { return base.W; } }
    /// <summary>
    /// Imaginary dimension for i
    /// </summary>
    public new DoubleBl X { get { return base.X; } }
    /// <summary>
    /// Imaginary dimension for j
    /// </summary>
    public new DoubleBl Y { get { return base.Y; } }
    /// <summary>
    /// Imaginary dimension for k
    /// </summary>
    public new DoubleBl Z { get { return base.Z; } }

    private DoubleBl BaseLength { get { return base.Length; } }

    public override DoubleBl Length {
      get {
        var length0 = BaseLength;
        var max = this.Abs.Max();
        return length0.IsInfinity.Condition((this / max).BaseLength * max, length0);
      }
    }
    static QuaternionBl() {
      Vecs.Vec4Arity<double>.CheckInit();
      Vecs.Vec3Arity<double>.CheckInit();
      Default = Identity;
      var val = Brand<BRAND>.Default;
      Console.WriteLine(val.CurrentValue);
    }
  }
  public partial class QuaternionBl : QuaternionBl<Vec4<double>, QuaternionBl, Vec4Arity<double>> {
    public QuaternionBl() : base(new Constant<Vec4<double>>(default(Vec4<double>))) { }
    public QuaternionBl(Expr<Vec4<double>> Underlying) : base(Underlying) { }
    public QuaternionBl(DoubleBl X, DoubleBl Y, DoubleBl Z, DoubleBl W) : base(X, Y, Z, W) { }
    public QuaternionBl(Point3DBl Axis, DoubleBl W) : base(Axis, W) { }
    public QuaternionBl(Point3DBl Axis, Angle2DBl Angle) : base(Axis, Angle) { }
    public QuaternionBl(Point3DBl Axis, Angle2DBl Angle, bool BeSafe) : base(Axis, Angle, BeSafe) { }
    static QuaternionBl() { Register(r => (r)); }
    public static implicit operator QuaternionBl(Expr<Vec4<double>> e) { return new QuaternionBl(e); }
  }
  public static class AngleExtensions {
    public static Angle2DBl PI(this double d) { return new Angle2DBl(d * Math.PI); }
    public static Angle2DBl PI(this int d) { return new Angle2DBl(d * Math.PI); }
    public static DegreeBl ToDegrees(this double d) { return new DegreeBl(d); }
    public static DegreeBl ToDegrees(this int d) { return new DegreeBl(d); }
    public static Angle2DBl ToAngle(this DoubleBl d) { return new Angle2DBl(d); }
    public static Angle2DBl ToAngle(this Expr<double> d) { return new Angle2DBl(d); }
    public static DegreeBl ToDegrees(this DoubleBl d) { return new DegreeBl(d); }
  }
  public partial class Point3DBl : BasePoint3DBl<Point3DBl>, ICoreBrandT, ICanWrapWPFPoint3D {
    public class AngleBetweenOp : Ops.Operator<Vec3<double>, Vec3<double>, Vec4<double>, AngleBetweenOp>, IMemoOperator<Vec4<double>> {
      public new static readonly AngleBetweenOp Instance = new AngleBetweenOp();
      private AngleBetweenOp() { }
      public override string Format(string argA, string argB) { return "angle(" + argA + ", " + argB + ")"; }
      public override Expr<Vec4<double>> Expand(Expr<Vec3<double>> argA, Expr<Vec3<double>> argB) {
        Point3DBl v0 = argA;
        Point3DBl v1 = argB;
        var Axis = v1.Cross(v0);
        var Angle = v1.Dot(v0).Acos;
        var q = new QuaternionBl(Axis, Angle);
        return q.Underlying;
      }

    }
    public QuaternionBl AngleBetween(Point3DBl v1) {
      return AngleBetweenOp.Instance.Make(this.Normalize, v1.Normalize);
    }
  }

}

