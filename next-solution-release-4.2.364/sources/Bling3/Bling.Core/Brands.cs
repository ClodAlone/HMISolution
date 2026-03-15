using System;
using System.Collections.Generic;
using System.Collections;
using linq = Microsoft.Linq.Expressions;
using Bling.Util;


namespace Bling.Core {
  using Bling.DSL;
  using Bling.Vecs;
  using Bling.Ops;
  using Bling.Semantics;

  /// <summary>
  /// Interface type for brand without underlying or self type.
  /// </summary>
  public partial interface IBrand {
    /// <summary>
    /// Coerce this brand to another.
    /// </summary>
    /// <typeparam name="BRAND0">Type to coerce to.</typeparam>
    /// <returns>Coerced brand.</returns>
    BRAND0 Coerce<BRAND0>() where BRAND0 : Brand<BRAND0>;
    /// <summary>
    /// Reveal the self and underlying type of this brand via a visitor.
    /// </summary>
    /// <typeparam name="R">Type to be returned by visitor.</typeparam>
    /// <param name="Reveal">Reveal type visitor.</param>
    /// <returns>Value returned by visitor</returns>
    R Reveal<R>(BrandTypeReveal<R> Reveal);
    /// <summary>
    /// Get the underlying expression of this brand.
    /// </summary>
    Expr Underlying { get; }
  }

  /// <summary>
  /// Interface type for brand with self type but without underlying type.
  /// </summary>
  /// <typeparam name="BRAND">Self type of brand.</typeparam>
  public partial interface IBrand<BRAND> : IBrand where BRAND : IBrand<BRAND> {
    /// <summary>
    /// Convert to brand type.
    /// </summary>
    BRAND AsBrand { get; }
    /// <summary>
    /// Bind this brand to another, brand is defined according to the underlying expression.
    /// </summary>
    BRAND Bind { get; set; }

    BRAND TwoWay { get; set; }

    /// <summary>
    /// Like bind, but with immediate semantics, defined according to the underlying expression.
    /// </summary>
    BRAND Now { get; set; }
  }

  public static class CanWrap<BRAND, T> where BRAND : ICanWrap<BRAND, T> {
    public static Func<Expr<T>, BRAND> To { get; set; }
    public static Func<BRAND, Expr<T>> From { get; set; }
  }
  public partial interface ICanWrap<BRAND, T> : IBrand<BRAND> where BRAND : IBrand<BRAND> { }

  /// <summary>
  /// Interface type for brand with underlying type but without self type.
  /// </summary>
  /// <typeparam name="T">Underlying type.</typeparam>
  public partial interface IBrandT<T> : IBrand {
    /// <summary>
    /// Get the underlying expression of this brand.
    /// </summary>
    new Expr<T> Underlying { get; }
  }
  public partial interface IBrand<T, BRAND> : IBrand<BRAND>, IBrandT<T> where BRAND : IBrand<T, BRAND> { }

  /// <summary>
  /// Visitor that reveals the type of a brand.
  /// </summary>
  /// <typeparam name="R">Type of value returned by visitor.</typeparam>
  public interface BrandTypeReveal<R> {
    /// <summary>
    /// Visit method that reveals underlying and self types of brand.
    /// </summary>
    /// <typeparam name="T">Underlying type</typeparam>
    /// <typeparam name="BRAND">Self type</typeparam>
    /// <param name="value">Brand being visited</param>
    /// <returns>Result of visit</returns>
    R Visit<T, BRAND>(Brand<T, BRAND> value) where BRAND : Brand<T, BRAND>;
  }
  /// <summary>
  /// Can be used as a condition of a target.
  /// </summary>
  /// <typeparam name="SELF">Self type of value that can be used as a condition target.</typeparam>
  public interface IConditionTarget<SELF> {
    /// <summary>
    /// Core of a condition. This expression is used as a the if true clause.
    /// </summary>
    /// <param name="Test">Test that determines what clause is chosen</param>
    /// <param name="IfFalse">Clause that is chosen if test is false</param>
    /// <returns>Either this or IfFalse, depending on Test</returns>
    SELF Choose(BoolBl Test, SELF IfFalse);
    SELF DChoose(BoolBl Test, SELF IfFalse);
  }
  public interface ICanTable<SELF> {
    SELF Table(IntBl Index, SELF[] Cases);
  }

  /// <summary>
  /// Specified that this brand extends the brand TO.
  /// </summary>
  /// <typeparam name="TO">A brand type extended by implementor of this interface</typeparam>
  public partial interface IExtends<TO> : IBrand where TO : Brand<TO> { }
  public partial interface IExtends<BRAND, TO> : IExtends<TO>
    where BRAND : Brand<BRAND>
    where TO : Brand<TO> { }

  public partial interface BrandTypeRevealS<R> {
    R Visit<T0, BRAND0>() where BRAND0 : Brand<T0,BRAND0>;
  }

  /// <summary>
  /// Base brand class without specified self or underlying types.
  /// </summary>
  public abstract partial class Brand : IBrand {
    internal interface BrandTypeRevealT {
      object Visit<T0, BRAND0>() where BRAND0 : Brand<T0, BRAND0>;
    }


    internal static readonly Dictionary<Type, Func<Expr, Brand>> ToBrands = new Dictionary<Type, Func<Expr, Brand>>();
    /// <summary>
    /// Create a brand (type unspecified) for an underlying expression.
    /// </summary>
    /// <param name="e">Underlying expression</param>
    /// <returns>A brand for the underlying expression</returns>
    public static Brand ToBrand(Expr e) { return ToBrands[e.TypeOfT](e); }
    public static Func<Expr,Brand> ToBrand(Type BrandType) {
      //if (ToBrands.ContainsKey(e.TypeOfT)) return ToBrands[e.TypeOfT](e);
      var TypeOfT = Brand.TypeOfT(BrandType);
      var constructor = BrandType.GetConstructor(new Type[] { typeof(Expr<>).MakeGenericType(TypeOfT) });
      (constructor != null).Assert();
      return e => (Brand) constructor.Invoke(new object[] { e });
    }
    /// <summary>
    /// Get the underlying expression of this brand.
    /// </summary>
    protected abstract Expr BaseUnderlying { get; }

    public Expr Underlying { get { return BaseUnderlying; } }


    /// <summary>
    /// Coerce this brand to another.
    /// </summary>
    /// <typeparam name="BRAND0">Type to coerce to.</typeparam>
    /// <returns>Coerced brand.</returns>
    public abstract BRAND0 Coerce<BRAND0>() where BRAND0 : Brand<BRAND0>;
    /// <summary>
    /// Reveal the self and underlying type of this brand via a visitor.
    /// </summary>
    /// <typeparam name="R">Type to be returned by visitor.</typeparam>
    /// <param name="Reveal">Reveal type visitor.</param>
    /// <returns>Value returned by visitor</returns>
    public abstract R Reveal<R>(BrandTypeReveal<R> Reveal);

    public static Type TypeOfT(Type Type) {
      var Type0 = (Type)typeof(Brand<>).MakeGenericType(Type);
      Type0 = (Type)Type0.GetProperty("TypeOfT").GetAccessors()[0].Invoke(null, new object[0]);
      return Type0;
    }
  }


  /// <summary>
  /// Base brand class with specified self type but without underlying type.
  /// </summary>
  public abstract partial class Brand<BRAND> : Brand, IBrand<BRAND>, IConditionTarget<BRAND>, ICanTable<BRAND> where BRAND : Brand<BRAND> {
    private static Type TypeOfT0;


    public new static Type TypeOfT {
      get {
        if (TypeOfT0 == null) {
          // blah, use reflection to initialize it!
          var tpe = typeof(BRAND);
          while (!tpe.IsGenericType || tpe.GetGenericTypeDefinition() != typeof(Brand<,>))
            tpe = tpe.BaseType;
          var TypeOfT = tpe.GetGenericArguments()[0];
          var value = typeof(Constant<>).MakeGenericType(TypeOfT).GetConstructor(new Type[0]).Invoke(new object[0]);

          value = typeof(BRAND).GetConstructor(new Type[] {
          typeof(Expr<>).MakeGenericType(TypeOfT),
          }).Invoke(new object[] { value });
          (TypeOfT0 == TypeOfT).Assert();
        }
        return TypeOfT0;
      }
      internal set {
        TypeOfT0 = value;
      }
    }
   

    protected static Func<Expr, BRAND> BaseToBrand { set; private get; }

    public new static BRAND ToBrand(Expr e) { return BaseToBrand(e); }


    public abstract BRAND Table(IntBl Index, BRAND[] Cases);
    private class BrandTypeRevealU<R> : BrandTypeRevealT {
      internal BrandTypeRevealS<R> V;
      public object Visit<T0, BRAND0>() where BRAND0 : Brand<T0, BRAND0> { return V.Visit<T0, BRAND0>(); }
    }

    public static R RevealType<R>(BrandTypeRevealS<R> visitor) {
      if (RevealTypeF == null) {
        typeof(BRAND).GetConstructor(new Type[0]).Invoke(new object[0]);
      }
      return (R)RevealTypeF(new BrandTypeRevealU<R>() { V = visitor }); 
    }
    static internal Func<BrandTypeRevealT, object> RevealTypeF { get; set; }

    /// <summary>
    /// Convert to brand type.
    /// </summary>
    public BRAND AsBrand { get { return (BRAND)((object)this); } }
    public static implicit operator BRAND(Brand<BRAND> brand) { return brand.AsBrand; }
    /// <summary>
    /// Convert array of brands into a table expression. 
    /// </summary>
    public static Func<BRAND[], IntBl, BRAND> DoTable { protected set; get; }
    /// <summary>
    /// Bind this brand to another, brand is defined according to the underlying expression.
    /// </summary>
    public abstract BRAND Bind { get; set; }
    public abstract BRAND TwoWay { get; set; }

    /// <summary>
    /// Like bind, but with immediate semantics, defined according to the underlying expression.
    /// </summary>
    public abstract BRAND Now { get; set; }
    internal interface WithTServices {
      BRAND CoerceFrom<T0, BRAND0>(BRAND0 value) where BRAND0 : Brand<T0, BRAND0>;
      BRAND DownCastFrom<T0, BRAND0>(BRAND0 value) where BRAND0 : Brand<T0, BRAND0>, IExtends<BRAND0,BRAND>;
    }
    internal static Func<BRAND[], IntBl, BRAND> TableF { get; set; }

    internal static WithTServices TServices { private get; set; }
    internal static BRAND CoerceFrom<T0, BRAND0>(BRAND0 value) where BRAND0 : Brand<T0, BRAND0> {
      return TServices.CoerceFrom<T0, BRAND0>(value);
    }
    internal static BRAND DownCastFrom<T0, BRAND0>(BRAND0 value) where BRAND0 : Brand<T0, BRAND0>, IExtends<BRAND0, BRAND> {
      return TServices.DownCastFrom<T0, BRAND0>(value);
    }

    /// <summary>
    /// Brand with underlying default value expression.
    /// </summary>
    public static BRAND Default { get; internal set; }
    /// <summary>
    /// To enable conditions.
    /// </summary>
    public abstract BRAND Choose(BoolBl Test, BRAND IfFalse);
    public abstract BRAND DChoose(BoolBl Test, BRAND IfFalse);
    /// <summary>
    /// Brand with underlying parameter expression.
    /// </summary>
    public static Func<object, BRAND> Param { get; internal set; }
  }
  public partial interface ICoreBrandT : IBrand { }


  public struct BrandId<T, BRAND> where BRAND : Brand<T, BRAND> { }

  /// <summary>
  /// Brand class that specified both self and underlying types.
  /// </summary>
  /// <typeparam name="T">Type of underlying expression</typeparam>
  /// <typeparam name="BRAND">Self type</typeparam>
  public abstract partial class Brand<T, BRAND> : Brand<BRAND>, IBrandT<T>, IExtends<BRAND, ObjectBl>, IBrand<T,BRAND>, ICanWrap<BRAND,T> where BRAND : Brand<T, BRAND> {

    public override BRAND Table(IntBl Index, BRAND[] Cases) {
      var Es = new Expr<T>[Cases.Length];
      for (int i = 0; i < Es.Length; i++) Es[i] = Cases[i];
      return ToBrand(new TableExpr<T>(Es, Index));
    }

    public static BrandId<T, BRAND> Id { get { return new BrandId<T, BRAND>(); } }
    /// <summary>
    /// Factory that converts underlying expression into brand.
    /// </summary>
    public new static Func<Expr<T>, BRAND> ToBrand {
      get {
        if (ToBrand0 == null) {
          var m = typeof(BRAND).GetConstructor(new Type[0]);
          (m != null).Assert();
          m.Invoke(new object[0]);
        }
        return ToBrand0;
      }
      protected set {
        (ToBrand0 == null).Assert();
        ToBrand0 = value; 
      }
    }
    private static Func<Expr<T>, BRAND> ToBrand0 = null;
    /// <summary>
    /// Underlying expression
    /// </summary>
    public new virtual Expr<T> Underlying { get { return Underlying0; } }

    private readonly Expr<T> Underlying0;


    public bool IsConstant { get { return Underlying.IsConstant; } }

    /// <summary>
    /// Underlying expression
    /// </summary>
    protected override Expr BaseUnderlying { get { return Underlying; } }
    public Brand(Expr<T> Underlying) { this.Underlying0 = Underlying; }
    public Brand() : this(new Constant<T>()) { }
    public override string ToString() { return Underlying.ToString(); }
    private static void RegisterT() {
      (!ToBrands.ContainsKey(typeof(T))).Assert();
      ToBrands[typeof(T)] = e => ToBrand((Expr<T>)e);

    }
    protected static void Register(Func<Expr<T>, BRAND> F) {
      //(ToBrand == null).Assert();
      ToBrand = F;
      var Default0 = F(new Constant<T>(default(T)));
      if (((object)Default) == null)
        Default = Default0;
      if (typeof(BRAND).IsSubclassOf(typeof(ICoreBrandT)))
        RegisterT();
    }
    public override R Reveal<R>(BrandTypeReveal<R> Reveal) {
      return Reveal.Visit(this);
    }
    public override BRAND Choose(BoolBl Test, BRAND IfFalse) {
      return ToBrand(ValueOperators<T>.Condition.Instance.Make(Test, this, IfFalse));
    }
    public override BRAND DChoose(BoolBl Test, BRAND IfFalse) {
      return ToBrand(ValueOperators<T>.DynamicCondition.Instance.Make(Test, this, IfFalse));
    }
    /// <summary>
    /// Create a string representation of this brand in expression form (result is also a brand).
    /// </summary>
    public virtual StringBl ToStringBl() {
      return ObjectBl.UpCast(this).ToStringBl();
    }
    public static implicit operator StringBl(Brand<T,BRAND> Value) {
      return Value.ToStringBl();
    }
    public static implicit operator ObjectBl(Brand<T, BRAND> Value) {
      return ObjectBl.UpCast(Value);
    }
    /// <summary>
    /// Bind this brand to another, brand is defined according to the underlying expression.
    /// </summary>
    public override BRAND Bind {
      get { return this; }
      set { Underlying.Bind = value.Underlying; }
    }

    public override BRAND TwoWay {
      get { return this; }
      set { Underlying.TwoWay = value.Underlying; }
    }

    /// <summary>
    /// Like bind, but with immediate semantics, defined according to the underlying expression.
    /// </summary>
    public override BRAND Now {
      get { return ToBrand(new NowExpr<T>(Underlying)); }
      set {
        var assign = new NowAssign<Action>();
        if (Underlying.Solve(assign, value)) {
          foreach (var r in assign.Results) r();
        } else throw new NotSupportedException();
      }
    }
    /// <summary>
    /// Creates a function that executes the assignment LHS = RHS.
    /// </summary>
    /// <param name="LHS">The Bl value being assigned to.</param>
    /// <param name="RHS">The Bl value being assigned from.</param>
    /// <returns>Function that executes the assignment.</returns>
    public static Action Assign(BRAND LHS, BRAND RHS) {
      var assign = new NowAssign<Action>();
      if (!LHS.Underlying.Solve(assign, RHS)) throw new NotSupportedException();
      var list = assign.Results;
      if (list.Count == 1) {
        return list[0];
      } else return () => {
        foreach (var n in list) n();
      };
    }
    public static Action Assign(BRAND LHS, Func<BRAND> RHS) {
      return Assign(LHS, RHS());
    }
    /// <summary>
    /// Creates a one argument function that executes the assignment s => LHS(s) = RHS(s), 
    /// where s is bound to the specified argument.
    /// </summary>
    /// <typeparam name="S">Type of the argument.</typeparam>
    /// <param name="LHS">Expression being assigned to.</param>
    /// <param name="RHS">Expression being assigned from.</param>
    /// <returns>Function that executes the assignment.</returns>
    public static Action<S> Assign<S>(Func<Expr<S>, BRAND> LHS, Func<Expr<S>, BRAND, BRAND> RHS) {
      var ArgS = new ParameterExpr<S>("ArgS");
      var assign = new NowAssign<Action<S>>(ArgS);
      var LHS0 = LHS(ArgS);
      var RHS0 = RHS(ArgS, LHS0);
      if (!LHS0.Underlying.Solve(assign, RHS0)) throw new NotSupportedException();
      var list = assign.Results;
      // flatten the list!
      if (list.Count == 0) throw new NotSupportedException();
      else if (list.Count == 1) return list[0];
      else return arg => {
        foreach (var n in list) n(arg);
      };
    }
    public Action<T> MakeAssign {
      get {
        return Assign<T>((t) => this, (t, lhs) => ToBrand(t));

      }
    }

