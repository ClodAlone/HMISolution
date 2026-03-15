using System;
using System.Collections;
using System.Collections.Generic;
using linq = Microsoft.Linq.Expressions;
using System.Linq;
using Bling.Util;

namespace Bling.DSL {
  using Bling.Core;
  /* A new evaluator.
   * Support sequential loops (induction). (Y operator?)
   * Support state (implicit monads). 
   * 
   * specify a loop...then put things under the loop.
   * collapse multiple computations if no dependencies. 
   * 
   */


  public static partial class Stores {
    public abstract class Node { }
    public class Empty : Node { internal Empty() { } }
    public static readonly Empty EMPTY = new Empty();
    public abstract class Append : Expr<Node> {
      public Expr<Node> Prev { get; internal set; }
    }
  }


  public static partial class LoopExpr<T> {
    public static LoopExpr<T, int> For(Expr<int> Start, Func<Expr<int>, Expr<bool>> Condition, Func<Expr<int>, Expr<int>> Step,
                                       Expr<T> Init,
                                       Func<Expr<T>, Expr<int>, Expr<T>> Body) {
      return new LoopExpr<T, int>(Init, Start, Body, Step);
    }
    public static LoopExpr<T, int> For(Expr<int> Limit,
                                       Expr<T> Init,
                                       Func<Expr<T>, Expr<int>, Expr<T>> Body) {
      return For(new Constant<int>(0), i => (i.Bl() < Limit.Bl()).Underlying, i => (i.Bl() + 1).Underlying, Init, Body);
    }
  }

  public partial class LoopExpr<T, K> : Expr<T> {
    public readonly Expr<T> InitA;
    public readonly Expr<K> InitB;
    public readonly Func<Expr<T>, Expr<K>, Expr<T>> StepA;
    public readonly Func<Expr<K>, Expr<K>> StepB;
    public LoopExpr(Expr<T> InitA, Expr<K> InitB, Func<Expr<T>, Expr<K>, Expr<T>> StepA, Func<Expr<K>, Expr<K>> StepB) {
      this.InitA = InitA;
      this.InitB = InitB;
      this.StepA = StepA;
      this.StepB = StepB;
    }
    protected override Eval<EVAL>.Info<T> Eval<EVAL>(Eval<EVAL> txt) {
      if (txt is ILoopEval<EVAL>) return ((ILoopEval<EVAL>)txt).Eval(this);
      throw new NotSupportedException();
    }
  }
  public partial interface ILoopEval<EVAL> : IEval<EVAL> where EVAL : Eval<EVAL> {
    Eval<EVAL>.Info<T> Eval<T, K>(LoopExpr<T, K> Expr);
  }

}