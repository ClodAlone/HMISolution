using System;
using Microsoft.Linq.Expressions;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using Bling.DSL;
using Bling.Util;
using Bling.Core;
using System.Linq;
namespace Bling.Core {
  public abstract partial class Brand<T, BRAND> {
    public class SimpleArray {
      internal T[] Values;
      public BRAND this[IntBl Idx] {
        get { return ToBrand(new ArrayExpr<T>(Values, Idx));  }
      }
    }
    public static SimpleArray Array(int Size, Func<IntBl, BRAND> F) {
      var Values = new T[Size];
      for (int i = 0; i < Size; i++)
        Values[i] = F(i).CurrentValue;
      return new SimpleArray() { Values = Values };
    }
  }
}
namespace Bling.DSL {
  public abstract partial class Expr : IExpr {
    public abstract object BaseCurrentValue { get; }
  }
  public abstract partial class Expr<T> : Expr, IExpr<T> {
    private Func<T> CurrentValue0;
    public T CurrentValue { get { return AsFunc(); } }
    public Func<T> AsFunc {
      get {
        if (CurrentValue0 == null) {
          CurrentValue0 = (new Bling.Linq.DefaultLinqEval()).Eval<T>(this);
          if (CurrentValue0() == null) {
            CurrentValue0 = (new Bling.Linq.DefaultLinqEval()).Eval<T>(this);
          }
        }
        return CurrentValue0;
      }
    }
    public override object BaseCurrentValue { get { return CurrentValue; } }

  }
  public interface IArrayEval<EVAL> : IEval<EVAL> where EVAL : Eval<EVAL> {
    Eval<EVAL>.Info<T> Eval<T>(T[] Values, Eval<EVAL>.Info<int> Index);
  }
  public class ArrayExpr<T> : Expr<T>, ICanAssignExpr<T,Expression> {
    private readonly T[] Values;
    private readonly Expr<int> Index;
    public ArrayExpr(T[] Values, Expr<int> Index) {
      this.Values = Values; this.Index = Index;
    }
    protected override string ToString1() {
      return Values + "[" + Index + "]";
    }
    protected override int GetHashCode0() {
      return Values.GetHashCode() + Index.GetHashCode();
    }
    protected override bool Equals0(Expr<T> obj) {
      if (obj is ArrayExpr<T>)
        return Values.Equals(((ArrayExpr<T>)obj).Values) &&
                Index.Equals(((ArrayExpr<T>)obj).Index);
      return base.Equals0(obj);
    }
    protected override Eval<EVAL>.Info<T> Eval<EVAL>(Eval<EVAL> txt) {
      if (txt is IArrayEval<EVAL>) return ((IArrayEval<EVAL>)txt).Eval(Values, txt.DoEval(Index));
      throw new NotImplementedException();
    }
    public Expression Assign<EVAL>(Expression RHS0, IExpressionEval<EVAL, Expression> txt) where EVAL : Eval<EVAL> {
      return Expression.Assign(
        Expression.ArrayAccess(
          Expression.Constant(Values, typeof(T[])), txt.Extract(txt.DoEval(Index))), RHS0);
    }

    public Expression LinqAssign<EVAL>(Expression RHS0, Linq.LinqEval<EVAL> txt) where EVAL : Eval<EVAL> {
      return Expression.Assign(
        Expression.ArrayAccess(
          Expression.Constant(Values, typeof(T[])), txt.DoEval(Index).Value), RHS0);
    }
  }
}
namespace Bling.Core {
  using Bling.Linq;
  public partial interface IBrandT<T> : IBrand {
    T CurrentValue { get; }
    Func<T> AsFunc { get; } 
  }
  public abstract partial class Brand<BRAND> : Brand, IBrand<BRAND>, IConditionTarget<BRAND> where BRAND : Brand<BRAND> {



  }

  public abstract partial class Brand<T, BRAND> : Brand<BRAND>, IBrandT<T>, IExtends<BRAND,ObjectBl> where BRAND : Brand<T, BRAND> {
    public T CurrentValue { get { return Underlying.CurrentValue; } }
    public Func<T> AsFunc { get { return Underlying.AsFunc; } }



    public static Func<T,S> Lambda<S,SBRAND>(BrandId<S,SBRAND> Id, Func<BRAND, SBRAND> F) where SBRAND : Brand<S,SBRAND> {
      return new DefaultLinqEval().Eval<T,S>(t => F(ToBrand(t)));
    }
    public static Func<T, S> Lambda<S, SBRAND>(Func<BRAND, Brand<S,SBRAND>> F) where SBRAND : Brand<S, SBRAND> {
      return new DefaultLinqEval().Eval<T, S>(t => F(ToBrand(t)));
    }
  }
  public static partial class BrandExtensions {
    //public static Func<T,S> F(this Brand

  }

}

namespace Bling.Vecs {
  public abstract partial class Arity : IArity {
    public abstract Func<Expression, Expression> PropertyFor(int Idx);
  }
  public partial class Arity1<T> : Arity<T, T, Arity1<T>> {
    public override Func<Expression, Expression> PropertyFor(int Idx) {
      (Idx == 0).Assert();
      return e => e;
    }
  }

  public partial interface IMultiArity : IArity {
    Func<Expression, Expression> PropertyFor(int Idx);
  }

  public abstract partial class MultiArity<K, T, ARITY> : Arity<K, T, ARITY>, IMultiArity where ARITY : MultiArity<K, T, ARITY>, new() {
    public abstract string[] PropertyNames { get; }
    public override Func<Expression,Expression> PropertyFor(int Idx) {
      var p = typeof(T).GetProperty(PropertyNames[Idx]);
      (p != null).Assert();
      return e => Expression.Property(e, p);
    }
  }
  public partial class Vec2Arity<T> : Arity2<T, Vec2<T>, Vec2Arity<T>> {
    public override string[] PropertyNames {
      get { return new string[] { "X", "Y" }; }
    }
  }
  public partial class Vec3Arity<T> : Arity3<T, Vec3<T>, Vec3Arity<T>> {
    public override string[] PropertyNames {
      get { return new string[] { "X", "Y", "Z", }; }
    }
  }
  public partial class Vec4Arity<T> : Arity4<T, Vec4<T>, Vec4Arity<T>> {
    public override string[] PropertyNames {
      get { return new string[] { "X", "Y", "Z", "W" }; }
    }

  }

}
namespace Bling.Ops {
  using linq = Microsoft.Linq.Expressions;
  using Bling.Vecs;

  public partial class Sign : StaticCallOperator<double, double, Sign> {
    public override Func<Expression, Expression> Linq(Operation<double, double> op) {
      return null;
    }
  }

  public partial interface IBaseOperator<S, TDelegate> : IOperator<S> {
    TDelegate AsFunc { get; }
  }

