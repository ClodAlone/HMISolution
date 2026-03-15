// <copyright file="VisualUtils.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using System.Windows;
using System.Reflection;
using System.Windows.Controls.Primitives;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Controls;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// Class that stores static methods that operate on visuals.
    /// </summary>
    public sealed class VisualUtils
    {
        #region Constants
        /// <summary>
        /// Represents the string that contains full name of root popup type name.
        /// </summary>
        private const string RootPopupTypeName = "System.Windows.Controls.Primitives.PopupRoot";
        #endregion

        #region Private member
        /// <summary>
        /// This member contains framework internal PopupRoot type.
        /// </summary>
        public static Type RootPopupType = null;
        #endregion

        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="VisualUtils"/> class.
        /// </summary>
        static VisualUtils()
        {
            Assembly assembly = Assembly.GetAssembly(typeof(FrameworkElement));
            string assemblyQualifiedName = Assembly.CreateQualifiedName(assembly.FullName, RootPopupTypeName);
            RootPopupType = Type.GetType(assemblyQualifiedName);
        }

        /// <summary>
        /// Prevents a default instance of the VisualUtils class from being created.
        /// </summary>
        private VisualUtils()
        {
        }
        #endregion

        #region Static Methods

       
        /// <summary>
        /// Looks for the root visual starting from the specified one.
        /// </summary>
        /// <param name="startingFrom">Specified <see cref="Visual"/> element from which search for root visual starts.</param>
        /// <returns><see cref="Visual"/> that represents root visual.</returns>
        public static Visual FindRootVisual(Visual startingFrom)
        {
            Visual root = null;

            if (startingFrom != null)
            {
                root = startingFrom;

                while ((startingFrom = VisualTreeHelper.GetParent(startingFrom) as Visual) != null)
                {
                    root = startingFrom;
                }
            }

            return root;
        }

        /// <summary>
        /// Looks for the visual ancestor of the specified type.
        /// </summary>
        /// <param name="startingFrom"><see cref="Visual"/> the search is started from.</param>
        /// <param name="typeAncestor">Desired type of the ancestor.</param>
        /// <returns>
        /// <see cref="Visual"/> of the specified type, or null if no ancestors of the specified type were found.
        /// </returns>
        public static Visual FindAncestor(Visual startingFrom, Type typeAncestor)
        {
            if (startingFrom != null)
            {
                DependencyObject parent = VisualTreeHelper.GetParent(startingFrom);

                while (parent != null && !typeAncestor.IsInstanceOfType(parent))
                {
                    parent = VisualTreeHelper.GetParent(parent);
                }

                return parent as Visual;
            }

            return null;
        }

        /// <summary>
        /// Finds the logical ancestor.
        /// </summary>
        /// <param name="startingFrom">The starting from.</param>
        /// <param name="typeAncestor">The type ancestor.</param>
        /// <returns></returns>
        public static Visual FindLogicalAncestor(Visual startingFrom, Type typeAncestor)
        {
            if (startingFrom != null)
            {
                DependencyObject parent = LogicalTreeHelper.GetParent(startingFrom);

                while (parent != null && !typeAncestor.IsInstanceOfType(parent))
                {
                    parent = LogicalTreeHelper.GetParent(parent);
                }

                return parent as Visual;
            }

            return null;
        }

        /// <summary>
        /// Looks for the visual Descendant of the specified type.
        /// </summary>
        /// <param name="startingFrom"><see cref="Visual"/> the search is started from.</param>
        /// <param name="typeDescendant">Desired type of the Descendant element.</param>
        /// <returns>
        /// <see cref="Visual"/> of the specified type, or null if no Descendants of the specified type were found.
        /// </returns>
        public static Visual FindDescendant(Visual startingFrom, Type typeDescendant)
        {
            if (startingFrom == null)
                return null;

            Visual visual = null;
            bool result = false;
            int iCount = VisualTreeHelper.GetChildrenCount(startingFrom);

            for (int i = 0; i < iCount; ++i)
            {
                Visual child = VisualTreeHelper.GetChild(startingFrom, i) as Visual;
               
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
        /// Enumerates all visuals in the target visual tree that are instances of the specified type.
        /// </summary>
        /// <param name="rootelement">Target element to search in.</param>
        /// <param name="typeChild">Type of the children to find.</param>
        /// <returns>
        /// Enumerator with children of the specified type.
        /// </returns>
        /// <remarks>
        /// Enumeration of children that is done through the iterate
        /// </remarks>
        public static IEnumerable<Visual> EnumChildrenOfType(Visual rootelement, Type typeChild)
        {
            if (rootelement != null)
            {
                int iCount = VisualTreeHelper.GetChildrenCount(rootelement);

                for (int i = 0; i < iCount; i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(rootelement, i);

                    if (child is Visual)
                    {
                        Visual visual = (Visual)child;

                        if (typeChild.IsInstanceOfType(visual))
                        {
                            yield return visual;
                        }

                        foreach (Visual vis in EnumChildrenOfType(visual, typeChild))
                        {
                            yield return vis;
                        }
                    }
                }
            }
            else
            {
                throw new ArgumentNullException("rootelement");
            }
        }

        /// <summary>
        /// Gets the items panel of corresponding items control.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="panelType">Type of the panel.</param>
        /// <returns></returns>
        public static Panel GetItemsPanel(ItemsControl owner, Type panelType)
        {
            Panel panel = null;

            if (owner != null && panelType != null)
            {
                Panel currentPanel = null;

                foreach (Visual visual in VisualUtils.EnumChildrenOfType(owner, panelType))
                {
                    currentPanel = visual as Panel;

                    if (currentPanel != null
                        && VisualUtils.GetItemsControlFromChildren(currentPanel) == owner)
                    {
                        panel = currentPanel;
                        break;
                    }
                }
            }

            return panel;
        }

        /// <summary>
        /// Finds parent items control for given element.
        /// </summary>
        public static ItemsControl GetItemsControlFromChildren(FrameworkElement element)
        {
            ItemsControl item = null;

            if (element != null)
            {
                item = element as ItemsControl;

                if (item == null)
                {
                    while (element != null)
                    {
                        element = VisualTreeHelper.GetParent(element) as FrameworkElement;

                        if (element is ItemsControl)
                        {
                            item = (ItemsControl)element;
                            break;
                        }
                    }
                }
            }

            return item;
        }

        /// <summary>
        /// Enumerates all instances in the target element logical tree that are instances of the specified type.
        /// </summary>
        /// <param name="rootelement">Target element to search in.</param>
        /// <param name="typeChild">Type of the children to find.</param>
        /// <returns>Enumerator with children of the specified type.</returns>
        /// <remarks>
        /// Enumeration of children that is done through the iterate
        /// </remarks>
        public static IEnumerable<DependencyObject> EnumLogicalChildrenOfType(DependencyObject rootelement, Type typeChild)
        {
            foreach (Object obj in LogicalTreeHelper.GetChildren(rootelement))
            {
                if (obj is DependencyObject)
                {
                    if (typeChild.IsInstanceOfType(obj))
                    {
                        yield return obj as DependencyObject;
                    }

                    foreach (DependencyObject obj1 in EnumLogicalChildrenOfType(obj as DependencyObject, typeChild))
                    {
                        yield return obj1;
                    }
                }
            }
        }

        /// <summary>
        /// Indicates whether node element contains in some element.
        /// </summary>
        /// <param name="reference">Root element.</param>
        /// <param name="node">Node element</param>
        /// <returns>Value indicates when node element contains in some element.</returns>
        public static bool IsDescendant(DependencyObject reference, DependencyObject node)
        {
            bool result = false;

            while (null != node)
            {
                if (node == reference)
                {
                    result = true;
                    break;
                }

                if (node.GetType() == RootPopupType)
                {
                    Popup popup = (node as FrameworkElement).Parent as Popup;
                    node = popup;

                    if (popup != null)
                    {
                        node = popup.Parent;

                        if (node == null)
                        {
                            node = popup.PlacementTarget;
                        }
                    }
                }
                else
                {
                    node = FindParent(node);
                }
            }

            return result;
        }

        /// <summary>
        /// Invalidates measurement of the element parent.
        /// </summary>
        /// <param name="element">Element of parent.</param>
        public static void InvalidateParentMeasure(FrameworkElement element)
        {
            FrameworkElement parent = VisualTreeHelper.GetParent(element) as FrameworkElement;

            if (parent != null)
            {
                parent.InvalidateMeasure();
            }
        }

        /// <summary>
        /// Looks for the logical ancestor of the specified type.
        /// </summary>
        /// <param name="rootelement"><see cref="FrameworkElement"/> the search is started from.</param>
        /// <param name="typeParent">Desired type of the parent.</param>
        /// <returns>
        /// Object of the specified type, or null if no ancestors of the specified type were found.
        /// </returns>
        public static FrameworkElement FindSomeParent(FrameworkElement rootelement, Type typeParent)
        {
            FrameworkElement parent = rootelement.Parent as FrameworkElement;

            while (null != parent && !typeParent.IsInstanceOfType(parent))
            {
                parent = parent.Parent as FrameworkElement;
            }

            return parent;
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

            // we�ve reached the end of the tree
            if (parentObject == null) return null;

            // check if the parent matches the type we�re looking for
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
        /// <summary>
        /// This method stops animation and set new value on given dependency property of target element.
        /// </summary>
        /// <param name="targetElement">Target element.</param>
        /// <param name="dependencyProperty">Given dependency property.</param>
        /// <param name="value">New value to set.</param>
        public static void SetDependencyPropretyUsedByAnimation(UIElement targetElement, DependencyProperty dependencyProperty, double value)
        {
            targetElement.BeginAnimation(dependencyProperty, null);
            targetElement.SetValue(dependencyProperty, value);
        }

        /// <summary>
        /// This method gets a value indicating whether the current element contains a child of specified type.
        /// </summary>
        /// <param name="rootelEment">The root element.</param>
        /// <param name="searchType">Specified type</param>
        /// <returns>Result of searching.</returns>
        public static bool HasChildOfType(Visual rootelEment, Type searchType)
        {
            bool result = false;
            int iCount = VisualTreeHelper.GetChildrenCount(rootelEment);

            for (int i = 0; i < iCount; ++i)
            {
                Visual child = VisualTreeHelper.GetChild(rootelEment, i) as Visual;
                //// result = ( searchType == child.GetType() );
                result = searchType.IsInstanceOfType(child);

                if (!result)
                {
                    if (child != null)
                    {
                        result = HasChildOfType(child, searchType);

                        if (result)
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

            return result;
        }

        /// <summary>
        /// Points to screen.
        /// </summary>
        /// <param name="visual">The visual value.</param>
        /// <param name="point">The point value.</param>
        /// <returns>Return the point value</returns>
        public static Point PointToScreen(UIElement visual, Point point)
        {
            if (PermissionHelper.HasUnmanagedCodePermission)
            {
                return PermissionHelper.GetSafePointToScreen(visual, point);
            }
            else
            {
                Point pt = GetPointRelativeTo(visual, null);
                return new Point(pt.X + point.X, pt.Y + point.Y);
            }
        }

        /// <summary>
        /// Gets the point relative to.
        /// </summary>
        /// <param name="visual">The visual.</param>
        /// <param name="relativeTo">The relative to.</param>
        /// <returns>Return point value.</returns>
        public static Point GetPointRelativeTo(UIElement visual, UIElement relativeTo)
        {
            Point pt1 = Mouse.PrimaryDevice.GetPosition(visual);
            Point pt2 = Mouse.PrimaryDevice.GetPosition(relativeTo);
            return new Point(pt2.X - pt1.X, pt2.Y - pt1.Y);
        }

        /// <summary>
        /// Search parent for element.
        /// </summary>
        /// <param name="d">Element for which search parent.</param>
        /// <returns>Parent element.</returns>
        private static DependencyObject FindParent(DependencyObject d)
        {
            Visual reference = d as Visual;
            ContentElement contentElement = (reference == null) ? (d as ContentElement) : null;

            if (null != contentElement)
            {
                d = ContentOperations.GetParent(contentElement);

                if (d != null)
                {
                    return d;
                }

                FrameworkContentElement frameworkContentElement = contentElement as FrameworkContentElement;

                if (null != frameworkContentElement)
                {
                    return frameworkContentElement.Parent;
                }
            }
            else if (reference != null)
            {
                return VisualTreeHelper.GetParent(reference);
            }

            return null;
        }

        /// <summary>
        /// Finds parent Page object through given element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static Page GetPageFromChildren(FrameworkElement element)
        {
            Page page = null;

            if (element != null)
            {
                page = element as Page;

                if (page == null)
                {
                    while (element != null)
                    {
                        element = VisualTreeHelper.GetParent(element) as FrameworkElement;

                        if (element is Page)
                        {
                            page = (Page)element;
                            break;
                        }
                    }
                }
            }

            return page;
        }
        /// <summary>
        /// Finds parent Window object through given element.
        /// </summary>
        public static Window GetWindowFromChildren(FrameworkElement element)
        {
            Window window = null;

            if (element != null)
            {
                window = element as Window;

                if (window == null)
                {
                    while (element != null)
                    {
                        element = VisualTreeHelper.GetParent(element) as FrameworkElement;

                        if (element is Window)
                        {
                            window = (Window)element;
                            break;
                        }
                    }
                }
            }

            return window;
        }
        #endregion
    }
    /// <summary>
    /// 
    /// </summary>
    public static class GarbageUtils
    {
        ///// <summary>
        ///// Garbages the pair kay value.
        ///// </summary>
        ///// <param name="visulstyleList">The visulstyle list.</param>
        //public static void GarbagePairKayValue(DictionaryList visulstyleList)
        //{
        //    foreach (KeyValuePair<string, object> keyValuePair in visulstyleList)
        //    {
        //        if (keyValuePair.Value is PairKeyValue)
        //        {
        //            PairKeyValue pairKeyValue = keyValuePair.Value as PairKeyValue;
        //            if (pairKeyValue.Value is DictionaryList)
        //            {
        //                DictionaryList temp = (pairKeyValue.Value as DictionaryList);
        //                temp.Clear();
        //                temp = null;
        //            }
        //        }
        //    }
        //    if (visulstyleList != null)
        //    {
        //        visulstyleList.Clear();
        //        visulstyleList = null;
        //    }
        //}
    }
}
