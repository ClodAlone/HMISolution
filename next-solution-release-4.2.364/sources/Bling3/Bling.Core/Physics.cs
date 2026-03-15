using System;
using System.Collections.Generic;
using System.Collections;
using Bling.DSL;
using Bling.Core;
using Bling.Util;
using Bling.Matrices;
using Bling.Linq;
using Bling.Blocks;
using Bling.Properties;
using linq = Microsoft.Linq.Expressions;


namespace Bling.Core {
  // Add step, relax (easy), link. 
  public partial interface IRelax<BRAND> {
    BRAND this[DoubleBl T] { get;  set; }
    BRAND this[BoolBl Guard] { get;  set; }
    BRAND this[DoubleBl T, BoolBl Guard] { get; set; }
    BRAND To { get; set; }
  }
  public partial interface IBrand<BRAND> : IBrand {
    IRelax<BRAND> Relax { get; }
    Func<BRAND, BRAND, BRAND> Step { set; }
  }
  public abstract partial class Brand<BRAND> : Brand, IBrand<BRAND>, IConditionTarget<BRAND> where BRAND : Brand<BRAND> {
    public abstract IRelax<BRAND> Relax { get; }
    public abstract Func<BRAND, BRAND, BRAND> Step { set; }
  }
  public abstract partial class Brand<T, BRAND> : Brand<BRAND>, IBrandT<T>, IExtends<BRAND,ObjectBl>, IBrand<T, BRAND> where BRAND : Brand<T, BRAND> {
    public BRAND NoAssign { get { return ToBrand(new NoAssignExpr<T>(Underlying)); } }
    private class OldEval : TranslateEval<OldEval> {
      public bool Check = false;
      protected override Expr<S> RealTranslate<S>(Expr<S> value) {
        if (value is PropertyExpr<S>) {
          var value0 = (PropertyExpr<S>)value;
          if (value0.Container is IBlockish) {
            var value1 = BasePropertyExpr<IBlockish>.Cv(value0).Normalize();
            var old = value1.ActualContainer.Manager.Old;
            if (Check && !value1.ActualContainer.Manager.Old.Contains(value1)) throw new Exception();
            value1.ActualContainer.Manager.Old.Add(value1);
            return value1.ActualContainer.Manager.OldExpression(value0);
          }
        }
        return base.RealTranslate<S>(value);
      }
    }
    private class StepAssign : IAssign {
      public IAssign Copy() { return this; }
      public bool Accept<S>(Expr<S> lhs, Expr<S> rhs) {
        if (!(lhs is PropertyExpr<S>)) return false;
        var lhs0 = (PropertyExpr<S>)lhs;
        if (lhs0.Container is IBlockish) {
          var normal = BasePropertyExpr<IBlockish>.Cv(lhs0).Normalize();
          if (normal.ActualContainer.Manager.HasStep.Contains(normal)) throw new Exception("Already has step");
          normal.ActualContainer.Manager.HasStep.Add(normal);
          if (!lhs0.BindWith(rhs)) throw new Exception();
          var added = normal.ActualContainer.Manager.Relax[normal.ActualContainer.Manager.Relax.Count - 1];
          (added.BaseLHS.Equals(BasePropertyExpr<IBlockish>.Cv(lhs0))).Assert();
          added.IsStep = true;
          return true;
        }
        return false;
      }
    }
    public override Func<BRAND, BRAND, BRAND> Step {
      set {
        var old = new OldEval().Translate(this.Underlying);
        var step = value(this, ToBrand(old));
        if (!Underlying.Solve(new StepAssign(), step))
          throw new NotSupportedException();
      }
    }
    public BRAND Old {
      get { return ToBrand(new OldEval() { Check = true }.Translate(this.Underlying)); }
      set { Old.Bind = value; }
    }
    private class DefaultCollect : IAssign {
      public readonly Dictionary<IPropertyExpr, Func<DoubleBl, Expr>> Found = new Dictionary<IPropertyExpr, Func<DoubleBl, Expr>>();
      public IAssign Copy() { return this; }

