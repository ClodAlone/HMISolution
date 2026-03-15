using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Bling;
using Semantics = Bling.Semantics;
using Bling.Util;
using Bling.DSL;
using Bling.Properties;
using Bling.Core;
using Bling.Mixins;
using Bling.Graphics;
using Bling.Matrices;


namespace Bling.DX {
  /// <summary>
  /// Mark buffers of various kinds, not typically used by users.
  /// </summary>
  public interface IBufferMarker { }
  public abstract class BaseBuffer : IBufferMarker {
    /// <summary>
    /// Element types of the buffer.
    /// </summary>
    public abstract List<SignatureEntry> InputSig { get; }
    /// <summary>
    /// Geometry topology of the buffer.
    /// </summary>
    public abstract Topology BaseTopology { get; }
  }
  /// <summary>
  /// A buffer with a typed input signature.
  /// </summary>
  /// <typeparam name="INPUT">Input signature type.</typeparam>
  public abstract class BaseBuffer<INPUT> : BaseBuffer where INPUT : ISignature {}
  /// <summary>
  /// A buffer with a typed input signature and geometric topology
  /// </summary>
  public abstract class BaseBuffer<INPUT, KIND> : BaseBuffer<INPUT>
    where INPUT : ISignature
    where KIND : Topology.Kind<KIND>, new() {}
  /// <summary>
  /// A buffer with a typed input signature and a geometry topology/kind.
  /// </summary>
  public abstract class BaseBuffer<INPUT, KIND, ORG> : BaseBuffer<INPUT,KIND>
    where INPUT : ISignature
    where KIND : Topology.Kind<KIND>, new()
    where ORG : Topology.Organization<ORG>, new() {
    public override List<SignatureEntry> InputSig { get { return ShaderSigs.Decode(typeof(INPUT)); } }
    public virtual ISignature InputSig0(ISignatureManager2 mgr) { return ShaderSigs.Generate<INPUT>(mgr); }
  }
  /// <summary>
  /// User-defined buffer
  /// </summary>
  public interface IUserBuffer {
    int VertexCount { get; }
    int InstanceCount { get; }

    Dictionary<string, Expr> BaseF(IntBl VertexId, IntBl InstanceId);
    /// <summary>
    /// User defined index buffer.
    /// </summary>
    IndexBuffer BaseIndices { get; }
    /// <summary>
    /// User defined topology.
    /// </summary>
    Topology BaseTopology { get; }
  }
  /// <summary>
  /// User buffer with typed input signature, geometric topology kind and organizations.
  /// </summary>
  public class UserBuffer<INPUT,KIND,ORG> : BaseBuffer<INPUT,KIND,ORG>, IUserBuffer
    where INPUT : ISignature
    where KIND : Topology.Kind<KIND>, new()
    where ORG : Topology.Organization<ORG>, new() {
    public UserBuffer() {
      InstanceCount = 1;
    }
    /// <summary>
    /// Number of instances defined by buffer, default is 1.
    /// </summary>
    public int InstanceCount { get; set; }
    /// <summary>
    /// Number of vertices defined by buffer.
    /// </summary>
    public int VertexCount { get; set; }
    /// <summary>
    /// Function that computes input element given vertex and instance Ids.
    /// </summary>
    public Action<IntBl, IntBl, INPUT> F { get; set; }
    /// <summary>
    /// Function that computes input element given vertex (ignoring instance ID).
    /// </summary>
    public Action<IntBl, INPUT> F0 { set { F = (v, i, input) => value(v, input); } }

    /// <summary>
    /// Internal use only.
    /// </summary>
    public Dictionary<string, Expr> BaseF(IntBl VertexId, IntBl InstanceId) {
      var Inputs = new DefaultSignatureManager2();
      var InputSig = InputSig0(Inputs);
      F(VertexId, InstanceId, (INPUT) InputSig);
      return Inputs.Bindings;
    }
    /// <summary>
    /// User specified index buffer.
    /// </summary>
    public IndexBuffer<KIND, ORG> Indices {
      get;
      set;
    }
    /// <summary>
    /// Initializes indices in init block.
    /// </summary>
    public Action<IndexBuffer<KIND, ORG>> InitIndices {
      set {
        Indices = new IndexBuffer<KIND, ORG>();
        value(Indices);
      }
    }
    public IndexBuffer BaseIndices { get { return Indices; } }
    /// <summary>
    /// User defined topology. 
    /// </summary>
    public Topology<KIND, ORG> Topology = new Topology<KIND, ORG>(); // simple topology. 
    public override Topology BaseTopology { get { return Topology; } }
  }
  /// <summary>
  /// Mark stream output buffers.
  /// </summary>
  public interface IStreamOutBufferMarker { }
  /// <summary>
  /// Stream out buffer interface.
  /// </summary>
  public interface IStreamOutBuffer : IStreamOutBufferMarker {
    /// <summary>
    /// Maximum number of elements the stream out buffer can hold.
    /// </summary>
    int MaxCount { get; }
  }
  /// <summary>
  /// Stream out buffer class.
  /// </summary>
  /// <typeparam name="INPUT">Signature type of stream out buffer.</typeparam>
  /// <typeparam name="KIND">Topology of stream out buffer; i.e., triangle, point, or line.</typeparam>
  public class StreamOutBuffer<INPUT, KIND> : BaseBuffer<INPUT,KIND,Topology.ListOrg>, IStreamOutBuffer
    where INPUT : ISignature
    where KIND : Topology.Kind<KIND>, new() {
    /// <summary>
    /// Maximum number of elements the stream out buffer can hold.
    /// </summary>
    public int MaxCount { get; set; }
    public override Topology BaseTopology {
      get {
        if (true || typeof(KIND) == typeof(Topology.PointKind))
          return new Topology<KIND, Topology.ListOrg>();
        else
          return new Topology<KIND, Topology.StripOrg>();
      }
    }
  }
  /// <summary>
  /// Interface for buffer brand.
  /// </summary>
  public interface IBaseBufferBl : IBrandT<IBufferMarker> { }
  /// <summary>
  /// Base buffer brand that wraps buffers for shader generation.
  /// </summary>
  public class BaseBufferBl<INPUT> : Brand<IBufferMarker, BaseBufferBl<INPUT>>, IBaseBufferBl where INPUT : ISignature {
    public BaseBufferBl(Expr<BaseBuffer<INPUT>> Underlying) : this(Ops.UpCastOperator<BaseBuffer<INPUT>,IBufferMarker>.Instance.Make(Underlying)) { }
    internal BaseBufferBl(Expr<IBufferMarker> Underlying) : base(Underlying) { }
    public static implicit operator BaseBufferBl<INPUT>(Expr<IBufferMarker> e) { return new BaseBufferBl<INPUT>(e); }
    public static implicit operator BaseBufferBl<INPUT>(Expr<BaseBuffer<INPUT>> e) { return new BaseBufferBl<INPUT>(e); }
    public static implicit operator BaseBufferBl<INPUT>(BaseBuffer<INPUT> t) { return new Constant<BaseBuffer<INPUT>>(t); }
    static BaseBufferBl() {
      ToBrand = (b) => new BaseBufferBl<INPUT>(b);
    }
  }
  /// <summary>
  /// Base buffer brand that wraps buffers for shader generation, adds topology kind for use with geometry shaders.
  /// </summary>
  public class BaseBufferBl<INPUT, KIND> : Brand<IBufferMarker, BaseBufferBl<INPUT, KIND>>, IBaseBufferBl
    where INPUT : ISignature
    where KIND : Topology.Kind<KIND>, new() {
    public BaseBufferBl(Expr<BaseBuffer<INPUT,KIND>> Underlying) : this(Ops.UpCastOperator<BaseBuffer<INPUT,KIND>, IBufferMarker>.Instance.Make(Underlying)) { }
    private BaseBufferBl(Expr<IBufferMarker> Underlying) : base(Underlying) { }
    public static implicit operator BaseBufferBl<INPUT,KIND>(Expr<IBufferMarker> e) { return new BaseBufferBl<INPUT,KIND>(e); }
    public static implicit operator BaseBufferBl<INPUT,KIND>(Expr<BaseBuffer<INPUT,KIND>> e) { return new BaseBufferBl<INPUT,KIND>(e); }
    public static implicit operator BaseBufferBl<INPUT,KIND>(BaseBuffer<INPUT,KIND> t) { return new Constant<BaseBuffer<INPUT,KIND>>(t); }
    public static implicit operator BaseBufferBl<INPUT>(BaseBufferBl<INPUT, KIND> t)  { return new BaseBufferBl<INPUT>(t.Underlying); }

    static BaseBufferBl() {
      ToBrand = (b) => new BaseBufferBl<INPUT,KIND>(b);
    }
  }

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
  }
  /// <summary>
  /// Device that accumulates rendering commands for later processing by DirectX. 
  /// </summary>
  public partial class RenderDevice : IDisposable /*, IRenderDevice */ {
    private class DisplayClass : RenderTarget {
      public override string ToString() { return "Display"; }
    }
    /// <summary>
    /// Represents the display of the hosting device.
    /// </summary>
    public readonly RenderTarget Display;
    public RenderDevice() {
      Display = new DisplayClass();
    }
    public readonly List<RenderEntry> Entries = new List<RenderEntry>();
    /// <summary>
    /// Add one or more render entry to device.
    /// </summary>
    public void AddRender(params RenderEntry[] Entries) {
      this.Entries.Add(Entries);
    }
    public virtual void Dispose() { }
  }

  public abstract partial class RenderEntry {
    /// <summary>
    /// What is being rendered to, could be display or texture.
    /// </summary>
    public RenderTargetBl Target; 
    /// <summary>
    /// Buffer that is initially pumped into render.
    /// </summary>
    public IBaseBufferBl Buffer { get; protected set; }
    protected abstract Effect.Technique BaseTechnique { get; }
    protected abstract IStreamOutBufferBl BaseStreamOut { get; }
    /// <summary>
    /// Technique used for rendering.
    /// </summary>
    public Effect.Technique Technique { get { return BaseTechnique; } }
    /// <summary>
    /// Optional stream out, if stream out is specified render target can be unset.
    /// </summary>
    public IStreamOutBufferBl StreamOut { get { return BaseStreamOut; } }
    /// <summary>
    /// Uniform bindings for rendering operation.
    /// </summary>
    public Dictionary<SignatureEntry,Expr> Uniforms { get { return BaseUniforms; } }
    protected abstract Dictionary<SignatureEntry, Expr> BaseUniforms { get; }

    /// <summary>
    /// Optional action to invoke after rendering.
    /// </summary>
    public Action Post { get; set; }
  }
  /// <summary>
  /// Typed rendering entry, used when rendering is defined without inference.
  /// </summary>
  public partial class RenderEntry<UNIFORMS, INPUT> : RenderEntry 
    where UNIFORMS : ISignature
    where INPUT : ISignature {

    private DefaultSignatureManager2 Uniforms0;
    public DefaultSignatureManager2 UniformsManager {
      get {
        if (Uniforms0 == null) Uniforms0 = new DefaultSignatureManager2();
        return Uniforms0;
      }
      set { Uniforms0 = value; } 
    }
    /// <summary>
    /// Uniforms used for rendering.
    /// </summary>
    public new UNIFORMS Uniforms {
      get { return ShaderSigs.Generate<UNIFORMS>(Uniforms0); }
    }
    protected override Dictionary<SignatureEntry, Expr> BaseUniforms {
      get {
        var ret = new Dictionary<SignatureEntry, Expr>();
        foreach (var p in ShaderSigs.Decode(typeof(UNIFORMS))) {
          ret[p] = Uniforms0.Bindings[p.Name];
        }
        return ret;
      }
    }
    /// <summary>
    /// Input buffer.
    /// </summary>
    public new BaseBufferBl<INPUT> Buffer {
      get { return (BaseBufferBl<INPUT>)base.Buffer; }
      set { base.Buffer = value; }
    }
    /// <summary>
    /// Technique used for rendering.
    /// </summary>
    public new Effect<UNIFORMS>.Technique<INPUT> Technique { get; set; }
    protected override Effect.Technique BaseTechnique { get { return Technique; } }
    protected override IStreamOutBufferBl BaseStreamOut { get { return null; } }
  }
  public partial class RenderEntry<UNIFORMS, INPUT, KIND> : RenderEntry<UNIFORMS,INPUT>
    where KIND : Topology.Kind<KIND>, new()
    where UNIFORMS : ISignature
    where INPUT : ISignature {}

  public partial class RenderEntry<UNIFORMS, INPUT, GOUT, KIND, KOUT, ORG> : RenderEntry<UNIFORMS,INPUT,KIND>
    where KIND : Topology.Kind<KIND>, new()
    where KOUT : Topology.Kind<KOUT>, new()
    where ORG : Topology.Organization<ORG>, new()
    where UNIFORMS : ISignature
    where INPUT : ISignature
    where GOUT : ISignature {
  
    /// <summary>
    /// Technique used for rendering.
    /// </summary>
    public new Effect<UNIFORMS>.Technique<INPUT, GOUT, KIND, KOUT> Technique { set { base.Technique = value; } }
    /// <summary>
    /// Alternate technique assignment if geometry outputs aren't specified.
    /// </summary>
    public Effect<UNIFORMS>.Technique<INPUT, KIND> TechniqueAlt { set { base.Technique = value; } }
    /// <summary>
    /// Streamout can be assigned explicitly.
    /// </summary>
    public new StreamOutBufferBl<GOUT, KOUT> StreamOut = null; 
    protected override IStreamOutBufferBl BaseStreamOut { get { return StreamOut; } }
  }
  /// <summary>
  /// Mark render targets.
  /// </summary>
  public interface IRenderTargetMarker { }
  /// <summary>
  /// Surface for rendering to; e.g., a display or texture.
  /// </summary>
  public abstract partial class RenderTarget : IRenderTargetMarker { } // assuming not stream out, allows for render to texture.
  /// <summary>
  /// Basic texture interface.
  /// </summary>
  public interface ITexture {
    int DimCount { get; }
  }
  /// <summary>
  /// Basic texture root class.
  /// </summary>
  public abstract partial class Texture : ITexture {
    public abstract int DimCount { get; }
  }
  /// <summary>
  /// Basic texture root class.
  /// </summary>
  /// <typeparam name="DIM">Dimensions of texture; e.g., D2 for 2D texture.</typeparam>
  public abstract partial class Texture<DIM> : Texture where DIM : DNum<DIM> {
    public override int DimCount { get { return DNum<DIM>.Count; } }
  }
  /// <summary>
  /// Basic interface for image-based texture.
  /// </summary>
  public interface IImageTexture : ITexture {
    Uri URI { get; }
  }
  /// <summary>
  /// Image based texture defined via a URI.
  /// </summary>
  /// <typeparam name="DIM">Dimensions of texture.</typeparam>
  public partial class ImageTexture<DIM> : Texture<DIM>, IImageTexture where DIM : DNum<DIM> {
    public Uri URI { get; private set; }
    public ImageTexture(Uri URI) { this.URI = URI; }
    public override int GetHashCode() {
      return URI.GetHashCode();
    }
    public override bool Equals(object obj) {
      return obj is ImageTexture<DIM> && ((ImageTexture<DIM>)obj).URI.Equals(URI);
    }
  }
  /// <summary>
  /// Basic user-defined texture interface.
  /// </summary>
  public interface IUserTexture : ITexture {
    /// <summary>
    /// Counts for each dimension.
    /// </summary>
    int[] DimSizes { get; }
    /// <summary>
    /// Function to initialize each pixel. Inputs are the coordinate of the pixel whose color is being polled. 
    /// Can be null if initialization is not desired.
    /// </summary>
    Func<IntBl[], ColorBl> Init { get; set; }
    /// <summary>
    /// Format of user-defined texture. 
    /// </summary>
    TextureFormat Format { get; set; }
  }
  /// <summary>
  /// User-defined texture.
  /// </summary>
  /// <typeparam name="DIM">Dimension of texture; e.g., D2 for 2D texture.</typeparam>
  public partial class UserTexture<DIM> : Texture<DIM>, IUserTexture where DIM : DNum<DIM> {
    /// <summary>
    /// Counts for each dimension.
    /// </summary>
    public int[] DimSizes { get; private set; }
    /// <summary>
    /// Format of user-defined texture. 
    /// </summary>
    public TextureFormat Format { get; set; }
    /// <summary>
    /// Initialize each pixel of the user defined texture.
    /// </summary>
    public Func<IntBl[], ColorBl> Init { get; set; }
    public UserTexture(params int[] Dims) {
      (DNum<DIM>.Count == Dims.Length).Assert();
      this.DimSizes = Dims;
      Format = new TextureFormat.Typical();
    }
  }
  /// <summary>
  /// Interface for texture used as render target.
  /// </summary>
  public interface IRenderTargetTexture : ITexture {
    /// <summary>
    /// Format of texture.
    /// </summary>
    TextureFormat Format { get; set; }
    /// <summary>
    /// Convert texture into render target.
    /// </summary>
    RenderTarget AsRenderTarget { get; }
    /// <summary>
    /// Size of each dimension
    /// </summary>
    int[] DimSizes { get; }
  }
  /// <summary>
  /// Texture used as render target.
  /// </summary>
  /// <typeparam name="DIM">Number of dimensions for texture; e.g., D2 for 2 dimensions.</typeparam>
  public partial class RenderTargetTexture<DIM> : Texture<DIM>, IRenderTargetTexture where DIM : DNum<DIM> {
    public int[] DimSizes { get; private set; }
    /// <summary>
    /// Format of texture.
    /// </summary>
    public TextureFormat Format { get; set; }
    public RenderTargetTexture(params int[] DimSizes) {
      (DNum<DIM>.Count == DimSizes.Length).Assert();
      this.DimSizes = DimSizes;
      Format = new TextureFormat.Typical();
    }
    public RenderTarget AsRenderTarget { get { return new Target() { Texture = this }; } }
    public static implicit operator DX.RenderTarget(RenderTargetTexture<DIM> Tex) { return Tex.AsRenderTarget; }
    public class Target : DX.RenderTarget {
      public RenderTargetTexture<DIM> Texture { get; internal set; }
      public override int GetHashCode() { return Texture.GetHashCode(); }
      public override bool Equals(object obj) {
        return obj is Target && ((Target)obj).Texture.Equals(Texture);
      }
    }
  }
  /// <summary>
  /// Brand around render targets.
  /// </summary>
  public class RenderTargetBl : Brand<IRenderTargetMarker, RenderTargetBl> {
    public RenderTargetBl(Expr<RenderTarget> Underlying) : base(Ops.UpCastOperator<RenderTarget, IRenderTargetMarker>.Instance.Make(Underlying)) { }
    private RenderTargetBl(Expr<IRenderTargetMarker> Underlying) : base(Underlying) { }
    public static implicit operator RenderTargetBl(Expr<RenderTarget> e) { return new RenderTargetBl(e); }
    public static implicit operator RenderTargetBl(RenderTarget t) { return new Constant<RenderTarget>(t); }
    static RenderTargetBl() {
      ToBrand = (b) => new RenderTargetBl(b);
    }
  }
  /// <summary>
  /// interface for brands around stream out buffers.
  /// </summary>
  public interface IStreamOutBufferBl : IBrand {}
  /// <summary>
  /// Brand for stream out buffers.
  /// </summary>
  /// <typeparam name="INPUT">Signature type of stream out buffer.</typeparam>
  /// <typeparam name="KIND">Topology of stream out buffer; i.e., triangles, points or lines.</typeparam>
  public class StreamOutBufferBl<INPUT, KIND> : Brand<IStreamOutBufferMarker, StreamOutBufferBl<INPUT, KIND>>, IStreamOutBufferBl
    where INPUT : ISignature
    where KIND : Topology.Kind<KIND>, new() {
    public StreamOutBufferBl(Expr<StreamOutBuffer<INPUT, KIND>> Underlying) : this(Ops.UpCastOperator<StreamOutBuffer<INPUT, KIND>, IStreamOutBufferMarker>.Instance.Make(Underlying)) { }
    public StreamOutBufferBl(Expr<IStreamOutBufferMarker> Underlying) : base(Underlying) { }
    public static implicit operator StreamOutBufferBl<INPUT, KIND>(Expr<StreamOutBuffer<INPUT, KIND>> e) { return new StreamOutBufferBl<INPUT, KIND>(e); }
    public static implicit operator StreamOutBufferBl<INPUT, KIND>(StreamOutBuffer<INPUT, KIND> t) { return new Constant<StreamOutBuffer<INPUT, KIND>>(t); }
    static StreamOutBufferBl() {
      ToBrand = (b) => new StreamOutBufferBl<INPUT,KIND>(b);
    }
  }
  /// <summary>
  /// Brand for textures.
  /// </summary>
  public abstract class TextureBl<DIM,PT,POINT,TEX,BRAND> : Brand<Texture<DIM>, BRAND> where POINT : Brand<PT,POINT>, IDoubleBl<POINT> where TEX : BaseTex<POINT,TEX> where BRAND : TextureBl<DIM,PT,POINT,TEX,BRAND> where DIM : DNum<DIM> {
    public TextureBl(Expr<Texture<DIM>> Underlying) : base(Underlying) { }
    /// <summary>
    /// Convert to acessible texture using default sample configuratoin.
    /// </summary>
    public static implicit operator TEX(TextureBl<DIM,PT, POINT, TEX, BRAND> Texture) {
      return Texture.Sample(new Sample.State()); // default.
    }
    /// <summary>
    /// Convert to accessible texture based on specified sample state.
    /// </summary>
    public TEX Sample(Sample.State state) {
      return BaseTex<POINT, TEX>.ToBrand(p => new Sample.Op<PT, DIM>(state, -1).Make(Underlying, p.Underlying));
    }
    /// <summary>
    /// Convert to accessible texture based on specified sample state and level.
    /// </summary>
    public TEX SampleLevel(Sample.State state, int Level) {
      (Level >= 0).Assert();
      return BaseTex<POINT, TEX>.ToBrand(p => new Sample.Op<PT,DIM>(state, Level).Make(Underlying, p.Underlying));
    }
    /// <summary>
    /// Access texture according to default sample state.
    /// </summary>
    public ColorBl this[POINT p] { get { return ((TEX)this)[p]; } }
    /// <summary>
    /// Access texture according at specified sample state.
    /// </summary>
    public ColorBl this[POINT p, Sample.State state] { get { return Sample(state)[p]; } }
  }
  /// <summary>
  /// Brand for 2D textures.
  /// </summary>
  public class TextureBl : TextureBl<D2,Vecs.Vec2<double>, PointBl, Tex2D, TextureBl> {
    public TextureBl(Expr<Texture<D2>> Underlying) : base(Underlying) { }
    public static implicit operator TextureBl(Expr<Texture<D2>> e) { return new TextureBl(e); }
    public static implicit operator TextureBl(Texture<D2> t) { return new Constant<Texture<D2>>(t); }
    static TextureBl() { Register(v => v); }
  }
  /// <summary>
  /// Brand for 1D textures.
  /// </summary>
  public class Texture1DBl : TextureBl<D1, double, DoubleBl, Tex1D, Texture1DBl> {
    public Texture1DBl(Expr<Texture<D1>> Underlying) : base(Underlying) { }
    public static implicit operator Texture1DBl(Expr<Texture<D1>> e) { return new Texture1DBl(e); }
    public static implicit operator Texture1DBl(Texture<D1> t) { return new Constant<Texture<D1>>(t); }
    static Texture1DBl() { Register(v => v); }
  }
  /// <summary>
  /// Brand for 3D textures.
  /// </summary>
  public class Texture3DBl : TextureBl<D3,Vecs.Vec3<double>, Point3DBl, Tex3D, Texture3DBl> {
    public Texture3DBl(Expr<Texture<D3>> Underlying) : base(Underlying) { }
    public static implicit operator Texture3DBl(Expr<Texture<D3>> e) { return new Texture3DBl(e); }
    public static implicit operator Texture3DBl(Texture<D3> t) { return new Constant<Texture<D3>>(t); }
    static Texture3DBl() { Register(v => v); }
  }
}
namespace Bling.DX {
  /// <summary>
  /// Base FX-style effect.
  /// </summary>
  public abstract partial class Effect {
    internal readonly List<Technique> Techniques = new List<Technique>();
    internal readonly List<IShader> Shaders = new List<IShader>();
    /// <summary>
    /// A technique inside an effect.
    /// </summary>
    public abstract partial class Technique {
      public readonly int Index;
      public Technique(Effect Effect) {
        this.Index = Effect.Techniques.Count;
        Effect.Techniques.Add(this);
      }
      public Effect Effect { get { return BaseEffect; } }
      protected abstract Effect BaseEffect { get; }

