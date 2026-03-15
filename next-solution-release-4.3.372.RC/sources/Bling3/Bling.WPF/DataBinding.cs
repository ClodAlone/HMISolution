using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Bling.Util;
using Bling.DSL;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using linq = Microsoft.Linq.Expressions;
namespace Bling.Linq {
}

namespace Bling.WPF.Util {
  using System.ComponentModel;
  using System.Collections.Specialized;
  public delegate void TypedDependencyPropertyChangedEventHandler<OWNER, TYPE>(OWNER owner, TYPE oldValue, TYPE newValue) where OWNER : DependencyObject;
  public interface ListChangeHandler<TYPE> {
    PropertyChangedEventHandler PropertyChangedEventHandler { get; }
    NotifyCollectionChangedEventHandler NotifyCollectionChangedEventHandler { get; }
    void addAll(IList<TYPE> list);
    void removeAll(IList<TYPE> list);
  }
}
namespace Bling.WPF {
  using Bling.Linq;
  using Bling.Core;
  using Bling.WPF.Util;

  public interface IDependencyPropertyExpr : IExpr, IHasContainer {
    DependencyObject Target { get; }
    DependencyProperty ReadProperty { get; }
    DependencyProperty WriteProperty { get; }
  }
  public interface IBaseDependencyPropertyExpr<T> : IDependencyPropertyExpr, ICanAssignNowExpr<T>, IBindableExpr<T> { }

  public abstract partial class DependencyPropertyExpr<T> : Ops.Operation<T>, IBaseDependencyPropertyExpr<T> {
    public abstract Expr BaseTargetValue { get; }
    public abstract DependencyObject Target { get; }
    public abstract Expr BaseContainer { get; }

    public override Expr<T> Expanded {
      get { return null; }
    }
    public override Expr this[int Idx] {
      get { throw new IndexOutOfRangeException(); }
    }
    public override Bling.Ops.IOperator<T> BaseOp {
      get { throw new NotSupportedException(); }
    }

    /// <summary>
    /// Dependency property used for reading.
    /// </summary>
    public DependencyProperty ReadProperty { get; internal set; }

    public DependencyProperty Property { set { WriteProperty = value; ReadProperty = value; } }

    protected override Expr<T> Derivative1(Dictionary<Expr, Expr> Map) {
      return new Constant<T>(default(T)); // treat as constant
    }

    /// <summary>
    /// Dependency property used for writing.
    /// </summary>
    public DependencyProperty WriteProperty { get; internal set; }

    public abstract TDelegate SetNow<TDelegate>(Expr<T> value, params IParameterExpr[] Args);
    public abstract bool SetNow(T value);

    public abstract bool BindWith(Expr<T> value);
    public abstract bool TwoWayBindWith(Expr<T> value);
    public bool BindWith(Expr value) { return BindWith((Expr<T>)value); }

    protected override int GetHashCode0() {
      return BaseTargetValue.GetHashCode() + ReadProperty.GetHashCode() + WriteProperty.GetHashCode();
    }
    protected override bool Equals0(Expr<T> obj) {
      if (!(obj is DependencyPropertyExpr<T>)) return false;
      var other = (DependencyPropertyExpr<T>)obj;
      return other.BaseTargetValue.Equals(BaseTargetValue) && other.ReadProperty == ReadProperty && other.WriteProperty == WriteProperty;
    }
    protected override string ToString1() {

      return (BaseContainer.GetHashCode() % 100) + "." + ReadProperty.Name; // +"{" + Target.GetValue(ReadProperty) + "}";
    }
    // XXX for debug.
    protected object DepCurrentValue { get { return Target.GetValue(ReadProperty); } }
  }
  /// <summary>
  /// Non-braded Bl value that is a wrapper around a WPF dependency object/property pair.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class DependencyPropertyExpr<S, T> : DependencyPropertyExpr<T>, IHasContainer<S, T>, ICanAssignExpr<T, linq.Expression>, IUseInUI<T> {
    public override Expr BaseContainer { get { return Container; } }
    public Expr<S> Container { get { return TargetValue; } }

    public override Expr<T> Expanded {
      get { return null; }
    }
    public override Expr this[int Idx] {
      get { throw new IndexOutOfRangeException(); }
    }
    public override Bling.Ops.IOperator<T> BaseOp {
      get { throw new NotImplementedException(); }
    }

