#define CacheToStringResults
//#define FastEquals

using System;
using System.Collections;
using System.Collections.Generic;
using linq = Microsoft.Linq.Expressions;
using System.Linq;
using Bling.Util;

namespace Bling.DSL {
  public partial interface IAssign {
    bool Accept<T>(Expr<T> LHS, Expr<T> RHS);
    IAssign Copy();
  }
  public partial interface ExprVisitor<R> {
    R Visit<T>(Expr<T> value);
  }
  public partial interface IExpr {
    Type TypeOfT { get; }
    object BaseEval<EVAL>(Eval<EVAL> txt) where EVAL : Eval<EVAL>;
    R Visit<R>(ExprVisitor<R> v);
    bool Solve(IAssign assign, IExpr value);
  }
  public partial interface IExpr<T> : IExpr { }
  public partial interface IHasContainer : IExpr {
    Expr BaseContainer { get; }
  }

  public partial interface IHasContainer<S, T> : IExpr<T>, IHasContainer {
    Expr<S> Container { get; }
  }
  public abstract partial class Expr : IExpr {
    public abstract Type TypeOfT { get; }
    public abstract object BaseEval<EVAL>(Eval<EVAL> txt) where EVAL : Eval<EVAL>;
    public abstract R Visit<R>(ExprVisitor<R> v);
    public abstract bool Solve(IAssign assign, IExpr value);
    public abstract int IdentityHashCode { get; }
  }
  public partial interface IBindableExpr : IExpr {
    bool BindWith(Expr value);
  }

  public partial interface IBindableExpr<T> : IExpr<T>, IBindableExpr {
    bool BindWith(Expr<T> value);
    bool TwoWayBindWith(Expr<T> value);
  }
  public partial interface ICanAssignNowExpr<T> : IExpr<T> {
    TDelegate SetNow<TDelegate>(Expr<T> value, params IParameterExpr[] param);
    bool SetNow(T value);
  }

  

  public partial class BindAssign : IAssign {
    [System.ThreadStaticAttribute]
    public static Helper Top = new Helper.Default();
    public static void Activate(Helper Helper, Action A) {
      (Helper.Previous == null).Assert();
      (Top != null).Assert();
      Helper.Previous = Top;
      Top = Helper;
      A();
      Top = Helper.Previous;
      Helper.Previous = null;
    }

    public bool Accept<T>(Expr<T> LHS, Expr<T> RHS) {
      return Top.AcceptBind(LHS, RHS);
    }
    public IAssign Copy() { return this; }
    public abstract class Helper {
      internal Helper Previous;
      public abstract bool AcceptBind<T>(Expr<T> LHS, Expr<T> RHS);

      public class Default : Helper {
        public override bool AcceptBind<T>(Expr<T> LHS, Expr<T> RHS) {
          if (LHS is IBindableExpr<T>)
            return ((IBindableExpr<T>)LHS).BindWith(RHS);
          return false;
        }
      }
    }
  }
  public partial class TwoWayAssign : IAssign {
    public bool Accept<T>(Expr<T> LHS, Expr<T> RHS) {
      if (LHS is IBindableExpr<T>)
        return ((IBindableExpr<T>)LHS).TwoWayBindWith(RHS);
      return false;
    }
    public IAssign Copy() { return this; }
  }

  /*
  public partial class ApplyNowAssign : Assign {
    public Assign Copy() { return this; }
    public bool Accept<T>(Expr<T> LHS, Expr<T> RHS) {
      if (LHS is AssignNowExpr<T>) {
        ((AssignNowExpr<T>)LHS).SetNow(RHS)();
        return true;
      }
      return false;
    }
  }
  */

