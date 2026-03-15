using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Bling.Util;
using Bling.DSL;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;


namespace Bling.WPF {
  using Bling.Vecs;
  using linq = Microsoft.Linq.Expressions;
  using Bling.Ops;
  using Bling.Core;
  using System.Reflection;
  using Bling.Matrices;
  using Bling.Shaders;
  using Bling.Graphics;

  public partial class BrushBl<T,BRAND> : ObjectBl<T,BRAND>, IExtends<BRAND,BrushBl> where T : Brush where BRAND : BrushBl<T,BRAND> {
    public BrushBl(Expr<T> Underlying) : base(Underlying) { }
    public BrushBl() : base() { }
    public static implicit operator BrushBl(BrushBl<T, BRAND> Value) {
      return BrushBl.UpCast(Value);
    }
    public static implicit operator Tex2D(BrushBl<T, BRAND> Brush) { return Tex2D.Make(Brush.Underlying); }
    public static explicit operator NormalMap(BrushBl<T, BRAND> Brush) { return (NormalMap)(Tex2D)Brush; }
    public static explicit operator HeightField(BrushBl<T, BRAND> Brush) { return (HeightField)(Tex2D)Brush; }
    public ColorBl this[PointBl uv] { get { return ((Tex2D)this)[uv]; } }
    static BrushBl() {
      WPFExtensions.CheckInit();
    }
  }
  public partial class BrushBl : BrushBl<Brush, BrushBl>, ICoreBrandT {
    public BrushBl(Expr<Brush> Underlying) : base(Underlying) { }
    public BrushBl() : base() { }
    public static implicit operator BrushBl(Expr<Brush> Value) { return new BrushBl(Value); }
    public static implicit operator BrushBl(Brush Value) { return new Constant<Brush>(Value); }
    static BrushBl() { Register(e => e); }
  }
  /// <summary>
  /// Bling wrapper around a WPF collection of primitives.
  /// </summary>
  /// <typeparam name="E">Collection element type.</typeparam>
  /// <typeparam name="EBRAND">Bling type of collection element.</typeparam>
  /// <typeparam name="T">Original collection type.</typeparam>
  /// <typeparam name="BRAND">Bling type of collection.</typeparam>
  public abstract class PrimitiveCollectionBl<E, EBRAND, T, BRAND> : CollectionBl<E, EBRAND, T, BRAND>
    where BRAND : PrimitiveCollectionBl<E, EBRAND, T, BRAND>
    where T : Freezable, IFormattable, IList, ICollection, IList<E>, ICollection<E>, IEnumerable<E>, IEnumerable, new()
    where EBRAND : ICanWrap<EBRAND, E>
    where E : new() {
    public PrimitiveCollectionBl(Expr<T> Underlying) : base(Underlying) { }

    /// <summary>
    ///     Makes the current object unmodifiable and sets its System.Windows.Freezable.IsFrozen
    ///     property to true.
    /// </summary>
    public void Freeze() {
      CurrentValue.Freeze();
    }
    /// <summary>
    /// Gets a value that indicates whether the object is currently modifiable.
    /// </summary>
    public BoolBl IsFrozen { get { return this.Map<bool,BoolBl>(c => c.IsFrozen); } }

    /// <summary>
    /// Gets a value that indicates whether the object can be made unmodifiable.
    /// </summary>
    public BoolBl CanFreeze { get { return this.Map<bool,BoolBl>(c => c.CanFreeze);  } }

    private linq.Expression AssignElements(linq.Expression currentBuffer, IList<EBRAND> Backing) {
      List<linq.Expression> statements = new List<linq.Expression>();
      var eval = new Bling.Linq.DefaultLinqEval();
      for (int i = 0; i < Backing.Count; i++) {
        var properties = typeof(IList<E>).GetProperties();
        var indexP = typeof(IList<E>).GetProperty("Item");

        var bufferAt = linq.Expression.Property(currentBuffer, indexP, linq.Expression.Constant(i, typeof(int)));
        eval.PushScope();
        var assignTo = eval.DoEval(CanWrap<EBRAND,E>.From(Backing[i])).Value;
        var assignElem = linq.Expression.Assign(bufferAt, assignTo);
        statements.Add(eval.PopScope(assignElem));
      }
      return linq.Expression.Block(statements);
    }
    /// <summary>
    /// Bind this value to a new populated primitive collection.
    /// </summary>
    /// <param name="Backing">Elements to populate the new primitive collection.</param>
    public void InitElements(IList<EBRAND> Backing) {
      T Buf = new T();
      for (int i = 0; i < Backing.Count; i++) {
        Buf.Add(CanWrap<EBRAND,E>.From(Backing[i]));
      }
      this.Now = ToBrand(new Constant<T>(Buf));
    }
    public Action RefreshAction(Func<IntBl, Expr<E>> F, int Count) {
      T[] Buffers = new T[2];
      Action[] Set = new Action[2];
      for (int i = 0; i < 2; i++) {
        Buffers[i] = new T();
        for (int j = 0; j < Count; j++) {
          Buffers[i].Add(new E());
        }
        Set[i] = PrimitiveCollectionBl<E, EBRAND, T, BRAND>.Assign(this, ToBrand(new Constant<T>(Buffers[i])));
      }
      int BufIndex = 0;
      Action Set0 = () => {
        Set[BufIndex]();
        BufIndex = (BufIndex + 1) % 2;
      };
      Func<T> Current0 = () => Buffers[BufIndex];
      List<linq.Expression> statements = new List<linq.Expression>();

      var currentBuffer = linq.Expression.Parameter(typeof(T), "CurrentBuffer");
      var assignBuffer = linq.Expression.Assign(currentBuffer,
        linq.Expression.Invoke(linq.Expression.Constant(Current0, typeof(Func<T>))));
      statements.Add(assignBuffer);
      {
        var eval = new Bling.Linq.DefaultLinqEval();
        linq.Expression loop = eval.For("i", Count, (i) => {
          var I = new Bling.Linq.LinqExpr<int>(i.Value);
          var assignTo = eval.DoEval(F(I)).Value;
          var properties = typeof(IList<E>).GetProperties();
          var indexP = typeof(IList<E>).GetProperty("Item");
          var bufferAt = linq.Expression.Property(currentBuffer, indexP, i.Value);
          var assignElem = linq.Expression.Assign(bufferAt, assignTo);
          return assignElem;
        });
        statements.Add(loop);
      }
      statements.Add(linq.Expression.Invoke(linq.Expression.Constant(Set0, typeof(Action))));
      var doit = linq.Expression.Lambda<Action>(linq.Expression.Block(new linq.ParameterExpression[] { currentBuffer }, statements));
      var doitF = doit.Compile();
      return doitF;
    }
    /// <summary>
    /// Creates an efficient swap collection refresh action. When the resulting action is called, a
    /// collection is efficiently re-populated via Backing and bound to this value.
    /// </summary>
    /// <param name="Backing">Elements to populate collection on refresh.</param>
    /// <returns>An action that refreshes the collection bound to this value.</returns>
    public Action RefreshAction(IList<EBRAND> Backing) {
      T[] Buffers = new T[2];
      Action[] Set = new Action[2];
      for (int i = 0; i < 2; i++) {
        Buffers[i] = new T();
        for (int j = 0; j < Backing.Count; j++) {
          Buffers[i].Add(new E());
        }
        Set[i] = PrimitiveCollectionBl<E, EBRAND, T, BRAND>.Assign(this, ToBrand(new Constant<T>(Buffers[i])));
      }
      int BufIndex = 0;
      Action Set0 = () => {
        Set[BufIndex]();
        BufIndex = (BufIndex + 1) % 2;
      };
      Func<T> Current0 = () => Buffers[BufIndex];
      List<linq.Expression> statements = new List<linq.Expression>();

      var currentBuffer = linq.Expression.Parameter(typeof(T), "CurrentBuffer");
      var assignBuffer = linq.Expression.Assign(currentBuffer,
        linq.Expression.Invoke(linq.Expression.Constant(Current0, typeof(Func<T>))));
      statements.Add(assignBuffer);
      statements.Add(AssignElements(currentBuffer, Backing));
      statements.Add(linq.Expression.Invoke(linq.Expression.Constant(Set0, typeof(Action))));
      var doit = linq.Expression.Lambda<Action>(linq.Expression.Block(new linq.ParameterExpression[] { currentBuffer }, statements));
      var doitF = doit.Compile();
      return doitF;
    }
  }
  public class PointCollectionBl : PrimitiveCollectionBl<Point, PointBl, PointCollection, PointCollectionBl>, ICoreBrandT {
    public PointCollectionBl(Expr<PointCollection> Underlying) : base(Underlying) { }
    public PointCollectionBl() : this(new Constant<PointCollection>(new PointCollection())) { }
    public static implicit operator PointCollectionBl(Expr<PointCollection> target) { return new PointCollectionBl((target)); }
    public static implicit operator PointCollectionBl(PointCollection target) { return (new Constant<PointCollection>(target)); }
    static PointCollectionBl() { Register(v => v); }
  }
  public class IntCollectionBl : PrimitiveCollectionBl<int, IntBl, Int32Collection, IntCollectionBl>, ICoreBrandT {
    public IntCollectionBl(Expr<Int32Collection> Underlying) : base(Underlying) { }
    public IntCollectionBl() : this(new Constant<Int32Collection>(new Int32Collection())) { }
    public static implicit operator IntCollectionBl(Expr<Int32Collection> target) { return new IntCollectionBl((target)); }
    public static implicit operator IntCollectionBl(Int32Collection target) { return (new Constant<Int32Collection>(target)); }
    static IntCollectionBl() { Register(v => v); }
  }

