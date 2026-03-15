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
using System.Windows.Controls.Primitives;
using Bling.Core;
using Bling.DSL;
using Bling.WPF;
using Bling.Util;


namespace Bling.WPF {
  using Bling.Matrices;

  public partial class BrushBl<T, BRAND> : ObjectBl<T, BRAND>, IExtends<BRAND,BrushBl>, IDependencyObjectBl<T, BRAND>
    where T : Brush
    where BRAND : BrushBl<T, BRAND> {
    /// <summary>
    /// Gets or sets the degree of opacity.
    /// </summary>
    public DoubleBl Opacity {
      get { return Underlying.Property<double>(Brush.OpacityProperty).Bl(); }
      set { Opacity.Bind = value; }
    }
    public Bling.WPF.TransformBl Transform {
      get {
        return new Bling.WPF.TransformBl(Underlying.Property<Transform>(Brush.TransformProperty)) {};
      }
      set { Transform.Bind = value; }
    }
    public Bling.WPF.TransformBl RelativeTransform {
      get {
        return new Bling.WPF.TransformBl(Underlying.Property<Transform>(Brush.RelativeTransformProperty)) {
          Origin = new PointBl(.5, .5),
        };
      }
      set { RelativeTransform.Bind = value; }
    }
  }
  public partial class BrushBl {
    public static implicit operator BrushBl(ImageSource image) {
      return new Bling.WPF.ImageBrushBl() { ImageSource = image };
    }
    public static implicit operator BrushBl(GeometryBl geometry) {
      return new DrawingBrushBl() { Drawing = new GeometryDrawingBl() { Geometry = geometry } };
    }
    public static implicit operator BrushBl(DrawingBl drawing) {
      return new DrawingBrushBl() { Drawing = drawing };
    }
    public static implicit operator BrushBl(ColorBl color) {
      return new Bling.WPF.SolidColorBrushBl() { Color = color };
    }
    public static implicit operator BrushBl(Color color) {
      return ((ColorBl)color);
    }
    public static explicit operator SolidColorBrushBl(BrushBl from) {
      return from.DownCast<SolidColorBrushBl>();
    }
    public static explicit operator ImageBrushBl(BrushBl from) {
      return from.DownCast<ImageBrushBl>();
    }
  }


  /// <summary>
  /// Anything that undergoes rendering.
  /// </summary>
  public abstract class RenderBl<T, BRAND> : Brand<T, BRAND>, ICoreBrandT, IDependencyObjectBl<T,BRAND>
    where T : DependencyObject
    where BRAND : RenderBl<T, BRAND> {
    public RenderBl(Expr<T> Underlying) : base(Underlying) { }
    public RenderBl() : base() { }
    /// <summary>
    /// Either Unspecified or Aliased.  If unspecified, no edge mode is specified. Do not alter the current edge mode of non-text
    ///     drawing primitives. This is the default value.
    /// If Aliased, render the edges of non-text drawing primitives as aliased edges.
    /// </summary>
    public EnumBl<EdgeMode> EdgeMode {
      get { return Underlying.Property<EdgeMode>(RenderOptions.EdgeModeProperty); }
      set { EdgeMode.Bind = value; }
    }
    /// <summary>
    /// When animating the scale of any bitmap, the default high-quality image re-sampling algorithm 
    /// can sometimes consume sufficient system resources to cause frame rate degradation, effectively 
    /// causing animations to stutter. By setting the BitmapScalingMode property of the RenderOptions 
    /// object to LowQuality you can create a smoother animation when scaling a bitmap. LowQuality 
    /// mode tells the WPF rendering engine to switch from a quality-optimized algorithm to a speed-optimized 
    /// algorithm when processing images.
    /// </summary>
    public EnumBl<BitmapScalingMode> BitmapScalingMode {
      get { return Underlying.Property<BitmapScalingMode>(RenderOptions.BitmapScalingModeProperty); }
      set { BitmapScalingMode.Bind = value; }
    }
    /// <summary>
    /// The idea is you set CacheHint to Cache to tell us to try to avoid updating the brush. 
    /// Now if you scaled the brush way up, it could look terrible and if you scale it down 
    /// it could look bad as well. So min/max thresholds are relative scales to the initial 
    /// size of the brush that tell us when to update things again. For example, if you set 
    /// ThresholdMinimum to .5 and the brush size halves, we’ll update the brush and cache the new result.
    /// </summary>
    public EnumBl<CachingHint> CachingHint {
      get { return Underlying.Property<CachingHint>(RenderOptions.CachingHintProperty); }
      set { CachingHint.Bind = value; }
    }
    private class CacheInvalidationThresholdSet : RangeBl<DoubleBl,DoubleBl> {
      internal Expr<T> Underlying;
      public override DoubleBl Minimum {
        get { return Underlying.Property<double>(RenderOptions.CacheInvalidationThresholdMinimumProperty); }
        set { Minimum.Bind = value; }
      }
      public override DoubleBl Maximum {
        get { return Underlying.Property<double>(RenderOptions.CacheInvalidationThresholdMaximumProperty); }
        set { Maximum.Bind = value; }
      }
    }
    /// <summary>
    /// WPF has introduced the cache scheme for those pre-rasterized content, 
    /// you could twist the cache hint through RenderOptions.CacheInvalidationThresholdMaximum 
    /// and  RenderOptions.CacheInvalidationThresholdMinimum attached properties, those two 
    /// properties could help to determine in what extent the rasterized bitmap will be scaled up 
    /// and down, it the scaling factor exceeds the threshold specified by 
    /// RenderOptions.CacheInvalidationThresholdMaximum and  RenderOptions.CacheInvalidationThresholdMinimum 
    /// attached properties, WPF will regenerate those bitmap by kicking off the software rendering pipeline again.
    /// </summary>
    public RangeBl<DoubleBl,DoubleBl> CacheInvalidationThreshold {
      get { return new CacheInvalidationThresholdSet() { Underlying = Underlying }; }
      set { CacheInvalidationThreshold.Bind(value); }
    }
  }
  /// <summary>
  ///     Provides rendering support in WPF, which includes hit testing, coordinate
  ///     transformation, and bounding box calculations.
  /// </summary>
  public abstract class VisualBl<T, BRAND> : RenderBl<T, BRAND>, IExtends<BRAND,VisualBl>
    where T : Visual
    where BRAND : VisualBl<T, BRAND> {
    public VisualBl(Expr<T> Underlying) : base(Underlying) { }
    public VisualBl() : base() { }
    public static implicit operator VisualBl(VisualBl<T, BRAND> mat) {
      return VisualBl.UpCast(mat);
    }
  }
  /// <summary>
  ///     Provides rendering support in WPF, which includes hit testing, coordinate
  ///     transformation, and bounding box calculations.
  /// </summary>
  public class VisualBl : VisualBl<Visual, VisualBl> {
    public VisualBl() : base() { }
    public VisualBl(Expr<Visual> Underlying) : base(Underlying) { }
    public static implicit operator VisualBl(Expr<Visual> target) { return new VisualBl((target)); }
    public static implicit operator VisualBl(Visual target) { return (new Constant<Visual>(target)); }
    static VisualBl() { Register(v => v); }
  }

  public abstract class UIElementBl<T, BRAND> : VisualBl<T, BRAND>, IExtends<BRAND,UIElementBl>
    where T : UIElement
    where BRAND : UIElementBl<T, BRAND> {
    public UIElementBl(Expr<T> Underlying) : base(Underlying) { }
    public UIElementBl() : base() { }
    public static implicit operator UIElementBl(UIElementBl<T, BRAND> mat) {
      return UIElementBl.UpCast(mat);
    }
  }
  public class UIElementBl : UIElementBl<UIElement, UIElementBl> {
    public UIElementBl() : base(new Constant<UIElement>(null)) { }
    public UIElementBl(Expr<UIElement> Underlying) : base(Underlying) { }
    public static implicit operator UIElementBl(Expr<UIElement> target) { return new UIElementBl((target)); }
    public static implicit operator UIElementBl(UIElement target) { return (new Constant<UIElement>(target)); }
    static UIElementBl() { Register(v => v); }
  }

  public abstract partial class TransformBl<T, BRAND> : Brand<T, BRAND>, IExtends<BRAND,TransformBl>
    where T : Transform
    where BRAND : TransformBl<T, BRAND> {
    public TransformBl(Expr<T> v) : base(v) { }
    public static implicit operator TransformBl(TransformBl<T,BRAND> self) {
      var ret = TransformBl.UpCast((Brand<BRAND>)self);
      return ret;
    }
    static TransformBl() {
      TransformBl.CheckInit();
    }
  }
  public partial class TransformBl : TransformBl<Transform, TransformBl> {
    public TransformBl(Expr<Transform> v) : base(v) { }
    static TransformBl() {
      Register(v => v);
    }
    public static implicit operator TransformBl(Expr<Transform> v) { return new TransformBl(v); }
    public static implicit operator TransformBl(Transform t) { return new Constant<Transform>(t); }

    public static void CheckInit() { Extensions.Trace("CheckInit: " + typeof(TransformBl) + " ToBrand=" + ToBrand); }


    /// <summary>
    /// Transforms the specified point according to the transform matrix.
    /// </summary>
    /// <param name="p">Point to be transformed.</param>
    /// <returns>Transformed point.</returns>
    public PointBl Transform(PointBl p) {
      return (p).Combine(this).Map((q, t) => { 
        var w = t.Transform(new Point(q.X, q.Y));
        return new Vecs.Vec2<double>() { X = w.X, Y = w.Y };
      });
    }
    /// <summary>
    /// The inverse of this matrix. 
    /// </summary>
    public TransformBl Inverse {
      get { return this.Map(t => (Transform) t.Inverse, t => (Transform) t.Inverse); }
    }
    /// <summary>
    /// Gets or sets the center point of any possible transform declared, relative to the bounds of the element.
    /// </summary>
    public PointBl Origin = new PointBl(0, 0);

    /// <summary>
    /// Set the center point of the scale/skew/rotate transform .
    /// </summary>
    /// <param name="c">Center point of rotation.</param>
    /// <returns>Itself.</returns>
    public TransformBl this[PointBl c] {
      get {
        this.Origin = c;
        return this;
      }
    }
    private TRANSFORM Transform<TRANSFORM>() where TRANSFORM : Transform, new() {
      TRANSFORM transform;
      if (Underlying.HasCurrentValue) {
        var current = (Transform)this.CurrentValue;
        if (current is TRANSFORM) transform = (TRANSFORM)current;
        else if (current is TransformGroup) {
          var group = (TransformGroup)current;
          transform = null;
          foreach (var v in group.Children) {
            if (v is TRANSFORM) transform = (TRANSFORM)v;
          }
          if (transform == null) {
            transform = new TRANSFORM();
            group.Children.Add(transform);
          }
        } else if (current == null) {
          transform = new TRANSFORM();
          this.Now = (transform);
        } else {
          var group = new TransformGroup();
          group.Children.Add(current);
          transform = new TRANSFORM();
          group.Children.Add(transform);
          this.Now = (group);
        }
      } else {
        throw new NotSupportedException();
      }
      return transform;
    }
    /// <summary>
    /// Rotate the UI element around center.
    /// </summary>
    public DegreeBl Rotate  {
      get {
        var rotate = Transform<RotateTransform>();
        var px = rotate.Bl<double>(RotateTransform.CenterXProperty).Bl();
        var py = rotate.Bl<double>(RotateTransform.CenterYProperty).Bl();
        (new PointBl(px, py)).Bind = Origin;
        var degrees = rotate.Bl<double>(RotateTransform.AngleProperty).Bl();
        return new DegreeBl(degrees);
      }
      set {
        Rotate.Bind = value;
      }
    }
    /// <summary>
    /// Scale the UI element around center.
    /// </summary>
    /// <param name="center">the center point of the scaling</param>
    /// <returns></returns>
    public PointBl Scale {
      get {
        var scale = Transform<ScaleTransform>();
        var px = scale.Bl<double>(ScaleTransform.CenterXProperty).Bl();
        var py = scale.Bl<double>(ScaleTransform.CenterYProperty).Bl();
        (new PointBl(px, py)).Bind = Origin;

        var sx = scale.Bl<double>(ScaleTransform.ScaleXProperty).Bl();
        var sy = scale.Bl<double>(ScaleTransform.ScaleYProperty).Bl();
        return new PointBl(sx, sy);
      }
      set { Scale.Bind = value; }
    }

    /// <summary>
    /// Set scale for both X and Y dimensions around center.
    /// </summary>
    public DoubleBl ScaleXY {
      get {
        var ret = Scale;
        ret.X = ret.Y;
        return ret.Y;
      }
      set {
        ScaleXY.Bind = value;
        //Scale.Bind = new PointBl(value, value);
      }
    }

    /// <summary>
    /// Skew the UI element.
    /// </summary>
    public DegreePointBl Skew {
      get {
        var skew = Transform<SkewTransform>();
        var px = skew.Bl<double>(SkewTransform.CenterXProperty).Bl();
        var py = skew.Bl<double>(SkewTransform.CenterYProperty).Bl();
        (new PointBl(px, py)).Bind = Origin;

        var sx = skew.Bl<double>(SkewTransform.AngleXProperty).Bl().ToDegrees();
        var sy = skew.Bl<double>(SkewTransform.AngleYProperty).Bl().ToDegrees();
        return new DegreePointBl(sx, sy);
      }
      set {
        this.Skew.Bind = value;
      }
    }
    /// <summary>
    /// Translate the UI element.
    /// </summary>
    public PointBl Translate {
      get {
        var trans = Transform<TranslateTransform>();
        var px = trans.Bl<double>(TranslateTransform.XProperty).Bl();
        var py = trans.Bl<double>(TranslateTransform.YProperty).Bl();
        return new PointBl(px, py);
      }
      set {
        this.Translate.Bind = value;
      }
    }
  }
  public class MatrixTransformBl : TransformBl<MatrixTransform, MatrixTransformBl> {
    public MatrixTransformBl(Expr<MatrixTransform> Provider) : base(Provider) { }
    public MatrixTransformBl() : this(new Constant<MatrixTransform>(new MatrixTransform())) { }
    public static implicit operator MatrixTransformBl(Expr<MatrixTransform> v) { return new MatrixTransformBl(v); }
    public static implicit operator MatrixTransformBl(MatrixTransform v) { return new Constant<MatrixTransform>(v); }
    static MatrixTransformBl() {
      Register(v => v);
    }
    /*
    public IBaseArray<DoubleBl> MatrixOld {
      get {
        var Core = Underlying.Property<Matrix>(MatrixTransform.MatrixProperty);
        return Core.BlOld();
      }
      set {
        var Core = Underlying.Property<Matrix>(MatrixTransform.MatrixProperty);
        Core.Bind = value.AsMatrix();
      }
    }
     */
    public MatrixBl<D3,D3> Matrix {
      get {
        var Core = Underlying.Property<Matrix>(MatrixTransform.MatrixProperty);
        return Core.Bl();
      }
      set {
        var Core = Underlying.Property<Matrix>(MatrixTransform.MatrixProperty);
        Core.Bind = value.AsMatrix();
      }
    }
  }


  /// <summary>
  ///     Paints an area with a solid color.
  /// </summary>
  public class SolidColorBrushBl : BrushBl<SolidColorBrush, SolidColorBrushBl> {
    public SolidColorBrushBl(Expr<SolidColorBrush> Provider) : base(Provider) { }
    public SolidColorBrushBl() : this(new Constant<SolidColorBrush>(new SolidColorBrush())) { }
    /// <summary>
    ///     Gets or sets the color of this System.Windows.Media.SolidColorBrush. This
    ///     is a dependency property. The default value is System.Windows.Media.Colors.Transparent.
    /// </summary>
    public ColorBl Color {
      get { return Underlying.Property<Color>(SolidColorBrush.ColorProperty); }
      set { Color.Bind = value; }
    }
    public static implicit operator SolidColorBrushBl(Expr<SolidColorBrush> v) { return new SolidColorBrushBl(v); }
    public static implicit operator SolidColorBrushBl(SolidColorBrush v) { return new Constant<SolidColorBrush>(v); }
    static SolidColorBrushBl() {
      Register(v => v);
    }
  }
  /// <summary>
  ///     Paints an area with an image.
  /// </summary>
  public class ImageBrushBl : TileBrushBl<ImageBrush, ImageBrushBl> {
    public ImageBrushBl(Expr<ImageBrush> Provider) : base(Provider) { }
    public ImageBrushBl() : this(new Constant<ImageBrush>(new ImageBrush())) { }
    public ImageBrushBl(ImageSourceBl Source) : this() { ImageSource = Source; } 
    /// <summary>
    ///     Gets or sets the image displayed by this System.Windows.Media.ImageBrush.
    /// </summary>
    public ImageSourceBl ImageSource {
      get { return Underlying.Property<ImageSource>(ImageBrush.ImageSourceProperty).Bl(); }
      set { ImageSource.Bind = value; }
    }
    public static implicit operator ImageBrushBl(Expr<ImageBrush> v) { return new ImageBrushBl(v); }
    public static implicit operator ImageBrushBl(ImageBrush v) { return new Constant<ImageBrush>(v); }
    
    static ImageBrushBl() {
      Register(v => v);
    }
  }
  /// <summary>
  ///     Paints an area with an image.
  /// </summary>
  public class VisualBrushBl : TileBrushBl<VisualBrush, VisualBrushBl> {
    public VisualBrushBl(Expr<VisualBrush> Provider) : base(Provider) { }
    public VisualBrushBl() : this(new Constant<VisualBrush>(new VisualBrush())) { }
    public VisualBl Visual {
      get { return Underlying.Property<Visual>(VisualBrush.VisualProperty); }
      set { Visual.Bind = value; }
    }
    public static implicit operator VisualBrushBl(Expr<VisualBrush> v) { return new VisualBrushBl(v); }
    public static implicit operator VisualBrushBl(VisualBrush v) { return new Constant<VisualBrush>(v); }

    static VisualBrushBl() {
      Register(v => v);
    }
  }


  /// <summary>
  ///     Abstract class that describes a 2-D drawing. 
  /// </summary>
  public class DrawingBl<T,BRAND> : Brand<T,BRAND>, IExtends<BRAND,DrawingBl> where T : Drawing where BRAND : DrawingBl<T,BRAND> {
    static DrawingBl() { DrawingBl.CheckInit(); }
    public DrawingBl(Expr<T> v) : base(v) { }
    public static implicit operator DrawingBl(DrawingBl<T, BRAND> b) {
      return DrawingBl.UpCast(b);
    }
  }

  /// <summary>
  ///     Abstract class that describes a 2-D drawing. 
  /// </summary>
  public class DrawingBl : DrawingBl<Drawing, DrawingBl> {
    public DrawingBl(Expr<Drawing> v) : base(v) { }
    public static implicit operator DrawingBl(Expr<Drawing> v) { return new DrawingBl(v); }
    public static implicit operator DrawingBl(Drawing d) { return new Constant<Drawing>(d); }
    static DrawingBl() {
      Register(v => v);
    }
    public static void CheckInit() { Extensions.Trace("CheckInit: " + typeof(DrawingBl) + " ToBrand=" + ToBrand); }
  }
  /// <summary>
  ///     Draws a System.Windows.Media.Geometry using the specified System.Windows.Media.GeometryDrawing.Brush
  ///     and System.Windows.Media.GeometryDrawing.Pen.
  /// </summary>
  public class GeometryDrawingBl : DrawingBl<GeometryDrawing, GeometryDrawingBl> {
    public GeometryDrawingBl(Expr<GeometryDrawing> v) : base(v) {
      typeof(GeometryBl).GetHashCode();
    }
    public GeometryDrawingBl() : base(new Constant<GeometryDrawing>(new GeometryDrawing())) { }
    public static implicit operator GeometryDrawingBl(Expr<GeometryDrawing> v) { return new GeometryDrawingBl(v); }
    public static implicit operator GeometryDrawingBl(GeometryDrawing d) { return new Constant<GeometryDrawing>(d); }
    static GeometryDrawingBl() {
      Register(v => v);
    }
    /// <summary>
    ///     Gets or sets the System.Windows.Media.Brush used to fill the interior of
    ///     the shape described by this System.Windows.Media.GeometryDrawing. This is
    ///     a dependency property. The default
    ///     value is null.
    /// </summary>
    public BrushBl Brush {
      get { return Underlying.Property<Brush>(GeometryDrawing.BrushProperty); }
      set { Brush.Bind = value; }
    }
    /// <summary>
    ///     Gets or sets the System.Windows.Media.Geometry that describes the shape of
    ///     this System.Windows.Media.GeometryDrawing. This is a dependency property. The default
    ///     value is null.
    /// </summary>
    public GeometryBl Geometry {
      get { return Underlying.Property<Geometry>(GeometryDrawing.GeometryProperty); }
      set { Geometry.Bind = value; }
    }

    public PenBl Pen {
      get { return Underlying.Property<Pen>(GeometryDrawing.PenProperty); }
      set { Pen.Bind = value; }
    }
  }
  /// <summary>
  ///     Represents the sequence of dashes and gaps that will be applied by a System.Windows.Media.Pen.
  /// </summary>
  public class DashStyleBl : Brand<DashStyle, DashStyleBl> {
    public DashStyleBl(Expr<DashStyle> v) : base(v) { }
    public DashStyleBl() : this(new Constant<DashStyle>(new DashStyle())) { }
    public static implicit operator DashStyleBl(Expr<DashStyle> v) { return new DashStyleBl(v); }
    public static implicit operator DashStyleBl(DashStyle d) { return new Constant<DashStyle>(d); }
    static DashStyleBl() { Register(v => v); }

    /// <summary>
    ///     Gets or sets the collection of dashes and gaps in this System.Windows.Media.DashStyle.
    ///     This is a dependency property. The default is an empty System.Windows.Media.DoubleCollection.
    /// </summary>
    public DoubleCollectionBl Dashes {
      get { return Underlying.Property<DoubleCollection>(DashStyle.DashesProperty); }
      set { Dashes.Bind = value; }
    }
    /// <summary>
    ///     Gets or sets how far in the dash sequence the stroke will start. This is
    ///     a dependency property.
    ///     The offset for the dash sequence. The default is 0.
    /// </summary>
    public DoubleBl Offset {
      get { return Underlying.Property<double>(DashStyle.OffsetProperty); }
      set { Offset.Bind = value; }
    }
  }

  /// <summary>
  ///     Describes how a shape is outlined.
  /// </summary>
  public class PenBl : Brand<Pen, PenBl> {
    public PenBl(Expr<Pen> v) : base(v) { }
    public PenBl() : this(new Constant<Pen>(new Pen())) { }
    public static implicit operator PenBl(Expr<Pen> v) { return new PenBl(v); }
    public static implicit operator PenBl(Pen d) { return new Constant<Pen>(d); }
    static PenBl() { Register(v => v); }

    /// <summary>
    ///     Gets or sets the fill the outline produced by this System.Windows.Media.Pen.
    ///     This is a dependency property. The default
    ///     value is null.
    /// </summary>
    public BrushBl Brush {
      get { return Underlying.Property<Brush>(Pen.BrushProperty); }
      set { Brush.Bind = value; }
    }
    /// <summary>
    ///     Gets or sets a value that describes the pattern of dashes generated by this
    ///     System.Windows.Media.Pen. This is a dependency property.
    ///     A value that describes the pattern of dashes generated by this System.Windows.Media.Pen.
    ///     The default is System.Windows.Media.DashStyles.Solid, which indicates that
    ///     there should be no dashes.
    /// </summary>
    public DashStyleBl DashStyle {
      get { return Underlying.Property<DashStyle>(Pen.DashStyleProperty); }
      set { DashStyle.Bind = value; }
    }
    /// <summary>
    ///     Gets or sets a value that specifies how the ends of each dash are drawn.
    ///     This is a dependency property. This setting applies to both
    ///     ends of each dash. The default value is System.Windows.Media.PenLineCap.Flat.
    /// </summary>
    public EnumBl<PenLineCap> DashCap {
      get { return Underlying.Property<PenLineCap>(Pen.DashCapProperty); }
      set { DashCap.Bind = value; }
    }
    /// <summary>
    ///     Gets or sets the type of shape to use at the end of a stroke. This is a dependency
    ///     property. The type of shape that ends the stroke. The default value is System.Windows.Media.PenLineCap.Flat.
    /// </summary>
    public EnumBl<PenLineCap> EndLineCap {
      get { return Underlying.Property<PenLineCap>(Pen.EndLineCapProperty); }
      set { EndLineCap.Bind = value; }
    }
    /// <summary>
    ///     Gets or sets the type of shape to use at the beginning of a stroke. This
    ///     is a dependency property. The type of shape that starts the stroke. The default value is System.Windows.Media.PenLineCap.Flat.
    /// </summary>
    public EnumBl<PenLineCap> StartLineCap {
      get { return Underlying.Property<PenLineCap>(Pen.StartLineCapProperty); }
      set { StartLineCap.Bind = value; }
    }
    /// <summary>
    ///     Gets or sets the type of joint used at the vertices of a shape's outline.
    ///     This is a dependency property. The type of joint used at the vertices of a shape's outline. The default
    ///     value is System.Windows.Media.PenLineJoin.Miter.
    /// </summary>
    public EnumBl<PenLineJoin> LineJoin {
      get { return Underlying.Property<PenLineJoin>(Pen.LineJoinProperty); }
      set { LineJoin.Bind = value; }
    }
    /// <summary>
    ///     Gets or sets the limit on the ratio of the miter length to half this pen's
    ///     System.Windows.Media.Pen.Thickness. This is a dependency property.
    ///     The limit on the ratio of the miter length to half the pen's System.Windows.Media.Pen.Thickness.
    ///     This value is always a positive number greater than or equal to 1. The default
    ///     value is 10.0.
    /// </summary>
    public DoubleBl MiterLimit {
      get { return Underlying.Property<double>(Pen.MiterLimitProperty); }
      set { MiterLimit.Bind = value; }
    }
    /// <summary>
    ///     Gets or sets the thickness of the stroke produced by this System.Windows.Media.Pen.
    ///     This is a dependency property. The thickness of the stroke produced by this System.Windows.Media.Pen. Default
    ///     is 1.
    /// </summary>
    public DoubleBl Thickness {
      get { return Underlying.Property<double>(Pen.ThicknessProperty); }
      set { Thickness.Bind = value; }
    }
  }


