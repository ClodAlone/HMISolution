#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Diagnostics;
using System.Windows;

#if WINRT_USING
using Windows.Foundation; 
#endif

namespace Syncfusion.UI.Xaml.Diagram.Utility
{
    //public class MatrixExt
    //{
    //    internal enum MatrixTypes
    //    {
    //        TRANSFORM_IS_IDENTITY = 0,
    //        TRANSFORM_IS_TRANSLATION = 1,
    //        TRANSFORM_IS_SCALING = 2,
    //        TRANSFORM_IS_UNKNOWN = 4
    //    }

    //    internal class MatrixUtil
    //    {
    //        internal static void TransformRect(Rect rect, MatrixExt matrix)
    //        {
    //            if (rect.IsEmpty)
    //            {
    //                return;
    //            }
    //            MatrixTypes type = matrix._type;
    //            if (type == MatrixTypes.TRANSFORM_IS_IDENTITY)
    //            {
    //                return;
    //            }
    //            if ((type & MatrixTypes.TRANSFORM_IS_SCALING) != MatrixTypes.TRANSFORM_IS_IDENTITY)
    //            {
    //                rect.X *= matrix._m11;
    //                rect.Y *= matrix._m22;
    //                rect.Width *= matrix._m11;
    //                rect.Height *= matrix._m22;
    //                if (rect.Width < 0.0)
    //                {
    //                    rect.X += rect.Width;
    //                    rect.Width = -rect.Width;
    //                }
    //                if (rect.Height < 0.0)
    //                {
    //                    rect.Y += rect.Height;
    //                    rect.Height = -rect.Height;
    //                }
    //            }
    //            if ((type & MatrixTypes.TRANSFORM_IS_TRANSLATION) != MatrixTypes.TRANSFORM_IS_IDENTITY)
    //            {
    //                rect.X += matrix._offsetX;
    //                rect.Y += matrix._offsetY;
    //            }
    //            if (type == MatrixTypes.TRANSFORM_IS_UNKNOWN)
    //            {
    //                Point point = matrix.Transform(new Point(rect.Left, rect.Top));
    //                Point point2 = matrix.Transform(new Point(rect.Right, rect.Top));
    //                Point point3 = matrix.Transform(new Point(rect.Right, rect.Bottom));
    //                Point point4 = matrix.Transform(new Point(rect.Left, rect.Bottom));
    //                rect.X = Math.Min(Math.Min(point.X, point2.X), Math.Min(point3.X, point4.X));
    //                rect.Y = Math.Min(Math.Min(point.Y, point2.Y), Math.Min(point3.Y, point4.Y));
    //                rect.Width = Math.Max(Math.Max(point.X, point2.X), Math.Max(point3.X, point4.X)) - rect.X;
    //                rect.Height = Math.Max(Math.Max(point.Y, point2.Y), Math.Max(point3.Y, point4.Y)) - rect.Y;
    //            }
    //        }
    //        internal static void MultiplyMatrix(MatrixExt matrix1, MatrixExt matrix2)
    //        {
    //            MatrixTypes type = matrix1._type;
    //            MatrixTypes type2 = matrix2._type;
    //            if (type2 == MatrixTypes.TRANSFORM_IS_IDENTITY)
    //            {
    //                return;
    //            }
    //            if (type == MatrixTypes.TRANSFORM_IS_IDENTITY)
    //            {
    //                matrix1 = matrix2;
    //                return;
    //            }
    //            if (type2 == MatrixTypes.TRANSFORM_IS_TRANSLATION)
    //            {
    //                matrix1._offsetX += matrix2._offsetX;
    //                matrix1._offsetY += matrix2._offsetY;
    //                if (type != MatrixTypes.TRANSFORM_IS_UNKNOWN)
    //                {
    //                    matrix1._type |= MatrixTypes.TRANSFORM_IS_TRANSLATION;
    //                }
    //                return;
    //            }
    //            if (type != MatrixTypes.TRANSFORM_IS_TRANSLATION)
    //            {
    //                int num = (int)((int)type << 4 | (int)type2);
    //                int num2 = num;
    //                switch (num2)
    //                {
    //                    case 34:
    //                        {
    //                            matrix1._m11 *= matrix2._m11;
    //                            matrix1._m22 *= matrix2._m22;
    //                            return;
    //                        }
    //                    case 35:
    //                        {
    //                            matrix1._m11 *= matrix2._m11;
    //                            matrix1._m22 *= matrix2._m22;
    //                            matrix1._offsetX = matrix2._offsetX;
    //                            matrix1._offsetY = matrix2._offsetY;
    //                            matrix1._type = (MatrixTypes.TRANSFORM_IS_TRANSLATION | MatrixTypes.TRANSFORM_IS_SCALING);
    //                            return;
    //                        }
    //                    case 36:
    //                        {
    //                            break;
    //                        }
    //                    default:
    //                        {
    //                            switch (num2)
    //                            {
    //                                case 50:
    //                                    {
    //                                        matrix1._m11 *= matrix2._m11;
    //                                        matrix1._m22 *= matrix2._m22;
    //                                        matrix1._offsetX *= matrix2._m11;
    //                                        matrix1._offsetY *= matrix2._m22;
    //                                        return;
    //                                    }
    //                                case 51:
    //                                    {
    //                                        matrix1._m11 *= matrix2._m11;
    //                                        matrix1._m22 *= matrix2._m22;
    //                                        matrix1._offsetX = matrix2._m11 * matrix1._offsetX + matrix2._offsetX;
    //                                        matrix1._offsetY = matrix2._m22 * matrix1._offsetY + matrix2._offsetY;
    //                                        return;
    //                                    }
    //                                case 52:
    //                                    {
    //                                        break;
    //                                    }
    //                                default:
    //                                    {
    //                                        switch (num2)
    //                                        {
    //                                            case 66:
    //                                            case 67:
    //                                            case 68:
    //                                                {
    //                                                    break;
    //                                                }
    //                                            default:
    //                                                {
    //                                                    return;
    //                                                }
    //                                        }
    //                                        break;
    //                                    }
    //                            }
    //                            break;
    //                        }
    //                }
    //                matrix1 = new MatrixExt(matrix1._m11 * matrix2._m11 + matrix1._m12 * matrix2._m21, matrix1._m11 * matrix2._m12 + matrix1._m12 * matrix2._m22, matrix1._m21 * matrix2._m11 + matrix1._m22 * matrix2._m21, matrix1._m21 * matrix2._m12 + matrix1._m22 * matrix2._m22, matrix1._offsetX * matrix2._m11 + matrix1._offsetY * matrix2._m21 + matrix2._offsetX, matrix1._offsetX * matrix2._m12 + matrix1._offsetY * matrix2._m22 + matrix2._offsetY);
    //                return;
    //            }
    //            double offsetX = matrix1._offsetX;
    //            double offsetY = matrix1._offsetY;
    //            matrix1 = matrix2;
    //            matrix1._offsetX = offsetX * matrix2._m11 + offsetY * matrix2._m21 + matrix2._offsetX;
    //            matrix1._offsetY = offsetX * matrix2._m12 + offsetY * matrix2._m22 + matrix2._offsetY;
    //            if (type2 == MatrixTypes.TRANSFORM_IS_UNKNOWN)
    //            {
    //                matrix1._type = MatrixTypes.TRANSFORM_IS_UNKNOWN;
    //                return;
    //            }
    //            matrix1._type = (MatrixTypes.TRANSFORM_IS_TRANSLATION | MatrixTypes.TRANSFORM_IS_SCALING);
    //        }
    //        internal static void PrependOffset(MatrixExt matrix, double offsetX, double offsetY)
    //        {
    //            if (matrix._type == MatrixTypes.TRANSFORM_IS_IDENTITY)
    //            {
    //                matrix = new MatrixExt(1.0, 0.0, 0.0, 1.0, offsetX, offsetY);
    //                matrix._type = MatrixTypes.TRANSFORM_IS_TRANSLATION;
    //                return;
    //            }
    //            matrix._offsetX += matrix._m11 * offsetX + matrix._m21 * offsetY;
    //            matrix._offsetY += matrix._m12 * offsetX + matrix._m22 * offsetY;
    //            if (matrix._type != MatrixTypes.TRANSFORM_IS_UNKNOWN)
    //            {
    //                matrix._type |= MatrixTypes.TRANSFORM_IS_TRANSLATION;
    //            }
    //        }
    //    }

