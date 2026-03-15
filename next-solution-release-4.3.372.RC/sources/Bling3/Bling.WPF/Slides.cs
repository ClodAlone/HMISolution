using System;
using System.Collections.Generic;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Bling;
using Bling.Util;
using Bling.Core;
using Bling.DSL;
using Bling.WPF;

namespace Bling.Slides {
  public class SlideHolder : Canvas, ICanStart {
    private readonly CanvasBl Navigation;
    private readonly List<Func<FrameworkElementBl>> Slides = new List<Func<FrameworkElementBl>>();
    private readonly ContentControlBl Content;
    private readonly List<string> Names = new List<string>();
    private readonly List<FrameworkElementBl> Activated = new List<FrameworkElementBl>();
    public Func<FrameworkElementBl> this[string name] {
      set { Slides.Add(value); Names.Add(name); }
    }
    public void Start() {
      var v = Content.Content.CurrentValue;
      if (v != null) {
        if (v is ICanStart) ((ICanStart)v).Start();
        if (v is IHasGlobalPosition && v is FrameworkElement) {
          var at = (FrameworkElement)v;
          PointBl p = 0d;
          while (true) {
            if (at.Parent is Canvas) p += at.Bl().LeftTop;
            else if (at.Parent is Window) {
              var atP = ((Window)at.Parent).Bl().LeftTop;
              p += atP;
              var delta = Utils.TransformToScreen(new Point(0, 0), at);
              delta = delta - new Vector(atP.CurrentValue.X, atP.CurrentValue.Y);
              p += delta.Bl();
              break;
            }
            at = (FrameworkElement)at.Parent;
          }
          ((IHasGlobalPosition)v).SetGlobalPosition(p);
        }
      }
    }
    public void Stop() {
      var v = Content.Content.CurrentValue;
      if (v != null) {
        if (v is ICanStart) ((ICanStart)v).Stop();
        if (v is IHasGlobalPosition && v is FrameworkElement) {
          ((IHasGlobalPosition)v).UnsetGlobalPosition();
        }
      }
    }
    private void Show(int n) {
      Stop();
      if (((object)Activated[n]) == null) Activated[n] = Slides[n]();
      Content.Content = Activated[n];
      Start();
    }
    public void Init() {
      Stop();
      Navigation.Children.Clear();
      Activated.Clear();

      var Reset = new ButtonBl(Navigation) {
        Content = "Reset",
        LeftTop = new PointBl(0, 0),
        Click = Init,
      };
      var At = Reset.RightTop;
      for (int i = 0; i < Slides.Count; i++) {
        Activated.Add(null);
        var ii = i;
        var b = new ButtonBl(Navigation) {
          Content = Names[i],
          Click = () => Show(ii),
        };
        b.LeftTop = (At.X + b.Width > Navigation.Width).Condition(new PointBl(0, At.Y + Reset.Height), At);
        At = b.RightTop;
      }
      Navigation.Height = At.Y + Reset.Height;
      Show(0);
    }
    public SlideHolder() {
      Navigation = new CanvasBl(this) {
        LeftTop = new PointBl(0, 0),
        Width = this.Bl().Width,
      };
      Content = new ContentControlBl(this) {
        LeftTop = Navigation.LeftBottom,
        Width = this.Bl().Width,
        Height = this.Bl().Height - Navigation.Height,
      };


    }

  }

}