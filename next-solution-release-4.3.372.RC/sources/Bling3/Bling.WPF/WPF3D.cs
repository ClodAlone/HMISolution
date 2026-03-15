using System;
using System.Collections.Generic;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.IO;
using Bling.Core;

using Bling.DSL;
using Bling.Util;
using Bling.WPF;
using linq = Microsoft.Linq.Expressions;
using Bling.Matrices;
using Bling.Linq;

namespace Bling.WPF3D {
  using Bling.Vecs;
  using Bling.Graphics;
  /// <summary>
  ///     Provides a rendering surface for 3-D visual content.
  /// </summary>
  public class Viewport3DBl : FrameworkElementBl<Viewport3D, Viewport3DBl> {
    public Viewport3DBl() : base(new Constant<Viewport3D>(new Viewport3D())) { }
    public Viewport3DBl(Expr<Viewport3D> Provider) : base(Provider) { }
    public Viewport3DBl(CanvasBl canvas) : base(canvas, new Viewport3D()) {}
    public static implicit operator Viewport3DBl(Expr<Viewport3D> target) { return new Viewport3DBl((target)); }
    public static implicit operator Viewport3DBl(Viewport3D target) { return (new Constant<Viewport3D>(target)); }
    static Viewport3DBl() { Register(v => v); }
    /// <summary>
    /// Configures this view port with a flat/bird eye perspective where its visual consumes its entire allocated space.
    /// </summary>
    public PointBl FlatPerspective {
      set {
        var MeshSize = value;
        this.ClipToBounds = false;
        this.IsHitTestVisible = false;
        this.EdgeMode = System.Windows.Media.EdgeMode.Aliased;
        this.BitmapScalingMode = System.Windows.Media.BitmapScalingMode.NearestNeighbor;
        this.Children.AddOne = new ModelVisual3DBl() {
          Content = new SpotLightBl() {
            Color = Colors.White,
            Position = new Point3DBl(MeshSize.X, MeshSize.Y * 0.25, 3),
            Direction = new Point3DBl(0, 0, -1),
            InnerConeAngle = 70d.ToDegrees(),
            OuterConeAngle = 100d.ToDegrees(),
          },
        };
        this.Camera = new OrthographicCameraBl() {
          Position = new Point3DBl(MeshSize.X * 0.5, MeshSize.Y * 0.5, 1.0).Portable,
          Direction = {
            Look = new Point3DBl(0, 0, -1).Portable,
            Up = new Point3DBl(0, 1, 0).Portable,
          },
          Width = MeshSize.X, PlaneDistance = { Far = 5, Near = 0.1 },
        };
      }
    }

    /// <summary>
    ///     Gets or sets a camera object that projects the 3-D contents of the System.Windows.Controls.Viewport3D to
    ///     the 2-D surface of the System.Windows.Controls.Viewport3D. This is a dependency
    ///     property.
    /// </summary>
    public CameraBl Camera {
      get { return Underlying.Property<Camera>(Viewport3D.CameraProperty); }
      set { Camera.Bind = value; }
    }
    /// <summary>
    /// Gets a collection of the System.Windows.Media.Media3D.Visual3D children of
    ///  the System.Windows.Controls.Viewport3D. This is a dependency property.
    /// </summary>
    public CollectionBl<Visual3D,Visual3DBl,Visual3DCollection> Children { get { return Underlying.Property<Visual3DCollection>(Viewport3D.ChildrenProperty); } }
  }
  public abstract class Model3DBl<T, BRAND> : Brand<T, BRAND>, IExtends<BRAND,Model3DBl>
    where T : Model3D
    where BRAND : Model3DBl<T, BRAND> {
    public Model3DBl(Expr<T> Underlying) : base(Underlying) { }

    public static implicit operator Model3DBl(Model3DBl<T, BRAND> mod) {
      return Model3DBl.UpCast(mod);
    }
    static Model3DBl() { Model3DBl.CheckInit(); }
  }
  /// <summary>
  ///     Abstract class that provides functionality for 3-D models.
  /// </summary>
  public class Model3DBl : Model3DBl<Model3D, Model3DBl> {
    public Model3DBl(Expr<Model3D> Underlying) : base(Underlying) { }
    public static implicit operator Model3DBl(Expr<Model3D> target) { return new Model3DBl((target)); }
    public static implicit operator Model3DBl(Model3D target) { return (new Constant<Model3D>(target)); }
    static Model3DBl() { Register(v => v); }
    internal static int CheckInit() { Extensions.Trace("INIT: " + ToBrand); return 0; }
  }
  /// <summary>
  ///     System.Windows.Media.Media3D.Model3D object that represents lighting applied
  ///     to a 3-D scene.
  /// </summary>
  public abstract class LightBl<T, BRAND, GENERIC> : Model3DBl<T, BRAND>, IExtends<BRAND,LightBl>, ILight
    where T : Light
    where BRAND : LightBl<T, BRAND, GENERIC> where GENERIC : LightCl {
    public LightBl(Expr<T> Underlying) : base(Underlying) { }
    /// <summary>
    ///     Gets or sets the color of the light.
    /// </summary>
    public ColorBl Color {
      get { return Underlying.Property<Color>(Light.ColorProperty); }
      set { Color.Bind = value; }
    }
    public static implicit operator LightBl(LightBl<T, BRAND, GENERIC> light) {
      return LightBl.UpCast(light);
    }
    static LightBl() { LightBl.CheckInit(); }

    public static LightGroup operator +(LightBl<T, BRAND, GENERIC> OpA, ILight OpB) {
      return new LightGroup(OpA, OpB);
    }

    protected LightCl Translate(Light light) {
      if (light is SpotLight)        return new        SpotLightCl() { CopyLight = ((SpotLightBl)((SpotLight)light)) };
      if (light is DirectionalLight) return new DirectionalLightCl() { CopyLight = ((DirectionalLightBl)((DirectionalLight)light)) };
      if (light is AmbientLight)     return new AmbientLightCl() { CopyLight = ((AmbientLightBl)((AmbientLight)light)) };
      if (light is PointLight)       return new PointLightCl() { CopyLight = ((PointLightBl)((PointLight)light)) };
      throw new NotSupportedException();
    }
    public virtual Point3DBl DirectionToPosition(Point3DBl Position) { return Translate(CurrentValue).DirectionToPosition(Position); }
    public virtual DoubleBl UseAttenuation(Point3DBl Direction, DoubleBl Distance) { return Translate(CurrentValue).UseAttenuation(Direction, Distance); }
    public virtual DoubleBl UseAttenuation(Point3DBl Direction) { return Translate(CurrentValue).UseAttenuation(Direction); }
  }
  /// <summary>
  ///     System.Windows.Media.Media3D.Model3D object that represents lighting applied
  ///     to a 3-D scene.
  /// </summary>
  public class LightBl : LightBl<Light, LightBl, LightCl> {
    public LightBl(Expr<Light> Underlying) : base(Underlying) { }
    public static implicit operator LightBl(Expr<Light> target) { return new LightBl((target)); }
    public static implicit operator LightBl(Light target) { return (new Constant<Light>(target)); }
    static LightBl() { Register(v => v); }
    internal static int CheckInit() { Extensions.Trace("INIT: " + ToBrand); return 0; }
  }
  /// <summary>
  ///     Light object that projects its effect along a direction specified by a System.Windows.Media.Media3D.Vector3D.
  /// </summary>
  public class DirectionalLightBl : LightBl<DirectionalLight, DirectionalLightBl, DirectionalLightCl>, IDirectionalLight {
    public DirectionalLightBl(Expr<DirectionalLight> Underlying) : base(Underlying) { }
    public DirectionalLightBl() : this(new Constant<DirectionalLight>(new DirectionalLight())) { }
    public static implicit operator DirectionalLightBl(Expr<DirectionalLight> target) { return new DirectionalLightBl((target)); }
    public static implicit operator DirectionalLightBl(DirectionalLight target) { return (new Constant<DirectionalLight>(target)); }
    static DirectionalLightBl() { 
      Register(v => v);
      Vector3DArity.CheckInit();
    }
    /// <summary>
    ///     Represents the vector along which the light's effect will be seen on models
    ///     in a 3-D scene. This is a dependency property.
    ///     Vector3D along which the light projects, and which must have a non-zero magnitude.
    ///     The default value is (0,0,-1).
    /// </summary>
    public Point3DBl Direction {
      get { return Underlying.Property<Vector3D>(DirectionalLight.DirectionProperty); }
      set { Direction.Bind = value; }
    }
    public override Point3DBl DirectionToPosition(Point3DBl Position) 
    { return new DirectionalLightCl() { CopyLight = this }.DirectionToPosition(Position); }
    public override DoubleBl UseAttenuation(Point3DBl Direction, DoubleBl Distance) 
    { return new DirectionalLightCl() { CopyLight = this }.UseAttenuation(Direction, Distance); }
    public override DoubleBl UseAttenuation(Point3DBl Direction) { return new DirectionalLightCl() { CopyLight = this }.UseAttenuation(Direction); }
    
  }
  /// <summary>
  ///     Light object that applies light to objects uniformly, regardless of their
  ///    shape.
  /// </summary>
  public class AmbientLightBl : LightBl<AmbientLight, AmbientLightBl, AmbientLightCl>, IAmbientLight {
    public AmbientLightBl(Expr<AmbientLight> Underlying) : base(Underlying) { }
    public AmbientLightBl() : this(new Constant<AmbientLight>(new AmbientLight())) { }
    public static implicit operator AmbientLightBl(Expr<AmbientLight> target) { return new AmbientLightBl((target)); }
    public static implicit operator AmbientLightBl(AmbientLight target) { return (new Constant<AmbientLight>(target)); }
    static AmbientLightBl() { Register(v => v); }

