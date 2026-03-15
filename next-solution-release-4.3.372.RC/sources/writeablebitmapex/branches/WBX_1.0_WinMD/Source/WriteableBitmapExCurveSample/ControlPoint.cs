#region Header
//
//   Project:           WriteableBitmapEx - Silverlight WriteableBitmap extensions
//   Description:       A control point for a spline curve.
//
//   Changed by:        $Author: unknown $
//   Changed on:        $Date: 2012-05-03 23:12:09 +0200 (Do, 03 Mai 2012) $
//   Changed in:        $Revision: 90031 $
//   Project:           $URL: https://writeablebitmapex.svn.codeplex.com/svn/branches/WBX_1.0_BitmapContext/Source/WriteableBitmapExCurveSample/ControlPoint.cs $
//   Id:                $Id: ControlPoint.cs 90031 2012-05-03 21:12:09Z unknown $
//
//
//   Copyright © 2009-2010 Rene Schulte and WriteableBitmapEx Contributors
//
//   This Software is weak copyleft open source. Please read the License.txt for details.
//
#endregion


using System.Windows;
using System;
using Schulte.Silverlight;

#if NETFX_CORE
using Windows.Foundation;
#endif

using Vector = Schulte.Silverlight.Vector;

namespace WriteableBitmapExCurveSample
{
   /// <summary>
   /// A control point for a spline curve.
   /// </summary>
   public class ControlPoint
   {
      private Vector point;      
      
      public int X { get { return point.X; } set { point.X = value; } }
      public int Y { get { return point.Y; } set { point.Y = value; } }
      

      public ControlPoint(Vector point)
      {
         this.point = point;
      }

      public ControlPoint()
         : this(Vector.Zero)
      {
      }

      public ControlPoint(int x, int y)
         : this(new Vector(x, y))
      {
      }

      public ControlPoint(Point point)
         : this((int)point.X, (int) point.Y)
      {
      }

      public override string ToString()
      {
         return String.Format("({0}, {1})", X, Y);;
      }
   }
}