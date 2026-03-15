using Bling.Util;
using Bling.DSL;
using Bling.Core;
using Bling.Linq;
using Bling.Shaders;
using Bling.WPF;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using DynamicShader.D3DX;
using linq = Microsoft.Linq.Expressions;
using Bling.Graphics;

namespace Bling.WPF {
  public partial class EffectBl : EffectBl<Effect, EffectBl> {
    /// <summary>
    /// Bind effect to a custom PixelEffect (or compatible function).
    /// </summary>
    /// <see cref="Bling.Graphics.PixelEffect"/>
    public Func<Tex2D,PointBl,ColorBl> Custom {
      set {
        var color = value(Tex2D.Input,Tex2D.UV);
        var shader = CustomShaderEffect.Make(color);
        this.Bind = shader;
      }
    }

    private class MyEval : DefaultLinqEval, CurrentPixelEval<DefaultLinqEval> {
      public readonly ParameterExpr<Vecs.Vec2<double>> InputUV;
      public MyEval() {
        InputUV = new ParameterExpr<Bling.Vecs.Vec2<double>>("UV");
        this.Parameters.Add(InputUV);
      }
      public Info<Vecs.Vec2<double>> CurrentPixelEval {
        get {
          return this.DoEval(InputUV);
        }
      }
    }
    public Func<Tex2D,PointBl, ColorBl> CustomWithTranslate {
      set {
        var color = value(Tex2D.Input,Tex2D.UV);
        var shader = CustomShaderEffect.Make(color);
        this.Bind = shader;
        var pt = shader.ExtractPoint(color);
        if (pt.Underlying is ICurrentPixel) return;
        var eval = new MyEval();
        var pt0 = eval.DoEval(pt.Underlying).Value;
        var body = eval.PopScope(pt0); // all the temps are out.
        var F0 = linq.Expression.Lambda<Microsoft.Func<Vecs.Vec2<double>, Vecs.Vec2<double>>>(body, eval.Parameters.Flush());
        var F1 = F0.Compile();
        shader.CustomEffectMapping = (p) => {
          var q = F1(new Vecs.Vec2<double>() { X = p.X, Y = p.Y });
          return new Point(q.X, q.Y);
        };
      }
    }

  }
}

namespace Bling.Shaders {
  static class WPFImplShaderExtensions {
    public static bool IsBrush(this Type t) {
      return t == typeof(Brush) || t.IsSubclassOf(typeof(Brush));
    }
  }
  /// <summary>
  /// Provides a custom bitmap effect by using a System.Windows.Media.Effects.PixelShader.
  /// </summary>
  public abstract class CustomShaderEffect : ShaderEffect {
    public abstract DependencyProperty GetProperty(int n);
    public abstract CustomShaderEffect MakeCopy();

    internal sealed class NoArg { }

    public static Type MakeType(Type[] Ts) {
      var Xs = new Type[20];
      (Ts.Length < Xs.Length).Assert();
      for (int i = 0; i < Ts.Length; i++) Xs[i] = Ts[i];
      for (int i = Ts.Length; i < Xs.Length; i++) Xs[i] = typeof(NoArg);
      var tpe = typeof(ActualShaderEffect<,,,,,,,,,,,,,,,,,,,>).MakeGenericType(Xs);
      return tpe;
    }
    public static CustomShaderEffect MakeShader(Type[] Ts, PixelShader PixelShader) {
      var tpe = MakeType(Ts);
      var c = tpe.GetConstructor(new Type[] { typeof(PixelShader) });
      var x = c.Invoke(new object[] { PixelShader });
      return (CustomShaderEffect)x;
    }
    public static linq.Expression MakeLinqShader(Type[] Ts, PixelShader PixelShader) {
      var tpe = MakeType(Ts);
      var c = tpe.GetConstructor(new Type[] { typeof(PixelShader) });
      return linq.Expression.New(c, linq.Expression.Constant(PixelShader, typeof(PixelShader)));
    }
    public abstract void Bind(Expr[] Exprs);
    public abstract Action<CustomShaderEffect, object[]> DelayBind(Expr[] RHS, params IParameterExpr[] Ps);
    private WPFShaderEval Eval;