    public override Point3DBl DirectionToPosition(Point3DBl Position) { return new AmbientLightCl() { CopyLight = this }.DirectionToPosition(Position); }
    public override DoubleBl UseAttenuation(Point3DBl Direction, DoubleBl Distance) { return new AmbientLightCl() { CopyLight = this }.UseAttenuation(Direction, Distance); }
    public override DoubleBl UseAttenuation(Point3DBl Direction) { return new AmbientLightCl() { CopyLight = this }.UseAttenuation(Direction); }
  }
  public abstract class PointLightBl<T, BRAND, GENERIC> : LightBl<T, BRAND, GENERIC>, IPointLight
    where T : PointLightBase
    where BRAND : PointLightBl<T, BRAND, GENERIC> where GENERIC : PointLightCl {
    public PointLightBl(Expr<T> Underlying) : base(Underlying) { }

    private class AttenuationSet : IAttenuationSet {
      internal Expr<T> Underlying;
      /// <summary>
      ///     Gets or sets a constant value by which the intensity of the light diminishes
      ///     over distance. This is a dependency property.
      /// </summary>
      public DoubleBl Constant {
        get { return Underlying.Property<double>(PointLightBase.ConstantAttenuationProperty); }
        set { Constant.Bind = value; }
      }
      /// <summary>
      ///     Gets or sets a value that specifies the linear diminution of the light's
      ///     intensity over distance. This is a dependency property.
      /// </summary>
      public DoubleBl Linear {
        get { return Underlying.Property<double>(PointLightBase.LinearAttenuationProperty); }
        set { Linear.Bind = value; }
      }
      /// <summary>
      ///     Gets or sets a value that specifies the diminution of the light's effect
      ///     over distance, calculated by a quadratic operation. This is a dependency
      ///     property.
      /// </summary>
      public DoubleBl Quadratic {
        get { return Underlying.Property<double>(PointLightBase.QuadraticAttenuationProperty); }
        set { Quadratic.Bind = value; }
      }
      public IAttenuationSet Bind {
        set {
          this.Linear = value.Linear; this.Quadratic = value.Quadratic;
          this.Constant = value.Constant;
        }
      }
      internal AttenuationSet() { }
    }
    /// <summary>
    /// Get or set values that specify the diminution of the light's effect
    /// over distance, calculated by constant, linear, and quadratic operations.
    /// likes:Attenuation = 1/constant + linear * distance + quadratic * distance * distance
    /// </summary>
    public IAttenuationSet Attenuation {
      get { return new AttenuationSet() { Underlying = Underlying }; }
      set { Attenuation.Bind = (value); }
    }
    /// <summary>
    ///     Gets or sets a System.Windows.Media.Media3D.Point3D that specifies the light's
    ///     position in world space. This is a dependency property.
    /// </summary>
    public Point3DBl Position {
      get { return Underlying.Property<Point3D>(PointLightBase.PositionProperty); }
      set { Position.Bind = value; }
    }
    static PointLightBl() {
      Point3DArity.CheckInit();
    }

    /// <summary>
    ///     Gets or sets the distance beyond which the light has no effect. This is a
    ///     dependency property.
    /// </summary>
    public DoubleBl Range {
      get { return Underlying.Property<double>(PointLightBase.RangeProperty); }
      set { Range.Bind = value; }
    }
  }