  public partial interface IOperatorX<S, T> : IOperator<T> {
    Func<linq.Expression, linq.Expression> Linq(Operation<S, T> op);
  }
  public partial interface OperatorX<S, T, U> : IOperator<U> {
    Func<linq.Expression, linq.Expression, linq.Expression> Linq(Operation<S, T, U> op);
  }
  public partial interface OperatorX<S, T, U, V> : IOperator<V> {
    Func<linq.Expression, linq.Expression, linq.Expression, linq.Expression> Linq(Operation<S, T, U, V> op);
  }
  public partial interface OperatorX<S, T, U, V, W> : IOperator<W> {
    Func<linq.Expression, linq.Expression, linq.Expression, linq.Expression, linq.Expression> Linq(Operation<S, T, U, V, W> op);
  }
  public partial class BaseOperatorX<S, T> : BaseOperatorX<T>, IOperatorX<S, T> {
    public virtual Func<linq.Expression, linq.Expression> Linq(Operation<S, T> op) { 
      return null; 
    }
    private Func<S, T> AsFunc0;
    public Func<S, T> AsFunc {
      get {
        if (AsFunc0 != null) return AsFunc0;
        var F = Linq(null);
        var paramS = linq.Expression.Parameter(typeof(S), "paramS");

        linq.Expression body;
        if (F == null) {
          var exprS = new Bling.Linq.LinqExpr<S>(paramS);
          var expand = Expand(exprS);
          if (expand == null) return null;
          // linq eval...
          AsFunc0 = (new Bling.Linq.DefaultLinqEval()).Eval<S, T>((exprS0) => Expand(exprS0));
        } else {
          body = F(paramS);
          var G = linq.Expression.Lambda<Microsoft.Func<S, T>>(body, paramS).Compile();
          AsFunc0 = (s) => G(s);
        }
        return AsFunc0;
      }
    }
  }
  public partial class BaseOperatorX<S, T, U> {
    public virtual Func<linq.Expression, linq.Expression, linq.Expression> Linq(Operation<S, T, U> op) { return null; }
    private Func<S, T, U> AsFunc0;
    public Func<S, T, U> AsFunc {
      get {
        if (AsFunc0 != null) return AsFunc0;
        var F = Linq(null);
        var paramS = linq.Expression.Parameter(typeof(S), "paramS");
        var paramT = linq.Expression.Parameter(typeof(T), "paramT");
        linq.Expression body;
        if (F == null) {
          var exprS = new Bling.Linq.LinqExpr<S>(paramS);
          var exprT = new Bling.Linq.LinqExpr<T>(paramT);
          var expand = Expand(exprS, exprT);
          if (expand == null) return null;
          // linq eval...
          AsFunc0 = (new Bling.Linq.DefaultLinqEval()).Eval<S,T,U>((exprS0, exprT0) => Expand(exprS0, exprT0));


        } else {
          body = F(paramS, paramT);
          var G = linq.Expression.Lambda<Microsoft.Func<S, T, U>>(body, paramS, paramT).Compile();
          AsFunc0 = (s, t) => G(s, t);
        }
        return AsFunc0;
      }
    }
  }
  public partial class BaseOperatorX<S, T, U, V> {
    public virtual Func<linq.Expression, linq.Expression, linq.Expression, linq.Expression> Linq(Operation<S, T, U, V> op) { return null; }
    private Func<S, T, U, V> AsFunc0;
    public Func<S, T, U, V> AsFunc {
      get {
        if (AsFunc0 != null) return AsFunc0;
        var F = Linq(null);
        var paramS = linq.Expression.Parameter(typeof(S), "paramS");
        var paramT = linq.Expression.Parameter(typeof(T), "paramT");
        var paramU = linq.Expression.Parameter(typeof(U), "paramU");
        linq.Expression body;
        if (F == null) {
          var exprS = new Bling.Linq.LinqExpr<S>(paramS);
          var exprT = new Bling.Linq.LinqExpr<T>(paramT);
          var exprU = new Bling.Linq.LinqExpr<U>(paramU);
          var expand = Expand(exprS, exprT, exprU);
          if (expand == null) return null;
          // linq eval...
          AsFunc0 = (new Bling.Linq.DefaultLinqEval()).Eval<S, T, U, V>((exprS0, exprT0, exprU0) => Expand(exprS0, exprT0, exprU0));
        } else {
          body = F(paramS, paramT, paramU);
          var G = linq.Expression.Lambda<Microsoft.Func<S, T, U, V>>(body, paramS, paramT, paramU).Compile();
          AsFunc0 = (s, t, u) => G(s, t, u);
        }
        return AsFunc0;
      }
    }
  }
  public partial class BaseOperatorX<S, T, U, V, W> {
    public virtual Func<linq.Expression, linq.Expression, linq.Expression, linq.Expression, linq.Expression> Linq(Operation<S, T, U, V, W> op) { return null; }
    private Func<S, T, U, V, W> AsFunc0;
    public Func<S, T, U, V, W> AsFunc {
      get {
        if (AsFunc0 != null) return AsFunc0;
        var F = Linq(null);
        var paramS = linq.Expression.Parameter(typeof(S), "paramS");
        var paramT = linq.Expression.Parameter(typeof(T), "paramT");
        var paramU = linq.Expression.Parameter(typeof(U), "paramU");
        var paramV = linq.Expression.Parameter(typeof(V), "paramV");
        linq.Expression body;
        if (F == null) {
          var exprS = new Bling.Linq.LinqExpr<S>(paramS);
          var exprT = new Bling.Linq.LinqExpr<T>(paramT);
          var exprU = new Bling.Linq.LinqExpr<U>(paramU);
          var exprV = new Bling.Linq.LinqExpr<V>(paramV);
          var expand = Expand(exprS, exprT, exprU, exprV);
          if (expand == null) return null;
          // linq eval...
          AsFunc0 = (new Bling.Linq.DefaultLinqEval()).Eval<S, T, U, V, W>((exprS0, exprT0, exprU0, exprV0) => Expand(exprS0, exprT0, exprU0, exprV0));
        } else {
          body = F(paramS, paramT, paramU, paramV);
          var G = linq.Expression.Lambda<Microsoft.Func<S, T, U, V, W>>(body, paramS, paramT, paramU, paramV).Compile();
          AsFunc0 = (s, t, u, v) => G(s, t, u, v);
        }
        return AsFunc0;
      }
    }
  }
  public partial class MapOperator<S, T> : BaseOperatorX<S, T> {
    public override Func<linq.Expression, linq.Expression> Linq(Operation<S, T> op) {
      var f = linq.Expression.Constant(F, typeof(Func<S, T>));
      return s => linq.Expression.Invoke(f, s);
    }
  }
  public partial class MapOperator<S, T, U> : BaseOperatorX<S, T, U> {
    public override Func<linq.Expression, linq.Expression, linq.Expression> Linq(Operation<S, T, U> op) {
      var f = linq.Expression.Constant(STU, typeof(Func<S, T, U>));
      return (s, t) => linq.Expression.Invoke(f, s, t);
    }
  }
  public abstract partial class CastOperator<S, T, CNT> : Operator<S, T, CNT> {
    public override Func<linq.Expression, linq.Expression> Linq(Operation<S, T> op) {
      return e => linq.Expression.Convert(e, typeof(T));
    }
  }
  public partial class ToStringOperator : Operator<object, string, ToStringOperator> {
    private static readonly MethodInfo ToStringM = typeof(object).GetMethod("ToString");
    public override Func<linq.Expression, linq.Expression> Linq(Operation<object, string> op) {
      return e => linq.Expression.Call(e, ToStringM);
    }
  }
  public abstract partial class BaseStaticCallOperator<T, S, CNT> : Operator<T, S, CNT> {
    private bool IsLinqInit = false;
    private MethodInfo Method;
    public override Func<linq.Expression, linq.Expression> Linq(Operation<T, S> op) {
      if (!IsLinqInit) {
        Method = TargetType.GetMethod(CallName, new Type[] { typeof(T), });
        IsLinqInit = true;
      }
      if (Method == null) return base.Linq(op);
      else return e => linq.Expression.Call(Method, e);
    }
  }
  public abstract partial class StaticCallOperator<T, S, U, CNT> : Operator<T, S, U, CNT> {
    private bool IsLinqInit = false;
    private MethodInfo Method;