      private class TranslateLoop : TranslateEval<TranslateLoop>, IAssign {
        private int FakeIndex = 0;
        public readonly Dictionary<LoopIndexExpr, LoopIndexExpr> FakeMap = new Dictionary<LoopIndexExpr, LoopIndexExpr>();
        public readonly Dictionary<LoopIndexExpr, Expr<int>> TranslateMap = new Dictionary<LoopIndexExpr, Expr<int>>();
        private LoopIndexExpr LoopTo;
        public LoopIndexExpr AllocateLoopTo {
          get {
            LoopTo = new LoopIndexExpr("Fake" + FakeIndex, 0);
            FakeIndex += 1;
            return LoopTo;
          }
        }
        public class TranlsateFake : TranslateEval<TranslateLoop> {
          public readonly TranslateLoop Outer;
          public TranlsateFake(TranslateLoop Outer) { this.Outer = Outer; }
          protected override Expr<S> RealTranslate<S>(Expr<S> value) {
            if (value is LoopIndexExpr) {
              var value0 = (LoopIndexExpr)(object)value;
              if (Outer.FakeMap.ContainsKey(value0)) {
                // load it up!
                var ret = (Outer.FakeMap[value0]);
                return (Expr<S>)(object)ret;
              }
            }
            return base.RealTranslate<S>(value);
          }
        }
        public IAssign Copy() { return this; }
        public bool Accept<S>(Expr<S> LHS, Expr<S> RHS) {
          if (LHS is LoopIndexExpr) {
            if (!RHS.Equals(LoopTo)) {
              var LHS0 = (LoopIndexExpr)(object)LHS;
              (!TranslateMap.ContainsKey(LHS0)).Assert();
              TranslateMap[LHS0] = (Expr<int>)(object)RHS;
              (!FakeMap.ContainsKey(LoopTo) || FakeMap[LoopTo] == LHS0).Assert();
              FakeMap[LoopTo] = LHS0;
            }
            return true;
          }
          return false;
        }
        protected override Expr<S> RealTranslate<S>(Expr<S> value) {
          if (value is LoopIndexExpr) {
            var value0 = (LoopIndexExpr)(object)value;
            if (TranslateMap.ContainsKey(value0)) {
              // load it up!
              var ret = (TranslateMap[value0]);
              return (Expr<S>) (object) ret;
            }
          }
          return base.RealTranslate<S>(value);
        }
      }

      public bool Accept<S>(Expr<S> LHS, Expr<S> RHS) {
        if (LHS is PropertyExpr<S> && ((PropertyExpr<S>) LHS).Container is IBlockish) {
          var LHS0 = (PropertyExpr<S>)LHS;
          var Container = (IBlockish)LHS0.Container;
          var TranslateLoops = new TranslateLoop();
          IBlockish At = Container;
          while (At != null) {
            At.Index.Underlying.Solve(TranslateLoops, TranslateLoops.AllocateLoopTo);
            At = At.ParentBlock;
          }
          // XXX: must support lerp!
          if (TranslateLoops.TranslateMap.Count > 0) {
            var t2 = new TranslateLoop.TranlsateFake(TranslateLoops);
            RHS = TranslateLoops.Translate(RHS);
            RHS = t2.Translate(RHS);
            // we have to translate LHS also!
            LHS0 = (PropertyExpr<S>) TranslateLoops.Translate(LHS0);
            LHS0 = (PropertyExpr<S>) t2.Translate(LHS0);
          }
          Found[LHS0] =
            t => {
              if (TranslateLoops.TranslateMap.Count > 0) {
                var t2 = new TranslateLoop.TranlsateFake(TranslateLoops);
                t = TranslateLoops.Translate(t.Underlying);
                t = t2.Translate(t.Underlying);
              }
              return Ops.DoubleOperators0<S>.Lerp.Instance.Make(LHS0, RHS, t);
            };
        }
        return false; // keep going. 
      }
    }
    private class BiRelaxSet : IRelax<BRAND> {
      internal BRAND Target;
      public BRAND this[DoubleBl Strength] {
        get { return Target; }
        set {
          var assign = new DefaultCollect();
          var ret = Target.Underlying.Solve(assign, value.Underlying);
          (!ret).Assert(); // will always be false. 
          if (assign.Found.Count == 0) throw new NotSupportedException();
          var UseStrength = Strength / assign.Found.Count;
          foreach (var p in assign.Found) {
            var RHS = p.Value(UseStrength);
            var LHS = p.Key;
            LHS.BindWith(RHS);
          }
        }
      }
      public BRAND this[BoolBl Guard] {
        get { return Target; }
        set { this[Guard.Condition<DoubleBl>(1d, 0d)] = value; }
      }
      public BRAND this[DoubleBl T, BoolBl Guard] {
        get { return Target; }
        set { this[T * Guard.Condition<DoubleBl>(1d, 0d)] = value; }
      }
      public BRAND To {
        get { return Target; }
        set {
          this[1d] = value;
        }
      }
    }
    public override IRelax<BRAND> Relax { get { return new BiRelaxSet() { Target = this }; } }

  }
  public partial class IntBl : BaseIntBl<int, IntBl, BoolBl>, IMonoNumericBl<int, IntBl> {
    public IntBl Wrap(IntBl Max) {
      return (this < 0).Condition(Max + this, this % Max);
    }
    public IntBl WrapUp(IntBl By, IntBl Max) { return WrapUpOperator.Instance.Make(this,By,Max); }
    public IntBl WrapDown(IntBl By, IntBl Max) { return WrapDownOperator.Instance.Make(this,By,Max); }
  }