  /// <summary>
  ///     Represents a light source that has a specified position in space and projects
  ///     its light in all directions.
  /// </summary>
  public class PointLightBl : PointLightBl<PointLight, PointLightBl, PointLightCl> {
    public PointLightBl(Expr<PointLight> Underlying) : base(Underlying) { }
    public PointLightBl() : this(new Constant<PointLight>(new PointLight())) { }
    public static implicit operator PointLightBl(Expr<PointLight> target) { return new PointLightBl((target)); }
    public static implicit operator PointLightBl(PointLight target) { return (new Constant<PointLight>(target)); }
    static PointLightBl() { Register(v => v); }
    public override Point3DBl DirectionToPosition(Point3DBl Position) { return new PointLightCl() { CopyLight = this }.DirectionToPosition(Position); }
    public override DoubleBl UseAttenuation(Point3DBl Direction, DoubleBl Distance) { return new PointLightCl() { CopyLight = this }.UseAttenuation(Direction, Distance); }
    public override DoubleBl UseAttenuation(Point3DBl Direction) { return new PointLightCl() { CopyLight = this }.UseAttenuation(Direction); }
  }
  /// <summary>
  ///     Light object that projects its effect in a cone-shaped area along a specified
  ///     direction.
  /// </summary>
  public class SpotLightBl : PointLightBl<SpotLight, SpotLightBl, SpotLightCl>, ISpotLight {
    public SpotLightBl(Expr<SpotLight> Underlying) : base(Underlying) { }
    public SpotLightBl() : this(new Constant<SpotLight>(new SpotLight())) { }
    public static implicit operator SpotLightBl(Expr<SpotLight> target) { return new SpotLightBl((target)); }
    public static implicit operator SpotLightBl(SpotLight target) { return (new Constant<SpotLight>(target)); }
    static SpotLightBl() { Register(v => v); Vector3DArity.CheckInit();  }
    /// <summary>
    ///     Gets or sets a System.Windows.Media.Media3D.Vector3D that specifies the direction
    ///     in which the System.Windows.Media.Media3D.SpotLight projects its light. This
    ///     is a dependency property. The default value is 0,0,-1.
    /// </summary>
    public Point3DBl Direction {
      get { return Underlying.Property<Vector3D>(SpotLight.DirectionProperty); }
      set { Direction.Bind = value; }
    }
    /// <summary>
    ///     Gets or sets an angle that specifies the proportion of a System.Windows.Media.Media3D.SpotLight's
    ///     cone-shaped projection in which the light fully illuminates objects in the
    ///     scene. This is a dependency property. The default value is 180.
    /// </summary>
    public DegreeBl InnerConeAngle {
      get { return Underlying.Property<double>(SpotLight.InnerConeAngleProperty).Bl().ToDegrees(); }
      set { InnerConeAngle.Bind = value; }
    }
    /// <summary>
    ///     Gets or sets an angle that specifies the proportion of a System.Windows.Media.Media3D.SpotLight's
    ///     cone-shaped projection outside which the light does not illuminate objects
    ///     in the scene. This is a dependency property. The default value is 90.
    /// </summary>
    public DegreeBl OuterConeAngle {
      get { return Underlying.Property<double>(SpotLight.OuterConeAngleProperty).Bl().ToDegrees(); }
      set { OuterConeAngle.Bind = value; }
    }
    public override Point3DBl DirectionToPosition(Point3DBl Position) { return new SpotLightCl() { CopyLight = this }.DirectionToPosition(Position); }
    public override DoubleBl UseAttenuation(Point3DBl Direction, DoubleBl Distance) { return new SpotLightCl() { CopyLight = this }.UseAttenuation(Direction, Distance); }
    public override DoubleBl UseAttenuation(Point3DBl Direction) { return new SpotLightCl() { CopyLight = this }.UseAttenuation(Direction); }
  }
  /// <summary>
  ///     Provides services and properties common to visual 3-D objects, including
  ///     hit-testing, coordinate transformation, and bounding box calculations.
  /// </summary>
  public abstract class Visual3DBl<T, BRAND> : RenderBl<T, BRAND>, IExtends<BRAND,Visual3DBl>
    where T : Visual3D
    where BRAND : Visual3DBl<T, BRAND> {
    public Visual3DBl(Expr<T> Underlying) : base(Underlying) { }
    public static implicit operator Visual3DBl(Visual3DBl<T, BRAND> mat) {
      return Visual3DBl.UpCast(mat);
    }
    /// <summary>
    ///     The transformation to apply to the 3-D object. The default is the System.Windows.Media.Media3D.Transform3D.Identity
    ///     transformation.
    /// </summary>
    public Transform3DBl Transform {
      get { return Underlying.Property<Transform3D>(Visual3D.TransformProperty); }
      set { Transform.Bind = value; }
    }
    static Visual3DBl() { Visual3DBl.CheckInit(); }
  }
  /// <summary>
  ///     Provides services and properties common to visual 3-D objects, including
  ///     hit-testing, coordinate transformation, and bounding box calculations.
  /// </summary>
  public class Visual3DBl : Visual3DBl<Visual3D, Visual3DBl> {
    public Visual3DBl(Expr<Visual3D> Underlying) : base(Underlying) { }
    public static implicit operator Visual3DBl(Expr<Visual3D> target) { return new Visual3DBl((target)); }
    public static implicit operator Visual3DBl(Visual3D target) { return (new Constant<Visual3D>(target)); }
    static Visual3DBl() { Register(v => v); }
    internal static int CheckInit() { Extensions.Trace("INIT: " + ToBrand); return 0; }
  }
  /// <summary>
  ///     System.Windows.Media.Visual that contains 3-D models.
  /// </summary>
  public class ModelVisual3DBl : Visual3DBl<ModelVisual3D, ModelVisual3DBl> {
    public ModelVisual3DBl(Expr<ModelVisual3D> Underlying) : base(Underlying) { }
    public ModelVisual3DBl() : base(new Constant<ModelVisual3D>(new ModelVisual3D())) { }
    public static implicit operator ModelVisual3DBl(Expr<ModelVisual3D> target) { return new ModelVisual3DBl((target)); }
    public static implicit operator ModelVisual3DBl(ModelVisual3D target) { return (new Constant<ModelVisual3D>(target)); }
    static ModelVisual3DBl() { Register(v => v); }
    /// <summary>
    ///     System.Windows.Media.Media3D.Transform3D set on the model. The default value
    ///     is System.Windows.Media.Media3D.MatrixTransform3D.
    /// </summary>
    public new Transform3DBl Transform {
      get { return Underlying.Property<Transform3D>(ModelVisual3D.TransformProperty); }
      set { Transform.Bind = value; }
    }
    /// <summary>
    ///     System.Windows.Media.Media3D.Model3D that comprises the content of the System.Windows.Media.Media3D.ModelVisual3D.
    /// </summary>
    public Model3DBl Content {
      get { return Underlying.Property<Model3D>(ModelVisual3D.ContentProperty); }
      set { Content.Bind = value; }
    }
  }
  /// <summary>
  ///     Renders the 2-D children within the specified 3-D viewport bounds.
  /// </summary>
  public class Viewport2DVisual3DBl : Visual3DBl<Viewport2DVisual3D, Viewport2DVisual3DBl> {
    public Viewport2DVisual3DBl(Expr<Viewport2DVisual3D> Underlying) : base(Underlying) { }
    public Viewport2DVisual3DBl() : base(new Constant<Viewport2DVisual3D>(new Viewport2DVisual3D())) { }
    public static implicit operator Viewport2DVisual3DBl(Expr<Viewport2DVisual3D> target) { return new Viewport2DVisual3DBl((target)); }
    public static implicit operator Viewport2DVisual3DBl(Viewport2DVisual3D target) { return (new Constant<Viewport2DVisual3D>(target)); }
    static Viewport2DVisual3DBl() { Register(v => v); }
    /// <summary>
    ///     The 3-D geometry for this System.Windows.Media.Media3D.Viewport2DVisual3D.
    /// </summary>
    public Geometry3DBl Geometry { get { return Underlying.Property<Geometry3D>(Viewport2DVisual3D.GeometryProperty); } set { Geometry.Bind = value; } }
    /// <summary>
    ///     The material for the 3-D object.
    /// </summary>
    public MaterialWl Material { get { return Underlying.Property<Material>(Viewport2DVisual3D.MaterialProperty); } set { Material.Bind = value; } }
    /// <summary>
    ///     The visual to be placed on the 3-D object.
    /// </summary>
    public VisualBl Visual { get { return Underlying.Property<Visual>(Viewport2DVisual3D.VisualProperty); } set { Visual.Bind = value; } }
  }

