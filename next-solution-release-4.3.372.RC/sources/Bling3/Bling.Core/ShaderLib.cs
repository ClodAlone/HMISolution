using System;
using System.Collections.Generic;
using System.Collections;
using Bling.Util;
using Bling.Graphics;
namespace Bling.Shaders {
  using Bling.Core;
  public abstract partial class LibTextureProvider {
    public LibTextureProvider() { 
      ShaderLib.ThreadInit(this); 
      CloudImage = LoadTexture("Resources/clouds.png", typeof(ShaderLib));
      NoiseImage = LoadTexture("Resources/noise.png", typeof(ShaderLib));
      SinCosATanImage = LoadTexture("Resources/sincosatan.png", typeof(ShaderLib));
      RainbowImage = LoadTexture("Resources/rainbow.png", typeof(ShaderLib));
    }
    protected abstract Tex2D LoadTexture(string Url, Type FromType);
    internal readonly Tex2D CloudImage;
    internal readonly Tex2D NoiseImage;
    internal readonly Tex2D SinCosATanImage;
    internal readonly Tex2D RainbowImage;
  }
  public static class ShaderLib {
    public static Tex2D Noise { get { return Driver.NoiseImage; } }
    public static Tex2D Cloud { get { return Driver.CloudImage; }}
    public static Tex2D SinCosATan { get { return Driver.SinCosATanImage; }}
    public static Tex2D Rainbow { get { return Driver.RainbowImage; } }


    [System.ThreadStatic] 
    private static LibTextureProvider Driver;
    internal static void ThreadInit(LibTextureProvider Driver) { ShaderLib.Driver = Driver; }

    public static RGBBl Sample(this Tex2D input, PointBl center, DoubleBl radius, int steps) {
      var dA = Angle2DBl.MaxAngle / steps;
      var Angle = 0.PI();
      var result = new RGBBl(0,0,0);
      for (int i = 0; i < steps; i++) {
        result += input[center + Angle.ToPoint * radius].ScRGB;
        Angle += dA;
      }
      return result / steps;
    }
    public static RGBBl Transfer(this RGBBl original, RGBBl other) {
      return Transfer(original, other, .1, .002);
    }
    public static RGBBl Transfer(this RGBBl original, RGBBl other, DoubleBl M, DoubleBl N) {
      var rgb = new DoubleBl[3];
      for (int i = 0; i < 3; i++) {
        rgb[i] = Transfer(original[i], other[i], M, N);
      }
      return new RGBBl(rgb[0], rgb[1], rgb[2]);
    }
    public static DoubleBl Transfer(this DoubleBl original, DoubleBl other, DoubleBl M, DoubleBl N) {
      var x = original;
      var y = other;
      // magical numbers :(
      var z = (x < y - M).Condition(M, (x > y - N).Condition(-N, y - x));
      x = x + z;
      //return y;
      return x;
    }
    public static PixelEffect Diffusion(this DoubleBl radius, int SegmentCount) {
      return new PixelEffect(input => new Tex2D((uv) => {
          RGBBl rgb = Sample(input, uv, radius, SegmentCount);
          return Transfer(input[uv].ScRGB, rgb);
        }));
    }
    public static PixelEffect Diffusion(this DoubleBl radius, int SegmentCount, DoubleBl M, DoubleBl N) {
      return new PixelEffect(input => new Tex2D((uv) => {
        RGBBl rgb = Sample(input, uv, radius, SegmentCount);
        return Transfer(input[uv].ScRGB, rgb);
      }));
    }
    public static Tex2D CloudPointT(DoubleBl Div, DoubleBl RandomSeed) {
      return Driver.CloudImage.Distort((uv => (uv / Div).MapY(y => (y + 0.9.Bl().Min(RandomSeed)).Frac)));
    }
    public static PointBl CloudPoint(PointBl uv, DoubleBl div, DoubleBl RandomSeed) {
      return CloudPointT(div, RandomSeed)[uv].XY() * 2.0 - 1.0.Bl();
    }
    public static Tex2D CloudPoint(this Tex2D Input, DoubleBl Div, DoubleBl RandomSeed, PointBl Progress) {
      return Input.Distort(uv => (uv + CloudPoint(uv, Div, RandomSeed) * Progress).Frac);
    }
    public static PixelEffect Water2(this Tex2D OldInput, PointBl uv, DoubleBl Progress, DoubleBl RandomSeed) {
      var t1 = CloudPoint(OldInput, 10, RandomSeed, Progress);
      return new PixelEffect(Input => {
        var t3 = Progress.Lerp(t1, Input);
        return t1.Map(Input, (c1, c2) => (c1.ScA <= 0.0).Condition(c2, Progress.Lerp(c1, c2)));
      });
    }

