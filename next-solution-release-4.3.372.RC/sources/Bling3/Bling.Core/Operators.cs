using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Bling.Util;
using Bling.DSL;

namespace Bling.DSL {
  using Bling.Ops;
  using Vecs;
  using System.Linq;
  public abstract partial class Expr : IExpr {
    public Expr Fold { get { return BaseFold; } }
    protected abstract Expr BaseFold { get; }
  }

  public abstract partial class Expr<T> : Expr, IExpr<T> {
    public new Expr<T> Fold { get { return new DefaultTransformer().Transform(this); } }
    protected override Expr BaseFold { get { return Fold; } }
    public virtual Expr<T> Transform(Transformer t) { return this; }

    public Expr<S> Map<S>(Func<T, S> F, Func<S, T> G) {
      return new MapOperator<T, S>(F) { G = G }.Make(this);
    }
    public Expr<S> Map<S>(Func<T, S> F) {
      return (new Ops.MapOperator<T, S>(F)).Make(this);
    }
    private string DumpT = null;
    public class DumpS {
      internal Expr<T> Outer;
      public string Look {
        get {
          if (Outer.DumpT != null) return Outer.DumpT;
          var eval = new ShareEval();
          eval.Go(Outer);
          eval.End();
          var output = new System.IO.StringWriter();
          eval.PrintShared(output);
          output.WriteLine(eval.Translate(Outer));
          Outer.DumpT = output.ToString();
          return Outer.DumpT;
        }
      }
    }
    public DumpS Dump { get { return new DumpS() { Outer = this }; } } 
  }
  public interface Transformer {
    Expr<T> Transform<T>(Expr<T> e);
  }
  public class DefaultTransformer : Transformer {
    public virtual Expr<T> Transform<T>(Expr<T> e) { return e.Transform(this); }
  }
  public abstract partial class Eval<EVAL> where EVAL : Eval<EVAL> {
    public virtual Info<S> Memo<S>(IExpr op) { return null; }
    public virtual Info<S> Memo<S>(IExpr op, Func<Info<S>> info, params Info[] args) { return info(); }

    public virtual Func<Info<S>, Info<T>> Operator<S, T>(Ops.Operation<S, T> op) { return null; }
    public virtual Func<Info<S>, Info<T>, Info<U>> Operator<S, T, U>(Ops.Operation<S, T, U> op) { return null; }
    public virtual Func<Info<S>, Info<T>, Info<U>, Info<V>> Operator<S, T, U, V>(Ops.Operation<S, T, U, V> op) { return null; }
    public virtual Func<Info<S>, Info<T>, Info<U>, Info<V>, Info<W>> Operator<S, T, U, V, W>(Ops.Operation<S, T, U, V, W> op) { return null; }


    public abstract Info<K> Access<K, T, ARITY>(Info<T> info, int Idx) where ARITY : MultiArity<K, T, ARITY>, new();
    public abstract Info<T> Composite<K, T, ARITY>(Vecs.IComposite<K,T> Expr, params Info<K>[] info) where ARITY : MultiArity<K, T, ARITY>, new();


  }
  public abstract partial class TranslateEval<EVAL> : Eval<EVAL> where EVAL : Eval<EVAL> {
    // pass everything through
    public override Func<Info<S>, Info<T>> Operator<S, T>(Ops.Operation<S, T> op) {
      return s => new MyInfo<T>() { Value = op.Op.Make(((MyInfo<S>)s).Value) };
    }
    public override Func<Info<S>, Info<T>, Info<U>> Operator<S, T, U>(Ops.Operation<S, T, U> op) {
      return (s, t) => new MyInfo<U>() { Value = op.Op.Make(((MyInfo<S>)s).Value, ((MyInfo<T>)t).Value) };
    }
    public override Func<Info<S>, Info<T>, Info<U>, Info<V>> Operator<S, T, U, V>(Ops.Operation<S, T, U, V> op) {
      return (s, t, u) => new MyInfo<V>() { Value = op.Op.Make(((MyInfo<S>)s).Value, ((MyInfo<T>)t).Value, ((MyInfo<U>)u).Value) };
    }
    public override Func<Info<S>, Info<T>, Info<U>, Info<V>, Info<W>> Operator<S, T, U, V, W>(Ops.Operation<S, T, U, V, W> op) {
      return (s, t, u, v) => new MyInfo<W>() { Value = op.Op.Make(((MyInfo<S>)s).Value, ((MyInfo<T>)t).Value, ((MyInfo<U>)u).Value, ((MyInfo<V>)u).Value) };
    }
    public override Eval<EVAL>.Info<K> Access<K, T, ARITY>(Eval<EVAL>.Info<T> info, int Idx) {
      return new MyInfo<K>() {
        Value = Vecs.Arity<K, T, ARITY>.ArityI.Access(((MyInfo<T>)info).Value, Idx)
      };
    }
    public override Eval<EVAL>.Info<T> Composite<K, T, ARITY>(IComposite<K,T> Expr, params Eval<EVAL>.Info<K>[] info) {
      var Exprs = new Expr<K>[info.Length];
      for (int i = 0; i < info.Length; i++) Exprs[i] = ((MyInfo<K>) info[i]).Value;
      return new MyInfo<T>() {
        Value = Vecs.Arity<K, T, ARITY>.ArityI.Make(Exprs),
      };
    }
  }



  public interface IShareEval<EVAL> : IEval<EVAL> where EVAL : Eval<EVAL> {
    Eval<EVAL>.Info<T> Share<T>(ShareEval.ShareExpr<T> Shared);
  }

  public class ShareEval : TranslateEval<ShareEval> {
    private readonly DSL.ExprKeys Keys = new DSL.ExprKeys();
    private readonly Dictionary<ExprKey, IShareExpr> Shares = new Dictionary<ExprKey, IShareExpr>();
    private List<ExprKey> Order = new List<ExprKey>();
    public interface IShareExpr : IExpr, IProxyExpr {
      int Id { get; set;  }
      bool Shared { get; }
      Expr BaseUse { get;  set; }
      HashSet<IShareExpr> Depends { get; }
    }
    private HashSet<IShareExpr> Depends;
    public class ShareExpr<T> : ProxyExpr<T>, IShareExpr {
      public HashSet<IShareExpr> Depends { get;  private set; }
      public Expr<T> Use { get; private set; }
      public int Id { get; set; }
      public bool Shared { get; internal set; }
      public ShareExpr(int Id, Expr<T> Underlying) : base(Underlying) { 
        this.Id = Id; Use = Underlying;
        this.Depends = new HashSet<IShareExpr>();
      }

      protected override Expr<S> Make<S>(Expr<S> e) {
        throw new NotSupportedException();
        //return e; // cast off sharing.
        //if (e.Equals(Use)) return (ProxyExpr<S>) (object) this;
        //throw new NotSupportedException();
        //return new JoinExpr<T>(e);
      }
      public Expr BaseUse { get { return Use; }  set { Use = (Expr<T>)value; } }
      protected override string ToString1() {
        if (!Shared) return Use.ToString();
        else return "T" + Id;
      }
      protected override Eval<EVAL>.Info<T> Eval<EVAL>(Eval<EVAL> txt) {
        if (txt is ShareEval) {
          var Ds = ((ShareEval)(object)txt).Depends;
          if (Ds != null) Ds.Add(this);
          return (Eval<EVAL>.Info<T>)((ShareEval)(object)txt).NoTranslation(this);
        } else if (txt is TranslateBack)
          return (Eval<EVAL>.Info<T>)((TranslateBack)(object)txt).NoTranslation(this);
        else if (txt is IShareEval<EVAL>) {
          return ((IShareEval<EVAL>)txt).Share(this);
        } else if (txt is TranslateEval<EVAL>) {
          return txt.DoEval(Use);
        }
        return base.Eval<EVAL>(txt);
      }
      protected override int GetHashCode0() {
        return IdentityHashCode;
      }
      protected override bool Equals0(Expr<T> obj) {
        return object.ReferenceEquals(this, obj);
      }
    }
    private readonly Queue<ExprKey> Unprocessed = new Queue<ExprKey>();
    protected override Expr<T> RealTranslate<T>(Expr<T> value) {
      if (value is Operation<T> || value is ICompositeT<T>) {
        var key = Keys[value];
        if (!AtEnd) {
          if (!Shares.ContainsKey(key)) {
            (Order.Count == Shares.Count).Assert();
            Unprocessed.Enqueue(key);
            Shares[key] = new ShareExpr<T>(Shares.Count, value);
            Order.Add(key);
          } else {
            ((ShareExpr<T>)Shares[key]).Shared = true;
          }
          return (ShareExpr<T>)Shares[key];
        } else if (Shares.ContainsKey(key)) {
          var ret = Shares[key];
          (ret.Shared).Assert();
          return (ShareExpr<T>) ret;
        }
      }
      value = base.RealTranslate<T>(value);
      return value;
    }
    public void Go<T>(Expr<T> Base) {
      var First = Translate(Base);
      while (Unprocessed.Count != 0) {
        var key = Unprocessed.Dequeue();
        var s = Shares[key];
        var e = s.BaseUnderlying;
        Depends = s.Depends;
        e = EvalWithoutTranslate(e);
        Depends = null;
        s.BaseUse = e;
      }
    }
    private bool AtEnd = false;
    public void End() {
      AtEnd = true;
      // eliminate unshared dependencies
      var TranslateBack = new TranslateBack();


      // sort based on dependencies. 
      {
        var NewOrder = new List<IShareExpr>();
        var RemoveKeys = new List<ExprKey>();
        {
          var Processed = new HashSet<IShareExpr>();
          Action<IShareExpr> Process = null;
          var ToRemove = new Queue<IShareExpr>();
          Process = (s) => {
            if (Processed.Contains(s)) return;
            foreach (var e in s.Depends) Process(e);
            Processed.Add(s);
            if (s.Shared) {
              s.BaseUse = TranslateBack.Translate(s.BaseUse);
              NewOrder.Add(s);
            } else {
              RemoveKeys.Add(Order[s.Id]);
            }

            ToRemove.Clear();
            foreach (var d in s.Depends) if (!d.Shared) ToRemove.Enqueue(d);
            foreach (var d in ToRemove) s.Depends.Remove(d);
            while (ToRemove.Count != 0) {
              var d = ToRemove.Dequeue();
              foreach (var dd in d.Depends)
                if (dd.Shared) s.Depends.Add(dd);
                else false.Assert();
            }

          };
          foreach (var e in Shares) Process(e.Value);
          foreach (var e in RemoveKeys) Shares.Remove(e);
        }


        var OldOrder = this.Order;
        this.Order = new List<ExprKey>();
        for (int i = 0; i < NewOrder.Count; i++) {
          (Order.Count == i).Assert();
          Order.Add(OldOrder[NewOrder[i].Id]);
          NewOrder[i].Id = i;
        }
      }
    }
    private class TranslateBack : TranslateEval<TranslateBack> {
      protected override Expr<T> RealTranslate<T>(Expr<T> value) {
        if ((value is ShareExpr<T>) && !((ShareExpr<T>)value).Shared) {
          return Translate(((ShareExpr<T>)value).Use);
        }
        return base.RealTranslate<T>(value);
      }
    }

    public List<IShareExpr> Shared {
      get {
        var ret = new List<IShareExpr>();
        for (int i = 0; i < Order.Count; i++) {
          var key = Order[i];
          var s = Shares[key];
          s.Shared.Assert();
          ret.Add(s);
        }
        return ret;
      }

    }

    public void PrintShared(System.IO.TextWriter output) {
      for (int i = 0; i < Order.Count; i++) {
        var key = Order[i];
        var s = Shares[key];
        if (!s.Shared) continue;
        output.WriteLine(s.TypeOfT.Name + " T" + s.Id + " = " + s.BaseUse + ";");
      }


    }
  }
  public abstract partial class ScopedEval<EVAL> : Eval<EVAL>, IParameterEval<EVAL> where EVAL : Eval<EVAL> {
    public ScopedEval() {
      Top = new Scope() { Previous = null, Depth = 0 };
      ScopeFor = (args) => Top;
      ParamInScopeF = (x, y) => { };
      AtLeast = (x, y) => { };
    }
    public partial interface LikeScope {
      int Depth { get; }
      Info<S> Publish<S>(ScopedEval<EVAL> self, IExpr op, Info<S> info, params Info[] args);
    }
    public partial class Scope : LikeScope {
      internal Scope Previous;
      public int Depth { get; internal set; }
      internal readonly Dictionary<IExpr, Info> Temps = new Dictionary<IExpr, Info>();
      internal readonly List<IExpr> Order = new List<IExpr>();
      public Info<S> Publish<S>(ScopedEval<EVAL> self, IExpr op, Info<S> Code, params Info[] args) {
        return self.PublishToScope(this, op, Code, args);
      }
      public Scope Find(int Depth) {
        (Depth <= this.Depth).Assert();
        if (Depth == this.Depth) return this;
        return Previous.Find(Depth);
      }
      internal readonly Dictionary<IParameterExpr, Info> Params = new Dictionary<IParameterExpr, Eval<EVAL>.Info>();
    }
    public virtual Eval<EVAL>.Info<T> Parameter<T>(ParameterExpr<T> value) {
      var scope = Top;
      while (scope != null) {
        if (scope.Params.ContainsKey(value)) return (Info<T>)scope.Params[value];
        scope = scope.Previous;
      }
      return null; // not found
    }
    public Action<Scope, Info> ParamInScopeF { private get; set; }
    public Action<Info, Info> AtLeast { get; set; }

    public void PutParamInScope<T>(ParameterExpr<T> Param, Info<T> Info) {
      ParamInScopeF(Top, Info);
      Top.Params[Param] = Info;
    }

    protected abstract Info<S> AllocateTemp<S>(Scope scope, Info<S> Code);
    private readonly Dictionary<Info, Info> CodeMap = new Dictionary<Info, Info>();
    protected string TempName { get { return "t" + Top.Depth + "_" + Top.Order.Count; } }