      public string Name { get { return "Technique" + Index; } }
      public abstract Pass[] BasePasses { get; }
      /// <summary>
      /// Does this technique stream out or not?
      /// </summary>
      public virtual bool IsStreamOut { get { return false; } }
    }
    /// <summary>
    /// A pass within an effect.
    /// </summary>
    public abstract partial class Pass {
      public abstract IVertexShader BaseVertexShader { get; }
      public abstract IPixelShader BasePixelShader { get; }
      public abstract IGeometryShader BaseGeometryShader { get; }
      public abstract Technique BaseTechnique { get; }
      private Blend.Set Blend0 = null;
      /// <summary>
      /// Blending used for this pass.
      /// </summary>
      public Blend.Set Blend {
        get {
          if (Blend0 == null) Blend0 = new Blend.Set();
          return Blend0;
        }
        set { Blend0 = value; }
      }
      private DepthStencil.Set DepthStencil0 = null;
      private Rasterizer.State Rasterizer0 = null;
      /// <summary>
      /// Depth stencil used for this pass.
      /// </summary>
      public DepthStencil.Set DepthStencil {
        get {
          if (DepthStencil0 == null) DepthStencil0 = new DepthStencil.Set();
          return DepthStencil0;
        }
        set { DepthStencil0 = value; }
      }
      /// <summary>
      /// Rasterizer state used for this pass.
      /// </summary>
      public Rasterizer.State Rasterizer {
        get {
          if (Rasterizer0 == null) Rasterizer0 = new Rasterizer.State();
          return Rasterizer0;
        }
        set { Rasterizer0 = value; }
      }
      public abstract string Name { get; }
    }
    /// <summary>
    /// Core shader interface.
    /// </summary>
    public interface IShader {
      string Name { get; }
    }
    /// <summary>
    /// Core shader interface with specific input signature type.
    /// </summary>
    public interface IShader<INPUT> : IShader where INPUT : ISignature { }
    /// <summary>
    /// Base class to represent pixel, vertex, and geometry shaders.
    /// </summary>
    public abstract partial class Shader : IShader {
      public readonly Effect Effect;
      public Shader(Effect Effect) { Index = Effect.Shaders.Count; Effect.Shaders.Add(this); this.Effect = Effect; }
      public string Name { get { return Prefix + Index.ToString(); } }
      public abstract string Prefix { get; }
      public readonly int Index;
    }
    public interface IVertexShader : IShader { }
    public abstract partial class VertexShader : Shader, IVertexShader {
      protected VertexShader(Effect Effect) : base(Effect) { }
      public override string Prefix { get { return "Vertex"; } }
    }
    public abstract partial class VertexShader<INPUT, OUTPUT> : VertexShader, IShader<INPUT>, IVertexShader
      where INPUT : ISignature
      where OUTPUT : ISignature {
      protected VertexShader(Effect Effect) : base(Effect) { }
    }
    public interface IPixelShader : IShader { }
    public abstract partial class PixelShader : Shader, IPixelShader {
      protected PixelShader(Effect Effect) : base(Effect) { }
      public override string Prefix { get { return "Pixel"; } }
    }
    public abstract partial class PixelShader<INPUT> : PixelShader, IShader<INPUT>, IPixelShader where INPUT : ISignature {
      protected PixelShader(Effect Effect) : base(Effect) { }
    }
    public interface IGeometryShader : IShader { }
    public abstract partial class GeometryShader : Shader, IGeometryShader {
      protected GeometryShader(Effect Effect) : base(Effect) { }
      public override string Prefix { get { return "Geom"; } }
      public abstract Func<ISignatureManager2, Func<IntBl, ISignatureManager2>, IGeometry, IGeometryStream> BaseF { get; }
    }
    public abstract class GeometryShader<INPUT, OUTPUT, KIN, KOUT> : GeometryShader, IShader<INPUT>, IGeometryShader
      where INPUT : ISignature
      where OUTPUT : ISignature where KIN : Topology.Kind<KIN>, new() where KOUT : Topology.Kind<KOUT>, new() {
      protected GeometryShader(Effect Effect) : base(Effect) { }
    }
    public interface IVertexInput {
      /// <summary>
      /// Index of current vertex.
      /// </summary>
      IntBl VertexId { get; }
      /// <summary>
      /// Index of current instance.
      /// </summary>
      IntBl InstanceId { get; }
    }
    public interface IPixelResult {
      /// <summary>
      /// Result color for pixel.
      /// </summary>
      ColorBl Color { get; set; }
      /// <summary>
      /// Result depth for pixel if depth buffer is used.
      /// </summary>
      DoubleBl Depth { get; set; }
    }
    public interface IPixel : IPixelResult {
      /// <summary>
      /// Index for primitive that contains pixel being shaded.
      /// </summary>
      IntBl PrimitiveId { get; }
      /// <summary>
      /// Index for instance that contains pixel being shaded.
      /// </summary>
      IntBl InstanceId { get; }
      /// <summary>
      /// Position set in vertex shader.
      /// </summary>
      Point4DBl Position { get; }
    }
    public interface IGeometryStream {
      /// <summary>
      /// Number of vertices that stream needs to hold.
      /// </summary>
      int CountVertices();
      void Output(System.IO.StringWriter Out, UseEval Eval);
    }
    public class GeometryStream<OUTPUT, KOUT> : DX.AppendStream<GeometryStream<OUTPUT,KOUT>, OUTPUT>, IGeometryStream
      where OUTPUT : ISignature
      where KOUT : Topology.Kind<KOUT>, new() {
      public class RestartStripN : HasPrevious {}
      /// <summary>
      /// Number of vertices that stream needs to hold, computed automatically.
      /// </summary>
      public int CountVertices() { return CountVertices(Node0); }
      private static int CountVertices(Node Node) {
        if (Node is Empty) return 0;
        if (Node is ChooseN)
          return Math.Max(CountVertices(((ChooseN)Node).IfTrue), CountVertices(((ChooseN)Node).IfFalse));
        if (Node is HasPrevious) {
          var n = CountVertices(((HasPrevious)Node).Prev);
          if (Node is AppendN) return n + 1;
          else if (Node is LoopN) {
            return n + ((LoopN)Node).Max * CountVertices(((LoopN)Node).Body(0));
          } else if (Node is TableN) {
            int m = 0;
            foreach (var Op in ((TableN)Node).Nodes) {
              m = Math.Max(m, CountVertices(Op));
            }
            return n + m;
          } else if (Node is RestartStripN)
            return n;
        }
        throw new NotSupportedException();
      }
      public void Output(System.IO.StringWriter Out, UseEval Eval) {
        PushWrite("  ", Node0, Out, Eval);
      }
      internal static void PushWrite(string Depth, Node To, System.IO.StringWriter Out, UseEval Eval)  {
        Eval.PushScope();
        var TempOut = new System.IO.StringWriter();
        Output(Depth, To, TempOut, Eval);
        var temps = Eval.PopScope();
        foreach (var t in temps) {
          Eval.Flatten(Depth, t.Code, Out);
          Out.WriteLine(Depth + Eval.TypeNameFor(((UseEval.MyInfo)t.AsTemp).TypeOfT) + " " +
            ((UseEval.MyInfo)t.AsTemp).Value + " = " + ((UseEval.MyInfo)t.Code).Value + ";");
        }
        Out.Write(TempOut.ToString());
      }
      internal static void Output(string Depth, Node Node, System.IO.StringWriter Out, UseEval Eval) {
        if (Node is Empty) return;
        if (Node is ChooseN) {
          var Node0 = (ChooseN)Node;
          var test = Eval.DoEval1(Node0.Test).Value;
          Out.WriteLine(Depth + "if (" + test + ") {");
          PushWrite(Depth + "  ", Node0.IfTrue, Out, Eval);
          if (Node0.IfFalse == EMPTY) {
            Out.WriteLine(Depth + "}");
          } else {
            Out.WriteLine(Depth + "} else {");
            PushWrite(Depth + "  ", Node0.IfFalse, Out, Eval);
            Out.WriteLine(Depth + "}");
          }
          return;
        }
        if (Node is TableN) {
          var Node0 = (TableN)Node;
          var index = Eval.DoEval1(Node0.Index).Value;
          Out.WriteLine(Depth + "switch(" + index + ") {");
          for (int i = 0; i < Node0.Nodes.Length; i++) {
            Out.WriteLine(Depth + "  case " + i + ": { ");
            PushWrite(Depth + "    ", Node0.Nodes[i], Out, Eval);
            Out.WriteLine(Depth + "  } break;");
          }
          Out.WriteLine("}");
          return;
        }
        if (Node is HasPrevious) {
          Output(Depth, ((HasPrevious)Node).Prev, Out, Eval);
          if (Node is LoopN) {
            var Node0 = (LoopN)Node;
            var LoopName = "I" + Node0.GetHashCode();
            var loopVar = new FixedExpr<int>(LoopName);
            var body = Node0.Body(loopVar);
            var limit = Eval.DoEval1(Node0.Limit).Value;
            Out.WriteLine(Depth + "for (int " + LoopName + " = 0; " + LoopName + " < " + limit + "; " + LoopName + "++) {");
            PushWrite(Depth + "  ", body, Out, Eval);
            Out.WriteLine(Depth + "}");
            return;
          } else if (Node is AppendN) {
            var Node0 = (AppendN)Node;
            foreach (var p in typeof(OUTPUT).GetAllProperties()) {
              var e = ((Brand) p.GetAccessors()[0].Invoke(Node0.Element, new object[0])).Underlying;
              var v = Eval.DoEval1(e);
              Eval.Flatten(Depth, v, Out);
              Out.WriteLine(Depth + "element." + p.Name + " = " + v.Value + ";");
            }
            Out.WriteLine(Depth + "stream.Append(element);");
            return;
          } else if (Node is RestartStripN) {
            var Node0 = (RestartStripN)Node;
            Out.WriteLine(Depth + "stream.RestartStrip();");
            return;
          }
        }
        throw new NotSupportedException();
      }
      internal GeometryStream<OUTPUT, KOUT> RestartStrip() {
        return Construct(new RestartStripN() { Prev = Node0 });
      }
      internal GeometryStream<OUTPUT, KOUT> RestartStrip(BoolBl Guard) {
        return RestartStrip().Choose(Guard, this);
      }
      public GeometryStream(Node Node0) : base(Node0) { }
      public GeometryStream() : this(AppendStream<GeometryStream<OUTPUT,KOUT>,OUTPUT>.EMPTY) { }
    }