  /// <summary>
  ///     Classes that derive from this abstract base class define geometric shapes.
  ///     System.Windows.Media.Geometry objects can be used for clipping, hit-testing,
  ///     and rendering 2-D graphic data.
  /// </summary>
  public class GeometryBl : GeometryBl<Geometry, GeometryBl> {
    public GeometryBl(Expr<Geometry> v) : base(v) { }
    public static implicit operator GeometryBl(Expr<Geometry> v) { return new GeometryBl(v); }
    public static implicit operator GeometryBl(Geometry d) { return new Constant<Geometry>(d); }
    static GeometryBl() {
      Register(v => v);
    }


    /// <summary>
    /// Creat a line geometry.
    /// </summary>
    /// <param name="Start">Start point of line.</param>
    /// <param name="End">End point of line.</param>
    public static GeometryBl Line(PointBl Start, PointBl End) {
      var ret = new LineGeometry();
      ret.Bl<Point>(LineGeometry.StartPointProperty).Bl().Bind = Start;
      ret.Bl<Point>(LineGeometry.  EndPointProperty).Bl().Bind = End;
      return new GeometryBl(new Constant<Geometry>(ret));
    }
    /// <summary>
    /// Create an ellipse geometry.
    /// </summary>
    /// <param name="Center">Center point of geometry.</param>
    /// <param name="Radius">XY radii of ellipse.</param>
    /// <returns></returns>
    public static GeometryBl Ellipse(PointBl Center, PointBl Radius) {
      var ret = new EllipseGeometry();
      ret.Bl<Point>(EllipseGeometry.CenterProperty).Bl().Bind = Center;
      ret.Bl<double>(EllipseGeometry.RadiusXProperty).Bl().Bind = Radius.X;
      ret.Bl<double>(EllipseGeometry.RadiusYProperty).Bl().Bind = Radius.Y;
      return new GeometryBl(new Constant<Geometry>(ret));
    }
    /// <summary>
    /// Create a circle geometry.
    /// </summary>
    /// <param name="Center">Center point of circle.</param>
    /// <param name="Radius">Radius of circle.</param>
    /// <returns></returns>
    public static GeometryBl Circle(PointBl Center, DoubleBl Radius) {
      return Ellipse(Center, new PointBl(Radius, Radius));
    }
    /// <summary>
    /// Create a rectangle geometry.
    /// </summary>
    /// <param name="LeftTop">Left-top point of rectangle.</param>
    /// <param name="Size">Size of rectangle.</param>
    /// <param name="Radius">XY radius of curved corners (0 if no curves desired)</param>
    public static GeometryBl Rectangle(PointBl LeftTop, PointBl Size, PointBl Radius) {
      var ret = new RectangleGeometry();
      Point4DBl Rect = ret.Bl<Rect>(RectangleGeometry.RectProperty);
      Rect.Bind = new Point4DBl(LeftTop, Size);
      ret.Bl<double>(RectangleGeometry.RadiusXProperty).Bl().Bind = Radius.X;
      ret.Bl<double>(RectangleGeometry.RadiusYProperty).Bl().Bind = Radius.Y;
      return new GeometryBl(new Constant<Geometry>(ret));
    }
    /// <summary>
    /// Create a rectangle geometry with non-curved corners.
    /// </summary>
    /// <param name="LeftTop">Left-top point of rectangle.</param>
    /// <param name="Size">Size of rectangle.</param>
    public static GeometryBl Rectangle(PointBl LeftTop, PointBl Size) {
      return Rectangle(LeftTop, Size, 0);
    }
    /// <summary>
    /// Create a rectangle geometry with uniformed-curved corners.
    /// </summary>
    /// <param name="LeftTop">Left-top point of rectangle.</param>
    /// <param name="Size">Size of rectangle.</param>
    /// <param name="Radius">Radius of curved corners (0 if no curves desired)</param>
    public static GeometryBl Rectangle(PointBl LeftTop, PointBl Size, DoubleBl Radius) {
      return Rectangle(LeftTop, Size, new PointBl(Radius, Radius));
    }
    /// <summary>
    /// Create a square geometry.
    /// </summary>
    /// <param name="LeftTop">Left top point of square.</param>
    /// <param name="Size">Width and height of square.</param>
    public static GeometryBl Square(PointBl LeftTop, DoubleBl Size) {
      return Rectangle(LeftTop, new PointBl(Size, Size));
    }
  }
  /// <summary>
  ///     Describes a way to paint a region by using one or more tiles.
  /// </summary>
  public abstract class TileBrushBl<T,SELF> : BrushBl<T,SELF> where T : TileBrush where SELF : TileBrushBl<T,SELF> {
    public TileBrushBl(Expr<T> Provider) : base(Provider) { }
    public class AlignmentSet {
      internal Expr<T> Underlying;
      /// <summary>
      ///     A value that specifies the horizontal position of System.Windows.Media.TileBrush
      ///     content in its base tile. The default value is System.Windows.HorizontalAlignment.Center.
      /// </summary>
      public EnumBl<AlignmentX> X {
        get { return Underlying.Property<AlignmentX>(TileBrush.AlignmentXProperty); }
        set { X.Bind = value; }
      }
      /// <summary>
      ///     A value that specifies the vertical position of System.Windows.Media.TileBrush
      ///     content in its base tile. The default value is System.Windows.Media.AlignmentY.Center.
      /// </summary>
      public EnumBl<AlignmentY> Y {
        get { return Underlying.Property<AlignmentY>(TileBrush.AlignmentYProperty); }
        set { Y.Bind = value; }
      }
      internal AlignmentSet Bind {
        set { X = value.X; Y = value.Y; }
      }
    }
    /// <summary>
    ///     The alignment of System.Windows.Media.TileBrush
    ///     content in its base tile. The default value is Center, Center.
    /// </summary>
    public AlignmentSet Alignment {
      get { return new AlignmentSet() { Underlying = Underlying }; }
      set { Alignment.Bind = value; }
    }
    /// <summary>
    ///     A value that specifies how this System.Windows.Media.TileBrush content is
    ///     projected onto its base tile. The default value is System.Windows.Media.Stretch.Fill.
    /// </summary>
    public EnumBl<Stretch> Stretch {
      get { return Underlying.Property<Stretch>(TileBrush.StretchProperty); }
      set { Stretch.Bind = value; }
    }
    /// <summary>
    ///     A value that specifies how the System.Windows.Media.TileBrush tiles fill
    ///     the output area when the base tile, which is specified by the System.Windows.Media.TileBrush.Viewport
    ///     property, is smaller than the output area. The default value is System.Windows.Media.TileMode.None.
    /// </summary>
    public EnumBl<TileMode> TileMode {
      get { return Underlying.Property<TileMode>(TileBrush.TileModeProperty); }
      set { TileMode.Bind = value; }
    }
    /// <summary>
    /// Dimensions and units for the TileBrushe's base tile (Port) and tile content (Box).
    /// </summary>
    public ViewSet View {
      get { return new ViewSet() { Underlying = Underlying }; }
      set { View.Bind = value; }
    }
    public class ViewSet {
      internal Expr<T> Underlying;
      /// <summary>
      /// Dimensions and units for the TileBrush's base tile.
      /// </summary>
      public ElementSet Port {
        get { return new ElementSet() { Underlying = Underlying, DimProperty = TileBrush.ViewportProperty, UnitsProperty = TileBrush.ViewportUnitsProperty }; }
        set { Port.Bind = value; }
      }
      /// <summary>
      /// Dimensions and units for the content of a tile in the TileBrush.
      /// </summary>
      public ElementSet Box {
        get { return new ElementSet() { Underlying = Underlying, DimProperty = TileBrush.ViewboxProperty, UnitsProperty = TileBrush.ViewboxUnitsProperty }; }
        set { Box.Bind = value; }
      }
      public class ElementSet {
        internal Expr<T> Underlying;
        internal DependencyProperty DimProperty;
        internal DependencyProperty UnitsProperty;
        /// <summary>
        //     The position and dimensions of view.
        //     The default value is a rectangle (System.Windows.Rect) with a System.Windows.Rect.TopLeft
        //     of (0,0) and a System.Windows.Rect.Width and System.Windows.Rect.Height of
        //     1.
        /// </summary>
        public Point4DBl Dim {
          get { return Underlying.Property<Rect>(DimProperty); }
          set { Dim.Bind = value;  }
        }
        /// <summary>
        ///     Indicates whether the value of the view is relative to the size of the whole output area. The default value
        ///     is System.Windows.Media.BrushMappingMode.RelativeToBoundingBox.
        /// </summary>
        public EnumBl<BrushMappingMode> Units {
          get { return Underlying.Property<BrushMappingMode>(UnitsProperty); }
          set { Units.Bind = value; }
        }
        internal ElementSet Bind {
          set { Dim = value.Dim; Units = value.Units; }
        }
      }
      internal ViewSet Bind {
        set { Port = value.Port; Box = value.Box; }
      }
    }
  }
  public class DrawingBrushBl : TileBrushBl<DrawingBrush, DrawingBrushBl> {
    public DrawingBrushBl(Expr<DrawingBrush> Provider) : base(Provider) { }
    public DrawingBrushBl() : this(new Constant<DrawingBrush>(new DrawingBrush())) { }
    public DrawingBl Drawing {
      get { return Underlying.Property<Drawing>(DrawingBrush.DrawingProperty); }
      set { Drawing.Bind = value; }
    }
    public static implicit operator DrawingBrushBl(Expr<DrawingBrush> v) { return new DrawingBrushBl(v); }

    static DrawingBrushBl() {
      Register(v => v);
    }
  }

  public abstract class GradientBrushBl<T,SELF> : BrushBl<T,SELF> where T : GradientBrush where SELF : GradientBrushBl<T,SELF> {
    public GradientBrushBl(Expr<T> Provider) : base(Provider) { }
    /// <summary>
    /// Gets or sets a System.Windows.Media.ColorInterpolationMode enumeration that
    ///     specifies how the gradient's colors are interpolated.
    /// </summary>
    public EnumBl<ColorInterpolationMode> ColorInterpolationMode {
      get { return Underlying.Property<ColorInterpolationMode>(GradientBrush.ColorInterpolationModeProperty); }
      set { ColorInterpolationMode.Bind = value; }
    }
    /// <summary>
    /// Gets or sets the type of spread method that specifies how to draw a gradient
    ///     that starts or ends inside the bounds of the object to be painted.
    /// </summary>
    public EnumBl<GradientSpreadMethod> SpreadMethod {
      get { return Underlying.Property<GradientSpreadMethod>(GradientBrush.SpreadMethodProperty); }
      set { SpreadMethod.Bind = value; }
    }
    /// <summary>
    /// Gets or sets a System.Windows.Media.BrushMappingMode enumeration that specifies
    ///     whether the gradient brush's positioning coordinates are absolute or relative
    ///     to the output area. 
    /// </summary>
    public EnumBl<BrushMappingMode> MappingMode {
      get { return Underlying.Property<BrushMappingMode>(GradientBrush.MappingModeProperty); }
      set { MappingMode.Bind = value; } 
    }
    /// <summary>
    /// Describes the location and color of a transition point in a gradient
    /// </summary>
    /// <param name="color">the color of the gradient stop</param>
    /// <param name="offset">the location of the gradient stop within the gradient vector</param>
    /// <returns></returns>
    public SELF Stop(ColorBl color, DoubleBl offset) {
      var target = (GradientBrush)Underlying.CurrentValue;
      var stop = new GradientStop();
      
      stop.Bl<Color>(GradientStop.ColorProperty).Bl().Bind = color;
      stop.Bl<double>(GradientStop.OffsetProperty).Bl().Bind = offset;
      target.GradientStops.Add(stop);
      return this;
    }
  }
  /// <summary>
  ///   Paints an area with a linear gradient.
  /// </summary>
  public class LinearGradientBrushBl : GradientBrushBl<LinearGradientBrush, LinearGradientBrushBl> {
    public LinearGradientBrushBl(Expr<LinearGradientBrush> Provider) : base(Provider) { }
    public LinearGradientBrushBl() : this(new Constant<LinearGradientBrush>(new LinearGradientBrush())) { }
    static LinearGradientBrushBl() {
      Register(v => v);
    }
    public static implicit operator LinearGradientBrushBl(Expr<LinearGradientBrush> v) { return new LinearGradientBrushBl(v); }

    /// <summary>
    /// Gets or sets the starting two-dimensional coordinates of the linear gradient.
    /// </summary>
    public PointBl StartPoint {
      get { return Underlying.Property<Point>(LinearGradientBrush.StartPointProperty); }
      set { StartPoint.Bind = value; }
    }
    /// <summary>
    /// Gets or sets the ending two-dimensional coordinates of the linear gradient.
    /// </summary>
    public PointBl EndPoint {
      get { return Underlying.Property<Point>(LinearGradientBrush.EndPointProperty); }
      set { EndPoint.Bind = value; }
    }

  }
  /// <summary>
  /// Paints an area with a radial gradient. A focal point defines the beginning
  ///     of the gradient, and a circle defines the end point of the gradient.
  /// </summary>
  public class RadialGradientBrushBl : GradientBrushBl<RadialGradientBrush, RadialGradientBrushBl> {
    public RadialGradientBrushBl(Expr<RadialGradientBrush> Provider) : base(Provider) { }
    public RadialGradientBrushBl() : this(new Constant<RadialGradientBrush>(new RadialGradientBrush())) { }
    static RadialGradientBrushBl() {
      Register(v => v);
    }
    public static implicit operator RadialGradientBrushBl(Expr<RadialGradientBrush> v) { return new RadialGradientBrushBl(v); }

    /// <summary>
    /// Gets or sets the center of the outermost circle of the radial gradient.
    /// </summary>
    public PointBl Center {
      get { return Underlying.Property<Point>(RadialGradientBrush.CenterProperty); }
      set { Center.Bind = value; }
    }
    /// <summary>
    /// Gets or sets the location of the two-dimensional focal point that defines
    ///     the beginning of the gradient.
    /// </summary>
    public PointBl GradientOrigin {
      get { return Underlying.Property<Point>(RadialGradientBrush.GradientOriginProperty); }
        set { GradientOrigin.Bind = value; }
    }
    /// <summary>
    /// Gets or sets the radius of the outermost circle of the radial gradient.
    /// </summary>
    public PointBl Radius {
      get {
        var x = Underlying.Property<double>(RadialGradientBrush.RadiusXProperty);
        var y = Underlying.Property<double>(RadialGradientBrush.RadiusYProperty);
        return new PointBl(x, y);
      }
      set { Radius.Bind = value; }
    }
  }
  public sealed partial class FrameworkElementBl : FrameworkElementBl<FrameworkElement, FrameworkElementBl> {
    internal static readonly GetProperty<FrameworkElement, double> CenterRotationProperty = 
      "CenterRotation".NewProperty<FrameworkElement, double>();
    internal static readonly GetProperty<FrameworkElement, Point> CenterScaleProperty = 
      "CenterScale".NewProperty<FrameworkElement, Point>();
    internal static readonly GetProperty<FrameworkElement, object> DragAdapterProperty = 
      "DragAdapter".NewProperty<FrameworkElement, object>(null);



    /// <summary>
    /// Create a dependency property to represent the canvas owner of a framework element.
    /// </summary>
    internal static readonly GetProperty<FrameworkElement, Canvas> CanvasParentProperty =
      "CanvasParent".NewProperty<FrameworkElement, Canvas>(null, (element, oldV, newV) => {
        if (oldV != null) {
          oldV.Children.Remove(element);
        }
        if (newV != null) newV.Children.Add(element);
      });