    protected virtual Info<S> PublishToScope<S>(Scope scope, IExpr op, Info<S> Code, params Info[] args) {
      var AsTemp = AllocateTemp(scope, Code); // create a new info to hold the temp, the current info is the code for the temp.
      if (AsTemp == null) 
        return Code; // don't allocate temp!
      ParamInScopeF(scope, AsTemp);
      CodeMap[AsTemp] = Code;
      (!scope.Temps.ContainsKey(op)).Assert();
      scope.Temps[op] = AsTemp;
      scope.Order.Add(op);
      (scope.Order.Count == scope.Temps.Count).Assert();
      return AsTemp;
    }
    /*
    public override Eval<EVAL>.Info<K> Access<K, T, ARITY>(Eval<EVAL>.Info<T> info, int Idx) {
      var ret = base.Access<K, T, ARITY>(info, Idx);
      AtLeast(ret, info);
      return ret;
    }
    public override Eval<EVAL>.Info<T> Composite<K, T, ARITY>(Composite<K,T> Expr, params Eval<EVAL>.Info<K>[] info) {
      var ret = base.Composite<K, T, ARITY>(Expr, info);
      foreach (var i in info) AtLeast(ret, i);
      return ret;
    }*/

    public Scope Top { get; private set; }
    public virtual void PushScope() {
      Top = new Scope() { Previous = Top, Depth = Top.Depth + 1 };
    }
    public bool IsEmpty { get { return Top == null; } }
    protected Scope TopScope { get { return Top; } }
    public struct Temp {
      //public object Op;
      public Info AsTemp;
      public Info Code;
    }
    public virtual IList<Temp> PopScope() {
      var array = new Temp[Top.Order.Count];
      (Top.Order.Count == Top.Temps.Count).Assert();
      for (int i = 0; i < Top.Order.Count; i++) {
        array[i] = new Temp() {
          //Op = Top.Order[i],
          AsTemp = Top.Temps[Top.Order[i]],
          Code = CodeMap[Top.Temps[Top.Order[i]]],
        };
      }
      Top = Top.Previous;
      return array;
    }
    public override Info<S> Memo<S>(IExpr op) {
      // look up.
      for (var t = Top; t != null; t = t.Previous)
        if (t.Temps.ContainsKey(op)) return (Info<S>)t.Temps[op];
      return null;
    }
    public Func<Info[], Scope> ScopeFor { set; private get; }

    //protected virtual Scope ScopeFor<S>(Operation<S> op, Info<S> Temp, params Eval<EVAL>.Info[] args) { return Top; }
    public override Info<S> Memo<S>(IExpr op, Func<Info<S>> Code, params Eval<EVAL>.Info[] args) {
      var Code0 = base.Memo(op, Code, args);
      var scope = ScopeFor(args);
      (Memo<S>(op) == null).Assert();
      (!(scope.Temps.ContainsKey(op))).Assert();
      return scope.Publish(this, op, Code0, args);
    }
    // push a scope for true, eval ifTrue in that scope, 
  }
  public interface ITableEval<EVAL> where EVAL : Eval<EVAL> {
    Eval<EVAL>.Info<T> Table<T>(TableExpr<T> value);
    Eval<EVAL>.Info<T> Table<T>(TableExpr2<T> value);
    Eval<EVAL>.Info<T> Block<T>(T[] Values, Eval<EVAL>.Info<int> idx);
  }
  public partial class TableExpr<T> : Expr<T> {
    public readonly Expr<int> Index;
    public readonly Expr<T>[] Values;
    public TableExpr(Expr<T>[] Values, Expr<int> Index) {
      this.Values = Values; this.Index = Index;
    }
    public bool BodyConstant {
      get {
        var result = true;
        for (int i = 0; i < Values.Length && result; i++)
          result = result && Values[i].IsConstant;
        return result;
      }
    }

    public override bool IsConstant {
      get {
        return Index.IsConstant && BodyConstant;
      }
    }
    protected override Eval<EVAL>.Info<T> Eval<EVAL>(Eval<EVAL> txt) {
      if (txt is ITableEval<EVAL>)
        return ((ITableEval<EVAL>)txt).Table<T>(this);
      Expr<T> result = new Constant<T>();
      for (int i = 0; i < Values.Length; i++) {
        result = ValueOperators<T>.Condition.Instance.Make(
          ValueOperators<int>.Equal.Instance.Make(Index, new Constant<int>(i)), Values[i], result);
      }
      return txt.DoEval(result);
    }
    public override Expr<T> Transform(Transformer t) {
      var Index = t.Transform(this.Index);
      var Values = new Expr<T>[this.Values.Length];
      for (int i = 0; i < Values.Length; i++) {
        Values[i] = t.Transform(this.Values[i]);
      }
      if (Index is Constant<int>) {
        var idx = (((Constant<int>)Index)).Value;
        if (Values[idx] is Constant<T>) return ((Constant<T>)Values[idx]).Value;
        else return Values[idx];
      } else return new TableExpr<T>(Values, Index);
    }
    protected override string ToString1() {
      return Values.ToList().ToString() + "[" + Index + "]";
    }
    protected override int GetHashCode0() {
      var hc = Index.GetHashCode();
      foreach (var e in Values) hc += e.GetHashCode();
      return hc;
    }
    protected override bool Equals0(Expr<T> obj) {
      if (obj is TableExpr<T>) {
        var v = (TableExpr<T>)obj;
        if (!Index.Equals(v.Index)) return false;
        if (Values.Length != v.Values.Length) return false;
        for (int i = 0; i < Values.Length; i++) if (!Values[i].Equals(v.Values[i])) return false;
        return true;
      } else return base.Equals0(obj);
    }
  }
  public partial class TableExpr2<T> : Expr<T> {
    public readonly Expr<int> Index;
    public readonly T[] Values;
    public TableExpr2(T[] Values, Expr<int> Index) {
      this.Values = Values; this.Index = Index;
    }
    protected override Eval<EVAL>.Info<T> Eval<EVAL>(Eval<EVAL> txt) {
      if (txt is ITableEval<EVAL>)
        return ((ITableEval<EVAL>)txt).Table<T>(this);
      Expr<T> result = new Constant<T>();
      for (int i = 0; i < Values.Length; i++) {
        result = ValueOperators<T>.Condition.Instance.Make(
          ValueOperators<int>.Equal.Instance.Make(Index, new Constant<int>(i)), Values[i], result);
      }
      return txt.DoEval(result);
    }
    public override Expr<T> Transform(Transformer t) {
      var Index = t.Transform(this.Index);
      if (Index is Constant<int>) {
        var idx = (((Constant<int>)Index)).Value;
        if (Values[idx] is Constant<T>) return ((Constant<T>)Values[idx]).Value;
        else return Values[idx];
      } else return new TableExpr2<T>(Values, Index);
    }
    protected override string ToString1() {
      return Values.ToList().ToString() + "[" + Index + "]";
    }
    protected override int GetHashCode0() {
      var hc = Index.GetHashCode() + Values.GetHashCode();
      return hc;
    }
    protected override bool Equals0(Expr<T> obj) {
      if (obj is TableExpr2<T>) {
        var v = (TableExpr2<T>)obj;
        if (!Index.Equals(v.Index)) return false;
        if (!object.ReferenceEquals(Values, v.Values)) return false;
        return true;
      } else return base.Equals0(obj);
    }
  }

}
namespace Bling.Vecs {
  using Bling.Ops;
  using Bling.Core;

  public static partial class VecExtensions {
    public static PointBl Point<T>(this Arity<double, T> arity, Expr<T> E, int i) {
      return new PointBl(arity.Access(E, i * 2 + 0), arity.Access(E, i * 2 + 1));
    }
    public static Point3DBl Point3D<T>(this Arity<double, T> arity, Expr<T> E, int i) {
      return new Point3DBl(arity.Access(E, i * 3 + 0), arity.Access(E, i * 3 + 1), arity.Access(E, i * 3 + 2));
    }

  }
  public interface IArity {
    int Count { get; }
    Type TypeOfT { get; }
    Type TypeOfK { get; }
    Expr Access0(Expr Target, int Idx);
    object Access(object Value, int Idx);
  }
  public interface IArity<K, T> : IArity { }

  public abstract partial class Arity : IArity {
    private static readonly Dictionary<Type, Arity> All = new Dictionary<Type,Arity>();
    public abstract int Count { get; }
    public abstract Type TypeOfT { get; }
    public abstract Type TypeOfK { get; }
    public abstract object Access(object Value, int Idx);
    public abstract Expr Access0(Expr Target, int Idx);
    public Arity() {
      (!All.ContainsKey(TypeOfT)).Assert();
      All[TypeOfT] = this;
    }
    public static Arity Find(Type TypeOfT) {
      if (All.ContainsKey(TypeOfT)) return All[TypeOfT];
      return null;
    }
  }
  public abstract partial class Arity<T> : Arity {
    public static Arity<T> ArityI { get; private set; }
    public Arity() {
      (Arity<T>.ArityI == null).Assert();
      Arity<T>.ArityI = this;
    }
    public override Type TypeOfT {
      get { return typeof(T); }
    }
  }


  public abstract partial class Arity<K, T> : Arity<T>, IArity<K,T> {
    public new static Arity<K, T> ArityI {
      get {
        return ArityI0;
      }
    }
    private static Arity<K, T> ArityI0;
    public abstract Expr<T> Make(params Expr<K>[] Params);
    internal virtual Expr<T> SwizzleExpand(params Expr<K>[] Params) { return Make(Params); }
    public abstract Expr<K> Access(Expr<T> Target, int Idx);
    public override Expr Access0(Expr Target, int Idx) {
      return Access((Expr<T>) Target, Idx);
    }
    public abstract K Access(T Value, int Idx);
    public override object Access(object Value, int Idx) {
      return Access((T) Value, Idx);
    }
    public abstract T Make(params K[] Params);
    public override Type TypeOfK {
      get { return typeof(K); }
    }
    public Arity() {
      Arity<K,T>.ArityI0 = this;
    }
    public Expr<T> Repeat(K k) {
      return Repeat(new Constant<K>(k));
    }
    public Expr<T> Repeat(Expr<K> k) {
      if (Count == 1) return (Expr<T>)(object)k;
      var Ids = new int[Count];
      for (int i = 0; i < Ids.Length; i++) Ids[i] = 0;
      return (new Swizzle<K, K, T>(Ids).Make(k));
    }
  }
  /// <typeparam name="K">unit type.</typeparam>
  /// <typeparam name="T">vector type.</typeparam>
  /// <typeparam name="ARITY">self type.</typeparam>
  public abstract partial class Arity<K,T,ARITY> : Arity<K,T> where ARITY : Arity<K,T,ARITY>, new()  {
    public new static readonly ARITY ArityI = new ARITY();
  }
  public interface ICompositeT<T> : IExpr<T> { }
  public interface IComposite<K> : IExpr {
    Expr<K> this[int Idx] { get; }
  }
  public interface IComposite<K, T> : IComposite<K>, ICompositeT<T> { }

  public partial class Arity1<T> : Arity<T,T,Arity1<T>> {
    public override int Count { get { return 1; } }
    // nothing to do.
    public override Expr<T> Make(params Expr<T>[] Params) { return Params[0]; }
    public override Expr<T> Access(Expr<T> Target, int Idx) { return Target; }
    public override T Make(params T[] Params) { return Params[0];  }
    public override T Access(T Value, int Idx) { return Value; }
    public static void CheckInit() { Extensions.Trace("CheckInit: " + typeof(Arity1<T>) + " " + ArityI); }
  }
  public interface HasXY { }
  public partial interface IMultiArity : IArity { }
  public abstract partial class MultiArity<K,T,ARITY> : Arity<K,T,ARITY>, IMultiArity where ARITY : MultiArity<K,T,ARITY>, new() { 
    // basically, Vec<T> = S.
    public override Expr<T> Make(params Expr<K>[] Params) {
      var AllConstant = true;
      foreach (var p in Params) AllConstant = AllConstant && p is Constant<K>;
      if (AllConstant) {
        var Ks = new K[ArityI.Count];
        for (int i = 0; i < ArityI.Count; i++)
          Ks[i] = ((Constant<K>)Params[i]).Value;
        return new Constant<T>(Make(Ks));
      }
      var AllEqual = true;
      for (int i = 1; i < ArityI.Count && AllEqual; i++)
        if (!Params[i].Equals(Params[0])) AllEqual = false;
      if (AllEqual) {
        return Repeat(Params[0]);
      }
      if (Params[0] is AccessExpr) {
        var AllSame = true;
        var iS = new int[ArityI.Count];
        iS[0] = ((AccessExpr)Params[0]).Idx;
        for (int i = 1; i < ArityI.Count && AllSame; i++)
          if (Params[i] is AccessExpr && 
            ((AccessExpr)Params[i]).Vector.Equals(((AccessExpr)Params[0]).Vector)) 
            iS[i] = ((AccessExpr)Params[i]).Idx;
          else AllSame = false;
        if (AllSame) {
          for (int i = 0; true; i++) {
            if (i == Params.Length)
              return ((AccessExpr)Params[0]).Vector;
            else if (i != iS[i]) break;
          }
          // swizzle
          return new Swizzle<K, T, T>(iS).Make(((AccessExpr)Params[0]).Vector);
        }
      }

      return Composite.Make(Params);
    }
    internal override Expr<T> SwizzleExpand(params Expr<K>[] Params) {
      return Composite.Make(Params);
    }

    static MultiArity() {
      Extensions.CheckInit<ARITY>();
    }
    public override Expr<K> Access(Expr<T> Value, int Idx) {
      if (Value is VecExpr) return ((VecExpr)Value)[Idx];
      return AccessExpr.Make(Value, Idx);
    }
    public partial class AccessExpr : Expr<K> {
      public readonly Expr<T> Vector;
      public readonly int Idx;

      public override bool IsConstant {
        get {
          return Vector.IsConstant;
        }
      }