    //    private const int c_identityHashCode = 0;
    //    private static MatrixExt s_identity = MatrixExt.CreateIdentity();
    //    internal double _m11;
    //    internal double _m12;
    //    internal double _m21;
    //    internal double _m22;
    //    internal double _offsetX;
    //    internal double _offsetY;
    //    internal MatrixTypes _type;
    //    internal int _padding;
    //    public static MatrixExt Identity
    //    {
    //        get
    //        {
    //            return MatrixExt.s_identity;
    //        }
    //    }
    //    public bool IsIdentity
    //    {
    //        get
    //        {
    //            return this._type == MatrixTypes.TRANSFORM_IS_IDENTITY || (this._m11 == 1.0 && this._m12 == 0.0 && this._m21 == 0.0 && this._m22 == 1.0 && this._offsetX == 0.0 && this._offsetY == 0.0);
    //        }
    //    }
    //    public double Determinant
    //    {
    //        get
    //        {
    //            switch (this._type)
    //            {
    //                case MatrixTypes.TRANSFORM_IS_IDENTITY:
    //                case MatrixTypes.TRANSFORM_IS_TRANSLATION:
    //                    {
    //                        return 1.0;
    //                    }
    //                case MatrixTypes.TRANSFORM_IS_SCALING:
    //                case MatrixTypes.TRANSFORM_IS_TRANSLATION | MatrixTypes.TRANSFORM_IS_SCALING:
    //                    {
    //                        return this._m11 * this._m22;
    //                    }
    //                default:
    //                    {
    //                        return this._m11 * this._m22 - this._m12 * this._m21;
    //                    }
    //            }
    //        }
    //    }
    //    public bool HasInverse
    //    {
    //        get
    //        {
    //            return !(Math.Abs(this.Determinant) < 2.2204460492503131E-15);
    //        }
    //    }
    //    public double M11
    //    {
    //        get
    //        {
    //            if (this._type == MatrixTypes.TRANSFORM_IS_IDENTITY)
    //            {
    //                return 1.0;
    //            }
    //            return this._m11;
    //        }
    //        set
    //        {
    //            if (this._type == MatrixTypes.TRANSFORM_IS_IDENTITY)
    //            {
    //                this.SetMatrix(value, 0.0, 0.0, 1.0, 0.0, 0.0, MatrixTypes.TRANSFORM_IS_SCALING);
    //                return;
    //            }
    //            this._m11 = value;
    //            if (this._type != MatrixTypes.TRANSFORM_IS_UNKNOWN)
    //            {
    //                this._type |= MatrixTypes.TRANSFORM_IS_SCALING;
    //            }
    //        }
    //    }
    //    public double M12
    //    {
    //        get
    //        {
    //            if (this._type == MatrixTypes.TRANSFORM_IS_IDENTITY)
    //            {
    //                return 0.0;
    //            }
    //            return this._m12;
    //        }
    //        set
    //        {
    //            if (this._type == MatrixTypes.TRANSFORM_IS_IDENTITY)
    //            {
    //                this.SetMatrix(1.0, value, 0.0, 1.0, 0.0, 0.0, MatrixTypes.TRANSFORM_IS_UNKNOWN);
    //                return;
    //            }
    //            this._m12 = value;
    //            this._type = MatrixTypes.TRANSFORM_IS_UNKNOWN;
    //        }
    //    }
    //    public double M21
    //    {
    //        get
    //        {
    //            if (this._type == MatrixTypes.TRANSFORM_IS_IDENTITY)
    //            {
    //                return 0.0;
    //            }
    //            return this._m21;
    //        }
    //        set
    //        {
    //            if (this._type == MatrixTypes.TRANSFORM_IS_IDENTITY)
    //            {
    //                this.SetMatrix(1.0, 0.0, value, 1.0, 0.0, 0.0, MatrixTypes.TRANSFORM_IS_UNKNOWN);
    //                return;
    //            }
    //            this._m21 = value;
    //            this._type = MatrixTypes.TRANSFORM_IS_UNKNOWN;
    //        }
    //    }
    //    public double M22
    //    {
    //        get
    //        {
    //            if (this._type == MatrixTypes.TRANSFORM_IS_IDENTITY)
    //            {
    //                return 1.0;
    //            }
    //            return this._m22;
    //        }
    //        set
    //        {
    //            if (this._type == MatrixTypes.TRANSFORM_IS_IDENTITY)
    //            {
    //                this.SetMatrix(1.0, 0.0, 0.0, value, 0.0, 0.0, MatrixTypes.TRANSFORM_IS_SCALING);
    //                return;
    //            }
    //            this._m22 = value;
    //            if (this._type != MatrixTypes.TRANSFORM_IS_UNKNOWN)
    //            {
    //                this._type |= MatrixTypes.TRANSFORM_IS_SCALING;
    //            }
    //        }
    //    }
    //    public double OffsetX
    //    {
    //        get
    //        {
    //            if (this._type == MatrixTypes.TRANSFORM_IS_IDENTITY)
    //            {
    //                return 0.0;
    //            }
    //            return this._offsetX;
    //        }
    //        set
    //        {
    //            if (this._type == MatrixTypes.TRANSFORM_IS_IDENTITY)
    //            {
    //                this.SetMatrix(1.0, 0.0, 0.0, 1.0, value, 0.0, MatrixTypes.TRANSFORM_IS_TRANSLATION);
    //                return;
    //            }
    //            this._offsetX = value;
    //            if (this._type != MatrixTypes.TRANSFORM_IS_UNKNOWN)
    //            {
    //                this._type |= MatrixTypes.TRANSFORM_IS_TRANSLATION;
    //            }
    //        }
    //    }
    //    public double OffsetY
    //    {
    //        get
    //        {
    //            if (this._type == MatrixTypes.TRANSFORM_IS_IDENTITY)
    //            {
    //                return 0.0;
    //            }
    //            return this._offsetY;
    //        }
    //        set
    //        {
    //            if (this._type == MatrixTypes.TRANSFORM_IS_IDENTITY)
    //            {
    //                this.SetMatrix(1.0, 0.0, 0.0, 1.0, 0.0, value, MatrixTypes.TRANSFORM_IS_TRANSLATION);
    //                return;
    //            }
    //            this._offsetY = value;
    //            if (this._type != MatrixTypes.TRANSFORM_IS_UNKNOWN)
    //            {
    //                this._type |= MatrixTypes.TRANSFORM_IS_TRANSLATION;
    //            }
    //        }
    //    }
    //    private bool IsDistinguishedIdentity
    //    {
    //        get
    //        {
    //            return this._type == MatrixTypes.TRANSFORM_IS_IDENTITY;
    //        }
    //    }
    //    public MatrixExt(double m11, double m12, double m21, double m22, double offsetX, double offsetY)
    //    {
    //        this._m11 = m11;
    //        this._m12 = m12;
    //        this._m21 = m21;
    //        this._m22 = m22;
    //        this._offsetX = offsetX;
    //        this._offsetY = offsetY;
    //        this._type = MatrixTypes.TRANSFORM_IS_UNKNOWN;
    //        this._padding = 0;
    //        this.DeriveMatrixType();
    //    }

    //    public MatrixExt()
    //    {
    //        // TODO: Complete member initialization
    //    }
    //    public void SetIdentity()
    //    {
    //        this._type = MatrixTypes.TRANSFORM_IS_IDENTITY;
    //    }
    //    public void Multiply(MatrixExt trans2)
    //    {
    //        MatrixUtil.MultiplyMatrix(this, trans2);
    //        //return trans1;
    //    }

