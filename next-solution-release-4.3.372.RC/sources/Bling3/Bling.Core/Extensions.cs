using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Text;
using System.Reflection;

namespace Bling.Util {
  public interface ICanStart {
    void Start();
    void Stop();
  }
  public interface IHasGlobalPosition {
    void SetGlobalPosition(Bling.Core.PointBl Point);
    void UnsetGlobalPosition();
  }


  public struct IntPoint {
    public int X;
    public int Y;

  }

  public static class MyMath {
    public static long Factorial(int n) {
      if (n <= 0) return 1;
      else return n * Factorial(n - 1);
    }
    private static readonly long[] FactTable = new long[20];
    static MyMath() {
      Func<int,long> Fact = null;
      int j = 0;
      Fact = (i) => {
        if (i <= 1) return 1;
        else if (i < j) return FactTable[j];
        else return i * Fact(i - 1);
      };
      for (j = 0; j < FactTable.Length; j++) FactTable[j] = Fact(j);
    }
  }

  public class MockArray<INDEX, ITEM> {
    public Func<INDEX, ITEM> Get { get; set; }
    public Action<INDEX, ITEM> Set { get; set; }
    public int Count;
    public ITEM this[INDEX idx] {
      get { return Get(idx); }
      set { Set(idx, value); }
    }
  }



  public interface ISimpleList<T> {
    T this[int index] { get; }
    int Count { get; }
  }
  public abstract class SimpleList<T> : ISimpleList<T> {
    public abstract T this[int index] { get; }
    public abstract int Count { get; }
    public static implicit operator SimpleList<T>(T[] list) { return new ListToSimple<T>(list); }
  }
  /*
  public class DefaultSimpleList<T> :  SimpleList<T> {
    public override int Count {
      get { return Count0(); }
    }
    public override T this[int index] {
      get { return F(index); }
    }
    private readonly Func<int> Count0;
    private readonly Func<int, T> F;
    public DefaultSimpleList(Func<int> Count, Func<int, T> F) {
      Count0 = Count;
    }
  }*/

  public class ListToSimple<T> : SimpleList<T> {
    readonly IList<T> Underlying;
    public ListToSimple(IList<T> Underlying) { this.Underlying = Underlying; }
    public override int Count {
      get { return Underlying.Count; }
    }
    public override T this[int index] {
      get { return Underlying[index]; }
    }
    public override string ToString() {
      return Underlying.ToString();
    }
  }

  public class MapList<T, S> : ISimpleList<S> {
    readonly ISimpleList<T> Underlying;
    readonly Func<T, S> f;
    public MapList(IList<T> Underlying, Func<T, S> F) : this(new ListToSimple<T>(Underlying), F) { }
    public MapList(ISimpleList<T> Underlying, Func<T, S> f) {
      this.Underlying = Underlying;
      this.f = f;
    }
    public S this[int index] { get { return f(Underlying[index]); } }
    public int Count { get { return Underlying.Count; } }
  }
  public class AList<T, S> : ISimpleList<S> {
    readonly T[] Underlying;
    readonly Func<T, S> f;
    readonly int Start;
    readonly int Count0;
    public AList(T[] Underlying, int Start, int Count, Func<T, S> f) {
      this.Underlying = Underlying;
      this.f = f;
      this.Start = Start;
      this.Count0 = Count;
    }
    public AList(T[] Underlying, Func<T, S> f) : this(Underlying, 0, Underlying.Length, f) { }
    public S this[int index] { get { return f(Underlying[Start + index]); } }
    public int Count { get { return Count0; } }
  }
  /*
  public class FuncBl<A, B> {
    public Func<A, B> F { get; internal set; }
    public static implicit operator Func<A, B>(FuncBl<A, B> F) { return F.F; }
    public static implicit operator FuncBl<A, B>(Func<A, B> F) { return new FuncBl<A, B>() { F = F }; }

    public FuncBl<C, B> Circle<C>(Func<C, A> G) {
      return new FuncBl<C, B>() { F = x => F(G(x)) };
    }
    public static Func<B, B> operator *(FuncBl<A, B> opA, Func<B, A> opB) {
      return opA.Circle(opB);
    }
    public static Func<A, A> operator *(Func<B, A> opB, FuncBl<A, B> opA) {
      return opB.Bl().Circle(opA.F);
    }
  }
  public class FuncBl<A, B, C> : FuncBl<A, Func<B, C>> {
    public new Func<A, B, C> F {
      get {
        return (a,b) => base.F(a)(b);
      }
      internal set {
        base.F = (a) => (b) => value(a, b);
      }
    }
    public static implicit operator Func<A, B,C>(FuncBl<A, B,C> F) { return F.F; }
    public static implicit operator FuncBl<A, B,C>(Func<A, B,C> F) { return new FuncBl<A, B,C>() { F = F }; }
  }
  */