    public interface IGeometry {
      IntBl PrimitiveId { get; }
      IntBl InstanceId { get; }
    }
  }
  public partial class Effect<UNIFORMS> : Effect where UNIFORMS : ISignature {
    public new abstract partial class Technique : Effect.Technique {
      public new readonly Effect<UNIFORMS> Effect;
      public Technique(Effect<UNIFORMS> Effect) : base(Effect) {
        this.Effect = Effect;
      }
      protected override Effect BaseEffect { get { return Effect; } }

    }
    public partial class Technique<INPUT> : Technique where INPUT : ISignature {
      public readonly List<Pass<INPUT>> Passes = new List<Pass<INPUT>>();
      public Technique(Effect<UNIFORMS> Effect) : base(Effect) {}
      public override Pass[] BasePasses { get { return Passes.ToArray(); } }
      /*
      public override List<SignatureEntry> InputProperties {
        get { return ShaderSigs.Decode(typeof(INPUT)); }
      }*/
    }
    public partial class Technique<INPUT, KIN> : Technique<INPUT>
      where INPUT : ISignature
      where KIN : Topology.Kind<KIN>, new() {
      public Technique(Effect<UNIFORMS> Effect) : base(Effect) { }
      public override bool IsStreamOut {
        get { return true; }
      }
    }
    // geometry shaders + stream out. 
    public partial class Technique<INPUT, GOUT, KIN, KOUT> : Technique<INPUT, KIN>
      where INPUT : ISignature
      where GOUT : ISignature
      where KIN : Topology.Kind<KIN>, new() // for stream out.
      where KOUT : Topology.Kind<KOUT>, new() {
      public Technique(Effect<UNIFORMS> Effect) : base(Effect) { }
      public bool DoStreamOut = false;
      public override bool IsStreamOut {
        get {
          return DoStreamOut;
        }
      }
    }
    public abstract partial class Pass<INPUT> : Pass where INPUT : ISignature {
      public readonly Technique<INPUT> Technique;
      public readonly int Index;
      public override Effect.Technique BaseTechnique {
        get { return Technique; }
      }
      public Pass(Technique<INPUT> Technique) {
        this.Technique = Technique;
        Index = Technique.Passes.Count;
        Technique.Passes.Add(this);
      }
      public override string Name { get { return "p" + Index; } }
    }
    public abstract class BasePass<INPUT> : Pass<INPUT>
      where INPUT : ISignature {
      public BasePass(Technique<INPUT> Technique) : base(Technique) {}
    }
    public partial class Pass<INPUT, OUTPUT> : BasePass<INPUT>
      where INPUT : ISignature
      where OUTPUT : ISignature, IPixelInput {
      public readonly VertexShader<INPUT, OUTPUT> VertexShader;
      public readonly PixelShader<OUTPUT> PixelShader;
      public Pass(Technique<INPUT> Technique, VertexShader<INPUT, OUTPUT> VertexShader, PixelShader<OUTPUT> PixelShader) : base(Technique) {
        this.VertexShader = VertexShader;
        this.PixelShader = PixelShader;
      }
      public override IVertexShader BaseVertexShader { get { return VertexShader; } }
      public override IPixelShader BasePixelShader { get { return PixelShader; } }
      public override IGeometryShader BaseGeometryShader { get { return null; } }
    }
    public abstract class BasePass<INPUT, KIN> : Pass<INPUT>
      where INPUT : ISignature
      where KIN : Topology.Kind<KIN>, new() {
      public BasePass(Technique<INPUT, KIN> Technique) : base(Technique) { }
    }
    public partial class Pass<INPUT, GIN, GOUT, KIN, KOUT> : BasePass<INPUT, KIN>
      where INPUT : ISignature
      where GIN : ISignature
      where GOUT : ISignature, IPixelInput
      where KIN : Topology.Kind<KIN>, new()
      where KOUT : Topology.Kind<KOUT>, new() {
      public readonly VertexShader<INPUT, GIN> VertexShader;
      public readonly GeometryShader<GIN, GOUT, KIN, KOUT> GeometryShader;
      public readonly PixelShader<GOUT> PixelShader;
      public Pass(Technique<INPUT, KIN> Technique, VertexShader<INPUT, GIN> VertexShader,
                  GeometryShader<GIN, GOUT, KIN, KOUT> GeometryShader, PixelShader<GOUT> PixelShader)
        : base(Technique) {
        this.VertexShader = VertexShader;
        this.PixelShader = PixelShader;
        this.GeometryShader = GeometryShader;
      }
      public Pass(Technique<INPUT, GOUT, KIN, KOUT> Technique, VertexShader<INPUT, GIN> VertexShader,
                  GeometryShader<GIN, GOUT, KIN, KOUT> GeometryShader, PixelShader<GOUT> PixelShader)
        : base(Technique) {
        this.VertexShader = VertexShader;
        this.PixelShader = PixelShader;
        this.GeometryShader = GeometryShader;
      }
      public override IVertexShader BaseVertexShader { get { return VertexShader; } }
      public override IPixelShader BasePixelShader { get { return PixelShader; } }
      public override IGeometryShader BaseGeometryShader { get { return GeometryShader; } }
    }
    public partial class PassNoPS<INPUT, GIN, GOUT, KIN, KOUT> : BasePass<INPUT, KIN>
      where INPUT : ISignature
      where GIN : ISignature
      where GOUT : ISignature
      where KIN : Topology.Kind<KIN>, new()
      where KOUT : Topology.Kind<KOUT>, new() {
      public readonly VertexShader<INPUT, GIN> VertexShader;
      public readonly GeometryShader<GIN, GOUT, KIN, KOUT> GeometryShader;
      public PassNoPS(Technique<INPUT, KIN> Technique, VertexShader<INPUT, GIN> VertexShader,
                  GeometryShader<GIN, GOUT, KIN, KOUT> GeometryShader)
        : base(Technique) {
        this.VertexShader = VertexShader;
        this.GeometryShader = GeometryShader;
      }
      public override IVertexShader BaseVertexShader { get { return VertexShader; } }
      public override IPixelShader BasePixelShader { get { return null; } }
      public override IGeometryShader BaseGeometryShader { get { return GeometryShader; } }
    }
    public Technique<INPUT> Make<INPUT, OUTPUT>(Action<UNIFORMS, INPUT, OUTPUT, IVertexInput> V,
                                               Action<UNIFORMS, OUTPUT, IPixel> P, Action<Pass<INPUT>> PPass) where INPUT : ISignature where OUTPUT : ISignature, IPixelInput {
      return MakeTechnique(new VertexShader<INPUT, OUTPUT>(this, V), P, PPass);
    }
    public Technique<INPUT> MakeTechnique<INPUT, OUTPUT>(VertexShader<INPUT, OUTPUT> Shader, Action<UNIFORMS, OUTPUT, IPixel> F, Action<Pass<INPUT>> PPass)
      where INPUT : ISignature
      where OUTPUT : ISignature, IPixelInput {
      var ret = new Technique<INPUT>((Effect<UNIFORMS>) Shader.Effect);
      PPass(new Pass<INPUT, OUTPUT>(ret, Shader, new PixelShader<OUTPUT>((Effect<UNIFORMS>) Shader.Effect, F)));
      return ret;
    }
    public new partial class VertexShader<INPUT, OUTPUT> : Effect.VertexShader<INPUT, OUTPUT>
      where INPUT : ISignature
      where OUTPUT : ISignature {
      public readonly Action<UNIFORMS, INPUT, OUTPUT, IVertexInput> F;

      public Action<ISignatureManager2, ISignatureManager2, ISignatureManager2, IVertexInput> BaseF {
        get { return (x, y, z, vertex) => F(
          ShaderSigs.Generate<UNIFORMS>(x),
          ShaderSigs.Generate<INPUT>(y),
          ShaderSigs.Generate<OUTPUT>(z), vertex); }
      }
      public VertexShader(Effect<UNIFORMS> Effect, Action<UNIFORMS, INPUT, OUTPUT, IVertexInput> F) : base(Effect) { this.F = F; }
    }
    public Technique<INPUT,GOUT,KIN,KOUT> Make<INPUT, GIN, GOUT, OUTPUT, KIN, KOUT>(
      Action<UNIFORMS, INPUT, GIN, IVertexInput> V,
      Func<UNIFORMS, Func<IntBl, GIN>, IGeometry, GeometryStream<GOUT,KOUT>> G,
      Action<UNIFORMS, GOUT, IPixel> P)
      where INPUT : ISignature
      where GIN : ISignature
      where GOUT : ISignature, IPixelInput
      where OUTPUT : ISignature
      where KIN : Topology.Kind<KIN>, new()
      where KOUT : Topology.Kind<KOUT>, new() {
      var vs = new VertexShader<INPUT, GIN>(this, V);
      var gs = new GeometryShader<GIN, GOUT, KIN, KOUT>(this, G);
      var ps = P == null ? null : new PixelShader<GOUT>(this, P);
      var ret = new Technique<INPUT, GOUT, KIN, KOUT>(this);
      new Pass<INPUT, GIN, GOUT, KIN, KOUT>(ret, vs, gs, ps);
      return ret;
    }
    public Technique<INPUT, GOUT, KIN, KOUT> Make<INPUT, GIN, GOUT, OUTPUT, KIN, KOUT>(
      Action<UNIFORMS, INPUT, GIN, IVertexInput> V,
      Func<UNIFORMS, Func<IntBl, GIN>, IGeometry, GeometryStream<GOUT, KOUT>> G)
      where INPUT : ISignature
      where GIN : ISignature
      where GOUT : ISignature, IPixelInput
      where OUTPUT : ISignature
      where KIN : Topology.Kind<KIN>, new()
      where KOUT : Topology.Kind<KOUT>, new() {
      var ret = Make<INPUT,GIN,GOUT,OUTPUT,KIN,KOUT>(V, G, (Action<UNIFORMS,GOUT,IPixel>) null);
      ret.DoStreamOut = true;
      return ret;
    }

    public new partial class GeometryShader<INPUT, OUTPUT, KIN, KOUT> : Effect.GeometryShader<INPUT, OUTPUT, KIN, KOUT>
      where INPUT : ISignature
      where OUTPUT : ISignature
      where KIN : Topology.Kind<KIN>, new()
      where KOUT : Topology.Kind<KOUT>, new() {
      public readonly Func<UNIFORMS, Func<IntBl, INPUT>, IGeometry, GeometryStream<OUTPUT,KOUT>> F;

      public override Func<ISignatureManager2, Func<IntBl,ISignatureManager2>, IGeometry, IGeometryStream> BaseF {
        get {
          return (x, y, geom) => F(
            ShaderSigs.Generate<UNIFORMS>(x),
            i => ShaderSigs.Generate<INPUT>(y(i)), geom);
        }
      }
      public GeometryShader(
        Effect<UNIFORMS> Effect,
        Func<UNIFORMS, Func<IntBl, INPUT>, IGeometry, GeometryStream<OUTPUT, KOUT>> F) : base(Effect) { this.F = F; }
    }
    public new partial class PixelShader<INPUT> : Effect.PixelShader<INPUT> where INPUT : ISignature,IPixelInput {
      public readonly Action<UNIFORMS, INPUT, IPixel> F;
      public PixelShader(Effect<UNIFORMS> Effect, Action<UNIFORMS, INPUT, IPixel> F) : base(Effect) { this.F = F; }
      public Action<ISignatureManager2, ISignatureManager2, IPixel> BaseF {
        get { return (x, y, pixel) => F(
          ShaderSigs.Generate<UNIFORMS>(x),
          ShaderSigs.Generate<INPUT>(y), pixel); }
      }
    }
  }
}


namespace Bling.DX {
  using Bling.Gen;
  using System.Reflection;
  // signatures.
  [AttributeUsage(AttributeTargets.Property)]
  public class Semantic : System.Attribute {
    public readonly string Name;
    public Semantic(string Name) { this.Name = Name; Index = 0; PerInstance = false;  }
    public int Index { set; get; }
    public bool PerInstance { set; get; }
    public override string ToString() {
      return Name + (Index == 0 ? "" : Index.ToString()) + (!PerInstance ? "" : "-inst");
    }
  }
  public struct SignatureEntry {
    public string Name;
    public Type Type;
    public Semantic Semantic;
    public bool IsInstance { get { return Semantic.PerInstance; } }
  }
  public static class Semantics {
    public class Position : Semantic { public Position() : base("POSITION") { } }
    public class Normal : Semantic { public Normal() : base("NORMAL") { } }
    public class TexCoord : Semantic { public TexCoord() : base("TEXCOORD") { } }
    public class Color : Semantic { public Color() : base("COLOR") { } }
  }
  /// <summary>
  /// Explicit input/output signature for vertex, pixel, and geometry shaders.
  /// </summary>
  public interface ISignature { }
  /// <summary>
  /// Basic input signature for pixel shader.
  /// </summary>
  public interface IPixelInput : ISignature {
    [Semantic("SV_Position")]
    Point4DBl Position { get; set; }
  }

  public class ShaderSigs : Generator<ISignatureManager2> {
    private ShaderSigs() { }
    public override string Name { get { return "SigGen"; } }

    public static SIG New<SIG>() where SIG : ISignature {
      var mgr = new DefaultSignatureManager2();
      return Generate<SIG>(mgr);
    }

    public static SIG Copy<SIG>(SIG sig) where SIG : ISignature {
      var mgr = new DefaultSignatureManager2();
      foreach (var p in typeof(SIG).GetAllProperties()) {
        mgr.Bindings[p.Name] = ((Brand)p.GetAccessors()[0].Invoke(sig, new object[] { })).Underlying;
      }
      return Generate<SIG>(mgr);
    }

    public override PropertyCode Property(PropertyInfo Property) {
      Brand.TypeOfT(Property.PropertyType);

      // want to force initialization of Property.PropertyType
      //var constructor = Property.PropertyType.GetConstructor(new Type[] { typeof(Expr<>).MakeGenericType(typeof(int)) });

      return new PropertyCode() {
        Get = mgr => mgr.Get(Property),
        Set = (mgr, value) => mgr.Set(Property, (Brand)value),
      };
    }
    public static T Generate<T>(ISignatureManager2 Mgr) {
      return Instance.Generate<T>()(Mgr);
    }
    public static List<SignatureEntry> Decode(Type Type) {
      var entries = new List<SignatureEntry>();
      foreach (var p in Type.GetAllProperties()) {
        Semantic Semantic = null;
        foreach (var a in p.GetCustomAttributes(true)) {
          if (a is Semantic) {
            Semantic = (Semantic)a; break;
          }
        }
        entries.Add(new SignatureEntry() { Name = p.Name, Type = Brand.TypeOfT(p.PropertyType), Semantic = Semantic });
      }
      return entries;
    }
    public static void StructDecl<SIG>(System.IO.StringWriter Out, string Name, Action A) where SIG : ISignature {
      StructDecl(Out, Name, Decode(typeof(SIG)), A);
    }
    public static void StructDecl(System.IO.StringWriter Out, string Name, List<SignatureEntry> Decode, Action A) {
      Out.WriteLine("struct " + Name + " {");
      foreach (var p in Decode) {
        var arity = Vecs.Arity.Find(p.Type) as Matrices.IMatrixArity;
        if (arity != null && (arity.Width == 1 || arity.Height == 1)) arity = null;
            

        var sstring = (p.Semantic.Name) + (p.Semantic.Index == 0 ? "" : p.Semantic.Index.ToString());
        Out.WriteLine("  " + (arity != null ? "row_major " : "") + Effect.TypeNameFor(p.Type) + " " + p.Name + " : " + sstring + ";");
      }
      A();
      Out.WriteLine("};");
    }



    public static readonly ShaderSigs Instance = new ShaderSigs();
  }
  public interface ISignatureManager2 {
    Brand Get(PropertyInfo Property);
    void Set(PropertyInfo Property, Brand Value);
  }
  public class DefaultSignatureManager2 : ISignatureManager2 {
    public readonly Dictionary<string, Expr> Bindings = new Dictionary<string, Expr>();
    public Brand Get(PropertyInfo Property) {
      var TypeOfT = Brand.TypeOfT(Property.PropertyType);
      Expr e;
      if (Bindings.ContainsKey(Property.Name)) e = Bindings[Property.Name];
      else e = (Expr) typeof(Constant<>).MakeGenericType(TypeOfT).GetConstructor(new Type[] {}).Invoke(new object[] {});

      var Constructor = Property.PropertyType.GetConstructor(new Type[] { typeof(Expr<>).MakeGenericType(TypeOfT) });
      return (Brand) Constructor.Invoke(new object[] { Bindings[Property.Name] });
      // return Brand.ToBrand(Bindings[Property.Name]); 
    }
    public void Set(PropertyInfo Property, Brand Value) { Bindings[Property.Name] = Value.Underlying; }
    public void Populate<T>(string Prefix) {
      Type FixedE = typeof(FixedExpr<>);
      foreach (var p in typeof(T).GetAllProperties()) {
        var BrandType = typeof(Brand<>).MakeGenericType(p.PropertyType);
        var TypeOfT = (Type) BrandType.GetProperty("TypeOfT").GetAccessors()[0].Invoke(null, new object[0] { });
        var FixedE0 = FixedE.MakeGenericType(TypeOfT);
        Expr e = (Expr) 
          FixedE0.GetConstructor(new Type[] { typeof(string) }).Invoke(new object[] { Prefix + p.Name });
        Bindings[p.Name] = (e);
      }
    }
    public static DefaultSignatureManager2 Input(string Prefix, List<SignatureEntry> Decode) {
      var mgr = new DefaultSignatureManager2();
      Type FixedE = typeof(FixedExpr<>);
      foreach (var p in Decode) {
        //var BrandType = typeof(Brand<>).MakeGenericType(p.BranType);
        var TypeOfT = p.Type; // (Type)BrandType.GetProperty("TypeOfT").GetAccessors()[0].Invoke(null, new object[0] { });
        var FixedE0 = FixedE.MakeGenericType(TypeOfT);
        Expr e = (Expr)
          FixedE0.GetConstructor(new Type[] { typeof(string) }).Invoke(new object[] { Prefix + p.Name });
        mgr.Bindings[p.Name] = (e);
      }
      return mgr;
    }
    public static T Input<T>(string Prefix) {
      var mgr = new DefaultSignatureManager2();
      mgr.Populate<T>(Prefix);
      return ShaderSigs.Generate<T>(mgr);
    }
    public T Output<T>() {
      return ShaderSigs.Generate<T>(this);
    }
  }

