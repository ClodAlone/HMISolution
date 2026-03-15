#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
#if WinRT
using Windows.Foundation;
#else
using System.Windows;
#endif


namespace Syncfusion.UI.Xaml.Utility
{
    internal static class DoubleUtil
    {
        internal const double DBL_EPSILON = 2.2204460492503131E-16;
        internal const float FLT_MIN = 1.175494E-38f;

        public static bool AreClose(double value1, double value2)
        {
            if (value1 == value2)
            {
                return true;
            }
            double num2 = ((Math.Abs(value1) + Math.Abs(value2)) + 10.0) * 2.2204460492503131E-16;
            double num = value1 - value2;
            return ((-num2 < num) && (num2 > num));
        }

        public static bool AreClose(Point point1, Point point2)
        {
            return (AreClose(point1.X, point2.X) && AreClose(point1.Y, point2.Y));
        }

        public static bool AreClose(Rect rect1, Rect rect2)
        {
            if (rect1.IsEmpty)
            {
                return rect2.IsEmpty;
            }
            return (((!rect2.IsEmpty && AreClose(rect1.X, rect2.X)) && (AreClose(rect1.Y, rect2.Y) && AreClose(rect1.Height, rect2.Height))) && AreClose(rect1.Width, rect2.Width));
        }

        public static bool AreClose(Size size1, Size size2)
        {
            return (AreClose(size1.Width, size2.Width) && AreClose(size1.Height, size2.Height));
        }

        public static int DoubleToInt(double val)
        {
            if (0.0 >= val)
            {
                return (int)(val - 0.5);
            }
            return (int)(val + 0.5);
        }

        public static bool GreaterThan(double value1, double value2)
        {
            return ((value1 > value2) && !AreClose(value1, value2));
        }

        public static bool GreaterThanOrClose(double value1, double value2)
        {
            if (value1 <= value2)
            {
                return AreClose(value1, value2);
            }
            return true;
        }

        public static bool IsBetweenZeroAndOne(double val)
        {
            return (GreaterThanOrClose(val, 0.0) && LessThanOrClose(val, 1.0));
        }

        public static bool IsOne(double value)
        {
            return (Math.Abs((double)(value - 1.0)) < 2.2204460492503131E-15);
        }

        public static bool IsZero(double value)
        {
            return (Math.Abs(value) < 2.2204460492503131E-15);
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
    }
}
