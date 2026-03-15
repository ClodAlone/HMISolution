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
