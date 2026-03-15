using System;
using System.Collections.Generic;
using System.Collections;
using Bling.Util;
using Bling.DSL;
using Bling.Core;
using Bling.NewVecs;


namespace Bling.NewCore {

  public interface IValueBrand<BRAND> : IBrand<BRAND> where BRAND : IValueBrand<BRAND> {
    BRAND Condition(BoolBl Test, BRAND Other);
  }

  /// <summary>
  /// Root of all value brands.
  /// </summary>
  public abstract partial class ValueBrand<T, BRAND> : Brand<T, BRAND>, IValueBrand<BRAND> where BRAND : ValueBrand<T,BRAND> {
    public ValueBrand(Expr<T> Underlying) : base(Underlying) { }
    public ValueBrand() : base() { }

    public new static readonly Func<Expr<T>, BRAND> ToBrand;
    static ValueBrand() {
      var c = typeof(BRAND).GetConstructor(new Type[1] { typeof(Expr<T>) });
      (c != null).Assert();
      ToBrand = e => (BRAND)c.Invoke(new object[] { e });
      Register(e => ToBrand(e));
    }
    public abstract partial class Operator<S, CNT> : Ops.Operator<T, S, CNT> where CNT : Operator<S, CNT> {

    }
    public abstract partial class Operator<S, U, CNT> : Ops.Operator<T, S, U, CNT> where CNT : Operator<S, U, CNT> {

    }
    public abstract partial class Operator<S, U, V, CNT> : Ops.Operator<T, S, U, V, CNT> where CNT : Operator<S, U, V, CNT> {

    }
    public BRAND Condition(BoolBl Test, BRAND Other) {
      throw new NotImplementedException();
    }
  }

  public abstract partial class BaseValueBrand<T, BRAND, K, D> : ValueBrand<T, BRAND>
    where BRAND : ValueBrand<T, BRAND, K, D>
    where D : Dim<D> {
    public BaseValueBrand(Expr<T> Underlying) : base(Underlying) { }
    public BaseValueBrand(Expr<IVec<K,D>> Underlying) : this(Convert(Underlying)) { }
    public BaseValueBrand() : base() { }
    public static readonly Func<Expr<IVec<K,D>>, BRAND> ToBrandD;
    static BaseValueBrand() {
      var c = typeof(BRAND).GetConstructor(new Type[1] { typeof(Expr<IVec<K,D>>) });
      (c != null).Assert();
      ToBrandD = e => (BRAND)c.Invoke(new object[] { e });
    }
    private static Expr<T> Convert(Expr<IVec<K, D>> Underlying) {
      if (Underlying is Expr<T>) return (Expr<T>)(object)Underlying;
      (Dim<D>.Count == 1).Assert();
      (typeof(T) == typeof(K)).Assert();
      return (Expr<T>)(object)AccessExpr<K, D>.Make(Underlying, 0);
    }

  }
  public abstract partial class ValueBrand<T, BRAND, K, D> : BaseValueBrand<T, BRAND, K, D>
    where BRAND : ValueBrand<T, BRAND, K, D>
    where D : Dim<D> {
    public ValueBrand(Expr<T> Underlying) : base(Underlying) { }
    public ValueBrand(Expr<IVec<K,D>> Underlying) : base(Underlying) { }
    public ValueBrand() : base() { }
    public Expr<IVec<K, D>> UnderlyingD {
      get {
        if (Underlying is Expr<IVec<K, D>>) return (Expr<IVec<K, D>>)(object)Underlying;
        (Dim<D>.Count == 1).Assert();
        (typeof(T) == typeof(K)).Assert();
        return Composite<K, D>.Make((Expr<K>)(object)Underlying);
      }
    }
    public static implicit operator Expr<IVec<K, D>>(ValueBrand<T, BRAND, K, D> value) { return value.UnderlyingD; }
  }
  public interface IValueBrand<BRAND,D,SCALAR> : IValueBrand<BRAND> where BRAND : IValueBrand<BRAND,D,SCALAR> where D : Dim<D> where SCALAR : IValueBrand<SCALAR,D1,SCALAR> {
    BRAND Condition<BBRAND>(BoolBl<D,BBRAND> Test, BRAND Other) where BBRAND : BoolBl<D,BBRAND>;
  }