    public override Func<linq.Expression, linq.Expression, linq.Expression> Linq(Operation<T, S, U> op) {
      if (!IsLinqInit) {
        Method = TargetType.GetMethod(CallName, new Type[] { typeof(T), typeof(S) });
        IsLinqInit = true;
      }
      if (Method == null) return base.Linq(op);
      else return (e0, e1) => linq.Expression.Call(Method, e0, e1);
    }
  }
  public abstract partial class StaticCallOperator<T, S, U, V, CNT> : Operator<T, S, U, V, CNT> {
    private bool IsLinqInit = false;
    private MethodInfo Method;
    public override Func<linq.Expression, linq.Expression, linq.Expression, linq.Expression> Linq(Operation<T, S, U, V> op) {
      if (!IsLinqInit) {
        Method = TargetType.GetMethod(CallName, new Type[] { typeof(T), typeof(S), typeof(U) });
        IsLinqInit = true;
      }
      if (Method == null) return base.Linq(op);
      else return (e0, e1, e2) => linq.Expression.Call(TargetType, CallName, new Type[] { }, e0, e1, e2);
    }
  }
  public partial class ExplicitConvertOperator<S, T> : CastOperator<S, T, ExplicitConvertOperator<S, T>> {
    public override Func<linq.Expression, linq.Expression> Linq(Operation<S, T> op) {
      if (ConvertF == null) return base.Linq(op);
      else return e => linq.Expression.Convert(e, typeof(T), ConvertF);
    }
  }
  public static partial class ValueOperators<T> {
    public partial class Equal : Ops.BinaryOperator<T, bool, Equal> {
      public override Func<linq.Expression, linq.Expression, linq.Expression> Linq(Bling.Ops.Operation<T, T, bool> op) {
        return linq.Expression.Equal;
      }
    }
    public partial class NotEqual : Ops.BinaryOperator<T, bool, NotEqual> {
      public override Func<linq.Expression, linq.Expression, linq.Expression> Linq(Bling.Ops.Operation<T, T, bool> op) {
        return linq.Expression.NotEqual;
      }
    }
    public partial class BaseCondition<OP> : Operator<bool, T, T, T, OP> where OP : BaseCondition<OP> {
      public override Func<linq.Expression, linq.Expression, linq.Expression, linq.Expression> Linq(Operation<bool, T, T, T> op) {
        return linq.Expression.Condition;
      }
    }
  }
  public static partial class BoolOperators<T> {
    public partial class And : UniformBinaryOperator<T, bool, And> {
      public override Func<linq.Expression, linq.Expression, linq.Expression> Linq(Bling.Ops.Operation<T, T, T> op) {
        if (Arity<bool, T>.ArityI.Count > 1) return base.Linq(op);
        return linq.Expression.And;
      }
    }
    public partial class Or : UniformBinaryOperator<T, bool, Or> {
      public override Func<linq.Expression, linq.Expression, linq.Expression> Linq(Bling.Ops.Operation<T, T, T> op) {
        if (Arity<bool, T>.ArityI.Count > 1) return base.Linq(op);
        return linq.Expression.Or;
      }
    }
    public partial class ExclusiveOr : UniformBinaryOperator<T, bool, ExclusiveOr> {
      public override Func<linq.Expression, linq.Expression, linq.Expression> Linq(Bling.Ops.Operation<T, T, T> op) {
        if (Arity<bool, T>.ArityI.Count > 1) return base.Linq(op);
        return linq.Expression.ExclusiveOr;
      }
    }
    public partial class Not : UniformUnaryOperator<T, bool, Not> {
      public override Func<linq.Expression, linq.Expression> Linq(Bling.Ops.Operation<T, T> op) {
        if (Arity<bool, T>.ArityI.Count > 1) return base.Linq(op);
        return linq.Expression.Not;
      }
    }
  }
  public static partial class NumericOperators0<T, K> {
    public abstract partial class MathOperator<CNT> : Ops.StaticCallOperator<T, T, CNT> {
      public override Func<linq.Expression, linq.Expression> Linq(Bling.Ops.Operation<T, T> op) {
        if (Arity<K, T>.ArityI.Count != 1) return null;
        else return base.Linq(op);
      }
    }
    public partial class Clamp : Math3Operator<Clamp> {
      public override Func<linq.Expression, linq.Expression, linq.Expression, linq.Expression> Linq(Bling.Ops.Operation<T, T, T, T> op) {
        return null;
      }
    }
    public partial class Negate : UniformUnaryOperator<T, K, Negate> {
      public override Func<linq.Expression, linq.Expression> Linq(Bling.Ops.Operation<T, T> op) {
        if (Arity<K, T>.ArityI.Count > 1) return base.Linq(op);
        return linq.Expression.Negate;
      }
    }
    public partial class Add : ArithmeticOperator<Add> {
      public override Func<linq.Expression, linq.Expression, linq.Expression> Linq(Bling.Ops.Operation<T, T, T> op) {
        if (Arity<K, T>.ArityI.Count > 1) return base.Linq(op);
        return linq.Expression.Add;
      }
    }
    public partial class Subtract : ArithmeticOperator<Subtract> {
      public override Func<linq.Expression, linq.Expression, linq.Expression> Linq(Bling.Ops.Operation<T, T, T> op) {
        if (Arity<K, T>.ArityI.Count > 1) return base.Linq(op);
        return linq.Expression.Subtract;
      }
    }
    public partial class Multiply : ArithmeticOperator<Multiply> {
      public override Func<linq.Expression, linq.Expression, linq.Expression> Linq(Bling.Ops.Operation<T, T, T> op) {
        if (Arity<K, T>.ArityI.Count > 1) return base.Linq(op);
        return linq.Expression.Multiply;
      }
    }
    public partial class Divide : ArithmeticOperator<Divide> {
      public override Func<linq.Expression, linq.Expression, linq.Expression> Linq(Bling.Ops.Operation<T, T, T> op) {
        if (Arity<K, T>.ArityI.Count > 1) return base.Linq(op);
        return linq.Expression.Divide;
      }
    }
    public partial class Modulo : ArithmeticOperator<Modulo> {
      public override Func<linq.Expression, linq.Expression, linq.Expression> Linq(Bling.Ops.Operation<T, T, T> op) {
        if (Arity<K, T>.ArityI.Count > 1) return base.Linq(op);
        return linq.Expression.Modulo;
      }
    }
  }
  public static partial class NumericOperators<T, K, BT> {
    public partial class GreaterThan : ArithmeticOperator<GreaterThan> {
      public override Func<linq.Expression, linq.Expression, linq.Expression> Linq(Bling.Ops.Operation<T, T, BT> op) {
        if (Arity<K, T>.ArityI.Count > 1) return base.Linq(op);
        return linq.Expression.GreaterThan;
      }
    }
    public partial class LessThan : ArithmeticOperator<LessThan> {
      public override Func<linq.Expression, linq.Expression, linq.Expression> Linq(Bling.Ops.Operation<T, T, BT> op) {
        if (Arity<K, T>.ArityI.Count > 1) return base.Linq(op);
        return linq.Expression.LessThan;
      }
    }
    public partial class GreaterThanOrEqual : ArithmeticOperator<GreaterThanOrEqual> {
      public override Func<linq.Expression, linq.Expression, linq.Expression> Linq(Bling.Ops.Operation<T, T, BT> op) {
        if (Arity<K, T>.ArityI.Count > 1) return base.Linq(op);
        return linq.Expression.GreaterThanOrEqual;
      }
    }
    public partial class LessThanOrEqual : ArithmeticOperator<LessThanOrEqual> {
      public override Func<linq.Expression, linq.Expression, linq.Expression> Linq(Bling.Ops.Operation<T, T, BT> op) {
        if (Arity<K, T>.ArityI.Count > 1) return base.Linq(op);
        return linq.Expression.LessThanOrEqual;
      }
    }
  }
  public static partial class DoubleOperators<T, B> {
    public abstract partial class DoubleOperator<CNT> : StaticCallOperator<T, B, CNT> {
      public override Func<linq.Expression, linq.Expression> Linq(Bling.Ops.Operation<T, B> op) {
        if (Arity<double, T>.ArityI.Count > 1) return null;
        else return base.Linq(op);
      }
    }
  }
  public partial class Operation<S, T> : BaseOperation<T, IOperatorX<S, T>> {
    internal Microsoft.Func<S, T> AsFuncLinq {
      get {
        if (FCache.ContainsKey(Op)) return (Microsoft.Func<S, T>)FCache[Op];
        var F = Op.Linq(this);
        Microsoft.Func<S, T> G;
        if (F != null) {
          var paramS = linq.Expression.Parameter(typeof(S), "paramS");
          var body = F(paramS);
          G = linq.Expression.Lambda<Microsoft.Func<S, T>>(body, paramS).Compile();
        } else {
          var G0 = new Linq.DefaultLinqEval().Eval<S, T>((s) => Op.Expand(s));
          G = (s) => G0(s);

        }
        FCache[Op] = G;
        return G;
      }
    }
    
    
    public override Expr<T> Transform(Transformer t) {
      var ArgA = t.Transform(this.ArgA);
      if (ArgA is Constant<S>) {
        Microsoft.Func<S, T> G = AsFuncLinq;
        if (G != null)
          return new Constant<T>(G(((Constant<S>)ArgA).Value));
        Expr<T> e = Op.Expand(ArgA);
        if (e != null) return t.Transform(e);
      }
      return new Operation<S, T>() { ArgA = ArgA, Op = Op, };
    }
  }
  public partial class Operation<S, T, U> : BaseOperation<U, OperatorX<S, T, U>> {
    internal Microsoft.Func<S, T, U> AsFuncLinq {
      get {
        if (FCache.ContainsKey(Op)) return (Microsoft.Func<S, T, U>)FCache[Op];
        var F = Op.Linq(this);
        Microsoft.Func<S, T, U> G;
        if (F != null) {
          var paramS = linq.Expression.Parameter(typeof(S), "paramS");
          var paramT = linq.Expression.Parameter(typeof(T), "paramT");
          var body = F(paramS, paramT);
          G = linq.Expression.Lambda<Microsoft.Func<S, T, U>>(body, paramS, paramT).Compile();
        } else {
          var G0 = new Linq.DefaultLinqEval().Eval<S, T, U>((s, t) => Op.Expand(s, t));
          G = (s,t) => G0(s,t);
        }
        FCache[Op] = G;
        return G;
      }
    }
    public override Expr<U> Transform(Transformer t) {
      var ArgA = t.Transform(this.ArgA);
      var ArgB = t.Transform(this.ArgB);
      if (ArgA is Constant<S> && ArgB is Constant<T>) {
        Microsoft.Func<S, T, U> G = AsFuncLinq;
        if (G != null)
          return new Constant<U>(G(((Constant<S>)ArgA).Value, ((Constant<T>)ArgB).Value));
        var e = Op.Expand(ArgA, ArgB);
        if (e != null) return t.Transform(e);
      }
      return new Operation<S, T, U>() { ArgA = ArgA, ArgB = ArgB, Op = Op };
    }
  }
  public partial class Operation<S, T, U, V> : BaseOperation<V, OperatorX<S, T, U, V>> {
    internal Microsoft.Func<S, T, U, V> AsFuncLinq {
      get {
        if (FCache.ContainsKey(Op)) return (Microsoft.Func<S, T, U, V>)FCache[Op];
        var F = Op.Linq(this);
        if (F == null) return null;
        var paramS = linq.Expression.Parameter(typeof(S), "paramS");
        var paramT = linq.Expression.Parameter(typeof(T), "paramT");
        var paramU = linq.Expression.Parameter(typeof(U), "paramU");
        var body = F(paramS, paramT, paramU);
        var G = linq.Expression.Lambda<Microsoft.Func<S, T, U, V>>(body, paramS, paramT, paramU).Compile();
        FCache[Op] = G;
        return G;
      }
    }
    public override Expr<V> Transform(Transformer t) {
      var ArgA = t.Transform(this.ArgA);
      var ArgB = t.Transform(this.ArgB);
      var ArgC = t.Transform(this.ArgC);
      if (ArgA is Constant<S> && ArgB is Constant<T> && ArgC is Constant<U>) {
        Microsoft.Func<S, T, U, V> G = AsFuncLinq;
        if (G != null)
          return new Constant<V>(G(((Constant<S>)ArgA).Value, ((Constant<T>)ArgB).Value, ((Constant<U>)ArgC).Value));
        var e = Op.Expand(ArgA, ArgB, ArgC);
        if (e != null) return t.Transform(e);
      }
      return new Operation<S, T, U, V>() { ArgA = ArgA, ArgB = ArgB, ArgC = ArgC, Op = Op };
    }
  }
  public partial class Operation<S, T, U, V, W> : BaseOperation<W, OperatorX<S, T, U, V, W>> {
    internal Microsoft.Func<S, T, U, V, W> AsFuncLinq {
      get {
        if (FCache.ContainsKey(Op)) return (Microsoft.Func<S, T, U, V, W>)FCache[Op];
        var F = Op.Linq(this);
        if (F == null) return null;
        var paramS = linq.Expression.Parameter(typeof(S), "paramS");
        var paramT = linq.Expression.Parameter(typeof(T), "paramT");
        var paramU = linq.Expression.Parameter(typeof(U), "paramU");
        var paramV = linq.Expression.Parameter(typeof(V), "paramV");
        var body = F(paramS, paramT, paramU, paramV);
        var G = linq.Expression.Lambda<Microsoft.Func<S, T, U, V, W>>(body, paramS, paramT, paramU, paramV).Compile();
        FCache[Op] = G;
        return G;
      }
    }
    public override Expr<W> Transform(Transformer t) {
      var ArgA = t.Transform(this.ArgA);
      var ArgB = t.Transform(this.ArgB);
      var ArgC = t.Transform(this.ArgC);
      var ArgD = t.Transform(this.ArgD);
      if (ArgA is Constant<S> && ArgB is Constant<T> && ArgC is Constant<U> && ArgD is Constant<V>) {
        Microsoft.Func<S, T, U, V, W> G = AsFuncLinq;
        if (G != null)
          return new Constant<W>(G(((Constant<S>)ArgA).Value, ((Constant<T>)ArgB).Value, ((Constant<U>)ArgC).Value, ((Constant<V>)ArgD).Value));

        var e = Op.Expand(ArgA, ArgB, ArgC, ArgD);
        if (e != null) return t.Transform(e);
      }
      return new Operation<S, T, U, V, W>() { ArgA = ArgA, ArgB = ArgB, ArgC = ArgC, ArgD = ArgD, Op = Op };
    }
  }
}

