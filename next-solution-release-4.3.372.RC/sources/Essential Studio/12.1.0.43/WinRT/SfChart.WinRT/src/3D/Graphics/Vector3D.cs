#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Windows;
#if !WINDOWS_PHONE
using Windows.Foundation;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents the coordinates of a 3D point.
    /// </summary>
    public struct Vector3D
    {
        #region Members
        internal double x;
        internal double y;
        internal double z;

        /// <summary>
        /// The empty <see cref="Vector3D"/>. All coordinates is zero.
        /// </summary>
        public static readonly Vector3D Empty = new Vector3D(0, 0, 0);
        #endregion

        #region Properties
        /// <summary>
        /// Gets the X coordinate.
        /// </summary>
        /// <value>The X.</value>
        public double X
        {
            get
            {
                return x;
            }
        }

        /// <summary>
        /// Gets the Y coordinate.
        /// </summary>
        /// <value>The Y.</value>
        public double Y
        {
            get
            {
                return y;
            }
        }

        /// <summary>
        /// Gets the Z coordinate.
        /// </summary>
        /// <value>The Z.</value>
        public double Z
        {
            get
            {
                return z;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is valid.
        /// </summary>
        /// <value><c>True</c> if this instance is valid; otherwise, <c>false</c>.</value>
        public bool IsValid
        {
            get
            {
                return !double.IsNaN(x) && !double.IsNaN(y) && !double.IsNaN(z);
            }
        }
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector3D"/> struct.
        /// </summary>
        public Vector3D(double vx, double vy, double vz)
        {
            x = vx;
            y = vy;
            z = vz;
        }

        public Vector3D(Point points, double vz)
        {
            x = points.X;
            y = points.Y;
            z = vz;
        }
        #endregion

        #region Operations
        /// <summary>
        /// Implements the operator -.
        /// </summary>
        /// <param name="v1">The v1.</param>
        /// <param name="v2">The v2.</param>
        /// <returns>The result of the operator.</returns>
        public static Vector3D operator -(Vector3D v1, Vector3D v2)
        {
            return new Vector3D(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);
        }

        /// <summary>
        /// Implements the operator +.
        /// </summary>
        /// <param name="v1">The v1.</param>
        /// <param name="v2">The v2.</param>
        /// <returns>The result of the operator.</returns>
        public static Vector3D operator +(Vector3D v1, Vector3D v2)
        {
            return new Vector3D(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
        }

        /// <summary>
        /// Implements the cross product operation.
        /// </summary>
        /// <param name="v1">The v1.</param>
        /// <param name="v2">The v2.</param>
        /// <returns>The result of the operator.</returns>
        public static Vector3D operator *(Vector3D v1, Vector3D v2)
        {
            var x = v1.y * v2.z - v2.y * v1.z;
            var y = v1.z * v2.x - v2.z * v1.x;
            var z = v1.x * v2.y - v2.x * v1.y;

            return new Vector3D(x, y, z);
        }

        /// <summary>
        /// Implements the dot product operation.
        /// </summary>
        /// <param name="v1">The v1.</param>
        /// <param name="v2">The v2.</param>
        /// <returns>The result of the operator.</returns>
        public static double operator &(Vector3D v1, Vector3D v2)
        {
            return (v1.x * v2.x + v1.y * v2.y + v1.z * v2.z);
        }

        /// <summary>
        /// Implements the operator *.
        /// </summary>
        /// <param name="v1">The v1.</param>
        /// <param name="val">The val.</param>
        /// <returns>The result of the operator.</returns>
        public static Vector3D operator *(Vector3D v1, double val)
        {
            var x = v1.x * val;
            var y = v1.y * val;
            var z = v1.z * val;

            return new Vector3D(x, y, z);
        }

        #endregion

        #region Public methods
        /// <summary>
        /// Gets the length.
        /// </summary>
        /// <returns></returns>
        public double GetLength()
        {
            return Math.Sqrt(this & this);
        }

        /// <summary>
        /// Normalizes this vector.
        /// </summary>
        public void Normalize()
        {
            var l = GetLength();

            x /= l;
            y /= l;
            z /= l;
        }

        /// <summary>
        /// Overrides <see cref="Object.ToString"/> method.
        /// </summary>
        /// <returns>The text.</returns>
        public override string ToString()
        {
            return String.Format("X = {0}, Y = {1}, Z = {2}", x, y, z);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">Another object to compare to.</param>
        /// <returns>
        /// true if obj and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            bool res;

            if (!(obj is Vector3D)) return false;
            var v1 = (Vector3D)obj;
            res = (Math.Abs(v1.x - x) < 0d) && (Math.Abs(v1.y - y) < 0d) && (Math.Abs(v1.z - z) < 0d);

            return res;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode();
        }
        #endregion
    }
}