  public abstract partial class BaseValueBrand<T, BRAND, K, D, SCALAR> : ValueBrand<T, BRAND, K, D>, IValueBrand<BRAND,D,SCALAR>
    where BRAND : ValueBrand<T, BRAND, K, D, SCALAR>
    where D : Dim<D>
    where SCALAR : ValueBrand<K, SCALAR, K, D1, SCALAR> {
    public BaseValueBrand(Expr<T> Underlying) : base(Underlying) { }
    public BaseValueBrand(Expr<IVec<K, D>> Underlying) : base(Underlying) { }
    public BaseValueBrand(SCALAR[] Es) : this(Convert(Es)) { }
    public BaseValueBrand() : base() { }
    private static Expr<IVec<K, D>> Convert(SCALAR[] Es) {
      (Es.Length == Dim<D>.Count).Assert();
      var Es0 = new Expr<K>[Es.Length];
      for (int i = 0; i < Es.Length; i++)
        Es0[i] = Es[i];
      return Composite<K, D>.Make(Es0);
    }
    public SCALAR this[IntBl Index] {
      get { return ValueBrand<K, SCALAR, K, D1, SCALAR>.ToBrand(AccessExpr<K, D>.Make(UnderlyingD, Index)); }
    }
    public static FlexBoolBl<D> operator ==(BRAND opB, BaseValueBrand<T, BRAND, K, D,SCALAR> opA) {
      return opB.Equal(opA);
    }
    public static FlexBoolBl<D> operator !=(BRAND opB, BaseValueBrand<T, BRAND, K, D, SCALAR> opA) {
      return opB.NotEqual(opA);
    }
    public new virtual FlexBoolBl<D> Equal(BRAND opB) {
      FlexBoolBl<D> result = EqualOp.Instance.Make(this, opB);
      result.ConvertByAll = true;
      return result;
    }
    public new virtual FlexBoolBl<D> NotEqual(BRAND opB) {
      FlexBoolBl<D> result = NotEqualOp.Instance.Make(this, opB);
      result.ConvertByAll = false; // if any is true then as bool is true.
      return result;
    }
    public override int GetHashCode() { return base.GetHashCode(); }
    public override bool Equals(object obj) { return base.Equals(obj); }

    // we have a dimension here, we can have dimension operators
    public abstract partial class UnaryOperatorD<S, CNT> : Ops.Operator<IVec<K,D>, IVec<S, D>, CNT> where CNT : UnaryOperatorD<S, CNT> {
      public abstract Func<Expr<K>, Expr<S>> Uniform { get; }
      public abstract string Name { get; }
      public override string Format(string s) { return Name + s; }
      public override Expr<IVec<S, D>> Expand(Expr<IVec<K, D>> argA) {
        if (Dim<D>.Count > 1) {
          var Es = new Expr<S>[Dim<D>.Count];
          for (int i = 0; i < Es.Length; i++) {
            Es[i] = Uniform(AccessExpr<K,D>.Make(argA, i));
          }
          return Composite<S, D>.Make(Es);
        }
        return base.Expand(argA);
      }
    }
    public abstract partial class BinaryOperatorD<S, CNT> : Ops.Operator<IVec<K,D>, IVec<K,D>,IVec<S, D>, CNT> where CNT : BinaryOperatorD<S, CNT> {
      public abstract string Name { get; }
      public abstract Func<Expr<K>, Expr<K>, Expr<S>> Uniform { get; }
      public override string Format(string argA, string argB) { return "(" + argA + Name + argB + ")"; }
      public override Expr<IVec<S, D>> Expand(Expr<IVec<K, D>> argA, Expr<IVec<K, D>> argB) {
        if (Dim<D>.Count > 1) {
          var Es = new Expr<S>[Dim<D>.Count];
          for (int i = 0; i < Es.Length; i++) {
            Es[i] = Uniform(AccessExpr<K, D>.Make(argA, i), AccessExpr<K, D>.Make(argB, i));
          }
          return Composite<S, D>.Make(Es);
        }
        return base.Expand(argA, argB);
      }
    }
    public abstract partial class TrinaryOperatorD<S, CNT> : Ops.Operator<IVec<K, D>, IVec<S, D>, IVec<S,D>, IVec<S, D>, CNT> where CNT : TrinaryOperatorD<S, CNT> {
      public abstract string Name { get; }
      public abstract Func<Expr<K>, Expr<S>, Expr<S>, Expr<S>> Uniform { get; }
      public override string Format(string argA, string argB, string argC) {
        return Name + "(" + argA + ", " + argB + ", " + argC + ")";
      }
      public override Expr<IVec<S, D>> Expand(Expr<IVec<K, D>> argA, Expr<IVec<S, D>> argB, Expr<IVec<S, D>> argC) {
        if (Dim<D>.Count > 1) {
          var Es = new Expr<S>[Dim<D>.Count];
          for (int i = 0; i < Es.Length; i++) {
            Es[i] = Uniform(AccessExpr<K, D>.Make(argA, i), AccessExpr<S, D>.Make(argB, i), AccessExpr<S, D>.Make(argC, i));
          }
          return Composite<S, D>.Make(Es);
        }
        return base.Expand(argA, argB, argC);
      }
    }


    public abstract partial class EqualityOp<CNT> : BinaryOperatorD<bool, CNT> where CNT : EqualityOp<CNT> {}
    public partial class EqualOp : EqualityOp<EqualOp> {
      public new static EqualOp Instance = new EqualOp();
      private EqualOp() { }
      public override string Name { get { return "=="; } }
      public override Func<Expr<K>, Expr<K>, Expr<bool>> Uniform {
        get { return ValueBrand<K, SCALAR, K, D1, SCALAR>.EqualOp.Instance.Bl(); }
      }
    }
    public partial class NotEqualOp : EqualityOp<NotEqualOp> {
      public new static NotEqualOp Instance = new NotEqualOp();
      private NotEqualOp() { }
      public override string Name { get { return "!="; } }
      public override Func<Expr<K>, Expr<K>, Expr<bool>> Uniform {
        get {
          return ValueBrand<K, SCALAR, K, D1, SCALAR>.NotEqualOp.Instance.Bl();
        }
      }
    }
    public partial class ConditionOp : Ops.Operator<IVec<bool, D>, T, T, IVec<K,D>, ConditionOp> {
      public new static readonly ConditionOp Instance = new ConditionOp();
      private ConditionOp() { }
      public override string Format(string argA, string argB, string argC) {
        return "if (" + argA + ") " + argB + " else " + argC;
      }
      public override Expr<IVec<K,D>> Expand(Expr<IVec<bool, D>> argA, Expr<T> argB, Expr<T> argC) {
        FlexBoolBl<D> ArgA = (argA);
        BRAND ArgB = ToBrand(argB);
        BRAND ArgC = ToBrand(argC);
        Expr<K>[] Es = new Expr<K>[Dim<D>.Count];
        for (int i = 0; i < Dim<D>.Count; i++) {
          Es[i] = ArgA[i].Condition(ArgB[i], ArgC[i]);
        }
        return Composite<K, D>.Make(Es);
      }
    }
    public BRAND Condition<BBRAND>(BoolBl<D,BBRAND> Test, BRAND Other) where BBRAND : BoolBl<D,BBRAND> {
      return ToBrandD(ConditionOp.Instance.Make(Test, this, Other));
    }
  }
  public abstract partial class ValueBrand<T, BRAND, K, D, SCALAR> : BaseValueBrand<T, BRAND, K, D,SCALAR>
    where BRAND : ValueBrand<T, BRAND, K, D, SCALAR>
    where D : Dim<D> 
    where SCALAR : ValueBrand<K,SCALAR,K,D1,SCALAR> {
    public ValueBrand(Expr<T> Underlying) : base(Underlying) { }
    public ValueBrand(Expr<IVec<K, D>> Underlying) : base(Underlying) { }
    public ValueBrand(SCALAR[] Es) : base(Es) { }
    public ValueBrand() : base() { }
    public static FlexBoolBl<D> operator ==(ValueBrand<T, BRAND, K, D, SCALAR> opA, BRAND opB) {
      return opA.Equal(opB);
    }
    public static FlexBoolBl<D> operator !=(ValueBrand<T, BRAND, K, D, SCALAR> opA, BRAND opB) {
      return opA.NotEqual(opB);
    }
    public override int GetHashCode() { return base.GetHashCode(); }
    public override bool Equals(object obj) { return base.Equals(obj); }
  }
  public abstract partial class BaseBoolBl<T, BRAND, D> : ValueBrand<T, BRAND, bool, D, BoolBl>
    where BRAND : BaseBoolBl<T, BRAND, D>
    where D : Dim<D> {
    public BaseBoolBl(Expr<T> Underlying) : base(Underlying) { }
    public BaseBoolBl(Expr<IVec<bool, D>> Underlying) : base(Underlying) { }
    public BaseBoolBl(BoolBl[] Es) : base(Es) { }
    public BaseBoolBl() : base() { }

    public abstract partial class BinaryOp<CNT> : BinaryOperatorD<bool,CNT> where CNT : BinaryOp<CNT> {}
    public partial class AndOp : BinaryOp<AndOp> {
      public new static AndOp Instance = new AndOp();
      private AndOp() { }
      public override string Name { get { return "&"; } }
      public override Func<Expr<bool>, Expr<bool>, Expr<bool>> Uniform {
        get { return BaseBoolBl<bool, BoolBl, D1>.AndOp.Instance.Bl(); }
      }
    }
    public partial class OrOp : BinaryOp<OrOp> {
      public new static OrOp Instance = new OrOp();
      private OrOp() { }
      public override string Name { get { return "|"; } }
      public override Func<Expr<bool>, Expr<bool>, Expr<bool>> Uniform {
        get { return BaseBoolBl<bool, BoolBl, D1>.OrOp.Instance.Bl(); }
      }
    }
    public partial class ExclusiveOrOp : BinaryOp<ExclusiveOrOp> {
      public new static ExclusiveOrOp Instance = new ExclusiveOrOp();
      private ExclusiveOrOp() { }
      public override string Name { get { return "^"; } }
      public override Func<Expr<bool>, Expr<bool>, Expr<bool>> Uniform {
        get { return BaseBoolBl<bool, BoolBl, D1>.ExclusiveOrOp.Instance.Bl(); }
      }
    }

    public partial class NotOp : UnaryOperatorD<bool, NotOp> {
      public new static NotOp Instance = new NotOp();
      private NotOp() { }
      public override string Name {
        get { return "!"; }
      }
      public override Func<Expr<bool>, Expr<bool>> Uniform {
        get { return BaseBoolBl<bool, BoolBl, D1>.NotOp.Instance.Bl(); }
      }
    }

    public static BRAND operator |(BaseBoolBl<T, BRAND, D> opA, BRAND opB) {
      return ToBrandD(OrOp.Instance.Make(opA, opB));
    }
    public static BRAND operator &(BaseBoolBl<T, BRAND, D> opA, BRAND opB) {
      return ToBrandD(AndOp.Instance.Make(opA, opB));
    }
    public static BRAND operator ^(BaseBoolBl<T, BRAND, D> opA, BRAND opB) {
      return ToBrandD(ExclusiveOrOp.Instance.Make(opA, opB));
    }
    public static BRAND operator !(BaseBoolBl<T, BRAND, D> opA) {
      return ToBrandD(NotOp.Instance.Make(opA));
    }
    public abstract partial class AnyAllOp<CNT> : Ops.Operator<IVec<bool, D>, bool, CNT> where CNT : AnyAllOp<CNT> { }
    public class AnyOp : AnyAllOp<AnyOp> {
      public new static readonly AnyOp Instance = new AnyOp();
      private AnyOp() { }
      public override Expr<bool> Expand(Expr<IVec<bool, D>> argA) {
        BoolBl Ret = false;
        for (int i = 0; i < Dim<D>.Count; i++)
          Ret = Ret | AccessExpr<bool, D>.Make(argA, i);
        return Ret.Underlying;
      }
      public override string Format(string s) {
        return "Any(" + s + ")";
      }
    }
    public class AllOp : AnyAllOp<AllOp> {
      public new static readonly AllOp Instance = new AllOp();
      private AllOp() { }
      public override Expr<bool> Expand(Expr<IVec<bool, D>> argA) {
        BoolBl Ret = true;
        for (int i = 0; i < Dim<D>.Count; i++)
          Ret = Ret & AccessExpr<bool, D>.Make(argA, i);
        return Ret.Underlying;
      }
      public override string Format(string s) {
        return "All(" + s + ")";
      }
    }
    public virtual BoolBl Any {
      get { return AnyOp.Instance.Make(this); }
    }
    public virtual BoolBl All {
      get { return AllOp.Instance.Make(this); }
    }

  }
  public abstract partial class BoolBl<T, BRAND, D> : BaseBoolBl<T, BRAND, D>
    where BRAND : BoolBl<T, BRAND, D>
    where D : Dim<D> {
    public BoolBl(Expr<T> Underlying) : base(Underlying) { }
    public BoolBl(Expr<IVec<bool, D>> Underlying) : base(Underlying) { }
    public BoolBl(BoolBl[] Es) : base(Es) { }
    public BoolBl() : base() { }

    public static BRAND operator |(BRAND opA, BoolBl<T, BRAND, D> opB) {
      return ToBrandD(OrOp.Instance.Make(opA, opB));
    }
    public static BRAND operator &(BRAND opA, BoolBl<T, BRAND, D> opB) {
      return ToBrandD(AndOp.Instance.Make(opA, opB));
    }
    public static BRAND operator ^(BRAND opA, BoolBl<T, BRAND, D> opB) {
      return ToBrandD(ExclusiveOrOp.Instance.Make(opA, opB));
    }
  }
  public abstract partial class BaseBoolBl : BoolBl<bool, BoolBl, D1> {
    public BaseBoolBl(Expr<bool> Underlying) : base(Underlying) { }
    public BaseBoolBl(Expr<IVec<bool, D1>> Underlying) : base(Underlying) { }
    public BaseBoolBl(BoolBl[] Es) : base(Es) { }
    public BaseBoolBl() : base() { }
    public BRAND Condition<BRAND>(IValueBrand<BRAND> ArgA, BRAND ArgB) where BRAND : IValueBrand<BRAND> {
      return ArgA.Condition(this, ArgB);
    }
  }
  public partial class BoolBl : BaseBoolBl {
    public BoolBl(Expr<bool> Underlying) : base(Underlying) { }
    public BoolBl(Expr<IVec<bool, D1>> Underlying) : base(Underlying) { }
    public BoolBl(BoolBl[] Es) : base(Es) { }
    public BoolBl() : base() { }