  /// <summary>
  ///     Abstract base class for materials.
  /// </summary>
  public abstract class MaterialWl<T, BRAND> : Brand<T, BRAND>, IExtends<BRAND,MaterialWl> 
    where T : Material
    where BRAND : MaterialWl<T, BRAND> {
    public MaterialWl(Expr<T> Underlying) : base(Underlying) { }
    public static implicit operator MaterialWl(MaterialWl<T, BRAND> mat) {
      return MaterialWl.UpCast(mat);
    }
    /// <summary>
    /// XXX: no documentation found.
    /// </summary>
    public BoolBl IsVisualHostMaterial {
      get { return Underlying.Property<bool>(Viewport2DVisual3D.IsVisualHostMaterialProperty); }
      set { IsVisualHostMaterial.Bind = value; }
    }
    static MaterialWl() { MaterialWl.CheckInit(); }
    public abstract Lighting this[ILight Light] { get; }
  }
  /// <summary>
  ///     Abstract base class for materials.
  /// </summary>
  public class MaterialWl : MaterialWl<Material, MaterialWl> {
    public MaterialWl(Expr<Material> Underlying) : base(Underlying) { }
    public static implicit operator MaterialWl(Expr<Material> target) { return new MaterialWl((target)); }
    public static implicit operator MaterialWl(Material target) { return (new Constant<Material>(target)); }
    static MaterialWl() { Register(v => v); }
    internal static int CheckInit() { Extensions.Trace("INIT: " + ToBrand); return 0; }
    public override Lighting this[ILight Light] {
      get { throw new NotImplementedException(); }
    }
  }
  /// <summary>
  ///     Allows the application of a 2-D brush, like a System.Windows.Media.SolidColorBrush
  ///     or System.Windows.Media.TileBrush, to a diffusely-lit 3-D model.
  /// </summary>
  public class DiffuseMaterialWl : MaterialWl<DiffuseMaterial, DiffuseMaterialWl> {
    public DiffuseMaterialWl(Expr<DiffuseMaterial> Underlying) : base(Underlying) { }
    public DiffuseMaterialWl() : this(new Constant<DiffuseMaterial>(new DiffuseMaterial())) { }
    public static implicit operator DiffuseMaterialWl(Expr<DiffuseMaterial> target) { return new DiffuseMaterialWl((target)); }
    public static implicit operator DiffuseMaterialWl(DiffuseMaterial target) { return (new Constant<DiffuseMaterial>(target)); }
    static DiffuseMaterialWl() { Register(v => v); }
    /// <summary>
    ///     A knob, the color allowed to emit from the System.Windows.Media.Media3D.Material.
    ///     The default value is white (#FFFFFF). Since all colors make up white, all colors
    ///     are visible by default.
    /// </summary>
    public ColorBl Color { get { return Underlying.Property<Color>(DiffuseMaterial.ColorProperty); } set { Color.Bind = value; } }
    /// <summary>
    ///     System.Windows.Media.Brush to be applied as a System.Windows.Media.Media3D.Material
    ///     to a 3-D model. This is a dependency property.
    /// </summary>
    public BrushBl Brush { get { return Underlying.Property<Brush>(DiffuseMaterial.BrushProperty); } set { Brush.Bind = value; } }
    /// <summary>
    ///     The color allowed to emit from the ambient light of the material. The default value
    ///     is white (#FFFFFF).
    /// </summary>
    public ColorBl AmbientColor { get { return Underlying.Property<Color>(DiffuseMaterial.AmbientColorProperty); } set { AmbientColor.Bind = value; } }

    public override Lighting this[ILight LightSource] {
      get { // not a great idea.
        return new Lighting((Position, Normal) => {
          var Dir = LightSource.DirectionToPosition(Position);
          var Dis = Dir.Length;
          Dir = Dir.Normalize;
          var Dot0 = Dir.Dot(Normal);
          // ambient light.
          ColorBl Ambient;
          if (LightSource is IAmbientLight) Ambient = AmbientColor * ((IAmbientLight)LightSource).Color;
          else Ambient = Colors.Black;
          ColorBl Diffuse;
          if (LightSource is IAmbientLight) Diffuse = Colors.Black;
          else Diffuse = Color * (LightSource.Color * Dot0.Max(0) * LightSource.UseAttenuation(Dir, Dis));
          return (Input, UV) => { // ignore input? 
            return (Ambient + Diffuse) * ((Tex2D) Brush)[UV];
          };
        });
      }
    }

  }
  /// <summary>
  ///     Allows a 2-D brush, like a System.Windows.Media.SolidColorBrush or System.Windows.Media.TileBrush,
  ///     to be applied to a specularly-lit 3-D model.
  /// </summary>
  public class SpecularMaterialWl : MaterialWl<SpecularMaterial, SpecularMaterialWl> {
    public SpecularMaterialWl(Expr<SpecularMaterial> Underlying) : base(Underlying) { }
    public SpecularMaterialWl() : this(new Constant<SpecularMaterial>(new SpecularMaterial())) { }
    public static implicit operator SpecularMaterialWl(Expr<SpecularMaterial> target) { return new SpecularMaterialWl((target)); }
    public static implicit operator SpecularMaterialWl(SpecularMaterial target) { return (new Constant<SpecularMaterial>(target)); }
    static SpecularMaterialWl() { Register(v => v); }
    /// <summary>
    ///     Gets or sets a value that filters the color properties of the material applied
    ///     to the model. This is a dependency property.
    /// </summary>
    public ColorBl Color { get { return Underlying.Property<Color>( SpecularMaterial.ColorProperty); } set { Color.Bind = value; } }
    /// <summary>
    ///     Gets or sets the 2-D brush to apply to a specularly-lit 3-D model. This is
    ///     a dependency property.
    /// </summary>
    public BrushBl Brush { get { return Underlying.Property<Brush>(SpecularMaterial.BrushProperty); } set { Brush.Bind = value; } }
    /// <summary>
    ///     Relative contribution, for a material applied as a 2-D brush to a 3-D model,
    ///     of the specular component of the lighting model.
    /// </summary>
    public DoubleBl Power { get { return Underlying.Property<double>(SpecularMaterial.SpecularPowerProperty); } set { Power.Bind = value; } }

