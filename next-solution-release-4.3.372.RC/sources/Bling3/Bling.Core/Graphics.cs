using System;
using System.Collections.Generic;
using System.Linq;
using Bling.DSL;
using Bling.Util;
using Bling.Linq;
using Bling.Core;
using Bling.Matrices;
//using Bling.Vertices;

namespace Bling.Graphics {


  public interface ITex<POINT> where POINT : Brand<POINT>, IDoubleBl<POINT> {
    ColorBl this[POINT p] { get; }
  }
  public static class BaseTex<POINT> where POINT : Brand<POINT>, IDoubleBl<POINT> {
    public static Func<Func<POINT, ColorBl>, ITex<POINT>> Make { get; internal set; }
    static BaseTex() {
      new Tex1D(d => new ColorBl(1, 1, 1, 1));
      new Tex2D(d => new ColorBl(1, 1, 1, 1));
      new Tex3D(d => new ColorBl(1, 1, 1, 1));
    }
  }

  public abstract partial class BaseTex<POINT, BRAND> : DoubleFunc2Bl<POINT, ColorBl, BRAND>, ITex<POINT>
    where POINT : Brand<POINT>, IDoubleBl<POINT>
    where BRAND : BaseTex<POINT, BRAND> {
    public BaseTex(Func<POINT, ColorBl> F) : base(F) { }
    static BaseTex() {
      BaseTex<POINT>.Make = f => ToBrand(f);
      new Tex1D(d => new ColorBl(1, 1, 1, 1));
      new Tex2D(d => new ColorBl(1, 1, 1, 1));
      new Tex3D(d => new ColorBl(1, 1, 1, 1));
    }
    public new static Func<Func<POINT,ColorBl>, BRAND> ToBrand { get { return DoubleFunc2Bl<POINT, ColorBl, BRAND>.ToBrand; } }

    /// <summary>
    /// Distort texture by transforming its input point.
    /// </summary>
    public BRAND Distort(Func<POINT, POINT> G) {
      return MapA(G);
    }
    /// <summary>
    /// Access color at texture coordinate p.
    /// </summary>
    /// <param name="p">Texture coordinate.</param>
    /// <returns>Color at specified texture coordinate.</returns>
    public new ColorBl this[POINT p] { get { return base[p]; } }

    /// <summary>
    /// Shift the left top corner of the texture's view box.
    /// </summary>
    public BRAND Offset(POINT d) {
      return ToBrand(p => this[d.Add(p)]);
    }
  }
  public partial class Tex1D : BaseTex<DoubleBl, Tex1D> {
    /// <summary>
    /// Create a new texture with specified function.
    /// </summary>
    /// <param name="F">Function from a texture coordinate ([0,0]-[1,1]) to a color.</param>
    public Tex1D(Func<DoubleBl, ColorBl> F) : base(F) { }
    /// <summary>
    /// Texture flipped.
    /// </summary>
    public Tex1D Flip { get { return Distort(p => 1d - p); } }


    public static implicit operator Tex1D(Func<DoubleBl, ColorBl> F) { return new Tex1D(F); }
    /// <summary>
    /// Create a solid color texture.
    /// </summary>
    /// <param name="Color">Color of texture at every point.</param>
    public static explicit operator Tex1D(ColorBl Color) { return new Tex1D(p => Color); }
    static Tex1D() {
      Register(v => v);
    }
  }


  /// <summary>
  /// A 2 dimensional texture defined as a function from texture coordinates ([0,0]-[1,1]) to colors.
  /// </summary>
  public partial class Tex2D : BaseTex<PointBl, Tex2D> {
    /// <summary>
    /// Create a new texture with specified function.
    /// </summary>
    /// <param name="F">Function from a texture coordinate ([0,0]-[1,1]) to a color.</param>
    public Tex2D(Func<PointBl, ColorBl> F) : base(F) { }
    /// <summary>
    /// Texture flipped horizontally.
    /// </summary>
    public Tex2D FlipHorizontal { get { return Distort(p => new PointBl(1d - p.X, p.Y)); } }
    /// <summary>
    /// Texture flipped vertically.
    /// </summary>
    public Tex2D FlipVertical { get { return Distort(p => new PointBl(p.X, 1d - p.Y)); } }

    /// <summary>
    /// Crop the viewbox of the texture.
    /// </summary>
    public Tex2D Crop(PointBl LeftTop, PointBl Size) {
      return new Tex2D(p => this[LeftTop + p.Clamp(new PointBl(0,0), Size)]);
    }
    public static implicit operator Tex2D(Func<PointBl, ColorBl> F) { return new Tex2D(F); }
    /// <summary>
    /// Create a solid color texture.
    /// </summary>
    /// <param name="Color">Color of texture at every point.</param>
    public static explicit operator Tex2D(ColorBl Color) { return new Tex2D(p => Color); }

