#region Header
//
//   Project:           Silverlight procedural Plant
//
//   Changed by:        $Author$
//   Changed on:        $Date$
//   Changed in:        $Revision$
//   Project:           $URL$
//   Id:                $Id$
//
//
//   Copyright © 2010 Rene Schulte and WriteableBitmapEx Contributors
//
//   This Software is weak copyleft open source. Please read the License.txt for details.
//
#endregion

using System.Windows;
using System;

namespace Schulte.Silverlight
{
   /// <summary>
   /// Integer vector.
   /// </summary>
   public struct Vector
   {
      public int X;
      public int Y;


      public static Vector Zero { get { return new Vector(0, 0); } }
      public static Vector One { get { return new Vector(1, 1); } }
      
      public int Length { get { return (int)System.Math.Sqrt(X * X + Y * Y); } }


      public Vector(int x, int y)
      {
         this.X = x;
         this.Y = y;
      }

      public Vector(Point point)
         : this((int)point.X, (int) point.Y)
      {
      }


      public static Vector operator +(Vector v1, Vector v2)
      {
         return new Vector(v1.X + v2.X, v1.Y + v2.Y);
      }

      public static Vector operator -(Vector v1, Vector v2)
      {
         return new Vector(v1.X - v2.X, v1.Y - v2.Y);
      }

      public static Vector operator *(Vector p, int s)
      {
         return new Vector(p.X * s, p.Y * s);
      }

      public static Vector operator *(int s, Vector p)
      {
         return new Vector(p.X * s, p.Y * s);
      }

      public static Vector operator *(Vector p, float s)
      {
         return new Vector((int)(p.X * s), (int)(p.Y * s));
      }

      public static Vector operator *(float s, Vector p)
      {
         return new Vector((int)(p.X * s), (int)(p.Y * s));
      }

      public static bool operator ==(Vector v1, Vector v2)
      {
         return v1.X == v2.X && v1.Y == v2.Y;
      }

      public static bool operator !=(Vector v1, Vector v2)
      {
         return v1.X != v2.X || v1.Y != v2.Y;
      }


      public Vector Interpolate(Vector v2, float amount)
      {
         return new Vector((int)(this.X + ((v2.X - this.X) * amount)), (int)(this.Y + ((v2.Y - this.Y) * amount)));
      }

      public int Dot(Vector v2)
      {
         return this.X * v2.X + this.Y * v2.Y;
      }

      public int Angle(Vector v2)
      {
         // Normalize this
         double s1 = 1.0f / this.Length;
         double x1 = this.X * s1;
         double y1 = this.Y * s1;

         // Normalize v2
         double s2 = 1.0f / v2.Length;
         double x2 = v2.X * s2;
         double y2 = v2.Y * s2;

         // The dot product is the cosine between the two vectors
         double dot = x1 * x2 + y1 * y2;
         double rad = Math.Acos(dot);
         
         // return the angle in degrees
         return (int)(rad * 57.295779513082320876798154814105);
      }


      public override bool Equals(object obj)
      {
         if (obj is Vector)
         {
            return ((Vector)obj) == this;
         }
         return false;
      }

      public override int GetHashCode()
      {
         return X.GetHashCode() ^ Y.GetHashCode();
      }

      public override string ToString()
      {
         return String.Format("({0}, {1})", X, Y);;
      }
   }
}