    public override Lighting this[ILight Light] {
      get { 
        
        
        throw new NotImplementedException(); 
      
      }
    }
  }
  /// <summary>
  ///     Applies a System.Windows.Media.Brush to a 3-D model so that it participates
  ///     in lighting calculations as if the System.Windows.Media.Media3D.Material
  ///     were emitting light equal to the color of the System.Windows.Media.Brush.
  /// </summary>
  public class EmissiveMaterialWl : MaterialWl<EmissiveMaterial, EmissiveMaterialWl> {
    public EmissiveMaterialWl(Expr<EmissiveMaterial> Underlying) : base(Underlying) { }
    public EmissiveMaterialWl() : this(new Constant<EmissiveMaterial>(new EmissiveMaterial())) { }
    public static implicit operator EmissiveMaterialWl(Expr<EmissiveMaterial> target) { return new EmissiveMaterialWl((target)); }
    public static implicit operator EmissiveMaterialWl(EmissiveMaterial target) { return (new Constant<EmissiveMaterial>(target)); }
    static EmissiveMaterialWl() { Register(v => v); }
    /// <summary>
    ///     The color of the light.
    /// </summary>
    public ColorBl Color { get { return Underlying.Property<Color>(EmissiveMaterial.ColorProperty); } set { Color.Bind = value; } }
    /// <summary>
    ///     The brush applied by the System.Windows.Media.Media3D.EmissiveMaterial. The
    ///     default value is null.
    /// </summary>
    public BrushBl Brush { get { return Underlying.Property<Brush>(EmissiveMaterial.BrushProperty); } set { Brush.Bind = value; } }
    public override Lighting this[ILight Light] {
      get { throw new NotImplementedException(); }
    }
  }
  /// <summary>
  ///     Classes that derive from this abstract base class define 3D geometric shapes.
  ///     The System.Windows.Media.Media3D.Geometry3D class of objects can be used
  ///     for hit-testing and rendering 3D graphic data.
  /// </summary>
  public abstract class Geometry3DBl<T, BRAND> : Brand<T, BRAND>, IExtends<BRAND,Geometry3DBl>
    where T : Geometry3D
    where BRAND : Geometry3DBl<T, BRAND> {
    public Geometry3DBl(Expr<T> Underlying) : base(Underlying) { }
    public static implicit operator Geometry3DBl(Geometry3DBl<T, BRAND> geom) {
      return Geometry3DBl.UpCast(geom);
    }
    static Geometry3DBl() { Geometry3DBl.CheckInit(); }
  }
  /// <summary>
  ///     Classes that derive from this abstract base class define 3D geometric shapes.
  ///     The System.Windows.Media.Media3D.Geometry3D class of objects can be used
  ///     for hit-testing and rendering 3D graphic data.
  /// </summary>
  public class Geometry3DBl : Geometry3DBl<Geometry3D, Geometry3DBl> {
    public Geometry3DBl(Expr<Geometry3D> Underlying) : base(Underlying) { }
    public static implicit operator Geometry3DBl(Expr<Geometry3D> target) { return new Geometry3DBl((target)); }
    public static implicit operator Geometry3DBl(Geometry3D target) { return (new Constant<Geometry3D>(target)); }
    static Geometry3DBl() { Register(v => v); }
    internal static int CheckInit() { Extensions.Trace("INIT: " + ToBrand); return 0; }
  }
  /// <summary>
  ///     Triangle primitive for building a 3-D shape.
  /// </summary>
  public class MeshGeometry3DBl : Geometry3DBl<MeshGeometry3D, MeshGeometry3DBl> {
    public MeshGeometry3DBl(Expr<MeshGeometry3D> Underlying) : base(Underlying) { }
    public MeshGeometry3DBl() : this(new Constant<MeshGeometry3D>(new MeshGeometry3D())) { }
    public static implicit operator MeshGeometry3DBl(Expr<MeshGeometry3D> target) { return new MeshGeometry3DBl((target)); }
    public static implicit operator MeshGeometry3DBl(MeshGeometry3D target) { return (new Constant<MeshGeometry3D>(target)); }
    static MeshGeometry3DBl() { Register(v => v); }
    /// <summary>
    ///     Gets or sets a collection of vertex positions for a System.Windows.Media.Media3D.MeshGeometry3D.
    ///     This is a dependency property.
    /// </summary>
    public Point3DCollectionBl Positions { 
      get { return Underlying.Property<Point3DCollection>(MeshGeometry3D.PositionsProperty); } 
      set { Positions.Bind = value; } 
    }
    /// <summary>
    ///     Gets or sets a collection of texture coordinates for the System.Windows.Media.Media3D.MeshGeometry3D.
    ///     This is a dependency property.
    /// </summary>
    public PointCollectionBl TextureCoordinates {
      get { return Underlying.Property<PointCollection>(MeshGeometry3D.TextureCoordinatesProperty); }
      set { TextureCoordinates.Bind = value; }
    }
    /// <summary>
    ///     Collection that contains the triangle indices of the MeshGeometry3D.
    /// </summary>
    public IntCollectionBl TriangleIndices {
      get { return Underlying.Property<Int32Collection>(MeshGeometry3D.TriangleIndicesProperty); }
      set { TriangleIndices.Bind = value; }
    }
    /// <summary>
    ///     Gets or sets a collection of normal vectors for the System.Windows.Media.Media3D.MeshGeometry3D.
    ///     This is a dependency property.
    /// </summary>
    public Vector3DCollectionBl Normals {
      get { return Underlying.Property<Vector3DCollection>(MeshGeometry3D.NormalsProperty); }
      set { Normals.Bind = value; }
    }
  }
  /// <summary>
  /// Associate a mesh with a number of rows and columns, will set all mesh properties accordingly. 
  /// </summary>
  public class GridMesh {
    private readonly MeshGeometry3DBl Mesh;
    /// <summary>
    /// Create a grid mesh with exisiting mesh geometry and specified column and row counts.
    /// </summary>
    /// <param name="Mesh">Existing mesh to manage.</param>
    /// <param name="ColumnCount">Number of columns in mesh.</param>
    /// <param name="RowCount">Number of rows in Mesh.</param>
    public GridMesh(MeshGeometry3DBl Mesh, int ColumnCount, int RowCount) : base() {
      this.Mesh = Mesh;
      this.ColumnCount = ColumnCount;
      this.RowCount = RowCount;
      Init();
    }
    public static implicit operator Geometry3DBl(GridMesh m) { return m.Mesh; }
    /// <summary>
    /// Create a grid mesh with a new mesh geometry and specified column and row counts.
    /// </summary>
    /// <param name="ColumnCount">Number of columns in mesh.</param>
    /// <param name="RowCount">Number of rows in Mesh.</param>
    public GridMesh(int ColumnCount, int RowCount) : this(new MeshGeometry3DBl(), ColumnCount, RowCount) { }
    /// <summary>
    /// Number of columns in mesh.
    /// </summary>
    public readonly int ColumnCount;
    /// <summary>
    /// Number of rows in mesh.
    /// </summary>
    public readonly int RowCount;
    /// <summary>
    /// Create an action that refreshes mesh geometry based on a 2D array of 3D points values.
    /// </summary>
    /// <param name="values">2D array of 3D points, column and row counts must be the same as the grid mesh.</param>
    /// <returns></returns>
    public Action Refresh(Bling.Arrays.Array2D<Point3DBl> values) {
      (values.Column.Count == ColumnCount && values.Row.Count == RowCount).Assert();
      var values0 = values.Vertical1D;
      return Mesh.Positions.RefreshAction(i => values0[i], ColumnCount * RowCount);
    }
    private void Init() {
      (ColumnCount > 1 && RowCount > 1).Assert();
      DoubleBl dx = 100d / ((double)(ColumnCount - 1));
      DoubleBl dy = 100d / ((double)(RowCount - 1));
      List<Point3DBl> Positions = new List<Point3DBl>();
      List<IntBl> TriangleIndices = new List<IntBl>();
      List<PointBl> TextureCoord = new List<PointBl>();
      DoubleBl xp = 0;
      double xn = ColumnCount - 1d;
      double yn = RowCount - 1d;
      for (double x = 0; x < ColumnCount; x++) {
        DoubleBl yp = 0;
        for (double y = 0; y < RowCount; y++) {
          Positions.Add(new Point3DBl(xp, yp, 0));
          TextureCoord.Add(new PointBl(x / xn, 1.0 - y / yn));
          yp += dy;
        }
        xp += dx;
      }
      for (int x = 0; x < ColumnCount - 1; x++) {
        for (int y = 0; y < RowCount - 1; y++) {
          if ((x + y) % 2 == 1) {
            TriangleIndices.Add(y + (x * RowCount));
            TriangleIndices.Add((y) + ((x + 1) * RowCount));
            TriangleIndices.Add((y + 1) + x * RowCount);

            TriangleIndices.Add(y + ((x + 1) * RowCount));
            TriangleIndices.Add((y + 1) + ((x + 1) * RowCount));
            TriangleIndices.Add((y + 1) + ((x) * RowCount));
          } else {
            TriangleIndices.Add(y + (x * RowCount));
            TriangleIndices.Add((y + 1) + ((x + 1) * RowCount));
            TriangleIndices.Add((y + 1) + x * RowCount);

            TriangleIndices.Add(y + (x * RowCount));
            TriangleIndices.Add(y + ((x + 1) * RowCount));
            TriangleIndices.Add((y + 1) + ((x + 1) * RowCount));
          }
        }
      }
      Mesh.Positions         .InitElements(Positions);
      Mesh.TextureCoordinates.InitElements(TextureCoord);
      Mesh.TriangleIndices   .InitElements(TriangleIndices);
    }
  }

 
  /// <summary>
  ///     Represents an imaginary viewing position and direction in 3-D coordinate
  ///     space that describes how a 3-D model is projected onto a 2-D visual.
  /// </summary>
  public abstract class CameraBl<T, BRAND> : Brand<T, BRAND>, IExtends<BRAND,CameraBl>, ICamera
    where T : Camera
    where BRAND : CameraBl<T, BRAND> {
    public CameraBl(Expr<T> Underlying) : base(Underlying) { }

    /// <summary>
    ///     Gets or sets the Transform3D applied to the camera. This is a dependency
    ///     property.
    /// </summary>
    public Transform3DBl Transform {
      get { return Underlying.Property<Transform3D>(Camera.TransformProperty); }
      set { Transform.Bind = value; }
    }
    public MatrixBl<D4, D4> Matrix {
      get { return View * Project; }
    }
    public abstract MatrixBl<D4, D4> View { get; }
    public abstract MatrixBl<D4, D4> Project { get; }


    public static implicit operator CameraBl(CameraBl<T,BRAND> camera) {
      return CameraBl.UpCast(camera);
    }
    static CameraBl() { CameraBl.CheckInit(); }
  }
  /// <summary>
  ///     Represents an imaginary viewing position and direction in 3-D coordinate
  ///     space that describes how a 3-D model is projected onto a 2-D visual.
  /// </summary>
  public class CameraBl : CameraBl<Camera, CameraBl> {
    internal static int CheckInit() { Extensions.Trace("INIT: " + ToBrand); return 0; }
    public CameraBl(Expr<Camera> Underlying) : base(Underlying) { }
    public static implicit operator CameraBl(Expr<Camera> target) { return new CameraBl((target)); }
    public static implicit operator CameraBl(Camera target) { return (new Constant<Camera>(target)); }
    static CameraBl() { Register(v => v); }
    public override MatrixBl<D4, D4> View {
      get { throw new NotImplementedException(); }
    }
    public override MatrixBl<D4, D4> Project {
      get { throw new NotImplementedException(); }
    }
  }
  /// <summary>
  ///     An abstract base class for perspective and orthographic projection cameras.
  /// </summary>
  public abstract class ProjectionCameraBl<T, BRAND> : CameraBl<T, BRAND>, IProjectionCamera
    where T : ProjectionCamera
    where BRAND : ProjectionCameraBl<T, BRAND> {
    public ProjectionCameraBl(Expr<T> Underlying) : base(Underlying) { }

    protected virtual void InitDefaults() {
      Direction.Look = 0d;
      Direction.Up = new Point3DBl(0, 1, 0);
      PlaneDistance.Near = .1d;
      PlaneDistance.Far = 100d;
      Position = 0d;
    }

    public class PlaneDistanceSet : IPlaneDistanceSet {
      internal Expr<T> Underlying;
      /// <summary>
      ///     Gets or sets a value that specifies the distance from the camera of the camera's
      ///     far clip plane. This is a dependency property. Default is 100.
      /// </summary>
      public DoubleBl Far {
        get { return Underlying.Property<double>(ProjectionCamera.FarPlaneDistanceProperty); }
        set { Far.Bind = value; }
      }
      /// <summary>
      ///     Gets or sets a value that specifies the distance from the camera of the camera's
      ///     near clip plane. This is a dependency property. Default is .1.
      /// </summary>
      public DoubleBl Near {
        get { return Underlying.Property<double>(ProjectionCamera.NearPlaneDistanceProperty); }
        set { Near.Bind = value; }
      }
      public IPlaneDistanceSet Bind {
        set { Far = value.Far; Near = value.Near; }
      }
    }
    /// <summary>
    ///     Gets or sets values that specify the distance from the camera of the camera's
    ///     far and near clip planes. This is a dependency property.
    /// </summary>
    public IPlaneDistanceSet PlaneDistance {
      get { return new PlaneDistanceSet() { Underlying = Underlying }; }
      set { ((PlaneDistanceSet) PlaneDistance).Bind=(value); }
    }
    public class DirectionSet : IDirectionSet {
      internal Expr<T> Underlying;
      /// <summary>
      ///     Gets or sets a System.Windows.Media.Media3D.Vector3D which defines the direction
      ///     in which the camera is looking in world coordinates. This is a dependency
      ///     property. Default is (0,0,0).
      /// </summary>
      public Point3DBl Look {
        get { return Underlying.Property<Vector3D>(ProjectionCamera.LookDirectionProperty).Bl(); }
        set { Look.Bind = value; }
      }
      /// <summary>
      ///     Gets or sets a System.Windows.Media.Media3D.Vector3D which defines the upward
      ///     direction of the camera. This is a dependency property.
      /// </summary>
      public Point3DBl Up {
        get { return Underlying.Property<Vector3D>(ProjectionCamera.UpDirectionProperty).Bl(); }
        set { Up.Bind = value; }
      }
      public IDirectionSet Bind {
        set { Look = value.Look; Up = value.Up; }
      }
      static DirectionSet() {
        Vector3DArity.CheckInit();
      }
    }
    /// <summary>
    ///     Get or set points that define the look and upward
    ///     directions of the camera.
    /// </summary>
    public IDirectionSet Direction {
      get { return new DirectionSet() { Underlying = Underlying }; }
      set { ((DirectionSet) Direction).Bind=(value); }
    }
    /// <summary>
    ///     Gets or sets the position of the camera in world coordinates. This is a dependency
    ///     property. Default is (0,0,0).
    /// </summary>
    public Point3DBl Position {
      get { return Underlying.Property<Point3D>(ProjectionCamera.PositionProperty).Bl(); }
      set { Position.Bind = value; }
    }
    public override MatrixBl<D4, D4> View {
      get { return Position.LookAt(Direction.Look, Direction.Up); }
    }
    static ProjectionCameraBl() {
      Point3DArity.CheckInit();
    }
  }
  /// <summary>
  ///     Represents a perspective projection camera.
  /// </summary>
  public class PerspectiveCameraBl : ProjectionCameraBl<PerspectiveCamera, PerspectiveCameraBl>, IPerspectiveCamera {
    public PerspectiveCameraBl(Expr<PerspectiveCamera> Underlying) : base(Underlying) { }
    public PerspectiveCameraBl() : this(new Constant<PerspectiveCamera>(new PerspectiveCamera())) {
      InitDefaults();
    }
    protected override void InitDefaults() {
      base.InitDefaults();
      FieldOfView = 45d.Bl().Degrees();
    }


