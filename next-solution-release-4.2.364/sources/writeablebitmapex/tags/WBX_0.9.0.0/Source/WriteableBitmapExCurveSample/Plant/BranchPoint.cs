#region Header
//
//   Project:           WriteableBitmapEx - Silverlight WriteableBitmap extensions
//   Description:       A branching point of a plant.
//
//   Changed by:        $Author$
//   Changed on:        $Date$
//   Changed in:        $Revision$
//   Project:           $URL$
//   Id:                $Id$
//
//
//   Copyright © 2009-2010 Rene Schulte and WriteableBitmapEx Contributors
//
//   This Software is weak copyleft open source. Please read the License.txt for details.
//
#endregion


using System.Collections.Generic;
using System;

namespace WriteableBitmapExCurveSample
{
   /// <summary>
   /// A branching point of a plant.
   /// </summary>
   public struct BranchPoint
   {
      public float Time;
      public int Angle;

      public BranchPoint(float time, int angle)
      {
         this.Time = time;
         this.Angle = angle;
      }
   }
}
