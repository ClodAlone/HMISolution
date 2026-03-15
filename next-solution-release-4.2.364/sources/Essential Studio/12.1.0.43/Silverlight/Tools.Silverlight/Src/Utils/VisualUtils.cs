#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Syncfusion.Silverlight.Shared
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
        /// Static constructor. 
        /// Note: Initialization static member.
        /// </summary>
        static VisualUtils()
        {
            RootPopupType = typeof(Popup);
        }

        /// <summary>
        /// Hides default public contructor - this class should not be created by users.
        /// </summary>
        private VisualUtils()
        {
        }
        #endregion

        /// <summary>
        /// Looks for the visual ancestor of the specified type.
        /// </summary>
        /// <param name="startingFrom"><see cref="DependencyObject"/> the search is started from.</param>
        /// <param name="typeAncestor">Desired type of the ancestor.</param>
        /// <returns>Type : DependencyObject</returns>
        public static DependencyObject FindAncestor(DependencyObject startingFrom, Type typeAncestor)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(startingFrom);

            while (parent != null && !typeAncestor.IsInstanceOfType(parent))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            return parent;
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
                    }
                }
                else
                {
                    node = VisualTreeHelper.GetParent(node);
                }
            }
            
            return result;
        }
    }

    /// <summary>
    /// Class used for determining the 
    /// </summary>
    public static class Position
    {
        /// <summary>
        /// Position enumeration
        /// </summary>
        public enum Corner
        {
            /// <summary>
            /// Top Left Position
            /// </summary>
            LeftTop,

            /// <summary>
            /// Bottom Left Position
            /// </summary>
            LeftBottom,

            /// <summary>
            /// Top Right Position
            /// </summary>
            RightTop,

            /// <summary>
            /// Bottom Right Position
            /// </summary>
            RightBottom
        }

        /// <summary>
        /// Gets the Absolute position of the Element
        /// </summary>
        /// <param name="e">Represents the Element whose Absolute Position is determined</param>
        /// <returns>Type : Point</returns>
        public static Point GetAbsolutePosition(FrameworkElement e)
        {
            return GetAbsolutePosition(e, Corner.LeftTop);
        }

        /// <summary>
        /// Gets the Absolute position of the Element
        /// </summary>
        /// <param name="e">Represents the Element whose Absolute Position is determined</param>
        /// <param name="p">Enumeration that determines the position</param>
        /// <returns>Type : Point</returns>
        public static Point GetAbsolutePosition(FrameworkElement e, Corner p)
        {
            return GetRelativePosition(e, Application.Current.RootVisual as FrameworkElement, p);
        }

        /// <summary>
        /// Gets the Relative position of the Element
        /// </summary>
        /// <param name="e">Represents the Element whose Relative Position is determined</param>
        /// <param name="relativeTo">Represent Element, Relative to this Element position is determined</param>
        /// <returns>Type : Point</returns>
        public static Point GetRelativePosition(FrameworkElement e, FrameworkElement relativeTo)
        {
            return GetRelativePosition(e, relativeTo, Corner.LeftTop);
        }

        /// <summary>
        /// Gets the Relative position of the Element
        /// </summary>
        /// <param name="e">Represents the Element whose Relative Position is determined</param>
        /// <param name="relativeTo">Represent Element, Relative to this Element position is determined</param>
        /// <param name="p">Enumeration that determines the position</param>
        /// <returns>Type : Point</returns>
        public static Point GetRelativePosition(FrameworkElement e, FrameworkElement relativeTo, Corner p)
        {
            GeneralTransform gt = e.TransformToVisual(relativeTo);
            Point po = new Point();
            if (p == Corner.LeftTop)
            {
                po = gt.Transform(new Point(0, 0));
            }

            if (p == Corner.LeftBottom)
            {
                po = gt.Transform(new Point(0, e.ActualHeight));
            }

            if (p == Corner.RightTop)
            {
                po = gt.Transform(new Point(e.ActualWidth, 0));
            }

            if (p == Corner.RightBottom)
            {
                po = gt.Transform(new Point(e.ActualWidth, e.ActualHeight));
            }

            return po;
        }
    } 
}