namespace Bling.Linq {
  using Bling.Vecs;

  public static partial class LinqExtensions {
    public static Func<Expr<int>, ICanAssignExpr<T,Expression>> Raise<T>(this T[] Values) {
      return idx => new ArrayExpr<T>(Values, idx);
    }
  }
  public class ParameterContext {
    public readonly List<IParameterExpr> Slots = new List<IParameterExpr>();
    public readonly Dictionary<IParameterExpr, ParameterExpression> Expressions = new Dictionary<IParameterExpr, ParameterExpression>();
    public ParameterExpression this[IParameterExpr valueX] {
      get {
        if (!Expressions.ContainsKey(valueX)) {
          if (Slots.Contains(valueX)) {
            Expressions[valueX] = Expression.Parameter(valueX.TypeOfT, valueX.Name);
          } else false.Assert();
        }
        return Expressions[valueX];
      }
      set {
        Expressions[valueX] = value;
      }
    }
    public ParameterExpression[] Flush() {
      var ret = new ParameterExpression[Slots.Count];
      for (int i = 0; i < Slots.Count; i++) {
        ret[i] = this[Slots[i]];
      }
      return ret;
    }

    public ParameterExpr<T> NewParameter<T>() {
      var ret = new ParameterExpr<T>();
      Slots.Add(ret);
      return ret;
    }
    public void Add<T>(ParameterExpr<T> arg) { Slots.Add(arg); }
    public void Add(IParameterExpr arg) { Slots.Add(arg); }
  }


  public class LinqExpr<T> : Expr<T> {
    public readonly Expression Expression;
    public LinqExpr(Expression Expression) { this.Expression = Expression; }
    protected override string ToString1() {
      return Expression.ToString();
    }
    protected  override Eval<EVAL>.Info<T> Eval<EVAL>(Eval<EVAL> txt) {
      if (txt is LinqEval<EVAL>) {
        var ret = new LinqEval<EVAL>.MyInfo<T>() { Value = Expression };
        return (Eval<EVAL>.Info<T>)(object)ret;
      } else if (txt is TranslateEval<EVAL>) {
        return ((TranslateEval<EVAL>)txt).NoTranslation(this);
      }
      throw new NotImplementedException();
    }
  }


  public abstract partial class LinqEval<EVAL> : ScopedEval<EVAL>, ITableEval<EVAL>, IParameterEval<EVAL>, Ops.IConditionEval<EVAL>, IArrayEval<EVAL>, ILoopEval<EVAL, Expression>, IExpressionEval<EVAL,Expression>, IRandomEval<EVAL> where EVAL : Eval<EVAL> {
    public ParameterContext Parameters { get; set; }
    public LinqEval() {
      Parameters = new ParameterContext();
    }
    public IParameterExpr[] ExpressionsUsed { get { return Parameters.Expressions.Keys.ToArray(); } }


    public Info<double> NextDouble(Random r) {
      return new MyInfo<double>() { Value = Expression.Call(Expression.Constant(r, typeof(Random)), "NextDouble", new Type[] { }) };
    }
    public override Info<T> Parameter<T>(ParameterExpr<T> value) {
      var ret = base.Parameter(value);
      if (ret != null) return ret;
      return new MyInfo<T>() { Value = Parameters[value] };
    }



