using System;
using System.Collections.Generic;
using System.Windows.Media;
using Bling.DSL;
using Bling.Util;
using Bling.Linq;
using Bling.Core;
using Bling.Matrices;
using Bling.Graphics;
using Bling.Shaders;

namespace Bling.Graphics {
  /// <summary>
  ///     Represents an imaginary viewing position and direction in 3-D coordinate
  ///     space that describes how a 3-D model is projected onto a 2-D visual.
  /// </summary>
  public interface ICamera : ICanBeMatrixBl<D4, D4> {
    /// <summary>
    ///     Gets 3D transform matrix applied to the camera. 
    /// </summary>
    new MatrixBl<D4, D4> Matrix { get; }
    MatrixBl<D4, D4> View { get; }
    MatrixBl<D4, D4> Project { get; }
  }
  public abstract class CameraCl : ICamera {
    public CameraCl() {
    }
    public virtual MatrixBl<D4, D4> Matrix { get { return Project * View; } }
    public abstract MatrixBl<D4, D4> View { get; }
    public abstract MatrixBl<D4, D4> Project { get; }
  }
  public class ParallelCameraCl : CameraCl {
    public PointBl Dim { get; set; }
    public PointBl Pan { get; set; }
    public DoubleBl ZoomFactor { get; set; }
    public DoubleBl Near { get; set; }
    public DoubleBl Far { get; set; }
    public ParallelCameraCl() {
      Dim = 100d;
      ZoomFactor = 1d;
      Pan = 0;
      Near = -10000;
      Far = +10000;

    }
    public MatrixBl<D4, D4> MakeViewToRenderPlaneTransform {
      get {
        var viewToViewport = MatrixBl<D4, D4>.Identity;
        var viewportCenterTransform = MatrixBl<D4, D4>.Identity;

        viewToViewport *= (Pan * ZoomFactor).InsertZ(0).Translate();
        viewToViewport *= ZoomFactor.XXX().Scale();

        viewportCenterTransform *= (0.5 * Dim).InsertZ(0).Translate();

        return viewportCenterTransform * viewToViewport;
      }
    }


    public override MatrixBl<D4, D4> Matrix {
      get {
        var viewProjection = MakeViewToRenderPlaneTransform.ToArray();
        viewProjection[2 * 4 + 2] = 2d / (Far - Near);
        viewProjection[2 * 4 + 3] = (Near + Far) / (Far - Near);
        return new MatrixBl<D4, D4>(viewProjection);
      }
    }


    public override MatrixBl<D4, D4> View {
      get { return MatrixBl<D4, D4>.Identity; }
    }
    public override MatrixBl<D4, D4> Project {
      get { return MatrixBl<D4, D4>.Identity; }
    }

  }


  public interface IPlaneDistanceSet {
    /// <summary>
    ///     Gets or sets a value that specifies the distance from the camera of the camera's
    ///     far clip plane. Default is 100.
    /// </summary>
    DoubleBl Far { get; set; }
    /// <summary>
    ///     Gets or sets a value that specifies the distance from the camera of the camera's
    ///     near clip plane. Default is .1.
    /// </summary>
    DoubleBl Near { get; set; }
    IPlaneDistanceSet Bind { set; }
  }
  public interface IDirectionSet {
    /// <summary>
    ///     Gets or sets a System.Windows.Media.Media3D.Vector3D which defines the direction
    ///     in which the camera is looking in world coordinates. Default is (0,0,0).
    /// </summary>
    Point3DBl Look { get; set; }
    /// <summary>
    ///     Gets or sets a System.Windows.Media.Media3D.Vector3D which defines the upward
    ///     direction of the camera. Default is (0,1,0).
    /// </summary>
    Point3DBl Up { get; set; }
    IDirectionSet Bind { set; }
  }

  public interface IProjectionCamera : ICamera {
    /// <summary>
    ///     Gets or sets values that specify the distance from the camera of the camera's
    ///     far and near clip planes. 
    /// </summary>
    IPlaneDistanceSet PlaneDistance { get; set; }
    /// <summary>
    ///     Get or set points that define the look and upward
    ///     directions of the camera.
    /// </summary>
    IDirectionSet Direction { get; set; }
    /// <summary>
    ///     Gets or sets the position of the camera in world coordinates. 
    /// </summary>
    Point3DBl Position { get; set; }
  }
  /// <summary>
  /// Lightweight projection camera that doesn't support databinding. 
  /// </summary>
  /// <see>Bing.WPF3D.ProjectionCameraBl</see>
  public abstract class ProjectionCameraCl : CameraCl, IProjectionCamera {
    private class PlaneDistanceSet : IPlaneDistanceSet {
      public DoubleBl Far { get; set; }
      public DoubleBl Near { get; set; }
      public PlaneDistanceSet() { Far = 100d; Near = 0.1; }
      public IPlaneDistanceSet Bind {
        set { Far = value.Far; Near = value.Near; }
      }
    }
    private class DirectionSet : IDirectionSet {
      public Point3DBl Look { get; set; }
      public Point3DBl Up { get; set; }
      public DirectionSet() { Look = 0; Up = new Point3DBl(0,1,0); }
      public IDirectionSet Bind {
        set { Look = value.Look; Up = value.Up; }
      }
    }
    public ProjectionCameraCl() {
      this.PlaneDistance = new PlaneDistanceSet();
      this.Direction = new DirectionSet();
      this.Position = 0d;
    }