  internal abstract class BaseWrapOperator<CNT> : Ops.Operator<int, int, int, int, CNT> where CNT : BaseWrapOperator<CNT> {
  }
  internal class WrapUpOperator : BaseWrapOperator<WrapUpOperator> {
    private WrapUpOperator() { }
    public new static readonly WrapUpOperator Instance = new WrapUpOperator();
    public override Expr<int> Expand(Expr<int> argA, Expr<int> argB, Expr<int> argC) {
      if (argA is Ops.Operation<int, int, int, int> && ((Ops.Operation<int>)argA).BaseBaseOp == WrapDownOperator.Instance) {
        var op = (Ops.Operation<int, int, int, int>)argA;
        if (op.ArgB.Equals(argB) && op.ArgC.Equals(argC)) {
          return op.ArgA;
        }
      }
      IntBl Self = argA;
      IntBl By = argB;
      IntBl Max = argC;
      return (Self + By) % Max;
    }
    public override Expr<int> InverseA(Expr<int> argA, Expr<int> argB, Expr<int> argC, Expr<int> result) {
      IntBl Self = argA;
      IntBl By = argB;
      IntBl Max = argC;
      return WrapDownOperator.Instance.Make(result, By, Max);
    }
    public override string Format(string argA, string argB, string argC) {
      return "Next" + base.Format(argA, argB, argC);
    }
  }
  internal class WrapDownOperator : BaseWrapOperator<WrapDownOperator> {
    private WrapDownOperator() { }
    public new static readonly WrapDownOperator Instance = new WrapDownOperator();
    public override Expr<int> Expand(Expr<int> argA, Expr<int> argB, Expr<int> argC) {
      if (argA is Ops.Operation<int, int, int, int> && ((Ops.Operation<int>)argA).BaseBaseOp == WrapUpOperator.Instance) {
        var op = (Ops.Operation<int, int, int, int>)argA;
        if (op.ArgB.Equals(argB) && op.ArgC.Equals(argC)) {
          return op.ArgA;
        }
      }
      IntBl Self = argA;
      IntBl By = argB;
      IntBl Max = argC;

      Self = Self - By;
      return (Self < 0).Condition(Max + Self, Self);
    }
    public override Expr<int> InverseA(Expr<int> argA, Expr<int> argB, Expr<int> argC, Expr<int> result) {
      IntBl Self = argA;
      IntBl By = argB;
      IntBl Max = argC;
      return WrapUpOperator.Instance.Make(result, By, Max);
    }
    public override string Format(string argA, string argB, string argC) {
      return "Prev" + base.Format(argA, argB, argC);
    }
  }
  /*
  internal class RelaxOperator<T> : Ops.Operator<T, double, T, RelaxOperator<T>> {
    private RelaxOperator() { }
    public new static readonly RelaxOperator<T> Instance = new RelaxOperator<T>();
    public override Expr<T> Expand(Expr<T> argA, Expr<double> argB) { return argA; }
    // argA.Relax[argB] = result
    // argA = argB.Lerp(argA, result)
    public override Expr<T> InverseA(Expr<T> argA, Expr<double> argB, Expr<T> result) {
      var isZero = (argB.Bl() == 0d);
      var lerp = Ops.DoubleOperators0<T>.Lerp.Instance.Make(argA, result, argB);
      return Ops.ValueOperators<T>.Condition.Instance.Make(isZero, argA, lerp);
    }
  }*/
}


namespace Bling.Physics {
  using Bling.Blocks;
  using Bling.Mixins;
  using Bling.Properties;

  public abstract class PhysicsMixin<TARGET, MIXIN> : Mixin<TARGET, MIXIN>
    where TARGET : Block<TARGET>, IHasMixin<TARGET,MIXIN>
    where MIXIN : Mixin<TARGET, MIXIN> {
    public PhysicsMixin(TARGET Target) : base(Target) { }
    private static bool DoneInit = false;
    private static Action<TARGET> RelaxBefore0 = (t) => { };
    public static Action<TARGET> RelaxBefore {
      set {
        if (DoneInit) throw new Exception("Too late");
        RelaxBefore0 = value;
      }
    }
    private static Action<TARGET> RelaxAfter0 = (t) => { };
    public static Action<TARGET> RelaxAfter {
      set {
        if (DoneInit) throw new Exception("Too late");
        RelaxAfter0 = value;
      }
    }

    public sealed override void DoInit(TARGET Target) {
      RelaxBefore0(Target);
      DoInit0(Target);
      RelaxAfter0(Target);
    }
    protected abstract void DoInit0(TARGET Target);

  }

