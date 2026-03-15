using System;
using System.Collections.Generic;
using Bling.DSL;
using Bling.Util;
using Bling.Linq;
using Bling.Core;

namespace Bling.Core {
}

namespace Bling.Arrays {
  public interface IDim<BRAND> where BRAND : Brand<BRAND> {
    int Count { get; }
    Array1D<BRAND> this[IntBl index] { get; set; }
  }
  public interface IBaseArray<BRAND> where BRAND : Brand<BRAND> {
    IDim<BRAND> Row { get; }
    IDim<BRAND> Column { get; }
    IBaseArray<BRAND> BaseTranspose { get; }
    BRAND this[IntBl Column, IntBl Row] { get; set; }
  }

  public interface IArray1D<BRAND> : IBaseArray<BRAND> where BRAND : Brand<BRAND> {
    int Count { get; }
    BRAND this[IntBl Index] { get; set;  }
  }
  public interface IArray<BRAND> : IBaseArray<BRAND> where BRAND : Brand<BRAND> {
    BRAND this[IntBl x, IntBl y] { get; set;  }
  }
  /// <summary>
  /// A one dimensional array of Bling values.
  /// </summary>
  /// <typeparam name="T">Type of values storied in array.</typeparam>
  public class Array1D<BRAND> : IArray1D<BRAND> where BRAND : Brand<BRAND> {
    private readonly Func<IntBl, BRAND> F;
    /// <summary>
    /// Create a new array.
    /// </summary>
    /// <param name="Count">Number of elements in array.</param>
    /// <param name="F">Function that produces elements of array by (Index).</param>
    public Array1D(int Count, Func<IntBl, BRAND> F) {
      this.Count = Count; this.F = F;
    }
    public T[] ToArray<T>(Func<BRAND, T> F) {
      var ret = new T[Count];
      for (int i = 0; i < ret.Length; i++) ret[i] = F(this[i]);
      return ret;
    }


    public BRAND this[IntBl Column, IntBl Row] { get { return this.Column[Column][Row]; } set { this[Column, Row].Bind = value; } }

    public static implicit operator Array1D<BRAND>(BRAND[] Values) {
      return new Array1D<BRAND>(Values.Length, Values.Table);
    }
    public static implicit operator BRAND[](Array1D<BRAND> Values) {
      var ret = new BRAND[Values.Count];
      for (int i = 0; i < ret.Length; i++) ret[i] = Values[i];
      return ret;
    }

    /// <summary>
    /// Number of elements in array.
    /// </summary>
    public int Count { get; private set; }
    public override string ToString() {
      return Count + "::" + F(IntBl.Param("I")).ToString();
    }
    public override int GetHashCode() {
      return F(IntBl.Param("I")).GetHashCode() + Count;
    }
    public override bool Equals(object obj) {
      if (obj is Array1D<BRAND>) {
        var a = (Array1D<BRAND>)obj;
        return a.Count == Count && a.F(IntBl.Param("I")).Equals(F(IntBl.Param("I")));
      }
      return base.Equals(obj);
    }

    private class ColumnDim : IDim<BRAND> {
      public Array1D<BRAND> Array;
      public int Count { get { return Array.Count; } }
      public Array1D<BRAND> this[IntBl index] {
        get { return new Array1D<BRAND>(1, j => Array[index]); }
        set {
          (value.Count == 1).Assert();
          this[index][0] = value[0];
        }
      }
    }
    private class RowDim : IDim<BRAND> {
      public Array1D<BRAND> Array;
      public int Count { get { return 1; } }
      public Array1D<BRAND> this[IntBl index] {
        get { return Array; }
        set {
          (value.Count == this[index].Count).Assert();
          for (int i = 0; i < value.Count; i++)
            this[index][i] = value[i];
        }
      }
    }
    public IDim<BRAND> Row { get { return new RowDim() { Array = this }; } }
    public IDim<BRAND> Column { get { return new ColumnDim() { Array = this }; } }


    /// <summary>
    /// Access element of array.
    /// </summary>
    /// <param name="i">Index of element to access.</param>
    public BRAND this[IntBl i] { get { return F(i); } set { this[i].Bind = value; } }

    public IBaseArray<BRAND> BaseTranspose {
      get { return new Array2D<BRAND>(1, Count, (i, j) => this[j]); }
    }

