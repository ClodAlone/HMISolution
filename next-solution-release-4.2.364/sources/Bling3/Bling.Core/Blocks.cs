using System;
using System.Collections.Generic;
using Bling.DSL;
using Bling.Util;
using Bling.Core;
using System.Linq;
using linq = Microsoft.Linq.Expressions;
namespace Bling.Blocks {
  using Bling.Properties;
  using Bling.Mixins;

  public static class BlockExtensions {
    public static BasePropertyExpr<IBlockish> Normalize(this BasePropertyExpr<IBlockish> self)  {
      return new BasePropertyExpr<IBlockish>() {
        ActualContainer = self.ActualContainer.BaseBaseNormalize,
        Property = self.Property,
      };
    }
    public static PropertyExpr<CONTAINER, T> Normalize<CONTAINER, T>(this PropertyExpr<CONTAINER, T> self) where CONTAINER : IBlockish<CONTAINER> {
      return new PropertyExpr<CONTAINER, T>(self.Container.Normalize, self.Property);
    }
    public static int AllocSize(this BasePropertyExpr<IBlockish> self) {
      Block block = self.ActualContainer.ParentBlock;
      var n = 1;
      while (block != null) {
        n = n * block.Count;
        block = block.ParentBlock;
      }
      return n;
    }
    internal static BasePropertyExpr<IBlockish> Cv<CONTAINER>(this IPropertyExpr<CONTAINER> self) where CONTAINER : IBlockish<CONTAINER> {
      return BasePropertyExpr<IBlockish>.Cv(self);
    }
    public static IntBl UseIndex<CONTAINER>(this IPropertyExpr<CONTAINER> self) where CONTAINER : IBlockish<CONTAINER> {
      return self.Cv().UseIndex();
    }
    public static IntBl UseIndex(this BasePropertyExpr<IBlockish> self) {
      IntBl UseIndex = 0;
      IBlockish block = self.ActualContainer;
      IntBl Dim = 1;
      while (true) {
        UseIndex += block.Index * Dim;
        if (block.ParentBlock == null) break;
        Dim *= block.ParentBlock.Count;
        block = block.ParentBlock;
      }
      return UseIndex;
    }
  }

  // outside configuration. 
  public abstract class BlockManager {
    public abstract Eval<EVAL>.Info<T> Eval<EVAL, CONTAINER, T>(Eval<EVAL> txt, PropertyExpr<CONTAINER,T> Property)
      where CONTAINER : IBlockish<CONTAINER>
      where EVAL : Eval<EVAL>;
    public readonly List<IPropertyConstraint> Relax = new List<IPropertyConstraint>();
    public readonly List<Action> DoInUI = new List<Action>();
    internal protected readonly List<IPropertyConstraint> Inits = new List<IPropertyConstraint>();
    internal protected readonly List<ILinkConstraint> Links = new List<ILinkConstraint>();
    internal protected readonly HashSet<BasePropertyExpr<IBlockish>> Bound = new HashSet<BasePropertyExpr<IBlockish>>();

    public abstract bool TryAssign<T>(ICanAssignExprT<T> LHS, Expr<T> RHS);

    public readonly HashSet<BasePropertyExpr<IBlockish>> Old = new HashSet<BasePropertyExpr<IBlockish>>();
    public readonly HashSet<BasePropertyExpr<IBlockish>> HasStep = new HashSet<BasePropertyExpr<IBlockish>>();
    public readonly List<ITopBlock> TopBlocks = new List<ITopBlock>();

    private T F<T>(T t) where T : IPropertyConstraint {
      Bound.Add(t.BaseNormalizeLHS);
      return t;
    }
    public abstract Expr<T> OldExpression<T>(PropertyExpr<T> Expr);

    internal void AddBind<CONTAINER, T>(PropertyExpr<CONTAINER,T> LHS, Expr<T> RHS) where CONTAINER : IBlockish<CONTAINER> {
      Relax.Add(F(new PropertyConstraint<CONTAINER, T>() { LHS = LHS, RHS = RHS, Context = Block.Stack.Reverse().ToArray() }));
    }
    internal void AddInit<CONTAINER, T>(PropertyExpr<CONTAINER,T> LHS, Expr<T> RHS) where CONTAINER : IBlockish<CONTAINER> {
      Inits.Add(F(new PropertyConstraint<CONTAINER, T>() { LHS = LHS, RHS = RHS, Context = Block.Stack.Reverse().ToArray() }));
    }
    internal void AddLink<CONTAINER, T>(PropertyExpr<CONTAINER,T> LHS, Expr<T> RHS, Expr<bool> Guard) where CONTAINER : IBlockish<CONTAINER> {
      Links.Add(F(new LinkConstraint<CONTAINER, T>() { LHS = LHS, RHS = RHS, Context = Block.Stack.Reverse().ToArray(), Guard = Guard }));
    }

    public abstract bool SetNow<CONTAINER, T>(PropertyExpr<CONTAINER, T> Property, T value) where CONTAINER : IBlockish<CONTAINER>;
    public abstract TDelegate SetNow<CONTAINER, TDelegate, T>(PropertyExpr<CONTAINER, T> Property, Expr<T> value, IParameterExpr[] param) where CONTAINER : IBlockish<CONTAINER>;

    public interface VisitConstraint {
      void Visit<CONTAINER, T>(PropertyConstraint<CONTAINER, T> Constraint) where CONTAINER : IBlockish<CONTAINER>;
      void Visit<CONTAINER, T>(LinkConstraint<CONTAINER, T> Constraint) where CONTAINER : IBlockish<CONTAINER>;
    }