    private class FindUV : TranslateEval<FindUV>, ExprVisitor<PointBl> {
      private readonly HashSet<PointBl> Found = new HashSet<PointBl>();
      protected override Expr<S> RealTranslate<S>(Expr<S> value) {
        if (value is Ops.Operation) {
          var op = (Ops.Operation) value;
          if (op.BaseBaseOp is ITex2D && op[0] is IInputTexture) {
            var b0 = (Expr<Vecs.Vec2<double>>)op[1];
            Found.Add(b0);
          }
        }
        return base.RealTranslate<S>(value);
      }
      public PointBl Visit<T>(Expr<T> value) {
        Translate(value);
        PointBl ret = null;
        foreach (var v in Found) ret = v;
        if (Found.Count == 1) return ret;
        else throw new Exception("not supported for this shader!");
      }
    }
    private class CustomTransformEval : TranslateEval<CustomTransformEval> {
      public CustomShaderEffect Effect;
      protected override Expr<T> RealTranslate<T>(Expr<T> value) {
        if (Effect.Eval.Registers.Registers.ContainsKey(value)) {
          var Property = Effect.GetProperty(Effect.Eval.Registers.Registers[value]);
          return new DependencyPropertyExpr<CustomShaderEffect,T>() { Property = Property, TargetValue = new Constant<CustomShaderEffect>(Effect) };
        } else return base.RealTranslate<T>(value);
      }

    }
    public PointBl ExtractPoint(ColorBl Color) {
      var found = Color.Underlying.Visit(new FindUV());
      var trans = new CustomTransformEval() { Effect = this };
      return trans.Translate(found.Underlying);
    }