    static Tex2D() {
      Register(v => v);
    }
    /// <summary>
    /// Access color at texture coordinate (x,y).
    /// </summary>
    public ColorBl this[DoubleBl x, DoubleBl y] { get { return this[new PointBl(x, y)]; } }

    /// <summary>
    /// Create a height field from the red channel of a texture.
    /// </summary>
    public static explicit operator HeightField(Tex2D Tex) {
      return new HeightField(p => { return Tex[p].ScR; }); // use the red channel.
    }
    /// <summary>
    /// Create a normal map from the RGB channel of a texture. 
    /// </summary>
    public static explicit operator NormalMap(Tex2D Tex) {
      return new NormalMap(UV => {
        return (Point3DBl)(Tex[UV].ScRGB * 2d - 1d);
      });
    }
  }
  public partial class Tex3D : BaseTex<Point3DBl, Tex3D> {
    /// <summary>
    /// Create a new texture with specified function.
    /// </summary>
    /// <param name="F">Function from a texture coordinate ([0,0]-[1,1]) to a color.</param>
    public Tex3D(Func<Point3DBl, ColorBl> F) : base(F) { }


    public static implicit operator Tex3D(Func<Point3DBl, ColorBl> F) { return new Tex3D(F); }
    /// <summary>
    /// Create a solid color texture.
    /// </summary>
    /// <param name="Color">Color of texture at every point.</param>
    public static explicit operator Tex3D(ColorBl Color) { return new Tex3D(p => Color); }

    static Tex3D() {
      Register(v => v);
    }
  }

  /// <summary>
  /// A 2D pixel effect defined as a function from/to 2D textures.
  /// </summary>
  public partial class PixelEffect : BaseArgDoubleFuncBl<Tex2D, Tex2D, PixelEffect> {
    public PixelEffect(Func<Tex2D, Tex2D> F) : base(F) { }
    public PixelEffect(Func<Tex2D, PointBl, ColorBl> F) : this(input => new Tex2D(uv => F(input, uv))) { }
    public PixelEffect(Func<Tex2D, Func<PointBl, ColorBl>> F) : this(input => new Tex2D(uv => F(input)(uv))) { }
    static PixelEffect() { Register(v => v); }

    public static implicit operator Func<Tex2D, Func<PointBl, ColorBl>>(PixelEffect pixel) {
      return tex => pixel.Function(tex);
    }
    public static implicit operator Func<Tex2D, PointBl, ColorBl>(PixelEffect pixel) {
      return (tex,uv) => pixel[tex][uv];
    }
    public static implicit operator PixelEffect(Func<Tex2D, Tex2D> F) { return new PixelEffect(F); }
    public static implicit operator PixelEffect(Func<Tex2D, Func<PointBl, ColorBl>> F) { return new PixelEffect(F); }
    public static implicit operator PixelEffect(Func<Tex2D, PointBl, ColorBl> F) { return new PixelEffect(F); }

    // used only for Id purposes. 
    private class FakeTexOp : Ops.Operator<Vecs.Vec2<double>, Vecs.Vec4<double>, FakeTexOp> {
      public static new readonly FakeTexOp Instance = new FakeTexOp();
      private FakeTexOp() { }
      public override string Format(string s) {
        return "ArgTex[" + s + "]";
      }
    }
    /// <summary>
    /// Compose pixel effects (circle composition).
    /// </summary>
    public PixelEffect Apply(PixelEffect Other) {
      return ToBrand(a => this[Other[a]]);  
    }

    public override Tex2D Id { get { return this[FakeTex]; } }
    internal static readonly Func<PointBl, ColorBl> FakeTex = p => FakeTexOp.Instance.Make(p);
  }
  /// <summary>
  /// A transition effect to a specified "next" texture, defined as a function (Progress, RandomSeed, NextTexture) => PixelEffect. 
  /// </summary>
  public partial class Transition : BaseDoubleFuncBl<PixelEffect, Func<DoubleBl,DoubleBl,Tex2D,PixelEffect>, Transition> {
    /// <summary>
    /// Create a new transition defined by the specified function.
    /// </summary>
    /// <param name="F">A function from progress [0,1], random seed [0,1], and a specified next texture, to a pixel effect that implements the transition (according to progress).</param>
    public Transition(Func<DoubleBl, DoubleBl, Tex2D, PixelEffect> F) : base(F) { }
    static Transition() { Register(v => v); }
    public static implicit operator Transition(Func<DoubleBl, DoubleBl, Tex2D, PixelEffect> F) { return new Transition(F); }
    public override PixelEffect Id {
      get {
        var P = DoubleBl.Param("Progress");
        var R = DoubleBl.Param("Random");
        return this[P, R, PixelEffect.FakeTex];
      }
    }
    public override Transition Map(Transition Other, Func<PixelEffect, PixelEffect, PixelEffect> F) {
      return ToBrand((a, b, c) => F(this[a, b, c], Other[a, b, c]));
    }
    public override Transition Map(Transition[] Other, Func<PixelEffect[], PixelEffect> F) {
      return ToBrand((a,b,c) => {
        var rS = new PixelEffect[Other.Length];
        for (int i = 0; i < Other.Length; i++) rS[i] = Other[i][a, b, c];
        return F(rS);
      });
    }