    public interface IPropertyConstraint {
      BasePropertyExpr<IBlockish> BaseLHS { get; }
      BasePropertyExpr<IBlockish> BaseNormalizeLHS { get; }
      Expr BaseRHS { get; }
      LoopIndexExpr[] Context { get; }
      void Accept(VisitConstraint Visitor);
      bool IsStep { get; set; }
    }
    public class PropertyConstraint<CONTAINER, T> : IPropertyConstraint where CONTAINER : IBlockish<CONTAINER> {
      public bool IsStep { get; set; }
      public LoopIndexExpr[] Context { get; set; }
      public PropertyExpr<CONTAINER, T> LHS;
      public BasePropertyExpr<IBlockish> BaseLHS { get { return LHS.Cv(); } }

      public PropertyExpr<CONTAINER,T> NormalizeLHS { get { return LHS.Normalize(); } }
      public BasePropertyExpr<IBlockish> BaseNormalizeLHS { get { return NormalizeLHS.Cv(); } }

      public Expr<T> RHS;
      public Expr BaseRHS { get { return RHS; } }
      public void Accept(VisitConstraint Visitor) { Visitor.Visit(this); }
    }
    public interface ILinkConstraint : IPropertyConstraint { Expr<bool> Guard { get; } }
    public class LinkConstraint<CONTAINER, T> : ILinkConstraint where CONTAINER : IBlockish<CONTAINER> {
      public PropertyExpr<CONTAINER, T> LHS;
      public bool IsStep { get { return false; } set { throw new NotSupportedException(); } }
      public BasePropertyExpr<IBlockish> BaseLHS { get { return LHS.Cv(); } }
      public Expr<T> RHS;
      public Expr BaseRHS { get { return RHS; } }
      public LoopIndexExpr[] Context { get; set; }
      public Expr<bool> Guard { get; internal set; }
      public PropertyExpr<CONTAINER, T> NormalizeLHS { get { return LHS.Normalize(); } }
      public BasePropertyExpr<IBlockish> BaseNormalizeLHS { get { return NormalizeLHS.Cv(); } }
      public void Accept(VisitConstraint Visitor) { Visitor.Visit(this); }
    }

  }
  public interface IBlockish : Properties.IPropertyContainer {
    BlockManager Manager { get; }
    Block ParentBlock { get; }
    IntBl Index { get; }
    IBlockish BaseBaseNormalize { get; }
  }
  public interface IBlockish<CONTAINER> : IBlockish, Properties.IPropertyContainer<CONTAINER> where CONTAINER : IBlockish<CONTAINER> {
    CONTAINER Normalize { get; }
  }


  public interface BlockEval<EVAL> : IEval<EVAL> where EVAL : Eval<EVAL> {
    //Eval<EVAL>.Info<T> Eval<CONTAINER, T>(IPropertyExpr<CONTAINER, T> Property) where CONTAINER : IBlockish<CONTAINER>;
    Eval<EVAL>.Info<int> AllIndex(Block block);
  }


  public interface IBlock : IBlockish { }
  public interface ISubBlock : IBlock { }
  public interface IBlock<BLOCK> : IBlock, IBlockish<BLOCK> where BLOCK : IBlock<BLOCK> { }
  public interface ISubBlock<BLOCK> : IBlock<BLOCK>, ISubBlock where BLOCK : ISubBlock<BLOCK> { }

  public abstract class Block : IBlock, IListBl<Block.ElemA> {
    public interface IElemA {
      ElemA AsElem { get; }
    }
    public class ElemA : Block.BaseElem<ElemA>, Block.IElemA {
      public ElemA AsElem { get { return this; } }
    }

    [ThreadStatic]
    internal static readonly Stack<LoopIndexExpr> Stack = new Stack<LoopIndexExpr>();
    public abstract IntBl Index { get; }
    protected abstract Block BaseNormalize { get; }
    public abstract Block ParentBlock { get; }
    public IBlockish BaseBaseNormalize { get { return BaseNormalize; } }
    protected abstract Block BaseTranslate<EVAL>(TranslateEval<EVAL> txt) where EVAL : Eval<EVAL>;

    public abstract class BaseElem<ELEM> : IBlockish<ELEM> where ELEM : BaseElem<ELEM>, new() {
      public IntBl Index { internal set; get; }
      public Block Block { get { return BaseBlock; } }
      internal Block BaseBlock { set; get; }
      public Block ParentBlock { get { return BaseBlock; } }
      public ELEM Translate<EVAL>(TranslateEval<EVAL> txt) where EVAL : Eval<EVAL> {
        return new ELEM() { BaseBlock = BaseBlock.BaseTranslate(txt), Index = txt.Translate(Index.Underlying) };
      }
      public ELEM Normalize {
        get { return new ELEM() { Index = 0, BaseBlock = BaseBlock.BaseNormalize }; }
      }
      public IBlockish BaseBaseNormalize { get { return Normalize; } }