    public static implicit operator BoolBl(Expr<bool> Underlying) { return new BoolBl(Underlying); }
    public static implicit operator BoolBl(bool Underlying) { return new Constant<bool>(Underlying); }
    public BRAND Condition<BRAND>(BRAND ArgA, IValueBrand<BRAND> ArgB) where BRAND : IValueBrand<BRAND> {
      return Condition((IValueBrand<BRAND>)ArgA, (BRAND)ArgB);
    }
  }
  public abstract partial class BaseBoolBl<D, BRAND> : BoolBl<IVec<bool, D>, BRAND, D>
    where D : Dim<D>
    where BRAND : BoolBl<D, BRAND> {
    public BaseBoolBl(Expr<IVec<bool, D>> Underlying) : base(Underlying) { }
    public BaseBoolBl(BoolBl[] Es) : base(Es) { }
    public BaseBoolBl() : base() { }
    public VBRAND Condition<VBRAND, SCALAR>(IValueBrand<VBRAND, D, SCALAR> ArgA, VBRAND ArgB)
      where VBRAND : IValueBrand<VBRAND, D, SCALAR>
      where SCALAR : IValueBrand<SCALAR, D1, SCALAR> {
      return ArgA.Condition<BRAND>(this, ArgB);
    }
  }
  public abstract partial class BoolBl<D, BRAND> : BaseBoolBl<D,BRAND>
    where D : Dim<D>
    where BRAND : BoolBl<D, BRAND> {
    public BoolBl(Expr<IVec<bool, D>> Underlying) : base(Underlying) { }
    public BoolBl(BoolBl[] Es) : base(Es) { }
    public BoolBl() : base() { }

    public VBRAND Condition<VBRAND, SCALAR>(VBRAND ArgA, IValueBrand<VBRAND, D, SCALAR> ArgB)
      where VBRAND : IValueBrand<VBRAND, D, SCALAR>
      where SCALAR : IValueBrand<SCALAR, D1, SCALAR> {
      return this.Condition((IValueBrand<VBRAND, D, SCALAR>)ArgA, (VBRAND)ArgB);
    }
  }

  public partial class FlexBoolBl<D> : BoolBl<D, FlexBoolBl<D>> where D : Dim<D> {
    internal bool ConvertByAll = true;
    public FlexBoolBl(Expr<IVec<bool, D>> Underlying) : base(Underlying) { }
    public FlexBoolBl(BoolBl[] Es) : base(Es) { }
    public FlexBoolBl() : base() { }
    public static implicit operator FlexBoolBl<D>(Expr<IVec<bool, D>> Underlying) {
      return new FlexBoolBl<D>(Underlying);
    }
    public override int GetHashCode() {
      return base.GetHashCode() + ConvertByAll.GetHashCode();
    }
    public override bool Equals(object obj) {
      return base.Equals(obj) && ((FlexBoolBl<D>)obj).ConvertByAll == ConvertByAll;
    }

    public static implicit operator BoolBl(FlexBoolBl<D> Value) { return Value.Convert; }
    public static implicit operator FlexBoolBl<D>(BoolBl v) {
      var Es = new BoolBl[Dim<D>.Count];
      for (int i = 0; i < Dim<D>.Count; i++) Es[i] = v;
      return new FlexBoolBl<D>(Es);
    }
    public BoolBl Convert {
      get {
        return ConvertByAll ? All : Any;
      }
    }
  }
  /// <summary>
  /// Unlike flex bool, cannot be converted implicitly in unit bool. Also, supports only 1 dimension. 
  /// </summary>
  public partial class BoolBl<D> : BoolBl<D, BoolBl<D>> where D : Dim1D<D>, IMultiDim1D<D> {
    public BoolBl(Expr<IVec<bool, D>> Underlying) : base(Underlying) { }
    public BoolBl(BoolBl[] Es) : base(Es) { }
    public BoolBl() : base() { }
    public static implicit operator BoolBl<D>(FlexBoolBl<D> v) {
      return new BoolBl<D>(v.Underlying);
    }
    public static implicit operator BoolBl<D>(BoolBl V) {
      var Es = new BoolBl[Dim<D>.Count];
      for (int i = 0; i < Es.Length; i++) Es[i] = V;
      return new BoolBl<D>(Es);
    }

  }
  public interface INumeric<BRAND, D, SCALAR> : IValueBrand<BRAND, D, SCALAR>
    where BRAND : INumeric<BRAND, D, SCALAR>
    where SCALAR : INumeric<SCALAR, D1, SCALAR> where D : Dim<D> {
    BRAND Add(BRAND OpB);
    BRAND Subtract(BRAND OpB);
    BRAND Multiply(BRAND OpB);
    BRAND Divide(BRAND OpB);
    BRAND Mod(BRAND OpB);
    BRAND Negate { get; }
    BRAND Abs { get; }
    BRAND Min(BRAND other);
    BRAND Max(BRAND other); 
    BRAND Clamp(BRAND min, BRAND max);
  }