    public override Transition Map(Transition OtherA, Transition OtherB, Func<PixelEffect, PixelEffect, PixelEffect, PixelEffect> F) {
      return ToBrand((a, b, c) => F(this[a, b, c], OtherA[a, b, c], OtherB[a,b,c]));
    }
    public override Transition MapR(Func<PixelEffect, PixelEffect> F) {
      return ToBrand((a, b, c) => F(this[a, b, c]));
    }
    /// <summary>
    /// Apply transition function to create a pixel effect.
    /// </summary>
    public PixelEffect this[DoubleBl Progress, DoubleBl RandomSeed, Tex2D NextInput] {
      get { return Function(Progress, RandomSeed, NextInput); }
    }
    /// <summary>
    /// Compose transitions.
    /// </summary>
    public Transition Apply(Transition Other) {
      return ToBrand((a, b, c) => this[a, b, c].Apply(Other[a, b, c])); 
    }
  }


  public partial interface ICurve {

  }
  /// <summary>
  /// A parametric cruve defined by a function F from a value between (0,1) to a point from [0,0] to [1,1].
  /// </summary>
  public class PCurve : DoubleFunc2Bl<DoubleBl,PointBl,PCurve>, ICurve {
    public PCurve(Func<DoubleBl, PointBl> F) : base(F) { }
    static PCurve() {
      Register(v => v);
    }
    public static implicit operator PCurve(Func<DoubleBl,PointBl> F) { return new PCurve(F); }

    /// <summary>
    /// Normals for this curve, which is a vector field perpendicular to this curve.
    /// </summary>
    public Func<DoubleBl, PointBl> Normals {
      get {
        var Df = this.Function.Derivative<PointBl,D2>();
        if (Df == null)
          throw new NotSupportedException();
        return t => {
          var p = Df(t);
          return p.Perpendicular.Normalize;
        };

      }
    }
  }
  public partial interface ICurve3D {

  }
  /// <summary>
  /// 3D parametric curve.
  /// </summary>
  public class PCurve3D : DoubleFunc2Bl<DoubleBl,Point3DBl,PCurve3D>, ICurve3D {
    public PCurve3D(Func<DoubleBl, Point3DBl> F) : base(F) {}
    static PCurve3D() { Register(v => v); }
    /// <summary>
    /// Sweep this curve with another to form a surface.
    /// </summary>
    public PSurface Sweep(Func<DoubleBl, Point3DBl> Scurve) {
      return new PSurface(p => this[p.X] + Scurve(p.Y));
    }
    public static implicit operator PCurve3D(Func<DoubleBl, Point3DBl> F) {
      return new PCurve3D(F);
    }
    public static implicit operator PCurve3D(PCurve Curve) {
      return Curve.Function;
    }
    public static implicit operator PCurve3D(Func<DoubleBl, PointBl> F) {
      return new PCurve3D(t => new Point3DBl(F(t), 0d));
    }
    /// <summary>
    /// Slope of this curve.
    /// </summary>
    public Func<DoubleBl, Point3DBl> Slope {
      get {
        var Df = this.Function.Derivative<Point3DBl, D3>();
        if (Df == null)
          throw new NotSupportedException();
        return t => {
          var p = Df(t);
          return p.Normalize;
        };

      }
    }
    /// <summary>
    /// Normals of this curve with respect to a specified axis.
    /// </summary>
    public Func<DoubleBl, Point3DBl> Normals(Point3DBl Axis) {
      return d => Slope(d).Cross(Axis);
    }
  }
  /// <summary>
  /// A 3D surface.
  /// </summary>
  public partial interface ISurface { 
    
  }

