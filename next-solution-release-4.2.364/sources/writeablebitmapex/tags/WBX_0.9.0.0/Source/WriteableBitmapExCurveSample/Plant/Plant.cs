#region Header
//
//   Project:           WriteableBitmapEx - Silverlight WriteableBitmap extensions
//   Description:       A simple plant.
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
using System.Windows.Media.Imaging;
using System.Collections.Generic;

namespace WriteableBitmapExCurveSample
{
   /// <summary>
   /// A simple plant.
   /// </summary>
   public class Plant
   {
      private Random rand;

      public Branch Root               { get; set; }
      public float Tension             { get; set; }
      public int MaxWidth              { get; private set; }
      public int MaxHeight             { get; private set; }
      public int BranchLenMin          { get; set; }
      public int BranchLenMax          { get; set; }
      public int BranchAngleVariance   { get; set; }
      public float GrowthRate          { get; set; }
      public int MaxGenerations        { get; set; }
      public Color Color               { get; set; }
      public Vector Start              { get; private set; }
      public Vector Scale              { get; private set; }
      public List<BranchPoint> BranchPoints { get; private set; }

      public Plant()
      {
         this.Tension      = 1f;
         this.rand         = new Random();
         this.BranchLenMin = 150;
         this.BranchLenMax = 200;
         this.GrowthRate   = 0.007f;
         this.BranchPoints = new List<BranchPoint>  
         { 
            new BranchPoint(1f,  40), // 40° Right at 100% of branch
            new BranchPoint(1f,   5), //  5° Right at 100% of branch
            new BranchPoint(1f,  -5), //  5° Left  at 100% of branch
            new BranchPoint(1f, -40), // 40° Left  at 100% of branch
         };
         this.BranchAngleVariance = 10;
         this.MaxGenerations      = int.MaxValue;
         this.Color               = Color.FromArgb(255, 100, 150, 0);
         this.Start               = Vector.Zero;
         this.Scale               = Vector.One;
      }

      public Plant(Vector start, Vector scale, int viewPortWidth, int viewPortHeight)
         : this()
      {
         this.Tension = 0.5f;
         this.Initialize(start, scale, viewPortWidth, viewPortHeight);
      }

      public void Initialize(Vector start, Vector scale, int viewPortWidth, int viewPortHeight)
      {
         this.Start = start;
         this.Scale = scale;
         MaxWidth = viewPortWidth;
         MaxHeight = viewPortHeight;
         Restart();
      }

      public void Restart()
      {
         var end = new Vector(Start.X, Start.Y + ((MaxHeight >> 4) * Scale.Y));
         this.Root = new Branch(Start, Start, end, GetRandomGrowthRate());
      }


      public void Grow()
      {
         Grow(this.Root, 0);
      }

      private void Grow(Branch branch, int generation)
      {
         if (generation <= MaxGenerations)
         {
            if (branch.End.Y >= 0 && branch.End.Y <= MaxHeight 
             && branch.End.X >= 0 && branch.End.X <= MaxWidth)
            {
               // Grow it
               branch.Grow();

               // Branch?
               foreach (var bp in BranchPoints)
               {
                  if (branch.Life >= bp.Time && branch.Life <= bp.Time + GrowthRate)
                  {
                     // Length and angle of the branch
                     var branchLen = rand.Next(BranchLenMin, BranchLenMax);
                     branchLen -= (int)(branchLen * 0.01f * generation);
                     var angle = rand.Next(bp.Angle - BranchAngleVariance, bp.Angle + BranchAngleVariance) * 0.017453292519943295769236907684886;

                     // Desired end of new branch
                     var endTarget = new Vector(branch.End.X + ((int)(Math.Sin(angle) * branchLen) * Scale.X),
                                                branch.End.Y + ((int)(Math.Cos(angle) * branchLen) * Scale.Y));

                     // Desired middle point
                     var bending = rand.Next(branchLen >> 4, branchLen >> 3) * 0.02;
                     angle += Math.Sign(bp.Angle) * bending;
                     var middleTarget = new Vector(branch.End.X + ((int)(Math.Sin(angle) * (branchLen >> 1)) * Scale.X),
                                                   branch.End.Y + ((int)(Math.Cos(angle) * (branchLen >> 1)) * Scale.Y));

                     // Add new branch
                     branch.Branches.Add(new Branch(branch.End, middleTarget, endTarget, GetRandomGrowthRate()));
                  }
               }
            }

            // Grow the child branches
            foreach (var b in branch.Branches)
            {
               Grow(b, generation+1);
            }
         }
      }

      private float GetRandomGrowthRate()
      {
          var r = (float)rand.NextDouble() * GrowthRate - GrowthRate * 0.5f;
          return GrowthRate + r;
      }

      public void Draw(WriteableBitmap writeableBmp)
      {
         if (writeableBmp != null)
         {
            writeableBmp.Clear();
            Draw(writeableBmp, this.Root);
            writeableBmp.Invalidate();
         }
      }

      private void Draw(WriteableBitmap writeableBmp, Branch branch)
      {
         int[] pts = new int[] 
         { 
            branch.Start.X,   branch.Start.Y,
            branch.Middle.X,  branch.Middle.Y,
            branch.End.X,     branch.End.Y,
         };

         // Draw with cardinal spline
         writeableBmp.DrawCurve(pts, Tension, this.Color);

         foreach (var b in branch.Branches)
         {
            Draw(writeableBmp, b);
         }
      }
   }
}
