using System;
using System.Collections.Generic;
using Bling.DSL;
using Bling.Util;
using Bling.Linq;
using Bling.Core;

namespace Bling.Matrices {
  using Bling.Vecs;
  using Microsoft.Linq.Expressions;
  using System.Reflection;

  public abstract class DNum {

  }

  public abstract class DNum<DIM> : DNum where DIM : DNum<DIM> {
    protected DNum() { throw new NotSupportedException("Singleton"); }
    public static int Count { get; protected set; }

    static DNum() {
      D1.CheckInit();
      D2.CheckInit();
      D3.CheckInit();
      D4.CheckInit();
    }
  }
  public interface INextDim<DIM, NEXT>
    where DIM : DNum<DIM>, INextDim<DIM, NEXT>
    where NEXT : DNum<NEXT>, IPrevDim<NEXT, DIM> { }

  public interface IPrevDim<DIM, PREV>
    where DIM : DNum<DIM>, IPrevDim<DIM, PREV>
    where PREV : DNum<PREV>, INextDim<PREV, DIM> { }

  public class D0 : DNum<D0>, INextDim<D0, D1> { static D0() { Count = 0; } internal static void CheckInit() { } }
  public class D1 : DNum<D1>, INextDim<D1, D2>, IPrevDim<D1, D0> { static D1() { Count = 1; } internal static void CheckInit() { } }
  public class D2 : DNum<D2>, INextDim<D2, D3>, IPrevDim<D2, D1> { static D2() { Count = 2; } internal static void CheckInit() { } }
  public class D3 : DNum<D3>, INextDim<D3, D4>, IPrevDim<D3, D2> { static D3() { Count = 3; } internal static void CheckInit() { } }
  public class D4 : DNum<D4>, INextDim<D4, D5>, IPrevDim<D4, D3> { static D4() { Count = 4; } internal static void CheckInit() { } }
  public class D5 : DNum<D5>, INextDim<D5, D6>, IPrevDim<D5, D4> { static D5() { Count = 5; } internal static void CheckInit() { } }
  public class D6 : DNum<D6>, INextDim<D6, D7>, IPrevDim<D6, D5> { static D6() { Count = 6; } internal static void CheckInit() { } }
  public class D7 : DNum<D7>, INextDim<D7, D8>, IPrevDim<D7, D6> { static D7() { Count = 7; } internal static void CheckInit() { } }
  public class D8 : DNum<D8>, IPrevDim<D8, D7> { static D8() { Count = 8; } internal static void CheckInit() { } }


  public interface IMatrixArity : IArity {
    int Width { get; }
    int Height { get; }
    //Expr AccessRow(Expr Matrix, int Rdx);
    //Expr FromRows(Expr[] Rows);
  }
  public interface IMatrixArity<K, T> : IMatrixArity, IArity<K, T> {

  }
  public interface IMatrix {

  }

  public sealed class Matrix<K, WIDTH, HEIGHT> : IMatrix
    where WIDTH : DNum<WIDTH>
    where HEIGHT : DNum<HEIGHT> {
    public static int Width { get { return DNum<WIDTH>.Count; } }
    public static int Height { get { return DNum<HEIGHT>.Count; } }
    public readonly K[] Values = new K[Width * Height];
    public K this[int Idx] {
      get { return Values[Idx]; }
      set { Values[Idx] = value; }
    }
    static Matrix() {
      Extensions.Trace("" + MatrixArity<K, WIDTH, HEIGHT>.ArityI);
    }
    private static int xxx;
    public static void CheckInit() { Extensions.Trace(typeof(Matrix<K, WIDTH, HEIGHT>).Name + " " + xxx); xxx += 1; }
    public K this[int Column, int Row] {
      get { return Values[Row * Width + Column]; }
      private set { Values[Row * Width + Column] = value; }
    }
    public Matrix() { }
    public Matrix(params K[] Values) {
      (this.Values.Length == Values.Length).Assert();
      for (int i = 0; i < Values.Length; i++) this[i] = Values[i];
    }
    public Matrix(K[,] Values) {
      (Values.GetLength(0) == Width).Assert();
      (Values.GetLength(1) == Height).Assert();
      for (int i = 0; i < Width; i++)
        for (int j = 0; j < Height; j++)
          this[i, j] = Values[i, j];
    }
    public override int GetHashCode() {
      int hc = 0;
      foreach (var v in Values) hc += v.GetHashCode();
      return hc;
    }
    public override bool Equals(object obj) {
      if (obj is Matrix<K, WIDTH, HEIGHT>) {
        var m = (Matrix<K, WIDTH, HEIGHT>)obj;
        for (int i = 0; i < Values.Length; i++)
          if (!Values[i].Equals(m.Values[i])) return false;
        return true;
      }
      return base.Equals(obj);
    }
    public override string ToString() {
      var str = "[";
      for (int j = 0; j < Height; j++) {
        str = str + (j == 0 ? "" : ", ") + "(";
        for (int i = 0; i < Width; i++)
          str = str + (i == 0 ? "" : ",") + this[i, j].ToString();
        str = str + ")";
      }
      return str + "]";
    }
  }
  public class MatrixArity<K, WIDTH, HEIGHT> : MultiArity<K, Matrix<K, WIDTH, HEIGHT>, MatrixArity<K, WIDTH, HEIGHT>>, IMatrixArity<K, Matrix<K, WIDTH, HEIGHT>>
    where WIDTH : DNum<WIDTH>
    where HEIGHT : DNum<HEIGHT> {
    public int Width { get { return DNum<WIDTH>.Count; } }
    public int Height { get { return DNum<HEIGHT>.Count; } }
    public override int Count { get { return Width * Height; } }
    public override K Access(Matrix<K, WIDTH, HEIGHT> Value, int Idx) { return Value[Idx]; }
    public override Matrix<K, WIDTH, HEIGHT> Make(params K[] Params) {
      return new Matrix<K, WIDTH, HEIGHT>(Params);
    }
    public override Expr<K> Access(Expr<Matrix<K, WIDTH, HEIGHT>> Value, int Idx) {
      return base.Access(Value, Idx);
    }


    public Expr<Matrix<K, WIDTH, HEIGHT>> Make(Expr<K>[,] Params) {
      var Es = new Expr<K>[Width * Height];
      for (int i = 0; i < Width; i++)
        for (int j = 0; j < Height; j++)
          Es[j * Width + i] = Params[i, j];
      return Make(Es);
    }
    public override string[] PropertyNames {
      get { throw new NotImplementedException(); }
    }
    public override Func<Expression, Expression> PropertyFor(int Idx) {
      //var Ps = typeof(Matrix<K, WIDTH, HEIGHT>).GetProperties();
      var p = typeof(Matrix<K, WIDTH, HEIGHT>).GetProperty("Item", typeof(K), new Type[] { typeof(int) });
      (p != null).Assert();
      return e => Expression.Property(e, p, Expression.Constant(Idx, typeof(int)));
    }
    public Expr<Matrix<K, HEIGHT, WIDTH>> Transpose(Expr<Matrix<K, WIDTH, HEIGHT>> e) {
      var Es = new Expr<K>[Width * Height];
      for (int i = 0; i < Width; i++)
        for (int j = 0; j < Height; j++)
          Es[i * Height + j] = Access(e, j * Width + i);
      return MatrixArity<K, HEIGHT, WIDTH>.ArityI.Make(Es);
    }
    internal static void CheckInit() {
      Extensions.Trace(typeof(MatrixArity<K, WIDTH, HEIGHT>) + " " + ArityI);
    }
  }


  public interface IMatrixBl<K, WIDTH, HEIGHT, MONO> : IBrandT<Matrix<K, WIDTH, HEIGHT>>
    where WIDTH : DNum<WIDTH>
    where HEIGHT : DNum<HEIGHT>
    where MONO : ValueBrand<K, K, MONO, MONO> {
    MONO this[int Column, int Row] { get; set; }
    MONO this[IntBl Column, IntBl Row] { get; set; }
    MONO this[int Idx] { get; set; }
    IMatrixBl<K, WIDTH, HEIGHT, MONO> BaseBind { set; }
    MatrixBl<K, WIDTH, HEIGHT, MONO>.IRowsSet Rows { get; }
    MatrixBl<K, WIDTH, HEIGHT, MONO>.IColumnsSet Columns { get; }
  }
  public interface INumericMatrixBl<K, WIDTH, HEIGHT, MONO> : IMatrixBl<K, WIDTH, HEIGHT, MONO>
    where WIDTH : DNum<WIDTH>
    where HEIGHT : DNum<HEIGHT>
    where MONO : BaseNumericBl<K, K, MONO, BoolBl, MONO> {
    INumericMatrixBl<K, HEIGHT2, HEIGHT, MONO> Product<HEIGHT2>(INumericMatrixBl<K, HEIGHT2, WIDTH, MONO> opB) where HEIGHT2 : DNum<HEIGHT2>;
  }
  public interface IMatrixBl<WIDTH, HEIGHT> : INumericMatrixBl<double, WIDTH, HEIGHT, DoubleBl>
    where WIDTH : DNum<WIDTH>
    where HEIGHT : DNum<HEIGHT> {
    MatrixBl<WIDTH, HEIGHT> Self { get; }
  }
  public interface IIntMatrixBl<WIDTH, HEIGHT> : INumericMatrixBl<int, WIDTH, HEIGHT, IntBl>
    where WIDTH : DNum<WIDTH>
    where HEIGHT : DNum<HEIGHT> {
    IntMatrixBl<WIDTH, HEIGHT> Self { get; }
  }