    /// <summary>
    /// Splice another array into this one.
    /// </summary>
    /// <param name="I">Index of splice.</param>
    /// <param name="other">Array to splice in.</param>
    public Array1D<BRAND> Splice(IntBl I, Array1D<BRAND> other) {
      var F0 = this.F;
      var F1 = other.F;
      return new Array1D<BRAND>(Count + other.Count,
                 (i) => (i < I).Condition(F(i),
          (i < I + other.Count).Condition(F1(i - I), F0(i - other.Count))));
    }
    /// <summary>
    /// Transform this 1D array into a 2D array with specified number of columns. 
    /// The resulting 2D array is laid out horizontally row by row.
    /// </summary>
    /// <param name="ColumnCount">Number of columns in resulting 2D array.</param>
    public Array2D<BRAND> Horizontal2D(int ColumnCount) {
      (Count % ColumnCount == 0).Assert();
      return new Array2D<BRAND>(ColumnCount, Count / ColumnCount, (i, j) => F(i + j * ColumnCount));
    }
    /// <summary>
    /// Transform this 1D array into a 2D array with specified number of rows. 
    /// The resulting 2D array is laid out vertically column by column.
    /// </summary>
    /// <param name="RowCount"></param>
    /// <returns></returns>
    public Array2D<BRAND> Vertical2D(int RowCount) {
      (Count % RowCount == 0).Assert();
      return new Array2D<BRAND>(Count / RowCount, RowCount, (i, j) => F(i * RowCount + j));
    }
    /// <summary>
    /// Reverse the order of elements in this array.
    /// </summary>
    public Array1D<BRAND> Reverse {
      get {
        var ret = new Array1D<BRAND>(Count, i => F(Count - i - 1));
        return ret;
      }
    }
    /// <summary>
    /// Map each element of the array according to a function into a new array of the same length.
    /// </summary>
    /// <typeparam name="BRAND0">Type of new array.</typeparam>
    /// <param name="F">Function that maps the elements.</param>
    public Array1D<BRAND0> Map<BRAND0>(Func<BRAND, BRAND0> F) where BRAND0 : Brand<BRAND0> {
      return new Array1D<BRAND0>(Count, i => F(this.F(i)));
    }
    /// <summary>
    /// Map each element of the array according to a function into a new array of the same length.
    /// </summary>
    /// <param name="F">Function that maps the elements.</param>
    public Array1D<BRAND> Map(Func<BRAND,BRAND> F) { return Map<BRAND>(F); }
    /// <summary>
    /// Combine two same-length arrays together via a combining function.
    /// </summary>
    /// <typeparam name="BRAND0">Element type of other array.</typeparam>
    /// <typeparam name="BRAND1">Element type of resulting array.</typeparam>
    /// <param name="array">Other array to combine with this one.</param>
    /// <param name="F">Combining function.</param>
    public Array1D<BRAND1> Map<BRAND0, BRAND1>(Array1D<BRAND0> array, Func<BRAND, BRAND0, BRAND1> F) where BRAND0 : Brand<BRAND0> where BRAND1 : Brand<BRAND1> {
      (array.Count == Count).Assert();
      return new Array1D<BRAND1>(Count, i => F(this.F(i), array.F(i)));
    }
    /// <summary>
    /// Combine two same-length arrays together via a combining function.
    /// </summary>
    /// <param name="array">Other array to combine with this one.</param>
    /// <param name="F">Combining function.</param>
    public Array1D<BRAND> Map(Array1D<BRAND> array, Func<BRAND,BRAND,BRAND> F) {
      return Map<BRAND,BRAND>(array, F);
    }
  }
  /// <summary>
  /// A two dimensional array of Bling values.
  /// </summary>
  /// <typeparam name="T">Type of values storied in array.</typeparam>
  public partial class Array2D<BRAND> : IArray<BRAND> where BRAND : Brand<BRAND> {

    public static Array2D<BRAND> Identity(BRAND One, int Dim) {
      return new Array2D<BRAND>(Dim, Dim, (i, j) => (i == j).Condition(One, Brand<BRAND>.Default));
    }
    private readonly Func<IntBl, IntBl, BRAND> F;

    //public Expr<T> Transform<T>(Func<K