  public interface ISignatureManager {
    bool Bind<T>(PropertyExpr<T> Property, Expr<T> RHS);
    Eval<EVAL>.Info<T> Eval<EVAL, T>(PropertyExpr<T> Property, Eval<EVAL> txt) where EVAL : Eval<EVAL>;
  }
  public struct InputSlotDesc {
    public bool IsDynamic;
    public bool IsInstance;
    public int StepRate;
    public static readonly InputSlotDesc Default = new InputSlotDesc() {
      IsDynamic = false, IsInstance = false, StepRate = 0,
    };
  }
}
// topologies.
namespace Bling.DX {

  public abstract class Topology {
    public bool IsAdjacent = false;
    public abstract Topology.Kind Kind0 { get; }
    public abstract Organization Organization0 { get; }

    public abstract class Organization {
      public abstract string Name { get; }
      public override string ToString() { return Name; }
    }
    public abstract class Organization<ORG> : Organization where ORG : Organization<ORG>, new() { }
    public class StripOrg : Organization<StripOrg> {
      public override string Name {
        get { return "Strip"; }
      }
    }
    public class ListOrg : Organization<ListOrg> {
      public override string Name {
        get { return "List"; }
      }
    }

    public abstract class Kind {
      public abstract int Arity { get; }
      public abstract string Name { get; }
      public virtual string InName { get { return Name.ToLower(); } }
      public virtual string StreamName { get { return Name + "Stream"; } }
      public override string ToString() { return Name; }
    }
    public abstract class Kind<KIND> : Kind where KIND : Kind<KIND>, new() { }
    // doesn't support indexing!
    public class PointKind : Kind<PointKind> {
      public override int Arity { get { return 1; } }
      public override string Name { get { return "Point"; } }
    }
    public class LineKind : Kind<LineKind> {
      public override int Arity { get { return 2; } }
      public override string Name { get { return "Line"; } }
    }
    public class TriangleKind : Kind<TriangleKind> {
      public override int Arity { get { return 3; } }
      public override string Name { get { return "Triangle"; } }
    }
  }
  public class Topology<KIND, ORG> : Topology
    where KIND : Topology.Kind<KIND>, new()
    where ORG : Topology.Organization<ORG>, new() {
    public override Topology.Kind Kind0 { get { return new KIND(); } }
    public override Topology.Organization Organization0 { get { return new ORG(); } }
    public override string ToString() {
      return (new KIND()).ToString() + (new ORG()).ToString() + (IsAdjacent ? "Adj" : "");
    }
  }
  public abstract class IndexBuffer {
    public abstract int[] ToArray();
    public static implicit operator IndexBuffer(int[] Array) { return new FixedIndexBuffer(Array); }
    public static implicit operator IndexBuffer(List<int> Array) { return new FixedIndexBuffer(Array.ToArray()); }
  }
  public class FixedIndexBuffer : IndexBuffer {
    private readonly int[] Array;
    public override int[] ToArray() { return Array; }
    public FixedIndexBuffer(int[] Array) {
      this.Array = Array;
    }
  }

  public class IndexBuffer<KIND, ORG> : IndexBuffer
    where KIND : Topology.Kind<KIND>, new()
    where ORG : Topology.Organization<ORG>, new() {
    internal readonly List<int> Indices = new List<int>();
    internal void Add(uint Index) { Indices.Add(Index == uint.MaxValue ? -1 : (int) Index); }
    public override int[] ToArray() {
      return Indices.ToArray();
    }
  }
  public static class IndexBufferExtensions {
    public static Effect.GeometryStream<OUTPUT, Topology.TriangleKind> RestartStrip<OUTPUT>(this Effect.GeometryStream<OUTPUT, Topology.TriangleKind> Stream, BoolBl Guard) where OUTPUT : ISignature {
      return Stream.RestartStrip(Guard);
    }
    public static Effect.GeometryStream<OUTPUT, Topology.LineKind> RestartStrip<OUTPUT>(this Effect.GeometryStream<OUTPUT, Topology.LineKind> Stream, BoolBl Guard) where OUTPUT : ISignature {
      return Stream.RestartStrip(Guard);
    }
    public static Effect.GeometryStream<OUTPUT, Topology.TriangleKind> RestartStrip<OUTPUT>(this Effect.GeometryStream<OUTPUT, Topology.TriangleKind> Stream) where OUTPUT : ISignature {
      return Stream.RestartStrip();
    }
    public static Effect.GeometryStream<OUTPUT, Topology.LineKind> RestartStrip<OUTPUT>(this Effect.GeometryStream<OUTPUT, Topology.LineKind> Stream) where OUTPUT : ISignature {
      return Stream.RestartStrip();
    }


    public static IndexBuffer<Topology.TriangleKind, Topology.ListOrg> AddTo(this IndexBuffer<Topology.TriangleKind, Topology.ListOrg> Indices, int A, int B, int C) {
      Indices.Add((uint)A);
      Indices.Add((uint)B);
      Indices.Add((uint)C);
      return Indices;
    }
    public static IndexBuffer<Topology.LineKind, Topology.ListOrg> AddTo(this IndexBuffer<Topology.LineKind, Topology.ListOrg> Indices, int A, int B) {
      Indices.Add((uint)A);
      Indices.Add((uint)B);
      return Indices;
    }
    public static IndexBuffer<Topology.TriangleKind, Topology.StripOrg> AddTo(this IndexBuffer<Topology.TriangleKind, Topology.StripOrg> Indices, int A, int B, int C, params int[] Rest) {
      Indices.Add((uint)A);
      Indices.Add((uint)B);
      Indices.Add((uint)C);
      foreach (var r in Rest) Indices.Add((uint)r);
      Indices.Add(uint.MaxValue);
      return Indices;
    }
    public static IndexBuffer<Topology.LineKind, Topology.StripOrg> AddTo(this IndexBuffer<Topology.LineKind, Topology.StripOrg> Indices, int A, int B, params int[] Rest) {
      Indices.Add((uint)A);
      Indices.Add((uint)B);
      foreach (var r in Rest) Indices.Add((uint)r);
      Indices.Add(uint.MaxValue);
      return Indices;
    }
  }
}
namespace Bling.DX {
  using Bling.Vecs;

  public interface IState {
    int Index { get;  set; }
    void Output(System.IO.StringWriter Output);
    string Name { get; }
  }
  public interface IStateEval<EVAL> : IEval<EVAL> where EVAL : Eval<EVAL> {
    string IdFor(IState state);
  }
  public abstract class TextureFormat {
    public enum ChannelWidth {
      BYTE, WORD, HALF_WORD, 
    }
    public enum ChannelType {
      FLOAT, UINT, SINT, TYPELESS, SNORM, UNORM, UNORM_SRGB,
    }
    public abstract int WordWidth(int Size);
    public abstract void CopyTo(int Index, ColorBl From, float[] To);

    public class Typical : TextureFormat {
      public int ChannelCount = 4; // RBGA
      public ChannelWidth Width = ChannelWidth.WORD;
      public ChannelType Type = ChannelType.FLOAT;

      public override int WordWidth(int Size) {
          int n = WordWidth0;
          ((n * Size) % 4 == 0).Assert();
          return ((n * Size) / 4);
      }
      private int WordWidth0 {
        get {
          int n;
          switch (Width) {
            case ChannelWidth.BYTE: n = 1; break;
            case ChannelWidth.HALF_WORD: n = 2; break;
            case ChannelWidth.WORD: n = 4; break;
            default: throw new NotSupportedException();
          }
          n = n * ChannelCount;
          return n;
        }
      }
      public override void CopyTo(int Index, ColorBl From, float[] To) {
        var WordWidth0 = this.WordWidth0;
        var Start = Index * WordWidth0;
        (Width == ChannelWidth.WORD).Assert();


        throw new NotImplementedException();
      }


      public override string ToString() {
        var ChannelCount = this.ChannelCount;
        (ChannelCount > 0 && ChannelCount <= 4).Assert();
        if (ChannelCount == 3) ChannelCount = 4;
        // 1, 2, or 4.
        string wd;
        switch (Width) {
          case ChannelWidth.BYTE: wd = "8"; break;
          case ChannelWidth.HALF_WORD: wd = "16"; break;
          case ChannelWidth.WORD: wd = "32"; break;
          default: throw new NotSupportedException();
        }
        var ret = "";
        if (ChannelCount >= 1) ret = ret + "R" + wd;
        if (ChannelCount >= 2) ret = ret + "G" + wd;
        if (ChannelCount >= 3) ret = ret + "B" + wd;
        if (ChannelCount >= 4) ret = ret + "A" + wd;
        ret = ret + "_" + Type.ToString();
        return ret;
      }
    }
  }
  /// <summary>
  /// Comparison options for depth stencil and samplers.
  /// </summary>
  public enum ComparisonFunc {
    /// <summary>
    /// Never pass the comparison. 
    /// </summary>
    Never,
    /// <summary>
    /// If the source data is less than the destination data, the comparison passes. 
    /// </summary>
    Less,
    /// <summary>
    /// If the source data is equal to the destination data, the comparison passes. 
    /// </summary>
    Equal,
    /// <summary>
    /// If the source data is less than or equal to the destination data, the comparison passes. 
    /// </summary>
    Less_Equal,
    /// <summary>
    /// If the source data is greater than the destination data, the comparison passes. 
    /// </summary>
    Greater,
    /// <summary>
    /// If the source data is not equal to the destination data, the comparison passes. 
    /// </summary>
    Not_Equal,
    /// <summary>
    /// If the source data is greater than or equal to the destination data, the comparison passes. 
    /// </summary>
    Greater_Equal,
    /// <summary>
    /// Always pass the comparison. 
    /// </summary>
    Always,
  }
  public static class DepthStencil {
    public class Set {
      public DepthStencil.State State;
      public uint StencilRef = 0;
    }
    public class DepthSet {
      public bool Enable = true;
      public DepthWriteMask WriteMask = DepthWriteMask.All;
      public ComparisonFunc ComparisonFunc = ComparisonFunc.Less;
      public override int GetHashCode() {
        return Enable.GetHashCode() + ComparisonFunc.GetHashCode() + WriteMask.GetHashCode();
      }
      public override bool Equals(object obj) {
        return obj is DepthSet &&
          ((DepthSet)obj).Enable == Enable &&
          ((DepthSet)obj).WriteMask == WriteMask &&
          ((DepthSet)obj).ComparisonFunc == ComparisonFunc;
      }
    }
    public enum DepthWriteMask {
      Zero, All,
    }
    public class StencilSet {
      public bool Enable = false;
      public int ReadMask = 0xff;
      public int WriteMask = 0xff;
      public override int GetHashCode() {
        return Enable.GetHashCode() + ReadMask.GetHashCode() + WriteMask.GetHashCode();
      }
      public override bool Equals(object obj) {
        return obj is StencilSet &&
          ((StencilSet)obj).Enable == Enable &&
          ((StencilSet)obj).WriteMask == WriteMask &&
          ((StencilSet)obj).ReadMask == ReadMask;
      }
    }

    public class State : IState {
      public DepthSet Depth = new DepthSet();
      public StencilSet Stencil = new StencilSet();
      public int Index { set; get; }
      public string Name { get { return "Depth" + Index; } }
      public void Output(System.IO.StringWriter output) {
        output.WriteLine("DepthStencilState " + Name + " {");
        {
          var DefaultDepth = new DepthSet();
          if (Depth.Enable != DefaultDepth.Enable) {
            output.WriteLine("  DepthEnable = " + Depth.Enable.ToString().ToUpper() + ";");
          }
          if (Depth.WriteMask != DefaultDepth.WriteMask) {
            output.WriteLine("  DepthWriteMask = " + Depth.WriteMask.ToString().ToUpper() + ";");
          }
          if (Depth.ComparisonFunc != DefaultDepth.ComparisonFunc) {
            output.WriteLine("  DepthFunc = " + Depth.ComparisonFunc.ToString().ToUpper() + ";");
          }
        }
        {
          var DefaultStencil = new StencilSet();
          if (Stencil.Enable != DefaultStencil.Enable) {
            output.WriteLine("  StencilEnable = " + Stencil.Enable.ToString().ToUpper() + ";");
          }
          if (Stencil.WriteMask != DefaultStencil.WriteMask) {
            output.WriteLine("  StencilWriteMask = " + Stencil.WriteMask.ToString().ToUpper() + ";");
          }
          if (Stencil.ReadMask != DefaultStencil.ReadMask) {
            output.WriteLine("  StencilWriteMask = " + Stencil.ReadMask.ToString().ToUpper() + ";");
          }

        }
        output.WriteLine("};");

      }
    }
  }

  public static class Rasterizer {
    public enum Fill {
      Wireframe = 2, Solid = 3,  
    }
    public enum Cull {
      None = 1, Front = 2, Back = 3,
    }
    public class State : IState {
      public Fill FillMode = Fill.Solid;
      public Cull CullMode = Cull.Back;
      public bool FrontCounterClockwise = false;
      public struct DepthSet {
        public int Bias;
        public double BiasClamp;
        public bool ClipEnable;
        public double SlopScaledBias;
      }
      public bool ScissorEnable = false;
      public bool MultisampleEnable = false;
      public bool AntialiasedLineEnable = false;

      public DepthSet Depth = new DepthSet();

      public override int GetHashCode() {
        return FillMode.GetHashCode() +
          CullMode.GetHashCode() +
          FrontCounterClockwise.GetHashCode() +
          Depth.GetHashCode() +
          ScissorEnable.GetHashCode() +
          MultisampleEnable.GetHashCode() +
          AntialiasedLineEnable.GetHashCode();
      }
      public override bool Equals(object obj) {
        if (!(obj is State)) return false;
        var state = (State)obj;
        return FillMode == state.FillMode &&
          CullMode == state.CullMode &&
          FrontCounterClockwise == state.FrontCounterClockwise &&
          Depth.Equals(state.Depth) &&
          ScissorEnable == state.ScissorEnable &&
          MultisampleEnable == state.MultisampleEnable &&
          AntialiasedLineEnable == state.AntialiasedLineEnable;
      }


      public State() {
        this.Depth.Bias = 0;
        this.Depth.BiasClamp = 0.0;
        this.Depth.ClipEnable = true;
        this.Depth.SlopScaledBias = 0f;
      }


      public int Index { set; get; }
      public string Name { get { return "RS" + Index; } }
      public void Output(System.IO.StringWriter output) {
        var dflt = new State();
        output.WriteLine("RasterizerState " + Name + " {");
        if (CullMode != dflt.CullMode) 
          output.WriteLine("  CullMode = " + CullMode.ToString() + ";");
        if (FillMode != dflt.FillMode)
          output.WriteLine("  FillMode = " + FillMode.ToString() + ";");
        if (FrontCounterClockwise != dflt.FrontCounterClockwise)
          output.WriteLine("  FrontCounterClockwise = " + FrontCounterClockwise.ToString() + ";");
        if (Depth.Bias != dflt.Depth.Bias)
          output.WriteLine("  DepthBias = " + Depth.Bias.ToString() + ";");
        if (Depth.BiasClamp != dflt.Depth.BiasClamp)
          output.WriteLine("  DepthBiasClamp = " + Depth.BiasClamp.ToString() + ";");
        if (Depth.SlopScaledBias != dflt.Depth.SlopScaledBias)
          output.WriteLine("  SlopeScaledDepthBiasClamp = " + Depth.SlopScaledBias.ToString() + ";");
        if (Depth.ClipEnable != dflt.Depth.ClipEnable)
          output.WriteLine("  DepthClipEnable = " + Depth.ClipEnable.ToString() + ";");
        if (ScissorEnable != dflt.ScissorEnable)
          output.WriteLine("  ScissorEnable = " + ScissorEnable.ToString() + ";");
        if (ScissorEnable != dflt.ScissorEnable)
          output.WriteLine("  MultisampleEnable = " + MultisampleEnable.ToString() + ";");
        if (ScissorEnable != dflt.ScissorEnable)
          output.WriteLine("  AntialiasedLineEnable = " + AntialiasedLineEnable.ToString() + ";");
        output.WriteLine("};");
      }
    }

  }

  public static class Blend {
    public class Set {
      public Blend.State State;
      public Point4DBl Factor;
      public uint SampleMask = 0xffffffff;
    }
    public enum ColorWriteEnable {
      Red = 1, Green = 2, Blue = 4, Alpha = 8, All = (Red | Green | Blue | Alpha),
    }
    public class Group {
      public Option Src = Option.One;
      public Option Dest = Option.Zero;
      public Operation Op = Operation.Add;
      public override int GetHashCode() {
        return Src.GetHashCode() + Dest.GetHashCode() + Op.GetHashCode();
      }
      public override bool Equals(object obj) {
        if (obj is Group) {
          var group = (Group)obj;
          return Src == group.Src && Dest == group.Dest && Op == group.Op;
        }
        return base.Equals(obj);
      }
      public void Output(string Postfix, System.IO.StringWriter output) {
        if (Src != Option.One) {
          output.WriteLine("  SrcBlend" + Postfix + " = " + Src.ToString().ToUpper() + ";");
        }
        if (Dest != Option.Zero) {
          output.WriteLine("  DestBlend" + Postfix + " = " + Dest.ToString().ToUpper() + ";");
        }
        if (Op != Operation.Add) {
          output.WriteLine("  BlendOp" + Postfix + " = " + Op.ToString().ToUpper() + ";");
        }
      }

    }

