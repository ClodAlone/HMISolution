#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System;
using System.Windows.Controls;
using System.Reflection;
using System.ComponentModel;

namespace Syncfusion.Windows.GridCommon
{
    /// <summary>
    /// Helper routines.
    /// </summary>
    public class GridUtil
    {
        /// <summary>
        /// Gets the pressed mouse button.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        public static MouseButton? GetMouseButton(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                return MouseButton.Left;
            else if (e.MiddleButton == MouseButtonState.Pressed)
                return MouseButton.Middle;
            else if (e.RightButton == MouseButtonState.Pressed)
                return MouseButton.Right;
            else if (e.XButton1 == MouseButtonState.Pressed)
                return MouseButton.XButton1;
            else if (e.XButton2 == MouseButtonState.Pressed)
                return MouseButton.XButton2;

            return null;
        }

        /// <summary>
        /// Gets the pressed mouse button.
        /// </summary>
        /// <returns></returns>
        public static MouseButton? GetMouseButton()
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
                return MouseButton.Left;
            else if (Mouse.MiddleButton == MouseButtonState.Pressed)
                return MouseButton.Middle;
            else if (Mouse.RightButton == MouseButtonState.Pressed)
                return MouseButton.Right;
            else if (Mouse.XButton1 == MouseButtonState.Pressed)
                return MouseButton.XButton1;
            else if (Mouse.XButton2 == MouseButtonState.Pressed)
                return MouseButton.XButton2;