  public partial class NowAssign<TDelegate> : IAssign {
    public readonly IParameterExpr[] Arguments;
    public NowAssign(params IParameterExpr[] Arguments) { this.Arguments = Arguments; }
    public readonly List<TDelegate> Results = new List<TDelegate>();
    public IAssign Copy() { return this; }
    public bool Accept<T>(Expr<T> LHS, Expr<T> RHS) {
      if (LHS is ICanAssignNowExpr<T>) {
        var now = ((ICanAssignNowExpr<T>)LHS).SetNow<TDelegate>(RHS, Arguments);
        if (now == null) return false;
        Results.Add(now);
        return true;
      } else if (LHS is NoAssignExpr<T>) return true;
      else return false;
    }
  }
  public partial interface IParameterExpr : IExpr {
    string Name { get; set; }
  }
  public partial class ParameterExpr<T> : Expr<T>, IParameterExpr {
    public string Name { get; set; }
    private readonly object Key;
    public ParameterExpr() {
      Key = new object();
      Name = "P" + (Key.GetHashCode() % 100);
    }
    public ParameterExpr(object Key) {
      this.Key = Key;
      Name = Key.ToString();
    }
    protected override string ToString1() { return Name; }
    protected override Eval<EVAL>.Info<T> Eval<EVAL>(Eval<EVAL> txt) {
      if (txt is IParameterEval<EVAL>) {
        var ret = ((IParameterEval<EVAL>)txt).Parameter(this);
        if (ret != null) return ret;
      }
      throw new NotImplementedException();
    }
    protected override int GetHashCode0() {
      return Key.GetHashCode();
    }
    protected override bool Equals0(Expr<T> obj) {
      if (obj is ParameterExpr<T>) return Key.Equals(((ParameterExpr<T>)obj).Key);
      return base.Equals0(obj);
    }
  }
  public partial interface IParameterEval<EVAL> where EVAL : Eval<EVAL> {
    Eval<EVAL>.Info<T> Parameter<T>(ParameterExpr<T> value);
  }
  public partial class LoopIndexExpr : ParameterExpr<int> {
    public readonly Expr<int> Count;
    public LoopIndexExpr(object key, Expr<int> Count) : base(key) { this.Count = Count; }
    public LoopIndexExpr(Expr<int> Count) : base() { this.Count = Count; }
  }
  // XXX: move to Vertices
  public enum DynamicMode {
    Static = 0, Dynamic = 1,
  }

  public interface IDynamicEval<EVAL> : IEval<EVAL> where EVAL : Eval<EVAL> {
    Eval<EVAL>.Info<T> Dynamic<T>(DynamicMode Mode);
    Eval<EVAL>.Info<T> Dynamic<T>(Expr Other);
  }
  public interface IExpressionEval<EVAL, EXPRESSION> : IEval<EVAL> where EVAL : Eval<EVAL> {
    EXPRESSION Extract<T>(Eval<EVAL>.Info<T> Info);
    IParameterExpr[] ExpressionsUsed { get; }
  }

  public interface ICanAssignExpr : IExpr { }
  public interface ICanAssignExpr<EXPRESSION> : ICanAssignExpr {
    EXPRESSION Assign<EVAL>(EXPRESSION RHS0, IExpressionEval<EVAL, EXPRESSION> txt) where EVAL : Eval<EVAL>;
  }
  public interface ICanAssignExprT<T> : ICanAssignExpr, IExpr<T> { }
  public interface ICanAssignExpr<T, EXPRESSION> : ICanAssignExprT<T>, ICanAssignExpr<EXPRESSION> { }
  public interface IUseInUI : IExpr {}
  public interface IUseInUI<T> : IUseInUI, IExpr<T> { }

