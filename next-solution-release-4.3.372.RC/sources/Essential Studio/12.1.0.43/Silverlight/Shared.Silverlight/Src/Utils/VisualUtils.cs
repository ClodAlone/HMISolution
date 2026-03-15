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
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public static class VisualUtil
    {
        /// <summary>
        /// Finds the ancestor.
        /// </summary>
        /// <param name="startingfrom">The startingfrom.</param>
        /// <param name="ancestortype">The ancestortype.</param>
        /// <returns></returns>
        public static DependencyObject FindAncestor(DependencyObject startingfrom, Type ancestortype)
        {
            var item = VisualTreeHelper.GetParent(startingfrom);

            while (item != null && !(item.GetType() == ancestortype))
            {
                if (item is DependencyObject)
                {
                    item = VisualTreeHelper.GetParent(item);
                }
                else
                {
                    break;
                }
            }
            return item as DependencyObject;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="startingFrom"></param>
        /// <param name="typeDescendant"></param>
        /// <returns></returns>
        public static DependencyObject FindDescendant(DependencyObject startingFrom, Type typeDescendant)
        {
            DependencyObject visual = null;
            bool result = false;
            int iCount = VisualTreeHelper.GetChildrenCount(startingFrom);

            for (int i = 0; i < iCount; ++i)
            {
                DependencyObject child = VisualTreeHelper.GetChild(startingFrom, i) as DependencyObject;

                if (typeDescendant.IsInstanceOfType(child))
                {
                    visual = child;
                    result = true;
                }

                if (!result)
                {
                    if (child != null)
                    {
                        visual = FindDescendant(child, typeDescendant);

                        if (visual != null)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    break;
                }
            }

            return visual;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static IEnumerable<DependencyObject> GetVisualDescendants(this DependencyObject element)
        {
            int childCount = VisualTreeHelper.GetChildrenCount(element);
            for (int i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(element, i);
                yield return child;
                foreach (var descendant in child.GetVisualDescendants())
                {
                    yield return descendant;
                }
            }
        }

        /// <summary>
        /// Finds a parent of a given item on the visual tree.
        /// </summary>
        /// <typeparam name="T">The type of the queried item.</typeparam>
        /// <param name="child">A direct or indirect child of the queried item.</param>
        /// <returns>The first parent item that matches the submitted type parameter. 
        /// If not matching item can be found, a null reference is being returned.</returns>
        public static T FindVisualParent<T>(DependencyObject child)
          where T : DependencyObject
        {
            // get parent item
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            // we’ve reached the end of the tree
            if (parentObject == null) return null;

            // check if the parent matches the type we’re looking for
            T parent = parentObject as T;
            if (parent != null)
            {
                return parent;
            }
            else
            {
                // use recursion to proceed with next level
                return FindVisualParent<T>(parentObject);
            }
        }
    }
}
