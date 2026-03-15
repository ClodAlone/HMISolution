// <copyright file="TreeViewItemAdvVirtualizingPanel.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

#region file using
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Syncfusion.Licensing;
using Syncfusion.Windows.Shared;
#endregion

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// An class that provides functionality for virtualizing <see cref="TreeViewItemAdv"/>.
    /// </summary>
    /// <list type="table">
    /// <listheader>
    /// <term>Help Page</term>
    /// <description>Syntax</description>
    /// </listheader>
    /// <example>
    /// <list type="table">
    /// <listheader>
    /// <description>C#</description>
    /// </listheader>
    /// <example><code>public class TreeViewItemAdvVirtualizingPanel : <see cref="TreeViewAdvItemsPanel"/></code></example>
    /// </list>
    /// <para/>
    /// <list type="table">
    /// <listheader>
    /// <description>XAML Object Element Usage</description>
    /// </listheader>
    /// <example>
    /// <code language="XAML">
    /// You cannot use this managed class in XAML.
    /// </code>
    /// </example>
    /// </list>
    /// </example>
    /// </list>
    /// <remarks>
    /// An class that provides functionality for virtualizing <see cref="TreeViewItemAdv"/> 
    /// and shows fake items. Using for TreeViewItemAdv.
    /// </remarks>
    /// <seealso cref="TreeViewAdv"/>
    /// <seealso cref="TreeViewItemAdv"/>
    /// <seealso cref="TreeViewAdvVirtualizingPanel"/>
    public class TreeViewItemAdvVirtualizingPanel : TreeViewAdvItemsPanel
    {
        #region Constants
        /// <summary>
        /// Default offset for LayoutClip.
        /// </summary>
        private const int C_layoutClipOffset = 1000;
        #endregion

        #region Properties
        /// <summary>
        /// Gets parent ItemsControl.
        /// </summary>
        private TreeViewItemAdv ParentTreeViewItem
        {
            get
            {
                return ParentItemsControl as TreeViewItemAdv;
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Raises the Initialized event. This method is invoked whenever IsInitialized 
        /// is set to true internally. 
        /// </summary>
        /// <param name="e">The RoutedEventArgs that contains the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            if (ParentTreeViewItem != null)
            {
              //  ParentTreeViewItem.ParentTreeView.ScrollHost.ScrollChanged += new ScrollChangedEventHandler(ScrollHost_ScrollChanged);
            }
        }

        /// <summary>
        /// Gets the height of the occupy.
        /// </summary>
        /// <returns>double value type</returns>
        protected override double GetOccupyHeight()
        {
            double height = 0;

            if (ParentTreeViewItem != null && ParentTreeViewItem.ParentTreeView != null
                && ParentTreeViewItem.ParentTreeView.ScrollHost != null
                && ParentItemsControl.IsVisible)
            {
                ScrollViewer scroller = ParentTreeViewItem.ParentTreeView.ScrollHost;
                height = VisualUtils.GetPointRelativeTo(ParentItemsControl, scroller).Y;
                height += (ParentTreeViewItem.CompleteHeaderElement == null) ?
                    0 : ParentTreeViewItem.CompleteHeaderElement.DesiredSize.Height;
            }

            if (ParentTreeViewItem != null && ParentTreeViewItem.ParentTreeView != null
                && ParentTreeViewItem.ParentTreeView.MultiColumnEnable
                && ParentTreeViewItem.ParentTreeView.HeaderRowPresenter != null)
            {
                height -= ParentTreeViewItem.ParentTreeView.HeaderRowPresenter.ActualHeight;
            }

            return height;
        }

        /// <summary>
        /// Measures the child elements of a <see cref="T:System.Windows.Controls.VirtualizingStackPanel"/> in anticipation of arranging them during the <see cref="M:System.Windows.Controls.VirtualizingStackPanel.ArrangeOverride(System.Windows.Size)"/> pass.
        /// </summary>
        /// <param name="constraint">An upper limit <see cref="T:System.Windows.Size"/> that should not be exceeded.</param>
        /// <returns>
        /// The <see cref="T:System.Windows.Size"/> that represents the desired size of the element.
        /// </returns>
        protected override Size MeasureOverride(Size constraint)
        {
           constraint= base.MeasureOverride(constraint);

           Size fakeSize = GetFakeItemsSize();
           constraint.Height += fakeSize.Height;

           if (constraint.Width < fakeSize.Width)
           {
               constraint.Width = fakeSize.Width;
           }

           return constraint;

        }

        /// <summary>
        /// Gets the height of the available.
        /// </summary>
        /// <param name="availableSize">Size of the available.</param>
        /// <returns>double value type</returns>
        protected override double GetAvailableHeight(Size availableSize)
        {
            double height = 0;

            if (ParentTreeViewItem != null && ParentTreeViewItem.ParentTreeView != null)
            {
                height = ParentTreeViewItem.ParentTreeView.ScrollHost.ViewportHeight;
            }

            return height;
        }

        /// <summary>
        /// Returns a geometry for a clipping mask. The mask applies if the layout system attempts 
        /// to arrange an element that is larger than the available display space. 
        /// </summary>
        /// <param name="layoutSlotSize">The size of the part of the element that does visual presentation.</param>
        /// <returns>The clipping geometry.</returns>
        protected override Geometry GetLayoutClip(Size layoutSlotSize)
        {
            double offset = C_layoutClipOffset;
            return new RectangleGeometry(new Rect(-offset, 0, layoutSlotSize.Width + 2 * offset, layoutSlotSize.Height));
        }

        #endregion
    }
}
