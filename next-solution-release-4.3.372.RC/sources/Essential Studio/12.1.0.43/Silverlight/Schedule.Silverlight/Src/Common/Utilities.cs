#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.Generic;
using System.Threading;
using System.Collections;
using System.Windows.Media.Imaging;

namespace Syncfusion.Windows.Controls.Schedule
{
    internal static class Utilities
    {
        public static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, false);
        }

        public static string GetEnumValueName<T>(T enumValue)
        {
            return Enum.GetName(enumValue.GetType(), enumValue);
        }

        public static void SetupButton<T>(ref T button, DependencyObject depObj, RoutedEventHandler routedEventHandler) where T : ButtonBase
        {
            if (button != null)
            {
                button.Click -= routedEventHandler;
            }
            button = depObj as T;
            if (button != null && routedEventHandler != null)
            {
                button.Click += routedEventHandler;
            }
        }

        public static T RetrieveVisualParentByType<T>(DependencyObject depObj) where T : class
        {
            T interFace = null;
            do
            {
                depObj = VisualTreeHelper.GetParent(depObj);
                interFace = depObj as T;
            }
            while (interFace == null && depObj != null);
            return interFace;
        }

        //public static IAppointmentEditor RetrieveAppEditor(DependencyObject depObj)
        //{
        //    return RetrieveVisualParentByType<IAppointmentEditor>(depObj);
        //}
    }

    internal interface ISupportDoubleClick
    {
        bool HandleMouseLeftButtonDoubleClick(MouseButtonEventArgs e);
    }

    internal class DoubleClickHandler
    {
        private ISupportDoubleClick owner;
        internal bool appointmentscontrolcondition;

        public DoubleClickHandler(ISupportDoubleClick owner, bool autoHandle, bool condition)
        {
            this.owner = owner;
            this.appointmentscontrolcondition = condition;
            if (autoHandle)
            {
                this.Attach();
            }
        }

        public void Attach()
        {
            if (this.owner != null)
            {
                UIElement element = this.owner as UIElement;

                if (element != null)
                {
                    element.MouseLeftButtonDown += new MouseButtonEventHandler(this.OnMouseLeftButtonDown);
                }
            }
        }

        public void Detach()
        {
            if (this.owner != null)
            {
                UIElement element = this.owner as UIElement;

                if (element != null)
                {
                    element.MouseLeftButtonDown -= new MouseButtonEventHandler(this.OnMouseLeftButtonDown);
                }
            }
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender == this.owner && this.appointmentscontrolcondition)
            {
                this.HandleMouseLeftButtonDoubleClick(e);
            }
        }

        public bool HandleMouseLeftButtonDoubleClick(MouseButtonEventArgs e)
        {
            bool handled = false;
            if (this.owner != null && this.appointmentscontrolcondition)
            {
                handled = this.owner.HandleMouseLeftButtonDoubleClick(e);
            }

            return handled;
        }
    }

    /// <summary>
    ///  class that holds enumerable extensions
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        ///  Method to yield IEnumerable from IList
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        public static IEnumerable<T> ToTypedList<T>(this IList source)
        {
            foreach (var item in source)
            {
                yield return (T)item;
            }
        }
    }

    /// <summary>
    ///  class that holds dependency object extensions
    /// </summary>
    public static class DependencyObjectExtensions
    {
        /// <summary>
        ///  method to find the resource using the key value
        /// </summary>
        /// <param name="element"></param>
        /// <param name="key"></param>
        public static object TryFindResource(this FrameworkElement element, string key)
        {
            FrameworkElement el = element;
            object item = null;
            while (el != null && (el.Parent != null) || ((el != null) && !(el.Parent is FrameworkElement)))
            {
                if (el.Resources[key] != null)
                {
                    item = el.Resources[key];
                    break;
                }
                el = el.Parent as FrameworkElement;
            }

            return item;
        }

        /// <summary>
        ///     Searches the subtree of an element (including that element) 
        ///     for an element of a particluar type.
        /// </summary>
        public static IEnumerable<T> FindElementsOfType<T>(this FrameworkElement element) where T : class
        {
            T correctlyTyped = element as T;
            if (correctlyTyped != null)
            {
                yield return correctlyTyped;
            }

            if (element != null)
            {
                int numChildren = VisualTreeHelper.GetChildrenCount(element);
                for (int i = 0;i < numChildren;i++)
                {
                    var children = FindElementsOfType<T>(VisualTreeHelper.GetChild(element, i) as FrameworkElement);
                    foreach (var child in children)
                    {
                        yield return child;
                    }
                }

                // Popups continue in another window, jump to that tree
                Popup popup = element as Popup;
                if (popup != null)
                {
                    var popupChildren = FindElementsOfType<T>(popup.Child as FrameworkElement);
                    foreach (var child in popupChildren)
                    {
                        yield return child;
                    }
                }
            }

            yield return null;
        }


        /// <summary>
        ///     Searches the subtree of an element (including that element) 
        ///     for an element of a particluar type.
        /// </summary>
        public static T FindElementOfType<T>(this FrameworkElement element) where T : FrameworkElement
        {
            T correctlyTyped = element as T;
            if (correctlyTyped != null)
            {
                return correctlyTyped;
            }

            if (element != null)
            {
                int numChildren = VisualTreeHelper.GetChildrenCount(element);
                for (int i = 0;i < numChildren;i++)
                {
                    T child = FindElementOfType<T>(VisualTreeHelper.GetChild(element, i) as FrameworkElement);
                    if (child != null)
                    {
                        return child;
                    }
                }

                // Popups continue in another window, jump to that tree
                Popup popup = element as Popup;
                if (popup != null)
                {
                    return FindElementOfType<T>(popup.Child as FrameworkElement);
                }
            }

            return null;
        }

        /// <summary>
        /// Finds the type of the parent element of T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static T FindParentElementOfType<T>(this FrameworkElement element)
    where T : class
        {
            T correctlyTyped = element as T;
            if (correctlyTyped != null)
            {
                return correctlyTyped;
            }

            if (element != null)
            {
                var parent = element.Parent == null ? VisualTreeHelper.GetParent(element) : element.Parent;
                T p1 = FindParentElementOfType<T>(parent as FrameworkElement);
                if (p1 != null)
                {
                    return p1;
                }
            }
            return null;
        }

        /// <summary>
        ///  method to transform the points to root visuals
        /// </summary>
        /// <param name="element"></param>
        public static Point PointFromRootVisual(this FrameworkElement element)
        {
#if SILVERLIGHT
            GeneralTransform objGeneralTransform = element.TransformToVisual(Application.Current.RootVisual as UIElement);
#else
            GeneralTransform objGeneralTransform = element.TransformToVisual(Application.Current.MainWindow as UIElement);
#endif
            Point point = objGeneralTransform.Transform(new Point(0, 0));
            return point;
        }

        private static readonly DependencyProperty MousePositionProperty = DependencyProperty.RegisterAttached(
            "MousePosition",
            typeof(Point),
            typeof(DependencyObjectExtensions),
            new PropertyMetadata(null));

        /// <summary>
        ///  this method gets the mouse position
        /// </summary>
        /// <param name="dpo"></param>
        public static Point GetMousePosition(DependencyObject dpo)
        {
            return (Point)dpo.GetValue(DependencyObjectExtensions.MousePositionProperty);
        }

        private static readonly DependencyProperty EnableMousePositionProperty = DependencyProperty.RegisterAttached(
            "EnableMousePosition",
            typeof(bool),
            typeof(DependencyObjectExtensions),
            new PropertyMetadata(OnEnableMousePositionChanged));

        /// <summary>
        /// this bool method return whether the mouse position is enable or not
        /// </summary>
        /// <param name="dpo"></param>
        public static bool GetEnableMousePosition(DependencyObject dpo)
        {
            return (bool)dpo.GetValue(DependencyObjectExtensions.EnableMousePositionProperty);
        }

        /// <summary>
        ///  this method set the mouse enable position and true while enabled
        /// </summary>
        /// <param name="dpo"></param>
        /// <param name="value">If set to <see langword="true"/>, then ; otherwise,
        /// .</param>
        public static void SetEnableMousePosition(DependencyObject dpo, bool value)
        {
            dpo.SetValue(DependencyObjectExtensions.EnableMousePositionProperty, value);
        }

        private static void OnEnableMousePositionChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var frameworkEl = dpo as FrameworkElement;
            if (frameworkEl == null)
            {
                throw new InvalidOperationException("Object has to be a FrameworkElement");
            }

            var value = (bool)args.NewValue;
            if (value)
            {
                frameworkEl.MouseMove += new MouseEventHandler(frameworkEl_MouseMove);
            }
            else
            {
                frameworkEl.MouseMove -= new MouseEventHandler(frameworkEl_MouseMove);
            }
        }

        private static void frameworkEl_MouseMove(object sender, MouseEventArgs e)
        {
            var framworkEl = sender as FrameworkElement;
            var point = e.GetPosition(framworkEl);
            framworkEl.SetValue(DependencyObjectExtensions.MousePositionProperty, point);
        }
    }

    /// <summary>
    ///  this class holds the offset point extensions
    /// </summary>
    public static class PointExtensions
    {
        /// <summary>
        ///  tis method calculate the point through the offset value
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="offsetX"></param>
        /// <param name="offsetY"></param>
        public static void Offset(this Point pt, double offsetX, double offsetY)
        {
            pt.X += offsetX;
            pt.Y += offsetY;
        }

        /// <summary>
        ///  this method returns whether the width intersect or not
        /// </summary>
        /// <param name="rec1"></param>
        /// <param name="rect2"></param>
        public static bool IntersectsWith(this Rect rec1, Rect rect2)
        {
            if (rec1.IsEmpty || rect2.IsEmpty)
            {
                return false;
            }

            return ((((rect2.Left <= rec1.Right) && (rect2.Right >= rec1.Left)) && (rect2.Top <= rec1.Bottom)) && (rect2.Bottom >= rec1.Top));
        }

        /// <summary>
        ///  this method calculates the value for the rectangle
        /// </summary>
        /// <param name="rect1"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public static void Inflate(this Rect rect1, double width, double height)
        {
            if (rect1.IsEmpty)
            {
                throw new InvalidOperationException("Rect is empty");
            }
            rect1.X -= width;
            rect1.Y -= height;
            rect1.Width += width;
            rect1.Width += width;
            rect1.Height += height;
            rect1.Height += height;
            if ((rect1.Width < 0.0) || (rect1.Height < 0.0))
            {
                rect1 = Rect.Empty;
            }
        }
    }

    /// <summary>
    ///  class that holds writable bitMap
    /// </summary>
    public class LineDrawWritableBitMap
    {
        #region DrawLine      

        /// <summary>
        /// Draws a colored line by connecting two points using an optimized DDA.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="x1">The x-coordinate of the start point.</param>
        /// <param name="y1">The y-coordinate of the start point.</param>
        /// <param name="x2">The x-coordinate of the end point.</param>
        /// <param name="y2">The y-coordinate of the end point.</param>
        /// <param name="color">The color for the line.</param>
        public void DrawLine(WriteableBitmap bmp, int x1, int y1, int x2, int y2, Color color)
        {  
#if SILVERLIGHT
            this.DrawLine(bmp.Pixels, bmp.PixelWidth, bmp.PixelHeight, x1, y1, x2, y2, (color.A << 24) | (color.R << 16) | (color.G << 8) | color.B);
#endif
        }       

        /// <summary>
        /// Draws a colored line by connecting two points using an optimized DDA. 
        /// Uses the pixels array and the width directly for best performance.
        /// </summary>
        /// <param name="pixels">An array containing the pixels as int RGBA value.</param>
        /// <param name="pixelWidth">The width of one scanline in the pixels array.</param>
        /// <param name="pixelHeight">The height of the bitmap.</param>
        /// <param name="x1">The x-coordinate of the start point.</param>
        /// <param name="y1">The y-coordinate of the start point.</param>
        /// <param name="x2">The x-coordinate of the end point.</param>
        /// <param name="y2">The y-coordinate of the end point.</param>
        /// <param name="color">The color for the line.</param>
        public void DrawLine(int[] pixels, int pixelWidth, int pixelHeight, int x1, int y1, int x2, int y2, int color)
        {
            // Check boundaries
            if (x1 < 0) { x1 = 0; }
            if (y1 < 0) { y1 = 0; }
            if (x2 < 0) { x2 = 0; }
            if (y2 < 0) { y2 = 0; }
            if (x1 >= pixelWidth) { x1 = pixelWidth - 1; }
            if (y1 >= pixelHeight) { y1 = pixelHeight - 1; }
            if (x2 >= pixelWidth) { x2 = pixelWidth - 1; }
            if (y2 >= pixelHeight) { y2 = pixelHeight - 1; }

            // Distance start and end point
            int dx = x2 - x1;
            int dy = y2 - y1;

            const int PRECISION_SHIFT = 4;
            const int PRECISION_VALUE = 1 << PRECISION_SHIFT;

            // Determine slope (absoulte value)
            int lenX, lenY;
            int incy1;
            if (dy >= 0)
            {
                incy1 = PRECISION_VALUE;
                lenY = dy;
            }
            else
            {
                incy1 = -PRECISION_VALUE;
                lenY = -dy;
            }

            int incx1;
            if (dx >= 0)
            {
                incx1 = 1;
                lenX = dx;
            }
            else
            {
                incx1 = -1;
                lenX = -dx;
            }

            if (lenX > lenY)
            { // x increases by +/- 1
                // Init steps and start
                int incy = (dy << PRECISION_SHIFT) / lenX;
                int y = y1 << PRECISION_SHIFT;

                // Walk the line!
                for (int i = 0; i < lenX; i++)
                {
                    pixels[(y >> PRECISION_SHIFT) * pixelWidth + x1] = color;
                    x1 += incx1;
                    y += incy;
                }
            }
            else
            { // since y increases by +/-1, we can safely add (*h) before the for() loop, since there is no fractional value for y
                // Prevent divison by zero
                if (lenY == 0)
                {
                    return;
                }

                // Init steps and start
                int incx = (dx << PRECISION_SHIFT) / lenY;
                int index = (x1 + y1 * pixelWidth) << PRECISION_SHIFT;

                // Walk the line!
                int inc = incy1 * pixelWidth + incx;
                for (int i = 0; i < lenY; i++)
                {
                    pixels[index >> PRECISION_SHIFT] = color;
                    index += inc;
                }
            }
        }

        #endregion
    }


}