    public class TargetEntry {
      public bool Enable = false;
      public ColorWriteEnable WriteMask = ColorWriteEnable.All;
      public override int GetHashCode() {
        return Enable.GetHashCode() + WriteMask.GetHashCode();
      }
      public override bool Equals(object obj) {
        if (obj is TargetEntry) {
          var group = (TargetEntry)obj;
          return Enable == group.Enable && WriteMask == group.WriteMask;
        }
        return base.Equals(obj);
      }
      public void Output(int Idx, System.IO.StringWriter output) {
        if (Enable != false) {
          output.WriteLine("  BlendEnable[" + Idx + "] = " + Enable.ToString().ToUpper() + ";");
        }
        WriteMask = ColorWriteEnable.Red | ColorWriteEnable.Green;
        if (WriteMask != ColorWriteEnable.All) {
          var mask = String.Format("{0:X}", (uint) WriteMask);
          while (mask.Length > 0 && mask[0] == '0') mask = mask.Substring(1);
          output.WriteLine("  RenderTargetWriteMask[" + Idx + "] = 0x" + mask + ";");
        }
      }
    }

    public class State : IState {
      public bool AlphaToCoverageEnable = false;
      public readonly TargetEntry[] Targets = new TargetEntry[8];

      public State() {
        for (int i = 0; i < Targets.Length; i++) Targets[i] = new TargetEntry();
      }

      public TargetEntry Target0 { set { Targets[0] = value; } get { return Targets[0]; } }
      public TargetEntry Target1 { set { Targets[1] = value; } get { return Targets[1]; } }
      public TargetEntry Target2 { set { Targets[2] = value; } get { return Targets[2]; } }
      public Group Core = new Group();
      public Group Alpha = new Group();

      public override int GetHashCode() {
        var hc = AlphaToCoverageEnable.GetHashCode() + Core.GetHashCode() + Alpha.GetHashCode();
        for (int i = 0; i < Targets.Length; i++) hc += Targets[i].GetHashCode();
        return hc;
      }
      public int Index { set; get; }
      public string Name { get { return "Blend" + Index; } }


      public void Output(System.IO.StringWriter output) {
        Func<object, string> SF = (obj) => obj.ToString().ToUpper();
        output.WriteLine("BlendState " + Name + " {");
        output.WriteLine("  AlphaToCoverageEnable = " + SF(AlphaToCoverageEnable) + ";");
        if (Core != new Group()) {
          Core.Output("", output);
        }
        if (Alpha != new Group()) {
          Alpha.Output("Alpha", output);
        }
        for (int i = 0; i < Targets.Length; i++) {
          var newEntry = new TargetEntry();
          if (!Targets[i].Equals(new TargetEntry())) Targets[i].Output(i, output);
        }

        output.WriteLine("};");


      }
      public override bool Equals(object obj) {
        if (obj is State) {
          var state = (State)obj;
          if (state.AlphaToCoverageEnable != this.AlphaToCoverageEnable) return false;
          if (state.Core != this.Core) return false;
          if (state.Alpha != this.Alpha) return false;
          for (int i = 0; i < Targets.Length; i++)
            if (state.Targets[i] != this.Targets[i]) return false;
          return true;
        }
        return base.Equals(obj);
      }

    }
    public abstract class Option {
      public static bool operator==(Option optionA, Option optionB) {
        return optionA.Equals(optionB);
      }
      public static bool operator !=(Option optionA, Option optionB) {
        return !optionA.Equals(optionB);
      }
      public abstract bool Equals(Option option);
      public override int GetHashCode() {
        return base.GetHashCode();
      }
      public override bool Equals(object obj) {
        return obj is Option && Equals((Option) obj);
      }
      public class BasicOption : Option {
        public readonly string Name;
        internal BasicOption(string Name) {
          this.Name = Name;
        }
        public override string ToString() {
          return Name;
        }
        public override bool Equals(Option option) {
          return Object.ReferenceEquals(option, this);
        }
      }
      public static readonly BasicOption One = new BasicOption("ONE");
      public static readonly BasicOption Zero = new BasicOption("ZERO");

    }
    public enum ColorType {
      Color, Alpha, Alpha_Sat,
    }
    public enum Source {
      Src, Src1, Dest, Blend_Factor,
    }
    public class ComplexOption : Option {
      public bool IsInv = false;
      public ColorType ColorType = ColorType.Color;
      public Source Source = Source.Src;

      public override bool Equals(Option option) {
        return option is ComplexOption &&
          ((ComplexOption)option).IsInv == IsInv &&
          ((ComplexOption)option).ColorType == ColorType &&
          ((ComplexOption)option).Source == Source;
      }
      public override int GetHashCode() {
        return IsInv.GetHashCode() + ColorType.GetHashCode() + Source.GetHashCode();
      }
      public override string ToString() {
        var str = (IsInv ? "INV_" : "") +
          Source.ToString().ToUpper();
        if (Source == Source.Blend_Factor) return str;
        str += "_" + ColorType.ToString().ToUpper();
        return str;
      }

    }

    public enum Operation {
      Add, Subtract, Rev_Subtract, Min, Max
    }
  }

  /// <summary>
  /// Static container for all things dealing with samples.
  /// </summary>
  public static partial class Sample {
    public enum Option {
      /// <summary>
      /// Use point sampling. 
      /// </summary>
      POINT, 
      /// <summary>
      /// Use linear interpolation.
      /// </summary>
      LINEAR,
    }
    /// <summary>
    /// Filtering options during texture sampling.
    /// </summary>
    public abstract class Filter {
      /// <summary>
      /// Can support comparison.
      /// </summary>
      public abstract class Comparison : Filter {
        /// <summary>
        ///  If true, compare the result to the comparison value.
        /// </summary>
        public bool IsComparison = false;
        public override int GetHashCode() { return IsComparison.GetHashCode(); }
        public override bool Equals(object obj) { return obj is Comparison && ((Comparison)obj).IsComparison == IsComparison; }
        public override string ToString() { return IsComparison ? "COMPARISON_" : ""; }
      }
      /// <summary>
      /// Supports minification, magnification, and mip-level sampling. 
      /// </summary>
      public class MinMagMip : Comparison {
        public Option Min = Option.POINT;
        public Option Mag = Option.POINT;
        public Option Mip = Option.POINT;
        public override int GetHashCode() {
          return base.GetHashCode() + Min.GetHashCode() + Mag.GetHashCode() + Mip.GetHashCode();
        }
        public override bool Equals(object obj) {
          return base.Equals(obj) && obj is MinMagMip &&
            ((MinMagMip)obj).Min == Min &&
            ((MinMagMip)obj).Mag == Mag &&
            ((MinMagMip)obj).Mip == Mip;
        }
        public override string ToString() {
          var str = base.ToString();
          var prefixes = new string[] {
            "MIN_", "MAG_", "MIP_",
          };
          var values = new Option[] {
            Min, Mag, Mip,
          };
          for (int i = 0; i < 3; i++) {
            str += prefixes[i];
            if (i == 2) str += values[i];
            else if (values[i + 1] != values[i]) str += (values[i] + "_");
          }
          return str;
        }
      }
      /// <summary>
      /// Use anisotropic interpolation for minification, magnification, and mip-level sampling. 
      /// </summary>
      public class Anistropic : Comparison {
        public override int GetHashCode() {
          return base.GetHashCode() + 10;
        }
        public override bool Equals(object obj) {
          return base.Equals(obj) && obj is Anistropic;
        }
        public override string ToString() {
          return base.ToString() + "ANISTROPIC";
        }
      }
      public class Text1Bit : Filter {
        public override int GetHashCode() {
          return GetType().GetHashCode();
        }
        public override bool Equals(object obj) {
          return obj is Text1Bit;
        }
        public override string ToString() {
          return "TEXT_1BIT";
        }
      }
    
    }
    /// <summary>
    /// Texture sample address mode options. 
    /// </summary>
    public enum AddressMode {
      /// <summary>
      /// Tile the texture at every integer junction. For example, for u values between 0 and 3, the texture is repeated three times.      
      /// </summary>
      Wrap, 
      /// <summary>
      /// Flip the texture at every integer junction. For u values between 0 and 1, for example, the texture is addressed normally; between 1 and 2, the texture is flipped (mirrored); between 2 and 3, the texture is normal again; and so on.
      /// </summary>
      Mirror, 
      /// <summary>
      /// Texture coordinates outside the range [0.0, 1.0] are set to the texture color at 0.0 or 1.0, respectively. 
      /// </summary>
      Clamp, 
      /// <summary>
      /// Texture coordinates outside the range [0.0, 1.0] are set to the border color.
      /// </summary>
      Border, 
      /// <summary>
      /// Takes the absolute value of the texture coordinate (thus, mirroring around 0), and then clamps to the maximum value. 
      /// </summary>
      MirrorOnce,
    }
    /// <summary>
    /// Describes a sampler state.
    /// </summary>
    public class State : IState {
      /// <summary> 
      /// Method to use for resolving a u texture coordinate that is outside the 0 to 1 range. Default is clamp.
      /// </summary>
      public AddressMode AddressU = AddressMode.Clamp;
      /// <summary> 
      /// Method to use for resolving a v texture coordinate that is outside the 0 to 1 range. Default is clamp. 
      /// </summary>
      public AddressMode AddressV = AddressMode.Clamp;
      /// <summary> 
      /// Method to use for resolving a w texture coordinate that is outside the 0 to 1 range. Default is clamp.
      /// </summary>
      public AddressMode AddressW = AddressMode.Clamp;
      /// <summary> 
      /// Set method to use for resolving a u, v, and w texture coordinates that are outside the 0 to 1 range. Default is clamp.
      /// </summary>
      public AddressMode Address {
        set {
          AddressU = value;
          AddressV = value;
          AddressW = value;
        }
      }
      /// <summary>
      /// Filtering method to use when sampling a texture. Default is point-based min,mag, mip sampling and no comparison.
      /// </summary>
      public Filter Filter = new Filter.MinMagMip();
      /// <summary>
      /// A function that compares sampled data against existing sampled data. Default is never.
      /// </summary>
      public ComparisonFunc ComparisonFunc = ComparisonFunc.Never;

      
      /// <summary>
      /// Border color to use if Border specified for AddressU, AddressV, or AddressW. Range must be between 0.0 and 1.0 inclusive. Default is transparent.
      /// </summary>
      public ColorBl BorderColor = new ColorBl(0, 0, 0, 0);
      /// <summary>
      /// Offset from the calculated mipmap level. For example, if Direct3D calculates that a texture should be sampled at mipmap level 3 and MipLODBias is 2, then the texture will be sampled at mipmap level 5. Default is 0.
      /// </summary>
      public DoubleBl MipMapLODBias = 0d;
      /// <summary>
      /// Lower and higher end of the mipmap range to clamp access to.
      /// For the minimum, 0 is the largest and most detailed mipmap level and any level higher than that is less detailed. 
      /// The maximum must be greater than or equal to the minimum. 
      /// To have no upper limit on LOD set this to a large value such float.MaxValue. 
      /// Default is [0d, float.Max]
      /// </summary>
      public RangeBl LOD = new RangeBl() {
        Minimum = 0d,
        Maximum = float.MaxValue,
      };
      /// <summary>
      /// Clamping value used if ANISOTROPIC filtering is specified. Valid values are between 1 and 16. Default is 16.
      /// </summary>
      public IntBl MaxAnisotropy = 16;

      /// <summary>
      /// Optional array size; a positive integer greater than or equal to 1.
      /// </summary>
      public int Index { set; get; }
      public string Name { get { return "Sample" + Index; } }
      public void Output(System.IO.StringWriter output) {
        Func<object, string> SF = obj => obj.ToString();

        

        output.WriteLine("SamplerState " + Name + " {");
        output.WriteLine("  AddressU = " + SF(AddressU) + ";");
        output.WriteLine("  AddressV = " + SF(AddressV) + ";");
        output.WriteLine("  AddressW = " + SF(AddressW) + ";");
        output.WriteLine("  Filter = " + SF(Filter) + ";");
        output.WriteLine("  ComparisonFunc = " + SF(ComparisonFunc) + ";");
        if (!BorderColor.Underlying.Equals(new ColorBl(0, 0, 0, 0).Underlying)) {
          float r, g, b, a;
          var clr = BorderColor.CurrentValue;
          r = (float)clr.X;
          g = (float)clr.Y;
          b = (float)clr.Z;
          a = (float)clr.W;
          output.WriteLine("  BorderColor = float4(" + r + "f, " + g + "f, " + b + "f, " + a + "f);");
        }
        if (!MaxAnisotropy.Underlying.Equals(16.Bl().Underlying)) {
          output.WriteLine("  MaxAnisotropy = " + MaxAnisotropy.CurrentValue + ";");
        }
        if (!LOD.Minimum.Underlying.Equals(0d.Bl().Underlying)) {
          output.WriteLine("  MinLOD = " + ((float)LOD.Minimum.CurrentValue) + "f;");
        }
        if (!LOD.Maximum.Underlying.Equals(((double)float.MaxValue).Bl().Underlying)) {
          output.WriteLine("  MaxLOD = " + ((float)LOD.Maximum.CurrentValue) + "f;");
        }
        if (!MipMapLODBias.Underlying.Equals(0d.Bl().Underlying)) {
          output.WriteLine("  MipMapLODBias = " + ((float)MipMapLODBias.CurrentValue) + "f;");
        }

        output.WriteLine("};");
      }
      public override int GetHashCode() {
        var hc = 0;
        hc += AddressU.GetHashCode();
        hc += AddressV.GetHashCode();
        hc += AddressW.GetHashCode();
        hc += Filter.GetHashCode();
        return hc;
      }
      public override bool Equals(object obj) {
        if (!(obj is State)) return base.Equals(obj);
        bool ret = true;
        var obj0 = (State)obj;
        ret = ret && AddressU.Equals(obj0.AddressU);
        ret = ret && AddressV.Equals(obj0.AddressV);
        ret = ret && AddressW.Equals(obj0.AddressW);
        ret = ret && Filter.Equals(obj0.Filter);
        return ret;
      }
      public override string ToString() {
        return "U=" + AddressU + ", V=" + AddressV + " W=" + AddressW + " F=" + Filter;
      }
    }

    internal partial class Op<PT,DIM> : Ops.BaseOperatorX<Texture<DIM>, PT, Vec4<double>> where DIM : DNum<DIM> {
      private readonly State State;
      private readonly int MipLevel;
      /// <summary>
      /// 
      /// </summary>
      /// <param name="State">Sampler state.</param>
      public Op(State State, int MipLevel) {
        this.State = State; this.MipLevel = MipLevel;
      }
      public override int GetHashCode() {
        return State.GetHashCode() + MipLevel.GetHashCode();
      }
      public override bool Equals(object obj) {
        return obj is Op<PT,DIM> && ((Op<PT,DIM>)obj).State.Equals(State) && ((Op<PT,DIM>)obj).MipLevel == MipLevel;
      }
      public override Expr<Vec4<double>> Extent(Expr<Texture<DIM>> argA, Expr<PT> argB, bool Max) {
        Point4DBl p = Max ? 1d : 0d;
        return p.Underlying;
      }

      public Op() : this(new State(), -1) {}
      public override string Format(string argA, string argB) {
        return argA + ".Sample" + (MipLevel >= 0 ? "Level" + MipLevel : "") + "(" + State + ", " + argB + ")";
      }

      private bool IsLevel { get { return MipLevel >= 0; } }
      public override Func<string, string, string> Shader<EVAL>(
        Bling.Shaders.BaseShaderEval<EVAL> txt, 
        Bling.Ops.Operation<Texture<DIM>, PT, Vec4<double>> op) {
        if (txt is IStateEval<EVAL>) {
          return (argA, argB) => argA + ".Sample" + (IsLevel ? "Level" : "") + 
            "(" + ((IStateEval<EVAL>)txt).IdFor(State) + ", " + argB + (IsLevel ? ", " + MipLevel : "") + ")";
        } else return null;
      }
    }
  }
}

namespace Bling.DX {
  using System.IO;
  using Bling.Shaders;
  // generating code for an effect.
  public abstract partial class Effect {
    internal static string TypeNameFor(Type type) {
      if (type == typeof(Texture<D1>)) return "Texture1D";
      else if (type == typeof(Texture<D2>)) return "Texture2D";
      else if (type == typeof(Texture<D3>)) return "Texture3D";
      else return BaseShaderEval.TypeNameFor(type, true);
    }
    public abstract partial class Pass {
      public void Output(StringWriter Before, StringWriter Out, Dictionary<object, object> Processed) {
        var Eval = new UseEval() { Processed = Processed, StateOut = Before };
        OutputCenter(Before, Processed);
        Out.WriteLine("  pass " + Name + " {");
        Out.WriteLine("    SetVertexShader(" + this.BaseVertexShader.Name + ");");
        Out.WriteLine("    SetGeometryShader(" + (this.BaseGeometryShader == null ? "NULL" :
          this.BaseGeometryShader.Name + (BaseTechnique.IsStreamOut ? "_SO" : "")) + ");");
        Out.WriteLine("    SetPixelShader(" + (this.BasePixelShader == null ? "NULL" : this.BasePixelShader.Name) + ");");
        if (Blend0 != null) {
          var mask = String.Format("0x{0:X}", Blend0.SampleMask);
          Out.WriteLine("   SetBlendState(" +
            Eval.IdFor(Blend0.State) + ", " +
            Eval.DoEval1(Blend0.Factor.Underlying).Value + ", " + mask + ");");
        }
        if (DepthStencil0 != null) {
          Out.WriteLine("   SetDepthStencilState(" + Eval.IdFor(DepthStencil0.State) + ", " + DepthStencil0.StencilRef + ");");
        }
        if (Rasterizer0 != null) {

          Out.WriteLine("   SetRasterizerState(" + Eval.IdFor(Rasterizer0) + ");");
        }
        Out.WriteLine("  }");
      }
      public abstract void OutputCenter(StringWriter Out, Dictionary<object, object> Processed);
    }
    protected abstract List<SignatureEntry> UniformDecode { get; }

