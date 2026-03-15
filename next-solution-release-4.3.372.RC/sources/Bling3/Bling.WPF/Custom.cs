using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using System.Windows.Media.Effects;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Controls.Primitives;
using Bling.Core;
using Bling.DSL;
using Bling.WPF;
using Bling.Util;

namespace Bling.WPF.Custom {

  public class ComplexDragManager<T,BRAND> where BRAND : FrameworkElementBl<T,BRAND> where T : FrameworkElement {
    public ComplexDragManager(BRAND Element) {
      Element.OnMouseDown = (Element0, Event) => {

        Event.Handled = true;
        DispatcherTimer Timer = new DispatcherTimer() {
          Interval = TimeSpan.FromMilliseconds(200),
        };
        Mouse.Capture(Element0.CurrentValue, CaptureMode.Element);
        Timer.Tick += (x, y) => {
          if (Timer == null) return;
          if (OnHold != null) OnHold(Element0);
          Timer.Stop();
          Timer = null;
        };
        Timer.Start();
        var DoDragging = false;
        Action<BRAND, bool> UpdateDrag = null;
        Element0.OnMouseMove = (Element1, Event1) => {
          // no point after the timer has expired, or we aren't pressed anymore.
          if (Event1.LeftButton != MouseButtonState.Pressed) return false;
          if (DoDragging) { if (UpdateDrag != null) UpdateDrag(Element1, true); }
          else {
            if (Timer == null) return false;
            Timer.Stop();
            Timer = null;
            DoDragging = true;
            if (StartDrag != null) UpdateDrag = StartDrag(Element1);
          }
          Event1.Handled = true;
          return true;
        };
        Element0.OnMouseUp = (Element1, Event1) => {
          Event1.Handled = true;
          if (Timer != null) {
            Timer.Stop();
            Timer = null;
            if (OnClick != null) OnClick(Element1);
          } else if (DoDragging) if (UpdateDrag != null) UpdateDrag(Element1, false);
          Mouse.Capture(Element1.CurrentValue, CaptureMode.None);
          return false;
        };
        return true;
      };
    }
    public Action<BRAND> OnHold;
    public Action<BRAND> OnClick;
    public Func<BRAND, Action<BRAND,bool>> StartDrag;
  }

  public class VirtualSphere : Canvas {
    public static GetProperty<VirtualSphere, Vecs.Vec4<double>> RotateAProperty = "RotateA".NewProperty<VirtualSphere, Vecs.Vec4<double>>(QuaternionBl.Identity.CurrentValue);
    public static GetProperty<VirtualSphere, Vecs.Vec4<double>> RotateBProperty = "RotateB".NewProperty<VirtualSphere, Vecs.Vec4<double>>(QuaternionBl.Identity.CurrentValue);
    public static GetProperty<VirtualSphere, Vecs.Vec3<double>> MouseInitProperty = "MouseInit".NewProperty<VirtualSphere, Vecs.Vec3<double>>();