  public static class Extensions {
    //public static FuncBl<A, B> Bl<A, B>(this Func<A, B> F) { return new FuncBl<A, B>() { F = F }; }
    //public static FuncBl<A, B, C> Bl<A, B, C>(this Func<A, B, C> F) { return new FuncBl<A, B, C>() { F = F }; }
    private static void GetAllProperties(this Type Type, List<PropertyInfo> Properties) {
      foreach (var p in Type.GetProperties()) Properties.Add(p);
      foreach (var t in Type.GetInterfaces()) GetAllProperties(t, Properties);
    }
    public static IList<PropertyInfo> GetAllProperties(this Type Type) {
      var list = new List<PropertyInfo>();
      Type.GetAllProperties(list);
      return list;
    }
    public static void Trace(string str) {

    }
    public static Action Concat(this Action a0, params Action[] a1) {
      return () => {
        a0();
        foreach (var a in a1) a();
      };
    }
    public static Action<T> Concat<T>(this Action<T> a0, params Action<T>[] a1) {
      return (t) => {
        a0(t);
        foreach (var a in a1) a(t);
      };
    }



    public static void Assert(this bool value) {
      if (!value) {
        Debug.Assert(false);
      }
    }
    /*
    public static void CheckInit(this object value) {
    }
    public static void CheckInit(this Type value) {
      value.TypeInitializer.Invoke(new object[] {});
    }
     */
    public static void CheckInit<T>() {
      var m = typeof(T).GetMethod("CheckInit", new Type[] { });
      if (m != null) m.Invoke(null, new object[] { });
    }

    public static T Convert<T>(this string str) {
      return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFrom(str);
    }

    public static Uri MakePackUri(this string relativeFile, Type From) {
      StringBuilder uriString = new StringBuilder();
      uriString.Append("pack://application:,,,");
      uriString.Append("/" + AssemblyShortName(From) + ";component/" + relativeFile);
      return new Uri(uriString.ToString(), UriKind.RelativeOrAbsolute);
    }
    private static string AssemblyShortName(Type From) {
      Assembly a = From.Assembly;

      // Pull out the short name.
      return a.ToString().Split(',')[0];
    }

    public static double Clamp(this double at, double min, double max) {
      if (at < min) at = min;
      if (at > max) at = max;
      return at;
    }
    public static float Clamp(this float at, float min, float max) {
      if (at < min) at = min;
      if (at > max) at = max;
      return at;
    }
    public static int Clamp(this int at, int min, int max) {
      if (at < min) at = min;
      if (at > max) at = max;
      return at;
    }
    public static bool IsNan(this double d) {
      return double.IsNaN(d);
    }

    public static double Square(this double t) { return t * t; }
    public static double Pow(this double t, double e) { return Math.Pow(t, e); }
    public static double Sqrt(this double t) { return Math.Sqrt(t); }



    public static T[] ToArray<T>(this MockArray<int, T> array) {
      var ret = new T[array.Count];
      for (int i = 0; i < array.Count; i++)
        ret[i] = array.Get(i);
      return ret;
    }
  }
}

