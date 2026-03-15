using Bling.DSL;
using Bling.Core;
using Bling.Util;
using System.Collections.Generic;
using System;

namespace Bling.NewVecs {
  public interface IDim<DIM> where DIM : IDim<DIM> { }

  public abstract class Dim {
    protected abstract int CountI { get; }
    protected abstract string NameI { get; }
  }
  public abstract class Dim<DIM> : Dim, IDim<DIM> where DIM : Dim<DIM> {
    protected Dim() { (Instance0 == null).Assert(); }
    private static DIM Instance0;
    protected static DIM Instance {
      get {
        if (Instance0 == null) Instance0 = (DIM)typeof(DIM).GetConstructor(new Type[0]).Invoke(new object[0]);
        return Instance0;
      }
    }
    public static int Count { get { return Instance.CountI; } }
    public static string Name { get { return Instance.NameI; } }
  }
  public interface INextDim<DIM, NEXT>
    where DIM : IDim1D<DIM>, INextDim<DIM, NEXT>
    where NEXT : IDim1D<NEXT>, IPrevDim<NEXT, DIM> { }

  public interface IPrevDim<DIM, PREV> : IDim<IPrevDim<DIM,PREV>>
    where DIM : IDim1D<DIM>, IPrevDim<DIM, PREV>
    where PREV : IDim1D<PREV>, INextDim<PREV, DIM> { }

  public interface IDim1D<D> : IDim<D> where D : IDim1D<D> { }

  public abstract class Dim2D<SELF> : Dim<SELF> where SELF : Dim2D<SELF> {
    public static int MCount { get { return Instance.MCountI; } }
    public static int NCount { get { return Instance.NCountI; } }
    protected abstract int MCountI { get; }
    protected abstract int NCountI { get; }
  }
  public class Dim<M, N> : Dim2D<Dim<M, N>>
    where M : Dim1D<M>
    where N : Dim1D<N> {
    protected override int MCountI { get { return Dim1D<M>.Count; } }
    protected override int NCountI { get { return Dim1D<N>.Count; } }
    protected override int CountI { get { return MCountI * NCountI; } }
    protected override string NameI {
      get { return "M" + MCountI + "x" + NCountI; }
    }
  }

  public abstract class Dim1D<D> : Dim<D>, IDim1D<D> where D : Dim1D<D> {
    protected override string NameI {
      get { return "V" + CountI; }
    }
  }
  public abstract class Dim1D<D, P, N> : Dim1D<D>, IPrevDim<D, P>, INextDim<D, N>
    where D : Dim1D<D, P, N>
    where P : Dim1D<P>, INextDim<P, D>
    where N : Dim1D<N>, IPrevDim<N, D> {
  }
  public interface ICoreDim1D<D> where D : Dim1D<D>, ICoreDim1D<D> { }

  public abstract class CoreDim1D<D, P, N> : Dim1D<D, P, N>, ICoreDim1D<D>
    where D : CoreDim1D<D, P, N>
    where P : Dim1D<P>, INextDim<P, D>
    where N : Dim1D<N>, IPrevDim<N, D> {
  }
  public interface IMultiDim1D<D> : ICoreDim1D<D> where D : Dim1D<D>, IMultiDim1D<D> { }
  public abstract class MultiDim1D<D, P, N> : CoreDim1D<D, P, N>, IMultiDim1D<D>
    where D : MultiDim1D<D, P, N>
    where P : Dim1D<P>, INextDim<P, D>
    where N : Dim1D<N>, IPrevDim<N, D> {
  }