    /// <summary>
    ///     Gets or sets values that specify the distance from the camera of the camera's
    ///     far and near clip planes. 
    /// </summary>
    public IPlaneDistanceSet PlaneDistance { get { return PlaneDistance0; } set { PlaneDistance.Bind = value; } }
    private readonly IPlaneDistanceSet PlaneDistance0 = new PlaneDistanceSet();

    /// <summary>
    ///     Get or set points that define the look and upward
    ///     directions of the camera.
    /// </summary>
    public IDirectionSet Direction { get { return Direction0; } set { Direction.Bind = value; } }
    private readonly IDirectionSet Direction0 = new DirectionSet();

    /// <summary>
    ///     Gets or sets the position of the camera in world coordinates. 
    /// </summary>
    public Point3DBl Position { get; set; }

    public override MatrixBl<D4, D4> View {
      get { return Position.LookAt(Direction.Look, Direction.Up); }
    }
  }
  /// <summary>
  ///     Represents a perspective projection camera.
  /// </summary>
  public interface IPerspectiveCamera : IProjectionCamera {
    /// <summary>
    ///     The camera's horizontal field of view, in degrees. The default value is 45.
    /// </summary>
    DegreeBl FieldOfView { get; set; }
  }
  /// <summary>
  /// Lightweight version of a perspective camera, unlike PerspectiveCameraBl does not support databinding. 
  /// </summary>
  /// <see>Bling.WPF3D.PerspectiveCameraBl</see>
  public class PerspectiveCameraCl : ProjectionCameraCl, IPerspectiveCamera {
    public PerspectiveCameraCl() {
      FieldOfView = .1d.PI().AsDegrees;
    }
    /// <summary>
    ///     The camera's horizontal field of view, in degrees. The default value is 45.
    /// </summary>
    public DegreeBl FieldOfView { get; set; }
    public override MatrixBl<D4, D4> Project {
      get { return FieldOfView.AsRadians.PerspectiveFovLH(1d, PlaneDistance.Near, PlaneDistance.Far); }
    }
  }

  public interface IOrthographicCamera : IProjectionCamera {
    /// <summary>
    ///     Width of the camera's viewing box, in world units.
    /// </summary>
    DoubleBl Width { get; set; }
  }
  public class OrthographicCameraCl : ProjectionCameraCl, IOrthographicCamera {
    /// <summary>
    ///     Width of the camera's viewing box, in world units.
    /// </summary>
    public DoubleBl Width {
      get { return Size.X; }
      set {
        var p = new PointBl(value, Size.Y);
        Size = p;
      }
    }
    public PointBl Size { get; set; }
    public OrthographicCameraCl() {
      Size = 1d;
    }
    public override MatrixBl<D4, D4> Project {
      get {
        return Size.OrthographicLH(PlaneDistance.Near, PlaneDistance.Far);
      }
    }
  }

  
  /// <summary>
  ///     object that represents lighting applied
  ///     to a 3-D scene.
  /// </summary>
  public interface ILight {
    /// <summary>
    ///     Gets or sets the color of the light. Default is white.
    /// </summary>
    ColorBl Color { get; set; }

    /// <summary>
    /// Direction to use in lighting calculation.
    /// </summary>
    /// <param name="Position">Position of pixel being lit.</param>
    Point3DBl DirectionToPosition(Point3DBl Position);
    DoubleBl UseAttenuation(Point3DBl Direction, DoubleBl Distance);
    DoubleBl UseAttenuation(Point3DBl Direction);
  }
  public abstract class LightCl : ILight {
    public LightCl() {
      this.Color = Colors.White;
    }

    public static LightGroup operator +(LightCl OpA, ILight OpB) {
      return new LightGroup(OpA, OpB);
    }

    /// <summary>
    ///     Gets or sets the color of the light. Default is white.
    /// </summary>
    public ColorBl Color { get; set; }
    public abstract Point3DBl DirectionToPosition(Point3DBl Position);
    public abstract DoubleBl UseAttenuation(Point3DBl Direction, DoubleBl Distance);
    public abstract DoubleBl UseAttenuation(Point3DBl Direction);
    public ILight CopyLight {
      set {
        Color = value.Color;
      }
    }
  }