    /// <summary>
    /// Creates a two argument function that executes the assignment s,u => LHS(s,u) = RHS(s,u), 
    /// where s,u are bound to specified arguments.
    /// </summary>
    /// <typeparam name="S">Type of the first argument.</typeparam>
    /// <typeparam name="U">Type of the second argument.</typeparam>
    /// <param name="LHS">Expression being assigned to.</param>
    /// <param name="RHS">Expression being assigned from.</param>
    /// <returns>Function that executes the assignment.</returns>
    public static Action<S, U> Assign<S, U>(Func<Expr<S>, Expr<U>, BRAND> LHS, Func<Expr<S>, Expr<U>, BRAND, BRAND> RHS) {
      var ArgS = new ParameterExpr<S>("ArgS");
      var ArgU = new ParameterExpr<U>("ArgU");
      var assign = new NowAssign<Action<S, U>>(ArgS, ArgU);
      var LHS0 = LHS(ArgS, ArgU);
      var RHS0 = RHS(ArgS, ArgU, LHS0);
      if (!LHS0.Underlying.Solve(assign, RHS0)) throw new NotSupportedException();
      var list = assign.Results;
      // flatten the list!
      if (list.Count == 0) throw new NotSupportedException();
      else if (list.Count == 1) return list[0];
      else return (argA, argB) => {
        foreach (var n in list) n(argA, argB);
      };
    }
    /// <summary>
    /// Creates a three argument function that executes the assignment s,u,v => LHS(s,u,v) = RHS(s,u,v), 
    /// where s,u,v are bound to specified arguments.
    /// </summary>
    /// <typeparam name="S">Type of the first argument.</typeparam>
    /// <typeparam name="U">Type of the second argument.</typeparam>
    /// <typeparam name="V">Type of the third argument.</typeparam>
    /// <param name="LHS">Expression being assigned to.</param>
    /// <param name="RHS">Expression being assigned from.</param>
    /// <returns>Function that executes the assignment.</returns>
    public static Action<S, U, V> Assign<S, U, V>(Func<Expr<S>, Expr<U>, Expr<V>, BRAND> LHS, Func<Expr<S>, Expr<U>, Expr<V>, BRAND, BRAND> RHS) {
      var ArgS = new ParameterExpr<S>("ArgS");
      var ArgU = new ParameterExpr<U>("ArgU");
      var ArgV = new ParameterExpr<V>("ArgV");
      var assign = new NowAssign<Action<S, U, V>>(ArgS, ArgU, ArgV);
      var LHS0 = LHS(ArgS, ArgU, ArgV);
      var RHS0 = RHS(ArgS, ArgU, ArgV, LHS0);
      if (!LHS0.Underlying.Solve(assign, RHS0)) throw new NotSupportedException();
      var list = assign.Results;
      // flatten the list!
      if (list.Count == 0) throw new NotSupportedException();
      else if (list.Count == 1) return list[0];
      else return (argA, argB, argC) => {
        foreach (var n in list) n(argA, argB, argC);
      };
    }
    /// <summary>
    /// Creates a four argument function that executes the assignment s,u,v,w => LHS(s,u,v,w) = RHS(s,u,v,w), 
    /// where s,u,v are bound to specified arguments.
    /// </summary>
    /// <typeparam name="S">Type of the first argument.</typeparam>
    /// <typeparam name="U">Type of the second argument.</typeparam>
    /// <typeparam name="V">Type of the third argument.</typeparam>
    /// <typeparam name="W">Type of the fourth argument.</typeparam>
    /// <param name="LHS">Expression being assigned to.</param>
    /// <param name="RHS">Expression being assigned from.</param>
    /// <returns>Function that executes the assignment.</returns>
    public static Action<S, U, V, W> Assign<S, U, V, W>(Func<Expr<S>, Expr<U>, Expr<V>, Expr<W>, BRAND> LHS, Func<Expr<S>, Expr<U>, Expr<V>, Expr<W>, BRAND> RHS) {
      var ArgS = new ParameterExpr<S>("ArgS");
      var ArgU = new ParameterExpr<U>("ArgU");
      var ArgV = new ParameterExpr<V>("ArgV");
      var ArgW = new ParameterExpr<W>("ArgW");
      var assign = new NowAssign<Action<S, U, V,W>>(ArgS, ArgU, ArgV, ArgW);
      var RHS0 = RHS(ArgS, ArgU, ArgV, ArgW);
      var LHS0 = LHS(ArgS, ArgU, ArgV, ArgW);
      if (!LHS0.Underlying.Solve(assign, RHS0)) throw new NotSupportedException();
      var list = assign.Results;
      // flatten the list!
      if (list.Count == 0) throw new NotSupportedException();
      else if (list.Count == 1) return list[0];
      else return (argA, argB, argC, argD) => {
        foreach (var n in list) n(argA, argB, argC, argD);
      };
    }


    private new class WithTServices : Brand<BRAND>.WithTServices {
      public BRAND CoerceFrom<T0, BRAND0>(BRAND0 value) where BRAND0 : Brand<T0, BRAND0> {
        return value.Coerce<T, BRAND>();
      }
      public BRAND DownCastFrom<T0, BRAND0>(BRAND0 value) where BRAND0 : Brand<T0, BRAND0>, IExtends<BRAND0,BRAND> {
        return ToBrand(Ops.DownCastOperator1<T0, T>.Instance.Make(value));
      }
    }
    static Brand() {
      CanWrap<BRAND, T>.To = (e) => ToBrand(e);
      CanWrap<BRAND, T>.From = (b) => b.Underlying;

      BaseToBrand = (e) => ToBrand((Expr<T>)e);
      //(TypeOfT == null).Assert();
      TypeOfT = typeof(T);
      DoTable = (values, index) => {
        var values0 = new Expr<T>[values.Length];
        for (int i = 0; i < values.Length; i++) 
          values0[i] = ((Brand<T, BRAND>)values[i]).Underlying;
        return ToBrand(new TableExpr<T>(values0, index));
      };
      TServices = new WithTServices();
      //(ToBrand != null).Assert();
      //Extensions.CheckInit<BRAND>();
      (RevealTypeF == null).Assert();
      RevealTypeF = (visitor) => visitor.Visit<T, BRAND>();
      TableF = (values, index) => {
        var values0 = new Expr<T>[values.Length];
        for (int i = 0; i < values.Length; i++) values0[i] = values[i];
        return ToBrand(new TableExpr<T>(values0, index));
      };
      Param = (key) => ToBrand(new ParameterExpr<T>(key));
    }
    public static implicit operator Expr<T>(Brand<T, BRAND> b) { return b.Underlying; }
    internal BRAND0 DownCast<T0, BRAND0>()
      where T0 : T
      where BRAND0 : Brand<T0, BRAND0> {
      return Brand<T0,BRAND0>.ToBrand(DownCastOperator<T, T0>.Instance.Make(Underlying));
    }
    /// <summary>
    /// Downcast this brand to specified extending brand.
    /// </summary>
    public BRAND0 DownCast<BRAND0>() where BRAND0 : Brand<BRAND0>, IExtends<BRAND0,BRAND> {
      return Brand<BRAND0>.CoerceFrom<T, BRAND>(this);
    }
    internal static BRAND UpCast<T0, BRAND0>(BRAND0 value)
      where T0 : T
      where BRAND0 : Brand<T0, BRAND0> {
      return ToBrand(UpCastOperator<T0, T>.Instance.Make(value.Underlying)); 
    }
    private class UpCastVisitor : BrandTypeReveal<BRAND> {
      public BRAND Visit<T0, BRAND0>(Brand<T0, BRAND0> value) where BRAND0 : Brand<T0, BRAND0> {
        return ToBrand(UpCastOperator1<T0, T>.Instance.Make(value.Underlying));
      }
    }
    /// <summary>
    /// Upcast value of extending brand to this brand.
    /// </summary>
    public static BRAND UpCast<BRAND0>(Brand<BRAND0> value) where BRAND0 : Brand<BRAND0>, IExtends<BRAND0,BRAND> {
      if (value is BRAND) return (BRAND)(object)value;
      else return value.Reveal(new UpCastVisitor());
    }
    /// <summary>
    /// Coerce this value to a brand using a conversion routine (not an up or down cast).
    /// </summary>
    public BRAND0 Coerce<T0, BRAND0>()
      where BRAND0 : Brand<T0, BRAND0> {
      return Brand<T0,BRAND0>.ToBrand(ExplicitConvertOperator<T, T0>.Instance.Make(Underlying));
    }
    /// <summary>
    /// Map this value to another of the same brand type.
    /// </summary>
    public BRAND MapM(Func<BRAND, BRAND> F) { return F(AsBrand); }

    /// <summary>
    /// Map this value to another value of a different type.
    /// </summary>
    /// <param name="F">Conversion function, pure</param>
    public BRAND0 Map<T0, BRAND0>(Func<T, T0> F)
      where BRAND0 : Brand<T0, BRAND0> {
      return Brand<T0,BRAND0>.ToBrand(new MapOperator<T, T0>(F).Make(this));
    }



    /// <summary>
    /// Map this value to another value of a different type. F is assumed to be time dependent.
    /// </summary>
    /// <param name="F">Conversion function, time dependent</param>
    public BRAND0 StatefulMap<T0, BRAND0>(Func<T, T0> F)
      where BRAND0 : ICanWrap<BRAND0, T0> {
      var e = new MapOperator<T, T0>(F) { CanOptimize = false }.Make(this);
      return CanWrap<BRAND0, T0>.To(e);
    }
    public static BRAND State(Func<T> F) {
      var e = new MapOperator<T, T>(t => F()) { CanOptimize = false }.Make(new Constant<T>(default(T)));
      return ToBrand(e);
    }


    /// <summary>
    /// Map this value to another value of a different type. Result supports inversion.
    /// </summary>
    /// <param name="F">Conversion function</param>
    /// <param name="G">Inversion function</param>
    public BRAND0 Map<T0, BRAND0>(Func<T, T0> F, Func<T0, T> G)
      where BRAND0 : Brand<T0, BRAND0> {
      return Brand<T0, BRAND0>.ToBrand(new MapOperator<T, T0>(F) { G = G }.Make(this));
    }
    /// <summary>
    /// Map this value to another value of a different type. Result supports inversion.
    /// </summary>
    /// <param name="F">Conversion function</param>
    /// <param name="G">Inversion function</param>
    public BRAND Map(Func<T, T> F, Func<T, T> G) {
      return Map<T, BRAND>(F, G);
    }
    /// <summary>
    /// Map this value to another value of a different type.
    /// </summary>
    /// <param name="F">Conversion function</param>
    public BRAND Map(Func<T, T> F) {
      return Map<T, BRAND>(F, null);
    }
    /// <summary>
    /// Combine two values together in map.
    /// </summary>
    public CombineSet<T0, BRAND0> Combine<T0, BRAND0>(ICanWrap<BRAND0,T0> Other) where BRAND0 : ICanWrap<BRAND0,T0> {
      return new CombineSet<T0, BRAND0>() { Other = Other, Self = this };
    }
    public partial class CombineSet<T0, BRAND0> where BRAND0 : ICanWrap<BRAND0,T0> {
      internal ICanWrap<BRAND0,T0> Other;
      internal BRAND Self;
      /// <summary>
      /// General map that supports inversion on either argument.
      /// </summary>
      /// <param name="F">Conversion function</param>
      /// <param name="G">Inversion function for first parameter</param>
      /// <param name="H">Inversion function for second parameter</param>
      public BRAND1 Map<T1, BRAND1>(Func<T, T0, T1> F, Func<T1, T0, T> G, Func<T1, T, T0> H) where BRAND1 : ICanWrap<BRAND1,T1> {
        return CanWrap<BRAND1,T1>.To(new MapOperator<T, T0, T1>(F) {
          UTS = G, UST = H,
        }.Make(Self, CanWrap<BRAND0,T0>.From((BRAND0) Other)));
      }
      /// <summary>
      /// Map that supports inversion on the first argument.
      /// </summary>
      /// <param name="F">Conversion function</param>
      /// <param name="G">Inversion function for first parameter</param>
      public BRAND1 Map<T1, BRAND1>(Func<T, T0, T1> F, Func<T1, T0, T> G) where BRAND1 : Brand<T1, BRAND1> {
        return Map<T1, BRAND1>(F, G, null);
      }
      /// <summary>
      /// Map that supports inversion on the second argument.
      /// </summary>
      /// <param name="F">Conversion function</param>
      /// <param name="H">Inversion function for second parameter</param>
      public BRAND1 MapB<T1, BRAND1>(Func<T, T0, T1> F, Func<T1, T, T0> H) where BRAND1 : Brand<T1, BRAND1> {
        return Map<T1, BRAND1>(F, null, H);
      }
      /// <summary>
      /// Map that does not support inversion.
      /// </summary>
      /// <param name="F">Conversion function</param>
      public BRAND1 Map<T1, BRAND1>(Func<T, T0, T1> F) where BRAND1 : ICanWrap<BRAND1,T1> {
        return Map<T1, BRAND1>(F, null, null);
      }
      /// <summary>
      /// General map that supports inversion on either argument.
      /// </summary>
      /// <param name="F">Conversion function</param>
      /// <param name="G">Inversion function for first parameter</param>
      /// <param name="H">Inversion function for second parameter</param>
      public BRAND Map(Func<T, T0, T> F, Func<T, T0, T> G, Func<T, T, T0> H) {
        return Map<T, BRAND>(F, G, H);
      }
      /// <summary>
      /// Map that supports inversion on the first argument.
      /// </summary>
      /// <param name="F">Conversion function</param>
      /// <param name="G">Inversion function for first parameter</param>
      public BRAND Map(Func<T, T0, T> F, Func<T, T0, T> G) {
        return Map(F, G, null);
      }
      /// <summary>
      /// Map that supports inversion on the second argument.
      /// </summary>
      /// <param name="F">Conversion function</param>
      /// <param name="H">Inversion function for second parameter</param>
      public BRAND MapB(Func<T, T0, T> F, Func<T, T, T0> H) {
        return Map<T, BRAND>(F, null, H);
      }
      /// <summary>
      /// Map that does not support inversion.
      /// </summary>
      /// <param name="F">Conversion function</param>
      public BRAND Map(Func<T, T0, T> F) {
        return Map<T, BRAND>(F, null, null);
      }
    }
    public static implicit operator Brand<T, BRAND>(T t) {
      return ToBrand(new Constant<T>(t));
    }

    public BoolBl Equal(BRAND other) { return ValueOperators<T>.Equal.Instance.Make(this.Underlying, other.Underlying); }
    public BoolBl NotEqual(BRAND other) { return ValueOperators<T>.NotEqual.Instance.Make(this.Underlying, other.Underlying); }
    public static BoolBl operator ==(Brand<T, BRAND> opA, Brand<T,BRAND> opB) { return opA.Equal(opB); }
    public static BoolBl operator !=(Brand<T, BRAND> opA, Brand<T,BRAND> opB) { return opA.NotEqual(opB); }

    public override int GetHashCode() { return Underlying.GetHashCode(); }
    public override bool Equals(object obj) {
      if (obj is Brand<T, BRAND>) return Underlying.Equals(((Brand<T, BRAND>)obj).Underlying);
      return base.Equals(obj);
    }
    /// <summary>
    /// General coercion.
    /// </summary>
    public override BRAND0 Coerce<BRAND0>() { return Brand<BRAND0>.CoerceFrom<T, BRAND>(this); }
  }

  public partial interface IValueBrand<BRAND> : IBrand<BRAND> where BRAND : IValueBrand<BRAND> { }
  public partial interface IValueBrand<BRAND, MONO> : IValueBrand<BRAND>
    where BRAND : IValueBrand<BRAND, MONO>
    where MONO : IValueBrand<MONO, MONO> { }

  public abstract partial class ValueBrand<T, BRAND> : Brand<T, BRAND>, IValueBrand<BRAND>
    where BRAND : ValueBrand<T, BRAND> {
    public ValueBrand(Expr<T> Underlying) : base(Underlying) { }
    public ValueBrand() : base() { }
  }
  public interface IValueBrandK<K, BRAND> : IBrand<BRAND> where BRAND : IValueBrandK<K, BRAND> {
  }
  public static class ValueBrandK<K, BRAND> where BRAND : IValueBrandK<K, BRAND> {
    public static Func<Expr<K>[], BRAND> Make { get; internal set; }
  }
  public interface IValueBrandNoT<BRAND, MONO>
    where BRAND : Brand<BRAND>, IValueBrandNoT<BRAND, MONO>
    where MONO : Brand<MONO>, IValueBrandNoT<MONO, MONO> {
  }

  public abstract partial class ValueBrand<T, K, BRAND, MONO> : ValueBrand<T, BRAND>, IValueBrand<BRAND,MONO>, IValueBrandK<K,BRAND>, IValueBrandNoT<BRAND,MONO>
    where BRAND : ValueBrand<T, K, BRAND, MONO> where MONO : ValueBrand<K,K,MONO,MONO> {
    public ValueBrand(Expr<T> Underlying) : base(Underlying) { }
    public ValueBrand() : base() { }

    static ValueBrand() {
      ValueBrandK<K, BRAND>.Make = Es => ToBrand(Arity<K, T>.ArityI.Make(Es));
    }

    public BRAND MapD(Func<MONO, MONO> F) {
      var Cs = new Expr<K>[Arity<K, T>.ArityI.Count];
      for (int i = 0; i < Cs.Length; i++) 
        Cs[i] = F(ValueBrand<K,K,MONO,MONO>.ToBrand(Arity<K, T>.ArityI.Access(Underlying, i)));
      return ToBrand(Arity<K, T>.ArityI.Make(Cs));
    }
    public MONO this[int Idx] { 
      get { return ValueBrand<K, K, MONO, MONO>.ToBrand(Arity<K, T>.ArityI.Access(Underlying, Idx)); } 
      set { this[Idx].Bind = value; } 
    }
    public MONO this[IntBl Idx] {
      get {
        var count = Arity<K, T>.ArityI.Count;
        MONO[] table = new MONO[count];
        for (int i = 0; i < count; i++) table[i] = this[i];
        return table.Table(Idx);
      }
      set { this[Idx].Bind = value; }
    }
    public static BRAND Make(params MONO[] p) {
      var Cs = new Expr<K>[Arity<K, T>.ArityI.Count];
      for (int i = 0; i < Cs.Length; i++) Cs[i] = p[i];
      return ToBrand(Arity<K, T>.ArityI.Make(Cs));
    }
    public static BRAND Make(int At, MONO At0, Func<int,MONO> Rest) {
      var Cs = new Expr<K>[Arity<K, T>.ArityI.Count];
      for (int i = 0; i < Cs.Length; i++) Cs[i] = (i == At) ? At0 : i < At ? Rest(i) : Rest(i - 1);
      return ToBrand(Arity<K, T>.ArityI.Make(Cs));
    }
  }
  /// <summary>
  /// Value brand with exactly one element. 
  /// </summary>
  public partial interface IMonoValue<T,BRAND> : IBrand<T,BRAND> where BRAND : ValueBrand<T,T,BRAND,BRAND> { }
  public partial interface IBoolBl<BRAND> : IValueBrand<BRAND> where BRAND : IBoolBl<BRAND> {
    BRAND And(BRAND other);
    BRAND Or(BRAND other);
    BRAND ExclusiveOr(BRAND other);
    BRAND Not { get; }
    BoolBl All { get; }
    BoolBl Any { get; }
    BoolBl None { get; }
    OTHER Condition<OTHER>(OTHER IfTrue, OTHER IfFalse) where OTHER : IConditionTarget<OTHER>;
  }

