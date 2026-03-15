using System;
using System.Collections.Generic;
using Bling.DSL;
using Bling.Util;
using Bling.Linq;
using Bling.Core;
using Bling.Matrices;
using Bling.Vecs;

namespace Bling.Core {
  using Bling.Matrices;
  using Bling.Derivatives;
  public static partial class BrandExtensions {
    //IDimPointBl<PORTABLE, DIM>
    public static Func<DoubleBl,POINT> Derivative<POINT,DIM>(this Func<DoubleBl, POINT> F) where POINT : IDimPointBl<POINT,DIM>, IValueBrandK<double,POINT> where DIM : DNum<DIM> {
      var Target = new TargetExpr<double>("T", 1d.Bl());
      var P = F(Target);
      var arity = DNum<DIM>.Count;
      var Ds = new Expr<double>[arity];
      for (int i = 0; i < arity; i++) {
        Ds[i] = P[i].Underlying.DerivativeX;
        if (Ds[i] == null) return null; // no derivative.
        Ds[i] = Ds[i].Fold;
      }
      return T => {
        var replace = new ReplaceEval();
        replace.Replace[Target] = T.Underlying;
        var Fs = new Expr<double>[arity];
        for (int i = 0; i < Fs.Length; i++) Fs[i] = replace.Translate(Ds[i]);
        return ValueBrandK<double, POINT>.Make(Fs);
      };
    }
    public static Func<PointBl,MatrixBl<D2,D3>> Derivative<BRAND>(this DoubleFuncBl<PointBl, Point3DBl, BRAND> F) where BRAND : DoubleFuncBl<PointBl, Point3DBl, BRAND> {
      return F.Function.Derivative();
    }
    public static Func<PointBl,MatrixBl<D2, D3>> Derivative(this Func<PointBl, Point3DBl> F) {
      // R_2 => R_3
      // result = 2 by 3 matrix
      // produce n by m matrix

      AsConstant<double>[] AsConst = new AsConstant<double>[] {
        new AsConstant<double>("AX"), new AsConstant<double>("AY"),
      };
      TargetExpr<double>[] Targets = new TargetExpr<double>[] {
        new TargetExpr<double>("X", 1d.Bl()), new TargetExpr<double>("Y", 1d.Bl()),
      };
      DoubleBl[,] M = new DoubleBl[2, 3];
      for (int i = 0; i < 3; i++) {
        for (int j = 0; j < 2; j++) {
          var XY = new PointBl(j == 0 ? (Expr<double>) Targets[0] : AsConst[0],
                               j == 1 ? (Expr<double>) Targets[1] : AsConst[1]);
          var target = F(XY)[i].Underlying.Fold;
          var mji = target.DerivativeX;
          if (mji == null) 
            return null;
          M[j, i] = mji;
        }
      }
      return P => {
        // replace....
        var replace = new ReplaceEval();
        replace.Replace[AsConst[0]] = P.X.Underlying;
        replace.Replace[Targets[0]] = P.X.Underlying;
        replace.Replace[AsConst[1]] = P.Y.Underlying;
        replace.Replace[Targets[1]] = P.Y.Underlying;
        DoubleBl[,] M1 = new DoubleBl[2, 3];
        for (int i = 0; i < 3; i++) {
          for (int j = 0; j < 2; j++) {
            M1[j,i] = replace.Translate(M[j,i].Underlying);

          }
        }
        return new MatrixBl<D2, D3>(M1);
      };
    }

  }
  public abstract partial class Brand<T, BRAND> : Brand<BRAND>, IBrandT<T>, IExtends<BRAND,ObjectBl>, IBrand<T, BRAND>, ICanWrap<BRAND, T> where BRAND : Brand<T, BRAND> {
    public BRAND Extent(bool Max) {
      return ToBrand(Underlying.Extent(Max));
    }
  }




}
namespace Bling.Derivatives {
  public class AsConstant<T> : ParameterExpr<T> {
    public AsConstant(object key) : base(key) { }
    protected override Expr<T> Derivative1(Dictionary<Expr,Expr> Map) {
      return new Constant<T>(default(T)); 
    }
  }
  public class TargetExpr<T> : ParameterExpr<T> {
    private readonly Expr<T> One;
    public TargetExpr(object key, Expr<T> One) : base(key) { this.One = One;  }
    protected override Expr<T> Derivative1(Dictionary<Expr, Expr> Map) {
      return One;
    }
  }

}