      public BlockManager Manager { get { return Block.Manager; } }
      public override string ToString() { return Block + "[" + Index + "]"; }
      public override int GetHashCode() { return Block.GetHashCode() + Index.GetHashCode(); }
      public bool Bind<T>(PropertyExpr<ELEM,T> Property, DSL.Expr<T> RHS) {
        Manager.AddBind<ELEM, T>(Property, RHS); return true;
      }
      public bool Init<T>(PropertyExpr<ELEM,T> Property, DSL.Expr<T> RHS) {
        Manager.AddInit<ELEM, T>(Property, RHS); return true;
      }
      public bool Link<T>(PropertyExpr<ELEM,T> Property, DSL.Expr<T> RHS, DSL.Expr<bool> Guard) {
        Manager.AddLink<ELEM, T>(Property, RHS, Guard); return true;
      }
      public bool SetNow<T>(PropertyExpr<ELEM, T> Property, T value) {
        return Manager.SetNow(Property, value);
      }
      public TDelegate SetNow<TDelegate, T>(PropertyExpr<ELEM, T> Property, Expr<T> value, IParameterExpr[] param) {
        return Manager.SetNow<ELEM,TDelegate,T>(Property, value, param);
      }
      public Eval<EVAL>.Info<T> Eval<EVAL, T>(PropertyExpr<ELEM, T> Property, Eval<EVAL> txt) where EVAL : Eval<EVAL> {
        return Manager.Eval<EVAL, ELEM, T>(txt, Property);
      }
      public override bool Equals(object obj) {
        if (obj is IElemA) {
          var elem = ((IElemA)obj).AsElem;
          return Block.Equals(elem.Block) && Index.Equals(elem.Index);
        } else return base.Equals(obj);
      }
      public class NextS {
        internal ELEM At;
        public ELEM this[IntBl Dx] { get { return new ELEM() { Index = (At.Index).WrapUp(Dx, At.Block.Count), BaseBlock = At.BaseBlock }; } }
      }
      public NextS Next { get { return new NextS() { At = (ELEM) this }; } }
      public class PrevS {
        internal ELEM At;
        public ELEM this[IntBl Dx] { get { return new ELEM() { Index = (At.Index).WrapDown(Dx, At.Block.Count), BaseBlock = At.BaseBlock }; } }
      }
      public PrevS Prev { get { return new PrevS() { At = (ELEM) this }; } }
    }

    public ElemA this[IntBl Index] { get { return new ElemA() { Index = Index, BaseBlock = this }; } }
    public Action<ElemA> ForAll { set { ForAll0<ElemA>(value); } }

    protected LoopIndexExpr LoopIndex {
      get { return new LoopIndexExpr(this, Count) { Name = ((char)('i' + Depth)).ToString() }; }
    }
    public abstract int Depth { get; }

    private class BindHelper : BindAssign.Helper {
      internal Block Outer;
      public override bool AcceptBind<T>(Expr<T> LHS, Expr<T> RHS) {
        if (LHS is PropertyExpr<T>) {
          return ((PropertyExpr<T>)LHS).BindWith(RHS);
        } else if (LHS is ICanAssignExprT<T>) {
          return Outer.Manager.TryAssign((ICanAssignExprT<T>) LHS, RHS);
        }
        return false;
      }
    }

    protected void ForAll0<ELEM>(Action<ELEM> Body) where ELEM : BaseElem<ELEM>, new() {
      Push(() => {
        Body(new ELEM() { BaseBlock = this, Index = LoopIndex });
      });
    }


    protected void Push(Action a) {
      Stack.Push(LoopIndex);
      BindAssign.Activate(new BindHelper() { Outer = this }, a);
      var b = Stack.Pop();
      (b.Equals(LoopIndex)).Assert();
    }
    public abstract int Count { get; }
    public abstract BlockManager Manager { get; }

  }
  public abstract class Block<BLOCK> : Block, IBlock<BLOCK>, IListBl<Block<BLOCK>.ElemB> where BLOCK : Block<BLOCK> {
    protected override Block BaseTranslate<EVAL>(TranslateEval<EVAL> txt) { return Translate(txt); }
    public abstract BLOCK Translate<EVAL>(TranslateEval<EVAL> txt) where EVAL : Eval<EVAL>;


    public abstract BLOCK Normalize { get; }
    protected override Block BaseNormalize { get { return Normalize; } }

    public Block() {
    }
    public bool Bind<T>(PropertyExpr<BLOCK, T> Property, DSL.Expr<T> RHS) {
      Manager.AddBind<BLOCK, T>(Property, RHS); return true;
    }
    public bool Init<T>(PropertyExpr<BLOCK,T> Property, DSL.Expr<T> RHS) {
      Manager.AddInit<BLOCK, T>(Property, RHS); return true;
    }
    public bool Link<T>(PropertyExpr<BLOCK,T> Property, DSL.Expr<T> RHS, DSL.Expr<bool> Guard) {
      Manager.AddLink<BLOCK, T>(Property, RHS, Guard); return true;
    }
    public bool SetNow<T>(PropertyExpr<BLOCK, T> Property, T value) {
      return Manager.SetNow(Property, value);
    }
    public TDelegate SetNow<TDelegate, T>(PropertyExpr<BLOCK, T> Property, Expr<T> value, IParameterExpr[] param) {
      return Manager.SetNow<BLOCK, TDelegate, T>(Property, value, param);
    }