    public VirtualSphere(DoubleBl Buffer) {
      var Self = this.Bl();
      var SphereDiameter = Self.Width;
      SphereDiameter = SphereDiameter - Buffer;
      SphereDiameter = SphereDiameter.Min(Self.Height);
      var sphere = new EllipseBl(Self) {
        Size = SphereDiameter,
        CenterPosition = Self.CenterSize,
        ZIndex = 2,
        Fill = Brushes.LightGray,
        Opacity = 1,
      };
      var outer = new EllipseBl(Self) {
        Size = SphereDiameter + Buffer,
        CenterPosition = Self.CenterSize,
        ZIndex = 1,
        Fill = Brushes.Black,
        Opacity = 1,
      };
      Func<DoubleBl, PointBl> FX = (d) =>
        Self.CenterSize + (2d.PI() * ((d + 1d) / 2d)).ToPoint * ((SphereDiameter / 2d) + Buffer / 4d);
      new EllipseBl(Self) {
        Fill = Brushes.Red,
        Stroke = { Thickness = 0 },
        Size = 10d,
        ZIndex = 2,
        IsHitTestVisible = false,
        CenterPosition = FX((Self.RotateA * Self.RotateB).X)
      };
      new EllipseBl(Self) {
        Fill = Brushes.Blue,
        Stroke = { Thickness = 0 },
        Size = 10d,
        ZIndex = 2,
        IsHitTestVisible = false,
        CenterPosition = FX((Self.RotateA * Self.RotateB).Y)
      };
      new EllipseBl(Self) {
        Fill = Brushes.White,
        Stroke = { Thickness = 0 },
        Size = 10d,
        ZIndex = 2,
        IsHitTestVisible = false,
        CenterPosition = FX((Self.RotateA * Self.RotateB).Z)
      };
      new EllipseBl(Self) {
        Fill = Brushes.Pink,
        Stroke = { Thickness = 0 },
        Size = 10d,
        ZIndex = 2,
        IsHitTestVisible = false,
        CenterPosition = FX((Self.RotateA * Self.RotateB).W)
      };

      var MouseAt = sphere.MousePosition / sphere.Size;
      MouseAt = MouseAt * 2d - 1d;
      MouseAt = MouseAt * new PointBl(+1d, -1d); // flip Y
      var MouseAt3D = new Point3DBl(MouseAt, (1d - MouseAt.LengthSquared).Max(0d).Sqrt);
      {
        Action SetMouseInit = Point3DBl.Assign(Self.MouseInit, MouseAt3D);
        var q = MouseAt3D.AngleBetween(Self.MouseInit);
        var qA = q;
        var ctl = BoolBl.State(() => {
          return Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
        });

        IntBl Idx = 0;
        Idx = (qA[1].Abs > qA[Idx].Abs).Condition(1, Idx);
        //Idx = (qA[2].Abs > qA[Idx].Abs).Condition(2, Idx);
        var mask = new Point3DBl(
          (Idx == 0).Condition(1d, 0d),
          (Idx == 1).Condition(1d, 0d),
          (Idx == 2).Condition(0d, 0d));
        var qXYZ = qA.XYZ() * mask;
        // renormalize.
        qA = new QuaternionBl(qXYZ, qA.W).Normalize;
        qA = ctl.Condition(qA, q);

        Action SetRotateB = QuaternionBl.Assign(Self.RotateB, qA);
        Action SetRotateA = QuaternionBl.Assign(Self.RotateA, Self.RotateA * Self.RotateB);
        Action ResetRotateB = QuaternionBl.Assign(Self.RotateB, QuaternionBl.Identity);

        sphere.IsHitTestVisible = false;
        outer.InstallDragHandler(() => {
          SetMouseInit();
          return true;
        }, SetRotateB, () => {
          SetRotateA();
          ResetRotateB();
        });
      }
    }
  }
  public class VirtualSphereBl : CanvasBl<VirtualSphere, VirtualSphereBl> {
    public VirtualSphereBl(Expr<VirtualSphere> e) : base(e) { }
    public VirtualSphereBl(CanvasBl Parent, DoubleBl Buffer) :
      base(Parent, new VirtualSphere(Buffer)) { }
    public static implicit operator VirtualSphereBl(Expr<VirtualSphere> o) { return (new VirtualSphereBl(o)); }
    public static implicit operator VirtualSphereBl(VirtualSphere o) { return (new Constant<VirtualSphere>(o)); }
    static VirtualSphereBl() {
      Register(o => o);
    }
    public QuaternionBl RotateA {
      get { return VirtualSphere.RotateAProperty[this]; }
      set { RotateA.Bind = value; }
    }
    public QuaternionBl RotateB {
      get { return VirtualSphere.RotateBProperty[this]; }
      set { RotateB.Bind = value; }
    }
    public Point3DBl MouseInit {
      get { return VirtualSphere.MouseInitProperty[this]; }
      set { MouseInit.Bind = value; }
    }
  }