  /// <summary>
  /// An additional layer of abstraction for a collection of vertices.
  /// </summary>
  /// <typeparam name="KEY"></typeparam>
  public interface ITopology<KEY> where KEY : Brand<KEY> {
    /// <summary>
    /// How many vertices?
    /// </summary>
    int Count { get; }
    /// <summary>
    /// Vertex key for nth vertices.
    /// </summary>
    /// <param name="n">Vertex index</param>
    /// <returns>Key</returns>
    KEY this[IntBl n] { get; }
    /// <summary>
    /// Triangle list index buffer.
    /// </summary>
    List<int> IndexBuffer { get; }
  }
  /// <summary>
  /// Vertex keys for parametric surfaces. Continuous surfaces is divided into Width by Height vertices specified by the keys. 
  /// </summary>
  public class PSurfaceSample : ITopology<PointBl> {
    public int Count { get { return Width * Height; } }
    /// <summary>
    /// Number of columns in vertex arrangement.
    /// </summary>
    public readonly int Width;
    /// <summary>
    /// Number of rows in vertex arrangment. 
    /// </summary>
    public readonly int Height;
    public PSurfaceSample(int Width, int Height) { this.Width = Width; this.Height = Height ; }
    public PointBl this[IntBl n] {
      get {
        return new PointBl(n % Width, n / Width) / new PointBl(Width - 1, Height - 1) - .5d;
      }
    }
    public List<int> IndexBuffer {
      get {
        var Indices = new List<int>();
        Func<int, int, int> fN = (i, j) => (j % Height) * Width + (i % Width);
        for (int i = 0; i < Width - 1; i++) {
          for (int j = 0; j < Height - 1; j++) {
            Indices.Add(fN(i, j), fN(i, j + 1), fN(i + 1, j));
            Indices.Add(fN(i + 1, j + 1), fN(i + 1, j), fN(i, j + 1));
          }
        }
        return Indices;
      }

    }
  }
  public class FaceTopology : ITopology<PointBl> {
    public int Count { get { return 4; } }
    private readonly PointBl[] Vertices;
    private readonly Point3DBl[] Normals;

    public FaceTopology() {
      Vertices = new PointBl[] {
        new PointBl( -1.0f, -1.0f), // SW
        new PointBl( +1.0f, -1.0f), // SE
        new PointBl( +1.0f, +1.0f), // NE
        new PointBl( -1.0f, +1.0f), // NW
      };
      Normals = new Point3DBl[]{
        new Point3DBl( 0.0f, 0.0f, -1.0f ), 
      };
    }
    public List<int> IndexBuffer {
      get {
        int[] Indices = {
          1, 0, 2,
          2, 0, 3,
        }; 
        return Indices.ToList();
      }
    }
    public PointBl this[IntBl n] {
      get {
        return Vertices.Table(n);
      }
    }
  }

  public class CubeTopology : ITopology<Point3DBl> {
    public int Count { get { return 4 * 6; } }

    private readonly Point3DBl[] Vertices;
    private readonly Point3DBl[] Normals;
    public int [] Indices;

    public CubeTopology() {
      Vertices = new Point3DBl[] {
       new Point3DBl( -1.0f, 1.0f, -1.0f ),
       new Point3DBl( 1.0f, 1.0f, -1.0f ),
       new Point3DBl( 1.0f, 1.0f, 1.0f ),
       new Point3DBl( -1.0f, 1.0f, 1.0f ),

       new Point3DBl( -1.0f, -1.0f, -1.0f ),
       new Point3DBl( 1.0f, -1.0f, -1.0f ), 
       new Point3DBl( 1.0f, -1.0f, 1.0f ), 
       new Point3DBl( -1.0f, -1.0f, 1.0f ), 

       new Point3DBl( -1.0f, -1.0f, 1.0f ), 
       new Point3DBl( -1.0f, -1.0f, -1.0f ), 
       new Point3DBl( -1.0f, 1.0f, -1.0f ), 
       new Point3DBl( -1.0f, 1.0f, 1.0f ), 

       new Point3DBl( 1.0f, -1.0f, 1.0f ), 
       new Point3DBl( 1.0f, -1.0f, -1.0f ), 
       new Point3DBl( 1.0f, 1.0f, -1.0f ), 
       new Point3DBl( 1.0f, 1.0f, 1.0f ), 

       new Point3DBl( -1.0f, -1.0f, -1.0f ), 
       new Point3DBl( 1.0f, -1.0f, -1.0f ), 
       new Point3DBl( 1.0f, 1.0f, -1.0f ), 
       new Point3DBl( -1.0f, 1.0f, -1.0f ),

       new Point3DBl( -1.0f, -1.0f, 1.0f ), 
       new Point3DBl( 1.0f, -1.0f, 1.0f ), 
       new Point3DBl( 1.0f, 1.0f, 1.0f ), 
       new Point3DBl( -1.0f, 1.0f, 1.0f ),   
       };
      Normals = new Point3DBl[]{
       new Point3DBl( 0.0f, 1.0f, 0.0f ), 
       new Point3DBl( 0.0f, -1.0f, 0.0f ), 
       new Point3DBl( -1.0f, 0.0f, 0.0f ), 
       new Point3DBl( 1.0f, 0.0f, 0.0f ), 
       new Point3DBl( 0.0f, 0.0f, -1.0f ), 
       new Point3DBl( 0.0f, 0.0f, 1.0f ), 
       };
    }
    public Point3DBl Normal(IntBl Face) { return Normals.Table(Face); }
    public Point3DBl this[IntBl n] {
      get {
        return Vertices.Table(n);
      }
    }
    public List<int> IndexBuffer {
      get {
        int[] Indices = {
                  3,1,0,
                  2,1,3,

                  6,4,5,
                  7,4,6,
                  
                  11,9,8,
                  10,9,11,

                  14,12,13,
                  15,12,14,
                  
                  19,17,16,
                  18,17,19,
                  
                  22,20,21,
                  23,20,22,

                         };

        this.Indices = Indices;
        var IB = new List<int>();
        IB.Add(Indices);
        return IB;
      }

    }
  }  
  