    public static implicit operator PerspectiveCameraBl(Expr<PerspectiveCamera> target) { return new PerspectiveCameraBl((target)); }
    public static implicit operator PerspectiveCameraBl(PerspectiveCamera target) { return (new Constant<PerspectiveCamera>(target)); }
    static PerspectiveCameraBl() { Register(v => v); }
    /// <summary>
    ///     The camera's horizontal field of view, in degrees. The default value is 45.
    /// </summary>
    public DegreeBl FieldOfView {
      get { return Underlying.Property<double>(PerspectiveCamera.FieldOfViewProperty).Bl().ToDegrees(); }
      set { FieldOfView.Bind = value; }
    }

    public override MatrixBl<D4, D4> Project {
      get { return FieldOfView.AsRadians.PerspectiveFovLH(1d, PlaneDistance.Near, PlaneDistance.Far); }
    }

    // XXX: doesn't work yet.
    public PointBl Size2D {
      set {
        var fovInRadians = FieldOfView.AsRadians;
        var zValue = value.X / (fovInRadians / 2).Tan / 2;
        Point3DBl negativeZAxis = new Vector3D(0, 0, -1);
        Point3DBl positiveYAxis = new Vector3D(0, 1, 0);
        Point3DBl cameraPosition = new Point3DBl(value.X / 2, value.Y / 2, zValue);
        this.Position = cameraPosition;
        this.Direction.Look = negativeZAxis;
        this.Direction.Up = positiveYAxis;
      }
    }

    /*
    public static explicit operator PerspectiveCameraBl(CameraBl camera) {
      return camera.Underlying.Coerce<PerspectiveCamera>(c => (PerspectiveCamera)c);
    }*/
  }
  /// <summary>
  ///     Represents an orthographic projection camera.
  /// </summary>
  public class OrthographicCameraBl : ProjectionCameraBl<OrthographicCamera, OrthographicCameraBl>, IOrthographicCamera {
    public OrthographicCameraBl(Expr<OrthographicCamera> Underlying) : base(Underlying) { }
    public OrthographicCameraBl() : this(new Constant<OrthographicCamera>(new OrthographicCamera())) { }
    public static implicit operator OrthographicCameraBl(Expr<OrthographicCamera> target) { return new OrthographicCameraBl((target)); }
    public static implicit operator OrthographicCameraBl(OrthographicCamera target) { return (new Constant<OrthographicCamera>(target)); }
    static OrthographicCameraBl() { Register(v => v); }
    /// <summary>
    ///     Width of the camera's viewing box, in world units.
    /// </summary>
    public DoubleBl Width {
      get { return Underlying.Property<double>(OrthographicCamera.WidthProperty); }
      set { Width.Bind = value; }
    }
    public override MatrixBl<D4, D4> Project {
      get { throw new NotImplementedException(); }
    }
  }