  public static class MatrixBl<K, WIDTH, HEIGHT, MONO>
    where WIDTH : DNum<WIDTH>
    where HEIGHT : DNum<HEIGHT>
    where MONO : ValueBrand<K, K, MONO, MONO> {
    public static Func<Expr<Matrix<K, WIDTH, HEIGHT>>, IMatrixBl<K, WIDTH, HEIGHT, MONO>> ToBrand { get; internal set; }
    static MatrixBl() {
      Matrix<K, WIDTH, HEIGHT>.CheckInit();
    }

    public interface IRowsSet {
      IMatrixBl<K, WIDTH, D1, MONO> this[IntBl Row] { get; set; }
    }
    public interface IColumnsSet {
      IMatrixBl<K, D1, HEIGHT, MONO> this[IntBl Column] { get; set; }
    }
  }
  public abstract class MatrixBl<K, WIDTH, HEIGHT, BRAND, MONO> : ValueBrand<Matrix<K, WIDTH, HEIGHT>, K, BRAND, MONO>, IMatrixBl<K, WIDTH, HEIGHT, MONO>
    where WIDTH : DNum<WIDTH>
    where HEIGHT : DNum<HEIGHT>
    where MONO : ValueBrand<K, K, MONO, MONO>
    where BRAND : MatrixBl<K, WIDTH, HEIGHT, BRAND, MONO> {
    public IMatrixBl<K, WIDTH, HEIGHT, MONO> BaseBind {
      set {
        this.Bind = ToBrand(value.Underlying);
      }
    }
    static MatrixBl() {
      MatrixBl<K, WIDTH, HEIGHT, MONO>.ToBrand = e => ToBrand(e);

      var Es = new MONO[MatrixArity<K, WIDTH, HEIGHT>.ArityI.Count];
      for (int i = 0; i < Es.Length; i++) {
        Es[i] = Brand<MONO>.Default;
      }
      Default = ToBrand(MatrixArity<K, WIDTH, HEIGHT>.ArityI.Make(Es.Underlying()));

    }
    public static implicit operator BRAND(MatrixBl<K, WIDTH, HEIGHT, BRAND, MONO> m) { return (BRAND)m; }

    public MatrixBl(Expr<Matrix<K, WIDTH, HEIGHT>> Underlying) : base(Underlying) { }
    public MatrixBl(params MONO[] Array) : base(MatrixArity<K, WIDTH, HEIGHT>.ArityI.Make(Array.Underlying())) { }
    public MatrixBl(MONO[,] Array) : base(MatrixArity<K, WIDTH, HEIGHT>.ArityI.Make(Array.Underlying())) { }
    public MatrixBl(Func<int, int, MONO> F) : this(ConvertF(F)) { }

    public MatrixBl(params IMatrixBl<K, D1, HEIGHT, MONO>[] Columns) : this(ConvertF(Columns)) { }
    public MatrixBl(params IMatrixBl<K, WIDTH, D1, MONO>[] Rows) : this(ConvertF(Rows)) { }
    public MatrixBl() : base() { }


    public class RowsSet : MatrixBl<K, WIDTH, HEIGHT, MONO>.IRowsSet {
      internal MatrixBl<K, WIDTH, HEIGHT, BRAND, MONO> Outer;
      public IMatrixBl<K, WIDTH, D1, MONO> this[IntBl Row] {
        get {
          var values = new Expr<K>[Width];
          for (int i = 0; i < Width; i++) values[i] = Outer[i, Row];
          return
            MatrixBl<K, WIDTH, D1, MONO>.ToBrand(
              MatrixArity<K, WIDTH, D1>.ArityI.Make(values));
        }
        set { this[Row].BaseBind = value; }
      }
    }
    public MatrixBl<K, WIDTH, HEIGHT, MONO>.IRowsSet Rows { get { return new RowsSet() { Outer = this }; } }
    public class ColumnsSet : MatrixBl<K, WIDTH, HEIGHT, MONO>.IColumnsSet {
      internal MatrixBl<K, WIDTH, HEIGHT, BRAND, MONO> Outer;
      public IMatrixBl<K, D1, HEIGHT, MONO> this[IntBl Column] {
        get {
          var values = new Expr<K>[Height];
          for (int i = 0; i < Height; i++) values[i] = Outer[Column, i];
          return
            MatrixBl<K, D1, HEIGHT, MONO>.ToBrand(
              MatrixArity<K, D1, HEIGHT>.ArityI.Make(values));
        }
        set { this[Column].BaseBind = value; }
      }
    }
    public MatrixBl<K, WIDTH, HEIGHT, MONO>.IColumnsSet Columns { get { return new ColumnsSet() { Outer = this }; } }


    private static MONO[,] ConvertF(IMatrixBl<K, D1, HEIGHT, MONO>[] Columns) {
      (Columns.Length == Width).Assert();
      var ret = new MONO[Width, Height];
      for (int i = 0; i < Width; i++)
        for (int j = 0; j < Height; j++)
          ret[i, j] = Columns[i][0, j];
      return ret;
    }
    private static MONO[,] ConvertF(IMatrixBl<K, WIDTH, D1, MONO>[] Rows) {
      (Rows.Length == Height).Assert();
      var ret = new MONO[Width, Height];
      for (int i = 0; i < Width; i++)
        for (int j = 0; j < Height; j++)
          ret[i, j] = Rows[j][i, 0];
      return ret;
    }

    private static MONO[,] ConvertF(Func<int, int, MONO> F) {
      var ret = new MONO[Width, Height];
      for (int i = 0; i < Width; i++)
        for (int j = 0; j < Height; j++)
          ret[i, j] = F(i, j);
      return ret;
    }

    public static int Width { get { return DNum<WIDTH>.Count; } }
    public static int Height { get { return DNum<HEIGHT>.Count; } }

    public MONO this[int Column, int Row] {
      get { return Brand<K, MONO>.ToBrand(MatrixArity<K, WIDTH, HEIGHT>.ArityI.Access(Underlying, Row * Width + Column)); }
      set { this[Column, Row] = value; }
    }
    public MONO this[IntBl Column, IntBl Row] {
      get { return this[Row * Width + Column]; }
      set { this[Column, Row] = value; }
    }
    public new MONO this[int Idx] {
      get { return Brand<K, MONO>.ToBrand(MatrixArity<K, WIDTH, HEIGHT>.ArityI.Access(Underlying, Idx)); }
      set { this[Idx] = value; }
    }
    public MONO[] ToArray() {
      var ret = new MONO[Width * Height];
      for (int i = 0; i < ret.Length; i++) ret[i] = this[i];
      return ret;
    }
    protected class TransposeOp : Ops.Operator<Matrix<K, WIDTH, HEIGHT>, Matrix<K, HEIGHT, WIDTH>, TransposeOp>, Ops.IMemoOperator<Matrix<K, HEIGHT, WIDTH>> {
      public new static TransposeOp Instance = new TransposeOp();
      private TransposeOp() { }

      public override string Format(string argA) { return "Transpose(" + argA + ")"; }
      public override Expr<Matrix<K, HEIGHT, WIDTH>> Expand(Expr<Matrix<K, WIDTH, HEIGHT>> argA) {
        return MatrixArity<K, WIDTH, HEIGHT>.ArityI.Transpose(argA);
      }
      public override Func<string, string> Shader(Bling.Ops.Operation<Matrix<K, WIDTH, HEIGHT>, Matrix<K, HEIGHT, WIDTH>> op) {
        if (typeof(WIDTH) == typeof(D1) ||
            typeof(HEIGHT) == typeof(D1)) return null;
        return a => "transpose(" + a + ")";
      }
    }

  }

