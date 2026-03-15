using System;
using System.Collections.Generic;
using Bling.Util;
using Bling.DSL;
using Bling.Core;
using Bling.Graphics;

namespace Bling.Ops {
  using Vecs;
  using Shaders;
  public partial interface IOperatorX<S, T> : IOperator<T> {
    Func<string, string> Shader<EVAL>(BaseShaderEval<EVAL> txt, Operation<S, T> op) where EVAL : Eval<EVAL>;
  }
  public partial interface OperatorX<S, T, U> : IOperator<U> {
    Func<string, string, string> Shader<EVAL>(BaseShaderEval<EVAL> txt, Operation<S, T, U> op) where EVAL : Eval<EVAL>;
  }
  public partial interface OperatorX<S, T, U, V> : IOperator<V> {
    Func<string, string, string, string> Shader(Operation<S, T, U, V> op);
  }
  public partial interface OperatorX<S, T, U, V, W> : IOperator<W> {
    Func<string, string, string, string, string> Shader(Operation<S, T, U, V, W> op);
  }
  public partial class BaseOperatorX<S, T> : BaseOperatorX<T>, IOperatorX<S, T> {
    public virtual Func<string, string> Shader(Operation<S, T> op) { return null; }
    public virtual Func<string, string> Shader<EVAL>(BaseShaderEval<EVAL> txt, Operation<S, T> op) where EVAL : Eval<EVAL> { return Shader(op); }
  }
  public partial class BaseOperatorX<S, T, U> {
    public virtual Func<string, string, string> Shader<EVAL>(BaseShaderEval<EVAL> txt, Operation<S, T, U> op) where EVAL : Eval<EVAL> { return null; }
  }
  public partial class BaseOperatorX<S, T, U, V> {
    public virtual Func<string, string, string, string> Shader(Operation<S, T, U, V> op) { return null; }
  }
  public partial class BaseOperatorX<S, T, U, V, W> {
    public virtual Func<string, string, string, string, string> Shader(Operation<S, T, U, V, W> op) { return null; }
  }
  public abstract partial class BinaryOperator<S, T, CNT> : AssociativeOperator<S, T, CNT> {
    public override Func<string, string, string> Shader<EVAL>(BaseShaderEval<EVAL> txt, Operation<S, S, T> op) {
      return Format;
    }
  }
  public abstract partial class BaseStaticCallOperator<T, S, CNT> : Operator<T, S, CNT> {
    public virtual string ShaderCallName { get { return CallName.ToLower(); } }
    public override Func<string, string> Shader(Operation<T, S> op) {
      return (t) => ShaderCallName + "(" + t + ")";
    }
  }
  public abstract partial class StaticCallOperator<T, S, U, CNT> : Operator<T, S, U, CNT> {
    public virtual string ShaderCallName { get { return CallName.ToLower(); } }
    public override Func<string, string, string> Shader<EVAL>(BaseShaderEval<EVAL> txt, Operation<T, S, U> op) {
      return (t, s) => ShaderCallName + "(" + t + ", " + s + ")";
    }
  }
  public abstract partial class StaticCallOperator<T, S, U, V, CNT> : Operator<T, S, U, V, CNT> {
    public virtual string ShaderCallName { get { return CallName.ToLower(); } }
    public override Func<string, string, string, string> Shader(Operation<T, S, U, V> op) {
      return (t, s, u) => ShaderCallName + "(" + t + ", " + s + ", " + u + ")";
    } 
  }
  public partial class Operator<S, T, U, V, CNT> : BaseOperatorX<S, T, U, V>, IOperator<S, T, U, V, CNT> where CNT : Operator<S,T,U,V,CNT> {
    public override Func<string, string, string, string> Shader(Operation<S, T, U, V> op) { return Format; }
  }
  public abstract partial class CastOperator<S, T, CNT> : Operator<S, T, CNT> {
    public override Func<string, string> Shader<EVAL>(BaseShaderEval<EVAL> txt, Operation<S, T> op) {
      return s => "(" + txt.TypeNameFor(typeof(T)) + ") " + s;
    }

  }
  public abstract partial class UniformUnaryOperator<T, K, CNT> : Ops.Operator<T, T, CNT> {
    public override Func<string, string> Shader(Operation<T, T> op) { return Format; }
  }
  public static partial class DoubleOperators0<T> {
    public partial class Recip : MathOperator<Recip> {
      public override Func<string, string> Shader(Operation<T, T> op) {
        return null;
      }
    }