  public class D0 : Dim1D<D0>, INextDim<D0, D1> {protected override int CountI {get{return 0;}}}
  public class D1 :  CoreDim1D<D1,D0,D2> { protected override int CountI { get { return 1; } } }
  public class D2 : MultiDim1D<D2,D1,D3> { protected override int CountI { get { return 2; } } }
  public class D3 : MultiDim1D<D3,D2,D4> { protected override int CountI { get { return 3; } } }
  public class D4 : MultiDim1D<D4,D3,D5> { protected override int CountI { get { return 4; } } }
  public class D5 : Dim1D<D5,D4,D6> { protected override int CountI { get { return 5; } } }
  public class D6 : Dim1D<D6,D5,D7> { protected override int CountI { get { return 6; } } }
  public class D7 : Dim1D<D7,D6,D8> { protected override int CountI { get { return 7; } } }
  public class D8 : Dim1D<D8,D7,D9> { protected override int CountI { get { return 8; } } }
  public class D9 : Dim1D<D9,D8,D10> { protected override int CountI { get { return 9; } } }
  public class D10 : Dim1D<D10,D9,D11> { protected override int CountI { get { return 10; } } }
  public class D11 : Dim1D<D11,D10,D12> { protected override int CountI { get { return 11; } } }
  public class D12 : Dim1D<D12,D11,D13> { protected override int CountI { get { return 12; } } }
  public class D13 : Dim1D<D13,D12,D14> { protected override int CountI { get { return 13; } } }
  public class D14 : Dim1D<D14,D13,D15> { protected override int CountI { get { return 14; } } }
  public class D15 : Dim1D<D15,D14,D16> { protected override int CountI { get { return 15; } } }
  public class D16 : Dim1D<D16>, IPrevDim<D16,D15> { protected override int CountI { get { return 16; } } }
  // get rid of arity constructs.