  /// <summary>
  ///     Light object that projects its effect along a direction specified by a System.Windows.Media.Media3D.Vector3D.
  /// </summary>
  public interface IDirectionalLight : ILight {
    /// <summary>
    ///     Represents the vector along which the light's effect will be seen on models
    ///     in a 3-D scene.  3D Vector along which the light projects, and which must have a non-zero magnitude.
    ///     The default value is (0,0,-1).
    /// </summary>
    Point3DBl Direction { get; set; }
  }
  public class DirectionalLightCl : LightCl, IDirectionalLight {
    public DirectionalLightCl() {
      Direction = new Point3DBl(0, 0, -1);
    }
    /// <summary>
    ///     Represents the vector along which the light's effect will be seen on models
    ///     in a 3-D scene.  3D Vector along which the light projects, and which must have a non-zero magnitude.
    ///     The default value is (0,0,-1).
    /// </summary>
    public Point3DBl Direction { get; set; }
    public override Point3DBl DirectionToPosition(Point3DBl Position) {
      return -Direction;
    }
    public override DoubleBl UseAttenuation(Point3DBl Direction, DoubleBl Distance) {
      return 1d; // no attenuation.
    }
    public override DoubleBl UseAttenuation(Point3DBl Direction) {
      return 1d;
    }
    public new IDirectionalLight CopyLight {
      set {
        ((LightCl)this).CopyLight = value;
        Direction = value.Direction;
      }
    }
  }

  /// <summary>
  ///    Light object that applies light to objects uniformly, regardless of their
  ///    shape.
  /// </summary>
  public interface IAmbientLight : ILight { }
  public class AmbientLightCl : LightCl, IAmbientLight {
    public override DoubleBl UseAttenuation(Point3DBl Direction, DoubleBl Distance) {
      throw new NotImplementedException();
    }
    public override Point3DBl DirectionToPosition(Point3DBl Position) {
      throw new NotImplementedException();
    }
    public override DoubleBl UseAttenuation(Point3DBl Direction) {
      throw new NotImplementedException();
    }
    public new IAmbientLight CopyLight {
      set {
        ((LightCl)this).CopyLight = value;
      }
    }
  }

  public interface IAttenuationSet {
    /// <summary>
    ///     Gets or sets a constant value by which the intensity of the light diminishes
    ///     over distance. Default is 1.
    /// </summary>
    DoubleBl Constant { get; set; }

    /// <summary>
    ///     Gets or sets a value that specifies the linear diminution of the light's
    ///     intensity over distance. Default is 0.
    /// </summary>
    DoubleBl Linear { get; set; }
    /// <summary>
    ///     Gets or sets a value that specifies the diminution of the light's effect
    ///     over distance, calculated by a quadratic operation. Default is 0.
    /// </summary>
    DoubleBl Quadratic { get; set; }

    IAttenuationSet Bind { set; }
  }
  public interface IPointLight : ILight {
    /// <summary>
    /// Get or set values that specify the diminution of the light's effect
    /// over distance, calculated by constant, linear, and quadratic operations.
    /// likes:Attenuation = 1/constant + linear * distance + quadratic * distance * distance
    /// </summary>
    IAttenuationSet Attenuation { get; set; }
    /// <summary>
    ///     Gets or sets a System.Windows.Media.Media3D.Point3D that specifies the light's
    ///     position in world space. Default is (0,0,0).
    /// </summary>
    Point3DBl Position { get; set; }
    /// <summary>
    ///     Gets or sets the distance beyond which the light has no effect. Default is Infinity.
    /// </summary>
    DoubleBl Range { get; set; }
  }
  public class PointLightCl : LightCl, IPointLight {
    public PointLightCl() {
      this.Position = 0d;
      this.Range = double.PositiveInfinity;
    }
    private class AttenuationSet : IAttenuationSet {
      public AttenuationSet() {
        Constant = 1; Linear = 0; Quadratic = 0;
      }
      public DoubleBl Constant { get; set; }
      public DoubleBl Linear { get; set; }
      public DoubleBl Quadratic { get; set; }
      public IAttenuationSet Bind {
        set {
          this.Constant = value.Constant;
          this.Linear = value.Linear;
          this.Quadratic = value.Quadratic;
        }
      }
    }
    private readonly AttenuationSet AS = new AttenuationSet();
    /// <summary>
    /// Get or set values that specify the diminution of the light's effect
    /// over distance, calculated by constant, linear, and quadratic operations.
    /// likes:Attenuation = 1/constant + linear * distance + quadratic * distance * distance
    /// </summary>
    public IAttenuationSet Attenuation { get { return AS; } set { AS.Bind = value; } }
    /// <summary>
    ///     Gets or sets a System.Windows.Media.Media3D.Point3D that specifies the light's
    ///     position in world space. Default is (0,0,0).
    /// </summary>
    public Point3DBl Position { get; set; }
    /// <summary>
    ///     Gets or sets the distance beyond which the light has no effect. Default is Infinity.
    /// </summary>
    public DoubleBl Range { get; set; }
    public override Point3DBl DirectionToPosition(Point3DBl Position) {
      return this.Position - Position;
    }
    public override DoubleBl UseAttenuation(Point3DBl Direction, DoubleBl Distance) {
      return this.Attenuation(Distance);
    }
    public override DoubleBl UseAttenuation(Point3DBl Direction) {
      return 1d;
    }
    public new IPointLight CopyLight {
      set {
        ((LightCl)this).CopyLight = value;
        this.Position = value.Position;
        this.Range = value.Range;
        this.Attenuation = value.Attenuation;
      }
    }
  }