  public abstract partial class BaseNumericBrand<T, BRAND, K, D, SCALAR> : ValueBrand<T, BRAND, K, D, SCALAR>, INumeric<BRAND,D,SCALAR>
    where BRAND : NumericBrand<T, BRAND, K, D, SCALAR>
    where D : Dim<D>
    where SCALAR : NumericBrand<K, SCALAR, K, D1, SCALAR> {
    public BaseNumericBrand(Expr<T> Underlying) : base(Underlying) { }
    public BaseNumericBrand(Expr<IVec<K, D>> Underlying) : base(Underlying) { }
    public BaseNumericBrand(SCALAR[] Es) : base(Es) { }
    public BaseNumericBrand() : base() { }

    public abstract class BinaryOp<CNT> : BinaryOperatorD<K, CNT> where CNT : BinaryOp<CNT> { }
    public abstract class UnaryOp<CNT> : UnaryOperatorD<K, CNT> where CNT : UnaryOp<CNT> { }

    public partial class AddOp : BinaryOp<AddOp> {
      public new static AddOp Instance = new AddOp();
      private AddOp() { }
      public override Func<Expr<K>, Expr<K>, Expr<K>> Uniform {
        get { return BaseNumericBrand<K,SCALAR,K,D1,SCALAR>.AddOp.Instance.Bl(); }
      }
      public override string Name { get { return "+"; } }
      public override Expr<IVec<K, D>> InverseA(Expr<IVec<K, D>> argA, Expr<IVec<K, D>> argB, Expr<IVec<K, D>> result) {
        // result = argA + argB
        // argA = result - argB
        return ToBrandD(result) - ToBrandD(argB);
      }
      public override Expr<IVec<K, D>> InverseB(Expr<IVec<K, D>> argA, Expr<IVec<K, D>> argB, Expr<IVec<K, D>> result) {
        return ToBrandD(result) - ToBrandD(argA);
      }
    }
    public partial class SubtractOp : BinaryOp<SubtractOp> {
      public new static SubtractOp Instance = new SubtractOp();
      private SubtractOp() { }
      public override Func<Expr<K>, Expr<K>, Expr<K>> Uniform {
        get { return BaseNumericBrand<K, SCALAR, K, D1, SCALAR>.SubtractOp.Instance.Bl(); }
      }
      public override string Name { get { return "-"; } }
      public override Expr<IVec<K, D>> InverseA(Expr<IVec<K, D>> argA, Expr<IVec<K, D>> argB, Expr<IVec<K, D>> result) {
        // result = argA - argB
        return ToBrandD(result) + ToBrandD(argB);
      }
      public override Expr<IVec<K, D>> InverseB(Expr<IVec<K, D>> argA, Expr<IVec<K, D>> argB, Expr<IVec<K, D>> result) {
        return ToBrandD(argA) - ToBrandD(result);
      }
    }
    public partial class MultiplyOp : BinaryOp<MultiplyOp> {
      public new static MultiplyOp Instance = new MultiplyOp();
      private MultiplyOp() { }
      public override Func<Expr<K>, Expr<K>, Expr<K>> Uniform {
        get { return BaseNumericBrand<K, SCALAR, K, D1, SCALAR>.MultiplyOp.Instance.Bl(); }
      }
      public override string Name { get { return "*"; } }
      public override Expr<IVec<K, D>> InverseA(Expr<IVec<K, D>> argA, Expr<IVec<K, D>> argB, Expr<IVec<K, D>> result) {
        // result = argA * argB
        return ToBrandD(result) / ToBrandD(argB);
      }
      public override Expr<IVec<K, D>> InverseB(Expr<IVec<K, D>> argA, Expr<IVec<K, D>> argB, Expr<IVec<K, D>> result) {
        return ToBrandD(result) / ToBrandD(argA);
      }
    }
    public partial class DivideOp : BinaryOp<DivideOp> {
      public new static DivideOp Instance = new DivideOp();
      private DivideOp() { }
      public override Func<Expr<K>, Expr<K>, Expr<K>> Uniform {
        get { return BaseNumericBrand<K, SCALAR, K, D1, SCALAR>.DivideOp.Instance.Bl(); }
      }
      public override string Name { get { return "/"; } }
      public override Expr<IVec<K, D>> InverseA(Expr<IVec<K, D>> argA, Expr<IVec<K, D>> argB, Expr<IVec<K, D>> result) {
        // result = argA / argB
        return ToBrandD(result) * ToBrandD(argB);
      }
      public override Expr<IVec<K, D>> InverseB(Expr<IVec<K, D>> argA, Expr<IVec<K, D>> argB, Expr<IVec<K, D>> result) {
        // result / argA = 1 / argB
        // argA / result = argB
        return ToBrandD(argA) / ToBrandD(result);
      }
    }
    public partial class ModOp : BinaryOp<ModOp> {
      public new static ModOp Instance = new ModOp();
      private ModOp() { }
      public override Func<Expr<K>, Expr<K>, Expr<K>> Uniform {
        get { return BaseNumericBrand<K, SCALAR, K, D1, SCALAR>.ModOp.Instance.Bl(); }
      }
      public override string Name { get { return "%"; } }
    }
    public partial class NegateOp : UnaryOp<NegateOp> {
      public new static NegateOp Instance = new NegateOp();
      private NegateOp() { }
      public override string Name { get { return "-"; } }
      public override Func<Expr<K>, Expr<K>> Uniform {
        get { return BaseNumericBrand<K, SCALAR, K, D1, SCALAR>.NegateOp.Instance.Bl(); }
      }
      public override Expr<IVec<K, D>> Inverse(Expr<IVec<K, D>> argA, Expr<IVec<K, D>> result) {
        return - ToBrandD(result);
      }
      public override Expr<IVec<K, D>> Expand(Expr<IVec<K, D>> argA) {
        return Default - ToBrandD(argA);
      }
    }
    public abstract partial class CallUnaryOp<CNT> : UnaryOp<CNT> where CNT : CallUnaryOp<CNT> {
      public override string Format(string s) {
        return Name + "(" + s + ")";
      }
    }
    public abstract partial class CallBinaryOp<CNT> : BinaryOp<CNT> where CNT : CallBinaryOp<CNT> {
      public override string Format(string opA, string opB) {
        return Name + "(" + opA + ", " + opB + ")";
      }
    }
    public abstract partial class CallTrinaryOp<CNT> : TrinaryOperatorD<K, CNT> where CNT : CallTrinaryOp<CNT> {
      public override string Format(string opA, string opB, string opC) {
        return Name + "(" + opA + ", " + opB + ", " + opC + ")";
      }
    }

    public partial class GreaterThanOp : EqualityOp<GreaterThanOp> {
      public new static GreaterThanOp Instance = new GreaterThanOp();
      private GreaterThanOp() { }
      public override string Name { get { return ">"; } }
      public override Func<Expr<K>, Expr<K>, Expr<bool>> Uniform {
        get { return BaseNumericBrand<K, SCALAR, K, D1, SCALAR>.GreaterThanOp.Instance.Bl(); }
      }
    }
    public partial class LessThanOp : EqualityOp<LessThanOp> {
      public new static LessThanOp Instance = new LessThanOp();
      private LessThanOp() { }
      public override string Name { get { return "<"; } }
      public override Func<Expr<K>, Expr<K>, Expr<bool>> Uniform {
        get { return BaseNumericBrand<K, SCALAR, K, D1, SCALAR>.LessThanOp.Instance.Bl(); }
      }
    }
    public partial class GreaterThanEqualOp : EqualityOp<GreaterThanEqualOp> {
      public new static GreaterThanEqualOp Instance = new GreaterThanEqualOp();
      private GreaterThanEqualOp() { }
      public override string Name { get { return ">="; } }
      public override Func<Expr<K>, Expr<K>, Expr<bool>> Uniform {
        get { return BaseNumericBrand<K, SCALAR, K, D1, SCALAR>.GreaterThanEqualOp.Instance.Bl(); }
      }
    }
    public partial class LessThanEqualOp : EqualityOp<LessThanEqualOp> {
      public new static LessThanEqualOp Instance = new LessThanEqualOp();
      private LessThanEqualOp() { }
      public override string Name { get { return "<="; } }
      public override Func<Expr<K>, Expr<K>, Expr<bool>> Uniform {
        get { return BaseNumericBrand<K, SCALAR, K, D1, SCALAR>.LessThanEqualOp.Instance.Bl(); }
      }
    }
    public partial class AbsOp : CallUnaryOp<AbsOp> {
      public new static AbsOp Instance = new AbsOp();
      private AbsOp() { }
      public override string Name { get { return "Abs"; } }
      public override Func<Expr<K>, Expr<K>> Uniform {
        get { return BaseNumericBrand<K, SCALAR, K, D1, SCALAR>.AbsOp.Instance.Bl(); }
      }
      public override Expr<IVec<K, D>> Expand(Expr<IVec<K, D>> argA) {
        return (ToBrandD(argA) < Default).Condition(ToBrandD(argA).Negate, ToBrandD(argA));
      }
    }
    public partial class MinOp : CallBinaryOp<MinOp> {
      public new static MinOp Instance = new MinOp();
      private MinOp() { }
      public override string Name { get { return "Min"; } }
      public override Func<Expr<K>, Expr<K>, Expr<K>> Uniform {
        get { return BaseNumericBrand<K, SCALAR, K, D1, SCALAR>.MinOp.Instance.Bl(); }
      }
      public override Expr<IVec<K, D>> Expand(Expr<IVec<K, D>> argA, Expr<IVec<K, D>> argB) {
        return (ToBrandD(argA) < ToBrandD(argB)).Condition(ToBrandD(argA), ToBrandD(argB));
      }
    }
    public partial class MaxOp : CallBinaryOp<MaxOp> {
      public new static MaxOp Instance = new MaxOp();
      private MaxOp() { }
      public override string Name { get { return "Max"; } }
      public override Func<Expr<K>, Expr<K>, Expr<K>> Uniform {
        get { return BaseNumericBrand<K, SCALAR, K, D1, SCALAR>.MaxOp.Instance.Bl(); }
      }
      public override Expr<IVec<K, D>> Expand(Expr<IVec<K, D>> argA, Expr<IVec<K, D>> argB) {
        return (ToBrandD(argA) > ToBrandD(argB)).Condition(ToBrandD(argA), ToBrandD(argB));
      }
    }
    public partial class ClampOp : CallTrinaryOp<ClampOp> {
      public new static ClampOp Instance = new ClampOp();
      private ClampOp() { }
      public override string Name { get { return "Clamp"; } }
      public override Func<Expr<K>, Expr<K>, Expr<K>, Expr<K>> Uniform {
        get { return BaseNumericBrand<K, SCALAR, K, D1, SCALAR>.ClampOp.Instance.Bl(); }
      }
      public override Expr<IVec<K, D>> Expand(Expr<IVec<K, D>> argA, Expr<IVec<K, D>> argB, Expr<IVec<K, D>> argC) {
        return ToBrandD(argA).Max(ToBrandD(argB)).Min(ToBrandD(argC));
      }
    }

    public virtual BRAND Add(BRAND OpB) { return ToBrandD(AddOp.Instance.Make(this, OpB)); }
    public virtual BRAND Subtract(BRAND OpB) { return ToBrandD(SubtractOp.Instance.Make(this, OpB)); }
    public virtual BRAND Multiply(BRAND OpB) { return ToBrandD(MultiplyOp.Instance.Make(this, OpB)); }
    public virtual BRAND Divide(BRAND OpB) { return ToBrandD(DivideOp.Instance.Make(this, OpB)); }
    public virtual BRAND Mod(BRAND OpB) { return ToBrandD(ModOp.Instance.Make(this, OpB)); }
    public virtual BRAND Negate { get { return ToBrandD(NegateOp.Instance.Make(this)); } }
    public virtual BRAND Abs { get { return ToBrandD(AbsOp.Instance.Make(this)); } }
    public virtual BRAND Min(BRAND other) { return ToBrandD(MinOp.Instance.Make(this, other)); }
    public virtual BRAND Max(BRAND other) { return ToBrandD(MaxOp.Instance.Make(this, other)); }
    public virtual BRAND Clamp(BRAND min, BRAND max) { return ToBrandD(ClampOp.Instance.Make(this, min, max)); }
    public BRAND Clamp(BRAND max) { return Clamp(Default, max); }

    // operator syntax definitions.
    public static BRAND operator +(BaseNumericBrand<T, BRAND, K, D, SCALAR> OpA, BRAND OpB) {
      return OpA.Add(OpB);
    }
    public static BRAND operator -(BaseNumericBrand<T, BRAND, K, D, SCALAR> OpA, BRAND OpB) {
      return OpA.Subtract(OpB);
    }
    public static BRAND operator *(BaseNumericBrand<T, BRAND, K, D, SCALAR> OpA, BRAND OpB) {
      return OpA.Multiply(OpB);
    }
    public static BRAND operator /(BaseNumericBrand<T, BRAND, K, D, SCALAR> OpA, BRAND OpB) {
      return OpA.Divide(OpB);
    }
    public static BRAND operator %(BaseNumericBrand<T, BRAND, K, D, SCALAR> OpA, BRAND OpB) {
      return OpA.Mod(OpB);
    }
    public static FlexBoolBl<D> operator >(BaseNumericBrand<T, BRAND, K, D, SCALAR> OpA, BRAND OpB) {
      return GreaterThanOp.Instance.Make(OpA, OpB);
    }
    public static FlexBoolBl<D> operator <(BaseNumericBrand<T, BRAND, K, D, SCALAR> OpA, BRAND OpB) {
      return LessThanOp.Instance.Make(OpA, OpB);
    }
    public static FlexBoolBl<D> operator >=(BaseNumericBrand<T, BRAND, K, D, SCALAR> OpA, BRAND OpB) {
      return GreaterThanEqualOp.Instance.Make(OpA, OpB);
    }
    public static FlexBoolBl<D> operator <=(BaseNumericBrand<T, BRAND, K, D, SCALAR> OpA, BRAND OpB) {
      return LessThanEqualOp.Instance.Make(OpA, OpB);
    }

    public static BRAND operator -(BaseNumericBrand<T, BRAND, K, D, SCALAR> OpA) {
      return ToBrandD(OpA.Negate);
    }
    public static BRAND operator +(BaseNumericBrand<T, BRAND, K, D, SCALAR> OpA) {
      return (BRAND) OpA;
    }
    // derived
    public SCALAR Min() {
      var min = this[0];
      for (int i = 1; i < Dim<D>.Count; i++)
        min = min.Min(this[i]);
      return min;
    }
    public SCALAR Max() {
      var max = this[0];
      for (int i = 1; i < Dim<D>.Count; i++)
        max = max.Max(this[i]);
      return max;
    }
  }
  public abstract partial class NumericBrand<T, BRAND, K, D, SCALAR> : BaseNumericBrand<T, BRAND, K, D, SCALAR>
    where BRAND : NumericBrand<T, BRAND, K, D, SCALAR>
    where D : Dim<D>
    where SCALAR : NumericBrand<K, SCALAR, K, D1, SCALAR> {
    public NumericBrand(Expr<T> Underlying) : base(Underlying) { }
    public NumericBrand(Expr<IVec<K, D>> Underlying) : base(Underlying) { }
    public NumericBrand(SCALAR[] Es) : base(Es) { }
    public NumericBrand() : base() { }

    public static BRAND operator +(BRAND OpA, NumericBrand<T, BRAND, K, D, SCALAR> OpB) {
      return OpA.Add(OpB);
    }
    public static BRAND operator -(BRAND OpA, NumericBrand<T, BRAND, K, D, SCALAR> OpB) {
      return OpA.Subtract(OpB);
    }
    public static BRAND operator *(BRAND OpA, NumericBrand<T, BRAND, K, D, SCALAR> OpB) {
      return OpA.Multiply(OpB);
    }
    public static BRAND operator /(BRAND OpA, NumericBrand<T, BRAND, K, D, SCALAR> OpB) {
      return OpA.Divide(OpB);
    }
    public static BRAND operator %(BRAND OpA, NumericBrand<T, BRAND, K, D, SCALAR> OpB) {
      return OpA.Mod(OpB);
    }
    public static FlexBoolBl<D> operator >(BRAND OpA, NumericBrand<T, BRAND, K, D, SCALAR> OpB) {
      return GreaterThanOp.Instance.Make(OpA, OpB);
    }
    public static FlexBoolBl<D> operator <(BRAND OpA, NumericBrand<T, BRAND, K, D, SCALAR> OpB) {
      return LessThanOp.Instance.Make(OpA, OpB);
    }
    public static FlexBoolBl<D> operator >=(BRAND OpA, NumericBrand<T, BRAND, K, D, SCALAR> OpB) {
      return GreaterThanEqualOp.Instance.Make(OpA, OpB);
    }
    public static FlexBoolBl<D> operator <=(BRAND OpA, NumericBrand<T, BRAND, K, D, SCALAR> OpB) {
      return LessThanEqualOp.Instance.Make(OpA, OpB);
    }
  }