namespace Bling.Mixins {
  using Bling.Util;
  using System.Reflection;
  public interface IHasMixin {
    //void DoInit();
  }
  public interface IHasMixin<TARGET> : IHasMixin where TARGET : IHasMixin<TARGET> {
    MixinManager<TARGET> Mixins { get; }
  }
  public interface IHasMixin<TARGET, MIXIN> : IHasMixin<TARGET>
    where TARGET : IHasMixin<TARGET, MIXIN>
    where MIXIN : IMixin<TARGET, MIXIN> {
  }
  public interface IMixin { }
  public interface IMixin<TARGET> : IMixin where TARGET : IHasMixin<TARGET> {
    void DoInit(TARGET block);
  }
  public interface IMixin<TARGET, MIXIN> : IMixin<TARGET>
    where TARGET : IHasMixin<TARGET, MIXIN>
    where MIXIN : IMixin<TARGET, MIXIN> {
  }
  public abstract class Mixin : IMixin {
    internal Mixin() { }
  }
  public abstract class Mixin<TARGET> : Mixin, IMixin<TARGET> where TARGET : IHasMixin<TARGET> {
    public readonly TARGET Target;
    protected internal void ForceInit<MIXIN>(TARGET Target) where MIXIN : Mixin<TARGET> {
      Target.Mixins.ForceInit(Target, typeof(MIXIN));
    }
    public abstract void DoInit(TARGET Target);
    internal Mixin(TARGET Target) { this.Target = Target; }
  }
  public abstract class Mixin<TARGET, MIXIN> : Mixin<TARGET>, IMixin<TARGET, MIXIN>
    where MIXIN : Mixin<TARGET, MIXIN>
    where TARGET : IHasMixin<TARGET, MIXIN> {
    public Mixin(TARGET Target) : base(Target) { }
  }
  public class MixinManager<TARGET> where TARGET : IHasMixin<TARGET> {
    private static readonly Dictionary<Type, ConstructorInfo> All = new Dictionary<Type, ConstructorInfo>();
    private readonly Dictionary<Type, IMixin<TARGET>> Instances = new Dictionary<Type, IMixin<TARGET>>();
    private readonly HashSet<Type> Initialized = new HashSet<Type>();
    private static bool IsInit = false;
    public IEnumerable<IMixin<TARGET>> Mixins { get { return Instances.Values; } }

    private static void InitMixinManager() {
      if (IsInit) return;
      IsInit = true;
      
      foreach (var intf in typeof(TARGET).GetInterfaces()) {

        if (intf.Name != typeof(IHasMixin<,>).Name) continue;
        if (intf.GetGenericTypeDefinition() != typeof(IHasMixin<,>)) continue;
        //ype t0 = typeof(IHasMixin<TARGET>);
        //intf.IsSubclassOf(typeof(IHasMixin<TARGET>)).Assert();
        var extension = intf.GetGenericArguments()[1];
        extension.IsSubclassOf(typeof(Mixin<TARGET>)).Assert();
        var c = extension.GetConstructor(new Type[] { typeof(TARGET) });
        (c != null).Assert();
        All[extension] = c; // (Extension<TARGET>)c.Invoke(new object[] { });
      }
    }
    public MIXIN Get<MIXIN>() where MIXIN : Mixin<TARGET> {
      return (MIXIN)Instances[typeof(MIXIN)];
    }
    public void ForceInit<MIXIN>(TARGET Target) where MIXIN : Mixin<TARGET> {
      Get<MIXIN>().ForceInit<MIXIN>(Target);
    }
    public MixinManager(TARGET Target) {
      InitMixinManager();
      foreach (var e in All) {
        Instances[e.Key] = (IMixin<TARGET>) e.Value.Invoke(new object[] { Target });
      }
    }
    public void ForceInit(TARGET Target, Type type) {
      if (Initialized.Contains(type) || !All.ContainsKey(type)) return;
      Initialized.Add(type);
      Instances[type].DoInit(Target);
    }
    public virtual void DoInit(TARGET Target) {
      foreach (var e in All) ForceInit(Target, e.Key);
    }
  }
}