    public Expr<S> TargetValue { get; internal set; }
    public override Expr BaseTargetValue { get { return TargetValue; } }
    /// <summary>
    /// Target dependency object.
    /// </summary>
    public override DependencyObject Target { get { return (DependencyObject)(object)TargetValue.CurrentValue; } }
    internal DependencyPropertyExpr() { }
    protected override Eval<EVAL>.Info<T> Eval<EVAL>(Eval<EVAL> txt) {
      if (txt is DependencyPropertyEval<EVAL>) return ((DependencyPropertyEval<EVAL>)txt).PropertyValue(this);
      else if (txt is TranslateEval<EVAL>) {
        return ((TranslateEval<EVAL>)txt).NoTranslation(this);
      } else if (txt is LinqEval<EVAL>) {
        var txt0 = ((LinqEval<EVAL>)txt);
        var targetI = txt0.DoEval(TargetValue);
        var ret = new LinqEval<EVAL>.MyInfo<T>() { Value = DatabindingExtensions.DoGetValue(typeof(T), targetI.Value, ReadProperty) };
        txt0.AtLeast(ret, targetI);
        return ret;
      } else if (txt is IDynamicEval<EVAL>) {
        return ((IDynamicEval<EVAL>)txt).Dynamic<T>(DynamicMode.Dynamic);
      }
      throw new NotSupportedException();
    }
    public override bool BindWith(Expr<T> RHS) {
      return (new LinqBindEval()).Accept(this, RHS);
    }
    public override bool TwoWayBindWith(Expr<T> RHS) {
      return (new LinqBindEval() { DoTwoWay = true }).Accept(this, RHS);
    }
    public linq.Expression Assign<EVAL>(linq.Expression RHS0, IExpressionEval<EVAL, linq.Expression> txt) where EVAL : Eval<EVAL> {
      return DatabindingExtensions.DoSetValue((txt).Extract(txt.DoEval(TargetValue)), WriteProperty, RHS0);
    }
    public override TDelegate SetNow<TDelegate>(Expr<T> RHS, params IParameterExpr[] Args) {
      if (RHS is Constant<T> && typeof(TDelegate) == typeof(Action)) {
        Action a = () => Target.SetValue(WriteProperty, ((Constant<T>)RHS).Value);
        object a0 = a;
        return (TDelegate)a0;
      } else return (new DefaultLinqEval()).DoSetNow<T, TDelegate>(this, RHS, Args);
    }
    public override bool SetNow(T RHS) {
      Target.SetValue(WriteProperty, RHS);
      return true;
    }
  }
  public interface DependencyPropertyEval<SELF> : IEval<SELF> where SELF : Eval<SELF> {
    Eval<SELF>.Info<T> PropertyValue<T>(DependencyPropertyExpr<T> value);
  }
  public class LinqBindEval : LinqEval<LinqBindEval>, IAssign, DependencyPropertyEval<LinqBindEval> /*, DependencyPropertyEval<LinqBindEval>*/ {
    public readonly linq.ParameterExpression Args;
    internal bool DoTwoWay = false;
    public LinqBindEval() : base() {
      // no slots, manage the one argument ourselves!
      Args = linq.Expression.Parameter(typeof(object[]), "Arguments");
    }
    private LinqBindEval(LinqBindEval toCopy) : base() {
      Args = toCopy.Args;
      foreach (var b in toCopy.Binding.Bindings)
        Binding.Bindings.Add(b);
      foreach (var k in toCopy.Properties)
        Properties[k.Key] = k.Value;
      toCopy.DoTwoWay = DoTwoWay;
    }
    private readonly Dictionary<Expr, int> Properties = new Dictionary<Expr, int>();
    public struct BindKey {
      public linq.Expression Target;
      public DependencyProperty Property;
    }
    public readonly List<BindKey> BindKeys = new List<BindKey>();

    public readonly MultiBinding Binding = new MultiBinding();
    public bool Delay = false;
    public IAssign Copy() {
      return new LinqBindEval(this);
    }
    public Info<T> PropertyValue<T>(DependencyPropertyExpr<T> value) {
      // be conservative!
      if (!Properties.ContainsKey(value)) {
        int index;
        if (Delay) {
          index = BindKeys.Count;
          BindKeys.Add(new BindKey() {
            Target = ((MyInfo)value.BaseTargetValue.BaseEval(this)).Value, Property = value.ReadProperty,
          });
        } else {
          index = Binding.Bindings.Count;
          Binding.Bindings.Add(new Binding() { Source = value.Target, Path = new PropertyPath(value.ReadProperty) });
        }
        Properties[value] = index;
      }
      var access = linq.Expression.ArrayAccess(Args, linq.Expression.Constant(Properties[value]));
      var convert = linq.Expression.Convert(access, typeof(T));
      return new MyInfo<T>() { Value = convert };
    }
    public Info<S> PropertyValue<R, S>(Info<R> container, DependencyProperty P) {
      var container0 = ((MyInfo<R>)container).Value;
      if (container0 is linq.ConstantExpression) {
        DependencyObject container1 = (DependencyObject)((linq.ConstantExpression)container0).Value;
        return this.DoEval(new DependencyPropertyExpr<DependencyObject, S>() { TargetValue = container1, ReadProperty = P, WriteProperty = P });
      } else if (Delay) {
        // can't support delay.
        throw new NotSupportedException();
      }
      throw new NotSupportedException();
    }
    public bool Accept<T>(Expr<T> LHS, Expr<T> RHS) {
      if (!(LHS is DependencyPropertyExpr<T>)) return false;
      var LHS0 = (DependencyPropertyExpr<T>)LHS;
      var RHS0 = DoEval(RHS).Value;
      var body = this.PopScope<T>(RHS0);

      var F0 = linq.Expression.Lambda<Microsoft.Func<object[], T>>(body, Args).Compile();
      Func<object[], T> F = (args) => F0(args);

      Binding.Mode = DoTwoWay ? BindingMode.TwoWay : BindingMode.OneWay;
      Func<T, object[]> G = null;
      if (DoTwoWay) {


      }
      Binding.Converter = new BindConverter<T>() { F = F, G = G };
      var Target = LHS0.Target;
      if (Target == null) {
        Target = LHS0.Target;
      }
      var e = BindingOperations.SetBinding(Target, LHS0.WriteProperty, Binding);
      return true;
    }

    private linq.Expression<Microsoft.Func<object[], T>> Convert<T>(linq.Expression body, IParameterExpr[] Ps) {
      var F1 = linq.Expression.Lambda(body, this.Parameters.Flush());
      var param = linq.Expression.Parameter(typeof(object[]), "params");
      // we don't have to create the array!
      var invokes = new linq.Expression[Ps.Length];
      for (int i = 0; i < Ps.Length; i++) {
        invokes[i] = linq.Expression.ArrayAccess(param, linq.Expression.Constant(i, typeof(int)));
        invokes[i] = linq.Expression.Convert(invokes[i], Ps[i].TypeOfT);
      }
      var F2 = linq.Expression.Lambda<Microsoft.Func<object[], T>>(
        linq.Expression.Invoke(F1, invokes), param);
      return F2;
    }
    public static Action<SOURCE> DelayBind<SOURCE, T>(Expr<T> RHS, DependencyProperty Property) where SOURCE : DependencyObject {
      var ret = DelayBind<SOURCE,T>(RHS, Property, new IParameterExpr[] { });
      return (source) => ret(source, new object[0]);
    }

    public static Action<SOURCE, object[]> DelayBind<SOURCE,T>(Expr<T> RHS, DependencyProperty Property, IParameterExpr[] Ps) where SOURCE : DependencyObject {
      var eval = new LinqBindEval() { Delay = true };
      foreach (var p in Ps) eval.Parameters.Add(p);

      var RHS0 = eval.DoEval(RHS).Value;
      RHS0 = eval.PopScope<T>(RHS0);

      // the function that we want to return to do databinding.
      var F0 = linq.Expression.Lambda<Microsoft.Func<object[], T>>(RHS0, eval.Args);
      var F2 = eval.Convert<Microsoft.Func<object[], T>>(F0, Ps);
      var F3 = F2.Compile();
      var keys = eval.BindKeys;
      var properties = new PropertyPath[keys.Count];
      var targets = new Microsoft.Func<object[], DependencyObject>[keys.Count];
      for (int i = 0; i < keys.Count; i++) {
        properties[i] = new PropertyPath(keys[i].Property);
        targets[i] = eval.Convert<DependencyObject>(keys[i].Target, Ps).Compile();
      }
      return (Source, Qs) => {
        var F4 = F3(Qs);
        var Binding = new MultiBinding();
        // copy the bindings.
        for (int i = 0; i < properties.Length; i++) {
          Binding.Bindings.Add(new Binding() { Source = targets[i](Qs), Path = properties[i] });
        }
        Binding.Mode = BindingMode.OneWay;
        Binding.Converter = new LinqBindEval.BindConverter<T>() { F = args => F4(args), G = null };
        var e = BindingOperations.SetBinding(Source, Property, Binding);
      };
    }


    internal class BindConverter<T> : IMultiValueConverter {
      public Func<object[], T> F;
      public Func<T, object[]> G;
      public object Convert(object[] args, Type type, object whatever, System.Globalization.CultureInfo useless) {
        return F(args);
      }
      public object[] ConvertBack(object result, Type[] argTypes, object whatever, System.Globalization.CultureInfo useless) {
        if (G == null) throw new NotSupportedException();
        return G((T)result);
      }
    }
  }
  public interface IDependencyObjectBl<BRAND>
    where BRAND : Brand<BRAND>, IDependencyObjectBl<BRAND>
    { }
  public interface IDependencyObjectBl<T, BRAND> : IDependencyObjectBl<BRAND> 
    where BRAND : Brand<T, BRAND>, IDependencyObjectBl<T, BRAND>
    where T : DependencyObject { }

  public static partial class DatabindingExtensions {
    public static BRAND Property<CNT, BRAND>(this Brand<CNT> Container, Brand<BRAND> Init)
      where BRAND : Brand<BRAND>
      where CNT : Brand<CNT>, IDependencyObjectBl<CNT> {
      var CntT = Brand<CNT>.TypeOfT;
      var Instance = (IDependencyPropertyFactory)
        typeof(DependencyPropertyFactory<>).MakeGenericType(CntT).GetField("Instance").GetValue(null);
      var ret = Instance.BaseAnon<BRAND>()(Container.Underlying);
      ret.Bind = Init; return ret;
    }
    public static BRAND Property<CNT, BRAND>(this CNT Container, Brand<BRAND> Init)
      where BRAND : Brand<BRAND>
      where CNT : DependencyObject {
      var Instance = 
        DependencyPropertyFactory<CNT>.Instance;
      var ret = Instance.Anon<BRAND>()(new Constant<CNT>(Container));
      
      ret.Bind = Init; 
      return ret;
    }
    public static BRAND Property<CNT, BRAND>(this CNT Container, Brand<BRAND> Init, Action<object,object> OnChange)
      where BRAND : Brand<BRAND>
      where CNT : DependencyObject {
      var Instance = 
        DependencyPropertyFactory<CNT>.Instance;
      var ret = Instance.Anon<BRAND>(OnChange)(new Constant<CNT>(Container));
      ret.Bind = Init; return ret;
    }

    public static DependencyPropertyExpr<S, T> Property<S, T>(this Expr<S> Container, DependencyProperty Property) {
      return new DependencyPropertyExpr<S, T>() { Property = Property, TargetValue = Container };
    }


    public static BRAND Property0<S, BRAND>(this Expr<S> Container, DependencyProperty Property) where BRAND : Brand<BRAND> {

      throw new NotSupportedException();


      //return new DependencyPropertyExpr<S, T>() { Property = Property, TargetValue = Container };
    }

    private static readonly MethodInfo GetValue = typeof(DependencyObject).GetMethod("GetValue");
    private static readonly MethodInfo SetValue = typeof(DependencyObject).GetMethod("SetValue", new Type[] { typeof(DependencyProperty), typeof(object) });

    internal static linq.Expression DoGetValue(Type type, linq.Expression target, DependencyProperty ReadProperty) {
      return linq.Expression.Convert(linq.Expression.Call(linq.Expression.Convert(target, typeof(DependencyObject)), GetValue, linq.Expression.Constant(ReadProperty, typeof(DependencyProperty))), type);
    }
    internal static linq.Expression DoSetValue(linq.Expression target, DependencyProperty WriteProperty, linq.Expression RHS) {
      return linq.Expression.Call(linq.Expression.Convert(target, typeof(DependencyObject)), SetValue, linq.Expression.Constant(WriteProperty, typeof(DependencyProperty)), linq.Expression.Convert(RHS, typeof(object)));
    }

    private class CommandX : IAssign {
      public Action<DependencyObject, DependencyProperty> F;
      public bool Accept<S>(Expr<S> LHS, Expr<S> RHS) {
        if (!(LHS is DependencyPropertyExpr<S>)) return false;
        var LHS0 = (DependencyPropertyExpr<S>)LHS;
        F(LHS0.Target, LHS0.WriteProperty);
        return true;
      }
      public IAssign Copy() { return this; }
    }
    /// <summary>
    /// Clear the current now assigned value of underlying properties.
    /// </summary>
    /// <remarks>WPF dependency property only.</remarks>
    public static void ClearNow<T,BRAND>(this Brand<T,BRAND> self) where BRAND : Brand<T,BRAND> {
      if (!self.Underlying.Solve(new CommandX() { F = (a, b) => a.ClearValue(b) }, new Constant<T>(default(T)))) throw new NotSupportedException();
    }
    /// <summary>
    /// Clear the current constraint of underlying properties.
    /// </summary>
    /// <remarks>WPF dependency property only.</remarks>
    public static void ClearConstraint<T, BRAND>(this Brand<T, BRAND> self) where BRAND : Brand<T, BRAND> {
      if (!self.Underlying.Solve(new CommandX() { F = (a, b) => BindingOperations.ClearBinding(a, b) }, new Constant<T>(default(T)))) throw new NotSupportedException();
    }
    /// <summary>
    /// Stop animation of the underlying dependency property.
    /// </summary>
    /// <param name="capture">Whether to capture the curent value as locally set.</param>
    /// <remarks>WPF dependency property only.</remarks>
    public static void Stop<T, BRAND>(this Brand<T, BRAND> self, bool capture) where BRAND : Brand<T, BRAND> {
      if (!self.Underlying.Solve(new CommandX() {
        F = (a, b) => {
          var v = a.GetValue(b);
          ((IAnimatable)a).BeginAnimation(b, null);
          if (capture) a.SetValue(b, v);
        }
      }, new Constant<T>(default(T)))) throw new NotSupportedException();
    }
    public static Animate<T,BRAND> Animate<T, BRAND>(this Brand<T, BRAND> self) where BRAND : Brand<T, BRAND> {
      return new Animate<T, BRAND>() { Target = self };
    }

    /// <summary>
    /// Clear all values, binding, and animation.
    /// </summary>
    /// <param name="capture">Whether to capture the curent value as locally set.</param>
    /// <remarks>WPF dependency property only.</remarks>
    public static void ClearAll<T, BRAND>(this Brand<T, BRAND> self, bool capture) where BRAND : Brand<T, BRAND> {
      if (!self.Underlying.Solve(new CommandX() {
        F = (a, b) => {
          var v = a.GetValue(b);
          a.ClearValue(b);
          BindingOperations.ClearBinding(a, b);
          if (a is IAnimatable) ((IAnimatable)a).BeginAnimation(b, null);
          if (capture) a.SetValue(b, v);
        }
      }, new Constant<T>(default(T)))) throw new NotSupportedException();
    }
    public static GetProperty<OWNER, TYPE> NewProperty<OWNER, TYPE>(this string name) where OWNER : DependencyObject {
      var dp = DependencyProperty.Register(name, typeof(TYPE), typeof(OWNER));
      return new GetProperty<OWNER, TYPE>() { Property = dp };
    }
    public static GetProperty<OWNER, TYPE> NewProperty<OWNER, TYPE>(this string name, TYPE defaultValue) where OWNER : DependencyObject {
      var dp = DependencyProperty.Register(name, typeof(TYPE), typeof(OWNER), new PropertyMetadata(defaultValue));
      return new GetProperty<OWNER, TYPE>() { Property = dp };
    }
    public static GetProperty<OWNER, TYPE> NewProperty<OWNER, TYPE>(this string name, TYPE defaultValue, TypedDependencyPropertyChangedEventHandler<OWNER, TYPE> update, Func<OWNER, TYPE, TYPE> coerce) where OWNER : DependencyObject {
      DependencyProperty dp = null;
      dp = DependencyProperty.Register(name, typeof(TYPE), typeof(OWNER), new PropertyMetadata(defaultValue, (x, y) => {
        if (y.Property == dp) update((OWNER)x, (TYPE)y.OldValue, (TYPE)y.NewValue);
      }, (self, value) => coerce((OWNER)self, (TYPE)value)));
      return new GetProperty<OWNER, TYPE>() { Property = dp };
    }
    public static GetProperty<OWNER, TYPE> NewProperty<OWNER, TYPE>(this string name, TYPE defaultValue, TypedDependencyPropertyChangedEventHandler<OWNER, TYPE> update) where OWNER : DependencyObject {
      DependencyProperty dp = null;
      dp = DependencyProperty.Register(name, typeof(TYPE), typeof(OWNER), new PropertyMetadata(defaultValue, (x, y) => {
        if (y.Property == dp) update((OWNER)x, (TYPE)y.OldValue, (TYPE)y.NewValue);
      }));
      return new GetProperty<OWNER, TYPE>() { Property = dp };
    }
    public static GetProperties<OWNER, TYPE> NewProperties<OWNER, TYPE>(this string name, int count) where OWNER : DependencyObject {
      var Properties = new DependencyProperty[count];
      for (int i = 0; i < count; i++) {
        Properties[i] = DependencyProperty.Register(name + "_" + i, typeof(TYPE), typeof(OWNER));
      }
      return new GetProperties<OWNER, TYPE>() {
        Properties = Properties,
      };
    }


    public static Expr<T> Property<T>(this Expr Expr, DependencyProperty Property) {
      return Expr.Property<T>(Property, Property);
    }
    public static Expr<T> Property<T>(this Expr Expr, DependencyProperty ReadProperty, DependencyProperty WriteProperty) {
      return Expr.Visit<Expr<T>>(new PropertyGet<T>() { ReadProperty = ReadProperty, WriteProperty = WriteProperty });
    }
    private class PropertyGet<T> : ExprVisitor<Expr<T>> {
      public DependencyProperty ReadProperty;
      public DependencyProperty WriteProperty;
      public Expr<T> Visit<S>(Expr<S> value) {
        return new DependencyPropertyExpr<S, T>() { TargetValue = value, ReadProperty = ReadProperty, WriteProperty = WriteProperty };
      }

    }

    public static Expr<T> Bl<T>(this DependencyObject Target, DependencyProperty Property) {
      return new DependencyPropertyExpr<DependencyObject, T>() {
        TargetValue = Target,
        ReadProperty = Property,
        WriteProperty = Property,
      };
    }
  }
  /// <summary>
  /// Auxillary class use to configure and execute animations.
  /// </summary>
  public class Animate<T, BRAND> where BRAND : Brand<T, BRAND> {
    internal BRAND Target;
    Func<AnimationTimeline, AnimationTimeline> F = t => t;
    Func<DoubleBl, DoubleBl> Tween0 = null;
    bool DoStopOnCompleted = false;
    /// <summary>
    /// Specify duration of animation in milliseconds.
    /// </summary>
    /// <param name="millis">The timeline's simple duration in milliseconds: the amount of time this timeline takes to
    ///     complete a single forward iteration</param>
    /// <returns>Itself.</returns>
    public Animate<T, BRAND> Duration(double millis) {
      var G = F;
      F = t => G(t).Duration(millis);
      return this;
    }
    /// <summary>
    /// Specify a callback to be called when the animation is completed.
    /// </summary>
    /// <param name="P">Callback called when the animation is done. Argument will be the animated property.</param>
    /// <returns>Itself.</returns>
    public Animate<T, BRAND> Completed(Action<BRAND> P) {
      var G = F;
      F = t => {
        t = G(t);
        t.Completed += (x, y) => P(Target);
        return t;
      };
      return this;
    }
    /// <summary>
    /// If true, stop and capture the value of this animation once it is completed. 
    /// This means that future local updates can affect the value. By default, stop 
    /// on completed is true for an animation, set to false if this is not desired behavior.
    /// </summary>
    /// <returns>Itself.</returns>
    public Animate<T, BRAND> StopOnCompleted(bool b) {
      DoStopOnCompleted = b;
      return this;
    }


    /// <summary>
    /// Specify duration of animation as a time span.
    /// </summary>
    /// <param name="millis">The timeline's simple duration: the amount of time this timeline takes to
    ///     complete a single forward iteration</param>
    /// <returns>Itself.</returns>
    public Animate<T, BRAND> Duration(TimeSpan span) {
      var G = F;
      F = t => G(t).Duration(span);
      return this;
    }
    /// <summary>
    /// Specify duration of animation as a time span.
    /// </summary>
    /// <param name="millis">The timeline's duration: the amount of time this timeline takes to
    ///     complete a single forward iteration</param>
    /// <returns>Itself.</returns>
    public Animate<T, BRAND> Duration(Duration duration) {
      var G = F;
      F = t => G(t).Duration(duration);
      return this;
    }
    /// <summary>
    /// Sets the time at which this System.Windows.Media.Animation.Timeline
    /// should begin
    /// </summary>
    /// <param name="millis">The time (in millseconds) at which this System.Windows.Media.Animation.Timeline should begin,
    ///     relative to its parent's System.Windows.Media.Animation.Timeline.BeginTime.
    ///     If this timeline is a root timeline, the time is relative to its interactive
    ///     begin time (the moment at which the timeline was triggered). This value may
    ///     be positive, negative.
    ///     The default value is zero</param>
    public Animate<T, BRAND> Delay(double millis) {
      var G = F;
      F = t => G(t).BeginTime(millis);
      return this;
    }
    /// <summary>
    /// Sets the time at which this System.Windows.Media.Animation.Timeline
    /// should begin
    /// </summary>
    /// <param name="span">The time at which this System.Windows.Media.Animation.Timeline should begin,
    ///     relative to its parent's System.Windows.Media.Animation.Timeline.BeginTime.
    ///     If this timeline is a root timeline, the time is relative to its interactive
    ///     begin time (the moment at which the timeline was triggered). This value may
    ///     be positive, negative.
    ///     The default value is zero</param>
    /// <returns></returns>
    public Animate<T, BRAND> Delay(TimeSpan span) {
      var G = F;
      F = t => G(t).BeginTime(span);
      return this;
    }


    /// <summary>
    /// Specify that animation should run forever.
    /// </summary>
    /// <returns>Itself.</returns>
    public Animate<T, BRAND> Forever() {
      var G = F;
      F = t => G(t).Forever();
      return this;
    }
    /// <summary>
    /// Specify that animation should reverse after completing.
    /// </summary>
    /// <returns>Itself.</returns>
    public Animate<T, BRAND> AutoReverse() {
      var G = F;
      F = t => G(t).AutoReverse();
      return this;
    }
    public Animate<T, BRAND> Tween(Func<DoubleBl, DoubleBl> Tween) {
      (Tween0 == null).Assert();
      Tween0 = Tween;
      return this;
    }

    /// <summary>
    /// Start destination-based animation to specified value. 
    /// </summary>
    public BRAND To {
      set {
        if (Percents0 != null) throw new NotSupportedException();
        if (DoStopOnCompleted) this.Completed(p => p.Stop(true));
        if (!Target.Underlying.Solve(new ToAnimateAssign() { F = F, Tween = Tween0 }, value)) throw new NotSupportedException();
      }
    }
    private DoubleBl[] Percents0;
    /// <summary>
    /// Set percentages of key frame animation.
    /// </summary>
    /// <param name="Percents">List of percents.</param>
    /// <returns>Itself.</returns>
    public Animate<T, BRAND> Percents(params DoubleBl[] Percents) {
      Percents0 = Percents;
      return this;
    }

    /// <summary>
    /// Start key frame animation according to percents previously specified.
    /// </summary>
    /// <param name="values">List of key values corresponding to previously specified percents.</param>
    public void Keys(params BRAND[] values) {
      if (Tween0 != null) throw new NotSupportedException();
      if (Percents0.Length != values.Length) throw new ArgumentException();
      if (DoStopOnCompleted) this.Completed(p => p.Stop(true));
      var a = new KeyAnimateAssign() { };
      double[] percents = new double[Percents0.Length];
      for (int i = 0; i < Percents0.Length; i++) {
        Target.Underlying.Solve(a, values[i]);
        percents[i] = Percents0[i].CurrentValue;
      }
      foreach (var r in a.Record) 
        r.Value.Animate(percents, F);
    }
  }
  public abstract class BaseCustomAnimation : AnimationTimeline {
    public abstract Func<DoubleBl, DoubleBl> Tween { set; }
    public abstract Expr From0 { set; }
    public abstract Expr To0 { set; }
  }
  public class CustomAnimation<T> : BaseCustomAnimation {
    protected static readonly GetProperty<CustomAnimation<T>, T> FromProperty = "From".NewProperty<CustomAnimation<T>, T>();
    protected static readonly GetProperty<CustomAnimation<T>, T> ToProperty = "To".NewProperty<CustomAnimation<T>, T>();
    private Func<double, T> CurrentValue;
    public override Func<DoubleBl, DoubleBl> Tween {
      set {
        CurrentValue = (new DefaultLinqEval()).Eval<double, T>(Fraction => {
          Fraction = value == null ? Fraction : value(Fraction).Underlying;
          var ret = Ops.DoubleOperators0<T>.Lerp.Instance.Make(FromProperty[this], ToProperty[this], Fraction);
          return ret;
        });
      }
    }
    public CustomAnimation(Func<DoubleBl, DoubleBl> Tween) { this.Tween = Tween; }
    public CustomAnimation() : this(d => d) { }
    private CustomAnimation(Func<double, T> CurrentValue) {
      this.CurrentValue = CurrentValue;
    }
    public override Expr From0 {
      set { this.SetValue(FromProperty.Property, value.BaseCurrentValue); }
    }
    public override Expr To0 {
      set { this.SetValue(ToProperty.Property, value.BaseCurrentValue); }
    }
    public override Type TargetPropertyType {
      get { return typeof(T); }
    }
    protected override Freezable CreateInstanceCore() {
      var frozen = new CustomAnimation<T>(CurrentValue);
      if (!frozen.Duration.HasTimeSpan) frozen.Duration = new Duration(TimeSpan.FromSeconds(1));
      return frozen;
    }
    public override object GetCurrentValue(object defaultOriginValue, object defaultDestinationValue, AnimationClock animationClock) {
      var fraction = animationClock.CurrentTime.Value.TotalSeconds / Duration.TimeSpan.TotalSeconds;
      return CurrentValue(fraction);
    }
  }
  class ToAnimateAssign : IAssign {
    public Func<AnimationTimeline, AnimationTimeline> F = t => t;
    public Func<DoubleBl, DoubleBl> Tween = null;
    public IAssign Copy() { return new ToAnimateAssign() { F = F, Tween = Tween }; }
    public bool Accept<T>(Expr<T> LHS, Expr<T> RHS) {
      if (!(LHS is DependencyPropertyExpr<T>)) return false;
      var LHS0 = (DependencyPropertyExpr<T>)LHS;
      var ani = new CustomAnimation<T>(Tween) {
        To0 = RHS,
        From0 = LHS,
      };
      ((IAnimatable)LHS0.Target).BeginAnimation(LHS0.WriteProperty, F(ani));
      return true;
    }
  }
  abstract class BaseKeyPair {
    public abstract void Animate(double[] percents, Func<AnimationTimeline, AnimationTimeline> F);
  }
  class KeyPair<T> : BaseKeyPair {
    public DependencyPropertyExpr<T> Target;
    public readonly List<T> Values = new List<T>();
    public override void Animate(double[] percents, Func<AnimationTimeline, AnimationTimeline> F) {
      AnimationTimeline a;
      if (typeof(T) == typeof(double)) {
        var b = new DoubleAnimationUsingKeyFrames();
        for (int i = 0; i < Values.Count; i++) {
          b.KeyFrames.Add(new LinearDoubleKeyFrame((double)(object)Values[i], KeyTime.FromPercent(percents[i])));
        }
        a = b;
      } else if (typeof(T) == typeof(Point)) {
        var b = new PointAnimationUsingKeyFrames();
        for (int i = 0; i < Values.Count; i++) {
          b.KeyFrames.Add(new LinearPointKeyFrame((Point)(object)Values[i], KeyTime.FromPercent(percents[i])));
        }
        a = b;
      } else if (typeof(T) == typeof(Color)) {
        var b = new ColorAnimationUsingKeyFrames();
        for (int i = 0; i < Values.Count; i++) {
          b.KeyFrames.Add(new LinearColorKeyFrame((Color)(object)Values[i], KeyTime.FromPercent(percents[i])));
        }
        a = b;
      } else if (typeof(T) == typeof(bool)) {
        var b = new BooleanAnimationUsingKeyFrames();
        for (int i = 0; i < Values.Count; i++) {
          b.KeyFrames.Add(new DiscreteBooleanKeyFrame((bool)(object)Values[i], KeyTime.FromPercent(percents[i])));
        }
        a = b;
      } else if (typeof(T) == typeof(int)) {
        var b = new Int32AnimationUsingKeyFrames();
        for (int i = 0; i < Values.Count; i++) {
          b.KeyFrames.Add(new DiscreteInt32KeyFrame((int)(object)Values[i], KeyTime.FromPercent(percents[i])));
        }
        a = b;
      } else throw new NotImplementedException();
      ((IAnimatable)Target.Target).BeginAnimation(Target.WriteProperty, F(a));
    }
  }
  class KeyAnimateAssign : IAssign {
    public IAssign Copy() { return this; }
    public readonly Dictionary<Expr, BaseKeyPair> Record = new Dictionary<Expr, BaseKeyPair>();
    public bool Accept<T>(Expr<T> LHS, Expr<T> RHS) {
      if (!(LHS is DependencyPropertyExpr<T>)) return false;
      var LHS0 = (DependencyPropertyExpr<T>)LHS;
      if (!Record.ContainsKey(LHS0)) Record[LHS0] = new KeyPair<T>() { Target = LHS0 };
      ((KeyPair<T>)Record[LHS0]).Values.Add(RHS.CurrentValue);
      return true;
    }
  }
  public abstract class GetProperty<S> where S : DependencyObject {
    internal DependencyProperty Property { get; set; }
    public abstract Expr BaseGet(Expr<S> Container);
    public abstract void BaseSet(Expr<S> Container, Expr Value);
  }

  public class GetProperty<S, T> : GetProperty<S> where S : DependencyObject {
    public DependencyPropertyExpr<T> this[Expr<S> Container] {
      get { return Container.Property<S,T>(Property); }
    }
    public override Expr BaseGet(Expr<S> Container) {
      return this[Container];
    }
    public override void BaseSet(Expr<S> Container, Expr Value) {
      this[Container].Bind = (Expr<T>) Value;
    }
  }



  public class GetProperties<S, T> where S : DependencyObject {
    internal DependencyProperty[] Properties;
    public int Count { get { return Properties.Length; } }
    public GetProperty<S, T> this[int idx] {
      get { return new GetProperty<S, T>() { Property = Properties[idx] }; }
    }
    public PropertyValues<S,T> this[Expr<S> Container] {
      get { return new PropertyValues<S, T>() { Container = Container, Properties = Properties }; }
    }
  }
  public class PropertyValues<S, T> where S : DependencyObject {
    internal DependencyProperty[] Properties;
    internal Expr<S> Container;
    public DependencyPropertyExpr<T> this[int idx] {
      get { return Container.Property<S,T>(Properties[idx]); }
      set { this[idx].Bind = value; }
    }
  }

  /// <summary>
  /// A dependency object that will create a list of objects whose count can change dynamically. 
  /// </summary>
  /// <typeparam name="T">Type of object that is contained in list.</typeparam>
  public class DynamicArray<T> : DependencyObject {
    private static readonly GetProperty<DynamicArray<T>, int> CountProperty =
      "Count".NewProperty<DynamicArray<T>, int>(0, (x, oldV, newV) => x.CountChanged(oldV, newV));
    private readonly Func<int, T> AddT;
    private readonly Action<int, T> RemT;
    /// <summary>
    /// Number of objects to create. This is a dependency property. 
    /// </summary>
    public IntBl Count {
      get { return CountProperty[this]; }
      set { Count.Bind = value; }
    }
    /// <summary>
    /// Create a new dynamic array with specified add and remove functions.
    /// </summary>
    /// <param name="AddT">Function to call when object is added.</param>
    /// <param name="RemT">Function to call when object is removed.</param>
    public DynamicArray(Func<int, T> AddT, Action<int, T> RemT) {
      this.AddT = AddT; this.RemT = RemT;
    }
    private readonly List<T> Elements = new List<T>();
    /// <summary>
    /// Clear all objects from list. 
    /// </summary>
    public void Clear() {
      while (Elements.Count > 0) {
        var e = Elements[Elements.Count - 1];
        Elements.RemoveAt(Elements.Count - 1);
        RemT(Elements.Count, e);
      }
      Count = 0;
    }
    /// <summary>
    /// Get element by index.
    /// </summary>
    /// <param name="index">Index of element to access.</param>
    /// <returns>Accessed element.</returns>
    public T this[int index] {
      get { return Elements[index]; }
    }
    private void CountChanged(int oldV, int newV) {
      while (Elements.Count < newV)
        Elements.Add(AddT(Elements.Count));
      while (Elements.Count > newV) {
        var e = Elements[Elements.Count - 1];
        Elements.RemoveAt(Elements.Count - 1);
        RemT(Elements.Count, e);
      }
    }
  }
  public interface IDependencyPropertyFactory {
    Func<Expr, BRAND> BaseAnon<BRAND>() where BRAND : Brand<BRAND>;

  }
  public class DependencyPropertyFactory<CNT> : Gen.Generator<Expr<CNT>>, IDependencyPropertyFactory where CNT : DependencyObject {
    public override string Name { get { return "DepProp"; } }
    private readonly Dictionary<PropertyInfo, GetProperty<CNT>> Properties = new Dictionary<PropertyInfo, GetProperty<CNT>>();

    private int AnonCount = 0;

    public Func<Expr, BRAND> BaseAnon<BRAND>() where BRAND : Brand<BRAND> {
      var f = Anon<BRAND>();
      return e => f((Expr<CNT>)e);
    }
    public Func<Expr<CNT>, BRAND> Anon<BRAND>() where BRAND : Brand<BRAND> {
      var name = "Anon" + AnonCount;
      AnonCount += 1;
      var TypeOfT = Brand<BRAND>.TypeOfT;
      var c = typeof(BRAND).GetConstructor(new Type[0]);
      c.Invoke(new object[0]);
      var Default0 = (Brand<BRAND>.Default).Underlying.BaseCurrentValue;
      var dp0 = DependencyProperty.Register(name, TypeOfT, typeof(CNT), new PropertyMetadata(Default0));
      var GetT = (typeof(GetProperty<,>)).MakeGenericType(typeof(CNT), TypeOfT);
      var dp = (GetProperty<CNT>)GetT.GetConstructor(new Type[0] { }).Invoke(new object[0] { });
      dp.Property = dp0;
      return cnt => Brand<BRAND>.ToBrand(dp.BaseGet(cnt));
    }
    public Func<Expr<CNT>, BRAND> Anon<BRAND>(Action<object,object> Changed) where BRAND : Brand<BRAND> {
      var name = "Anon" + AnonCount;
      AnonCount += 1;
      var TypeOfT = Brand<BRAND>.TypeOfT;
      var c = typeof(BRAND).GetConstructor(new Type[0]);
      c.Invoke(new object[0]);
      var Default0 = (Brand<BRAND>.Default).Underlying.BaseCurrentValue;
      var dp0 = DependencyProperty.Register(name, TypeOfT, typeof(CNT), new PropertyMetadata(Default0, (x, y) => {
        Changed(y.OldValue, y.NewValue);
      }));
      var GetT = (typeof(GetProperty<,>)).MakeGenericType(typeof(CNT), TypeOfT);
      var dp = (GetProperty<CNT>)GetT.GetConstructor(new Type[0] { }).Invoke(new object[0] { });
      dp.Property = dp0;
      return (cnt) => Brand<BRAND>.ToBrand(dp.BaseGet(cnt));
    }

    public override PropertyCode Property(PropertyInfo Property) {
      GetProperty<CNT> dp;
      if (!Properties.ContainsKey(Property)) {
        var T = Brand.TypeOfT(Property.PropertyType);

        Property.PropertyType.GetConstructor(new Type[0]).Invoke(new object[0]);
        var Default0 = typeof(Brand<>).MakeGenericType(Property.PropertyType).GetProperty("Default");
        var Default1 = (Brand)Default0.GetAccessors()[0].Invoke(null, new object[0]);
        var Default2 = Default1.Underlying.BaseCurrentValue;

        var dp0 = DependencyProperty.Register(Property.Name, T, typeof(CNT), new PropertyMetadata(Default2));
        var GetT = (typeof(GetProperty<,>)).MakeGenericType(typeof(CNT), T);


        dp = (GetProperty<CNT>)GetT.GetConstructor(new Type[0] { }).Invoke(new object[0] { });
        dp.Property = dp0;
        Properties[Property] = dp;
      } else dp = Properties[Property];
      var ToBrand = Brand.ToBrand(Property.PropertyType);

      var pc = new PropertyCode() {
        Get = (cnt) => ToBrand(dp.BaseGet(cnt)),
        Set = (cnt,value) => dp.BaseSet(cnt, ((Brand) value).Underlying),
      };

      return pc;
    }
    public new static Func<Expr<CNT>,T> Generate<T>() {
      return Instance.Generate0<T>();
    }
    private Func<Expr<CNT>, T> Generate0<T>() {
      return base.Generate<T>();
    }

    private DependencyPropertyFactory() { }
    public static readonly DependencyPropertyFactory<CNT> Instance = new DependencyPropertyFactory<CNT>();




  }
  [AttributeUsage(AttributeTargets.Property)]
  public class PropertyMetaData : System.Attribute  {
    public object Default;

  }
  public class WayPoint : BrandTypeReveal<int> {
    private readonly Brand[] Subjects;
    private readonly object[] Values;
    public WayPoint(params Brand[] Subjects) {
      this.Subjects = Subjects;
      Values = new object[Subjects.Length];
      for (int i = 0; i < Values.Length; i++) {
        Values[i] = Subjects[i].Underlying.BaseCurrentValue;
      }
    }
    private DoubleBl t;
    private int idx;
    public void Restore(DoubleBl t) { // should be from zero to one.
      this.t = t;
      for (idx = 0; idx < Subjects.Length; idx++) Subjects[idx].Reveal(this);
    }
    public int Visit<T, BRAND>(Brand<T, BRAND> value) where BRAND : Brand<T, BRAND> {
      var From = value.Now;
      var To = Brand<T, BRAND>.ToBrand(new DSL.Constant<T>((T)Values[idx]));
      value.ClearNow();
      var From0 = ((ISupportsLerp<BRAND>)From);
      value.Bind = From0.Lerp0(To, t);
      return 0;
    }
  }
}