  public abstract partial class IntBl<T, BRAND, D> : NumericBrand<T, BRAND, int, D, IntBl>
    where BRAND : IntBl<T, BRAND, D>
    where D : Dim<D> {
    public IntBl(Expr<T> Underlying) : base(Underlying) { }
    public IntBl(Expr<IVec<int, D>> Underlying) : base(Underlying) { }
    public IntBl(IntBl[] Es) : base(Es) { }
    public IntBl() : base() { }
  }

  public partial class IntBl : IntBl<int, IntBl, D1> {
    public IntBl(Expr<int> Underlying) : base(Underlying) { }
    public IntBl(Expr<IVec<int, D1>> Underlying) : base(Underlying) { }
    public IntBl(IntBl[] Es) : base(Es) { }
    public IntBl() : base() { }
    public static implicit operator IntBl(IntBl<D1> v) {
      return new IntBl(v.Underlying);
    }
    public static implicit operator IntBl<D1>(IntBl v) {
      return new IntBl<D1>(new IntBl[] { v });
    }
    public static implicit operator IntBl(Expr<int> Underlying) { return new IntBl(Underlying); }
    public static implicit operator IntBl(int Underlying) { return new Constant<int>(Underlying); }
  }
  public partial class IntBl<D, BRAND> : IntBl<IVec<int, D>, BRAND, D>
    where D : Dim<D>
    where BRAND : IntBl<D, BRAND> {
    public IntBl(Expr<IVec<int, D>> Underlying) : base(Underlying) { }
    public IntBl(IntBl[] Es) : base(Es) { }
    public IntBl() : base() { }
  }
  public partial class FlexIntBl<D> : IntBl<D, FlexIntBl<D>> where D : Dim<D> {
    public FlexIntBl(Expr<IVec<int, D>> Underlying) : base(Underlying) { }
    public FlexIntBl(IntBl[] Es) : base(Es) { }
    public FlexIntBl() : base() { }
    public static implicit operator FlexIntBl<D>(Expr<IVec<int, D>> Underlying) { return new FlexIntBl<D>(Underlying); }
    // public static implicit operator FlexIntBl<D>(IVec<int, D> Underlying) { return new Constant<IVec<int, D>>(Underlying); }
  }
  public partial class IntBl<D> : IntBl<D, IntBl<D>> where D : Dim1D<D> {
    public IntBl(Expr<IVec<int, D>> Underlying) : base(Underlying) { }
    public IntBl(params IntBl[] Es) : base(Es) { }
    public IntBl() : base() { }
    public static implicit operator IntBl<D>(FlexIntBl<D> V) { return new IntBl<D>(V.Underlying); }
    public static implicit operator FlexIntBl<D>(IntBl<D> V) { return new FlexIntBl<D>(V.Underlying); }
    public static implicit operator IntBl<D>(Expr<IVec<int, D>> Underlying) { return new IntBl<D>(Underlying); }

    public static implicit operator IntBl<D>(IntBl V) {
      var Es = new IntBl[Dim<D>.Count];
      for (int i = 0; i < Es.Length; i++) Es[i] = V;
      return new IntBl<D>(Es);
    }
  }


  /// <summary>
  /// Base class of all double-like numeric types.
  /// </summary>
  public abstract partial class DoubleBl<T, BRAND, D> : NumericBrand<T, BRAND, double, D, DoubleBl>, IHasX
    where BRAND : DoubleBl<T, BRAND, D>
    where D : Dim<D> {
    public DoubleBl(Expr<T> Underlying) : base(Underlying) { }
    public DoubleBl(Expr<IVec<double, D>> Underlying) : base(Underlying) { }
    public DoubleBl(DoubleBl[] Es) : base(Es) { }
    public DoubleBl() : base() { }

    public partial class SwizzleOp<DX> : Ops.BaseOperatorX<IVec<double, D>, IVec<double, DX>> where DX : Dim<DX> {
      private readonly int[] Indices;
      public SwizzleOp(int[] Indices) { this.Indices = Indices; }
      public override string Format(string s) {
        string[] Codes = { "x", "y", "z", "w" };
        string ret = s + ".";
        for (int i = 0; i < Indices.Length; i++) {
          ret = ret + ((Indices[i] < Codes.Length) ? Codes[Indices[i]] : ("_" + Indices[i]));
        }
        return ret;
      }

      public override int GetHashCode() {
        var hc = 0;
        for (int i = 0; i < Indices.Length; i++) hc += (i + 1) * Indices[i];
        return hc;
      }
      public override bool Equals(object obj) {
        if (obj is SwizzleOp<DX>) {
          var other = (SwizzleOp<DX>)obj;
          for (int i = 0; i < Indices.Length; i++)
            if (Indices[i] != other.Indices[i]) return false;
          return true;
        }
        return base.Equals(obj);
      }
      public override Expr<IVec<double, DX>> Expand(Expr<IVec<double, D>> argA) {
        var Es = new Expr<double>[Dim<DX>.Count];
        for (int i = 0; i < Indices.Length; i++) {
          Es[i] = ToBrandD(argA)[Indices[i]];
        }
        return Composite<double, DX>.Make(Es);
      }
    }
    public DoubleBl X { get { return this[0]; } }
    public FlexDoubleBl<DX> Swizzle<DX>(params int[] Indices) where DX : Dim<DX> {
      return new FlexDoubleBl<DX>(new SwizzleOp<DX>(Indices).Make(this));
    }
    public Double2Bl Swizzle(int Index0, int Index1) {
      return Swizzle<D2>(Index0, Index1);
    }
    public Double3Bl Swizzle(int Index0, int Index1, int Index2) {
      return Swizzle<D3>(Index0, Index1, Index2);
    }
    public Double4Bl Swizzle(int Index0, int Index1, int Index2, int Index3) {
      return Swizzle<D4>(Index0, Index1, Index2, Index3);
    }
  }
  /// <summary>
  /// Basic 1D double for measuring distance, progress, deltas, and so on. 
  /// </summary>
  public partial class DoubleBl : DoubleBl<double, DoubleBl, D1> {
    public DoubleBl(Expr<double> Underlying) : base(Underlying) { }
    public DoubleBl(Expr<IVec<double, D1>> Underlying) : base(Underlying) { }
    public DoubleBl(DoubleBl[] Es) : base(Es) { }
    public DoubleBl() : base() { }
    public static implicit operator DoubleBl(FlexDoubleBl<D1> v) { return new DoubleBl(v.Underlying); }
    public static implicit operator FlexDoubleBl<D1>(DoubleBl v) {
      return new FlexDoubleBl<D1>(new DoubleBl[] { v });
    }
    public static implicit operator DoubleBl(Expr<double> E) { return new DoubleBl(E); }
    public static implicit operator DoubleBl(double d) { return new Constant<double>(d); }

    public Double2Bl AddX(DoubleBl X) { return new Double2Bl(X, this); }
    public Double2Bl AddY(DoubleBl Y) { return new Double2Bl(this, Y); }
  }
  public abstract partial class DoubleBl<D, BRAND> : DoubleBl<IVec<double, D>, BRAND, D>
    where D : Dim<D>
    where BRAND : DoubleBl<D, BRAND> {
    public DoubleBl(Expr<IVec<double, D>> Underlying) : base(Underlying) { }
    public DoubleBl(DoubleBl[] Es) : base(Es) { }
    public DoubleBl() : base() { }
  }
  // Flexible, can represent double vectors for any D, even D0!
  public partial class FlexDoubleBl<D> : DoubleBl<D, FlexDoubleBl<D>> where D : Dim<D> {
    public FlexDoubleBl(Expr<IVec<double, D>> Underlying) : base(Underlying) { }
    public FlexDoubleBl(params DoubleBl[] Es) : base(Es) { }
    public FlexDoubleBl() : base() { }
  }
  public abstract partial class FixDoubleBl<D, BRAND> : DoubleBl<D,BRAND>
    where D : Dim1D<D>, IMultiDim1D<D>
    where BRAND : FixDoubleBl<D, BRAND> {
    public FixDoubleBl(Expr<IVec<double, D>> Underlying) : base(Underlying) { }
    public FixDoubleBl(params DoubleBl[] Es) : base(Es) { }
    public FixDoubleBl() : base() { }

    public static implicit operator MatrixBl<D1, D>(FixDoubleBl<D, BRAND> b) {
      return new MatrixBl<D1, D>((m, n) => b[n]);
    }
    public static explicit operator MatrixBl<D, D1>(FixDoubleBl<D, BRAND> b) {
      return b.Transpose;
    }
    public MatrixBl<D, D1> Transpose {
      get { return ((MatrixBl<D1, D>)this).Transpose; }
    }
  }
  public interface IHasX {
    DoubleBl X { get; }
    FlexDoubleBl<DX> Swizzle<DX>(params int[] Indices) where DX : Dim<DX>;
  }
  public interface IHasXY : IHasX {
    DoubleBl Y { get; }
  }
  public interface IHasXYZ : IHasXY {
    DoubleBl Z { get; }
  }
  public interface IHasXYZW : IHasXYZ {
    DoubleBl W { get; }
  }
  public class X : Y {
    public override int Index { get { return 0; } }
  }
  public class Y : Z {
    public override int Index { get { return 1; } }
  }
  public class Z : W {
    public override int Index { get { return 2; } }
  }
  public class W {
    public virtual int Index { get { return 3; } }
  }