  public abstract partial class BaseBoolBl<T, BRAND> : ValueBrand<T, bool, BRAND, BoolBl>, IBoolBl<BRAND>
    where BRAND : BaseBoolBl<T, BRAND> {
    public BaseBoolBl(Expr<T> Underlying) : base(Underlying) { }
    public BaseBoolBl() : base() { }

    public abstract BoolBl All { get; }
    public abstract BoolBl Any { get; }
    public BoolBl None { get { return !Any; } }

    /// <summary>
    /// A functional conditional expression. Not garunteed to short circuit, will often evaluate both the true and false branches regardless of the condition value.
    /// </summary>
    public OTHER Condition<OTHER>(OTHER IfTrue, OTHER IfFalse) where OTHER : IConditionTarget<OTHER> {
      return IfTrue.Choose(All, IfFalse);
    }
    /// <summary>
    /// Like Condition but will attempt to short circuit evaluation if possible. 
    /// In pixel shaders, should be used where coherence exists in the condition's test; i.e., 
    /// pixels near each other have simular condition test values. 
    /// </summary>
    public OTHER DCondition<OTHER>(OTHER IfTrue, OTHER IfFalse) where OTHER : IConditionTarget<OTHER> {
      return IfTrue.DChoose(All, IfFalse);
    }

    public DoubleBl Condition(double IfTrue, double IfFalse) {
      return ((DoubleBl)IfTrue).Choose(All, IfFalse);
    }
    public IntBl Condition(int IfTrue, int IfFalse) {
      return ((IntBl)IfTrue).Choose(All, IfFalse);
    }
    public StringBl Condition(string IfTrue, string IfFalse) {
      return ((StringBl)IfTrue).Choose(All, IfFalse);
    }

    public BRAND And(BRAND other) { return ToBrand(BoolOperators<T>.And.Instance.Make(this, other)); }
    public BRAND Or(BRAND other) { return ToBrand(BoolOperators<T>.Or.Instance.Make(this, other)); }
    public BRAND ExclusiveOr(BRAND other) { return ToBrand(BoolOperators<T>.ExclusiveOr.Instance.Make(this, other)); }
    public BRAND Not { get { return ToBrand(BoolOperators<T>.Not.Instance.Make(this)); } }

    public static BRAND operator !(BaseBoolBl<T, BRAND> opA) { return opA.Not; }
    public static BRAND operator &(BaseBoolBl<T, BRAND> opA, BRAND opB) { return opA.And(opB); }
    public static BRAND operator |(BaseBoolBl<T, BRAND> opA, BRAND opB) { return opA.Or(opB); }
    public static BRAND operator ^(BaseBoolBl<T, BRAND> opA, BRAND opB) { return opA.ExclusiveOr(opB); }
    public static BRAND operator &(T bA, BaseBoolBl<T, BRAND> bB) { return ToBrand(new Constant<T>(bA)).And(bB); }
    public static BRAND operator |(T bA, BaseBoolBl<T, BRAND> bB) { return ToBrand(new Constant<T>(bA)).Or(bB); }
    public static BRAND operator ^(T bA, BaseBoolBl<T, BRAND> bB) { return ToBrand(new Constant<T>(bA)).ExclusiveOr(bB); }
    static BaseBoolBl() {
    }
  }
  public partial class BoolBl : BaseBoolBl<bool, BoolBl>, IMonoValue<bool, BoolBl>, ICoreBrandT {
    public BoolBl(Expr<bool> Underlying) : base(Underlying) { }
    public BoolBl() : base() { }
    public static implicit operator BoolBl(Expr<bool> Value) { return new BoolBl(Value); }
    public static implicit operator BoolBl(bool Value) { return new Constant<bool>(Value); }
    static BoolBl() { 
      Register(e => e);
      Arity1<bool>.CheckInit();
    }

    public Expr<T> Condition0<T>(Expr<T> IfTrue, Expr<T> IfFalse) {
      return ValueOperators<T>.Condition.Instance.Make(Underlying, IfTrue, IfFalse);
    }
    public Expr<T> DCondition0<T>(Expr<T> IfTrue, Expr<T> IfFalse) {
      return ValueOperators<T>.DynamicCondition.Instance.Make(Underlying, IfTrue, IfFalse);
    }

    public override BoolBl All { get { return this; } }
    public override BoolBl Any { get { return this; } }

    public static implicit operator Bool2Bl(BoolBl b) { return new Bool2Bl(b, b); }
    public static implicit operator Bool3Bl(BoolBl b) { return new Bool3Bl(b, b, b); }
    public static implicit operator Bool4Bl(BoolBl b) { return new Bool4Bl(b, b, b, b); }

  }
  public partial interface MultiBoolBl<BRAND> : IBoolBl<BRAND> where BRAND : MultiBoolBl<BRAND> { }
  public abstract partial class MultiBoolBl<B, BRAND, ARITY> : BaseBoolBl<B, BRAND>, MultiBoolBl<BRAND>
    where BRAND : MultiBoolBl<B, BRAND, ARITY>
    where ARITY : MultiArity<bool, B, ARITY>, new() {
    public MultiBoolBl(Expr<B> Underlying) : base(Underlying) { }
    public MultiBoolBl() : base() { }
    static MultiBoolBl() {
      Extensions.CheckInit<ARITY>();
    }

    public static implicit operator MultiBoolBl<B, BRAND, ARITY>(BoolBl b) {
      return ToBrand(Arity<bool,B,ARITY>.ArityI.Repeat(b.Underlying));
    }
    public static implicit operator BoolBl(MultiBoolBl<B, BRAND, ARITY> b) { return b.All; }
    public override BoolBl All {
      get {
        BoolBl ret = true;
        for (int i = 0; i < Arity<bool, B>.ArityI.Count; i++)
          ret = ret & Arity<bool, B>.ArityI.Access(Underlying, i).Bl();
        return ret;
      }
    }
    public override BoolBl Any { 
      get {
        BoolBl ret = false;
        for (int i = 0; i < Arity<bool, B>.ArityI.Count; i++)
          ret = ret | Arity<bool, B>.ArityI.Access(Underlying, i).Bl();
        return ret;
      }
    }


    // Add bool operators.
    public static BRAND operator &(BoolBl opA, MultiBoolBl<B, BRAND, ARITY> opB) { return ((MultiBoolBl<B, BRAND, ARITY>)opA).And(opB); }
    public static BRAND operator |(BoolBl opA, MultiBoolBl<B, BRAND, ARITY> opB) { return ((MultiBoolBl<B, BRAND, ARITY>)opA).Or(opB); }
    public static BRAND operator ^(BoolBl opA, MultiBoolBl<B, BRAND, ARITY> opB) { return ((MultiBoolBl<B, BRAND, ARITY>)opA).ExclusiveOr(opB); }
  }


  public interface HasXY<BRAND> {
    BRAND X { get; set;  }
    BRAND Y { get; set;  }
  }
  public interface HasXYZ<BRAND> : HasXY<BRAND> {
    BRAND Z { get; set;  }
  }
  public interface HasXYZW<BRAND> : HasXYZ<BRAND> {
    BRAND W { get; set;  }
  }
  public partial class Bool2Bl : MultiBoolBl<Vec2<bool>, Bool2Bl, Vec2Arity<bool>>, HasXY<BoolBl>, ICoreBrandT {
    public Bool2Bl(Expr<Vec2<bool>> Underlying) : base(Underlying) { }
    public Bool2Bl() : base() { }
    public Bool2Bl(BoolBl bA, BoolBl bB) : base(Vec2Arity<bool>.ArityI.Make(bA, bB)) { }
    public static implicit operator Bool2Bl(Expr<Vec2<bool>> Value) { return new Bool2Bl(Value); }
    public static implicit operator Bool2Bl(Vec2<bool> Value) { return new Constant<Vec2<bool>>(Value); }
    public static implicit operator Bool2Bl(bool Value) { return (BoolBl) Value; }
    static Bool2Bl() { 
      Register(e => e);
    }

    public BoolBl X { get { return (Vec2Arity<bool>.ArityI.Access(Underlying, 0)); } set { X.Bind = value; } }
    public BoolBl Y { get { return (Vec2Arity<bool>.ArityI.Access(Underlying, 1)); } set { Y.Bind = value; } }
  }
  public partial class Bool3Bl : MultiBoolBl<Vec3<bool>, Bool3Bl, Vec3Arity<bool>>, HasXYZ<BoolBl>, ICoreBrandT {
    public Bool3Bl(Expr<Vec3<bool>> Underlying) : base(Underlying) { }
    public Bool3Bl() : base() { }
    public Bool3Bl(BoolBl bA, BoolBl bB, BoolBl bC) : base(Vec3Arity<bool>.ArityI.Make(bA, bB, bC)) { }
    public static implicit operator Bool3Bl(Expr<Vec3<bool>> Value) { return new Bool3Bl(Value); }
    public static implicit operator Bool3Bl(Vec3<bool> Value) { return new Constant<Vec3<bool>>(Value); }
    public static implicit operator Bool3Bl(bool Value) { return (BoolBl)Value; }
    static Bool3Bl() { 
      Register(e => e);
    }
    public BoolBl X { get { return Vec3Arity<bool>.ArityI.Access(Underlying, 0); } set { X.Bind = value; } }
    public BoolBl Y { get { return Vec3Arity<bool>.ArityI.Access(Underlying, 1); } set { Y.Bind = value; } }
    public BoolBl Z { get { return Vec3Arity<bool>.ArityI.Access(Underlying, 2); } set { Z.Bind = value; } }
  }
  public partial class Bool4Bl : MultiBoolBl<Vec4<bool>, Bool4Bl, Vec4Arity<bool>>, HasXYZW<BoolBl>, ICoreBrandT {
    public Bool4Bl(Expr<Vec4<bool>> Underlying) : base(Underlying) { }
    public Bool4Bl() : base() { }
    public Bool4Bl(BoolBl bA, BoolBl bB, BoolBl bC, BoolBl bD) : base(Vec4Arity<bool>.ArityI.Make(bA, bB, bC, bD)) { }
    public static implicit operator Bool4Bl(Expr<Vec4<bool>> Value) { return new Bool4Bl(Value); }
    public static implicit operator Bool4Bl(Vec4<bool> Value) { return new Constant<Vec4<bool>>(Value); }
    public static implicit operator Bool4Bl(bool Value) { return (BoolBl)Value; }
    static Bool4Bl() { 
      Register(e => e);
    }
    public BoolBl X { get { return Vec4Arity<bool>.ArityI.Access(Underlying, 0); } set { X.Bind = value; } }
    public BoolBl Y { get { return Vec4Arity<bool>.ArityI.Access(Underlying, 1); } set { Y.Bind = value; } }
    public BoolBl Z { get { return Vec4Arity<bool>.ArityI.Access(Underlying, 2); } set { Z.Bind = value; } }
    public BoolBl W { get { return Vec4Arity<bool>.ArityI.Access(Underlying, 3); } set { W.Bind = value; } }
  }
  public static partial class NumericFactory<BRAND> where BRAND : INumericBl<BRAND> {
    public static BRAND Zero { get { return Zero0(); } }
    internal static Func<BRAND> Zero0;
  }
  public partial interface ISupportsLerp<BRAND> : IConditionTarget<BRAND>, ICanTable<BRAND> {
    BRAND Lerp0(BRAND Other, DoubleBl t);
  }
  public partial interface IBaseNumericBl<BRAND> : ISupportsLerp<BRAND> where BRAND : IBaseNumericBl<BRAND> {
    BRAND Add(BRAND other);
    BRAND Subtract(BRAND other);
    BRAND Multiply(BRAND other);
    BRAND Divide(BRAND other);
    BRAND Modulo(BRAND other);
    BRAND Min(BRAND Other);
    BRAND Max(BRAND Other);
    BRAND Clamp(BRAND min, BRAND max);
  }
  public partial interface INumericBl<BRAND> : IValueBrand<BRAND>, 
      IBaseNumericBl<BRAND> where BRAND : INumericBl<BRAND> {
    BRAND Negate { get; }
    BRAND Abs { get; }
  }
  public partial interface INumericNoT<BRAND, MONO> : IValueBrandNoT<BRAND, MONO>, INumericBl<BRAND>
    where BRAND : Brand<BRAND>, INumericNoT<BRAND, MONO>
    where MONO : Brand<MONO>, INumericNoT<MONO, MONO> {
  }

  public interface IBaseNumericBl<T, BRAND> : INumericBl<BRAND> where BRAND : IBaseNumericBl<T, BRAND> { }
  public interface IBaseNumericBl<T, K, BRAND> : IBaseNumericBl<T, BRAND> where BRAND : IBaseNumericBl<T, K, BRAND> { }
  public interface IBaseNumericBl<T, K, BRAND, MONO> : IBaseNumericBl<T, K, BRAND>, 
    INumericNoT<BRAND, MONO>
    where BRAND : Brand<BRAND>, IBaseNumericBl<T, K, BRAND, MONO>
    where MONO : Brand<MONO>, IBaseNumericBl<K, K, MONO, MONO> { }
  public partial interface INumericBl<BRAND, BOOL> : INumericBl<BRAND> where BRAND : INumericBl<BRAND, BOOL> {
    BOOL GreaterThan(BRAND other);
    BOOL LessThan(BRAND other);
    BOOL GreaterThanOrEqual(BRAND other);
    BOOL LessThanOrEqual(BRAND other);

  }

  public partial interface IMonoNumericBl<T, BRAND> : IMonoValue<T, BRAND>, ICoreBrandT where BRAND : BaseNumericBl<T, T, BRAND, BRAND>, IMonoNumericBl<T, BRAND> { }
  public abstract partial class BaseNumericBl<T, K, BRAND, MONO> : ValueBrand<T, K, BRAND, MONO>, IBaseNumericBl<T,K,BRAND,MONO>
    where BRAND : BaseNumericBl<T, K, BRAND, MONO> where MONO : BaseNumericBl<K, K, MONO, MONO> {

    static BaseNumericBl() {
      NumericOperators0<T, K>.StaticInit();
    }

    public BaseNumericBl(Expr<T> e) : base(e) { }
    public BaseNumericBl() : base() { }
    public abstract BRAND Negate { get; }
    public abstract BRAND Add(BRAND other);
    public abstract BRAND Subtract(BRAND other);
    public abstract BRAND Multiply(BRAND other);
    public abstract BRAND Divide(BRAND other);
    public abstract BRAND Modulo(BRAND other);
    public abstract BRAND Abs { get; }
    public abstract BRAND Min(BRAND Other);
    public abstract BRAND Max(BRAND Other);
    public abstract BRAND Lerp0(BRAND Other, DoubleBl t);
    public abstract BRAND Clamp(BRAND min, BRAND max);
  }

  public abstract partial class BaseNumericBl<T, K, BRAND, BOOL, MONO> : BaseNumericBl<T, K, BRAND, MONO>, INumericBl<BRAND, BOOL>
    where BRAND : BaseNumericBl<T, K, BRAND, BOOL, MONO>
    where BOOL : IBoolBl<BOOL> 
    where MONO : BaseNumericBl<K,K,MONO,BoolBl,MONO> {
    public BaseNumericBl(Expr<T> e) : base(e) { }
    public BaseNumericBl() : base() { }
    public override BRAND Negate { get { return ToBrand(NumericOperators0<T, K>.Negate.Instance.Make(this)); } }
    public override BRAND Abs { get { return ToBrand(NumericOperators0<T, K>.Abs.Instance.Make(this)); } }
    public override BRAND Min(BRAND other) { return ToBrand(NumericOperators0<T, K>.Min.Instance.Make(this, other)); }
    public override BRAND Max(BRAND other) { return ToBrand(NumericOperators0<T, K>.Max.Instance.Make(this, other)); }
    public override BRAND Clamp(BRAND min, BRAND max) { return ToBrand(NumericOperators0<T, K>.Clamp.Instance.Make(this, min, max)); }
    public BRAND Clamp(BRAND max) {
      return Clamp(Brand<BRAND>.Default, max); 
    }
    public MONO Max() {
      MONO ret = this[0];
      for (int i = 1; i < Arity<K, T>.ArityI.Count; i++)
        ret = ret.Max(this[i]);
      return ret;
    }
    public MONO Min() {
      MONO ret = this[0];
      for (int i = 1; i < Arity<K, T>.ArityI.Count; i++)
        ret = ret.Min(this[i]);
      return ret;
    }

    static BaseNumericBl() {
      Extensions.CheckInit<BOOL>();
      NumericFactory<BRAND>.Zero0 = () => ToBrand(new Constant<T>(default(T)));
    }

    //public override BRAND Lerp0(BRAND other, DoubleBl t) { throw new NotImplementedException(); }

    public override BRAND Add(BRAND other) { return ToBrand(NumericOperators0<T, K>.Add.Instance.Make(this, other)); }
    public override BRAND Subtract(BRAND other) { return ToBrand(NumericOperators0<T, K>.Subtract.Instance.Make(this, other)); }
    public override BRAND Multiply(BRAND other) { return ToBrand(NumericOperators0<T, K>.Multiply.Instance.Make(this, other)); }
    public override BRAND Divide(BRAND other) { return ToBrand(NumericOperators0<T, K>.Divide.Instance.Make(this, other)); }
    public override BRAND Modulo(BRAND other) { return ToBrand(NumericOperators0<T, K>.Modulo.Instance.Make(this, other)); }

    public abstract BOOL GreaterThan(BRAND other);
    public abstract BOOL LessThan(BRAND other); 
    public abstract BOOL GreaterThanOrEqual(BRAND other); 
    public abstract BOOL LessThanOrEqual(BRAND other);

    public static implicit operator BaseNumericBl<T, K, BRAND, BOOL, MONO>(T t) {
      return ToBrand(new Constant<T>(t));
    }
    public static BRAND operator &(BaseNumericBl<T, K, BRAND, BOOL, MONO> op0, BoolBl op1) {
      BRAND IfFalse = ToBrand(Arity<K,T>.ArityI.Repeat(default(K)));
      return op1.Condition<BRAND>(op0, IfFalse);
    }

    public static BRAND operator -(BaseNumericBl<T, K, BRAND, BOOL, MONO> bA) { return (bA).Negate; }
    public static BRAND operator +(BaseNumericBl<T, K, BRAND, BOOL, MONO> bA) { return bA; }

    public static BRAND operator +(BaseNumericBl<T, K, BRAND, BOOL, MONO> bA, BaseNumericBl<T, K, BRAND, BOOL, MONO> bB) { return bA.     Add(bB); }
    public static BRAND operator -(BaseNumericBl<T, K, BRAND, BOOL, MONO> bA, BaseNumericBl<T, K, BRAND, BOOL, MONO> bB) { return bA.Subtract(bB); }
    public static BRAND operator *(BaseNumericBl<T, K, BRAND, BOOL, MONO> bA, BaseNumericBl<T, K, BRAND, BOOL, MONO> bB) { return bA.Multiply(bB); }
    public static BRAND operator /(BaseNumericBl<T, K, BRAND, BOOL, MONO> bA, BaseNumericBl<T, K, BRAND, BOOL, MONO> bB) { return bA.  Divide(bB); }
    public static BRAND operator %(BaseNumericBl<T, K, BRAND, BOOL, MONO> bA, BaseNumericBl<T, K, BRAND, BOOL, MONO> bB) { return bA.  Modulo(bB); }
    //public static BRAND operator -(BaseNumericBl<T, K, BRAND, BOOL, MONO> bA, BRAND bB) { return bA.Subtract(bB); }
    //public static BRAND operator *(BaseNumericBl<T, K, BRAND, BOOL, MONO> bA, BRAND bB) { return bA.Multiply(bB); }
    //public static BRAND operator /(BaseNumericBl<T, K, BRAND, BOOL, MONO> bA, BRAND bB) { return bA.Divide(bB); }
    //public static BRAND operator %(BaseNumericBl<T, K, BRAND, BOOL, MONO> bA, BRAND bB) { return bA.Modulo(bB); }

    public static BOOL operator >(BaseNumericBl<T, K, BRAND, BOOL, MONO> bA, BRAND bB) { return bA.GreaterThan(bB); }
    public static BOOL operator <(BaseNumericBl<T, K, BRAND, BOOL, MONO> bA, BRAND bB) { return bA.LessThan(bB); }
    public static BOOL operator >=(BaseNumericBl<T, K, BRAND, BOOL, MONO> bA, BRAND bB) { return bA.GreaterThanOrEqual(bB); }
    public static BOOL operator <=(BaseNumericBl<T, K, BRAND, BOOL, MONO> bA, BRAND bB) { return bA.LessThanOrEqual(bB); }

    //public static BRAND operator +(T bA, BaseNumericBl<T, K, BRAND, BOOL, MONO> bB) { return ToBrand(new Constant<T>(bA)).Add(bB); }
    //public static BRAND operator -(T bA, BaseNumericBl<T, K, BRAND, BOOL, MONO> bB) { return ToBrand(new Constant<T>(bA)).Subtract(bB); }
    //public static BRAND operator *(T bA, BaseNumericBl<T, K, BRAND, BOOL, MONO> bB) { return ToBrand(new Constant<T>(bA)).Multiply(bB); }
    //public static BRAND operator /(T bA, BaseNumericBl<T, K, BRAND, BOOL, MONO> bB) { return ToBrand(new Constant<T>(bA)).Divide(bB); }
    //public static BRAND operator %(T bA, BaseNumericBl<T, K, BRAND, BOOL, MONO> bB) { return ToBrand(new Constant<T>(bA)).Modulo(bB); }
  }
  public partial interface IBaseDoubleBl<BRAND> : IBaseNumericBl<BRAND> where BRAND : IBaseDoubleBl<BRAND> {
    BRAND Multiply(DoubleBl other);
    BRAND Divide(DoubleBl other);
  }
  public partial interface IDoubleBl<BRAND> : INumericBl<BRAND>, IBaseDoubleBl<BRAND> where BRAND : IDoubleBl<BRAND> {
    BRAND Floor { get; }
    BRAND Ceiling { get; }
    BRAND Round { get; }
    BRAND Exp { get; }
    BRAND Log { get; }
    BRAND Exp2 { get; }
    BRAND Log2 { get; }
    BRAND Exp10 { get; }
    BRAND Log10 { get; }
    //BRAND Pow(BRAND Power);
    BRAND LogN(BRAND Base);
    BRAND Recip { get; }
    BRAND Square { get; }
    BRAND Sqrt { get; }
    BRAND Rsqrt { get; }
    BRAND Ddx { get; }
    BRAND Ddy { get; }
    BRAND Fwidth { get; }
    BRAND Frac { get; }
    BRAND Ldexp(BRAND other);