    public static readonly Transition Water = 
      new Transition((Progress, RandomSeed, NextInput) => new PixelEffect(Input => new Tex2D((uv) => {
          PointBl offset = CloudPoint(uv, 10, RandomSeed);
          ColorBl next = NextInput[(uv + offset * Progress).Frac];
          ColorBl prev = Input[uv];
          return (next.ScA <= 0.0).Condition(prev, Progress.Lerp(prev, next));
        })));

    public static readonly Transition Blood = 
      new Transition((Progress, RandomSeed, NextInput) =>
      new PixelEffect(Input => new Tex2D((uv) => {
          DoubleBl offset = (Progress + Progress * Driver.CloudImage[new PointBl(uv.X, RandomSeed)].ScR).Min(1.0);
          var uv2 = new PointBl(uv.X, uv.Y - offset);
          return (uv2.Y > 0.0).Condition(Input[uv2], NextInput[uv2.Frac]);
        })));

    public static PixelEffect Crumple2(this Tex2D OldInput, DoubleBl Progress, DoubleBl RandomSeed) {
      return new PixelEffect(Input => new Tex2D((uv) => {
          PointBl offset = CloudPoint(uv, 5, RandomSeed);
          DoubleBl p = Progress * 2d;
          p = (p > 1.0).Condition(1.0 - (p - 1.0), p);
          ColorBl c1 = OldInput[(uv + offset * p).Frac];
          ColorBl c2 = Input[(uv + offset * p).Frac];
          return Progress.Lerp(c1, c2);
        }));
    }
    public static readonly Transition Crumple = new Transition((Progress, RandomSeed, NextInput) =>
      new PixelEffect(Input => {
        DoubleBl p = Progress * 2d;
        p = (p > 1.0).Condition(1.0 - (p - 1.0), p);
        var t1 = CloudPoint(Input, 10, RandomSeed, p);
        var t2 = CloudPoint(NextInput, 10, RandomSeed, p);
        return Progress.Lerp(t1, t2);
      }));

    public static readonly Transition RotateCrumple = new Transition((Progress, RandomSeed, NextInput) =>
      new PixelEffect(Input => new Tex2D((uv) => {
          PointBl offset = CloudPoint(uv, 10, RandomSeed);
          PointBl center = uv + offset / 10.0;
          PointBl toUV = uv - center;
          DoubleBl len = toUV.Length;
          PointBl normToUV = toUV / len;
          Angle2DBl angle = normToUV.Atan2; // SimpleAngle;
          angle += 1d.PI() * 2d * Progress;
          PointBl newOffset = angle.ToPoint * len;

          ColorBl c1 = Input[(center + newOffset).Frac];
          ColorBl c2 = NextInput[(center + newOffset).Frac];
          return Progress.Lerp(c1, c2);
        })));

    public static readonly Transition RotateWiggle = new Transition((Progress, RandomSeed, NextInput) =>
      new PixelEffect(Input => new Tex2D((uv) => {
        PointBl center = new PointBl(.5, .5);
        PointBl toUV = uv - center;
        DoubleBl distanceFromCenter = toUV.Length;
        PointBl normalToUV = toUV / distanceFromCenter;
        DoubleBl angle = (normalToUV.Atan2 /*SimpleAngle*/ + 1d.PI()) / (2d.PI());
        DoubleBl offset1 = (Driver.CloudImage[angle, (Progress / 2 + distanceFromCenter / 5 + RandomSeed).Frac]).X * 2.0 - 1.0;
        DoubleBl offset2 = offset1 * 2.0 * 0.3.Bl().Min(1 - Progress) * distanceFromCenter;
        offset1 = offset1 * 2.0 * 0.3.Bl().Min(Progress) * distanceFromCenter;
        ColorBl c1 = Input[(center + normalToUV * (distanceFromCenter + offset1)).Frac];
        ColorBl c2 = NextInput[(center + normalToUV * (distanceFromCenter + offset2)).Frac];
        return Progress.Lerp(c1, c2);
      })));