    /// <summary>
    /// Create a new 2D array.
    /// </summary>
    /// <param name="ColumnCount">Number of columns in array.</param>
    /// <param name="RowCount">Number of rows in array.</param>
    /// <param name="F">Function to produce array's elements by (Column, Row).</param>
    public Array2D(int ColumnCount, int RowCount, Func<IntBl,IntBl,BRAND> F) {
      this.RowCount = RowCount; this.ColumnCount = ColumnCount; this.F = F;
    }
    /// <summary>
    /// Access element of array.
    /// </summary>
    /// <param name="i">Column index of element.</param>
    /// <param name="j">Row index of element.</param>
    public BRAND this[IntBl i, IntBl j] { get { return F(i, j); } set { this[i, j].Bind = value; } }


    public override string ToString() {
      return "[" + ColumnCount + ", " + RowCount + "]::" + F(IntBl.Param("I"), IntBl.Param("J")).ToString();
    }
    public override int GetHashCode() {
      return F(IntBl.Param("I"), IntBl.Param("J")).GetHashCode() + RowCount + ColumnCount;
    }
    public override bool Equals(object obj) {
      if (obj is Array2D<BRAND>) {
        var a = (Array2D<BRAND>)obj;
        return a.RowCount == RowCount && a.ColumnCount == ColumnCount && 
          a.F(IntBl.Param("I"), IntBl.Param("J")).Equals(F(IntBl.Param("I"), IntBl.Param("J")));
      }
      return base.Equals(obj);
    }



    /// <summary>
    /// Number of rows in array.
    /// </summary>
    private readonly int RowCount;
    /// <summary>
    /// Number of columns in array.
    /// </summary>
    private readonly int ColumnCount;


    private class ColumnDim : IDim<BRAND> {
      public Array2D<BRAND> Array;
      public int Count { get { return Array.ColumnCount; } }
      public Array1D<BRAND> this[IntBl index] {
        get { return new Array1D<BRAND>(Array.RowCount, j => Array[index, j]); }
        set {
          (value.Count == this[index].Count).Assert();
          for (int i = 0; i < this[index].Count; i++)
            this[index][i] = value[i];
        }
      }
    }
    private class RowDim : IDim<BRAND> {
      public Array2D<BRAND> Array;
      public int Count { get { return Array.RowCount; } }
      public Array1D<BRAND> this[IntBl index] {
        get { return new Array1D<BRAND>(Array.ColumnCount, j => Array[j, index]); }
        set {
          (value.Count == this[index].Count).Assert();
          for (int i = 0; i < this[index].Count; i++)
            this[index][i] = value[i];
        }
      }
    }
    public IDim<BRAND> Column { get { return new ColumnDim() { Array = this }; } }
    public IDim<BRAND> Row { get { return new RowDim() { Array = this }; } }


    /// <summary>
    /// Transform this array into a 1D array, horizontally row by row.
    /// </summary>
    public Array1D<BRAND> Horizontal1D {
      get {
        return new Array1D<BRAND>(ColumnCount * RowCount, (i) => F(i % ColumnCount, i / ColumnCount));
      }
    }
    /// <summary>
    /// Transform this array into a 1D array, vertically column by column.
    /// </summary>
    public Array1D<BRAND> Vertical1D {
      get {
        return new Array1D<BRAND>(ColumnCount * RowCount, (i) => F(i / RowCount, i % RowCount));
      }
    }
    /// <summary>
    /// Reverse the row order of this array.
    /// </summary>
    public Array2D<BRAND> ReverseRows {
      get {
        return new Array2D<BRAND>(ColumnCount, RowCount, (i, j) => F(i, RowCount - j - 1));
      }
    }
    /// <summary>
    /// Reverse the column order of this array.
    /// </summary>
    public Array2D<BRAND> ReverseColumns {
      get {
        return new Array2D<BRAND>(ColumnCount, RowCount, (i, j) => F(ColumnCount - i - 1, j));
      }
    }
    /// <summary>
    /// Swap row and column dimensions.
    /// </summary>
    public Array2D<BRAND> Transpose {
      get {
        return new Array2D<BRAND>(RowCount, ColumnCount, (i, j) => F(j, i));
      }
    }
    public IBaseArray<BRAND> BaseTranspose {
      get {
        return Transpose;
      }
    }