    public FrameworkElementBl(Expr<FrameworkElement> Provider) : base(Provider) { }
    public FrameworkElementBl() : base() { }
    static FrameworkElementBl() {
      Register(v => v);
    }
    public static implicit operator FrameworkElementBl(Expr<UIElement> v) {
      return Ops.DownCastOperator<UIElement, FrameworkElement>.Instance.Make(v);
    }
    public static implicit operator FrameworkElementBl(Expr<FrameworkElement> v) { return new FrameworkElementBl(v); }
    public static implicit operator FrameworkElementBl(FrameworkElement e) {
      return new Constant<FrameworkElement>(e);
    }
  }
  /// <summary>
  //     Provides a WPF framework-level set of properties, events, and methods for
  //     Windows Presentation Foundation (WPF) elements. This class represents the
  //     provided WPF framework-level implementation built on the WPF core-level APIs
  //     defined by System.Windows.UIElement.
  /// </summary>
  public abstract partial class FrameworkElementBl<T,BRAND> : UIElementBl<T,BRAND>, IExtends<BRAND,FrameworkElementBl> 
    where T : FrameworkElement
    where BRAND : FrameworkElementBl<T,BRAND> {


    public FrameworkElementBl(Expr<T> Provider) : base(Provider) { }
    public FrameworkElementBl() : base() { }
    public FrameworkElementBl(CanvasBl canvas, T target)
      : this(new Constant<T>(target)) {
      //(((object)canvas) ).CheckInit();
      this.CanvasParent = canvas;
    }
    public PointBl ScaleTo {
      set {
        var sz = value / Size;
        this.RenderTransform.ScaleXY = sz.X.Min(sz.Y);
      }
    }
    public static implicit operator T(FrameworkElementBl<T, BRAND> self) { return self.CurrentValue; }    
    
    public static implicit operator FrameworkElementBl(FrameworkElementBl<T, BRAND> self) {      
      var ret = FrameworkElementBl.UpCast((Brand<BRAND>) self);
      return ret;
    }    
    public static implicit operator Expr<FrameworkElement>(FrameworkElementBl<T, BRAND> self) {
      FrameworkElementBl b = self;
      return b.Underlying;
    }
    /// <summary>
    ///     Gets or sets a value indicating whether to clip the content of this element
    ///     (or content coming from the child elements of this element) to fit into the
    ///     size of the containing element. This is a dependency property.
    /// </summary>
    public BoolBl ClipToBounds {
      get { return Underlying.Property<bool>(UIElement.ClipToBoundsProperty); }
      set { ClipToBounds.Bind = value; }
    }

    /// <summary>
    ///     Gets or sets the geometry used to define the outline of the contents of an
    ///     element. This is a dependency property.
    /// </summary>
    public GeometryBl Clip {
      get { return Underlying.Property<Geometry>(UIElement.ClipProperty); }
      set { Clip.Bind = value; }
    }
    /// <summary>
    ///     Gets or sets a value that determines whether rendering for this element should
    ///     use device-specific pixel settings during rendering. This is a dependency
    ///     property.
    /// </summary>
    public BoolBl SnapsToDevicePixels {
      get { return Underlying.Property<bool>(UIElement.SnapsToDevicePixelsProperty); }
      set { SnapsToDevicePixels.Bind = value; }
    }
    /// <summary>
    ///     Gets or sets an opacity mask, as a System.Windows.Media.Brush implementation
    ///     that is applied to any alpha-channel masking for the rendered content of
    ///     this element.
    /// </summary>
    public BrushBl OpacityMask {
      get { return Underlying.Property<Brush>(FrameworkElement.OpacityMaskProperty); }
      set { OpacityMask.Bind = value; }
    }


    public class AlignmentSet {
      internal Expr<T> Underlying;
      internal DependencyProperty HorizontalAlignmentProperty;
      internal DependencyProperty VerticalAlignmentProperty;

      /// <summary>
      ///     Gets or sets horizontal alignment. 
      /// </summary>
      public EnumBl<HorizontalAlignment> Horizontal {
        get { return Underlying.Property<HorizontalAlignment>(HorizontalAlignmentProperty); }
        set { Horizontal.Bind = value; }
      }
      /// <summary>
      ///     Gets or sets vertical alignment. 
      /// </summary>
      public EnumBl<VerticalAlignment> Vertical {
        get { return Underlying.Property<VerticalAlignment>(VerticalAlignmentProperty); }
        set { Vertical.Bind = value; }
      }
      internal AlignmentSet Bind {
        set {
          this.Horizontal.Bind = value.Horizontal;
          this.Vertical.Bind = value.Vertical;
        }
      }
    }
    /// <summary>
    /// Gets or sets the alignment characteristics applied to this element
    /// when it is composed within a parent element, such as a panel or items control.
    /// Default is Horizontal and Vertical Stretch.
    /// </summary>
    public AlignmentSet Alignment {
      get { return new AlignmentSet() { Underlying = Underlying, HorizontalAlignmentProperty = FrameworkElement.HorizontalAlignmentProperty, VerticalAlignmentProperty = FrameworkElement.VerticalAlignmentProperty }; }
      set { Alignment.Bind = (value); }
    }
    /// <summary>
    /// Gets the position of mouse
    /// </summary>
    public PointBl MousePosition {
      get {
        return this.StatefulMap<Point,PointBl>(f => {
          return Mouse.GetPosition(f);
        });
      }
    }
    /// <summary>
    ///     Gets or sets a value indicating whether this element is enabled in the user
    ///     interface (UI). This is a dependency property.
    /// </summary>
    /// <returns>
    ///     true if the element is enabled; otherwise, false. The default value is true.
    ///</returns>
    public BoolBl IsEnabled {
      get { return Underlying.Property<bool>(FrameworkElement.IsEnabledProperty); }
      set { IsEnabled.Bind = value; }
    }
    /// <summary>
    ///    Gets or sets a value that declares whether this element can possibly be returned
    ///     as a hit test result from some portion of its rendered content.
    /// </summary>
    public BoolBl IsHitTestVisible {
      get { return Underlying.Property<bool>(FrameworkElement.IsHitTestVisibleProperty); }
      set { IsHitTestVisible.Bind = value; }
    }

    /// <summary>
    ///     Gets a value that determines whether this element has logical focus. This
    ///     is a dependency property.
    /// </summary>
    /// <returns>
    ///     true if this element has logical focus; otherwise, false.
    ///</returns>
    public BoolBl IsFocused {
      get { return Underlying.Property<bool>(FrameworkElement.IsFocusedProperty); }
    }
    /// <summary>
    ///     Gets a value indicating whether the mouse is captured to this element. This
    /// </summary>
    /// <returns>
    ///     true if the element has mouse capture; otherwise, false. The default is false.
    ///</returns>
    public BoolBl IsMouseCaptured {
      get { return Underlying.Property<bool>(FrameworkElement.IsMouseCapturedProperty); }
    }
    /// <summary>
    ///    Gets a value indicating whether the mouse pointer is located over this element
    ///     (including child elements in the visual tree). This is a dependency property.
    /// </summary>
    /// <returns>
    ///     true if mouse pointer is over the element or its child elements; otherwise,
    ///     false. The default is false. 
    ///</returns>
    public BoolBl IsMouseOver {
      get { return Underlying.Property<bool>(FrameworkElement.IsMouseOverProperty); }
    }
    /// <summary>
    /// Set a mouse enter handler. First argument is current widget, second argument is 
    /// mouse event arguments. Return true if you want the handler to remain installed, 
    /// else return false to uninstall this handler.
    /// </summary>
    public Func<BRAND, System.Windows.Input.MouseEventArgs, bool> OnMouseEnter {
      set {
        System.Windows.Input.MouseEventHandler F = null;
        F = (x, y) => {
          if (!value(FrameworkElementBl<T, BRAND>.ToBrand(new Constant<T>((T)x)), y))
            CurrentValue.MouseEnter -= F;
        };
        CurrentValue.MouseEnter += F;
      }
    }
    /// <summary>
    /// Set a mouse leave handler. First argument is current widget, second argument is 
    /// mouse event arguments. Return true if you want the handler to remain installed, 
    /// else return false to uninstall this handler.
    /// </summary>
    public Func<BRAND, System.Windows.Input.MouseEventArgs, bool> OnMouseLeave {
      set {
        System.Windows.Input.MouseEventHandler F = null;
        F = (x, y) => {
          if (!value(FrameworkElementBl<T, BRAND>.ToBrand(new Constant<T>((T)x)), y))
            CurrentValue.MouseLeave -= F;
        };
        CurrentValue.MouseLeave += F;
      }
    }
    /// <summary>
    /// Set a mouse down handler. First argument is current widget, second argument is 
    /// mouse event arguments. Return true if you want the handler to remain installed, 
    /// else return false to uninstall this handler.
    /// </summary>
    public Func<BRAND, System.Windows.Input.MouseButtonEventArgs, bool> OnMouseDown {
      set {
        System.Windows.Input.MouseButtonEventHandler F = null;
        F = (x, y) => {
          if (!value(FrameworkElementBl<T, BRAND>.ToBrand(new Constant<T>((T)x)), y))
            CurrentValue.MouseDown -= F;
        };
        CurrentValue.MouseDown += F;
      }
    }

    public void InstallDragHandler(Func<bool> Start, Action Move, Action Finish) {
      var IsGoing = false;
      OnMouseDown = (self, e) => {
        if (IsGoing) { // glitch?
          IsGoing = false;
          Finish();
          Mouse.Capture(self.CurrentValue, CaptureMode.None);
        }
        if (!Start()) return false;
        e.Handled = true;
        IsGoing = true;
        Mouse.Capture(self.CurrentValue, CaptureMode.Element);
        self.OnMouseUp = (self0, e0) => {
          if (IsGoing) {
            IsGoing = false;
            Finish();
            Mouse.Capture(self0.CurrentValue, CaptureMode.None);
            e0.Handled = true;
          }
          return false;
        };
        self.OnMouseMove = (self0, e0) => {
          if (!IsGoing) return false;
          e0.Handled = true;
          if (Mouse.LeftButton != MouseButtonState.Pressed) {
            IsGoing = false;
            Finish();
            Mouse.Capture(self0.CurrentValue, CaptureMode.None);
            return false;
          }
          Move();
          return true;
        };
        return true;
      };
    }
    /// <summary>
    /// Set a mouse up handler. First argument is current widget, second argument is 
    /// mouse event arguments. Return true if you want the handler to remain installed, 
    /// else return false to uninstall this handler.
    /// </summary>
    public Func<BRAND, System.Windows.Input.MouseButtonEventArgs, bool> OnMouseUp {
      set {
        System.Windows.Input.MouseButtonEventHandler F = null;
        F = (x, y) => {
          if (!value(FrameworkElementBl<T, BRAND>.ToBrand(new Constant<T>((T)x)), y))
            CurrentValue.MouseUp -= F;
        };
        CurrentValue.MouseUp += F;
      }
    }
    /// <summary>
    /// Set a mouse move handler. First argument is current widget, second argument is 
    /// mouse event arguments. Return true if you want the handler to remain installed, 
    /// else return false to uninstall this handler.
    /// </summary>
    public Func<BRAND, System.Windows.Input.MouseEventArgs, bool> OnMouseMove {
      set {
        System.Windows.Input.MouseEventHandler F = null;
        F = (x, y) => {
          if (!value(FrameworkElementBl<T, BRAND>.ToBrand(new Constant<T>((T)x)), y))
            CurrentValue.MouseMove -= F;
        };
        CurrentValue.MouseMove += F;
      }
    }



    /// <summary>
    ///    Gets a value indicating whether this element is visible in the user interface
    ///     (UI). This is a dependency property.
    /// </summary>
    /// <returns>
    ///     true if the element is visible; otherwise, false.
    ///</returns>
    public BoolBl IsVisible {
      get { return Underlying.Property<bool>(FrameworkElement.IsVisibleProperty); }
    }
    /// <summary>
    ///     Gets or sets the user interface (UI) visibility of this element. This is
    ///     a dependency property.
    /// </summary>
    /// <returns>
    ///     A value of the enumeration. The default value is System.Windows.Visibility.Visible.
    ///</returns>
    public VisibilityBl Visibility {
      get { return Underlying.Property<Visibility>(FrameworkElement.VisibilityProperty); }
      set { Visibility.Bind = value; }
    }
    /// <summary>
    ///     Gets or sets the opacity factor applied to the entire System.Windows.UIElement
    ///     when it is rendered in the user interface (UI). This is a dependency property.
    /// </summary>
    /// <returns>
    ///     The opacity factor. Default opacity is 1.0. Expected values are between 0.0
    ///     and 1.0.
    ///</returns>
    public DoubleBl Opacity {
      get { return Underlying.Property<double>(FrameworkElement.OpacityProperty); }
      set { Opacity.Bind = value; }
    }
    /// <summary>
    ///     Gets or sets the identifying name of the element. The name provides a reference
    ///     so that code-behind, such as event handler code, can refer to a markup element
    ///     after it is constructed during processing by a XAML processor. This is a
    ///     dependency property.
    /// </summary>
    /// <returns>
    ///    The name of the element. The default is an empty string.
    ///</returns>
    public StringBl Name {
      get { return Underlying.Property<string>(FrameworkElement.NameProperty); }
      set { Name.Bind = value; }
    }
    /// <summary>
    ///     Gets or sets the tool-tip object that is displayed for this element in the
    ///     user interface (UI). This is a dependency property.
    /// </summary>
    /// <returns>
    ///    The tooltip object. See returns below for details on why this parameter is
    ///     not strongly typed.
    ///</returns>
    public ObjectBl ToolTip {
      get { return Underlying.Property<object>(FrameworkElement.ToolTipProperty); }
      set { ToolTip.Bind = value; }
    }
    /// <summary>
    /// A special parent property that will manage adding/removing this UI element from a canvas. 
    /// When set to a canvas, the element will be added to that canvas, when unset, the element will be removed
    /// from the canvas. 
    /// </summary>
    public CanvasBl CanvasParent {
      get {
        return Underlying.Property<Canvas>(FrameworkElementBl.CanvasParentProperty.Property);
      }
      set { CanvasParent.Bind = value; }
    }
    /// <summary>
    /// This element's logical parent.
    /// </summary>
    public FrameworkElementBl Parent {
      get {
        return this.Map<FrameworkElement, FrameworkElementBl>(value => {
          return (FrameworkElement)value.Parent;
        });
      }
    }
    [Obsolete("Use CanvasParent = CanvasBl.None instead.")]
    public void Remove() {
      this.CanvasParent = CanvasBl.None;
    }
    /// <summary>
    /// Gets or sets the UI element's left position.
    /// </summary>
    public virtual DoubleBl Left {
      get {
        return Underlying.Property<double>(Canvas.LeftProperty).Bl();
      }
      set { Left.Bind = value; }
    }
    /// <summary>
    /// Gets or sets the UI element's top position.
    /// </summary>
    public virtual DoubleBl Top
    {
      get {
        return Underlying.Property<double>(Canvas.TopProperty).Bl();
      }
      set { Top.Bind = value; }
    }
    /// <summary>
    /// Gets or sets the UI element's width. 
    /// </summary>
    public DoubleBl Width {
      get {
        return Underlying.Property<double>(FrameworkElement.ActualWidthProperty, FrameworkElement.WidthProperty).Bl(); // .Restrict(0, double.MaxValue);
      }
      set { Width.Bind = value; }
    }

    public ObjectBl Tag {
      get {
        return Underlying.Property<object>(FrameworkElement.TagProperty).Bl(); // .Restrict(0, double.MaxValue);
      }
      set { Tag.Bind = value; }
    }

    /// <summary>
    /// Gets or sets the UI element's height.
    /// </summary>
    public DoubleBl Height {
      get {
        return Underlying.Property<double>(FrameworkElement.ActualHeightProperty, FrameworkElement.HeightProperty).Bl(); //.Restrict(0, double.MaxValue);
      }
      set { Height.Bind = value; }
    }
    /// <summary>
    /// Gets or sets the UI element's right position. The right position is defined as Left + Width.
    /// </summary>
    public DoubleBl Right {
      get { return Left + Width; }
      set { Right.Bind = value; }
    }
    /// <summary>
    /// Gets or sets the UI element's bottom position. The right position is defined as Top + Height.
    /// </summary>
    public DoubleBl Bottom {
      get { return Top + Height; }
      set { Bottom.Bind = value; }
    }
    /// <summary>
    /// Gets or sets the UI element's size, defined as a point of its width and height.
    /// </summary>
    public PointBl Size {
      get { return new PointBl(Width, Height); }
      set { Size.Bind = value; }
    }

    public Point4DBl Bounds {
      get { return new Point4DBl(LeftTop, Size); }
      set { Bounds.Bind = value; }
    }

    /// <summary>
    /// Get or set the UI element's center relative point, which is Size / 2.
    /// </summary>
    public PointBl CenterSize
    {
      get { return new PointBl(Width / 2, Height / 2); }
      set { CenterSize.Bind = value; }
    }
    /// <summary>
    /// Gets or sets the UI element's position with respect to its left-top corner.
    /// </summary>
    public PointBl LeftTop
    {
      get { return new PointBl(Left, Top); }
      set { LeftTop.Bind = value; }
    }
    /// <summary>
    /// Gets or sets the UI element's position with respect to its right-bottom corner.
    /// </summary>
    public PointBl RightBottom
    {
      get { return new PointBl(Right, Bottom); }
      set { RightBottom.Bind = value; }
    }
    /// <summary>
    /// Gets or sets the UI element's position with respect to its right-top corner.
    /// </summary>
    public PointBl RightTop {
      get { return new PointBl(Right, Top); }
      set { RightTop.Bind = value; }
    }
    /// <summary>
    /// Gets or sets the UI element's position with respect to its left-bottom corner.
    /// </summary>
    public PointBl LeftBottom {
      get { return new PointBl(Left, Bottom); }
      set { LeftBottom.Bind = value; }
    }
    /// <summary>
    /// Gets or sets the UI element's position with respect to its right-top corner.
    /// </summary>
    public PointBl CenterPosition {
      get { return new PointBl(Left + Width / 2, Top + Height / 2); }
      set { CenterPosition.Bind = value; }
    }

    private TransformBl RenderTransform0;

    ///     Gets or sets transform information that affects the rendering position of
    ///     this element. This is a dependency property. Will cache specified center size.
    /// </summary>
    public TransformBl RenderTransform {
      get {
        if (((object) RenderTransform0) == null)
          RenderTransform0 = new TransformBl(Underlying.Property<Transform>(FrameworkElement.RenderTransformProperty)) {
            Origin = CenterSize,
          };
        return RenderTransform0;
      }
      set { RenderTransform.Bind = value; }
    }
    /// <summary>
    /// Directly set rotation of this element rather than go through a RenderTransform. Only use when direct access to property is needed.
    /// </summary>
    public DegreeBl AtRotation {
      get {
        return Underlying.Property<double>(FrameworkElementBl.CenterRotationProperty.Property).Bl().ToDegrees();
      }
      set { AtRotation.Bind = value; }
    }
    public PointBl EnableAtRotation {
      set {
        RenderTransform[value].Rotate.Bind = AtRotation;
      }
    }
    /// <summary>
    /// Directly set scale of this element rather than go through a RenderTransform. Only use when direct access to property is needed.
    /// </summary>
    public PointBl AtScale {
      get {
        return Underlying.Property<Point>(FrameworkElementBl.CenterScaleProperty.Property).Bl();
      }
      set { AtScale.Bind = value; }
    }
    public PointBl EnableAtScale {
      set {
        RenderTransform[value].Scale.Bind = AtScale;
      }
    }



    /// <summary>
    /// Translates a pixel-based peer position of a UI element into a percentage-based relative position of the UI element. 
    /// </summary>
    /// <param name="p">A pixel-based position relative to this UI element's container.</param>
    /// <returns>A percentage-based position relative to this UI element's position and size.</returns>
    public PointBl Relative(PointBl p) {
      return (p - LeftTop) / Size;
    }


    /// <summary>
    /// Gets or sets the UI element's position with respect to its center at the top
    /// </summary>
    public PointBl CenterTop {
      get { return new PointBl(Left + Width / 2, Top); }
      set { CenterTop.Bind = value; }
    }
    /// <summary>
    /// Gets or sets the UI element's position with respect to its center at the bottom
    /// </summary>
    public PointBl CenterBottom {
      get { return new PointBl(Left + Width / 2, Bottom); }
      set { CenterBottom.Bind = value; }
    }
    /// <summary>
    /// Gets or sets the UI element's position with respect to its center at the left
    /// </summary>
    public PointBl LeftCenter {
      get { return new PointBl(Left, Top + Height / 2); }
      set { LeftCenter.Bind = value; }
    }
    /// <summary>
    /// Gets or sets the UI element's position with respect to its center at the right
    /// </summary>
    public PointBl RightCenter {
      get { return new PointBl(Right, Top + Height / 2); }
      set { RightCenter.Bind = value; }
    }
    /// <summary>
    /// Gets or sets the UI element's Zindex to describe the order in Zbuffer
    /// </summary>
    public IntBl ZIndex {
      get { return Underlying.Property<int>(Panel.ZIndexProperty).Bl(); }
      set { ZIndex.Bind = value; }
    }
    /// <summary>
    /// make two thumb,the red one is at the lefttop to drag, the blue one is at the rightbottom to scale
    /// </summary>
    public virtual bool HasThumbs {
      set {
        if (value) {
          new ThumbBl(CanvasParent) {
            Background = Brushes.Red,
            MapF = (self,p) => p.Clamp(new Point(0, 0), CanvasParent.Size),
            DragPoint = LeftTop, ZIndex = ZIndex + 1
          };
          new ThumbBl(CanvasParent) {
            Background = Brushes.Blue,
            MapF = (self,p) => p.Clamp(new Point(0, 0), CanvasParent.Size),
            DragPoint = Size + LeftTop, 
            ZIndex = ZIndex + 1
          };
        }
      }
    }
    /// <summary>
    /// make a thumb to rotate
    /// </summary>
    public virtual bool HasRotateThumb {
      set {
        if (value) {
          var r = RenderTransform.Rotate;
          new ThumbBl(CanvasParent) {
            Background = Brushes.Purple,
            ZIndex = ZIndex + 1,
            DragPoint = r.ToPoint * Width.Max(Height) + CenterPosition
          };
        }
      }
    }
    /// <summary>
    /// The pixel shader effect of this UI element.  Set this if you want to use a built-in pixel shader effect, otherwise use Effect.Custom to specify your own effect.
    /// </summary>
    public EffectBl Effect {
      get { return Underlying.Property<Effect>(UIElement.EffectProperty); }
      set { this.Effect.Bind = value; }
    }
    /// <summary>
    /// Return a drag adapter that can be installed by calling Install. 
    /// </summary>
    public DragAdapter Drag {
      get {
        object value = Underlying.Property<object>(FrameworkElementBl.DragAdapterProperty.Property).CurrentValue;
        if (value == null) {
          value = new DragAdapter(this);
          Underlying.Property<object>(FrameworkElementBl.DragAdapterProperty.Property).Bl().Now = (ObjectBl) value;
        }
        return (DragAdapter) value;
      }
    }

    /// <summary>
    /// Wrap thumb-like drag functionality.
    /// </summary>
    public class DragAdapter : DependencyObject {
      public class Driver {
        public Func<Func<BRAND, bool>> DetectStartDrag { set; private get; }
        public Action<Func<BRAND, bool>> DetectStopDrag { set; private get; }
        public Action<Func<BRAND, bool>> DetectMoveDrag { set; private get; }

      }
      private readonly BRAND Underlying;
      private static readonly GetProperty<DragAdapter, bool> IsDraggingProperty = "IsDragging".NewProperty<DragAdapter, bool>();

      /// <summary>
      /// Compute point property that drag delta is added to. By default, this is the LeftTop property of the framework element. Will be ignored if OnDrag is set.
      /// </summary>
      public Func<BRAND, PointBl> DragTarget { private get; set; }
      /// <summary>
      /// Manually deal with the drag delta, the argument of the action is the start drag point with respect to this UI element
      /// and the position of the mouse with respect to this UI element.
      /// By default, this is null and not used, if set this will be called rather than use DragTarget. 
      /// </summary>
      public Action<BRAND, PointBl, PointBl> OnDrag { private get; set; }

      /// <summary>
      /// Manually deal with the drag delta, the argument of the action is the start drag point with respect to this UI element 
      /// and the position of the mouse with respect to the containing canvas.
      /// By default, this is null and not used, if set this will be called rather than use OnDrag or DragTarget. 
      /// </summary>
      public Action<BRAND, PointBl, PointBl> OnDragInCanvas { private get; set; }

      /// <summary>
      /// Called when dragging starts with the point of contact relative to this UI element. 
      /// </summary>
      public Action<BRAND, PointBl> DragStart { private get; set; }
      /// <summary>
      /// Called when dragging stops with the point of contact relative to this UI element. 
      /// </summary>
      public Action<BRAND, PointBl> DragStop { private get; set; }
      /// <summary>
      /// Whether or not the element is being dragged. This is a dependency property. 
      /// </summary>
      public BoolBl IsDragging { get { return IsDraggingProperty[this]; }  }

      /// <summary>
      /// Whether to start dragging on a mouse down event. Default is to accept dragging. 
      /// </summary>
      public Func<BRAND, MouseButtonEventArgs, bool> DragAccept = (element, args) => true;


      /// <summary>
      /// Start dragging the element, this can be issued programmatically or automatically via MouseInstall and a MouseDown event.
      /// </summary>
      /// <param name="GetPosition">Function for getting input point of drag relative to a UI element. If null, will be the mouse position.</param>
      /// <param name="InstallMove">Function for installing move handlers, if null will install on mouse move.</param>
      /// <param name="DoCapture">Function for capturing input focus. If null will capture mouse.</param>
      public void StartDrag(Func<FrameworkElementBl,PointBl> GetPosition, Action<BRAND, Func<BRAND,bool>> InstallMove, Action<BRAND> DoCapture) {
        var element = Underlying;
        if (active) throw new NotSupportedException();
        if (GetPosition == null) GetPosition = element0 => element0.MousePosition;
        if (InstallMove == null) InstallMove = (element0, F) => element0.OnMouseMove = (element1, args) => F(element1);
        if (DoCapture == null) DoCapture = (element0) => Mouse.Capture(element0.CurrentValue, CaptureMode.Element);
        active = true;

        IsDraggingProperty[this].Bl().Now = true;
        DoCapture(element);
        PointBl Begin = GetPosition(element).Now;
        PointBl ActualBegin = Begin.Now;
        DragStart(element, GetPosition(Underlying));
        (OnDrag == null || OnDragInCanvas == null).Assert();
        if (OnDragInCanvas != null) {
          InstallMove(element, element0 => {
            if (!active) return false;
            OnDragInCanvas(element0, Begin, GetPosition(element0.CanvasParent));
            return true;
          });
        } else if (OnDrag != null) {
          InstallMove(element, element0 => {
            if (!active) return false;
            OnDrag(element0, Begin, GetPosition(element0));
            return true;
          });
        } else if (DragTarget != null) {
          InstallMove(element, element0 => {
            if (!active) return false;
            DragTarget(element0).Now += (GetPosition(element0) - Begin);
            return true;
          });
        }
      }
      /// <summary>
      /// Stop dragging, can be called programmatically or automatically on MouseUp via MouseInstall.
      /// </summary>
      /// <param name="DoUncapture">Uncapture input focus. If null will uncapture mouse focus.</param>
      public void StopDrag(Action<BRAND> DoUncapture) {
        var element = Underlying;
        if (DoUncapture == null) 
          DoUncapture = element1 => Mouse.Capture(element1.CurrentValue, CaptureMode.None);
        if (!active) throw new NotSupportedException();
        active = false;
        DragStop(element, element.MousePosition);
        DoUncapture(element);
        IsDraggingProperty[this].Bl().Now = false;
      }

      private bool active = false;
      private bool mouseInstalled = false;
      /// <summary>
      /// If true, install this drag adapter using the configuration specified according to this Bling value. Note that after the drag adapter is installed, it should no longer be manipulated
      /// except to uninstall. 
      /// </summary>
      public bool MouseInstall {
        set {
          if (!value) {
            mouseInstalled = false; return;
          }
          active = false;
          mouseInstalled = true;
          Underlying.OnMouseDown = (element, downArgs) => {
            if (!DragAccept(element,downArgs)) return false;
            if (!mouseInstalled) return false;
            StartDrag(null, null, null);
            element.OnMouseUp = (element0, upArgs) => {
              if (active)
                StopDrag(null);
              return false;
            };
            return true;
          };
        }
      }
      internal DragAdapter(BRAND Underlying) {
        this.Underlying = Underlying;
        DragTarget = (element) => element.LeftTop;
        DragStart = (element, delta) => { };
        DragStop = (element, delta) => { };

      }
    }
  }


  /// <summary>
  /// Bl wrapper around FontFamily
  /// </summary>
  public partial class FontFamilyBl : Brand<FontFamily, FontFamilyBl> {
    public FontFamilyBl(Expr<FontFamily> v) : base(v) { }
    public static implicit operator FontFamilyBl(Expr<FontFamily> v) { return new FontFamilyBl(v); }
    public static implicit operator FontFamilyBl(FontFamily v) { return new Constant<FontFamily>(v); }

    static FontFamilyBl() {
      Register(v => v);
    }
  }
  /// <summary>
  /// Bl wrapper around FontStyle
  /// </summary>
  public partial class FontStyleBl : Brand<FontStyle, FontStyleBl> {
    public FontStyleBl(Expr<FontStyle> v) : base(v) { }
    public static implicit operator FontStyleBl(Expr<FontStyle> v) { return new FontStyleBl(v); }
    public static implicit operator FontStyleBl(FontStyle v) { return new Constant<FontStyle>(v); }

    static FontStyleBl() {
      Register(v => v);
    }
  }

  /*
  public partial class TextAlignmentBl : BrandedBlValue<TextAlignment, TextAlignmentBl> {
    public TextAlignmentBl(Value<TextAlignment> v) : base(v) { }
    public static implicit operator TextAlignmentBl(Value<TextAlignment> v) { return new TextAlignmentBl(v); }
    public static implicit operator TextAlignmentBl(TextAlignment v) { return new Constant<TextAlignment>(v); }

    static TextAlignmentBl() {
      Register(v => v);
    }
  }*/

  /// <summary>
  /// Bl wrapper around FontWeight
  /// </summary>
  public partial class FontWeightBl : Brand<FontWeight, FontWeightBl> {
    public FontWeightBl(Expr<FontWeight> v) : base(v) { }
    public static implicit operator FontWeightBl(Expr<FontWeight> v) { return new FontWeightBl(v); }
    public static implicit operator FontWeightBl(FontWeight v) { return new Constant<FontWeight>(v); }

    /// <summary>
    /// Sets if the font is in bold type
    /// </summary>
    public bool IsBold {
      set { if (value) this.Bind = FontWeights.Bold; }
    }
    /// <summary>
    /// Sets if the font is in black type
    /// </summary>
    public bool IsBlack {
      set { if (value) this.Bind = FontWeights.Black; }
    }
    static FontWeightBl() {
      Register(v => v);
    }
  }
  public interface IFontBl {
    /// <summary>
    ///     Gets or sets the font size. This is a dependency property.
    /// </summary>
    /// <returns>
    ///    The size of the text in the System.Windows.Controls.Control. The default
    ///     is System.Windows.SystemFonts.MessageFontSize. The font size must be a positive
    ///     number.
    ///</returns>    
    DoubleBl Size { get; set; }
    /// <summary>
    ///     Gets or sets the font style. This is a dependency property.
    /// </summary>
    /// <returns>
    ///     A System.Windows.FontStyle value. The default is System.Windows.FontStyles.Normal.
    ///</returns>    
    FontStyleBl Style { get; set; }
    /// <summary>
    ///     Gets or sets the weight or thickness of the specified font. This is a dependency
    ///    property.
    /// </summary>
    /// <returns>
    ///     A System.Windows.FontWeight value. The default is System.Windows.FontWeights.Normal.
    ///</returns>    
    FontWeightBl Weight { get; set; }
    FontFamilyBl Family { get; set; }
    /// <summary>
    /// Foreground color of font.
    /// </summary>
    BrushBl Brush { get; set; }