    public partial class Ceiling : MathOperator<Ceiling> {
      public override string ShaderCallName { get { return "ceil"; } }
    }
    public partial class Exp10 : MathOperator<Exp10> {
      public override Func<string, string> Shader(Operation<T, T> op) { return null; }
    }
    public partial class LogN : Math2Operator<LogN> {
      public override Func<string, string, string> Shader<EVAL>(BaseShaderEval<EVAL> txt, Operation<T, T, T> op) { return null; }
    }
    public partial class LengthSquared : StaticCallOperator<T, double, LengthSquared> {
      public override Func<string, string> Shader(Operation<T, double> op) { return null; }
    }
    public partial class Square : MathOperator<Square> {
      public override Func<string, string> Shader(Operation<T, T> op) { return null; }
    }
  }
  public partial class Swizzle<K, S, T> : BaseOperatorX<S, T> {
    public override Func<string, string> Shader(Operation<S, T> op) {
      return Format;
    }
  }
  public partial class TupleConstruct<K, S, T, U> : Operator<S, T, U, TupleConstruct<K, S, T, U>>,
    ITupleConstruct<K, U, TupleConstruct<K, S, T, U>> {
    public override Func<string, string, string> Shader<EVAL>(BaseShaderEval<EVAL> txt, Operation<S, T, U> op) {
      string prefix = null;
      if (typeof(K) == typeof(double) || typeof(K) == typeof(float)) prefix = "float";
      else if (typeof(K) == typeof(int)) prefix = "int";
      if (prefix == null) return base.Shader(txt, op);
      else return (a, b) => prefix + Arity<K, U>.ArityI.Count + "(" + a + ", " + b + ")";
    }

  }
  public static partial class DoubleOperators<T, B> {
    public partial class IsInfinity : DoubleOperator<IsInfinity> {
      public override string ShaderCallName { get { return "isinf"; } }
    }
    public partial class IsPositiveInfinity : DoubleOperator<IsPositiveInfinity> {
      public override Func<string, string> Shader(Operation<T, B> op) { return null; }
    }
    public partial class IsNegativeInfinity : DoubleOperator<IsNegativeInfinity> {
      public override Func<string, string> Shader(Operation<T, B> op) { return null; }
    }
  }
  public static partial class Trig1Operators<T> {
    public partial class SinCos : StaticCallOperator<double, T, SinCos> {
      public override Func<string, string> Shader(Operation<double, T> op) { return null; }
    }
  }
  public static partial class VecConvert<T, K, VEC> where VEC : Vec<K> {
    public abstract partial class Operator<A, B, CNT> : Ops.Operator<A, B, CNT> where CNT : Operator<A,B,CNT> {
      // no-op.
      public override Func<string, string> Shader(Operation<A, B> op) { return s => s; }
    }
  }

}
namespace Bling.Shaders {
  using Ops = Bling.Ops;
  using Brands = Bling.Core;
  using Bling.Core;

  public partial interface ITex2D { }
  public partial class Tex2DOp<TEX, P, CLR> : Ops.Operator<TEX, P, CLR, Tex2DOp<TEX, P, CLR>>, ITex2D {
    public new static readonly Tex2DOp<TEX,P,CLR> Instance = new Tex2DOp<TEX,P,CLR>();
    private Tex2DOp() {}
    public override string Format(string argA, string argB) {
      return argA + "[" + argB + "]";
    }
    public override Func<string, string, string> Shader<EVAL>(BaseShaderEval<EVAL> txt, Ops.Operation<TEX, P, CLR> op) {
      return (s, t) => "tex2D(" + s + ", " + t + ")";
    }
  }
  partial class ToTexture<T> : Ops.Operator<T, T, ToTexture<T>> {
    public new static readonly ToTexture<T> Instance = new ToTexture<T>();
    private ToTexture() { }
    public override Eval<EVAL>.Info<T> Eval<EVAL>(Bling.Ops.Operation<T, T> op, Eval<EVAL> txt) {
      if (txt is ShaderEval<EVAL>) {
        var txt0 = ((ShaderEval<EVAL>)txt);
        var t0 = new ShaderEval<EVAL>.MyInfo<T>() { InShaderStatus = false, Value = null };
        txt0.Registers.BringIn<T>(op.ArgA, t0);
        (t0.InShader).Assert();
        (t0.Value != null).Assert();
        return t0;
      }
      throw new SampleException("Can only sample texture inside pixel shader");
    }
  }
  public partial class SampleException : Exception {
    public SampleException(string msg) : base(msg) { }
  }
  public interface IInputTexture { }

  partial class InputTexture : Expr<object>, IInputTexture {
    private InputTexture() { }
    public static readonly InputTexture Instance = new InputTexture();
    protected  override Eval<EVAL>.Info<object> Eval<EVAL>(Eval<EVAL> txt) {
      if (txt is ShaderEval<EVAL>) {
        var txt0 = ((ShaderEval<EVAL>)txt);
        return new ShaderEval<EVAL>.MyInfo<object>() { InShaderStatus = true, Value = "input" };
      }      
      throw new SampleException("Can only access input texture within pixel shader");
    }
    protected override string ToString1() { return "input"; }
  }
  public interface ICurrentPixel { }
  public interface CurrentPixelEval<EVAL> where EVAL : Eval<EVAL> {
    Eval<EVAL>.Info<Vecs.Vec2<double>> CurrentPixelEval { get; }
  }