    DoubleBl Dot(BRAND Other);
    DoubleBl LengthSquared { get; }
    DoubleBl Length { get; }
    DoubleBl Distance(BRAND other);
    BRAND Smoothstep(BRAND b, BRAND x);
    DoubleBl this[int Idx] { get; set; }
    PointBl this[int i, int j] { get; set; }
    Point3DBl this[int i, int j, int k] { get; set; }
    Point4DBl this[int i, int j, int k, int l] { get; set; }
  }
  public partial interface IDoubleBl<BRAND, BOOL> : INumericBl<BRAND, BOOL>, IDoubleBl<BRAND> where BRAND : IDoubleBl<BRAND,BOOL> {
    BOOL IsInfinity { get; }
    BOOL IsNaN { get; }
    BOOL IsPositiveInfinity { get; }
    BOOL IsNegativeInfinity { get; }
  }
  public partial interface IBaseDoubleBl<T, BRAND> : IDoubleBl<BRAND> where BRAND : IBaseDoubleBl<T,BRAND> {
    DoubleBl this[IntBl i] { get; set; }
  }

  public abstract partial class BaseDoubleLikeBl<T, BRAND, BOOL> :
    BaseNumericBl<T, double, BRAND, BOOL, DoubleBl>
    where BRAND : BaseDoubleLikeBl<T, BRAND, BOOL>
    where BOOL : IBoolBl<BOOL> {
    public BaseDoubleLikeBl(Expr<T> e) : base(e) { }
    public BaseDoubleLikeBl() : base() { }
    public BRAND Floor { get { return ToBrand(DoubleOperators0<T>.Floor.Instance.Make(this)); } }
    public BRAND Ceiling { get { return ToBrand(DoubleOperators0<T>.Ceiling.Instance.Make(this)); } }
    public BRAND Round { get { return ToBrand(DoubleOperators0<T>.Round.Instance.Make(this)); } }

    static BaseDoubleLikeBl() {
      DoubleOperators0<T>.CheckInit();
    }

    public StringBl ToStringBl(DoubleBl e) {
      return MapD(d => d.RoundN(e)).ToStringBl();
    }

    /// <summary>
    /// Clamp components between 0 and 1.
    /// </summary>
    public BRAND Saturate { get { return ToBrand(DoubleOperators0<T>.Saturate.Instance.Make(this)); } }

    /// <summary>
    /// Round the number then be devided by the param d
    /// </summary>
    /// <param name="d"></param>
    /// <returns></returns>
    public BRAND RoundN(DoubleBl d) { return (this * (d)).Round / d; }
   
    public static implicit operator BaseDoubleLikeBl<T, BRAND, BOOL>(DoubleBl d) {
      return ToBrand(Arity<double, T>.ArityI.Repeat(d));
    }
    protected static BRAND Convert(DoubleBl d) { return (BaseDoubleLikeBl<T, BRAND, BOOL>)d; }

    public virtual BRAND Multiply(DoubleBl d) { return Multiply(Convert(d)); }
    public virtual BRAND Divide(DoubleBl d) { return Divide(Convert(d)); }

    public static BRAND operator *(BaseDoubleLikeBl<T, BRAND, BOOL> bA, BaseDoubleLikeBl<T, BRAND, BOOL> bB) { return bA.Multiply(bB); }
    public static BRAND operator /(BaseDoubleLikeBl<T, BRAND, BOOL> bA, BaseDoubleLikeBl<T, BRAND, BOOL> bB) { return bA.Divide(bB); }
  }


  public abstract partial class BaseDoubleBl<T, BRAND, BOOL> : 
    BaseDoubleLikeBl<T, BRAND, BOOL>, IDoubleBl<BRAND, BOOL>, IBaseDoubleBl<T,BRAND> 
    where BRAND : BaseDoubleBl<T, BRAND, BOOL>
    where BOOL : IBoolBl<BOOL> {
    public BaseDoubleBl(Expr<T> e) : base(e) { }
    public BaseDoubleBl() : base() { }

    public static BRAND Unit {
      get {
        Expr<double>[] d = new Expr<double>[Vecs.Arity<double, T>.ArityI.Count];
        d[0] = 1d.Bl();
        for (int i = 1; i < d.Length; i++) d[i] = 0d.Bl();
        BRAND Unit = ToBrand(Vecs.Arity<double, T>.ArityI.Make(d));
        return Unit;
      }
    }
    public static BRAND NaN {
      get {
        Expr<double>[] d = new Expr<double>[Vecs.Arity<double, T>.ArityI.Count];
        for (int i = 0; i < d.Length; i++) d[i] = double.NaN;
        return ToBrand(Vecs.Arity<double, T>.ArityI.Make(d));
      }
    }

    public static implicit operator BaseDoubleBl<T, BRAND, BOOL>(DoubleBl d) {
      return ToBrand(Arity<double, T>.ArityI.Repeat(d));
    }
    public static implicit operator BaseDoubleBl<T, BRAND, BOOL>(double d) {
      return ((DoubleBl)d);
    }
    public static implicit operator BaseDoubleBl<T, BRAND, BOOL>(IntBl d) {
      return ((DoubleBl)d);
    }
    public static implicit operator BaseDoubleBl<T, BRAND, BOOL>(int d) {
      return ((DoubleBl)((double) d));
    }
    public static implicit operator BaseDoubleBl<T, BRAND, BOOL>(Random r) {
      var Cs = new Expr<double>[Arity<double, T>.ArityI.Count];
      for (int i = 0; i < Arity<double, T>.ArityI.Count; i++) Cs[i] = new RandomExpr(r);
      return ToBrand(Arity<double, T>.ArityI.Make(Cs));
    }
    public override BRAND Lerp0(BRAND other, DoubleBl t) {
      return ToBrand(DoubleOperators0<T>.Lerp.Instance.Make(this, other, t));
    }

    public static BRAND operator +(BaseDoubleBl<T, BRAND, BOOL> bA, BaseDoubleBl<T, BRAND, BOOL> bB) { return bA.Add(bB); }
    
    public static BRAND operator -(BaseDoubleBl<T, BRAND, BOOL> bA, BaseDoubleBl<T, BRAND, BOOL> bB) { return bA.Subtract(bB); }
    public static BRAND operator *(BaseDoubleBl<T, BRAND, BOOL> bA, BaseDoubleBl<T, BRAND, BOOL> bB) { return bA.Multiply(bB); }

    public static BRAND operator /(BaseDoubleBl<T, BRAND, BOOL> bA, BaseDoubleBl<T, BRAND, BOOL> bB) { return bA.  Divide(bB); }
    public static BRAND operator %(BaseDoubleBl<T, BRAND, BOOL> bA, BaseDoubleBl<T, BRAND, BOOL> bB) { return bA.  Modulo(bB); }
    public static BOOL operator >(BaseDoubleBl<T, BRAND, BOOL> bA, BaseDoubleBl<T, BRAND, BOOL> bB) { return bA.GreaterThan(bB); }
    public static BOOL operator <(BaseDoubleBl<T, BRAND, BOOL> bA, BaseDoubleBl<T, BRAND, BOOL> bB) { return bA.LessThan(bB); }
    public static BOOL operator >=(BaseDoubleBl<T, BRAND, BOOL> bA, BaseDoubleBl<T, BRAND, BOOL> bB) { return bA.GreaterThanOrEqual(bB); }
    public static BOOL operator <=(BaseDoubleBl<T, BRAND, BOOL> bA, BaseDoubleBl<T, BRAND, BOOL> bB) { return bA.LessThanOrEqual(bB); }
    
    private static BRAND Convert(int n) {
      DoubleBl d = n;
      if (Arity<double, T>.ArityI.Count == 1) return (BRAND)(object)d;
      var Cs = new Expr<double>[Arity<double, T>.ArityI.Count];
      for (int i = 0; i < Cs.Length; i++) Cs[i] = d;
      return FactoryIntBl<BRAND, BOOL>.Apply(Cs);
    }
    public BRAND Exp { get { return ToBrand(DoubleOperators0<T>.Exp.Instance.Make(this)); } }
    public BRAND Log { get { return ToBrand(DoubleOperators0<T>.Log.Instance.Make(this)); } }
    public BRAND Exp2 { get { return ToBrand(DoubleOperators0<T>.Exp2.Instance.Make(this)); } }
    public BRAND Log2 { get { return ToBrand(DoubleOperators0<T>.Log2.Instance.Make(this)); } }
    public BRAND Exp10 { get { return ToBrand(DoubleOperators0<T>.Exp10.Instance.Make(this)); } }
    public BRAND Log10 { get { return ToBrand(DoubleOperators0<T>.Log10.Instance.Make(this)); } }
    public BRAND Pow(BaseDoubleBl<T, BRAND, BOOL> Power) { return ToBrand(DoubleOperators0<T>.Pow.Instance.Make(this, Power)); }

    public BRAND Pow(DoubleBl d) {
      return ToBrand(DoubleOperators0<T>.Pow.Instance.Make(this, (BaseDoubleBl<T, BRAND, BOOL>) d));
      //throw new NotSupportedException();
    }
    public BRAND Pow(IntBl d) {
      return Pow((DoubleBl)d);
    }
    public BRAND Pow(double d) {
      return Pow((DoubleBl)d);
    }
    public BRAND Pow(int d) {
      return Pow((DoubleBl)(double) d);
    }

    //public BRAND Pow(BaseDoubleBl<T,BRAND,BOOL> Power) { return Pow((BRAND) Power); }



    public BRAND LogN(BRAND Base) { return ToBrand(DoubleOperators0<T>.LogN.Instance.Make(this, Base));}
    public BRAND Recip { get { return ToBrand(DoubleOperators0<T>.Recip.Instance.Make(this)); } }
    public BRAND Square { get { return ToBrand(DoubleOperators0<T>.Square.Instance.Make(this)); } }
    public BRAND Sqrt { get { return ToBrand(DoubleOperators0<T>.Sqrt.Instance.Make(this)); } }
    public BRAND Rsqrt { get { return ToBrand(DoubleOperators0<T>.Rsqrt.Instance.Make(this)); } }
    public DoubleBl Dot(BRAND Other) { return Semantics.Normal.Instance.Transform<DoubleBl>(DoubleOperators0<T>.Dot.Instance.Make(this, Other)); }
    public DoubleBl LengthSquared {
      get {
        if (IsNormalized) return 1d;
        return DoubleOperators0<T>.LengthSquared.Instance.Make(this);
      }
    }
    public virtual DoubleBl Length {
      get {
        if (IsNormalized) return 1d;
        return DoubleOperators0<T>.Length.Instance.Make(this);
      }
    }
    public BRAND Normalize {
      get {
        if (IsNormalized) return this;
        var ret = ToBrand(DoubleOperators0<T>.Normalize.Instance.Make(this));
        ret.IsNormalized = true;
        return ret;
      }
    }
    /// <summary>
    /// Optimization. True if value is conservatiely known to be normalized.
    /// </summary>
    public bool IsNormalized { get; internal set; }


    public BRAND Ddx { get { return ToBrand(DoubleOperators0<T>.Ddx.Instance.Make(this)); } }
    public BRAND Ddy { get { return ToBrand(DoubleOperators0<T>.Ddy.Instance.Make(this)); } }
    public BRAND Fwidth { get { return ToBrand(DoubleOperators0<T>.FWidth.Instance.Make(this)); } }
    public BRAND Frac { get { return ToBrand(DoubleOperators0<T>.Frac.Instance.Make(this)); } }
    public BRAND Ldexp(BRAND other) {
      return ToBrand(DoubleOperators0<T>.Ldexp.Instance.Make(this, other));
    }
    public abstract BOOL IsInfinity { get; }
    public abstract BOOL IsNaN { get; }
    public abstract BOOL IsPositiveInfinity { get; }
    public abstract BOOL IsNegativeInfinity { get; }
    public abstract BRAND Smoothstep(BRAND b, BRAND x);

    public PointBl this[int i, int j] { get { return new Swizzle<double, T, Vec2<double>>(i, j).Make(this); } set { this[i, j].Bind = value; } }
    public Point3DBl this[int i, int j, int k] { get { return new Swizzle<double, T, Vec3<double>>(i, j, k).Make(this); } set { this[i, j, k].Bind = value; } }
    public Point4DBl this[int i, int j, int k, int l] { get { return new Swizzle<double, T, Vec4<double>>(i, j, k, l).Make(this); } set { this[i, j, k, l].Bind = value; } }

    public virtual BRAND To3D {
      get {
        var Cs = new Expr<double>[Arity<double, T>.ArityI.Count];
        for (int i = 0; i < Cs.Length; i++) Cs[i] = this[i] * 2d - 1d;
        return ToBrand(Arity<double,T>.ArityI.Make(Cs));
      }
    }
    public virtual BRAND ToScreen {
      get {
        var Cs = new Expr<double>[Arity<double, T>.ArityI.Count];
        for (int i = 0; i < Cs.Length; i++) Cs[i] = (this[i] + 1d) / 2d;
        return ToBrand(Arity<double, T>.ArityI.Make(Cs));
      }
    }

    static BaseDoubleBl() {
      Point4DBl.CheckInit();
      Point3DBl.CheckInit();
    }
  }
  public abstract partial class BaseDoubleBl<BRAND> : BaseDoubleBl<double, BRAND, BoolBl> where BRAND : BaseDoubleBl<BRAND> {
    public BaseDoubleBl(Expr<double> e) : base(e) { }
    public BaseDoubleBl() : base() { }
    public static explicit operator IntBl(BaseDoubleBl<BRAND> d) {
      return d.Coerce<IntBl>();
    }
    public static implicit operator DoubleBl(BaseDoubleBl<BRAND> d) { return new DoubleBl(d.Underlying); }
    public static implicit operator PointBl(BaseDoubleBl<BRAND> b) { return new PointBl(b, b); }
    public static implicit operator Point3DBl(BaseDoubleBl<BRAND> b) { return new Point3DBl(b, b, b); }
    public static implicit operator Point4DBl(BaseDoubleBl<BRAND> b) { return new Point4DBl(b, b, b, b); }
    public override BoolBl IsInfinity { get { return DoubleOperators<double, bool>.IsInfinity.Instance.Make(this); } }
    public override BoolBl IsNaN { get { return DoubleOperators<double, bool>.IsNaN.Instance.Make(this); } }
    public override BoolBl IsPositiveInfinity { get { return DoubleOperators<double, bool>.IsPositiveInfinity.Instance.Make(this); } }
    public override BoolBl IsNegativeInfinity { get { return DoubleOperators<double, bool>.IsNegativeInfinity.Instance.Make(this); } }
    public DoubleBl Sign { get { return Ops.Sign.Instance.Make(this); } }
    public BRAND0 Lerp<BRAND0>(BRAND0 From, BRAND0 To) where BRAND0 : ISupportsLerp<BRAND0> {
      return From.Lerp0(To, this);
    }
    public ColorBl Lerp(ColorBl A, ColorBl B) { return Lerp<ColorBl>(A, B); } 

    public override BRAND Smoothstep(BRAND b, BRAND x) {
      return ToBrand(DoubleOperators<double, bool>.Smoothstep.Instance.Make(this, b, x));
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
  }

  public partial class DoubleBl : BaseDoubleBl<DoubleBl>, IMonoNumericBl<double, DoubleBl> {
    public DoubleBl(Expr<double> e) : base(e) { }
    public DoubleBl() : base() { }

    public DoubleBl One { get { return 1d; } }


    public static implicit operator DoubleBl(Expr<double> Value) { return new DoubleBl(Value); }
    public static implicit operator DoubleBl(double Value) { return new Constant<double>(Value); }
    public static implicit operator DoubleBl(IntBl Value) { return Value.ToDouble<DoubleBl>(); }
    public static implicit operator DoubleBl(Random r) {
      return ((BaseDoubleBl<double, DoubleBl, BoolBl>)r);
    }
    static DoubleBl() { 
      Register(e => e);
      Arity1<double>.CheckInit();
    }
  }
  public partial interface IMultiDoubleBl<BRAND, BOOL> : IDoubleBl<BRAND, BOOL>
    where BRAND : IMultiDoubleBl<BRAND, BOOL>
    where BOOL : IBoolBl<BOOL> { }