  public static class Vecs {
    public static string ToString<T, D>(IVec<T, D> v) where D : Dim<D> {
      string s = "[";
      for (int i = 0; i < Dim<D>.Count; i++) {
        s += v[i] + ", ";
      }
      return s + "]";
    }
  }
  public interface IVec { }
  public interface IVec<T> : IVec {
    T this[int Idx] { get; }
  }
  public interface IVec<T, D> : IVec<T> where D : Dim<D> { }
  public static class VecExtensions {
    public static T Access<T, D>(this IVec<T, D> vec, int m, int n) where D : Dim2D<D> {
      return vec[m * Dim2D<D>.NCount + n];
    }
  }
}
namespace Bling.NewVecs { // default implementations. 
  public struct Vec0<T> : IVec<T, D0> {
    public override string ToString() {
      return Vecs.ToString(this);
    }
    public T this[int Idx] {
      get {
        switch (Idx) {
          default: throw new IndexOutOfRangeException();
        }
      }
    }
  }
  public struct Vec1<T> : IVec<T, D1> {
    public T _0;
    public T this[int Idx] {
      get {
        switch (Idx) {
          case 0: return _0;
          default: throw new IndexOutOfRangeException();
        }
      }
    }
    public override string ToString() {
      return Vecs.ToString(this);
    }
  }
  public struct Vec2<T> : IVec<T, D2> {
    public T _0;
    public T _1;
    public T this[int Idx] {
      get {
        switch (Idx) {
          case 0: return _0;
          case 1: return _1;
          default: throw new IndexOutOfRangeException();
        }
      }
    }
    public override string ToString() {
      return Vecs.ToString(this);
    }
  }
  public struct Vec3<T> : IVec<T, D3> {
    public T _0;
    public T _1;
    public T _2;
    public T this[int Idx] {
      get {
        switch (Idx) {
          case 0: return _0;
          case 1: return _1;
          case 2: return _2;
          default: throw new IndexOutOfRangeException();
        }
      }
    }
    public override string ToString() {
      return Vecs.ToString(this);
    }
  }
  public struct Vec4<T> : IVec<T, D4> {
    public T _0;
    public T _1;
    public T _2;
    public T _3;
    public T this[int Idx] {
      get {
        switch (Idx) {
          case 0: return _0;
          case 1: return _1;
          case 2: return _2;
          case 3: return _3;
          default: throw new IndexOutOfRangeException();
        }
      }
    }
    public override string ToString() {
      return Vecs.ToString(this);
    }
  }
  public struct Vec5<T> : IVec<T, D5> {
    public T _0;
    public T _1;
    public T _2;
    public T _3;
    public T _4;
    public T this[int Idx] {
      get {
        switch (Idx) {
          case 0: return _0;
          case 1: return _1;
          case 2: return _2;
          case 3: return _3;
          case 4: return _4;
          default: throw new IndexOutOfRangeException();
        }
      }
    }
    public override string ToString() {
      return Vecs.ToString(this);
    }
  }
  public struct Vec6<T> : IVec<T, D6> {
    public T _0;
    public T _1;
    public T _2;
    public T _3;
    public T _4;
    public T _5;
    public T this[int Idx] {
      get {
        switch (Idx) {
          case 0: return _0;
          case 1: return _1;
          case 2: return _2;
          case 3: return _3;
          case 4: return _4;
          case 5: return _5;
          default: throw new IndexOutOfRangeException();
        }
      }
    }
    public override string ToString() {
      return Vecs.ToString(this);
    }
  }
  public struct Vec7<T> : IVec<T, D7> {
    public T _0;
    public T _1;
    public T _2;
    public T _3;
    public T _4;
    public T _5;
    public T _6;
    public T this[int Idx] {
      get {
        switch (Idx) {
          case 0: return _0;
          case 1: return _1;
          case 2: return _2;
          case 3: return _3;
          case 4: return _4;
          case 5: return _5;
          case 6: return _6;
          default: throw new IndexOutOfRangeException();
        }
      }
    }
    public override string ToString() {
      return Vecs.ToString(this);
    }
  }
  public struct Vec8<T> : IVec<T, D8> {
    public T _0;
    public T _1;
    public T _2;
    public T _3;
    public T _4;
    public T _5;
    public T _6;
    public T _7;
    public T this[int Idx] {
      get {
        switch (Idx) {
          case 0: return _0;
          case 1: return _1;
          case 2: return _2;
          case 3: return _3;
          case 4: return _4;
          case 5: return _5;
          case 6: return _6;
          case 7: return _7;
          default: throw new IndexOutOfRangeException();
        }
      }
    }
    public override string ToString() {
      return Vecs.ToString(this);
    }
  }
  public struct Vec9<T> : IVec<T, D9> {
    public T _0;
    public T _1;
    public T _2;
    public T _3;
    public T _4;
    public T _5;
    public T _6;
    public T _7;
    public T _8;
    public T this[int Idx] {
      get {
        switch (Idx) {
          case 0: return _0;
          case 1: return _1;
          case 2: return _2;
          case 3: return _3;
          case 4: return _4;
          case 5: return _5;
          case 6: return _6;
          case 7: return _7;
          case 8: return _8;
          default: throw new IndexOutOfRangeException();
        }
      }
    }
    public override string ToString() {
      return Vecs.ToString(this);
    }
  }
  public struct Vec10<T> : IVec<T, D10> {
    public T _0;
    public T _1;
    public T _2;
    public T _3;
    public T _4;
    public T _5;
    public T _6;
    public T _7;
    public T _8;
    public T _9;
    public T this[int Idx] {
      get {
        switch (Idx) {
          case 0: return _0;
          case 1: return _1;
          case 2: return _2;
          case 3: return _3;
          case 4: return _4;
          case 5: return _5;
          case 6: return _6;
          case 7: return _7;
          case 8: return _8;
          case 9: return _9;
          default: throw new IndexOutOfRangeException();
        }
      }
    }
    public override string ToString() {
      return Vecs.ToString(this);
    }
  }
  public struct Vec11<T> : IVec<T, D11> {
    public T _0;
    public T _1;
    public T _2;
    public T _3;
    public T _4;
    public T _5;
    public T _6;
    public T _7;
    public T _8;
    public T _9;
    public T _10;
    public T this[int Idx] {
      get {
        switch (Idx) {
          case 0: return _0;
          case 1: return _1;
          case 2: return _2;
          case 3: return _3;
          case 4: return _4;
          case 5: return _5;
          case 6: return _6;
          case 7: return _7;
          case 8: return _8;
          case 9: return _9;
          case 10: return _10;
          default: throw new IndexOutOfRangeException();
        }
      }
    }
    public override string ToString() {
      return Vecs.ToString(this);
    }
  }
  public struct Vec12<T> : IVec<T, D12> {
    public T _0;
    public T _1;
    public T _2;
    public T _3;
    public T _4;
    public T _5;
    public T _6;
    public T _7;
    public T _8;
    public T _9;
    public T _10;
    public T _11;
    public T this[int Idx] {
      get {
        switch (Idx) {
          case 0: return _0;
          case 1: return _1;
          case 2: return _2;
          case 3: return _3;
          case 4: return _4;
          case 5: return _5;
          case 6: return _6;
          case 7: return _7;
          case 8: return _8;
          case 9: return _9;
          case 10: return _10;
          case 11: return _11;
          default: throw new IndexOutOfRangeException();
        }
      }
    }
    public override string ToString() {
      return Vecs.ToString(this);
    }
  }
  public struct Vec13<T> : IVec<T, D13> {
    public T _0;
    public T _1;
    public T _2;
    public T _3;
    public T _4;
    public T _5;
    public T _6;
    public T _7;
    public T _8;
    public T _9;
    public T _10;
    public T _11;
    public T _12;
    public T this[int Idx] {
      get {
        switch (Idx) {
          case 0: return _0;
          case 1: return _1;
          case 2: return _2;
          case 3: return _3;
          case 4: return _4;
          case 5: return _5;
          case 6: return _6;
          case 7: return _7;
          case 8: return _8;
          case 9: return _9;
          case 10: return _10;
          case 11: return _11;
          case 12: return _12;
          default: throw new IndexOutOfRangeException();
        }
      }
    }
    public override string ToString() {
      return Vecs.ToString(this);
    }
  }
  public struct Vec14<T> : IVec<T, D14> {
    public T _0;
    public T _1;
    public T _2;
    public T _3;
    public T _4;
    public T _5;
    public T _6;
    public T _7;
    public T _8;
    public T _9;
    public T _10;
    public T _11;
    public T _12;
    public T _13;
    public T this[int Idx] {
      get {
        switch (Idx) {
          case 0: return _0;
          case 1: return _1;
          case 2: return _2;
          case 3: return _3;
          case 4: return _4;
          case 5: return _5;
          case 6: return _6;
          case 7: return _7;
          case 8: return _8;
          case 9: return _9;
          case 10: return _10;
          case 11: return _11;
          case 12: return _12;
          case 13: return _13;
          default: throw new IndexOutOfRangeException();
        }
      }
    }
    public override string ToString() {
      return Vecs.ToString(this);
    }
  }
  public struct Vec15<T> : IVec<T, D15> {
    public T _0;
    public T _1;
    public T _2;
    public T _3;
    public T _4;
    public T _5;
    public T _6;
    public T _7;
    public T _8;
    public T _9;
    public T _10;
    public T _11;
    public T _12;
    public T _13;
    public T _14;
    public T this[int Idx] {
      get {
        switch (Idx) {
          case 0: return _0;
          case 1: return _1;
          case 2: return _2;
          case 3: return _3;
          case 4: return _4;
          case 5: return _5;
          case 6: return _6;
          case 7: return _7;
          case 8: return _8;
          case 9: return _9;
          case 10: return _10;
          case 11: return _11;
          case 12: return _12;
          case 13: return _13;
          case 14: return _14;
          default: throw new IndexOutOfRangeException();
        }
      }
    }
    public override string ToString() {
      return Vecs.ToString(this);
    }
  }
  public struct Vec16<T> : IVec<T, D16> {
    public T _0;
    public T _1;
    public T _2;
    public T _3;
    public T _4;
    public T _5;
    public T _6;
    public T _7;
    public T _8;
    public T _9;
    public T _10;
    public T _11;
    public T _12;
    public T _13;
    public T _14;
    public T _15;
    public T this[int Idx] {
      get {
        switch (Idx) {
          case 0: return _0;
          case 1: return _1;
          case 2: return _2;
          case 3: return _3;
          case 4: return _4;
          case 5: return _5;
          case 6: return _6;
          case 7: return _7;
          case 8: return _8;
          case 9: return _9;
          case 10: return _10;
          case 11: return _11;
          case 12: return _12;
          case 13: return _13;
          case 14: return _14;
          case 15: return _15;
          default: throw new IndexOutOfRangeException();
        }
      }
    }
    public override string ToString() {
      return Vecs.ToString(this);
    }
  }
}

