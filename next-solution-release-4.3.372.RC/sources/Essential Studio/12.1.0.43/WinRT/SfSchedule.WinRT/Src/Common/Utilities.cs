#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if WINRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
#else
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
#endif

namespace Syncfusion.UI.Xaml.Schedule
{
    /// <summary>
    /// Represents an extension to find parent of an element.
    /// </summary>
    public static class DependencyObjectExtensions
    {
        public static T FindParentElementOfType<T>(this FrameworkElement element) where T : class
        {
            var correctlyTyped = element as T;
            if (correctlyTyped != null)
            {
                return correctlyTyped;
            }

            if (element != null)
            {
                var parent = element.Parent ?? VisualTreeHelper.GetParent(element);
                var p1 = FindParentElementOfType<T>(parent as FrameworkElement);
                if (p1 != null)
                {
                    return p1;
                }
            }
            return null;
        }

        /// <summary>
        ///     Searches the sub tree of an element (including that element) 
        ///     for an element of a particular type.
        /// </summary>
        public static T FindElementOfType<T>(this FrameworkElement element) where T : FrameworkElement
        {
            var correctlyTyped = element as T;
            if (correctlyTyped != null)
            {
                return correctlyTyped;
            }

            if (element != null)
            {
                int numChildren = VisualTreeHelper.GetChildrenCount(element);
                for (int i = 0; i < numChildren; i++)
                {
                    var child = FindElementOfType<T>(VisualTreeHelper.GetChild(element, i) as FrameworkElement);
                    if (child != null)
                    {
                        return child;
                    }
                }

                // Popup continue in another window, jump to that tree
                var popup = element as Popup;
                if (popup != null)
                {
                    return FindElementOfType<T>(popup.Child as FrameworkElement);
                }
            }

            return null;
        }
        /// <summary>
        ///     Searches the sub tree of an element (including that element) 
        ///     for an element of a particular type.
        /// </summary>
        internal static T FindElementOfTypeWithName<T>(this FrameworkElement element, string name) where T : FrameworkElement
        {
            var correctlyTyped = element as T;
            if (correctlyTyped != null && element.Name.Contains(name))
            {
                return correctlyTyped;
            }

            if (element != null)
            {
                int numChildren = VisualTreeHelper.GetChildrenCount(element);
                for (int i = 0; i < numChildren; i++)
                {
                    var child = FindElementOfTypeWithName<T>(VisualTreeHelper.GetChild(element, i) as FrameworkElement,name);
                    if (child != null)
                    {
                        return child;
                    }
                }

                // Popup continue in another window, jump to that tree
                var popup = element as Popup;
                if (popup != null)
                {
                    return FindElementOfTypeWithName<T>(popup.Child as FrameworkElement,name);
                }
            }

            return null;
        }

    }
}