  public abstract partial class MultiDoubleBl<T, BRAND, B, BOOL, ARITY> : BaseDoubleBl<T, BRAND, BOOL>, IMultiDoubleBl<BRAND,BOOL>
    where BRAND : MultiDoubleBl<T, BRAND, B, BOOL, ARITY>
    where BOOL : BaseBoolBl<B, BOOL>
    where ARITY : MultiArity<double, T, ARITY>, new() {
    public MultiDoubleBl(Expr<T> Underlying) : base(Underlying) { }
    public MultiDoubleBl() : base() { }
    public static implicit operator MultiDoubleBl<T, BRAND, B, BOOL, ARITY>(DoubleBl d) {
      return ToBrand(Arity<double, T, ARITY>.ArityI.Repeat(d));
    }
    static MultiDoubleBl() {
      Extensions.CheckInit<ARITY>();
    }
    public override BOOL IsInfinity {
      get {
        return BaseBoolBl<B, BOOL>.ToBrand(
          DoubleOperators<T, B>.IsInfinity.Instance.Make(this)
        );
      }
    }
    public override BOOL IsNaN {
      get {
        return BaseBoolBl<B, BOOL>.ToBrand(
          DoubleOperators<T, B>.IsNaN.Instance.Make(this)
        );
      }
    }
    public override BOOL IsPositiveInfinity {
      get {
        return BaseBoolBl<B, BOOL>.ToBrand(
          DoubleOperators<T, B>.IsPositiveInfinity.Instance.Make(this)
        );
      }
    }
    public override BOOL IsNegativeInfinity {
      get {
        return BaseBoolBl<B, BOOL>.ToBrand(
          DoubleOperators<T, B>.IsNegativeInfinity.Instance.Make(this)
        );
      }
    }
    public override BRAND Smoothstep(BRAND b, BRAND x) {
      return ToBrand(DoubleOperators<T, B>.Smoothstep.Instance.Make(this, b, x));
    }

    
    public override BOOL GreaterThan(BRAND other) {
      return BaseBoolBl<B, BOOL>.ToBrand(
        NumericOperators<T, double, B>.GreaterThan.Instance.Make(this, other)
      );
    }
    public override BOOL LessThan(BRAND other) {
      return BaseBoolBl<B, BOOL>.ToBrand(
        NumericOperators<T, double, B>.LessThan.Instance.Make(this, other)
      );
    }
    public override BOOL GreaterThanOrEqual(BRAND other) {
      return BaseBoolBl<B, BOOL>.ToBrand(
        NumericOperators<T, double, B>.GreaterThanOrEqual.Instance.Make(this, other)
      );
    }
    public override BOOL LessThanOrEqual(BRAND other) {
      return BaseBoolBl<B, BOOL>.ToBrand(
        NumericOperators<T, double, B>.LessThanOrEqual.Instance.Make(this, other)
      );
    }
  }
  public static partial class Swizzles {
    public static PointBl XY<BRAND>(this IDoubleBl<BRAND> p) where BRAND : HasXY<DoubleBl>, IDoubleBl<BRAND> { return p[0, 1]; }
    public static PointBl XX<BRAND>(this IDoubleBl<BRAND> p) where BRAND : IDoubleBl<BRAND> { return p[0, 0]; }
    public static PointBl YX<BRAND>(this IDoubleBl<BRAND> p) where BRAND : HasXY<DoubleBl>, IDoubleBl<BRAND> { return p[1, 0]; }
    public static PointBl YY<BRAND>(this IDoubleBl<BRAND> p) where BRAND : HasXY<DoubleBl>, IDoubleBl<BRAND> { return p[1, 1]; }

    public static Point3DBl XYX<BRAND>(this IDoubleBl<BRAND> p) where BRAND : HasXY<DoubleBl>, IDoubleBl<BRAND> { return p[0, 1, 0]; }
    public static Point3DBl XXX<BRAND>(this IDoubleBl<BRAND> p) where BRAND : IDoubleBl<BRAND> { return p[0, 0, 0]; }
    public static Point3DBl YXX<BRAND>(this IDoubleBl<BRAND> p) where BRAND : HasXY<DoubleBl>, IDoubleBl<BRAND> { return p[1, 0, 0]; }
    public static Point3DBl YYX<BRAND>(this IDoubleBl<BRAND> p) where BRAND : HasXY<DoubleBl>, IDoubleBl<BRAND> { return p[1, 1, 0]; }
    public static Point3DBl XYY<BRAND>(this IDoubleBl<BRAND> p) where BRAND : HasXY<DoubleBl>, IDoubleBl<BRAND> { return p[0, 1, 1]; }
    public static Point3DBl XXY<BRAND>(this IDoubleBl<BRAND> p) where BRAND : HasXY<DoubleBl>, IDoubleBl<BRAND> { return p[0, 0, 1]; }
    public static Point3DBl YXY<BRAND>(this IDoubleBl<BRAND> p) where BRAND : HasXY<DoubleBl>, IDoubleBl<BRAND> { return p[1, 0, 1]; }
    public static Point3DBl YYY<BRAND>(this IDoubleBl<BRAND> p) where BRAND : HasXY<DoubleBl>, IDoubleBl<BRAND> { return p[1, 1, 1]; }

    public static Point4DBl XXYX<BRAND>(this IDoubleBl<BRAND> p) where BRAND : HasXY<DoubleBl>, IDoubleBl<BRAND> { return p[0, 0, 1, 0]; }
    public static Point4DBl XXXX<BRAND>(this IDoubleBl<BRAND> p) where BRAND : IDoubleBl<BRAND> { return p[0, 0, 0, 0]; }
    public static Point4DBl XYXX<BRAND>(this IDoubleBl<BRAND> p) where BRAND : HasXY<DoubleBl>, IDoubleBl<BRAND> { return p[0, 1, 0, 0]; }
    public static Point4DBl XYYX<BRAND>(this IDoubleBl<BRAND> p) where BRAND : HasXY<DoubleBl>, IDoubleBl<BRAND> { return p[0, 1, 1, 0]; }
    public static Point4DBl XXYY<BRAND>(this IDoubleBl<BRAND> p) where BRAND : HasXY<DoubleBl>, IDoubleBl<BRAND> { return p[0, 0, 1, 1]; }
    public static Point4DBl XXXY<BRAND>(this IDoubleBl<BRAND> p) where BRAND : HasXY<DoubleBl>, IDoubleBl<BRAND> { return p[0, 0, 0, 1]; }
    public static Point4DBl XYXY<BRAND>(this IDoubleBl<BRAND> p) where BRAND : HasXY<DoubleBl>, IDoubleBl<BRAND> { return p[0, 1, 0, 1]; }
    public static Point4DBl XYYY<BRAND>(this IDoubleBl<BRAND> p) where BRAND : HasXY<DoubleBl>, IDoubleBl<BRAND> { return p[0, 1, 1, 1]; }

    public static Point4DBl YXYX<BRAND>(this IDoubleBl<BRAND> p) where BRAND : HasXY<DoubleBl>, IDoubleBl<BRAND> { return p[1, 0, 1, 0]; }
    public static Point4DBl YXXX<BRAND>(this IDoubleBl<BRAND> p) where BRAND : HasXY<DoubleBl>, IDoubleBl<BRAND> { return p[1, 0, 0, 0]; }
    public static Point4DBl YYXX<BRAND>(this IDoubleBl<BRAND> p) where BRAND : HasXY<DoubleBl>, IDoubleBl<BRAND> { return p[1, 1, 0, 0]; }
    public static Point4DBl YYYX<BRAND>(this IDoubleBl<BRAND> p) where BRAND : HasXY<DoubleBl>, IDoubleBl<BRAND> { return p[1, 1, 1, 0]; }
    public static Point4DBl YXYY<BRAND>(this IDoubleBl<BRAND> p) where BRAND : HasXY<DoubleBl>, IDoubleBl<BRAND> { return p[1, 0, 1, 1]; }
    public static Point4DBl YXXY<BRAND>(this IDoubleBl<BRAND> p) where BRAND : HasXY<DoubleBl>, IDoubleBl<BRAND> { return p[1, 0, 0, 1]; }
    public static Point4DBl YYXY<BRAND>(this IDoubleBl<BRAND> p) where BRAND : HasXY<DoubleBl>, IDoubleBl<BRAND> { return p[1, 1, 0, 1]; }
    public static Point4DBl YYYY<BRAND>(this IDoubleBl<BRAND> p) where BRAND : HasXY<DoubleBl>, IDoubleBl<BRAND> { return p[1, 1, 1, 1]; }

    public static Point4DBl ZZZZ<BRAND>(this IDoubleBl<BRAND> p) where BRAND : HasXYZW<DoubleBl>, IDoubleBl<BRAND> { return p[2, 2, 2, 2]; }
    public static Point4DBl WWWW<BRAND>(this IDoubleBl<BRAND> p) where BRAND : HasXYZW<DoubleBl>, IDoubleBl<BRAND> { return p[3, 3, 3, 3]; }

    public static PointBl ZW<BRAND>(this IDoubleBl<BRAND> p) where BRAND : HasXYZW<DoubleBl>, IDoubleBl<BRAND> { return p[2,3]; }

    public static PointBl XZ<BRAND>(this IDoubleBl<BRAND> p) where BRAND : HasXYZ<DoubleBl>, IDoubleBl<BRAND> { return p[0, 2]; }
    public static PointBl YZ<BRAND>(this IDoubleBl<BRAND> p) where BRAND : HasXYZ<DoubleBl>, IDoubleBl<BRAND> { return p[1, 2]; }
    public static PointBl ZX<BRAND>(this IDoubleBl<BRAND> p) where BRAND : HasXYZ<DoubleBl>, IDoubleBl<BRAND> { return p[2, 0]; }
    public static PointBl ZY<BRAND>(this IDoubleBl<BRAND> p) where BRAND : HasXYZ<DoubleBl>, IDoubleBl<BRAND> { return p[2, 1]; }
    public static PointBl ZZ<BRAND>(this IDoubleBl<BRAND> p) where BRAND : HasXYZ<DoubleBl>, IDoubleBl<BRAND> { return p[2, 2]; }


    public static Point3DBl XYZ<BRAND>(this IDoubleBl<BRAND> p) where BRAND : HasXYZ<DoubleBl>, IDoubleBl<BRAND> { return p[0, 1, 2]; }
    public static Point3DBl XXZ<BRAND>(this IDoubleBl<BRAND> p) where BRAND : HasXYZ<DoubleBl>, IDoubleBl<BRAND> { return p[0, 0, 2]; }
    public static Point3DBl YXZ<BRAND>(this IDoubleBl<BRAND> p) where BRAND : HasXYZ<DoubleBl>, IDoubleBl<BRAND> { return p[1, 0, 2]; }
    public static Point3DBl YYZ<BRAND>(this IDoubleBl<BRAND> p) where BRAND : HasXYZ<DoubleBl>, IDoubleBl<BRAND> { return p[1, 1, 2]; }

    public static Point3DBl XZX<BRAND>(this IDoubleBl<BRAND> p) where BRAND : HasXYZ<DoubleBl>, IDoubleBl<BRAND> { return p[0, 2, 0]; }
    public static Point3DBl YZX<BRAND>(this IDoubleBl<BRAND> p) where BRAND : HasXYZ<DoubleBl>, IDoubleBl<BRAND> { return p[1, 2, 0]; }
    public static Point3DBl XZY<BRAND>(this IDoubleBl<BRAND> p) where BRAND : HasXYZ<DoubleBl>, IDoubleBl<BRAND> { return p[0, 2, 1]; }
    public static Point3DBl YZY<BRAND>(this IDoubleBl<BRAND> p) where BRAND : HasXYZ<DoubleBl>, IDoubleBl<BRAND> { return p[1, 2, 1]; }
    public static Point3DBl XZZ<BRAND>(this IDoubleBl<BRAND> p) where BRAND : HasXYZ<DoubleBl>, IDoubleBl<BRAND> { return p[0, 2, 2]; }
    public static Point3DBl YZZ<BRAND>(this IDoubleBl<BRAND> p) where BRAND : HasXYZ<DoubleBl>, IDoubleBl<BRAND> { return p[1, 2, 2]; }

    public static Point3DBl ZZX<BRAND>(this IDoubleBl<BRAND> p) where BRAND : HasXYZ<DoubleBl>, IDoubleBl<BRAND> { return p[2, 2, 0]; }
    public static Point3DBl ZZY<BRAND>(this IDoubleBl<BRAND> p) where BRAND : HasXYZ<DoubleBl>, IDoubleBl<BRAND> { return p[2, 2, 1]; }
    public static Point3DBl ZZZ<BRAND>(this IDoubleBl<BRAND> p) where BRAND : HasXYZ<DoubleBl>, IDoubleBl<BRAND> { return p[2, 2, 2]; }

    public static Point3DBl ZYX<BRAND>(this IDoubleBl<BRAND> p) where BRAND : HasXYZ<DoubleBl>, IDoubleBl<BRAND> { return p[2, 1, 0]; }
    public static Point3DBl ZXX<BRAND>(this IDoubleBl<BRAND> p) where BRAND : HasXYZ<DoubleBl>, IDoubleBl<BRAND> { return p[2, 0, 0]; }
    public static Point3DBl ZYY<BRAND>(this IDoubleBl<BRAND> p) where BRAND : HasXYZ<DoubleBl>, IDoubleBl<BRAND> { return p[2, 1, 1]; }
    public static Point3DBl ZXY<BRAND>(this IDoubleBl<BRAND> p) where BRAND : HasXYZ<DoubleBl>, IDoubleBl<BRAND> { return p[2, 0, 1]; }
    public static Point3DBl ZYZ<BRAND>(this IDoubleBl<BRAND> p) where BRAND : HasXYZ<DoubleBl>, IDoubleBl<BRAND> { return p[2, 1, 2]; }
    public static Point3DBl ZXZ<BRAND>(this IDoubleBl<BRAND> p) where BRAND : HasXYZ<DoubleBl>, IDoubleBl<BRAND> { return p[2, 0, 2]; }
    public static ColorBl ZYXW(this ColorBl c) { return c[2, 1, 0, 3]; }
  }
  public partial interface IHasPortableBl<PORTABLE, MONO> : IBrand
    where PORTABLE : IHasPortableBl<PORTABLE, MONO>
    where MONO : IValueBrand<MONO, MONO> {
    PORTABLE Portable { get; }
    MONO this[IntBl Idx] { get; set; }
  }
  public partial interface IDimPointBl<DIM> : IBrand where DIM : Matrices.DNum<DIM> { }

  public partial interface IBasePointBl<PORTABLE> : IHasPortableBl<PORTABLE, DoubleBl> where PORTABLE : IBasePointBl<PORTABLE> { }

  public partial interface IDimPointBl<PORTABLE, DIM> : IDimPointBl<DIM>, IBasePointBl<PORTABLE>
    where DIM : Matrices.DNum<DIM>
    where PORTABLE : IDimPointBl<PORTABLE, DIM> { }

  public partial interface IDimPointBl<PORTABLE, DIM, DIM_NEXT> :
    IDimPointBl<PORTABLE, DIM>
    where DIM : Matrices.DNum<DIM>, Matrices.INextDim<DIM, DIM_NEXT>
    where DIM_NEXT : Matrices.DNum<DIM_NEXT>, Matrices.IPrevDim<DIM_NEXT, DIM>
    where PORTABLE : IDimPointBl<PORTABLE, DIM, DIM_NEXT> { }


  public partial interface IPointBl<BRAND> : IBrand<BRAND>, IDimPointBl<PointBl, Matrices.D2, Matrices.D3> where BRAND : IPointBl<BRAND> { }
  public partial interface IPoint3DBl<BRAND> : IBrand<BRAND>, IDimPointBl<Point3DBl, Matrices.D3, Matrices.D4> where BRAND : IPoint3DBl<BRAND> { }
  public partial interface IPoint4DBl<BRAND> : IBrand<BRAND>, IDimPointBl<Point4DBl, Matrices.D4> where BRAND : IPoint4DBl<BRAND> { }

  public abstract partial class PointBl<T, BRAND, ARITY> : MultiDoubleBl<T, BRAND, Vec2<bool>, Bool2Bl, ARITY>, HasXY<DoubleBl>, IPointBl<BRAND>
    where BRAND : PointBl<T, BRAND, ARITY>
    where ARITY : Arity2<double, T, ARITY>, new() {
    public PointBl(Expr<T> Underlying) : base(Underlying) { }
    public PointBl() : base() { }
    public PointBl(DoubleBl bA, DoubleBl bB) : base(Arity2<double, T, ARITY>.ArityI.Make(bA, bB)) { }

    public static explicit operator PointBl(PointBl<T, BRAND, ARITY> p) { 
      return VecConvert<T, double, Vec2<double>>.ToVec.Instance.Make(p); 
    }
    public PointBl Portable { get { return (PointBl) this; } }
    public static explicit operator PointBl<T, BRAND, ARITY>(PointBl p) { return ToBrand(VecConvert<T, double, Vec2<double>>.FromVec.Instance.Make(p)); }

    public DoubleBl X { get { return  Arity<double, T, ARITY>.ArityI.Access(Underlying, 0); } set { X.Bind = value; } }
    public DoubleBl Y { get { return Arity<double, T, ARITY>.ArityI.Access(Underlying, 1); } set { Y.Bind = value; } }

    public BRAND MapY(Func<DoubleBl, DoubleBl> F) { return Make((X), F(Y)); }
    public BRAND MapX(Func<DoubleBl, DoubleBl> F) { return Make(F(X), (Y)); }

    public Func<Func<DoubleBl,A>,Func<DoubleBl,B>,C> Cart<A, B, C>(Func<A, B, C> F)
      where A : Brand<A>
      where B : Brand<B>
      where C : Brand<C> {
      return (aF, bF) => F(aF(this[0]), bF(this[1]));
    }
    public Func<Func<DoubleBl, DoubleBl>, Func<DoubleBl, DoubleBl>, C> Cart<C>(Func<DoubleBl, DoubleBl, C> F)
      where C : Brand<C> {
      return (aF, bF) => F(aF(this[0]), bF(this[1]));
    }
    /// <summary>
    /// Compute 2D point cross product, which is useful in same side computations.
    /// </summary>
    public DoubleBl Cross(BRAND Other) {
      return X * Other.Y - Y * Other.X;
    }

    public static BRAND Make(DoubleBl bA, DoubleBl bB) {
      return ToBrand(Arity2<double, T, ARITY>.ArityI.Make(bA, bB));
    }
    /// <summary>
    /// A vector perpendicular to this one.
    /// </summary>
    public BRAND Perpendicular { get { return Make(-Y, X); } set { Perpendicular.Bind = value; } }
    /// <summary>
    /// Rotate this point by specified vector.
    /// </summary>
    /// <param name="w">A vector.</param>
    public BRAND Rotate(BRAND w) {
      var v = w.Normalize;
      v = this * v.X + Perpendicular * v.Y;
      return v;
    }
  }
  public abstract partial class BasePointBl<BRAND> : PointBl<Vec2<double>, BRAND, Vec2Arity<double>> where BRAND : BasePointBl<BRAND> {
    public BasePointBl(Expr<Vec2<double>> Underlying) : base(Underlying) { }
    public BasePointBl() : base() { }
    public BasePointBl(DoubleBl bA, DoubleBl bB) : base(bA, bB) { }
  }
  public partial class PointBl : BasePointBl<PointBl>, ICoreBrandT, ICanWrapWPFPoint {
    public PointBl() : base() { }
    public PointBl(Expr<Vec2<double>> Underlying) : base(Underlying) { }
    public PointBl(DoubleBl bA, DoubleBl bB) : base(bA, bB) { }
    public static implicit operator PointBl(Random r) { return (PointBl)((BaseDoubleBl<Vec2<double>, PointBl, Bool2Bl>)r); }
    public static implicit operator PointBl(Expr<Vec2<double>> Value) { return new PointBl(Value); }
    public static implicit operator PointBl(Vec2<double> Value) { return new Constant<Vec2<double>>(Value); }
    public static implicit operator PointBl(double Value) { return (DoubleBl)Value; }
    static PointBl() {
      Register(e => e);
    }
    private static readonly PointBl[] CounterClockwise0 = {
      new PointBl(-1, -1),
      new PointBl(+1, -1),
      new PointBl(+1, +1),
      new PointBl(-1, +1),
    };
    public static SimpleList<PointBl> CounterClockwise { get { return CounterClockwise0; } }
    /// <summary>
    /// Create 3D point by adding an x-value.
    /// </summary>
    public Point3DBl InsertX(DoubleBl X) { return new Point3DBl(X, this); }
    /// <summary>
    /// Create 3D point by adding an y-value.
    /// </summary>
    public Point3DBl InsertY(DoubleBl Y) { return new Point3DBl(this.X, Y, this.Y); }
    /// <summary>
    /// Create 3D point by adding an z-value.
    /// </summary>
    public Point3DBl InsertZ(DoubleBl Z) { return new Point3DBl(this, Z); }