  public class DoubleCollectionBl : PrimitiveCollectionBl<double, DoubleBl, DoubleCollection, DoubleCollectionBl>, ICoreBrandT {
    public DoubleCollectionBl(Expr<DoubleCollection> Underlying) : base(Underlying) { }
    public DoubleCollectionBl() : this(new Constant<DoubleCollection>(new DoubleCollection())) { }
    public static implicit operator DoubleCollectionBl(Expr<DoubleCollection> target) { return new DoubleCollectionBl((target)); }
    public static implicit operator DoubleCollectionBl(DoubleCollection target) { return (new Constant<DoubleCollection>(target)); }
    static DoubleCollectionBl() { Register(v => v); }
  }
  public abstract class BasePoint3DCollectionBl<E, EBRAND, T, BRAND> : PrimitiveCollectionBl<E, EBRAND, T, BRAND>
    where BRAND : PrimitiveCollectionBl<E, EBRAND, T, BRAND>
    where T : Freezable, IFormattable, IList, ICollection, IList<E>, ICollection<E>, IEnumerable<E>, IEnumerable, new()
    where EBRAND : ICanWrap<EBRAND, E>
    where E : new() {
    public BasePoint3DCollectionBl(Expr<T> Underlying) : base(Underlying) { }
  }


  public class Point3DCollectionBl : BasePoint3DCollectionBl<Point3D, Point3DBl, Point3DCollection, Point3DCollectionBl>, ICoreBrandT {
    public Point3DCollectionBl(Expr<Point3DCollection> Underlying) : base(Underlying) { }
    public Point3DCollectionBl() : this(new Constant<Point3DCollection>(new Point3DCollection())) { }
    public static implicit operator Point3DCollectionBl(Expr<Point3DCollection> target) { return new Point3DCollectionBl((target)); }
    public static implicit operator Point3DCollectionBl(Point3DCollection target) { return (new Constant<Point3DCollection>(target)); }
    static Point3DCollectionBl() { Register(v => v); }
  }
  public class Vector3DCollectionBl : BasePoint3DCollectionBl<Vector3D, Point3DBl, Vector3DCollection, Vector3DCollectionBl>, ICoreBrandT {
    public Vector3DCollectionBl(Expr<Vector3DCollection> Underlying) : base(Underlying) { }
    public Vector3DCollectionBl() : this(new Constant<Vector3DCollection>(new Vector3DCollection())) { }
    public static implicit operator Vector3DCollectionBl(Expr<Vector3DCollection> target) { return new Vector3DCollectionBl((target)); }
    public static implicit operator Vector3DCollectionBl(Vector3DCollection target) { return (new Constant<Vector3DCollection>(target)); }
    static Vector3DCollectionBl() { Register(v => v); }
  }