  public abstract class NumericMatrixBl<K, A, B, BRAND, MONO> :
    MatrixBl<K, A, B, BRAND, MONO>,
    INumericMatrixBl<K, A, B, MONO>
    where A : DNum<A>
    where B : DNum<B>
    where MONO : Core.BaseNumericBl<K, K, MONO, BoolBl, MONO>
    where BRAND : NumericMatrixBl<K, A, B, BRAND, MONO> {
    public NumericMatrixBl(Expr<Matrix<K, A, B>> Underlying) : base(Underlying) { }
    public NumericMatrixBl(params MONO[] Array) : base(Array) { }
    public NumericMatrixBl(MONO[,] Array) : base(Array) { }
    public NumericMatrixBl(params IMatrixBl<K, D1, B, MONO>[] Columns) : base(Columns) { }
    public NumericMatrixBl(params IMatrixBl<K, A, D1, MONO>[] Rows) : base(Rows) { }
    public NumericMatrixBl(Func<int, int, MONO> F) : base(F) { }
    public NumericMatrixBl() : base() { }

    public static implicit operator BRAND(NumericMatrixBl<K, A, B, BRAND, MONO> m) { return (BRAND)m; }

    public BRAND Self { get { return (BRAND)this; } }

    protected class ProductOp<C> : Ops.Operator<Matrix<K, A, B>, Matrix<K, C, A>, Matrix<K, C, B>, ProductOp<C>>, Ops.IMemoOperator<Matrix<K, C, B>>
      where C : DNum<C> {
      public new static readonly ProductOp<C> Instance = new ProductOp<C>();
      private ProductOp() { }
      public override string Format(string argA, string argB) { return argA + " * " + argB; }
      public override Expr<Matrix<K, C, B>> Expand(Expr<Matrix<K, A, B>> argA,
                                                   Expr<Matrix<K, C, A>> argB) {
        // (b,a) * (a,c) => (b,c)
        MatrixBl<K, A, B, BRAND, MONO> MBA = ToBrand(argA);
        IMatrixBl<K, C, A, MONO> MAC = MatrixBl<K, C, A, MONO>.ToBrand(argB);
        var Es = new Expr<K>[DNum<A>.Count * DNum<C>.Count];

        for (int c = 0; c < DNum<C>.Count; c++)
          for (int b = 0; b < DNum<B>.Count; b++)
            Es[b * DNum<C>.Count + c] = MBA.Rows[b].Dot(MAC.Columns[c]);

        return MatrixArity<K, C, B>.ArityI.Make(Es);
      }
      public override Func<string, string, string> Shader<EVAL>(Shaders.BaseShaderEval<EVAL> txt, Bling.Ops.Operation<Matrix<K, A, B>, Matrix<K, C, A>, Matrix<K, C, B>> op) {
        return (a, b) => "mul(" + a + ", " + b + ")";
      }
    }


    public INumericMatrixBl<K, HEIGHT2, B, MONO> Product<HEIGHT2>(INumericMatrixBl<K, HEIGHT2, A, MONO> opB) where HEIGHT2 : DNum<HEIGHT2> {
      return (INumericMatrixBl<K, HEIGHT2, B, MONO>)
        MatrixBl<K, HEIGHT2, B, MONO>.ToBrand(ProductOp<HEIGHT2>.Instance.Make(this, opB.Underlying));
    }
    public static INumericMatrixBl<K, B, B, MONO> operator *(NumericMatrixBl<K, A, B, BRAND, MONO> opA, INumericMatrixBl<K, B, A, MONO> opB) {
      return opA.Product(opB);
    }
    public static INumericMatrixBl<K, D1, B, MONO> operator *(NumericMatrixBl<K, A, B, BRAND, MONO> opA, INumericMatrixBl<K, D1, A, MONO> opB) {
      return opA.Product(opB);
    }
    public static INumericMatrixBl<K, D2, B, MONO> operator *(NumericMatrixBl<K, A, B, BRAND, MONO> opA, INumericMatrixBl<K, D2, A, MONO> opB) {
      return opA.Product(opB);
    }
    public static INumericMatrixBl<K, D3, B, MONO> operator *(NumericMatrixBl<K, A, B, BRAND, MONO> opA, INumericMatrixBl<K, D3, A, MONO> opB) {
      return opA.Product(opB);
    }
    public static INumericMatrixBl<K, D4, B, MONO> operator *(NumericMatrixBl<K, A, B, BRAND, MONO> opA, INumericMatrixBl<K, D4, A, MONO> opB) {
      return opA.Product(opB);
    }
    public static INumericMatrixBl<K, A, D1, MONO> operator *(INumericMatrixBl<K, B, D1, MONO> opA, NumericMatrixBl<K, A, B, BRAND, MONO> opB) {
      return opA.Product(opB);
    }
    public static INumericMatrixBl<K, A, D2, MONO> operator *(INumericMatrixBl<K, B, D2, MONO> opA, NumericMatrixBl<K, A, B, BRAND, MONO> opB) {
      return opA.Product(opB);
    }
    public static INumericMatrixBl<K, A, D3, MONO> operator *(INumericMatrixBl<K, B, D3, MONO> opA, NumericMatrixBl<K, A, B, BRAND, MONO> opB) {
      return opA.Product(opB);
    }
    public static INumericMatrixBl<K, A, D4, MONO> operator *(INumericMatrixBl<K, B, D4, MONO> opA, NumericMatrixBl<K, A, B, BRAND, MONO> opB) {
      return opA.Product(opB);
    }
  }
  public interface ICanBeMatrixBl<WIDTH, HEIGHT>
    where WIDTH : DNum<WIDTH>
    where HEIGHT : DNum<HEIGHT> {
    MatrixBl<WIDTH, HEIGHT> Matrix { get; }
  }
  public abstract class BaseMatrixBl<WIDTH, HEIGHT> : NumericMatrixBl<double, WIDTH, HEIGHT, MatrixBl<WIDTH, HEIGHT>, DoubleBl>, IMatrixBl<WIDTH, HEIGHT>, ICanBeMatrixBl<WIDTH, HEIGHT>
    where WIDTH : DNum<WIDTH>
    where HEIGHT : DNum<HEIGHT> {
    public BaseMatrixBl(Expr<Matrix<double, WIDTH, HEIGHT>> Underlying) : base(Underlying) { }
    public BaseMatrixBl(params DoubleBl[] Array) : base(Array) { }
    public BaseMatrixBl(DoubleBl[,] Array) : base(Array) { }
        public BaseMatrixBl() : base() { }

    public BaseMatrixBl(params IMatrixBl<double, D1, HEIGHT, DoubleBl>[] Columns) : base(Columns) { }
    public BaseMatrixBl(params IMatrixBl<double, WIDTH, D1, DoubleBl>[] Rows) : base(Rows) { }
    public BaseMatrixBl(Func<int, int, DoubleBl> F) : base(F) { }

    public MatrixBl<WIDTH, HEIGHT> Matrix { get { return (MatrixBl<WIDTH, HEIGHT>)this; } }
  }