namespace Bling.DSL {
  using Bling.Ops;
  public abstract partial class Expr<T> : Expr, IExpr<T> {
    public Expr<T> DerivativeX {
      get {
        return Derivative1(new Dictionary<Expr,Expr>());
      }
    }
    public Expr<T> Derivative(Dictionary<Expr, Expr> Map) { return Derivative1(Map); }
    protected virtual Expr<T> Derivative1(Dictionary<Expr, Expr> Map) {
      return null;
    }
    public virtual Expr<T> Extent(bool Max) {
        throw new NotSupportedException();
    }

  }
  public partial class Constant<T> : Expr<T>, IConstant {
    protected override Expr<T> Derivative1(Dictionary<Expr,Expr> Map) {
      return new Constant<T>(default(T)); 
    }
    public override Expr<T> Extent(bool Max) {
      return this;
    }
  }
  public abstract partial class ProxyExpr<T> : Expr<T>, IProxyExpr {
    protected override Expr<T> Derivative1(Dictionary<Expr,Expr> Map) {
      return Make(Underlying.Derivative(Map)); // remain proxied 
    }
    public override Expr<T> Extent(bool Max) {
      return Underlying.Extent(Max);
    }
  }
}

namespace Bling.Ops {
  public partial interface IOperatorX<S, T> : IOperator<T> {
    Expr<T> Derivative(Expr<S> argA, Dictionary<Expr,Expr> Map);
    Expr<T> Extent(Expr<S> argA, bool Max);
  }
  public partial interface OperatorX<S, T, U> : IOperator<U> {
    Expr<U> Derivative(Expr<S> argA, Expr<T> argB, Dictionary<Expr, Expr> Map);
    Expr<U> Extent(Expr<S> argA, Expr<T> argB, bool Max);
  }
  public partial interface OperatorX<S, T, U, V> : IOperator<V> {
    Expr<V> Derivative(Expr<S> argA, Expr<T> argB, Expr<U> argC, Dictionary<Expr, Expr> Map);
    Expr<V> Extent(Expr<S> argA, Expr<T> argB, Expr<U> argC, bool Max);
  }
  public partial interface OperatorX<S, T, U, V, W> : IOperator<W> {
    Expr<W> Derivative(Expr<S> argA, Expr<T> argB, Expr<U> argC, Expr<V> argD, Dictionary<Expr, Expr> Map);
    Expr<W> Extent(Expr<S> argA, Expr<T> argB, Expr<U> argC, Expr<V> argD, bool Max);
  }
  public abstract partial class CastOperator<S, T, CNT> : Operator<S, T, CNT> where CNT : CastOperator<S, T, CNT> {
    public override Expr<T> Extent(Expr<S> argA, bool Max) {
      var a = argA.Extent(Max);
      return Instance.Make(a);
    }
  }