    //    public void Append(MatrixExt matrix)
    //    {
    //        this.Multiply(matrix);
    //    }
    //    //public void Prepend(Matrix matrix)
    //    //{
    //    //    this = matrix * this;
    //    //}
    //    public void Rotate(double angle)
    //    {
    //        angle %= 360.0;
    //        this.Multiply(MatrixExt.CreateRotationRadians(angle * 0.017453292519943295));
    //    }
    //    //public void RotatePrepend(double angle)
    //    //{
    //    //    angle %= 360.0;
    //    //    this = Matrix.CreateRotationRadians(angle * 0.017453292519943295) * this;
    //    //}
    //    public void RotateAt(double angle, double centerX, double centerY)
    //    {
    //        angle %= 360.0;
    //        this.Multiply(MatrixExt.CreateRotationRadians(angle * 0.017453292519943295, centerX, centerY));
    //    }
    //    //public void RotateAtPrepend(double angle, double centerX, double centerY)
    //    //{
    //    //    angle %= 360.0;
    //    //    this = Matrix.CreateRotationRadians(angle * 0.017453292519943295, centerX, centerY) * this;
    //    //}
    //    public void Scale(double scaleX, double scaleY)
    //    {
    //        this.Multiply(MatrixExt.CreateScaling(scaleX, scaleY));
    //    }
    //    //public void ScalePrepend(double scaleX, double scaleY)
    //    //{
    //    //    this = Matrix.CreateScaling(scaleX, scaleY) * this;
    //    //}
    //    public void ScaleAt(double scaleX, double scaleY, double centerX, double centerY)
    //    {
    //        this.Multiply(MatrixExt.CreateScaling(scaleX, scaleY, centerX, centerY));
    //    }
    //    //public void ScaleAtPrepend(double scaleX, double scaleY, double centerX, double centerY)
    //    //{
    //    //    this = Matrix.CreateScaling(scaleX, scaleY, centerX, centerY) * this;
    //    //}
    //    public void Skew(double skewX, double skewY)
    //    {
    //        skewX %= 360.0;
    //        skewY %= 360.0;
    //        this.Multiply(MatrixExt.CreateSkewRadians(skewX * 0.017453292519943295, skewY * 0.017453292519943295));
    //    }
    //    //public void SkewPrepend(double skewX, double skewY)
    //    //{
    //    //    skewX %= 360.0;
    //    //    skewY %= 360.0;
    //    //    this = Matrix.CreateSkewRadians(skewX * 0.017453292519943295, skewY * 0.017453292519943295) * this;
    //    //}
    //    public void Translate(double offsetX, double offsetY)
    //    {
    //        if (this._type == MatrixTypes.TRANSFORM_IS_IDENTITY)
    //        {
    //            this.SetMatrix(1.0, 0.0, 0.0, 1.0, offsetX, offsetY, MatrixTypes.TRANSFORM_IS_TRANSLATION);
    //            return;
    //        }
    //        if (this._type == MatrixTypes.TRANSFORM_IS_UNKNOWN)
    //        {
    //            this._offsetX += offsetX;
    //            this._offsetY += offsetY;
    //            return;
    //        }
    //        this._offsetX += offsetX;
    //        this._offsetY += offsetY;
    //        this._type |= MatrixTypes.TRANSFORM_IS_TRANSLATION;
    //    }
    //    //public void TranslatePrepend(double offsetX, double offsetY)
    //    //{
    //    //    this = Matrix.CreateTranslation(offsetX, offsetY) * this;
    //    //}
    //    public Point Transform(Point point)
    //    {
    //        double x, y;
    //        x = point.X;
    //        y = point.Y;
    //        this.MultiplyPoint(ref x, ref y);
    //        return new Point(x, y);
    //    }
    //    public void Transform(Point[] points)
    //    {
    //        if (points != null)
    //        {
    //            for (int i = 0; i < points.Length; i++)
    //            {
    //                double x, y;
    //                x = points[i].X;
    //                y = points[i].Y;
    //                this.MultiplyPoint(ref x, ref y);
    //                points[i] = new Point(x, y);
    //            }
    //        }
    //    }
    //    //public Vector Transform(Vector vector)
    //    //{
    //    //    Vector result = vector;
    //    //    this.MultiplyVector(ref result._x, ref result._y);
    //    //    return result;
    //    //}
    //    //public void Transform(Vector[] vectors)
    //    //{
    //    //    if (vectors != null)
    //    //    {
    //    //        for (int i = 0; i < vectors.Length; i++)
    //    //        {
    //    //            this.MultiplyVector(ref vectors[i]._x, ref vectors[i]._y);
    //    //        }
    //    //    }
    //    //}
    //    public void Invert()
    //    {
    //        double determinant = this.Determinant;
    //        if (Math.Abs(determinant) < 2.2204460492503131E-15)
    //        {
    //            throw new InvalidOperationException("Transform_NotInvertible");
    //        }
    //        switch (this._type)
    //        {
    //            case MatrixTypes.TRANSFORM_IS_IDENTITY:
    //                {
    //                    break;
    //                }
    //            case MatrixTypes.TRANSFORM_IS_TRANSLATION:
    //                {
    //                    this._offsetX = -this._offsetX;
    //                    this._offsetY = -this._offsetY;
    //                    return;
    //                }
    //            case MatrixTypes.TRANSFORM_IS_SCALING:
    //                {
    //                    this._m11 = 1.0 / this._m11;
    //                    this._m22 = 1.0 / this._m22;
    //                    return;
    //                }
    //            case MatrixTypes.TRANSFORM_IS_TRANSLATION | MatrixTypes.TRANSFORM_IS_SCALING:
    //                {
    //                    this._m11 = 1.0 / this._m11;
    //                    this._m22 = 1.0 / this._m22;
    //                    this._offsetX = -this._offsetX * this._m11;
    //                    this._offsetY = -this._offsetY * this._m22;
    //                    return;
    //                }
    //            default:
    //                {
    //                    double num = 1.0 / determinant;
    //                    this.SetMatrix(this._m22 * num, -this._m12 * num, -this._m21 * num, this._m11 * num, (this._m21 * this._offsetY - this._offsetX * this._m22) * num, (this._offsetX * this._m12 - this._m11 * this._offsetY) * num, MatrixTypes.TRANSFORM_IS_UNKNOWN);
    //                    break;
    //                }
    //        }
    //    }
    //    internal void MultiplyVector(ref double x, ref double y)
    //    {
    //        switch (this._type)
    //        {
    //            case MatrixTypes.TRANSFORM_IS_IDENTITY:
    //            case MatrixTypes.TRANSFORM_IS_TRANSLATION:
    //                {
    //                    return;
    //                }
    //            case MatrixTypes.TRANSFORM_IS_SCALING:
    //            case MatrixTypes.TRANSFORM_IS_TRANSLATION | MatrixTypes.TRANSFORM_IS_SCALING:
    //                {
    //                    x *= this._m11;
    //                    y *= this._m22;
    //                    return;
    //                }
    //            default:
    //                {
    //                    double num = y * this._m21;
    //                    double num2 = x * this._m12;
    //                    x *= this._m11;
    //                    x += num;
    //                    y *= this._m22;
    //                    y += num2;
    //                    return;
    //                }
    //        }
    //    }
    //    internal void MultiplyPoint(ref double x, ref double y)
    //    {
    //        switch (this._type)
    //        {
    //            case MatrixTypes.TRANSFORM_IS_IDENTITY:
    //                {
    //                    return;
    //                }
    //            case MatrixTypes.TRANSFORM_IS_TRANSLATION:
    //                {
    //                    x += this._offsetX;
    //                    y += this._offsetY;
    //                    return;
    //                }
    //            case MatrixTypes.TRANSFORM_IS_SCALING:
    //                {
    //                    x *= this._m11;
    //                    y *= this._m22;
    //                    return;
    //                }
    //            case MatrixTypes.TRANSFORM_IS_TRANSLATION | MatrixTypes.TRANSFORM_IS_SCALING:
    //                {
    //                    x *= this._m11;
    //                    x += this._offsetX;
    //                    y *= this._m22;
    //                    y += this._offsetY;
    //                    return;
    //                }
    //            default:
    //                {
    //                    double num = y * this._m21 + this._offsetX;
    //                    double num2 = x * this._m12 + this._offsetY;
    //                    x *= this._m11;
    //                    x += num;
    //                    y *= this._m22;
    //                    y += num2;
    //                    return;
    //                }
    //        }
    //    }
    //    internal static MatrixExt CreateRotationRadians(double angle)
    //    {
    //        return MatrixExt.CreateRotationRadians(angle, 0.0, 0.0);
    //    }
    //    internal static MatrixExt CreateRotationRadians(double angle, double centerX, double centerY)
    //    {
    //        MatrixExt result = new MatrixExt();
    //        double num = Math.Sin(angle);
    //        double num2 = Math.Cos(angle);
    //        double offsetX = centerX * (1.0 - num2) + centerY * num;
    //        double offsetY = centerY * (1.0 - num2) - centerX * num;
    //        result.SetMatrix(num2, num, -num, num2, offsetX, offsetY, MatrixTypes.TRANSFORM_IS_UNKNOWN);
    //        return result;
    //    }
    //    internal static MatrixExt CreateScaling(double scaleX, double scaleY, double centerX, double centerY)
    //    {
    //        MatrixExt result = new MatrixExt();
    //        result.SetMatrix(scaleX, 0.0, 0.0, scaleY, centerX - scaleX * centerX, centerY - scaleY * centerY, MatrixTypes.TRANSFORM_IS_TRANSLATION | MatrixTypes.TRANSFORM_IS_SCALING);
    //        return result;
    //    }
    //    internal static MatrixExt CreateScaling(double scaleX, double scaleY)
    //    {
    //        MatrixExt result = new MatrixExt();
    //        result.SetMatrix(scaleX, 0.0, 0.0, scaleY, 0.0, 0.0, MatrixTypes.TRANSFORM_IS_SCALING);
    //        return result;
    //    }
    //    internal static MatrixExt CreateSkewRadians(double skewX, double skewY)
    //    {
    //        MatrixExt result = new MatrixExt();
    //        result.SetMatrix(1.0, Math.Tan(skewY), Math.Tan(skewX), 1.0, 0.0, 0.0, MatrixTypes.TRANSFORM_IS_UNKNOWN);
    //        return result;
    //    }
    //    internal static MatrixExt CreateTranslation(double offsetX, double offsetY)
    //    {
    //        MatrixExt result = new MatrixExt();
    //        result.SetMatrix(1.0, 0.0, 0.0, 1.0, offsetX, offsetY, MatrixTypes.TRANSFORM_IS_TRANSLATION);
    //        return result;
    //    }
    //    private static MatrixExt CreateIdentity()
    //    {
    //        MatrixExt result = new MatrixExt();
    //        result.SetMatrix(1.0, 0.0, 0.0, 1.0, 0.0, 0.0, MatrixTypes.TRANSFORM_IS_IDENTITY);
    //        return result;
    //    }
    //    private void SetMatrix(double m11, double m12, double m21, double m22, double offsetX, double offsetY, MatrixTypes type)
    //    {
    //        this._m11 = m11;
    //        this._m12 = m12;
    //        this._m21 = m21;
    //        this._m22 = m22;
    //        this._offsetX = offsetX;
    //        this._offsetY = offsetY;
    //        this._type = type;
    //    }
    //    private void DeriveMatrixType()
    //    {
    //        this._type = MatrixTypes.TRANSFORM_IS_IDENTITY;
    //        if (this._m21 != 0.0 || this._m12 != 0.0)
    //        {
    //            this._type = MatrixTypes.TRANSFORM_IS_UNKNOWN;
    //            return;
    //        }
    //        if (this._m11 != 1.0 || this._m22 != 1.0)
    //        {
    //            this._type = MatrixTypes.TRANSFORM_IS_SCALING;
    //        }
    //        if (this._offsetX != 0.0 || this._offsetY != 0.0)
    //        {
    //            this._type |= MatrixTypes.TRANSFORM_IS_TRANSLATION;
    //        }
    //        if ((this._type & (MatrixTypes.TRANSFORM_IS_TRANSLATION | MatrixTypes.TRANSFORM_IS_SCALING)) == MatrixTypes.TRANSFORM_IS_IDENTITY)
    //        {
    //            this._type = MatrixTypes.TRANSFORM_IS_IDENTITY;
    //        }
    //    }
    //    [Conditional("DEBUG")]
    //    private void Debug_CheckType()
    //    {
    //        switch (this._type)
    //        {
    //            case MatrixTypes.TRANSFORM_IS_IDENTITY:
    //            case MatrixTypes.TRANSFORM_IS_TRANSLATION:
    //            case MatrixTypes.TRANSFORM_IS_SCALING:
    //            case MatrixTypes.TRANSFORM_IS_TRANSLATION | MatrixTypes.TRANSFORM_IS_SCALING:
    //            case MatrixTypes.TRANSFORM_IS_UNKNOWN:
    //                {
    //                    return;
    //                }
    //        }
    //    }
    //}