      private AccessExpr(Expr<T> Vector, int Idx) {
        (!(Vector is VecExpr)).Assert();
        this.Vector = Vector; this.Idx = Idx;
      }
      public static Expr<K> Make(Expr<T> Vector, int Idx) {
        if (Vector is Constant<T>) return new Constant<K>(ArityI.Access(((Constant<T>)Vector).Value, Idx));
        else if (Vector is VecExpr) return ((VecExpr)Vector)[Idx];
        else return new AccessExpr(Vector, Idx);
      }
      public override Expr<K> Transform(Transformer t) {
        var Vector = t.Transform(this.Vector);
        if (Vector is Constant<T>) {
          return new Constant<K>(ArityI.Access(((Constant<T>)Vector).Value, Idx));
        }
        return new AccessExpr(Vector, Idx);
      }
      protected override int GetHashCode0() {
        return Vector.GetHashCode() + Idx;
      }
      protected override bool Equals0(Expr<K> obj) {
        if (obj is AccessExpr)
          return Vector.Equals(((AccessExpr)obj).Vector) && Idx.Equals(((AccessExpr)obj).Idx);
        return base.Equals0(obj);
      }
      protected override string ToString1() {
        return Vector.ToString() + "[" + Idx + "]";
      }
      protected  override Eval<EVAL>.Info<K> Eval<EVAL>(Eval<EVAL> txt) {
        var info = txt.Memo<K>(this);
        if (info != null) return info;
        var Vector = this.Vector;
        if (false && Vector is Operation<T>) {
          var expanded = ((Operation<T>)Vector).Expanded;
          if (expanded is VecExpr) 
            Vector = expanded;
        }
        if (Vector is VecExpr) {
          return txt.DoEval(((VecExpr)Vector)[Idx]);
        } else {
          return txt.Access<K, T, ARITY>(txt.DoEval(Vector), Idx);
        }
      }
    }
    public abstract partial class VecExpr : Expr<T>, IComposite<K, T> {
      public abstract Expr<K> this[int idx] { get; }
      public override bool IsConstant {
        get {
          var result = true;
          for (int i = 0; i < ArityI.Count && result; i++)
            result = result && this[i].IsConstant;
          return result;
        }
      }
      protected override string ToString1() {
        var str = "(";
        for (int i = 0; i < ArityI.Count; i++)
          str = str + (i == 0 ? "" : ", ") + this[i].ToString();
        return str + ")";
      }
    }
    public partial class Composite : VecExpr {
      private readonly Expr<K>[] Components;
      private Composite(params Expr<K>[] Components) { this.Components = Components; }

      internal static Expr<T> Make(params Expr<K>[] Components) {
        bool AllConstant = true;
        foreach (var k in Components)
          AllConstant = AllConstant && false && k.IsConstant;
        if (!AllConstant) return new Composite(Components);

        var ks = new K[Components.Length];
        for (int i = 0; i < Components.Length; i++)
          ks[i] = (Components[i]).CurrentValue;
        return new Constant<T>(ArityI.Make(ks));
      }

      public override Expr<T> Transform(Transformer t) {
          bool AllConstant = true;
          var Components = new Expr<K>[this.Components.Length];
          for (int i = 0; i < Components.Length; i++) {
            Components[i] = t.Transform(this.Components[i]);
            AllConstant = AllConstant && Components[i].IsConstant;
          }
          if (AllConstant) {
            var Cs = new K[Components.Length];
            for (int i = 0; i < Cs.Length; i++)
              Cs[i] = ((Constant<K>) Components[i]).Value;
            return new Constant<T>(ArityI.Make(Cs));
          }
          return new Composite(Components);
      }
      public override bool Solve(IAssign P, Expr<T> other) {
        var b = true;
        for (int i = 0; i < ArityI.Count; i++)
          b = b && Components[i].Solve(P, ArityI.Access(other, i));
        return b;
      }

      public override Expr<K> this[int idx] { get { return Components[idx]; } }
      protected override int GetHashCode0() {
        var hc = 0;
        for (int i = 0; i < ArityI.Count; i++) hc += this[i].GetHashCode();
        return hc;
      }
      protected override bool Equals0(Expr<T> obj) {
        if (obj is Composite) {
          for (int i = 0; i < ArityI.Count; i++)
            if (!this[i].Equals(((Composite)obj)[i])) return false;
          return true;
        }
        return base.Equals0(obj);
      }
      protected override Eval<EVAL>.Info<T> Eval<EVAL>(Eval<EVAL> txt) {
        {
          var memo = txt.Memo<T>(this);
          if (memo != null) 
            return memo;
        }
        Eval<EVAL>.Info<K>[] Components = new Eval<EVAL>.Info<K>[ArityI.Count];
        for (int i = 0; i < ArityI.Count; i++)
          Components[i] = txt.DoEval(this.Components[i]);
        return txt.Memo<T>(this, () => {
          return txt.Composite<K, T, ARITY>(this, Components);
        }, Components);
      }
    }
  }
  public abstract partial class Arity2<K, T, ARITY> : MultiArity<K, T, ARITY>, HasXY where ARITY : Arity2<K, T, ARITY>, new() {
    public override int Count { get { return 2; } }
  }
  public interface HasXYZ : HasXY { }
  public abstract partial class Arity3<K, T, ARITY> : MultiArity<K, T, ARITY>, HasXYZ where ARITY : Arity3<K, T, ARITY>, new() {
    public override int Count { get { return 3; } }
  }
  public interface HasXYZW : HasXYZ { }
  public abstract partial class Arity4<K, T, ARITY> : MultiArity<K, T, ARITY>, HasXYZW where ARITY : Arity4<K, T, ARITY>, new() {
    public override int Count { get { return 4; } }
  }
  public interface Vec {
    Type TypeOfK { get; }
    int Arity { get; }
  }
  public interface Vec<T> : Vec {

  }
  public static class VecX {
    static internal readonly Dictionary<Type, Vec> Samples = new Dictionary<Type, Vec>();
    public static Vec Sample<T>() { return Samples[typeof(T)]; }
  }

  public struct Vec2<T> : Vec<T> {
    public T X { get; set; }
    public T Y { get; set; }
    public override string ToString() {
      return "(" + X + ", " + Y + ")";
    }
    public Type TypeOfK { get { return typeof(T); } }
    public int Arity { get { return 2; } }
    static Vec2() { VecX.Samples[typeof(Vec2<T>)] = new Vec2<T>(); }

    public static bool operator ==(Vec2<T> v0, Vec2<T> v1) {
      return v0.X.Equals(v1.X) && v0.Y.Equals(v1.Y);
    }
    public static bool operator !=(Vec2<T> v0, Vec2<T> v1) {
      return !(v0 == v1);
    }
  }
  public partial class Vec2Arity<T> : Arity2<T, Vec2<T>, Vec2Arity<T>> {
    public override T Access(Vec2<T> Value, int Idx) {
      return (Idx == 0) ? Value.X : Value.Y;
    }
    public override Vec2<T> Make(params T[] Params) {
      return new Vec2<T>() { X = Params[0], Y = Params[1] };
    }
    public static void CheckInit() { Extensions.Trace("CheckInit: " + typeof(Vec2Arity<T>) + " " + ArityI); }
  }
  public struct Vec3<T> : Vec<T> { 
    public T X { get; set; }
    public T Y { get; set; }
    public T Z { get; set; }
    public override string ToString() {
      return "(" + X + ", " + Y + ", " + Z + ")";
    }
    public Type TypeOfK { get { return typeof(T); } }
    public int Arity { get { return 3; } }
    static Vec3() { VecX.Samples[typeof(Vec3<T>)] = new Vec3<T>(); }
  }
  public partial class Vec3Arity<T> : Arity3<T, Vec3<T>, Vec3Arity<T>> {
    public override T Access(Vec3<T> Value, int Idx) {
      return (Idx == 0) ? Value.X : (Idx == 1) ? Value.Y : (Idx == 2) ? Value.Z : Value.Z;
    }
    public override Vec3<T> Make(params T[] Params) {
      return new Vec3<T>() { X = Params[0], Y = Params[1], Z = Params[2], };
    }
    public static void CheckInit() { Extensions.Trace("CheckInit: " + typeof(Vec3Arity<T>) + " " + ArityI); }
  }
  public struct Vec4<T> : Vec<T> {
    public T X { get; set; }
    public T Y { get; set; }
    public T Z { get; set; }
    public T W { get; set; }
    public override string ToString() {
      return "(" + X + ", " + Y + ", " + Z + ", " + W + ")";
    }
    public Type TypeOfK { get { return typeof(T); } }
    public int Arity { get { return 4; } }
    static Vec4() { VecX.Samples[typeof(Vec4<T>)] = new Vec4<T>(); }
  }
  public partial class Vec4Arity<T> : Arity4<T, Vec4<T>, Vec4Arity<T>> {
    public override T Access(Vec4<T> Value, int Idx) {
      return (Idx == 0) ? Value.X : (Idx == 1) ? Value.Y : (Idx == 2) ? Value.Z : Value.W;
    }
    public override Vec4<T> Make(params T[] Params) {
      return new Vec4<T>() { X = Params[0], Y = Params[1], Z = Params[2], W = Params[3] };
    }
    public static void CheckInit() { Extensions.Trace("CheckInit: " + typeof(Vec4Arity<T>) + " " + ArityI); }
  }

}

namespace Bling.Ops {
  using Vecs;
  using Bling.Core;

  public partial interface IOperator { }
  public partial interface IOperator<S> : IOperator { }

  public partial interface IMemoOperator<S> : IOperator<S> { }

  public partial interface IBaseOperator<S,TDelegate> : IOperator<S> { }
  public partial interface IOperator<S, CNT> : IOperator<S> where CNT : IOperator<S, CNT> { }

  public partial interface IOperatorX<S, T> : IBaseOperator<T,Func<S,T>> {
    Expr<T> Expand(Expr<S> argA);
    Expr<S> Inverse(Expr<S> argA, Expr<T> result);
    string Format(string s);
    Expr<T> Make(Expr<S> ArgA);
    Eval<EVAL>.Info<T> Eval<EVAL>(Operation<S, T> op, Eval<EVAL> txt) where EVAL : Eval<EVAL>;
  }
  public partial interface IOperator<S, T, CNT> : IOperatorX<S,T>, IOperator<T, CNT> where CNT : IOperator<S,T,CNT> { }


  public partial interface OperatorX<S, T, U> : IBaseOperator<U, Func<S,T,U>> {
    Expr<U> Expand(Expr<S> argA, Expr<T> argB);
    Expr<S> InverseA(Expr<S> argA, Expr<T> argB, Expr<U> result);
    Expr<T> InverseB(Expr<S> argA, Expr<T> argB, Expr<U> result);
    string Format(string argA, string argB);
    Eval<EVAL>.Info<U> Eval<EVAL>(Operation<S,T,U> op, Eval<EVAL> txt) where EVAL : Eval<EVAL>;
    Expr<U> Make(Expr<S> ArgA, Expr<T> ArgB);
  }
  public partial interface IOperator<S, T, U, CNT> : OperatorX<S, T, U>, IOperator<U, CNT> where CNT : IOperator<S,T,U,CNT>  { }
  public partial interface OperatorX<S, T, U, V> : IBaseOperator<V, Func<S,T,U,V>> {
    Expr<V> Expand(Expr<S> argA, Expr<T> argB, Expr<U> argC);
    Expr<S> InverseA(Expr<S> argA, Expr<T> argB, Expr<U> argC, Expr<V> result);
    Expr<T> InverseB(Expr<S> argA, Expr<T> argB, Expr<U> argC, Expr<V> result);
    Expr<U> InverseC(Expr<S> argA, Expr<T> argB, Expr<U> argC, Expr<V> result);
    string Format(string argA, string argB, string argC);
    Expr<V> Make(Expr<S> ArgA, Expr<T> ArgB, Expr<U> ArgC);
    Eval<EVAL>.Info<V> Eval<EVAL>(Operation<S, T, U, V> op, Eval<EVAL> txt) where EVAL : Eval<EVAL>;
  }
  public partial interface OperatorX<S, T, U, V, W> : IBaseOperator<W, Func<S,T,U,V,W>> {
    Expr<W> Expand(Expr<S> argA, Expr<T> argB, Expr<U> argC, Expr<V> argD);
    Expr<S> InverseA(Expr<S> argA, Expr<T> argB, Expr<U> argC, Expr<V> argD, Expr<W> result);
    Expr<T> InverseB(Expr<S> argA, Expr<T> argB, Expr<U> argC, Expr<V> argD, Expr<W> result);
    Expr<U> InverseC(Expr<S> argA, Expr<T> argB, Expr<U> argC, Expr<V> argD, Expr<W> result);
    Expr<V> InverseD(Expr<S> argA, Expr<T> argB, Expr<U> argC, Expr<V> argD, Expr<W> result);
    string Format(string argA, string argB, string argC, string argD);
    Expr<W> Make(Expr<S> ArgA, Expr<T> ArgB, Expr<U> ArgC, Expr<V> ArgD);
    Eval<EVAL>.Info<W> Eval<EVAL>(Operation<S, T, U, V, W> op, Eval<EVAL> txt) where EVAL : Eval<EVAL>;
  }
  public partial interface IOperator<S, T, U, V, CNT>    : OperatorX<S, T, U, V   >, IOperator<V, CNT> where CNT : IOperator<S,T,U,V,CNT  > { }
  public partial interface IOperator<S, T, U, V, W, CNT> : OperatorX<S, T, U, V, W>, IOperator<W, CNT> where CNT : IOperator<S,T,U,V,W,CNT> { }

  public partial class BaseOperatorX<S> : IOperator<S> {
    public bool CanOptimize { get; set; }
    public BaseOperatorX() { CanOptimize = true; }
  }

