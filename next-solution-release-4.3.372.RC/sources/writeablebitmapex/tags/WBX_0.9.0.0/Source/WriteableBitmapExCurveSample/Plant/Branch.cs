#region Header
//
//   Project:           WriteableBitmapEx - Silverlight WriteableBitmap extensions
//   Description:       A branch of a plant.
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
   /// A branch of a plant.
   /// </summary>
   public class Branch
   {
      public List<Branch> Branches  { get; private set; }
      public Vector Start           { get; set; }
      public Vector Middle          { get; set; }
      public Vector MiddleTarget    { get; set; }
      public Vector End             { get; set; }
      public Vector EndTarget       { get; set; }
      public float Life             { get; private set; }
      public float GrowthRate       { get; private set; }

      public Branch()
      {
         this.Branches     = new List<Branch>();
         this.Life         = 0;
      }

      public Branch(Vector start, Vector middleTarget, Vector endTarget, float growthRate)
         : this()
      {
         this.Start        = start;
         this.MiddleTarget = middleTarget;
         this.Middle       = start;
         this.EndTarget    = endTarget;
         this.End          = start;
         this.GrowthRate   = growthRate;
      }

      public void Grow()
      {
         // Slightly overlap
         const float endStart = 0.3f;
         const float endEnd = 1;
         if (Life >= endStart && Life <= endEnd)
         {
            End = Start.Interpolate(EndTarget, (Life - endStart) * (1 / (endEnd - endStart)));
         }
         else if (Life <= 0.5)
         {
            Middle = Start.Interpolate(MiddleTarget, Life * 2);
         }

         // Everyone gets older ya know
         Life += this.GrowthRate;
      }
   }
}