  public partial class BaseOperatorX<S, T> : BaseOperatorX<T>, IOperatorX<S, T> {
    public virtual Expr<T> Derivative(Expr<S> argA, Dictionary<Expr, Expr> Map) {
      var e = Expand(argA);
      if (e != null) return e.Derivative(Map);
      return null; 
    }
    public virtual Expr<T> Extent(Expr<S> argA, bool Max) {
      var e = Expand(argA);
      if (e != null) return e.Extent(Max);
      throw new NotSupportedException();
    }
  }
  public partial class BaseOperatorX<S, T, U> {
    public virtual Expr<U> Derivative(Expr<S> argA, Expr<T> argB, Dictionary<Expr, Expr> Map) {
      var e = Expand(argA, argB);
      if (e != null) return e.Derivative(Map);
      return null;
    }
    public virtual Expr<U> Extent(Expr<S> argA, Expr<T> argB, bool Max) {
      var e = Expand(argA, argB);
      if (e != null) return e.Extent(Max);
      throw new NotSupportedException();
    }
  }
  public partial class BaseOperatorX<S, T, U, V> {
    public virtual Expr<V> Derivative(Expr<S> argA, Expr<T> argB, Expr<U> argC, Dictionary<Expr, Expr> Map) {
      var e = Expand(argA, argB, argC);
      if (e != null) return e.Derivative(Map);
      return null;
    }
    public virtual Expr<V> Extent(Expr<S> argA, Expr<T> argB, Expr<U> argC, bool Max) {
      var e = Expand(argA, argB, argC);
      if (e != null) return e.Extent(Max);
      throw new NotSupportedException();
    }
  }
  public partial class BaseOperatorX<S, T, U, V, W> {
    public virtual Expr<W> Derivative(Expr<S> argA, Expr<T> argB, Expr<U> argC, Expr<V> argD, Dictionary<Expr, Expr> Map) {
      var e = Expand(argA, argB, argC, argD);
      if (e != null) return e.Derivative(Map);
      return null;
    }
    public virtual Expr<W> Extent(Expr<S> argA, Expr<T> argB, Expr<U> argC, Expr<V> argD, bool Max) {
      var e = Expand(argA, argB, argC, argD);
      if (e != null) return e.Extent(Max);
      throw new NotSupportedException();
    }
  }
  public partial class Operation<S, T> : BaseOperation<T, IOperatorX<S, T>> {
    protected override Expr<T> Derivative1(Dictionary<Expr,Expr> Map) {
      var ret = Op.Derivative(ArgA, Map);
      return ret;
    }
    public override Expr<T> Extent(bool Max) {
      return Op.Extent(ArgA, Max);
    }
  }
  public partial class Operation<S, T, U> : BaseOperation<U, OperatorX<S, T, U>> {
    protected override Expr<U> Derivative1(Dictionary<Expr,Expr> Map) {
      if (Map.ContainsKey(this)) return (Expr<U>)Map[this];
      var ret = Op.Derivative(ArgA,ArgB, Map);
      if (ret != null) Map[this] = ret;
      return ret;
    }
    public override Expr<U> Extent(bool Max) {
      return Op.Extent(ArgA, ArgB, Max);
    }
  }
  public partial class Operation<S, T, U, V> : BaseOperation<V, OperatorX<S, T, U, V>> {
    protected override Expr<V> Derivative1(Dictionary<Expr,Expr> Map) {
      if (Map.ContainsKey(this)) return (Expr<V>)Map[this];
      var ret = Op.Derivative(ArgA, ArgB, ArgC, Map);
      if (ret != null) Map[this] = ret;
      return ret;
    }
    public override Expr<V> Extent(bool Max) {
      return Op.Extent(ArgA, ArgB, ArgC, Max);
    }
  }
  public partial class Operation<S, T, U, V, W> : BaseOperation<W, OperatorX<S, T, U, V, W>> {
    protected override Expr<W> Derivative1(Dictionary<Expr,Expr> Map) {
      if (Map.ContainsKey(this)) return (Expr<W>)Map[this];
      var ret = Op.Derivative(ArgA, ArgB, ArgC, ArgD, Map);
      if (ret != null) Map[this] = ret;
      return ret;
    }
    public override Expr<W> Extent(bool Max) {
      return Op.Extent(ArgA, ArgB, ArgC, ArgD, Max);
    }
  }
  public static partial class BoolOperators<T> {
    private static readonly int minMaxInit0 = minMaxInit();
    private static int minMaxInit() {
      ValueOperators<T>.Max = Or.Instance.Make;
      ValueOperators<T>.Min = And.Instance.Make;
      return 0;
    }
  }
  public static partial class NumericOperators0<T, K> {
    public partial class Abs : MathOperator<Abs> {
      public override Expr<T> Derivative(Expr<T> argA, Dictionary<Expr, Expr> Map) {
        var DArgA = argA.Derivative(Map);
        if (DArgA == null) return base.Derivative(argA, Map);
        var lt = NumericOperators<T, K, bool>.LessThan.Instance.Make(argA, Arity<K,T>.ArityI.Repeat(default(K)));
        return ValueOperators<T>.Condition.Instance.Make(lt, Negate.Instance.Make(argA).Derivative(Map), DArgA);
      }
      public override Expr<T> Extent(Expr<T> argA, bool MaxD) {
        var a0 = Instance.Make(argA.Extent( MaxD));
        var a1 = Instance.Make(argA.Extent(!MaxD));
        if (MaxD) return Max.Instance.Make(a0, a1);
        else return Min.Instance.Make(a0, a1);
      }
    }