  public static class Swizzles {
    public static Double2Bl Sw<A, B>(this IHasXY V)
      where A : Y, new()
      where B : Y, new() {
      var Idx = new int[2];
      Idx[0] = (new A()).Index;
      Idx[1] = (new B()).Index;
      return V.Swizzle<D2>(Idx);
    }
    public static Double2Bl Sw<A, B>(this IHasXYZ V)
      where A : Z, new()
      where B : Z, new() {
      var Idx = new int[2];
      Idx[0] = (new A()).Index;
      Idx[1] = (new B()).Index;
      return V.Swizzle<D2>(Idx);
    }
    public static Double2Bl Sw<A, B>(this IHasXYZW V)
      where A : W, new()
      where B : W, new() {
      var Idx = new int[2];
      Idx[0] = (new A()).Index;
      Idx[1] = (new B()).Index;
      return V.Swizzle<D2>(Idx);
    }
    public static Double3Bl Sw<A, B, C>(this IHasXY V)
      where A : Y, new()
      where B : Y, new()
      where C : Y, new() {
      var Idx = new int[3];
      Idx[0] = (new A()).Index;
      Idx[1] = (new B()).Index;
      Idx[2] = (new C()).Index;
      return V.Swizzle<D3>(Idx);
    }
    public static Double3Bl Sw<A, B, C>(this IHasXYZ V)
      where A : Z, new()
      where B : Z, new()
      where C : Z, new() {
      var Idx = new int[3];
      Idx[0] = (new A()).Index;
      Idx[1] = (new B()).Index;
      Idx[2] = (new C()).Index;
      return V.Swizzle<D3>(Idx);
    }
    public static Double3Bl Sw<A, B, C>(this IHasXYZW V)
      where A : W, new()
      where B : W, new()
      where C : W, new() {
      var Idx = new int[3];
      Idx[0] = (new A()).Index;
      Idx[1] = (new B()).Index;
      Idx[2] = (new C()).Index;
      return V.Swizzle<D3>(Idx);
    }
    public static Double4Bl Sw<A, B, C, D>(this IHasXY V)
      where A : Y, new()
      where B : Y, new()
      where C : Y, new()
      where D : Y, new() {
      var Idx = new int[3];
      Idx[0] = (new A()).Index;
      Idx[1] = (new B()).Index;
      Idx[2] = (new C()).Index;
      Idx[3] = (new D()).Index;
      return V.Swizzle<D4>(Idx);
    }
    public static Double4Bl Sw<A, B, C, D>(this IHasXYZ V)
      where A : Z, new()
      where B : Z, new()
      where C : Z, new()
      where D : Z, new() {
      var Idx = new int[3];
      Idx[0] = (new A()).Index;
      Idx[1] = (new B()).Index;
      Idx[2] = (new C()).Index;
      Idx[3] = (new D()).Index;
      return V.Swizzle<D4>(Idx);
    }
    public static Double4Bl Sw<A, B, C, D>(this IHasXYZW V)
      where A : W, new()
      where B : W, new()
      where C : W, new()
      where D : W, new() {
      var Idx = new int[3];
      Idx[0] = (new A()).Index;
      Idx[1] = (new B()).Index;
      Idx[2] = (new C()).Index;
      Idx[3] = (new D()).Index;
      return V.Swizzle<D4>(Idx);
    }
    public static Double2Bl XX(this IHasX  V) { return V.Swizzle<D2>(0, 0); }
    public static Double3Bl XXX(this IHasX V) { return V.Swizzle<D3>(0, 0, 0); }
    public static Double4Bl XXXX(this IHasX V) { return V.Swizzle<D4>(0, 0, 0, 0); }
    public static Double2Bl YY(this IHasXY V) { return V.Sw<Y, Y>(); }
    public static Double3Bl YYY(this IHasXY V) { return V.Sw<Y, Y, Y>(); }
    public static Double4Bl YYYY(this IHasXY V) { return V.Sw<Y, Y, Y, Y>(); }
    public static Double2Bl ZZ(this IHasXYZ V) { return V.Sw<Z, Z>(); }
    public static Double3Bl ZZZ(this IHasXYZ V) { return V.Sw<Z, Z, Z>(); }
    public static Double4Bl ZZZZ(this IHasXYZ V) { return V.Sw<Z, Z, Z, Z>(); }
    public static Double2Bl WW(this IHasXYZW V) { return V.Sw<W, W>(); }
    public static Double3Bl WWW(this IHasXYZW V) { return V.Sw<W, W, W>(); }
    public static Double4Bl WWWW(this IHasXYZW V) { return V.Sw<W, W, W, W>(); }

    public static Double2Bl XY(this IHasXY V) { return V.Sw<X,Y>(); }
    public static Double2Bl YX(this IHasXY V) { return V.Sw<Y,X>(); }

    public static Double2Bl YZ(this IHasXYZ V) { return V.Sw<Y, Z>(); }
    public static Double2Bl XZ(this IHasXYZ V) { return V.Sw<X,Z>(); }
    public static Double2Bl ZY(this IHasXYZ V) { return V.Sw<Z,Y>(); }
    public static Double2Bl ZX(this IHasXYZ V) { return V.Sw<Z,X>(); }

    public static Double3Bl ZYX(this IHasXYZ V) { return V.Sw<Z,Y,X>(); }
    public static Double3Bl ZXY(this IHasXYZ V) { return V.Sw<Z,X,Y>(); }
    public static Double3Bl YZX(this IHasXYZ V) { return V.Sw<Y,Z,X>(); }
    public static Double3Bl XZY(this IHasXYZ V) { return V.Sw<X,Z,Y>(); }
    public static Double3Bl YXZ(this IHasXYZ V) { return V.Sw<Y,X,Z>(); }
    public static Double3Bl XYZ(this IHasXYZ V) { return V.Sw<X,Y,Z>(); }

    public static Double2Bl YW(this IHasXYZW V) { return V.Sw<Y, W>(); }
    public static Double2Bl ZW(this IHasXYZW V) { return V.Sw<Z, W>(); }
    public static Double2Bl XW(this IHasXYZW V) { return V.Sw<X, W>(); }
    public static Double2Bl WY(this IHasXYZW V) { return V.Sw<W, Y>(); }
    public static Double2Bl WZ(this IHasXYZW V) { return V.Sw<W, Z>(); }
    public static Double2Bl WX(this IHasXYZW V) { return V.Sw<W, X>(); }

    public static Double3Bl XYW(this IHasXYZW V) { return V.Sw<X, Y, W>(); }
    public static Double3Bl XZW(this IHasXYZW V) { return V.Sw<X, Z, W>(); }
    public static Double3Bl YXW(this IHasXYZW V) { return V.Sw<Y, X, W>(); }
    public static Double3Bl YZW(this IHasXYZW V) { return V.Sw<Y, Z, W>(); }
    public static Double3Bl ZXW(this IHasXYZW V) { return V.Sw<Z, X, W>(); }
    public static Double3Bl ZYW(this IHasXYZW V) { return V.Sw<Z, Y, W>(); }

    public static Double3Bl XWY(this IHasXYZW V) { return V.Sw<X, W, Y>(); }
    public static Double3Bl XWZ(this IHasXYZW V) { return V.Sw<X, W, Z>(); }
    public static Double3Bl YWX(this IHasXYZW V) { return V.Sw<Y, W, X>(); }
    public static Double3Bl YWZ(this IHasXYZW V) { return V.Sw<Y, W, Z>(); }
    public static Double3Bl ZWX(this IHasXYZW V) { return V.Sw<Z, W, X>(); }
    public static Double3Bl ZWY(this IHasXYZW V) { return V.Sw<Z, W, Y>(); }

    public static Double3Bl WXY(this IHasXYZW V) { return V.Sw<W, X, Y>(); }
    public static Double3Bl WXZ(this IHasXYZW V) { return V.Sw<W, X, Z>(); }
    public static Double3Bl WYX(this IHasXYZW V) { return V.Sw<W, Y, X>(); }
    public static Double3Bl WYZ(this IHasXYZW V) { return V.Sw<W, Y, Z>(); }
    public static Double3Bl WZX(this IHasXYZW V) { return V.Sw<W, Z, X>(); }
    public static Double3Bl WZY(this IHasXYZW V) { return V.Sw<W, Z, Y>(); }

    public static Double4Bl XYZW(this IHasXYZW V) { return V.Sw<X, Y, Z, W>(); }
    public static Double4Bl XZYW(this IHasXYZW V) { return V.Sw<X, Z, Y, W>(); }
    public static Double4Bl YXZW(this IHasXYZW V) { return V.Sw<Y, X, Z, W>(); }
    public static Double4Bl YZXW(this IHasXYZW V) { return V.Sw<Y, Z, X, W>(); }
    public static Double4Bl ZXYW(this IHasXYZW V) { return V.Sw<Z, X, Y, W>(); }
    public static Double4Bl ZYXW(this IHasXYZW V) { return V.Sw<Z, Y, X, W>(); }