  public partial class BaseOperatorX<S, T> : BaseOperatorX<T>, IOperatorX<S, T> {
    public virtual Expr<T> Expand(Expr<S> argA) { return null; }
    public virtual Expr<S> Inverse(Expr<S> argA, Expr<T> result) { return null; }
    public virtual string Format(string s) { return "(" + s + ")"; }
    public virtual Expr<T> Make(Expr<S> ArgA) {
      if (ArgA is Constant<S> && AsFunc != null && CanOptimize) {
        return new Constant<T>(AsFunc(((Constant<S>)ArgA).Value));
      }
      return new Operation<S, T>() { ArgA = ArgA, Op = this }; 
    }
    public virtual Eval<EVAL>.Info<T> Eval<EVAL>(Operation<S, T> op, Eval<EVAL> txt) where EVAL : Eval<EVAL> {
      {
        var memo = txt.Memo<T>(op);
        if (memo != null) return memo;
      }
      // try to compute, otherwise...
      var f = txt.Operator<S, T>(op);
      if (f != null) {
        var argA = txt.DoEval(op.ArgA);


        return txt.Memo(op, () => f(argA), argA);
      }
      return txt.DoEval(op.Expand0);
      //return txt.Memo(this, Expand0.Eval(txt));
    }
  }
  public partial class Operator<S, T, CNT> : BaseOperatorX<S, T>, IOperator<S, T, CNT> where CNT : Operator<S,T,CNT> {
    public static CNT Instance { get; private set; }
    public Operator() { (Instance == null).Assert(); Instance = (CNT) this; }
  }
  public partial class BaseOperatorX<S, T, U> : BaseOperatorX<U>, OperatorX<S, T, U> {
    public virtual Expr<U> Expand(Expr<S> argA, Expr<T> argB) { return null; }
    public virtual Expr<S> InverseA(Expr<S> argA, Expr<T> argB, Expr<U> result) { return null; }
    public virtual Expr<T> InverseB(Expr<S> argA, Expr<T> argB, Expr<U> result) { return null; }
    public virtual string Format(string argA, string argB) { return "(" + argA + ", " + argB + ")"; }
    public virtual Expr<U> Make(Expr<S> ArgA, Expr<T> ArgB) {
      if (CanOptimize && ArgA is Constant<S> && ArgB is Constant<T> && AsFunc != null) {
        return new Constant<U>(AsFunc(((Constant<S>)ArgA).Value, ((Constant<T>)ArgB).Value));
      }
      return new Operation<S, T, U>() { ArgA = ArgA, ArgB = ArgB, Op = this }; 
    }
    public virtual Eval<EVAL>.Info<U> Eval<EVAL>(Operation<S, T,U> op, Eval<EVAL> txt) where EVAL : Eval<EVAL> {
      {
        var memo = txt.Memo<U>(op);
        if (memo != null) return memo;
      }
      var f = txt.Operator<S, T, U>(op);
      if (f != null) {
        var argA = txt.DoEval(op.ArgA);
        var argB = txt.DoEval(op.ArgB);
        return txt.Memo(op, () => f(argA, argB), argA, argB);
      }
      return txt.DoEval(op.Expand0);
      //return txt.Memo(this, Expand0.Eval(txt));
    }
  }
  public partial class Operator<S, T, U, CNT> : BaseOperatorX<S, T, U>, IOperator<S, T, U, CNT> where CNT : Operator<S, T, U, CNT> {
    public static CNT Instance { get; private set; }
    public Operator() { (Instance == null).Assert(); Instance = (CNT) this; }
  }
  public partial class BaseOperatorX<S, T, U, V> : BaseOperatorX<V>, OperatorX<S, T, U, V> {
    public virtual Expr<V> Expand(Expr<S> argA, Expr<T> argB, Expr<U> argC) { return null; }
    public virtual Expr<S> InverseA(Expr<S> argA, Expr<T> argB, Expr<U> argC, Expr<V> result) { return null; }
    public virtual Expr<T> InverseB(Expr<S> argA, Expr<T> argB, Expr<U> argC, Expr<V> result) { return null; }
    public virtual Expr<U> InverseC(Expr<S> argA, Expr<T> argB, Expr<U> argC, Expr<V> result) { return null; }
    public virtual string Format(string argA, string argB, string argC) { return "(" + argA + ", " + argB + ", " + argC + ")"; }
    public Expr<V> Make(Expr<S> ArgA, Expr<T> ArgB, Expr<U> ArgC) {
      if (CanOptimize && ArgA is Constant<S> && ArgB is Constant<T> && ArgC is Constant<U> && AsFunc != null) {
        return new Constant<V>(AsFunc(((Constant<S>)ArgA).Value, ((Constant<T>)ArgB).Value, ((Constant<U>)ArgC).Value));
      }
      return new Operation<S, T, U, V>() { ArgA = ArgA, ArgB = ArgB, ArgC = ArgC, Op = this }; 
    }
    public virtual Eval<EVAL>.Info<V> Eval<EVAL>(Operation<S, T, U, V> op, Eval<EVAL> txt) where EVAL : Eval<EVAL> {
      {
        var memo = txt.Memo<V>(op);
        if (memo != null) return memo;
      }
      return EvalCenter(op, txt);
    }
    protected virtual Eval<EVAL>.Info<V> EvalCenter<EVAL>(Operation<S, T, U, V> op, Eval<EVAL> txt) where EVAL : Eval<EVAL> {
      var f = txt.Operator<S, T, U, V>(op);
      if (f != null) {
        var argA = txt.DoEval(op.ArgA);
        var argB = txt.DoEval(op.ArgB);
        var argC = txt.DoEval(op.ArgC);
        return txt.Memo(op, () => f(argA, argB, argC), argA, argB, argC);
      }
      return txt.DoEval(op.Expand0);
    }
  }
  public partial class Operator<S, T, U, V, CNT> : BaseOperatorX<S, T, U, V>, IOperator<S, T, U, V, CNT> where CNT : Operator<S, T, U, V, CNT> {
    public static CNT Instance { get; private set; }
    public Operator() { (Instance == null).Assert(); Instance = (CNT) this; }
  }
  public partial class BaseOperatorX<S, T, U, V, W> : BaseOperatorX<W>, OperatorX<S, T, U, V, W> {
    public virtual Expr<W>   Expand(Expr<S> argA, Expr<T> argB, Expr<U> argC, Expr<V> argD) { return null; }
    public virtual Expr<S> InverseA(Expr<S> argA, Expr<T> argB, Expr<U> argC, Expr<V> argD, Expr<W> result) { return null; }
    public virtual Expr<T> InverseB(Expr<S> argA, Expr<T> argB, Expr<U> argC, Expr<V> argD, Expr<W> result) { return null; }
    public virtual Expr<U> InverseC(Expr<S> argA, Expr<T> argB, Expr<U> argC, Expr<V> argD, Expr<W> result) { return null; }
    public virtual Expr<V> InverseD(Expr<S> argA, Expr<T> argB, Expr<U> argC, Expr<V> argD, Expr<W> result) { return null; }
    public virtual string Format(string argA, string argB, string argC, string argD) { return "(" + argA + ", " + argB + ", " + argC + ", " + argD + ")"; }
    public Expr<W> Make(Expr<S> ArgA, Expr<T> ArgB, Expr<U> ArgC, Expr<V> ArgD) {
      if (CanOptimize && ArgA is Constant<S> && ArgB is Constant<T> && ArgC is Constant<U> && ArgD is Constant<V> && AsFunc != null) {
        return new Constant<W>(AsFunc(((Constant<S>)ArgA).Value, ((Constant<T>)ArgB).Value, ((Constant<U>)ArgC).Value, ((Constant<V>)ArgD).Value));
      }
      return new Operation<S, T, U, V, W>() { ArgA = ArgA, ArgB = ArgB, ArgC = ArgC, ArgD = ArgD, Op = this };
    }
    public virtual Eval<EVAL>.Info<W> Eval<EVAL>(Operation<S, T, U, V, W> op, Eval<EVAL> txt) where EVAL : Eval<EVAL> {
      {
        var memo = txt.Memo<W>(op);
        if (memo != null) return memo;
      }
      var f = txt.Operator<S, T, U, V, W>(op);
      if (f != null) {
        var argA = txt.DoEval(op.ArgA);
        var argB = txt.DoEval(op.ArgB);
        var argC = txt.DoEval(op.ArgC);
        var argD = txt.DoEval(op.ArgD);
        return txt.Memo(op, () => f(argA, argB, argC, argD), argA, argB, argC, argD);
      }
      return txt.DoEval(op.Expand0);
    }
  }
  public partial class Operator<S, T, U, V, W, CNT> : BaseOperatorX<S, T, U, V, W>, IOperator<S, T, U, V, W, CNT> where CNT : Operator<S, T, U,V,W, CNT> {
    public static CNT Instance { get; private set; }
    public Operator() { (Instance == null).Assert(); Instance = (CNT) this; }
  }
  public partial class ProtectOperator<T> : BaseOperatorX<T, T> {
    public readonly Func<Expr<T>, Expr<T>> InverseF;
    public ProtectOperator(Func<Expr<T>, Expr<T>> F) {
      this.InverseF = F;
    }
    public override Expr<T> Expand(Expr<T> argA) { return argA; }
    public override Expr<T> Inverse(Expr<T> argA, Expr<T> result) {
      return InverseF(result);
    }
  }
  public interface IMapEval<EVAL> where EVAL : Eval<EVAL> {
    Eval<EVAL>.Info<T> Map<S, T>(Operation<S, T> op);
  }

  public partial class MapOperator<S, T> : BaseOperatorX<S, T> {
    public readonly Func<S, T> F;
    public Func<T, S> G;
    public MapOperator(Func<S, T> F) {
      this.F = F;
    }
    public override int GetHashCode() {
      return F.GetHashCode() + (G == null ? 0 : G.GetHashCode());
    }
    public override bool Equals(object obj) {
      if (obj is MapOperator<S, T>) {
        return F == ((MapOperator<S, T>)obj).F && G == ((MapOperator<S, T>)obj).G;
      }
      return base.Equals(obj);
    }
    public override Eval<EVAL>.Info<T> Eval<EVAL>(Operation<S, T> op, Eval<EVAL> txt) {
      if (txt is IMapEval<EVAL>) return ((IMapEval<EVAL>)txt).Map<S, T>(op);
      return base.Eval<EVAL>(op, txt);
    }

    public override string Format(string s) {
      return "Map" + base.Format(s);
    }
    public override Expr<S> Inverse(Expr<S> argA, Expr<T> result) {
      if (G == null) return base.Inverse(argA, result);
      return new MapOperator<T, S>(G) { G = F }.Make(result);
    }
  }
  public partial class MapOperator<S, T, U> : BaseOperatorX<S, T, U> {
    public readonly Func<S, T, U> STU;                      // u = s + t
    public virtual Func<S, U, T> SUT { private get; set; }
    public virtual Func<T, S, U> TSU { private get; set; }
    public virtual Func<T, U, S> TUS { private get; set; }  // s = -t + u
    public virtual Func<U, T, S> UTS { private get; set; }  // s = u - t
    public virtual Func<U, S, T> UST { private get; set; }  // t = u - s
    public MapOperator(Func<S, T, U> F) {
      this.STU = F;
    }
    public override string Format(string s, string t) {
      return "Map" + base.Format(s, t);
    }
    // given U and T
    public override Expr<S> InverseA(Expr<S> argA, Expr<T> argB, Expr<U> result) {
      if (UTS != null)
        return new MapOperator<U, T, S>(UTS) { UTS = STU, UST = SUT, SUT = UST, TUS = TSU, TSU = TUS }.Make(result, argB);
      if (TUS != null)
        return new MapOperator<T, U, S>(TUS) { UTS = SUT, UST = STU, SUT = TSU, TUS = UST, TSU = UTS }.Make(argB, result);

      return base.InverseA(argA, argB, result);

    }
    public override Expr<T> InverseB(Expr<S> argA, Expr<T> argB, Expr<U> result) {
      if (UST != null) 
        return new MapOperator<U, S, T>(UST) { UTS = TSU, UST = TUS, SUT = UTS, TUS = STU, TSU = SUT }.Make(result, argA);
      if (SUT != null)
        return new MapOperator<S, U, T>(SUT) { UTS = TUS, UST = TSU, SUT = STU, TUS = UTS, TSU = UST }.Make(argA, result);

      return base.InverseB(argA, argB, result);
    }
  }
  // direct coercion.
  public abstract partial class CastOperator<S, T, CNT> : Operator<S, T, CNT> where CNT : CastOperator<S,T,CNT> {
    public override string Format(string s) {
      return "To" + typeof(T).Name + base.Format(s);
    }
  }
  public partial class DownCastOperator<S, T> : CastOperator<S, T, DownCastOperator<S, T>> where T : S {
    private DownCastOperator() {}
    public new static readonly DownCastOperator<S,T> Instance = new DownCastOperator<S,T>();
    public override Expr<S> Inverse(Expr<S> argA, Expr<T> result) {
      return UpCastOperator<T, S>.Instance.Make(result);
    }
  }
  public partial class DownCastOperator1<S, T> : CastOperator<S, T, DownCastOperator1<S, T>> {
    private DownCastOperator1() { }
    public new static readonly DownCastOperator1<S, T> Instance = new DownCastOperator1<S, T>();
    public override Expr<S> Inverse(Expr<S> argA, Expr<T> result) {
      return UpCastOperator1<T, S>.Instance.Make(result);
    }
  }
  public partial class UpCastOperator<S, T> : CastOperator<S, T, UpCastOperator<S, T>> where S : T {
    private UpCastOperator() { }
    public new static readonly UpCastOperator<S, T> Instance = new UpCastOperator<S, T>();
    public override Expr<S> Inverse(Expr<S> argA, Expr<T> result) {
      return DownCastOperator<T, S>.Instance.Make(result);
    }
  }
  public partial class UpCastOperator1<S, T> : CastOperator<S, T, UpCastOperator1<S, T>> {
    private UpCastOperator1() { }
    public new static readonly UpCastOperator1<S, T> Instance = new UpCastOperator1<S, T>();
    public override Expr<S> Inverse(Expr<S> argA, Expr<T> result) {
      return DownCastOperator1<T, S>.Instance.Make(result);
    }
  }
  public partial class ToStringOperator : Operator<object, string, ToStringOperator> {
    public new static readonly ToStringOperator Instance = new ToStringOperator();
    public override string Format(string s) {
      return "ToString" + base.Format(s);
    }
  }
  public partial class ExplicitConvertOperator<S, T> : CastOperator<S, T, ExplicitConvertOperator<S, T>> {
    private static readonly System.Reflection.MethodInfo ConvertF;
    static ExplicitConvertOperator() {
      foreach (var m in typeof(S).GetMethods()) {
        if (!(m.Name == "op_Explicit") && !(m.Name == "op_Implicit")) continue;
        if (m.GetParameters().Length != 1) continue;
        if (!m.IsStatic) continue;
        if (m.ReturnType != typeof(T)) continue;
        if (m.GetParameters()[0].ParameterType != typeof(S)) continue;
        ConvertF = m;
        break;
      }
    }
    private ExplicitConvertOperator() { }
    public new static readonly ExplicitConvertOperator<S, T> Instance = new ExplicitConvertOperator<S, T>();
    public override Expr<S> Inverse(Expr<S> argA, Expr<T> result) {
      return ExplicitConvertOperator<T, S>.Instance.Make(result);
    }
  }
  public partial class SimpleOperator<S, T, U, CNT> : Operator<S, T, U, CNT> where CNT : SimpleOperator<S,T,U,CNT> {
  }
  public partial class AssociativeOperator<S, T, CNT> : SimpleOperator<S, S, T, CNT> where CNT : AssociativeOperator<S,T,CNT> {
    public override Expr<S> InverseB(Expr<S> argA, Expr<S> argB, Expr<T> result) {
      return InverseA(argA, argB, result);
    }
  }
  public abstract partial class BinaryOperator<S, T, CNT> : AssociativeOperator<S, T, CNT> where CNT : BinaryOperator<S,T,CNT> {
    public abstract string Infix { get; }
    public override string Format(string argA, string argB) {
      return "(" + argA + " " + Infix + " " + argB + ")";
    }
  }
  public abstract partial class BaseStaticCallOperator<T, S, CNT> : Operator<T, S, CNT> where CNT : BaseStaticCallOperator<T,S,CNT> {
    protected abstract string CallName { get; }
    protected abstract Type TargetType { get; }
    public override string Format(string s) {
      return TargetType.Name + "." + CallName + base.Format(s);
    }
  }
  public abstract partial class StaticCallOperator<T, S,    CNT> : BaseStaticCallOperator<T, S, CNT> where CNT : StaticCallOperator<T,S,CNT> { }
  public abstract partial class StaticCallOperator<T, S, U, CNT> : Operator<T, S, U, CNT> where CNT : StaticCallOperator<T,S,U,CNT> {
    protected abstract string CallName { get; }
    protected abstract Type TargetType { get; }
    public override string Format(string sA, string sB) {
      return TargetType.Name + "." + CallName + base.Format(sA, sB);
    }
  }
  public abstract partial class StaticCallOperator<T, S, U, V, CNT> : Operator<T, S, U, V, CNT> where CNT : StaticCallOperator<T,S,U,V,CNT> {
    protected abstract string CallName { get; }
    protected abstract Type TargetType { get; }
    public override string Format(string sA, string sB, string sC) {
      return TargetType.Name + "." + CallName + base.Format(sA, sB, sC);
    }
  }
  public static class OperatorExtensions {
    /*
    public static Expr<T> Access<T, ARITY>(this Expr<Vec<T, ARITY>> vec, int Idx) where ARITY : MultiArity<K,T>, new() {
      return MultiArity<K,T>.ArityI.Access<T>(vec, Idx);
    }*/
  }
  public interface IConditionEval<EVAL> : IEval<EVAL> where EVAL : Eval<EVAL> {
    Eval<EVAL>.Info<S> Condition<S>(Expr<bool> Test, Expr<S> IfTrue, Expr<S> IfFalse);
  }
  public interface IConditionEval2<EVAL> : IConditionEval<EVAL> where EVAL : Eval<EVAL> {
    Eval<EVAL>.Info<S> DCondition<S>(Expr<bool> Test, Expr<S> IfTrue, Expr<S> IfFalse);
  }