    private static readonly int minMaxInit0 = minMaxInit();
    private static int minMaxInit() {
      ValueOperators<T>.Max = Max.Instance.Make;
      ValueOperators<T>.Min = Min.Instance.Make;
      return 0;
    }

    public partial class Min : Math2Operator<Min> {
      public override Expr<T> Derivative(Expr<T> argA, Expr<T> argB, Dictionary<Expr, Expr> Map) {
        var DArgA = argA.Derivative(Map);
        var DArgB = argB.Derivative(Map);
        if (DArgA == null || DArgB == null) return base.Derivative(argA,argB,Map);
        var lt = NumericOperators<T, K, bool>.LessThan.Instance.Make(argA, argB);
        return ValueOperators<T>.Condition.Instance.Make(lt, DArgA, DArgB);
      }
      public override Expr<T> Extent(Expr<T> argA, Expr<T> argB, bool MaxD) {
        var a = argA.Extent(MaxD);
        var b = argB.Extent(MaxD);
        if (MaxD) return Max.Instance.Make(a, b);
        else return Min.Instance.Make(a, b);
      }
    }
    public partial class Max : Math2Operator<Max> {
      public override Expr<T> Derivative(Expr<T> argA, Expr<T> argB, Dictionary<Expr, Expr> Map) {
        var DArgA = argA.Derivative(Map);
        var DArgB = argB.Derivative(Map);
        if (DArgA == null || DArgB == null) return base.Derivative(argA, argB, Map);
        var gt = NumericOperators<T, K, bool>.GreaterThan.Instance.Make(argA, argB);
        return ValueOperators<T>.Condition.Instance.Make(gt, DArgA, DArgB);
      }
      public override Expr<T> Extent(Expr<T> argA, Expr<T> argB, bool MaxD) {
        var a = argA.Extent(MaxD);
        var b = argB.Extent(MaxD);
        if (MaxD) return Max.Instance.Make(a, b);
        else return Min.Instance.Make(a, b);
      }
    }
    public partial class Negate : UniformUnaryOperator<T, K, Negate> {
      public override Expr<T> Derivative(Expr<T> argA, Dictionary<Expr, Expr> Map) {
        var DargA = argA.Derivative(Map);
        if (DargA != null)
          return Instance.Make(DargA);
        return base.Derivative(argA, Map);
      }
      public override Expr<T> Extent(Expr<T> argA, bool Max) {
        var a = argA.Extent(!Max);
        return Negate.Instance.Make(a);
      }
    }
    public abstract partial class ArithmeticOperator<CNT> : UniformBinaryOperator<T, K, CNT> where CNT : ArithmeticOperator<CNT> {
    }

    public partial class Add : ArithmeticOperator<Add> {
      public override Expr<T> Derivative(Expr<T> argA, Expr<T> argB, Dictionary<Expr, Expr> Map) {
        var DargA = argA.Derivative(Map);
        var DargB = argB.Derivative(Map);
        if (DargA == null || DargB == null) return base.Derivative(argA, argB, Map);
        return Add.Instance.Make(DargA, DargB);
      }
      public override Expr<T> Extent(Expr<T> argA, Expr<T> argB, bool Max) {
        var a = argA.Extent(Max);
        var b = argB.Extent(Max);
        return Instance.Make(a, b);
      }
    }
    public partial class Subtract : ArithmeticOperator<Subtract> {
      public override Expr<T> Derivative(Expr<T> argA, Expr<T> argB, Dictionary<Expr, Expr> Map) {
        var DargA = argA.Derivative(Map);
        var DargB = argB.Derivative(Map);
        if (DargA == null || DargB == null) return base.Derivative(argA, argB, Map);
        return Subtract.Instance.Make(DargA, DargB);
      }
      public override Expr<T> Extent(Expr<T> argA, Expr<T> argB, bool Max) {
        var a = argA.Extent(Max);
        var b = Negate.Instance.Make(argB).Extent(Max);
        return Add.Instance.Make(a, b);
      }
    }
    public partial class Multiply : ArithmeticOperator<Multiply> {
      public override Expr<T> Derivative(Expr<T> argA, Expr<T> argB, Dictionary<Expr, Expr> Map) {
        var DargA = argA.Derivative(Map);
        var DargB = argB.Derivative(Map);
        if (DargA == null || DargB == null) return base.Derivative(argA, argB, Map);
        return Add.Instance.Make(Multiply.Instance.Make(argA, DargB), Multiply.Instance.Make(argB, DargA));
      }
      public override Expr<T> Extent(Expr<T> argA, Expr<T> argB, bool MaxD) {
        var a0 = argA.Extent( MaxD);
        var b0 = argB.Extent( MaxD);
        return Instance.Make(a0, b0);
      }
    }
    public partial class Divide : ArithmeticOperator<Divide> {
      public override Expr<T> Derivative(Expr<T> argA, Expr<T> argB, Dictionary<Expr,Expr> Map) {
        // 1 * (1 / u)
        if (typeof(K) == typeof(double)) {
          // convert argB into 1d / argB
          argB = DoubleOperators0<T>.Recip.Instance.Make(argB);
          return Multiply.Instance.Make(argA, argB).Derivative(Map);
        } else return base.Derivative(argA, argB, Map);
      }
      public override Expr<T> Extent(Expr<T> argA, Expr<T> argB, bool MaxD) {
        if (MaxD) {
          var aH = argA.Extent( MaxD);
          var bL = argB.Extent(!MaxD);
          return Instance.Make(aH, bL);
        } else {
          var aL = argA.Extent(!MaxD);
          var bH = argB.Extent( MaxD);
          return Instance.Make(aL, bH);
        }
      }
    }
  }
  public static partial class DoubleOperators0<T> {

