#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Collections.Generic;

#if !WinRT
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
namespace Syncfusion.Windows
#else
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.Devices.Input;

namespace Syncfusion.WinRT
#endif
{
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public static class DependencyObjectExtensions
    {
        public static object TryFindResource(this FrameworkElement element, string key)
        {
            FrameworkElement el = element;
            object item = null;
            while (el != null && (el.Parent != null) || ((el != null) && !(el.Parent is FrameworkElement)))
            {
#if WinRT
                if (el.Resources.ContainsKey(key))
#else
                if (el.Resources.Contains(key))
#endif
                {
                    item = el.Resources[key];
                    break;
                }
                var parent = el.Parent == null ? (FrameworkElement)VisualTreeHelper.GetParent(el) : (FrameworkElement)el.Parent;
                el = parent;
            }

            return item;
        }

        /// <summary>
        /// Searches the subtree of an element (including that element) 
        /// for an element of a particluar type.
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
        public static T FindElementOfType<T>(this FrameworkElement element) where T : class
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

        public static Point PointFromRootVisual(this FrameworkElement element)
        {
#if !WinRT
            GeneralTransform objGeneralTransform = element.TransformToVisual(Application.Current.RootVisual as UIElement);
            Point point = objGeneralTransform.Transform(new Point(0, 0));
            return point;
#else
            return new Point();
#endif
        }

        private static readonly DependencyProperty MousePositionProperty = DependencyProperty.RegisterAttached(
            "MousePosition",
            typeof(Point),
            typeof(DependencyObjectExtensions),
            new PropertyMetadata(null));

        public static Point GetMousePosition(DependencyObject dpo)
        {
            return (Point)dpo.GetValue(DependencyObjectExtensions.MousePositionProperty);
        }
        private static readonly DependencyProperty EnableMousePositionProperty = DependencyProperty.RegisterAttached(
            "EnableMousePosition",
            typeof(bool),
            typeof(DependencyObjectExtensions),
#if !WinRT
            new PropertyMetadata(OnEnableMousePositionChanged));
#else
 new PropertyMetadata(true, OnEnableMousePositionChanged));
#endif

        public static bool GetEnableMousePosition(DependencyObject dpo)
        {
            return (bool)dpo.GetValue(DependencyObjectExtensions.EnableMousePositionProperty);
        }

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
#if !WinRT
                frameworkEl.MouseMove += new MouseEventHandler(frameworkEl_MouseMove);
#else
                frameworkEl.PointerMoved += frameworkEl_PointerMoved;
#endif
            }
            else
            {
#if !WinRT
                frameworkEl.MouseMove -= new MouseEventHandler(frameworkEl_MouseMove);
#else
                frameworkEl.PointerMoved -= frameworkEl_PointerMoved;
#endif
            }
        }
#if WinRT
        static void frameworkEl_PointerMoved(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var framworkEl = sender as FrameworkElement;
            var point = e.GetCurrentPoint(framworkEl);
            framworkEl.SetValue(DependencyObjectExtensions.MousePositionProperty, point);
        }
#else
        private static void frameworkEl_MouseMove(object sender, MouseEventArgs e)
        {
            var framworkEl = sender as FrameworkElement;
            var point = e.GetPosition(framworkEl);
            framworkEl.SetValue(DependencyObjectExtensions.MousePositionProperty, point);
        }
#endif
    }
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public static class PointExtensions
    {
        public static void Offset(this Point pt, double offsetX, double offsetY)
        {
            pt.X += offsetX;
            pt.Y += offsetY;
        }

        public static bool IntersectsWith(this Rect rec1, Rect rect2)
        {
            if (rec1.IsEmpty || rect2.IsEmpty)
            {
                return false;
            }

            return ((((rect2.Left <= rec1.Right) && (rect2.Right >= rec1.Left)) && (rect2.Top <= rec1.Bottom)) && (rect2.Bottom >= rec1.Top));
        }

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

}