    public Info<T> Eval<T>(T[] Values, Info<int> Index) {
      return new MyInfo<T>() {
        Value = Expression.ArrayAccess(
          Expression.Constant(Values, typeof(T[])), ((MyInfo<int>)Index).Value),
      };
    }
    public TDelegate DoSetNow<T,TDelegate>(ICanAssignExpr<T,Expression> LHS, Expr<T> RHS, params IParameterExpr[] Args) {
      foreach (var arg in Args) Parameters.Add(arg);
      var RHS0 = DoEval(RHS).Value;
      Expression assign = LHS.Assign(RHS0, this);
      return (TDelegate)Expression.Lambda<TDelegate>(this.PopScope(assign), Parameters.Flush()).Compile();
    }
    public Expression DoSetNow00<T>(ICanAssignExpr<T, Expression> LHS, Expr<T> RHS, params IParameterExpr[] Args) {
      foreach (var arg in Args) Parameters.Add(arg);
      var RHS0 = DoEval(RHS).Value;
      Expression assign = LHS.Assign(RHS0, this);
      return (assign);
      //return Expression.Lambda<TDelegate>(this.PopScope(assign), Parameters.Flush());
    }

    public TDelegate DoSetNow<T, TDelegate>(Func<Expression,EVAL,Expression> Assign, Expr<T> RHS, params IParameterExpr[] Args) {
      foreach (var arg in Args) Parameters.Add(arg);
      var RHS0 = DoEval(RHS).Value;
      Expression assign = Assign(RHS0, this);
      return (TDelegate)Expression.Lambda<TDelegate>(this.PopScope(assign), Parameters.Flush()).Compile();
    }
    public override Func<Info<S>, Info<T>> Operator<S, T>(Bling.Ops.Operation<S, T> op) {
      var linq = op.Op.Linq(op);
      if (linq == null) {
        if (op.Op is Ops.IMemoOperator<T>) {
          var G = op.AsFuncLinq;
          if (G != null) {
            return s => new MyInfo<T>() { Value = Expression.Invoke(
              Expression.Constant(G), ((MyInfo<S>) s).Value), 
            };
          }
        }
        return base.Operator<S, T>(op);
      }
      return s => new MyInfo<T>() { Value = linq(((MyInfo<S>)s).Value) };
    }
    public override Func<Info<S>, Info<T>, Info<U>> Operator<S, T, U>(Bling.Ops.Operation<S, T, U> op) {
      var linq = op.Op.Linq(op);
      if (linq == null) {
        if (op.Op is Ops.IMemoOperator<U>) {
          var G = op.AsFuncLinq;
          if (G != null) {
            return (s,t) => new MyInfo<U>() {
              Value = Expression.Invoke(
                Expression.Constant(G), ((MyInfo<S>)s).Value, ((MyInfo<T>)t).Value),
            };
          }
        }
        return base.Operator<S, T, U>(op);
      }
      return (s, t) => new MyInfo<U>() { Value = linq(((MyInfo<S>)s).Value, ((MyInfo<T>)t).Value) };
    }

    public override Func<Info<S>, Info<T>, Info<U>, Info<V>> Operator<S, T, U, V>(Bling.Ops.Operation<S, T, U, V> op) {
      var linq = op.Op.Linq(op);
      if (linq == null) {
        if (op.Op is Ops.IMemoOperator<V>) {
          var G = op.AsFuncLinq;
          if (G != null) {
            return (s, t, u) => new MyInfo<V>() {
              Value = Expression.Invoke(
                Expression.Constant(G), ((MyInfo<S>)s).Value, ((MyInfo<T>)t).Value, ((MyInfo<U>)u).Value),
            };
          }
        }
        return base.Operator<S, T, U, V>(op);
      }
      return (s, t, u) => new MyInfo<V>() { Value = linq(((MyInfo<S>)s).Value, ((MyInfo<T>)t).Value, ((MyInfo<U>)u).Value) };
    }
    public override Func<Info<S>, Info<T>, Info<U>, Info<V>, Info<W>> Operator<S, T, U, V, W>(Bling.Ops.Operation<S, T, U, V, W> op) {
      var linq = op.Op.Linq(op);
      if (linq == null) {
        if (op.Op is Ops.IMemoOperator<W>) {
          var G = op.AsFuncLinq;
          if (G != null) {
            return (s, t, u, v) => new MyInfo<W>() {
              Value = Expression.Invoke(
                Expression.Constant(G), ((MyInfo<S>)s).Value, ((MyInfo<T>)t).Value, ((MyInfo<U>)u).Value, ((MyInfo<V>)v).Value),
            };
          }
        }
        return base.Operator<S, T, U, V, W>(op);
      }
      return (s, t, u, v) => new MyInfo<W>() { Value = linq(((MyInfo<S>)s).Value, ((MyInfo<T>)t).Value, ((MyInfo<U>)u).Value, ((MyInfo<V>)v).Value) };
    }
    public override Info<T> Constant<T>(Constant<T> constant) {
      Type t = typeof(T);
      return new MyInfo<T>() {
        Value = Expression.Constant(constant.Value, t),
      };
    }


    public override Info<K> Access<K, T, ARITY>(Info<T> info, int Idx) {
      var p = MultiArity<K, T, ARITY>.ArityI.PropertyFor(Idx);
      Expression e = p((((MyInfo<T>)info).Value));
      if (e.Type != typeof(K)) e = Expression.Convert(e, typeof(K));
      var ret = new MyInfo<K>() { Value = e };
      AtLeast(ret, info);
      return ret;
    }
    public override Info<T> Composite<K, T, ARITY>(Vecs.IComposite<K,T> Expr, params Info<K>[] info) {
      var param = Expression.Parameter(typeof(T), "composite");
      (info.Length == MultiArity<K, T, ARITY>.ArityI.Count).Assert();
      var body = new Expression[info.Length + 2];
      body[0] = Expression.Assign(param, Expression.New(typeof(T)));
      for (int i = 0; i < info.Length; i++) {
        var p = MultiArity<K, T, ARITY>.ArityI.PropertyFor(i);
        var ke = ((MyInfo<K>)info[i]).Value;
        var e = p(param);
        if (e.Type != typeof(K)) ke = Expression.Convert(ke, e.Type);
        body[i + 1] = Expression.Assign(e, ke);
      }
      body[info.Length + 1] = param;
      var retE = Expression.Block(new ParameterExpression[] { param }, body);
      var retF = new MyInfo<T>() { Value = retE };
      foreach (var i in info) AtLeast(retF, i);
      return retF;
    }
    public Info<T> Table<T>(TableExpr<T> value) {
      var idx = DoEval(value.Index).Value;
      var temp = Expression.Parameter(typeof(T), "q" + value.GetHashCode());
      var cases = new SwitchCase[value.Values.Length];
      for (int i = 0; i < value.Values.Length; i++) {
        PushScope();
        var e = DoEval(value.Values[i]).Value;
        cases[i] = 
          Expression.SwitchCase(PopScope(Expression.Assign(temp, e)), Expression.Constant(i, typeof(int)));
      }
      Expression defaultBody = Expression.Constant(default(T), typeof(T));
      var switchBody = Expression.Switch(idx, Expression.Assign(temp, defaultBody), cases);
      var block = Expression.Block(typeof(T), new ParameterExpression[] { temp }, switchBody, temp);
      return new MyInfo<T>() { Value = block };
    }
    public Info<T> Table<T>(TableExpr2<T> value) {
      var idx = DoEval(value.Index).Value;
      var block = Expression.ArrayAccess(Expression.Constant(value.Values, typeof(T[])), idx);
      return new MyInfo<T>() { Value = block };
    }
    public Info<T> Block<T>(T[] Values, Info<int> idx) {
      var idx0 = ((MyInfo<int>)idx).Value;
      if (idx0 is ConstantExpression) {
        var idx1 = (int)((ConstantExpression)idx0).Value;
        return new MyInfo<T>() { Value = Expression.Constant(Values[idx1], typeof(T)) };
      } else return new MyInfo<T>() {
        Value = Expression.ArrayAccess(Expression.Constant(Values, typeof(T).MakeArrayType()), idx0),
      };
    }
    private Expression PopScope(Expression e, Func<ParameterExpression[], IList<Expression>, Expression> F) {
      var temps = base.PopScope();
      if (temps.Count == 0) return e;
      var tempDefs = new ParameterExpression[temps.Count];
      var Instructions = new List<Expression>();
      for (int i = 0; i < temps.Count; i++) {
        tempDefs[i] = (ParameterExpression)((MyInfo)temps[i].AsTemp).Value;
        Instructions.Add(Expression.Assign(tempDefs[i], ((MyInfo)temps[i].Code).Value));
      }
      Instructions.Add(e);
      return F(tempDefs, Instructions);
    }
    public Expression PopScope(Expression e) {
      return PopScope(e, (tempDefs, Instructions) => Expression.Block(tempDefs, Instructions));
    }
    public Expression PopScope<T>(Expression e) {
      return PopScope(e, (tempDefs, Instructions) => Expression.Block(typeof(T), tempDefs, Instructions));
    }
    protected override Info<S> AllocateTemp<S>(Scope scope, Info<S> Code) {
      var name = "t" + scope.Depth + "_" + scope.Order.Count;
      return new MyInfo<S> { Value = Expression.Parameter(typeof(S), name) };
    }
    public Info<S> Condition<S>(Expr<bool> Test, Expr<S> IfTrue, Expr<S> IfFalse) {
      MyInfo<bool> TestE = DoEval(Test);
      MyInfo<S> IfTrueD, IfFalseD;
      {
        Expression IfTrueE, IfFalseE;
        {
          PushScope();
          IfTrueD = DoEval(IfTrue);
          IfTrueE = IfTrueD.Value;
          IfTrueE = PopScope<S>(IfTrueE);
        }
        {
          PushScope();
          IfFalseD = DoEval(IfFalse);
          IfFalseE = IfFalseD.Value;
          IfFalseE = PopScope<S>(IfFalseE);
        }
        var ret = new MyInfo<S>() {
          Value = Expression.Condition(TestE.Value, IfTrueE, IfFalseE),
        };
        this.AtLeast(ret, TestE);
        this.AtLeast(ret, IfTrueD);
        this.AtLeast(ret, IfFalseD);
        return ret;
      }
    }
    public interface MyInfo : Info {
      Expression Value { get; }
    }
    public new MyInfo DoEval(Expr Expr) { return (MyInfo)base.DoEval(Expr); }
    public new MyInfo<T> DoEval<T>(Expr<T> Expr) { return (MyInfo<T>)base.DoEval(Expr); }
    public class MyInfo<T> : Info<T>, MyInfo {
      public Expression Value { get; set; }
    }