    public partial class Length : StaticCallOperator<T, double, Length> {
      public override Expr<double> Extent(Expr<T> argA, bool Max) {
        return Instance.Make(argA.Extent(Max));
      }
    }

    public partial class Cross : Math2Operator<Cross> {
      public override Expr<T> Derivative(Expr<T> argA, Expr<T> argB, Dictionary<Expr,Expr> Map) {
        return base.Derivative(argA, argB, Map);
      }
    }
    public partial class Recip : MathOperator<Recip> {
      public override Expr<T> Derivative(Expr<T> argA, Dictionary<Expr,Expr> Map) {
        return Pow.Instance.Make(argA, Arity<double, T>.ArityI.Repeat(-1)).Derivative(Map);
      }
      public override Expr<T> Extent(Expr<T> argA, bool Max) {
        return Instance.Make(argA.Extent(!Max));
      }
    }
    public partial class Exp : MathOperator<Exp> {
      public override Expr<T> Derivative(Expr<T> argA, Dictionary<Expr,Expr> Map) {
        // e^argA
        var DargA = argA.Derivative(Map);
        if (DargA == null) return base.Derivative(argA, Map);
        return NumericOperators0<T, double>.Multiply.Instance.Make(DargA,
          Exp.Instance.Make(argA));
      }
      public override Expr<T> Extent(Expr<T> argA, bool Max) {
        return Instance.Make(argA.Extent(Max));
      }
    }
    public partial class Log : MathOperator<Log> {
      public override Expr<T> Derivative(Expr<T> argA, Dictionary<Expr,Expr> Map) {
        var DargA = argA.Derivative(Map);
        if (DargA == null) return base.Derivative(argA, Map);
        return NumericOperators0<T, double>.Divide.Instance.Make(DargA, argA);
      }
      public override Expr<T> Extent(Expr<T> argA, bool Max) {
        return Instance.Make(argA.Extent(Max));
      }
    }
    public partial class Pow : Math2Operator<Pow> {
      public override Expr<T> Derivative(Expr<T> argA, Expr<T> argB, Dictionary<Expr,Expr> Map) {
        var a = argA.Fold;
        var r = argB.Fold;
        if (a is Constant<T>) {
          // a^x 
          return NumericOperators0<T, double>.Multiply.Instance.Make(Log.Instance.Make(a), 
            Pow.Instance.Make(a, r));
        } else if (r is Constant<T>) {
          Expr<T> one = Arity<double, T>.ArityI.Repeat(1d);
          return NumericOperators0<T, double>.Multiply.Instance.Make(r,
            Pow.Instance.Make(a, NumericOperators0<T, double>.Subtract.Instance.Make(r, one)));
        }
        return base.Derivative(argA, argB, Map);
      }
      public override Expr<T> Extent(Expr<T> argA, Expr<T> argB, bool Max) {
        var a = argA.Extent(Max);
        var r = argB.Extent(Max);
        return Instance.Make(a, r);
      }

    }
    public partial class Sqrt : MathOperator<Sqrt> {
      public override Expr<T> Derivative(Expr<T> argA, Dictionary<Expr,Expr> Map) {
        var c = Arity<double, T>.ArityI.Repeat(.5);
        return Pow.Instance.Make(argA, c).Derivative(Map);
      }
      public override Expr<T> Extent(Expr<T> argA, bool Max) {
        return Instance.Make(argA.Extent(Max));
      }
    }
  }
  public static partial class TrigOperators<T> {
    public partial class Sin : MathOperator<Sin> {
      public override Expr<T> Derivative(Expr<T> argA, Dictionary<Expr,Expr> Map) {
        var DargA = argA.Derivative(Map);
        if (DargA == null) return base.Derivative(argA, Map);
        return NumericOperators0<T, double>.Multiply.Instance.Make(
          DargA, Cos.Instance.Make(argA));
      }
    }
    public partial class Cos : MathOperator<Cos> {
      public override Expr<T> Derivative(Expr<T> argA, Dictionary<Expr,Expr> Map) {
        var DargA = argA.Derivative(Map);
        if (DargA == null) return base.Derivative(argA, Map);
        return NumericOperators0<T, double>.Multiply.Instance.Make(
          DargA, NumericOperators0<T, double>.Negate.Instance.Make(Sin.Instance.Make(argA)));
      }
    }
    public partial class Asin : MathOperator<Asin> {
      public override Expr<T> Derivative(Expr<T> argA, Dictionary<Expr,Expr> Map) {
        var DargA = argA.Derivative(Map);
        if (DargA == null) return base.Derivative(argA, Map);
        return NumericOperators0<T, double>.Divide.Instance.Make(
          DargA, DoubleOperators0<T>.Sqrt.Instance.Make(
           NumericOperators0<T, double>.Subtract.Instance.Make(
           Arity<double,T>.ArityI.Repeat(1d), 
             DoubleOperators0<T>.Square.Instance.Make(argA)
           )));
      }
    }
    public partial class Acos : MathOperator<Acos> {
      public override Expr<T> Derivative(Expr<T> argA, Dictionary<Expr,Expr> Map) {
        var DargA = Asin.Instance.Make(argA).Derivative(Map);
        if (DargA == null) return base.Derivative(argA, Map);
        return NumericOperators0<T, double>.Negate.Instance.Make(DargA);
      }
    }
  }
  public static partial class ValueOperators<T> {
    public static Func<Expr<T>, Expr<T>, Expr<T>> Max;
    public static Func<Expr<T>, Expr<T>, Expr<T>> Min;