  public static partial class ValueOperators<T> {
    public partial class Equal : Ops.BinaryOperator<T, bool, Equal> {
      private Equal() { }
      public override string Infix { get { return "=="; } }
      public new static readonly Equal Instance = new Equal();
    }
    public partial class NotEqual : Ops.BinaryOperator<T, bool, NotEqual> {
      private NotEqual() { }
      public override string Infix { get { return "!="; } }
      public new static readonly NotEqual Instance = new NotEqual();
    }
    public partial class BaseCondition<OP> : Operator<bool, T, T, T, OP> where OP : BaseCondition<OP> {
      public override string Format(string argA, string argB, string argC) {
        return argA + " ? " + argB + " : " + argC;
      }
      protected override Eval<EVAL>.Info<T> EvalCenter<EVAL>(Operation<bool, T, T, T> op, Eval<EVAL> txt) {
        if (txt is IConditionEval<EVAL>) {
          return txt.Memo(op, () => ((IConditionEval<EVAL>)txt).Condition(op.ArgA, op.ArgB, op.ArgC));
        } else if (txt is TranslateEval<EVAL>) {
          return base.EvalCenter(op, txt);
        } else {
          return base.EvalCenter<EVAL>(op, txt);
        }
      }
    }
    public partial class Condition : BaseCondition<Condition> {
      public new static readonly Condition Instance = new Condition();
      private Condition() { }
    }
    public partial class DynamicCondition : BaseCondition<DynamicCondition> {
      public new static readonly DynamicCondition Instance = new DynamicCondition();
      protected override Eval<EVAL>.Info<T> EvalCenter<EVAL>(Operation<bool, T, T, T> op, Eval<EVAL> txt) {
        if (txt is IConditionEval2<EVAL>) {
          return txt.Memo(op, () => ((IConditionEval2<EVAL>)txt).DCondition(op.ArgA, op.ArgB, op.ArgC));
        } else return base.EvalCenter(op, txt);
      }
    }

  }
  public abstract partial class BinaryOperator<T, K, T0, K0, CNT> : BinaryOperator<T, T0, CNT> where CNT : BinaryOperator<T,K,T0,K0,CNT> {
    public abstract Func<Expr<K>, Expr<K>, Expr<K0>> Uniform { get; }
    public override Expr<T0> Expand(Expr<T> argA, Expr<T> argB) {
      if (Arity<K,T>.ArityI.Count == 1) return base.Expand(argA, argB);
      var Cs = new Expr<K0>[Arity<K,T>.ArityI.Count];
      for (int i = 0; i < Arity<K,T>.ArityI.Count; i++) {
        Cs[i] = Uniform(Arity<K,T>.ArityI.Access(argA, i), Arity<K,T>.ArityI.Access(argB, i));
      }
      return Arity<K0,T0>.ArityI.Make(Cs);
    }
  }
  public abstract partial class UniformBinaryOperator<T, K, CNT> : Ops.BinaryOperator<T, T, CNT> where CNT : UniformBinaryOperator<T,K,CNT> {
    public abstract Ops.OperatorX<K, K, K> Uniform { get; }
    public override Expr<T> Expand(Expr<T> argA, Expr<T> argB) {
      if (Arity<K,T>.ArityI.Count == 1) return base.Expand(argA, argB);
      var Cs = new Expr<K>[Arity<K, T>.ArityI.Count];
      for (int i = 0; i < Arity<K, T>.ArityI.Count; i++) {
        Cs[i] = Uniform.Make(Arity<K, T>.ArityI.Access(argA, i), Arity<K, T>.ArityI.Access(argB, i));
      }
      return Arity<K, T>.ArityI.Make(Cs);
    }
  }
  public abstract partial class UniformUnaryOperator<T, K, CNT> : Ops.Operator<T, T, CNT> where CNT : UniformUnaryOperator<T,K,CNT> {
    public abstract IOperatorX<K, K> Uniform { get; }
    public abstract string Unary { get; }
    public override string Format(string s) { return Unary + "(" + s + ")"; }
    public override Expr<T> Expand(Expr<T> argA) {
      if (Arity<K,T>.ArityI.Count == 1) return base.Expand(argA);
      var Cs = new Expr<K>[Arity<K, T>.ArityI.Count];
      for (int i = 0; i < Arity<K, T>.ArityI.Count; i++)
        Cs[i] = Uniform.Make(Arity<K, T>.ArityI.Access(argA, i));
      return Arity<K, T>.ArityI.Make(Cs);
    }
  }
  public static partial class BoolOperators<T> {
    public partial class And : UniformBinaryOperator<T, bool, And> {
      public new static readonly And Instance = new And();
      private And() { }
      public override string Infix { get { return "&&"; } }
      public override Ops.OperatorX<bool, bool, bool> Uniform { get { return BoolOperators<bool>.And.Instance; } }
    }
    public partial class Or : UniformBinaryOperator<T, bool, Or> {
      public new static readonly Or Instance = new Or();
      private Or() { }
      public override string Infix { get { return "||"; } }
      public override Ops.OperatorX<bool, bool, bool> Uniform { get { return BoolOperators<bool>.Or.Instance; } }
    }
    public partial class ExclusiveOr : UniformBinaryOperator<T, bool, ExclusiveOr> {
      public new static readonly ExclusiveOr Instance = new ExclusiveOr();
      private ExclusiveOr() { }
      public override string Infix { get { return "^"; } }
      public override Ops.OperatorX<bool, bool, bool> Uniform { get { return BoolOperators<bool>.ExclusiveOr.Instance; } }
    }
    public partial class Not : UniformUnaryOperator<T, bool, Not> {
      public new static readonly Not Instance = new Not();
      private Not() { }
      public override string Unary { get { return "!"; } }
      public override Ops.IOperatorX<bool, bool> Uniform { get { return BoolOperators<bool>.Not.Instance; } }
    }
  }
  public static partial class VecConvert<T, K, VEC> where VEC : Vec<K> {
    public abstract partial class Operator<A, B, CNT> : Ops.Operator<A, B, CNT> where CNT : Operator<A,B,CNT> {}
    public partial class FromVec : Operator<VEC, T, FromVec> {
      public new static FromVec Instance = new FromVec();
      private FromVec() { }
      public override Expr<T> Make(Expr<VEC> ArgA) {
        if (typeof(VEC) == typeof(T)) return (Expr<T>)(object)ArgA;
        else if (ArgA is Operation<T, VEC> && ((Operation<T, VEC>)ArgA).Op == ToVec.Instance) {
          // prevent round tripping. 
          return ((Operation<T, VEC>)ArgA).ArgA;
        } else return base.Make(ArgA);
      }
      public override Expr<T> Expand(Expr<VEC> argA) {
        (Arity<K,VEC>.ArityI.Count == Arity<K,T>.ArityI.Count).Assert();
        var Cs = new Expr<K>[Arity<K,VEC>.ArityI.Count];
        for (int i = 0; i < Arity<K, VEC>.ArityI.Count; i++ )
          Cs[i] = Arity<K, VEC>.ArityI.Access(argA, i);
        return Arity<K, T>.ArityI.Make(Cs);
      }
      public override Expr<VEC> Inverse(Expr<VEC> argA, Expr<T> result) {
        return ToVec.Instance.Make(result);
      }
    }
    public partial class ToVec : Operator<T, VEC, ToVec> {
      public new static ToVec Instance = new ToVec();
      private ToVec() {
      }
      public override Expr<VEC> Make(Expr<T> ArgA) {
        if (typeof(VEC) == typeof(T)) return (Expr<VEC>)(object)ArgA;
        else if (ArgA is Operation<VEC, T> && ((Operation<VEC, T>)ArgA).Op == FromVec.Instance) {
          // prevent round tripping. 
          return ((Operation<VEC, T>)ArgA).ArgA;
        } else return base.Make(ArgA);
      }
      public override Expr<VEC> Expand(Expr<T> argA) {
        (Arity<K, VEC>.ArityI.Count == Arity<K, T>.ArityI.Count).Assert();
        var Cs = new Expr<K>[Arity<K, VEC>.ArityI.Count];
        for (int i = 0; i < Arity<K, VEC>.ArityI.Count; i++)
          Cs[i] = Arity<K, T>.ArityI.Access(argA, i);
        return Arity<K, VEC>.ArityI.Make(Cs);
      }
      public override Expr<T> Inverse(Expr<T> argA, Expr<VEC> result) {
        return FromVec.Instance.Make(result);
      }
    }
  }
  public static partial class NumericOperators0<T, K> {
    public static int StaticInit() { return 0; }
    public abstract partial class MathOperator<CNT> : Ops.StaticCallOperator<T, T, CNT> where CNT : MathOperator<CNT> {
      public abstract IOperatorX<K, K> Uniform { get; }
      protected override Type TargetType { get { return typeof(Math); } }
      public override Expr<T> Expand(Expr<T> argA) {
        if (Arity<K,T>.ArityI.Count == 1) return base.Expand(argA);
        var Cs = new Expr<K>[Arity<K, T>.ArityI.Count];
        for (int i = 0; i < Arity<K, T>.ArityI.Count; i++)
          Cs[i] = Uniform.Make(Arity<K, T>.ArityI.Access(argA, i));
        return Arity<K, T>.ArityI.Make(Cs);
      }
    }
    public abstract class Math2Operator<CNT> : Ops.StaticCallOperator<T, T, T, CNT> where CNT : Math2Operator<CNT> {
      public abstract OperatorX<K, K, K> Uniform { get; }
      protected override Type TargetType { get { return typeof(Math); } }
      public override Expr<T> Expand(Expr<T> argA, Expr<T> argB) {
        if (Arity<K,T>.ArityI.Count == 1) return base.Expand(argA, argB);
        var Cs = new Expr<K>[Arity<K,T>.ArityI.Count];
        for (int i = 0; i < Arity<K,T>.ArityI.Count; i++)
          Cs[i] = Uniform.Make(Arity<K,T>.ArityI.Access(argA, i), Arity<K,T>.ArityI.Access(argB, i));
        return Arity<K,T>.ArityI.Make(Cs);
      }
    }
    public abstract class Math3Operator<CNT> : Ops.StaticCallOperator<T, T, T, T, CNT> where CNT : Math3Operator<CNT> {
      public abstract OperatorX<K, K, K, K> Uniform { get; }
      protected override Type TargetType { get { return typeof(Math); } }
      public override Expr<T> Expand(Expr<T> argA, Expr<T> argB, Expr<T> argC) {
        if (Arity<K,T>.ArityI.Count == 1) return base.Expand(argA, argB, argC);
        var Cs = new Expr<K>[Arity<K,T>.ArityI.Count];
        for (int i = 0; i < Arity<K,T>.ArityI.Count; i++)
          Cs[i] = Uniform.Make(Arity<K,T>.ArityI.Access(argA, i), Arity<K,T>.ArityI.Access(argB, i), Arity<K,T>.ArityI.Access(argC, i));
        return Arity<K,T>.ArityI.Make(Cs);
      }
    }

    public partial class Abs : MathOperator<Abs> {
      public new static readonly Abs Instance = new Abs();
      private Abs() { }
      public override IOperatorX<K, K> Uniform { get { return NumericOperators0<K, K>.Abs.Instance; } }
      protected override string CallName { get { return "Abs"; } }
    }
    public partial class Min : Math2Operator<Min> {
      public new static readonly Min Instance = new Min();
      private Min() { }
      public override OperatorX<K, K, K> Uniform { get { return NumericOperators0<K, K>.Min.Instance; } }
      protected override string CallName { get { return "Min"; } }
    }
    public partial class Max : Math2Operator<Max> {
      public new static readonly Max Instance = new Max();
      private Max() { }
      public override OperatorX<K, K, K> Uniform { get { return NumericOperators0<K, K>.Max.Instance; } }
      protected override string CallName { get { return "Max"; } }
    }
    public partial class Clamp : Math3Operator<Clamp> {
      public new static readonly Clamp Instance = new Clamp();
      private Clamp() { }
      public override OperatorX<K, K, K, K> Uniform { get { return NumericOperators0<K, K>.Clamp.Instance; } }
      protected override string CallName { get { return "Clamp"; } }
      public override Expr<T> Expand(Expr<T> argA, Expr<T> argB, Expr<T> argC) {
        return Min.Instance.Make(Max.Instance.Make(argA, argB), argC);
      }
    }



