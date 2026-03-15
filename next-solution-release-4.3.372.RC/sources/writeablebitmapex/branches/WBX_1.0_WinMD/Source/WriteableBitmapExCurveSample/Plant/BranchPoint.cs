#region Header
//
//   Project:           Silverlight procedural Plant
//
//   Changed by:        $Author: unknown $
//   Changed on:        $Date: 2011-12-16 19:50:22 +0100 (Fr, 16 Dez 2011) $
//   Changed in:        $Revision: 83935 $
//   Project:           $URL: https://writeablebitmapex.svn.codeplex.com/svn/branches/WBX_1.0_BitmapContext/Source/WriteableBitmapExCurveSample/Plant/BranchPoint.cs $
//   Id:                $Id: BranchPoint.cs 83935 2011-12-16 18:50:22Z unknown $
//
//
//   Copyright © 2010 Rene Schulte and WriteableBitmapEx Contributors
//
//   This Software is weak copyleft open source. Please read the License.txt for details.
//
#endregion

using System.Collections.Generic;
using System;

namespace Schulte.Silverlight
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