  partial class CurrentPixel : Expr<Vecs.Vec2<double>>, ICurrentPixel {
    private CurrentPixel() { }
    public static readonly CurrentPixel Instance = new CurrentPixel();
    protected  override Eval<EVAL>.Info<Vecs.Vec2<double>> Eval<EVAL>(Eval<EVAL> txt) {
      if (txt is ShaderEval<EVAL>) {
        var txt0 = ((ShaderEval<EVAL>)txt);
        return new ShaderEval<EVAL>.MyInfo<Vecs.Vec2<double>>() { InShaderStatus = true, Value = "uv" };
      } else if (txt is CurrentPixelEval<EVAL>) return ((CurrentPixelEval<EVAL>)txt).CurrentPixelEval;
      throw new SampleException("Can only access input texture within pixel shader");
    }
    protected override string ToString1() { return "uv"; }
  }
  public delegate Func<PointBl,ColorBl> CustomTexture(Tex2D Original);

  public static partial class BaseShaderEval {
    public static string TypeNameFor(Type type, bool SupportsOthers) {
      if (type == typeof(uint)) return "uint";

      var arity = Vecs.Arity.Find(type);
      if (arity == null) throw new NotSupportedException();
      string prefix;
      if (arity.Count == 1) {
        var K = arity.TypeOfK;
        if (K == typeof(float) || K == typeof(double)) prefix = "float";
        else if (SupportsOthers) {
          if (K == typeof(int)) prefix = "int";
          else if (K == typeof(uint)) prefix = "uint";
          else if (K == typeof(bool)) prefix = "bool";
          else throw new NotSupportedException();
        } else prefix = "float";
      } else prefix = TypeNameFor(arity.TypeOfK, SupportsOthers);

      if (arity is Matrices.IMatrixArity) {
        var matrix = (Matrices.IMatrixArity)arity;
        if (matrix.Height == 1) return prefix + matrix.Width.ToString();
        else if (matrix.Width == 1) return prefix + matrix.Height.ToString();
        return prefix + matrix.Height + "x" + matrix.Width;
      } else return prefix + (arity.Count == 1 ? "" : arity.Count.ToString());

    }


  }
  public abstract partial class BaseShaderEval<EVAL> : ScopedEval<EVAL>, Ops.IConditionEval2<EVAL>, ITableEval<EVAL> where EVAL : Eval<EVAL> {
    protected override Info<S> AllocateTemp<S>(Scope scope, Info<S> Code) {
      return NewInfo<S>(TempName, Code); // copy attributes from code. 
    }
    public override Func<Eval<EVAL>.Info<S>, Eval<EVAL>.Info<T>> Operator<S, T>(Bling.Ops.Operation<S, T> op) {
      var shader = op.Op.Shader(this, op);
      if (shader == null) return base.Operator<S, T>(op);
      return (s) => Append(() => NewInfo<T>(shader(ValueFor(s))), new Expr[] {
          op.ArgA, 
        }, s);
    }
    public override Func<Eval<EVAL>.Info<S>, Eval<EVAL>.Info<T>, Eval<EVAL>.Info<U>> Operator<S, T, U>(Bling.Ops.Operation<S, T, U> op) {
      var shader = op.Op.Shader(this, op);
      if (shader == null) return base.Operator<S, T, U>(op);
      return (s, t) => Append(() => NewInfo<U>(shader(ValueFor(s), ValueFor(t))), new Expr[] {
          op.ArgA, op.ArgB, 
        }, s, t);
    }
    public override Func<Eval<EVAL>.Info<S>, Eval<EVAL>.Info<T>, Eval<EVAL>.Info<U>, Eval<EVAL>.Info<V>> Operator<S, T, U, V>(Bling.Ops.Operation<S, T, U, V> op) {
      var shader = op.Op.Shader(op);
      if (shader == null) return base.Operator<S, T, U, V>(op);
      return (s, t, u) => Append(() => NewInfo<V>(shader(ValueFor(s), ValueFor(t), ValueFor(u))),
        new Expr[] {
          op.ArgA, op.ArgB, op.ArgC, 
        }, s,t,u);
    }
    public override Func<Eval<EVAL>.Info<S>, Eval<EVAL>.Info<T>, Eval<EVAL>.Info<U>, Eval<EVAL>.Info<V>, Eval<EVAL>.Info<W>> Operator<S, T, U, V, W>(Bling.Ops.Operation<S, T, U, V, W> op) {
      var shader = op.Op.Shader(op);
      if (shader == null) return base.Operator<S, T, U, V, W>(op);
      return (s, t, u, v) => Append(() => NewInfo<W>(shader(ValueFor(s), ValueFor(t), ValueFor(u), ValueFor(v))),
        new Expr[] {
          op.ArgA, op.ArgB, op.ArgC, op.ArgD, 
        },
        s,t,u,v);
    }
    public override Info<K> Access<K, T, ARITY>(Info<T> info, int Idx) {
      var arity = Vecs.Arity<K, T>.ArityI;
      string select;
      var marity = arity as Matrices.IMatrixArity<K, T>;
      if (marity != null && marity.Width > 1 && marity.Height > 1) {
        // find the row first.
        var row = Idx / marity.Width;
        var column = Idx % marity.Width;
        select = "_m" + row + column;
      } else switch (Idx) {
        case 0: select = "x"; break;
        case 1: select = "y"; break;
        case 2: select = "z"; break;
        case 3: select = "w"; break;
        default: throw new NotSupportedException();
      }
      var info0 = ((MyInfo<T>)info).Value;
      // not in shader yet
      if (info0 == null) return Append(() => NewInfo<K>(null), null, info);
      return Append(() => NewInfo<K>(info0 + "." + select, info), null, info);
    }
    public override Info<T> Composite<K, T, ARITY>(Vecs.IComposite<K,T> Expr, params Info<K>[] infos) {
      var exprs = new Expr[infos.Length];
      for (int i = 0; i < infos.Length; i++) exprs[i] = Expr[i];

      return Prepare<T>(() => Append(() => {
        bool AnyValueNull = false;
        bool AllValueNull = true;
        for (int i = 0; i < infos.Length; i++) {
          var info = (MyInfo<K>)infos[i];
          AnyValueNull = AnyValueNull || info.Value == null;
          AllValueNull = AllValueNull && info.Value == null;
        }
        if (AnyValueNull) {
          AllValueNull.Assert();
          return NewInfo<T>(null, infos[0]);
        } 

        var str = TypeNameFor(typeof(T)) + "(";
        for (int i = 0; i < infos.Length; i++) {
          (((MyInfo<K>)infos[i]).Value != null).Assert();
          str = str + (i == 0 ? "" : ", ") + ((MyInfo<K>)infos[i]).Value;
        }
        return NewInfo<T>(str + ")", infos[0]);
      }, exprs, infos), exprs, infos);
    }
    public void Composite0(MyInfo InfoT, params Info[] infos) {
      var str = TypeNameFor(InfoT.TypeOfT) + "(";
      for (int i = 0; i < infos.Length; i++) {
        str = str + (i == 0 ? "" : ", ") + ((MyInfo)infos[i]).Value;
      }
      InfoT.Value = str + ")";
    }
    public override Eval<EVAL>.Info<T> Constant<T>(Constant<T> constant) {
      var arity = Vecs.Arity.Find(typeof(T));
      if (arity != null) {
        Func<int, string> EToString = (idx) => {
          var e = arity.Access(constant.Value, idx);
          if (arity.TypeOfK == typeof(bool)) return ((bool)e) ? "1" : "0";
          else if (arity.TypeOfK == typeof(double)) return ((double)e).ToString(System.Globalization.NumberFormatInfo.InvariantInfo);
          else return e.ToString();
        };
        if (arity.Count == 1) {
          return NewInfo<T>(EToString(0));
        }
        var str = TypeNameFor(arity.TypeOfT) + "(";
        for (int i = 0; i < arity.Count; i++) {
          str = str + (i == 0 ? "" : ", ") + EToString(i);
        }
        return NewInfo<T>(str + ")");
      }
      throw new NotImplementedException();
    }
    private int ConditionCounter = 0;
    public virtual MyInfo<T> Block<T>(string cname, Expr<T> Expr) {
      PushScope();
      var exprI = DoEval1(Expr);
      if (exprI.Value == null) {
        PopScope();
        return exprI;
      }
      return Block(cname, exprI);
    }

