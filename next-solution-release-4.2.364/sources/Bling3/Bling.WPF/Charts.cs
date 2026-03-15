using System;
using System.Collections.Generic;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Bling.DSL;
using Bling.Util;
using Bling.Core;
using Bling.WPF;
using Bling.Shapes;

namespace Bling.Charts {
  /// <summary>
  /// A pie chart, the radius of the pie depends on its dimensions, which should be the same (Width = Height). 
  /// </summary>
  public class Pie : Canvas {
    private readonly DoubleBl[] Slices;
    private readonly PiePiece[] pieces;
    /// <summary>
    /// Total value for all slices of the pie. 
    /// </summary>
    public readonly DoubleBl Total;
    /// <summary>
    /// Create a new pie with specified slices.
    /// </summary>
    /// <param name="Slices">Slices of the pie, will be used to sequentially create pie pieces.</param>
    public Pie(params DoubleBl[] Slices) {
      this.Slices = Slices;
      Total = 0d;
      foreach (var s in Slices) Total += s;
      pieces = new PiePiece[Slices.Length];
      Angle2DBl rotation = 0.PI();
      for (int i = 0; i < pieces.Length; i++) {
        pieces[i] = new PiePieceBl(this.Bl()) {
          Fill = i.SelectColor(),
          LeftTop = this.Bl().CenterSize,
          InnerRadius = 0, Radius = this.Bl().Width / 2d,
          WedgeAngle = 2d.PI() * Slices[i] / Total,
          RenderTransform = {
            Origin = new PointBl(0, 0),
            Rotate = { AsRadians = rotation },
          },
        };
        rotation += pieces[i].Bl().WedgeAngle;
      }
    }
    /// <summary>
    /// Pie piece for slice.
    /// </summary>
    /// <param name="index">Index of slice to retrieve pie piece for.</param>
    /// <returns></returns>
    public PiePiece this[int index] { get { return pieces[index]; } }
    /// <summary>
    /// Rotation, in radians, of pie piece for slice.
    /// </summary>
    /// <param name="index">Index of slice to retrieve rotation for.</param>
    /// <returns></returns>
    public Angle2DBl Rotation(int index) {
      //return this[index].Bl().RenderTransform[new PointBl(0, 0)].Rotate.AsRadians - .5.PI();
      return this[index].Bl().RenderTransform[new PointBl(0, 0)].Rotate.AsRadians;
    }
  }
  /// <summary>
  /// A pie chart, the radius of the pie depends on its dimensions, which should be the same (Width = Height). 
  /// </summary>
  public class PieBl : CanvasBl<Pie, PieBl> {
    public PieBl(Expr<Pie> v) : base(v) { }
    /// <summary>
    /// Create a new pie with specified slices.
    /// </summary>
    /// <param name="Parent">Canvas that contains this pie.</param>
    /// <param name="Slices">Slices of the pie, will be used to sequentially create pie pieces.</param>
    public PieBl(CanvasBl Parent, params DoubleBl[] Slices) : base(Parent, new Pie(Slices)) { }
    public static implicit operator PieBl(Expr<Pie> v) { return new PieBl(v); }
    public static implicit operator PieBl(Pie v) { return new Constant<Pie>(v); }
    static PieBl() { Register(v => v); }
    /// <summary>
    /// Pie piece for slice.
    /// </summary>
    /// <param name="index">Index of slice to retrieve pie piece for.</param>
    /// <returns></returns>
    public PiePieceBl this[int index] { get { return CurrentValue[index]; } }
    /// <summary>
    /// Rotation, in radians, of pie piece for slice.
    /// </summary>
    /// <param name="index">Index of slice to retrieve rotation for.</param>
    /// <returns></returns>
    public Angle2DBl Rotation(int index) {
      return CurrentValue.Rotation(index);
    }
    /// <summary>
    /// Total value for all slices of the pie. 
    /// </summary>
    public DoubleBl Total { get { return CurrentValue.Total; } }
  }
}