    public static CustomShaderEffect Make(ColorBl Color) {
      var Eval = new WPFShaderEval();
      var PixelShader = Eval.MakeShader(Color);

      var Ts = new Type[Eval.FlushedRegisters.Length];
      for (int i = 0; i < Ts.Length; i++) Ts[i] = Eval.FlushedRegisters[i].TypeOfT;
      var Shader = MakeShader(Ts, PixelShader);
      Shader.Eval = Eval;
      Shader.Bind(Eval.FlushedRegisters);
      return Shader;
    }
    public static Func<object[], CustomShaderEffect> Make(ColorBl Color, params IParameterExpr[] Ps) {
      var Eval = new WPFShaderEval();
      var PixelShader = Eval.MakeShader(Color);
      var Ts = new Type[Eval.FlushedRegisters.Length];
      for (int i = 0; i < Ts.Length; i++) Ts[i] = Eval.FlushedRegisters[i].TypeOfT;
      var Shader = MakeShader(Ts, PixelShader);
      Shader.Eval = Eval;
      var F = Shader.DelayBind(Eval.FlushedRegisters, Ps);
      return (Qs) => {
        var Shader0 = Shader.MakeCopy();
        F(Shader0, Qs);
        return Shader0;
      };
    }
    public static Func<S, CustomShaderEffect> Make<S>(ColorBl Color, ParameterExpr<S> ArgS) {
      var F = Make(Color, new IParameterExpr[] { ArgS });
      return s => F(new object[] { s });
    }
    public static Func<S, T, CustomShaderEffect> Make<S, T>(ColorBl Color, ParameterExpr<S> ArgS, ParameterExpr<T> ArgT) {
      var F = Make(Color, new IParameterExpr[] { ArgS, ArgT });
      return (s,t) => F(new object[] { s, t });
    }
    public static Func<S, T, U, CustomShaderEffect> Make<S, T, U>(ColorBl Color, ParameterExpr<S> ArgS, ParameterExpr<T> ArgT, ParameterExpr<U> ArgU) {
      var F = Make(Color, new IParameterExpr[] { ArgS, ArgT, ArgU });
      return (s, t, u) => F(new object[] { s, t, u });
    }
    public static Func<S, T, U, V, CustomShaderEffect> Make<S, T, U, V>(ColorBl Color, ParameterExpr<S> ArgS, ParameterExpr<T> ArgT, ParameterExpr<U> ArgU, ParameterExpr<V> ArgV) {
      var F = Make(Color, new IParameterExpr[] { ArgS, ArgT, ArgU, ArgV });
      return (s, t, u, v) => F(new object[] { s, t, u, v });
    }
    private class MyGeneralTransform : GeneralTransform {
      internal Func<Point, Point> F;
      protected override Freezable CreateInstanceCore() {
        return this;
      }
      public override GeneralTransform Inverse {
        get {
          return this;
        }
      }
      public override bool TryTransform(Point inPoint, out Point result) {
        result = F(inPoint);
        return true;
      }
      public override Rect TransformBounds(Rect rect) {
        return new Rect(F(rect.TopLeft), F(rect.BottomRight));
      }
    }
    protected override GeneralTransform EffectMapping {
      get {
        if (CustomEffectMapping != null) {
          return new MyGeneralTransform() { F = CustomEffectMapping };
        }
        return base.EffectMapping;
      }
    }
    internal Func<Point, Point> CustomEffectMapping { get; set; }
  }
  abstract class CustomShaderEffect<SELF> : CustomShaderEffect where SELF : CustomShaderEffect<SELF> {
    internal static readonly List<DependencyProperty> Registers = new List<DependencyProperty>();
    internal static readonly List<DependencyProperty> Inputs = new List<DependencyProperty>();
    public static readonly List<DependencyProperty> AllProperties = new List<DependencyProperty>();
    public override DependencyProperty GetProperty(int n) { return AllProperties[n+1]; }
    public static readonly DependencyProperty InputProperty = AddInput("Input");
    private class ImmediateBindVisitor : ExprVisitor<int> {
      public int Idx;
      public SELF Outer;
      public int Visit<T>(Expr<T> value) {

        var p = Outer.GetProperty(Idx);
        var dp = new DependencyPropertyExpr<SELF, T>() { ReadProperty = p, WriteProperty = p, TargetValue = new Constant<SELF>(Outer) };
        dp.Bind = value;
        return 0;
      }
    }
    public void Bind(Expr e, int idx) {
      e.Visit(new ImmediateBindVisitor() { Idx = idx, Outer = (SELF)this });
    }
    public override void Bind(Expr[] Exprs) {
      for (int i = 0; i < Exprs.Length; i++) Bind(Exprs[i], i);
    }
    private class DelayedBindVisitor : ExprVisitor<Action<SELF, object[]>> {
      public int Idx;
      public SELF Outer;
      public IParameterExpr[] Ps;
      public Action<SELF, object[]> Visit<T>(Expr<T> RHS) {
        return LinqBindEval.DelayBind<SELF, T>(RHS, Outer.GetProperty(Idx), Ps);
      }
    }
    public Action<SELF,object[]> DelayBind(Expr RHS, int Idx, params IParameterExpr[] Ps) {
      return RHS.Visit(new DelayedBindVisitor() { Idx = Idx, Outer = (SELF) this, Ps = Ps });
    }
    public override Action<CustomShaderEffect, object[]> DelayBind(Expr[] RHS, params IParameterExpr[] Ps) {
      Action<SELF, object[]>[] All = new Action<SELF, object[]>[RHS.Length];
      for (int i = 0; i < All.Length; i++) All[i] = DelayBind(RHS[i], i, Ps);
      return (self, param) => {
        var self0 = (SELF)self;
        for (int i = 0; i < All.Length; i++) All[i](self0, param);
      };
    }
    protected static Type SelfType {
      get { return typeof(SELF); }
    }
    protected static DependencyProperty AddParameter<T>(string name) {
      return AddParameter(name, typeof(T));
    }
    protected static DependencyProperty AddParameter(string name, Type type) {
      if (type == typeof(NoArg)) return null;
      if (type.IsBrush()) return AddInput(name);
      var self = SelfType;
      var dp = DependencyProperty.Register(name, type, SelfType,
        new UIPropertyMetadata(PixelShaderConstantCallback(Registers.Count)));
      Registers.Add(dp);
      AllProperties.Add(dp);
      return dp;
    }
    private static DependencyProperty AddInput(string name) {
      var dp = ShaderEffect.RegisterPixelShaderSamplerProperty(name, SelfType, Inputs.Count);
      Inputs.Add(dp);
      AllProperties.Add(dp);
      return dp;
    }
  }
  class ActualShaderEffect<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19> : CustomShaderEffect<ActualShaderEffect<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19>> {
    public ActualShaderEffect(PixelShader PixelShader) {
      this.PixelShader = PixelShader;
      foreach (var dp in AllProperties)
        this.UpdateShaderValue(dp);
    }
    public override CustomShaderEffect MakeCopy() {
      var ret = new ActualShaderEffect<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9,
                             T10, T11, T12, T13, T14, T15, T16, T17, T18, T19>(PixelShader);
      return ret;
    }
    public DependencyPropertyExpr<T0> Arg0 { get { return new DependencyPropertyExpr<ActualShaderEffect<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19>, T0>() { Property = AllProperties[1], TargetValue = this }; } }
    public DependencyPropertyExpr<T1> Arg1 { get { return new DependencyPropertyExpr<ActualShaderEffect<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19>, T1>() { Property = AllProperties[2], TargetValue = this }; } }

