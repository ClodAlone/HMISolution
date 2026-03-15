#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using System;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// Represents the coordinates of a 3D point.
    /// </summary>
    public struct Vector3D
    {
        #region Members
        internal double m_x;
        internal double m_y;
        internal double m_z;

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
                return m_x;
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
                return m_y;
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
                return m_z;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        /// <value><c>True</c> if this instance is empty; otherwise, <c>false</c>.</value>
        public bool IsEmpty
        {
            get
            {
                return m_x == 0 && m_y == 0 && m_z == 0;
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
                return !double.IsNaN(m_x) && !double.IsNaN(m_y) && !double.IsNaN(m_z);
            }
        }
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector3D"/> struct.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="z">The z.</param>
        public Vector3D(double x, double y, double z)
        {
            m_x = x;
            m_y = y;
            m_z = z;
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
            return new Vector3D(v1.m_x - v2.m_x, v1.m_y - v2.m_y, v1.m_z - v2.m_z);
        }

        /// <summary>
        /// Implements the operator +.
        /// </summary>
        /// <param name="v1">The v1.</param>
        /// <param name="v2">The v2.</param>
        /// <returns>The result of the operator.</returns>
        public static Vector3D operator +(Vector3D v1, Vector3D v2)
        {
            return new Vector3D(v1.m_x + v2.m_x, v1.m_y + v2.m_y, v1.m_z + v2.m_z);
        }

        /// <summary>
        /// Implements the cross product operation.
        /// </summary>
        /// <param name="v1">The v1.</param>
        /// <param name="v2">The v2.</param>
        /// <returns>The result of the operator.</returns>
        public static Vector3D operator *(Vector3D v1, Vector3D v2)
        {
            double x = v1.m_y * v2.m_z - v2.m_y * v1.m_z;
            double y = v1.m_z * v2.m_x - v2.m_z * v1.m_x;
            double z = v1.m_x * v2.m_y - v2.m_x * v1.m_y;

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
            return (v1.m_x * v2.m_x + v1.m_y * v2.m_y + v1.m_z * v2.m_z);
        }

        /// <summary>
        /// Implements the operator *.
        /// </summary>
        /// <param name="v1">The v1.</param>
        /// <param name="val">The val.</param>
        /// <returns>The result of the operator.</returns>
        public static Vector3D operator *(Vector3D v1, double val)
        {
            double x = v1.m_x * val;
            double y = v1.m_y * val;
            double z = v1.m_z * val;

            return new Vector3D(x, y, z);
        }

        /// <summary>
        /// Implements the operator !.
        /// </summary>
        /// <param name="v1">The v1.</param>
        /// <returns>The result of the operator.</returns>
        public static Vector3D operator !(Vector3D v1)
        {
            return new Vector3D(-v1.m_x, -v1.m_y, -v1.m_z);
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="v1">The v1.</param>
        /// <param name="v2">The v2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(Vector3D v1, Vector3D v2)
        {
            return (v1.m_x == v2.m_x) && (v1.m_y == v2.m_y) && (v1.m_z == v2.m_z);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="v1">The v1.</param>
        /// <param name="v2">The v2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(Vector3D v1, Vector3D v2)
        {
            return (v1.m_x != v2.m_x) || (v1.m_y != v2.m_y) || (v1.m_z != v2.m_z);
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
            double l = GetLength();

            m_x /= l;
            m_y /= l;
            m_z /= l;
        }

        /// <summary>
        /// Overrides <see cref="Object.ToString"/> method.
        /// </summary>
        /// <returns>The text.</returns>
        public override string ToString()
        {
            return String.Format("X = {0}, Y = {1}, Z = {2}", m_x, m_y, m_z);
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
            bool res = false;

            if (obj is Vector3D)
            {
                Vector3D v1 = (Vector3D)obj;
                res = (v1.m_x == m_x) && (v1.m_y == m_y) && (v1.m_z == m_z);
            }

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
            return m_x.GetHashCode() ^ m_y.GetHashCode() ^ m_z.GetHashCode();
        }
        #endregion
    }
}