    protected virtual void OutputUniforms(StringWriter Out) {
      foreach (var p in UniformDecode) Out.WriteLine(Effect.TypeNameFor(p.Type) + " " + p.Name + ";");
    }
    public virtual void Output(StringWriter Out) {
      OutputUniforms(Out);
      var Processed = new Dictionary<object, object>();
      foreach (var t in Techniques) ((Technique)t).Output(Out, Processed);
    }
    public abstract partial class Technique {
      public virtual void Output(StringWriter Out, Dictionary<object, object> Processed) {
        //var inputManager = new DefaultSignatureManager2();
        //var input = Signature<INPUT>.Make(inputManager);
        var After = new StringWriter();

        After.WriteLine("technique10 " + Name + " {");
        foreach (var p in BasePasses) {
          p.Output(Out, After, Processed);
        }
        After.WriteLine("}");
        Out.Write(After.ToString());
      }
    }
    public abstract partial class VertexShader : Shader, IVertexShader {
      protected string SIn { get { return this.Name + "_In"; } }
      protected string SOut { get { return this.Name + "_Out"; } }

      protected abstract Dictionary<string, Expr> OutputSig(StringWriter Out);

      public abstract void OutputCode(StringWriter Out, Dictionary<string, Expr> Outputs); 
      
      public virtual void Output(StringWriter Out, Dictionary<object, object> Processed) {
        if (Processed.ContainsKey(this)) return;
        Processed[this] = Processed.Count;
        //var vertex = 
        var Outputs = OutputSig(Out);
        Out.WriteLine(SOut + " " + this.Name + "_F(" + SIn + " input) {");
        Out.WriteLine("  " + SOut + " output = (" + SOut + ") 0;");
        OutputCode(Out, Outputs);
        Out.WriteLine("  return output;");
        Out.WriteLine("}");
        Out.WriteLine("VertexShader " + Name + " = CompileShader( vs_4_0, " + Name + "_F() );");
      }
    }
    public abstract partial class PixelShader : Shader, IPixelShader {
      protected string SIn { get { return this.Name + "_In"; } }
      protected abstract IPixelResult OutputSig(StringWriter Out);
      protected abstract string OutputCode(StringWriter TempOut, StringWriter StateOut, IPixelResult pixel, Dictionary<object, object> Processed);
      public virtual void Output(StringWriter Out, Dictionary<object, object> Processed) {
        if (Processed.ContainsKey(this)) return;
        Processed[this] = Processed.Count;
        var pixel = OutputSig(Out);
        var TempOut = new StringWriter();
        TempOut.WriteLine("float4 " + this.Name + "_F(" + SIn + " input) : SV_Target {");
        var ColorValue = OutputCode(TempOut, Out, pixel, Processed);
        Out.WriteLine(TempOut);

        Out.WriteLine("  return " + ColorValue + ";");
        Out.WriteLine("}");
        Out.WriteLine("PixelShader " + Name + " = CompileShader( ps_4_0, " + Name + "_F() );");
      }
    }
    public abstract partial class GeometryShader : Shader, IGeometryShader {
      protected abstract List<SignatureEntry> DecodeInput { get; }
      protected abstract List<SignatureEntry> DecodeOutput { get; }
      protected abstract Topology.Kind BaseKIn { get; } 
      protected abstract Topology.Kind BaseKOut { get; }
      internal class Geometry0 : IGeometry {
        public IntBl PrimitiveId { get { return new FixedExpr<int>("input.PrimitiveId"); } }
        public IntBl InstanceId { get { return new FixedExpr<int>("input.InstanceId"); } }
      }
      class UseManager2 : ISignatureManager2 {
        public Expr<int> Index;

        public void Set(System.Reflection.PropertyInfo Property, Brand Value) { throw new NotSupportedException(); }
        public Brand Get(System.Reflection.PropertyInfo Property) {
          var e = (Expr)typeof(FixedExpr2<>).MakeGenericType(Brand.TypeOfT(Property.PropertyType)).
            GetConstructor(new Type[] { typeof(string), typeof(Expr<int>) }).
            Invoke(new object[] { Property.Name, Index });
          return Brand.ToBrand(Property.PropertyType)(e);
        }
      }
      public virtual void Output(StringWriter Out, Dictionary<object, object> Processed) {
        var Uniforms = DefaultSignatureManager2.Input("", this.Effect.UniformDecode);
        if (Processed.ContainsKey(this)) return;
        Processed[this] = Processed.Count;
        var SIn = this.Name + "_In";
        ShaderSigs.StructDecl(Out, SIn, DecodeInput, () => {});
        var SOut = this.Name + "_Out";
        ShaderSigs.StructDecl(Out, SOut, DecodeOutput, () => {});

        var geom = new Geometry0();
        var stream = BaseF(Uniforms, i => (new UseManager2() { Index = i }), geom);
        // ok. 
        var kin = BaseKIn;
        var kout = BaseKOut;
        var vertexCount = stream.CountVertices();
        {
          var NOut = 0;
          foreach (var n in DecodeOutput) {
            NOut += Vecs.Arity.Find(n.Type).Count;
          }

          if (NOut * vertexCount > 1024) {
            vertexCount = 1024 / NOut;
          }
          if (vertexCount == 0) vertexCount = 1;

          var After = new System.IO.StringWriter();
          After.WriteLine("[maxvertexcount(" + vertexCount + ")]");
          After.WriteLine("void " + Name + "_F(" +
            kin.InName + " " + SIn + " input[" + kin.Arity + "], inout " +
            kout.StreamName + "<" + SOut + "> stream) {");
          After.WriteLine("  " + SOut + " element = (" + SOut + ") 0;");


          var eval = new UseEval() { StateOut = Out, Processed = Processed };
          stream.Output(After, eval);
          Out.Write(After.ToString());
          {
            var pop = eval.PopScope();
            (pop.Count == 0).Assert();
            (eval.IsEmpty).Assert();
          }
        }
        Out.WriteLine("}");
        Out.WriteLine("GeometryShader " + Name + " = CompileShader( gs_4_0, " + Name + "_F() );");
        Out.WriteLine("GeometryShader " + Name + "_SO = ConstructGSWithSO(compile gs_4_0 " + Name + "_F(),");
        Out.Write("  \"");
        bool IsFirst = true;
        foreach (var p in DecodeOutput) {
          if (!IsFirst) Out.Write("; ");
          IsFirst = false;
          Out.Write(p.Semantic.Name + (p.Semantic.Index > 0 ? p.Semantic.Index.ToString() : ""));
          var arity = Bling.Vecs.Arity.Find(p.Type);
          if (arity != null) {
            Out.Write(".");
            if (arity.Count >= 1) Out.Write("x");
            if (arity.Count >= 2) Out.Write("y");
            if (arity.Count >= 3) Out.Write("z");
            if (arity.Count >= 4) Out.Write("w");
            if (arity.Count > 4) {
              int x = arity.Count - 4;
              (p.Semantic.Index == 0).Assert();
              int i = 1;
              while (x > 0) {
                Out.Write("; ");
                Out.Write(p.Semantic.Name + i.ToString() + ".");
                if (x >= 1) Out.Write("x");
                if (x >= 2) Out.Write("y");
                if (x >= 3) Out.Write("z");
                if (x >= 4) Out.Write("w");
                x -= 4;
              }
            }
          }
        }
        //Out.Write(" SV_Position.xyzw");
        Out.WriteLine("\");");
      }
    }
  }
  public class UseEval : BaseShaderEval<UseEval>, IStateEval<UseEval> {
    public override MyInfo<T> NewInfo<T>(string Value) {
      return new MyInfo<T>() { Value = Value };
    }
    public override MyInfo<T> NewInfo<T>(string Value, Eval<UseEval>.Info Copy) {
      return new MyInfo<T>() { Value = Value };
    }
    public Dictionary<object, object> Processed;
    public System.IO.StringWriter StateOut;
    public override string TypeNameFor(Type type) {
      return Effect.TypeNameFor(type);
    }
    public string IdFor(IState state) {
      if (!Processed.ContainsKey(state)) {
        state.Index = Processed.Count;
        Processed[state] = state.Index;
        state.Output(StateOut);
      } else {
        state.Index = (int)Processed[state];
      }
      return state.Name;
    }
  }

  static class FixedExpr {
    public static Expr Make(Type T, string Name) {
      return (Expr) (typeof(FixedExpr<>)).MakeGenericType(T).GetConstructor(new Type[1] { typeof(string) }).Invoke(new object[1] { Name });
    }
  }
  class FixedExpr<T> : Expr<T> {
    protected readonly string Name;
    public FixedExpr(string Name) {
      this.Name = Name;
    }
    protected override Eval<EVAL>.Info<T> Eval<EVAL>(Eval<EVAL> txt) {
      if (txt is BaseShaderEval<EVAL>) return ((BaseShaderEval<EVAL>)txt).NewInfo<T>(Name);
      else if (txt is TranslateEval<EVAL>) return ((TranslateEval<EVAL>)txt).NoTranslation(this);
      throw new NotImplementedException();
    }
    protected override string ToString1() {
      return Name;
    }
    protected override Expr<T> Derivative1(Dictionary<Expr, Expr> Map) {
      return new Constant<T>(default(T));
    }
  }
  class FixedExpr2<T> : FixedExpr<T> {
    public readonly Expr<int> Index;
    public FixedExpr2(string Name, Expr<int> Index) : base(Name) { this.Index = Index; }
    protected override Eval<EVAL>.Info<T> Eval<EVAL>(Eval<EVAL> txt) {
      var Index0 = txt.DoEval(Index);
      if (txt is BaseShaderEval<EVAL>) {
        var ret = ((BaseShaderEval<EVAL>)txt).NewInfo<T>(
          "input[" + ((BaseShaderEval<EVAL>.MyInfo)Index0).Value + "]." + Name
        );
        ret.Append(Index0);
        return ret;
      }
      throw new NotImplementedException();
    }
  }
  public partial class Effect<UNIFORMS> : Effect where UNIFORMS : ISignature {
    protected override List<SignatureEntry> UniformDecode {
      get { return ShaderSigs.Decode(typeof(UNIFORMS)); }
    }
    public new abstract partial class Technique : Effect.Technique { }
    public partial class Technique<INPUT> : Technique where INPUT : ISignature { }
    public partial class Technique<INPUT, KIN> : Technique<INPUT>
      where INPUT : ISignature
      where KIN : Topology.Kind<KIN>, new() { }
    public partial class Technique<INPUT, GOUT, KIN, KOUT> : Technique<INPUT, KIN>
      where INPUT : ISignature
      where GOUT : ISignature
      where KIN : Topology.Kind<KIN>, new() // for stream out.
      where KOUT : Topology.Kind<KOUT>, new() {
    }
    public abstract partial class Pass<INPUT> : Pass where INPUT : ISignature {
      
    }
    public partial class Pass<INPUT, OUTPUT> : BasePass<INPUT>
      where INPUT : ISignature
      where OUTPUT : ISignature, IPixelInput {
      public override void OutputCenter(StringWriter Out, Dictionary<object,object> Processed) {
        VertexShader.Output(Out, Processed);
        PixelShader.Output(Out, Processed);
      }
    }
    public partial class Pass<INPUT, GIN, GOUT, KIN, KOUT> : BasePass<INPUT, KIN>
      where INPUT : ISignature
      where GIN : ISignature
      where GOUT : ISignature, IPixelInput
      where KIN : Topology.Kind<KIN>, new()
      where KOUT : Topology.Kind<KOUT>, new() {
      public override void OutputCenter(StringWriter Out, Dictionary<object,object> Processed) {
        VertexShader.Output(Out, Processed);
        GeometryShader.Output(Out, Processed);
        PixelShader.Output(Out, Processed);
      }
    }
    public partial class PassNoPS<INPUT, GIN, GOUT, KIN, KOUT> : BasePass<INPUT, KIN>
      where INPUT : ISignature
      where GIN : ISignature
      where GOUT : ISignature
      where KIN : Topology.Kind<KIN>, new()
      where KOUT : Topology.Kind<KOUT>, new() {
      public override void OutputCenter(StringWriter Out, Dictionary<object, object> Processed) {
        VertexShader.Output(Out, Processed);
        GeometryShader.Output(Out, Processed);
        Technique.IsStreamOut.Assert();
      }
    }
    public new partial class VertexShader<INPUT, OUTPUT> : Effect.VertexShader<INPUT, OUTPUT>
      where INPUT : ISignature
      where OUTPUT : ISignature {
      public new Effect<UNIFORMS> Effect { get { return (Effect<UNIFORMS>)base.Effect; } }
      protected List<SignatureEntry> DecodeInput {
        get { return ShaderSigs.Decode(typeof(INPUT)); }
      }
      protected List<SignatureEntry> DecodeOutput {
        get { return ShaderSigs.Decode(typeof(OUTPUT)); }
      }
      private class Vertex0 : IVertexInput {
        public IntBl VertexId { get { return new FixedExpr<int>("input.VertexId"); } }
        public IntBl InstanceId { get { return new FixedExpr<int>("input.InstanceId"); } }
      }

      protected override Dictionary<string, Expr> OutputSig(System.IO.StringWriter Out) {
        var Uniforms = DefaultSignatureManager2.Input("", this.Effect.UniformDecode);

        var vertex = new Vertex0();
        ShaderSigs.StructDecl(Out, SIn, DecodeInput, () => { });
        ShaderSigs.StructDecl(Out, SOut, DecodeOutput, () => { });
        var outputMgr = new DefaultSignatureManager2();
        this.BaseF(Uniforms, DefaultSignatureManager2.Input("input.", DecodeInput), outputMgr, vertex);
        var ret = new Dictionary<string, Expr>();
        foreach (var p in DecodeOutput)
          ret[p.Name] = outputMgr.Bindings[p.Name];
        return ret;
      }
      public override void OutputCode(StringWriter Out, Dictionary<string, Expr> Outputs) {
        var eval = new UseEval();
        var LastOut = new StringWriter();
        foreach (var p in Outputs) {
          var e = eval.DoEval1(p.Value);
          eval.Flatten("  ", e, LastOut);
          LastOut.WriteLine("  output." + p.Key + " = " + e.Value + ";");
        }
        var temps = eval.PopScope();
        (eval.IsEmpty).Assert();
        foreach (var t in temps) {
          eval.Flatten("  ", t.Code, Out);
          Out.WriteLine("  " + eval.TypeNameFor(((UseEval.MyInfo)t.AsTemp).TypeOfT) + " " +
            ((UseEval.MyInfo)t.AsTemp).Value + " = " + ((UseEval.MyInfo)t.Code).Value + ";");
        }
        Out.Write(LastOut.ToString());
      }

    }
    public new partial class GeometryShader<INPUT, OUTPUT, KIN, KOUT> : Effect.GeometryShader<INPUT, OUTPUT, KIN, KOUT>
      where INPUT : ISignature
      where OUTPUT : ISignature
      where KIN : Topology.Kind<KIN>, new()
      where KOUT : Topology.Kind<KOUT>, new() {

      protected override Topology.Kind BaseKIn { get { return new KIN(); } }
      protected override Topology.Kind BaseKOut { get { return new KOUT(); } }

      protected override List<SignatureEntry> DecodeInput {
        get { return ShaderSigs.Decode(typeof(INPUT)); }
      }
      protected override List<SignatureEntry> DecodeOutput {
        get { return ShaderSigs.Decode(typeof(OUTPUT)); }
      }
    }
    public new partial class PixelShader<INPUT> : Effect.PixelShader<INPUT> where INPUT : ISignature, IPixelInput {
      protected List<SignatureEntry> DecodeInput {
        get { return ShaderSigs.Decode(typeof(INPUT)); }
      }
      public new Effect<UNIFORMS> Effect { get { return (Effect<UNIFORMS>)base.Effect; } }
      private class Pixel0 : IPixel {
        public IntBl PrimitiveId { get { return new FixedExpr<int>("input.PrimitiveId"); } }
        public IntBl InstanceId { get { return new FixedExpr<int>("input.InstanceId"); } }
        public Point4DBl Position { get { return new FixedExpr<Vecs.Vec4<double>>("input.Position"); } }
        public ColorBl Color { set; get; }
        public DoubleBl Depth { set; get; }
      }
      protected override IPixelResult OutputSig(StringWriter Out) {
        var Uniforms = DefaultSignatureManager2.Input("", this.Effect.UniformDecode);
        ShaderSigs.StructDecl(Out, SIn, DecodeInput, () => { });
        var pixel = new Pixel0();
        var input = DefaultSignatureManager2.Input("input.", DecodeInput);
        BaseF(Uniforms, input, pixel);
        return pixel;
      }
      protected override string OutputCode(StringWriter TempOut, StringWriter StateOut, IPixelResult pixel, Dictionary<object, object> Processed) {
        var eval = new UseEval() { StateOut = StateOut, Processed = Processed };
        var Color0 = eval.DoEval1(pixel.Color.Underlying);
        // add any samplers.
        // now dump all the registers to Out.
        var temps = eval.PopScope();
        (eval.IsEmpty).Assert();
        foreach (var t in temps) {
          eval.Flatten("  ", t.Code, TempOut);
          TempOut.WriteLine("  " + eval.TypeNameFor(((UseEval.MyInfo)t.AsTemp).TypeOfT) + " " +
            ((UseEval.MyInfo)t.AsTemp).Value + " = " + ((UseEval.MyInfo)t.Code).Value + ";");
        }
        eval.Flatten("  ", Color0, TempOut);
        return Color0.Value;
      }
    }
  }

}