  public abstract partial class Transform3DBl<T, BRAND> : Brand<T, BRAND>, IExtends<BRAND,Transform3DBl> where T : Transform3D where BRAND : Transform3DBl<T,BRAND> {
    public Transform3DBl(Expr<T> v) : base(v) { }
    public static implicit operator Transform3DBl(Transform3DBl<T, BRAND> value) {
      return Transform3DBl.UpCast(value);
    }
    public MatrixBl<D4, D4> Value {
      get {
        //return Ops.VecConvert<Matrix3D, double, Matrix<D4, D4>>.ToVec.Instance.Make(this);

        var expr = this.Underlying.Map<Matrix3D>(transform => {
          var ret = transform.Value;
          return ret;
        }).Bl();
        return expr;
      }
    }

    public Point3DBl Transform(Point3DBl p) {
      return Combine<Point3D, Point3DBl>(p).Map<Point3D, Point3DBl>((t, q) => t.Transform(q));
    }
    public Point4DBl Transform(Point4DBl p) {
      return Combine<Point4D, Point4DBl>(p).Map<Point4D, Point4DBl>((t, q) => t.Transform(q));
    }
    /// <summary>
    ///     Gets a value that specifies whether the matrix is affine.
    /// </summary>
    public BoolBl IsAffine {
      get { return Map<bool, BoolBl>(t => t.IsAffine); }
    }
    /// <summary>
    /// The inverse of this matrix. 
    /// </summary>
    public Transform3DBl Inverse {
      get {
        Func<T, Transform3D> F = t => (Transform3D)t.Inverse;
        return Map<Transform3D,Transform3DBl>(F);
      }
    }
  }
  public partial class MatrixTransform3DBl : Transform3DBl<MatrixTransform3D, MatrixTransform3DBl> {
    public MatrixTransform3DBl(Expr<MatrixTransform3D> v) : base(v) { }
    public MatrixTransform3DBl() : base(new Constant<MatrixTransform3D>(new MatrixTransform3D())) { }
    static MatrixTransform3DBl() { Register(v => v); }
    public static implicit operator MatrixTransform3DBl(Expr<MatrixTransform3D> v) { return new MatrixTransform3DBl(v); }
    public static implicit operator MatrixTransform3DBl(MatrixTransform3D t) { return new Constant<MatrixTransform3D>(t); }
    /*
    public IBaseArray<DoubleBl> MatrixOld {
      get {
        var Core = Underlying.Property<Matrix3D>(MatrixTransform3D.MatrixProperty);
        return Core.BlOld();
      }
      set {
        var Core = Underlying.Property<Matrix3D>(MatrixTransform3D.MatrixProperty);
        Core.Bind = value.AsMatrix3D();
      }
    }
     */
    public MatrixBl<D4,D4> Matrix {
      get {
        var Core = Underlying.Property<Matrix3D>(MatrixTransform3D.MatrixProperty);
        return Core.Bl();
      }
      set {
        var Core = Underlying.Property<Matrix3D>(MatrixTransform3D.MatrixProperty);
        Core.Bind = value.AsMatrix3D();
      }
    }

  }
  public class Matrix3DArity : MultiArity<double, Matrix3D, Matrix3DArity>, IMatrixArity<double, Matrix3D> {
    public override Matrix3D Make(params double[] Params) {
      return new Matrix3D(Params[0], Params[1], Params[2], Params[3],
                          Params[4], Params[5], Params[6], Params[7],
                          Params[8], Params[9], Params[10], Params[11],
                          Params[12], Params[13], Params[14], Params[15]);
    }
    public int Height { get { return 4; } }
    public int Width  { get { return 4; } }

    public override double Access(Matrix3D Value, int Idx) {
      switch (Idx) {
        case 0: return Value.M11;
        case 1: return Value.M12;
        case 2: return Value.M13;
        case 3: return Value.M14;
        case 4: return Value.M21;
        case 5: return Value.M22;
        case 6: return Value.M23;
        case 7: return Value.M24;
        case 8: return Value.M31;
        case 9: return Value.M32;
        case 10: return Value.M33;
        case 11: return Value.M34;
        case 12: return Value.OffsetX;
        case 13: return Value.OffsetY;
        case 14: return Value.OffsetZ;
        case 15: return Value.M44;
        default: throw new NotSupportedException();
      }
    }
    public override int Count { get { return 16; } }
    public override string[] PropertyNames {
      get {
        return new string[] {
          "M11",
          "M12",
          "M13",
          "M14",

          "M21",
          "M22",
          "M23",
          "M24",

          "M31",
          "M32",
          "M33",
          "M34",

          "OffsetX",
          "OffsetY",
          "OffsetZ",
          "M44",
        };
      }
    }
  }


  /// <summary>
  ///     Provides a parent class for all three-dimensional transformations, including
  ///     translation, rotation, and scale transformations.
  /// </summary>
  public partial class Transform3DBl : Transform3DBl<Transform3D, Transform3DBl> {
    public Transform3DBl(Expr<Transform3D> v) : base(v) { }
    static Transform3DBl() { Register(v => v); }
    public static implicit operator Transform3DBl(Expr<Transform3D> v) { return new Transform3DBl(v); }
    public static implicit operator Transform3DBl(Transform3D t) { return new Constant<Transform3D>(t); }

    public Point3DBl CenterSize = new Point3DBl(0, 0, 0);

