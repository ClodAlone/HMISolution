#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

#if WINRT_USING
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
#else
using System.Windows.Media; 
#endif

namespace Syncfusion.UI.Xaml.Diagram.Utility
{
    public static class VisualTreeExtensions
    {
        internal static T FindVisualChild_Depth<T>(this DependencyObject dobj) where T : DependencyObject
        {
            int cnt = VisualTreeHelper.GetChildrenCount(dobj);
            for (int i = 0; i < cnt; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(dobj, i);
                if (child is T)
                {
                    return child as T;
                }
                else if (child != null)
                {
                    child = child.FindVisualChild_Depth<T>();
                    if (child is T)
                    {
                        return child as T;
                    }
                }
            }
            return null;
        }

        internal static T FindVisualChild_Depth<T>(this DependencyObject dobj, string searchName) where T : FrameworkElement
        {
            int cnt = VisualTreeHelper.GetChildrenCount(dobj);
            for (int i = 0; i < cnt; i++)
            {
                FrameworkElement child = VisualTreeHelper.GetChild(dobj, i) as FrameworkElement;
                if (child is T && child.Name == searchName)
                {
                    return child as T;
                }
                else if (child != null)
                {
                    child = child.FindVisualChild_Depth<T>(searchName);
                    if (child is T && child.Name == searchName)
                    {
                        return child as T;
                    }
                }
            }
            return null;
        }

        public static T FindVisualParent<T>(this DependencyObject dobj) where T : DependencyObject
        {
            DependencyObject parent = null;
            try
            {
                if (dobj.Dispatcher != null)
                {
                    parent = (dobj as FrameworkElement).Parent ?? VisualTreeHelper.GetParent(dobj); 
                }
            }
            catch
            {

            }
            if (parent == null && dobj is FrameworkElement)
            {
                parent = (dobj as FrameworkElement).Parent;
            }
            if (parent is T)
            {
                return parent as T;
            }
            else if (parent != null)
            {
                return parent.FindVisualParent<T>();
            }
            else
            {
                return null;
            }
        }

        public static T FindVisualParent<T>(this FrameworkElement dobj, string searchName) where T : FrameworkElement
        {
            FrameworkElement parent = VisualTreeHelper.GetParent(dobj) as FrameworkElement;
            if (parent == null)
            {
                parent = dobj.Parent as FrameworkElement;
            }
            if (parent is T && dobj.Name == searchName)
            {
                return parent as T;
            }
            else if (parent != null)
            {
                return parent.FindVisualParent<T>(searchName);
            }
            else
            {
                return null;
            }
        }
    }
}