    // convenience.
    protected Expression<TDelegate> Eval0<TDelegate>(Expression result) {
      var body = PopScope(result);
      var lambda = Expression.Lambda<TDelegate>(body, Parameters.Flush());
      return lambda;
    }
    protected TDelegate Eval0<T, TDelegate>(Expr<T> value) {
      var result = DoEval(value).Value;
      return Eval<TDelegate>((result));
    }

    public TDelegate Eval<TDelegate>(Expression result) {
      return Eval0<TDelegate>(result).Compile();
    }
    public Func<T> Eval<T>(Expr<T> value) {
      var f = Eval0<T, Microsoft.Func<T>>(value);
      return () => f();
    }
    public Func<S, T> Eval<S, T>(Func<Expr<S>, Expr<T>> value) {
      var slots = Parameters;
      var slot = slots.NewParameter<S>();
      var f = Eval0<T, Microsoft.Func<S, T>>(value(slot));
      return (e1) => f(e1);
    }
    public Expression Eval000<S, T>(Func<Expr<S>, Expr<T>> value) {
      var slots = Parameters;
      var slot = slots.NewParameter<S>();
      var result = DoEval(value(slot)).Value;
      return this.PopScope(result);
    }
    public Func<S, T, U> Eval<S, T, U>(Func<Expr<S>, Expr<T>, Expr<U>> value) {
      var slots = Parameters;
      var slot1 = slots.NewParameter<S>();
      var slot2 = slots.NewParameter<T>();
      var f = Eval0<U, Microsoft.Func<S, T, U>>(value(slot1, slot2));
      return (e1, e2) => f(e1, e2);
    }
    public Func<S, T, U, V> Eval<S, T, U, V>(Func<Expr<S>, Expr<T>, Expr<U>, Expr<V>> value) {
      var slots = Parameters;
      var slot1 = slots.NewParameter<S>();
      var slot2 = slots.NewParameter<T>();
      var slot3 = slots.NewParameter<U>();
      return Eval0<V, Func<S, T, U, V>>(value(slot1, slot2, slot3));
    }
    public Func<S, T, U, V, W> Eval<S, T, U, V, W>(Func<Expr<S>, Expr<T>, Expr<U>, Expr<V>, Expr<W>> value) {
      var slots = Parameters;
      var slot1 = slots.NewParameter<S>();
      var slot2 = slots.NewParameter<T>();
      var slot3 = slots.NewParameter<U>();
      var slot4 = slots.NewParameter<V>();
      return Eval0<W, Func<S, T, U, V, W>>(value(slot1, slot2, slot3, slot4));
    }

    public Expression MakeLoopAssign<T>(
      int count, string nm, 
      Func<Expr<int>, ICanAssignExpr<T,Expression>> LHS,
      Func<Expr<int>, Expr<T>> RHS) {
      return MakeMultiLoopAssign<T>(new int[] { count }, new string[] { nm },
        es => LHS(es[0]), es => RHS(es[0]));
    }
    public Expression MakeMultiLoopAssign<T>(
      int[] count, string[] nm,
      Func<Expr<int>[], ICanAssignExpr<T,Expression>> LHS,
      Func<Expr<int>[], Expr<T>> RHS) {
      var loopVarsA = new ParameterExpression[count.Length];
      var loopVars = new LinqExpr<int>[count.Length];

      for (int i = 0; i < loopVarsA.Length; i++) {
        loopVarsA[i] = Expression.Parameter(typeof(int), nm[i]);
        loopVars[i] = new LinqExpr<int>(loopVarsA[i]);
      }
      var RHS1 = RHS(loopVars);
      var LHS1 = LHS(loopVars);
      // do the last one first.
      var lastIndex = count.Length - 1;
      (lastIndex >= 0).Assert();
      Expression body;
      {
        PushScope();
        var RHS0 = DoEval(RHS1).Value;
        body = LHS1.Assign(RHS0, this);
        body = PopScope(body);
      }
      while (lastIndex >= 0) {
        // convert to loop.
        var updateVar = Expression.Assign(loopVarsA[lastIndex], Expression.Add(loopVarsA[lastIndex], Expression.Constant(1, typeof(int))));
        var breakLabel = Expression.Label("BREAK_" + nm[lastIndex]);
        var checkBreak = Expression.IfThen(Expression.Equal(loopVarsA[lastIndex], Expression.Constant(count[lastIndex], typeof(int))), 
          Expression.Break(breakLabel));
        var loop = Expression.Loop(Expression.Block(body, updateVar, checkBreak), breakLabel);
        body = Expression.Block(new ParameterExpression[] { loopVarsA[lastIndex] },
            Expression.Assign(loopVarsA[lastIndex], Expression.Constant(0, typeof(int))),
            loop);
        lastIndex -= 1;
      }
      return body;
    }
    public Expression End(Expression e) {
      (Top.Depth == 0).Assert();
      return PopScope(e);
    }

    public Expression DoIf(Expression Test, Func<EVAL,Expression> IfTrue, Func<EVAL,Expression> IfFalse) {
      Expression IfTrue0, IfFalse0;
      PushScope();
      IfTrue0 = PopScope(IfTrue(this));
      PushScope();
      IfFalse0 = PopScope(IfFalse(this));
      return Expression.IfThenElse(Test, IfTrue0, IfFalse0);

      //return (Expression.Condition(Test, IfTrue0, IfFalse0));
    }
    public Expression Exception {
      get {
        var e = Expression.New(typeof(NotSupportedException));
        return Expression.Throw(e);
      }
    }
    public Expression Block(IList<Expression> Expressions) {
      if (Expressions.Count == 0) return Expression.Empty();
      else if (Expressions.Count == 1) return Expressions[0];
      else return Expression.Block(Expressions);
    }