namespace Bling.NewVecs {
  public interface IVecEval<EVAL> : IEval<EVAL> where EVAL : Eval<EVAL> {
    Eval<EVAL>.Info<T> Access<T, D>(Eval<EVAL>.Info<IVec<T, D>> Vec, Eval<EVAL>.Info<int> Index) where D : Dim<D>;
    Eval<EVAL>.Info<T> Access<T, D>(Eval<EVAL>.Info<IVec<T, D>> Vec, Eval<EVAL>.Info<int> m, Eval<EVAL>.Info<int> n) where D : Dim2D<D>;

    Eval<EVAL>.Info<T> Access<T, D>(Eval<EVAL>.Info<IVec<T, D>> Vec, int Index) where D : Dim<D>;
    Eval<EVAL>.Info<T> Access<T, D>(Eval<EVAL>.Info<IVec<T, D>> Vec, int m, int n) where D : Dim2D<D>;

    Eval<EVAL>.Info<IVec<T,D>> Composite<T, D>(Eval<EVAL>.Info<T>[] Es) where D : Dim<D>;
  }
  public abstract class BaseAccessExpr<T, D> : Expr<T> where D : Dim<D> {
    public readonly Expr<IVec<T, D>> Vector;
    static BaseAccessExpr() {
      (Dim<D>.Count >= 1).Assert();
    }
    public BaseAccessExpr(Expr<IVec<T, D>> Vector) {
      this.Vector = Vector; ;
    }
    protected override int GetHashCode0() {
      return Vector.GetHashCode();
    }
    protected override bool Equals0(Expr<T> e) {
      if (!(e is BaseAccessExpr<T, D>)) return false;
      return Vector.Equals(((BaseAccessExpr<T, D>)e).Vector);
    }
    protected override string ToString1() {
      return Vector.ToString();
    }
  }
  public partial class AccessExpr<T, D> : BaseAccessExpr<T,D> where D : Dim<D> {
    public readonly Expr<int> Index;
    private AccessExpr(Expr<IVec<T, D>> Vector, Expr<int> Index) : base(Vector) { this.Index = Index; }
    protected override int GetHashCode0() {
      return base.GetHashCode0() + Index.GetHashCode();
    }
    public static Expr<T> Make(Expr<IVec<T, D>> Vector, int Index) {
      return Make(Vector, new Constant<int>(Index));
    }
    public static Expr<T> Make(Expr<IVec<T, D>> Vector, Expr<int> Index) {
      if (Vector is Composite<T, D> && Dim<D>.Count == 1) { // don't skip other composites.
        return ((Composite<T, D>)Vector).Components[0];
      }
      return new AccessExpr<T, D>(Vector, Index);
    }
    protected override bool Equals0(Expr<T> e) {
      if (!base.Equals0(e)) return false;
      if (!(e is AccessExpr<T, D>)) return false;
      return Index.Equals(((AccessExpr<T, D>)e).Index);
    }
    protected override string ToString1() {
      if (Dim<D>.Count == 1) return base.ToString1();
      else return base.ToString1() + "[" + Index + "]";
    }
    protected override Eval<EVAL>.Info<T> Eval<EVAL>(Eval<EVAL> txt) {
      if (txt is IVecEval<EVAL>) {
        if (Index.IsConstant) {
          return ((IVecEval<EVAL>)txt).Access(txt.DoEval(Vector), Index.CurrentValue);
        } else return ((IVecEval<EVAL>)txt).Access(txt.DoEval(Vector), txt.DoEval(Index));
      }
      throw new NotImplementedException();
    }
  }
  public class Access2DExpr<T, D> : BaseAccessExpr<T, D>
    where D : Dim2D<D> {
    protected readonly Expr<int> IndexM;
    protected readonly Expr<int> IndexN;
    public Access2DExpr(Expr<IVec<T, D>> Vector, Expr<int> IndexM, Expr<int> IndexN) : base(Vector) {
      this.IndexM = IndexM;
      this.IndexN = IndexN;
    }
    protected override string ToString1() {
      if (Dim<D>.Count == 1) return base.ToString1();
      else return base.ToString1() + "[" + IndexM + ", " + IndexN + "]";
    }
    protected override int GetHashCode0() {
      return base.GetHashCode0() + IndexM.GetHashCode() + IndexN.GetHashCode();
    }
    protected override bool Equals0(Expr<T> e) {
      if (!base.Equals0(e)) return false;
      if (!(e is Access2DExpr<T, D>)) return false;
      return IndexM.Equals(((Access2DExpr<T, D>)e).IndexM) && 
             IndexN.Equals(((Access2DExpr<T, D>)e).IndexN);
    }
    protected override Eval<EVAL>.Info<T> Eval<EVAL>(Eval<EVAL> txt) {
      if (txt is IVecEval<EVAL>) {
        if (IndexM.IsConstant && IndexN.IsConstant)
          return ((IVecEval<EVAL>)txt).Access(txt.DoEval(Vector), IndexM.CurrentValue, IndexN.CurrentValue);
        else return ((IVecEval<EVAL>)txt).Access(txt.DoEval(Vector), txt.DoEval(IndexM), txt.DoEval(IndexN));
      }
      throw new NotImplementedException();
    }
  }
  // matrix or vector.
  public class Composite<T, D> : Expr<IVec<T, D>> where D : Dim<D> {
    internal readonly Expr<T>[] Components;
    private Composite(params Expr<T>[] Es) {
      this.Components = Es;
    }
    public static Expr<IVec<T, D>> Make(params Expr<T>[] Es) {
      (Es.Length == Dim<D>.Count).Assert();
      // a common case to get rid of extra accesses.
      if (Es.Length == 1 && Es[0] is BaseAccessExpr<T, D>) {
        var access = (BaseAccessExpr<T, D>)Es[0];
        return access.Vector;
      }
      return new Composite<T,D>(Es);
    }
    protected override int GetHashCode0() {
      var hc = 0;
      foreach (var e in Components) hc += e.GetHashCode();
      return hc;
    }
    public Expr<T> this[int Idx] { get { return Components[Idx]; } }
    protected override bool Equals0(Expr<IVec<T, D>> e) {
      if (!(e is Composite<T, D>)) return false;
      for (int i = 0; i < Dim<D>.Count; i++)
        if (!Components[i].Equals(((Composite<T, D>)e).Components[i])) return false;
      return true;
    }
    protected override string ToString1() {
      if (Dim<D>.Count == 1) return Components[0].ToString();
      var str = "[";
      for (int i = 0; i < Dim<D>.Count; i++) {
        if (i > 0) str += ", ";
        str += Components[i].ToString();
      }
      return str + "]";
    }
    protected override Eval<EVAL>.Info<IVec<T, D>> Eval<EVAL>(Eval<EVAL> txt) {
      if (txt is IVecEval<EVAL>) {
        var Es = new Eval<EVAL>.Info<T>[Dim<D>.Count];
        for (int i = 0; i < Dim<D>.Count; i++)
          Es[i] = txt.DoEval(Components[i]);
        return ((IVecEval<EVAL>)txt).Composite<T,D>(Es);
      }
      throw new NotImplementedException();
    }
  }
}