  /// <summary>
  ///     Light object that projects its effect in a cone-shaped area along a specified
  ///     direction.
  /// </summary>
  public interface ISpotLight : IPointLight {
    /// <summary>
    ///     Gets or sets a System.Windows.Media.Media3D.Vector3D that specifies the direction
    ///     in which the System.Windows.Media.Media3D.SpotLight projects its light. This
    ///     is a dependency property. The default value is (0,0,-1).
    /// </summary>
    Point3DBl Direction { get; set; }
    /// <summary>
    ///     Gets or sets an angle that specifies the proportion of a System.Windows.Media.Media3D.SpotLight's
    ///     cone-shaped projection in which the light fully illuminates objects in the
    ///     scene. This is a dependency property. The default value is 180.
    /// </summary>
    DegreeBl InnerConeAngle { get; set; }
    /// <summary>
    ///     Gets or sets an angle that specifies the proportion of a System.Windows.Media.Media3D.SpotLight's
    ///     cone-shaped projection outside which the light does not illuminate objects
    ///     in the scene. This is a dependency property. The default value is 90.
    /// </summary>
    DegreeBl OuterConeAngle { get; set; }
  }
  public class SpotLightCl : PointLightCl, ISpotLight {
    public SpotLightCl() {
      Direction = new Point3DBl(0, 0, -1);
      InnerConeAngle = 180.ToDegrees();
      OuterConeAngle = 90.ToDegrees();
    }
    /// <summary>
    ///     Gets or sets a System.Windows.Media.Media3D.Vector3D that specifies the direction
    ///     in which the System.Windows.Media.Media3D.SpotLight projects its light. The default value is (0,0,-1).
    /// </summary>
    public Point3DBl Direction { get; set; }
    /// <summary>
    ///     Gets or sets an angle that specifies the proportion of a System.Windows.Media.Media3D.SpotLight's
    ///     cone-shaped projection in which the light fully illuminates objects in the
    ///     scene. The default value is 180.
    /// </summary>
    public DegreeBl InnerConeAngle { get; set; }
    /// <summary>
    ///     Gets or sets an angle that specifies the proportion of a System.Windows.Media.Media3D.SpotLight's
    ///     cone-shaped projection outside which the light does not illuminate objects
    ///     in the scene. The default value is 90.
    /// </summary>
    public DegreeBl OuterConeAngle { get; set; }

    public override DoubleBl UseAttenuation(Point3DBl DirectionToPosition, DoubleBl Distance) {
      var dot1 = -this.Direction.Dot(DirectionToPosition);
      var innerCos = this.InnerConeAngle.Cos;
      var outerCos = this.OuterConeAngle.Cos;
      var att = this.Attenuation(Distance) * dot1;
      var spot = (dot1 > innerCos).Condition(att, (dot1 > outerCos).Condition(att * (dot1 - outerCos) / (innerCos - outerCos), 0.0));
      return spot;
    }
    public override DoubleBl UseAttenuation(Point3DBl Direction) {
      var dot1 = -this.Direction.Dot(Direction);
      var innerCos = this.InnerConeAngle.Cos;
      var outerCos = this.OuterConeAngle.Cos;
      var spot = (dot1 > innerCos).Condition(dot1, (dot1 > outerCos).Condition(dot1 * (dot1 - outerCos) / (innerCos - outerCos), 0.0));
      return spot;
    }
    public new ISpotLight CopyLight {
      set {
        ((PointLightCl)this).CopyLight = value;
        this.InnerConeAngle = value.InnerConeAngle;
        this.OuterConeAngle = value.OuterConeAngle;
        this.Direction = value.Direction;
      }
    }
  }
  public interface IMaterial {
    Lighting ApplyLights(LightGroup Light);
  }
  public interface HasKnob {
    RGBBl Knob { get; set; }
    DoubleBl Factor { set; }
    void Bind(HasKnob Other);
  }

  /// <summary>
  /// the material for object, usually combine with the lighting effect.
  /// </summary>
  public abstract class MaterialCl : IMaterial {
    public MaterialCl() {}

    /// <summary>
    /// Apply one light to this material.
    /// </summary>
    public Lighting ApplyLight(ILight Light) { return ApplyLights(new LightGroup(Light)); }
    /// <summary>
    /// Apply a group of lights to this material.
    /// </summary>
    public abstract Lighting ApplyLights(LightGroup Light);
    protected virtual MaterialGroup Add(MaterialCl Other) {
      return new MaterialGroup(this, Other);
    }
    /// <summary>
    /// Compose materials together so that specified results are applied to all materials.
    /// </summary>
    public static MaterialGroup operator +(MaterialCl opA, MaterialCl opB) {
      return opA.Add(opB);
    }
  }