    protected virtual MyInfo<T> Block<T>(string cname, MyInfo<T> exprI) {
      (exprI.Value != null).Assert();
      // don't use append here, already buit into exprI.
      var temps = PopScope();
      var ret = NewInfo<T>(cname, exprI); // because we will copy this expression's shader mode.
      ret.Append("{");
      // dump temps.
      foreach (var t in temps) {
        (((MyInfo)t.AsTemp).BlockBefore == null).Assert();
        ret.Append(t.Code);
        ret.Append("  " + this.TypeNameFor(((MyInfo)t.AsTemp).TypeOfT) + " " + ((MyInfo)t.AsTemp).Value + " = " + ((MyInfo)t.Code).Value + ";");
      }
      ret.Append(exprI); // for any of exprI's baggage
      ret.Append("  " + cname + " = " + exprI.Value + ";", "}");
      return ret;
    }
    private class Block0Visitor : MyInfoVisitor<MyInfo> {
      public BaseShaderEval<EVAL> Outer; public string cname;
      public MyInfo Visit<T>(MyInfo<T> Info) {
        return (Outer).Block<T>(cname, Info);
      }
    }
    protected MyInfo Block(string cname, MyInfo exprI) {
      return exprI.Accept(new Block0Visitor() { Outer = this, cname = cname });
    }