  public interface INoAssignExpr : IExpr { }
  public class NoAssignExpr<T> : Expr<T>, INoAssignExpr {
    private readonly Expr<T> Underlying;
    public NoAssignExpr(Expr<T> Underlying) {
      this.Underlying = Underlying;
    }
    protected override int GetHashCode0() { return Underlying.GetHashCode(); }
    protected override bool Equals0(Expr<T> obj) {
      return obj is NoAssignExpr<T> && ((NoAssignExpr<T>)obj).Underlying.Equals(Underlying);
    }
    protected override string ToString1() { return Underlying.ToString(); }
    protected override Eval<EVAL>.Info<T> Eval<EVAL>(Eval<EVAL> txt) {
      if (txt is TranslateEval<EVAL>) return ((TranslateEval<EVAL>)txt).NoTranslation(new NoAssignExpr<T>(
        ((TranslateEval<EVAL>.MyInfo<T>)txt.DoEval(Underlying)).Value
        ));
      return txt.DoEval(Underlying);
    }
  }

  public partial interface ILoopEval<EVAL, EXPRESSION> : IExpressionEval<EVAL, EXPRESSION> where EVAL : Eval<EVAL> {
    EXPRESSION DoLoop(LoopIndexExpr i, Func<EVAL, EXPRESSION> a);
    EXPRESSION MakeAssign<T>(Expr<T> LHS, Expr<T> RHS);
    EXPRESSION MakeAssign(Expr LHS, Expr RHS);
    EXPRESSION DoIf(EXPRESSION Test, Func<EVAL, EXPRESSION> IfTrue, Func<EVAL, EXPRESSION> IfFalse);
    EXPRESSION Exception { get; }
    EXPRESSION Block(IList<EXPRESSION> Expressions);
    EXPRESSION End(EXPRESSION e);
  }

  public static partial class DSLExtensions {
    public static EXPRESSION DoLoopAssign<T, EVAL, EXPRESSION>(ILoopEval<EVAL, EXPRESSION> Eval, Expr<T> LHS, Expr<T> RHS, params LoopIndexExpr[] Indices)
      where EVAL : Eval<EVAL> {
      if (Indices.Length == 1) {
        return Eval.DoLoop(Indices[0], (txt) => ((ILoopEval<EVAL, EXPRESSION>)txt).MakeAssign<T>(LHS, RHS));
      } else {
        var NewIndices = new LoopIndexExpr[Indices.Length - 1];
        for (int i = 1; i < Indices.Length; i++) NewIndices[i - 1] = Indices[i];
        return Eval.DoLoop(Indices[0], (txt) =>
          DoLoopAssign((ILoopEval<EVAL, EXPRESSION>)txt, LHS, RHS, NewIndices));
      }

    }
  }
  public abstract partial class TranslateEval<EVAL> : Eval<EVAL>, IParameterEval<EVAL>, ITableEval<EVAL> where EVAL : Eval<EVAL> {
    public interface MyInfo : Info {
      Expr BaseValue { get; }
    }
    public Eval<EVAL>.Info<T> Table<T>(TableExpr<T> value) {
      var Idx0 = Translate(value.Index);
      var Values0 = new Expr<T>[value.Values.Length];
      for (int i = 0; i < Values0.Length; i++)
        Values0[i] = Translate(value.Values[i]);
      return NoTranslation(new TableExpr<T>(Values0, Idx0));
    }
    public Eval<EVAL>.Info<T> Table<T>(TableExpr2<T> value) {
      var Idx0 = Translate(value.Index);
      return NoTranslation(new TableExpr2<T>(value.Values, Idx0));
    }
    public Eval<EVAL>.Info<T> Block<T>(T[] Values, Eval<EVAL>.Info<int> idx) { throw new NotSupportedException(); }


    public class MyInfo<T> : Info<T>, MyInfo {
      public Expr<T> Value;
      public Expr BaseValue { get { return Value; } }
    }
    private MyInfo<T> DoEvalX<T>(Expr<T> Expr) { return (MyInfo<T>)((Eval<EVAL>)this).DoEval(Expr); }
    private MyInfo DoEvalX(Expr Expr) { return (MyInfo)((Eval<EVAL>)this).DoEval(Expr); }

