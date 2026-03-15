#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Gantt
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;
    using System.Windows.Controls.Primitives;
    using System.Windows.Controls;

    public static class DependencyObjectExtensions
    {
#if !SILVERLIGHT
        /// <summary>
        /// Checks if the node is present in the VisualTree.
        /// </summary>
        /// <param name="dpo"></param>
        /// <param name="node"></param>
        public static bool IsDescendant(this DependencyObject dpo, DependencyObject node)
        {
            while (node != null)
            {
                if (dpo == node)
                    return true;
                FrameworkElement root = (FrameworkElement)node;
                if (root.GetType().Name == "PopupRoot")
                {
                    Popup parent = root.Parent as Popup;
                    node = parent;
                    if (parent != null)
                    {
                        node = parent.Parent;
                        if (node == null)
                        {
                            node = parent.PlacementTarget;
                        }
                    }
                }
                else
                {
                    node = node.Parent();
                }
            }
            return false;
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


        /// <summary>
        ///     Returns the template element of the given name within the Control.
        /// </summary>
        public static T FindName<T>(this Control control, string name) where T : FrameworkElement
        {
            ControlTemplate template = control.Template;
            if (template != null)
            {
                return template.FindName(name, control) as T;
            }

            return null;
        }

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
#endif

#if SILVERLIGHT
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
#endif
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
                for (int i = 0; i < numChildren; i++)
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
                for (int i = 0; i < numChildren; i++)
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
    }
}
