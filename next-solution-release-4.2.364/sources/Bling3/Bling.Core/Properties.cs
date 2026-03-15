using System;
using System.Collections.Generic;
using Bling.DSL;
using Bling.Util;
using Bling.Core;

namespace Bling.DSL {
  public partial interface InitBindableExpr<T> : IExpr<T> {
    bool InitBindWith(Expr<T> value);
  }
  public partial interface ILinkableExpr<T> : IExpr<T> {
    bool LinkBindWith(Expr<T> value, Expr<bool> Guard);
  }
}
namespace Bling.Core {
  public abstract partial class Brand<BRAND> : Brand, IBrand<BRAND>, IConditionTarget<BRAND> where BRAND : Brand<BRAND> {
    public abstract BRAND Init { get; set; }
  }
  public abstract partial class Brand<T, BRAND> : Brand<BRAND>, IBrandT<T>, IExtends<BRAND,ObjectBl>, IBrand<T, BRAND> where BRAND : Brand<T, BRAND> {
    public override BRAND Init {
      get { return this; }
      set {
        if (!Underlying.Solve(new InitAssign(), value.Underlying)) throw new NotSupportedException();
      }
    }
    public class LinkSet {
      internal BRAND Underlying;
      public BRAND this[BoolBl Guard] {
        get { return Underlying; }
        set {
          if (!Underlying.Underlying.Solve(new Bling.Properties.LinkAssign() { Guard = Guard }, value.Underlying)) throw new NotSupportedException();
        }
      }
      public BRAND To { get { return Underlying; } set { this[true] = value; } }
    }
    public LinkSet Link { get { return new LinkSet() { Underlying = this }; } }
    private class InitAssign : IAssign {
      public IAssign Copy() { return this; }
      public bool Accept<S>(Expr<S> LHS, Expr<S> RHS) {
        return (LHS is InitBindableExpr<S>) &&
          ((InitBindableExpr<S>)LHS).InitBindWith(RHS);
      }
    }
  }
}
namespace Bling.Properties {

  public class LinkAssign : IAssign {
    public BoolBl Guard = true;
    public IAssign Copy() { return this; }
    public bool Accept<S>(Expr<S> LHS, Expr<S> RHS) {
      return (LHS is ILinkableExpr<S>) &&
        ((ILinkableExpr<S>)LHS).LinkBindWith(RHS, Guard);
    }
  }

  public interface IPropertyContainer {}
  public interface IPropertyContainer<CONTAINER> : IPropertyContainer where CONTAINER : IPropertyContainer<CONTAINER> {
    CONTAINER Translate<EVAL>(TranslateEval<EVAL> txt) where EVAL : Eval<EVAL>;
    bool Bind<T>(PropertyExpr<CONTAINER,T> Property, Expr<T> RHS);
    bool Init<T>(PropertyExpr<CONTAINER, T> Property, Expr<T> RHS);
    bool Link<T>(PropertyExpr<CONTAINER, T> Property, Expr<T> RHS, Expr<bool> Guard);
    bool SetNow<T>(PropertyExpr<CONTAINER, T> Property, T value);
    TDelegate SetNow<TDelegate,T>(PropertyExpr<CONTAINER, T> Property, Expr<T> value, IParameterExpr[] param);
    Eval<EVAL>.Info<T> Eval<EVAL, T>(PropertyExpr<CONTAINER, T> Property, Eval<EVAL> txt) where EVAL : Eval<EVAL>;
  }
  public interface IBasePropertyExpr {
    Property Property { get; }
    IPropertyContainer Container { get; }
  }
  public interface IPropertyExpr : IBasePropertyExpr, IBindableExpr { }
  public interface IPropertyExpr<CONTAINER> : IBasePropertyExpr where CONTAINER : IPropertyContainer<CONTAINER> {
    new Property<CONTAINER> Property { get; }
    new CONTAINER Container { get; }
  }
  public struct BasePropertyExpr<CONTAINER> : IBasePropertyExpr where CONTAINER : IPropertyContainer {
    public Property Property { get; set; }
    public CONTAINER ActualContainer { get; set; }
    public IPropertyContainer Container { get { return ActualContainer; } }
    public static BasePropertyExpr<CONTAINER> Cv<CONTAINER0>(IPropertyExpr<CONTAINER0> p) where CONTAINER0 : CONTAINER, IPropertyContainer<CONTAINER0> {
      return new BasePropertyExpr<CONTAINER> { Property = p.Property, ActualContainer = p.Container };
    }
    public static BasePropertyExpr<CONTAINER> Cv<T>(IPropertyExprT<T> p) {
      return new BasePropertyExpr<CONTAINER> { Property = p.Property, ActualContainer = (CONTAINER) p.Container };
    }
    public override string ToString() {
      return ActualContainer.ToString() + "." + Property.Name;
    }
  }