	internal struct MatrixExt
    {
        internal enum MatrixTypes
        {
            TRANSFORM_IS_IDENTITY = 0,
            TRANSFORM_IS_TRANSLATION = 1,
            TRANSFORM_IS_SCALING = 2,
            TRANSFORM_IS_UNKNOWN = 4
        }

        internal static class MatrixUtil
        {
            internal static void TransformRect(ref Rect rect, ref MatrixExt matrix)
            {
                if (rect.IsEmpty)
                {
                    return;
                }
                MatrixTypes type = matrix._type;
                if (type == MatrixTypes.TRANSFORM_IS_IDENTITY)
                {
                    return;
                }
                if ((type & MatrixTypes.TRANSFORM_IS_SCALING) != MatrixTypes.TRANSFORM_IS_IDENTITY)
                {
                    rect.X *= matrix._m11;
                    rect.Y *= matrix._m22;
                    rect.Width *= matrix._m11;
                    rect.Height *= matrix._m22;
                    if (rect.Width < 0.0)
                    {
                        rect.X += rect.Width;
                        rect.Width = -rect.Width;
                    }
                    if (rect.Height < 0.0)
                    {
                        rect.Y += rect.Height;
                        rect.Height = -rect.Height;
                    }
                }
                if ((type & MatrixTypes.TRANSFORM_IS_TRANSLATION) != MatrixTypes.TRANSFORM_IS_IDENTITY)
                {
                    rect.X += matrix._offsetX;
                    rect.Y += matrix._offsetY;
                }
                if (type == MatrixTypes.TRANSFORM_IS_UNKNOWN)
                {
                    Point point = matrix.Transform(new Point(rect.Left, rect.Top));
                    Point point2 = matrix.Transform(new Point(rect.Right, rect.Top));
                    Point point3 = matrix.Transform(new Point(rect.Right, rect.Bottom));
                    Point point4 = matrix.Transform(new Point(rect.Left, rect.Bottom));
                    rect.X = Math.Min(Math.Min(point.X, point2.X), Math.Min(point3.X, point4.X));
                    rect.Y = Math.Min(Math.Min(point.Y, point2.Y), Math.Min(point3.Y, point4.Y));
                    rect.Width = Math.Max(Math.Max(point.X, point2.X), Math.Max(point3.X, point4.X)) - rect.X;
                    rect.Height = Math.Max(Math.Max(point.Y, point2.Y), Math.Max(point3.Y, point4.Y)) - rect.Y;
                }
            }
            internal static void MultiplyMatrix(ref MatrixExt matrix1, ref MatrixExt matrix2)
            {
                MatrixTypes type = matrix1._type;
                MatrixTypes type2 = matrix2._type;
                if (type2 == MatrixTypes.TRANSFORM_IS_IDENTITY)
                {
                    return;
                }
                if (type == MatrixTypes.TRANSFORM_IS_IDENTITY)
                {
                    matrix1 = matrix2;
                    return;
                }
                if (type2 == MatrixTypes.TRANSFORM_IS_TRANSLATION)
                {
                    matrix1._offsetX += matrix2._offsetX;
                    matrix1._offsetY += matrix2._offsetY;
                    if (type != MatrixTypes.TRANSFORM_IS_UNKNOWN)
                    {
                        matrix1._type |= MatrixTypes.TRANSFORM_IS_TRANSLATION;
                    }
                    return;
                }
                if (type != MatrixTypes.TRANSFORM_IS_TRANSLATION)
                {
                    int num = (int)type << 4 | (int)type2;
                    int num2 = num;
                    switch (num2)
                    {
                        case 34:
                            {
                                matrix1._m11 *= matrix2._m11;
                                matrix1._m22 *= matrix2._m22;
                                return;
                            }
                        case 35:
                            {
                                matrix1._m11 *= matrix2._m11;
                                matrix1._m22 *= matrix2._m22;
                                matrix1._offsetX = matrix2._offsetX;
                                matrix1._offsetY = matrix2._offsetY;
                                matrix1._type = (MatrixTypes.TRANSFORM_IS_TRANSLATION | MatrixTypes.TRANSFORM_IS_SCALING);
                                return;
                            }
                        case 36:
                            {
                                break;
                            }
                        default:
                            {
                                switch (num2)
                                {
                                    case 50:
                                        {
                                            matrix1._m11 *= matrix2._m11;
                                            matrix1._m22 *= matrix2._m22;
                                            matrix1._offsetX *= matrix2._m11;
                                            matrix1._offsetY *= matrix2._m22;
                                            return;
                                        }
                                    case 51:
                                        {
                                            matrix1._m11 *= matrix2._m11;
                                            matrix1._m22 *= matrix2._m22;
                                            matrix1._offsetX = matrix2._m11 * matrix1._offsetX + matrix2._offsetX;
                                            matrix1._offsetY = matrix2._m22 * matrix1._offsetY + matrix2._offsetY;
                                            return;
                                        }
                                    case 52:
                                        {
                                            break;
                                        }
                                    default:
                                        {
                                            switch (num2)
                                            {
                                                case 66:
                                                case 67:
                                                case 68:
                                                    {
                                                        break;
                                                    }
                                                default:
                                                    {
                                                        return;
                                                    }
                                            }
                                            break;
                                        }
                                }
                                break;
                            }
                    }
                    matrix1 = new MatrixExt(matrix1._m11 * matrix2._m11 + matrix1._m12 * matrix2._m21, matrix1._m11 * matrix2._m12 + matrix1._m12 * matrix2._m22, matrix1._m21 * matrix2._m11 + matrix1._m22 * matrix2._m21, matrix1._m21 * matrix2._m12 + matrix1._m22 * matrix2._m22, matrix1._offsetX * matrix2._m11 + matrix1._offsetY * matrix2._m21 + matrix2._offsetX, matrix1._offsetX * matrix2._m12 + matrix1._offsetY * matrix2._m22 + matrix2._offsetY);
                    return;
                }
                double offsetX = matrix1._offsetX;
                double offsetY = matrix1._offsetY;
                matrix1 = matrix2;
                matrix1._offsetX = offsetX * matrix2._m11 + offsetY * matrix2._m21 + matrix2._offsetX;
                matrix1._offsetY = offsetX * matrix2._m12 + offsetY * matrix2._m22 + matrix2._offsetY;
                if (type2 == MatrixTypes.TRANSFORM_IS_UNKNOWN)
                {
                    matrix1._type = MatrixTypes.TRANSFORM_IS_UNKNOWN;
                    return;
                }
                matrix1._type = (MatrixTypes.TRANSFORM_IS_TRANSLATION | MatrixTypes.TRANSFORM_IS_SCALING);
            }
            internal static void PrependOffset(ref MatrixExt matrix, double offsetX, double offsetY)
            {
                if (matrix._type == MatrixTypes.TRANSFORM_IS_IDENTITY)
                {
                    matrix = new MatrixExt(1.0, 0.0, 0.0, 1.0, offsetX, offsetY);
                    matrix._type = MatrixTypes.TRANSFORM_IS_TRANSLATION;
                    return;
                }
                matrix._offsetX += matrix._m11 * offsetX + matrix._m21 * offsetY;
                matrix._offsetY += matrix._m12 * offsetX + matrix._m22 * offsetY;
                if (matrix._type != MatrixTypes.TRANSFORM_IS_UNKNOWN)
                {
                    matrix._type |= MatrixTypes.TRANSFORM_IS_TRANSLATION;
                }
            }
        }

