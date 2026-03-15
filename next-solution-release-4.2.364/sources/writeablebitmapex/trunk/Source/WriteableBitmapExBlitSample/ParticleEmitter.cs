#region Header
//
//   Project:           WriteableBitmapEx - Silverlight WriteableBitmap extensions
//   Description:       Blit Sample for the WriteableBitmap extension methods.
//
//   Changed by:        $Author$
//   Changed on:        $Date$
//   Changed in:        $Revision$
//   Project:           $URL$
//   Id:                $Id$
//
//
//   Copyright © 2009-2010 Bill Reiss, Rene Schulte and WriteableBitmapEx Contributors
//
//   This Software is weak copyleft open source. Please read the License.txt for details.
//
#endregion

using System;

#if NETFX_CORE
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Foundation;
using System.Collections.Generic;
using Windows.UI;
#else
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
#endif

namespace WriteableBitmapExBlitSample
{
   public class ParticleEmitter
   {
      #region Fields

      public Point Center { get; set; }
      public List<Particle> Particles = new List<Particle>();
      Random rand = new Random();
      public WriteableBitmap TargetBitmap;
      public WriteableBitmap ParticleBitmap;
      Rect sourceRect = new Rect(0, 0, 32, 32);
      double elapsedRemainder;
      double updateInterval = .003;
      HslColor particleColor = new HslColor();

      #endregion

      #region Contructors

      public ParticleEmitter()
      {
         particleColor = HslColor.FromColor(Colors.Red);
         particleColor.L *= .75;
      }

      #endregion

      #region Methods

      void CreateParticle()
      {
         Particle p = new Particle();
         double speed = rand.Next(20) + 140;
         double angle = Math.PI * 2 * rand.Next(10000) / 10000;
         p.Velocity.X = Math.Sin(angle) * speed;
         p.Velocity.Y = Math.Cos(angle) * speed;
         p.Position = new Point(Center.X - 16, Center.Y - 16);
         p.Color = particleColor.ToColor();
         p.Lifespan = .5 + rand.Next(200) / 1000d;
         p.Initiailize();
         Particles.Add(p);
      }

      public void Update(double elapsedSeconds)
      {
          elapsedRemainder += elapsedSeconds;
          while (elapsedRemainder > updateInterval)
          {
              elapsedRemainder -= updateInterval;
              CreateParticle();
              particleColor.H += .1;
              particleColor.H = particleColor.H % 255;
              for (int i = Particles.Count - 1; i >= 0; i--)
              {
                  Particle p = Particles[i];
                  p.Update(updateInterval);
                  if (p.Color.A == 0) Particles.Remove(p);
              }
          }
          using (TargetBitmap.GetBitmapContext())
          {
              using (ParticleBitmap.GetBitmapContext())
              {
                  for (int i = 0; i < Particles.Count; i++)
                  {
                      Particle p = Particles[i];
                      TargetBitmap.Blit(p.Position, ParticleBitmap, sourceRect, p.Color, WriteableBitmapExtensions.BlendMode.Additive);
                  }
              }
          }
      }
      #endregion
   }
}
