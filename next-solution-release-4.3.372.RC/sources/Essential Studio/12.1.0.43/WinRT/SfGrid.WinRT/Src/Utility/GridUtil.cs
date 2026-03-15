#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
#if WinRT
using Windows.Foundation;
using System.Reflection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Controls;
#else
using System.Windows.Media;
using System.Windows.Controls.Primitives;
#if WPF
using System.ComponentModel;
using System.Data;
using System.Reflection;
using System.Windows.Interop;
#endif
using System.Windows;
using System.Windows.Controls;
#endif

namespace Syncfusion.UI.Xaml.Utility
{
    internal static class GridUtil
    {
        public static FrameworkElement FindDescendantByName(FrameworkElement element, string name)
        {
            if (element == null || string.IsNullOrWhiteSpace(name)) { return null; }

            if (name.Equals(element.Name, StringComparison.OrdinalIgnoreCase))
            {
                return element;
            }
            var childCount = VisualTreeHelper.GetChildrenCount(element);
            for (int i = 0; i < childCount; i++)
            {
                var child = (VisualTreeHelper.GetChild(element, i) as FrameworkElement);
                if (child == null) return null;
                var result = FindDescendantByName(child, name);
                if (result != null) { return result; }
            }
            return null;
        }

        public static DependencyObject FindDescendant(object source, Type typeDescendant)
        {
            var startingFrom = source as DependencyObject;
            var parent = VisualTreeHelper.GetParent(startingFrom);
            if (parent == null)
            {
                if (source is FrameworkElement && (source as FrameworkElement).Parent != null)
                {
                    parent = (source as FrameworkElement).Parent;
                    if (parent.GetType() == typeDescendant)
                        return parent;
                    return FindDescendant(parent, typeDescendant);
                }
                return null;
            }
            if (parent.GetType() == typeDescendant)
                return parent;
            return FindDescendant(parent, typeDescendant);
        }

        public static void Descendant(DependencyObject root, ref List<DependencyObject> list)
        {
            var count = VisualTreeHelper.GetChildrenCount(root);
            for (var i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(root, i);
                if (child != null)
                {
                    if (child is Popup)
                    {
                        child = (child as Popup).Child;
                        if (child == null)
                            continue;
                    }
                    list.Add(child);
                    Descendant(child, ref list);
                }
            }
        }

        /// <summary>
        /// Generic method to get the navigatable child Controls
        /// </summary>
        public static void GetNavigatableDescendants(UIElement uiElement, ref List<Control> list)
        {
            List<DependencyObject> childrens = new List<DependencyObject>();

            if (uiElement != null)
            {
                GridUtil.Descendant(uiElement, ref childrens);
                foreach (DependencyObject item in childrens)
                {
                    var controlItem = item as Control;
                    if (controlItem != null && controlItem.IsTabStop && controlItem.Visibility == Visibility.Visible)
                        list.Add(controlItem);
                }
            }
        }

        /// <summary>
        /// Generic method to get the child object
        /// </summary>
        public static T GetChildObject<T>(DependencyObject obj, string name) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject c = VisualTreeHelper.GetChild(obj, i);
                if (c.GetType().FullName == typeof(T).FullName && (String.IsNullOrEmpty(name) || ((FrameworkElement)c).Name == name))
                {
                    return (T)c;
                }

                object gc = GetChildObject<T>(c as DependencyObject, name);
                if (gc != null)
                    return (T)gc;
            }
            return null;
        }

        public static Rect FromLTRB(double left, double top, double right, double bottom)
        {
            if (right < left)
                return FromLTRB(right, top, left, bottom);
            if (bottom < top)
                return FromLTRB(left, bottom, right, top);
            return double.IsInfinity(left) ? Rect.Empty : new Rect(left, top, right - left, bottom - top);
        }

        /// <summary>
        /// Indicates whether the specified Type has nested properties.
        /// </summary>
        /// <param name="t">The Type to be checked.</param>
        /// <returns>True if nested properties are found; False otherwise.</returns>
        public static bool IsComplexType(Type t)
        {
            Type underlyingType = t == null ? null : Nullable.GetUnderlyingType(t);
            if (underlyingType != null)
                t = underlyingType;

            if (t != typeof(object)
                && t != typeof(Decimal)
                && t != typeof(DateTime)
                && t != typeof(Type)
                && t != typeof(TimeSpan)
                //&& t != typeof(System.Drawing.Color)
                && t != typeof(string)
                && t != typeof(Guid)
#if !WinRT
                && t.BaseType != typeof(Enum)
                && !t.IsPrimitive
#else
                && !t.GetTypeInfo().IsPrimitive
#endif
                )
                return true;

            return false;
        }

#if WPF
        /// <summary>
        /// Returns the PropertyDescriptorCollection for the relation or nested collection.
        /// </summary>
        /// <param name="pd"></param>
        /// <returns></returns>
        public static DataRelation GetDataRelation(PropertyDescriptor pd)
        {
            if (!BrowserInteropHelper.IsBrowserHosted)
            {
                if (pd != null && pd.GetType().FullName == "System.Data.DataRelationPropertyDescriptor")
                {
                    var t = pd.GetType();
                    var pi = t.GetProperty("Relation", BindingFlags.GetProperty
                                                       | BindingFlags.Public | BindingFlags.IgnoreReturn
                                                       | BindingFlags.Instance | BindingFlags.NonPublic);
                    if (pi != null)
                    {
                        var mInfo = pi.GetGetMethod(true);
                        if (mInfo != null)
                        {
                            var dr = mInfo.Invoke(pd, new object[0]) as DataRelation;
                            return dr;
                        }
                    }
                }
            }
            return null;
        }
#endif
    }
}