		private static readonly MatrixExt s_identity = MatrixExt.CreateIdentity();
		internal double _m11;
		internal double _m12;
		internal double _m21;
		internal double _m22;
		internal double _offsetX;
		internal double _offsetY;
		internal MatrixTypes _type;
		internal int _padding;
		private const int c_identityHashCode = 0;
		/// <summary> Gets an identity <see cref="T:System.Windows.Media.Matrix" />. </summary>
		/// <returns>An identity matrix.</returns>
		public static MatrixExt Identity
		{
			get
			{
				return MatrixExt.s_identity;
			}
		}
		/// <summary> Gets a value that indicates whether this <see cref="T:System.Windows.Media.Matrix" /> structure is an identity matrix. </summary>
		/// <returns>true if the <see cref="T:System.Windows.Media.Matrix" /> structure is an identity matrix; otherwise, false. The default is true.</returns>
		public bool IsIdentity
		{
			get
			{
				return this._type == MatrixTypes.TRANSFORM_IS_IDENTITY || (this._m11 == 1.0 && this._m12 == 0.0 && this._m21 == 0.0 && this._m22 == 1.0 && this._offsetX == 0.0 && this._offsetY == 0.0);
			}
		}
		/// <summary> Gets the determinant of this <see cref="T:System.Windows.Media.Matrix" /> structure. </summary>
		/// <returns>The determinant of this <see cref="T:System.Windows.Media.Matrix" />.</returns>
		public double Determinant
		{
			get
			{
				switch (this._type)
				{
					case MatrixTypes.TRANSFORM_IS_IDENTITY:
					case MatrixTypes.TRANSFORM_IS_TRANSLATION:
					{
						return 1.0;
					}
					case MatrixTypes.TRANSFORM_IS_SCALING:
					case MatrixTypes.TRANSFORM_IS_TRANSLATION | MatrixTypes.TRANSFORM_IS_SCALING:
					{
						return this._m11 * this._m22;
					}
					default:
					{
						return this._m11 * this._m22 - this._m12 * this._m21;
					}
				}
			}
		}
		/// <summary> Gets a value that indicates whether this <see cref="T:System.Windows.Media.Matrix" /> structure is invertible. </summary>
		/// <returns>true if the <see cref="T:System.Windows.Media.Matrix" /> has an inverse; otherwise, false. The default is true.</returns>
		public bool HasInverse
		{
			get
			{
			    return !(Math.Abs(this.Determinant) < 2.2204460492503131E-15); 
			}
		}
		/// <summary>Gets or sets the value of the first row and first column of this <see cref="T:System.Windows.Media.Matrix" /> structure. </summary>
		/// <returns>The value of the first row and first column of this <see cref="T:System.Windows.Media.Matrix" />. The default value is 1.</returns>
		public double M11
		{
			get
			{
				if (this._type == MatrixTypes.TRANSFORM_IS_IDENTITY)
				{
					return 1.0;
				}
				return this._m11;
			}
			set
			{
				if (this._type == MatrixTypes.TRANSFORM_IS_IDENTITY)
				{
					this.SetMatrix(value, 0.0, 0.0, 1.0, 0.0, 0.0, MatrixTypes.TRANSFORM_IS_SCALING);
					return;
				}
				this._m11 = value;
				if (this._type != MatrixTypes.TRANSFORM_IS_UNKNOWN)
				{
					this._type |= MatrixTypes.TRANSFORM_IS_SCALING;
				}
			}
		}
		/// <summary> Gets or sets the value of the first row and second column of this <see cref="T:System.Windows.Media.Matrix" /> structure. </summary>
		/// <returns>The value of the first row and second column of this <see cref="T:System.Windows.Media.Matrix" />. The default value is 0.</returns>
		public double M12
		{
			get
			{
				if (this._type == MatrixTypes.TRANSFORM_IS_IDENTITY)
				{
					return 0.0;
				}
				return this._m12;
			}
			set
			{
				if (this._type == MatrixTypes.TRANSFORM_IS_IDENTITY)
				{
					this.SetMatrix(1.0, value, 0.0, 1.0, 0.0, 0.0, MatrixTypes.TRANSFORM_IS_UNKNOWN);
					return;
				}
				this._m12 = value;
				this._type = MatrixTypes.TRANSFORM_IS_UNKNOWN;
			}
		}
		/// <summary> Gets or sets the value of the second row and first column of this <see cref="T:System.Windows.Media.Matrix" /> structure.</summary>
		/// <returns>The value of the second row and first column of this <see cref="T:System.Windows.Media.Matrix" />. The default value is 0.</returns>
		public double M21
		{
			get
			{
				if (this._type == MatrixTypes.TRANSFORM_IS_IDENTITY)
				{
					return 0.0;
				}
				return this._m21;
			}
			set
			{
				if (this._type == MatrixTypes.TRANSFORM_IS_IDENTITY)
				{
					this.SetMatrix(1.0, 0.0, value, 1.0, 0.0, 0.0, MatrixTypes.TRANSFORM_IS_UNKNOWN);
					return;
				}
				this._m21 = value;
				this._type = MatrixTypes.TRANSFORM_IS_UNKNOWN;
			}
		}
		/// <summary>Gets or sets the value of the second row and second column of this <see cref="T:System.Windows.Media.Matrix" /> structure. </summary>
		/// <returns>The value of the second row and second column of this <see cref="T:System.Windows.Media.Matrix" /> structure. The default value is 1.</returns>
		public double M22
		{
			get
			{
				if (this._type == MatrixTypes.TRANSFORM_IS_IDENTITY)
				{
					return 1.0;
				}
				return this._m22;
			}
			set
			{
				if (this._type == MatrixTypes.TRANSFORM_IS_IDENTITY)
				{
					this.SetMatrix(1.0, 0.0, 0.0, value, 0.0, 0.0, MatrixTypes.TRANSFORM_IS_SCALING);
					return;
				}
				this._m22 = value;
				if (this._type != MatrixTypes.TRANSFORM_IS_UNKNOWN)
				{
					this._type |= MatrixTypes.TRANSFORM_IS_SCALING;
				}
			}
		}
		/// <summary>Gets or sets the value of the third row and first column of this <see cref="T:System.Windows.Media.Matrix" /> structure.  </summary>
		/// <returns>The value of the third row and first column of this <see cref="T:System.Windows.Media.Matrix" /> structure. The default value is 0.</returns>
		public double OffsetX
		{
			get
			{
				if (this._type == MatrixTypes.TRANSFORM_IS_IDENTITY)
				{
					return 0.0;
				}
				return this._offsetX;
			}
			set
			{
				if (this._type == MatrixTypes.TRANSFORM_IS_IDENTITY)
				{
					this.SetMatrix(1.0, 0.0, 0.0, 1.0, value, 0.0, MatrixTypes.TRANSFORM_IS_TRANSLATION);
					return;
				}
				this._offsetX = value;
				if (this._type != MatrixTypes.TRANSFORM_IS_UNKNOWN)
				{
					this._type |= MatrixTypes.TRANSFORM_IS_TRANSLATION;
				}
			}
		}
		/// <summary>Gets or sets the value of the third row and second column of this <see cref="T:System.Windows.Media.Matrix" /> structure. </summary>
		/// <returns>The value of the third row and second column of this <see cref="T:System.Windows.Media.Matrix" /> structure. The default value is 0.</returns>
		public double OffsetY
		{
			get
			{
				if (this._type == MatrixTypes.TRANSFORM_IS_IDENTITY)
				{
					return 0.0;
				}
				return this._offsetY;
			}
			set
			{
				if (this._type == MatrixTypes.TRANSFORM_IS_IDENTITY)
				{
					this.SetMatrix(1.0, 0.0, 0.0, 1.0, 0.0, value, MatrixTypes.TRANSFORM_IS_TRANSLATION);
					return;
				}
				this._offsetY = value;
				if (this._type != MatrixTypes.TRANSFORM_IS_UNKNOWN)
				{
					this._type |= MatrixTypes.TRANSFORM_IS_TRANSLATION;
				}
			}
		}
		private bool IsDistinguishedIdentity
		{
			get
			{
				return this._type == MatrixTypes.TRANSFORM_IS_IDENTITY;
			}
		}
		/// <summary> Initializes a new instance of the <see cref="T:System.Windows.Media.Matrix" /> structure. </summary>
		/// <param name="m11">The new <see cref="T:System.Windows.Media.Matrix" /> structure's <see cref="P:System.Windows.Media.Matrix.M11" /> coefficient.</param>
		/// <param name="m12">The new <see cref="T:System.Windows.Media.Matrix" /> structure's <see cref="P:System.Windows.Media.Matrix.M12" /> coefficient.</param>
		/// <param name="m21">The new <see cref="T:System.Windows.Media.Matrix" /> structure's <see cref="P:System.Windows.Media.Matrix.M21" /> coefficient.</param>
		/// <param name="m22">The new <see cref="T:System.Windows.Media.Matrix" /> structure's <see cref="P:System.Windows.Media.Matrix.M22" /> coefficient.</param>
		/// <param name="offsetX">The new <see cref="T:System.Windows.Media.Matrix" /> structure's <see cref="P:System.Windows.Media.Matrix.OffsetX" /> coefficient.</param>
		/// <param name="offsetY">The new <see cref="T:System.Windows.Media.Matrix" /> structure's <see cref="P:System.Windows.Media.Matrix.OffsetY" /> coefficient.</param>
		public MatrixExt(double m11, double m12, double m21, double m22, double offsetX, double offsetY)
		{
			this._m11 = m11;
			this._m12 = m12;
			this._m21 = m21;
			this._m22 = m22;
			this._offsetX = offsetX;
			this._offsetY = offsetY;
			this._type = MatrixTypes.TRANSFORM_IS_UNKNOWN;
			this._padding = 0;
			this.DeriveMatrixType();
		}
		/// <summary> Changes this <see cref="T:System.Windows.Media.Matrix" /> structure into an identity matrix. </summary>
		public void SetIdentity()
		{
			this._type = MatrixTypes.TRANSFORM_IS_IDENTITY;
		}
		/// <summary> Multiplies a <see cref="T:System.Windows.Media.Matrix" /> structure by another <see cref="T:System.Windows.Media.Matrix" /> structure. </summary>
		/// <returns>The result of multiplying <paramref name="trans1" /> by <paramref name="trans2" />.</returns>
		/// <param name="trans1">The first <see cref="T:System.Windows.Media.Matrix" /> structure to multiply.</param>
		/// <param name="trans2">The second <see cref="T:System.Windows.Media.Matrix" /> structure to multiply.</param>
		public static MatrixExt operator *(MatrixExt trans1, MatrixExt trans2)
		{
			MatrixUtil.MultiplyMatrix(ref trans1, ref trans2);
			return trans1;
		}
		/// <summary> Multiplies a <see cref="T:System.Windows.Media.Matrix" /> structure by another <see cref="T:System.Windows.Media.Matrix" /> structure. </summary>
		/// <returns>The result of multiplying <paramref name="trans1" /> by <paramref name="trans2" />.</returns>
		/// <param name="trans1">The first <see cref="T:System.Windows.Media.Matrix" /> structure to multiply.</param>
		/// <param name="trans2">The second <see cref="T:System.Windows.Media.Matrix" /> structure to multiply.</param>
		public static MatrixExt Multiply(MatrixExt trans1, MatrixExt trans2)
		{
			MatrixUtil.MultiplyMatrix(ref trans1, ref trans2);
			return trans1;
		}
		/// <summary> Appends the specified <see cref="T:System.Windows.Media.Matrix" /> structure to this <see cref="T:System.Windows.Media.Matrix" /> structure. </summary>
		/// <param name="matrix">The <see cref="T:System.Windows.Media.Matrix" /> structure to append to this <see cref="T:System.Windows.Media.Matrix" /> structure.</param>
		public void Append(MatrixExt matrix)
		{
			this *= matrix;
		}
		/// <summary> Prepends the specified <see cref="T:System.Windows.Media.Matrix" /> structure onto this <see cref="T:System.Windows.Media.Matrix" /> structure. </summary>
		/// <param name="matrix">The <see cref="T:System.Windows.Media.Matrix" /> structure to prepend to this <see cref="T:System.Windows.Media.Matrix" /> structure.</param>
		public void Prepend(MatrixExt matrix)
		{
			this = matrix * this;
		}
		/// <summary> Applies a rotation of the specified angle about the origin of this <see cref="T:System.Windows.Media.Matrix" /> structure. </summary>
		/// <param name="angle">The angle of rotation.</param>
		public void Rotate(double angle)
		{
			angle %= 360.0;
			this *= MatrixExt.CreateRotationRadians(angle * 0.017453292519943295);
		}
		/// <summary> Prepends a rotation of the specified angle to this <see cref="T:System.Windows.Media.Matrix" /> structure. </summary>
		/// <param name="angle">The angle of rotation to prepend.</param>
		public void RotatePrepend(double angle)
		{
			angle %= 360.0;
			this = MatrixExt.CreateRotationRadians(angle * 0.017453292519943295) * this;
		}
		/// <summary>Rotates this matrix about the specified point.</summary>
		/// <param name="angle">The angle, in degrees, by which to rotate this matrix. </param>
		/// <param name="centerX">The x-coordinate of the point about which to rotate this matrix.</param>
		/// <param name="centerY">The y-coordinate of the point about which to rotate this matrix.</param>
		public void RotateAt(double angle, double centerX, double centerY)
		{
			angle %= 360.0;
			this *= MatrixExt.CreateRotationRadians(angle * 0.017453292519943295, centerX, centerY);
		}
		/// <summary>Prepends a rotation of the specified angle at the specified point to this <see cref="T:System.Windows.Media.Matrix" /> structure.</summary>
		/// <param name="angle">The rotation angle, in degrees.</param>
		/// <param name="centerX">The x-coordinate of the rotation center.</param>
		/// <param name="centerY">The y-coordinate of the rotation center.</param>
		public void RotateAtPrepend(double angle, double centerX, double centerY)
		{
			angle %= 360.0;
			this = MatrixExt.CreateRotationRadians(angle * 0.017453292519943295, centerX, centerY) * this;
		}
		/// <summary> Appends the specified scale vector to this <see cref="T:System.Windows.Media.Matrix" /> structure. </summary>
		/// <param name="scaleX">The value by which to scale this <see cref="T:System.Windows.Media.Matrix" /> along the x-axis.</param>
		/// <param name="scaleY">The value by which to scale this <see cref="T:System.Windows.Media.Matrix" /> along the y-axis.</param>
		public void Scale(double scaleX, double scaleY)
		{
			this *= MatrixExt.CreateScaling(scaleX, scaleY);
		}
		/// <summary> Prepends the specified scale vector to this <see cref="T:System.Windows.Media.Matrix" /> structure. </summary>
		/// <param name="scaleX">The value by which to scale this <see cref="T:System.Windows.Media.Matrix" /> structure along the x-axis.</param>
		/// <param name="scaleY">The value by which to scale this <see cref="T:System.Windows.Media.Matrix" /> structure along the y-axis.</param>
		public void ScalePrepend(double scaleX, double scaleY)
		{
			this = MatrixExt.CreateScaling(scaleX, scaleY) * this;
		}
		/// <summary>Scales this <see cref="T:System.Windows.Media.Matrix" /> by the specified amount about the specified point.</summary>
		/// <param name="scaleX">The amount by which to scale this <see cref="T:System.Windows.Media.Matrix" /> along the x-axis. </param>
		/// <param name="scaleY">The amount by which to scale this <see cref="T:System.Windows.Media.Matrix" /> along the y-axis.</param>
		/// <param name="centerX">The x-coordinate of the scale operation's center point.</param>
		/// <param name="centerY">The y-coordinate of the scale operation's center point.</param>
		public void ScaleAt(double scaleX, double scaleY, double centerX, double centerY)
		{
			this *= MatrixExt.CreateScaling(scaleX, scaleY, centerX, centerY);
		}
		/// <summary>Prepends the specified scale about the specified point of this <see cref="T:System.Windows.Media.Matrix" />.</summary>
		/// <param name="scaleX">The x-axis scale factor.</param>
		/// <param name="scaleY">The y-axis scale factor.</param>
		/// <param name="centerX">The x-coordinate of the point about which the scale operation is performed.</param>
		/// <param name="centerY">The y-coordinate of the point about which the scale operation is performed.</param>
		public void ScaleAtPrepend(double scaleX, double scaleY, double centerX, double centerY)
		{
			this = MatrixExt.CreateScaling(scaleX, scaleY, centerX, centerY) * this;
		}
		/// <summary> Appends a skew of the specified degrees in the x and y dimensions to this <see cref="T:System.Windows.Media.Matrix" /> structure. </summary>
		/// <param name="skewX">The angle in the x dimension by which to skew this <see cref="T:System.Windows.Media.Matrix" />.</param>
		/// <param name="skewY">The angle in the y dimension by which to skew this <see cref="T:System.Windows.Media.Matrix" />.</param>
		public void Skew(double skewX, double skewY)
		{
			skewX %= 360.0;
			skewY %= 360.0;
			this *= MatrixExt.CreateSkewRadians(skewX * 0.017453292519943295, skewY * 0.017453292519943295);
		}
		/// <summary> Prepends a skew of the specified degrees in the x and y dimensions to this <see cref="T:System.Windows.Media.Matrix" /> structure. </summary>
		/// <param name="skewX">The angle in the x dimension by which to skew this <see cref="T:System.Windows.Media.Matrix" />.</param>
		/// <param name="skewY">The angle in the y dimension by which to skew this <see cref="T:System.Windows.Media.Matrix" />.</param>
		public void SkewPrepend(double skewX, double skewY)
		{
			skewX %= 360.0;
			skewY %= 360.0;
			this = MatrixExt.CreateSkewRadians(skewX * 0.017453292519943295, skewY * 0.017453292519943295) * this;
		}
		/// <summary> Appends a translation of the specified offsets to this <see cref="T:System.Windows.Media.Matrix" /> structure. </summary>
		/// <param name="offsetX">The amount to offset this <see cref="T:System.Windows.Media.Matrix" /> along the x-axis.</param>
		/// <param name="offsetY">The amount to offset this <see cref="T:System.Windows.Media.Matrix" /> along the y-axis.</param>
		public void Translate(double offsetX, double offsetY)
		{
			if (this._type == MatrixTypes.TRANSFORM_IS_IDENTITY)
			{
				this.SetMatrix(1.0, 0.0, 0.0, 1.0, offsetX, offsetY, MatrixTypes.TRANSFORM_IS_TRANSLATION);
				return;
			}
			if (this._type == MatrixTypes.TRANSFORM_IS_UNKNOWN)
			{
				this._offsetX += offsetX;
				this._offsetY += offsetY;
				return;
			}
			this._offsetX += offsetX;
			this._offsetY += offsetY;
			this._type |= MatrixTypes.TRANSFORM_IS_TRANSLATION;
		}
		/// <summary> Prepends a translation of the specified offsets to this <see cref="T:System.Windows.Media.Matrix" /> structure. </summary>
		/// <param name="offsetX">The amount to offset this <see cref="T:System.Windows.Media.Matrix" /> along the x-axis.</param>
		/// <param name="offsetY">The amount to offset this <see cref="T:System.Windows.Media.Matrix" /> along the y-axis.</param>
		public void TranslatePrepend(double offsetX, double offsetY)
		{
			this = MatrixExt.CreateTranslation(offsetX, offsetY) * this;
		}
		/// <summary>Transforms the specified point by the <see cref="T:System.Windows.Media.Matrix" /> and returns the result.</summary>
		/// <returns>The result of transforming <paramref name="point" /> by this <see cref="T:System.Windows.Media.Matrix" />.</returns>
		/// <param name="point">The point to transform.</param>
		public Point Transform(Point point)
		{
			Point result = point;
		    double x = result.X;
		    double y = result.Y;
			this.MultiplyPoint(ref x, ref y);
		    result = new Point(x, y);
			return result;
		}
		/// <summary>Transforms the specified points by this <see cref="T:System.Windows.Media.Matrix" />. </summary>
		/// <param name="points">The points to transform. The original points in the array are replaced by their transformed values.</param>
		public void Transform(Point[] points)
		{
			if (points != null)
			{
				for (int i = 0; i < points.Length; i++)
                {
                    double x = points[i].X;
                    double y = points[i].Y;
					this.MultiplyPoint(ref x, ref y);
                    points[i] = new Point(x, y);
                }
			}
		}
		/// <summary> Inverts this <see cref="T:System.Windows.Media.Matrix" /> structure. </summary>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Windows.Media.Matrix" /> structure is not invertible.</exception>
		public void Invert()
		{
			double determinant = this.Determinant;
			if (Math.Abs(determinant) < 2.2204460492503131E-15)
			{
				throw new InvalidOperationException("Transform_NotInvertible");
			}
			switch (this._type)
			{
				case MatrixTypes.TRANSFORM_IS_IDENTITY:
				{
					break;
				}
				case MatrixTypes.TRANSFORM_IS_TRANSLATION:
				{
					this._offsetX = -this._offsetX;
					this._offsetY = -this._offsetY;
					return;
				}
				case MatrixTypes.TRANSFORM_IS_SCALING:
				{
					this._m11 = 1.0 / this._m11;
					this._m22 = 1.0 / this._m22;
					return;
				}
				case MatrixTypes.TRANSFORM_IS_TRANSLATION | MatrixTypes.TRANSFORM_IS_SCALING:
				{
					this._m11 = 1.0 / this._m11;
					this._m22 = 1.0 / this._m22;
					this._offsetX = -this._offsetX * this._m11;
					this._offsetY = -this._offsetY * this._m22;
					return;
				}
				default:
				{
					double num = 1.0 / determinant;
					this.SetMatrix(this._m22 * num, -this._m12 * num, -this._m21 * num, this._m11 * num, (this._m21 * this._offsetY - this._offsetX * this._m22) * num, (this._offsetX * this._m12 - this._m11 * this._offsetY) * num, MatrixTypes.TRANSFORM_IS_UNKNOWN);
					break;
				}
			}
		}
		internal void MultiplyVector(ref double x, ref double y)
		{
			switch (this._type)
			{
				case MatrixTypes.TRANSFORM_IS_IDENTITY:
				case MatrixTypes.TRANSFORM_IS_TRANSLATION:
				{
					return;
				}
				case MatrixTypes.TRANSFORM_IS_SCALING:
				case MatrixTypes.TRANSFORM_IS_TRANSLATION | MatrixTypes.TRANSFORM_IS_SCALING:
				{
					x *= this._m11;
					y *= this._m22;
					return;
				}
				default:
				{
					double num = y * this._m21;
					double num2 = x * this._m12;
					x *= this._m11;
					x += num;
					y *= this._m22;
					y += num2;
					return;
				}
			}
		}
		internal void MultiplyPoint(ref double x, ref double y)
		{
			switch (this._type)
			{
				case MatrixTypes.TRANSFORM_IS_IDENTITY:
				{
					return;
				}
				case MatrixTypes.TRANSFORM_IS_TRANSLATION:
				{
					x += this._offsetX;
					y += this._offsetY;
					return;
				}
				case MatrixTypes.TRANSFORM_IS_SCALING:
				{
					x *= this._m11;
					y *= this._m22;
					return;
				}
				case MatrixTypes.TRANSFORM_IS_TRANSLATION | MatrixTypes.TRANSFORM_IS_SCALING:
				{
					x *= this._m11;
					x += this._offsetX;
					y *= this._m22;
					y += this._offsetY;
					return;
				}
				default:
				{
					double num = y * this._m21 + this._offsetX;
					double num2 = x * this._m12 + this._offsetY;
					x *= this._m11;
					x += num;
					y *= this._m22;
					y += num2;
					return;
				}
			}
		}
		internal static MatrixExt CreateRotationRadians(double angle)
		{
			return MatrixExt.CreateRotationRadians(angle, 0.0, 0.0);
		}
		internal static MatrixExt CreateRotationRadians(double angle, double centerX, double centerY)
		{
			MatrixExt result = default(MatrixExt);
			double num = Math.Sin(angle);
			double num2 = Math.Cos(angle);
			double offsetX = centerX * (1.0 - num2) + centerY * num;
			double offsetY = centerY * (1.0 - num2) - centerX * num;
			result.SetMatrix(num2, num, -num, num2, offsetX, offsetY, MatrixTypes.TRANSFORM_IS_UNKNOWN);
			return result;
		}
		internal static MatrixExt CreateScaling(double scaleX, double scaleY, double centerX, double centerY)
		{
			MatrixExt result = default(MatrixExt);
			result.SetMatrix(scaleX, 0.0, 0.0, scaleY, centerX - scaleX * centerX, centerY - scaleY * centerY, MatrixTypes.TRANSFORM_IS_TRANSLATION | MatrixTypes.TRANSFORM_IS_SCALING);
			return result;
		}
		internal static MatrixExt CreateScaling(double scaleX, double scaleY)
		{
			MatrixExt result = default(MatrixExt);
			result.SetMatrix(scaleX, 0.0, 0.0, scaleY, 0.0, 0.0, MatrixTypes.TRANSFORM_IS_SCALING);
			return result;
		}
		internal static MatrixExt CreateSkewRadians(double skewX, double skewY)
		{
			MatrixExt result = default(MatrixExt);
			result.SetMatrix(1.0, Math.Tan(skewY), Math.Tan(skewX), 1.0, 0.0, 0.0, MatrixTypes.TRANSFORM_IS_UNKNOWN);
			return result;
		}
		internal static MatrixExt CreateTranslation(double offsetX, double offsetY)
		{
			MatrixExt result = default(MatrixExt);
			result.SetMatrix(1.0, 0.0, 0.0, 1.0, offsetX, offsetY, MatrixTypes.TRANSFORM_IS_TRANSLATION);
			return result;
		}
		/// <summary> Determines whether the two specified <see cref="T:System.Windows.Media.Matrix" /> structures are identical.</summary>
		/// <returns>true if <paramref name="matrix1" /> and <paramref name="matrix2" /> are identical; otherwise, false.</returns>
		/// <param name="matrix1">The first <see cref="T:System.Windows.Media.Matrix" /> structure to compare.</param>
		/// <param name="matrix2">The second <see cref="T:System.Windows.Media.Matrix" /> structure to compare.</param>
		public static bool operator ==(MatrixExt matrix1, MatrixExt matrix2)
		{
			if (matrix1.IsDistinguishedIdentity || matrix2.IsDistinguishedIdentity)
			{
				return matrix1.IsIdentity == matrix2.IsIdentity;
			}
			return matrix1.M11 == matrix2.M11 && matrix1.M12 == matrix2.M12 && matrix1.M21 == matrix2.M21 && matrix1.M22 == matrix2.M22 && matrix1.OffsetX == matrix2.OffsetX && matrix1.OffsetY == matrix2.OffsetY;
		}
		/// <summary> Determines whether the two specified <see cref="T:System.Windows.Media.Matrix" /> structures are not identical.</summary>
		/// <returns>true if <paramref name="matrix1" /> and <paramref name="matrix2" /> are not identical; otherwise, false.</returns>
		/// <param name="matrix1">The first <see cref="T:System.Windows.Media.Matrix" /> structure to compare.</param>
		/// <param name="matrix2">The second <see cref="T:System.Windows.Media.Matrix" /> structure to compare.</param>
		public static bool operator !=(MatrixExt matrix1, MatrixExt matrix2)
		{
			return !(matrix1 == matrix2);
		}
		/// <summary> Determines whether the two specified <see cref="T:System.Windows.Media.Matrix" /> structures are identical.</summary>
		/// <returns>true if <paramref name="matrix1" /> and <paramref name="matrix2" /> are identical; otherwise, false.</returns>
		/// <param name="matrix1">The first <see cref="T:System.Windows.Media.Matrix" /> structure to compare.</param>
		/// <param name="matrix2">The second <see cref="T:System.Windows.Media.Matrix" /> structure to compare.</param>
		public static bool Equals(MatrixExt matrix1, MatrixExt matrix2)
		{
			if (matrix1.IsDistinguishedIdentity || matrix2.IsDistinguishedIdentity)
			{
				return matrix1.IsIdentity == matrix2.IsIdentity;
			}
			return matrix1.M11.Equals(matrix2.M11) && matrix1.M12.Equals(matrix2.M12) && matrix1.M21.Equals(matrix2.M21) && matrix1.M22.Equals(matrix2.M22) && matrix1.OffsetX.Equals(matrix2.OffsetX) && matrix1.OffsetY.Equals(matrix2.OffsetY);
		}
		/// <summary> Determines whether the specified <see cref="T:System.Object" /> is a <see cref="T:System.Windows.Media.Matrix" /> structure that is identical to this <see cref="T:System.Windows.Media.Matrix" />. </summary>
		/// <returns>true if <paramref name="o" /> is a <see cref="T:System.Windows.Media.Matrix" /> structure that is identical to this <see cref="T:System.Windows.Media.Matrix" /> structure; otherwise, false.</returns>
		/// <param name="o">The <see cref="T:System.Object" /> to compare.</param>
		public override bool Equals(object o)
		{
			if (o == null || !(o is MatrixExt))
			{
				return false;
			}
			MatrixExt matrix = (MatrixExt)o;
			return MatrixExt.Equals(this, matrix);
		}
		/// <summary> Determines whether the specified <see cref="T:System.Windows.Media.Matrix" /> structure is identical to this instance. </summary>
		/// <returns>true if instances are equal; otherwise, false. </returns>
		/// <param name="value">The instance of <see cref="T:System.Windows.Media.Matrix" /> to compare to this instance.</param>
		public bool Equals(MatrixExt value)
		{
			return MatrixExt.Equals(this, value);
		}
		/// <summary> Returns the hash code for this <see cref="T:System.Windows.Media.Matrix" /> structure. </summary>
		/// <returns>The hash code for this instance.</returns>
		public override int GetHashCode()
		{
			if (this.IsDistinguishedIdentity)
			{
				return 0;
			}
			return this.M11.GetHashCode() ^ this.M12.GetHashCode() ^ this.M21.GetHashCode() ^ this.M22.GetHashCode() ^ this.OffsetX.GetHashCode() ^ this.OffsetY.GetHashCode();
		}