    /// <summary>
    /// Splice the rows of another array into this one. 
    /// </summary>
    /// <param name="J">Row index where splice begins.</param>
    /// <param name="other">Other array to splice in, must have same number of columns as this array.</param>
    /// <returns>Combined array with additional rows.</returns>
    public Array2D<BRAND> SpliceRows(IntBl J, IBaseArray<BRAND> other) {
      (other.Column.Count == ColumnCount).Assert();
      Func<IntBl,IntBl,BRAND> F0 = this.F;
      Func<IntBl, IntBl, BRAND> F1 = (i, j) => other.Column[i][j];
      return new Array2D<BRAND>(ColumnCount, RowCount + other.Row.Count, 
                 (x,y) => (y < J).Condition(F0(x,y), 
         (y < J + other.Row.Count).Condition(F1(x,y - J), F0(x, y - other.Row.Count)))); 
    }
    /// <summary>
    /// Splice the columns of another array into this one. 
    /// </summary>
    /// <param name="I">Column index where splice begins.</param>
    /// <param name="other">Other array to splice in, must have same number of rows as this array.</param>
    /// <returns>Combined array with additional columns.</returns>
    public Array2D<BRAND> SpliceColumns(IntBl I, IBaseArray<BRAND> other) {
      (other.Row.Count == RowCount).Assert();
      var F0 = this.F;
      Func<IntBl,IntBl,BRAND> F1 = (i,j) => other.Column[i][j];
      return new Array2D<BRAND>(ColumnCount + other.Column.Count, RowCount,
                   (x, y) => (x < I).Condition(F0(x, y),
         (x < I + other.Column.Count).Condition(F1(x - I, y), F0(x - other.Column.Count, y))));
    }
    /// <summary>
    /// Map each element of the array according to a function into a new array of the same dimensions.
    /// </summary>
    /// <typeparam name="BRAND0">Type of new array.</typeparam>
    /// <param name="F">Function that maps the elements.</param>
    public Array2D<BRAND0> Map<BRAND0>(Func<IntBl,IntBl,BRAND,BRAND0> F) where BRAND0 : Brand<BRAND0> {
      var G = this.F;
      return new Array2D<BRAND0>(ColumnCount, RowCount, (i,j) => F(i,j, G(i,j)));
    }
    /// <summary>
    /// Map each element of the array according to a function into a new array of the same dimensions.
    /// </summary>
    /// <param name="F">Function that maps the elements.</param>
    public Array2D<BRAND> Map(Func<IntBl, IntBl, BRAND, BRAND> F) { return Map<BRAND>(F); }

    /// <summary>
    /// Combine two same-dimensions arrays together via a combining function.
    /// </summary>
    /// <typeparam name="BRAND0">Element type of other array.</typeparam>
    /// <typeparam name="BRAND1">Element type of resulting array.</typeparam>
    /// <param name="array">Other array to combine with this one.</param>
    /// <param name="F">Combining function.</param>
    public Array2D<BRAND1> Map<BRAND0, BRAND1>(Array2D<BRAND0> array, Func<BRAND, BRAND0, BRAND1> F) where BRAND0 : Brand<BRAND0> where BRAND1 : Brand<BRAND1> {
      (array.RowCount == RowCount && array.ColumnCount == ColumnCount).Assert();
      return new Array2D<BRAND1>(ColumnCount, RowCount, (i,j) => F(this.F(i,j), array.F(i,j)));
    }
    /// <summary>
    /// Combine two same-dimensions arrays together via a combining function.
    /// </summary>
    /// <param name="array">Other array to combine with this one.</param>
    /// <param name="F">Combining function.</param>
    public Array2D<BRAND> Map(Array2D<BRAND> array, Func<BRAND, BRAND, BRAND> F) { return Map<BRAND, BRAND>(array, F); }
  }

  public static partial class ArrayExtensions {
    public static Array2D<DoubleBl> Inverse(this Array2D<DoubleBl> M) {
      var DET_M = M.Determanent();
      //var M_T = M.Transpose;
      var M_A = M.Adjugate();
      return M_A.Multiply(1d / DET_M);
    }

