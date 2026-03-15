using System;
using System.Collections.Generic;
using Bling.DSL;
using Bling.Util;
using Bling.Core;
using System.Threading;
using Bling.Properties;
using Bling.Linq;
using linq = Microsoft.Linq.Expressions;

namespace Bling.Continuous {
  // just in case ....
  public interface BindingBl { 
    bool Invalidate<CLIENT>(CLIENT Client, Property<CLIENT> Property) where CLIENT : DependencyObjectBl<CLIENT>;
  }
  public abstract class DependencyObjectBl : IPropertyContainer {
  }
  public abstract class DependencyObjectBl<CLIENT> : DependencyObjectBl, IPropertyContainer<CLIENT> where CLIENT : DependencyObjectBl<CLIENT> {
    public interface IPropertyA : IProperty<CLIENT> {
      int Id { get; }
    }


    public class PropertyA<BRAND> : Properties.Property<CLIENT, BRAND>, IPropertyA where BRAND : Brand<BRAND> {
      public int Id { get; private set; }
      public PropertyA(string name) : base(name) {
        if (InstanceCreated) throw new Exception("Too late");
        var Id = Properties.Count;
        Properties[Id] = this;
      }
    }
    public static PropertyA<BRAND> NewProperty<BRAND>(string name) where BRAND : Brand<BRAND> { return new PropertyA<BRAND>(name); }



    private readonly Dictionary<Property<CLIENT>, PropertyBinding> InBound = new Dictionary<Property<CLIENT>, PropertyBinding>();
    private readonly Dictionary<Property<CLIENT>, List<WeakReference>> OutBound = new Dictionary<Property<CLIENT>, List<WeakReference>>();
    private static readonly Dictionary<Property<CLIENT>, object> OnChangeMap = new Dictionary<Property<CLIENT>, object>();
    private static readonly Dictionary<int, Property<CLIENT>> Properties = new Dictionary<int, Property<CLIENT>>();
    private readonly object[] PropertyValues;
    private static bool InstanceCreated = false;
    static DependencyObjectBl() {
    }
    public static void OnChange<T, BRAND>(Property<CLIENT, BRAND> Property, Action<CLIENT, Property<CLIENT>, T, T> action) where BRAND : Brand<T, BRAND> {
      OnChangeMap[Property] = action;
    }
    public DependencyObjectBl() {
      InstanceCreated = true;
      PropertyValues = new object[Properties.Count];
    }
    private abstract class PropertyBinding : BindingBl {
      public CLIENT Outer;
      public Property<CLIENT> Property;
      public int PID;
      public bool IsActive { get; set; }
      public abstract bool Invalidate<CLIENT0>(CLIENT0 client0, Property<CLIENT0> Property0) where CLIENT0 : DependencyObjectBl<CLIENT0>;
    }

    private class PropertyBinding<T> : PropertyBinding {
      public Func<T> Compute;
      public override bool Invalidate<CLIENT0>(CLIENT0 client0, Property<CLIENT0> Property0) {
        if (!IsActive) return false;
        // changed.
        var newValue = Compute();
        var oldValue = (T)Outer.PropertyValues[PID];
        if (newValue.Equals(oldValue)) return true;
        Outer.PropertyValues[PID] = newValue;
        Outer.Changed<T>(Property, oldValue, newValue);
        return true;
      }
    }
    protected bool Changed<T>(Property<CLIENT> Property, T OldValue, T NewValue) {
      var ret = false;
      if (OnChangeMap.ContainsKey(Property)) {
        var action = (Action<CLIENT, Property<CLIENT>, T, T>)OnChangeMap[Property];
        action((CLIENT)this, Property, OldValue, NewValue);
        ret = true;
      }
      if (!OutBound.ContainsKey(Property)) return ret;
      var list = OutBound[Property];
      int i = 0;
      while (i < list.Count) {
        var r = list[i];
        if (!r.IsAlive) { list.RemoveAt(i); continue; }
        var isActive = ((BindingBl)r.Target).Invalidate<CLIENT>((CLIENT) this, Property);
        if (!isActive) { list.RemoveAt(i); continue; }
        ret = true;
        i++;
      }
      return ret;
    }
    public void Subscribe(Property<CLIENT> Property, BindingBl binding) {
      if (!OutBound.ContainsKey(Property)) OutBound[Property] = new List<WeakReference>();
      OutBound[Property].Add(new WeakReference(binding));
    }
    public CLIENT Translate<EVAL>(TranslateEval<EVAL> txt) where EVAL : Eval<EVAL> { return (CLIENT) this; }

