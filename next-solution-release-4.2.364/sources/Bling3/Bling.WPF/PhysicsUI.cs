using System;
using System.Collections.Generic;
using System.Collections;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Bling;
using Bling.Util;
using Bling.Core;
using Bling.DSL;
using Bling.WPF;
using Bling.Blocks;
using Bling.Physics;
using Bling.Properties;
using Bling.WPF.Util;
using Bling.Mixins;

namespace Bling.Physics.UI {
  public static class HasUI {
    public interface IHas<TARGET, BRAND> : IHasMixin<TARGET, Mixin<TARGET, BRAND>>
      where TARGET : TopBlock<TARGET, BRAND>, IHas<TARGET, BRAND>
      where BRAND : Brand<BRAND>, IExtends<BRAND,FrameworkElementBl> { }
    public class Mixin<TARGET, BRAND> : PhysicsMixin<TARGET, Mixin<TARGET,BRAND>>, IInitMixin<TARGET,BRAND>
      where TARGET : TopBlock<TARGET,BRAND>, IHas<TARGET, BRAND>
      where BRAND : Brand<BRAND>, IExtends<BRAND,FrameworkElementBl> {
      public Mixin(TARGET Target) : base(Target) {}

      public void Init(BlockManager Manager, int Idx, TARGET Block, BRAND Element) { }
      protected override void DoInit0(TARGET Target) { }
    }
  }
  
  public static class MouseInput {
    public interface IHas<TARGET,BRAND> : IHasMixin<TARGET, Mixin<TARGET,BRAND>>, 
      HasParticles.IHas<TARGET>, HasUI.IHas<TARGET,BRAND> 
      where TARGET : TopBlock<TARGET,BRAND>, IHas<TARGET,BRAND> 
    where BRAND : Brand<BRAND>, IExtends<BRAND,FrameworkElementBl> { }


    public static BoolBl IsPinned<TARGET,BRAND>(this TopBlock<TARGET,BRAND>.ElemC elem) where TARGET : TopBlock<TARGET,BRAND>, IHas<TARGET,BRAND>
    where BRAND : Brand<BRAND>, IExtends<BRAND,FrameworkElementBl> {
      return
        Mixin<TARGET, BRAND>.IsPinnedProperty[elem];
    }

    public class Mixin<TARGET,BRAND> : PhysicsMixin<TARGET, Mixin<TARGET,BRAND>>, IInitMixin<TARGET,BRAND>
      where TARGET : TopBlock<TARGET, BRAND>, IHas<TARGET, BRAND>
      where BRAND : Brand<BRAND>, IExtends<BRAND, FrameworkElementBl> {
      public Mixin(TARGET Target) : base(Target) {}

      public static readonly Property<Block<TARGET>.ElemB, BoolBl> IsPinnedProperty =
        Block<TARGET>.NewElementProperty<BoolBl>("IsPinned");

      private Action<int, int, double> DoAssign;
      private Action<int, int> ResetAssign;
      private Action<int, bool> SetPinned;
      private Func<int, double>[] GetLength;
      public void Init(BlockManager Manager, int Idx, TARGET Block, BRAND Element0) {
        var Element = Element0.Coerce<FrameworkElementBl>();
        var Parent = Element.CanvasParent;
        Element.OnMouseDown = (x, y) => {
          if (DoAssign == null) DoAssign = PointBl.Assign<int, int, double>(
            (idx, jdx, dist     ) => Block[idx].Particles()[jdx].Position,
            (idx, jdx, dist, lhs) => lhs.Spring(Parent.MousePosition, dist));


          if (ResetAssign == null)
            ResetAssign = PointBl.Assign<int, int>((i, j     ) => Block[i].Particles()[j].Position.Old, 
                                                   (i, j, lhs) => Block[i].Particles()[j].Position);

          if (SetPinned == null)
            SetPinned = BoolBl.Assign<int, bool>((i, b) => Block[i].IsPinned(), (i, b, p) => b);

          if (GetLength == null) {
            GetLength = new Func<int, double>[HasParticles.Mixin<TARGET>.ParticleCount];
            for (int j = 0; j < HasParticles.Mixin<TARGET>.ParticleCount; j++)
              GetLength[j] = IntBl.Lambda((idx) => {
                var Element1 = Block[idx].At.Coerce<FrameworkElementBl>();
                var MousePosition = Parent.MousePosition;
                var delta = Block[idx].Particles()[j].Position - MousePosition;
                return delta.Length;
              });
          }
          double[] Distances = new double[HasParticles.Mixin<TARGET>.ParticleCount];
          for (int i = 0; i < Distances.Length; i++) 
            Distances[i] = GetLength[i](Idx);

          Action DoInUI = () => {
              for (int i = 0; i < Distances.Length; i++) {
                DoAssign(Idx, i, Distances[i]);
                //ResetAssign(Idx, i);
              }
          };

          {
            Mouse.Capture(Element.CurrentValue, CaptureMode.Element);
            SetPinned(Idx, true);
            Manager.DoInUI.Add(DoInUI);
            Element.OnMouseUp = (x0, y0) => {
              Mouse.Capture(Element.CurrentValue, CaptureMode.None);
              SetPinned(Idx, false);
              Manager.DoInUI.Remove(DoInUI);
              return false;
            };
          }
          return true;
        };
      }
      protected override void DoInit0(TARGET Target) {
        Target.ForAll = (body) => {
          body.IsPinned().Init = false;

        };
      }
    }
  }



  /// <summary>
  /// Creates a world, canvas, and driver for a physical simulation.
  /// </summary>
  public abstract class PhysicsCanvas : Canvas, ICanStart {
    private readonly LinqBlockManager Manager = new LinqBlockManager();
    private readonly BackgroundTimer Timer;
    public PhysicsCanvas() {
      var post = Init(this, Manager);
      Manager.BeforeGenerate();
      Manager.DoGenerate();
      Manager.AfterGenerate();
      post();
      Timer = new BackgroundTimer(this.Dispatcher, 30, () => {
        foreach (var relax in Manager.DoRelax) relax();
      }, () => {
        foreach (var link in Manager.DoLinks) link();
        foreach (var assign in Manager.DoAssigns) assign();
        foreach (var action in Manager.DoInUI) action();
        foreach (var facade in Manager.UIFacades) facade();
      });
    }
    protected abstract Action Init(CanvasBl MainCanvas, BlockManager Manager);
    public void Start() { Timer.Start();  }
    public void Stop() { Timer.Stop();  }
  }
}