    public static Double4Bl XYWZ(this IHasXYZW V) { return V.Sw<X, Y, W, Z>(); }
    public static Double4Bl XZWY(this IHasXYZW V) { return V.Sw<X, Z, W, Y>(); }
    public static Double4Bl YXWZ(this IHasXYZW V) { return V.Sw<Y, X, W, Z>(); }
    public static Double4Bl YZWX(this IHasXYZW V) { return V.Sw<Y, Z, W, X>(); }
    public static Double4Bl ZXWY(this IHasXYZW V) { return V.Sw<Z, X, W, Y>(); }
    public static Double4Bl ZYWX(this IHasXYZW V) { return V.Sw<Z, Y, W, X>(); }

    public static Double4Bl XWYZ(this IHasXYZW V) { return V.Sw<X, W, Y, Z>(); }
    public static Double4Bl XWZY(this IHasXYZW V) { return V.Sw<X, W, Z, Y>(); }
    public static Double4Bl YWXZ(this IHasXYZW V) { return V.Sw<Y, W, X, Z>(); }
    public static Double4Bl YWZX(this IHasXYZW V) { return V.Sw<Y, W, Z, X>(); }
    public static Double4Bl ZWXY(this IHasXYZW V) { return V.Sw<Z, W, X, Y>(); }
    public static Double4Bl ZWYX(this IHasXYZW V) { return V.Sw<Z, W, Y, X>(); }

    public static Double4Bl WXYZ(this IHasXYZW V) { return V.Sw<W, X, Y, Z>(); }
    public static Double4Bl WXZY(this IHasXYZW V) { return V.Sw<W, X, Z, Y>(); }
    public static Double4Bl WYXZ(this IHasXYZW V) { return V.Sw<W, Y, X, Z>(); }
    public static Double4Bl WYZX(this IHasXYZW V) { return V.Sw<W, Y, Z, X>(); }
    public static Double4Bl WZXY(this IHasXYZW V) { return V.Sw<W, Z, X, Y>(); }
    public static Double4Bl WZYX(this IHasXYZW V) { return V.Sw<W, Z, Y, X>(); }
  }


  public partial class Double2Bl : FixDoubleBl<D2, Double2Bl>, IHasXY {
    public Double2Bl(Expr<IVec<double, D2>> Underlying) : base(Underlying) { }
    public Double2Bl(DoubleBl X, DoubleBl Y) : base(X, Y) { }
    public Double2Bl() : base() { }
    public static implicit operator Double2Bl(FlexDoubleBl<D2> v) { return new Double2Bl(v.Underlying); }
    public static implicit operator Double2Bl(MatrixBl<D1, D2> D) { return new Double2Bl(D[0, 0], D[0, 1]); }
    public static explicit operator Double2Bl(MatrixBl<D2, D1> D) { return D.Transpose; }
    public static implicit operator FlexDoubleBl<D2>(Double2Bl v) { return new FlexDoubleBl<D2>(v.Underlying); }
    public static implicit operator Double2Bl(DoubleBl d) { return d.XX(); }
    public static implicit operator Double2Bl(Expr<IVec<double, D2>> E) { return new Double2Bl(E); }
    public DoubleBl Y { get { return this[1]; } }

    public Double3Bl AddX(DoubleBl X) { return new Double3Bl(X, this.X, this.Y); }
    public Double3Bl AddY(DoubleBl Y) { return new Double3Bl(this.X, Y, this.Y); }
    public Double3Bl AddZ(DoubleBl Z) { return new Double3Bl(this, Z); }
  }
  public partial class Double3Bl : FixDoubleBl<D3, Double3Bl>, IHasXYZ {
    public Double3Bl(Expr<IVec<double, D3>> Underlying) : base(Underlying) { }
    public Double3Bl(DoubleBl X, DoubleBl Y, DoubleBl Z) : base(X, Y, Z) { }
    public Double3Bl(Double2Bl XY, DoubleBl Z) : base(XY.X, XY.Y, Z) { }
    public Double3Bl() : base() { }
    public static implicit operator Double3Bl(MatrixBl<D1, D3> D) { return new Double3Bl(D[0, 0], D[0, 1], D[0, 2]); }
    public static explicit operator Double3Bl(MatrixBl<D3, D1> D) { return D.Transpose; }
    public static implicit operator Double3Bl(FlexDoubleBl<D3> v) { return new Double3Bl(v.Underlying); }
    public static implicit operator FlexDoubleBl<D3>(Double3Bl v) { return new FlexDoubleBl<D3>(v.Underlying); }
    public static implicit operator Double3Bl(DoubleBl d) { return d.XXX(); }
    public static implicit operator Double3Bl(Expr<IVec<double, D3>> E) { return new Double3Bl(E); }
    public DoubleBl Y { get { return this[1]; } }
    public DoubleBl Z { get { return this[2]; } }

    public Double4Bl AddX(DoubleBl X) { return new Double4Bl(X, this.X, this.Y, this.Z); }
    public Double4Bl AddY(DoubleBl Y) { return new Double4Bl(this.X, Y, this.Y, this.Z); }
    public Double4Bl AddZ(DoubleBl Z) { return new Double4Bl(this.X, this.Y, Z, this.Z); }
    public Double4Bl AddW(DoubleBl W) { return new Double4Bl(this, W); }
  }
  public partial class Double4Bl : FixDoubleBl<D4, Double4Bl>, IHasXYZW {
    public Double4Bl(Expr<IVec<double, D4>> Underlying) : base(Underlying) { }
    public Double4Bl(DoubleBl X, DoubleBl Y, DoubleBl Z, DoubleBl W) : base(X, Y, Z, W) { }
    public Double4Bl(Double3Bl XYZ, DoubleBl W) : base(XYZ.X, XYZ.Y, XYZ.Z, W) { }
    public Double4Bl(Double2Bl XY, Double2Bl ZW) : base(XY.X, XY.Y, ZW.X, ZW.Y) { }
    public Double4Bl() : base() { }

    public static implicit operator Double4Bl(MatrixBl<D1, D4> D) { return new Double4Bl(D[0, 0], D[0, 1], D[0, 2], D[0, 3]); }
    public static explicit operator Double4Bl(MatrixBl<D4, D1> D) { return D.Transpose; }
    public static implicit operator Double4Bl(FlexDoubleBl<D4> v) { return new Double4Bl(v.Underlying); }
    public static implicit operator FlexDoubleBl<D4>(Double4Bl v) { return new FlexDoubleBl<D4>(v.Underlying); }
    public static implicit operator Double4Bl(DoubleBl d) { return d.XXXX(); }
    public static implicit operator Double4Bl(Expr<IVec<double, D4>> E) { return new Double4Bl(E); }

    public static implicit operator Double4Bl(Double3Bl D) { return new Double4Bl(D, 1d); }
    public static explicit operator Double3Bl(Double4Bl D) { return D.XYZ() / D.W; }
    public DoubleBl Y { get { return this[1]; } }
    public DoubleBl Z { get { return this[2]; } }
    public DoubleBl W { get { return this[3]; } }
  }