  public class MatrixBl<WIDTH, HEIGHT> : BaseMatrixBl<WIDTH, HEIGHT>
    where WIDTH : DNum<WIDTH>
    where HEIGHT : DNum<HEIGHT> {
    public MatrixBl(Expr<Matrix<double, WIDTH, HEIGHT>> Underlying) : base(Underlying) { }
    public MatrixBl(params DoubleBl[] Array) : base(Array) { }
    public MatrixBl(DoubleBl[,] Array) : base(Array) { }
    public MatrixBl() : base() { }

    public MatrixBl(params IMatrixBl<double, D1, HEIGHT, DoubleBl>[] Columns) : base(Columns) { }
    public MatrixBl(params IMatrixBl<double, WIDTH, D1, DoubleBl>[] Rows) : base(Rows) { }
    public MatrixBl(Func<int, int, DoubleBl> F) : base(F) { }


    /// <summary>
    /// Create a matrix out of rows (can be points)
    /// </summary>
    public static MatrixBl<WIDTH, HEIGHT> FromRows(params MatrixBl<WIDTH, D1>[] Rows) {
      return new MatrixBl<WIDTH, HEIGHT>(Rows);
    }
    /// <summary>
    /// Create a matrix out of columns (can be points)
    /// </summary>
    public static MatrixBl<WIDTH, HEIGHT> FromColumns(params MatrixBl<D1, HEIGHT>[] Columns) {
      return new MatrixBl<WIDTH, HEIGHT>(Columns);
    }


    public static implicit operator MatrixBl<WIDTH, HEIGHT>(Expr<Matrix<double, WIDTH, HEIGHT>> e) { return new MatrixBl<WIDTH, HEIGHT>(e); }
    public new MatrixBl<WIDTH, HEIGHT> Self { get { return this; } }
    public static MatrixBl<WIDTH, WIDTH> Identity {
      get { return new MatrixBl<WIDTH, WIDTH>((i, j) => i == j ? 1d : 0d); }
    }
    public static MatrixBl<WIDTH, HEIGHT> Zero {
      get { return new MatrixBl<WIDTH, HEIGHT>((i, j) => 0d); }
    }
    public MatrixBl<HEIGHT, WIDTH> Transpose {
      get { return TransposeOp.Instance.Make(Underlying); }
      set { Transpose.Bind = value; }
    }
    public MatrixBl<HEIGHT2, HEIGHT> Product<HEIGHT2>(MatrixBl<HEIGHT2, WIDTH> opB) where HEIGHT2 : DNum<HEIGHT2> {
      return base.Product<HEIGHT2>(opB).Underlying;
    }

    public static MatrixBl<HEIGHT, HEIGHT> operator *(MatrixBl<WIDTH, HEIGHT> opA, ICanBeMatrixBl<HEIGHT, WIDTH> opB) {
      return opA.Product(opB.Matrix);
    }
    public static MatrixBl<WIDTH, WIDTH> operator *(ICanBeMatrixBl<HEIGHT, WIDTH> opA, MatrixBl<WIDTH, HEIGHT> opB) {
      return opA.Matrix.Product(opB);
    }

    public static MatrixBl<HEIGHT, HEIGHT> operator *(MatrixBl<WIDTH, HEIGHT> opA, MatrixBl<HEIGHT, WIDTH> opB) {
      return opA.Product(opB.Self);
    }
    public static MatrixBl<WIDTH, WIDTH> operator *(BaseMatrixBl<HEIGHT, WIDTH> opA, MatrixBl<WIDTH, HEIGHT> opB) {
      return opA.Self.Product(opB);
    }

    public static MatrixBl<D1, HEIGHT> operator *(MatrixBl<WIDTH, HEIGHT> opA, BaseMatrixBl<D1, WIDTH> opB) {
      return opA.Product(opB.Self);
    }
    public static MatrixBl<D2, HEIGHT> operator *(MatrixBl<WIDTH, HEIGHT> opA, BaseMatrixBl<D2, WIDTH> opB) {
      return opA.Product(opB.Self);
    }
    public static MatrixBl<D3, HEIGHT> operator *(MatrixBl<WIDTH, HEIGHT> opA, BaseMatrixBl<D3, WIDTH> opB) {
      return opA.Product(opB.Self);
    }

    public static MatrixBl<D4, HEIGHT> operator *(MatrixBl<WIDTH, HEIGHT> opA, BaseMatrixBl<D4, WIDTH> opB) {
      return opA.Product(opB.Self);
    }
    public static MatrixBl<WIDTH, D1> operator *(BaseMatrixBl<HEIGHT, D1> opA, MatrixBl<WIDTH, HEIGHT> opB) {
      return opA.Self.Product(opB);
    }
    public static MatrixBl<WIDTH, D2> operator *(BaseMatrixBl<HEIGHT, D2> opA, MatrixBl<WIDTH, HEIGHT> opB) {
      return opA.Self.Product(opB);
    }
    public static MatrixBl<WIDTH, D3> operator *(BaseMatrixBl<HEIGHT, D3> opA, MatrixBl<WIDTH, HEIGHT> opB) {
      return opA.Self.Product(opB);
    }
    public static MatrixBl<WIDTH, D4> operator *(BaseMatrixBl<HEIGHT, D4> opA, MatrixBl<WIDTH, HEIGHT> opB) {
      return opA.Self.Product(opB);
    }
    static MatrixBl() {
      Register(s => (s));
      MatrixArity<double, WIDTH, HEIGHT>.CheckInit();

      MatrixBl<D1, D1>.CheckInit();
      MatrixBl<D1, D2>.CheckInit();
      MatrixBl<D1, D3>.CheckInit();
      MatrixBl<D1, D4>.CheckInit();

      MatrixBl<D2, D1>.CheckInit();
      MatrixBl<D2, D2>.CheckInit();
      MatrixBl<D2, D3>.CheckInit();
      MatrixBl<D2, D4>.CheckInit();

      MatrixBl<D3, D1>.CheckInit();
      MatrixBl<D3, D2>.CheckInit();
      MatrixBl<D3, D3>.CheckInit();
      MatrixBl<D3, D4>.CheckInit();

      MatrixBl<D4, D1>.CheckInit();
      MatrixBl<D4, D2>.CheckInit();
      MatrixBl<D4, D3>.CheckInit();
      MatrixBl<D4, D4>.CheckInit();
    }
    private static void CheckInit() {
      Extensions.Trace(typeof(MatrixBl<WIDTH, HEIGHT>).ToString());
    }
  }
  public class IntMatrixBl<WIDTH, HEIGHT> : NumericMatrixBl<int, WIDTH, HEIGHT, IntMatrixBl<WIDTH, HEIGHT>, IntBl>, IIntMatrixBl<WIDTH, HEIGHT>
    where WIDTH : DNum<WIDTH>
    where HEIGHT : DNum<HEIGHT> {
    public IntMatrixBl(Expr<Matrix<int, WIDTH, HEIGHT>> Underlying) : base(Underlying) { }
    public IntMatrixBl(params IntBl[] Array) : base(Array) { }
    public IntMatrixBl(IntBl[,] Array) : base(Array) { }

    public IntMatrixBl(params IntMatrixBl<D1, HEIGHT>[] Columns) : base(Columns) { }
    public IntMatrixBl(params IntMatrixBl<WIDTH, D1>[] Rows) : base(Rows) { }
    public IntMatrixBl(Func<int, int, IntBl> F) : base(F) { }
    public static implicit operator IntMatrixBl<WIDTH, HEIGHT>(Expr<Matrix<int, WIDTH, HEIGHT>> e) { return new IntMatrixBl<WIDTH, HEIGHT>(e); }
    public IntMatrixBl<WIDTH, HEIGHT> Self { get { return this; } }
    public static IntMatrixBl<WIDTH, WIDTH> Identity {
      get { return new IntMatrixBl<WIDTH, WIDTH>((i, j) => i == j ? 1 : 0); }
    }
    public static IntMatrixBl<WIDTH, HEIGHT> Zero {
      get { return new IntMatrixBl<WIDTH, HEIGHT>((i, j) => 0); }
    }
    public IntMatrixBl<HEIGHT, WIDTH> Transpose {
      get { return TransposeOp.Instance.Make(Underlying); }
      set { Transpose.Bind = value; }
    }
    public IntMatrixBl<HEIGHT2, HEIGHT> Product<HEIGHT2>(IntMatrixBl<HEIGHT2, WIDTH> opB) where HEIGHT2 : DNum<HEIGHT2> {
      return base.Product<HEIGHT2>(opB).Underlying;
    }
    public static IntMatrixBl<HEIGHT, HEIGHT> operator *(IntMatrixBl<WIDTH, HEIGHT> opA, IntMatrixBl<HEIGHT, WIDTH> opB) {
      return opA.Product(opB);
    }
    public static IntMatrixBl<D1, HEIGHT> operator *(IntMatrixBl<WIDTH, HEIGHT> opA, IIntMatrixBl<D1, WIDTH> opB) {
      return opA.Product(opB.Self);
    }
    public static IntMatrixBl<D2, HEIGHT> operator *(IntMatrixBl<WIDTH, HEIGHT> opA, IIntMatrixBl<D2, WIDTH> opB) {
      return opA.Product(opB.Self);
    }
    public static IntMatrixBl<D3, HEIGHT> operator *(IntMatrixBl<WIDTH, HEIGHT> opA, IIntMatrixBl<D3, WIDTH> opB) {
      return opA.Product(opB.Self);
    }
    public static IntMatrixBl<D4, HEIGHT> operator *(IntMatrixBl<WIDTH, HEIGHT> opA, IIntMatrixBl<D4, WIDTH> opB) {
      return opA.Product(opB.Self);
    }
    public static IntMatrixBl<WIDTH, D1> operator *(IIntMatrixBl<HEIGHT, D1> opA, IntMatrixBl<WIDTH, HEIGHT> opB) {
      return opA.Self.Product(opB);
    }
    public static IntMatrixBl<WIDTH, D2> operator *(IIntMatrixBl<HEIGHT, D2> opA, IntMatrixBl<WIDTH, HEIGHT> opB) {
      return opA.Self.Product(opB);
    }
    public static IntMatrixBl<WIDTH, D3> operator *(IIntMatrixBl<HEIGHT, D3> opA, IntMatrixBl<WIDTH, HEIGHT> opB) {
      return opA.Self.Product(opB);
    }
    public static IntMatrixBl<WIDTH, D4> operator *(IIntMatrixBl<HEIGHT, D4> opA, IntMatrixBl<WIDTH, HEIGHT> opB) {
      return opA.Self.Product(opB);
    }
    static IntMatrixBl() {
      Register(s => (s));
      MatrixArity<int, WIDTH, HEIGHT>.CheckInit();

      IntMatrixBl<D1, D1>.CheckInit();
      IntMatrixBl<D1, D2>.CheckInit();
      IntMatrixBl<D1, D3>.CheckInit();
      IntMatrixBl<D1, D4>.CheckInit();

      IntMatrixBl<D2, D1>.CheckInit();
      IntMatrixBl<D2, D2>.CheckInit();
      IntMatrixBl<D2, D3>.CheckInit();
      IntMatrixBl<D2, D4>.CheckInit();

      IntMatrixBl<D3, D1>.CheckInit();
      IntMatrixBl<D3, D2>.CheckInit();
      IntMatrixBl<D3, D3>.CheckInit();
      IntMatrixBl<D3, D4>.CheckInit();

      IntMatrixBl<D4, D1>.CheckInit();
      IntMatrixBl<D4, D2>.CheckInit();
      IntMatrixBl<D4, D3>.CheckInit();
      IntMatrixBl<D4, D4>.CheckInit();
    }
    private static void CheckInit() {
      Extensions.Trace(typeof(IntMatrixBl<WIDTH, HEIGHT>).ToString());
    }
  }