    public override Info<T> DoEval<T>(Expr<T> Expr) {
      return base.DoEval(RealTranslate(Expr));
    }
    protected Expr<T> EvalWithoutTranslate<T>(Expr<T> Expr) {
      return ((MyInfo<T>)base.DoEval(Expr)).Value;
    }
    private class MyVisitor : ExprVisitor<Expr> {
      internal TranslateEval<EVAL> Outer;
      public Expr Visit<T>(Expr<T> e) {
        return Outer.EvalWithoutTranslate(e);
      }
    }

    protected Expr EvalWithoutTranslate(Expr Expr) {
      return Expr.Visit(new MyVisitor() { Outer = this });
    }

    public Expr<T> Translate<T>(Expr<T> value) { return DoEvalX(value).Value; }
    public BRAND Translate<T, BRAND>(Core.Brand<T, BRAND> value) where BRAND : Core.Brand<T, BRAND> {
      return Core.Brand<T, BRAND>.ToBrand(Translate(value.Underlying));
    }

    public Expr Translate(Expr value) { return DoEvalX(value).BaseValue; }
    protected virtual Expr<T> RealTranslate<T>(Expr<T> value) { return value; }
    public Info<T> NoTranslation<T>(Expr<T> value) {
      return new MyInfo<T>() { Value = value };
    }
    public override Info<T> Constant<T>(Constant<T> constant) { return NoTranslation(constant); }
    public Info<T> Parameter<T>(ParameterExpr<T> value) { return NoTranslation(value); }

  }
  public class ReplaceEval : TranslateEval<ReplaceEval> {
    public readonly Dictionary<IExpr, IExpr> Replace = new Dictionary<IExpr, IExpr>();
    public IExpr this[IExpr From] { set { Replace[From] = value; } }
    protected override Expr<T> RealTranslate<T>(Expr<T> value) {
      if (Replace.ContainsKey(value)) return (Expr<T>)Replace[value];
      else return base.RealTranslate<T>(value);
    }
  }

  public abstract partial class Expr<T> : Expr, IExpr<T> {
    // public T CurrentValue { get; }
    public override R Visit<R>(ExprVisitor<R> v) {
      return v.Visit<T>(this);
    }
    public override int IdentityHashCode { get { return base.GetHashCode(); } }

    public virtual Expr Identity { get { return this; } }
    public virtual bool HasCurrentValue { get { return true; } }
    public static implicit operator Expr<T>(T t) { return new Constant<T>(t); }

    public virtual bool IsConstant { get { return false; } }

#if CacheToStringResults
    private string ToString0;
#endif

    public sealed override string ToString() {
#if CacheToStringResults
      if (ToString0 != null) return ToString0;
#endif
      var ret = ToString1();
#if CacheToStringResults
      ToString0 = ret;
#endif
      return ret;
    }

    private int? hc;
    public sealed override int GetHashCode() {
      if (hc == null) hc = GetHashCode0();
      return (int)hc;
    }
    public sealed override bool Equals(object obj) {
      if (object.ReferenceEquals(this, obj)) return true;
      if (!(obj is Expr<T>)) return false;
      if (GetHashCode() != obj.GetHashCode()) return false;
#if FastEquals
      return ToString().Equals(obj.ToString());
#else
      return (Equals0((Expr<T>)obj));
#endif
    }
    protected virtual bool Equals0(Expr<T> e) {
      return base.Equals(e);
    }


    protected virtual int GetHashCode0() { return base.GetHashCode(); }
    protected virtual string ToString1() { return base.ToString(); }

    public override Type TypeOfT {
      get { return typeof(T); }
    }
    public virtual bool Solve(IAssign P, Expr<T> other) {
      var ret = P.Accept(this, other);
      return ret;
    }
    public override bool Solve(IAssign assign, IExpr value) {
      return Solve(assign, (Expr<T>)value);
    }
    protected abstract Eval<EVAL>.Info<T> Eval<EVAL>(Eval<EVAL> txt) where EVAL : Eval<EVAL>;
    internal Eval<EVAL>.Info<T> Eval000<EVAL>(Eval<EVAL> txt) where EVAL : Eval<EVAL> { return Eval(txt); }