    public void Flatten(string Indent0, Info Info, System.IO.StringWriter Output) {
      var Info0 = (MyInfo)Info;
      if (Info0.BlockBefore == null) return;
      // e = string | list of e
      string Indent = Indent0;
      Action<Action> DoIndent = (a) => {
        var oldIndent = Indent;
        Indent += "  ";
        a();
        Indent = oldIndent;
      };
      Action<List<object>> Process = null;
      Process = list => {
        (list.Count > 0).Assert();
        foreach (var e in list) {
          if (e is string) Output.WriteLine(Indent + ((string)e));
          else DoIndent(() => Process((List<object>) e));
        }
      };
      Process(Info0.BlockBefore);
    }

    public virtual Eval<EVAL>.Info<T> Block<T>(T[] Values, Eval<EVAL>.Info<int> idx) { throw new NotSupportedException(); }
    public virtual Eval<EVAL>.Info<T> Table<T>(TableExpr2<T> value) {
      var Idx = DoEval1(value.Index);
      if (Idx.Value == null) return NewInfo<T>(null, Idx);
      throw new NotSupportedException();
    }

    public virtual Eval<EVAL>.Info<T> Table<T>(TableExpr<T> value) {
      var cname = "test" + ConditionCounter;
      ConditionCounter += 1;
      var Idx = DoEval1(value.Index);
      MyInfo<T>[] Cases = new MyInfo<T>[value.Values.Length];
      Expr[] All = new Expr[value.Values.Length + 1];
      MyInfo[] Infos = new MyInfo[value.Values.Length + 1];
      All[0] = value.Index;
      Infos[0] = Idx;
      for (int i = 0; i < Cases.Length; i++) {
        PushScope();
        Cases[i] = Block(cname, DoEval1(value.Values[i]));
        Infos[i + 1] = Cases[i];
        (Cases[i].Value == cname || Cases[i].Value == null).Assert();
        All[i + 1] = value.Values[i];
      }
      var ret = Prepare<T>(() => NewInfo<T>(cname), All, Infos);
      if (ret.Value != null) {
        ret.Append(TypeNameFor(typeof(T)) + " " + cname + ";");
        ret.Append("switch (" + Idx.Value + ") {");
        int i = 0;
        foreach (var Case in Cases) {
          ret.Append("  case " + i + ": ");
          if (Case.Value == cname)
            ret.Append(Case);
          else {
            (Case.BlockBefore == null && Case.Value != null).Assert();
            ret.Append("  " + cname + " = " + Case.Value + ";");
          }
          ret.Append("  break;");
          i++;
        }
        ret.Append("  default: " + cname + " = " + DoEval1<T>(new Constant<T>()).Value + "; break;");
        ret.Append("}");
      }
      return ret;
    }

    public Eval<EVAL>.Info<S> Condition<S>(Expr<bool> Test, Expr<S> IfTrue, Expr<S> IfFalse) {
        var Te = DoEval1(Test);
        var IfT = DoEval1(IfTrue);
        var IfF = DoEval1(IfFalse);
        return Prepare<S>(() => NewInfo<S>("(" + Te.Value + " ? " + IfT.Value + " : " + IfF.Value + ")"),
          new Expr[] { Test, IfTrue, IfFalse, }, Te, IfT, IfF);
    }
    public Eval<EVAL>.Info<S> DCondition<S>(Expr<bool> Test, Expr<S> IfTrue, Expr<S> IfFalse) {
      var TestE = DoEval1(Test);
      var cname = "test" + ConditionCounter;
      ConditionCounter += 1;
      var IfTrueE = Block(cname, IfTrue);
      var IfFalseE = Block(cname, IfFalse);
      // we only care about the block before.
      (IfTrueE.Value == cname || IfTrueE.Value == null).Assert();
      (IfFalseE.Value == cname || IfFalseE.Value == null).Assert();

      var ret = Prepare<S>(() => NewInfo<S>(cname), new Expr[] { Test, IfTrue, IfFalse }, TestE, IfTrueE, IfFalseE);

      // custom append
      if (ret.Value != null) {
        ret.Append(TypeNameFor(typeof(S)) + " " + cname + ";");
        ret.Append("if (" + TestE.Value + ") ");
        if (IfTrueE.Value == cname)
          ret.Append(IfTrueE);
        else {
          (IfTrueE.BlockBefore == null && IfTrueE.Value != null).Assert();
          ret.Append("  " + cname + " = " + IfTrueE.Value + ";");
        }

        ret.Append("else");
        if (IfFalseE.Value == cname)
          ret.Append(IfFalseE);
        else {
          (IfFalseE.BlockBefore == null && IfFalseE.Value != null).Assert();
          ret.Append("  " + cname + " = " + IfFalseE.Value + ";");
        }
      } else (ret.BlockBefore == null).Assert();
      return ret;
    }
    protected virtual bool SupportsOthers { get { return false; } }
    public virtual string TypeNameFor(Type type) {
      return BaseShaderEval.TypeNameFor(type, SupportsOthers);
    }
    public interface MyInfoVisitor<R> {
        R Visit<T>(MyInfo<T> Info);
      }