    public Point3DBl Insert(int n, DoubleBl D) {
      return n == 0 ? InsertX(D) : n == 1 ? InsertY(D) : InsertZ(D);
    }
  }
  public abstract partial class Point3DBl<T, BRAND, ARITY> : MultiDoubleBl<T, BRAND, Vec3<bool>, Bool3Bl, ARITY>, HasXYZ<DoubleBl>, IPoint3DBl<BRAND>
    where BRAND : Point3DBl<T, BRAND, ARITY>
    where ARITY : Arity3<double, T, ARITY>, new() {
    public Point3DBl(Expr<T> Underlying) : base(Underlying) { }
    public Point3DBl() : base() { }
    public Point3DBl(DoubleBl bA, DoubleBl bB, DoubleBl bC) : base(Arity3<double, T, ARITY>.ArityI.Make(bA, bB, bC)) { }
    public Point3DBl(PointBl bAB, DoubleBl bC)
      : this(TupleConstruct<double,
        Vec2<double>, double, T>.Instance.Make(bAB.Underlying, bC.Underlying)) { }
    public Point3DBl(DoubleBl bA, PointBl bBC)
      : this(TupleConstruct<double,
        double, Vec2<double>, T>.Instance.Make(bA.Underlying, bBC.Underlying)) { }
    public Point3DBl Portable { get { return VecConvert<T, double, Vec3<double>>.ToVec.Instance.Make(this); } }

    public static explicit operator Point3DBl(Point3DBl<T, BRAND, ARITY> p) {
      return VecConvert<T, double, Vec3<double>>.ToVec.Instance.Make(p);
    }
    public static explicit operator Point3DBl<T, BRAND, ARITY>(Point3DBl p) { return ToBrand(VecConvert<T, double, Vec3<double>>.FromVec.Instance.Make(p)); }


    public DoubleBl X { get { return Arity<double, T, ARITY>.ArityI.Access(Underlying, 0); } set { X.Bind = value; } }
    public DoubleBl Y { get { return Arity<double, T, ARITY>.ArityI.Access(Underlying, 1); } set { Y.Bind = value; } }
    public DoubleBl Z { get { return Arity<double, T, ARITY>.ArityI.Access(Underlying, 2); } set { Z.Bind = value; } }

    public BRAND Cross(BRAND Other) { return ToBrand(DoubleOperators0<T>.Cross.Instance.Make(this, Other)); }


    public BRAND Rotate(Point3DBl v) {
      v = v.Normalize;
      var p0 = this;
      var p1 = Make(-p0.Y, p0.X, p0.Z);
      var p2 = p0.Cross(p1);

      return ((p0 * v.X + p1 * v.Y + p2 * v.Z));
    }

  }
  public abstract partial class BasePoint3DBl<BRAND> : Point3DBl<Vec3<double>, BRAND, Vec3Arity<double>> where BRAND : BasePoint3DBl<BRAND> {
    public BasePoint3DBl(Expr<Vec3<double>> Underlying) : base(Underlying) { }
    public BasePoint3DBl() : base() { }
    public BasePoint3DBl(DoubleBl bA, DoubleBl bB, DoubleBl bC) : base(bA, bB, bC) { }
    public BasePoint3DBl(PointBl bAB, DoubleBl bC) : base(bAB, bC) { }
    public BasePoint3DBl(DoubleBl X, PointBl YZ) : base(X, YZ) { }
  }
  /// <summary>
  ///     Defines an x-, y-, and z-coordinate in 3-D space.
  /// </summary>
  public partial class Point3DBl : BasePoint3DBl<Point3DBl>, ICoreBrandT, ICanWrapWPFPoint3D {
    public Point3DBl(Expr<Vec3<double>> Underlying) : base(Underlying) { }
    public Point3DBl() : base() { }
    public Point3DBl(DoubleBl X, DoubleBl Y, DoubleBl Z) : base(X, Y, Z) { }
    public Point3DBl(PointBl XY, DoubleBl Z) : base(XY, Z) { }
    public Point3DBl(DoubleBl X, PointBl YZ) : base(X, YZ) { }

    public static implicit operator Point3DBl(Expr<Vec3<double>> Value) { return new Point3DBl(Value); }
    public static implicit operator Point3DBl(Vec3<double> Value) { return new Constant<Vec3<double>>(Value); }
    public static implicit operator Point3DBl(double Value) { return (DoubleBl)Value; }
    public static explicit operator Point3DBl(PointBl P) { return new Point3DBl(P, 0d); }

    static Point3DBl() {
      Register(e => e); 
    }
    public static void CheckInit() {
      Vec4Arity<double>.CheckInit();
      Extensions.Trace("" + ToBrand); 
    }
  }


  public abstract partial class Point4DBl<T, BRAND, ARITY> : MultiDoubleBl<T, BRAND, Vec4<bool>, Bool4Bl, ARITY>, HasXYZW<DoubleBl>, IPoint4DBl<BRAND>
    where BRAND : Point4DBl<T, BRAND, ARITY>
    where ARITY : Arity4<double, T, ARITY>, new() {
    public Point4DBl(Expr<T> Underlying) : base(Underlying) { }
    public Point4DBl() : base() { }
    public Point4DBl(DoubleBl bA, DoubleBl bB, DoubleBl bC, DoubleBl bD) : base(Arity4<double, T, ARITY>.ArityI.Make(bA, bB, bC, bD)) { }
    public Point4DBl(Point3DBl bABC, DoubleBl bD)
      : this(TupleConstruct<double, Vec3<double>, double, T>.Instance.Make(bABC.Underlying, bD.Underlying)) { }
    public Point4DBl(PointBl bAB, PointBl bCD)
      : this(TupleConstruct<double,
        Vec2<double>, Vec2<double>, T>.Instance.Make(bAB.Underlying, bCD.Underlying)) { }
    public Point4DBl Portable { get { return (Point4DBl) this; } }
    public static explicit operator Point4DBl(Point4DBl<T, BRAND, ARITY> p) { return VecConvert<T, double, Vec4<double>>.ToVec.Instance.Make(p); }
    public static explicit operator Point4DBl<T, BRAND, ARITY>(Point4DBl p) { return ToBrand(VecConvert<T, double, Vec4<double>>.FromVec.Instance.Make(p)); }

    public DoubleBl X { get { return Arity<double, T, ARITY>.ArityI.Access(Underlying, 0); } set { X.Bind = value; } }
    public DoubleBl Y { get { return Arity<double, T, ARITY>.ArityI.Access(Underlying, 1); } set { Y.Bind = value; } }
    public DoubleBl Z { get { return Arity<double, T, ARITY>.ArityI.Access(Underlying, 2); } set { Z.Bind = value; } }
    public DoubleBl W { get { return Arity<double, T, ARITY>.ArityI.Access(Underlying, 3); } set { W.Bind = value; } }
  }
  public abstract partial class BasePoint4DBl<BRAND> : Point4DBl<Vec4<double>, BRAND, Vec4Arity<double>> where BRAND : BasePoint4DBl<BRAND> {
    public BasePoint4DBl(Expr<Vec4<double>> Underlying) : base(Underlying) { }
    public BasePoint4DBl() : base() { }
    public BasePoint4DBl(DoubleBl bA, DoubleBl bB, DoubleBl bC, DoubleBl bD) : base(bA, bB, bC, bD) { }
    public BasePoint4DBl(Point3DBl bABC, DoubleBl bD) : base(bABC, bD) {}
    public BasePoint4DBl(PointBl bAB, PointBl bCD) : base(bAB, bCD) {}
  }
  /// <summary>
  ///     Represents an x-, y-, z-, and w-coordinate point in world space used in performing
  ///     transformations with non-affine 3-D matrices.
  /// </summary>
  public partial class Point4DBl : BasePoint4DBl<Point4DBl>, ICoreBrandT, ICanWrapWPFPoint4D, ICanWrapWPFRect {
    public Point4DBl(Expr<Vec4<double>> Underlying) : base(Underlying) { }
    public Point4DBl() : base() { }
    public Point4DBl(DoubleBl X, DoubleBl Y, DoubleBl Z, DoubleBl W) : base(X, Y, Z, W) { }
    public Point4DBl(PointBl XY, PointBl ZW) : base(XY, ZW) { }
    public Point4DBl(Point3DBl XYZ, DoubleBl W) : base(XYZ, W) { }
    // a one goes here because of how positions are handled!
    //public Point4DBl(Point3DBl XYZ) : base(XYZ, 1) { }
    //public Point4DBl(PointBl XY) : this(new Point3DBl(XY, 0)) { }


    public static implicit operator Point4DBl(Expr<Vec4<double>> Value) { return new Point4DBl(Value); }
    public static implicit operator Point4DBl(Vec4<double> Value) { return new Constant<Vec4<double>>(Value); }
    public static implicit operator Point4DBl(double Value) { return (DoubleBl)Value; }
    public static implicit operator Point4DBl(Point3DBl Value) { return new Point4DBl(Value, 1d); }
    public static explicit operator Point3DBl(Point4DBl Value) { return Value.XYZ() / Value.W; }
    public static explicit operator PointBl(Point4DBl Value) { return ((Point3DBl) Value).XY(); }



    public static explicit operator Point4DBl(PointBl Value) { return new Point4DBl(Value, new PointBl(0d, 1d)); }
    static Point4DBl() {
      Register(e => e);
    }
    public static void CheckInit() {
      Vec4Arity<double>.CheckInit();
      Extensions.Trace("" + ToBrand); 
    }
  }


  /// <summary>
  ///     Describes a color in terms of red, green, and blue channels.
  /// </summary>
  public class RGBBl : Point3DBl<Vec3<double>, RGBBl, Vec3Arity<double>> {
    public RGBBl(Expr<Vec3<double>> v) : base(v) { }
    public RGBBl(DoubleBl ScR, DoubleBl ScG, DoubleBl ScB) : base(ScR, ScG, ScB) { }
    public RGBBl(DoubleBl ScRGB) : base(new Ops.Swizzle<double,double,Vec3<double>>(0,0,0).Make(ScRGB)) { }
    public static implicit operator RGBBl(Expr<Vec3<double>> v) { return new RGBBl(v); }
    static RGBBl() {
      Register(v => v);
    }
    public override Expr<Vec3<double>> Underlying {
      get {
        return Semantics.Color.Instance.Transform(base.Underlying);
      }
    }
    /// <summary>
    /// The color's red channel.
    /// </summary>
    public DoubleBl ScR { get { return X; } set { ScR.Bind = value; } }
    /// <summary>
    /// The color's green channel.
    /// </summary>
    public DoubleBl ScG { get { return Y; } set { ScG.Bind = value; } }
    /// <summary>
    /// The color's blue channel.
    /// </summary>
    public DoubleBl ScB { get { return Z; } set { ScB.Bind = value; } }
  }


  public abstract partial class ColorBl<T, BRAND, ARITY> : MultiDoubleBl<T, BRAND, Vec4<bool>, Bool4Bl, ARITY>, HasXYZW<DoubleBl>
    where BRAND : ColorBl<T, BRAND, ARITY>
    where ARITY : Arity4<double, T, ARITY>, new() {
    public ColorBl(Expr<T> Underlying) : base(Underlying) { }
    public ColorBl() : base() { }
    public ColorBl(DoubleBl ScR, DoubleBl ScG, DoubleBl ScB, DoubleBl ScA) : base(Arity4<double, T, ARITY>.ArityI.Make(ScR, ScG, ScB, ScA)) { }
    public ColorBl(DoubleBl ScR, DoubleBl ScG, DoubleBl ScB) : base(Arity4<double, T, ARITY>.ArityI.Make(ScR, ScG, ScB, 1d.Bl())) { }

    public ColorBl(RGBBl ScRGB, DoubleBl ScA) : this(TupleConstruct<double, 
      Vec3<double>, double, T>.Instance.Make(ScRGB.Underlying, ScA.Underlying)) { }
    public ColorBl(RGBBl ScRGB) : this(ScRGB, 1d) { }
    public ColorBl Portable { get { return this; } }
    /*
    public override Expr<T> Underlying {
      get {
        return Semantics.Color.Instance.Transform(base.Underlying);
      }
    }*/

    public static implicit operator ColorBl(ColorBl<T, BRAND, ARITY> p) { return VecConvert<T, double, Vec4<double>>.ToVec.Instance.Make(p); }
    public static implicit operator ColorBl<T, BRAND, ARITY>(ColorBl p) { return ToBrand(VecConvert<T, double, Vec4<double>>.FromVec.Instance.Make(p)); }
    /// <summary>
    /// The color's red channel.
    /// </summary>
    public DoubleBl ScR { get { return X; } set { ScR.Bind = value; } }
    /// <summary>
    /// The color's green channel.
    /// </summary>
    public DoubleBl ScG { get { return Y; } set { ScG.Bind = value; } }
    /// <summary>
    /// The color's blue channel.
    /// </summary>
    public DoubleBl ScB { get { return Z; } set { ScB.Bind = value; } }
    /// <summary>
    /// The color's alpha channel.
    /// </summary>
    public DoubleBl ScA { get { return W; } set { ScA.Bind = value; } }
    public DoubleBl X { get { return Arity<double, T, ARITY>.ArityI.Access(Underlying, 0); } set { X.Bind = value; } }
    public DoubleBl Y { get { return Arity<double, T, ARITY>.ArityI.Access(Underlying, 1); } set { Y.Bind = value; } }
    public DoubleBl Z { get { return Arity<double, T, ARITY>.ArityI.Access(Underlying, 2); } set { Z.Bind = value; } }
    public DoubleBl W { get { return Arity<double, T, ARITY>.ArityI.Access(Underlying, 3); } set { W.Bind = value; } }
    public static ColorBl operator *(RGBBl ScRGB, ColorBl<T, BRAND, ARITY> c) { return ((ColorBl)ScRGB) * c; }
    public static ColorBl operator *(ColorBl<T, BRAND, ARITY> c, RGBBl ScRGB) { return c * ((ColorBl)ScRGB); }
    public static ColorBl operator +(ColorBl<T, BRAND, ARITY> c, RGBBl ScRGB) { return c + ((ColorBl)ScRGB); }
    public static ColorBl operator +(RGBBl ScRGB, ColorBl<T, BRAND, ARITY> c) { return ((ColorBl)ScRGB) + c; }

    public override BRAND Add(BRAND other) {
      return base.Add(other);
    }

    public static ColorBl operator *(DoubleBl t, ColorBl<T, BRAND, ARITY> c) { return new ColorBl(new RGBBl (t), 1) * c; }
    public static ColorBl operator *(ColorBl<T, BRAND, ARITY> c, DoubleBl t) { return c * new ColorBl(new RGBBl(t), 1); }
    //public static ColorBl operator *(ColorBl<T, BRAND, ARITY> c1, ColorBl<T, BRAND, ARITY> c2) { return c1 * c2; }

    /// <summary>
    /// The RGB channel of this color.
    /// </summary>
    public RGBBl ScRGB { get { return this.XYZ().Underlying; } set { ScRGB.Bind = value; } }
    static ColorBl() {
      Extensions.CheckInit<ARITY>();
    }
  }
  /// <summary>
  ///     Describes a color in terms of alpha, red, green, and blue channels.
  /// </summary>
  public partial class ColorBl : ColorBl<Vec4<double>, ColorBl, Vec4Arity<double>> {
    public ColorBl(Expr<Vec4<double>> Underlying) : base(Underlying) { }
    public ColorBl(DoubleBl ScR, DoubleBl ScG, DoubleBl ScB, DoubleBl ScA) : base(ScR, ScG, ScB, ScA) { }
    public ColorBl(RGBBl ScRGB, DoubleBl ScA) : base(ScRGB, ScA) { }
    public ColorBl(DoubleBl ScR, DoubleBl ScG, DoubleBl ScB) : base(ScR, ScG, ScB) { }
    public static implicit operator ColorBl(Random r) {
      return new ColorBl((RGBBl)r, 1);
    }

    public static ColorBl RandomFor(Random r, Expr Key) {
      return new ColorBl(
        new RandomExpr2(r, Key),
        new RandomExpr2(r, Key),
        new RandomExpr2(r, Key), 1d);
    }

    public ColorBl Over(ColorBl pB) {
      var pA = this;
      var result = pA.ScRGB * pA.ScA + pB.ScRGB * (1d - pA.ScA) * pB.ScA;
      //Console.WriteLine("RES: " + result);
      var alpha = pA.ScA + (1d - pA.ScA) * pB.ScA;
      //Console.WriteLine("ALPHA " + alpha);
      // unscale, really necessary?
      return new ColorBl(result / alpha, alpha);
    }
    public ColorBl Additive(ColorBl cB) {
      var cA = this;
      return new ColorBl(cA.ScRGB + cB.ScRGB, (cA.ScA + cB.ScB) / 2d);
    }

    public static implicit operator ColorBl(Expr<Vec4<double>> Value) { return new ColorBl(Value); }
    public static implicit operator ColorBl(Vec4<double> Value) { return new Constant<Vec4<double>>(Value); }
    public static implicit operator ColorBl(Point4DBl Value) { return (Value.Underlying); }
    public static implicit operator ColorBl(RGBBl Value) { return new ColorBl(Value, 1); }
    public static explicit operator ColorBl(Point3DBl Value) { return new ColorBl(Value.Underlying, 1); }
    static ColorBl() {
      Register(e => e);
    }

  }
  public partial interface IIntBl<BRAND, BOOL> : INumericBl<BRAND, BOOL> 
    where BRAND : IIntBl<BRAND, BOOL>
    where BOOL : IBoolBl<BOOL> {
  }
  public partial interface IToDoubleBl<BOOL> : IBrand
    where BOOL : IBoolBl<BOOL> {
    DOUBLE ToDouble<DOUBLE>() where DOUBLE : IDoubleBl<DOUBLE, BOOL>;
  }
  public static partial class FactoryIntBl<DOUBLE, BOOL>
    where BOOL : IBoolBl<BOOL>
    where DOUBLE : IDoubleBl<DOUBLE, BOOL> {
    public static Func<Expr<double>[], DOUBLE> Apply { get; internal set; }
  }

  public abstract partial class BaseIntBl<T, BRAND, BOOL> : BaseNumericBl<T, int, BRAND, BOOL, IntBl>, IIntBl<BRAND, BOOL>, IToDoubleBl<BOOL>
    where BRAND : BaseIntBl<T, BRAND, BOOL>
    where BOOL : IBoolBl<BOOL> {
    public BaseIntBl(Expr<T> e) : base(e) { }
    public BaseIntBl() : base() { }

    public abstract DOUBLE ToDouble<DOUBLE>() where DOUBLE : IDoubleBl<DOUBLE, BOOL>;

    public override BRAND Lerp0(BRAND Other, DoubleBl t) {
      throw new NotImplementedException();
    }

    static BaseIntBl() {
      IntBl.CheckInit();
      Int2Bl.CheckInit();
      Int3Bl.CheckInit();
      Int4Bl.CheckInit();
    }
  }
  public partial class IntBl : BaseIntBl<int, IntBl, BoolBl>, IMonoNumericBl<int,IntBl> {
    public IntBl(Expr<int> e) : base(e) { }
    public IntBl() : base() { }
    public static implicit operator IntBl(Expr<int> Value) { return new IntBl(Value); }
    public static implicit operator IntBl(int Value) { return new Constant<int>(Value); }
    static IntBl() { 
      Register(e => e);
      Arity1<int>.CheckInit();
    }
    public override DOUBLE ToDouble<DOUBLE>() {
      var d = this.Coerce<DoubleBl>();
      return (DOUBLE)(object)d;
    }
    public static void CheckInit() { Extensions.Trace("CheckInit: " + typeof(IntBl) + " ToBrand=" + ToBrand); }