  public static partial class MatrixExtensions {
    public static MatrixBl<DIM_NEXT, DIM_NEXT> Scale<PORTABLE, DIM, DIM_NEXT>(this IDimPointBl<PORTABLE, DIM, DIM_NEXT> Scale,
                                                                                   IDimPointBl<PORTABLE, DIM, DIM_NEXT> Center)
      where PORTABLE : IDimPointBl<PORTABLE, DIM, DIM_NEXT>
      where DIM : DNum<DIM>, INextDim<DIM, DIM_NEXT>
      where DIM_NEXT : DNum<DIM_NEXT>, IPrevDim<DIM_NEXT, DIM> {
      return new MatrixBl<DIM_NEXT, DIM_NEXT>((i, j) =>
        (i == j && i < DNum<DIM>.Count) ? Scale[i] :
        (i != j && i == DNum<DIM>.Count && Center != null) ? Center.Portable[j] - Scale[j] * Center.Portable[j] :
        (i == j) ? 1d : 0d);
    }
    public static MatrixBl<DIM_NEXT, DIM_NEXT> Scale<PORTABLE, DIM, DIM_NEXT>(this IDimPointBl<PORTABLE, DIM, DIM_NEXT> Point)
      where PORTABLE : IDimPointBl<PORTABLE, DIM, DIM_NEXT>
      where DIM : DNum<DIM>, INextDim<DIM, DIM_NEXT>
      where DIM_NEXT : DNum<DIM_NEXT>, IPrevDim<DIM_NEXT, DIM> {
      return Point.Scale(null);
    }
    /*
    public static MatrixBl<DIM_NEXT, DIM_NEXT> Scale<DIM, DIM_NEXT>(this DoubleBl Value)
      where DIM : DNum<DIM>, INextDim<DIM, DIM_NEXT>
      where DIM_NEXT : DNum<DIM_NEXT>, IPrevDim<DIM_NEXT, DIM> {


      //return Point.Scale(null);
    }
     */
    public static MatrixBl<DIM_NEXT, DIM_NEXT> Translate<PORTABLE, DIM, DIM_NEXT>(this IDimPointBl<PORTABLE, DIM, DIM_NEXT> Point)
      where PORTABLE : IDimPointBl<PORTABLE, DIM, DIM_NEXT>
      where DIM : DNum<DIM>, INextDim<DIM, DIM_NEXT>
      where DIM_NEXT : DNum<DIM_NEXT>, IPrevDim<DIM_NEXT, DIM> {
      return new MatrixBl<DIM_NEXT, DIM_NEXT>((i, j) =>
        (i == j) ? 1d : (i == DNum<DIM>.Count) ? Point[j] : 0d);
    }
    /*
    public static DoubleBl Dot<DIM>(this MatrixBl<DIM, D1> Row, MatrixBl<D1, DIM> Column) where DIM : DNum<DIM> {
      DoubleBl Result = 0d;
      for (int i = 0; i < DNum<DIM>.Count; i++)
        Result = Result + (Row[i, 0] * Column[0, i]);
      return Result;
    }
     */
    public static MatrixBl<DN, DN> Pad<D, DN>(this MatrixBl<D, D> Matrix)
      where D : DNum<D>, INextDim<D, DN>
      where DN : DNum<DN>, IPrevDim<DN, D> {

      var Es = new DoubleBl[DNum<DN>.Count, DNum<DN>.Count];
      for (int i = 0; i < DNum<D>.Count; i++) {
        for (int j = 0; j < DNum<D>.Count; j++) {
          Es[i, j] = Matrix[i, j];
        }
        Es[i, DNum<D>.Count] = 0d;
      }
      for (int j = 0; j <= DNum<D>.Count; j++) {
        Es[DNum<D>.Count, j] = (j == DNum<D>.Count) ? 1d : 0d;
      }
      return new MatrixBl<DN, DN>(Es);
    }


    public static MatrixBl<WIDTH_NEXT, HEIGHT> SpliceColumn<WIDTH, WIDTH_NEXT, HEIGHT>(this MatrixBl<WIDTH, HEIGHT> Matrix, int ColumnAt, MatrixBl<D1, HEIGHT> Column)
      where WIDTH : DNum<WIDTH>, INextDim<WIDTH, WIDTH_NEXT>
      where WIDTH_NEXT : DNum<WIDTH_NEXT>, IPrevDim<WIDTH_NEXT, WIDTH>
      where HEIGHT : DNum<HEIGHT> {
      var Es = new DoubleBl[DNum<WIDTH_NEXT>.Count * DNum<HEIGHT>.Count];
      for (int i = 0; i < ColumnAt; i++) {
        for (int j = 0; j < DNum<HEIGHT>.Count; j++) {
          Es[j * DNum<WIDTH_NEXT>.Count + i] = Matrix[i, j];
        }
      }
      for (int j = 0; j < DNum<HEIGHT>.Count; j++)
        Es[j * DNum<WIDTH_NEXT>.Count + ColumnAt] = Column[0, j];
      for (int i = ColumnAt + 1; i < DNum<WIDTH>.Count + 1; i++)
        for (int j = 0; j < DNum<HEIGHT>.Count; j++) {
          Es[j * DNum<WIDTH_NEXT>.Count + i] = Matrix[i - 1, j];
        }
      return new MatrixBl<WIDTH_NEXT, HEIGHT>(Es);
    }
    public static MatrixBl<WIDTH, HEIGHT_NEXT> SpliceRow<WIDTH, HEIGHT, HEIGHT_NEXT>(this MatrixBl<WIDTH, HEIGHT> Matrix, int RowAt, MatrixBl<WIDTH, D1> Row)
      where HEIGHT : DNum<HEIGHT>, INextDim<HEIGHT, HEIGHT_NEXT>
      where HEIGHT_NEXT : DNum<HEIGHT_NEXT>, IPrevDim<HEIGHT_NEXT, HEIGHT>
      where WIDTH : DNum<WIDTH> {
      var Es = new DoubleBl[DNum<WIDTH>.Count * DNum<HEIGHT_NEXT>.Count];
      for (int i = 0; i < DNum<WIDTH>.Count; i++) {
        for (int j = 0; j < RowAt; j++)
          Es[j * DNum<WIDTH>.Count + i] = Matrix[i, j];

        Es[RowAt * DNum<WIDTH>.Count + i] = Row[i, 0];

        for (int j = RowAt + 1; j < DNum<HEIGHT>.Count + 1; j++)
          Es[j * DNum<WIDTH>.Count + i] = Matrix[i, j - 1];
      }
      return new MatrixBl<WIDTH, HEIGHT_NEXT>(Es);
    }

    public static MatrixBl<DN, DN> Expand<D, DN>(this MatrixBl<D, D> Matrix)
      where D : DNum<D>, INextDim<D, DN>
      where DN : DNum<DN>, IPrevDim<DN, D> {
      var ret = Matrix.Expand<D, DN>(DNum<D>.Count);
      return ret;
    }
    public static MatrixBl<DN, DN> Expand<D, DN>(this MatrixBl<D, D> Matrix, int n)
      where D : DNum<D>, INextDim<D, DN>
      where DN : DNum<DN>, IPrevDim<DN, D> {

      var m0 = Matrix.SpliceRow<D, D, DN>(n,
        new MatrixBl<D, D1>((x, y) => 0d));
      var m1 = m0.SpliceColumn<D, DN, DN>(n, new MatrixBl<D1, DN>((x, y) => (y == n) ? 1d : 0d));
      return m1;

    }


    public static DoubleBl M11(this MatrixBl<D3, D3> M) { return M[0, 0]; }
    public static DoubleBl M12(this MatrixBl<D3, D3> M) { return M[1, 0]; }
    public static DoubleBl M21(this MatrixBl<D3, D3> M) { return M[0, 1]; }
    public static DoubleBl M22(this MatrixBl<D3, D3> M) { return M[1, 1]; }
    public static DoubleBl OffsetX(this MatrixBl<D3, D3> M) { return M[0, 2]; }
    public static DoubleBl OffsetY(this MatrixBl<D3, D3> M) { return M[1, 2]; }

    public static BRAND Transform<T, BRAND, ARITY>(this MatrixBl<D3, D3> M, PointBl<T, BRAND, ARITY> P)
      where BRAND : PointBl<T, BRAND, ARITY>
      where ARITY : Arity2<double, T, ARITY>, new() {

      var xadd = P.Y * M.M21() + M.OffsetX();
      var yadd = P.X * M.M12() + M.OffsetY();

      var newX = P.X;
      newX *= M.M11();
      newX += xadd;

      var newY = P.Y;
      newY *= M.M22();
      newY += yadd;

      return PointBl<T, BRAND, ARITY>.Make(newX, newY);
    }
    public static MONO Dot<DIM, K, MONO>(this IMatrixBl<K, DIM, D1, MONO> Column, IMatrixBl<K, D1, DIM, MONO> Row)
      where DIM : DNum<DIM>
      where MONO : BaseNumericBl<K, K, MONO, MONO> {
      return BaseNumericBl<K, K, MONO, MONO>.ToBrand(Dot<K, DIM>.Instance.Make(Column.Underlying, Row.Underlying));
    }


    public static DoubleBl M00(this MatrixBl<D2, D2> M) { return M[0, 0]; }
    public static DoubleBl M01(this MatrixBl<D2, D2> M) { return M[1, 0]; }
    public static DoubleBl M10(this MatrixBl<D2, D2> M) { return M[0, 1]; }
    public static DoubleBl M11(this MatrixBl<D2, D2> M) { return M[1, 1]; }

