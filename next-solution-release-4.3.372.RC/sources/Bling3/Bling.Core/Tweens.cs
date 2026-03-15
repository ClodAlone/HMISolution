using System;
using System.Collections.Generic;
using System.Collections;
using linq = Microsoft.Linq.Expressions;
using Bling.Util;

namespace Bling.Core {
  public abstract partial class BaseDoubleBl<T, BRAND, BOOL> :
    BaseDoubleLikeBl<T, BRAND, BOOL>, IDoubleBl<BRAND, BOOL>, IBaseDoubleBl<T, BRAND>
    where BRAND : BaseDoubleBl<T, BRAND, BOOL>
    where BOOL : IBoolBl<BOOL> {

    public class OutInBl { 
      internal BRAND Target;
      internal Func<BRAND, BRAND> Out0;
      internal Func<BRAND, BRAND> In0;
      internal Func<BRAND, BRAND> InOut0;
      internal Func<BRAND, BRAND> OutIn0;
      internal OutInBl() {
        InOut0 = (t) => (t < (0.5)).Condition(In0(t * 2) / 2d, .5 + Out0((t * 2) - 1) * .5);
        OutIn0 = (t) => (t < (0.5)).Condition(Out0(t * 2) / 2d, .5 + In0((t * 2) - 1) * .5);
      }
      /// <summary>
      /// Decelerating from zero velocity.
      /// </summary>
      public BRAND Out { get { return Out0(Target); } }
      /// <summary>
      /// Accelerating from zero velocity.
      /// </summary>
      public BRAND In { get { return In0(Target); } }
      /// <summary>
      /// Acceleration until halfway, then deceleration.
      /// </summary>
      public BRAND InOut { get { return InOut0(Target); } }
      /// <summary>
      /// Deceleration until halfway, then acceleration.
      /// </summary>
      public BRAND OutIn { get { return OutIn0(Target); } }

    }
    public class EaseBl {
      internal BRAND Target;
      /// <summary>
      /// Easing equation function for an exponential (2^t) easing
      /// /// </summary>
      public OutInBl Expo {
        get {
          return new OutInBl() {
            Target = Target,
            Out0 = t => 1d - Convert(2d).Pow(-10d * t),
            In0 = t => Convert(2d).Pow(10d * (t - 1d)),
            InOut0 = t => {
              double d = 1;
              double c = 1;
              double b = 0;
              return ((t /= d / 2) < 1).Condition(
                 c / 2 * (Convert(2d).Pow(10 * (t - 1)) + 0) + b,
                 c / 2 * (Convert(-2d).Pow(-10 * (t - 1)) + 2) + b);

            },
          };
        }
      }
      /// <summary>
      /// Easing equation function for a circular (sqrt(1-t^2)) easing
      /// </summary>
      public OutInBl Circ {
        get {
          return new OutInBl() {
            Target = Target,
            Out0 = t => (1d - (t - 1).Square).Sqrt,
            In0 = t => -((1d - t.Square).Sqrt - 1d),
            InOut0 = t => (t < 0.5).Condition(
              -.5 * ((1 - (2 * t).Square).Sqrt - 1d),
              .5 * ((1 - ((2 * t) - 2).Square).Sqrt + 1d)),
          };
        }
      }
      /// <summary>
      /// Easing equation function for a sinusoidal (sin(t)) easing  
      /// </summary>
      public OutInBl Sine {
        get {
          return new OutInBl() {
            Target = Target,
            Out0 = t => (t / 2d).SinU,
            In0 = t => -1d * (t / 2d).CosU + 1,
            InOut0 = t => {
              double d = 1;
              double c = 1;
              double b = 0;
              return ((t /= d / 2) < 1).Condition(
                +c / 2 * (((t - 0) / 2).SinU - 0) + b,
                -c / 2 * (((t - 1) / 2).CosU - 2) + b);
            },
          };
        }
      }

      /// <summary>
      /// Easing equation function for a quadratic (t^2). 
      /// </summary>
      public OutInBl Quad {
        get {
          return new OutInBl() {
            Target = Target,
            Out0 = t => {
              double c = 1, d = 1, b = 0;
              return -c * (t /= d) * (t - 2) + b;
            },
            In0 = t => {
              double c = 1, d = 1, b = 0;
              return c * (t /= d) * t + b;
            },
            InOut0 = t => {
              double c = 1, d = 1, b = 0;
              return ((t /= d / 2) < 1).Condition(
               +c / 2 * ((t - 0) * (t - 0) - 0) + b,
               -c / 2 * ((t - 1) * (t - 3) - 1) + b);
            },
          };
        }
      }

      /// <summary>
      /// Easing equation function for a cubic (t^3). 
      /// </summary>
      public OutInBl Cubic {
        get {
          return new OutInBl() {
            Target = Target,
            Out0 = t => {
              double c = 1, d = 1, b = 0;
              return c * ((t = t / d - 1).Pow(3) + 1) + b;
            },
            In0 = t => {
              double c = 1, d = 1, b = 0;
              return c * (t /= d).Pow(3) + b;
            },
            InOut0 = t => {
              double c = 1, d = 1, b = 0;
              return ((t /= d / 2) < 1).Condition(
                c / 2 * ((t - 0).Pow(3) + 0) + b,
                c / 2 * ((t - 2).Pow(3) + 2) + b);
            },
          };
        }
      }
      /// <summary>
      /// Easing equation function for a quatric (t^4). 
      /// </summary>
      public OutInBl Quatric {
        get {
          return new OutInBl() {
            Target = Target,
            Out0 = t => {
              double c = 1, d = 1, b = 0;
              return -c * ((t = t / d - 1).Pow(4) - 1) + b;
            },
            In0 = t => {
              double c = 1, d = 1, b = 0;
              return c * (t /= d).Pow(4) + b;
            },
            InOut0 = t => {
              double c = 1, d = 1, b = 0;
              return ((t /= d / 2) < 1).Condition(
                +c / 2 * ((t - 0).Pow(4) - 0) + b,
                -c / 2 * ((t - 2).Pow(4) - 2) + b);
            },
          };
        }
      }
      /// <summary>
      /// Easing equation function for a quintic (t^5). 
      /// </summary>
      public OutInBl Quintic {
        get {
          return new OutInBl() {
            Target = Target,
            Out0 = t => {
              double c = 1, d = 1, b = 0;
              return c * ((t = t / d - 1).Pow(5) + 1) + b;
            },
            In0 = t => {
              double c = 1, d = 1, b = 0;
              return c * (t /= d).Pow(5) + b;
            },
            InOut0 = t => {
              double c = 1, d = 1, b = 0;
              return ((t /= d / 2) < 1).Condition(
                c / 2 * ((t - 0).Pow(5) + 0) + b,
                c / 2 * ((t - 2).Pow(5) + 2) + b);
            },
          };
        }
      }


      /// <summary>
      /// Easing equation function for an elastic (exponentially decaying sine wave) easing out/in: 
      /// deceleration until halfway, then acceleration.
      /// </summary>
      public OutInBl Elastic {
        get {
          return new OutInBl() {
            Target = Target,
            Out0 = t => {
              double p = 1d * .3;
              double s = p / 4;
              return Convert(2d).Pow(-10 * t) * ((t * 1d - s) * (2) / p).SinU + 1d;
            },
            In0 = t => {
              double p = 1d * .3;
              double s = p / 4;
              return -(Convert(2d).Pow(10 * (t - 1)) * (((t - 1) * 1d - s) * 2 / p).SinU + 0d);
            },
          };
        }
      }
      /// <summary>
      /// Easing equation function for a bounce (exponentially decaying parabolic bounce) easing out: 
      /// decelerating from zero velocity.
      /// </summary>
      public OutInBl Bounce {
        get {
          var result = new OutInBl() {
            Target = Target,
            Out0 = t => {
              var b0 = t < 1.0d / 2.75;
              var b1 = t < 2.0d / 2.75;
              var b2 = t < 2.5d / 2.75;
              return (t < 1.0d / 2.75).Condition(7.5625 * t.Square,
                /**/ (t < 2.0d / 2.75).Condition(7.5625 * (t - (1.50 / 2.75)).Square + .75,
                /**/ (t < 2.5d / 2.75).Condition(7.5625 * (t - (2.25 / 2.75)).Square + .9375,
                /**********************/ 7.5625 * (t - (2.625 / 2.75)).Square + .984375)));
            },
          };
          result.In0 = t => 1d - result.Out0(1d - t);
          return result;
        }
      }
      /// <summary>
      /// Easing equation function for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing in/out: 
      /// acceleration until halfway, then deceleration.
      /// </summary>
      public OutInBl Back {
        get {
          return new OutInBl() {
            Target = Target,
            Out0 = t => {
              double c = 1d;
              double d = 1d;
              double b = 0d;
              return c * ((t = t / d - 1) * t * ((1.70158 + 1) * t + 1.70158) + 1) + b;
            },
            In0 = t => {
              double c = 1d;
              double d = 1d;
              double b = 0d;
              return c * (t /= d) * t * ((1.70158 + 1) * t - 1.70158) + b;
            },
            InOut0 = t => {
              double c = 1d;
              double d = 1d;
              double b = 0d;

              double s = 1.70158;
              double s0 = s;
              double s1 = s;

              t /= d / 2;
              BRAND t0 = t;
              BRAND t1 = t;
              return (t < 1).Condition(
                c / 2 * (t0 * t0 * (((s0 *= (1.525)) + 1) * t0 - s0)) + b,
                c / 2 * ((t1 -= 2) * t1 * (((s1 *= (1.525)) + 1) * t1 + s1) + 2) + b);
            },
          };
        }
      }
    }
    public EaseBl Ease { get { return new EaseBl() { Target = this }; } }

  }
}