    static ActualShaderEffect() {
      AddParameter<T0>("Arg0");
      AddParameter<T1>("Arg1");
      AddParameter<T2>("Arg2");
      AddParameter<T3>("Arg3");
      AddParameter<T4>("Arg4");
      AddParameter<T5>("Arg5");
      AddParameter<T6>("Arg6");
      AddParameter<T7>("Arg7");
      AddParameter<T8>("Arg8");
      AddParameter<T9>("Arg9");
      AddParameter<T10>("Arg10");
      AddParameter<T11>("Arg11");
      AddParameter<T12>("Arg12");
      AddParameter<T13>("Arg13");
      AddParameter<T14>("Arg14");
      AddParameter<T15>("Arg15");
      AddParameter<T16>("Arg16");
      AddParameter<T17>("Arg17");
      AddParameter<T18>("Arg18");
      AddParameter<T19>("Arg19");
    }
  }
  class WPFShaderEval : ShaderEval<WPFShaderEval>, IParameterEval<WPFShaderEval>, DependencyPropertyEval<WPFShaderEval> {
    internal Expr[] FlushedRegisters;

    public new class ShaderRegisterFile : ShaderEval<WPFShaderEval>.ShaderRegisterFile {
      public int InputCount = 1; // leave first for input!
      public int RegCount = 0;
      public readonly Dictionary<Expr, int> RegIndices = new Dictionary<Expr, int>();
      public ShaderRegisterFile(WPFShaderEval Outer) : base(Outer) { }
      protected override string RegisterName<T>(Expr<T> Expr, int Idx, BaseShaderEval<WPFShaderEval>.MyInfo<T> Info) {
        if (typeof(T).IsBrush()) {
          RegIndices[Expr] = InputCount;
          var ret = "i" + InputCount;
          InputCount += 1;
          return ret;
        } else {
          RegIndices[Expr] = RegCount;
          var ret = "r" + RegCount;
          RegCount += 1;
          return ret;
        }
      }
      public override void BringIn<T>(Expr<T> Expr, BaseShaderEval<WPFShaderEval>.MyInfo<T> InfoXX) {
        MyInfo<T> Info = (MyInfo<T>)InfoXX;
        Expr value = Expr;
        MyInfo value1;
        if (value is Expr<Vecs.Vec4<double>>) {
          var value0 = (Expr<Vecs.Vec4<double>>)(object)value;
          value1 = new MyInfo<Point4D>() { Value = Info.Value, InShaderStatus = Info.InShaderStatus };
          BringIn(Ops.VecConvert<Point4D, double, Vecs.Vec4<double>>.FromVec.Instance.Make(value0), (MyInfo<Point4D>)value1);
        } else if (value is Expr<Vecs.Vec3<double>>) {
          var value0 = (Expr<Vecs.Vec3<double>>)(object)value;
          value1 = new MyInfo<Point3D>() { Value = Info.Value, InShaderStatus = Info.InShaderStatus };
          BringIn(Ops.VecConvert<Point3D, double, Vecs.Vec3<double>>.FromVec.Instance.Make(value0), (MyInfo<Point3D>)value1);
        } else if (value is Expr<Vecs.Vec2<double>>) {
          var value0 = (Expr<Vecs.Vec2<double>>)(object)value;
          value1 = new MyInfo<Point>() { Value = Info.Value, InShaderStatus = Info.InShaderStatus };
          BringIn(Ops.VecConvert<Point, double, Vecs.Vec2<double>>.FromVec.Instance.Make(value0), (MyInfo<Point>)value1);
        } else {
          value1 = Info;
          base.BringIn<T>(Expr, (MyInfo<T>)value1);
        }
        Info.Value = value1.Value;
        Info.InShaderStatus = value1.InShaderStatus;
      }
    }
    protected override ShaderEval<WPFShaderEval>.ShaderRegisterFile MakeRegisterFile() {
      return new ShaderRegisterFile(this);
    }
    public new ShaderRegisterFile Registers { get { return (ShaderRegisterFile)base.Registers; } }