    public partial class BaseCondition<OP> : Operator<bool, T, T, T, OP> where OP : BaseCondition<OP> {
      public override Expr<T> Derivative(Expr<bool> argA, Expr<T> argB, Expr<T> argC, Dictionary<Expr, Expr> Map) {
        var DargB = argB.Derivative(Map);
        var DargC = argC.Derivative(Map);
        if (DargB == null || DargC == null) return null;
        return Instance.Make(argA, DargB, DargC);
      }
      public override Expr<T> Extent(Expr<bool> argA, Expr<T> argB, Expr<T> argC, bool MaxD) {
        var a = argB.Extent(MaxD);
        var b = argC.Extent(MaxD);
        var arity = Arity<T>.ArityI;
        if (MaxD) return Max(a, b);
        else return Min(a, b);
      }
    }
  }
  public static partial class VecConvert<T, K, VEC> where VEC : Vec<K> {
    public partial class FromVec : Operator<VEC, T, FromVec> {
      public override Expr<T> Derivative(Expr<VEC> argA, Dictionary<Expr, Expr> Map) {
        var DargA = argA.Derivative(Map);
        if (DargA != null) return Instance.Make(DargA);
        return base.Derivative(argA, Map);
      }
    }
    public partial class ToVec : Operator<T, VEC, ToVec> {
      public override Expr<VEC> Derivative(Expr<T> argA, Dictionary<Expr,Expr> Map) {
        var DargA = argA.Derivative(Map);
        if (DargA != null) return Instance.Make(DargA);
        return base.Derivative(argA, Map);
      }
    }

  }
}
namespace Bling.Vecs {
  using Bling.Ops;