    public MyInfo DoEval1(Expr Expr) { return (MyInfo)((Eval<EVAL>)this).DoEval(Expr); }
    public interface MyInfo : Info {
      List<object> BlockBefore { get; }
      string Value { get; set; }
      Type TypeOfT { get; }
      R Accept<R>(MyInfoVisitor<R> Visitor);
      MyInfo BaseAppend(params Info[] Infos);
    }
    public abstract class RegisterFile {
      public readonly EVAL Outer;
      public RegisterFile(EVAL Outer) { this.Outer = Outer; }
      public readonly Dictionary<Expr, int> Registers = new Dictionary<Expr, int>();
      private readonly Dictionary<Expr, MyInfo> RegisterAllocs = new Dictionary<Expr, MyInfo>();
      public MyInfo RegisterAlloc(Expr e) { return RegisterAllocs[e]; }
      public MyInfo<T> RegisterAlloc<T>(Expr<T> e) { return (MyInfo<T>) RegisterAllocs[e]; }
      public virtual Expr[] FlushRegisters {
        get {
          var ret = new Expr[Registers.Count];
          foreach (var v in Registers) ret[v.Value] = v.Key;
          return ret;
        }
      }
      private class ExprVisitor : ExprVisitor<int> {
        public RegisterFile Outer;
        public MyInfo Info;
        public int Visit<T>(Expr<T> Expr) {
          var Info = (MyInfo<T>)this.Info;
          Outer.BringIn<T>(Expr, Info);
          return 0;
        }
      }
      protected virtual string RegisterName<T>(Expr<T> Expr, int Idx, MyInfo<T> Info) { return "r" + Idx; }
      protected abstract bool BringIn0<T>(MyInfo<T> Info);
      protected virtual void Copy<T>(MyInfo<T> From, MyInfo<T> To) {
        To.Value = From.Value;
      }

      public virtual void BringIn<T>(Expr<T> Expr, MyInfo<T> Info) {
        if (Info.Value != null) return;
        if (!BringIn0(Info)) return;
        if (Registers.ContainsKey(Expr)) {
          Copy(RegisterAlloc(Expr), Info);
        } else {
          RegisterAllocs[Expr] = BringInNew(Expr, Info);
        }
        return;
      }
      public virtual MyInfo<T> BringInNew<T>(Expr<T> Expr, MyInfo<T> Info) {
        var name = RegisterName(Expr, Registers.Count, Info);
        Info.Value = name;
        Registers[Expr] = Registers.Count;
        return Info;
      }

      public void BringIn(Expr e, MyInfo info) {
        e.Visit((new ExprVisitor() { Outer = this, Info = info }));
      }
    }
    protected virtual MyInfo<T> Prepare<T>(Func<MyInfo<T>> Target, Expr[] Exprs, params Info[] Infos) {
      var AllNotNull = true;
      foreach (var info in Infos) AllNotNull = AllNotNull && ((MyInfo)info).Value != null;
      if (AllNotNull)
        return Target();
      else return NewInfo<T>(null);
    }
    public virtual MyInfo<T> Append<T>(Func<MyInfo<T>> Target, Expr[] Exprs, params Info[] Infos) {
      return (MyInfo<T>) Prepare(Target, Exprs, Infos).BaseAppend(Infos);
    }
    public class MyInfo<T> : Info<T>, MyInfo {
      public List<object> BlockBefore { get; set; }
      public string Value { get; set; }
      public Type TypeOfT { get { return typeof(T); } }

      public MyInfo BaseAppend(params Info[] infos) { return Append(infos); }

      public MyInfo<T> Append(params Info[] infos) {
        foreach (var info in infos) {
          var info0 = (MyInfo)info;
          if (info0.BlockBefore == null) continue;
          if (BlockBefore == null) BlockBefore = new List<object>();
          (info0.BlockBefore.Count > 0).Assert();
          if (info0.BlockBefore.Count == 1) BlockBefore.Add(info0.BlockBefore[0]);
          else BlockBefore.Add(info0.BlockBefore);
        }
        return this;
      }
      public R Accept<R>(MyInfoVisitor<R> Visitor) {
        return Visitor.Visit(this);
      }
      public MyInfo<T> Append(params string[] stuff) {
        if (BlockBefore == null) {
          BlockBefore = new List<object>();
        }
        foreach (var s in stuff) BlockBefore.Add(s);
        return this;
      }
    }
    public MyInfo<T> DoEval1<T>(Expr<T> Expr) { return (MyInfo<T>)((Eval<EVAL>)this).DoEval(Expr); }
    public abstract MyInfo<T> NewInfo<T>(string Value);
    protected string ValueFor<T>(Info<T> Info) { 
      var ret = ((MyInfo<T>)Info).Value;
      (ret != null).Assert();

      return ret;
    }
    public abstract MyInfo<T> NewInfo<T>(string Value, Info Copy);
  }
  public abstract partial class ShaderEval<EVAL> : BaseShaderEval<EVAL>, Ops.IConditionEval<EVAL> /* , TableEval<EVAL> */ where EVAL : Eval<EVAL> {
    public override BaseShaderEval<EVAL>.MyInfo<T> NewInfo<T>(string Value) { return new MyInfo<T>() { Value = Value, InShaderStatus = false }; }
    public override BaseShaderEval<EVAL>.MyInfo<T> NewInfo<T>(string Value, Info Copy) { return new MyInfo<T>() { Value = Value, InShaderStatus = ((MyInfo)Copy).InShaderStatus }; }