            return null;
        }


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
        /// Pushes the clip.
        /// </summary>
        /// <param name="dc">The dc.</param>
        /// <param name="clipRect">The clip rect.</param>
        public static void PushClip(DrawingContext dc, Rect clipRect)
        {
            RectangleGeometry rg = new RectangleGeometry(clipRect);
            rg.Freeze();
            dc.PushClip(rg);
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

        /// <summary>
        /// Sets the canvas Left, Top, Right and Bottom attached properties.
        /// </summary>
        /// <param name="el">The element for which to set the attached properties.</param>
        /// <param name="rect">The rect.</param>
        public static void SetCanvasBounds(UIElement el, Rect rect)
        {
            Canvas.SetLeft(el, rect.Left);
            Canvas.SetTop(el, rect.Top);
            Canvas.SetRight(el, rect.Right);
            Canvas.SetBottom(el, rect.Bottom);
        }

        /// <summary>
        /// Sets the clip rect.
        /// </summary>
        /// <param name="el">The element.</param>
        /// <param name="r">The clip recangle.</param>
        /// <returns></returns>
        public static RectangleGeometry SetClipRect(UIElement el, Rect r)
        {
            RectangleGeometry oldClip = el.Clip as RectangleGeometry;

            if (oldClip != null && oldClip.Rect == r)
                return null;

            RectangleGeometry clip = new RectangleGeometry(r);
            clip.Freeze();
            el.Clip = clip;
            return oldClip;
        }

        /// <summary>
        /// Sets the clip rect.
        /// </summary>
        /// <param name="dv">The DrawingVisual.</param>
        /// <param name="r">The clip recangle.</param>
        /// <returns></returns>
        public static RectangleGeometry SetClipRect(DrawingVisual dv, Rect r)
        {
            RectangleGeometry oldClip = dv.Clip as RectangleGeometry;

            if (oldClip != null && oldClip.Rect == r)
                return null;

            RectangleGeometry clip = new RectangleGeometry(r);
            clip.Freeze();
            dv.Clip = clip;
            return oldClip;
        }

        static FieldInfo _clientAreaAnimationField;

        /// <summary>
        /// Gets or sets a value indicating whether WPF controls are animated. See also <see cref="SystemParameters.ClientAreaAnimation"/>.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if WPF controls are animated; otherwise, <c>false</c>.
        /// </value>
        public static bool IsAnimated
        {
            get { return SystemParameters.ClientAreaAnimation; }
            set
            {
                if (value != IsAnimated)
                {
                    if (_clientAreaAnimationField == null)
                        _clientAreaAnimationField = typeof(SystemParameters).GetField("_clientAreaAnimation", BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.SetField);
                    _clientAreaAnimationField.SetValue(null, value);
                }
            }
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
            return MinMax(point, bounds.Location, new Point(bounds.Right, bounds.Bottom));
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
                if (dpo is Visual)
                    dpo = VisualTreeHelper.GetParent(dpo);
                else
                    dpo = LogicalTreeHelper.GetParent(dpo);
                if (dpo == null)
                    return defaultValue;
            }
            return dpo.GetValue(dp);
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
        public static object GetValueInherited(DependencyObject dpo, DependencyProperty dp, DependencyProperty dp2, object defaultValue)
        {
            while (dpo.ReadLocalValue(dp) == DependencyProperty.UnsetValue || dpo.ReadLocalValue(dp2) == DependencyProperty.UnsetValue)
            {
                if (dpo is Visual)
                    dpo = VisualTreeHelper.GetParent(dpo);
                else
                    dpo = LogicalTreeHelper.GetParent(dpo);
                if (dpo == null)
                    return defaultValue;
            }
            return dpo.GetValue(dp);
        }

        /// <summary>
        /// Gets the value from the specified object. If the value was not set
        /// for the object then the method will walk up parent nodes until
        /// it finds a parent object with the value set.
        /// </summary>
        /// <param name="dobj">The dependency object.</param>
        /// <param name="dp">The dependency property.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="dpo">The parent object in which the value was set.</param>
        /// <returns></returns>
        public static object GetValueInherited(DependencyObject dobj, DependencyProperty dp, object defaultValue, out DependencyObject dpo)
        {
            dpo = dobj;
            while (dpo.ReadLocalValue(dp) == DependencyProperty.UnsetValue)
            {
                if (dpo is Visual)
                    dpo = VisualTreeHelper.GetParent(dpo);
                else
                    dpo = LogicalTreeHelper.GetParent(dpo);
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
                if (obj is Visual)
                    obj = VisualTreeHelper.GetParent(obj);
                else
                    obj = LogicalTreeHelper.GetParent(obj);
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
                else if (element.TemplatedParent != null)
                    return element.TemplatedParent;
            }
            FrameworkContentElement element2 = current as FrameworkContentElement;
            if (element2 != null)
            {
                return element2.Parent;
            }
            return null;
        }

        public static T GetXamlConvertedValue<T>(string value)
        {
            var converter = TypeDescriptor.GetConverter(typeof(T));
            if (converter != null)
            {
                if (typeof(T).Equals(typeof(Brush)))
                {
                    return (T)new BrushConverter().ConvertFromInvariantString(value);
                }
                else
                {
                    return (T)converter.ConvertFromInvariantString(value);
                }
            }
            return default(T);
        }

        public static Size GetSize(Rect rect)
        {
            return rect.Size;
        }

        public static Brush GetHoverColor(Brush brush, double offset)
        {  
            if (brush is LinearGradientBrush)
            {
                var linearBrush = new LinearGradientBrush();
                foreach (var gradientStops in (brush as LinearGradientBrush).GradientStops.Clone())
                {
                    Color c = gradientStops.Color;
                    byte r = (byte)(c.R + ((255 - c.R) * offset));
                    byte g = (byte)(c.G + ((255 - c.G) * offset));
                    byte b = (byte)(c.B + ((255 - c.B) * offset));
                    gradientStops.Color = Color.FromRgb(r, g, b);
                    linearBrush.GradientStops.Add(gradientStops);
                }
                linearBrush.StartPoint = (brush as LinearGradientBrush).StartPoint;
                linearBrush.EndPoint = (brush as LinearGradientBrush).EndPoint;
                return linearBrush;
            }
            else
            {
                Color c = (brush as SolidColorBrush).Color;
                byte r = (byte)(c.R + ((255 - c.R) * offset));
                byte g = (byte)(c.G + ((255 - c.G) * offset));
                byte b = (byte)(c.B + ((255 - c.B) * offset));
                return new SolidColorBrush(Color.FromRgb(r, g, b));       
            }
            return brush;
        }

    }
}