  public abstract partial class MultiArity<K, T, ARITY> : Arity<K, T, ARITY>, IMultiArity where ARITY : MultiArity<K, T, ARITY>, new() {
    public abstract partial class VecExpr : Expr<T>, IComposite<K> {
      public override Expr<T> Extent(bool Max) {
        var Ek = new Expr<K>[ArityI.Count];
        for (int i = 0; i < ArityI.Count; i++)
          Ek[i] = this[i].Extent(Max);
        return ArityI.Make(Ek);
      }

      protected override Expr<T> Derivative1(Dictionary<Expr, Expr> Map) {
        var Ek = new Expr<K>[ArityI.Count];

        for (int i = 0; i < ArityI.Count; i++) {
          Ek[i] = this[i].Derivative(Map);
          if (Ek[i] == null) return base.Derivative(Map);
        }
        var hc = Ek[0].GetHashCode();


        return ArityI.Make(Ek);
      }
      
    }
    public partial class AccessExpr : Expr<K> {
      public override Expr<K> Extent(bool Max) {
        return new AccessExpr(Vector.Extent(Max), Idx);
      }

      protected override Expr<K> Derivative1(Dictionary<Expr, Expr> Map) {
        var Vector = this.Vector;
        var DVector = Vector.Derivative(Map);
        if (DVector != null) 
          return Make(DVector, Idx);

        //return new AccessExpr(DVector, Idx);


        Expr<T> expanded = null;
        Operation<T> lastExpanded = null;
        (!(Vector is VecExpr)).Assert();
        expanded = Vector;
        while (true) {
          if (expanded is Operation<T>) {
            lastExpanded = (Operation<T>)expanded;
            if (((Operation<T>)expanded).BaseOp == ValueOperators<T>.Condition.Instance) {
              var expanded0 = (Operation<bool, T, T, T>)expanded;
              if (expanded0.ArgB is VecExpr && expanded0.ArgC is VecExpr) {
                var ret = ValueOperators<K>.Condition.Instance.Make(expanded0.ArgA,
                  ((VecExpr)expanded0.ArgB)[Idx],
                  ((VecExpr)expanded0.ArgC)[Idx]).Derivative(Map);
                if (ret != null) return ret;
                else expanded = null;
              } else if (expanded0.ArgB is Operation<T> && expanded0.ArgC is Operation<T>) {
                var argB = ((Operation<T>)expanded0.ArgB).Expanded;
                var argC = ((Operation<T>)expanded0.ArgC).Expanded;
                if (argB != null && argC != null)
                  expanded = ValueOperators<T>.Condition.Instance.Make(expanded0.ArgA, argB, argC);
                else
                  expanded = null;
              } else
                expanded = null;
            } else {
              expanded = ((Operation<T>)expanded).Expanded;
              if (expanded is VecExpr) {
                var e = ((VecExpr)expanded)[Idx];
                var ret = e.Derivative(Map);
                if (ret != null) return ret;
                else expanded = null;
              } else {
                true.Assert();
              }
            }
          } else if (expanded is Constant<T>) return new Constant<K>(default(K));
          else if (expanded is ProxyExpr<T>) expanded = ((ProxyExpr<T>)expanded).Underlying;
          else break;
        }
        return base.Derivative1(Map);
      }
    }
  }
}


namespace Bling.Derivatives {
  public abstract class DeriveInfo : Eval<DeriveEval>.Info {
    public abstract object BaseValue { get; }
  }
  public class DeriveInfo<T> : DeriveInfo, Eval<DeriveEval>.Info<T> {
    public T Value;
    public override object BaseValue { get { return Value; } }
  }
  public class DeriveEval : Eval<DeriveEval> {


    public override Eval<DeriveEval>.Info<T> Constant<T>(Constant<T> constant) {
      return new DeriveInfo<T>() { Value = constant.Value };
    }

    // do not worry about these.
    public override Eval<DeriveEval>.Info<T> Composite<K, T, ARITY>(IComposite<K, T> Expr, params Eval<DeriveEval>.Info<K>[] info) {
      throw new NotImplementedException();
    }
    public override Eval<DeriveEval>.Info<K> Access<K, T, ARITY>(Eval<DeriveEval>.Info<T> info, int Idx) {
      throw new NotImplementedException();
    }
  }

}