namespace Bling.DX {
  public abstract partial class AppendStream<BRAND, ELEM> : IConditionTarget<BRAND>, ICanTable<BRAND> where BRAND : AppendStream<BRAND, ELEM> {
    public abstract class Node { }
    public class Empty : Node { internal Empty() { }  }
    public static readonly Empty EMPTY = new Empty();
    public abstract class HasPrevious : Node {
      public Node Prev { get; internal set; }
    }
    public class AppendN : HasPrevious {
      public ELEM Element { get; internal set; }
    }
    public class LoopN : HasPrevious {
      public Func<Expr<int>, Node> Body { get; internal set; }
      public Expr<int> Limit { get; internal set; }
      public int Max { get { return Limit.Extent(true).CurrentValue; } }
    }
    public class ChooseN : Node {
      public Expr<bool> Test { get; internal set; }
      public Node IfTrue { get; internal set; }
      public Node IfFalse { get; internal set; }
    }
    public class TableN : HasPrevious {
      public Expr<int> Index { get; internal set; }
      public Node[] Nodes { get; internal set; }
    }

    public readonly Node Node0;
    protected static readonly Func<Node, BRAND> Construct;
    static AppendStream() {
      var method = typeof(BRAND).GetConstructor(new Type[] { typeof(Node) });
      Construct = node => (BRAND) method.Invoke(new object[] { node });
    }
    public AppendStream(Node Node0) { this.Node0 = Node0; }

    public BRAND Append(BoolBl Guard, ELEM Elem) {
      return Append(Elem).Choose(Guard, (BRAND) this);
    }
    public BRAND Append(ELEM Elem) { return Construct(new AppendN() { Prev = Node0, Element = Elem }); }

    public BRAND Choose(BoolBl Test, BRAND IfFalse) {
      return Construct(new ChooseN() {
        IfTrue = Node0, IfFalse = IfFalse.Node0, Test = Test,
      });
    }
    public BRAND DChoose(BoolBl Test, BRAND IfFalse) {
      return Construct(new ChooseN() {
        IfTrue = Node0,
        IfFalse = IfFalse.Node0,
        Test = Test,
      });
    }
    public BRAND Table(IntBl Index, params BRAND[] Options) {
      var Nodes = new Node[Options.Length];
      for (int i = 0; i < Options.Length; i++) Nodes[i] = Options[i].Node0;
      return Construct(new TableN() { Prev = Node0, Index = Index, Nodes = Nodes, });
    }
    public BRAND For(IntBl Limit, Func<IntBl, BRAND, BRAND> Body) {
      return Construct(new LoopN() { Prev = Node0, Body = i => Body(i, Construct(EMPTY)).Node0, Limit = Limit });
    }
    public BRAND For(int Limit, Func<IntBl, BRAND, BRAND> Body) {
      return For((IntBl) Limit, Body);
    }
  }
}
namespace Bling.DX {
  using System.IO;
  public partial class RenderDevice : IDisposable /*, IRenderDevice */ {

    private class Vertex : IVertex {
      public ColorBl Color { internal get; set; }

      private Point4DBl Pos0;
      public Point4DBl Position {
        internal get { return Pos0; }
        set { Pos0 = /*Semantics.Position.Instance.Transform*/(value); }
      }
    }

    internal enum InfoStatus {
      // ToVertex means that the vertex is touched
      Pixel = 0, Vertex = 1, ToVertex = 2, ToInstance = 3, Uniform = 4
    }
    internal interface IStatusEval<EVAL> where EVAL : Eval<EVAL> {
      InfoStatus StatusFor(Eval<EVAL>.Info info);
      void BringIn(Eval<EVAL>.Info Info, Expr Expr, InfoStatus At);
      // InfoStatus At { get; }
      Eval<EVAL>.Info<T> NewInfo0<T>(string value);
      void SetStatusFor(Eval<EVAL>.Info info, InfoStatus status);
      //string this[SamplerState State] { get; }
    }
    internal interface IStatusExpr : IExpr {
      InfoStatus At { get; }
    }
    internal class AllVertices : LoopIndexExpr, IStatusExpr {
      public InfoStatus At { get { return InfoStatus.ToVertex; } }
      public AllVertices(int Count) : base("I", new Constant<int>(Count)) { }
      protected override Eval<EVAL>.Info<int> Eval<EVAL>(Eval<EVAL> txt) {
        if (txt is IStatusEval<EVAL>) {
          var ret = ((IStatusEval<EVAL>)txt).NewInfo0<int>(null);
          ((IStatusEval<EVAL>)txt).SetStatusFor(ret, InfoStatus.ToVertex);
          return ret;
        }
        return base.Eval<EVAL>(txt);
      }
    }
    internal class AllInstances : LoopIndexExpr, IStatusExpr {
      public InfoStatus At { get { return InfoStatus.ToInstance; } }
      public AllInstances(int Count) : base("J", new Constant<int>(Count)) { }
      protected override Eval<EVAL>.Info<int> Eval<EVAL>(Eval<EVAL> txt) {
        if (txt is IStatusEval<EVAL>) {
          var ret = ((IStatusEval<EVAL>)txt).NewInfo0<int>(null);
          ((IStatusEval<EVAL>)txt).SetStatusFor(ret, InfoStatus.ToInstance);
          return ret;
        }
        return base.Eval<EVAL>(txt);
      }
    }
    internal abstract class BringInExpr<T> : ProxyExpr<T>, IStatusExpr {
      public abstract InfoStatus At { get; }
      public BringInExpr(Expr<T> Underlying) : base(Underlying) { }
      protected override Eval<EVAL>.Info<T> Eval<EVAL>(Eval<EVAL> txt) {
        if (txt is IStatusEval<EVAL>) {
          var txt0 = (IStatusEval<EVAL>)txt;
          var info = txt.DoEval(Underlying);
          txt0.BringIn(info, Underlying, At);
          //(info.Status == At).Assert();
          return info;
        }
        return base.Eval<EVAL>(txt);
      }
      protected override string ToString1() {
        return Underlying.ToString() + "[" + At + "]";
      }
    }

    internal class PixelExpr<T> : BringInExpr<T> {
      public override InfoStatus At {
        get { return InfoStatus.Pixel; }
      }
      public PixelExpr(Expr<T> Underlying) : base(Underlying) { }
      protected override Expr<S> Make<S>(Expr<S> e) {
        return new PixelExpr<S>(e);
      }
      protected override Eval<EVAL>.Info<T> Eval<EVAL>(Eval<EVAL> txt) {
        if (txt is ShareEval) {
          true.Assert();
        }
        return base.Eval<EVAL>(txt);
      }
    }
    internal class VertexExpr<T> : BringInExpr<T> {
      public override InfoStatus At {
        get { return InfoStatus.Vertex; }
      }
      public VertexExpr(Expr<T> Underlying) : base(Underlying) { }
      protected override Expr<S> Make<S>(Expr<S> e) {
        return new VertexExpr<S>(e);
      }
    }

    public class RenderEntryInferred : RenderEntry {
      private readonly Dictionary<SignatureEntry, Expr> Uniforms0 = new Dictionary<SignatureEntry, Expr>();
      protected override Dictionary<SignatureEntry, Expr> BaseUniforms { get { return Uniforms0; } }

      private readonly MyTechnique Technique0;
      //protected override Effect BaseEffect { get { return Technique0.Effect; } }
      // for geometry shader-capable techniques.
      protected override Effect.Technique BaseTechnique { get { return Technique0; } }
      protected override IStreamOutBufferBl BaseStreamOut { get { return null; } }

      private class MyEffect : Effect {
        // Don't need to use this.
        protected override List<SignatureEntry> UniformDecode { get { return Outer.Uniforms.Keys.ToList(); } }
        public readonly RenderEntryInferred Outer;
        internal MyEffect(RenderEntryInferred Outer) { this.Outer = Outer; }
      }
      private class MyVertexShader : Effect.VertexShader {
        internal readonly List<SignatureEntry> VertexInputs = new List<SignatureEntry>();
        internal readonly Dictionary<string, Expr> VertexOutputs = new Dictionary<string, Expr>();
        protected override Dictionary<string, Expr> OutputSig(StringWriter Out) {
          ShaderSigs.StructDecl(Out, SIn, VertexInputs, () => { });
          ShaderSigs.StructDecl(Out, SOut, PixelShader.PixelInputs, () => { });
          return VertexOutputs;
        }
        public override void OutputCode(StringWriter Out, Dictionary<string, Expr> Outputs) {
          var eval = new GeneralEval() { Processed = null, StateOut = Out };
          var Outer = ((MyEffect)this.Effect).Outer;
          eval.Init(Outer.UniformParams).Init(Outer.VertexParams);
          var LastOut = new StringWriter();
          foreach (var p in Outputs) {
            var e = eval.DoEval1(p.Value);
            eval.Flatten("  ", e, LastOut);
            LastOut.WriteLine("  output." + p.Key + " = " + e.Value + ";");
          }
          var temps = eval.PopScope();
          (temps.Count == 0).Assert();
          (eval.IsEmpty).Assert();
          eval.OutputTemps(Out);
          Out.Write(LastOut.ToString());
        }
        public MyVertexShader(MyEffect Effect, MyPixelShader PixelShader) : base(Effect) {
          this.PixelShader = PixelShader;
        }
        private readonly MyPixelShader PixelShader;
      }
      private class MyPixelShader : Effect.PixelShader {
        internal readonly List<SignatureEntry> PixelInputs = new List<SignatureEntry>();
        internal Effect.IPixelResult Result;
        protected override Effect.IPixelResult OutputSig(StringWriter Out) {
          ShaderSigs.StructDecl(Out, SIn, PixelInputs, () => { });
          return Result;
        }
        protected override string OutputCode(StringWriter TempOut, StringWriter StateOut, Effect.IPixelResult pixel, Dictionary<object, object> Processed) {
          var eval = new GeneralEval() { Processed = Processed, StateOut = StateOut };
          var Outer = ((MyEffect)this.Effect).Outer;
          eval.Init(Outer.UniformParams).Init(Outer.PixelParams);

          var Color0 = eval.DoEval1(pixel.Color.Underlying);
          // add any samplers.
          // now dump all the registers to Out.
          var temps = eval.PopScope();
          (temps.Count == 0).Assert(); // we aren't using these temps for top scope.
          (eval.IsEmpty).Assert();
          eval.OutputTemps(TempOut);
          eval.Flatten("  ", Color0, TempOut);
          return Color0.Value;
        }

        public MyPixelShader(MyEffect Effect) : base(Effect) { }
      }
      public Effect.Pass Pass {
        get { return Technique0.Pass; }
      }

      private class MyPass : Effect.Pass {
        internal readonly MyTechnique Technique0;
        internal readonly MyVertexShader VertexShader;
        internal readonly MyPixelShader PixelShader;
        public MyPass(MyTechnique Technique0) { 
          this.Technique0 = Technique0;
          PixelShader = new MyPixelShader(Technique0.Effect0);
          VertexShader = new MyVertexShader(Technique0.Effect0, PixelShader);
        }
        public override string Name { get { return "p0"; } }
        public override Effect.Technique BaseTechnique { get { return Technique0; } }
        public override Effect.IVertexShader BaseVertexShader { get { return VertexShader; } }
        public override Effect.IGeometryShader BaseGeometryShader { get { return null; } }
        public override Effect.IPixelShader BasePixelShader { get { return PixelShader; } }
        public override void OutputCenter(StringWriter Out, Dictionary<object, object> Processed) {
          ((MyVertexShader)BaseVertexShader).Output(Out, Processed);
          ((MyPixelShader)BasePixelShader).Output(Out, Processed);
        }
      }
      private class MyTechnique : Effect.Technique {
        public readonly MyPass Pass;
        public override Effect.Pass[] BasePasses { get { return new Effect.Pass[] { Pass }; } }
        protected override Effect BaseEffect { get { return Effect0; } }
        private MyTechnique(MyEffect Effect) : base(Effect) { 
          this.Effect0 = Effect;
          this.Pass = new MyPass(this);
        }
        public MyTechnique(RenderEntryInferred Entry) : this(new MyEffect(Entry)) { }
        internal readonly MyEffect Effect0;
      }
      private class MyBuffer : BaseBuffer, IUserBuffer {
        public readonly RenderEntryInferred Outer;
        public MyBuffer(RenderEntryInferred Outer) { this.Outer = Outer; }

        internal readonly List<SignatureEntry> Inputs0 = new List<SignatureEntry>();
        internal readonly Dictionary<string, Expr> InputBindings = new Dictionary<string, Expr>();

        public override List<SignatureEntry> InputSig { get { return Inputs0; } }
        public override Topology BaseTopology { get { return new Topology<Topology.TriangleKind,Topology.ListOrg>(); } }
        public int VertexCount { get; internal set; }
        public int InstanceCount { get; internal set;  }
        public Dictionary<string, Expr> BaseF(IntBl VertexId, IntBl InstanceId) {
          var replace = new ReplaceEval();
          if (((object)Outer.AllVertices0) != null) replace[Outer.AllVertices0.Underlying] = VertexId.Underlying;
          if (((object)Outer.AllInstances0) != null) replace[Outer.AllInstances0.Underlying] = InstanceId.Underlying;
          var ret = new Dictionary<string, Expr>();
          foreach (var v in InputBindings) ret[v.Key] = replace.Translate(v.Value);
          return ret; 
        }
        public IndexBuffer BaseIndices { get; internal set;  }
      }
      private class MyBufferBl : Brand<IBufferMarker, MyBufferBl>, IBaseBufferBl {
        public MyBufferBl(MyBuffer Buffer) : base(new Constant<IBufferMarker>(Buffer)) { }
      }
      private class Pixel0 : Effect.IPixelResult {
        public ColorBl Color { get; set; }
        public DoubleBl Depth { get; set; }
      }

      private readonly IntBl AllInstances0, AllVertices0;
      private readonly Dictionary<Expr, string> UniformParams = new Dictionary<Expr, string>();
      private readonly Dictionary<Expr, string> VertexParams = new Dictionary<Expr, string>();
      private readonly Dictionary<Expr, string> PixelParams = new Dictionary<Expr, string>();

      internal RenderEntryInferred(RenderDevice Outer, int VertexCount, int InstanceCount, Action<IntBl, IntBl, IVertex> F, List<int> IndexBuffer) {
        Technique0 = new MyTechnique(this);
        ColorBl Color; Point4DBl Position;
        {
          var v = new Vertex();
          var AllVertices = new AllVertices(VertexCount);
          var AllInstances = InstanceCount > 1 ? (Expr<int>)new AllInstances(InstanceCount) : new Constant<int>(0);
          AllVertices0 = AllVertices;
          AllInstances0 = InstanceCount > 1 ? ((IntBl) AllInstances) : null;
          F(AllVertices, AllInstances, v);
          Color = v.Color; Position = v.Position;
          if (((object)Color) == null || ((object)Position) == null) throw new NotSupportedException();

        }
        var share = new ShareEval();
        share.Go(Color.Underlying);
        share.Go(Position.Underlying);

        share.End();
        var color0 = share.Translate(Color);
        var pos0 = share.Translate(Position);

        if (true) {
          var output = new System.IO.StringWriter();
          share.PrintShared(output);
          Console.WriteLine(output.ToString());
          Console.WriteLine(color0);
          Console.WriteLine(pos0);
        }
        var classify = new ClassifyEval();
        { // bring in position/color.
          var pos1 = classify.DoEval(pos0.Underlying);
          classify.BringIn(pos1, pos0.Underlying, InfoStatus.Vertex);

          var color1 = classify.DoEval(color0.Underlying);
          classify.BringIn(color1, color0.Underlying, InfoStatus.Pixel);
        }
        {
          var Buffer = new MyBuffer(this) {
            VertexCount = VertexCount,
            InstanceCount = InstanceCount,
            BaseIndices = IndexBuffer,
          };
          this.Buffer = new MyBufferBl(Buffer);
          this.Target = Outer.Display;
          // define uniforms.
          foreach (var uniform in classify.Uniforms) {
            var Id = this.Uniforms.Count;
            var Name = "U" + Id;
            var SigEntry = new SignatureEntry() {
              Name = Name,
              Type = uniform.TypeOfT,
            };
            this.Uniforms0[SigEntry] = uniform;
            UniformParams[uniform] = Name;
          }
          // concerns technique inputs, buffer inputs, and vertex shader inputs
          var SemanticUses = 0;
          Func<bool,Semantic> AllocateSemantic = (PerInstance) => {
            var max = 'Z' - 'A';
            var count = SemanticUses / max + 1;
            string str = "";
            for (int i = 0; i < count; i++) {
              str += (((char)('A' + (SemanticUses % max))).ToString());
            }
            SemanticUses += 1;
            return new Semantic(str) {
              PerInstance = PerInstance,
            };
          };
          { // add vertex/instance id.
            var VertexId = new SignatureEntry() {
              Name = "vertexId",
              Type = typeof(uint),
              Semantic = new Semantic("SV_VertexID"),
            };
            Technique0.Pass.VertexShader.VertexInputs.Add(VertexId);
            VertexParams[AllVertices0.Underlying] = "input.vertexId";
          }
          if (((object)AllInstances0) != null) { // add vertex/instance id.
            var InstanceId = new SignatureEntry() {
              Name = "instanceId",
              Type = typeof(uint),
              Semantic = new Semantic("SV_InstanceID"),
            };
            Technique0.Pass.VertexShader.VertexInputs.Add(InstanceId);
            VertexParams[AllInstances0.Underlying] = "input.instanceId";
          }
          var Vertices = classify.VertexParams.ToList();
          //Vertices.Reverse();

          foreach (var vparam in Vertices) {
            var Id = Buffer.Inputs0.Count;
            var Name = "V" + Id;
            var SigEntry = new SignatureEntry() {
              Name = Name,
              Type = vparam.Key.TypeOfT,
              Semantic = AllocateSemantic(vparam.Value.Status == InfoStatus.ToInstance),
            };
            Technique0.Pass.VertexShader.VertexInputs.Add(SigEntry);
            var vparamKey = (vparam.Key);

            VertexParams[vparamKey] = "input." + Name;
            Buffer.Inputs0.Add(SigEntry);
            // don't forget to translate...vparam.Key
            Buffer.InputBindings[Name] = vparamKey;
          }
          {
            Technique0.Pass.VertexShader.VertexOutputs["Position"] = pos0.Underlying;
          }
          List<Expr> epparams = new List<Expr>();
          foreach (var pparam in classify.PixelParams) {
            var Id = Technique0.Pass.PixelShader.PixelInputs.Count;
            var Name = "P" + Id;
            Technique0.Pass.VertexShader.VertexOutputs[Name] = (pparam.Key);
            var SigEntry = new SignatureEntry() {
              Name = Name,
              Type = pparam.Key.TypeOfT,
              Semantic = AllocateSemantic(false),
            };
            Technique0.Pass.PixelShader.PixelInputs.Add(SigEntry);
            epparams.Add(pparam.Key);
          }
          // set replace after...we've translated vertex.
          for (int Id = 0; Id < Technique0.Pass.PixelShader.PixelInputs.Count; Id += 1) {
            var Name = "P" + Id;
            PixelParams[epparams[Id]] = "input." + Name;
          }
          Technique0.Pass.PixelShader.PixelInputs.Add(new SignatureEntry() {
            Name = "Position",
            Semantic = new Semantic("SV_Position"),
            Type = typeof(Vecs.Vec4<double>),
          });
          {
            var pixel = new Pixel0() {
              Color = color0,
            };
            Technique0.Pass.PixelShader.Result = pixel;
          }
        }
      }
    }