    public override object BaseEval<EVAL>(Eval<EVAL> txt) {
      return Eval(txt);
    }
    public Expr<T> Bind {
      set {
        if (!Solve(new BindAssign(), value))
          throw new NotSupportedException();
      }
    }
    public Expr<T> TwoWay {
      set {
        if (!Solve(new TwoWayAssign(), value))
          throw new NotSupportedException();
      }
    }
  }
  public interface IProxyExpr : IExpr {
    Expr BaseUnderlying { get; }
  }

  public abstract partial class ProxyExpr<T> : Expr<T>, IProxyExpr {
    public readonly Expr<T> Underlying;
    public ProxyExpr(Expr<T> Underlying) {
      this.Underlying = Underlying;
    }
    protected override int GetHashCode0() {
      return Underlying.GetHashCode();
    }
    protected override bool Equals0(Expr<T> obj) {
      return obj is ProxyExpr<T> && Underlying.Equals(((ProxyExpr<T>)obj).Underlying);
    }
    protected override string ToString1() {
      return Underlying.ToString();
    }
    public Expr BaseUnderlying { get { return Underlying; } }
    protected override Eval<EVAL>.Info<T> Eval<EVAL>(Eval<EVAL> txt) {
      if (txt is TranslateEval<EVAL>) return
        ((TranslateEval<EVAL>)txt).NoTranslation(
          Make(((TranslateEval<EVAL>)txt).Translate(Underlying))
        );
      return txt.DoEval(Underlying);
    }
    public Expr<S> Make0<S>(Func<Expr<T>, Expr<S>> f) {
      return Make<S>(f(Underlying));
    }
    protected abstract Expr<S> Make<S>(Expr<S> u);
    public override bool Solve(IAssign P, Expr<T> other) {
      return base.Solve(P, other) || Underlying.Solve(P, other);
    }
    public override bool IsConstant {
      get { return false; }
    }
  }
  public partial interface IEval<EVAL> where EVAL : Eval<EVAL> {
    Eval<EVAL>.Info<T> DoEval<T>(Expr<T> Expr);
  }
  public abstract partial class Eval {
    public interface Info { }
    public interface Info<T> : Info { }
  }

  public abstract partial class Eval<EVAL> : Eval, IEval<EVAL> where EVAL : Eval<EVAL> {
    public new interface Info : Eval.Info { }
    public new interface Info<T> : Eval.Info<T>, Info { }
    public abstract Info<T> Constant<T>(Constant<T> constant);
    public static implicit operator EVAL(Eval<EVAL> eval) { return (EVAL)eval; }
    public virtual Info<T> DoEval<T>(Expr<T> Expr) { return Expr.Eval000(this); }

    private class EvalVisitor : ExprVisitor<Info> {
      public EVAL Outer;
      public Info Visit<T>(Expr<T> value) {
        return Outer.DoEval(value);
      }
    }
    public Info DoEval(Expr Expr) {
      return Expr.Visit(new EvalVisitor() { Outer = this });
    }
  }
  public interface IConstant : IExpr {
    object BaseValue { get; }
  }
  public interface IRandomEval<EVAL> : IEval<EVAL> where EVAL : Eval<EVAL> {
    Eval<EVAL>.Info<double> NextDouble(Random r);
  }

  public class RandomExpr : Expr<double> {
    protected readonly Random r;
    public RandomExpr(Random r) {
      this.r = r;
    }
    public RandomExpr() : this(new Random()) { }
    protected override string ToString1() {
      return "Rand()";
    }
    public override Expr<double> Extent(bool Max) {
      return new Constant<double>(Max ? 1d : 0d);
    }