    IFontBl Bind { set; }
    IFontBl Now { set; }
  }
  public abstract class BaseFontBl : IFontBl {
    /// <summary>
    ///     Gets or sets the font size. This is a dependency property.
    /// </summary>
    /// <returns>
    ///    The size of the text in the System.Windows.Controls.Control. The default
    ///     is System.Windows.SystemFonts.MessageFontSize. The font size must be a positive
    ///     number.
    ///</returns>    
    public abstract DoubleBl Size { get; set; }
    /// <summary>
    ///     Gets or sets the font style. This is a dependency property.
    /// </summary>
    /// <returns>
    ///     A System.Windows.FontStyle value. The default is System.Windows.FontStyles.Normal.
    ///</returns>    
    public abstract FontStyleBl Style { get; set; }
    /// <summary>
    ///     Gets or sets the weight or thickness of the specified font. This is a dependency
    ///    property.
    /// </summary>
    /// <returns>
    ///     A System.Windows.FontWeight value. The default is System.Windows.FontWeights.Normal.
    ///</returns>    
    public abstract FontWeightBl Weight { get; set; }
    public abstract FontFamilyBl Family { get; set; }
    /// <summary>
    /// Foreground color of font.
    /// </summary>
    public abstract BrushBl Brush { get; set; }
    public IFontBl Bind {
      set {
        if (value is FontCl) {
          if (((object)value.Brush) != null)
            this.Brush.Bind = value.Brush;
          if (((object)value.Family) != null)
            this.Family.Bind = value.Family;
          if (((object)value.Size) != null)
            this.Size.Bind = value.Size;
          if (((object)value.Style) != null)
            this.Style.Bind = value.Style;
          if (((object)value.Weight) != null)
            this.Weight.Bind = value.Weight;
        } else {
          this.Brush.Bind = value.Brush;
          this.Family.Bind = value.Family;
          this.Size.Bind = value.Size;
          this.Style.Bind = value.Style;
          this.Weight.Bind = value.Weight;
        }
      }
    }
    public IFontBl Now {
      set {
        if (value is FontCl) {
          if (((object)value.Family) != null)
            this.Family.Now = value.Family;
          if (((object)value.Size) != null)
            this.Size.Now = value.Size;
          if (((object)value.Style) != null)
            this.Style.Now = value.Style;
          if (((object)value.Weight) != null)
            this.Weight.Now = value.Weight;

        } else {
          this.Family.Now = value.Family;
          this.Size.Now = value.Size;
          this.Style.Now = value.Style;
          this.Weight.Now = value.Weight;
        }
      }
    }
  }

  public class FontCl : BaseFontBl {
    /// <summary>
    ///     Gets or sets the font size. This is NOT a dependency property.
    /// </summary>
    /// <returns>
    ///    The size of the text in the System.Windows.Controls.Control. The default
    ///     is System.Windows.SystemFonts.MessageFontSize. The font size must be a positive
    ///     number.
    ///</returns>    
    public override DoubleBl Size { get; set; }
    /// <summary>
    ///     Gets or sets the font style. This is NOT a dependency property.
    /// </summary>
    /// <returns>
    ///     A System.Windows.FontStyle value. The default is System.Windows.FontStyles.Normal.
    ///</returns>    
    public override FontStyleBl Style { get; set; }
    /// <summary>
    ///     Gets or sets the weight or thickness of the specified font. This is NOT a dependency
    ///    property.
    /// </summary>
    /// <returns>
    ///     A System.Windows.FontWeight value. The default is System.Windows.FontWeights.Normal.
    ///</returns>    
    public override FontWeightBl Weight { get; set; }
    public override FontFamilyBl Family { get; set; }
    /// <summary>
    /// Foreground color of font.
    /// </summary>
    public override BrushBl Brush { get; set; }
  }

  internal class FontBl : BaseFontBl {
    internal Expr<FrameworkElement> Underlying;
    internal DependencyProperty SizeProperty;
    internal DependencyProperty StyleProperty;
    internal DependencyProperty WeightProperty;
    internal DependencyProperty FamilyProperty;
    internal DependencyProperty BrushProperty;

    internal FontBl() { }

    public FrameworkElementBl Target { get { return Underlying; } }

    /// <summary>
    ///     Gets or sets the font size. This is a dependency property.
    /// </summary>
    /// <returns>
    ///    The size of the text in the System.Windows.Controls.Control. The default
    ///     is System.Windows.SystemFonts.MessageFontSize. The font size must be a positive
    ///     number.
    ///</returns>    
    public override DoubleBl Size { get { return Underlying.Property<double>(SizeProperty); } set { Size.Bind = value; } }
    /// <summary>
    ///     Gets or sets the font style. This is a dependency property.
    /// </summary>
    /// <returns>
    ///     A System.Windows.FontStyle value. The default is System.Windows.FontStyles.Normal.
    ///</returns>    
    public override FontStyleBl Style { get { return Underlying.Property<FontStyle>(StyleProperty); } set { Style.Bind = value; } }
    /// <summary>
    ///     Gets or sets the weight or thickness of the specified font. This is a dependency
    ///    property.
    /// </summary>
    /// <returns>
    ///     A System.Windows.FontWeight value. The default is System.Windows.FontWeights.Normal.
    ///</returns>    
    public override FontWeightBl Weight { get { return Underlying.Property<FontWeight>(WeightProperty); } set { Weight.Bind = value; } }

    public override FontFamilyBl Family { get { return Underlying.Property<FontFamily>(FamilyProperty); } set { Family.Bind = value; } }


    /// <summary>
    /// Foreground color of font.
    /// </summary>
    public override BrushBl Brush { get { return Underlying.Property<Brush>(BrushProperty); } set { Brush.Bind = value; } }
  }


  /// <summary>
  ///  Represents the base class for user interface (UI) elements that use a System.Windows.Controls.ControlTemplate
  ///     to define their appearance.
  /// </summary>
  /// <typeparam name="TARGET"></typeparam>
  public abstract partial class ControlBl<T,SELF> : FrameworkElementBl<T,SELF> where T : Control where SELF : ControlBl<T,SELF> {
    public ControlBl(Expr<T> Provider) : base(Provider) { }
    public ControlBl(CanvasBl canvas, T target) : base(canvas, target) {}
    /// <summary>
    /// Access font properties.
    /// </summary>
    public IFontBl Font {
      get {
        return new FontBl() {
          Underlying = ((FrameworkElementBl) this),
          SizeProperty = Control.FontSizeProperty,
          WeightProperty = Control.FontWeightProperty,
          StyleProperty = Control.FontStyleProperty,
          BrushProperty = Control.ForegroundProperty,
          FamilyProperty = Control.FontFamilyProperty,
        };
      }
      set { Font.Bind = (value); }
    }
    /// <summary>
    ///     Gets or sets the alignment of the control's content. Default is Horizontal Left Vertical Top.
    /// </summary>
    public AlignmentSet ContentAlignment {
      get { return new AlignmentSet() { Underlying = Underlying, HorizontalAlignmentProperty = Control.HorizontalContentAlignmentProperty, VerticalAlignmentProperty = Control.VerticalContentAlignmentProperty }; }
      set { ContentAlignment.Bind = (value); }
    }


    /// <summary>
    ///     Gets or sets a brush that describes the background of a control. This is
    ///     a  dependency property.
    /// </summary>
    /// <returns>
    ///     The brush that is used to fill the background of the control. The default
    ///     is System.Windows.Media.Brushes.Transparent.
    ///</returns> 
    public BrushBl Background {
      get { return Underlying.Property<Brush>(Control.BackgroundProperty).Bl(); }
      set { Background.Bind = value; }
    }
    /// <summary>
    ///     Gets or sets a brush that describes the foreground color. This is a dependency
    ///     property.
    /// </summary>
    /// <returns>
    ///     The brush that paints the foreground of the control. The default value is
    ///     the system dialog font color.
    ///</returns>    
    public BrushBl Foreground {
      get { return Underlying.Property<Brush>(Control.ForegroundProperty).Bl(); }
      set { Foreground.Bind = value; }
    }

    /// <summary>
    /// Get and set border properties.
    /// </summary>
    public BorderInfoSet Border {
      get {
        return new BorderInfo<T> {
          Underlying = Underlying,
          BorderBrushProperty = Control.BorderBrushProperty,
          BorderThicknessProperty = Control.BorderThicknessProperty,
        };
      }
      set {
        Border.Bind(value);
      }
    }
    /// <summary>
    ///     The amount of space between the content of a System.Windows.Controls.Control
    ///     and its System.Windows.FrameworkElement.Margin or System.Windows.Controls.Border.
    ///     The default is a thickness of 0 on all four sides.
    /// </summary>
    public ThicknessBl Padding {
      get { return Underlying.Property<Thickness>(Control.PaddingProperty); }
      set { Padding.Bind = value; }

    }

  }
  public abstract partial class RangeBaseBl<T,SELF> : ControlBl<T,SELF> where T : RangeBase where SELF : RangeBaseBl<T,SELF> {
    public RangeBaseBl(Expr<T> Provider) : base(Provider) { }
    public RangeBaseBl(CanvasBl canvas, T target) : base(canvas, target) {}
    public static implicit operator DoubleBl(RangeBaseBl<T, SELF> v) { return v.Value; }

    /// <summary>
    ///     Gets or sets the current magnitude of the range control. This is a dependency
    /// </summary>
    /// <returns>
    ///    The current magnitude of the range control. The default is 0.
    ///</returns>    
    public DoubleBl Value {
      get { return Underlying.Property<double>(RangeBase.ValueProperty).Bl(); }
      set { Value.Bind = value; }
    }
    /// <summary>
    ///    Gets or sets the highest possible System.Windows.Controls.Primitives.RangeBase.Value
    ///     of the range element. This is a dependency property.
    /// </summary>
    /// <returns>
    ///    The highest possible System.Windows.Controls.Primitives.RangeBase.Value of
    ///     the range element. The default is 1.
    ///</returns>    
    public DoubleBl Maximum {
      get { return Underlying.Property<double>(RangeBase.MaximumProperty).Bl(); }
      set { Maximum.Bind = value; }
    }
    /// <summary>
    ///    Gets or sets the System.Windows.Controls.Primitives.RangeBase.Minimum possible
    ///     System.Windows.Controls.Primitives.RangeBase.Value of the range element.
    ///     This is a dependency property.
    /// </summary>
    /// <returns>
    ///    System.Windows.Controls.Primitives.RangeBase.Minimum possible System.Windows.Controls.Primitives.RangeBase.Value
    ///     of the range element. The default is 0.
    ///</returns>    
    public DoubleBl Minimum {
      get { return Underlying.Property<double>(RangeBase.MinimumProperty).Bl(); }
      set { Minimum.Bind = value; }
    }
    private class RangeSetX : RangeBl<DoubleBl, DoubleBl> {
      internal SELF Self;
      public override DoubleBl Maximum {
        get { return Self.Maximum; }
        set { Maximum.Bind = value;  }
      }
      public override DoubleBl Minimum {
        get { return Self.Minimum; }
        set { Minimum.Bind = value; }
      }
    }
    public RangeBl<DoubleBl, DoubleBl> Range { get { return new RangeSetX() { Self = this }; } set { Range.Bind(value); } }

    public class ChangeSet {
      internal Expr<T> Underlying;
      /// <summary>
      ///    Gets or sets a System.Windows.Controls.Primitives.RangeBase.Value to be added
      ///     to or subtracted from the System.Windows.Controls.Primitives.RangeBase.Value
      ///     of a System.Windows.Controls.Primitives.RangeBase control. This is a dependency
      ///     property.
      /// </summary>
      /// <returns>
      ///    System.Windows.Controls.Primitives.RangeBase.Value to add to or subtract
      ///     from the System.Windows.Controls.Primitives.RangeBase.Value of the System.Windows.Controls.Primitives.RangeBase
      ///     element. The default is 0.1.
      ///</returns>    
      public DoubleBl Small {
        get { return Underlying.Property<double>(RangeBase.SmallChangeProperty).Bl(); }
        set { Small.Bind = value; }
      }
      /// <summary>
      ///    Gets or sets a value to be added to or subtracted from the System.Windows.Controls.Primitives.RangeBase.Value
      ///     of a System.Windows.Controls.Primitives.RangeBase control. This is a dependency
      ///     property.
      /// </summary>
      /// <returns>
      ///    System.Windows.Controls.Primitives.RangeBase.Value to add to or subtract
      ///     from the System.Windows.Controls.Primitives.RangeBase.Value of the System.Windows.Controls.Primitives.RangeBase
      ///     element. The default is 1.
      ///</returns>    
      public DoubleBl Large {
        get { return Underlying.Property<double>(RangeBase.LargeChangeProperty).Bl(); }
        set { Large.Bind = value; }
      }
      internal ChangeSet Bind {
        set {
          this.Small = value.Small;
          this.Large = value.Large;
        }
      }
    }
    /// <summary>
    /// Large and small change values. 
    /// </summary>
    public ChangeSet Change {
      get { return new ChangeSet() { Underlying = Underlying }; }
      set { Change.Bind = value; }
    }
    public Action<SELF, double, double> ValueChanged {
      set {
        CurrentValue.ValueChanged += (x, y) => {
          value(ToBrand(new Constant<T>((T)x)), y.OldValue, y.NewValue);
        };
      }
    }
  }
  public abstract partial class TextBoxBl<T, BRAND> : ControlBl<T, BRAND>
    where T : TextBoxBase
    where BRAND : TextBoxBl<T, BRAND> {
    public TextBoxBl(Expr<T> Provider) : base(Provider) { }
    public TextBoxBl(CanvasBl canvas, T target) : base(canvas, target) { }
    /// <summary>
    ///     Gets or sets a value that indicates how the text editing control responds
    ///     when the user presses the ENTER key.
    /// </summary>
    /// <remarks>
    ///     true if pressing the ENTER key inserts a new line at the current cursor position;
    ///     otherwise, the ENTER key is ignored. The default value is false for System.Windows.Controls.TextBox
    ///     and true for System.Windows.Controls.RichTextBox.
    ///     </remarks>
    public BoolBl AcceptsReturn { get { return Underlying.Property<bool>(TextBoxBase.AcceptsReturnProperty); } set { AcceptsReturn.Bind = value; } }
    /// <summary>
    ///     Gets or sets a value that indicates how the text editing control responds
    ///     when the user presses the TAB key. This is a dependency property.
    ///</summary>
    /// <remarks>
    ///     true if pressing the TAB key inserts a tab character at the current cursor
    ///     position; false if pressing the TAB key moves the focus to the next control
    ///     that is marked as a tab stop and does not insert a tab character.  The default
    ///     value is false.
    ///     </remarks>
    public BoolBl AcceptsTab { get { return Underlying.Property<bool>(TextBoxBase.AcceptsTabProperty); } set { AcceptsTab.Bind = value; } }
    /// <summary>
    ///     Gets or sets a value that determines whether when a user selects part of
    ///     a word by dragging across it with the mouse, the rest of the word is selected.
    ///     This is a dependency property.
    ///</summary>
    /// <remarks>
    ///     true if automatic word selection is enabled; otherwise, false.  The default
    ///     value is false.
    ///     </remarks>
    public BoolBl AutoWordSelection { get { return Underlying.Property<bool>(TextBoxBase.AutoWordSelectionProperty); } set { AutoWordSelection.Bind = value; } }
    /// <summary>
    ///     Gets or sets a value that indicates whether the text editing control is read-only
    ///     to a user interacting with the control. This is a dependency property.
    ///</summary>
    /// <remarks>
    ///     true if the contents of the text editing control are read-only to a user;
    ///     otherwise, the contents of the text editing control can be modified by the
    ///     user. The default value is false.
    ///     </remarks>
    public BoolBl IsReadOnly { get { return Underlying.Property<bool>(TextBoxBase.IsReadOnlyProperty); } set { IsReadOnly.Bind = value; } }
    /// <summary>
    ///     Gets or sets a value that indicates whether undo support is enabled for the
    ///     text-editing control. This is a dependency property.
    ///</summary>
    /// <remarks>
    ///     true to enable undo support; otherwise, false. The default value is true.
    ///     </remarks>
    public BoolBl IsUndoEnabled { get { return Underlying.Property<bool>(TextBoxBase.IsUndoEnabledProperty); } set { IsUndoEnabled.Bind = value; } }
    /// <summary>
    ///     Gets or sets a value that indicates whether a vertical scroll bar is shown.
    ///     This is a dependency property.
    ///</summary>
    /// <remarks>
    ///     A value that is defined by the System.Windows.Controls.ScrollBarVisibility
    ///     enumeration. The default value is System.Windows.Visibility.Hidden.
    ///     </remarks>
    public EnumBl<ScrollBarVisibility> VerticalScrollBarVisibility { get { return Underlying.Property<ScrollBarVisibility>(TextBoxBase.VerticalScrollBarVisibilityProperty); } set { VerticalScrollBarVisibility.Bind = value; } }
    /// <summary>
    //     Gets or sets a value that indicates whether a horizontal scroll bar is shown.
    //     This is a dependency property.
    ///</summary>
    /// <remarks>
    //     A value that is defined by the System.Windows.Controls.ScrollBarVisibility
    //     enumeration.The default value is System.Windows.Visibility.Hidden.
    ///     </remarks>
    public EnumBl<ScrollBarVisibility> HorizontalScrollBarVisibility { get { return Underlying.Property<ScrollBarVisibility>(TextBoxBase.HorizontalScrollBarVisibilityProperty); } set { VerticalScrollBarVisibility.Bind = value; } }
    /// <summary>
    ///     Gets or sets the number of actions stored in the undo queue.
    ///</summary>
    /// <remarks>
    ///     The number of actions stored in the undo queue. The default is –1, which
    ///     means the undo queue is limited to the memory that is available.
    ///     </remarks>
    /// <exception cref="System.InvalidOperationException">
    ///     System.Windows.Controls.Primitives.TextBoxBase.UndoLimit is set after calling
    ///     System.Windows.Controls.Primitives.TextBoxBase.BeginChange() and before calling
    ///     System.Windows.Controls.Primitives.TextBoxBase.EndChange().
    /// </exception>
    public IntBl UndoLimit { get { return Underlying.Property<int>(TextBoxBase.UndoLimitProperty); } set { UndoLimit.Bind = value; } }

    public Action<BRAND> TextChanged {
      set { CurrentValue.TextChanged += (x, y) => value(ToBrand(new Constant<T>((T)x))); }
    }

  }
  /// <summary>
  /// Represents a control that can be used to display or edit unformatted text.
  /// </summary>
  public partial class TextBoxBl : TextBoxBl<TextBox, TextBoxBl> {
    public TextBoxBl() : base(new Constant<TextBox>(new TextBox())) { }
    public TextBoxBl(Expr<TextBox> Provider) : base(Provider) { }
    public TextBoxBl(CanvasBl canvas) : base(canvas, new TextBox()) {
    }
    public static implicit operator TextBoxBl(Expr<TextBox> target) { return new TextBoxBl((target)); }
    public static implicit operator TextBoxBl(TextBox target) { return (new Constant<TextBox>(target)); }
    static TextBoxBl() { Register(v => v); }


    /// <summary>
    ///     Gets or sets the text contents of the text box. This is a dependency property.
    ///</summary><remarks>
    ///     A string containing the text contents of the text box. The default is an
    ///     empty string ("").
    ///     </remarks>
    public StringBl Text {
      get { return Underlying.Property<string>(TextBox.TextProperty).Bl(); }
      set { Text.Bind = value; }
    }
    /// <summary>
    ///     Gets or sets the maximum number of characters that can be manually entered
    ///     into the text box. This is a dependency property.
    ///</summary><remarks>
    ///     The maximum number of characters that can be manually entered into the text
    ///     box. The default is 0, which indicates no limit.
    ///     </remarks>
    public IntBl MaxLength { get { return Underlying.Property<int>(TextBox.MaxLengthProperty); } set { MaxLength.Bind = value; } }
    /// <summary>
    ///     Gets or sets the maximum number of visible lines. This is a dependency property.
    ///</summary><remarks>
    ///     The maximum number of visible lines. The default is System.Int32.MaxValue.
    ///     </remarks>
    /// <exception cref="System.Exception">
    ///     System.Windows.Controls.TextBox.MaxLines is less than System.Windows.Controls.TextBox.MinLines.
    /// </exception>
    public IntBl MaxLines { get { return Underlying.Property<int>(TextBox.MaxLinesProperty); } set { MaxLines.Bind = value; } }
    /// <summary>
    ///     Gets or sets the minimum number of visible lines. This is a dependency property.
    ///</summary><remarks>
    ///     The minimum number of visible lines. The default is 1.
    ///     </remarks>
    /// <exception cref="System.Exception">
    ///     System.Windows.Controls.TextBox.MinLines is greater than System.Windows.Controls.TextBox.MaxLines.
    /// </exception>
    public IntBl MinLines { get { return Underlying.Property<int>(TextBox.MinLinesProperty); } set { MinLines.Bind = value; } }
  }
  /// <summary>
  /// Represents a control that lets the user select from a range of values by
  ///     moving a System.Windows.Controls.Primitives.Track.Thumb control along a System.Windows.Controls.Primitives.Track.
  /// </summary>
  public partial class SliderBl : RangeBaseBl<Slider,SliderBl> {
    public SliderBl() : base(new Constant<Slider>(new Slider())) { }
    public SliderBl(Expr<Slider> Provider) : base(Provider) { }
    public SliderBl(CanvasBl canvas) : base(canvas, new Slider()) {
      this.ToolTip = this.Value.ToStringBl();
      this.Minimum = 0d;
      this.Maximum = 1d;
      this.Value = 0d;
    }
    public static implicit operator SliderBl(Expr<Slider> target) { return new SliderBl((target)); }
    public static implicit operator SliderBl(Slider target) { return (new Constant<Slider>(target)); }
    static SliderBl() { Register(v => v); }
    /// <summary>
    ///    Gets or sets the orientation of a System.Windows.Controls.Slider. This is
    ///     a dependency property.
    /// </summary>
    /// <returns>
    ///    One of the System.Windows.Controls.Slider.Orientation values. The default
    ///     is System.Windows.Controls.Orientation.Horizontal.
    ///</returns>    
    public OrientationBl Orientation {
      get { return Underlying.Property<Orientation>(Slider.OrientationProperty); }
      set { Orientation.Bind = value; }
    }
  }

  // combine a label with a slider
  public class LabelSliderBl : CanvasBl {
    public readonly SliderBl Slider;
    public readonly ButtonBl Label;
    public LabelSliderBl(CanvasBl Parent)
      : base(Parent) {
      Label = new ButtonBl(this) {
        LeftTop = new PointBl(0, 0),
        Width = Width * .1,
        Font = { Weight = FontWeights.SemiBold, Size = 20 },
        Padding = 0d, ContentAlignment = { Horizontal = HorizontalAlignment.Left },
        Background = Brushes.Transparent,
      };
      Slider = new SliderBl(this) {
        LeftBottom = Label.RightBottom,
        Width = Width - Label.Width,
      };
      Height = Label.Height;
      Width = Parent.Width;
      LabelName = "Value";
      Slider.OnMouseEnter = (x, y) => {
        Slider.Value.Stop(true);
        return true;
      };
      Label.Click = () => {
        Slider.Value.Stop(false);
        Slider.Value.Now = Slider.Minimum;
        Slider.Value.Animate().Forever().AutoReverse().To = Slider.Maximum;
      };
      WidthRatio = .15;
    }
    public static implicit operator DoubleBl(LabelSliderBl v) { return v.Slider; }
    public DoubleBl Value {
      get { return Slider.Value; }
      set { Value.Bind = value; } 
    }
    public DoubleBl Minimum {
      get { return Slider.Minimum; }
      set { Minimum.Bind = value; }
    }
    public DoubleBl Maximum {
      get { return Slider.Maximum; }
      set { Maximum.Bind = value; }
    }
    public DoubleBl WidthRatio {
      set { Label.Width = CanvasParent.Width * value; }
    }

    public StringBl LabelName {
      set {
        Slider.Tag = value;
        var round = Slider.Maximum - Slider.Minimum;
        round = round / 100;
        round = round.Log10;
        var v = Slider.Value;
        var round0 = 10d.Bl().Pow(round.Abs.Ceiling);
        v = (round < 0).Condition(v.RoundN(round0), v);

        Label.Content = value + ": " + v;
      }
    }
  }



