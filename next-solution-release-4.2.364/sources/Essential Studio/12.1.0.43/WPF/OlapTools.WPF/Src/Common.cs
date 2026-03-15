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
using System.Windows.Media;

namespace Syncfusion.Windows.Shared.Olap
{
    public class Common
    {
        public static T GetParentWindow<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject dependencyObject = VisualTreeHelper.GetParent(child);
            if (dependencyObject != null)
            {
                T parent = dependencyObject as T;
                if (parent != null)
                {
                    return parent;
                }
                else
                {
                    return GetParentWindow<T>(dependencyObject);
                }
            }
            else
            {
                return null;
            }
        }

        public static List<T> GetChildItem<T>(DependencyObject parent) where T : DependencyObject
        {
            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            List<T> list = new List<T>();
            for (int i = 0; i < childrenCount; i++)
            {
                DependencyObject dependencyObj = VisualTreeHelper.GetChild(parent, i);
                T child = dependencyObj as T;
                if (child != null)
                {
                    list.Add(child);
                }
                else
                {
                    List<T> newChild = GetChildItem<T>(dependencyObj);
                    if (newChild != null)
                    {
                        list.AddRange(newChild);
                    }
                }
            }
            return list;
        }
    }
}