namespace Bling.NewVecs {
  using System.Reflection.Emit;
  using System.Reflection;
  using linq = Microsoft.Linq.Expressions;

  static class NewVec {
    internal static readonly ModuleBuilder Module;
    static NewVec() {
      AppDomain Domain = System.Threading.Thread.GetDomain();
      AssemblyBuilder AsmBuilder = Domain.DefineDynamicAssembly(new AssemblyName() { Name = "Vecs" }, AssemblyBuilderAccess.RunAndSave);
      Module = AsmBuilder.DefineDynamicModule("VecsModule", true);
    }
  }
  public static class Vecs<D> where D : Dim<D> {
    private static Type VecType0;
    public static Type VecType {
      get {
        if (VecType0 != null) return VecType0;
        TypeBuilder Bld = NewVec.Module.DefineType(Dim<D>.Name, TypeAttributes.Public |
            TypeAttributes.Sealed | TypeAttributes.SequentialLayout |
            TypeAttributes.Serializable, typeof(ValueType));
        var T = Bld.DefineGenericParameters("T")[0];
        Bld.AddInterfaceImplementation(typeof(IVec<,>).MakeGenericType(T, typeof(D)));
        Bld.DefineDefaultConstructor(MethodAttributes.Public);
        // add fields
        for (int i = 0; i < Dim<D>.Count; i++) {
          var fld = Bld.DefineField("_" + i, T, FieldAttributes.Public);
        }
        VecType0 = Bld;
        return VecType0;
      }
    }
  }
  public static class Matrix<T, D> where D : Dim2D<D> {
    public static IVec<T, D> New(params T[][] values) {
      var t = new T[Dim<D>.Count];
      (values.Length == Dim2D<D>.MCount).Assert();
      for (int i = 0; i < Dim2D<D>.MCount; i++) {
        (values[i].Length == Dim2D<D>.NCount).Assert();
        for (int j = 0; j < Dim2D<D>.NCount; j++) {
          t[i * Dim2D<D>.NCount + j] = values[i][j];
        }
      }
      return Vecs<T, D>.New(t);
    }

  }