    public Info<T> PropertyValue<T>(DependencyPropertyExpr<T> value) {
      var ret = new MyInfo<T>() { Value = null, InShaderStatus = false };
      return ret;
    }
    public override Info<T> Parameter<T>(ParameterExpr<T> value) {
      var p = base.Parameter(value);
      if (p != null) return p;
      return new MyInfo<T>() { 
        Value = null, InShaderStatus = false,
      };
    }
    static WPFShaderEval() {
      Point3DArity.CheckInit();
    }

    public PixelShader MakeShader(ColorBl Color) {
      string ShaderCode;
      {
        var Color0 = Color.Underlying;
        //Color0 = Color0.Fold;
        var info = (MyInfo<Vecs.Vec4<double>>) Block("retV", Color0);
        (info.Value == "retV").Assert();
        var popped = this.PopScope();
        (popped.Count == 0 && this.IsEmpty).Assert();
        if (!info.InShader && info.Value == null) 
          Registers.BringIn<Vecs.Vec4<double>>(Color, info);

        var output = new System.IO.StringWriter();
        FlushedRegisters = Registers.FlushRegisters;
        output.WriteLine("sampler2D input : register(s0);");
        for (int i = 0; i < FlushedRegisters.Length; i++) {
          var r = FlushedRegisters[i];
          if (!r.TypeOfT.IsBrush()) continue;
          output.WriteLine("sampler2D " + Registers.RegisterAlloc(r).Value + " : register(s" + (Registers.RegIndices[r]) + ");");
        }
        for (int i = 0; i < FlushedRegisters.Length; i++) {
          var r = FlushedRegisters[i];
          if (r.TypeOfT.IsBrush()) continue;
          output.WriteLine(TypeNameFor(r.TypeOfT) + " " + Registers.RegisterAlloc(r).Value + " : register(c" + Registers.RegIndices[r] + ");");
        }
        output.WriteLine("float4 main(float2 uv : TEXCOORD) : COLOR {");
        output.WriteLine("  float4 retV;");
        Flatten("  ", info, output);
        output.WriteLine("  return retV;");
        output.WriteLine("}");
        ShaderCode = output.ToString();
      }
      PixelShader PixelShader;
      Console.Out.WriteLine(ShaderCode);
      {
        var effect = DynamicShader.D3DX.CompiledEffect.FromString(ShaderCode, CompilerOptions.None);
        if (effect.CompileSucceeded) {
          var shader0 = effect.CompileShader("main", "ps_2_0", CompilerOptions.None);
          if (shader0.CompileSucceeded) {
            PixelShader = new PixelShader();
            PixelShader.SetStreamSource(new System.IO.MemoryStream(shader0.Data));
          } else throw new ArgumentException("COMPILE FAILED " + shader0.ErrorMessage);
        } else {
          throw new ArgumentException("PARSE FAILED " + effect.ErrorMessage);
        }
      }
      return PixelShader;
    }
  }
  public class WPFTextureProvider : LibTextureProvider {
    protected override Tex2D LoadTexture(string Url, Type FromType) {
      return new ImageBrush(new BitmapImage(Url.MakePackUri(FromType))).Bl();
    }
  }


}