  /*
  /// <summary>
  /// Vertex to be rendered by device. 
  /// </summary>
  public interface IVertex {
    /// <summary>
    /// Color of pixel, or vertex if interpolation is used.
    /// </summary>
    ColorBl Color { set; }
    /// <summary>
    /// Final position of vertex.
    /// </summary>
    Point4DBl Position { set; } 
    // XXX: add depth later, whatever else. 
  }
  public interface IRenderDevice {
    /// <summary>
    /// Render vertices.
    /// </summary>
    /// <param name="Count">Number of vertices to render.</param>
    /// <param name="F">Rendering implementation.</param>
    /// <param name="IndexBuffer">Triangle list.</param>
    void Render(int Count, Action<IntBl,IVertex> F, List<int> IndexBuffer) ;
    void Render(int VertexCount, int InstanceCount, Action<IntBl, IntBl, IVertex> F, List<int> IndexBuffer);
  }
  */

  /// <summary>
  /// A parametric surface formed by a function F from +-[1/2,1/2] to +-[1,1,1].
  /// </summary>
  public class PSurface : DoubleFunc2Bl<PointBl,Point3DBl,PSurface>, ISurface {
    /// <summary>
    /// Construct a parametric surface using F.
    /// </summary>
    /// <param name="F">A function from +-[1/2,1/2] to +-[1,1,1], which forms a 3D position.</param>
    public PSurface(Func<PointBl, Point3DBl> F) : base(F) { }
    static PSurface() { Register(v => v); }
    public static implicit operator PSurface(Func<PointBl, Point3DBl> F) {
      return new PSurface(F);
    }

    public static PSurface operator *(PSurface Surface, MatrixBl<D4, D4> Matrix) {
      return new PSurface(p => (Point3DBl) ((Point4DBl) (Surface[p] * Matrix)));
    }
    public static PSurface operator *(MatrixBl<D4, D4> Matrix, PSurface Surface) {
      return new PSurface(p => (Point3DBl)((Point4DBl)(Matrix * Surface[p])));
    }

    /// <summary>
    /// Displace this surface using a height field to create a new surface.
    /// </summary>
    /// <param name="HeightField">Height field that will displace surface.</param>
    /// <returns>Displaced surface.</returns>
    public PSurface Displace(Func<PointBl, DoubleBl> HeightField) {
      return new PSurface(p =>
        this[p] + Normals[p] * HeightField(p));
    }
    /// <summary>
    /// The normal mapping for this surface computed from the partial derivatives of parametric position function F.
    /// </summary>
    /// <exception cref="NotSupportedException">If F's derivatives cannot be computed automatically.</exception>
    public NormalMap Normals {
      // we can use derivation.
      get {
        if (Normals0 == null) {
          var F = this.Derivative();
          if (F == null) throw new NotSupportedException("no normal map");
          Normals0 = new NormalMap(p => {
            var M = F(p); // oh...so nice!
            var rowA = (MatrixBl<D1, D3>)M.Columns[0];
            var rowB = (MatrixBl<D1, D3>)M.Columns[1];

            var cross = ((Point3DBl)rowA).Cross((Point3DBl)rowB);
            return cross.Normalize;

          });
        }
        return Normals0;
      } 
    }


    private NormalMap Normals0 = null;