    public partial class Negate : UniformUnaryOperator<T, K, Negate> {
      public new static readonly Negate Instance = new Negate();
      private Negate() { }
      public override string Unary { get { return "-"; } }
      public override IOperatorX<K, K> Uniform { get { return NumericOperators0<K, K>.Negate.Instance; } }
      public override Expr<T> Inverse(Expr<T> argA, Expr<T> result) { return Make(result); }
    }
    public abstract partial class ArithmeticOperator<CNT> : UniformBinaryOperator<T, K, CNT> where CNT : ArithmeticOperator<CNT> {
      protected virtual OperatorX<T, T, T> InverseAOp { get { return null; } }
      protected virtual OperatorX<T, T, T> InverseBOp { get { return null; } }
      public override Expr<T> InverseA(Expr<T> argA, Expr<T> argB, Expr<T> result) {
        if (InverseAOp != null) return InverseAOp.Make(result, argB);
        return base.InverseA(argA, argB, result);
      }
      public override Expr<T> InverseB(Expr<T> argA, Expr<T> argB, Expr<T> result) {
        if (InverseBOp != null) return InverseBOp.Make(argA, result);
        return base.InverseB(argA, argB, result);
      }
    }
    public partial class Add : ArithmeticOperator<Add> {
      public new static readonly Add Instance = new Add();
      private Add() { }
      protected override OperatorX<T, T, T> InverseAOp { get { return Subtract.Instance; } }
      protected override OperatorX<T, T, T> InverseBOp { get { return Subtract.Instance; } }
      public override string Infix { get { return "+"; } }
      public override OperatorX<K, K, K> Uniform { get { return NumericOperators0<K, K>.Add.Instance; } }

      public override Expr<T> Make(Expr<T> ArgA, Expr<T> ArgB) {
        if (ArgA is Constant<T> && ((Constant<T>)ArgA).Value.Equals(default(T))) return ArgB;
        if (ArgB is Constant<T> && ((Constant<T>)ArgB).Value.Equals(default(T))) return ArgA;
        return base.Make(ArgA, ArgB);
      }

    }
    public partial class Subtract : ArithmeticOperator<Subtract> {
      public new static readonly Subtract Instance = new Subtract();
      private Subtract() { }
      protected override OperatorX<T, T, T> InverseAOp { get { return Add.Instance; } } // argA - argB = result => result + argB
      protected override OperatorX<T, T, T> InverseBOp { get { return this; } } // argA - result
      public override string Infix { get { return "-"; } }
      public override OperatorX<K, K, K> Uniform { get { return NumericOperators0<K, K>.Subtract.Instance; } }
    }
    public partial class Multiply : ArithmeticOperator<Multiply> {
      public new static readonly Multiply Instance = new Multiply();
      private Multiply() { }
      protected override OperatorX<T, T, T> InverseAOp { get { return Divide.Instance; } }
      protected override OperatorX<T, T, T> InverseBOp { get { return Divide.Instance; } }
      public override string Infix { get { return "*"; } }
      public override OperatorX<K, K, K> Uniform { get { return NumericOperators0<K, K>.Multiply.Instance; } }
      public override Expr<T> Make(Expr<T> ArgA, Expr<T> ArgB) {
        if (ArgA is Constant<T> && ((Constant<T>)ArgA).Value.Equals(default(T))) return new Constant<T>(default(T));
        if (ArgB is Constant<T> && ((Constant<T>)ArgB).Value.Equals(default(T))) return new Constant<T>(default(T));
        if (typeof(T) == typeof(double)) {
          if (ArgA.Equals(new Constant<double>(1d))) return ArgB;
          if (ArgB.Equals(new Constant<double>(1d))) return ArgA;
        }
        return base.Make(ArgA, ArgB);
      }

    }
    public partial class Divide : ArithmeticOperator<Divide> {
      public new static readonly Divide Instance = new Divide();
      private Divide() { }
      protected override OperatorX<T, T, T> InverseAOp { get { return Multiply.Instance; } } // result * argB
      protected override OperatorX<T, T, T> InverseBOp { get { return this; } }              // argA / result
      public override string Infix { get { return "/"; } }
      public override OperatorX<K, K, K> Uniform { get { return NumericOperators0<K, K>.Divide.Instance; } }
      public override Expr<T> Make(Expr<T> ArgA, Expr<T> ArgB) {
        if (ArgA is Constant<T> && ((Constant<T>)ArgA).Value.Equals(default(T))) return new Constant<T>(default(T));
        if (typeof(T) == typeof(double)) {
          if (ArgB.Equals(new Constant<double>(1d))) return ArgA;
        }
        return base.Make(ArgA, ArgB);
      }
    }
    public partial class Modulo : ArithmeticOperator<Modulo> {
      public new static readonly Modulo Instance = new Modulo();
      private Modulo() { }
      public override string Infix { get { return "%"; } }
      public override OperatorX<K, K, K> Uniform { get { return NumericOperators0<K, K>.Modulo.Instance; } }
    }
  }
  public static partial class NumericOperators<T, K, BT> {
    public abstract partial class ArithmeticOperator<CNT> : BinaryOperator<T, K, BT, bool, CNT> where CNT : ArithmeticOperator<CNT> {
    }
    public partial class GreaterThan : ArithmeticOperator<GreaterThan> {
      public new static readonly GreaterThan Instance = new GreaterThan();
      private GreaterThan() { }
      public override string Infix { get { return ">"; } }
      public override Func<Expr<K>, Expr<K>, Expr<bool>> Uniform { get { return NumericOperators<K, K, bool>.GreaterThan.Instance.Make; } }
    }
    public partial class LessThan : ArithmeticOperator<LessThan> {
      public new static readonly LessThan Instance = new LessThan();
      private LessThan() { }
      public override string Infix { get { return "<"; } }
      public override Func<Expr<K>, Expr<K>, Expr<bool>> Uniform { get { return NumericOperators<K, K, bool>.LessThan.Instance.Make; } }
    }
    public partial class GreaterThanOrEqual : ArithmeticOperator<GreaterThanOrEqual> {
      public new static readonly GreaterThanOrEqual Instance = new GreaterThanOrEqual();
      private GreaterThanOrEqual() { }
      public override string Infix { get { return ">="; } }
      public override Func<Expr<K>, Expr<K>, Expr<bool>> Uniform { get { return NumericOperators<K, K, bool>.GreaterThanOrEqual.Instance.Make; } }
    }
    public partial class LessThanOrEqual : ArithmeticOperator<LessThanOrEqual> {
      public new static readonly LessThanOrEqual Instance = new LessThanOrEqual();
      private LessThanOrEqual() { }
      public override string Infix { get { return "<="; } }
      public override Func<Expr<K>, Expr<K>, Expr<bool>> Uniform { get { return NumericOperators<K, K, bool>.LessThanOrEqual.Instance.Make; } }
    }
  }
  public static partial class DoubleOperators0<T>  {
    public abstract partial class MathOperator<CNT> : NumericOperators0<T, double>.MathOperator<CNT> where CNT : MathOperator<CNT> { }
    public abstract partial class Math2Operator<CNT> : NumericOperators0<T, double>.Math2Operator<CNT> where CNT : Math2Operator<CNT> { }

    public static void CheckInit() {
      //Extensions.Trace(""+ DoubleOperators0<T>.Lerp);
    }

    public partial class Saturate : MathOperator<Saturate> {
      public new static readonly Saturate Instance = new Saturate();
      private Saturate() { }
      public override IOperatorX<double, double> Uniform { get { return DoubleOperators0<double>.Saturate.Instance; } }
      protected override string CallName { get { return "Saturate"; } }
      public override Expr<T> Expand(Expr<T> argA) {
        return NumericOperators0<T, double>.Clamp.Instance.Make(argA, 
          Arity<double, T>.ArityI.Repeat(0d), 
          Arity<double, T>.ArityI.Repeat(1d));
      }
    }

    public partial class Lerp : StaticCallOperator<T, T, double, T, Lerp> {
      public new static readonly Lerp Instance = new Lerp();
      private Lerp() { }
      protected override string CallName { get { return "Lerp"; } }
      protected override Type TargetType { get { return typeof(Math); } }
      public override Expr<T> Expand(Expr<T> argA, Expr<T> argB, Expr<double> argC) {
        var Cs = new Expr<double>[Arity<double, T>.ArityI.Count];
        var a = argA;
        var b = argB;
        var ba = NumericOperators0<T, double>.Subtract.Instance.Make(b, a);
        var t = Arity<double, T>.ArityI.Repeat(argC);
        ba = NumericOperators0<T, double>.Multiply.Instance.Make(ba, t);
        return NumericOperators0<T, double>.Add.Instance.Make(a, ba);
      }
    }


    public partial class Floor : MathOperator<Floor> {
      public new static readonly Floor Instance = new Floor();
      private Floor() { }
      public override IOperatorX<double, double> Uniform { get { return DoubleOperators0<double>.Floor.Instance; } }
      protected override string CallName { get { return "Floor"; } }
    }
    public partial class Ceiling : MathOperator<Ceiling> {
      public new static readonly Ceiling Instance = new Ceiling();
      private Ceiling() { }
      public override IOperatorX<double, double> Uniform { get { return DoubleOperators0<double>.Ceiling.Instance; } }
      protected override string CallName { get { return "Ceiling"; } }
    }
    public partial class Round : MathOperator<Round> {
      public new static readonly Round Instance = new Round();
      private Round() { }
      public override IOperatorX<double, double> Uniform { get { return DoubleOperators0<double>.Round.Instance; } }
      protected override string CallName { get { return "Round"; } }
    }
    public partial class Exp : MathOperator<Exp> {
      public new static readonly Exp Instance = new Exp();
      private Exp() { }
      public override IOperatorX<double, double> Uniform { get { return DoubleOperators0<double>.Exp.Instance; } }
      protected override string CallName { get { return "Exp"; } }
      public override Expr<T> Inverse(Expr<T> argA, Expr<T> result) {
        return Log.Instance.Make(result);
      }
    }
    public partial class Log : MathOperator<Log> {
      public new static readonly Log Instance = new Log();
      private Log() { }
      public override IOperatorX<double, double> Uniform { get { return DoubleOperators0<double>.Log.Instance; } }
      protected override string CallName { get { return "Log"; } }
      public override Expr<T> Inverse(Expr<T> argA, Expr<T> result) {
        return Exp.Instance.Make(result);
      }
    }
    public partial class Exp2 : MathOperator<Exp2> {
      public new static readonly Exp2 Instance = new Exp2();
      private Exp2() { }
      public override IOperatorX<double, double> Uniform { get { return DoubleOperators0<double>.Exp2.Instance; } }
      protected override string CallName { get { return "Exp2"; } }
      public override Expr<T> Inverse(Expr<T> argA, Expr<T> result) {
        return Log2.Instance.Make(result);
      }
      public override Expr<T> Expand(Expr<T> argA) {
        return Pow.Instance.Make(Arity<double,T>.ArityI.Repeat(2d), argA);
      }
    }
    public partial class Log2 : MathOperator<Log2> {
      public new static readonly Log2 Instance = new Log2();
      private Log2() { }
      public override IOperatorX<double, double> Uniform { get { return DoubleOperators0<double>.Log2.Instance; } }
      protected override string CallName { get { return "Log2"; } }
      public override Expr<T> Inverse(Expr<T> argA, Expr<T> result) {
        return Exp2.Instance.Make(result);
      }
      public override Expr<T> Expand(Expr<T> argA) {
        return LogN.Instance.Make(argA, Arity<double,T>.ArityI.Repeat(2d));
      }
    }
    public partial class Exp10 : MathOperator<Exp10> {
      public new static readonly Exp10 Instance = new Exp10();
      private Exp10() { }
      public override IOperatorX<double, double> Uniform { get { return DoubleOperators0<double>.Exp10.Instance; } }
      protected override string CallName { get { return "Exp10"; } }
      public override Expr<T> Inverse(Expr<T> argA, Expr<T> result) {
        return Log10.Instance.Make(result);
      }
      public override Expr<T> Expand(Expr<T> argA) {
        return Pow.Instance.Make(Arity<double,T>.ArityI.Repeat(10d), argA);
      }
    }
    public partial class Log10 : MathOperator<Log10> {
      public new static readonly Log10 Instance = new Log10();
      private Log10() { }
      public override IOperatorX<double, double> Uniform { get { return DoubleOperators0<double>.Log10.Instance; } }
      protected override string CallName { get { return "Log10"; } }
      public override Expr<T> Inverse(Expr<T> argA, Expr<T> result) {
        return Exp10.Instance.Make(result);
      }
      public override Expr<T> Expand(Expr<T> argA) {
        return LogN.Instance.Make(argA, Arity<double,T>.ArityI.Repeat(10d));
      }
    }