    public Expression DoLoop(LoopIndexExpr LoopIndex, Func<EVAL, Expression> Body) {
      var loopVar = Expression.Parameter(typeof(int), LoopIndex.Name);
      return DoLoop(LoopIndex, loopVar, Body);
    }
    public Expression DoLoop(LoopIndexExpr LoopIndex, ParameterExpression loopVar, Func<EVAL, Expression> Body) {
      var count = DoEval(LoopIndex.Count).Value;
      PushScope();
      PutParamInScope(LoopIndex, new MyInfo<int>() { Value = loopVar });
      var body = Body((EVAL)(object)this);
      body = PopScope(body);
      Expression loop;
      {
        var updateVar = Expression.Assign(loopVar, Expression.Add(loopVar, Expression.Constant(1)));
        var breakLabel = Expression.Label("BREAK_" + LoopIndex.Name);
        var checkBreak = Expression.IfThen(Expression.Equal(loopVar, count), Expression.Break(breakLabel));
        loop = Expression.Loop(Expression.Block(body, updateVar, checkBreak), breakLabel);
      }
      {
        var initVar = Expression.Assign(loopVar, Expression.Constant(0));
        loop = Expression.Block(new ParameterExpression[] { loopVar }, initVar, loop);
      }
      return loop;
    }
    public Expression MakeAssign<T>(Expr<T> LHS, Expr<T> RHS) {
      if (LHS is ICanAssignExpr<T, Expression>) {
        return ((ICanAssignExpr<T, Expression>)LHS).Assign(Extract(DoEval(RHS)), this);
      } else if (LHS is NoAssignExpr<T>) return Expression.Empty();
      else throw new NotSupportedException();
    }
    public Expression MakeAssign(Expr LHS, Expr RHS) {
      if (LHS is ICanAssignExpr<Expression>) {
        return ((ICanAssignExpr<Expression>)LHS).Assign(Extract(DoEval(RHS)), this);
      } else if (LHS is INoAssignExpr) return Expression.Empty();
      else throw new NotSupportedException();
    }

    public Expression Extract<T>(Info<T> Info) { return ((MyInfo<T>)Info).Value; }
    public Expression Extract(Info Info) { return ((MyInfo)Info).Value; }

    private class TryLoopEval : TranslateEval<TryLoopEval> {
      public readonly HashSet<LoopIndexExpr> Loops = new HashSet<LoopIndexExpr>();
      protected override Expr<T> RealTranslate<T>(Expr<T> value) {
        if (value is LoopIndexExpr) Loops.Add((LoopIndexExpr)(object)value);
        return base.RealTranslate<T>(value);
      }
    }
    private class TryLoopEval2 : TranslateEval<TryLoopEval2> {
      public LoopIndexExpr OldIndex;
      public Expr<int> NewIndex;
      protected override Expr<T> RealTranslate<T>(Expr<T> value) {
        if (value.Equals(OldIndex)) return (Expr<T>)(object)NewIndex;
        else return base.RealTranslate<T>(value);
      }
    }