		private static MatrixExt CreateIdentity()
		{
			MatrixExt result = default(MatrixExt);
			result.SetMatrix(1.0, 0.0, 0.0, 1.0, 0.0, 0.0, MatrixTypes.TRANSFORM_IS_IDENTITY);
			return result;
		}
		private void SetMatrix(double m11, double m12, double m21, double m22, double offsetX, double offsetY, MatrixTypes type)
		{
			this._m11 = m11;
			this._m12 = m12;
			this._m21 = m21;
			this._m22 = m22;
			this._offsetX = offsetX;
			this._offsetY = offsetY;
			this._type = type;
		}
		private void DeriveMatrixType()
		{
			this._type = MatrixTypes.TRANSFORM_IS_IDENTITY;
			if (this._m21 != 0.0 || this._m12 != 0.0)
			{
				this._type = MatrixTypes.TRANSFORM_IS_UNKNOWN;
				return;
			}
			if (this._m11 != 1.0 || this._m22 != 1.0)
			{
				this._type = MatrixTypes.TRANSFORM_IS_SCALING;
			}
			if (this._offsetX != 0.0 || this._offsetY != 0.0)
			{
				this._type |= MatrixTypes.TRANSFORM_IS_TRANSLATION;
			}
			if ((this._type & (MatrixTypes.TRANSFORM_IS_TRANSLATION | MatrixTypes.TRANSFORM_IS_SCALING)) == MatrixTypes.TRANSFORM_IS_IDENTITY)
			{
				this._type = MatrixTypes.TRANSFORM_IS_IDENTITY;
			}
		}
		[Conditional("DEBUG")]
		private void Debug_CheckType()
		{
			switch (this._type)
			{
				case MatrixTypes.TRANSFORM_IS_IDENTITY:
				case MatrixTypes.TRANSFORM_IS_TRANSLATION:
				case MatrixTypes.TRANSFORM_IS_SCALING:
				case MatrixTypes.TRANSFORM_IS_TRANSLATION | MatrixTypes.TRANSFORM_IS_SCALING:
				case MatrixTypes.TRANSFORM_IS_UNKNOWN:
				{
					return;
				}
			}
		}
	}
}