  public class MatrixArity : MultiArity<double, Matrix, MatrixArity> {
    public override Matrix Make(params double[] Params) {
      return new Matrix(Params[0], Params[1], Params[2], Params[3], Params[4], Params[5]);
    }
    public override double Access(Matrix Value, int Idx) {
      switch (Idx) {
        case 0: return Value.M11;
        case 1: return Value.M12;
        case 2: return Value.M21;
        case 3: return Value.M22;
        case 4: return Value.OffsetX;
        case 5: return Value.OffsetY;
        default: throw new NotSupportedException();
      }
    }
    public override int Count { get { return 6; } }
    public override string[] PropertyNames {
      get {
        return new string[] {
          "M11",
          "M12",
          "M21",
          "M22",
          "OffsetX",
          "OffsetY",
        };
      }
    }
  }
  public static partial class WPFExtensions {
    public static void CheckInit() {
    }

    public static BrushBl Bl(this Expr<Brush> p) { return p; }
    public static BrushBl Bl(this Brush p) { return p; }

    public static ImageSource ImageFor(this string relativeFile, Type From) {
      return (new System.Windows.Media.Imaging.BitmapImage(relativeFile.MakePackUri(From)));
    }
    public static ImageSource[] ImageFor(this string path, Type From, params string[] args) {
      var ret = new ImageSource[args.Length];
      for (int i = 0; i < args.Length; i++) {
        string relativeFile = path + "/" + args[i];
        ret[i] = (new System.Windows.Media.Imaging.BitmapImage(relativeFile.MakePackUri(From)));
      }
      return ret;
    }
    public static Point Clamp(this Point at, Point min, Point max) {
      return new Point(at.X.Clamp(min.X, max.X), at.Y.Clamp(min.Y, max.Y));
    }