    public Action Now<T>(PropertyExpr<BLOCK,T> Property, DSL.Expr<T> RHS) {
      throw new Exception();
    }
    public Eval<EVAL>.Info<T> Eval<EVAL, T>(PropertyExpr<BLOCK,T> Property, Eval<EVAL> txt) where EVAL : Eval<EVAL> {
      return Manager.Eval<EVAL, BLOCK, T>(txt, Property);
    }
    public class Property<CONTAINER,BRAND> : Properties.Property<CONTAINER,BRAND> where CONTAINER : IBlockish<CONTAINER> where BRAND : Brand<BRAND> {
      public Property(string name) : base(name) { }
    }
    public static Property<BLOCK,BRAND> NewBlockProperty<BRAND>(string name) where BRAND : Brand<BRAND> { return new Property<BLOCK,BRAND>(name); }
    public static Property<ElemB,BRAND>  NewElementProperty<BRAND>(string name) where BRAND : Brand<BRAND> { return new Property<ElemB,BRAND>(name); }

    public interface IElemB : IElemA {
      new ElemB AsElem { get; }
    }
    public abstract class Elem0 : BaseElem<ElemB>, IElemA {
      public ElemA AsElem { get { return this; } }
      public static implicit operator ElemA(Elem0 e) { return ((Block)e.Block)[e.Index]; }
      public new BLOCK Block { get { return (BLOCK)BaseBlock; } }
    }
    public class ElemB : Elem0, IElemB {
      public new ElemB AsElem { get { return this; } }
    }
    public new ElemB this[IntBl Index] { get { return new ElemB() { BaseBlock = Self, Index = Index }; } }
    public new Action<ElemB> ForAll { set { ForAll0<ElemB>(value); } }
    public BLOCK Self { get { return (BLOCK)this; } }
    static Block() {}
  }
  public abstract class SubBlock<SUB> : Block<SUB>, ISubBlock<SUB> where SUB : SubBlock<SUB>, new() {
    public ElemA Container { get; private set; }
    public override int GetHashCode() { return typeof(SUB).GetHashCode() + Container.GetHashCode(); }
    public override bool Equals(object obj) {
      if (obj is SUB)
        return ((SUB)obj).Container.Equals(Container);
      else return base.Equals(obj);
    }
    public override SUB Translate<EVAL>(TranslateEval<EVAL> txt) {
      return new SUB() { Container = Container.Translate(txt) };
    }

    public override Block ParentBlock { get { return Container.ParentBlock; } }
    public override IntBl Index { get { return Container.Index; } }
    public override SUB Normalize {
      get { return new SUB() { Container = Container.Normalize }; }
    }

    public override string ToString() { return Container + "." + typeof(SUB).Name; }
    public override BlockManager Manager { get { return Container.Block.Manager; } }
    public static SUB Get<BLOCK>(Block<BLOCK>.ElemB Elem) where BLOCK : Block<BLOCK> { return new SUB() { Container = Elem }; } 

    public static class Factory<BLOCK> where BLOCK : Block<BLOCK> {
      //private readonly BLOCK Block;
      //internal Factory(BLOCK Block) { this.Block = Block; }
      //public override string ToString() { return typeof(BLOCK).Name + "." + typeof(SUB).Name;  }
    }
    public override int Depth { get { return ParentBlock.Depth + 1; } }
  }
  public interface ITopBlock : IBlock {
    void DoInit(); 
  }

  public abstract class TopBlock<BLOCK> : Block<BLOCK>, IHasMixin<BLOCK>, ITopBlock where BLOCK : TopBlock<BLOCK> {
    public override BlockManager Manager { get { return Manager0; } }
    private readonly BlockManager Manager0;
    public MixinManager<BLOCK> Mixins { get; private set; }
    public TopBlock(BlockManager Manager) { 
      this.Manager0 = Manager;
      Manager.TopBlocks.Add(this);
      Mixins = new MixinManager<BLOCK>((BLOCK) this);
    }
    public void DoInit() {
      Mixins.DoInit((BLOCK)this);
      AfterMixins();
    }
    public Action AfterMixins = () => { };

    public override int Depth { get { return 0; } }

    public override BLOCK Translate<EVAL>(TranslateEval<EVAL> txt) { return (BLOCK) this; }
    public override BLOCK Normalize { get { return (BLOCK) this; } }
    public override IntBl Index { get { return 0; } }
    public override Block ParentBlock { get { return null; } }
  }
  public interface IInitMixin<BLOCK, BRAND> : IMixin<BLOCK>
    where BLOCK : TopBlock<BLOCK, BRAND>
    where BRAND : Brand<BRAND> {
    void Init(BlockManager Manager, int Idx, BLOCK Block, BRAND Brand);
  }