    protected override Eval<EVAL>.Info<double> Eval<EVAL>(Eval<EVAL> txt) {
      if (txt is IRandomEval<EVAL>) return ((IRandomEval<EVAL>)txt).NextDouble(r);
      else if (txt is TranslateEval<EVAL>) return ((TranslateEval<EVAL>)txt).NoTranslation(this);
      else if (txt is IDynamicEval<EVAL>) return ((IDynamicEval<EVAL>)txt).Dynamic<double>(DynamicMode.Dynamic);
      throw new NotImplementedException();
    }
  }
  public class RandomExpr2 : RandomExpr {
    private readonly Expr Key;
    public RandomExpr2(Random r, Expr Key)
      : base(r) {
      this.Key = Key;
    }
    protected override string ToString1() {
      return "Rand()@" + Key;
    }
    protected override Eval<EVAL>.Info<double> Eval<EVAL>(Eval<EVAL> txt) {
      if (txt is IDynamicEval<EVAL>) return ((IDynamicEval<EVAL>)txt).Dynamic<double>(Key);
      else if (txt is TranslateEval<EVAL>) return ((TranslateEval<EVAL>)txt).NoTranslation(new RandomExpr2(r, ((TranslateEval<EVAL>)txt).Translate(Key)));
      return base.Eval(txt);
    }
  }
  public partial class Constant<T> : Expr<T>, IConstant {
    public readonly T Value;
    public Constant(T value) { this.Value = value; }
    public Constant() : this(default(T)) { }
    protected override string ToString1() {
      return Value.ToString();
    }
    protected override Eval<EVAL>.Info<T> Eval<EVAL>(Eval<EVAL> txt) {
      return txt.Constant(this);
    }
    protected override int GetHashCode0() {
      return Value == null ? 0 : Value.GetHashCode();
    }
    protected override bool Equals0(Expr<T> obj) {
      if (Value == null) return obj == null;
      else if (obj is Constant<T>) return Value.Equals(((Constant<T>)obj).Value);
      else return base.Equals0(obj);
    }
    public object BaseValue { get { return Value; } }
    public override bool IsConstant {
      get { return true; }
    }
  }
  partial class NowExpr<T> : Expr<T>, IBindableExpr<T> {
    public readonly Expr<T> Underlying;
    private new readonly T CurrentValue;
    public NowExpr(Expr<T> Underlying) {
      this.Underlying = Underlying;
      this.CurrentValue = Underlying.CurrentValue;
    }
    protected override string ToString1() {
      return CurrentValue.ToString();
    }
    protected override Eval<EVAL>.Info<T> Eval<EVAL>(Eval<EVAL> txt) {
      return txt.DoEval(new Constant<T>(CurrentValue));
    }
    public bool BindWith(Expr<T> value) {
      var assign = new NowAssign<Action>();
      if (Underlying.Solve(assign, value)) {
        foreach (var r in assign.Results) r();
        return true;
      }
      return false;
    }
    public bool TwoWayBindWith(Expr<T> value) {
      return false;
    }
    public bool BindWith(Expr value) { return BindWith((Expr<T>)value); }
  }
  // Allow for fast expression keying.
  public class ExprKey {
    // avoid evaluating this if possible!
    internal Func<string> F { set; private get; }
    private string Key0;
    internal int HashCode;
    internal Expr Original;
    public override int GetHashCode() {
      return HashCode;
    }
    private string Key() {
      if (Key0 == null) {
        Key0 = F();
        F = null;
      }
      return Key0;
    }
    public override bool Equals(object obj) {
      if (obj is ExprKey) {
        if (object.ReferenceEquals(Original, ((ExprKey)obj).Original)) return true;
        if (((ExprKey)obj).HashCode == HashCode) {
          return Key() == ((ExprKey)obj).Key();
        }
      }
      return base.Equals(obj);
    }
  }
  // fast expression keying.
  public class ExprKeys {
    private class MyCompar : EqualityComparer<Expr> {
      public override int GetHashCode(Expr obj) {
        return obj.IdentityHashCode;
      }
      public override bool Equals(Expr x, Expr y) {
        return object.ReferenceEquals(x, y);
      }
    }
    private readonly Dictionary<Expr, ExprKey> Keys = new Dictionary<Expr, ExprKey>(new MyCompar());
    public ExprKey this[Expr e] {
      get {
        if (Keys.ContainsKey(e)) return Keys[e];
        Func<string> str = () => e.ToString() + e.GetHashCode() + e.TypeOfT;
        var key = new ExprKey() {
          HashCode = e.GetHashCode(),
          F = str,
          Original = e,
        };
        Keys[e] = key;
        return key;
      }
    }
  }
}
namespace Bling.Semantics {
  using Bling.DSL;
  using Bling.Core;
  public abstract class Role {
    public abstract string Name { get; }
    public abstract Expr<T> Transform<T>(Expr<T> Expr);
    public abstract BRAND Transform<BRAND>(BRAND b) where BRAND : Brand<BRAND>;
  }