  public interface IPropertyExprT<T> : IPropertyExpr, InitBindableExpr<T> { }

  public interface IPropertyExpr<CONTAINER, T> : IPropertyExprT<T>, IPropertyExpr<CONTAINER>, IExpr<T> where CONTAINER : IPropertyContainer<CONTAINER> { }

  public interface RevealPropertyExpr<R> {
    R Visit<CONTAINER, T>(PropertyExpr<CONTAINER, T> Property) where CONTAINER : IPropertyContainer<CONTAINER>;
  }


  public abstract class PropertyExpr<T> : Expr<T>, IPropertyExprT<T>, IBindableExpr<T>, InitBindableExpr<T>, ICanAssignNowExpr<T>, ILinkableExpr<T> {
    public Property Property { get { return BaseProperty; } }
    public IPropertyContainer Container { get { return BaseContainer; } }
    protected abstract Property BaseProperty { get; }
    protected abstract IPropertyContainer BaseContainer { get; }
    public abstract bool BindWith(Expr<T> value);
    public abstract bool TwoWayBindWith(Expr<T> value);
    public bool BindWith(Expr value) { return BindWith((Expr<T>)value); }
    public abstract bool InitBindWith(Expr<T> RHS);
    public abstract bool LinkBindWith(Expr<T> value, Expr<bool> Guard);
    public abstract TDelegate SetNow<TDelegate>(Expr<T> value, params IParameterExpr[] param);
    public abstract bool SetNow(T value);
    public abstract R Accept<R>(RevealPropertyExpr<R> Visitor);
  }
  public class PropertyExpr<CONTAINER, T> : PropertyExpr<T>, IPropertyExpr<CONTAINER, T> where CONTAINER : IPropertyContainer<CONTAINER> {
    public new CONTAINER Container { get { return Container0; } }
    public new Property<CONTAINER> Property { get { return Property0; } }
    private readonly CONTAINER Container0;
    private readonly Property<CONTAINER> Property0;
    public PropertyExpr(CONTAINER Container, Property<CONTAINER> Property) { this.Container0 = Container; this.Property0 = Property; }
    protected override Property BaseProperty { get { return Property; } }
    protected override IPropertyContainer BaseContainer { get { return Container; } }
        protected override string ToString1() { return Container.ToString() + "." + Property.Name; }
    protected override int GetHashCode0() { return Container.GetHashCode() + Property.GetHashCode(); }
    protected override bool Equals0(Expr<T> obj) {
      if (obj is PropertyExpr<CONTAINER, T>)
        return ((PropertyExpr<CONTAINER, T>)obj).Container.Equals(Container) &&
               ((PropertyExpr<CONTAINER, T>)obj).Property.Equals(Property);
      return base.Equals0(obj);
    }
    public override bool BindWith(DSL.Expr<T> value) { return Container.Bind<T>(this, value); }
    public override bool TwoWayBindWith(DSL.Expr<T> value) { return false; }
    public override bool InitBindWith(DSL.Expr<T> RHS) { return Container.Init<T>(this, RHS); }
    public override bool LinkBindWith(DSL.Expr<T> value, DSL.Expr<bool> Guard) { return Container.Link<T>(this, value, Guard); }

    public override TDelegate SetNow<TDelegate>(Expr<T> value, params IParameterExpr[] param) { return Container.SetNow<TDelegate,T>(this, value, param); }
    public override bool SetNow(T value) { return Container.SetNow(this, value); }
    protected override Eval<EVAL>.Info<T> Eval<EVAL>(Eval<EVAL> txt) {
      if (txt is TranslateEval<EVAL>) {
        return ((TranslateEval<EVAL>) txt).NoTranslation(new PropertyExpr<CONTAINER, T>(Container.Translate((TranslateEval<EVAL>) txt), Property));
      } else return Container.Eval<EVAL, T>(this, txt); }
    public override R Accept<R>(RevealPropertyExpr<R> Visitor) { return Visitor.Visit(this); }

    protected override Expr<T> Derivative1(Dictionary<Expr, Expr> Map) {
      return new Constant<T>(default(T)); // treat as constant
    }

  }
  public interface IProperty {
    string Name { get; }
    Type TypeOfT { get; }
  }
  public abstract class Property : IProperty {
    public string Name { get; private set; }
    public Property(string Name) { this.Name = Name; }
    public abstract Type TypeOfT { get; }
  }
  public interface IProperty<CONTAINER>  : IProperty where CONTAINER : IPropertyContainer<CONTAINER> {
    Brand this[CONTAINER Container] { get; }
  }
  public abstract class Property<CONTAINER> : Property, IProperty<CONTAINER> where CONTAINER : IPropertyContainer<CONTAINER> {
    public Property(string Name) : base(Name) { }
    public Brand this[CONTAINER Container] { get { return Get(Container); } }
    protected abstract Brand Get(CONTAINER Container);
    //public static Func<Property<CONTAINER>,object> InitMetaInfo { protected get; set; }
    static Property() {
      //InitMetaInfo = (p) => null;
    }
    //public abstract object MetaInfo { get; }
  }
  public interface IPropertyWithBrand<BRAND> : IProperty where BRAND : Brand<BRAND> {
  }

