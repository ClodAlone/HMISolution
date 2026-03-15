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
    /// Represents the matrix 4x4.
    /// </summary>
    public struct Matrix3D
    {
        #region Constants
        private const int MATRIX_SIZE = 4;
        #endregion

        #region Members
        private double[][] m_data;
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
                return (m_data[0][3] == 0) && (m_data[1][3] == 0)
                  && (m_data[2][3] == 0) && (m_data[3][3] == 1);
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
                return m_data[i][j];
            }

            set
            {
                m_data[i][j] = value;
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
            m_data = new double[size][];

            for (int i = 0; i < size; i++)
            {
                m_data[i] = new double[size];
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
                        m_data[0][0] = m11;
                        m_data[1][0] = m12;
                        m_data[2][0] = m13;
                        m_data[3][0] = m14;

                        m_data[0][1] = m21;
                        m_data[1][1] = m22;
                        m_data[2][1] = m23;
                        m_data[3][1] = m24;

                        m_data[0][2] = m31;
                        m_data[1][2] = m32;
                        m_data[2][2] = m33;
                        m_data[3][2] = m34;

                        m_data[0][3] = m41;
                        m_data[1][3] = m42;
                        m_data[2][3] = m43;
                        m_data[3][3] = m44;
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
            Matrix3D m = new Matrix3D(4);

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
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
            double x = m1.m_data[0][0] * point.m_x + m1.m_data[1][0] * point.m_y + m1.m_data[2][0] * point.m_z + m1.m_data[3][0];
            double y = m1.m_data[0][1] * point.m_x + m1.m_data[1][1] * point.m_y + m1.m_data[2][1] * point.m_z + m1.m_data[3][1];
            double z = m1.m_data[0][2] * point.m_x + m1.m_data[1][2] * point.m_y + m1.m_data[2][2] * point.m_z + m1.m_data[3][2];

            if (!m1.IsAffine)
            {
                double c = 1d / (m1.m_data[0][3] * point.m_x + m1.m_data[1][3] * point.m_y + m1.m_data[2][3] * point.m_z + m1.m_data[3][3]);
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
            double x = m1.m_data[0][0] * v1.m_x + m1.m_data[1][0] * v1.m_y + m1.m_data[2][0] * v1.m_z;
            double y = m1.m_data[0][1] * v1.m_x + m1.m_data[1][1] * v1.m_y + m1.m_data[2][1] * v1.m_z;
            double z = m1.m_data[0][2] * v1.m_x + m1.m_data[1][2] * v1.m_y + m1.m_data[2][2] * v1.m_z;

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
            int length = m1.m_data.Length;
            Matrix3D res = new Matrix3D(length);

            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    res.m_data[i][j] = m1.m_data[i][j] * f1;
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
            Matrix3D res = Matrix3D.GetIdentity();

            for (int i = 0; i < MATRIX_SIZE; i++)
            {
                for (int j = 0; j < MATRIX_SIZE; j++)
                {
                    double v = 0;

                    for (int k = 0; k < MATRIX_SIZE; k++)
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
            bool res = true;

            for (int i = 0; i < m1.m_data.Length; i++)
            {
                for (int j = 0; j < m1.m_data.Length; j++)
                {
                    if (m1.m_data[i][j] != m2.m_data[i][j])
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
            bool res = true;

            for (int i = 0; i < m1.m_data.Length; i++)
            {
                for (int j = 0; j < m1.m_data.Length; j++)
                {
                    if (m1.m_data[i][j] != m2.m_data[i][j])
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
        public static Matrix3D GetInvertal(Matrix3D matrix3D)
        {
            Matrix3D m = Matrix3D.Identity;

            for (int i = 0; i < MATRIX_SIZE; i++)
            {
                for (int j = 0; j < MATRIX_SIZE; j++)
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
        public static double GetMinor(Matrix3D dd, int columnIndex, int rowIndex)
        {
            return (((columnIndex + rowIndex) % 2 == 0) ? 1 : -1) * GetDeterminant(GetMMtr(dd.m_data, columnIndex, rowIndex));
        }

        /// <summary>
        /// Gets the determinant.
        /// </summary>
        /// <param name="matrix3D">The matrix.</param>
        /// <returns></returns>
        public static double GetD(Matrix3D matrix3D)
        {
            return GetDeterminant(matrix3D.m_data);
        }

        /// <summary>
        /// Gets the identity matrix.
        /// </summary>
        /// <returns></returns>
        public static Matrix3D GetIdentity()
        {
            Matrix3D m = new Matrix3D(MATRIX_SIZE);

            for (int i = 0; i < MATRIX_SIZE; i++)
            {
                m[i, i] = 1.0f;
            }

            return m;
        }

        /// <summary>
        /// Gets the gauss result.
        /// </summary>
        /// <remarks>
        /// The columns of matrix is the A, B, C, D parameters of equations.
        /// </remarks>
        /// <param name="m1">The parameters.</param>
        /// <returns></returns>
        public static Vector3D GetGauss(Matrix3D m1)
        {
            Matrix3D m2 = m1;

            for (int i = 0; i < 3; i++)
            {
                for (int j = i; j < 3; j++)
                {
                    double a = m2[i, j];

                    for (int k = 0; (a != 0) && (k < 4); k++)
                    {
                        m2[k, j] *= (1 / a);
                    }
                }

                for (int j = i; j < 2; j++)
                {
                    for (int k = 0; (k < 4) && (m2[k, i] != 0); k++)
                    {
                        m2[k, j + 1] -= m2[k, i];
                    }
                }
            }

            double z = m2[3, 2] / m2[2, 2];
            double y = (m2[3, 1] - z * m2[2, 1]) / m2[1, 1];
            double x = (m2[3, 0] - z * m2[2, 0] - y * m2[1, 0]) / m1[0, 0];

            return new Vector3D(x, y, z);
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
            Matrix3D res = Matrix3D.GetIdentity();//new Matrix3D( MATRIX_SIZE );

            res.m_data[3][0] = x;
            res.m_data[3][1] = y;
            res.m_data[3][2] = z;

            return res;
        }

        /// <summary>
        /// Turns by the specified angle.
        /// </summary>
        /// <param name="angle">The angle.</param>
        /// <returns></returns>
        public static Matrix3D Turn(double angle)
        {
            Matrix3D res = Matrix3D.GetIdentity();

            res[0, 0] = (double)(Math.Cos(angle));
            res[2, 0] = -(double)(Math.Sin(angle));
            res[0, 2] = (double)(Math.Sin(angle));
            res[2, 2] = (double)(Math.Cos(angle));

            return res;
        }

        /// <summary>
        /// Tilts by the specified angle.
        /// </summary>
        /// <param name="angle">The angle.</param>
        /// <returns></returns>
        public static Matrix3D Tilt(double angle)
        {
            Matrix3D res = Matrix3D.GetIdentity();

            res[1, 1] = (Math.Cos(angle));
            res[2, 1] = (Math.Sin(angle));
            res[1, 2] = -(Math.Sin(angle));
            res[2, 2] = (Math.Cos(angle));

            return res;
        }

        /// <summary>
        /// Twists by the specified angle.
        /// </summary>
        /// <param name="angle">The angle.</param>
        /// <returns></returns>
        public static Matrix3D Twist(double angle)
        {
            Matrix3D res = Matrix3D.GetIdentity();

            res[0, 0] = (Math.Cos(angle));
            res[1, 0] = (Math.Sin(angle));
            res[0, 1] = -(Math.Sin(angle));
            res[1, 1] = (Math.Cos(angle));

            return res;
        }

        /// <summary>
        /// Scales by  the specified values.
        /// </summary>
        /// <param name="dx">The X scale.</param>
        /// <param name="dy">The Y scale.</param>
        /// <param name="dz">The Z scale.</param>
        /// <returns></returns>
        public static Matrix3D Scale(double dx, double dy, double dz)
        {
            Matrix3D res = Matrix3D.GetIdentity();

            res[0, 0] = dx;
            res[1, 1] = dy;
            res[2, 2] = dz;

            return res;
        }

        /// <summary>
        /// Transposes the specified matrix.
        /// </summary>
        /// <param name="matrix3D">The matrix.</param>
        /// <returns></returns>
        public static Matrix3D Transposed(Matrix3D matrix3D)
        {
            Matrix3D m = Matrix3D.Identity;

            for (int i = 0; i < MATRIX_SIZE; i++)
            {
                for (int j = 0; j < MATRIX_SIZE; j++)
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
            Matrix3D res = Matrix3D.Identity;

            res[1, 0] = xy;
            res[2, 0] = xz;
            res[0, 1] = yx;
            res[2, 1] = yz;
            res[0, 2] = zx;
            res[1, 2] = zy;

            return res;
        }

        /// <summary>
        /// Creates transformation matrix that rotates polygon around OX axis.
        /// </summary>
        /// <param name="angle">The angle to rotate.</param>
        /// <returns>Transformation matrix.</returns>
        public static Matrix3D RotateAlongOX(double angle)
        {
            Matrix3D res = Matrix3D.Identity;

            res[1, 1] = Math.Cos((double)angle);
            res[1, 2] = -Math.Sin((double)angle);
            res[2, 1] = Math.Sin((double)angle);
            res[2, 2] = Math.Cos((double)angle);

            return res;
        }

        /// <summary>
        /// Creates transformation matrix that rotates polygon around OY axis.
        /// </summary>
        /// <param name="angle">The angle to rotate.</param>
        /// <returns>Transformation matrix.</returns>
        public static Matrix3D RotateAlongOY(double angle)
        {
            Matrix3D res = Matrix3D.Identity;

            res[0, 0] = Math.Cos((double)angle); res[0, 2] = -Math.Sin((double)angle);
            res[2, 0] = Math.Sin((double)angle); res[2, 2] = Math.Cos((double)angle);

            return res;
        }

        /// <summary>
        /// Creates transformation matrix that rotates polygon around OZ axis.
        /// </summary>
        /// <param name="angle">The angle to rotate.</param>
        /// <returns>Transformation matrix.</returns>
        public static Matrix3D RotateAlongOZ(double angle)
        {
            Matrix3D res = Matrix3D.Identity;

            res[0, 0] = Math.Cos((double)angle); res[0, 1] = -Math.Sin((double)angle);
            res[1, 0] = Math.Sin((double)angle); res[1, 1] = Math.Cos((double)angle);

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
            Matrix3D m1 = (Matrix3D)obj;
            bool res = true;

            for (int i = 0; i < m1.m_data.Length; i++)
            {
                for (int j = 0; j < m1.m_data.Length; j++)
                {
                    if (m1.m_data[i][j] != m_data[i][j])
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
            return m_data.GetHashCode();
        }
        #endregion

        #region Helper methdos
        /// <summary>
        /// Calculates determinant row given matrix..
        /// </summary>
        /// <param name="dd">The matrix to calculate determinant.</param>
        /// <returns>Determinant of the given matrix.</returns>
        private static double GetDeterminant(double[][] dd)
        {
            int count = dd.Length;
            double res = 0;

            if (count < 2)
            {
                res = dd[0][0];
            }
            else
            {
                int k = 1;

                for (int i = 0; i < count; i++)
                {
                    double[][] dm = GetMMtr(dd, i, 0);

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
        private static double[][] GetMMtr(double[][] dd, int columnIndex, int rowIndex)
        {
            int count = dd.Length - 1;
            double[][] d = new double[count][];
            int m, n;

            for (int i = 0; i < count; i++)
            {
                m = (i >= columnIndex) ? i + 1 : i;
                d[i] = new double[count];

                for (int j = 0; j < count; j++)
                {
                    n = (j >= rowIndex) ? j + 1 : j;

                    d[i][j] = dd[m][n];
                }
            }

            return d;
        }
        #endregion
    }
}