  /// <summary>
  /// Represents a control that provides a scroll bar that has a sliding System.Windows.Controls.Primitives.Thumb
  ///     whose position corresponds to a value.
  /// </summary>
  public class ScrollBarBl : RangeBaseBl<ScrollBar, ScrollBarBl> {
    public ScrollBarBl(Expr<ScrollBar> Provider) : base(Provider) { }
    public ScrollBarBl(CanvasBl canvas) : base(canvas, new ScrollBar()) { }
    public ScrollBarBl() : base(null, new ScrollBar()) { }
    /// <summary>
    /// Gets or sets the amount of the scrollable content that is currently visible.
    /// </summary>
    public DoubleBl ViewportSize {
      get {
        return Underlying.Property<double>(ScrollBar.ViewportSizeProperty);
      }
      set {
        ViewportSize.Bind = value;
      }
    }
    /// <summary>
    /// Gets or sets whether the System.Windows.Controls.Primitives.ScrollBar is
    ///     displayed horizontally or vertically. This is a dependency property.
    /// </summary>
    public OrientationBl Orientation {
      get {
        return Underlying.Property<Orientation>(ScrollBar.OrientationProperty);
      }
      set {
        Orientation.Bind = value;
      }
    }
  }
  public abstract partial class DecoratorBl<T, SELF> : FrameworkElementBl<T, SELF>
    where T : Decorator
    where SELF : DecoratorBl<T, SELF> {
    public DecoratorBl(Expr<T> Provider) : base(Provider) { }
    public DecoratorBl(CanvasBl canvas, T target)
      : base(canvas, target) {
    }
    public UIElement Child {
      get { return CurrentValue.Child; }
      set { CurrentValue.Child = value; }
    }
  }
  public class TextBlockBl : FrameworkElementBl<TextBlock, TextBlockBl> {
    public TextBlockBl(Expr<TextBlock> Provider) : base(Provider) { }
    public TextBlockBl(CanvasBl canvas) : base(canvas, new TextBlock()) { }
    public TextBlockBl() : base(new TextBlock()) { }
    public static implicit operator TextBlockBl(Expr<TextBlock> v) { return new TextBlockBl(v); }
    static TextBlockBl() {
      Register(v => v);
    }
    public StringBl Text {
      get { return Underlying.Property<string>(TextBlock.TextProperty); }
      set { Text.Bind = value; }
    }
    /// <summary>
    /// Get or set font properties.
    /// </summary>
    public IFontBl Font {
      get {
        return new FontBl() {
          Underlying = ((FrameworkElementBl) this),
          SizeProperty = TextBlock.FontSizeProperty,
          WeightProperty = TextBlock.FontWeightProperty,
          StyleProperty = TextBlock.FontStyleProperty,
          BrushProperty = TextBlock.ForegroundProperty,
          FamilyProperty = TextBlock.FontFamilyProperty,
        };
      }
      set {
        Font.Bind= (value);
      }
    }
    public EnumBl<TextAlignment> TextAlignment {
      get { return Underlying.Property<TextAlignment>(TextBlock.TextAlignmentProperty); }
      set { TextAlignment.Bind = value; }
    }
    public BrushBl Foreground {
      get { return Underlying.Property<Brush>(TextBlock.ForegroundProperty); }
      set { Foreground.Bind = value; }
    }
    public BrushBl Background {
      get { return Underlying.Property<Brush>(TextBlock.BackgroundProperty); }
      set { Background.Bind = value; }
    }

  }

  public abstract class BorderInfoSet {
    /// <summary>
    /// Gets or sets the System.Windows.Media.Brush that draws the outer border color.
    /// </summary>
    public abstract BrushBl Brush { get; set; }
    /// <summary>
    /// Gets or sets the relative System.Windows.Thickness of a System.Windows.Controls.Border.
    /// </summary>
    public abstract ThicknessBl Thickness { get; set; }
    public void Bind(BorderInfoSet info) {
      this.Brush = info.Brush; this.Thickness = info.Thickness;
    }
  }  
  internal class BorderInfo<T> : BorderInfoSet where T : FrameworkElement {    
    internal DependencyProperty BorderBrushProperty { private get; set; }    
    internal DependencyProperty BorderThicknessProperty { private get; set; }    
    internal Expr<T> Underlying { private get; set; }    
    public override BrushBl Brush {     
      get { return Underlying.Property<Brush>(BorderBrushProperty); }      
      set { Brush.Bind = value; }    }
    public override ThicknessBl Thickness {      
      get { return Underlying.Property<Thickness>(BorderThicknessProperty); }      
      set { Thickness.Bind = value; }    
    }  
  }
  class CornerRadiusArity : Vecs.Arity4<double, CornerRadius, CornerRadiusArity> {
    public override CornerRadius Make(params double[] Params) {
      return new CornerRadius(Params[0], Params[1], Params[2], Params[3]);
    }
    public override double Access(CornerRadius Value, int Idx) {
      switch (Idx) {
        case 0: return Value.TopLeft;
        case 1: return Value.TopRight;
        case 2: return Value.BottomRight;
        case 3: return Value.BottomLeft;
        default: throw new NotSupportedException();
      }
    }
    public override string[] PropertyNames {
      get {
        return new string[] {
        "TopLeft",
        "TopRight",
        "BottomRight",
        "BottomLeft",
      };
      }
    }
    internal static void CheckInit() { Extensions.Trace(ArityI + " " + typeof(CornerRadiusArity)); }
  }

  public abstract class BorderBl<T, BRAND> : DecoratorBl<T, BRAND>, IExtends<BRAND,BorderBl>
    where T : Border
    where BRAND : BorderBl<T, BRAND> {
    public BorderBl(Expr<T> Provider) : base(Provider) { }
    public BorderBl(CanvasBl canvas, T t) : base(canvas, t) { }
    public static implicit operator BorderBl(BorderBl<T, BRAND> canvas) {
      return BorderBl.UpCast(canvas);
    }
  }

  public class BorderBl : BorderBl<Border, BorderBl> {
    public BorderBl(Expr<Border> Provider) : base(Provider) { }
    public BorderBl() : this(new Constant<Border>(new Border())) { }
    public BorderBl(CanvasBl canvas) : base(canvas, new Border()) { }
    public static implicit operator BorderBl(Expr<Border> v) { return new BorderBl(v); }
    static BorderBl() {
      Register(v => v);
      CornerRadiusArity.CheckInit();
    }
    public BrushBl Background {
      get { return Underlying.Property<Brush>(System.Windows.Controls.Border.BackgroundProperty); }
      set { Background.Bind = value; }
    }
    public Point4DBl CornerRadius {
      get { return Ops.VecConvert<CornerRadius, double, Vecs.Vec4<double>>.ToVec.Instance.Make(Underlying.Property<CornerRadius>(System.Windows.Controls.Border.CornerRadiusProperty)); }
      set { CornerRadius.Bind = value; }
    }

    /// <summary>
    /// Get and set border properties.
    /// </summary>
    public BorderInfoSet Border {
      get {
        return new BorderInfo<Border> {
          Underlying = Underlying,
          BorderBrushProperty = System.Windows.Controls.Border.BorderBrushProperty,
          BorderThicknessProperty = System.Windows.Controls.Border.BorderThicknessProperty,
        };
      }
      set {
        Border.Bind(value);
      }
    }
  }
  /// <summary>
  ///     Represents a pop-up window that has content.
  /// </summary>
  public class PopupBl : FrameworkElementBl<Popup, PopupBl> {
    public PopupBl(Expr<Popup> Provider) : base(Provider) { }
    public PopupBl() : base(new Constant<Popup>(new Popup())) { }
    static PopupBl() { Register(v => v); }
    public static implicit operator PopupBl(Expr<Popup> v) { return new PopupBl(v); }
    public static implicit operator PopupBl(Popup v) { return new Constant<Popup>(v); }
    /// <summary>
    ///     Gets or sets a value that indicates whether a System.Windows.Controls.Primitives.Popup
    ///     control can contain transparent content. This is a dependency property. The default is false.
    /// </summary>
    public BoolBl AllowsTransparency {
      get { return Underlying.Property<bool>(Popup.AllowsTransparencyProperty); }
      set { AllowsTransparency.Bind = value; }
    }
    /// <summary>
    ///     Gets or sets the content of the System.Windows.Controls.Primitives.Popup
    ///     control. This is a dependency property. The default is null.
    /// </summary>
    public FrameworkElementBl Child {
      get { return Underlying.Property<UIElement>(Popup.ChildProperty); }
      set { Child.Bind = value; }
    }
    /// <summary>
    ///     Gets or sets a delegate handler method that positions the System.Windows.Controls.Primitives.Popup
    ///     control. This is a dependency property. The default is null.
    /// </summary>
    // public CustomPopupPlacementCallback CustomPopupPlacementCallback { get; set; }

    /// <summary>
    ///     Gets a value that indicates whether a System.Windows.Controls.Primitives.Popup
    ///     is displayed with a drop shadow effect. This is a dependency property.
    /// </summary>
    public BoolBl HasDropShadow {
      get { return Underlying.Property<bool>(Popup.HasDropShadowProperty); }
    }
    /// <summary>
    ///     Get or sets the distance between the target origin and the popup
    ///     alignment point. This is a dependency property. For information about the target origin and popup alignment point,
    ///     see Popup Placement Behavior. The default is 0,0.
    /// </summary>
    public PointBl Offset {
      get {
        DoubleBl h = Underlying.Property<double>(Popup.HorizontalAlignmentProperty);
        DoubleBl v = Underlying.Property<double>(Popup.  VerticalAlignmentProperty);
        return new PointBl(h, v);
      }
      set { Offset.Bind = value; }
    }
    /// <summary>
    ///     Gets or sets a value that indicates whether the System.Windows.Controls.Primitives.Popup
    ///     is visible. This is a dependency property. The default is false.
    /// </summary>
    public BoolBl IsOpen {
      get { return Underlying.Property<bool>(Popup.IsOpenProperty); }
      set { IsOpen.Bind = value; }
    }
    public class PlacementSet {
      internal Expr<Popup> Underlying;
      /// <summary>
      ///     Gets or sets the orientation of the System.Windows.Controls.Primitives.Popup
      ///     control when the control opens, and specifies the behavior of the System.Windows.Controls.Primitives.Popup
      ///     control when it overlaps screen boundaries. This is a dependency property. 
      /// The default is System.Windows.Controls.Primitives.PlacementMode.Bottom.
      /// </summary>
      public EnumBl<PlacementMode> Mode {
        get { return Underlying.Property<PlacementMode>(Popup.PlacementProperty); }
        set { Mode.Bind = value; }
      }
      /// <summary>
      ///     Gets or sets the rectangle relative to which the System.Windows.Controls.Primitives.Popup
      ///     control is positioned when it opens. This is a dependency property. The default is null.
      /// </summary>
      public Point4DBl Rectangle {
        get { return Underlying.Property<Rect>(Popup.PlacementRectangleProperty); }
        set { Rectangle.Bind = value; }
      }
      /// <summary>
      ///     Gets or sets the element relative to which the System.Windows.Controls.Primitives.Popup
      ///     is positioned when it opens. This is a dependency property. The default is null.
      /// </summary>
      public FrameworkElementBl Target {
        get { return Underlying.Property<UIElement>(Popup.PlacementTargetProperty); }
        set { Target.Bind = value; }
      }
      internal PlacementSet Bind {
        set {
          Mode.Bind = value.Mode; Rectangle.Bind = value.Rectangle;
          Target.Bind = value.Target;
        }
      }
    }
    /// <summary>
    ///     Gets or sets the mode, rectangle, and target relative to which the System.Windows.Controls.Primitives.Popup
    ///     control is positioned when it opens. This is a dependency property. 
    /// </summary>
    public PlacementSet Placement {
      get { return new PlacementSet() { Underlying = Underlying }; }
      set { Placement.Bind = value; }
    }
    /// <summary>
    ///     Gets or sets an animation for the opening and closing of a System.Windows.Controls.Primitives.Popup
    ///     control. This is a dependency property. The default is System.Windows.Controls.Primitives.PopupAnimation.None.
    /// </summary>
    public EnumBl<PopupAnimation> PopupAnimation {
      get { return Underlying.Property<PopupAnimation>(Popup.PopupAnimationProperty); }
      set { PopupAnimation.Bind = value; }
    }
    /// <summary>
    ///     Gets or sets a value that indicates whether the System.Windows.Controls.Primitives.Popup
    ///     control closes when the control is no longer in focus. This is a dependency
    ///     property. The default is true.
    /// </summary>
    public BoolBl StaysOpen {
      get { return Underlying.Property<bool>(Popup.StaysOpenProperty); }
      set { StaysOpen.Bind = value; }
    }
  }

  public class WindowBl : ContentControlBl<Window, WindowBl> {
    public WindowBl(Expr<Window> Provider) : base(Provider) { }
    public WindowBl() : base(new Constant<Window>(new Window())) { }
    static WindowBl() {
      Register(v => v);
    }
    public static implicit operator WindowBl(Expr<Window> v) { return new WindowBl(v); }
    public static implicit operator WindowBl(Window  v) { return new Constant<Window>(v); }


    /// <summary>
    ///     Gets or sets the System.Windows.Window that owns this System.Windows.Window. This is not a dependency property.
    /// </summary>
    public WindowBl Owner {
      get { return CurrentValue.Owner; }
      set { CurrentValue.Owner = value.CurrentValue; }
    }

    public BoolBl AllowsTransparency {
      get { return Underlying.Property<bool>(Window.AllowsTransparencyProperty).Bl(); }
      set { AllowsTransparency.Bind = value; }
    }
    public BoolBl ShowInTaskbar {
      get { return Underlying.Property<bool>(Window.ShowInTaskbarProperty).Bl(); }
      set { ShowInTaskbar.Bind = value; }
    }
    public BoolBl IsActive {
      get { return Underlying.Property<bool>(Window.IsActiveProperty).Bl(); }
      set { IsActive.Bind = value; }
    }
    public BoolBl Topmost {
      get { return Underlying.Property<bool>(Window.TopmostProperty).Bl(); }
      set { Topmost.Bind = value; }
    }
    public override DoubleBl Left {
      get { return Underlying.Property<double>(Window.LeftProperty).Bl(); }
      set { this.Left = value; }
    }
    public override DoubleBl Top {
      get { return Underlying.Property<double>(Window.TopProperty).Bl(); }
      set { this.Top = value; }
    }
    public StringBl Title {
      get { return Underlying.Property<string>(Window.TitleProperty).Bl(); }
      set { Title.Bind = value; }
    }
    public EnumBl<WindowStyle> WindowStyle {
      get { return Underlying.Property<WindowStyle>(Window.WindowStyleProperty); }
      set { WindowStyle.Bind = value; }
    }
    public EnumBl<WindowState> WindowState {
      get { return Underlying.Property<WindowState>(Window.WindowStateProperty); }
      set { WindowState.Bind = value; }
    }
    public EnumBl<SizeToContent> SizeToContent {
      get { return Underlying.Property<SizeToContent>(Window.SizeToContentProperty); }
      set { SizeToContent.Bind = value; }
    }

  }


  /// <summary>
  /// Represents a scrollable area that can contain other visible elements.
  /// </summary>
  public class ScrollViewerBl : ContentControlBl<ScrollViewer, ScrollViewerBl> {
    public ScrollViewerBl(Expr<ScrollViewer> Provider) : base(Provider) {}
    public ScrollViewerBl(CanvasBl canvas) : base(canvas, new ScrollViewer()) { }
    static ScrollViewerBl() {
      Register(v => v);
    }
    public static implicit operator ScrollViewerBl(Expr<ScrollViewer> v) { return new ScrollViewerBl(v); }

    /// <summary>
    /// Gets or sets a value that indicates whether elements that support the System.Windows.Controls.Primitives.IScrollInfo
    /// interface are allowed to scroll.
    /// </summary>
    public BoolBl CanContentScroll {
      get { return Underlying.Property<bool>(ScrollViewer.CanContentScrollProperty).Bl(); }
      set { CanContentScroll.Bind = value; }
    }
    public PointBl ContentOffset {
      get {
        var x = Underlying.Property<double>(ScrollViewer.ContentHorizontalOffsetProperty).Bl();
        var y = Underlying.Property<double>(ScrollViewer.ContentVerticalOffsetProperty).Bl();
        return new PointBl(x, y);
      }
    }
    /// <summary>
    /// Gets a value that contains the horizontal and the horizontal offset of the scrolled content.
    /// </summary>
    public PointBl Offset {
      get {
        var x = Underlying.Property<double>(ScrollViewer.HorizontalOffsetProperty).Bl();
        var y = Underlying.Property<double>(ScrollViewer.VerticalOffsetProperty).Bl();
        return new PointBl(x, y);
      }
    }
    /// <summary>
    /// Gets a value that contains the vertical and the horizontal  size of the extent
    /// </summary>
    public PointBl ExtentSize {
      get {
        var x = Underlying.Property<double>(ScrollViewer.ExtentWidthProperty).Bl();
        var y = Underlying.Property<double>(ScrollViewer.ExtentHeightProperty).Bl();
        return new PointBl(x, y);
      }
    }
    /// <summary>
    /// Gets a value that represents the vertical and the horizontal  size of the content element that
    /// can be scrolled.
    /// </summary>
    public PointBl ScrollableSize {
      get {
        var x = Underlying.Property<double>(ScrollViewer.ScrollableWidthProperty).Bl();
        var y = Underlying.Property<double>(ScrollViewer.ScrollableHeightProperty).Bl();
        return new PointBl(x, y);
      }
    }
    /// <summary>
    /// Gets a value that contains the vertical and the horizontal size of the content's viewport
    /// </summary>
    public PointBl ViewportSize {
      get {
        var x = Underlying.Property<double>(ScrollViewer.ViewportWidthProperty).Bl();
        var y = Underlying.Property<double>(ScrollViewer.ViewportHeightProperty).Bl();
        return new PointBl(x, y);
      }
    }
    /// <summary>
    /// Get the Visibility of ScrollBar
    /// </summary>
    public OrientationX<ScrollViewer, ScrollBarVisibility, EnumBl<ScrollBarVisibility>> ScrollBarVisibility {
      get {
        return new OrientationX<ScrollViewer, ScrollBarVisibility, EnumBl<ScrollBarVisibility>>() {
          Provider = Underlying, HorizontalProperty = ScrollViewer.HorizontalScrollBarVisibilityProperty,
          VerticalProperty = ScrollViewer.VerticalScrollBarVisibilityProperty,
        };
      }
    }
    /// <summary>
    /// Get the Visibility of Computed ScrollBar
    /// </summary>
    public OrientationX<ScrollViewer, Visibility, VisibilityBl> ComputedScrollBarVisibility {
      get {
        return new OrientationX<ScrollViewer, Visibility, VisibilityBl>() {
          Provider = Underlying, HorizontalProperty = ScrollViewer.HorizontalScrollBarVisibilityProperty,
          VerticalProperty = ScrollViewer.VerticalScrollBarVisibilityProperty,
        };
      }
    }
  }
  public class OrientationX<TARGET, T, SELF> where SELF : Brand<T,SELF> {
    public Expr<TARGET> Provider;
    public DependencyProperty HorizontalProperty;
    public DependencyProperty VerticalProperty;
    public SELF this[OrientationBl orientation] {
      get {
        SELF h = Brand<T, SELF>.ToBrand(Provider.Property<T>(HorizontalProperty));
        SELF v = Brand<T, SELF>.ToBrand(Provider.Property<T>(VerticalProperty));
        return (orientation == Orientation.Horizontal).Condition(h, v);
      }
      set {
        this[orientation].Bind = value;
      }
    }
    /// <summary>
    /// Gets or sets the value of HorizontalProperty
    /// </summary>
    public SELF Horizontal {
      get { return Brand<T, SELF>.ToBrand(Provider.Property<T>(HorizontalProperty)); }
      set { Horizontal.Bind = value; }
    }
    /// <summary>
    /// Gets or sets the value of VerticalProperty
    /// </summary>
    public SELF Vertical {
      get { return Brand<T, SELF>.ToBrand(Provider.Property<T>(VerticalProperty)); }
      set { Vertical.Bind = value; }
    }
    public SELF X { get { return Horizontal; } set { X.Bind = value; } }
    public SELF Y { get { return Horizontal; } set { X.Bind = value; } }

  }

  /// <summary>
  /// Base class for controls that can switch states, such as System.Windows.Controls.CheckBox.
  /// </summary>
  /// <typeparam name="TARGET"></typeparam>
  public abstract partial class ToggleButtonBl<T,SELF> : ButtonBaseBl<T,SELF> where T : ToggleButton where SELF : ToggleButtonBl<T,SELF> {
    public ToggleButtonBl(Expr<T> Provider) : base(Provider) { }
    public ToggleButtonBl(CanvasBl canvas, T target) : base(canvas, target) { }
    /// <summary>
    ///    Gets or sets whether the System.Windows.Controls.Primitives.ToggleButton
    ///     is checked. This is a dependency property.
    /// </summary>
    /// <returns>
    ///    true if the System.Windows.Controls.Primitives.ToggleButton is checked; false
    ///     if the System.Windows.Controls.Primitives.ToggleButton is unchecked; otherwise
    ///     null. The default is false.
    ///</returns>    
    public BoolBl IsChecked {
      get { return Underlying.Property<bool>(ToggleButton.IsCheckedProperty).Bl(); }
      set { IsChecked.Bind = value; }
    }
    public Action Checked {
      set { Underlying.CurrentValue.Checked += (x, y) => value(); }
    }
    public Action Unchecked {
      set { Underlying.CurrentValue.Unchecked += (x, y) => value(); }
    }
    public BoolBl IsThreeState {
      get { return Underlying.Property<bool>(ToggleButton.IsThreeStateProperty); }
      set { IsThreeState.Bind = value; }
    }
    public static implicit operator BoolBl(ToggleButtonBl<T, SELF> t) { return t.IsChecked; }
    public static implicit operator DoubleBl(ToggleButtonBl<T, SELF> t) { return t.IsChecked.Condition<DoubleBl>(1d,0d); }
    public static BoolBl operator !(ToggleButtonBl<T, SELF> t) {
      return !t.IsChecked;
    }

  }
  /// <summary>
  /// Bl wrapper around a checkbox that epresents a control that a user can select and clear.
  /// </summary>
  public partial class CheckBoxBl : ToggleButtonBl<CheckBox, CheckBoxBl> {
    public CheckBoxBl(Expr<CheckBox> Provider) : base(Provider) { }
    public CheckBoxBl(CanvasBl canvas) : base(canvas, new CheckBox()) { }
    public CheckBoxBl() : base(new CheckBox()) { }
    public static implicit operator CheckBoxBl(Expr<CheckBox> v) { return new CheckBoxBl(v); }
    public static implicit operator CheckBoxBl(CheckBox v) { return (new Constant<CheckBox>(v)); }

    static CheckBoxBl() {
      Register(v => v);
    }
  }
  public partial class RadioButtonBl : ToggleButtonBl<RadioButton, RadioButtonBl> {
    public RadioButtonBl(Expr<RadioButton> Provider) : base(Provider) { }
    public RadioButtonBl(CanvasBl canvas) : base(canvas, new RadioButton()) { }
    public static implicit operator RadioButtonBl(Expr<RadioButton> v) { return new RadioButtonBl(v); }
    public static implicit operator RadioButtonBl(RadioButton v) { return (new Constant<RadioButton>(v)); }
    static RadioButtonBl() {
      Register(v => v);
    }
    public StringBl GroupName {
      get { return Underlying.Property<string>(RadioButton.GroupNameProperty); }
      set { GroupName.Bind = value; }
    }

  }

  public abstract partial class PanelBl<T,SELF> : FrameworkElementBl<T,SELF> where T : Panel where SELF : PanelBl<T,SELF> {
    public PanelBl(Expr<T> Provider) : base(Provider) { }
    public PanelBl() : base() { }
    public PanelBl(CanvasBl canvas, T target) : base(canvas, target) { }
    /// <summary>
    ///    Gets or sets a System.Windows.Media.Brush that is used to fill the area between
    ///     the borders of a System.Windows.Controls.Panel. This is a dependency property.
    /// </summary>
    /// <returns>
    ///    A System.Windows.Media.Brush. This default value is null.
    ///</returns>    
    public BrushBl Background {
      get { return Underlying.Property<Brush>(Canvas.BackgroundProperty).Bl(); }
      set { Background.Bind = value; }
    }
  }
  /// <summary>
  /// Bl wrapper around panel
  /// </summary>
  public sealed partial class PanelBl : PanelBl<Panel,PanelBl> {
    public PanelBl(Expr<Panel> Provider) : base(Provider) { }
    static PanelBl() { Register(s => s); }
    public static implicit operator PanelBl(Expr<Panel> Value) { return new PanelBl(Value); }

  }
  /*
  public partial class ItemCollectionBl : CollectionBl<object, ObjectBl, ItemCollection, ItemCollectionBl> {
    public ItemCollectionBl(Expr<ItemCollection> Underlying) : base(Underlying) { }
  }
   */