  /// <summary>
  /// Group of multiple materials composed together. Each material is applied to specified lights. 
  /// </summary>
  public class MaterialGroup : MaterialCl {
    private readonly IMaterial[] Materials;
    public MaterialGroup(params IMaterial[] Materials) {
      this.Materials = Materials;
    }
    public override Lighting ApplyLights(LightGroup Light) {
        Lighting ret = new Lighting((p, n) => (input => Colors.Black)); // nothing.
        foreach (var mat in Materials) ret += mat.ApplyLights(Light); // additive.
        return ret;
    }
    protected override MaterialGroup Add(MaterialCl Other) {
      var array = new IMaterial[Materials.Length + 1];
      for (int i = 0; i < Materials.Length; i++) array[i] = Materials[i];
      array[Materials.Length] = Other;
      return new MaterialGroup(array);
    }
  }
  /// <summary>
  /// Group of lights composed together. Each light is applied from a material. 
  /// </summary>
  public class LightGroup {
    public readonly ILight[] Lights;
    public LightGroup(params ILight[] Lights) {
      this.Lights = Lights;
    }
    public static LightGroup operator +(LightGroup Group, ILight light) {
      var result = new ILight[Group.Lights.Length + 1];
      for (int i = 0; i < Group.Lights.Length; i++) result[i] = Group.Lights[i];
      result[Group.Lights.Length] = light;
      return new LightGroup(result);
    }
  }

  /// <summary>
  ///  Diffuselighting effect of material only, it depends on the colors of both of the light source and the material
  /// </summary>
  public class DiffuseMaterialCl : MaterialCl, HasKnob {
    public DiffuseMaterialCl() {
      Knob = Colors.White.Bl().ScRGB;
      Ambient.Knob = Colors.White.Bl().ScRGB;
    }
    /// <summary>
    /// Filters lighting effect. Default is white (reflect all). 
    /// </summary>
    /// <see>http://blogs.msdn.com/wpf3d/archive/2006/12/08/material-color-knobs.aspx.</see>
    public RGBBl Knob { get; set; }
    public DoubleBl Factor { set { Knob = (Colors.White.Bl().ScRGB) * value; } }
    public void Bind(HasKnob knob) {
      this.Knob = knob.Knob;
    }

    public class AmbientSet : HasKnob {
      /// <summary>
      /// Filters amount of ambient light that is reflected. Default is White (reflects all light).
      /// </summary>
      /// <see>http://blogs.msdn.com/wpf3d/archive/2006/12/08/material-color-knobs.aspx.</see>
      public RGBBl Knob { get; set; }
      /// <summary>
      /// Reflectance coefficient of ambient light of the material, between 0 ~ 1. Default is 1. This will actually set Knob
      /// to White * value.
      /// </summary>
      public DoubleBl Factor { set { Knob = (Colors.White.Bl().ScRGB) * value; } }
      public void Bind(HasKnob knob) { Knob = knob.Knob; }
    }
    /// <summary>
    ///Ambient lighting provides constant lighting for a scene. It lights all objects the same because it is not dependent on any other lighting factors 
    ///such as vertex normals,light direction, light position, range, or attenuation. It is the fastest type of lighting but it produces the least realistic
    ///results. You can use a single global ambient light. Alternatively, you can set any light object to provide thier ambient lighting.
    /// </summary>
    public AmbientSet Ambient { get { return Ambient0; } set { Ambient0.Bind(value); } }
    private readonly AmbientSet Ambient0 = new AmbientSet();
    public override Lighting ApplyLights(LightGroup Lights) {
      return new Lighting((Position, Normal, Eye) => {
        RGBBl LightingResult = Colors.Black.Bl().ScRGB;
        foreach (var LightSource in Lights.Lights) {
          if (LightSource is IAmbientLight) {
            LightingResult += Ambient.Knob * ((IAmbientLight)LightSource).Color.ScRGB;
          } else {
            Point3DBl Dir = LightSource.DirectionToPosition(Position);
            DoubleBl Dis = Dir.Length;
            Dir = Dir.Normalize;
            LightingResult = LightingResult + Knob * ((LightSource.Color.ScRGB * Dir.Dot(Normal).Max(0) * 
              LightSource.UseAttenuation(Dir, Dis)));
          }
        }
        return (Input) => LightingResult * Input.ScRGB;
      });
    }
  }

  /// <summary>
  /// Specular lighting effect of material, or called highlight sometimes, it only depends on the light source
  /// </summary>
  public class SpecularMaterialCl : MaterialCl, HasKnob {
    public SpecularMaterialCl() {
      Power = 1d;
      Knob = Colors.White.Bl().ScRGB;
    }
    public bool Is2D = false;
    /// <summary>
    /// Filters lighting effect. Default is white (no effect). 
    /// </summary>
    /// <see>http://blogs.msdn.com/wpf3d/archive/2006/12/08/material-color-knobs.aspx.</see>
    public RGBBl Knob { get; set; }

    public void Bind(HasKnob knob) { Knob = knob.Knob; }

    /// <summary>
    /// the reflective index,usually from 1 to 2000, 
    /// the higher this number is, the surface looks smoother, 
    /// that means the highlight spot smaller and brighter
    /// </summary>
    public DoubleBl Power { get; set; }
    /// <summary>
    /// specularity factor, usually between 0 and 1, can control the whole strength of the specular color. Sets knob to White * value.
    /// </summary>
    public DoubleBl Factor { set { Knob = (Colors.White.Bl().ScRGB) * value; } }