  public abstract class TopBlock<BLOCK, BRAND> : TopBlock<BLOCK>, IListBl<TopBlock<BLOCK,BRAND>.ElemC>
    where BLOCK : TopBlock<BLOCK, BRAND>
    where BRAND : Brand<BRAND> {
    //private readonly BRAND[] Elements;
    private readonly int Count0;

    public TopBlock(BlockManager Manager, int Count, Func<BLOCK, int, BRAND> InitF) : base(Manager) { 
      this.Count0 = Count;
      for (int i = 0; i < Count; i++) {
        var ret = InitF((BLOCK) this, i);
        foreach (var mixin in this.Mixins.Mixins) {
          if (mixin is IInitMixin<BLOCK, BRAND>)
            ((IInitMixin<BLOCK, BRAND>)mixin).Init(Manager, i, (BLOCK) this, ret);
        }
        this[i].At.Init = ret;
      }
    }
    public static readonly Property<Block<BLOCK>.ElemB,BRAND> AtProperty = NewElementProperty<BRAND>("At");

    public interface IElemC : TopBlock<BLOCK>.IElemB {
      new ElemC AsElem { get; }
    }
    public new abstract class Elem0 : BaseElem<ElemC>, IElemA {
      public new BLOCK Block { get { return (BLOCK)BaseBlock; } }
      public static implicit operator ElemA(Elem0 e) { return ((Block)e.Block)[e.Index]; }
      public static implicit operator Block<BLOCK>.ElemB(Elem0 e) { return ((Block<BLOCK>)e.Block)[e.Index]; }
      public ElemA AsElem { get { return this; } } 
    }
    public abstract class Elem1 : Elem0, TopBlock<BLOCK>.IElemB {
      public new TopBlock<BLOCK>.ElemB AsElem { get { return this; } }
    }


    public class ElemC : Elem1, IElemC {
      public BRAND At { get { return AtProperty[this]; } set { At.Bind = value; } }
      public new ElemC AsElem { get { return this; } }
    }
    public new ElemC this[IntBl Index] { get { return new ElemC() { BaseBlock = Self, Index = Index }; } }
    public new Action<ElemC> ForAll { set { ForAll0<ElemC>(value); } }
    public override int Count { get { return Count0; } }
  }


}
namespace Bling.Blocks {
  using Bling.Ops;
  using Bling.Properties;
  using Bling.Linq;
  public abstract class BlockManager<EVAL,EXPRESSION> : BlockManager where EVAL : ScopedEval<EVAL>, ILoopEval<EVAL,EXPRESSION>, new() {
    protected List<EXPRESSION> InitCode { get; private set; }
    protected Dictionary<IPropertyConstraint, EXPRESSION> RelaxCode { get; private set; }
    protected Dictionary<IPropertyConstraint, EXPRESSION> LinkCode { get; private set; }

    protected virtual Expr<T> ConvertLHSForRelax<CONTAINER, T>(PropertyExpr<CONTAINER, T> e) where CONTAINER : IBlockish<CONTAINER> { return e; }
    protected virtual Expr<T> ConvertLHSForNormal<CONTAINER, T>(PropertyExpr<CONTAINER, T> e) where CONTAINER : IBlockish<CONTAINER> { return e; }

    protected readonly Dictionary<IUseInUI, Expr> UIToBlock = new Dictionary<IUseInUI, Expr>();
    protected abstract Expr<T> ForUI<T>(IUseInUI<T> UI, LoopIndexExpr[] Context);
    protected virtual Expr<T> ConvertFromUI<T>(IUseInUI<T> Expr, IPropertyConstraint Constraint) {
      if (UIToBlock.ContainsKey(Expr)) return (Expr<T>)UIToBlock[Expr];
      var block = ForUI(Expr, Constraint.Context);
      UIToBlock[Expr] = block;
      return block;
    }
    private class MyVisitConstraint : VisitConstraint {
      public BlockManager<EVAL, EXPRESSION> Outer;
      public readonly Dictionary<IPropertyConstraint, EXPRESSION> Output = new Dictionary<IPropertyConstraint,EXPRESSION>();
      public bool IsRelax = false;
      protected Expr<T> ConvertLHS<CONTAINER, T>(PropertyExpr<CONTAINER, T> e) where CONTAINER : IBlockish<CONTAINER> {
        if (IsRelax) return Outer.ConvertLHSForRelax(e);
        else return Outer.ConvertLHSForNormal(e);
      }
      // will always execute in background thread.

      private class ConvertRHS : TranslateEval<ConvertRHS> {
        public BlockManager<EVAL, EXPRESSION> Outer;
        public IPropertyConstraint Constraint;
        protected override Expr<T> RealTranslate<T>(Expr<T> value) {
          if (value is IUseInUI<T>) {
            return Outer.ConvertFromUI((IUseInUI<T>) value, Constraint);
          }
          return base.RealTranslate(value); 
        }
      }
      public void Visit<CONTAINER, T>(PropertyConstraint<CONTAINER, T> Constraint) where CONTAINER : IBlockish<CONTAINER> {
        Func<EVAL, EXPRESSION> F = eval0 => {
          return eval0.MakeAssign(ConvertLHS(Constraint.LHS), !IsRelax ? Constraint.RHS : new ConvertRHS() { 
            Outer = Outer, Constraint = Constraint, 
          }.Translate(Constraint.RHS));
        };
        Func<int, Func<EVAL, EXPRESSION>> G = null;
        G = (Idx) => {
          if (Idx == Constraint.Context.Length) return F;
          return txt => txt.DoLoop(Constraint.Context[Idx], G(Idx + 1));
        };
        var eval = Outer.MakeEval();
        EXPRESSION e = G(0)(eval);
        e = eval.End(e);
        Output[Constraint] = (e);
      }
      private class ReverseLink : IAssign {
        internal EVAL Eval;
        internal readonly List<EXPRESSION> Result = new List<EXPRESSION>();
        public IAssign Copy() { return this; }
        public bool Accept<T>(Expr<T> LHS, Expr<T> RHS) {
          if (LHS is ICanAssignExpr<T, EXPRESSION>) {
            var LHS0 = ((ICanAssignExpr<T, EXPRESSION>)LHS);
            var RHS0 = Eval.Extract(Eval.DoEval(RHS));
            Result.Add(LHS0.Assign(RHS0, Eval));
            return true;
          } else if (LHS is NoAssignExpr<T>) return true;
          else return false;
        } 
      }
      // will execute in UI thread.
      public void Visit<CONTAINER, T>(LinkConstraint<CONTAINER, T> Constraint) where CONTAINER : IBlockish<CONTAINER> {
        Func<EVAL, EXPRESSION> F = eval0 => {
          var test = eval0.Extract(eval0.DoEval(Constraint.Guard));
          return eval0.DoIf(test,
            eval1 => {
              var assign = new ReverseLink() { Eval = eval1 };
              if (!Constraint.RHS.Solve(assign, Constraint.LHS)) {
                return eval1.Exception;
              } else return eval1.Block(assign.Result);
            },
            eval1 => eval1.MakeAssign(ConvertLHS(Constraint.LHS), Constraint.RHS));
        };
        Func<int, Func<EVAL, EXPRESSION>> G = null;
        G = (Idx) => {
          if (Idx == Constraint.Context.Length) return F;
          return txt => txt.DoLoop(Constraint.Context[Idx], G(Idx + 1));
        };
        var eval = Outer.MakeEval();
        EXPRESSION e = G(0)(eval);
        e = eval.End(e);
        Output[Constraint] = (e);
      }
    }