    public RenderEntryInferred Render(int Count, Action<IntBl, IVertex> F, List<int> IndexBuffer) {
      return Render(Count, 1, (vid, iid, vertex) => F(vid, vertex), IndexBuffer);
    }
    public RenderEntryInferred Render(int VertexCount, int InstanceCount, Action<IntBl, IntBl, IVertex> F, List<int> IndexBuffer) {
      var re = new RenderEntryInferred(this, VertexCount, InstanceCount, F, IndexBuffer) { };
      Entries.Add(re);
      return re;
    }
  }
  public static class DXExtensions {
    /// <summary>
    /// Render vertices based on custom vertex samples. 
    /// </summary>
    /// <param name="Samples">Specification of vertex topology.</param>
    /// <param name="F">Rendering action from a 3 argument function, (n,k,v) where n is the vertex index, k is a custom sample value, and v is the vertex result structure.</param>
    public static RenderDevice.RenderEntryInferred Render<SAMP>(this RenderDevice Device, ITopology<SAMP> Samples, Action<IntBl, SAMP, IVertex> F) where SAMP : Brand<SAMP> {
      return Device.Render(Samples.Count, (idx, vertex) => F(idx, Samples[idx], vertex), Samples.IndexBuffer);
    }

    /// <summary>
    /// Render vertices based on custom vertex samples. 
    /// </summary>
    /// <param name="Surface">Parametric surface to render.</param>
    /// <param name="Width">How many samples to make along the parameterized x dimension.</param>
    /// <param name="Height">How many samples to make along the parameterized y dimension.</param>
    /// <param name="F">Rendering action from a 3 argument function, (n,k,v) where n is the vertex index, k is a custom sample value, and v is the vertex result structure.</param>
    public static RenderDevice.RenderEntryInferred Render(this RenderDevice Device, PSurface Surface, int Width, int Height, Action<IntBl, PointBl, IVertex> F) {
      var Samples = new PSurfaceSample(Width, Height);
      return Device.Render(Samples.Count, (idx, vertex) => F(idx, Samples[idx], vertex), Samples.IndexBuffer);
    }

    public static RenderDevice.RenderEntryInferred RenderCube(this RenderDevice Device, Action<IntBl, IVertex> F) {
      var Samples = new CubeTopology();
      return Device.Render(Samples.Count, (idx, vertex) => { vertex.Position = Samples[idx]; }, Samples.IndexBuffer);
    }


    /// <summary>
    /// Force the evaluation of this value to occur in a resulting pixel shader. All expressions that include this expression will also be evaluated in the pixel shader.
    /// </summary>
    public static BRAND PerPixel<T, BRAND>(this Brand<T, BRAND> Value) where BRAND : Brand<T, BRAND> {
      return Brand<T, BRAND>.ToBrand(new RenderDevice.PixelExpr<T>(Value.Underlying));
    }
    public static BRAND PerVertex<T, BRAND>(this Brand<T, BRAND> Value) where BRAND : Brand<T, BRAND> {
      return Brand<T, BRAND>.ToBrand(new RenderDevice.VertexExpr<T>(Value.Underlying));
    }
  }
  public static partial class Sample {
    internal partial class Op<PT, DIM> : Ops.BaseOperatorX<Texture<DIM>, PT, Vecs.Vec4<double>> where DIM : DNum<DIM> {
      public override Eval<EVAL>.Info<Vecs.Vec4<double>> Eval<EVAL>(Bling.Ops.Operation<Texture<DIM>, PT, Vecs.Vec4<double>> op, Eval<EVAL> txt) {
        if (txt is RenderDevice.IStatusEval<EVAL>) { // push it down to pixel level. 
          { // arg a must be a uniform.
            ((RenderDevice.IStatusEval<EVAL>)txt).BringIn(null, op.ArgA, RenderDevice.InfoStatus.Uniform);
          }
          {
            var argB = txt.DoEval(op.ArgB);
            ((RenderDevice.IStatusEval<EVAL>)txt).BringIn(argB, op.ArgB, RenderDevice.InfoStatus.Pixel);
          }
          var info = ((RenderDevice.IStatusEval<EVAL>)txt).NewInfo0<Vecs.Vec4<double>>("sample");
          ((RenderDevice.IStatusEval<EVAL>)txt).SetStatusFor(info, RenderDevice.InfoStatus.Pixel);
          return info;
        } else return base.Eval<EVAL>(op, txt);
      }
    }
  }
}
namespace Bling.DX {
  using Bling.Shaders;
  public partial class RenderDevice : IDisposable /*, IRenderDevice */ {
    internal abstract class CoreClassifyEval<EVAL> : BaseShaderEval<EVAL>, IStatusEval<EVAL>, IShareEval<EVAL>, IDynamicEval<EVAL>, ITableEval<EVAL> where EVAL : Eval<EVAL> {
      public virtual Info<T> Dynamic<T>(DynamicMode Mode) {
        var ret = (MyInfo<T>)NewInfo<T>(null);
        ret.Status = InfoStatus.Uniform;
        ret.Dynamic = Mode;
        return ret;
      }
      public virtual Eval<EVAL>.Info<T> Dynamic<T>(Expr Other) {
        var ret = (MyInfo<T>)NewInfo<T>(null, DoEval1(Other));
        return ret;
      }




      public new interface MyInfo : BaseShaderEval<EVAL>.MyInfo {
        InfoStatus Status { get; set; }
        DynamicMode Dynamic { get; set; }
        MyInfo Copy();
      }
      public new class MyInfo<T> : BaseShaderEval<EVAL>.MyInfo<T>, MyInfo {
        public InfoStatus Status { get; set; }
        public DynamicMode Dynamic { get; set; }
        public MyInfo Copy() {
          return new MyInfo<T>() {
            Value = Value,
            Dynamic = Dynamic,
            //Semantic = Semantic,
            Status = Status,
          };
        }
      }
      public abstract Info<T> Share<T>(ShareEval.ShareExpr<T> share);
      public void SetStatusFor(Info info, InfoStatus Status) { ((MyInfo)info).Status = Status; }
      public InfoStatus StatusFor(Info info) { return ((MyInfo)info).Status; }
      public Info<T> NewInfo0<T>(string value) { return NewInfo<T>(value); }
      public abstract void BringIn0(MyInfo info0, Expr Expr, InfoStatus To);
      public void BringIn(Info info0, Expr Expr, InfoStatus To) {
        var info = (MyInfo)info0;
        if (info != null) {
          ((int)info.Status >= (int)To).Assert();
          if (info.Status == To) {
            return;
          }
          if (info.Status == InfoStatus.Uniform && info.Value != null) return; // constant!
        }
        BringIn0(info, Expr, To);
        if (info != null) info.Status = To;
      }
      public override Info<T> Constant<T>(Constant<T> constant) {
        var ret = (MyInfo<T>)base.Constant<T>(constant);
        ret.Status = InfoStatus.Uniform;
        (ret.Value != null).Assert();
        return ret;
      }
      protected override BaseShaderEval<EVAL>.MyInfo<T> Prepare<T>(Func<BaseShaderEval<EVAL>.MyInfo<T>> Target, Expr[] Exprs, params Eval<EVAL>.Info[] Infos) {
        if (Exprs == null) {
          (Infos.Length == 1).Assert();
          var ret = (MyInfo<T>)base.Prepare(Target, Exprs, Infos);
          ret.Status = ((MyInfo)Infos[0]).Status;
          return ret;
        }
        InfoStatus Status = InfoStatus.Uniform; // highest status, find min
        //DynamicMode Mode = DynamicMode.Static;
        //bool IsUniform = false;
        bool IsConstant = true;
        bool? AllSame = null;
        foreach (var info in Infos) {
          var AtIsConstant = (((MyInfo)info).Status == InfoStatus.Uniform && ((MyInfo)info).Value != null);
          IsConstant = IsConstant && AtIsConstant;
          if (AtIsConstant) continue;
          if (AllSame == null) AllSame = true;
          else AllSame = ((bool)AllSame) && ((MyInfo)info).Status == Status;

          Status = (InfoStatus)Math.Min((int)Status, (int)((MyInfo)info).Status);
          //IsUniform = IsUniform || ((MyInfo)info).Status == InfoStatus.Uniform;
        }
        if (IsConstant) { // all uniform, all constant
          var ret = (MyInfo<T>)base.Prepare(Target, Exprs, Infos);
          ret.Status = InfoStatus.Uniform;
          (ret.Value != null).Assert();
          return ret;
        }


        // if we have uniform and to vertex, go down to to vertex
        if (AllSame != null && !((bool)AllSame) &&
            ((Status == InfoStatus.ToVertex) || (Status == InfoStatus.ToInstance)))
          Status = InfoStatus.Vertex;
        for (int i = 0; i < Infos.Length; i++)
          BringIn(Infos[i], Exprs[i], Status);
        // we actually don't care...about....but do it anyways...
        {
          var ret = (MyInfo<T>)base.Prepare(Target, Exprs, Infos);
          ret.Status = Status;
          return ret;
        }
      }
      public new MyInfo<T> DoEval<T>(Expr<T> Expr) { return (MyInfo<T>)((BaseShaderEval<EVAL>)this).DoEval(Expr); }
      public override BaseShaderEval<EVAL>.MyInfo<T> NewInfo<T>(string Value, Info Copy) {
        var Copy0 = (MyInfo)Copy;
        return new MyInfo<T>() { Value = Value, Status = Copy0.Status, Dynamic = Copy0.Dynamic };
      }
      public override BaseShaderEval<EVAL>.MyInfo<T> NewInfo<T>(string Value) {
        return new MyInfo<T>() { Value = Value, Status = InfoStatus.Uniform, Dynamic = DynamicMode.Static };
      }
      public CoreClassifyEval() {
      }
      public override Eval<EVAL>.Info<T> Table<T>(TableExpr<T> value) {
        var Idx = DoEval(value.Index);
        if (Idx.Status >= InfoStatus.Vertex) {
          (Idx.Value == null).Assert();
          var ret = NewInfo<T>(null, Idx);
          return ret;
        }
        throw new NotSupportedException();
      }
      public override Eval<EVAL>.Info<T> Block<T>(T[] Values, Eval<EVAL>.Info<int> idx) { throw new NotSupportedException(); }
    }

    internal class GeneralEval : BaseShaderEval<GeneralEval>, IStateEval<GeneralEval>, IShareEval<GeneralEval> {
      public readonly Dictionary<IExpr, int> ShareTemps = new Dictionary<IExpr, int>();
      public readonly List<MyInfo> TempCode = new List<MyInfo>();
      public readonly Dictionary<IExpr, string> Params = new Dictionary<IExpr, string>();



      public void OutputTemps(System.IO.StringWriter Out) {
        for (int i = 0; i < this.TempCode.Count; i++) {
          Out.WriteLine("  " + this.TypeNameFor(this.TempCode[i].TypeOfT) +
            " T" + i + " = " + this.TempCode[i].Value + ";");
        }
      }

      public Dictionary<object, object> Processed;
      public System.IO.StringWriter StateOut;
      public override string TypeNameFor(Type type) {
        return Effect.TypeNameFor(type);
      }
      public GeneralEval Init(Dictionary<Expr, string> Params0) {
        foreach (var u in Params0) Params[u.Key] = u.Value;
        return this;
      }

      public string IdFor(IState state) {
        if (!Processed.ContainsKey(state)) {
          state.Index = Processed.Count;
          Processed[state] = state.Index;
          state.Output(StateOut);
        } else {
          state.Index = (int)Processed[state];
        }
        return state.Name;
      }
      protected override Eval<GeneralEval>.Info<S> AllocateTemp<S>(Scope scope, Info<S> Code) {
        if (scope == this.TopScope)
          return null; // no need at top, all temps pre-allocated.
        else return base.AllocateTemp(scope, Code);
      }
      public override Eval<GeneralEval>.Info<S> Memo<S>(IExpr op) {
        // don't allow these to hit us!
        if (Params.ContainsKey(op)) {
          var info = (MyInfo<S>)NewInfo<S>(Params[op]);
          return info;
        } else if (ShareTemps.ContainsKey(op)) {
          return NewInfo<S>("T" + ShareTemps[op]);
        } else return base.Memo<S>(op);
      }
      public override Eval<GeneralEval>.Info<T> DoEval<T>(Expr<T> Expr) {
        if (Params.ContainsKey(Expr)) {
          var info = (MyInfo<T>)NewInfo<T>(Params[Expr]);
          return info;
        } else if (ShareTemps.ContainsKey(Expr)) {
          return NewInfo<T>("T" + ShareTemps[Expr]);
        }
        return base.DoEval<T>(Expr);
      }
      
      public Info<T> Share<T>(ShareEval.ShareExpr<T> share) {
        if (ShareTemps.ContainsKey(share)) {
          return NewInfo<T>("T"+ShareTemps[share]);
        } else if (Params.ContainsKey(share)) {
          var info = (MyInfo<T>)NewInfo<T>(Params[share]);
          return info;
        } else {
          var infoX = DoEval1(share.Use);
          (infoX.Value != null).Assert();
          var Idx = ShareTemps.Count;
          (TempCode.Count == Idx).Assert();
          TempCode.Add(infoX);
          ShareTemps[share] = Idx;
          return NewInfo<T>("T" + Idx);
        }
      }
      public override MyInfo<T> NewInfo<T>(string Value) { return new MyInfo<T>() { Value = Value }; }
      public override MyInfo<T> NewInfo<T>(string Value, Eval<GeneralEval>.Info Copy) { return new MyInfo<T>() { Value = Value }; }
    }

    internal class ClassifyEval : CoreClassifyEval<ClassifyEval>, Ops.IMapEval<ClassifyEval> {
      protected override Eval<ClassifyEval>.Info<S> AllocateTemp<S>(Scope scope, Info<S> Code) {
        return null; // no need
      }
      public Eval<ClassifyEval>.Info<T> Map<S, T>(Ops.Operation<S, T> op) {
        Uniforms.Add(op);
        var ret = (MyInfo<T>) NewInfo<T>("MAP");
        ret.Status = InfoStatus.Uniform;
        return ret;
      }
      public override void BringIn0(CoreClassifyEval<ClassifyEval>.MyInfo info, Expr Expr, InfoStatus To) {
        if ((info == null && To == InfoStatus.Uniform) || (info.Status == InfoStatus.Uniform)) {
          if (info != null) {
            info.Value = "U";
          }
          Uniforms.Add(Expr);
        } else if (info == null) {
          false.Assert();
        } else if (info.Status == InfoStatus.ToVertex ||
                   info.Status == InfoStatus.ToInstance) {
          info.Value = "V";
          VertexParams[Expr] = info.Copy();
          if (To == InfoStatus.Pixel)
            PixelParams[Expr] = info.Copy();
        } else if (info.Status == InfoStatus.Vertex) {
          info.Value = "P";
          PixelParams[Expr] = info.Copy();
        }
      }
      public readonly HashSet<Expr> Uniforms = new HashSet<Expr>();
      public readonly Dictionary<Expr, MyInfo> VertexParams = new Dictionary<Expr, CoreClassifyEval<ClassifyEval>.MyInfo>();
      public readonly Dictionary<Expr, MyInfo> PixelParams = new Dictionary<Expr, CoreClassifyEval<ClassifyEval>.MyInfo>();
      public readonly Dictionary<ShareEval.IShareExpr, MyInfo> Classification = new Dictionary<ShareEval.IShareExpr, MyInfo>();
      public override Info<T> Share<T>(ShareEval.ShareExpr<T> share) {
        // they are already classified
        if (!Classification.ContainsKey(share)) {
          var status = (ClassifyEval.MyInfo<T>)DoEval(share.Use);
          Classification[share] = status.Copy();
          return status;
        }
        var ret = (MyInfo<T>)Classification[share].Copy();
        return ret;
      }
    }

  }
}