    public NormalMap Tangents {
      // we can use derivation.
      get {
        var F = this.Derivative();
        if (F == null) throw new NotSupportedException("no tangent");
        return new NormalMap(p => {
          var M = F(p); // oh...so nice!
          var row = (MatrixBl<D1, D3>)M.Columns[0];
          
          return ((Point3DBl)row).Normalize;
        });
      } 
    }
  }
  /// <summary>
  /// A specification of 3D normals for an otherwise 2D surface.
  /// </summary>
  public class NormalMap : DoubleFunc2Bl<PointBl,Point3DBl,NormalMap> {
    public NormalMap(Func<PointBl, Point3DBl> F) : base(F) { }
    static NormalMap() { Register(v => v); }
    public static implicit operator NormalMap(Func<PointBl, Point3DBl> F) { return new NormalMap(F); }
    public static implicit operator PSurface(NormalMap F) { return F.Function; }
  }
  /// <summary>
  /// A specification of heights for a 2D surface. 
  /// </summary>
  public class HeightField : DoubleFunc2Bl<PointBl,DoubleBl,HeightField> {
    public HeightField(Func<PointBl, DoubleBl> F) : base(F) { }
    static HeightField() { Register(v => v); }
    public static implicit operator HeightField(Func<PointBl, DoubleBl> F) { return new HeightField(F); }
    public static implicit operator PSurface(HeightField F) {
      return new PSurface(p => new Point3DBl(p * 2d, F[p]));
    }
    public static implicit operator Func<PointBl, Point3DBl>(HeightField F) {
      return ((PSurface)F);
    }
    /// <summary>
    /// Create new height field with adjusted frequency, magnitude, and phase.
    /// </summary>
    /// <param name="Frequency">The number of occurrences of a repeating distortion per unit length. Suggested values: 10, 20, or 30.</param>
    /// <param name="Magnitude">Relative height of field's distortion. Suggested values: 0.05, .1.</param>
    /// <param name="Phase">Fraction of a complete cycle corresponding to an offset in the displacement from origin. Should be between 0 and 1.</param>
    public HeightField Adjust(DoubleBl Frequency, DoubleBl Magnitude, DoubleBl Phase) {
      return new HeightField(p => Magnitude * this[Frequency * (p + Phase)]);
    }



    /// <summary>
    /// Convert to normal map using approximation.
    /// </summary>
    /// <param name="Size">Size of target image undergoing approximation.</param>
    /// <param name="Scale">Extremeness of approximated normals.</param>
    /// <returns>Normal map populated with approximated values.</returns>
    public NormalMap ToNormals(PointBl Size, DoubleBl Scale) {
      return new NormalMap(UV => {
        var dx = this[new PointBl(UV.X + 1d / Size.X, UV.Y)] -
                 this[new PointBl(UV.X - 1d / Size.X, UV.Y)];
        var dy = this[new PointBl(UV.X, UV.Y + 1d / Size.Y)] -
                 this[new PointBl(UV.X, UV.Y - 1d / Size.Y)];
        return new Point3DBl(-dx * Scale, -dy * Scale, 1).Normalize;
      });
    }
  }

  public class ColorToColor : BaseDoubleFuncBl<ColorBl, Func<Tex2D,PointBl,ColorBl>, ColorToColor> {
    public ColorToColor(Func<Tex2D, PointBl, ColorBl> F) : base(F) { }
    public ColorToColor(Func<ColorBl, ColorBl> F) : this((input,uv) => F(input[uv])) { }

    static ColorToColor() { Register(v => v); }
    /// <summary>
    /// Convert color based directly on another color.
    /// </summary>
    public ColorBl ForColor(ColorBl Color) {
      return Function(new Tex2D(uv => Color), 0d);
    }
    /// <summary>
    /// Convert color based directly on a pixel addressed in a texture.
    /// </summary>
    public ColorBl ForPixel(Tex2D Input, PointBl UV) {
      return Function(Input, UV);
    }
    /*
    public ColorBl this[Tex2D Input, PointBl UV] {
      get { return Function(Input, UV); }
    }*/

    public static implicit operator ColorToColor(Func<Tex2D, PointBl, ColorBl> F) { return new ColorToColor(F); }
    public static implicit operator ColorToColor(Func<ColorBl, ColorBl> F) { return new ColorToColor(F); }
    public static implicit operator Func<ColorBl, ColorBl>(ColorToColor F) { return clr => F.ForColor(clr); }
    public override ColorToColor Map(ColorToColor Other, Func<ColorBl, ColorBl, ColorBl> F) {
      return new ColorToColor((Input, UV) => F(this.ForPixel(Input, UV), Other.ForPixel(Input, UV)));
    }
    public override ColorToColor Map(ColorToColor[] Other, Func<ColorBl[], ColorBl> F) {
      return ToBrand((a,b) => {
        var rS = new ColorBl[Other.Length];
        for (int i = 0; i < Other.Length; i++) rS[i] = Other[i].ForPixel(a, b);
        return F(rS);
      });
    }