    public static implicit operator Int2Bl(IntBl b) { return new Int2Bl(b, b); }
    public static implicit operator Int3Bl(IntBl b) { return new Int3Bl(b, b, b); }
    public static implicit operator Int4Bl(IntBl b) { return new Int4Bl(b, b, b, b); }

    public override BoolBl GreaterThan(IntBl other) {
      return NumericOperators<int, int, bool>.GreaterThan.Instance.Make(this, other);
    }
    public override BoolBl LessThan(IntBl other) {
      return NumericOperators<int, int, bool>.LessThan.Instance.Make(this, other);
    }
    public override BoolBl GreaterThanOrEqual(IntBl other) {
      return NumericOperators<int, int, bool>.GreaterThanOrEqual.Instance.Make(this, other);
    }
    public override BoolBl LessThanOrEqual(IntBl other) {
      return NumericOperators<int, int, bool>.LessThanOrEqual.Instance.Make(this, other);
    }
  }
  public abstract partial class MultiIntBl<T, BRAND, B, BOOL, ARITY> : BaseIntBl<T, BRAND, BOOL>
    where BRAND : MultiIntBl<T, BRAND, B, BOOL, ARITY>
    where BOOL : BaseBoolBl<B, BOOL>
    where ARITY : MultiArity<int, T, ARITY>, new() {
    public MultiIntBl(Expr<T> Underlying) : base(Underlying) { }
    public override DOUBLE ToDouble<DOUBLE>() {
      var Cs = new Expr<double>[Arity<int, T, ARITY>.ArityI.Count];
      for (int i = 0; i < Cs.Length; i++)
        Cs[i] = ((IntBl) Arity<int, T, ARITY>.ArityI.Access(this, i)).ToDouble<DoubleBl>();
      return FactoryIntBl<DOUBLE, BOOL>.Apply(Cs);
    }
    static MultiIntBl() { Extensions.CheckInit<ARITY>(); }
    public static implicit operator MultiIntBl<T, BRAND, B, BOOL, ARITY>(IntBl d) {
      return ToBrand(Arity<int,T,ARITY>.ArityI.Repeat(d));
    }

    public override BOOL GreaterThan(BRAND other) {
      return BaseBoolBl<B,BOOL>.ToBrand(
        NumericOperators<T, int, B>.GreaterThan.Instance.Make(this, other)
      );
    }
    public override BOOL LessThan(BRAND other) {
      return BaseBoolBl<B, BOOL>.ToBrand(
        NumericOperators<T, int, B>.LessThan.Instance.Make(this, other)
      );
    }
    public override BOOL GreaterThanOrEqual(BRAND other) {
      return BaseBoolBl<B, BOOL>.ToBrand(
        NumericOperators<T, int, B>.GreaterThanOrEqual.Instance.Make(this, other)
      );
    }
    public override BOOL LessThanOrEqual(BRAND other) {
      return BaseBoolBl<B, BOOL>.ToBrand(
        NumericOperators<T, int, B>.LessThanOrEqual.Instance.Make(this, other)
      );
    }
  }

  public partial class Int4Bl : MultiIntBl<Vec4<int>, Int4Bl, Vec4<bool>, Bool4Bl, Vec4Arity<int>>, HasXYZW<IntBl>, ICoreBrandT {
    public Int4Bl(Expr<Vec4<int>> Underlying) : base(Underlying) { }
    public Int4Bl(IntBl bA, IntBl bB, IntBl bC, IntBl bD) : base(Vec4Arity<int>.ArityI.Make(bA, bB, bC, bD)) { }
    public static implicit operator Int4Bl(Expr<Vec4<int>> Value) { return new Int4Bl(Value); }
    public static implicit operator Int4Bl(Vec4<int> Value) { return new Constant<Vec4<int>>(Value); }
    public static implicit operator Int4Bl(int Value) { return (IntBl)Value; }
    static Int4Bl() {
      Register(e => e); 
    }
    public IntBl X { get { return Vec4Arity<int>.ArityI.Access(Underlying, 0); } set { X.Bind = value; } }
    public IntBl Y { get { return Vec4Arity<int>.ArityI.Access(Underlying, 1); } set { Y.Bind = value; } }
    public IntBl Z { get { return Vec4Arity<int>.ArityI.Access(Underlying, 2); } set { Z.Bind = value; } }
    public IntBl W { get { return Vec4Arity<int>.ArityI.Access(Underlying, 3); } set { W.Bind = value; } }
    public static void CheckInit() { Extensions.Trace("CheckInit: " + typeof(Int4Bl) + " ToBrand=" + ToBrand); }
  }
  public partial class Int3Bl : MultiIntBl<Vec3<int>, Int3Bl, Vec3<bool>, Bool3Bl, Vec3Arity<int>>, HasXYZ<IntBl>, ICoreBrandT {
    public Int3Bl(Expr<Vec3<int>> Underlying) : base(Underlying) { }
    public Int3Bl(IntBl bA, IntBl bB, IntBl bC) : base(Vec3Arity<int>.ArityI.Make(bA, bB, bC)) { }
    public static implicit operator Int3Bl(Expr<Vec3<int>> Value) { return new Int3Bl(Value); }
    public static implicit operator Int3Bl(Vec3<int> Value) { return new Constant<Vec3<int>>(Value); }
    public static implicit operator Int3Bl(int Value) { return (IntBl)Value; }
    static Int3Bl() {
      Register(e => e); 
    }
    public static void CheckInit() { Extensions.Trace("CheckInit: " + typeof(Int3Bl) + " ToBrand=" + ToBrand); }
    public IntBl X { get { return Vec3Arity<int>.ArityI.Access(Underlying, 0); } set { X.Bind = value; } }
    public IntBl Y { get { return Vec3Arity<int>.ArityI.Access(Underlying, 1); } set { Y.Bind = value; } }
    public IntBl Z { get { return Vec3Arity<int>.ArityI.Access(Underlying, 2); } set { Z.Bind = value; } }
  }
  public partial class Int2Bl : MultiIntBl<Vec2<int>, Int2Bl, Vec2<bool>, Bool2Bl, Vec2Arity<int>>, HasXY<IntBl>, ICoreBrandT {
    public Int2Bl(Expr<Vec2<int>> Underlying) : base(Underlying) { }
    public Int2Bl(IntBl bA, IntBl bB) : base(Vec2Arity<int>.ArityI.Make(bA, bB)) { }
    public static implicit operator Int2Bl(Expr<Vec2<int>> Value) { return new Int2Bl(Value); }
    public static implicit operator Int2Bl(Vec2<int> Value) { return new Constant<Vec2<int>>(Value); }
    public static implicit operator Int2Bl(int Value) { return (IntBl)Value; }
    static Int2Bl() {
      Register(e => e); 
    }
    public IntBl X { get { return Vec2Arity<int>.ArityI.Access(Underlying, 0); } set { X.Bind = value; } }
    public IntBl Y { get { return Vec2Arity<int>.ArityI.Access(Underlying, 1); } set { Y.Bind = value; } }
    public static void CheckInit() { Extensions.Trace("CheckInit: " + typeof(Int2Bl) + " ToBrand=" + ToBrand); }
  }


  //  string is unique
  public partial class StringBl : ObjectBl<string, StringBl>, ICoreBrandT {
    public StringBl(Expr<string> Underlying) : base(Underlying) { }
    public StringBl() : base() { }
    public static implicit operator StringBl(Expr<string> Value) { return new StringBl(Value); }
    public static implicit operator StringBl(string Value) { return new Constant<string>(Value); }
    static StringBl() { 
      Register(e => e);
      CheckInit();
    }
    public override StringBl ToStringBl() { return this; }
    public static StringBl operator +(StringBl opA, StringBl opB) {
      return opA.Combine(opB).Map((s0, s1) => s0 + s1);
    }
    public StringBl ToLower() {
      throw new NotImplementedException();
    }
    public StringBl ToUpper() {
      throw new NotImplementedException();
    }
    public static void CheckInit() { Extensions.Trace("CheckInit: " + typeof(StringBl) + " ToBrand=" + ToBrand); }

  }
  public abstract partial class ObjectBl<T, BRAND> : Brand<T, BRAND> where BRAND : ObjectBl<T, BRAND> {
    public ObjectBl(Expr<T> Underlying) : base(Underlying) { }
    public ObjectBl() : base() { }
    static ObjectBl() {
    }
  }
  public partial class ObjectBl : ObjectBl<object, ObjectBl>, ICoreBrandT {
    public ObjectBl(Expr<object> Underlying) : base(Underlying) { }
    public ObjectBl() : base() { }
    public static implicit operator ObjectBl(Expr<object> Value) { return new ObjectBl(Value); }
    public static implicit operator ObjectBl(string Value) { return new Constant<object>(Value); }
    static ObjectBl() { Register(e => e); }
    public override StringBl ToStringBl() { return Map<string,StringBl>(obj => obj.ToString()); }

  }
  public partial class EnumBl<T> : Brand<T, EnumBl<T>>, ICoreBrandT {
    public EnumBl() : base() { }
    public EnumBl(Expr<T> v) : base(v) { }
    public static implicit operator EnumBl<T>(Expr<T> v) { return new EnumBl<T>(v); }
    public static implicit operator EnumBl<T>(T v) { return new Constant<T>(v); }
    static EnumBl() {
      Register(v => v);
    }
  }
  public interface ICollectionBl<E, EBRAND> : IBrand where EBRAND : ICanWrap<EBRAND, E> {
    IList<EBRAND> AddMany { set; }
    void Add(params EBRAND[] value);
    EBRAND AddOne { set; }
  }
  /// <summary>
  /// Bling wrapper around collections.
  /// </summary>
  /// <typeparam name="E">Collection element type.</typeparam>
  /// <typeparam name="EBRAND">Bling type of collection element.</typeparam>
  /// <typeparam name="T">Original collection type.</typeparam>
  /// <typeparam name="BRAND">Bling type of collection.</typeparam>
  public abstract class CollectionBl<E, EBRAND, T, BRAND> : Brand<T, BRAND>, ICollectionBl<E, EBRAND>
    where BRAND : CollectionBl<E, EBRAND, T, BRAND>
    where T : IList, ICollection, /* IList<E>, ICollection<E>, IEnumerable<E>, */ IEnumerable
    where EBRAND : ICanWrap<EBRAND, E> {
    public CollectionBl(Expr<T> Underlying) : base(Underlying) { }
    /// <summary>
    /// Add multiple elements to this collection using setter syntax. Useful for intilizers.
    /// </summary>
    public IList<EBRAND> AddMany {
      set { foreach (var e in value) this.CurrentValue.Add(
        CanWrap<EBRAND,E>.From(e).CurrentValue); }
    }
    /// <summary>
    /// Add elements to this collection.
    /// </summary>
    public void Add(params EBRAND[] value) { AddMany = value; }
    /// <summary>
    /// Add one element to this collection using setter syntax. Useful for initializers.
    /// </summary>
    public EBRAND AddOne {
      set { this.CurrentValue.Add(CanWrap<EBRAND,E>.From(value).CurrentValue); }
    }
    public EBRAND this[IntBl Idx] {
      get {
        return Idx.Combine<T, BRAND>(this).Map<E,EBRAND>((idx, coll) => {
          return (E)coll[idx];
        });
      }
    }

  }
  /// <summary>
  /// For Bling wrappers around implicitly typed collections that don't require unique brands. 
  /// </summary>
  /// <typeparam name="E">Collection element type.</typeparam>
  /// <typeparam name="EBRAND">Bling type of collection element.</typeparam>
  /// <typeparam name="T">Original collection type.</typeparam>
  public class CollectionBl<E, EBRAND, T> : CollectionBl<E, EBRAND, T, CollectionBl<E, EBRAND, T>>
    where T : IList, ICollection, IEnumerable
    where EBRAND : ICanWrap<EBRAND, E> {
    public CollectionBl(Expr<T> Underlying) : base(Underlying) { }
    public static implicit operator CollectionBl<E, EBRAND, T>(Expr<T> target) { return new CollectionBl<E, EBRAND, T>((target)); }
    public static implicit operator CollectionBl<E, EBRAND, T>(T target) { return (new Constant<T>(target)); }
    static CollectionBl() { Register(v => v); }
  }
  /// <summary>
  /// For Bling wrappers around explicitly typed collections that don't require unique brands.
  /// </summary>
  /// <typeparam name="E">Collection element type.</typeparam>
  /// <typeparam name="EBRAND">Bling type of collection element.</typeparam>
  /// <typeparam name="T">Original collection type.</typeparam>
  public class CollectionXBl<E, EBRAND, T> : CollectionBl<E, EBRAND, T, CollectionBl<E, EBRAND, T>>
    where T : IList, ICollection, IList<E>, ICollection<E>, IEnumerable<E>, IEnumerable, new()
    where EBRAND : ICanWrap<EBRAND, E> {
    public CollectionXBl(Expr<T> Underlying) : base(Underlying) { }
    public CollectionXBl() : this(new Constant<T>(new T())) { }
    public static implicit operator CollectionXBl<E, EBRAND, T>(Expr<T> target) { return new CollectionXBl<E, EBRAND, T>((target)); }
    public static implicit operator CollectionXBl<E, EBRAND, T>(T target) { return (new Constant<T>(target)); }
    static CollectionXBl() { Register(v => v); }
    public static implicit operator CollectionXBl<E, EBRAND, T>(EBRAND[] values) {
      return new CollectionXBl<E, EBRAND, T>() {
        AddMany = values,
      };
    }
  }
  public interface IPrimitiveCollectionBl<E, EBRAND> : ICollectionBl<E, EBRAND> where EBRAND : ICanWrap<EBRAND, E> {
    void InitElements(IList<EBRAND> Backing);
    Action RefreshAction(IList<EBRAND> Backing);
  }
  public interface IListBl<T> {
    int Count { get; }
    T this[IntBl Idx] { get; }
  }
  public abstract partial class BaseFuncBl<RET, FUNC, BRAND> : IConditionTarget<BRAND>, ICanTable<BRAND>
    where RET : IConditionTarget<RET>,ICanTable<RET>
    where BRAND : BaseFuncBl<RET, FUNC, BRAND> {
    public static Func<FUNC, BRAND> ToBrand { get; private set; }

    protected static void Register(Func<FUNC, BRAND> ToBrand) {
      (BaseFuncBl<RET, FUNC, BRAND>.ToBrand == null).Assert();
      BaseFuncBl<RET, FUNC, BRAND>.ToBrand = ToBrand;
    }
    public static implicit operator FUNC(BaseFuncBl<RET, FUNC, BRAND> F) { return F.Function; }
    public static implicit operator BaseFuncBl<RET, FUNC, BRAND>(FUNC F) { return ToBrand(F); }

    public readonly FUNC Function;
    public BaseFuncBl(FUNC Function) { this.Function = Function; }

    public abstract RET Id { get; }
    public abstract BRAND MapR(Func<RET, RET> F);
    public override string ToString() {
      return Id.ToString();
    }
    public override int GetHashCode() {
      return Id.GetHashCode();
    }
    public override bool Equals(object obj) {
      if (obj is BRAND) return ((BRAND)obj).Id.Equals(Id);
      else return base.Equals(obj);
    }
    public abstract BRAND Map(BRAND Other, Func<RET, RET, RET> F);
    public abstract BRAND Map(BRAND[] Others, Func<RET[], RET> F);

    public abstract BRAND Map(BRAND OtherA, BRAND OtherB, Func<RET, RET, RET, RET> F);
    public BRAND Choose(BoolBl Test, BRAND IfFalse) { // that works.
      return Map(IfFalse, Test.Condition);
    }
    public BRAND DChoose(BoolBl Test, BRAND IfFalse) { // that works.
      return Map(IfFalse, Test.DCondition);
    }
    public BRAND Table(IntBl Index, params BRAND[] Options) {
      return Map(Options, (rS) => rS.Table(Index));
    }
  }
  public abstract partial class BaseDoubleFuncBl<RET, FUNC, BRAND> : BaseFuncBl<RET, FUNC, BRAND>, IBaseDoubleBl<BRAND>
    where RET : IBaseDoubleBl<RET>
    where BRAND : BaseDoubleFuncBl<RET, FUNC, BRAND> {
    public BaseDoubleFuncBl(FUNC Function) : base(Function) { }

    public BRAND Add(BRAND Op) { return this + Op; }
    public BRAND Subtract(BRAND Op) { return this - Op; }
    public BRAND Multiply(BRAND Op) { return this * Op; }
    public BRAND Divide(BRAND Op) { return this / Op; }
    public BRAND Modulo(BRAND Op) { return this % Op; }
    public BRAND Multiply(DoubleBl Op) { return this * Op; }
    public BRAND Divide(DoubleBl Op) { return this / Op; }

    public static BRAND operator +(BaseDoubleFuncBl<RET, FUNC, BRAND> opA, BRAND opB) { return opA.Map(opB, (c0, c1) => c0.Add(c1)); }
    public static BRAND operator -(BaseDoubleFuncBl<RET, FUNC, BRAND> opA, BRAND opB) { return opA.Map(opB, (c0, c1) => c0.Subtract(c1)); }
    public static BRAND operator *(BaseDoubleFuncBl<RET, FUNC, BRAND> opA, BRAND opB) { return opA.Map(opB, (c0, c1) => c0.Multiply(c1)); }
    public static BRAND operator /(BaseDoubleFuncBl<RET, FUNC, BRAND> opA, BRAND opB) { return opA.Map(opB, (c0, c1) => c0.Divide(c1)); }
    public static BRAND operator %(BaseDoubleFuncBl<RET, FUNC, BRAND> opA, BRAND opB) { return opA.Map(opB, (c0, c1) => c0.Modulo(c1)); }

    public static BRAND operator +(BaseDoubleFuncBl<RET, FUNC, BRAND> opA, RET c1) { return opA.MapR((c0) => c0.Add(c1)); }
    public static BRAND operator -(BaseDoubleFuncBl<RET, FUNC, BRAND> opA, RET c1) { return opA.MapR((c0) => c0.Subtract(c1)); }
    public static BRAND operator *(BaseDoubleFuncBl<RET, FUNC, BRAND> opA, RET c1) { return opA.MapR((c0) => c0.Multiply(c1)); }
    public static BRAND operator /(BaseDoubleFuncBl<RET, FUNC, BRAND> opA, RET c1) { return opA.MapR((c0) => c0.Divide(c1)); }
    public static BRAND operator %(BaseDoubleFuncBl<RET, FUNC, BRAND> opA, RET c1) { return opA.MapR((c0) => c0.Modulo(c1)); }

    public static BRAND operator +(RET c0, BaseDoubleFuncBl<RET, FUNC, BRAND> opB) { return opB.MapR((c1) => c0.Add(c1)); }
    public static BRAND operator -(RET c0, BaseDoubleFuncBl<RET, FUNC, BRAND> opB) { return opB.MapR((c1) => c0.Subtract(c1)); }
    public static BRAND operator *(RET c0, BaseDoubleFuncBl<RET, FUNC, BRAND> opB) { return opB.MapR((c1) => c0.Multiply(c1)); }
    public static BRAND operator /(RET c0, BaseDoubleFuncBl<RET, FUNC, BRAND> opB) { return opB.MapR((c1) => c0.Divide(c1)); }
    public static BRAND operator %(RET c0, BaseDoubleFuncBl<RET, FUNC, BRAND> opB) { return opB.MapR((c1) => c0.Modulo(c1)); }


    public static BRAND operator *(BaseDoubleFuncBl<RET, FUNC, BRAND> opA, DoubleBl opB) { return opA.MapR((c0) => c0.Multiply(opB)); }
    public static BRAND operator /(BaseDoubleFuncBl<RET, FUNC, BRAND> opA, DoubleBl opB) { return opA.MapR((c0) => c0.Divide(opB)); }

    public BRAND Clamp(RET min, RET max) { return MapR(c => c.Clamp(min, max)); }
    public BRAND Clamp(BRAND min, BRAND max) { return Map(min, max, (a,b,c) => a.Clamp(b,c)); }


    public BRAND Lerp0(BRAND Other, DoubleBl t) { return Map(Other, (c0, c1) => t.Lerp(c0, c1)); }
    public BRAND Max(BRAND Other) { return Map(Other, (c0, c1) => c0.Max(c1)); }
    public BRAND Min(BRAND Other) { return Map(Other, (c0, c1) => c0.Min(c1)); }
  }
  public abstract partial class BaseArgDoubleFuncBl<ARG, RET, BRAND> : BaseDoubleFuncBl<RET, Func<ARG,RET>, BRAND>
    where RET : IBaseDoubleBl<RET>
    where BRAND : BaseArgDoubleFuncBl<ARG, RET, BRAND> {
    public BaseArgDoubleFuncBl(Func<ARG, RET> Function) : base(Function) { }
    public RET this[ARG arg] { get { return Function(arg); } }
    public Func<A, RET> Circle<A>(Func<A, ARG> G) where A : Brand<A> { return a => Function(G(a)); }
    public override BRAND MapR(Func<RET, RET> F) {
      return ToBrand(a => F(Function(a)));
    }
    public BRAND MapA(Func<ARG, ARG> G) {
      return ToBrand(p => Function(G(p)));
    }
    public BRAND Map(Func<ARG, RET> Other, Func<RET, RET, RET> F) {
      return ToBrand(Map<RET, RET>(Other, F));
    }
    public override BRAND Map(BRAND Other, Func<RET, RET, RET> F) {
      return Map((Func<ARG, RET>)Other, F);
    }
    public override BRAND Map(BRAND[] Other, Func<RET[], RET> F) {
      return ToBrand(p => {
        var rS = new RET[Other.Length];
        for (int i = 0; i < Other.Length; i++) rS[i] = Other[i][p];
        return F(rS);
      });
    }


    public override BRAND Map(BRAND OtherA, BRAND OtherB, Func<RET, RET, RET, RET> F) {
      return ToBrand(p => F(this[p], OtherA[p], OtherB[p]));
    }

    public Func<ARG, B> Map<A, B>(Func<ARG, A> Other, Func<RET, A, B> F) {
      return (p => F(this[p], Other(p)));
    }
    public static implicit operator BaseArgDoubleFuncBl<ARG, RET, BRAND>(Func<ARG, RET> F) { return ToBrand(F); }

    public static BRAND operator *(BaseArgDoubleFuncBl<ARG, RET, BRAND> opA, Func<ARG,DoubleBl> opB) {
      return ToBrand(arg => opA[arg].Multiply(opB(arg)));
    }
    public static BRAND operator /(BaseArgDoubleFuncBl<ARG, RET, BRAND> opA, Func<ARG,DoubleBl> opB) {
      return ToBrand(arg => opA[arg].Divide(opB(arg)));
    }
    public static BRAND operator *(Func<ARG, DoubleBl> opB, BaseArgDoubleFuncBl<ARG, RET, BRAND> opA) {
      return ToBrand(arg => opA[arg].Multiply(opB(arg)));
    }


  }
  // common.
  public abstract partial class DoubleFuncBl<ARG, RET, BRAND> : BaseArgDoubleFuncBl<ARG, RET, BRAND>
    where ARG : IBaseDoubleBl<ARG>
    where RET : IBaseDoubleBl<RET>
    where BRAND : DoubleFuncBl<ARG, RET, BRAND> {

    public DoubleFuncBl(Func<ARG, RET> Function) : base(Function) { }

    public static BRAND operator +(DoubleFuncBl<ARG, RET, BRAND> opA, ARG opB) { return opA.MapA(p => p.Add(opB)); }
    public static BRAND operator -(DoubleFuncBl<ARG, RET, BRAND> opA, ARG opB) { return opA.MapA(p => p.Subtract(opB)); }
    public static BRAND operator *(DoubleFuncBl<ARG, RET, BRAND> opA, ARG opB) { return opA.MapA(p => p.Multiply(opB)); }
    public static BRAND operator /(DoubleFuncBl<ARG, RET, BRAND> opA, ARG opB) { return opA.MapA(p => p.Divide(opB)); }
    public static BRAND operator %(DoubleFuncBl<ARG, RET, BRAND> opA, ARG opB) { return opA.MapA(p => p.Modulo(opB)); }

    public static BRAND operator +(ARG opA, DoubleFuncBl<ARG, RET, BRAND> opB) { return opB.MapA(p => opA.Add(p)); }
    public static BRAND operator -(ARG opA, DoubleFuncBl<ARG, RET, BRAND> opB) { return opB.MapA(p => opA.Subtract(p)); }
    public static BRAND operator *(ARG opA, DoubleFuncBl<ARG, RET, BRAND> opB) { return opB.MapA(p => opA.Multiply(p)); }
    public static BRAND operator /(ARG opA, DoubleFuncBl<ARG, RET, BRAND> opB) { return opB.MapA(p => opA.Divide(p)); }
    public static BRAND operator %(ARG opA, DoubleFuncBl<ARG, RET, BRAND> opB) { return opB.MapA(p => opA.Modulo(p)); }
    public static implicit operator DoubleFuncBl<ARG, RET, BRAND>(Func<ARG, RET> F) { return ToBrand(F); }
  }
  public abstract partial class DoubleFunc2Bl<ARG, RET, BRAND> : DoubleFuncBl<ARG, RET, BRAND>, ISupportsLerp<BRAND>
    where ARG : Brand<ARG>, IBaseDoubleBl<ARG>
    where RET : IBaseDoubleBl<RET>
    where BRAND : DoubleFunc2Bl<ARG, RET, BRAND> {
    public DoubleFunc2Bl(Func<ARG, RET> Function) : base(Function) { }
    public override RET Id {
      get { return this[Brand<ARG>.Param("_A")]; }
    }
  }