    /// <summary>
    /// Set the center point of the scale/skew/rotate transform .
    /// </summary>
    /// <param name="c">Center point of rotation.</param>
    /// <returns>Itself.</returns>
    public Transform3DBl this[Point3DBl c] {
      get {
        this.CenterSize = c;
        return this;
      }
    }
    private TRANSFORM Transform<TRANSFORM>() where TRANSFORM : Transform3D, new() {
      TRANSFORM transform;
      if (Underlying.HasCurrentValue) {
        var current = (Transform3D)this.CurrentValue;
        if (current is TRANSFORM) transform = (TRANSFORM)current;
        else if (current is Transform3DGroup) {
          var group = (Transform3DGroup)current;
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
          var group = new Transform3DGroup();
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
    ///     Specifies a rotation transformation.
    /// </summary>
    public Rotation3DBl Rotate {
      get {
        var rotate = Transform<RotateTransform3D>();
        var px = rotate.Bl<double>(RotateTransform3D.CenterXProperty).Bl();
        var py = rotate.Bl<double>(RotateTransform3D.CenterYProperty).Bl();
        var pz = rotate.Bl<double>(RotateTransform3D.CenterZProperty).Bl();
        (new Point3DBl(px, py, pz)).Bind = CenterSize;
        Rotation3DBl rotateX = rotate.Bl<Rotation3D>(RotateTransform3D.RotationProperty);
        return rotateX;
      }
      set {
        Rotate.Bind = value;
      }
    }
    /// <summary>
    ///     Scales an object in the three-dimensional x-y-z plane, starting from a defined
    ///     center point. Scale factors are defined in x-, y-, and z- directions from
    ///     this center point.
    /// </summary>
    public Point3DBl Scale {
      get {
        var scale = Transform<ScaleTransform3D>();
        var px = scale.Bl<double>(ScaleTransform3D.CenterXProperty).Bl();
        var py = scale.Bl<double>(ScaleTransform3D.CenterYProperty).Bl();
        var pz = scale.Bl<double>(ScaleTransform3D.CenterZProperty).Bl();
        (new Point3DBl(px, py, pz)).Bind = CenterSize;
        var qx = scale.Bl<double>(ScaleTransform3D.ScaleXProperty).Bl();
        var qy = scale.Bl<double>(ScaleTransform3D.ScaleYProperty).Bl();
        var qz = scale.Bl<double>(ScaleTransform3D.ScaleZProperty).Bl();
        return new Point3DBl(qx, qy, qz);
      }
      set { Scale.Bind = value; }
    }
    /// <summary>
    ///     Translates an object in the three-dimensional x-y-z plane.
    /// </summary>
    public Point3DBl Translate {
      get {
        var translate = Transform<TranslateTransform3D>();
        var qx = translate.Bl<double>(TranslateTransform3D.OffsetXProperty).Bl();
        var qy = translate.Bl<double>(TranslateTransform3D.OffsetYProperty).Bl();
        var qz = translate.Bl<double>(TranslateTransform3D.OffsetZProperty).Bl();
        return new Point3DBl(qx, qy, qz);
      }
      set { Translate.Bind = value; }
    }
  }
  public static partial class WPF3DExtensions {
    public static QuaternionBl Bl(this Expr<Quaternion> v) { return v; }
    public static QuaternionRotation3DBl Bl(this Expr<QuaternionRotation3D> v) { return v; }

    public static QuaternionBl Bl(this Quaternion v) { return v; }
    public static QuaternionRotation3DBl Bl(this QuaternionRotation3D v) { return v; }
/*
    public static Array2D<DoubleBl> BlOld(this Expr<Matrix3D> Core) {
        DoubleBl[] Table = new DoubleBl[16];
        var arity = Matrix3DArity.ArityI;
        for (int i = 0; i < 16; i++)
          Table[i] = arity.Access(Core, i);

        return new Array2D<DoubleBl>(4, 4, (i, j) => Table.Table(j * 4 + i));
    }
 */
    public static MatrixBl<D4,D4> Bl(this Expr<Matrix3D> Core) {
      DoubleBl[] Table = new DoubleBl[16];
      var arity = Matrix3DArity.ArityI;
      for (int i = 0; i < 16; i++)
        Table[i] = arity.Access(Core, i);
      return new MatrixBl<D4,D4>(Table);
    }
    /*
    public static Expr<Matrix3D> AsMatrix3D(this IBaseArray<DoubleBl> value) {
      (value.Row.Count == value.Column.Count).Assert();
      (value.Row.Count == 4).Assert();
      Expr<double>[] Es = new Expr<double>[16];
      for (int i = 0; i < 4; i++)
        for (int j = 0; j < 4; j++)
          Es[j * 4 + i] = value[i, j];
      var arity = Matrix3DArity.ArityI;
      var matrix = arity.Make(Es);
      return matrix;
    }*/
    public static Expr<Matrix3D> AsMatrix3D(this MatrixBl<D4,D4> value) {
      Expr<double>[] Es = new Expr<double>[16];
      for (int i = 0; i < 4; i++)
        for (int j = 0; j < 4; j++)
          Es[j * 4 + i] = value[i, j];
      var arity = Matrix3DArity.ArityI;
      var matrix = arity.Make(Es);
      return matrix;
    }
  }
  /// <summary>
  /// Specifies the 3-D rotation to be used in a transformation.
  /// </summary>
  public abstract partial class Rotation3DBl<T, BRAND> : Brand<T, BRAND>, IExtends<BRAND,Rotation3DBl> where T : Rotation3D where BRAND : Rotation3DBl<T,BRAND> {
    public Rotation3DBl(Expr<T> v) : base(v) { }
    public static implicit operator Rotation3DBl(Rotation3DBl<T, BRAND> value) {
      return Rotation3DBl.UpCast(value);
    }
    static Rotation3DBl() { Rotation3DBl.CheckInit(); }
  }

  /// <summary>
  /// Specifies the 3-D rotation to be used in a transformation.
  /// </summary>
  public class Rotation3DBl : Rotation3DBl<Rotation3D, Rotation3DBl> {
    public Rotation3DBl(Expr<Rotation3D> Underlying) : base(Underlying) { }
    public static implicit operator Rotation3DBl(Expr<Rotation3D> target) { return new Rotation3DBl((target)); }
    public static implicit operator Rotation3DBl(Rotation3D target) { return (new Constant<Rotation3D>(target)); }
    static Rotation3DBl() { Register(v => v); }
    public static readonly Rotation3DBl Identity = Rotation3D.Identity;
    internal static int CheckInit() { Extensions.Trace("INIT: " + ToBrand); return 0; }
  }

  /// <summary>
  ///     Represents a 3-D rotation of a specified angle about a specified axis.
  /// </summary>
  public class AxisAngleRotation3DBl : Rotation3DBl<AxisAngleRotation3D, AxisAngleRotation3DBl> {
    public AxisAngleRotation3DBl(Expr<AxisAngleRotation3D> v) : base(v) { }

    public AxisAngleRotation3DBl() : this(new Constant<AxisAngleRotation3D>(new AxisAngleRotation3D())) { }
    public static implicit operator AxisAngleRotation3DBl(Expr<AxisAngleRotation3D> v) { return new AxisAngleRotation3DBl(v); }
    public static implicit operator AxisAngleRotation3DBl(AxisAngleRotation3D v) { return new Constant<AxisAngleRotation3D>(v); }

    public static implicit operator AxisAngleRotation3DBl(AxisAngle3DBl angle) {
      return new AxisAngleRotation3DBl() {
        Angle = angle.Angle.AsDegrees, Axis = angle.Axis,
      };
    }
    public static implicit operator AxisAngle3DBl(AxisAngleRotation3DBl angle) {
      return new AxisAngleRotation3DBl() {
        Angle = angle.Angle.AsDegrees,
        Axis = angle.Axis,
      };
    }


    static AxisAngleRotation3DBl() { Register(v => v); }
    /// <summary>
    ///     Gets or sets the angle of a 3-D rotation, in degrees. This is a dependency
    ///     property.
    /// </summary>
    public DegreeBl Angle { 
      get { return Underlying.Property<double>(AxisAngleRotation3D.AngleProperty).Bl().ToDegrees(); }
      set { Angle.Bind = value; } 
    }
    /// <summary>
    ///     Gets or sets the axis of a 3-D rotation. This is a dependency property.
    /// </summary>
    public Point3DBl Axis {
      get { return Underlying.Property<Vector3D>(AxisAngleRotation3D.AxisProperty); }
      set { Axis.Bind = value; }
    }
  }

  /// <summary>
  /// Represents a rotation transformation defined as a quaternion.
  /// </summary>
  public class QuaternionRotation3DBl : Rotation3DBl<QuaternionRotation3D, QuaternionRotation3DBl> {
    public QuaternionRotation3DBl(Expr<QuaternionRotation3D> v) : base(v) { }
    public QuaternionRotation3DBl() : this(new Constant<QuaternionRotation3D>(new QuaternionRotation3D())) { }
    public static implicit operator QuaternionRotation3DBl(Expr<QuaternionRotation3D> v) { return new QuaternionRotation3DBl(v); }
    public static implicit operator QuaternionRotation3DBl(QuaternionRotation3D v) { return new Constant<QuaternionRotation3D>(v); }
    static QuaternionRotation3DBl() { Register(v => v); }
    /// <summary>
    ///  Gets or sets the System.Windows.Media.Media3D.Quaternion that defines the
    ///  destination rotation. This is a dependency property.
    /// </summary>
    public QuaternionBl Quaternion {
      get { return Underlying.Property<Quaternion>(QuaternionRotation3D.QuaternionProperty); }
      set { Quaternion.Bind = value; }
    }
  }
}