  public static class Vecs<T,D> where D : Dim<D> {
    private static Func<T[], IVec<T, D>> FactoryB;
    private static readonly Func<IVec<T, D>, T>[] Access0 = new Func<IVec<T, D>, T>[Dim<D>.Count];
    private static object Access1;

    public static Type AccessDelegateType {
      get {
        var Type = Vecs<D>.VecType.MakeGenericType(typeof(T));
        var DelegateType = typeof(Microsoft.Func<,,>).MakeGenericType(Type, typeof(int), typeof(T));
        return DelegateType;
      }
    }
    public static object Access() {
      if (Access1 != null) return Access1;
      var Type = Vecs<D>.VecType.MakeGenericType(typeof(T));
      var vec = linq.Expression.Parameter(Type, "vec");
      var index = linq.Expression.Parameter(typeof(int), "index");
      linq.Expression e = vec;
      var cases = new linq.SwitchCase[Dim<D>.Count];
      for (int i = 0; i < Dim<D>.Count; i++)
        cases[i] = linq.Expression.SwitchCase(linq.Expression.Field(e, "_" + i), linq.Expression.Constant(i, typeof(int)));

      e = linq.Expression.Switch(typeof(T), index, linq.Expression.Constant(default(T)), null, cases);
      var DelegateType = AccessDelegateType;
      var lambda = linq.Expression.Lambda(DelegateType, e, vec, index);
      var f = lambda.Compile();
      Access1 = f;
      return Access1;
    }
    public static linq.Expression Access(linq.Expression Vec, int i) {
      linq.Expression e = Vec;
      e = linq.Expression.Field(e, "_" + i);
      return e;
    }
    public static T Access(IVec<T, D> V, int i) {
      if (Access0[i] != null) return Access0[i](V);
      var vec = linq.Expression.Parameter(typeof(IVec<T, D>), "vec");
      linq.Expression e = linq.Expression.Convert(vec, Vecs<D>.VecType.MakeGenericType(typeof(T)));
      e = linq.Expression.Field(e, "_" + i);
      var f = linq.Expression.Lambda<Microsoft.Func<IVec<T, D>, T>>(e, vec).Compile();
      Access0[i] = vec0 => f(vec0);
      return Access0[i](V);
    }