    public override Lighting ApplyLights(LightGroup Lights) {
      return new Lighting((Position, Normal, Eye) => {
        RGBBl LightingResult = Colors.Black.Bl().ScRGB;
        foreach (var LightSource in Lights.Lights) {
          if (LightSource is IAmbientLight) continue;
          var Dir = LightSource.DirectionToPosition(Position).Normalize;
          var dot = Dir.Dot(Normal);
          var r = -Dir + 2 * dot * Normal;
          var viewDir = (Is2D ? Eye - Position : Position - Eye).Normalize;
          var spot = LightSource.UseAttenuation(Dir); // XXX: is one OK here?
          LightingResult += (dot > 0).Condition(LightSource.Color.ScRGB * r.Dot(viewDir).Max(0).Pow(Power) * spot, Colors.Black.Bl().ScRGB);
        }
        LightingResult = Knob * LightingResult;
        return (Input) => LightingResult;
      });
    }
  }

  /// <summary>
  /// The material effect to simulate cover a piece of glass on the object, with glass's simple refraction and the specular effect of its smooth surface.
  /// Note that we calculate the refraction only one time, so this is generally used in 2D.
  /// </summary>
  public class GlassMaterialCl : SpecularMaterialCl {
    public GlassMaterialCl()
      : base() {
      GlassColor = Colors.White;
      RefractiveIndex = 1.5;
      Thickness = 3;
      Transparency = 0.5;
    }
    /// <summary>
    /// the color of the glass
    /// </summary>
    public ColorBl GlassColor { get; set; }


    /// <summary>
    /// Here means the relative refractive index (RI).    
    /// the relative RI = RI of refracting medium /  RI of incident medium.     
    /// Default is the number 1.5, which equals to: RI of glass / RI of air.
    /// </summary>
    public DoubleBl RefractiveIndex { get; set; }
    /// <summary>
    /// The thickness of glass, usually between 0 and 10, the thicker the glass is, the effect of refraction is more obvious.
    /// </summary>
    public DoubleBl Thickness { get; set; }
    /// <summary>
    /// The transparency of the glass, usually between 0 and 1.
    /// </summary>
    public DoubleBl Transparency { get; set; }

    public override Lighting ApplyLights(LightGroup Lights) {
      var specularLight = base.ApplyLights(Lights);
      return new Lighting((Position, Normal, Eye) => {
        //if (LightSource is IAmbientLight) return (Input, UV) => Colors.Black;
        var specularColor = specularLight.ApplyAt(Position, Normal);
        var viewDir = (Position - Eye).Normalize;
        var cos1 = viewDir.Dot(Normal);
        var cos2 = (1 - (1 - cos1.Square) * RefractiveIndex.Square).Sqrt;
        var bias = (-RefractiveIndex * viewDir - (cos2 - RefractiveIndex * cos1) * Normal) * (Thickness / 100);
        return (Input, UV) => // we don't add our own skin, since its in specular already.
          Input[(UV + new PointBl(bias.X, bias.Y))] * Transparency +
            GlassColor * (1.0 - Transparency) + specularColor.ForPixel(Input, UV);
      });
    }
  }
  /// <summary>
  /// The diffraction effect of material.Most surface reflection models in computer graphics ignore the wavelike effects of natural light.
  /// This is fine whenever the surface detail is much larger than the wavelength of light (roughly a micron). For surfaces with small-scale
  /// detail such as a compact disc, however, wave effects cannot be neglected. The small-scale surface detail causes the reflected waves to
  /// interfere with one another. This phenomenon is known as diffraction.Diffraction causes the reflected light from these surfaces to exhibit
  /// many colorful patterns, as you can see in the subtle reflections from a compact disc. Other surfaces that exhibit diffraction are now 
  /// common and are mass-produced to create funky wrapping paper, colorful toys, and fancy watermarks, for example.
  /// </summary>
  /// <see>This method is from GPU Gems:Chapter 8. Simulating Diffraction, by Jos Stam, Alias Systems you can see more on:
  /// http://http.developer.nvidia.com/GPUGems/gpugems_ch08.html</see>
  public class DiffractionMaterialCl : MaterialCl {
    public DiffractionMaterialCl() {
      TangentF = (UV, Position, Normal) => {
        return Normal.Cross(new Point3DBl(1, 0, 0));
      };
      //Eye = new Point3DBl(0, 0, -1);
      Distance = 1;
      SHparameter = 10;
    }
    /// <summary>
    /// The function to get tangent vectors that supply the local direction of the narrow bands on the surface
    /// </summary>
    public Func<PointBl, Point3DBl, Point3DBl, Point3DBl> TangentF { get; set; }
    /// <summary>
    /// The position of eye/view point
    /// </summary>
    //public Point3DBl Eye { get; set; }

    /// <summary>
    /// Our simple diffraction shader models the reflection of light from a surface commonly known as a diffraction grating. 
    /// A diffraction grating is composed of a set of parallel, narrow reflecting bands separated by this distance.
    /// usually greater than 1.
    /// </summary>
    public DoubleBl Distance { get; set; }
    /// <summary>
    /// A roughness parameterThe to model spread of the highlight caused by the irregularities in the bumps on each of the bands of the diffraction grating
    /// </summary>
    public DoubleBl SHparameter { get; set; }