    internal readonly List<AssignConstraint> Assigns = new List<AssignConstraint>();
    protected readonly List<EXPRESSION> AssignCode = new List<EXPRESSION>();
    internal abstract class AssignConstraint {
      public LoopIndexExpr[] Context { get; set; }
      public abstract ICanAssignExpr<EXPRESSION> BaseLHS { get; }
      public abstract Expr BaseRHS { get; }
    }
    internal class AssignConstraint<T> : AssignConstraint {
      public ICanAssignExpr<T, EXPRESSION> LHS;
      public Expr<T> RHS;
      public override ICanAssignExpr<EXPRESSION> BaseLHS { get { return LHS; } }
      public override Expr BaseRHS { get { return RHS; } }
    }
    public override bool TryAssign<T>(ICanAssignExprT<T> LHS, Expr<T> RHS) {
      if (LHS is ICanAssignExpr<T, EXPRESSION>) {
        Assigns.Add(new AssignConstraint<T>() {
          Context = Block.Stack.Reverse().ToArray(),
          LHS = (ICanAssignExpr<T, EXPRESSION>)LHS,
          RHS = RHS,
        });
        return true;
      }
      return false;
    }

    public virtual void DoGenerate() {
      {
        var initVisitor = new MyVisitConstraint() { Outer = this, IsRelax = false };
        foreach (var init in Inits) init.Accept(initVisitor);
        InitCode = new List<EXPRESSION>();
        foreach (var init in Inits) InitCode.Add(initVisitor.Output[init]);
      }
      {
        var constraintVisitor = new MyVisitConstraint() { Outer = this, IsRelax = true };
        foreach (var constraint in Relax)
          constraint.Accept(constraintVisitor);
        RelaxCode = constraintVisitor.Output;
      }
      {
        var linkVisitor = new MyVisitConstraint() { Outer = this, IsRelax = false };
        foreach (var link in Links) link.Accept(linkVisitor);
        LinkCode = linkVisitor.Output;
      }
      foreach (var a in Assigns) {
        Func<EVAL, EXPRESSION> F = eval0 => {
          return eval0.MakeAssign((Expr) a.BaseLHS, a.BaseRHS);
        };
        Func<int, Func<EVAL, EXPRESSION>> G = null;
        G = (Idx) => {
          if (Idx == a.Context.Length) return F;
          return txt => txt.DoLoop(a.Context[Idx], G(Idx + 1));
        };
        var eval = MakeEval();
        EXPRESSION e = G(0)(eval);
        e = eval.End(e);
        AssignCode.Add(e);
      }
      // links....
    }
    private EVAL MakeEval() {
      Dictionary<Eval<EVAL>.Info, int> Depth = new Dictionary<Eval<EVAL>.Info, int>();
      var Eval = new EVAL();
      return Eval;
    }
  }
  // not bad for linq specific code!
  public class LinqBlockManager : BlockManager<DefaultLinqEval, linq.Expression> {
    public interface IArrayInfo {
      linq.Expression ReadArrayE { get; }
      linq.Expression WriteArrayE { get; }
      linq.Expression OldArrayE { get; }
      linq.Expression DoSwap { get; }
      linq.Expression DoStepSwap { get; }
    }
    private class ArrayInfoExpr<T> : Expr<T>, ICanAssignExpr<T, linq.Expression>, ICanAssignNowExpr<T> {
      public IntBl Index;
      public Func<linq.Expression> Array;
      protected  override Eval<EVAL>.Info<T> Eval<EVAL>(Eval<EVAL> txt) {
        if (txt is LinqEval<EVAL>) {
          var Index1 = txt.DoEval(Index.Underlying);
          var Index0 = ((LinqEval<EVAL>) txt).Extract(Index1);
          var ret = new LinqEval<EVAL>.MyInfo<T>() { Value = linq.Expression.ArrayAccess(Array(), Index0) };
          ((LinqEval<EVAL>)txt).AtLeast(ret, Index1);
          return ret;
        } else if (txt is TranslateEval<EVAL>)
          return new TranslateEval<EVAL>.MyInfo<T>() {
            Value = new ArrayInfoExpr<T>() { Array = Array, Index = ((TranslateEval<EVAL>)txt).Translate(Index.Underlying), },
          };
        else if (txt is IDynamicEval<EVAL>) {
          return ((IDynamicEval<EVAL>)txt).Dynamic<T>(DynamicMode.Dynamic);
        }

        throw new NotImplementedException();
      }
      public linq.Expression Assign<EVAL>(linq.Expression RHS0, IExpressionEval<EVAL, linq.Expression> txt) where EVAL : Eval<EVAL> {
        var Index0 = ((LinqEval<EVAL>)txt).Extract(txt.DoEval(Index.Underlying));
        return linq.Expression.Assign(linq.Expression.ArrayAccess(Array(), Index0), RHS0);
      }
      public TDelegate SetNow<TDelegate>(Expr<T> value, params IParameterExpr[] param) {
        return (new DefaultLinqEval()).DoSetNow<T, TDelegate>(this, value, param);
      }
      public bool SetNow(T value) {
        (new DefaultLinqEval()).DoSetNow<T, Action>(this, new Constant<T>(value))();
        return true;
      }
    }
    public class ArrayInfo<T> : IArrayInfo {
      public T[] ReadArray;
      public T[] WriteArray;
      public T[] OldArray;
      public ArrayInfo(int Count, bool IsOld) {
        ReadArray = new T[Count];
        WriteArray = new T[Count];
        if (IsOld) OldArray = new T[Count];
      }
      public linq.Expression ReadArrayE {
        get {
          var self = linq.Expression.Constant(this, typeof(ArrayInfo<T>));
          return linq.Expression.Field(self, "ReadArray");
        }
      }
      public linq.Expression WriteArrayE {
        get {
          var self = linq.Expression.Constant(this, typeof(ArrayInfo<T>));
          return linq.Expression.Field(self, "WriteArray");
        }
      }
      public linq.Expression OldArrayE {
        get {
          var self = linq.Expression.Constant(this, typeof(ArrayInfo<T>));
          return linq.Expression.Field(self, "OldArray");
        }
      }
      public linq.Expression DoSwap {
        get {
          var swap = linq.Expression.Parameter(typeof(T[]), "Swap");
          var swapFrRead = linq.Expression.Assign(swap, ReadArrayE);
          var readFrWrite = linq.Expression.Assign(ReadArrayE, WriteArrayE);
          var writeFrSwap = linq.Expression.Assign(WriteArrayE, swap);
          return linq.Expression.Block(new linq.ParameterExpression[] { swap }, swapFrRead, readFrWrite, writeFrSwap);
        }
      }
      public linq.Expression DoStepSwap {
        get {
          (OldArray != null).Assert();
          var swap = linq.Expression.Parameter(typeof(T[]), "Swap");
          // swap := old, old := read, read := write, write := swap
          var swapFrOld = linq.Expression.Assign(swap, OldArrayE);
          var oldFrRead = linq.Expression.Assign(OldArrayE, ReadArrayE);
          var readFrWrite = linq.Expression.Assign(ReadArrayE, WriteArrayE);
          var writeFrSwap = linq.Expression.Assign(WriteArrayE, swap);
          return linq.Expression.Block(new linq.ParameterExpression[] { swap }, swapFrOld, oldFrRead, readFrWrite, writeFrSwap);
        }
      }
    }
    private readonly Dictionary<BasePropertyExpr<IBlockish>, IArrayInfo> Arrays0 = new Dictionary<BasePropertyExpr<IBlockish>, IArrayInfo>();
    public readonly List<Action> DoRelax = new List<Action>();
    public readonly List<Action> DoLinks = new List<Action>();