    public partial class Pow : Math2Operator<Pow> {
      public new static readonly Pow Instance = new Pow();
      private Pow() { }
      public override OperatorX<double, double, double> Uniform { get { return DoubleOperators0<double>.Pow.Instance; } }
      protected override string CallName { get { return "Pow"; } }
      public override Expr<T> InverseB(Expr<T> argA, Expr<T> argB, Expr<T> result) {
        var Base = argA;
        var Power = argB;
        var Raised = result;
        // Raised = Pow(Base, Power);
        return LogN.Instance.Make(Raised, Base);
      }
    }
    public partial class LogN : Math2Operator<LogN> {
      public new static readonly LogN Instance = new LogN();
      private LogN() { }
      public override OperatorX<double, double, double> Uniform { get { return DoubleOperators0<double>.LogN.Instance; } }
      protected override string CallName { get { return "Log"; } }
      public override Expr<T> InverseA(Expr<T> argA, Expr<T> argB, Expr<T> result) {
        var Power = result;
        var Raised = argA;
        var Base = argB;
        // Power = Log(Raised, Base)
        return Pow.Instance.Make(Base, Power);
      }
    }
    public partial class Recip : MathOperator<Recip> {
      public new static readonly Recip Instance = new Recip();
      private Recip() { }
      public override IOperatorX<double, double> Uniform { get { return DoubleOperators0<double>.Recip.Instance; } }
      protected override string CallName { get { return "Recip"; } }
      public override Expr<T> Inverse(Expr<T> argA, Expr<T> result) {
        return this.Make(result);
      }
      public override Expr<T> Expand(Expr<T> argA) {
        return NumericOperators0<T, double>.Divide.Instance.Make(Arity<double,T>.ArityI.Repeat((1d)), argA);
      }
    }
    public partial class Square : MathOperator<Square> {
      public new static readonly Square Instance = new Square();
      private Square() { }
      public override IOperatorX<double, double> Uniform { get { return DoubleOperators0<double>.Square.Instance; } }
      protected override string CallName { get { return "Square"; } }
      public override Expr<T> Inverse(Expr<T> argA, Expr<T> result) {
        return Sqrt.Instance.Make(result);
      }
      public override Expr<T> Expand(Expr<T> argA) {
        return NumericOperators0<T, double>.Multiply.Instance.Make(argA, argA);
      }
    }
    public partial class Sqrt : MathOperator<Sqrt> {
      public new static readonly Sqrt Instance = new Sqrt();
      private Sqrt() { }
      public override IOperatorX<double, double> Uniform { get { return DoubleOperators0<double>.Sqrt.Instance; } }
      protected override string CallName { get { return "Sqrt"; } }
      public override Expr<T> Inverse(Expr<T> argA, Expr<T> result) {
        return Square.Instance.Make(result);
      }
    }
    /*
    public partial class Sign : StaticCallOperator<T, int, Sign> {
      public new static readonly Sign Instance = new Sign();
      private Sign() { }
      public override OperatorX<double, double> Uniform { get { return DoubleOperators<double>.Sign.Instance; } }
      protected override string CallName { get { return "Sign"; } }
    }
     */
    public partial class Rsqrt : MathOperator<Rsqrt> {
      public new static readonly Rsqrt Instance = new Rsqrt();
      private Rsqrt() { }
      public override IOperatorX<double, double> Uniform { get { return DoubleOperators0<double>.Rsqrt.Instance; } }
      protected override string CallName { get { return "Rsqrt"; } }
      public override Expr<T> Inverse(Expr<T> argA, Expr<T> result) {
        return Square.Instance.Make(result);
      }
      public override Expr<T> Expand(Expr<T> argA) {
        return Recip.Instance.Make(Sqrt.Instance.Make(argA));
      }
    }
    public partial class Frac : MathOperator<Frac> {
      private Frac() {  }
      public new static readonly Frac Instance = new Frac();
      protected override string CallName { get { return "Frac"; } }
      public override IOperatorX<double, double> Uniform { get { return DoubleOperators0<double>.Frac.Instance; } }
      public override Expr<T> Expand(Expr<T> argA) {
        return NumericOperators0<T, double>.Abs.Instance.Make(
          NumericOperators0<T, double>.Modulo.Instance.Make(argA, Arity<double,T>.ArityI.Repeat(1d)));
      }
    }
    public partial class Ldexp : Math2Operator<Ldexp> {
      private Ldexp() { }
      public new static readonly Ldexp Instance = new Ldexp();
      public override OperatorX<double, double, double> Uniform { get { return DoubleOperators0<double>.Ldexp.Instance; } }
      protected override string CallName { get { return "Ldexp"; } }
      public override Expr<T> Expand(Expr<T> argA, Expr<T> argB) {
        var x = argA;
        var exp = argB;
        return NumericOperators0<T, double>.Multiply.Instance.Make(x, Exp2.Instance.Make(exp));
      }
    }
    public partial class Ddx : MathOperator<Ddx> {
      private Ddx() { }
      public override IOperatorX<double, double> Uniform { get { return DoubleOperators0<double>.Ddx.Instance; } }
      public new static readonly Ddx Instance = new Ddx();
      protected override string CallName { get { return "Ddx"; } }
    }
    public partial class Ddy : MathOperator<Ddy> {
      private Ddy() { }
      public override IOperatorX<double, double> Uniform { get { return DoubleOperators0<double>.Ddy.Instance; } }
      public new static readonly Ddy Instance = new Ddy();
      protected override string CallName { get { return "Ddy"; } }
    }
    public partial class FWidth : MathOperator<FWidth> {
      private FWidth() { }
      public override IOperatorX<double, double> Uniform { get { return DoubleOperators0<double>.FWidth.Instance; } }
      public new static readonly FWidth Instance = new FWidth();
      protected override string CallName { get { return "FWidth"; } }
      public override Expr<T> Expand(Expr<T> argA) {
        return NumericOperators0<T, double>.Add.Instance.Make(
          NumericOperators0<T, double>.Abs.Instance.Make(Ddx.Instance.Make(argA)),
          NumericOperators0<T, double>.Abs.Instance.Make(Ddy.Instance.Make(argA)));
      }
    }

    /*
    public partial class Faceforward : Operator<T,T,T,T,T,Faceforward> {
      public override Func<linq.Expression, linq.Expression, linq.Expression, linq.Expression, linq.Expression> Linq(Operation<T, T, T, T, T> op) {
        return null;
      }
      public override Expr<T> Expand(Expr<T> argA, Expr<T> argB, Expr<T> argC, Expr<T> argD) {
        var n = argA;
        var i = argB;
        var ng = argC;
        var ret = argD;

        var nx = NumericOperators<T, double>.Negate.Instance.Make(n);
        var i_dot_ng = Dot.Instance.Make(i, ng);
        var sin_i_dot_ng = Sign.Instance.Uniform(i_dot_ng);
        var d_sin_i_dot_ng = NumericOperators<int, Arity1>.Convert<double>.Instance.Convert()(sin_i_dot_ng);
        return NumericOperators<T, double>.Multiply.Instance.Make(nx, Arity<K,T>.Composite<double>.Repeat(d_sin_i_dot_ng));
      }


      public override string Format(string argA, string argB, string argC, string argD) {
        return "faceforward" + base.Format(argA, argB, argC, argD);
      }
    }
    */



    public partial class LengthSquared : StaticCallOperator<T, double, LengthSquared> {
      public new static readonly LengthSquared Instance = new LengthSquared();
      private LengthSquared() { }
      protected override Type TargetType { get { return typeof(Math); } }
      protected override string CallName { get { return "LengthSquared"; } }
      public override Expr<double> Expand(Expr<T> argA) {
        return Dot.Instance.Make(argA, argA);
      }
    }
    public partial class Length  : StaticCallOperator<T, double, Length> {
      public new static readonly Length Instance = new Length();
      private Length() { }
      protected override Type TargetType { get { return typeof(Math); } }
      protected override string CallName { get { return "Length"; } }
      public override Expr<double> Expand(Expr<T> argA) {
        return DoubleOperators0<double>.Sqrt.Instance.Make(LengthSquared.Instance.Make(argA));
      }
    }
    public partial class Normalize : MathOperator<Normalize> {
      public new static readonly Normalize Instance = new Normalize();
      private Normalize() { }
      public override IOperatorX<double, double> Uniform { get { return DoubleOperators0<double>.Normalize.Instance; } }
      protected override string CallName { get { return "Normalize"; } }
      public override Expr<T> Expand(Expr<T> argA) {
        var length = Length.Instance.Make(argA);
        var isTooSmall = NumericOperators<double, double, bool>.LessThanOrEqual.Instance.Make(length, new Constant<double>(0d));
        var op1 = NumericOperators0<T, double>.Divide.Instance.Make(argA,
          Arity<double,T>.ArityI.Repeat(length));

        var Unit = new double[Arity<double, T>.ArityI.Count];
        int N = 0;
        Unit[N] = 1;
        for (int i = 0; i < Unit.Length; i++) if (i != N) Unit[i] = 0;
        Expr<T> Unit0 = new Constant<T>(Arity<double, T>.ArityI.Make(Unit));
        Unit0 = argA;
        return ValueOperators<T>.Condition.Instance.Make(isTooSmall, (Unit0), op1);
      }
    }


    public partial class Dot : StaticCallOperator<T, T, double, Dot> {
      public new static readonly Dot Instance = new Dot();
      private Dot() { }
      protected override string CallName { get { return "Dot"; } }
      protected override Type TargetType { get { return typeof(Math); } }
      public override Expr<double> Expand(Expr<T> argA, Expr<T> argB) {
        Expr<double> result = new Constant<double>(default(double));
        for (int i = 0; i < Arity<double,T>.ArityI.Count; i++) {
          result = NumericOperators0<double, double>.Add.Instance.Make(result,
            NumericOperators0<double, double>.Multiply.Instance.Make(
              Arity<double,T>.ArityI.Access(argA, i),
              Arity<double,T>.ArityI.Access(argB, i)));
        }
        return result;
      }
    }
    private static Expr<T> Add(Expr<T> a, Expr<T> b) {
      return NumericOperators0<T, double>.Add.Instance.Make(a, b);
    }
    private static Expr<T> Sub(Expr<T> a, Expr<T> b) {
      return NumericOperators0<T, double>.Subtract.Instance.Make(a, b);
    }
    private static Expr<T> Mult(Expr<T> a, Expr<T> b) {
      return NumericOperators0<T, double>.Multiply.Instance.Make(a, b);
    }
    private static Expr<double> Add(Expr<double> a, Expr<double> b) {
      return NumericOperators0<double, double>.Add.Instance.Make(a, b);
    }
    private static Expr<double> Sub(Expr<double> a, Expr<double> b) {
      return NumericOperators0<double, double>.Subtract.Instance.Make(a, b);
    }
    private static Expr<double> Mult(Expr<double> a, Expr<double> b) {
      return NumericOperators0<double, double>.Multiply.Instance.Make(a, b);
    }

    public partial class Cross : Math2Operator<Cross> {
      private Cross() { }
      public new static readonly Cross Instance = new Cross();
      protected override string CallName { get { return "Cross"; } }
      public override OperatorX<double, double, double> Uniform { get { return DoubleOperators0<double>.Cross.Instance; } }
      public override Expr<T> Expand(Expr<T> argA, Expr<T> argB) {
        if (false) {
          var v = argA;
          var w = argB;
          int I = 1;
          Expr<double>[] Cs = new Expr<double>[Arity<double, T>.ArityI.Count];
          for (int i = 0; i < Arity<double, T>.ArityI.Count; i++) {
            var j0 = (I + i + 0) % Arity<double, T>.ArityI.Count; // i = 0, j0 = 1
            var j1 = (i + i + 1) % Arity<double, T>.ArityI.Count; // i = 0, j1 = 2
            var t0A = NumericOperators0<double, double>.Multiply.Instance.Make(Arity<double, T>.ArityI.Access(v, j0), Arity<double, T>.ArityI.Access(w, j1));
            var t0B = NumericOperators0<double, double>.Multiply.Instance.Make(Arity<double, T>.ArityI.Access(w, j0), Arity<double, T>.ArityI.Access(v, j1));
            Cs[i] = NumericOperators0<double, double>.Subtract.Instance.Make(t0A, t0B);
          }
          return Arity<double, T>.ArityI.Make(Cs);
        } else {
          (Arity<double, T>.ArityI.Count == 3).Assert();
          var a = argA;
          var b = argB;

          var a1 = Arity<double, T>.ArityI.Access(a, 0);
          var a2 = Arity<double, T>.ArityI.Access(a, 1);
          var a3 = Arity<double, T>.ArityI.Access(a, 2);

          var b1 = Arity<double, T>.ArityI.Access(b, 0);
          var b2 = Arity<double, T>.ArityI.Access(b, 1);
          var b3 = Arity<double, T>.ArityI.Access(b, 2);

          var ret = new Expr<double>[3];
          // a × b = (a2b3 − a3b2, a3b1 − a1b3, a1b2 − a2b1).
          ret[0] = Sub(Mult(a2, b3), Mult(a3, b2)); // a2b3 − a3b2
          ret[1] = Sub(Mult(a3, b1), Mult(a1, b3)); // a3b1 − a1b3
          ret[2] = Sub(Mult(a1, b2), Mult(a2, b1)); // a1b2 − a2b1
          return Arity<double, T>.ArityI.Make(ret);



        }
      }
    }

  }
  public partial class Sign : StaticCallOperator<double, double, Sign> {
    private Sign() { }
    public new static readonly Sign Instance = new Sign();
    protected override string CallName {
      get { return "Sign"; }
    }
    protected override Type TargetType {
      get { return typeof(Math); }
    }
    public override Expr<double> Expand(Expr<double> argA) {
      var d = argA.Bl();
      return (d < 0d).Condition(-1, (d > 0d).Condition(+1, (d == 0d).Condition(0, d)));
    }
  }

