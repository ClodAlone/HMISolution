#region Header
//
//   Project:           WriteableBitmapEx - Silverlight WriteableBitmap extensions
//   Description:       Blit Sample for the WriteableBitmap extension methods.
//
//   Changed by:        $Author: unknown $
//   Changed on:        $Date: 2011-12-16 19:50:22 +0100 (Fr, 16 Dez 2011) $
//   Changed in:        $Revision: 83935 $
//   Project:           $URL: https://writeablebitmapex.svn.codeplex.com/svn/branches/WBX_1.0_BitmapContext/Source/WriteableBitmapExBlitSample/Particle.cs $
//   Id:                $Id: Particle.cs 83935 2011-12-16 18:50:22Z unknown $
//
//
//   Copyright © 2009-2010 Bill Reiss, Rene Schulte and WriteableBitmapEx Contributors
//
//   This Software is weak copyleft open source. Please read the License.txt for details.
//
#endregion

using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace WriteableBitmapExBlitSample
{
   public class Particle
   {
      #region Fields

      public Point Position;
      public Point Velocity;
      public Color Color;
      public double Lifespan;
      public double Elapsed;

      #endregion

      #region Methods

      public void Initiailize()
      {
         Elapsed = 0;
      }

      public void Update(double elapsedSeconds)
      {
         Elapsed += elapsedSeconds;
         if (Elapsed > Lifespan)
         {
            Color.A = 0;
            return;
         }
         Color.A = (byte)(255 - ((255 * Elapsed)) / Lifespan);
         Position.X += Velocity.X * elapsedSeconds;
         Position.Y += Velocity.Y * elapsedSeconds;
      }

      #endregion
   }
}