    public readonly List<Action> DoAssigns = new List<Action>();

    //public class Cell<T> { public T Value; }
    public readonly List<Action> UIFacades = new List<Action>();

    private interface UIFacade {
     // Action GenAssign { get; }
    }
    private class UIFacade<T> : Expr<T>, ICanAssignExpr<T, linq.Expression>, UIFacade {
      private T[] Cells;
      private Expr<int> Index;
      internal IUseInUI<T> UIValue;
      internal UIFacade() { }

      public Action GenAssign(LoopIndexExpr[] Context) {
        var eval = new DefaultLinqEval();
        IParameterExpr[] Es = new IParameterExpr[Context.Length];
        for (int i = 0; i < Es.Length; i++) Es[i] = Context[i];
        var block = eval.DoSetNow00<T>(this, (Expr<T>) UIValue, Es);
        var Used = eval.ExpressionsUsed;
        (Used.Length <= 1).Assert();
        if (Used.Length != 0) {
          var scope = eval.Top;
          block = eval.DoLoop((LoopIndexExpr)Used[0], eval.Parameters.Expressions[Used[0]], eval0 => {
            (scope != eval0.Top).Assert();
            foreach (var s in scope.Temps) eval0.Top.Temps.Add(s.Key, s.Value);
            foreach (var o in scope.Order) eval0.Top.Order.Add(o);
            scope.Temps.Clear();
            scope.Order.Clear();
            return block;
          });
        }
        block = eval.PopScope(block);
        return linq.Expression.Lambda<Action>((block)).Compile();

      }
      protected  override Eval<EVAL>.Info<T> Eval<EVAL>(Eval<EVAL> txt) {
        if (txt is LinqEval<EVAL>) {
          (Cells != null).Assert();
          var self = linq.Expression.Constant(this, typeof(UIFacade<T>));
          var mthd = this.GetType().GetMethod("Get", new Type[1] { typeof(int) });
          var fld = linq.Expression.Call(self, mthd, ((LinqEval<EVAL>) txt).DoEval(Index).Value);
          return new LinqEval<EVAL>.MyInfo<T>() { Value = fld };
        } else if (txt is TranslateEval<EVAL>) return ((TranslateEval<EVAL>) txt).NoTranslation(this);
        else throw new NotSupportedException();
      }
      public T Get(int Index) {
        return Cells[Index];
      }