    public override Lighting ApplyLights(LightGroup Lights) {
      return new Lighting((Position, Normal, Eye) => {
        var V = (Eye - Position).Normalize;

        return (Input, UV) => {
          ColorBl Result = Colors.Black;
          foreach (var Light in Lights.Lights) {
            if (Light is IAmbientLight) continue;
            var L = Light.DirectionToPosition(Position).Normalize;
            var H = L + V;

            var u = TangentF(UV, Position, Normal).Dot(H) * Distance;
            var e = SHparameter * u / Normal.Dot(H);
            var c = Math.E.Bl().Pow(-e.Square);
            u = (u < 0).Condition(-u, u);
            var cdiff = new RGBBl(0, 0, 0);
            for (int ni = 1; ni < 8; ni++) {
              cdiff += ShaderLib.Rainbow[new PointBl(2 * u / ni - 1, 0.1)].ScRGB;
            }
            Result += (new RGBBl(c) + cdiff);
          }
          return Result.Additive(Input[UV]);
        };
      });
    }
  }
  public class ViewCamera : ICanBeMatrixBl<D4,D4> {
    public ViewCamera() {
      Up = new Point3DBl(0, 1, 0);
      At = 0d;
      Eye = new Point3DBl(1, 0, 0);
    }
    public Point3DBl Eye { get; set; }
    public Point3DBl At { get; set; }
    public Point3DBl Up { get; set; }
    public MatrixBl<D4, D4> Matrix {
      get { return Eye.LookAt(At, Up); }
    }
  }


  public static partial class Core3DExtensions {
    /// <summary>
    /// calculate the attenuation
    /// </summary>
    /// <param name="light">information of the Ambientlight source</param>
    /// <param name="Displacement">the Displacement from a point to the position of the point/spot light source</param>
    /// <returns></returns>
    public static DoubleBl Attenuation(this IPointLight light, DoubleBl Displacement) {
      var attenuationCoefficient = light.Attenuation;
      return 1d / (attenuationCoefficient.Quadratic * Displacement * Displacement +
                   attenuationCoefficient.Linear * Displacement + attenuationCoefficient.Constant);
    }

    /// <summary>
    /// creates a view matrix derived from an eye point, a reference point indicating the center of the	scene, and an UP vector,
    /// usually uses this to set the way(like position, direction) how the observer or camera browses the scene.
    /// </summary>
    /// <param name="Eye">the	position of the	eye point</param>
    /// <param name="At">the position of the	reference point to look at, usually indicating the center of the	scene</param>
    /// <param name="Up">the direction indicating which way is up.</param>
    /// <returns>the 4x4 view matrix</returns>
    public static MatrixBl<D4, D4> LookAt(this Point3DBl Eye, Point3DBl At, Point3DBl Up) {
      var zaxis = (At - Eye).Normalize;
      var xaxis = (Up.Cross(zaxis)).Normalize;
      var yaxis = (zaxis.Cross(xaxis)).Normalize;

      var c0 = new Point4DBl(xaxis, -xaxis.Dot(Eye));
      var c1 = new Point4DBl(yaxis, -yaxis.Dot(Eye));
      var c2 = new Point4DBl(zaxis, -zaxis.Dot(Eye));
      var c3 = new Point4DBl(0, 0, 0, 1);
      var m = new MatrixBl<D4, D4>(
        (MatrixBl<D1,D4>) c0,
        (MatrixBl<D1,D4>) c1,
        (MatrixBl<D1,D4>) c2,
        (MatrixBl<D1,D4>) c3
      );
      return m.Transpose;


/*
      zaxis = normal(At - Eye)
xaxis = normal(cross(Up, zaxis))
yaxis = cross(zaxis, xaxis)
    
 xaxis.x           yaxis.x           zaxis.x          0
 xaxis.y           yaxis.y           zaxis.y          0
 xaxis.z           yaxis.z           zaxis.z          0
-dot(xaxis, eye)  -dot(yaxis, eye)  -dot(zaxis, eye)  l
      */
      /*
      Point3DBl viewDir = (At - Eye).Normalize;
      Point3DBl viewUp = (Up - Up.Dot(viewDir) * viewDir).Normalize;
      Point3DBl viewSide = viewDir.Cross(viewUp);

      MatrixBl<D3, D3> rotate = new MatrixBl<D3, D3>(
        (MatrixBl<D3, D1>) viewSide,
        (MatrixBl<D3, D1>)viewUp,
        (MatrixBl<D3, D1>) (+1d * viewDir));

      Point3DBl eyeInv = -1d * ((Point3DBl)(rotate * Eye));

      var rotate0 = rotate.SpliceColumn<D3, D4, D3>(3, (MatrixBl<D1, D3>) eyeInv);
      var rotate1 = rotate0.SpliceRow<D4, D3, D4>(3, (MatrixBl<D4, D1>) new Point4DBl(0, 0, 0, 1));

      return rotate1;
       */
      /*
      Point3DBl Right, Vec;
      Vec = (At - Eye).Normalize;
      Right = Up.Cross(Vec);
      Up = Vec.Cross(Right).Normalize;
      Right = Right.Normalize;

      MatrixBl<D4, D1> c0 = (new Point4DBl(Right, -Right.Dot(Eye)));
      MatrixBl<D4, D1> c1 = (new Point4DBl(Up, -Up.Dot(Eye)));
      MatrixBl<D4, D1> c2 = (new Point4DBl(Vec, -Vec.Dot(Eye)));
      MatrixBl<D4, D1> c3 = (new Point4DBl((Point3DBl)0d, 1.0));
      return new MatrixBl<D4, D4>(c0, c1, c2, c3);
       */
    }
    /// <summary>
    /// Builds a left-handed perspective projection matrix based on a field of view, to set up a perspective transform
    /// which transforms geometry from 3-D view space to 2-D viewport space, with
    /// a perspective divide making objects smaller in the distance.
    /// </summary>
    /// <param name="Fovy">Field of view angle in the y direction(0~PI/2, usually PI/4 )</param>
    /// <param name="Aspect">Aspect ratio that determines the field of view in the x direction, defined as the view space width divided by height</param>
    /// <param name="Zn">Z-value of the near view clipping plane</param>
    /// <param name="Zf">Z-value of the far view clipping plane,defines the distances at which geometry should no longer be rendered with Zn, should be positive</param>
    /// <returns>the 4x4 projection matrix</returns>
    public static MatrixBl<D4, D4> PerspectiveFovLH(this Angle2DBl fov, DoubleBl aspect, DoubleBl near, DoubleBl far) {

      var d = 1.0d / (fov / 2d).Tan;
      var a = aspect;
      var zf = far;
      var zn = near;

      return new MatrixBl<D4, D4>(
        d/a, 0,          0,                0,
          0, d,          0,                0,
          0, 0, zf/(zf-zn), -(zn*zf)/(zf-zn),
          0, 0,          1,                0
      );
    }
    public static MatrixBl<D4, D4> OrthographicLH(this PointBl Sz, DoubleBl near, DoubleBl far) {
      DoubleBl w = Sz.X, h = Sz.Y, zn = near, zf = far;
      return new MatrixBl<D4, D4>(
        2d/w,   0,          0,          0,
          0, 2d/h,          0,          0,
          0,    0, 1 /(zf-zn), zn/(zf-zn),
          0,    0,          0,          1
      );


    }