  public static class HasPosition {
    public interface IHas<TARGET> : IHasMixin<TARGET, Mixin<TARGET>> where TARGET : TopBlock<TARGET>, IHas<TARGET> {}
    public class Mixin<TARGET> : PhysicsMixin<TARGET, Mixin<TARGET>> where TARGET : TopBlock<TARGET>, IHas<TARGET> {
      public static readonly Property<Block<TARGET>.ElemB, PointBl> PositionProperty =
        Block<TARGET>.NewElementProperty<PointBl>("Position");
      public Mixin(TARGET Target) : base(Target) { }
      protected override void DoInit0(TARGET Target) { }
    }
    public static PointBl Position<TARGET>(this Block<TARGET>.IElemB elem) where TARGET : TopBlock<TARGET>, IHas<TARGET> {
      return Mixin<TARGET>.PositionProperty[elem.AsElem];
    }
  }
  public static class HasParticles {
    public interface IHas<TARGET> : IHasMixin<TARGET, Mixin<TARGET>> where TARGET : TopBlock<TARGET>, IHas<TARGET> {}
    public class Mixin<TARGET> : PhysicsMixin<TARGET, Mixin<TARGET>> where TARGET : TopBlock<TARGET>, IHas<TARGET> {
      public Mixin(TARGET Target) : base(Target) { }
      public static int ParticleCount {
        get { DoneInit = true;  return ParticleCount0; } 
        set { if (DoneInit) throw new Exception("Too late"); ParticleCount0 = value; } 
      }
      private static int ParticleCount0 = 4;
      private static bool DoneInit = false;
      protected override void DoInit0(TARGET Target) {}
      public class ParticleBlock : SubBlock<ParticleBlock> {
        public static readonly Property<Block<ParticleBlock>.ElemB, PointBl> PositionProperty =
          ParticleBlock.NewElementProperty<PointBl>("Position");
        public override int Count { get { return ParticleCount; } }

        public interface IElem : SubBlock<ParticleBlock>.IElemB {
          new Elem AsElem { get; }
        }
        public new abstract class Elem0 : BaseElem<Elem>, IElemA {
          public new ParticleBlock Block { get { return (ParticleBlock)BaseBlock; } }
          public static implicit operator ElemA(Elem0 e) { return ((Block)e.Block)[e.Index]; }
          public static implicit operator Block<ParticleBlock>.ElemB(Elem0 e) { return ((Block<ParticleBlock>)e.Block)[e.Index]; }
          public ElemA AsElem { get { return this; } }
        }
        public abstract class Elem1 : Elem0, SubBlock<ParticleBlock>.IElemB {
          public new SubBlock<ParticleBlock>.ElemB AsElem { get { return this; } }
        }
        public class Elem : Elem1, IElem {
          public PointBl Position { get { return PositionProperty[this]; } set { Position.Bind = value; } }
          public new Elem AsElem { get { return this; } }
        }
        public new Elem this[IntBl Index] { get { return new Elem() { BaseBlock = Self, Index = Index }; } }
        public new Action<Elem> ForAll { set { ForAll0<Elem>(value); } }
      }
    }
    public static Mixin<TARGET>.ParticleBlock Particles<TARGET>(this Block<TARGET>.IElemB elem) where TARGET : TopBlock<TARGET>, IHas<TARGET> {
      return Mixin<TARGET>.ParticleBlock.Get<TARGET>(elem.AsElem);
    }
  }
  public static class HasShape {
    public interface IHas<TARGET> : IHasMixin<TARGET, Mixin<TARGET>>, HasParticles.IHas<TARGET> where TARGET : TopBlock<TARGET>, IHas<TARGET> { }
    public class Mixin<TARGET> : PhysicsMixin<TARGET, Mixin<TARGET>> where TARGET : TopBlock<TARGET>, IHas<TARGET> {
      private readonly PointBl[] Points;
      private readonly VecAngle2DBl[] Angles;
      private readonly DoubleBl[] Lengths;

      public static Func<IntBl, DoubleBl> Weight = idx => 1d;
      public static Func<IntBl, DoubleBl> ShapeStrength = idx => 1d;