    public static Array2D<DoubleBl> Adjugate(this Array2D<DoubleBl> Matrix) {
      (Matrix.Column.Count == Matrix.Row.Count).Assert();
      if (Matrix.Column.Count == 2) {
        var a = Matrix[0, 0];
        var b = Matrix[1, 0];
        var c = Matrix[0, 1];
        var d = Matrix[1, 1];
        var Table = new DoubleBl[] {
          d, -b, 
          -c, a,
        };
        return new Array2D<DoubleBl>(2, 2, (i, j) => Table.Table(i * 2 + j));
      }
      throw new NotSupportedException();
    }

    public static DoubleBl Determanent(this Array2D<DoubleBl> Matrix) {
      (Matrix.Column.Count == Matrix.Row.Count).Assert();
      if (Matrix.Column.Count == 2) {
        var a = Matrix[0,0];
        var b = Matrix[1,0];
        var c = Matrix[0,1];
        var d = Matrix[1,1];
        return a * d - b * c;
        // return a * d − b * c;
      } else if (Matrix.Column.Count == 3) {
        var a = Matrix[0, 0];
        var b = Matrix[1, 0];
        var c = Matrix[2, 0];

        var d = Matrix[0, 1];
        var e = Matrix[1, 1];
        var f = Matrix[2, 1];

        var g = Matrix[0, 2];
        var h = Matrix[1, 2];
        var i = Matrix[2, 2];
        return (a * e * i) - (a * f * h) - (b * d * i) + (b * f * g) + (c * d * h) - (c * e * g);
      }
      throw new NotSupportedException();

    }

    public static Array2D<BRAND> Identity<BRAND>(this BRAND One, int Dim) where BRAND : Brand<BRAND> {
      return Array2D<BRAND>.Identity(One, Dim);
    }
    public static Array2D<DoubleBl> Affine(this Array2D<DoubleBl> Self) {
      Self = Self.SpliceRows(Self.Row.Count, new Array1D<DoubleBl>(Self.Column.Count, i => 0d));
      Self = Self.SpliceColumns(Self.Column.Count, new Array1D<DoubleBl>(Self.Row.Count, i => (i == Self.Row.Count - 1).Condition(1d, 0d)).BaseTranspose);
      return Self;
    }
    public static Array2D<DoubleBl> ScaleB<T, BRAND>(this IBaseDoubleBl<T,BRAND> By) where BRAND : IBaseDoubleBl<T, BRAND> {
      var dim = Vecs.Arity<double, T>.ArityI.Count;
      return new Array2D<DoubleBl>(dim, dim, (i, j) => (i != j).Condition(0d.Bl(), By[i])).Affine();
    }
    public static Array2D<DoubleBl> ScaleB<T, BRAND, PORTABLE>(this IBaseDoubleBl<T, BRAND> Scale, IBasePointBl<PORTABLE> Center) where BRAND : IBaseDoubleBl<T, BRAND>, IBasePointBl<PORTABLE> where PORTABLE : IBasePointBl<PORTABLE> {
      var dim = Vecs.Arity<double, T>.ArityI.Count;
      var Self = new Array2D<DoubleBl>(dim, dim, (i, j) => (i != j).Condition(0d.Bl(), Scale[i]));
      Self = Self.SpliceRows(dim, new Array1D<DoubleBl>(dim, i => Center.Portable[i] - Scale[i] * Center.Portable[i]));
      Self = Self.SpliceColumns(dim, new Array1D<DoubleBl>(dim + 1, i => (i != dim).Condition(0d, 1d)).BaseTranspose);
      return Self;
    }
    public static Array2D<DoubleBl> Translate<T, BRAND>(this IBaseDoubleBl<T, BRAND> By) where BRAND : IBaseDoubleBl<T, BRAND> {
      var dim = Vecs.Arity<double, T>.ArityI.Count;
      var Self = 1d.Bl().Identity(dim);
      // splice in x,y,z as bottom row
      Self = Self.SpliceRows(dim, new Array1D<DoubleBl>(dim, i => By[i]));
      Self = Self.SpliceColumns(dim, new Array1D<DoubleBl>(dim + 1, i => (i != dim).Condition(0d, 1d)).BaseTranspose);
      return Self;
    }
    public static Array1D<BRAND> Add<BRAND>(this Array1D<BRAND> array, BRAND value) where BRAND : Brand<BRAND>, INumericBl<BRAND> {
      return array.Map(v => v.Add(value));
    }
    public static Array1D<BRAND> Subtract<BRAND>(this Array1D<BRAND> array, BRAND value) where BRAND : Brand<BRAND>, INumericBl<BRAND> {
      return array.Map(v => v.Subtract(value));
    }
    public static Array1D<BRAND> Multiply<BRAND>(this Array1D<BRAND> array, BRAND value) where BRAND : Brand<BRAND>, INumericBl<BRAND> {
      return array.Map(v => v.Multiply(value));
    }
    public static Array1D<BRAND> Divide<BRAND>(this Array1D<BRAND> array, BRAND value) where BRAND : Brand<BRAND>, INumericBl<BRAND> {
      return array.Map(v => v.Divide(value));
    }
    public static Array1D<BRAND> Modulo<BRAND>(this Array1D<BRAND> array, BRAND value) where BRAND : Brand<BRAND>, INumericBl<BRAND> {
      return array.Map(v => v.Modulo(value));
    }
    public static Array2D<BRAND> Add<BRAND>(this Array2D<BRAND> array, BRAND value) where BRAND : Brand<BRAND>, INumericBl<BRAND> {
      return array.Map((i,j,v) => v.Add(value));
    }
    public static Array2D<BRAND> Subtract<BRAND>(this Array2D<BRAND> array, BRAND value) where BRAND : Brand<BRAND>, INumericBl<BRAND> {
      return array.Map((i, j, v) => v.Subtract(value));
    }
    public static Array2D<BRAND> Multiply<BRAND>(this Array2D<BRAND> array, BRAND value) where BRAND : Brand<BRAND>, INumericBl<BRAND> {
      return array.Map((i, j, v) => v.Multiply(value));
    }
    public static Array2D<BRAND> Divide<BRAND>(this Array2D<BRAND> array, BRAND value) where BRAND : Brand<BRAND>, INumericBl<BRAND> {
      return array.Map((i, j, v) => v.Divide(value));
    }