    public override ColorToColor Map(ColorToColor OtherA, ColorToColor OtherB, Func<ColorBl, ColorBl, ColorBl, ColorBl> F) {
      return new ColorToColor((Input, UV) => F(this.ForPixel(Input, UV), OtherA.ForPixel(Input, UV), OtherB.ForPixel(Input,UV)));
    }
    public override ColorToColor MapR(Func<ColorBl, ColorBl> F) {
      return new ColorToColor((Input, UV) => F(this.ForPixel(Input,UV)));
    }
    public override ColorBl Id {
      get { return Function(PixelEffect.FakeTex, PointBl.Param("UV")); }
    }
  }

  /// <summary>
  /// A lighting function from a 3D position, normal, eye position to a color. 
  /// </summary>
  public class Lighting : DoubleFuncBl<Point3DBl, Point3DBl, Point3DBl, ColorToColor, Lighting> {
    /// <summary>
    /// Create a lighting function.
    /// </summary>
    /// <param name="F">Underlying function from a 3D position, normal, eye position to the Color of resulting light.</param>
    private Lighting(Func<Point3DBl, Point3DBl, Point3DBl, ColorToColor> F) : base(F) { }
    public Lighting(Func<Point3DBl, Point3DBl, Point3DBl, Func<ColorBl, ColorBl>> F) : this((p,n,e) => (ColorToColor)F(p, n, e)) { }
    public Lighting(Func<Point3DBl, Point3DBl, Point3DBl, Func<Tex2D, PointBl, ColorBl>> F) : this((p, n, e) => (ColorToColor)F(p, n, e)) { }

    public Lighting(Func<Point3DBl, Point3DBl, Func<ColorBl, ColorBl>> F) : this((p, n, e) => (ColorToColor)F(p, n)) { }
    public Lighting(Func<Point3DBl, Point3DBl, Func<Tex2D, PointBl, ColorBl>> F) : this((p, n, e) => (ColorToColor)F(p, n)) { }


    static Lighting() { Register(v => v); }
    public static implicit operator Lighting(Func<Point3DBl, Point3DBl, Point3DBl, ColorToColor> F) { return new Lighting(F); }
    public static implicit operator Lighting(Func<Point3DBl, Point3DBl, Point3DBl, Func<ColorBl,ColorBl>> F) { return new Lighting(F); }
    /// <summary>
    /// Apply lighting function to specified position and normal.
    /// </summary>
    public ColorToColor ApplyAt(Point3DBl Position, Point3DBl Normal) { return this[Position, Normal, new Point3DBl(0,0,-1)]; } 
    /// <summary>
    /// Apply lighting function to specified position, normal, and eye position.
    /// </summary>
    public ColorToColor ApplyAt(Point3DBl Position, Point3DBl Normal, Point3DBl Eye) { return this[Position, Normal, Eye]; }

    //public new ColorToColor this[Point3DBl Position, Point3DBl Normal, Point3DBl Eye] { get { return this[Position, Normal, Eye]; } }


    /// <summary>
    /// Create a texture from this lighting functiong using a specified surface and normal map.
    /// </summary>
    /// <param name="Surface">A PSurface or compatible function.</param>
    /// <param name="Normals">A NormalMap or compatible function.</param>
    public PixelEffect Apply(PSurface Surface, NormalMap Normals) {
      return new PixelEffect(Input => new Tex2D(UV => this.ApplyAt(Surface[UV], Normals[UV]).ForPixel(Input, UV))); 
    }
    public PixelEffect Apply(PSurface Surface, NormalMap Normals, Point3DBl Eye) {
      return new PixelEffect(Input => new Tex2D(UV => this[Surface[UV], Normals[UV], Eye].ForPixel(Input, UV))); 
    }


    /// <summary>
    /// Create a texture from specified surface.
    /// </summary>
    /// <param name="Surface">PSurface or compatible function.</param>
    public PixelEffect Apply(PSurface Surface) {
        return this.Apply(Surface, (Surface).Normals);
    }
    /// <summary>
    /// Create a lit texture from specified surface.
    /// </summary>
    /// <param name="Surface">PSurface or compatible function.</param>
    /// <param name="Eye">Position of observer.</param>
    public PixelEffect Apply(PSurface Surface, Point3DBl Eye) {
        return this.Apply(Surface, (Surface).Normals, Eye);
    }
    /// <summary>
    /// Create a texture from a specified height field and normal map with respect to a specific target texture size.
    /// </summary>
    /// <param name="Heights">HeightField or compatible function.</param>
    /// <param name="Normals">NormalMap or compatible function.</param>
    /// <param name="Size">Size.XY():Size of target texture that lighting will be applied to.
    /// Size.XY():Size of target texture that lighting will be applied to.
    /// Size.Z:Scale of the height to make it map the texture best.
    /// </param>
    public PixelEffect Apply(HeightField Heights, NormalMap Normals, Point3DBl Size) {
        return this.Apply(Heights, Normals, Size, new Point3DBl(0,0,0));
    }