  public abstract partial class BaseMatrixBl<D, BRAND> : DoubleBl<D, BRAND>
    where D : Dim2D<D>
    where BRAND : BaseMatrixBl<D, BRAND> {
    public BaseMatrixBl(Expr<IVec<double, D>> Underlying) : base(Underlying) { }
    public BaseMatrixBl(DoubleBl[] Es) : base(Es) { }
    public BaseMatrixBl(DoubleBl[,] Es) : base(Convert(Es)) { }
    public BaseMatrixBl() : base() { }
    private static DoubleBl[] Convert(DoubleBl[,] Es) {
      var Es0 = new DoubleBl[Dim<D>.Count];
      for (int m = 0; m < Es.GetLength(0); m++)
        for (int n = 0; n < Es.GetLength(1); n++) {
          Es0[m * Es.GetLength(1) + n] = Es[m, n];
        }
      return Es0;
    }
  }
  public abstract partial class MatrixBl<M, N, BRAND> : BaseMatrixBl<Dim<M,N>, BRAND>
    where M : Dim1D<M> where N : Dim1D<N>
    where BRAND : MatrixBl<M, N, BRAND> {
    public MatrixBl(Expr<IVec<double, Dim<M,N>>> Underlying) : base(Underlying) { }
    public MatrixBl(DoubleBl[] Es) : base(Es) { }
    public MatrixBl(DoubleBl[,] Es) : base(Es) { }
    public MatrixBl() : base() { }
    public MatrixBl(Func<int, int, DoubleBl> F) : this(Convert(F)) { }
    public MatrixBl(params MatrixBl<D1, N>[] Rows) : this(Convert(Rows)) { }
    public MatrixBl(params MatrixBl<M, D1>[] Columns) : this(Convert(Columns)) { }

    public DoubleBl this[int m, int n] {
      get { return new Access2DExpr<double, Dim<M, N>>(Underlying, m, n); }
    }
    private static DoubleBl[] Convert(MatrixBl<D1, N>[] Rows) {
      (Rows.Length == Dim<M>.Count).Assert();
      var Ds = new DoubleBl[Dim<N>.Count * Dim<M>.Count];
      for (int i = 0; i < Dim<M>.Count; i++) {
        for (int j = 0; j < Dim<N>.Count; j++) {
          Ds[i * Dim<N>.Count + j] = Rows[i][j];
        }
      }
      return Ds;
    }
    private static DoubleBl[] Convert(MatrixBl<M,D1>[] Columns) {
      (Columns.Length == Dim<N>.Count).Assert();
      var Ds = new DoubleBl[Dim<N>.Count * Dim<M>.Count];
      for (int i = 0; i < Dim<M>.Count; i++) {
        for (int j = 0; j < Dim<N>.Count; j++) {
          Ds[i * Dim<N>.Count + j] = Columns[j][i];
        }
      }
      return Ds;
    }

    private static DoubleBl[] Convert(Func<int, int, DoubleBl> F) {
      var Es0 = new DoubleBl[Dim<M,N>.Count];
      for (int m = 0; m < Dim1D<M>.Count; m++)
        for (int n = 0; n < Dim1D<N>.Count; n++) {
          Es0[m * Dim1D<N>.Count + n] = F(m,n);
        }
      return Es0;
    }
    public MatrixBl<N, M> Transpose {
      get { return new MatrixBl<N, M>((m, n) => this[n, m]); }
    }
  }
  public partial class MatrixBl<M, N> : MatrixBl<M, N, MatrixBl<M, N>> where M : Dim1D<M> where N : Dim1D<N> {
    public MatrixBl(Expr<IVec<double, Dim<M, N>>> Underlying) : base(Underlying) { }
    public MatrixBl(params DoubleBl[] Es) : base(Es) { }
    public MatrixBl(DoubleBl[,] Es) : base(Es) { }
    public MatrixBl() : base() { }
    public MatrixBl(Func<int, int, DoubleBl> F) : base(F) { }
    public MatrixBl(params MatrixBl<D1, N>[] Rows) : base(Rows) { }
    public MatrixBl(params MatrixBl<M, D1>[] Columns) : base(Columns) { }
    public static implicit operator MatrixBl<M, N>(Expr<IVec<double, Dim<M, N>>> M) { return new MatrixBl<M, N>(M); }
  }
  public partial class Matrix2Bl : MatrixBl<D2, D2, Matrix2Bl> {
    public Matrix2Bl(Expr<IVec<double, Dim<D2, D2>>> Underlying) : base(Underlying) { }
    public Matrix2Bl(params DoubleBl[] Es) : base(Es) { }
    public Matrix2Bl(DoubleBl[,] Es) : base(Es) { }
    public Matrix2Bl(Double2Bl rowA, Double2Bl rowB) : base(rowA, rowB) { }
    public Matrix2Bl(MatrixBl<D2, D1> colA, MatrixBl<D2, D1> colB) : base(colA,colB) { }
    public Matrix2Bl() : base() { }
    public Matrix2Bl(Func<int, int, DoubleBl> F) : base(F) { }
    public static implicit operator MatrixBl<D2, D2>(Matrix2Bl M) { return new MatrixBl<D2, D2>(M.Underlying); }
    public static implicit operator Matrix2Bl(MatrixBl<D2, D2> M) { return new Matrix2Bl(M.Underlying); }
    public static implicit operator Matrix2Bl(Expr<IVec<double,Dim<D2,D2>>> M) { return new Matrix2Bl(M); }
    public static Matrix2Bl ByColumns(Double2Bl colA, Double2Bl colB) {
      return new Matrix2Bl(colA.Transpose, colB.Transpose);
    }
  }
  public partial class Matrix3Bl : MatrixBl<D3, D3, Matrix3Bl> {
    public Matrix3Bl(Expr<IVec<double, Dim<D3, D3>>> Underlying) : base(Underlying) { }
    public Matrix3Bl(params DoubleBl[] Es) : base(Es) { }
    public Matrix3Bl(DoubleBl[,] Es) : base(Es) { }
    public Matrix3Bl(Double3Bl rowA, Double3Bl rowB, Double3Bl rowC) : base(rowA, rowB, rowC) { }
    public Matrix3Bl(MatrixBl<D3, D1> colA, MatrixBl<D3, D1> colB, MatrixBl<D3, D1> colC) : base(colA, colB, colC) { }
    public Matrix3Bl() : base() { }
    public Matrix3Bl(Func<int, int, DoubleBl> F) : base(F) { }
    public static implicit operator MatrixBl<D3, D3>(Matrix3Bl M) { return new MatrixBl<D3, D3>(M.Underlying); }
    public static implicit operator Matrix3Bl(MatrixBl<D3, D3> M) { return new Matrix3Bl(M.Underlying); }
    public static implicit operator Matrix3Bl(Expr<IVec<double, Dim<D3, D3>>> M) { return new Matrix3Bl(M); }
    public static Matrix3Bl ByColumns(Double3Bl colA, Double3Bl colB, Double3Bl colC) {
      return new Matrix3Bl(colA.Transpose, colB.Transpose, colC.Transpose);
    }
  }
  public partial class Matrix4Bl : MatrixBl<D4, D4, Matrix4Bl> {
    public Matrix4Bl(Expr<IVec<double, Dim<D4, D4>>> Underlying) : base(Underlying) { }
    public Matrix4Bl(params DoubleBl[] Es) : base(Es) { }
    public Matrix4Bl(DoubleBl[,] Es) : base(Es) { }
    public Matrix4Bl(Double4Bl rowA, Double4Bl rowB, Double4Bl rowC, Double4Bl rowD) : base(rowA, rowB, rowC, rowD) { }
    public Matrix4Bl(MatrixBl<D4, D1> colA, MatrixBl<D4, D1> colB, MatrixBl<D4, D1> colC, MatrixBl<D4, D1> colD) : base(colA, colB, colC, colD) { }
    public Matrix4Bl() : base() { }
    public Matrix4Bl(Func<int, int, DoubleBl> F) : base(F) { }
    public static implicit operator MatrixBl<D4, D4>(Matrix4Bl M) { return new MatrixBl<D4, D4>(M.Underlying); }
    public static implicit operator Matrix4Bl(MatrixBl<D4, D4> M) { return new Matrix4Bl(M.Underlying); }
    public static implicit operator Matrix4Bl(Expr<IVec<double, Dim<D4, D4>>> M) { return new Matrix4Bl(M); }
    public static Matrix4Bl ByColumns(Double4Bl colA, Double4Bl colB, Double4Bl colC, Double4Bl colD) {
      return new Matrix4Bl(colA.Transpose, colB.Transpose, colC.Transpose, colD.Transpose);
    }
  }


  public static class BrandExtensions {
    public static DoubleBl Bl(this Expr<double> Value) { return Value; }
    public static DoubleBl Bl(this double Value) { return Value; }
    public static DoubleBl Bl(this FlexDoubleBl<D1> Value) { return Value; }

    public static Double2Bl Bl(this Expr<IVec<double,D2>> Value) { return Value; }
    public static Double2Bl Bl(this IVec<double, D2> Value) { return new Constant<IVec<double,D2>>(Value); }
    public static Double2Bl Bl(this FlexDoubleBl<D2> Value) { return Value; }

    public static Double3Bl Bl(this Expr<IVec<double, D3>> Value) { return Value; }
    public static Double3Bl Bl(this IVec<double, D3> Value) { return new Constant<IVec<double, D3>>(Value); }
    public static Double3Bl Bl(this FlexDoubleBl<D3> Value) { return Value; }

    public static Double4Bl Bl(this Expr<IVec<double, D4>> Value) { return Value; }
    public static Double4Bl Bl(this IVec<double, D4> Value) { return new Constant<IVec<double, D4>>(Value); }
    public static Double4Bl Bl(this FlexDoubleBl<D4> Value) { return Value; }

    public static BoolBl Bl(this Expr<bool> Value) { return Value; }
    public static BoolBl Bl(this bool Value) { return Value; }
    public static BoolBl Bl<D>(this FlexBoolBl<D> Value) where D : Dim<D> { return Value; }
    public static BoolBl<D> Cl<D>(this FlexBoolBl<D> Value) where D : Dim1D<D>, IMultiDim1D<D> { return Value; }

    public static IntBl Bl(this Expr<int> Value) { return Value; }
    public static IntBl Bl(this int Value) { return Value; }
    public static IntBl Bl(this IntBl<D1> Value) { return Value; }

    public static Matrix2Bl Bl(this MatrixBl<D2, D2> Value) { return Value; }
    public static Matrix3Bl Bl(this MatrixBl<D3, D3> Value) { return Value; }
    public static Matrix4Bl Bl(this MatrixBl<D4, D4> Value) { return Value; }


    public static Func<Expr<A>, Expr<C>> Bl<A, C>(this Func<Expr<IVec<A,D1>>, Expr<IVec<C, D1>>> F) {
      return (a) => AccessExpr<C,D1>.Make(F(Composite<A,D1>.Make(a)), 0);
    }
    public static Func<Expr<A>, Expr<B>, Expr<C>> Bl<A, B, C>(this Func<Expr<IVec<A, D1>>, Expr<IVec<B, D1>>, Expr<IVec<C, D1>>> F) {
      return (a, b) => AccessExpr<C, D1>.Make(F(Composite<A, D1>.Make(a), Composite<B, D1>.Make(b)), 0);
    }
    public static Func<Expr<A>, Expr<B>, Expr<C>, Expr<D>> Bl<A, B, C, D>(this Func<Expr<IVec<A, D1>>, Expr<IVec<B, D1>>, Expr<IVec<C, D1>>, Expr<IVec<D, D1>>> F) {
      return (a, b, c) => AccessExpr<D, D1>.Make(F(Composite<A, D1>.Make(a), Composite<B, D1>.Make(b), Composite<C, D1>.Make(c)), 0);
    }
    public static Func<Expr<A>, Expr<C>> Bl<A, C>(this Ops.IOperatorX<IVec<A, D1>, IVec<C, D1>> F) {
      Func<Expr<IVec<A,D1>>, Expr<IVec<C, D1>>> G = F.Make;
      return G.Bl();
    }
    public static Func<Expr<A>, Expr<B>, Expr<C>> Bl<A, B, C>(this Ops.OperatorX<IVec<A, D1>, IVec<B, D1>, IVec<C, D1>> F) {
      Func<Expr<IVec<A, D1>>, Expr<IVec<B, D1>>, Expr<IVec<C, D1>>> G = F.Make;
      return G.Bl();
    }
    public static Func<Expr<A>, Expr<B>, Expr<C>, Expr<D>> Bl<A, B, C, D>(this Ops.OperatorX<IVec<A, D1>, IVec<B, D1>, IVec<C, D1>, IVec<D, D1>> F) {
      Func<Expr<IVec<A, D1>>, Expr<IVec<B, D1>>, Expr<IVec<C, D1>>, Expr<IVec<D, D1>>> G = F.Make;
      return G.Bl();
    }


  }

}