    public static DoubleBl M00(this MatrixBl<D4, D4> M) { return M[0, 0]; }
    public static DoubleBl M01(this MatrixBl<D4, D4> M) { return M[1, 0]; }
    public static DoubleBl M02(this MatrixBl<D4, D4> M) { return M[2, 0]; }
    public static DoubleBl M03(this MatrixBl<D4, D4> M) { return M[3, 0]; }
    public static DoubleBl M10(this MatrixBl<D4, D4> M) { return M[0, 1]; }
    public static DoubleBl M11(this MatrixBl<D4, D4> M) { return M[1, 1]; }
    public static DoubleBl M12(this MatrixBl<D4, D4> M) { return M[2, 1]; }
    public static DoubleBl M13(this MatrixBl<D4, D4> M) { return M[3, 1]; }
    public static DoubleBl M20(this MatrixBl<D4, D4> M) { return M[0, 2]; }
    public static DoubleBl M21(this MatrixBl<D4, D4> M) { return M[1, 2]; }
    public static DoubleBl M22(this MatrixBl<D4, D4> M) { return M[2, 2]; }
    public static DoubleBl M23(this MatrixBl<D4, D4> M) { return M[3, 2]; }
    public static DoubleBl M30(this MatrixBl<D4, D4> M) { return M[0, 3]; }
    public static DoubleBl M31(this MatrixBl<D4, D4> M) { return M[1, 3]; }
    public static DoubleBl M32(this MatrixBl<D4, D4> M) { return M[2, 3]; }
    public static DoubleBl OffsetX(this MatrixBl<D4, D4> M) { return M[0, 3]; }
    public static DoubleBl OffsetY(this MatrixBl<D4, D4> M) { return M[1, 3]; }
    public static DoubleBl OffsetZ(this MatrixBl<D4, D4> M) { return M[2, 3]; }
    public static DoubleBl M33(this MatrixBl<D4, D4> M) { return M[3, 3]; }

    public static Point3DBl Transform(this MatrixBl<D4, D4> M, Point3DBl P) {
      return (Point3DBl)(Point4DBl) (M * P);
    }
    /*
    public static BRAND Transform<T, BRAND, ARITY>(this MatrixBl<D4, D4> M, Point3DBl<T, BRAND, ARITY> P)
      where BRAND : Point3DBl<T, BRAND, ARITY>
      where ARITY : Arity3<double, T, ARITY>, new() {

      var PM = (Point3DBl)P;
      MatrixBl<D1,D4> QM = M * PM;
      var QN = (Point4DBl)QM;
      var QP = QN.XYZ() / QN.W;
      return (Point3DBl<T, BRAND, ARITY>) QP;

      var newX = P.X * M.M00() + P.Y * M.M10() + P.Z * M.M20() + M.OffsetX();
      var newY = P.X * M.M01() + P.Y * M.M11() + P.Z * M.M21() + M.OffsetY();
      var newZ = P.X * M.M02() + P.Y * M.M12() + P.Z * M.M22() + M.OffsetZ();
      var ret = Point3DBl<T, BRAND, ARITY>.Make(newX, newY, newZ);
      var w = P.X * M.M03() + P.Y * M.M13() + P.Z * M.M23() + M.M33();
      return ret / w;
    }*/

    public static MatrixBl<D2, D2> Quadrant(this MatrixBl<D4, D4> A, bool IsLeft, bool IsTop) {
      var C = MatrixBl<D4, D4>.Identity;
      var r0 = IsTop ? 0 : 2;
      var r1 = IsTop ? 1 : 3;

      var c0 = IsLeft ? 0 : 2;
      var c1 = IsLeft ? 1 : 3;

      return MatrixBl<D2, D2>.Make(A[c0, r0], A[c1, r0], A[c0, r1], A[c1, r1]);


    }
    public static DoubleBl DetOld(this MatrixBl<D4, D4> A) {
      return ((A.M00() * A.M11()) - (A.M10() * A.M01()));
    }

    public class InverseOp : Ops.Operator<Matrix<double, D4, D4>, Matrix<double, D4, D4>, InverseOp>, Ops.IMemoOperator<Matrix<double, D4, D4>> {
      public new static InverseOp Instance = new InverseOp();
      private InverseOp() { }

      public override string Format(string argA) { return "Invert(" + argA + ")"; }
      public override Expr<Matrix<double, D4, D4>> Inverse(Expr<Matrix<double, D4, D4>> argA, Expr<Matrix<double, D4, D4>> result) {
        return Instance.Make(result);
      }

      public override Expr<Matrix<double, D4, D4>> Expand(Expr<Matrix<double, D4, D4>> argA) {
        MatrixBl<D4, D4> A = argA;
        var C = MatrixBl<D4, D4>.Identity;
        MatrixBl<D2, D2> r1, r2, r3, r4, r5, r6, r7, c11, c12, c21, c22;

        var a11 = A.Quadrant(true, true);
        var a12 = A.Quadrant(false, true);
        var a21 = A.Quadrant(true, false);
        var a22 = A.Quadrant(false, false);

        var one_over_det = 1.0 / A.DetOld(); // ((A.M00() * A.M11()) - (A.M10() * A.M01()));

        r1 = MatrixBl<D2, D2>.Make(one_over_det * A.M11(), one_over_det * -A.M01(), one_over_det * -A.M10(), one_over_det * A.M00());

        /* R2 = a21 x R1 */
        r2 = a21 * r1;

        /* R3 = R1 x a12 */
        r3 = r1 * a12;

        /* R4 = a21 x R3 */
        r4 = a21 * r3;

        /* R5 = R4 - a22 */
        r5 = MatrixBl<D2, D2>.Make(r4.M00() - a22.M00(), r4.M01() - a22.M01(), r4.M10() - a22.M10(), r4.M11() - a22.M11());

        /* R6 = inverse( R5 ) */
        one_over_det = 1.0 / ((r5.M00() * r5.M11()) - (r5.M10() * r5.M01()));
        r6 = MatrixBl<D2, D2>.Make(one_over_det * r5.M11(), one_over_det * -r5.M01(), one_over_det * -r5.M10(), one_over_det * r5.M00());

        /* c12 = R3 x R6 */
        c12 = r3 * r6;

        /* c21 = R6 x R2 */
        c21 = r6 * r2;

        /* R7 = R3 x c21 */
        r7 = r3 * c21;

        /* c11 = R1 - R7 */
        c11 = MatrixBl<D2, D2>.Make(r1.M00() - r7.M00(), r1.M01() - r7.M01(), r1.M10() - r7.M10(), r1.M11() - r7.M11());

        /* c22 = -R6 */
        c22 = MatrixBl<D2, D2>.Make(-r6.M00(), -r6.M01(), -r6.M10(), -r6.M11());

        return MatrixBl<D4, D4>.Make(c11.M00(), c11.M01(), c12.M00(), c12.M01(),
                                     c11.M10(), c11.M11(), c12.M10(), c12.M11(),
                                     c21.M00(), c21.M01(), c22.M00(), c22.M01(),
                                     c21.M10(), c21.M11(), c22.M10(), c22.M11()).Underlying;
      }
    }

    public static MatrixBl<D4, D4> InverseOld(this MatrixBl<D4, D4> A) {
      return InverseOp.Instance.Make(A);
    }

    public static DoubleBl Det(this MatrixBl<D1, D1> M) {
      return M[0, 0];
    }
    /// <summary>
    /// Compute the determinant of this matrix.
    /// </summary>
    public static DoubleBl Det(this MatrixBl<D2, D2> M) {
      return M[0, 0] * M[1, 1] - M[1, 0] * M[0, 1];
    }
    /// <summary>
    /// Compute the determinant of this matrix.
    /// </summary>
    public static DoubleBl Det(this MatrixBl<D3, D3> M) {
      DoubleBl Sum = 0d;
      // a b c
      // d e f
      // g h i

      //det (A) = aei − afh + bfg − bdi + cdh − ceg.
      for (int i = 0; i < 3; i++) {
        Sum += M[(i + 0) % 3, 0] * M[(i + 1) % 3, 1] * M[(i + 2) % 3, 2];
        Sum -= M[(i + 0) % 3, 0] * M[(i + 2) % 3, 1] * M[(i + 1) % 3, 2];
      }
      return Sum;
    }
    public static DoubleBl Minor(this MatrixBl<D4, D4> M, int Column, int Row) {
      var N = new MatrixBl<D3, D3>((i, j) => {
        i = (i < Column) ? i : i + 1;
        j = (j < Row) ? j : j + 1;
        return M[i, j];
      });
      return N.Det();
    }
    public static DoubleBl Minor(this MatrixBl<D3, D3> M, int Column, int Row) {
      var N = new MatrixBl<D2, D2>((i, j) => {
        i = (i < Column) ? i : i + 1;
        j = (j < Row) ? j : j + 1;
        return M[i, j];
      });
      return N.Det();
    }
    public static DoubleBl Minor(this MatrixBl<D2, D2> M, int Column, int Row) {
      var N = new MatrixBl<D1, D1>((i, j) => {
        i = (i < Column) ? i : i + 1;
        j = (j < Row) ? j : j + 1;
        return M[i, j];
      });
      return N.Det();
    }
    public static DoubleBl Cofactor(this MatrixBl<D4, D4> M, int Column, int Row) {
      return M.Minor(Column, Row) * (((Column + Row) % 2) == 0 ? +1d : -1d);
    }
    public static DoubleBl Cofactor(this MatrixBl<D3, D3> M, int Column, int Row) {
      return M.Minor(Column, Row) * (((Column + Row) % 2) == 0 ? +1d : -1d);
    }
    public static DoubleBl Cofactor(this MatrixBl<D2, D2> M, int Column, int Row) {
      return M.Minor(Column, Row) * (((Column + Row) % 2) == 0 ? +1d : -1d);
    }
    public static DoubleBl Det(this MatrixBl<D4, D4> M) {
      DoubleBl ret = 0;
      for (int i = 0; i < 4; i++) {
        ret += M.Cofactor(i, 0) * M[i, 0];
      }
      return ret;
    }
    public static MatrixBl<D4, D4> Inverse(this MatrixBl<D4, D4> M) {
      if (true) return M.InverseOld();
      return new MatrixBl<D4, D4>((i, j) => M.Cofactor(i, j) / M.Det());
    }
    public static MatrixBl<D3, D3> Inverse(this MatrixBl<D3, D3> M) {
      return new MatrixBl<D3, D3>((i, j) => M.Cofactor(i, j) / M.Det());
    }
    public static MatrixBl<D2, D2> Inverse(this MatrixBl<D2, D2> M) {
      return new MatrixBl<D2, D2>((i, j) => M.Cofactor(i, j) / M.Det());
    }