    public Expression TryLoop(Expr<int> Index, Expr RHS, Func<Expression, Expression, Expression> F) {
      var findLoops = new TryLoopEval();
      findLoops.Translate(Index);
      if (findLoops.Loops.Count == 0) { // nothing to do, just evaluate directly

        PushScope();
        var index0 = DoEval(Index).Value;
        index0 = PopScope(index0);

        PushScope();
        var RHS0 = DoEval(RHS).Value;
        RHS0 = PopScope(RHS0);

        return F(index0, RHS0);
      } else if (findLoops.Loops.Count == 1) {
        var e = findLoops.Loops.GetEnumerator();
        e.MoveNext();
        var loop = e.Current;
        return For(loop.Name, loop.Count, i => {
          LinqExpr<int> NewIndex = new LinqExpr<int>(i.Value);
          var translate = new TryLoopEval2() { OldIndex = loop, NewIndex = NewIndex };
          var Index1 = translate.Translate(Index);
          var RHS1 = translate.Translate(RHS);
          
          PushScope();
          var index0 = DoEval(Index1).Value;
          index0 = PopScope(index0);

          PushScope();
          var RHS0 = DoEval(RHS1).Value;
          RHS0 = PopScope(RHS0);

          return F(index0, RHS0);
        });
      }
      throw new NotSupportedException();
    }
    public Expression For(string nm, Expr<int> count, Func<MyInfo<int>, Expression> Body) {
      var loopVar = Expression.Parameter(typeof(int), nm);
      
      PushScope();
      var body = Body(new MyInfo<int>() { Value = loopVar });
      body = PopScope(body);

      PushScope();
      var count0 = DoEval(count).Value;
      count0 = PopScope(count0);

      var updateVar = Expression.Assign(loopVar, Expression.Add(loopVar, Expression.Constant(1)));
      var breakLabel = Expression.Label("BREAK_" + nm);
      var checkBreak = Expression.IfThen(Expression.Equal(loopVar, count0), Expression.Break(breakLabel));
      var loop = Expression.Loop(Expression.Block(body, updateVar, checkBreak), breakLabel);
      return Expression.Block(new ParameterExpression[] { loopVar },
          Expression.Assign(loopVar, Expression.Constant(0)),
          loop);
    }
    public Expression For(LoopIndexExpr i, Func<ParameterExpression,Expression> Body) {
      var loopVar = Expression.Parameter(typeof(int), i.Name);

      Expression body;
      {
        (!this.Parameters.Slots.Contains(i)).Assert();
        this.Parameters[i] = loopVar;
        PushScope();
        body = Body(loopVar);
        body = PopScope(body);
        this.Parameters.Slots.Remove(i);
      }
      Expression count0;
      {
        PushScope();
        count0 = DoEval(i.Count).Value;
        count0 = PopScope(count0);
      }
      var updateVar = Expression.Assign(loopVar, Expression.Add(loopVar, Expression.Constant(1)));
      var breakLabel = Expression.Label("BREAK_" + i.Name);
      var checkBreak = Expression.IfThen(Expression.Equal(loopVar, count0), Expression.Break(breakLabel));
      var loop = Expression.Loop(Expression.Block(body, updateVar, checkBreak), breakLabel);
      return Expression.Block(new ParameterExpression[] { loopVar },
          Expression.Assign(loopVar, Expression.Constant(0)),
          loop);


    }

  }
  public partial class DefaultLinqEval : LinqEval<DefaultLinqEval> {}
}
namespace Bling.Arrays {
  using Bling.Core;
  using linq = Microsoft.Linq.Expressions;
  using Bling.Linq;
  public partial class Array2D<BRAND> : IArray<BRAND> where BRAND : Brand<BRAND> {
    public linq.Expression Loop<EVAL>(LinqEval<EVAL> eval, Func<BRAND, linq.Expression> F) where EVAL : Eval<EVAL> {
      return eval.For("i", ColumnCount, (i) => {
        LinqExpr<int> I = new LinqExpr<int>(i.Value);
        return eval.For("j", RowCount, (j) => {
          LinqExpr<int> J = new LinqExpr<int>(j.Value);
          var value = this[I, J];
          return F(value);
        });
      });
    }
  }
}
/*
namespace Bling.NewOps {
  public abstract partial class Operator<RET, ARG0, ARG1, ARG2> : IOperator<RET, ARG0, ARG1, ARG2> {

    private int InitAsFunc() {

      return 0;
    }

  }


}

namespace Bling.Linq {
  using Bling.NewOps;
  using Bling.NewVecs;
  public static class LinqOperators {
    public readonly static Dictionary<IOperator,object> Ops = new Dictionary<IOperator,object>();

    private static Func<Expression, Expression, Expression> Bin(Func<Expression, Expression, Expression> F) {
      return F;
    }
    private static Func<Expression, Expression> Unary(Func<Expression, Expression> F) {
      return F;
    }
    private static Func<Expression, Expression, Expression, Expression> Trinary(Func<Expression, Expression, Expression, Expression> F) {
      return F;
    }

    private static void NumericOps<T>() {
      Ops[NumericOps<T, D1>.Add] = Bin(Expression.Add);
      Ops[NumericOps<T, D1>.Subtract] = Bin(Expression.Subtract);
      Ops[NumericOps<T, D1>.Multiply] = Bin(Expression.Multiply);
      Ops[NumericOps<T, D1>.Divide] = Bin(Expression.Divide);
      Ops[NumericOps<T, D1>.Mod] = Bin(Expression.Modulo);

      Ops[NumericOps<T, D1>.Negate] = Unary(Expression.Negate);

      Ops[CompareOps<T, D1>.GreaterThan] = Bin(Expression.GreaterThan);
      Ops[CompareOps<T, D1>.LessThan] = Bin(Expression.LessThan);
      Ops[CompareOps<T, D1>.GreaterThanEqual] = Bin(Expression.GreaterThanOrEqual);
      Ops[CompareOps<T, D1>.LessThanEqual] = Bin(Expression.LessThanOrEqual);
    }
    private static void GeneralOps<T>() {
      Ops[EqualityOps<T, D1>.Equals] = Bin(Expression.Equal);
      Ops[EqualityOps<T, D1>.NotEquals] = Bin(Expression.NotEqual);
    }
    private static void BoolOps() {
      Ops[BoolOps<D1>.Not] = Unary(Expression.Not);
      Ops[BoolOps<D1>.And] = Bin(Expression.And);
      Ops[BoolOps<D1>.Or] = Bin(Expression.Or);
      Ops[BoolOps<D1>.ExclusiveOr] = Bin(Expression.ExclusiveOr);
    }
    static LinqOperators() {
      GeneralOps<int>();
      NumericOps<int>();
      //MathOps<int>();
      
      GeneralOps<double>();
      NumericOps<double>();
      //MathOps<double>();

      GeneralOps<bool>();
      BoolOps();
    }
    public static object FindOperator(IOperator Op) {
      if (Ops.ContainsKey(Op)) return Ops[Op];
      if (Op is IMathDefined) {
        var op = (IMathDefined)Op;
        var ArgTypes = op.ArgTypes;
        var args = new Type[ArgTypes.Length];
        for (int i = 0; args != null && i < ArgTypes.Length; i++) {
          Type t = ArgTypes[i];
          if (t.IsGenericType) t = t.GetGenericTypeDefinition();
          if (t == typeof(IVec<,>)) {
            var gargs = ArgTypes[i].GetGenericArguments();
            if (gargs[1] == typeof(D1)) {
              args[i] = gargs[0];
            } else args = null;
          } else args[i] = t;
        }
        if (args != null) {
          var mthd = typeof(Math).GetMethod(op.Id, args);
          if (mthd == null && op.Alt != null)
            mthd = typeof(Math).GetMethod(op.Alt, args);
          if (mthd == null)
            mthd = args[0].GetMethod(op.Id, args);
          else if (mthd == null && op.Alt != null)
            mthd = args[0].GetMethod(op.Alt, args);
          object ret;
          if (mthd != null) {
            if (ArgTypes.Length == 1)
              ret = Unary(e => Expression.Call(mthd, e));
            else if (ArgTypes.Length == 2)
              ret = Bin((e0, e1) => Expression.Call(mthd, e0, e1));
            else if (ArgTypes.Length == 3)
              ret = Trinary((e0, e1, e2) => Expression.Call(mthd, e0, e1, e2));
            else throw new NotImplementedException();
          } else ret = null;
          Ops[op] = ret;
          return ret;
        }
      }
      return null;
    }
  }

  public abstract partial class LinqEval<EVAL> : ScopedEval<EVAL>, ITableEval<EVAL>, IParameterEval<EVAL>, Ops.IConditionEval<EVAL>, IArrayEval<EVAL>, ILoopEval<EVAL, Expression>, IExpressionEval<EVAL, Expression>, IRandomEval<EVAL> where EVAL : Eval<EVAL> {
    public override Func<Eval<EVAL>, Operation<RET, ARG0>, Eval<EVAL>.Info<RET>> Lookup<RET, ARG0>(IOperator<RET, ARG0> Op) {
      var f = (Func<Expression, Expression>) LinqOperators.FindOperator(Op);
      if (f != null) {
        return (txt, op) => {
          var arg = ((LinqEval<EVAL>) txt).DoEval(op.Arg0).Value;
          return new LinqEval<EVAL>.MyInfo<RET>() {
            Value = f(arg),
          };
        };
      }
      return base.Lookup<RET, ARG0>(Op);
    }
    public override Func<Eval<EVAL>, Operation<RET, ARG0, ARG1>, Eval<EVAL>.Info<RET>> Lookup<RET, ARG0, ARG1>(IOperator<RET, ARG0, ARG1> Op) {
      var f = (Func<Expression, Expression, Expression>)LinqOperators.FindOperator(Op);
      if (f != null) {
        return (txt, op) => {
          var arg0 = ((LinqEval<EVAL>)txt).DoEval(op.Arg0).Value;
          var arg1 = ((LinqEval<EVAL>)txt).DoEval(op.Arg1).Value;
          return new LinqEval<EVAL>.MyInfo<RET>() {
            Value = f(arg0, arg1),
          };
        };
      }
      return base.Lookup<RET, ARG0, ARG1>(Op);
    }
    public override Func<Eval<EVAL>, Operation<RET, ARG0, ARG1, ARG2>, Eval<EVAL>.Info<RET>> Lookup<RET, ARG0, ARG1, ARG2>(IOperator<RET, ARG0, ARG1, ARG2> Op) {
      var f = (Func<Expression, Expression, Expression, Expression>)LinqOperators.FindOperator(Op);
      if (f != null) {
        return (txt, op) => {
          var arg0 = ((LinqEval<EVAL>)txt).DoEval(op.Arg0).Value;
          var arg1 = ((LinqEval<EVAL>)txt).DoEval(op.Arg1).Value;
          var arg2 = ((LinqEval<EVAL>)txt).DoEval(op.Arg2).Value;
          return new LinqEval<EVAL>.MyInfo<RET>() {
            Value = f(arg0, arg1, arg2),
          };
        };
      }
      return base.Lookup<RET, ARG0, ARG1, ARG2>(Op);
    }
  }
}

namespace Bling.Linq {
  using Bling.NewOps;
  using Bling.NewVecs;
  public abstract partial class LinqEval<EVAL> : ScopedEval<EVAL>, ITableEval<EVAL>, IParameterEval<EVAL>, Ops.IConditionEval<EVAL>, IArrayEval<EVAL>, ILoopEval<EVAL, Expression>, IExpressionEval<EVAL, Expression>, IRandomEval<EVAL> where EVAL : Eval<EVAL> {
    private static readonly Dictionary<Type, Type> TypeTransforms = new Dictionary<Type, Type>();
    public virtual Type Tn(Type T) {
      if (TypeTransforms.ContainsKey(T)) return TypeTransforms[T];
      if (T.IsGenericType) {
        if (T.GetGenericTypeDefinition() == typeof(IVec<,>)) {
          var TOfT = T.GetGenericArguments()[0];
          var DOfT = T.GetGenericArguments()[1];
          if (DOfT == typeof(D1)) {
            TypeTransforms[T] = TOfT;
            return TOfT;
          }
          Type S = (Type)typeof(Vecs<,>).MakeGenericType(DOfT).GetProperty("VecType").GetAccessors()[0].Invoke(null, new object[0]);
          S = S.MakeGenericType(TOfT);
          TypeTransforms[T] = S;
          return S;
        } else {
          var Ts = T.GetGenericArguments();
          for (int i = 0; i < Ts.Length; i++) Ts[i] = Tn(Ts[i]);
          var S = T.GetGenericTypeDefinition().MakeGenericType(Ts);
          TypeTransforms[T] = S;
          return S;
        }
      }
      return T;
    }


    public override Eval<EVAL>.Info<IVec<T, D>> Composite<T, D>(params Eval<EVAL>.Info<T>[] info) {
      (info.Length == Dim<D>.Count).Assert();

      var Es = new Expression[info.Length];
      for (int i = 0; i < info.Length; i++) Es[i] = ((MyInfo<T>)info[i]).Value;
      if (Dim<D>.Count == 1) return new MyInfo<IVec<T, D>>() { Value = Es[0], }; 
      else return new MyInfo<IVec<T, D>>() { Value = Vecs<T, D>.New(Es), };
    }
    public override Eval<EVAL>.Info<T> Access<T, D>(Eval<EVAL>.Info<IVec<T, D>> info, int Idx) {
      // don't bother doing access if only one dimension.
      if (Dim<D>.Count == 1) return new MyInfo<T>() { Value = ((MyInfo<IVec<T, D>>)info).Value };
      else return new MyInfo<T>() { Value = Vecs<T, D>.Access(((MyInfo<IVec<T, D>>)info).Value, Idx) };
    }

  }

}
*/