  public abstract class Role<ROLE> : Role where ROLE : Role<ROLE>, new() {
    public static ROLE Instance {
      get { return new ROLE(); }
    }
    public override string Name { get { return typeof(ROLE).Name.ToUpper(); } }
    public override int GetHashCode() {
      return typeof(ROLE).GetHashCode();
    }
    public override bool Equals(object obj) {
      return obj is Role<ROLE>;
    }
    public override Expr<T> Transform<T>(Expr<T> Expr) {
      return RoleExpr<T, ROLE>.Make00(Expr);
    }
    private class Visitor<BRAND> : ExprVisitor<BRAND> where BRAND : Brand<BRAND> {
      public BRAND Visit<T>(Expr<T> value) {
        return Brand<BRAND>.ToBrand(RoleExpr<T, ROLE>.Make00(value));
      }
    }
    public override BRAND Transform<BRAND>(BRAND b) {
      return b.Underlying.Visit(new Visitor<BRAND>());
    }
    private class At<BRAND> where BRAND : Brand<BRAND> {
      public readonly BRAND Under;
      public At(BRAND Under) { this.Under = Under; }
      public static implicit operator BRAND(At<BRAND> w) { return Instance.Transform(w.Under); }
      public static implicit operator At<BRAND>(BRAND w) { return new At<BRAND>(w); }
    }
  }
  public class NoRole : Role<NoRole> {
    public override Expr<T> Transform<T>(Expr<T> Expr) {
      return Expr;
    }
    public override BRAND Transform<BRAND>(BRAND b) { return (b); }
  }

  public interface IRoleEval<EVAL> : IEval<EVAL> where EVAL : Eval<EVAL> {
    Eval<EVAL>.Info<T> Eval<T, ROLE>(RoleExpr<T, ROLE> Expr) where ROLE : Role<ROLE>, new();
  }
  public class RoleExpr<T, ROLE> : ProxyExpr<T> where ROLE : Role<ROLE>, new() {
    private RoleExpr(Expr<T> Underlying) : base(Underlying) { }

    internal static RoleExpr<T, ROLE> Make00(Expr<T> Underlying) { return new RoleExpr<T, ROLE>(Underlying); }

    protected override int GetHashCode0() {
      return base.GetHashCode0() + typeof(ROLE).GetHashCode();
    }
    protected override bool Equals0(Expr<T> obj) {
      return base.Equals0(obj) && obj is RoleExpr<T, ROLE>;
    }
    protected override string ToString1() {
      return base.ToString1() + "[" + typeof(ROLE).Name + "]";
    }
    protected override Eval<EVAL>.Info<T> Eval<EVAL>(Eval<EVAL> txt) {
      if (txt is IRoleEval<EVAL>) return ((IRoleEval<EVAL>)txt).Eval(this);
      else return txt.DoEval(Underlying);
    }
    protected override Expr<S> Make<S>(Expr<S> e) { return new RoleExpr<S, ROLE>(e); }
  }

}
