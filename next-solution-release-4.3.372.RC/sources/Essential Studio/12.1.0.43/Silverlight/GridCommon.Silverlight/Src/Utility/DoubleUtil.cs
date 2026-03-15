#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
#if !WinRT
using System.Windows;
namespace Syncfusion.Windows
#else
using Windows.Foundation;
namespace Syncfusion.WinRT
#endif
{
    internal static class DoubleUtil
    {
        internal const double DBL_EPSILON = 1E-06;

        internal static bool AreClose(double value1, double value2)
        {
            if (value1 == value2)
            {
                return true;
            }
            double num = ((Math.Abs(value1) + Math.Abs(value2)) + 10.0) * 1E-06;
            double num2 = value1 - value2;
            return ((-num < num2) && (num > num2));
        }

        internal static bool GreaterThan(double value1, double value2)
        {
            return ((value1 > value2) && !AreClose(value1, value2));
        }

        public static bool AreClose(Point point1, Point point2)
        {
            return (AreCloseinternal(point1.X, point2.X) && AreCloseinternal(point1.Y, point2.Y));
        }

        internal static bool AreCloseinternal(double value1, double value2)
        {
            if (value1 == value2)
            {
                return true;
            }
            double num2 = ((Math.Abs(value1) + Math.Abs(value2)) + 10.0) * 2.2204460492503131E-16;
            double num = value1 - value2;
            return ((-num2 < num) && (num2 > num));
        }

        public static bool GreaterThanOrClose(double value1, double value2)
        {
            if (value1 <= value2)
            {
                return AreClose(value1, value2);
            }
            return true;
        }

        public static bool LessThan(double value1, double value2)
        {
            return ((value1 < value2) && !AreClose(value1, value2));
        }

        public static bool LessThanOrClose(double value1, double value2)
        {
            if (value1 >= value2)
            {
                return AreClose(value1, value2);
            }
            return true;
        }

        internal static bool IsZero(double value)
        {
            return (Math.Abs(value) < 9.9999999999999991E-06);
        }
    }
}