  public abstract class Property<CONTAINER, BRAND> : Property<CONTAINER>, IPropertyWithBrand<BRAND>
    where BRAND : Brand<BRAND>
    where CONTAINER : IPropertyContainer<CONTAINER> {
    public Property(string Name) : base(Name) {}
    public new BRAND this[CONTAINER Container] { get { return Brand<BRAND>.RevealType(new MakeExpr() { Container = Container, Property = this }); } }
    protected override Brand Get(CONTAINER Container) { return this[Container]; }
    private class MakeExpr : BrandTypeRevealS<BRAND> {
      internal CONTAINER Container;
      internal Property<CONTAINER, BRAND> Property;
      public BRAND Visit<T0, BRAND0>() where BRAND0 : Brand<T0, BRAND0> {
        (typeof(BRAND) == typeof(BRAND0)).Assert();
        BRAND0 ret = Brand<BRAND0>.ToBrand(new PropertyExpr<CONTAINER,T0>(Container, Property));
        (ret.GetType() == typeof(BRAND0)).Assert();
        return (BRAND)(object)ret;
      }
    }
    public override Type TypeOfT { get { return Brand<BRAND>.TypeOfT; } }

    public override string ToString() {
      return typeof(CONTAINER).Name + "." + Name + " : " + typeof(BRAND).Name;
    }

  }
  public interface VisitConstraint {
    void Visit<CONTAINER, T>(PropertyConstraint<CONTAINER, T> Constraint) where CONTAINER : IPropertyContainer<CONTAINER>;
    void Visit<CONTAINER, T>(LinkConstraint<CONTAINER, T> Constraint) where CONTAINER : IPropertyContainer<CONTAINER>;
  }
  public interface IPropertyConstraint {
    IPropertyExpr BaseLHS { get; }
    Expr BaseRHS { get; }
    LoopIndexExpr[] Context { get; }
    void Accept(VisitConstraint Visitor);
    bool IsStep { get; set; }
  }
  public class PropertyConstraint<CONTAINER, T> : IPropertyConstraint where CONTAINER : IPropertyContainer<CONTAINER> {
    public bool IsStep { get; set; }
    public LoopIndexExpr[] Context { get; set; }
    public PropertyExpr<CONTAINER, T> LHS;
    public IPropertyExpr BaseLHS { get { return LHS; } }
    public Expr<T> RHS;
    public Expr BaseRHS { get { return RHS; } }
    public void Accept(VisitConstraint Visitor) { Visitor.Visit(this); }
  }
  public interface ILinkConstraint : IPropertyConstraint { Expr<bool> Guard { get; } }
  public class LinkConstraint<CONTAINER, T> : ILinkConstraint where CONTAINER : IPropertyContainer<CONTAINER> {
    public PropertyExpr<CONTAINER, T> LHS;
    public bool IsStep { get { return false; } set { throw new NotSupportedException(); } }
    public IPropertyExpr BaseLHS { get { return LHS; } }
    public Expr<T> RHS;
    public Expr BaseRHS { get { return RHS; } }
    public LoopIndexExpr[] Context { get; set; }
    public Expr<bool> Guard { get; internal set; }
    public void Accept(VisitConstraint Visitor) { Visitor.Visit(this); }
  }


}
namespace Bling.Properties2 {
  // use reflection
  public abstract class PropertyManager {
    public abstract bool Bind<T>(string Name, Expr<T> RHS);
    public abstract Expr<T> Read<T>(string Name);

    public PROPERTIES Generate<PROPERTIES>() {
      throw new NotImplementedException();
    }

  }
  




}
namespace Bling.Gen {
  using System.Reflection.Emit;
  using System.Reflection;
  using linq = Microsoft.Linq.Expressions;
  public abstract class Generator<T> {
    public abstract string Name { get; }
    internal readonly Dictionary<Type, Func<T, object>> Constructors = new Dictionary<Type, Func<T, object>>();
    private readonly ModuleBuilder Module;