      public void SetSize(PointBl Size) {
        SetPoints(0d, new PointBl(Size.X, 0d), Size, new PointBl(0d, Size.Y));
      }

      public void SetPoints(params PointBl[] Points) {
        if (DoneInit) throw new Exception("Too late");
        (this.Points.Length == Points.Length).Assert();
        for (int i = 0; i < Points.Length; i++)
          this.Points[i] = Points[i];
      }
      public Mixin(TARGET Target) : base(Target) {
        Points = new PointBl[HasParticles.Mixin<TARGET>.ParticleCount];
        Angles = new VecAngle2DBl[HasParticles.Mixin<TARGET>.ParticleCount];
        Lengths = new DoubleBl[HasParticles.Mixin<TARGET>.ParticleCount];
      }
      private bool DoneInit = false;
      private Action<TARGET> RelaxCenter0 = (t) => { };
      public Action<TARGET> AddRelaxCenter {
        set {
          if (DoneInit) throw new Exception("Too late");
          var Old = RelaxCenter0;
          RelaxCenter0 = (t) => {
            Old(t); value(t);
          };
        }
      }
      public int RelaxCount = 1;
      private Action<TARGET> RelaxFirst0 = (t) => { };
      public Action<TARGET> AddRelaxFirst {
        set {
          if (DoneInit) throw new Exception("Too late");
          var Old = RelaxFirst0;
          RelaxFirst0 = (t) => {
            Old(t);
            value(t);
          };
        }
      }
      protected override void DoInit0(TARGET Target) {
        DoneInit = true;
        Target.Mixins.ForceInit<HasParticles.Mixin<TARGET>>(Target);
        {
          PointBl Center = 0d;
          DoubleBl Total = 0d;
          for (int i = 0; i < Points.Length; i++) {
            Center += (Points[i] * Weight(i));
            Total += Weight(i);
          }
          Center = Center / Total;
          for (int i = 0; i < Points.Length; i++) {
            Angles[i] = new VecAngle2DBl(Points[i] - Center);
            Lengths[i] = (Points[i] - Center).Length;
          }
        }
        for (int i = 0; i < RelaxCount; i++) {
          RelaxFirst0(Target);
          RefreshCenterRotate();
          if (i == RelaxCount - 1) RelaxCenter0(Target);
          Target.ForAll = (body) => {
            body.Particles().ForAll = (particle) => {
              particle.Position.Relax[ShapeStrength(particle.Index)] = 
                body.Center() + (body.Rotation() + Angles.Table(particle.Index)).ToPoint * Lengths.Table(particle.Index) * Scale(body.Index);
            };
          };
        }
      }
      public Func<IntBl, DoubleBl> Scale = (Idx) => 1d;

      public void RefreshCenterRotate() {
        // find current center by averaging all points
        {
          Target.ForAll = (body) => {
            PointBl Center = 0d;
            DoubleBl Total = 0d;
            for (int i = 0; i < Points.Length; i++) {
              Center += body.Particles()[i].Position * Weight(i);
              Total += Weight(i);
            }
            body.Center().Bind = (Center / Total);
            // now find current rotation
            PointBl RotationPoint = 0;
            for (int i = 0; i < Points.Length; i++) {
              RotationPoint += (new VecAngle2DBl(body.Particles()[i].Position - body.Center()) - Angles[i]).ToPoint * Weight(i);
            }
            body.Rotation().Bind = new VecAngle2DBl(RotationPoint);
          };
        }
      }

      public static readonly Property<Block<TARGET>.ElemB, PointBl> CenterProperty =
        Block<TARGET>.NewElementProperty<PointBl>("Center");

      public static readonly Property<Block<TARGET>.ElemB, VecAngle2DBl> RotationProperty =
        Block<TARGET>.NewElementProperty<VecAngle2DBl>("Rotation");
    }
    public static Mixin<TARGET> HasShapeE<TARGET>(this Block<TARGET> block) where TARGET : TopBlock<TARGET>, IHas<TARGET> {
      return ((TARGET)block).Mixins.Get<Mixin<TARGET>>();
    }

    public static PointBl Center<TARGET>(this Block<TARGET>.IElemB elem) where TARGET : TopBlock<TARGET>, IHas<TARGET> {
      return Mixin<TARGET>.CenterProperty[elem.AsElem];
    }
    public static VecAngle2DBl Rotation<TARGET>(this Block<TARGET>.IElemB elem) where TARGET : TopBlock<TARGET>, IHas<TARGET> {
      return Mixin<TARGET>.RotationProperty[elem.AsElem];
    }
  }
}