  public class HasOverlay : Canvas, ICanStart {
    public static GetProperty<HasOverlay,Vecs.Vec2<double>> GlobalPositionProperty =
      "GlobalPosition".NewProperty<HasOverlay, Vecs.Vec2<double>>();
    public static GetProperty<HasOverlay, Canvas> OverlayContentProperty =
      "OverlayContent".NewProperty<HasOverlay, Canvas>();
    public void Start() { OpenOverlay(); }
    public void Stop() { CloseOverlay(); }

    private Window ParentWindow;
    private WindowBl OverlayWindow;
    private readonly EventHandler Recompute;

    internal HasOverlay() {
      Recompute = (x, y) => this.DoRecompute();
      var self = this.Bl();
      var canvas = new CanvasBl();
      self.OverlayContent = canvas;
      self.OverlayContent.LeftTop = 0d;
      self.OverlayContent.Size = this.Bl().Size;
    }
    private void OpenOverlay() {
      (((object)OverlayWindow) == null).Assert();
      OverlayWindow = new WindowBl() {
        WindowStyle = WindowStyle.None,
        AllowsTransparency = true,
        Background = Brushes.Transparent,
        Content = this.Bl().OverlayContent,
        Size = this.Bl().Size,
        //Topmost = true,
        LeftTop = this.Bl().GlobalPosition,
        ShowInTaskbar = false,
        Owner = ParentWindow,
        
      };
      OverlayWindow.CurrentValue.Show();
    }
    private void CloseOverlay() {
      if (((object)OverlayWindow) == null) return;
      OverlayWindow.Content = new LabelBl();
      OverlayWindow.CurrentValue.Close();
      OverlayWindow = null;
    }

    private void DoRecompute() {
      CloseOverlay();
      PointBl p = 0d;
      var at = (FrameworkElement)this.Parent;
      BoolBl IsVisible = true;
      while (true) {
        //IsVisible = IsVisible & at.Bl().IsVisible;
        if (at.Parent is Canvas) p += at.Bl().LeftTop;
        else if (at.Parent is Window) {
          ParentWindow = ((Window)at.Parent);
          var atP = ParentWindow.Bl().LeftTop;
          p += atP;
          var delta = Utils.TransformToScreen(new Point(0, 0), at);
          delta = delta - new Vector(atP.CurrentValue.X, atP.CurrentValue.Y);
          p += delta.Bl();
          break;
        }
        var child = at;
        at = (FrameworkElement)child.Parent;
        var parent = at;
        if (at == null) {
          RoutedEventHandler e = null;
          e = (x, y) => {
            child.Loaded -= e;
            DoRecompute();
          };
          child.Loaded += e;
          return;
        } 
      }
      this.Bl().GlobalPosition = p + this.Bl().LeftTop;
      OpenOverlay(); //this.Bl().IsStart);
      ParentWindow.Closed += (x, y) => CloseOverlay();
    }


    protected override void OnVisualParentChanged(DependencyObject oldParent) {
      base.OnVisualParentChanged(oldParent);
      DoRecompute();
    }
  }
  public class HasOverlayBl : CanvasBl<HasOverlay, HasOverlayBl> {
    public PointBl GlobalPosition {
      get { return HasOverlay.GlobalPositionProperty[Underlying]; }
      internal set { GlobalPosition.Bind = value; }
    }
    public CanvasBl OverlayContent {
      get { return HasOverlay.OverlayContentProperty[Underlying]; }
      internal set { OverlayContent.Bind = value; }
    }
    public HasOverlayBl(Expr<HasOverlay> e) : base(e) { }
    public HasOverlayBl(CanvasBl Parent) :
      base(Parent, new HasOverlay()) { }
    public static implicit operator HasOverlayBl(Expr<HasOverlay> o) { return (new HasOverlayBl(o)); }
    public static implicit operator HasOverlayBl(HasOverlay o) { return (new Constant<HasOverlay>(o)); }
    static HasOverlayBl() {
      Register(o => o);
    }
  }
  public static class CustomExtensions {
    public static HasOverlayBl Bl(this HasOverlay o) { return o; }
    public static VirtualSphereBl Bl(this VirtualSphere o) { return o; }
  }
}