    public static BoolBl IsAffine(this MatrixBl<D4, D4> M) {
      IMatrixBl<double, D1, D4, DoubleBl> c = M.Columns[3];
      BoolBl ret = true;
      for (int i = 0; i < 3; i++) ret = ret & (c[i] == 0d);
      return ret & (c[3] == 1d);
    }
  }
  public partial class Dot<K, DIM> : Ops.StaticCallOperator<Matrix<K, DIM, D1>, Matrix<K, D1, DIM>, K, Dot<K, DIM>> where DIM : DNum<DIM> {
    public new static readonly Dot<K, DIM> Instance = new Dot<K, DIM>();
    private Dot() { }
    protected override string CallName {
      get { return "Dot"; }
    }
    protected override Type TargetType {
      get { return typeof(Math); }
    }
    public override Expr<K> Expand(Expr<Matrix<K, DIM, D1>> argA, Expr<Matrix<K, D1, DIM>> argB) {
      var Row = argA;
      var Column = argB;
      Expr<K> Result = new Constant<K>(default(K));
      for (int i = 0; i < DNum<DIM>.Count; i++) {
        var r = MatrixArity<K, DIM, D1>.ArityI.Access(Row, i);
        var c = MatrixArity<K, D1, DIM>.ArityI.Access(Column, i);
        Result = Ops.NumericOperators0<K, K>.Add.Instance.Make(Result,
           Ops.NumericOperators0<K, K>.Multiply.Instance.Make(r, c));
      }
      return Result;
    }
  }
}

namespace Bling.Core {
  using Bling.Matrices;
  using Bling.Vecs;


  public partial interface IDimPointBl<DIM> : IBrand where DIM : Matrices.DNum<DIM> {
    MatrixBl<DIM, D1> AsMatrix { get; }
    IDimPointBl<DIM> FromMatrix(MatrixBl<DIM, D1> m);
  }

  /*
  public abstract partial class DoubleFunc3Bl<ARG, DARG, RET, DRET, BRAND> : DoubleFunc2Bl<ARG, RET, BRAND>, ISupportsLerp<BRAND>
    where ARG : Brand<ARG>, IBaseDoubleBl<ARG>, IDimPointBl<DARG>
    where RET : Brand<RET>, IBaseDoubleBl<RET>, IDimPointBl<DRET>
    where BRAND : DoubleFunc3Bl<ARG, DARG, RET, DRET, BRAND>
    where DRET : DNum<DRET>
    where DARG : DNum<DARG> {
    public DoubleFunc3Bl(Func<ARG, RET> Function) : base(Function) { }

    public static BRAND operator *(MatrixBl<DRET, DRET> Matrix, DoubleFunc3Bl<ARG, DARG, RET, DRET, BRAND> F) {
      return ToBrand(arg => {
        var ret0 = F.Function(arg);
        var t0 = Matrix * ret0.AsMatrix.Transpose;
        return (RET)ret0.FromMatrix(t0.Transpose);
      });
    }
  }*/


  public partial class Int2Bl : MultiIntBl<Vec2<int>, Int2Bl, Vec2<bool>, Bool2Bl, Vec2Arity<int>>, HasXY<IntBl>, ICoreBrandT {
    public static implicit operator IntMatrixBl<D1, D2>(Int2Bl p) {
      p = Semantics.NoRole.Instance.Transform(p);
      return new IntMatrixBl<D1, D2>(p[0], p[1]);
    }
    public static implicit operator IntMatrixBl<D2, D1>(Int2Bl p) {
      p = Semantics.NoRole.Instance.Transform(p);
      return new IntMatrixBl<D2, D1>(p[0], p[1]);
    }
    public static implicit operator Int2Bl(IntMatrixBl<D2, D1> p) {
      return Semantics.NoRole.Instance.Transform(Make(p[0, 0], p[1, 0]));
    }
    public static implicit operator Int2Bl(IntMatrixBl<D1, D2> p) {
      return Semantics.NoRole.Instance.Transform(Make(p[0, 0], p[0, 1]));
    }
    public IntMatrixBl<D2, D1> AsMatrix { get { return this; } }
  }
  public partial class Int3Bl : MultiIntBl<Vec3<int>, Int3Bl, Vec3<bool>, Bool3Bl, Vec3Arity<int>>, HasXYZ<IntBl>, ICoreBrandT {
    public static implicit operator IntMatrixBl<D1, D3>(Int3Bl p) {
      p = Semantics.NoRole.Instance.Transform(p);
      return new IntMatrixBl<D1, D3>(p[0], p[1]);
    }
    public static implicit operator IntMatrixBl<D3, D1>(Int3Bl p) {
      p = Semantics.NoRole.Instance.Transform(p);
      return new IntMatrixBl<D3, D1>(p[0], p[1]);
    }
    public static implicit operator Int3Bl(IntMatrixBl<D3, D1> p) {
      return Semantics.NoRole.Instance.Transform(Make(p[0, 0], p[1, 0]));
    }
    public static implicit operator Int3Bl(IntMatrixBl<D1, D3> p) {
      return Semantics.NoRole.Instance.Transform(Make(p[0, 0], p[0, 1]));
    }
    public IntMatrixBl<D3, D1> AsMatrix { get { return this; } }
  }
  public partial class Int4Bl : MultiIntBl<Vec4<int>, Int4Bl, Vec4<bool>, Bool4Bl, Vec4Arity<int>>, HasXYZW<IntBl>, ICoreBrandT {
    public static implicit operator IntMatrixBl<D1, D4>(Int4Bl p) {
      p = Semantics.NoRole.Instance.Transform(p);
      return new IntMatrixBl<D1, D4>(p[0], p[1]);
    }
    public static implicit operator IntMatrixBl<D4, D1>(Int4Bl p) {
      p = Semantics.NoRole.Instance.Transform(p);
      return new IntMatrixBl<D4, D1>(p[0], p[1]);
    }
    public static implicit operator Int4Bl(IntMatrixBl<D4, D1> p) {
      return Semantics.NoRole.Instance.Transform(Make(p[0, 0], p[1, 0]));
    }
    public static implicit operator Int4Bl(IntMatrixBl<D1, D4> p) {
      return Semantics.NoRole.Instance.Transform(Make(p[0, 0], p[0, 1]));
    }
    public IntMatrixBl<D4, D1> AsMatrix { get { return this; } }
  }

