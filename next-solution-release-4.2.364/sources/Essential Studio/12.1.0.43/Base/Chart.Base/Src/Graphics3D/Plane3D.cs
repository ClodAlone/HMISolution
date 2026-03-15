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
using System.Drawing;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// Represents the 3D plane.
    /// </summary>
    public class Plane3D
    {
        #region Members
        /// <summary>
        /// The normal of plane.
        /// </summary>
        protected Vector3D m_normal;

        /// <summary>
        /// The constant of plane.
        /// </summary>
        protected double m_d;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the normal.
        /// </summary>
        /// <value>The normal.</value>
        public Vector3D Normal
        {
            get
            {
                return m_normal;
            }
        }

        /// <summary>
        /// Gets the A component.
        /// </summary>
        /// <value>The A component.</value>
        public double A
        {
            get
            {
                return m_normal.X;
            }
        }

        /// <summary>
        /// Gets the B component.
        /// </summary>
        /// <value>The B component.</value>
        public double B
        {
            get
            {
                return m_normal.Y;
            }
        }

        /// <summary>
        /// Gets the C component.
        /// </summary>
        /// <value>The C component.</value>
        public double C
        {
            get
            {
                return m_normal.Z;
            }
        }

        /// <summary>
        /// Gets the D component.
        /// </summary>
        /// <value>The D component.</value>
        public double D
        {
            get
            {
                return m_d;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="Plane3D"/> class.
        /// </summary>
        /// <param name="normal">The normal.</param>
        /// <param name="d">The d.</param>
        public Plane3D(Vector3D normal, double d)
        {
            m_normal = normal;
            m_d = d;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Plane3D"/> class.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <param name="c">The c.</param>
        /// <param name="d">The d.</param>
        public Plane3D(double a, double b, double c, double d)
        {
            m_normal = new Vector3D(a, b, c);
            m_d = d;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Plane3D"/> class.
        /// </summary>
        /// <param name="v1">The v1.</param>
        /// <param name="v2">The v2.</param>
        /// <param name="v3">The v3.</param>
        public Plane3D(Vector3D v1, Vector3D v2, Vector3D v3)
        {
            CalcNormal(v1, v2, v3);
        }
        #endregion

        #region Public methots
        /// <summary>
        /// Gets the point on the plane.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>Returns Vector3D instance.</returns>
        public Vector3D GetPoint(double x, double y)
        {
            double z = -(A * x + B * y + D) / C;

            return new Vector3D(x, y, z);
        }

        /// <summary>
        /// Gets the point of intersect ray with plane.
        /// </summary>
        /// <param name="pos">The pos.</param>
        /// <param name="ray">The ray.</param>
        /// <returns>Returns Vector3D instance.</returns>
        public Vector3D GetPoint(Vector3D pos, Vector3D ray)
        {
            Vector3D dir = m_normal * (-m_d) - pos;

            double sv = dir & m_normal;
            double sect = sv / (m_normal & ray);

            return pos + ray * sect;
        }

        /// <summary>
        /// Transforms by the specified matrix.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        public virtual void Transform(Matrix3D matrix)
        {
            Vector3D v = matrix * (m_normal * -m_d);

            m_normal = matrix & m_normal;
            m_normal.Normalize();
            m_d = -(m_normal & v);
        }

        /// <summary>
        /// Clones this instance and apply the specified transformation.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <returns>Returns Plane3D instance.</returns>
        public Plane3D Clone(Matrix3D matrix)
        {
            Plane3D res = new Plane3D(m_normal, m_d);

            res.Transform(matrix);

            return res;
        }

        /// <summary>
        /// Tests this instance to the existing.
        /// </summary>
        /// <returns>Indicates whether Normal of Plane is valid or Not.</returns>
        public bool Test()
        {
            return !m_normal.IsValid;
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Calculates the normal.
        /// </summary>
        /// <param name="v1">The v1.</param>
        /// <param name="v2">The v2.</param>
        /// <param name="v3">The v3.</param>
        protected void CalcNormal(Vector3D v1, Vector3D v2, Vector3D v3)
        {
            m_normal = ChartMath.GetNormal(v1, v2, v3);
            m_d = -(A * v1.X + B * v1.Y + C * v1.Z);
        }
        #endregion
    }
}