    /// <summary>
    /// Returns the world*perspective matrix for the camera
    /// </summary>
    /// <param name="camera">Perspective projection camera</param>
    /// <param name="aspectRatio">The aspect ratio of the device surface, defined as the view space width divided by height</param>
    /// <returns></returns>
    public static MatrixBl<D4, D4> ToMatrix3DLH(this IPerspectiveCamera camera, DoubleBl aspectRatio) {
      MatrixBl<D4, D4> result = camera.LookAtMatrixLH();
      result = result * camera.Matrix;
      result = result * camera.PerspectiveMatrixLH(aspectRatio);
      return result;
    }
    /// <summary>
    /// return a 4x4 projection matrix derived from a Perspective projection camera and the aspect ratio
    /// </summary>
    /// <param name="camera">Perspective projection camera</param>
    /// <param name="aspectRatio">The aspect ratio of the device surface, defined as the view space width divided by height</param>
    /// <returns>the 4x4 projection matrix</returns>
    public static MatrixBl<D4, D4> PerspectiveMatrixLH(this IPerspectiveCamera camera, DoubleBl aspectRatio) {
      Angle2DBl fov = camera.FieldOfView.AsRadians;

      DoubleBl zn = camera.PlaneDistance.Near;
      DoubleBl zf = camera.PlaneDistance.Far;
      DoubleBl f = 1.0 / (fov / 2.0).Tan;
      DoubleBl xScale = f / aspectRatio;
      DoubleBl yScale = f;
      DoubleBl n = (1.0 / (zf - zn));
      DoubleBl m33 = zf * n;
      DoubleBl m43 = -zf * zn * n;

      return new MatrixBl<D4, D4>(
              xScale, 0, 0, 0,
              0, yScale, 0, 0,
              0, 0, m33, 1,
              0, 0, m43, 0);
    }
    /// <summary>
    /// return a 4x4 view matrix derived from a Perspective projection camera
    /// </summary>
    /// <param name="camera">Perspective projection camera</param>
    /// <returns>the 4x4 view matrix</returns>
    public static MatrixBl<D4, D4> LookAtMatrixLH(this IPerspectiveCamera camera) {
      var f = (camera.Position - camera.Direction.Look).Normalize;
      var vUpActual = camera.Direction.Up.Normalize;
      var s = f.Cross(vUpActual);
      var u = s.Cross(f);

      MatrixBl<D1, D4> c0 = new Point4DBl(s, -camera.Position.X);
      MatrixBl<D1, D4> c1 = new Point4DBl(u, -camera.Position.Y);
      MatrixBl<D1, D4> c2 = new Point4DBl(f, -camera.Position.Z);
      MatrixBl<D1, D4> c3 = new Point4DBl((Point3DBl)0d, 1d);
      return new MatrixBl<D4, D4>(c0, c1, c2, c3);
    }

  }

}