    /// <summary>
    /// Create a texture from a specified height field and normal map with respect to a specific target texture size.
    /// Will apply parallax bias to resulting effect.
    /// </summary>
    /// <param name="Heights">HeightField or compatible function.</param>
    /// <param name="Normals">NormalMap or compatible function.</param>
    /// <param name="Size">Size.XY():Size of target texture that lighting will be applied to.
    /// Size.XY():Size of target texture that lighting will be applied to.
    /// Size.Z:Scale of the height to make it map the texture best.
    /// </param>
    /// <param name="Eye">the positon of eye/view point</param>
    /// <returns></returns>
    public PixelEffect Apply(HeightField Heights, NormalMap Normals, Point3DBl Size, Point3DBl Eye) {
      return new PixelEffect(input => new Tex2D(UV => {
        Point3DBl Position = new Point3DBl(UV * Size.XY(), Heights[UV] * Size.Z);
        var eyeDir = (Eye - Position).Normalize;
        var UV0 = UV + eyeDir.XY() * Heights[UV] * Size.Z;
        return this[Position, Normals[UV0], Eye].ForPixel(input, UV0);
      }));
    }
    /// <summary>
    /// Create a texture from a normal map with respect to a specific target texture size. Does not use high map like the previous API.
    /// </summary>
    /// <param name="Normals">NormalMap or compatible function.</param>
    /// <param name="Size">Size of target texture that lighting will be applied to.</param>
    public PixelEffect Apply(NormalMap Normals, PointBl Size) {
        // XXX: a property default eye?
        return this.Apply(Normals, Size, new Point3DBl(Size / 2, -1));
    }
    /// <summary>
    /// Create a texture from a normal map with respect to a specific target texture size. Does not use high map like the previous API.
    /// </summary>
    /// <param name="Normals">NormalMap or compatible function.</param>
    /// <param name="Size">Size of target texture that lighting will be applied to.</param>
    /// <param name="Eye">3D eye position of observer (default to middle of texture straight ahead)</param>
    public PixelEffect Apply(NormalMap Normals, PointBl Size, Point3DBl Eye) {
        return this.Apply(new HeightField(p => 0d), Normals, Size.InsertZ(0), Eye);
    } 
    /// <summary>
    /// Create a texture from a specified height field with respect to a target texture size and approximate normal scale.
    /// </summary>
    /// <param name="Heights">HeightField or compatible function.</param>
    /// <param name="Size">Size of target texture that lighting will be applied to.</param>
    /// <param name="Scale">Scale to use in normal approximation.</param>
    public PixelEffect Apply(HeightField Heights, Point3DBl Size, DoubleBl Scale) {
        return this.Apply(Heights, (Heights).ToNormals(Size.XY(), Scale), Size, new Point3DBl(0,0,0));
    }
    /// <summary>
    /// Create a texture from a specified height field with respect to a target texture size and approximate normal scale.
    /// </summary>
    /// <param name="Heights">HeightField or compatible function.</param>
    /// <param name="Size">Size of target texture that lighting will be applied to.</param>
    /// <param name="Scale">Scale to use in normal approximation.</param>
    /// <param name="Eye">Location of observer.</param>
    public PixelEffect Apply(HeightField Heights, Point3DBl Size, DoubleBl Scale, Point3DBl Eye) {
        return this.Apply(Heights, (Heights).ToNormals(Size.XY(), Scale), Size, Eye);
    }
  }  
  public static class GraphicDefinitions {    
    public static readonly PCurve Circle = new PCurve(t => {
      return t.PI2().SinCos.YX();
    });
    public static readonly PSurface Sphere = new PSurface((PointBl p) => {
      PointBl sin = p.SinU;
      PointBl cos = p.CosU;
      return new Point3DBl(sin[0] * cos[1], sin[0] * sin[1], cos[0]);
    });
    public static PCurve Bezier(params PointBl[] points) {
      return new PCurve(t => t.Bezier(points));
    }
    public static PSurface Cylinder(DoubleBl Height) {
      // alternative definition.
      //return PCurve3DBl.Sweep(Circle.Function.AddZ(0d), t => new Point3DBl(0, 0, Height * t));


      return new PSurface(p => {
        return new Point3DBl((p.X).PI2().SinCos.YX(), Height * p.Y);
      });
    }
    // height fields.
    public static readonly HeightField Flat = new HeightField(p => 0d);
    public static readonly HeightField Ripple = new HeightField(p => p.Length.SinU);
    public static readonly HeightField EggCrate = new HeightField((PointBl p) => {
      p = p.SinCosU;
      return p.X * p.Y;
    });
  }


  public static class GraphicsExtensions {
  }
}
