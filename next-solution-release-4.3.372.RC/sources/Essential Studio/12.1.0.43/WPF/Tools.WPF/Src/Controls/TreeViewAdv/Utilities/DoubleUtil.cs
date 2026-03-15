// <copyright file="DoubleUtil.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

#region file using

using System;
using System.Windows;

#endregion file using

namespace Syncfusion.Windows.Tools
{
    /// <summary>
    /// Represents DoubleUtil class
    /// </summary>
    internal static class DoubleUtil
    {
        #region Constants

        /// <summary>
        /// Presents DBL_EPSILON
        /// </summary>
        private const double DBL_EPSILON = 2.2204460492503131E-16;

        /// <summary>
        /// Presents FLT_MIN
        /// </summary>
        private const float FLT_MIN = 1.175494E-38f;

        #endregion Constants

        #region Public methods

        /// <summary>
        /// Ares the close.
        /// </summary>
        /// <param name="value1">The value1.</param>
        /// <param name="value2">The value2.</param>
        /// <returns> bool type rect</returns>
        public static bool AreClose(double value1, double value2)
        {
            if (value1 == value2)
            {
                return true;
            }

            double num = ((Math.Abs(value1) + Math.Abs(value2)) + 10) * DBL_EPSILON;
            double num2 = value1 - value2;

            if (-num < num2)
            {
                return num > num2;
            }

            return false;
        }

        /// <summary>
        /// Ares the close.
        /// </summary>
        /// <param name="point1">The point1.</param>
        /// <param name="point2">The point2.</param>
        /// <returns> bool type rect</returns>
        public static bool AreClose(Point point1, Point point2)
        {
            if (AreClose(point1.X, point2.X))
            {
                return AreClose(point1.Y, point2.Y);
            }

            return false;
        }

        /// <summary>
        /// Ares the close.
        /// </summary>
        /// <param name="rect1">The rect1 value.</param>
        /// <param name="rect2">The rect2 value.</param>
        /// <returns> bool type rect</returns>
        public static bool AreClose(Rect rect1, Rect rect2)
        {
            if (rect1.IsEmpty)
            {
                return rect2.IsEmpty;
            }

            if ((!rect2.IsEmpty && AreClose(rect1.X, rect2.X))
                && (AreClose(rect1.Y, rect2.Y) && AreClose(rect1.Height, rect2.Height)))
            {
                return AreClose(rect1.Width, rect2.Width);
            }

            return false;
        }

        /// <summary>
        /// Ares the close.
        /// </summary>
        /// <param name="size1">The size1.</param>
        /// <param name="size2">The size2.</param>
        /// <returns>  bool type rect</returns>
        public static bool AreClose(Size size1, Size size2)
        {
            if (AreClose(size1.Width, size2.Width))
            {
                return AreClose(size1.Height, size2.Height);
            }

            return false;
        }

        /// <summary>
        /// Ares the close.
        /// </summary>
        /// <param name="vector1">The vector1.</param>
        /// <param name="vector2">The vector2.</param>
        /// <returns> bool type rect </returns>
        public static bool AreClose(Vector vector1, Vector vector2)
        {
            if (AreClose(vector1.X, vector2.X))
            {
                return AreClose(vector1.Y, vector2.Y);
            }

            return false;
        }

        /// <summary>
        /// Doubles to int.
        /// </summary>
        /// <param name="val">The val double to int.</param>
        /// <returns> int type rect</returns>
        public static int DoubleToInt(double val)
        {
            if (0 >= val)
            {
                return (int)(val - 0.5);
            }

            return (int)(val + 0.5);
        }

        /// <summary>
        /// Greaters the than.
        /// </summary>
        /// <param name="value1">The value1.</param>
        /// <param name="value2">The value2.</param>
        /// <returns> bool type rect</returns>
        public static bool GreaterThan(double value1, double value2)
        {
            if (value1 > value2)
            {
                return !AreClose(value1, value2);
            }

            return false;
        }

        /// <summary>
        /// Greaters the than or close.
        /// </summary>
        /// <param name="value1">The value1.</param>
        /// <param name="value2">The value2.</param>
        /// <returns> bool type rect</returns>
        public static bool GreaterThanOrClose(double value1, double value2)
        {
            if (value1 <= value2)
            {
                return AreClose(value1, value2);
            }

            return true;
        }

        /// <summary>
        /// Determines whether [is between zero and one] [the specified val].
        /// </summary>
        /// <param name="val">The val is between .</param>
        /// <returns>
        /// true if [is between zero and one] [the specified val]; otherwise, false.
        /// </returns>
        public static bool IsBetweenZeroAndOne(double val)
        {
            if (GreaterThanOrClose(val, 0))
            {
                return LessThanOrClose(val, 1);
            }

            return false;
        }

        /// <summary>
        /// Determines whether [is na N] [the specified value].
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// true if [is na N] [the specified value]; otherwise, false.
        /// </returns>
        public static bool IsNaN(double value)
        {
            NanUnion union = new NanUnion();
            union.DoubleValue = value;
            ////TODO: We need use const instead of value
            ulong num = union.UintValue & 18442240474082181120;
            ulong num2 = union.UintValue & ((ulong)0xfffffffffffff);

            ////TODO: We need use const instead of value
            if ((num != 0x7ff0000000000000) && (num != 18442240474082181120))
            {
                return false;
            }

            return num2 != 0;
        }

        /// <summary>
        /// Determines whether the specified value is one.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// true if the specified value is one; otherwise, false.
        /// </returns>
        public static bool IsOne(double value)
        {
            ////TODO: We need use const instead of value
            return Math.Abs((double)(value - 1)) < 2.2204460492503131E-15;
        }

        /// <summary>
        /// Determines whether the specified value is zero.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// true if the specified value is zero; otherwise, false.
        /// </returns>
        public static bool IsZero(double value)
        {
            ////TODO: We need use const instead of value
            return Math.Abs(value) < 2.2204460492503131E-15;
        }

        /// <summary>
        /// Lesses the than.
        /// </summary>
        /// <param name="value1">The value1.</param>
        /// <param name="value2">The value2.</param>
        /// <returns> bool type rect</returns>
        public static bool LessThan(double value1, double value2)
        {
            if (value1 < value2)
            {
                return !AreClose(value1, value2);
            }

            return false;
        }

        /// <summary>
        /// Lesses the than or close.
        /// </summary>
        /// <param name="value1">The value1.</param>
        /// <param name="value2">The value2.</param>
        /// <returns> bool type rect</returns>
        public static bool LessThanOrClose(double value1, double value2)
        {
            if (value1 >= value2)
            {
                return AreClose(value1, value2);
            }

            return true;
        }

        /// <summary>
        /// Rects the has na N.
        /// </summary>
        /// <param name="r">The r rect has Nan.</param>
        /// <returns> bool type rect</returns>
        public static bool RectHasNaN(Rect r)
        {
            if ((!IsNaN(r.X) && !IsNaN(r.Y)) && (!IsNaN(r.Height) && !IsNaN(r.Width)))
            {
                return false;
            }

            return true;
        }

        #endregion Public methods
    }
}