  public abstract partial class PointBl<T, BRAND, ARITY> : MultiDoubleBl<T, BRAND, Vec2<bool>, Bool2Bl, ARITY>, HasXY<DoubleBl>, IPointBl<BRAND>
    where BRAND : PointBl<T, BRAND, ARITY>
    where ARITY : Arity2<double, T, ARITY>, new() {
    public static implicit operator MatrixBl<D1, D2>(PointBl<T, BRAND, ARITY> p) {
      var q = Semantics.NoRole.Instance.Transform<BRAND>(p).Portable;
      return Ops.VecConvert<Matrix<double, D1, D2>, double, Vecs.Vec2<double>>.FromVec.Instance.Make(q.Underlying);

      //return new MatrixBl<D1, D2>(p[0], p[1]);
    }
    public static implicit operator MatrixBl<D2, D1>(PointBl<T, BRAND, ARITY> p) {
      var q = Semantics.NoRole.Instance.Transform<BRAND>(p).Portable;
      return Ops.VecConvert<Matrix<double, D2, D1>, double, Vecs.Vec2<double>>.FromVec.Instance.Make(q.Underlying);
      //return new MatrixBl<D2, D1>(p[0], p[1]);
    }
    public static implicit operator PointBl<T, BRAND, ARITY>(MatrixBl<D2, D1> p) {
      PointBl q = Ops.VecConvert<Matrix<double, D2, D1>, double, Vecs.Vec2<double>>.ToVec.Instance.Make(p.Underlying);
      return (PointBl<T, BRAND, ARITY>)Semantics.NoRole.Instance.Transform(q);
      //return Semantics.NoRole.Instance.Transform<BRAND>(Make(p[0, 0], p[1, 0]));
    }
    public static implicit operator PointBl<T, BRAND, ARITY>(MatrixBl<D1, D2> p) {
      PointBl q = Ops.VecConvert<Matrix<double, D1, D2>, double, Vecs.Vec2<double>>.ToVec.Instance.Make(p.Underlying);
      return (PointBl<T, BRAND, ARITY>)Semantics.NoRole.Instance.Transform(q);
      //return Semantics.NoRole.Instance.Transform<BRAND>(Make(p[0, 0], p[0, 1]));
    }
    public MatrixBl<D2, D1> AsMatrix { get { return this; } }

    public IDimPointBl<D2> FromMatrix(MatrixBl<D2, D1> m) {
      return (PointBl<T, BRAND, ARITY>)m;
    }
  }
  public abstract partial class Point3DBl<T, BRAND, ARITY> : MultiDoubleBl<T, BRAND, Vec3<bool>, Bool3Bl, ARITY>, HasXYZ<DoubleBl>, IPoint3DBl<BRAND>
    where BRAND : Point3DBl<T, BRAND, ARITY>
    where ARITY : Arity3<double, T, ARITY>, new() {

    public static implicit operator MatrixBl<D1, D3>(Point3DBl<T, BRAND, ARITY> p) {
      var q = Semantics.NoRole.Instance.Transform<BRAND>(p).Portable;
      return Ops.VecConvert<Matrix<double, D1, D3>, double, Vecs.Vec3<double>>.FromVec.Instance.Make(q.Underlying);
    }
    public static implicit operator MatrixBl<D3, D1>(Point3DBl<T, BRAND, ARITY> p) {
      var q = Semantics.NoRole.Instance.Transform<BRAND>(p).Portable;
      return Ops.VecConvert<Matrix<double, D3, D1>, double, Vecs.Vec3<double>>.FromVec.Instance.Make(q.Underlying);
    }
    public static implicit operator Point3DBl<T, BRAND, ARITY>(MatrixBl<D3, D1> p) {
      Point3DBl q = Ops.VecConvert<Matrix<double, D3, D1>, double, Vecs.Vec3<double>>.ToVec.Instance.Make(p.Underlying);
      return (Point3DBl<T, BRAND, ARITY>)Semantics.NoRole.Instance.Transform(q);
    }
    public static implicit operator Point3DBl<T, BRAND, ARITY>(MatrixBl<D1, D3> p) {
      Point3DBl q = Ops.VecConvert<Matrix<double, D1, D3>, double, Vecs.Vec3<double>>.ToVec.Instance.Make(p.Underlying);
      return (Point3DBl<T, BRAND, ARITY>)Semantics.NoRole.Instance.Transform(q);
    }
    public MatrixBl<D3, D1> AsMatrix { get { return this; } }
    public IDimPointBl<D3> FromMatrix(MatrixBl<D3, D1> m) {
      return (Point3DBl<T, BRAND, ARITY>)m;
    }

    public static implicit operator MatrixBl<D1, D4>(Point3DBl<T, BRAND, ARITY> p) {
      return new Point4DBl(p.Portable, 1d);
    }
    public static implicit operator MatrixBl<D4, D1>(Point3DBl<T, BRAND, ARITY> p) {
      return new Point4DBl(p.Portable, 1d);
    }
    public static implicit operator BaseMatrixBl<D4, D1>(Point3DBl<T, BRAND, ARITY> p) {
      return (MatrixBl<D4, D1>)p;
    }
    public static implicit operator BaseMatrixBl<D1, D4>(Point3DBl<T, BRAND, ARITY> p) {
      return (MatrixBl<D1, D4>)p;
    }

    public static Point4DBl operator *(Point3DBl<T, BRAND, ARITY> opA, MatrixBl<D4, D4> opB) {
      Point4DBl p = new Point4DBl(opA.XYZ(), 1d);
      return p * opB;
    }


    public static explicit operator Point3DBl<T, BRAND, ARITY>(MatrixBl<D4, D1> p) {
      var q = (Point4DBl)p;
      var q0 = (Point3DBl)q;
      return (Point3DBl<T, BRAND, ARITY>)q0;
    }
    public static explicit operator Point3DBl<T, BRAND, ARITY>(MatrixBl<D1, D4> p) {
      var q = (Point4DBl)p;
      var q0 = (Point3DBl)q;
      return (Point3DBl<T, BRAND, ARITY>)q0;
    }

    /*
    public static implicit operator Point3DBl<T, BRAND, ARITY>(MatrixBl<D4, D1> p) {
      Point4DBl q = p;
      return (Point3DBl<T, BRAND, ARITY>) (q.XYZ() );
    }
    public static implicit operator Point3DBl<T, BRAND, ARITY>(MatrixBl<D1, D4> p) {
      Point4DBl q = p;
      return (Point3DBl<T, BRAND, ARITY>) (q.XYZ() );
    }
    */
  }
  public abstract partial class Point4DBl<T, BRAND, ARITY> : MultiDoubleBl<T, BRAND, Vec4<bool>, Bool4Bl, ARITY>, HasXYZW<DoubleBl>, IPoint4DBl<BRAND>
    where BRAND : Point4DBl<T, BRAND, ARITY>
    where ARITY : Arity4<double, T, ARITY>, new() {
    //NumericMatrixBl<double, HEIGHT, WIDTH, MatrixBl<HEIGHT, WIDTH>, DoubleBl>
    public static implicit operator MatrixBl<D1, D4>(Point4DBl<T, BRAND, ARITY> p) {
      var q = Semantics.NoRole.Instance.Transform<BRAND>(p).Portable;
      return Ops.VecConvert<Matrix<double, D1, D4>, double, Vecs.Vec4<double>>.FromVec.Instance.Make(q.Underlying);
    }
    public static implicit operator MatrixBl<D4, D1>(Point4DBl<T, BRAND, ARITY> p) {
      var q = Semantics.NoRole.Instance.Transform<BRAND>(p).Portable;
      return Ops.VecConvert<Matrix<double, D4, D1>, double, Vecs.Vec4<double>>.FromVec.Instance.Make(q.Underlying);
    }
    public static implicit operator BaseMatrixBl<D4, D1>(Point4DBl<T, BRAND, ARITY> p) {
      return (MatrixBl<D4, D1>)p;
    }
    public static implicit operator BaseMatrixBl<D1, D4>(Point4DBl<T, BRAND, ARITY> p) {
      return (MatrixBl<D1, D4>)p;
    }


    public static implicit operator Point4DBl<T, BRAND, ARITY>(MatrixBl<D4, D1> p) {
      Point4DBl q = Ops.VecConvert<Matrix<double, D4, D1>, double, Vecs.Vec4<double>>.ToVec.Instance.Make(p.Underlying);
      return (Point4DBl<T, BRAND, ARITY>)Semantics.NoRole.Instance.Transform(q);
    }
    public static implicit operator Point4DBl<T, BRAND, ARITY>(MatrixBl<D1, D4> p) {
      Point4DBl q = Ops.VecConvert<Matrix<double, D1, D4>, double, Vecs.Vec4<double>>.ToVec.Instance.Make(p.Underlying);
      return (Point4DBl<T, BRAND, ARITY>)Semantics.NoRole.Instance.Transform(q);
    }
    public MatrixBl<D4, D1> AsMatrix { get { return this; } }
    public IDimPointBl<D4> FromMatrix(MatrixBl<D4, D1> m) {
      return (Point4DBl<T, BRAND, ARITY>)m;
    }
  }
  public partial class PointBl : BasePointBl<PointBl>, ICoreBrandT, ICanWrapWPFPoint {
    public static implicit operator PointBl(MatrixBl<D2, D1> p) {
      return (PointBl<Vec2<double>, PointBl, Vec2Arity<double>>)p;
    }
    public static implicit operator PointBl(MatrixBl<D1, D2> p) {
      return (PointBl<Vec2<double>, PointBl, Vec2Arity<double>>)p;
    }
  }
  public partial class Point3DBl : BasePoint3DBl<Point3DBl>, ICoreBrandT, ICanWrapWPFPoint3D {
    public static implicit operator Point3DBl(MatrixBl<D3, D1> p) {
      return (Point3DBl<Vec3<double>, Point3DBl, Vec3Arity<double>>)p;
    }
    public static implicit operator Point3DBl(MatrixBl<D1, D3> p) {
      return (Point3DBl<Vec3<double>, Point3DBl, Vec3Arity<double>>)p;
    }
    public static Point3DBl operator *(Point3DBl opA, MatrixBl<D3, D3> matrix) {
      return ((BaseMatrixBl<D3, D1>)opA) * matrix;
    }

  }
  public partial class Point4DBl : BasePoint4DBl<Point4DBl>, ICoreBrandT, ICanWrapWPFPoint4D, ICanWrapWPFRect {
    public static implicit operator Point4DBl(MatrixBl<D4, D1> p) {
      return (Point4DBl<Vec4<double>, Point4DBl, Vec4Arity<double>>)p;
    }
    public static implicit operator Point4DBl(MatrixBl<D1, D4> p) {
      return (Point4DBl<Vec4<double>, Point4DBl, Vec4Arity<double>>)p;
    }
    public static Point4DBl operator *(Point4DBl opA, MatrixBl<D4, D4> matrix) {
      return ((BaseMatrixBl<D4, D1>)opA) * matrix;
    }
  }
}