    public static Color Clamp(this Color at, Color min, Color max) {
      return Color.FromScRgb(at.ScA.Clamp(min.ScA, max.ScA),
                             at.ScR.Clamp(min.ScR, max.ScR),
                             at.ScG.Clamp(min.ScG, max.ScG),
                             at.ScB.Clamp(min.ScB, max.ScB));


    }


    public static Point Size(this ImageSource source) { return new Point(source.Width, source.Height); }
    public static Point PairWiseDivide(this Point point, Point op) { return new Point(point.X / op.X, point.Y / op.Y); }
    public static Point PairWiseMultiply(this Point point, Point op) { return new Point(point.X * op.X, point.Y * op.Y); }

    /*
    public static System.Windows.Media.Imaging.BitmapSource LoadBitmap(this System.Drawing.Bitmap source) {
      return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(source.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty,
          System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
    }*/


    public static Vector Delta(this DragDeltaEventArgs source) {
      return new Vector(source.HorizontalChange, source.VerticalChange);
    }
    public static Orientation Reverse(this Orientation dir) {
      if (dir == Orientation.Horizontal) return Orientation.Vertical;
      else return Orientation.Horizontal;
    }

    public static double Select(this Vector vector, Orientation dir) {
      if (dir == Orientation.Horizontal) return vector.X;
      else return vector.Y;
    }
    public static double Select(this Point vector, Orientation dir) {
      if (dir == Orientation.Horizontal) return vector.X;
      else return vector.Y;
    }
    public static void Select(this Point vector, Orientation dir, double value) {
      if (dir == Orientation.Horizontal) vector.X = value;
      else vector.Y = value;
    }
    public static Vector Vector(this Orientation dir, double x) {
      if (dir == Orientation.Horizontal) return new Vector(x, 0);
      else return new Vector(0, x);
    }
    public static Point Point(this Orientation dir, double x) {
      if (dir == Orientation.Horizontal) return new Point(x, 0);
      else return new Point(0, x);
    }
    public static readonly ColorBl[] ColorsSelect = new ColorBl[] {
      Colors.Red, 
      Colors.Blue, 
      Colors.Green,
      Colors.Violet, 
      Colors.Purple, 
      Colors.Orange, 
      Colors.Yellow, 
      Colors.MidnightBlue,
      Colors.SaddleBrown,
      Colors.DarkBlue,
      Colors.DarkViolet,
      Colors.DarkRed,
      Colors.DarkGreen, 
      Colors.RoyalBlue, 
      Colors.Tomato,
      Colors.Pink, Colors.Peru,
      Colors.Silver, Colors.Turquoise, 
      Colors.RosyBrown, Colors.PowderBlue,
      Colors.PeachPuff, 
      Colors.Black,
    };
    /// <summary>
    /// Select a fixed color based on an integer index. 
    /// </summary>
    /// <param name="idx">Index of color to select.</param>
    /// <returns>An arbitrary color associated with the index.</returns>
    public static ColorBl SelectColor(this IntBl Idx) {
      return ColorsSelect.Table(Idx % ColorsSelect.Length);
    }
    public static ColorBl SelectColor(this int idx) {
      return ((IntBl)idx).SelectColor();
    }