      public void Set(int Index, T Value) {
        Cells[Index] = Value;
      }

      public linq.Expression Assign<EVAL>(linq.Expression RHS0, IExpressionEval<EVAL, linq.Expression> txt) where EVAL : Eval<EVAL> {
        (Cells == null).Assert();
        var Used = txt.ExpressionsUsed;
        (Used.Length <= 1).Assert();
        if (Used.Length > 0) {
          Index = (LoopIndexExpr)Used[0];
          var Count = ((LoopIndexExpr)Used[0]).Count;
          // create some code to re-initialize Cells based on size.
          Count.IsConstant.Assert();
          Cells = new T[Count.Bl().CurrentValue];
        } else {
          Index = new Constant<int>(0);
          Cells = new T[1];
        }
        var self = linq.Expression.Constant(this, typeof(UIFacade<T>));
        var mthd = this.GetType().GetMethod("Set", new Type[2] { typeof(int), typeof(T) });

        return linq.Expression.Call(self, mthd, txt.Extract(txt.DoEval(Index)), RHS0);

        //var fld = linq.Expression.ArrayAccess(self, txt.Extract(txt.DoEval(Index)));
        //return linq.Expression.Assign(fld, RHS0);
      }
    }
    protected override Expr<T> ForUI<T>(IUseInUI<T> UI, LoopIndexExpr[] Context) {
      var ret = new UIFacade<T>() { UIValue = UI };
      UIFacades.Add(ret.GenAssign(Context));
      return ret;
    }

    public override void DoGenerate() {
      var ArrayInfoType = typeof(ArrayInfo<>);
      foreach (var b in this.Bound) {
        var UseArrayInfoType = ArrayInfoType.MakeGenericType(b.Property.TypeOfT);
        var c = UseArrayInfoType.GetConstructor(new Type[] { typeof(int), typeof(bool) });
        Arrays0[b] = (IArrayInfo)c.Invoke(new object[] { b.AllocSize(), Old.Contains(b) });
      }
      // now we can generate array expressions. 
      base.DoGenerate();
      { // convert the results to actions. 
        foreach (var c in this.Relax) {
          var info = this.Arrays0[c.BaseNormalizeLHS];
          var e = this.RelaxCode[c];
          e = linq.Expression.Block(e, c.IsStep ? info.DoStepSwap : info.DoSwap);
          DoRelax.Add(linq.Expression.Lambda<Action>(e).Compile());
        }
        foreach (var c in this.Links) {
          var e = this.LinkCode[c];
          DoLinks.Add(linq.Expression.Lambda<Action>(e).Compile());
        }
        foreach (var c in this.AssignCode) {
          DoAssigns.Add(linq.Expression.Lambda<Action>(c).Compile());
        }
      }
    }
    public void BeforeGenerate() {
      foreach (var block in TopBlocks) block.DoInit();
    }
    public void AfterGenerate() {
      foreach (var init in InitCode) {

        linq.Expression.Lambda<Action>(init).Compile()();
      }

    }
    protected ICanAssignExpr<T,linq.Expression> PropertyArray<CONTAINER, T>(PropertyExpr<CONTAINER,T> Property) where CONTAINER : IBlockish<CONTAINER> {
      var key = Property.Normalize().Cv();

      return new ArrayInfoExpr<T>() {
        Index = Property.UseIndex(),
        Array = () => Arrays0[key].ReadArrayE,
      };
    }
    public override Expr<T> OldExpression<T>(PropertyExpr<T> Property) {
      var bs = new BasePropertyExpr<IBlockish>() {
        ActualContainer = (IBlockish)Property.Container,
        Property = Property.Property,
      };
      return new ArrayInfoExpr<T>() {
        Index = bs.UseIndex(),
        Array = () => Arrays0[bs.Normalize()].OldArrayE,
      };
    }
    protected override Expr<T> ConvertLHSForRelax<CONTAINER, T>(PropertyExpr<CONTAINER, T> Property) {
      return new ArrayInfoExpr<T>() {
        Index = Property.UseIndex(),
        Array = () => Arrays0[Property.Normalize().Cv()].WriteArrayE,
      };
    }
    public override bool SetNow<CONTAINER, T>(PropertyExpr<CONTAINER, T> Property, T value) {
      return new ArrayInfoExpr<T>() {
        Index = Property.UseIndex(),
        Array = () => Arrays0[Property.Normalize().Cv()].ReadArrayE,
      }.SetNow(value);
    }
    public override TDelegate SetNow<CONTAINER, TDelegate, T>(PropertyExpr<CONTAINER, T> Property, Expr<T> value, IParameterExpr[] param) {
      return new ArrayInfoExpr<T>() {
        Index = Property.UseIndex(),
        Array = () => Arrays0[Property.Normalize().Cv()].ReadArrayE,
      }.SetNow<TDelegate>(value, param);
    }
    protected override Expr<T> ConvertLHSForNormal<CONTAINER, T>(PropertyExpr<CONTAINER, T> Property) {
      return (Expr<T>)PropertyArray(Property);
    }
    public override Eval<EVAL>.Info<T> Eval<EVAL, CONTAINER, T>(Eval<EVAL> txt, PropertyExpr<CONTAINER, T> Property) {
      return txt.DoEval(((Expr<T>)PropertyArray(Property)));
    }
  }
}