    public static linq.Expression New(params linq.Expression[] Args) {
      (Dim<D>.Count == Args.Length).Assert();
      return linq.Expression.Invoke(linq.Expression.Constant(New0), Args);
    }
    public static Func<T[], IVec<T, D>> New0 {
      get {
        if (FactoryB != null) return FactoryB;
        var Type = Vecs<D>.VecType.MakeGenericType(typeof(T));
        var Exprs = new linq.Expression[Dim<D>.Count + 2];
        var temp = linq.Expression.Parameter(Type, "vec");
        var args = linq.Expression.Parameter(Type.MakeArrayType(), "args");
        Exprs[0] = linq.Expression.Assign(temp, linq.Expression.New(Type));
        for (int i = 0; i < Dim<D>.Count; i++)
          Exprs[i + 1] = linq.Expression.Assign(
            linq.Expression.Field(temp, "_" + i),
            linq.Expression.ArrayAccess(args, linq.Expression.Constant(i, typeof(int))));
        Exprs[Dim<D>.Count + 1] = temp;

        var block = linq.Expression.Block(Type, new linq.ParameterExpression[] { temp }, Exprs);
        var lambda = linq.Expression.Lambda<Microsoft.Func<T[], IVec<T, D>>>(block, args);
        var f0 = lambda.Compile();
        FactoryB = Ts => f0(Ts);
        return FactoryB;
      }
    }
    public static IVec<T, D> New(params T[] Args) {
      (Dim<D>.Count == Args.Length).Assert();
      return New0(Args);
    }
  }

}