    public Generator() {
      AppDomain Domain = System.Threading.Thread.GetDomain();
      AssemblyBuilder AsmBuilder = Domain.DefineDynamicAssembly(new AssemblyName() { Name = Name }, AssemblyBuilderAccess.RunAndSave);
      Module = AsmBuilder.DefineDynamicModule(Name + "Module", true);
    }
    public struct PropertyCode {
      public Func<T,object> Get;
      public Action<T,object> Set;
    }
    private Func<T, object> Create(Type type) {
      (type.IsInterface).Assert();
      TypeBuilder Bld = Module.DefineType(type.Name + "Impl", TypeAttributes.Public);
      Bld.AddInterfaceImplementation(type);
      var MainFld = Bld.DefineField("MainT", typeof(T), FieldAttributes.Private | FieldAttributes.InitOnly);
      {
        var Constr = Bld.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new Type[] { typeof(T) });
        var il = Constr.GetILGenerator();
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Call, typeof(object).GetConstructor(new Type[0]));
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldarg_1);
        il.Emit(OpCodes.Stfld, MainFld);
        il.Emit(OpCodes.Ret);
      }
      foreach (var p in type.GetAllProperties()) {
        var Prop = Bld.DefineProperty(p.Name, p.Attributes, p.PropertyType, new Type[0]);
        if (p.CanRead) { // define a getter.
          // define a static method
          var Fld = Bld.DefineField(p.Name + "Get0", typeof(Func<T, object>), FieldAttributes.Static | FieldAttributes.Public);
          var Invoke = typeof(Func<T, object>).GetMethod("Invoke", new Type[] { typeof(T) });
          (Invoke.ReturnType == typeof(object)).Assert();
          var Getter = Bld.DefineMethod("get_" + p.Name, 
            MethodAttributes.Public |      MethodAttributes.HideBySig | 
            MethodAttributes.SpecialName | MethodAttributes.Virtual, Prop.PropertyType, null);
          {
            var il = Getter.GetILGenerator();
            il.Emit(OpCodes.Ldsfld, Fld);
            il.Emit(OpCodes.Ldarg_0); // load this
            il.Emit(OpCodes.Ldfld, MainFld); // get T
            // now call invoke.
            il.Emit(OpCodes.Callvirt, Invoke);
            if (Prop.PropertyType.IsValueType) {
              il.Emit(OpCodes.Unbox, Prop.PropertyType);
            } else {
              il.Emit(OpCodes.Castclass, Prop.PropertyType);
            }
            il.Emit(OpCodes.Ret);
          }
          Prop.SetGetMethod(Getter);
          //Bld.DefineMethodOverride(Getter, p.GetAccessors()[0]);
        }
        if (p.CanWrite) {
          var Fld = Bld.DefineField(p.Name + "Set0", typeof(Action<T, object>), FieldAttributes.Static | FieldAttributes.Public);
          var Invoke = typeof(Action<T, object>).GetMethod("Invoke", new Type[] { typeof(T), typeof(object) });
          (Invoke.ReturnType == typeof(void)).Assert();
          var Setter = Bld.DefineMethod("set_" + p.Name, MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.Virtual, 
            null, new Type[1] { Prop.PropertyType });
          {
            var il = Setter.GetILGenerator();
            il.Emit(OpCodes.Ldsfld, Fld);
            il.Emit(OpCodes.Ldarg_0); // load this
            il.Emit(OpCodes.Ldfld, MainFld); // get T
            il.Emit(OpCodes.Ldarg_1);
            if (Prop.PropertyType.IsValueType) 
              il.Emit(OpCodes.Box, Prop.PropertyType);
            // now call invoke.
            il.Emit(OpCodes.Callvirt, Invoke);
            il.Emit(OpCodes.Ret);
          }
          Prop.SetGetMethod(Setter);
        }
      }
      // now we are done.
      var TypeS = Bld.CreateType();
      foreach (var p in type.GetAllProperties()) {
        var info = Property(p);
        if (p.CanRead) 
          TypeS.GetField(p.Name + "Get0").SetValue(null, info.Get);
        if (p.CanWrite)
          TypeS.GetField(p.Name + "Set0").SetValue(null, info.Set);
      }
      var Constr0 = TypeS.GetConstructor(new Type[1] { typeof(T) });
      {
        var param = linq.Expression.Parameter(typeof(T), "MainT");
        var create = linq.Expression.New(Constr0, param);
        var lambda = linq.Expression.Lambda<Microsoft.Func<T, object>>(create, param);
        var mf = lambda.Compile();
        return t => mf(t);
      }
    }
    public abstract PropertyCode Property(PropertyInfo Property);
    public Func<T, S> Generate<S>() {
      if (!Constructors.ContainsKey(typeof(S)))
        Constructors[typeof(S)] = Create(typeof(S));
      var f = Constructors[typeof(S)];
      return t => (S)f(t);
    }
  }
}


namespace Bling.Properties.Lightweight {

}