    /// <summary>
    /// Ripple effect.
    /// </summary>
    /// <param name="Input">Texture that effect will be applied to.</param>
    /// <param name="uv">Current pixel position being shaded.</param>
    /// <param name="Center">Center of ripple as a percentage-relative point in texture ((0,0) to (1,1)).</param>
    /// <param name="Amplitude">Amplitude of ripple between 0 and 1.</param>
    /// <param name="Frequency">Frequency of ripple between 0 and 100.</param>
    /// <param name="Phase">Phase of ripple between 0 and 10.</param>
    /// <returns>Color of pixel for desired ripple.</returns>
    public static PixelEffect Ripple(this PointBl Center, DoubleBl Amplitude, DoubleBl Frequency, DoubleBl Phase) {
      return new PixelEffect(Input => new Tex2D(UV => {
        var ToPoint = UV - Center;
        DoubleBl Distance = ToPoint.Length;
        PointBl Direction = ToPoint / Distance;
        PointBl Wave = new Angle2DBl(Frequency * Distance + Phase).ToPoint;
        DoubleBl Falloff = (1d - Distance).Saturate.Square;
        DoubleBl Lighting = (Wave.Y * Falloff).Saturate * 0.2 + 0.8;

        var Distance2 = Distance + Amplitude * Wave.X * Falloff;
        PointBl UV2 = Center + Distance2 * Direction;

        ColorBl Color = Input[UV2];
        return new ColorBl(Color.ScRGB * Amplitude.Lerp(1d, Lighting), Color.ScA);
      }));
    }

    public static readonly Transition TrySwirl = new Transition((Progress, Scale, NextInput) =>
      new PixelEffect(Input => new Tex2D((uv) => {
        var offset = (((uv.X - 0.5 * Progress) * 6400d * Scale).ToDegrees().Sin * ((uv.Y - 0.5 * Progress) * 6400d * Scale).ToDegrees().Cos + 1) / 2;
        ColorBl c1 = Input[(uv + uv * offset * Progress).Frac];
        ColorBl c2 = NextInput[(uv + uv * offset * (1 - Progress)).Frac];
        return Progress.Lerp(c1, c2);
      })));


    public static readonly Transition TryRipple = new Transition((Progress, Scale, NextInput) =>
      new PixelEffect(Input => new Tex2D((uv) => {
        var offset = ((((uv.X - 0.5).Square + (uv.Y - 0.5).Square) * 7000d * Scale).ToDegrees().Sin + 1) / 2;
        ColorBl c1 = Input[(uv + uv * offset * Progress).Frac];
        ColorBl c2 = NextInput[(uv + uv * offset * (1 - Progress)).Frac];
        return Progress.Lerp(c1, c2);
      })));


    public static readonly Transition TryRipple2 = new Transition((Progress, Scale, NextInput) =>
      new PixelEffect(Input => new Tex2D((uv) => {
        var Radius = ((uv.X - 0.5).Square + (uv.Y - 0.5).Square).Sqrt;
        var Offset = ((uv.X - 0.5 * Progress) * 3200d * Scale).ToDegrees().Sin + (Radius * 3200d * Scale).ToDegrees().Sin / (Radius + 0.05);
        ColorBl c1 = Input[(uv + uv * Offset * Progress).Frac];
        ColorBl c2 = NextInput[(uv + uv * (1 - Offset) * (1 - Progress)).Frac];
        return Progress.Lerp(c1, c2);
      })));


    public static readonly Transition TrySin = new Transition((Progress, Scale, NextInput) =>
      new PixelEffect(Input => new Tex2D((uv) => {
        var r = ((uv.X).Square + (uv.Y).Square).Sqrt;
        var offset = ((uv.X - 0.5 * Progress) * 3200d * Scale).ToDegrees().Sin;
        ColorBl c1 = Input[(uv + uv * offset * Progress).Frac];
        ColorBl c2 = NextInput[(uv + uv * (1 - offset) * (1 - Progress)).Frac];
        return Progress.Lerp(c1, c2);
      })));
  }
}