    public static BRAND Dot<BRAND>(this Array1D<BRAND> arrayA, Array1D<BRAND> arrayB) where BRAND : Brand<BRAND>, INumericBl<BRAND> {
      (arrayA.Count == arrayB.Count).Assert();
      BRAND result = NumericFactory<BRAND>.Zero;
      for (int i = 0; i < arrayA.Count; i++) {
        result = result.Add(arrayA[i].Multiply(arrayB[i]));
      }
      return result;
    }
    public static IBaseArray<BRAND> Product<BRAND>(this IBaseArray<BRAND> mA, IBaseArray<BRAND> mB) where BRAND : Brand<BRAND>, INumericBl<BRAND> {
      (mA.Column.Count == mB.Row.Count).Assert();
      return new Array2D<BRAND>(mB.Column.Count, mA.Row.Count, (i, j) => mA.Row[j].Dot(mB.Column[i]));
    }
    public static PointBl Transform(this IBaseArray<DoubleBl> M, PointBl P) {
      (M.Column.Count == M.Row.Count).Assert();
      (M.Column.Count == 3).Assert();
      var xadd = P.Y * M[0, 1] + M[0, 2];
      var yadd = P.X * M[1, 0] + M[1, 2];

      var newX = P.X;
      newX *= M[0, 0];
      newX += xadd;

      var newY = P.Y;
      newY *= M[1, 1];
      newY += yadd;
      return new PointBl(newX, newY);
    }

    public static BRAND Product<T, K, BRAND, MONO>(this IBaseArray<MONO> mA, BaseNumericBl<T, K, BRAND, MONO> mB) where BRAND : BaseNumericBl<T, K, BRAND, MONO> where MONO : BaseNumericBl<K,K,MONO,MONO> {
      (mA.Column.Count == mA.Row.Count).Assert();
      (Vecs.Arity<K, T>.ArityI.Count == mA.Column.Count).Assert();
      Array1D<MONO> v0 = new Array1D<MONO>(mA.Column.Count, i => mB[i]);

      var rows = new MONO[mA.Column.Count];
      var v1 = new Array1D<MONO>(rows.Length, i => Dot<MONO>(mA.Row[i], v0));
      var Es = v1.ToArray<Expr<K>>(m => m);
      return Brand<BRAND>.ToBrand(Vecs.Arity<K, T>.ArityI.Make(Es));
    }


    public static void test() {
      Array1D<DoubleBl> array = null;
      DoubleBl d = null;
      array = array.Add(d);
    }
  }
}