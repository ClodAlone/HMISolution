// <copyright file="ChartMath.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

namespace ChartWpf
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Represents ChartMath class
    /// </summary>
    /// <exclude/>
    #if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
    #endif
    internal class ChartMath
    {
        #region Constants
        /// <summary>
        /// Declares DEGREE_TO_RADIAL
        /// </summary>
        public const double DEGREE_TO_RADIAL = Math.PI / 180;

        /// <summary>
        /// Declares RADIAL_TO_DEGREE
        /// </summary>
        public const double RADIAL_TO_DEGREE = 180 / Math.PI;
        #endregion

        #region Public methods
        /// <summary>
        /// Method to calculate Camera value
        /// </summary>
        /// <param name="a">The Vector3D a value</param>
        /// <param name="b">The Vector3D b value</param>
        /// <param name="c">The Vector3D c value</param>
        /// <param name="d">The Vector3D d value</param>
        /// <returns>Returns Vector3D</returns>
        public static Vector3D CalcCameron(Vector3D a, Vector3D b, Vector3D c, Vector3D d)
        {
            Matrix3D mat = new Matrix3D(a.X, a.Y, a.Z, 0, b.X, b.Y, b.Z, 0, c.X, c.Y, c.Z, 0, 0, 0, 0, 1);
            Matrix3D mat1 = new Matrix3D(d.X, a.Y, a.Z, 0, d.Y, b.Y, b.Z, 0, d.Z, c.Y, c.Z, 0, 0, 0, 0, 1);
            Matrix3D mat2 = new Matrix3D(a.X, d.X, a.Z, 0, b.X, d.Y, b.Z, 0, c.X, d.Z, c.Z, 0, 0, 0, 0, 1);
            Matrix3D mat3 = new Matrix3D(a.X, a.Y, d.X, 0, b.X, b.Y, d.Y, 0, c.X, c.Y, d.Z, 0, 0, 0, 0, 1);

            double md = mat.Determinant;
            double md1 = mat1.Determinant;
            double md2 = mat2.Determinant;
            double md3 = mat3.Determinant;

            return new Vector3D(md1 / md, md2 / md, md3 / md);
        }

        /// <summary>
        /// GetRotateBy method
        /// </summary>
        /// <param name="oDirVector">The first DirVector</param>
        /// <param name="oUpVector">The first UpVector</param>
        /// <param name="nDirVector">The second DirVector</param>
        /// <param name="nUpVector">The second UpVector</param>
        /// <returns>The RotateTransform3D</returns>
        public static RotateTransform3D GetRotateBy(Vector3D oDirVector, Vector3D oUpVector, Vector3D nDirVector, Vector3D nUpVector)
        {
            RotateTransform3D rt = new RotateTransform3D();

            oUpVector.Normalize();
            nUpVector.Normalize();
            oDirVector.Normalize();
            nDirVector.Normalize();

            if (oUpVector != nUpVector)
            {
                rt.Rotation = new AxisAngleRotation3D(Vector3D.CrossProduct(oUpVector, nUpVector), Math.Acos(Vector3D.AngleBetween(oUpVector, nUpVector)));
            }

            return rt;
        }
        #endregion
    }
}
