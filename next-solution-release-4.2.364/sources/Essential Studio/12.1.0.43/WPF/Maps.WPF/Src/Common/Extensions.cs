#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Map
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;
    using System.Windows.Controls.Primitives;

    internal static class Extensions
    {
#if WPF
        /// <summary>
        /// Finds the type of the parent element of T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static T FindParentElementOfType<T>(this FrameworkElement element) where T : FrameworkElement
        {
            T correctlyTyped = element as T;
            if (correctlyTyped != null)
            {
                return correctlyTyped;
            }

            if (element != null)
            {
                var parent = element.Parent();
                T p1 = FindParentElementOfType<T>(parent as FrameworkElement);
                if (p1 != null)
                {
                    return p1;
                }
            }

            return null;
        }

        public static DependencyObject Parent(this DependencyObject o)
        {
            DependencyObject reference = o as Visual;
            if (reference == null)
            {
                reference = o as Visual3D;
            }

            ContentElement element = (reference == null) ? (o as ContentElement) : null;

            if (element != null)
            {
                o = ContentOperations.GetParent(element);
                if (o != null)
                {
                    return o;
                }

                FrameworkContentElement element2 = element as FrameworkContentElement;
                if (element2 != null)
                {
                    return element2.Parent;
                }
            }
            else if (reference != null)
            {
                var parent = VisualTreeHelper.GetParent(reference);
                if (parent != null)
                {
                    return parent;
                }

                var frameworkEl = reference as FrameworkElement;
                if (frameworkEl != null)
                {
                    return frameworkEl.Parent;
                }
            }

            return null;
        }
#else
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
#endif
    }
}