    protected override Info<S> AllocateTemp<S>(Scope scope, Info<S> Code) {
      var Code0 = (MyInfo<S>)Code;
      if (!Code0.InShader) return null;
      (Code0.InShader).Assert(); // otherwise, we shouldn't be allocating a temp!
      return base.AllocateTemp<S>(scope, Code);
    }
    protected override BaseShaderEval<EVAL>.MyInfo<T> Prepare<T>(Func<BaseShaderEval<EVAL>.MyInfo<T>> Target, Expr[] Exprs, params Eval<EVAL>.Info[] Infos) {
      ShaderStatus InShaderStatus = new OutShader();
      var IsConstant = true;
      foreach (var info in Infos) {
        InShaderStatus = InShaderStatus | ((MyInfo)info).InShaderStatus;
        IsConstant = IsConstant && ((MyInfo)info).Value != null;
      }
      IsConstant = IsConstant && !InShaderStatus.In;

      if (!InShaderStatus.In && !IsConstant) { // get rid of any constant values. 
        foreach (var info in Infos) ((MyInfo)info).Value = null;

        var ret0 = base.Prepare(Target, Exprs, Infos);
        ((MyInfo)ret0).Value = null;
        ((MyInfo)ret0).InShaderStatus = InShaderStatus;
        return ret0;
      }
      for (int i = 0; i < Infos.Length; i++) {
        if (InShaderStatus.In && !((MyInfo) Infos[i]).InShader && ((MyInfo) Infos[i]).Value == null) 
          Registers.BringIn(Exprs[i], (MyInfo)Infos[i]);
      }
      var ret = base.Prepare<T>(Target, Exprs, Infos);
      ((MyInfo) ret).InShaderStatus = InShaderStatus;
      if (InShaderStatus.In) (ret.Value != null).Assert();
      return ret;
    }
    protected virtual ShaderRegisterFile MakeRegisterFile() { return new ShaderRegisterFile(this); }
    public ShaderEval() {
      Registers = MakeRegisterFile();
    }
    public class ShaderRegisterFile : RegisterFile {
      public ShaderRegisterFile(EVAL Outer) : base(Outer) { }
      protected override bool BringIn0<T>(BaseShaderEval<EVAL>.MyInfo<T> Info) {
        var Info0 = (MyInfo<T>)Info;
        (!Info0.InShader).Assert();
        (Info0.BlockBefore == null).Assert();
        Info0.InShaderStatus = true;
        return true;
      }
      protected override void Copy<T>(BaseShaderEval<EVAL>.MyInfo<T> From, BaseShaderEval<EVAL>.MyInfo<T> To) {
        base.Copy<T>(From, To);
        ((MyInfo<T>)To).InShaderStatus = ((MyInfo<T>)From).InShaderStatus;
      }
    }
    public readonly ShaderRegisterFile Registers;
    public abstract class ShaderStatus {
      public abstract bool In { get; }
      public static implicit operator ShaderStatus(bool b) {
        return (b) ? (ShaderStatus) new InShader() : new OutShader();
      }
      public static ShaderStatus operator |(ShaderStatus opA, ShaderStatus opB) {
        if (opA is InShader || opB is InShader) return new InShader();
        return ((OutShader)opA).Merge((OutShader)opB);
      }
    }
    public sealed class InShader : ShaderStatus {
      public override bool In {
        get { return true; }
      }
      public override string ToString() {
        return "In";
      }
    }
    public class OutShader : ShaderStatus {
      public int Depth = 0;
      public override bool In {
        get { return false; }
      }
      public override string ToString() {
        return "Out-" + Depth;
      }
      public OutShader Merge(OutShader o1) {
        return new OutShader() { Depth = Math.Max(Depth, o1.Depth) };
      }
    }

    public new interface MyInfo : BaseShaderEval<EVAL>.MyInfo {
      ShaderStatus InShaderStatus { get; set; }
      bool InShader { get; } // set;  }
    }
    public new MyInfo<T> DoEval<T>(Expr<T> Expr) { return (MyInfo<T>)((Eval<EVAL>)this).DoEval(Expr); }
    public new class MyInfo<T> : BaseShaderEval<EVAL>.MyInfo<T>, MyInfo {
      public ShaderStatus InShaderStatus { get; set; }
      public bool InShader {
        get { return InShaderStatus.In; }
        // set { InShaderStatus = (value ? (ShaderStatus) new InShader() : new OutShader()); }
      }
      public MyInfo() { // false.
        InShaderStatus = new OutShader();
      }
    }
  }
}