    public static Point Max(this Point self, params Point[] others) {
      foreach (var other in others) {
        self = new Point(Math.Max(self.X, other.X), Math.Max(self.Y, other.Y));
      }
      return self;
    }
    public static Vector AbsMax(this Vector self, Vector other) {
      if (Math.Abs(other.X) > Math.Abs(self.X)) self.X = other.X;
      if (Math.Abs(other.Y) > Math.Abs(self.Y)) self.Y = other.Y;
      return self;
    }
    public static Point Min(this Point self, params Point[] others) {
      foreach (var other in others) {
        self = new Point(Math.Min(self.X, other.X), Math.Min(self.Y, other.Y));
      }
      return self;
    }
    public static Point Abs(this Point self) {
      return new Point(Math.Abs(self.X), Math.Abs(self.Y));
    }
    public static Vector Max(this Vector self, Vector other) {
      return new Vector(Math.Max(self.X, other.X), Math.Max(self.Y, other.Y));
    }
    public static Vector Min(this Vector self, Vector other) {
      return new Vector(Math.Min(self.X, other.X), Math.Min(self.Y, other.Y));
    }
    public static Vector Abs(this Vector self) {
      return new Vector(Math.Abs(self.X), Math.Abs(self.Y));
    }
    public static Point Inflect(this Point self, Point with) {
      var dx = self.X;
      var dy = self.Y;
      if (dx > with.X) dx = (2 * with.X) - dx;
      if (dy > with.Y) dy = (2 * with.Y) - dy;
      return new Point(dx, dy);
    }
    public static T DecelerationRatio<T>(this T target, double value) where T : Timeline {
      target.DecelerationRatio = value;
      return target;
    }
    public static T AccelerationRatio<T>(this T target, double value) where T : Timeline {
      target.AccelerationRatio = value;
      return target;
    }
    public static T Duration<T>(this T target, Duration value) where T : Timeline {
      target.Duration = value;
      return target;
    }
    public static T Duration<T>(this T target, TimeSpan value) where T : Timeline {
      target.Duration = new Duration(value);
      return target;
    }

    public static T Duration<T>(this T target, double value) where T : Timeline {
      return target.Duration((TimeSpan.FromMilliseconds(value)));
    }
    public static T Delay<T>(this T target, double value) where T : Timeline {
      target.BeginTime = TimeSpan.FromMilliseconds(value);
      return target;
    }
    public static T BeginTime<T>(this T target, TimeSpan timeSpan) where T : Timeline {
      target.BeginTime = timeSpan;
      return target;
    }
    /// <summary>
    /// Return a random point between (0,0) and (1,1).
    /// </summary>
    public static Point NextPoint(this Random r) {
      return new Point(r.NextDouble(), r.NextDouble());
    }

    public static T SpeedRatio<T>(this T target, double d) where T : Timeline {
      target.SpeedRatio = d;
      return target;
    }

    public static T BeginTime<T>(this T target, double value) where T : Timeline {
      return target.BeginTime(TimeSpan.FromMilliseconds(value));
    }
    public static T Repeat<T>(this T target, RepeatBehavior repeat) where T : Timeline {
      target.RepeatBehavior = repeat;
      return target;
    }
    public static T Forever<T>(this T target) where T : Timeline {
      return target.Repeat(RepeatBehavior.Forever);
    }
    public static T AutoReverse<T>(this T target, bool autoReverse) where T : Timeline {
      target.AutoReverse = autoReverse;
      return target;
    }
    public static T AutoReverse<T>(this T target) where T : Timeline {
      return target.AutoReverse(true);
    }
    public static bool IsNan(this Point d) {
      return d.X.IsNan() || d.Y.IsNan();
    }
    public static bool IsNan(this Point3D d) {
      return d.X.IsNan() || d.Y.IsNan() || d.Z.IsNan();
    }
    public static MatrixBl<D3,D3> Bl(this Expr<Matrix> Core) {
      DoubleBl[] Table = new DoubleBl[9];
      var arity = MatrixArity.ArityI;
      MatrixBl<D1, D3>[] Columns = new MatrixBl<D1, D3>[3];
      for (int i = 0; i < 3; i++) {
        Columns[i] = new Point3DBl(arity.Point(Core, i), (i == 2 ? 1d : 0d));
      }
      return new MatrixBl<D3,D3>(Columns);
    }
    public static Expr<Matrix> AsMatrix(this MatrixBl<D3,D3> M) {
      var arity = MatrixArity.ArityI;
      var matrix = arity.Make(M.M11(), M.M12(), M.M21(), M.M22(), M.OffsetX(), M.OffsetY());
      return matrix;
    }

  }
}