  public abstract partial class ItemsControlBl<T,SELF> : ControlBl<T,SELF> where T : ItemsControl where SELF : ItemsControlBl<T,SELF> {
    public ItemsControlBl(Expr<T> Underlying) : base(Underlying) { }
    public ItemsControlBl(CanvasBl canvas, T target) : base(canvas, target) { }
    /// <summary>
    ///     Gets the collection used to generate the content of the System.Windows.Controls.ItemsControl.
    /// </summary>
    /// <returns>
    ///     The collection that is used to generate the content of the System.Windows.Controls.ItemsControl.
    ///     The default is an empty collection.
    /// </returns>
    public ItemCollection Items {
      get { return CurrentValue.Items; }
    }
    /// <summary>
    ///     Gets or sets number of alternating item containers in the System.Windows.Controls.ItemsControl,
    ///     which enables alternating containers to have a unique appearance.
    /// </summary>
    /// <returns>
    ///     The number of alternating item containers in the System.Windows.Controls.ItemsControl.
    /// </returns>
    public IntBl AlternationCount { get { return Underlying.Property<int>(ItemsControl.AlternationCountProperty); } set { AlternationCount.Bind = value; } }
    /// <summary>
    ///     Gets or sets a path to a value on the source object to serve as the visual
    ///     representation of the object. This is a dependency property.
    /// </summary>
    /// <returns>
    ///     The path to a value on the source object. This can be any path, or an XPath
    ///     such as "@Name". The default is an empty string ("").
    /// </returns>
    public StringBl DisplayMemberPath { get { return Underlying.Property<string>(ItemsControl.DisplayMemberPathProperty); } set { DisplayMemberPath.Bind = value; } }
    /// <summary>
    ///     Gets a value that indicates whether the System.Windows.Controls.ItemsControl
    ///     contains items. This is a dependency property.
    /// </summary>
    /// <returns>
    ///     true if the items count is greater than 0; otherwise, false.The default is
    ///     false.
    /// </returns>
    public BoolBl HasItems { get { return Underlying.Property<bool>(ItemsControl.HasItemsProperty); } }
    /// <summary>
    ///     Gets a value that indicates whether the control is using grouping. This is
    ///     a dependency property.
    /// </summary>
    /// <returns>
    ///     true if a control is using grouping; otherwise, false.
    /// </returns>
    public BoolBl IsGrouping { get { return Underlying.Property<bool>(ItemsControl.IsGroupingProperty); } }
    /// <summary>
    ///     Gets or sets a value that indicates whether System.Windows.Controls.TextSearch
    ///     is enabled on the System.Windows.Controls.ItemsControl instance. This is
    ///     a dependency property.
    /// </summary>
    /// <returns>
    ///     true if System.Windows.Controls.TextSearch is enabled; otherwise, false.
    ///     The default is false.
    /// </returns>
    public BoolBl IsTextSearchEnabled { get { return Underlying.Property<bool>(ItemsControl.IsTextSearchEnabledProperty); } set { IsTextSearchEnabled.Bind = value; } }
    /// <summary>
    ///     Gets or sets a composite string that specifies how to format the items in
    ///     the System.Windows.Controls.ItemsControl if they are displayed as strings.
    /// </summary>
    /// <returns>
    ///     A composite string that specifies how to format the items in the System.Windows.Controls.ItemsControl
    ///     if they are displayed as strings.
    /// </returns>
    public StringBl ItemStringFormat { get { return Underlying.Property<string>(ItemsControl.ItemStringFormatProperty); } set { ItemStringFormat.Bind = value; } }

  }
  public abstract partial class SelectorBl<T, SELF> : ItemsControlBl<T, SELF>
    where T : Selector
    where SELF : SelectorBl<T, SELF> {
    public SelectorBl(Expr<T> Underlying) : base(Underlying) { }
    public SelectorBl(CanvasBl canvas, T target) : base(canvas, target) { }
    /// <summary>
    ///      Gets or sets a value that indicates whether a System.Windows.Controls.Primitives.Selector
    ///     should keep the System.Windows.Controls.Primitives.Selector.SelectedItem
    ///     synchronized with the current item in the System.Windows.Controls.ItemsControl.Items
    ///     property. This is a dependency property.
    /// </summary>
    public BoolBl IsSynchronizedWithCurrentItem {
      get {
        var p = Underlying.Property<bool?>(Selector.IsSynchronizedWithCurrentItemProperty);
        return new Ops.MapOperator<bool?, bool>(b => b != null && ((bool)b)) { G = b => b }.Make(p);
      }
      set { IsSynchronizedWithCurrentItem.Bind = value; }
    }
    /// <summary>
    ///     Gets or sets the index of the first item in the current selection or returns
    ///     negative one (-1) if the selection is empty. This is a dependency property.
    /// </summary>
    public IntBl SelectedIndex { get { return Underlying.Property<int>(Selector.SelectedIndexProperty); } set { SelectedIndex.Bind = value; } }
    /// <summary>
    ///     Gets or sets the first item in the current selection or returns null if the
    ///     selection is empty This is a dependency property.
    /// </summary>
    public ObjectBl SelectedItem { get { return Underlying.Property<object>(Selector.SelectedItemProperty); } set { SelectedItem.Bind = value; } }
    /// <summary>
    ///     Gets or sets the value of the System.Windows.Controls.Primitives.Selector.SelectedItem,
    ///     obtained by using System.Windows.Controls.Primitives.Selector.SelectedValuePath.
    ///     This is a dependency property.
    /// </summary>
    public ObjectBl SelectedValue { get { return Underlying.Property<object>(Selector.SelectedValueProperty); } set { SelectedValue.Bind = value; } }
    /// <summary>
    ///     Gets or sets the path that is used to get the System.Windows.Controls.Primitives.Selector.SelectedValue
    ///     from the System.Windows.Controls.Primitives.Selector.SelectedItem. This is
    ///     a dependency property.
    /// </summary>
    public StringBl SelectedValuePath 
    { get { return Underlying.Property<string>(Selector.SelectedValuePathProperty); } set { SelectedValuePath.Bind = value; } }

  }
  public partial class ListBoxBl : SelectorBl<ListBox, ListBoxBl> {
    public ListBoxBl(Expr<ListBox> Underlying) : base(Underlying) { }
    public ListBoxBl(CanvasBl Parent) : base(Parent, new ListBox()) { }
    /// <summary>
    ///     Gets the currently selected items. This is a dependency property.
    /// </summary>
    public CollectionBl<object, ObjectBl, System.Collections.IList> SelectedItems { get { return Underlying.Property<System.Collections.IList>(ListBox.SelectedItemsProperty); } }
    /// <summary>
    ///     Gets or sets the selection behavior for a System.Windows.Controls.ListBox.
    ///     This is a dependency property. The default is System.Windows.Controls.SelectionMode.Single
    ///     selection.
    /// </summary>
    public EnumBl<SelectionMode> SelectionMode { get { return Underlying.Property<SelectionMode>(ListBox.SelectionModeProperty); } set { SelectionMode.Bind = value; } }
  }
  public partial class ComboBoxBl : SelectorBl<ComboBox, ComboBoxBl> {
    public ComboBoxBl(Expr<ComboBox> Underlying) : base(Underlying) { }
    public ComboBoxBl(CanvasBl Parent) : base(Parent, new ComboBox()) { }
    public ComboBoxBl() : base(new ComboBox()) { }

    /// <summary>
    ///     Gets or sets a value that indicates whether the drop-down for a combo box
    ///     is currently open. This is a dependency property. The default is false.
    /// </summary>
    public BoolBl IsDropDownOpen { get { return Underlying.Property<bool>(ComboBox.IsDropDownOpenProperty); } set { IsDropDownOpen.Bind = value; } }
    /// <summary>
    ///     Gets or sets a value that enables or disables editing of the text in text
    ///     box of the System.Windows.Controls.ComboBox. This is a  dependency property. The default is false.
    /// </summary>
    public BoolBl IsEditable { get { return Underlying.Property<bool>(ComboBox.IsEditableProperty); } set { IsEditable.Bind = value; } }
    /// <summary>
    ///     Gets or sets a value that enables selection-only mode, in which the contents
    ///     of the combo box are selectable but not editable. This is a dependency property. The default is false.
    /// </summary>
    public BoolBl IsReadOnly { get { return Underlying.Property<bool>(ComboBox.IsReadOnlyProperty); } set { IsReadOnly.Bind = value; } }
    
    /// <summary>
    ///     Gets whether the System.Windows.Controls.ComboBox.SelectionBoxItem is highlighted.
    /// </summary>
    //public BoolBl IsSelectionBoxHighlighted { get { return Underlying.Property<bool>(ComboBox.IsSe); } }
    
    /// <summary>
    ///     Gets or sets the maximum height for a combo box drop-down. This is a dependency
    ///     property.    The default value as defined to the property system is a calculated value
    ///     based on taking a one-third fraction of the system max screen height parameters,
    ///     but this default is potentially overridden by various control templates.
    /// </summary>
    public DoubleBl MaxDropDownHeight { get { return Underlying.Property<double>(ComboBox.MaxDropDownHeightProperty); } set { MaxDropDownHeight.Bind = value; } }
    /// <summary>
    ///     Gets the item that is displayed in the selection box. This is a dependency
    ///     property.
    /// </summary>
    public ObjectBl SelectionBoxItem { get { return Underlying.Property<object>(ComboBox.SelectionBoxItemProperty); } }
    /// <summary>
    ///     Gets a composite string that specifies how to format the selected item in
    ///     the selection box if it is displayed as a string.
    /// </summary>
    public StringBl SelectionBoxItemStringFormat { get { return Underlying.Property<string>(ComboBox.SelectionBoxItemStringFormatProperty); } }
    
    /// <summary>
    ///     Gets the item template of the selection box content. This is a dependency
    ///     property.
    /// </summary>
    //public DataTemplate SelectionBoxItemTemplate { get; }
    
    /// <summary>
    ///     Gets or sets whether a System.Windows.Controls.ComboBox that is open and
    ///     displays a drop-down control will remain open when a user clicks the System.Windows.Controls.TextBox.
    ///     This is a dependency property. True to keep the drop-down control open when the user clicks on the text
    ///     area to start editing; otherwise, false. The default is false.
    /// </summary>
    public BoolBl StaysOpenOnEdit { get { return Underlying.Property<bool>(ComboBox.StaysOpenOnEditProperty); } set { StaysOpenOnEdit.Bind = value; } }
    /// <summary>
    ///     Gets or sets the text of the currently selected item. This is a dependency
    ///     property. The default is an empty string
    ///     ("").
    /// </summary>
    public StringBl Text { get { return Underlying.Property<string>(ComboBox.TextProperty); } set { Text.Bind = value; } }
  }




  public abstract partial class CanvasBl<T, BRAND> : PanelBl<T, BRAND>, IExtends<BRAND,CanvasBl>
    where T : Canvas
    where BRAND : CanvasBl<T, BRAND> {
    public CanvasBl(Expr<T> Provider) : base(Provider) { }
    public CanvasBl() : base() { }
    public CanvasBl(CanvasBl canvas, T target) : base(canvas, target) { }
    public void Add(params FrameworkElementBl[] es) {
      foreach (var e in es) e.CanvasParent = this;
    }
    public static implicit operator CanvasBl(CanvasBl<T, BRAND> canvas) {
      return CanvasBl.UpCast(canvas);
    }
    static CanvasBl() {  }
    /*
    public CollectionBl<FrameworkElement, FrameworkElementBl> Children0 {
      get {


      }
    }
    */

    public UIElementCollection Children {
      get { return CurrentValue.Children; }
    }
    public FrameworkElementBl Child(int n) {
      return ((FrameworkElement)Children[n]).Bl();
    }
    public FrameworkElementBl Last {
      get {
        return Child(Children.Count - 1);
      }
    }
    public System.Windows.Threading.Dispatcher Dispatcher {
      get { return CurrentValue.Dispatcher; }
    }

    public LinearGradientBrushBl MakeInnerBorder(ColorBl start, ColorBl stop, PointBl thickness) {
      var Content = this;
      Func<Orientation, bool, LinearGradientBrushBl> MakeShade = (orient, isEnd) => {
        var rect = new RectangleBl(Content) {
          ZIndex = 1, IsHitTestVisible = false,
          Left = new PointBl(!isEnd ? 0 : Content.Width - thickness.X, 0)[(int) orient],
          Top = new PointBl(0, !isEnd ? 0 : Content.Height - thickness.Y)[(int) orient],

          Size = new PointBl(new PointBl(thickness.Y, Content.Width)[(int) orient],
                             new PointBl(Content.Height, thickness.X)[(int) orient]),
        };
        var brush = new LinearGradientBrushBl() {
          RelativeTransform = { Rotate = (90 * (new PointBl(isEnd ? -1 : +1, isEnd ? +1 : -1)[(int) orient])).ToDegrees() },
        }.Stop(start, 0).Stop(stop, 1);
        rect.Fill = brush;
        return brush;
      };
      LinearGradientBrushBl[] brushes = new LinearGradientBrushBl[] {
        MakeShade(Orientation.Horizontal, false),
        MakeShade(Orientation.Horizontal, true),
        MakeShade(Orientation.Vertical, false),
        MakeShade(Orientation.Vertical, true),
      };
      for (int i = 1; i < 4; i++) {
        brushes[i].MappingMode = brushes[0].MappingMode;
        brushes[i].Opacity = brushes[0].Opacity;
        brushes[i].SpreadMethod = brushes[0].SpreadMethod;
      }
      return brushes[0];
    }
  }
  public class RightSizeCanvasBl : CanvasBl<RightSizeCanvas, RightSizeCanvasBl> {
    public RightSizeCanvasBl(Expr<RightSizeCanvas> v) : base(v) { }
    public RightSizeCanvasBl(CanvasBl Parent) : base(Parent, new RightSizeCanvas()) { }
    public static implicit operator RightSizeCanvasBl(Expr<RightSizeCanvas> v) { return new RightSizeCanvasBl(v); }
    public static implicit operator RightSizeCanvasBl(RightSizeCanvas v) { return new Constant<RightSizeCanvas>(v); }
    public PointBl AtSize {
      get { return CurrentValue.AtSize; }
    }

  }

  public class RightSizeCanvas : Canvas {

