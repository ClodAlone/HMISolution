#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Windows;
using System.Windows.Input;
using System;
using System.Reflection;
using System.ComponentModel;

#if !WinRT
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Browser;
using Syncfusion.Windows.Controls.Scroll;
using Syncfusion.Windows.Controls.Cells;
namespace Syncfusion.Windows.GridCommon
#else
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Syncfusion.WinRT.GridCommon
#endif
{
    /// <summary>
    /// Helper routines.
    /// </summary>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridUtil
    {

        /// <summary>
        /// Gets the surrounding rect.
        /// </summary>
        /// <param name="pt">The pt.</param>
        /// <param name="size">The size.</param>
        /// <returns></returns>
        public static Rect GetSurroundingRect(Point pt, Size size)
        {
            return new Rect(pt.X - size.Width, pt.Y - size.Height, size.Width * 2, size.Height * 2);
        }

        /// <summary>
        /// Determines whether a point is within double click range of another point and also ensures that the click was within double click time.
        /// </summary>
        /// <param name="pt">The pt.</param>
        /// <param name="mouseDownTick">The mouse down tick (Environment.TickCount at time of click).</param>
        /// <param name="mouseDownPoint">The mouse down point.</param>
        /// <returns>
        /// 	<c>true</c> if this is matches double click criteria; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsDoubleClick(Point pt, int mouseDownTick, Point mouseDownPoint)
        {
            return Environment.TickCount - mouseDownTick <= SystemInformation.DoubleClickTime
                    && GridUtil.GetSurroundingRect(mouseDownPoint, SystemInformation.DoubleClickSize).Contains(pt);
        }

        public static bool IsDoubleClick(Point pt, int tickCount, int mouseDownTick, Point mouseDownPoint)
        {
            return tickCount - mouseDownTick <= SystemInformation.DoubleClickTime
                    && GridUtil.GetSurroundingRect(mouseDownPoint, SystemInformation.DoubleClickSize).Contains(pt);
        }

        /// <summary>
        /// Sets the clip rect.
        /// </summary>
        /// <param name="el">The element.</param>
        /// <param name="r">The clip recangle.</param>
        /// <returns></returns>
        public static RectangleGeometry SetClipRect(FrameworkElement el, Rect r)
        {
            RectangleGeometry oldClip = el.Clip as RectangleGeometry;

            if (oldClip != null && oldClip.Rect == r)
                return null;

            RectangleGeometry clip = new RectangleGeometry();
            clip.Rect = r;
            //clip.Freeze();
            el.Clip = clip;
            return oldClip;
        }

        /// <summary>
        /// Creates a rectangle.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="top">The top.</param>
        /// <param name="right">The right.</param>
        /// <param name="bottom">The bottom.</param>
        /// <returns></returns>
        public static Rect FromLTRB(double left, double top, double right, double bottom)
        {
            if (right < left)
                return FromLTRB(right, top, left, bottom);
            if (bottom < top)
                return FromLTRB(left, bottom, right, top);
            if (double.IsInfinity(left))
                return Rect.Empty;
            return new Rect(left, top, right - left, bottom - top);
        }


        /// <summary>
        /// Remove border margins to get cells client area.
        /// </summary>
        /// <param name="cellRect"></param>
        /// <param name="mi"></param>
        /// <returns></returns>
        public static Rect SubtractBorderMargins(Rect cellRect, Thickness mi)
        {
            if (cellRect.IsEmpty || cellRect.Width <= mi.Left + mi.Right || cellRect.Height <= mi.Top + mi.Bottom)
                return new Rect(0, 0, 0, 0);

            cellRect.Height -= mi.Bottom + mi.Top;
            cellRect.Y += mi.Top;
            cellRect.Width -= mi.Right + mi.Left;
            cellRect.X += mi.Left;

            return cellRect;
        }

        /// <summary>
        /// Determines whether any of the specified flags are set.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="flags">The flags.</param>
        /// <returns>
        /// 	<c>true</c> if the specified value is set; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsSet(int value, int flags)
        {
            return (value & flags) != 0;
        }

        /// <summary>
        /// Determines whether none of the the specified flags is set.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="flags">The flags.</param>
        /// <returns>
        /// 	<c>true</c> if none of the the specified flags is set; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNotSet(int value, int flags)
        {
            return (value & flags) == 0;
        }

        /// <summary>
        /// Determines if all of the specified flags are set.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="flags">The flags.</param>
        /// <returns></returns>
        public static bool AllSet(int value, int flags)
        {
            return (value & flags) == flags;
        }

        /// <summary>
        /// Resets the specified flags.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="flags">The flags.</param>
        public static void ClearBits(ref int value, int flags)
        {
            value &= ~flags;
        }

        /// <summary>
        /// Returns the value not smaller and not larger than a specified range.
        /// </summary>
        /// <param name="value">The value to compare.</param>
        /// <param name="min">The smallest value.</param>
        /// <param name="max">The largest value.</param>
        /// <returns>
        /// The value not smaller than min and and not larger than max.
        /// </returns>
        /// <overload>
        /// Returns the value not smaller and not larger than a specified range.
        /// </overload>
        public static int MinMax(int value, int min, int max)
        {
            return Math.Min(Math.Max(value, min), max);
        }

        /// <summary>
        /// Returns the value not smaller and not larger than a specified range.
        /// </summary>
        /// <param name="value">The value to compare.</param>
        /// <param name="min">The smallest value.</param>
        /// <param name="max">The largest value.</param>
        /// <returns>
        /// The value not smaller than min and and not larger than max.
        /// </returns>
        public static float MinMax(float value, float min, float max)
        {
            return Math.Min(Math.Max(value, min), max);
        }

        /// <summary>
        /// Returns the value not smaller and not larger than a specified range.
        /// </summary>
        /// <param name="value">The value to compare.</param>
        /// <param name="min">The smallest value.</param>
        /// <param name="max">The largest value.</param>
        /// <returns>
        /// The value not smaller than min and and not larger than max.
        /// </returns>
        public static double MinMax(double value, double min, double max)
        {
            return Math.Min(Math.Max(value, min), max);
        }

        /// <summary>
        /// Returns the value not smaller and not larger than a specified range.
        /// </summary>
        /// <param name="point">The value to compare.</param>
        /// <param name="min">The smallest value.</param>
        /// <param name="max">The largest value.</param>
        /// <returns>
        /// The value not smaller than min and and not larger than max.
        /// </returns>
        /// <remarks>
        /// The calculation is made for both the X and Y coordinates separately.
        /// </remarks>
        public static Point MinMax(Point point, Point min, Point max)
        {
            point.X = Math.Min(Math.Max(point.X, min.X), max.X);
            point.Y = Math.Min(Math.Max(point.Y, min.Y), max.Y);
            return point;
        }

        /// <summary>
        /// Returns the value not smaller and not larger than a specified range.
        /// </summary>
        /// <param name="point">The value to compare.</param>
        /// <param name="bounds">The rectangle with bounds.</param>
        /// <returns>
        /// The value not smaller than min and and not larger than max.
        /// </returns>
        /// <remarks>
        /// The calculation is made for both the X and Y coordinates separately and compared
        /// with the rectangles boundaries.
        /// </remarks>
        public static Point MinMax(Point point, Rect bounds)
        {
            return MinMax(point, new Point(bounds.X, bounds.Y), new Point(bounds.Right, bounds.Bottom));
        }

        /// <summary>
        /// Returns the point with smaller value for X and Y coordinates each of two points.
        /// </summary>
        /// <param name="point1">The first point to compare.</param>
        /// <param name="point2">The second point to compare.</param>
        /// <returns>
        /// The point with smaller value for X and Y coordinates each of two points
        /// </returns>
        /// <overload>
        /// Returns the point with smaller value for X and Y coordinates each of two points.
        /// </overload>
        public static Point Min(Point point1, Point point2)
        {
            point1.X = Math.Min(point1.X, point2.X);
            point1.Y = Math.Min(point1.Y, point2.Y);
            return point1;
        }

        /// <summary>
        /// Returns the point with larger value for X and Y coordinates each of two points.
        /// </summary>
        /// <param name="point1">The first point to compare.</param>
        /// <param name="point2">The second point to compare.</param>
        /// <returns>
        /// The point with larger value for X and Y coordinates each of two points.
        /// </returns>
        /// <overload>
        /// Returns the point with larger value for X and Y coordinates each of two points.
        /// </overload>
        public static Point Max(Point point1, Point point2)
        {
            point1.X = Math.Max(point1.X, point2.X);
            point1.Y = Math.Max(point1.Y, point2.Y);
            return point1;
        }


        /// <summary>
        /// Returns the size with smaller value for Width and Height coordinates for each of two values.
        /// </summary>
        /// <param name="size1">The first size to compare.</param>
        /// <param name="size2">The second size to compare.</param>
        /// <returns>
        /// The size with smaller value for Width and Height coordinates for each of two values.
        /// </returns>
        public static Size Min(Size size1, Size size2)
        {
            size1.Width = Math.Min(size1.Width, size2.Width);
            size1.Height = Math.Min(size1.Height, size2.Height);
            return size1;
        }

        /// <summary>
        /// Returns the size with larger value for Width and Height coordinates for each of two values.
        /// </summary>
        /// <param name="size1">The first size to compare.</param>
        /// <param name="size2">The second size to compare.</param>
        /// <returns>
        /// The size with larger value for Width and Height coordinates for each of two values.
        /// </returns>
        public static Size Max(Size size1, Size size2)
        {
            size1.Width = Math.Max(size1.Width, size2.Width);
            size1.Height = Math.Max(size1.Height, size2.Height);
            return size1;
        }

        /// <summary>
        /// Gets the value from the specified object. If the value was not set
        /// for the object then the method will walk up parent nodes until
        /// it finds a parent object with the value set.
        /// </summary>
        /// <param name="dpo">The dependency object.</param>
        /// <param name="dp">The dp.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public static object GetValueInherited(DependencyObject dpo, DependencyProperty dp, object defaultValue)
        {
            while (dpo.ReadLocalValue(dp) == DependencyProperty.UnsetValue)
            {
                if (dpo is UIElement)
                    dpo = VisualTreeHelper.GetParent(dpo);
                else
                    dpo = null;// LogicalTreeHelper.GetParent(dpo);
                if (dpo == null)
                    return defaultValue;
            }
            return dpo.GetValue(dp);
        }

        /// <summary>
        /// Determines whether a given object is a descendant of a parent in the visual or logical tree.
        /// </summary>
        /// <param name="parent">The parent object to be tested.</param>
        /// <param name="obj">The descendant object to be tested.</param>
        /// <returns></returns>
        public static bool IsObjectDescendantOfParent(DependencyObject parent, DependencyObject obj)
        {
            while (obj != null && obj != parent)
            {
                if (obj is UIElement)
                    obj = VisualTreeHelper.GetParent(obj);
                else
                    obj = null;// LogicalTreeHelper.GetParent(obj);
            }
            return obj == parent;
        }

        public static DependencyObject GetParent(DependencyObject current)
        {
            if (current == null)
            {
                throw new ArgumentNullException("current");
            }
            FrameworkElement element = current as FrameworkElement;
            if (element != null)
            {
                if (element.Parent != null)
                    return element.Parent;
                //else if (element.TemplatedParent != null)
                //    return element.TemplatedParent;
            }
            //FrameworkContentElement element2 = current as FrameworkContentElement;
            //if (element2 != null)
            //{
            //    return element2.Parent;
            //}
            return null;
        }

        /// <summary>
        /// TopLeft of the rect
        /// </summary>
        /// <param name="rect">Rect</param>
        /// <returns>TopLeft Point</returns>
        public static Point TopLeft(Rect rect)
        {
            return new Point(rect.Left, rect.Top);
        }


        /// <summary>
        /// TopRight of the rect
        /// </summary>
        /// <param name="rect">Rect</param>
        /// <returns>TopRight Point</returns>
        public static Point TopRight(Rect rect)
        {
            return new Point(rect.Right, rect.Top);
        }


        /// <summary>
        /// BottomLeft of the rect
        /// </summary>
        /// <param name="rect">Rect</param>
        /// <returns>BottomLeft Point</returns>
        public static Point BottomLeft(Rect rect)
        {
            return new Point(rect.Left, rect.Bottom);
        }

        /// <summary>
        /// BottomRight of the rect
        /// </summary>
        /// <param name="rect">Rect</param>
        /// <returns>BottomRight Point</returns>
        public static Point BottomRight(Rect rect)
        {
            return new Point(rect.Right, rect.Bottom);
        }

        /// <summary>
        /// Size of rect
        /// </summary>
        /// <param name="rect">Rect</param>
        /// <returns>Size</returns>        
        public static Size GetSize(Rect rect)
        {
            return new Size(rect.Width, rect.Height);
        }
    }
}