    private abstract class BaseBindLinqEval<EVAL> : LinqEval<EVAL> where EVAL : Eval<EVAL> {
      public abstract PropertyBinding BaseTarget { get; }
    }
    private class BindLinqEval<T> : BaseBindLinqEval<BindLinqEval<T>> {
      public PropertyBinding<T> Target;
      public override DependencyObjectBl<CLIENT>.PropertyBinding BaseTarget {
        get { return Target; }
      }
    }
    public bool Bind<T>(PropertyExpr<CLIENT, T> Expr, Expr<T> RHS) {
      //RHS.CurrentValue;
      var binding = new PropertyBinding<T>() {
        Outer = (CLIENT)this,
        Property = Expr.Property,
        PID = ((IPropertyA) Expr.Property).Id,
      };
      // walk RHS and install listeners, also compute function.
      binding.Compute = (new BindLinqEval<T>() { Target = binding }).Eval<T>(RHS);

      if (InBound.ContainsKey(Expr.Property)) InBound[Expr.Property].IsActive = false;
      InBound[Expr.Property] = binding;
      binding.Invalidate<CLIENT>(null, null);
      return true;
    }
    public Eval<EVAL>.Info<T> Eval<EVAL, T>(PropertyExpr<CLIENT, T> Expr, Eval<EVAL> txt) where EVAL : Eval<EVAL> {
      if (txt is BaseBindLinqEval<EVAL>) {
        Subscribe(Expr.Property, ((BaseBindLinqEval<EVAL>)txt).BaseTarget);
      }
      var PID = ((IPropertyA) Expr.Property).Id;

      if (txt is LinqEval<EVAL>) {
        var arraySlot = linq.Expression.ArrayAccess(linq.Expression.Constant(PropertyValues, typeof(object[])), 
                                                    linq.Expression.Constant(PID, typeof(int)));
        return new LinqEval<EVAL>.MyInfo<T>() { Value = linq.Expression.Convert(arraySlot, typeof(T)), };
      }
      throw new NotImplementedException();
    }
    public bool SetNow<T>(PropertyExpr<CLIENT, T> Expr, T value) {
      var PID = ((IPropertyA) Expr.Property).Id;
      var OldValue = (T) PropertyValues[PID];
      if (OldValue.Equals(value)) return true;
      PropertyValues[PID] = value;
      Changed<T>(Expr.Property, OldValue, value);
      return true;
    }
    public TDelegate SetNow<TDelegate, T>(PropertyExpr<CLIENT, T> Expr, Expr<T> RHS, IParameterExpr[] Args) {
      var PID = ((IPropertyA) Expr.Property).Id;
      var Property = Expr.Property;
      return (new DefaultLinqEval()).DoSetNow<T, TDelegate>((RHS0, txt0) => {
        var arraySlot = linq.Expression.ArrayAccess(linq.Expression.Constant(PropertyValues, typeof(object[])), linq.Expression.Constant(PID, typeof(int)));
        var oldValue = linq.Expression.Convert(arraySlot, typeof(T));
        linq.Expression newValue = RHS0;
        var Equals = linq.Expression.Equal(newValue, oldValue);
        var NotEquals = linq.Expression.Not(Equals);
        var Assign = linq.Expression.Assign(arraySlot, linq.Expression.Convert(newValue, typeof(object)));
        // notification. 
        var DoChange = linq.Expression.Call(
          linq.Expression.Constant(this, typeof(DependencyObjectBl<CLIENT>)),
          "Changed", new Type[] { typeof(T) },
          linq.Expression.Constant(Property, typeof(Property<CLIENT>)),
          oldValue, newValue);
        return linq.Expression.IfThen(NotEquals, linq.Expression.Block(Assign,DoChange));
      }, RHS, Args);
    }
    // not applicable, maybe...
    public bool Init<T>(PropertyExpr<CLIENT, T> Property, Expr<T> RHS) { throw new NotImplementedException(); }
    public bool Link<T>(PropertyExpr<CLIENT, T> Property, Expr<T> RHS, Expr<bool> Guard) { throw new NotImplementedException(); }
  }
}