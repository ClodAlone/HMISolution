#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents the matrix 4x4.
    /// </summary>
    public struct Matrix3D
    {
        #region Constants
        private const int MATRIX_SIZE = 4;
        #endregion

        #region Members
        private readonly double[][] mData;
        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether this matrix is affine.
        /// </summary>
        /// <value><c>true</c> if this matrix is affine; otherwise, <c>false</c>.</value>
        public bool IsAffine
        {
            get
            {
                return (mData[0][3] == 0) && (mData[1][3] == 0)
                  && (mData[2][3] == 0) && (mData[3][3] == 1);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Double"/> with the specified column and row.
        /// </summary>
        /// <value></value>
        public double this[int i, int j]
        {
            get
            {
                return mData[i][j];
            }

            set
            {
                mData[i][j] = value;
            }
        }

        /// <summary>
        /// Gets the identity matrix.
        /// </summary>
        /// <value>The identity matrix.</value>
        public static Matrix3D Identity
        {
            get
            {
                return GetIdentity();
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix3D"/> class.
        /// </summary>
        /// <param name="size">The size.</param>
        private Matrix3D(int size)
        {
            mData = new double[size][];

            for (var i = 0; i < size; i++)
            {
                mData[i] = new double[size];
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix3D"/> class.
        /// </summary>
        /// <param name="m11">The M11 element of matrix.</param>
        /// <param name="m12">The M12 element of matrix.</param>
        /// <param name="m13">The M13 element of matrix.</param>
        /// <param name="m14">The M14 element of matrix.</param>
        /// <param name="m21">The M21 element of matrix.</param>
        /// <param name="m22">The M22 element of matrix.</param>
        /// <param name="m23">The M23 element of matrix.</param>
        /// <param name="m24">The M24 element of matrix.</param>
        /// <param name="m31">The M31 element of matrix.</param>
        /// <param name="m32">The M32 element of matrix.</param>
        /// <param name="m33">The M33 element of matrix.</param>
        /// <param name="m34">The M34 element of matrix.</param>
        /// <param name="m41">The M41 element of matrix.</param>
        /// <param name="m42">The M42 element of matrix.</param>
        /// <param name="m43">The M43 element of matrix.</param>
        /// <param name="m44">The M44 element of matrix.</param>
        public Matrix3D(double m11, double m12, double m13, double m14,
                        double m21, double m22, double m23, double m24,
                        double m31, double m32, double m33, double m34,
                        double m41, double m42, double m43, double m44)
                                                                  : this(MATRIX_SIZE)
                    {
                        mData[0][0] = m11;
                        mData[1][0] = m12;
                        mData[2][0] = m13;
                        mData[3][0] = m14;

                        mData[0][1] = m21;
                        mData[1][1] = m22;
                        mData[2][1] = m23;
                        mData[3][1] = m24;

                        mData[0][2] = m31;
                        mData[1][2] = m32;
                        mData[2][2] = m33;
                        mData[3][2] = m34;

                        mData[0][3] = m41;
                        mData[1][3] = m42;
                        mData[2][3] = m43;
                        mData[3][3] = m44;
                    }
        #endregion

        #region Public methods

        #region Operators
        /// <summary>
        /// 
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <returns></returns>
        public static Matrix3D operator +(Matrix3D m1, Matrix3D m2)
        {
            var m = new Matrix3D(4);

            for (var i = 0; i < 4; i++)
            {
                for (var j = 0; j < 4; j++)
                {
                    m[i, j] = m1[i, j] + m2[i, j];
                }
            }

            return m;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Vector3D operator *(Matrix3D m1, Vector3D point)
        {
            var x = m1.mData[0][0] * point.x + m1.mData[1][0] * point.y + m1.mData[2][0] * point.z + m1.mData[3][0];
            var y = m1.mData[0][1] * point.x + m1.mData[1][1] * point.y + m1.mData[2][1] * point.z + m1.mData[3][1];
            var z = m1.mData[0][2] * point.x + m1.mData[1][2] * point.y + m1.mData[2][2] * point.z + m1.mData[3][2];

            if (!m1.IsAffine)
            {
                var c = 1d / (m1.mData[0][3] * point.x + m1.mData[1][3] * point.y + m1.mData[2][3] * point.z + m1.mData[3][3]);
                x *= c;
                y *= c;
                z *= c;
            }

            return new Vector3D(x, y, z);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vector3D operator &(Matrix3D m1, Vector3D v1)
        {
            var x = m1.mData[0][0] * v1.x + m1.mData[1][0] * v1.y + m1.mData[2][0] * v1.z;
            var y = m1.mData[0][1] * v1.x + m1.mData[1][1] * v1.y + m1.mData[2][1] * v1.z;
            var z = m1.mData[0][2] * v1.x + m1.mData[1][2] * v1.y + m1.mData[2][2] * v1.z;

            return new Vector3D(x, y, z);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="f1"></param>
        /// <param name="m1"></param>
        /// <returns></returns>
        public static Matrix3D operator *(double f1, Matrix3D m1)
        {
            var length = m1.mData.Length;
            var res = new Matrix3D(length);

            for (var i = 0; i < length; i++)
            {
                for (var j = 0; j < length; j++)
                {
                    res.mData[i][j] = m1.mData[i][j] * f1;
                }
            }

            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <returns></returns>
        public static Matrix3D operator *(Matrix3D m1, Matrix3D m2)
        {
            var res = GetIdentity();

            for (var i = 0; i < MATRIX_SIZE; i++)
            {
                for (var j = 0; j < MATRIX_SIZE; j++)
                {
                    double v = 0;

                    for (var k = 0; k < MATRIX_SIZE; k++)
                    {
                        v += m1[k, j] * m2[i, k];
                    }

                    res[i, j] = v;
                }
            }

            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <returns></returns>
        public static bool operator ==(Matrix3D m1, Matrix3D m2)
        {
            var res = true;

            for (var i = 0; i < m1.mData.Length; i++)
            {
                for (var j = 0; j < m1.mData.Length; j++)
                {
                    if (m1.mData[i][j] != m2.mData[i][j])
                    {
                        res = false;
                    }
                }
            }

            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <returns></returns>
        public static bool operator !=(Matrix3D m1, Matrix3D m2)
        {
            var res = true;

            for (var i = 0; i < m1.mData.Length; i++)
            {
                for (var j = 0; j < m1.mData.Length; j++)
                {
                    if (m1.mData[i][j] != m2.mData[i][j])
                    {
                        res = false;
                    }
                }
            }

            return !res;
        }
        #endregion

        /// <summary>
        /// Intervals the matrix.
        /// </summary>
        /// <param name="matrix3D">The matrix.</param>
        /// <returns></returns>
        internal static Matrix3D GetInvertal(Matrix3D matrix3D)
        {
            var m = Identity;

            for (var i = 0; i < MATRIX_SIZE; i++)
            {
                for (var j = 0; j < MATRIX_SIZE; j++)
                {
                    m[i, j] = GetMinor(matrix3D, i, j);
                }
            }

            m = Transposed(m);
            m = (1f / GetD(matrix3D)) * m;

            return m;
        }

        /// <summary>
        /// Gets the minor.
        /// </summary>
        /// <param name="dd">The matrix.</param>
        /// <param name="columnIndex">The index of column.</param>
        /// <param name="rowIndex">The index of row.</param>
        /// <returns></returns>
        internal static double GetMinor(Matrix3D dd, int columnIndex, int rowIndex)
        {
            return (((columnIndex + rowIndex) % 2 == 0) ? 1 : -1) * GetDeterminant(GetMMtr(dd.mData, columnIndex, rowIndex));
        }

        /// <summary>
        /// Gets the determinant.
        /// </summary>
        /// <param name="matrix3D">The matrix.</param>
        /// <returns></returns>
        public static double GetD(Matrix3D matrix3D)
        {
            return GetDeterminant(matrix3D.mData);
        }

        /// <summary>
        /// Gets the identity matrix.
        /// </summary>
        /// <returns></returns>
        public static Matrix3D GetIdentity()
        {
            var m = new Matrix3D(MATRIX_SIZE);

            for (var i = 0; i < MATRIX_SIZE; i++)
            {
                m[i, i] = 1.0f;
            }

            return m;
        }

        /// <summary>
        /// Transforms the specified vector.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        /// <param name="z">The Z coordinate.</param>
        /// <returns></returns>
        public static Matrix3D Transform(double x, double y, double z)
        {
            var res = GetIdentity();//new Matrix3D( MATRIX_SIZE );

            res.mData[3][0] = x;
            res.mData[3][1] = y;
            res.mData[3][2] = z;

            return res;
        }

        /// <summary>
        /// Turns by the specified angle.
        /// </summary>
        /// <param name="angle">The angle.</param>
        /// <returns></returns>
        public static Matrix3D Turn(double angle)
        {
            var res = GetIdentity();

            res[0, 0] = Math.Cos(angle);
            res[2, 0] = -Math.Sin(angle);
            res[0, 2] = Math.Sin(angle);
            res[2, 2] = Math.Cos(angle);

            return res;
        }

        /// <summary>
        /// Tilts by the specified angle.
        /// </summary>
        /// <param name="angle">The angle.</param>
        /// <returns></returns>
        public static Matrix3D Tilt(double angle)
        {
            var res = GetIdentity();

            res[1, 1] = (Math.Cos(angle));
            res[2, 1] = (Math.Sin(angle));
            res[1, 2] = -(Math.Sin(angle));
            res[2, 2] = (Math.Cos(angle));

            return res;
        }

        /// <summary>
        /// Transposes the specified matrix.
        /// </summary>
        /// <param name="matrix3D">The matrix.</param>
        /// <returns></returns>
        public static Matrix3D Transposed(Matrix3D matrix3D)
        {
            var m = Identity;

            for (var i = 0; i < MATRIX_SIZE; i++)
            {
                for (var j = 0; j < MATRIX_SIZE; j++)
                {
                    m[i, j] = matrix3D[j, i];
                }
            }

            return m;
        }

        /// <summary>
        /// Shears the specified values.
        /// </summary>
        /// <param name="xy">The xy shear.</param>
        /// <param name="xz">The xz shear.</param>
        /// <param name="yx">The yx shear.</param>
        /// <param name="yz">The yz shear.</param>
        /// <param name="zx">The zx shear.</param>
        /// <param name="zy">The zy shear.</param>
        /// <returns></returns>
        public static Matrix3D Shear(double xy, double xz, double yx, double yz, double zx, double zy)
        {
            var res = Identity;

            res[1, 0] = xy;
            res[2, 0] = xz;
            res[0, 1] = yx;
            res[2, 1] = yz;
            res[0, 2] = zx;
            res[1, 2] = zy;

            return res;
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
            var m1 = (Matrix3D)obj;
            var res = true;

            for (var i = 0; i < m1.mData.Length; i++)
            {
                for (var j = 0; j < m1.mData.Length; j++)
                {
                    if (m1.mData[i][j] != mData[i][j])
                    {
                        res = false;
                    }
                }
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
            return mData.GetHashCode();
        }
        #endregion

        #region Helper methdos
        /// <summary>
        /// Calculates determinant row given matrix..
        /// </summary>
        /// <param name="dd">The matrix to calculate determinant.</param>
        /// <returns>Determinant of the given matrix.</returns>
        private static double GetDeterminant(IList<double[]> dd)
        {
            var count = dd.Count;
            double res = 0;

            if (count < 2)
            {
                res = dd[0][0];
            }
            else
            {
                var k = 1;

                for (var i = 0; i < count; i++)
                {
                    var dm = GetMMtr(dd, i, 0);

                    res += k * dd[i][0] * GetDeterminant(dm);
                    k = (k > 0) ? -1 : 1;
                }
            }

            return res;
        }

        /// <summary>
        /// Gets the minor.
        /// </summary>
        /// <param name="dd">The matrix.</param>
        /// <param name="columnIndex">The index of column.</param>
        /// <param name="rowIndex">The index of row.</param>
        /// <returns></returns>
        private static double[][] GetMMtr(IList<double[]> dd, int columnIndex, int rowIndex)
        {
            var count = dd.Count - 1;
            var d = new double[count][];

            for (var i = 0; i < count; i++)
            {
                var m = (i >= columnIndex) ? i + 1 : i;
                d[i] = new double[count];

                for (var j = 0; j < count; j++)
                {
                    var n = (j >= rowIndex) ? j + 1 : j;

                    d[i][j] = dd[m][n];
                }
            }

            return d;
        }
        #endregion
    }
}