namespace Bling {
  using Bling.Vecs;
  using Bling.Core;
  // points with configurable roles?

  namespace Semantics {
    public abstract class PointRole<ROLE> : Role<ROLE> where ROLE : PointRole<ROLE>, new() { }
    public class Normal : PointRole<Normal> { }
    public class Position : PointRole<Position> { }
    public class TexCoord : PointRole<TexCoord> { }

    public class Color : Role<Color> { }
  }
}

/*
namespace Bling.Shaders {
  using Bling.NewOps;
  using Bling.NewVecs;
  public static class ShaderOperators {
    public readonly static Dictionary<IOperator, object> Ops = new Dictionary<IOperator, object>();
    private static Func<string, string, string> Binary(Func<string, string, string> F) {
      return F;
    }
    private static Func<string,string> Unary(Func<string, string> F) {
      return F;
    }
    private static Func<string, string, string, string> Trinary(Func<string, string, string, string> F) {
      return F;
    }
    public static object FindOperator(IOperator op) {
      if (Ops.ContainsKey(op)) return Ops[op];
      var Ts = op.ArgTypes;
      object ret;
      if (op is IPrimitiveDefined) {
        var op0 = (IPrimitiveDefined)op;
        string id;
        if (op0.Alt != null) id = op0.Alt;
        else id = op0.Id;
        if (Ts.Length == 1) ret = Unary((e0) => id + "(" + e0 + ")");
        else if (Ts.Length == 2) ret = Binary((e0, e1) => "(" + e0 + ") " + id + " (" + e1 + ")");
        else throw new NotSupportedException();
      } else if (op is IShaderDefined) {
        var op0 = (IShaderDefined)op;
        string id;
        if (op0.Alt != null) id = op0.Alt;
        else id = op0.Id;
        id = id.ToLower();
        if (Ts.Length == 1) ret = Unary((e0) => op0.Id + "(" + e0 + ")");
        else if (Ts.Length == 2) ret = Binary((e0, e1) => id + "(" + e0 + ", " + e1 + ")");
        else if (Ts.Length == 3) ret = Trinary((e0, e1, e2) => id + "(" + e0 + ", " + e1 + ", " + e2 + ")");
        else throw new NotSupportedException();
      } else ret = null;
      Ops[op] = ret;
      return ret;
    }
  }
  public abstract partial class BaseShaderEval<EVAL> : ScopedEval<EVAL>, Ops.IConditionEval2<EVAL>, ITableEval<EVAL> where EVAL : Eval<EVAL> {
    public override Func<Eval<EVAL>, Operation<RET, ARG0>, Eval<EVAL>.Info<RET>> Lookup<RET, ARG0>(IOperator<RET, ARG0> Op) {
      var f = (Func<string, string>)ShaderOperators.FindOperator(Op);
      if (f != null) return (txt, op) => {
        var arg0 = ((ShaderEval<EVAL>)txt).DoEval(op.Arg0);
        var ret = ((ShaderEval<EVAL>)txt).NewInfo<RET>(f(arg0.Value));
        ret.Append(arg0);
        return ret;
      };
      return base.Lookup<RET, ARG0>(Op);
    }
    public override Func<Eval<EVAL>, Operation<RET, ARG0, ARG1>, Eval<EVAL>.Info<RET>> Lookup<RET, ARG0, ARG1>(IOperator<RET, ARG0, ARG1> Op) {
      var f = (Func<string, string, string>)ShaderOperators.FindOperator(Op);
      if (f != null) return (txt, op) => {
        var arg0 = ((ShaderEval<EVAL>)txt).DoEval(op.Arg0);
        var arg1 = ((ShaderEval<EVAL>)txt).DoEval(op.Arg1);
        var ret = ((ShaderEval<EVAL>)txt).NewInfo<RET>(f(arg0.Value, arg1.Value));
        ret.Append(arg0, arg1);
        return ret;
      };
      return base.Lookup<RET, ARG0, ARG1>(Op);
    }
    public override Func<Eval<EVAL>, Operation<RET, ARG0, ARG1, ARG2>, Eval<EVAL>.Info<RET>> Lookup<RET, ARG0, ARG1, ARG2>(IOperator<RET, ARG0, ARG1, ARG2> Op) {
      var f = (Func<string, string, string, string>)ShaderOperators.FindOperator(Op);
      if (f != null) return (txt, op) => {
        var arg0 = ((ShaderEval<EVAL>)txt).DoEval(op.Arg0);
        var arg1 = ((ShaderEval<EVAL>)txt).DoEval(op.Arg1);
        var arg2 = ((ShaderEval<EVAL>)txt).DoEval(op.Arg2);
        var ret = ((ShaderEval<EVAL>)txt).NewInfo<RET>(f(arg0.Value, arg1.Value, arg2.Value));
        ret.Append(arg0, arg1, arg2);
        return ret;
      };
      return base.Lookup<RET, ARG0, ARG1, ARG2>(Op);
    }

  }


}
*/