  // common.
  public abstract partial class DoubleFuncBl<ARGA, ARGB, RET, BRAND> : BaseDoubleFuncBl<RET, Func<ARGA,ARGB,RET>, BRAND>, ISupportsLerp<BRAND>
    where ARGA : Brand<ARGA>, IDoubleBl<ARGA>
    where ARGB : Brand<ARGB>, IDoubleBl<ARGB>
    where RET : IBaseDoubleBl<RET>
    where BRAND : DoubleFuncBl<ARGA, ARGB, RET, BRAND> {

    public DoubleFuncBl(Func<ARGA, ARGB, RET> Function) : base(Function) { }
    public RET this[ARGA argA, ARGB argB] { get { return Function(argA, argB); }  }

    public override BRAND MapR(Func<RET, RET> F) {
      return ToBrand((a, b) => F(Function(a, b)));
    }
    public BRAND MapA(Func<ARGA, ARGA> G) {
      return ToBrand((a, b) => Function(G(a), b));
    }
    public BRAND Map(Func<ARGA, ARGB, RET> Other, Func<RET, RET, RET> F) {
      return ToBrand(Map<RET, RET>(Other, F));
    }
    public override BRAND Map(BRAND Other, Func<RET, RET, RET> F) {
      return Map((Func<ARGA, ARGB, RET>)Other, F);
    }
    public override BRAND Map(BRAND[] Other, Func<RET[], RET> F) {
      return ToBrand((p,q) => {
        var rS = new RET[Other.Length];
        for (int i = 0; i < Other.Length; i++) rS[i] = Other[i][p,q];
        return F(rS);
      });
    }


    public override BRAND Map(BRAND OtherA, BRAND OtherB, Func<RET, RET, RET, RET> F) {
      return ToBrand((p,q) => F(this[p,q], OtherA[p,q], OtherB[p,q]));
    }

    public Func<ARGA, ARGB, B> Map<A, B>(Func<ARGA, ARGB, A> Other, Func<RET, A, B> F) {
      return ((a, b) => F(this[a, b], Other(a, b)));
    }
    public static implicit operator DoubleFuncBl<ARGA, ARGB, RET, BRAND>(Func<ARGA, ARGB, RET> F) { return ToBrand(F); }
    public override RET Id {
      get { return this[Brand<ARGA>.Param("_A"), Brand<ARGB>.Param("_B")]; }
    }
  }
  public abstract partial class DoubleFuncBl<ARGA, ARGB, ARGC, RET, BRAND> : BaseDoubleFuncBl<RET, Func<ARGA, ARGB, ARGC, RET>, BRAND>, ISupportsLerp<BRAND>
    where ARGA : Brand<ARGA>, IDoubleBl<ARGA>
    where ARGB : Brand<ARGB>, IDoubleBl<ARGB>
    where ARGC : Brand<ARGC>, IDoubleBl<ARGC>
    where RET : IBaseDoubleBl<RET>
    where BRAND : DoubleFuncBl<ARGA, ARGB, ARGC, RET, BRAND> {

    public DoubleFuncBl(Func<ARGA, ARGB, ARGC, RET> Function) : base(Function) { }
    public RET this[ARGA argA, ARGB argB, ARGC argC] { get { return Function(argA, argB, argC); } }

    public override BRAND MapR(Func<RET, RET> F) {
      return ToBrand((a, b, c) => F(Function(a, b, c)));
    }
    public BRAND MapA(Func<ARGA, ARGA> G) {
      return ToBrand((a, b, c) => Function(G(a), b, c));
    }
    public BRAND Map(Func<ARGA, ARGB, ARGC, RET> Other, Func<RET, RET, RET> F) {
      return ToBrand((a,b,c) => F(this[a,b,c], Other(a,b,c)));
    }
    public override BRAND Map(BRAND Other, Func<RET, RET, RET> F) {
      return Map((Func<ARGA, ARGB, ARGC, RET>)Other, F);
    }
    public override BRAND Map(BRAND[] Other, Func<RET[], RET> F) {
      return ToBrand((a,b,c) => {
        var rS = new RET[Other.Length];
        for (int i = 0; i < Other.Length; i++) rS[i] = Other[i][a,b,c];
        return F(rS);
      });
    }
    public override BRAND Map(BRAND OtherA, BRAND OtherB, Func<RET, RET, RET, RET> F) {
      return ToBrand((a, b, c) => F(this[a,b,c], OtherA[a,b,c], OtherB[a,b,c]));
    }
    /*
    public Func<ARGA, ARGB, ARGC, B> Map<A, B>(Func<ARGA, ARGB, A> Other, Func<RET, A, B> F) {
      return ((a, b, c) => F(this[a, b,c], Other(a, b,c)));
    }*/
    public static implicit operator DoubleFuncBl<ARGA, ARGB, ARGC, RET, BRAND>(Func<ARGA, ARGB, ARGC, RET> F) { return ToBrand(F); }
    public override RET Id {
      get { return this[Brand<ARGA>.Param("_A"), Brand<ARGB>.Param("_B"), Brand<ARGC>.Param("_C")]; }
    }
  }

  public static partial class BrandExtensions {
    public static Func<A, Point3DBl> AddZ<A>(this Func<A, PointBl> F, DoubleBl Z) {
      return a => F(a).InsertZ(Z);
    }
    public static Func<A, Point3DBl> AddZ<A,BRAND>(this BRAND F, DoubleBl Z) where BRAND : BaseDoubleFuncBl<PointBl,Func<A,PointBl>,BRAND> {
      return F.Function.AddZ(Z);
    }
    public static Func<A, Point3DBl> AddX<A>(this Func<A, PointBl> F, DoubleBl Z) {
      return a => F(a).InsertX(Z);
    }
    public static Func<A, Point3DBl> AddX<A, BRAND>(this BRAND F, DoubleBl Z)
      where BRAND : BaseDoubleFuncBl<PointBl, Func<A, PointBl>, BRAND> {
      return F.Function.AddX(Z);
    }
    public static Func<A, Point3DBl> AddY<A>(this Func<A, PointBl> F, DoubleBl Z) {
      return a => F(a).InsertY(Z);
    }
    public static Func<A, Point3DBl> AddY<A, BRAND>(this BRAND F, DoubleBl Z)
      where BRAND : BaseDoubleFuncBl<PointBl, Func<A, PointBl>, BRAND> {
      return F.Function.AddY(Z);
    }
    public static void Add(this List<Int3Bl> list, IntBl A, IntBl B, IntBl C) {
      list.Add(new Int3Bl(A, B, C));
    }
    public static void Add<T>(this List<T> list, params T[] Ps) {
      for (int i = 0; i < Ps.Length; i++) list.Add(Ps[i]);
    }


    public static PointBl[] Portable<BRAND>(this IPointBl<BRAND>[] Points) where BRAND : IPointBl<BRAND> {
      if (Points is PointBl[]) return (PointBl[])Points;
      var ret = new PointBl[Points.Length];
      for (int i = 0; i < Points.Length; i++)
        ret[i] = Points[i].Portable;
      return ret;
    }
    public static void Bind<BRAND>(this BRAND[] Values, params BRAND[] To) where BRAND : Brand<BRAND> {
      (Values.Length == To.Length).Assert();
      for (int i = 0; i < Values.Length; i++)
        Values[i].Bind = To[i];
    }
    public static Expr<T>[] Underlying<T, BRAND>(this Brand<T,BRAND>[] Values) where BRAND : Brand<T, BRAND> {
      var ret = new Expr<T>[Values.Length];
      for (int i = 0; i < Values.Length; i++) ret[i] = Values[i];
      return ret;
    }
    public static Expr<T>[,] Underlying<T, BRAND>(this Brand<T, BRAND>[,] Values) where BRAND : Brand<T, BRAND> {
      var ret = new Expr<T>[Values.GetLength(0), Values.GetLength(1)];
      for (int i = 0; i < Values.GetLength(0); i++)
        for (int j = 0; j < Values.GetLength(1); j++) 
          ret[i,j] = Values[i,j];
      return ret;
    }

    public static BRAND Table<BRAND>(this BRAND[] Values, IntBl Index) where BRAND : ICanTable<BRAND> {
      Index = Index.Underlying.Fold;
      if (Index.Underlying is Constant<int>)
        return Values[((Constant<int>)Index.Underlying).Value];
      return Values[0].Table(Index, Values);
    }

    /// <summary>
    /// Convert unbranded Bl value to branded Bl value.
    /// </summary>
    /// <param name="d">Unbranded Bl value.</param>
    /// <returns>Branded Bl value.</returns>
    public static DoubleBl Bl(this Expr<double> d) { return d; }
    public static PointBl Bl(this Expr<Vecs.Vec2<double>> d) { return d; }
    public static Point3DBl Bl(this Expr<Vecs.Vec3<double>> d) { return d; }

    /// <summary>
    /// Convert unbranded Bl value to branded Bl value.
    /// </summary>
    /// <param name="d">Unbranded Bl value.</param>
    /// <returns>Branded Bl value.</returns>
    public static StringBl Bl(this Expr<string> d) { return d; }
    /// <summary>
    /// Convert unbranded Bl value to branded Bl value.
    /// </summary>
    /// <param name="d">Unbranded Bl value.</param>
    /// <returns>Branded Bl value.</returns>
    public static IntBl Bl(this Expr<int> d) { return d; }
    /// <summary>
    /// Convert unbranded Bl value to branded Bl value.
    /// </summary>
    /// <param name="d">Unbranded Bl value.</param>
    /// <returns>Branded Bl value.</returns>
    public static BoolBl Bl(this Expr<bool> d) { return d; }
    /// <summary>
    /// Convert unbranded Bl value to branded Bl value.
    /// </summary>
    /// <param name="d">Unbranded Bl value.</param>
    /// <returns>Branded Bl value.</returns>
    public static ObjectBl Bl(this Expr<object> d) { return d; }
    public static Func<IntBl, DoubleBl> Bl(this Func<IntBl, Expr<double>> F) { return x => F(x).Bl(); }
    public static Func<IntBl, IntBl> Bl(this Func<IntBl, Expr<int>> F) { return x => F(x).Bl(); }
    public static Func<IntBl, BoolBl> Bl(this Func<IntBl, Expr<bool>> F) { return x => F(x).Bl(); }
  
    /// <summary>
    /// Convert core language value to branded Bl value.
    /// </summary>
    /// <param name="d">Core language value.</param>
    /// <returns>Branded Bl value.</returns>
    public static DoubleBl Bl(this double d) { return d; }

    /// <summary>
    /// Convert core language value to branded Bl value.
    /// </summary>
    /// <param name="d">Core language value.</param>
    /// <returns>Branded Bl value.</returns>
    public static IntBl Bl(this int d) { return d; }
    /// <summary>
    /// Convert core language value to branded Bl value.
    /// </summary>
    /// <param name="d">Core language value.</param>
    /// <returns>Branded Bl value.</returns>
    public static BoolBl Bl(this bool d) { return d; }
    /// <summary>
    /// Convert unbranded Bl value to branded Bl value.
    /// </summary>
    /// <param name="d">Unbranded Bl value.</param>
    /// <returns>Branded Bl value.</returns>
    public static StringBl Bl(this string d) { return d; }
  }



  /// <summary>
  /// Any minimum/maximum property pair.
  /// </summary>
  public abstract class RangeBl<BRAND, MONO> where BRAND : Brand<BRAND>, INumericNoT<BRAND, MONO> 
    where MONO : Brand<MONO>, INumericNoT<MONO, MONO> {
    /// <summary>
    /// Maximum value of this range set.
    /// </summary>
    public abstract BRAND Maximum { get; set; }
    /// <summary>
    /// Minimum value of this range set. 
    /// </summary>
    public abstract BRAND Minimum { get; set; }
    public void Bind(RangeBl<BRAND, MONO> other) {
      Maximum.Bind = other.Maximum; Minimum.Bind = other.Minimum;
    }
  }
  /// <summary>
  /// Default range implementation. 
  /// </summary>
  public class RangeBl : RangeBl<DoubleBl, DoubleBl> {
    public override DoubleBl Maximum { get; set; }
    public override DoubleBl Minimum { get; set; }
    public StringBl ToStringBl(DoubleBl d) {
      return Minimum.ToStringBl(d) + " - " + Maximum.ToStringBl(d);
    }
    public override string ToString() {
      return Minimum + " - " + Maximum;
    }
  }
}