  public static partial class DoubleOperators<T, B> {
    public abstract partial class DoubleOperator<CNT> : StaticCallOperator<T, B, CNT> where CNT : DoubleOperator<CNT> {
      public abstract IOperatorX<double, bool> Uniform { get; }
      protected override Type TargetType { get { return typeof(double); } }
      public override Expr<B> Expand(Expr<T> argA) {
        if (Arity<double,T>.ArityI.Count == 1) return base.Expand(argA);
        var Cs = new Expr<bool>[Arity<double,T>.ArityI.Count];
        for (int i = 0; i < Arity<double,T>.ArityI.Count; i++)
          Cs[i] = Uniform.Make(Arity<double,T>.ArityI.Access(argA, i));
        return Arity<bool,B>.ArityI.Make(Cs);
      }
    }
    public partial class IsInfinity : DoubleOperator<IsInfinity> {
      public new static readonly IsInfinity Instance = new IsInfinity();
      private IsInfinity() { }
      public override IOperatorX<double, bool> Uniform { get { return DoubleOperators<double, bool>.IsInfinity.Instance; } }
      protected override string CallName { get { return "IsInfinity"; } }
    }
    public partial class IsNaN : DoubleOperator<IsNaN> {
      public new static readonly IsNaN Instance = new IsNaN();
      private IsNaN() { }
      public override IOperatorX<double, bool> Uniform { get { return DoubleOperators<double, bool>.IsNaN.Instance; } }
      protected override string CallName { get { return "IsNaN"; } }
    }
    public partial class IsPositiveInfinity : DoubleOperator<IsPositiveInfinity> {
      public new static readonly IsPositiveInfinity Instance = new IsPositiveInfinity();
      private IsPositiveInfinity() { }
      public override IOperatorX<double, bool> Uniform { get { return DoubleOperators<double, bool>.IsPositiveInfinity.Instance; } }
      protected override string CallName { get { return "IsPositiveInfinity"; } }
      public override Expr<B> Expand(Expr<T> argA) {
        return BoolOperators<B>.And.Instance.Make(IsInfinity.Instance.Make(argA),
               NumericOperators<T, double, B>.GreaterThanOrEqual.Instance.Make(argA, Arity<double,T>.ArityI.Repeat(0d)));  
      }
    }
    public partial class IsNegativeInfinity : DoubleOperator<IsNegativeInfinity> {
      public new static readonly IsNegativeInfinity Instance = new IsNegativeInfinity();
      private IsNegativeInfinity() { }
      public override IOperatorX<double, bool> Uniform { get { return DoubleOperators<double, bool>.IsNegativeInfinity.Instance; } }
      protected override string CallName { get { return "IsNegativeInfinity"; } }
      public override Expr<B> Expand(Expr<T> argA) {
        return BoolOperators<B>.And.Instance.Make(IsInfinity.Instance.Make(argA),
               NumericOperators<T, double, B>.LessThan.Instance.Make(argA, Arity<double,T>.ArityI.Repeat(0d)));
      }
    }
    public partial class Smoothstep : StaticCallOperator<T, T, T, T, Smoothstep> {
      private Smoothstep() { }
      public new static readonly Smoothstep Instance = new Smoothstep();
      protected override string CallName { get { return "Smoothstep"; } }
      protected override Type TargetType { get { return typeof(Math); } }
      public override Expr<T> Expand(Expr<T> argA, Expr<T> argB, Expr<T> argC) {
        var a = argA;
        var b = argB;
        var x = argC;
        var t0 = NumericOperators<T,double,B>.LessThan.Instance.Make(x, a);
        var t1 = NumericOperators<T, double, B>.GreaterThanOrEqual.Instance.Make(x, b);

        var x_a_b_a = NumericOperators0<T, double>.Divide.Instance.Make(
          NumericOperators0<T, double>.Subtract.Instance.Make(x, a), 
          NumericOperators0<T, double>.Subtract.Instance.Make(b, a));
        x = x_a_b_a;

        var x_x = NumericOperators0<T, double>.Multiply.Instance.Make(x, x);
        var n3 = Arity<double,T>.ArityI.Repeat(3);
        var n3_2_x = NumericOperators0<T, double>.Subtract.Instance.Make(n3,
          NumericOperators0<T, double>.Multiply.Instance.Make(Arity<double,T>.ArityI.Repeat(2), x_x));
        Expr<T> x_x_n3_2_x = NumericOperators0<T, double>.Multiply.Instance.Make(x_x, n3);
        var Cs = new Expr<double>[Arity<double,T>.ArityI.Count];
        for (int i = 0; i < Arity<double, T>.ArityI.Count; i++) {
          var cA = ValueOperators<double>.Condition.Instance.Make(Arity<bool,B>.ArityI.Access(t1, i), 
                                   new Constant<double>(1d), Arity<double,T>.ArityI.Access(x_x_n3_2_x, i));
          Cs[i] = ValueOperators<double>.Condition.Instance.Make(Arity<bool,B>.ArityI.Access(t0, i), 
            new Constant<double>(0), cA);
        }
        return Arity<double,T>.ArityI.Make(Cs);
      }

    }

  }
  public partial interface ITupleConstruct<K, T, CNT> : IOperator<T, CNT> where CNT : ITupleConstruct<K,T,CNT> {
  }
  public partial class TupleConstruct<K, S, T, U> : Operator<S, T, U, TupleConstruct<K, S,T,U>>, 
    ITupleConstruct<K,U,TupleConstruct<K,S,T,U>> {
    public new static TupleConstruct<K, S, T, U> Instance = new TupleConstruct<K, S, T, U>();
    private TupleConstruct() {
      (Arity<K, S>.ArityI.Count + Arity<K, T>.ArityI.Count == Arity<K, U>.ArityI.Count).Assert();
    }
    public override string Format(string argA, string argB) {
      return typeof(K).Name +　"(" + argA + ", " + argB + ")";
    }
    public override Expr<U> Expand(Expr<S> argA, Expr<T> argB) {
      var countS = Arity<K, S>.ArityI.Count;
      var countT = Arity<K, T>.ArityI.Count;
      var Es = new Expr<K>[countS + countT];
      for (int i = 0; i < countS; i++)
        Es[i] = Arity<K, S>.ArityI.Access(argA, i);
      for (int i = 0; i < countT;  i++)
        Es[i + countS] = Arity<K, T>.ArityI.Access(argB, i);

      return Arity<K, U>.ArityI.Make(Es);
    }
  }

  public partial class Swizzle<K, S, T> : BaseOperatorX<S, T> {
    private readonly int[] Indices;

    public Swizzle(params int[] Indices) {
      (Arity<K, T>.ArityI.Count == Indices.Length).Assert();
      (Indices.Length > 1).Assert();
      this.Indices = Indices;
    }
    public override int GetHashCode() {
      var hc = typeof(Swizzle<K,S,T>).GetHashCode();
      foreach (var i in Indices) hc += i;
      return hc;
    }
    public override bool Equals(object obj) {
      if (obj is Swizzle<K, S, T>) {
        var obj0 = (Swizzle<K, S, T>)obj;
        for (int i = 0; i < Indices.Length; i++)
          if (Indices[i] != obj0.Indices[i]) return false;
        return true;
      }
      return base.Equals(obj);
    }
    public override string ToString() {
      return "Swizzle" + Indices.ToList() + "";
    }
    public override string Format(string s) {
      Func<int, string> ToS = (idx) => {
        switch (idx) {
          case 0: return "x";
          case 1: return "y";
          case 2: return "z";
          case 3: return "w";
          default: return "c" + idx;
        }
      };
      var str = s + ".";
      foreach (var i in Indices) str = str + ToS(i);
      return str;
    }
    public override Expr<T> Expand(Expr<S> argA) {
      var Es = new Expr<K>[Indices.Length];
      for (int i = 0; i < Indices.Length; i++)
        Es[i] = Arity<K, S>.ArityI.Access(argA, Indices[i]);
      return Arity<K, T>.ArityI.SwizzleExpand(Es);
    }
    public override Expr<S> Inverse(Expr<S> argA, Expr<T> result) {
      var Es = new Expr<K>[Arity<K,S>.ArityI.Count];
      for (int i = 0; i < Indices.Length; i++) {
        Es[Indices[i]] = Arity<K,T>.ArityI.Access(result, i);
      }
      for (int i = 0; i < Es.Length; i++)
        if (Es[i] == null) Es[i] = Arity<K, S>.ArityI.Access(argA, i);

      return Arity<K, S>.ArityI.Make(Es);
    }
  }


  public interface Operation : IExpr {
    IOperator BaseBaseOp { get; }
    Expr this[int Idx] { get; }
  }
  public abstract partial class Operation<S> : Expr<S>, Operation {
    public abstract IOperator<S> BaseOp { get; }
    public IOperator BaseBaseOp { get { return BaseOp; } }
    public abstract Expr this[int Idx] { get; }
    public abstract Expr<S> Expanded { get; }
    internal Expr<S> Expand0 {
      get {
        var f = Expanded;
        if (f == null) 
          throw new NotSupportedException();
        return f;
      }
    }
    internal static readonly Dictionary<IOperator, object> FCache = new Dictionary<IOperator, object>();
    internal static TDelegate Find<T, TDelegate>(IBaseOperator<T, TDelegate> op) {
      if (FCache.ContainsKey(op)) return (TDelegate) FCache[op];
      return default(TDelegate);
    }

  }
  public abstract partial class BaseOperation<S, OPR> : Operation<S> where OPR : IOperator<S> {
    public OPR Op { get; internal set; }
    public override IOperator<S> BaseOp { get { return Op; } }
  }


  public partial class Operation<S, T> : BaseOperation<T, IOperatorX<S,T>> {
    public Expr<S> ArgA { get; internal set; }
    public override Expr this[int Idx] {
      get { return ArgA; }
    }
    public override bool IsConstant {
      get {
        return ArgA.IsConstant;
      }
    }
    protected override string ToString1() {
      return Op.Format(ArgA.ToString());
    }
    protected override int GetHashCode0() {
      return Op.GetHashCode() + ArgA.GetHashCode();
    }
    protected override bool Equals0(Expr<T> obj) {
      if (!(obj is Operation<S, T>)) return false;
      var op = (Operation<S, T>)obj;
      return Op.Equals(op.Op) && ArgA.Equals(op.ArgA);
    }
    public override Expr<T> Expanded { get { return Op.Expand(ArgA); } }
    protected override Eval<EVAL>.Info<T> Eval<EVAL>(Eval<EVAL> txt) {
      return Op.Eval(this, txt);
    }
    public override bool Solve(IAssign P, Expr<T> other) {
      Expr<S> newRHS = Op.Inverse(ArgA, other);
      if (newRHS != null && ArgA.Solve(P, newRHS)) return true;
      var f = Expanded;
      return f == null ? false : f.Solve(P, other);
    }
  }
  public partial class Operation<S, T, U> : BaseOperation<U, OperatorX<S, T, U>> {
    public Expr<S> ArgA { get; internal set; }
    public Expr<T> ArgB { get; internal set; }
    public override Expr this[int Idx] {
      get { return Idx == 0 ? ArgA : ((Expr) ArgB); }
    }
    public override bool IsConstant {
      get {
        return ArgA.IsConstant && ArgB.IsConstant;
      }
    }
    protected override string ToString1() {
      return Op.Format(ArgA.ToString(), ArgB.ToString());
    }
    protected override int GetHashCode0() {
      return Op.GetHashCode() + ArgA.GetHashCode() + ArgB.GetHashCode();
    }
    protected override bool Equals0(Expr<U> obj) {
      if (!(obj is Operation<S, T, U>)) return false;
      var op = (Operation<S, T, U>)obj;
      return Op.Equals(op.Op) && ArgA.Equals(op.ArgA) && ArgB.Equals(op.ArgB);
    }

    public override Expr<U> Expanded { get { return Op.Expand(ArgA, ArgB); } }
    protected override Eval<EVAL>.Info<U> Eval<EVAL>(Eval<EVAL> txt) {
      return Op.Eval(this, txt);
    }
    public override bool Solve(IAssign P, Expr<U> other) {
      {
        Expr<S> newRHS = Op.InverseA(ArgA, ArgB, other);
        if (newRHS != null && ArgA.Solve(P, newRHS)) return true;
      }
      {
        Expr<T> newRHS = Op.InverseB(ArgA, ArgB, other);
        if (newRHS != null && ArgB.Solve(P, newRHS)) return true;
      }
      var f = Expanded;
      return f == null ? false : f.Solve(P, other);
    }
  }


  public partial class Operation<S, T, U, V> : BaseOperation<V, OperatorX<S, T, U, V>> {
    public Expr<S> ArgA { get; internal set; }
    public Expr<T> ArgB { get; internal set; }
    public Expr<U> ArgC { get; internal set; }

    public override bool IsConstant {
      get {
        return ArgA.IsConstant && ArgB.IsConstant && ArgC.IsConstant;
      }
    }
    public override Expr this[int Idx] {
      get { return Idx == 0 ? ArgA : Idx == 1 ? ArgB : ((Expr) ArgC); }
    }
    protected override string ToString1() {
      return Op.Format(ArgA.ToString(), ArgB.ToString(), ArgC.ToString());
    }
    protected override int GetHashCode0() {
      return Op.GetHashCode() + ArgA.GetHashCode() + ArgB.GetHashCode() + ArgC.GetHashCode();
    }
    protected override bool Equals0(Expr<V> obj) {
      if (!(obj is Operation<S, T, U, V>)) return false;
      var op = (Operation<S, T, U, V>)obj;
      return Op.Equals(op.Op) && ArgA.Equals(op.ArgA) && ArgB.Equals(op.ArgB) && ArgC.Equals(op.ArgC); ;
    }
    public override Expr<V> Expanded { get { return Op.Expand(ArgA, ArgB, ArgC); } }
    protected override Eval<EVAL>.Info<V> Eval<EVAL>(Eval<EVAL> txt) {
      return Op.Eval(this, txt);
    }
    public override bool Solve(IAssign P, Expr<V> other) {
      {
        Expr<S> newRHS = Op.InverseA(ArgA, ArgB, ArgC, other);
        if (newRHS != null && ArgA.Solve(P, newRHS)) return true;
      }
      {
        Expr<T> newRHS = Op.InverseB(ArgA, ArgB, ArgC, other);
        if (newRHS != null && ArgB.Solve(P, newRHS)) return true;
      }
      {
        Expr<U> newRHS = Op.InverseC(ArgA, ArgB, ArgC, other);
        if (newRHS != null && ArgC.Solve(P, newRHS)) return true;
      }
      var f = Expanded;
      return f == null ? false : f.Solve(P, other);
    }
  }
  public partial class Operation<S, T, U, V, W> : BaseOperation<W, OperatorX<S, T, U, V, W>> {
    public Expr<S> ArgA { get; internal set; }
    public Expr<T> ArgB { get; internal set; }
    public Expr<U> ArgC { get; internal set; }
    public Expr<V> ArgD { get; internal set; }
    public override bool IsConstant {
      get {
        return ArgA.IsConstant && ArgB.IsConstant && ArgC.IsConstant && ArgD.IsConstant;
      }
    }
    public override Expr this[int Idx] {
      get { return Idx == 0 ? ArgA : Idx == 1 ? ArgB : Idx == 2 ? ArgC : ((Expr) ArgD); }
    }
    protected override string ToString1() {
      return Op.Format(ArgA.ToString(), ArgB.ToString(), ArgC.ToString(), ArgD.ToString());
    }
    protected override int GetHashCode0() {
      return Op.GetHashCode() + ArgA.GetHashCode() + ArgB.GetHashCode() + ArgC.GetHashCode() + ArgD.GetHashCode();
    }
    protected override bool Equals0(Expr<W> obj) {
      if (!(obj is Operation<S, T, U, V, W>)) return false;
      var op = (Operation<S, T, U, V, W>)obj;
      return Op.Equals(op.Op) && ArgA.Equals(op.ArgA) && ArgB.Equals(op.ArgB) && ArgC.Equals(op.ArgC) && ArgD.Equals(op.ArgD); ;
    }
    public override Expr<W> Expanded { get { return Op.Expand(ArgA, ArgB, ArgC, ArgD); } }
    protected override Eval<EVAL>.Info<W> Eval<EVAL>(Eval<EVAL> txt) {
      return Op.Eval(this, txt);
    }
    public override bool Solve(IAssign P, Expr<W> other) {
      {
        Expr<S> newRHS = Op.InverseA(ArgA, ArgB, ArgC, ArgD, other);
        if (newRHS != null && ArgA.Solve(P, newRHS)) return true;
      }
      {
        Expr<T> newRHS = Op.InverseB(ArgA, ArgB, ArgC, ArgD, other);
        if (newRHS != null && ArgB.Solve(P, newRHS)) return true;
      }
      {
        Expr<U> newRHS = Op.InverseC(ArgA, ArgB, ArgC, ArgD, other);
        if (newRHS != null && ArgC.Solve(P, newRHS)) return true;
      }
      {
        Expr<V> newRHS = Op.InverseD(ArgA, ArgB, ArgC, ArgD, other);
        if (newRHS != null && ArgD.Solve(P, newRHS)) return true;
      }
      var f = Expanded;
      return f == null ? false : f.Solve(P, other);
    }
  }
}