    public PointBl AtSize { get; private set; }
    private PointBl AtSize0 = new PointBl(0, 0);
    public RightSizeCanvas() {}
    protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved) {
      if (visualAdded != null && visualAdded is FrameworkElement) {
        FrameworkElementBl m = (FrameworkElement)visualAdded;
        AtSize = AtSize0;
        AtSize0 = (AtSize.Max(m.RightBottom));
        this.Bl().Size = AtSize0;
      }
      if (visualRemoved != null && visualRemoved is FrameworkElement) {
        throw new NotSupportedException();
      }
      base.OnVisualChildrenChanged(visualAdded, visualRemoved);
    }
  }
  /// <summary>
  /// Bl wrapper around canvas which defines an area within which you can explicitly position child elements by
  /// using coordinates that are relative to the System.Windows.Controls.Canvas
  /// area.
  /// </summary>
  public partial class CanvasBl : CanvasBl<Canvas, CanvasBl> {
    private CanvasBl(Expr<Canvas> Provider) : base(Provider) { }
    public CanvasBl() : this((Expr<Canvas>) new Constant<Canvas>(new Canvas())) { }
    public CanvasBl(CanvasBl canvas) : base(canvas, new Canvas()) { }
    public static implicit operator CanvasBl(Canvas target) { return new CanvasBl((Expr<Canvas>) new Constant<Canvas>(target)); }
    public static implicit operator CanvasBl(Expr<Canvas> target) { return new CanvasBl((target)); }
    /// <summary>
    /// No canvas, use when you want to remove an element from a canvas by assigning CanvasParent.
    /// </summary>
    public static CanvasBl None {
      get {
        if (((object)None0) == null) None0 = new CanvasBl(((Expr<Canvas>)new Constant<Canvas>(null)));
        return None0;
      }
    }
    private static CanvasBl None0;

    static CanvasBl() {
      Register(v => v);
    }

  }
  /// <summary>
  /// Bl wrapper around imageSource which represents a object type that has a width, height, and System.Windows.Media.ImageMetadata
  ///     such as a System.Windows.Media.Imaging.BitmapSource and a System.Windows.Media.DrawingImage.
  /// </summary>
  public class ImageSourceBl : Brand<ImageSource, ImageSourceBl> {
    public ImageSourceBl(Expr<ImageSource> underlying) : base(underlying) { }
    public ImageSourceBl() : base() { }
    static ImageSourceBl() {
      Register(v => v);
    }
    public static implicit operator ImageSourceBl(ImageSource Source) { return new ImageSourceBl(new Constant<ImageSource>(Source)); }
    //public static implicit operator ImageSourceBl(System.Drawing.Bitmap bitmap) { return bitmap.LoadBitmap(); }
    public static implicit operator ImageSourceBl(Expr<ImageSource> v) { return new ImageSourceBl(v); }

    public DoubleBl Width {
      get { return CurrentValue.Width; }
    }
    public DoubleBl Height {
      get { return CurrentValue.Height; }
    }
    /// <summary>
    ///  Size of image source.
    /// </summary>
    public PointBl Size {
      get { return new PointBl(Width, Height); }
    }
  }

  /// <summary>
  /// Bl wrapper around imageSource which represents a control that displays an image
  /// </summary>
  public partial class ImageBl : FrameworkElementBl<Image,ImageBl> {
    public ImageBl(Expr<Image> Provider) : base(Provider) { }
    public ImageBl() : this(new Constant<Image>(new Image())) { }
    public ImageBl(CanvasBl canvas) : base(canvas, new Image()) { }
    /// <summary>
    ///     The source of the drawn image. The default value is null.
    /// </summary>
    public ImageSourceBl Source {
      get { return Underlying.Property<ImageSource>(Image.SourceProperty); }
      set { Source.Bind = value; }
    }

    /// <summary>
    ///     Gets or sets a value that describes how an System.Windows.Controls.Image
    ///     should be stretched to fill the destination rectangle. This is a dependency
    ///     property. The default is System.Windows.Media.Stretch.Uniform.
    /// </summary>
    public EnumBl<Stretch> Stretch {
      get { return Underlying.Property<Stretch>(Image.StretchProperty); }
      set { Stretch.Bind = value; }
    }
    /// <summary>
    ///     Gets or sets a value that indicates how the image is scaled. This is a dependency
    ///     property. The default is
    ///     System.Windows.Controls.StretchDirection.Both.
    /// </summary>
    public EnumBl<StretchDirection> StretchDirection {
      get { return Underlying.Property<StretchDirection>(Image.StretchDirectionProperty); }
      set { StretchDirection.Bind = value; }
    }

    public static implicit operator ImageBl(Expr<Image> v) { return new ImageBl(v); }
    public static implicit operator ImageBl(Image v) { return new Constant<Image>(v); }
    static ImageBl() { Register(v => v); }
  }
  public interface StrokeSet {
    /// <summary>
    ///     A System.Windows.Media.Brush that specifies how the System.Windows.Shapes.Shape
    ///     outline is painted. The default is null.
    /// </summary>
    BrushBl Brush { get; set; }
    /// <summary>
    ///     The width of the System.Windows.Shapes.Shape outline.
    /// </summary>
    DoubleBl Thickness { get; set; }
    /// <summary>
    /// This stroke's dash properties.
    /// </summary>
    DashSet Dash { get; set; }
    StrokeSet Bind { set; }
    /// <summary>
    /// The line properties of this stroke.
    /// </summary>
    LineSet Line { get; set; }
    /// <summary>
    ///     The limit on the ratio of the miter length to the System.Windows.Shapes.Shape.StrokeThickness
    ///     of a System.Windows.Shapes.Shape element. This value is always a positive
    ///     number that is greater than or equal to 1.
    /// </summary>
    DoubleBl MiterLimit { get; set;  }
  }
  public interface LineSet {
    /// <summary>
    ///     Gets or sets a System.Windows.Media.PenLineCap enumeration value that describes
    ///     the System.Windows.Shapes.Shape at the end of a line. The default
    ///     is System.Windows.Media.PenLineCap.Flat.
    /// </summary>
    EnumBl<PenLineCap> EndCap { get; set; }
    /// <summary>
    ///     Gets or sets a System.Windows.Media.PenLineJoin enumeration value that specifies
    ///     the type of join that is used at the vertices of a System.Windows.Shapes.Shape.
    /// </summary>
    EnumBl<PenLineJoin> Join { get; set; }
    /// <summary>
    ///     Gets or sets a System.Windows.Media.PenLineCap enumeration value that describes
    ///     the System.Windows.Shapes.Shape at the start of a System.Windows.Shapes.Shape.Stroke. The default
    ///     is System.Windows.Media.PenLineCap.Flat.
    /// </summary>
    EnumBl<PenLineCap> StartCap { get; set; }
    LineSet Bind { set; }
  }
  public interface DashSet {
    /// <summary>
    ///     A System.Double that represents the distance within the dash pattern where
    ///     a dash begins.
    /// </summary>
    DoubleBl Offset { get; set; }
    /// <summary>
    ///     A collection of System.Double values that specify the pattern of dashes and
    ///     gaps.
    /// </summary>
    DoubleCollectionBl Array { get; set; }
    /// <summary>
    ///     Gets or sets a System.Windows.Media.PenLineCap enumeration value that specifies
    ///     how the ends of a dash are drawn. The default
    ///     is System.Windows.Media.PenLineCap.Flat.
    /// </summary>
    EnumBl<PenLineCap> Cap { get; set; }
    DashSet Bind { set; }
  }

  /// <summary>
  /// Bl wrapper around a base class for shape elements
  /// </summary>
  public abstract partial class ShapeBl<T,SELF> : FrameworkElementBl<T,SELF> where T : Shape where SELF : ShapeBl<T,SELF> {
    public ShapeBl(Expr<T> Provider) : base(Provider) { }
    public ShapeBl(CanvasBl canvas, T shape) : base(canvas, shape) {
      this.Stroke.Brush = Brushes.Black;
      this.Stroke.Thickness = 1d;   
    }
    static ShapeBl() { ShapeBl.CheckInit(); }
    /// <summary>
    ///    Gets or sets the System.Windows.Media.Brush that specifies how the shape's
    ///     interior is painted.
    /// </summary>
    /// <returns>
    ///    A System.Windows.Media.Brush that describes how the shape's interior is painted.
    ///     The default is null.
    ///</returns>    
    public BrushBl Fill {
      get {
        
        return Underlying.Property<Brush>(Shape.FillProperty).Bl(); }
      set { Fill.Bind = value; }
    }
    private class StrokeSetImpl : StrokeSet {
      internal Expr<T> Underlying;
      /// <summary>
      ///     A System.Windows.Media.Brush that specifies how the System.Windows.Shapes.Shape
      ///     outline is painted. The default is null.
      /// </summary>
      public BrushBl Brush {
        get { return Underlying.Property<Brush>(Shape.StrokeProperty).Bl(); }
        set { Brush.Bind = value; }
      }
      /// <summary>
      ///     The width of the System.Windows.Shapes.Shape outline.
      /// </summary>
      public DoubleBl Thickness {
        get { return Underlying.Property<double>(Shape.StrokeThicknessProperty).Bl(); }
        set { Thickness.Bind = value; }
      }
      private class DashSetImpl : DashSet {
        internal Expr<T> Underlying;
        /// <summary>
        ///     A System.Double that represents the distance within the dash pattern where
        ///     a dash begins.
        /// </summary>
        public DoubleBl Offset {
          get { return Underlying.Property<double>(Shape.StrokeDashOffsetProperty).Bl(); }
          set { Offset.Bind = value; }
        }
        /// <summary>
        ///     A collection of System.Double values that specify the pattern of dashes and
        ///     gaps.
        /// </summary>
        public DoubleCollectionBl Array {
          get { return Underlying.Property<DoubleCollection>(Shape.StrokeDashArrayProperty); }
          set { Array.Bind = value; }
        }
        /// <summary>
        ///     Gets or sets a System.Windows.Media.PenLineCap enumeration value that specifies
        ///     how the ends of a dash are drawn. The default
        ///     is System.Windows.Media.PenLineCap.Flat.
        /// </summary>
        public EnumBl<PenLineCap> Cap {
          get { return Underlying.Property<PenLineCap>(Shape.StrokeDashCapProperty); }
          set { Cap.Bind = value; }
        }
        public DashSet Bind {
          set {
            this.Offset.Bind = value.Offset;
            this.Array.Bind = value.Array;
            this.Cap.Bind = value.Cap;
          }
        }
      }
      /// <summary>
      /// This stroke's dash properties.
      /// </summary>
      public DashSet Dash {
        get { return new DashSetImpl() { Underlying = Underlying }; }
        set { this.Dash.Bind = value; }
      }
      /// <summary>
      ///     The limit on the ratio of the miter length to the System.Windows.Shapes.Shape.StrokeThickness
      ///     of a System.Windows.Shapes.Shape element. This value is always a positive
      ///     number that is greater than or equal to 1.
      /// </summary>
      public DoubleBl MiterLimit {
        get { return Underlying.Property<double>(Shape.StrokeMiterLimitProperty); }
        set { MiterLimit.Bind = value; }
      }
      private class LineSetImpl : LineSet {
        internal Expr<T> Underlying;
        /// <summary>
        ///     Gets or sets a System.Windows.Media.PenLineCap enumeration value that describes
        ///     the System.Windows.Shapes.Shape at the end of a line. The default
        ///     is System.Windows.Media.PenLineCap.Flat.
        /// </summary>
        public EnumBl<PenLineCap> EndCap {
          get { return Underlying.Property<PenLineCap>(Shape.StrokeEndLineCapProperty); }
          set { EndCap.Bind = value; }
        }
        /// <summary>
        ///     Gets or sets a System.Windows.Media.PenLineJoin enumeration value that specifies
        ///     the type of join that is used at the vertices of a System.Windows.Shapes.Shape.
        /// </summary>
        public EnumBl<PenLineJoin> Join {
          get { return Underlying.Property<PenLineJoin>(Shape.StrokeLineJoinProperty); }
          set { Join.Bind = value; }
        }
        /// <summary>
        ///     Gets or sets a System.Windows.Media.PenLineCap enumeration value that describes
        ///     the System.Windows.Shapes.Shape at the start of a System.Windows.Shapes.Shape.Stroke. The default
        ///     is System.Windows.Media.PenLineCap.Flat.
        /// </summary>
        public EnumBl<PenLineCap> StartCap {
          get { return Underlying.Property<PenLineCap>(Shape.StrokeStartLineCapProperty); }
          set { StartCap.Bind = value; }
        }
        public LineSet Bind {
          set {
            this.EndCap.Bind = value.EndCap;
            this.StartCap.Bind = value.StartCap;
            this.Join = value.Join;
          }
        }
      }
      /// <summary>
      /// The line properties of this stroke.
      /// </summary>
      public LineSet Line {
        get { return new LineSetImpl() { Underlying = Underlying }; }
        set { Line.Bind = value; }
      }
      public StrokeSet Bind {
        set {
          this.Brush.Bind = value.Brush;
          this.Thickness.Bind = value.Thickness;
          this.MiterLimit.Bind = value.MiterLimit;
          this.Dash.Bind = value.Dash;
          this.Line.Bind = value.Line;
        }
      }
    }
    /// <summary>
    /// Properties of a shape's stroke.
    /// </summary>
    public StrokeSet Stroke {
      get { return new StrokeSetImpl() { Underlying = Underlying }; }
      set { Stroke.Bind = (value); }
    }
  }
  /// <summary>
  /// Bl wrapper around shape
  /// </summary>
  public sealed partial class ShapeBl : ShapeBl<Shape,ShapeBl> {
    public ShapeBl(Expr<Shape> Provider) : base(Provider) { }
    static ShapeBl() {
      Register(v => v);
    }
    public static implicit operator ShapeBl(Expr<Shape> v) { return new ShapeBl(v); }
    public static implicit operator ShapeBl(Shape s) { return new Constant<Shape>(s); }
    public static void CheckInit() { Extensions.Trace("CheckInit: " + typeof(ShapeBl) + " ToBrand=" + ToBrand); }
  }
  /// <summary>
  /// Bl wrapper to draw a rectangle
  /// </summary>
  public partial class RectangleBl : ShapeBl<Rectangle, RectangleBl> {
    public RectangleBl(Expr<Rectangle> Provider) : base(Provider) { }
    public RectangleBl(CanvasBl canvas) : base(canvas, new Rectangle()) { }
    static RectangleBl() { Register(v => v); }
    public static implicit operator RectangleBl(Expr<Rectangle> v) { return new RectangleBl(v); }


    /// <summary>
    /// set the CornerRadius to draw a Rounded Rectangle
    /// </summary>
    public PointBl CornerRadius {
      get { return new PointBl(Underlying.Property<double>(Rectangle.RadiusXProperty), Underlying.Property<double>(Rectangle.RadiusYProperty)); }
      set { CornerRadius.Bind = value; }
    }
  }
  /// <summary>
  /// Bl wrapper to draw a ellipse
  /// </summary>
  public partial class EllipseBl : ShapeBl<Ellipse,EllipseBl> {
    public EllipseBl(Expr<Ellipse> Provider) : base(Provider) { }
    public EllipseBl() : base(new Constant<Ellipse>(new Ellipse())) { }
    public EllipseBl(CanvasBl canvas) : base(canvas, new Ellipse()) { }
    static EllipseBl() { Register(v => v); }
    public static implicit operator EllipseBl(Expr<Ellipse> v) { return new EllipseBl(v); }
  }
  /// <summary>
  /// Bl wrapper to draw a line
  /// </summary>
  public partial class LineBl : ShapeBl<Line, LineBl> {
    public LineBl(Expr<Line> Provider) : base(Provider) { }
    public LineBl(CanvasBl canvas) : base(canvas, new Line()) { }
    public PointBl Vector {
      get { return (End * -1d + Start); } 
    }
    static LineBl() { Register(v => v); }
    public static implicit operator LineBl(Expr<Line> v) { return new LineBl(v); }

    /// <summary>
    /// Gets or sets the start point of the line
    /// </summary>
    public PointBl Start {
      get { return new PointBl(Underlying.Property<double>(Line.X1Property).Bl(), Underlying.Property<double>(Line.Y1Property)); }
      set { Start.Bind = value; }
    }
    /// <summary>
    /// Gets or sets the end point of the line
    /// </summary>
    public PointBl End {
      get { return new PointBl(Underlying.Property<double>(Line.X2Property).Bl(), Underlying.Property<double>(Line.Y2Property)); }
      set { End.Bind = value; }
    }
    public FrameworkElement EndThumb {

      set {
        FrameworkElementBl e = value.Bl();
        Opacity = e.Opacity;
        ZIndex = e.ZIndex;
        Visibility = e.Visibility;
        End.Bind = e.CenterPosition;
      }
    }

    /// <summary>
    /// initialize the line with both of its start and  end point
    /// </summary>
    [Obsolete("just set start and end directly")]
    public LineBl Init(PointBl Start, PointBl End) {
      this.Start = Start;
      this.End = End;
      return this;
    }
    /// <summary>
    /// if draw thumbs at the both ends of the line to drag or scale
    /// </summary>
    public override bool HasThumbs {
      set {
        if (value) {
          new ThumbBl(CanvasParent) {
            Background = Stroke.Brush,
            MapF = (self,p) => p.Clamp(new Point(0, 0), CanvasParent.Size),
            DragPoint = Start, ZIndex = ZIndex + 1
          };
          new ThumbBl(CanvasParent) {
            Background = Stroke.Brush,
            MapF = (self,p) => p.Clamp(new Point(0, 0), CanvasParent.Size),
            DragPoint = End, ZIndex = ZIndex + 1
          };
        }
      }
    }
  }


  /// <summary>
  /// Defines the different orientations that a control or layout can have.
  /// </summary>
  public class OrientationBl : ValueBrand<Orientation, OrientationBl> {
    public OrientationBl(Expr<Orientation> v) : base(v) { }
    public static implicit operator OrientationBl(Expr<Orientation> v) { return new OrientationBl(v); }
    public static implicit operator OrientationBl(Orientation o) { return new Constant<Orientation>(o); }

    static OrientationBl() {
      Register(v => v);
    }
    /// <summary>
    /// Gets or sets the orientation horizontal or not
    /// </summary>
    public BoolBl IsHorizontal {
      get { return this == Orientation.Horizontal; }
      set { this.Bind = value.Condition<OrientationBl>(Orientation.Horizontal, Orientation.Vertical); }
    }
    /// <summary>
    /// Gets or sets the orientation vertical or not
    /// </summary>
    public BoolBl IsVertical {
      get { return !IsHorizontal; }
      set { IsHorizontal = !value; }
    }
    /// <summary>
    /// Reverse the orientation
    /// </summary>
    public OrientationBl Reverse {
      get { return IsHorizontal.Condition<OrientationBl>(Orientation.Vertical, Orientation.Horizontal); }
    }
  }
  /// <summary>
  /// Specifies the display state of an element.
  /// </summary>
  public class VisibilityBl : ValueBrand<Visibility, VisibilityBl> {
    public VisibilityBl(Expr<Visibility> v) : base(v) { }
    public static implicit operator VisibilityBl(Expr<Visibility> v) { return new VisibilityBl(v); }
    public static implicit operator VisibilityBl(Visibility o) { return new Constant<Visibility>(o); }
    public static implicit operator VisibilityBl(BoolBl isVisible) {
      return isVisible.Condition<VisibilityBl>(Visibility.Visible, Visibility.Hidden);
    }
    public static implicit operator VisibilityBl(bool isVisible) {
      return isVisible.Bl();
    }
    private class AsBool : Expr<bool> {
      public Expr<Visibility> Underlying;
    protected override string ToString1() {
        return Underlying + " == Visible";
      }
      protected override int GetHashCode0() {
        return Underlying.GetHashCode();
      }
      protected override bool Equals0(Expr<bool> obj) {
        return (obj is AsBool && ((AsBool)obj).Underlying.Equals(Underlying));
      }
      protected override Eval<EVAL>.Info<bool> Eval<EVAL>(Eval<EVAL> txt) {
        return txt.DoEval((((VisibilityBl)Underlying) == Visibility.Visible).Underlying);
      }
      public override bool Solve(IAssign P, Expr<bool> other) {
        return Underlying.Solve(P, ((VisibilityBl) other.Bl()).Underlying);
      }
    }
    public static implicit operator BoolBl(VisibilityBl isVisible) {
      return new AsBool() { Underlying = isVisible.Underlying };
    }
    public static VisibilityBl operator &(VisibilityBl opA, VisibilityBl opB) {
      return ((BoolBl)opA) & ((BoolBl)opB);
    }
    public static VisibilityBl operator |(VisibilityBl opA, VisibilityBl opB) {
      return ((BoolBl)opA) | ((BoolBl)opB);
    }
    public static VisibilityBl operator !(VisibilityBl opA) {
      return !((BoolBl)opA);
    }
    static VisibilityBl() {
      Register(v => v);
    }
  }




  public class ThicknessArity : Vecs.Arity4<double, Thickness, ThicknessArity> {
    public override string[] PropertyNames {
      get { return new string[] { "Left", "Top", "Right", "Bottom", }; }
    }
    public override Thickness Make(params double[] Params) {
      return new Thickness() { Left = Params[0], Top = Params[1], Right = Params[2], Bottom = Params[3] };
    }
    public override double Access(Thickness Value, int Idx) {
      switch (Idx) {
        case 0: return Value.Left;
        case 1: return Value.Top;
        case 2: return Value.Right;
        case 3: return Value.Bottom;
        default: throw new NotSupportedException();
      }
    }
  }

  /// <summary>
  /// Bl wrapper round the Thickness 
  ///  Describes the thickness of a frame around a rectangle. Four 
  ///     values describe the Left, Top,Right, and Bottom sides
  ///     of the rectangle, respectively.
  /// </summary>
  public class ThicknessBl : Point4DBl<Thickness, ThicknessBl, ThicknessArity> {
    public ThicknessBl(Expr<Thickness> v) : base(v) { }
    public ThicknessBl(DoubleBl d) : this(d, d, d, d) { }

    /// <summary>
    ///     Initializes a new instance of the System.Windows.Thickness structure that
    ///     has specific lengths (supplied as a System.Double) applied to each side of
    ///     the rectangle.    
    ///     </summary>
    /// <param name="left">The thickness for the left side of the rectangle.</param>
    /// <param name="top">The thickness for the upper side of the rectangle.</param>
    /// <param name="right">The thickness for the right side of the rectangle.</param>
    /// <param name="bottom">The thickness for the lower side of the rectangle.</param>
    public ThicknessBl(DoubleBl left, DoubleBl top, DoubleBl right, DoubleBl bottom) :
      base(left, top, right, bottom) { }
    /// <summary>
    /// Initializes a new instance of the System.Windows.Thickness structure that
    ///     has specific lengths (supplied as a System.Double) applied to each side of
    ///     the rectangle. 
    /// </summary>
    /// <param name="leftTop">pointBl.X:the thickness for the left side of the rectangle, and pintBl.Y:the thickness for the upper side of the rectangle.</param>
    /// <param name="rightBottom">pointBl.X:the thickness for the right side of the rectangle, and pintBl.Y:the thickness for the lower side of the rectangle.</param>
    public ThicknessBl(PointBl leftTop, PointBl rightBottom)
      : this(leftTop.X, leftTop.Y, rightBottom.X, rightBottom.Y) { }

    public static implicit operator ThicknessBl(Expr<Thickness> t) { return new ThicknessBl(t); }
    public static implicit operator ThicknessBl(Thickness t) { return new ThicknessBl(new Constant<Thickness>(t)); }
    public static implicit operator ThicknessBl(DoubleBl t) { return new ThicknessBl(t); }
    public static implicit operator ThicknessBl(double t) { return new ThicknessBl(t); }
    /// <summary>
    /// Gets the width, in pixels, of the left side of the bounding rectangle.
    /// </summary>
    public DoubleBl Left { get { return this[0]; } set { Left.Bind = value; } }
    /// <summary>
    /// Gets  the hight, in pixels, of the upper side of the bounding rectangle.
    /// </summary>
    public DoubleBl Top { get { return this[1]; } set { Top.Bind = value; } }
    /// <summary>
    /// Gets the width, in pixels, of the right side of the bounding rectangle.
    /// </summary>
    public DoubleBl Right { get { return this[2]; } set { Right.Bind = value; } }
    /// <summary>
    /// Gets the height, in pixels, of the lower side of the bounding rectangle.
    /// </summary>
    public DoubleBl Bottom { get { return this[3]; } set { Bottom.Bind = value; } }

    /// <summary>
    /// Gets width in pixels of left/right bounding rectangles.
    /// </summary>
    public DoubleBl Width { get { return Left; } set { Left = value; Right = value; } }
    /// <summary>
    /// Gets height in pixels of top/bottom bounding rectangles.
    /// </summary>
    public DoubleBl Height { get { return Height; } set { Height = value; Bottom = value; } }
    static ThicknessBl() {
      Register(v => v);
    }
  }

  /// <summary>
  ///     Provides a custom bitmap effect.
  /// </summary>
  public abstract class EffectBl<T, BRAND> : Brand<T, BRAND>, IExtends<BRAND,EffectBl>
    where T : Effect
    where BRAND : EffectBl<T, BRAND> {
    public EffectBl(Expr<T> Provider) : base(Provider) { }
    public static implicit operator EffectBl(EffectBl<T, BRAND> effect) {
      return EffectBl.UpCast(effect);
    }
  }
  /// <summary>
  ///     Provides a custom bitmap effect.
  /// </summary>
  public partial class EffectBl : EffectBl<Effect, EffectBl> {
    public EffectBl(Expr<Effect> Provider) : base(Provider) { }
    public EffectBl(Effect Provider) : base(new Constant<Effect>(Provider)) { }
    public static implicit operator EffectBl(Expr<Effect> v) { return new EffectBl(v); }
    public static implicit operator EffectBl(Effect v) { return new EffectBl(v); }
    static EffectBl() {
      Register(v => v);
    }
    public static readonly EffectBl None = new EffectBl(new Constant<Effect>(null));
  }
  public class ShaderEffectBl : EffectBl<ShaderEffect, ShaderEffectBl> {
    public ShaderEffectBl(Expr<ShaderEffect> Provider) : base(Provider) { }
    public ShaderEffectBl(ShaderEffect Provider) : this(new Constant<ShaderEffect>(Provider)) { }

    public static implicit operator ShaderEffectBl(Expr<ShaderEffect> v) { return new ShaderEffectBl(v); }
    public static implicit operator ShaderEffectBl(ShaderEffect v) { return new ShaderEffectBl(v); }
    static ShaderEffectBl() {
      Register(v => v);
    }
  }

  public interface HasRenderingBias {
    EnumBl<RenderingBias> RenderingBias { get; set; }
  }

  /// <summary>
  ///     A blur effect.
  /// </summary>
  public class BlurEffectBl : EffectBl<BlurEffect, BlurEffectBl>, HasRenderingBias {
    public BlurEffectBl(Expr<BlurEffect> Provider) : base(Provider) { }
    public BlurEffectBl(BlurEffect Provider) : this(new Constant<BlurEffect>(Provider)) { }
    public BlurEffectBl() : this(new BlurEffect()) { }

    public static implicit operator BlurEffectBl(Expr<BlurEffect> v) { return new BlurEffectBl(v); }
    public static implicit operator BlurEffectBl(BlurEffect v) { return new BlurEffectBl(v); }
    static BlurEffectBl() {
      Register(v => v);
    }

    /// <summary>
    /// Gets or sets a value that indicates the radius of the blur effect's curve. This is a dependency property. 
    /// </summary>
    public DoubleBl Radius {
      get { return Underlying.Property<double>(BlurEffect.RadiusProperty); }
      set { Radius.Bind = value; }
    }
    /// <summary>
    ///     Gets or sets a value that indicates whether the system renders an effect
    ///     with emphasis on speed or quality. This is a dependency property. The default
    ///     is System.Windows.Media.Effects.RenderingBias.Performance.
    /// </summary>
    public EnumBl<RenderingBias> RenderingBias {
      get { return Underlying.Property<RenderingBias>(BlurEffect.RenderingBiasProperty); }
      set { RenderingBias.Bind = value; }
    }
    /// <summary>
    ///     Gets or sets a value representing the curve that is used to calculate the
    ///     blur. This is a dependency property. The default is System.Windows.Media.Effects.KernelType.Gaussian.
    /// </summary>
    public EnumBl<KernelType> KernelType {
      get { return Underlying.Property<KernelType>(BlurEffect.KernelTypeProperty); }
      set { KernelType.Bind = value; }
    }
  }
  /// <summary>
  ///     A bitmap effect that paints a drop shadow around the target texture.
  /// </summary>
  public class DropShadowEffectBl : EffectBl<DropShadowEffect, DropShadowEffectBl>, HasRenderingBias {
    public DropShadowEffectBl(Expr<DropShadowEffect> Provider) : base(Provider) { }
    public DropShadowEffectBl(DropShadowEffect Provider) : this(new Constant<DropShadowEffect>(Provider)) { }
    public DropShadowEffectBl() : this(new DropShadowEffect()) { }

    public static implicit operator DropShadowEffectBl(Expr<DropShadowEffect> v) { return new DropShadowEffectBl(v); }
    public static implicit operator DropShadowEffectBl(DropShadowEffect v) { return new DropShadowEffectBl(v); }
    static DropShadowEffectBl() {
      Register(v => v);
    }

    /// <summary>
    ///     A value that indicates the radius of the shadow's blur effect. The default
    ///     is 5.
    /// </summary>
    public DoubleBl BlurRadius {
      get { return Underlying.Property<double>(DropShadowEffect.BlurRadiusProperty); }
      set { BlurRadius.Bind = value; }
    }
    /// <summary>
    ///     The color of the drop shadow. The default is System.Windows.Media.Colors.Black.
    /// </summary>
    public ColorBl Color {
      get { return Underlying.Property<Color>(DropShadowEffect.ColorProperty); }
      set { Color.Bind = value; }
    }
    /// <summary>
    ///     The direction of the drop shadow, in degrees. The default is 315.
    /// </summary>
    public DegreeBl Direction {
      get { return Underlying.Property<double>(DropShadowEffect.DirectionProperty).Bl().ToDegrees(); }
      set { Direction.Bind = value; }
    }
    /// <summary>
    ///     The opacity of the drop shadow. The default is 1.
    /// </summary>
    public DoubleBl Opacity {
      get { return Underlying.Property<double>(DropShadowEffect.OpacityProperty); }
      set { Opacity.Bind = value; }
    }
    /// <summary>
    ///     The distance of the drop shadow below the texture. The default is 5.
    /// </summary>
    public DoubleBl ShadowDepth {
      get { return Underlying.Property<double>(DropShadowEffect.ShadowDepthProperty); }
      set { ShadowDepth.Bind = value; }
    }
    /// <summary>
    ///     A System.Windows.Media.Effects.RenderingBias value that indicates whether
    ///     the system renders the drop shadow with emphasis on speed or quality. The
    ///     default is System.Windows.Media.Effects.RenderingBias.Performance.
    /// </summary>
    public EnumBl<RenderingBias> RenderingBias {
      get { return Underlying.Property<RenderingBias>(DropShadowEffect.RenderingBiasProperty); }
      set { RenderingBias.Bind = value; }
    }
  }

  /// <summary>
  ///  Represents the class for user interface (UI) elements that use a System.Windows.Controls.ControlTemplate
  //     to define their appearance.
  /// </summary>
  public sealed partial class ControlBl : ControlBl<Control, ControlBl> {
    public ControlBl(Expr<Control> Provider) : base(Provider) { }
  }
  public abstract class ButtonBaseBl<T, SELF> : ContentControlBl<T, SELF>
    where T : ButtonBase
    where SELF : ButtonBaseBl<T, SELF> {
    public ButtonBaseBl(Expr<T> Provider) : base(Provider) { }
    public ButtonBaseBl(CanvasBl canvas, T target) : base(canvas, target) { }

    public BoolBl IsPressed {
      get { return Underlying.Property<bool>(ButtonBase.IsPressedProperty); }
    }
    public Action Click {
      set { CurrentValue.Click += (x, y) => value(); }
    }
    public Action<SELF> Click0 {
      set { CurrentValue.Click += (x, y) => value(this); }
    }
  }

  public class ButtonBl : ButtonBaseBl<Button, ButtonBl> {
    public ButtonBl(Expr<Button> Provider) : base(Provider) { }
    public ButtonBl(CanvasBl canvas) : base(canvas, new Button()) { }
    public BoolBl IsCancel {
      get { return Underlying.Property<bool>(Button.IsCancelProperty); }
      set { IsCancel.Bind = value; }
    }
    public BoolBl IsDefaulted {
      get { return Underlying.Property<bool>(Button.IsDefaultedProperty); }
      set { IsDefaulted.Bind = value; }
    }
    public BoolBl IsDefault {
      get { return Underlying.Property<bool>(Button.IsDefaultProperty); }
      set { IsDefault.Bind = value; }
    }
  }
  public class ToggleButtonBl : ToggleButtonBl<ToggleButton, ToggleButtonBl> {
    public ToggleButtonBl(Expr<ToggleButton> Provider) : base(Provider) { }
    public ToggleButtonBl(CanvasBl canvas) : base(canvas, new ToggleButton()) { }
    

  }
  public class ContentControlBl : ContentControlBl<ContentControl, ContentControlBl> {
    public ContentControlBl(Expr<ContentControl> Provider) : base(Provider) { }
    public ContentControlBl(CanvasBl canvas) : base(canvas, new ContentControl()) { }
    public static implicit operator ContentControlBl(Expr<ContentControl> c) { return new ContentControlBl(c); }
    public static implicit operator ContentControlBl(ContentControl c) { return new Constant<ContentControl>(c); }
  }

  public abstract partial class ContentControlBl<T,BRAND> : ControlBl<T,BRAND>, IExtends<BRAND,ContentControlBl> where T : ContentControl where BRAND : ContentControlBl<T,BRAND> {
    public ContentControlBl(Expr<T> Provider) : base(Provider) { }
    public ContentControlBl(CanvasBl canvas, T target) : base(canvas, target) { }
    public static implicit operator ContentControlBl(ContentControlBl<T, BRAND> c) {
      return ContentControlBl.UpCast(c);
    }

    /// <summary>
    ///    Gets or sets the content of a System.Windows.Controls.ContentControl. This
    ///     is a dependency property.
    /// </summary>
    /// <returns>
    ///    An object that contains the control's content. The default value is null.
    ///</returns>    
    public ObjectBl Content {
      get { return Underlying.Property<object>(ContentControl.ContentProperty).Bl(); }
      set { Content.Bind = value; }
    }


  }

  /// <summary>
  /// Represents the text label for a control and provides support for access keys.
  /// </summary>
  public partial class LabelBl : ContentControlBl<Label, LabelBl> {
    public LabelBl(Expr<Label> Provider) : base(Provider) { }
    public LabelBl() : this(new Constant<Label>(new Label())) { }
    public LabelBl(CanvasBl canvas)
      : base(canvas, new Label()) {
    }
    static LabelBl() {
      Register(v => v);
    }
    public static implicit operator LabelBl(Expr<Label> v) { return new LabelBl(v); }

  }


  /// <summary>
  /// Represents a control that can be dragged by the user.
  /// </summary>
  public partial class ThumbBl : ControlBl<Thumb, ThumbBl> {
    public ThumbBl(Expr<Thumb> Provider) : base(Provider) { }
    public ThumbBl() : this(new Constant<Thumb>(new Thumb())) { }
    public ThumbBl(CanvasBl canvas)
      : base(canvas, new Thumb()) {
      Size = new Point(10, 10);
    }
    //public static implicit operator PointWp(ThumbBl thumb) { return thumb.CenterPosition; }
    public static implicit operator PointBl(ThumbBl thumb) { return thumb.CenterPosition; }

    public Func<ThumbBl,PointBl, PointBl> MapF = (self,p) => p;


    private Action<Point> CanDragF;

    public Action<ThumbBl,PointBl> CanDragG {
      set {
        CanDragF = (p) => value(this,p);

      }
    }

    /// <summary>
    /// set the thumb if can be dragged
    /// </summary>
    public bool CanDrag {
      set {
        if (value) {
          if (CanDragF == null) {
            CanDragF = PointBl.Assign<Point>(p => LeftTop, (p,lhs) => MapF(this, lhs + p.Bl()));

          }
          ((Thumb)this.CurrentValue).DragDelta += (x, y) => CanDragF((Point)y.Delta());
            
            //((Thumb)x).Bl().LeftTop.Now = Map(this, ((Thumb)x).Bl().LeftTop + y.Delta());
        } 
      }
    }
    /// <summary>
    /// set the dragpoint
    /// </summary>
    public PointBl DragPoint {
      set {
        this.CenterPosition.Bind = value;
        ((Thumb)this.CurrentValue).DragDelta += (x, y) => value.Now = MapF(this, value + ((PointBl)(Point) y.Delta()));
      }
    }
    /// <summary>
    /// Is this thumb currently being dragged by the user?
    /// </summary>
    public BoolBl IsDragging {
      get { return Underlying.Property<bool>(Thumb.IsDraggingProperty); }
    }

    static ThumbBl() {
      Register(v => v);
    }
    public static implicit operator ThumbBl(Expr<Thumb> v) { return new ThumbBl(v); }

  }


  public class GeometryBl<T,BRAND> : Brand<T,BRAND>, IExtends<BRAND,GeometryBl> where T : Geometry where BRAND : GeometryBl<T,BRAND> {
    public GeometryBl(Expr<T> Provider) : base(Provider) { }
    public static implicit operator GeometryBl(GeometryBl<T,BRAND> v) {
      return GeometryBl.UpCast(v) ;
    }
    /// <summary>
    ///     Gets or sets the System.Windows.Media.Transform object applied to a System.Windows.Media.Geometry.
    ///     This is a dependency property.
    ///     The transformation applied to the System.Windows.Media.Geometry. Note that
    ///     this value may be a single System.Windows.Media.Transform or a System.Windows.Media.TransformCollection
    ///     cast as a System.Windows.Media.Transform.
    /// </summary>
    public TransformBl Transform {
      get {
        return new TransformBl(Underlying.Property<Transform>(Geometry.TransformProperty));
      }
      set {
        Transform.Bind = value;
      }
    }
    public static PathGeometryBl operator +(GeometryBl op0, GeometryBl<T,BRAND> op1) {
      return (Geometry.Combine(op0.CurrentValue, op1.CurrentValue, GeometryCombineMode.Union, null));
    }
    public static PathGeometryBl operator -(GeometryBl op0, GeometryBl<T, BRAND> op1) {
      return (Geometry.Combine(op0.CurrentValue, op1.CurrentValue, GeometryCombineMode.Exclude, null));
    }
    public static PathGeometryBl operator *(GeometryBl op0, GeometryBl<T, BRAND> op1) {
      return (Geometry.Combine(op0.CurrentValue, op1.CurrentValue, GeometryCombineMode.Intersect, null));
    }
    public static PathGeometryBl operator ^(GeometryBl op0, GeometryBl<T, BRAND> op1) {
      return (Geometry.Combine(op0.CurrentValue, op1.CurrentValue, GeometryCombineMode.Xor, null));
    }
  }
  /// <summary>
  ///     Represents a complex shape that may be composed of arcs, curves, ellipses,
  ///     lines, and rectangles.
  /// </summary>
  public partial class PathGeometryBl : GeometryBl<PathGeometry, PathGeometryBl> {
    public PathGeometryBl(Expr<PathGeometry> Provider) : base(Provider) { }
    public PathGeometryBl() : this(new Constant<PathGeometry>(new PathGeometry())) { }
    public static implicit operator PathGeometryBl(Expr<PathGeometry> v) { return new PathGeometryBl(v); }
    public static implicit operator PathGeometryBl(PathGeometry geom) { return new PathGeometryBl(new Constant<PathGeometry>(geom)); }
    static PathGeometryBl() { Register(v => v); }

    /// <summary>
    ///     Gets or sets the collection of System.Windows.Media.PathFigure objects that
    ///     describe the path's contents. This is a dependency property.
    ///     A collection of System.Windows.Media.PathFigure objects that describe the
    ///     path's contents. Each individual System.Windows.Media.PathFigure describes
    ///     a shape.
    /// </summary>
    public CollectionBl<PathFigure,PathFigureBl,PathFigureCollection> Figures {
      get { return Underlying.Property<PathFigureCollection>(PathGeometry.FiguresProperty); }
      set { Figures.Bind = value; }
    }
    /// <summary>
    ///     Gets or sets a value that determines how the intersecting areas contained
    ///     in this System.Windows.Media.PathGeometry are combined. This is a dependency
    ///     property. Indicates how the intersecting areas of this System.Windows.Media.PathGeometry
    ///     are combined. The default value is EvenOdd.
    /// </summary>
    public EnumBl<FillRule> FillRule {
      get { return Underlying.Property<FillRule>(PathGeometry.FillRuleProperty); }
      set { FillRule.Bind = value; }
    }
  }
  /// <summary>
  /// Represents a subsection of a geometry, a single connected series of two-dimensional
  ///     geometric segments.
  /// </summary>
  public partial class PathFigureBl : Brand<PathFigure, PathFigureBl> {
    static PathFigureBl() { Register(s => s); }
    public static implicit  operator PathFigureBl(Expr<PathFigure> e) { return new PathFigureBl(e); }

    public PathFigureBl(Expr<PathFigure> Provider) : base(Provider) { }
    public PathFigureBl() : this(new PathFigure()) { }
    /// <summary>
    ///    Gets or sets the Point where the PathFigure
    ///     begins. This is a dependency property. The default value is 0,0.
    /// </summary>    
    public PointBl StartPoint {
      get { return Underlying.Property<Point>(PathFigure.StartPointProperty).Bl(); }
      set { StartPoint.Bind = value; }
    }
    /// <summary>
    ///     Gets or sets a value that specifies whether this figures first and last segments
    ///     are connected. This is a dependency property.
    ///     The default value is false.
    /// </summary>    
    public BoolBl IsClosed {
      get { return Underlying.Property<bool>(PathFigure.IsClosedProperty).Bl(); }
      set { IsClosed.Bind = value; }
    }
    /// <summary>
    ///     Gets or sets whether the contained area of this System.Windows.Media.PathFigure
    ///     is to be used for hit-testing, rendering, and clipping. This is a dependency
    ///     property. The default value
    ///     is true.
    ///</summary>    
    public BoolBl IsFilled {
      get { return Underlying.Property<bool>(PathFigure.IsFilledProperty); }
      set { IsFilled.Bind = value; }
    }

    /// <summary>
    ///     Gets or sets the collection of segments that define the shape of this System.Windows.Media.PathFigure
    ///     object. This is a dependency property. The default value is an empty collection.
    /// </summary>    
    public CollectionBl<PathSegment,PathSegmentBl,PathSegmentCollection> Segments {
      get { return Underlying.Property<PathSegmentCollection>(PathFigure.SegmentsProperty); }
      set { Segments.Bind = value; }
    }
  }
  /// <summary>
  /// Represents a segment of a System.Windows.Media.PathFigure object.
  /// </summary>
  /// <typeparam name="TARGET"></typeparam>
  public abstract partial class PathSegmentBl<T,BRAND> : Brand<T,BRAND>, IExtends<BRAND,PathSegmentBl> where T : PathSegment where BRAND : PathSegmentBl<T,BRAND> {
    public PathSegmentBl(Expr<T> Provider) : base(Provider) { }
    public static implicit operator T(PathSegmentBl<T, BRAND> s) { return s.CurrentValue; }

    /// <summary>
    ///     Gets or sets a value that indicates whether the join between this System.Windows.Media.PathSegment
    ///     and the previous System.Windows.Media.PathSegment is treated as a corner
    ///     when it is stroked with a System.Windows.Media.Pen. This is a dependency
    ///     property.
    /// </summary>
    /// <returns>
    ///     true if the join between this System.Windows.Media.PathSegment and the previous
    ///     System.Windows.Media.PathSegment is not to be treated as a corner; otherwise,
    ///     false. The default is false.
    /// </returns>
    public BoolBl IsSmoothJoin {
      get { return Underlying.Property<bool>(PathSegment.IsSmoothJoinProperty).Bl(); }
      set { IsSmoothJoin.Bind = value; }
    }
    /// <summary>
    ///   Gets or sets a value that indicates whether the segment is stroked. This
    ///     is a dependency property.
    /// </summary>
    /// <returns>
    ///   true if the segment is stroked when a System.Windows.Media.Pen is used to
    ///     render the segment; otherwise, the segment is not stroked. The default is
    ///     true.
    /// </returns>
    public BoolBl IsStroked {
      get { return Underlying.Property<bool>(PathSegment.IsStrokedProperty).Bl(); }
      set { IsStroked.Bind = value; }
    }
    public static implicit operator PathSegmentBl(PathSegmentBl<T, BRAND> value) {
      return PathSegmentBl.UpCast(value);
    }
    static PathSegmentBl() {
      PathSegmentBl.CheckInit();
    }
  }
  /// <summary>
  ///     Represents a segment of a System.Windows.Media.PathFigure object.
  /// </summary>
  public class PathSegmentBl : PathSegmentBl<PathSegment, PathSegmentBl> {
    public PathSegmentBl(Expr<PathSegment> Underlying) : base(Underlying) { }
    //public PathSegmentBl() : base(new Constant<PathSegment>(new PathSegment())) { }
    public static implicit operator PathSegmentBl(Expr<PathSegment> target) { return new PathSegmentBl((target)); }
    public static implicit operator PathSegmentBl(PathSegment target) { return (new Constant<PathSegment>(target)); }
    static PathSegmentBl() { Register(v => v); }
    public static implicit operator PathSegmentBl(PointBl p) {
      return new LineSegmentBl() { Point = p };
    }
    public static void CheckInit() { Extensions.Trace("CheckInit: " + typeof(PathSegmentBl) + " ToBrand=" + ToBrand); }

  }
  /// <summary>
  /// Represents a cubic Bezier curve drawn between two points.
  /// </summary>
  public partial class BezierSegmentBl : PathSegmentBl<BezierSegment, BezierSegmentBl> {
    public BezierSegmentBl(Expr<BezierSegment> Provider) : base(Provider) { }
    public BezierSegmentBl() : this(new BezierSegment()) { }
    static BezierSegmentBl() { Register(v => v); }
    public static implicit operator BezierSegmentBl(Expr<BezierSegment> v) { return new BezierSegmentBl(v); }
    /// <summary>
    ///  All points of this curve.
    /// </summary>
    /// <param name="Start">Start point of curve that is implicit in preceding segment or figure start point.</param>
    public PointBl[] Points(PointBl Start) {
      return new PointBl[] { Start, Point1, Point2, Point3 };
    }
    /// <summary>
    /// The first control point.
    /// </summary>
    public PointBl Point1 {
      get { return Underlying.Property<Point>(BezierSegment.Point1Property).Bl(); }
      set { Point1.Bind = value; }
    }
    /// <summary>
    /// The second control point.
    /// </summary>
    public PointBl Point2 {
      get { return Underlying.Property<Point>(BezierSegment.Point2Property).Bl(); }
      set { Point2.Bind = value; }
    }
    /// <summary>
    /// The end control point.
    /// </summary>
    public PointBl Point3 {
      get { return Underlying.Property<Point>(BezierSegment.Point3Property).Bl(); }
      set { Point3.Bind = value; }
    }
  }
  /// <summary>
  ///     Creates a line between two points in a System.Windows.Media.PathFigure.
  /// </summary>
  public partial class LineSegmentBl : PathSegmentBl<LineSegment, LineSegmentBl> {
    public LineSegmentBl(Expr<LineSegment> Provider) : base(Provider) { }
    public LineSegmentBl() : this(new LineSegment()) { }
    public static implicit operator LineSegmentBl(Expr<LineSegment> v) { return new LineSegmentBl(v); }
    public static implicit operator LineSegmentBl(LineSegment v) { return new Constant<LineSegment>(v); }
    static LineSegmentBl() { Register(v => v); }
    /// <summary>
    /// Gets or sets the end point of the line segment. This is a dependency property.
    /// </summary>
    public PointBl Point {
      get { return Underlying.Property<Point>(LineSegment.PointProperty).Bl(); }
      set { Point.Bind = value; }
    }
  }
  /// <summary>
  /// reates a quadratic Bezier curve between two points in a PathFigure.
  /// </summary>
  public partial class QuadraticBezierSegmentBl : PathSegmentBl<QuadraticBezierSegment, QuadraticBezierSegmentBl> {
    public QuadraticBezierSegmentBl(Expr<QuadraticBezierSegment> Provider) : base(Provider) { }
    public QuadraticBezierSegmentBl() : this(new QuadraticBezierSegment()) { }
    public static implicit operator QuadraticBezierSegmentBl(Expr<QuadraticBezierSegment> v) { return new QuadraticBezierSegmentBl(v); }
    public static implicit operator QuadraticBezierSegmentBl(QuadraticBezierSegment v) { return new Constant<QuadraticBezierSegment>(v); }
    static QuadraticBezierSegmentBl() {
      Register(v => v);
    }
    /* TODO
    public PointBl this[PointBl Start, DoubleBl t, int idx] {
      get { return PointBl.Bezier(t, idx, Points(Start)); }
      set { this[Start, t, idx].Bind = value; }
    }
     */

    /// <summary>
    /// All control points of this curve.
    /// </summary>
    /// <param name="Start">Start point that is implicit in preceding figure or figure start point.</param>
    public PointBl[] Points(PointBl Start) {
      return new PointBl[] { Start, Point1, Point2 };
    }
    /// <summary>
    /// The control point
    /// </summary>
    public PointBl Point1 {
      get { return Underlying.Property<Point>(QuadraticBezierSegment.Point1Property).Bl(); }
      set { Point1.Bind = value; }
    }
    public class MidPointX {
      internal QuadraticBezierSegmentBl Target { private get; set; }
      public PointBl this[PointBl start, DoubleBl t] {
        get {
          return GeometryExtensions.MidPoint(start, Target.Point1, Target.Point2, t);
        }
        set {
          this[start, t].Bind = value;
        }
      }
    }
    public MidPointX MidPoint { get { return new MidPointX() { Target = this }; } }


    /// <summary>
    /// The end point
    /// </summary>
    public PointBl Point2 {
      get { return Underlying.Property<Point>(QuadraticBezierSegment.Point2Property).Bl(); }
      set { Point2.Bind = value; }
    }
  }
  /// <summary>
  ///     Represents an elliptical arc between two points.
  /// </summary>
  public partial class ArcSegmentBl : PathSegmentBl<ArcSegment, ArcSegmentBl> {
    public ArcSegmentBl(Expr<ArcSegment> Provider) : base(Provider) { }
    public ArcSegmentBl() : this(new ArcSegment()) { }
    static ArcSegmentBl() { Register(v => v); }
    public static implicit operator ArcSegmentBl(Expr<ArcSegment> v) { return new ArcSegmentBl(v); }
    /// <summary>
    ///     Gets or sets the endpoint of the elliptical arc. This is a dependency property. The default value is (0,0).
    /// </summary>
    public PointBl Point {
      get { return Underlying.Property<Point>(ArcSegment.PointProperty).Bl(); }
      set { Point.Bind = value; }
    }
    /// <summary>
    ///     A System.Windows.Size structure that describes the x- and y-radius of the
    ///     elliptical arc. The System.Windows.Size structure's System.Windows.Size.Width
    ///     property specifies the arc's x-radius; its System.Windows.Size.Height property
    ///     specifies the arc's y-radius. The default value is 0,0.
    /// </summary>
    public PointBl Size {
      get { 
        return Underlying.Property<Size>(ArcSegment.SizeProperty);
      }
      set { Size.Bind = value; }
    }
    /// <summary>
    /// Like size except the x and y radius are made the same. 
    /// </summary>
    public DoubleBl Radius {
      get {
        var sz = Size;
        sz.Y = sz.X;
        return sz.X;
      }
      set { Radius.Bind = value; }
    }
    /// <summary>
    ///     The amount (in degrees) by which the ellipse is rotated about the x-axis.
    ///     The default value is 0.
    /// </summary>
    public DegreeBl RotationAngle {
      get { return Underlying.Property<double>(ArcSegment.RotationAngleProperty).Bl().ToDegrees(); }
      set { RotationAngle.Bind = value; }
    }
    /// <summary>
    ///     A value that specifies the direction in which the arc is drawn. The default
    ///     value is System.Windows.Media.SweepDirection.Counterclockwise.
    /// </summary>
    public EnumBl<SweepDirection> SweepDirection {
      get { return Underlying.Property<SweepDirection>(ArcSegment.SweepDirectionProperty); }
      set { SweepDirection.Bind = value; }
    }
    /// <summary>
    ///     True if the arc should be greater than 180 degrees; otherwise, false. The
    ///     default value is false.
    /// </summary>
    public BoolBl IsLargeArc {
      get {
        return Underlying.Property<bool>(ArcSegment.IsLargeArcProperty);
      }
      set {
        IsLargeArc.Bind = value;
      }
    }
  }
  public static partial class WPFExtensions {



    /// <summary>
    /// Create WPF Bl wrapper around this object.
    /// </summary>
    public static QuadraticBezierSegmentBl Bl(this QuadraticBezierSegment element) {
      return new QuadraticBezierSegmentBl(new Constant<QuadraticBezierSegment>(element));
    }
    public static ImageSourceBl Bl(this ImageSource element) {
      return element;
    }
    /// <summary>
    /// Create WPF Bl wrapper around this object.
    /// </summary>
    public static BezierSegmentBl Bl(this BezierSegment element) {
      return new BezierSegmentBl(new Constant<BezierSegment>(element));
    }
    public static BezierSegment BezierSegment(this PointBl[] points) {
      return new BezierSegmentBl() {
        Point1 = points[0],
        Point2 = points[1],
        Point3 = points[2],
      };
    }
    public static BezierSegment PolygonSegment(this DoubleBl smoothValue, params PointBl[] Points) {
      return BezierSegment(GeometryExtensions.PolygonSegment(smoothValue, Points));
    }



    public static LineSegmentBl Bl(this LineSegment element) {
      return new LineSegmentBl(new Constant<LineSegment>(element));
    }

    /// <summary>
    /// Create WPF Bl wrapper around this object.
    /// </summary>
    public static PathFigureBl Bl(this PathFigure element) {
      return new PathFigureBl(new Constant<PathFigure>(element));
    }
    /// <summary>
    /// Create WPF Bl wrapper around this object.
    /// </summary>
    public static FrameworkElementBl Bl(this FrameworkElement element) {
      return (element);
    }
    /*
    public static FrameworkElementBl Bl0<T>(this Expr<T> element) where T : FrameworkElement {
      return FrameworkElementBl.UpCast<T>(element);
    }
    */


    public static ImageBl Bl(this Image element) {
      return (element);
    }

    /*
    public static FrameworkElementBl Bl(this FrameworkElement[] elements, IntBl index) {
      return new FrameworkElementBl(elements.BlA(index));
    }
     */
    /// <summary>
    /// Create WPF Bl wrapper around this object.
    /// </summary>
    public static SliderBl Bl(this Slider element) {
      return new SliderBl(element);
    }
    /// <summary>
    /// Create WPF Bl wrapper around this object.
    /// </summary>
    public static ShapeBl Bl(this Shape element) {
      return new ShapeBl(element);
    }
    /*
    public static ShapeBl Bl(this Shape[] elements, IntBl index) {
      return new ShapeBl(elements.BlA(index));
    }
     */
    /// <summary>
    /// Create WPF Bl wrapper around this object.
    /// </summary>
    public static LineBl Bl(this Line element) {
      return new LineBl(element);
    }
    /// <summary>
    /// Create WPF Bl wrapper around this object.
    /// </summary>
    public static RectangleBl Bl(this Rectangle element) {
      return new RectangleBl(element);
    }

    /*
    public static RectBl Bl(this Value<Rect> element) {
      return (element);
    }
     */
    public static OrientationBl Bl(this Expr<Orientation> element) {
      return (element);
    }
    public static OrientationBl Bl(this Orientation element) {
      return (element);
    }/*
    public static RectBl Bl(this Rect element) {
      return (element);
    }*/

    /// <summary>
    /// Create WPF Bl wrapper around this object.
    /// </summary>
    public static EllipseBl Bl(this Ellipse element) {
      return new EllipseBl(element);
    }
    /// <summary>
    /// Create WPF Bl wrapper around this object.
    /// </summary>
    public static ControlBl Bl(this Control element) {
      return new ControlBl(element);
    }

    public static WindowBl Bl(this Window element) {
      return new WindowBl(element);
    }

    /// <summary>
    /// Create WPF Bl wrapper around this object.
    /// </summary>
    public static PanelBl Bl(this Panel element) {
      return new PanelBl(element);
    }
    /// <summary>
    /// Create WPF Bl wrapper around this object.
    /// </summary>
    public static CanvasBl Bl(this Canvas element) {
      return element ;
    }

    public static ContentControlBl Bl(this ContentControl element) {
      return element;
    }

    /// <summary>
    /// Create WPF Bl wrapper around this object.
    /// </summary>
    public static ThumbBl Bl(this Thumb element) {
      return new ThumbBl(element);
    }
    /// <summary>
    /// Create WPF Bl wrapper around this object.
    /// </summary>
    public static SolidColorBrushBl Bl(this SolidColorBrush element) {
      return new SolidColorBrushBl(element);
    }
    /// <summary>
    /// Create WPF Bl wrapper around this object.
    /// </summary>
    public static ImageBrushBl Bl(this ImageBrush element) {
      return new ImageBrushBl(element);
    }
    /// <summary>
    /// Create WPF Bl wrapper around this object.
    /// </summary>
    public static ImageSourceBl Bl(this Expr<ImageSource> element) {
      return new ImageSourceBl(element);
    }
    /// <summary>
    /// Create WPF Bl wrapper around this object.
    /// </summary>
    public static DrawingBl Bl(this Expr<Drawing> element) {
      return new DrawingBl(element);
    }
    /// <summary>
    /// Create WPF Bl wrapper around this object.
    /// </summary>
    public static GeometryBl Bl(this Expr<Geometry> element) {
      return new GeometryBl(element);
    }
    public static GeometryBl Bl(this Geometry element) {
      return element;
    }
    public static PathGeometryBl Bl(this PathGeometry element) {
      return element;
    }

    /// <summary>
    /// Create a line segement bound to this point.
    /// </summary>
    public static LineSegmentBl Segment(this PointBl point) {
      var ret = new LineSegmentBl();
      ret.Point.Bind = point;
      return ret;
    }
    /// <summary>
    /// Install multiple line segments on this path figure.
    /// </summary>
    /// <param name="points">Points of the line segments to install.</param>
    public static void Segments(this PathFigure figure, params PointBl[] points) {
      foreach (var p in points) {
        var ret = p.Segment();
        figure.Segments.Add(ret);
      }
    }
    /// <summary>
    /// Create a line segement bound to this point and added to figure.
    /// </summary>
    public static LineSegment Segment(this PointBl point, PathFigureBl figure) {      
      var ret = point.Segment();
      figure.Segments.Add(ret);
      return ret;
    }
  }
}


