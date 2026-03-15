// <copyright file="GalleryGroupPanel.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Panel used for layout of Gallery content.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class GalleryGroupPanel : Panel
    {
        #region Fields
        /// <summary>
        /// Represents the Ribbon Gallery
        /// </summary>
        private RibbonGallery m_ribbonGallery;

        /// <summary>
        /// Represents the Initialized value
        /// </summary>
        private bool m_bInitialized = false;

        /// <summary>
        /// Represents the Group Count
        /// </summary>
        private int m_iGroupCount = 0;
        #endregion

        #region Implementation

        /// <summary>
        /// Raises the <see cref="E:System.Windows.FrameworkElement.Initialized"/> event. This method is invoked whenever <see cref="P:System.Windows.FrameworkElement.IsInitialized"/> is set to true internally.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            FrameworkElement fe = VisualUtils.FindRootVisual(this as Visual) as FrameworkElement;

            Popup popup = fe.Parent as Popup;

            m_ribbonGallery = popup.TemplatedParent as RibbonGallery;
            
            m_ribbonGallery.CurrentFilterChanged += new PropertyChangedCallback(M_RibbonGallery_CurrentFilterChanged);
        }

        /// <summary>
        /// M_ribbons the gallery_ current filter changed.
        /// </summary>
        /// <param name="d">The d param value.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void M_RibbonGallery_CurrentFilterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Debug.WriteLine(e.ToString());
        }

        /// <summary>
        /// Attaches the gallery groups.
        /// </summary>
        /// <param name="g">The g param value.</param>
        private void AttachGalleryGroups(RibbonGallery g)
        {
            if (g != null)
            {
                m_bInitialized = true;

                foreach (RibbonGalleryGroup group in g.GalleryGroups)
                {
                    DependencyObject groupItemParent = group.Parent;
                    if (groupItemParent != null)
                    {
                        GalleryGroupPanel panel = groupItemParent as GalleryGroupPanel;
                        panel.RemoveVisualChild(group);
                        panel.RemoveLogicalChild(group);
                    }

                    AddLogicalChild(group);
                    AddVisualChild(group);
                }

                g.GalleryGroups.CollectionChanged += new NotifyCollectionChangedEventHandler(GalleryGroups_CollectionChanged);
            }

            m_iGroupCount = g.GalleryGroups.Count;
        }

        /// <summary>
        /// Handles the CollectionChanged event of the GalleryGroups control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void GalleryGroups_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// When overridden in a derived class, measures the size in layout required for child elements and determines a size for the <see cref="T:System.Windows.FrameworkElement"/>-derived class.
        /// </summary>
        /// <param name="availableSize">The available size that this element can give to child elements. Infinity can be specified as a value to indicate that the element will size to whatever content is available.</param>
        /// <returns>
        /// The size that this element determines it needs during layout, based on its calculations of child element sizes.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            UIElementCollection children = Children;

            foreach (FrameworkElement child in children)
            {
                child.Measure(availableSize);
            }

            ObservableCollection<RibbonGalleryGroup> groups = m_ribbonGallery.GalleryGroups;

            double itemWidth = m_ribbonGallery.ItemWidth;
            double itemHeight = m_ribbonGallery.ItemHeight;

            double height = 0;

            RibbonGalleryFilter currentFilter = m_ribbonGallery.CurrentFilter;

            if (currentFilter != null)
            {
                int currentFilterIndex = m_ribbonGallery.GalleryFilters.IndexOf(currentFilter);

                foreach (RibbonGalleryGroup group in groups)
                {
                    Int32Collection filterIndexes = RibbonGallery.GetFilterIndexes(group);

                    if (filterIndexes.Contains(currentFilterIndex))
                    {
                        group.InvalidateMeasure();
                        group.Measure(availableSize);

                        height += group.DesiredSize.Height + 1;

                        int count = group.Items.Count;

                        int widthCount = (int)(availableSize.Width / m_ribbonGallery.ItemWidth);

                        int heightCount = 0;

                        if (widthCount > count)
                        {
                            heightCount = 1;
                        }
                        else
                        {
                            if (widthCount != 0)
                            {
                                heightCount = (int)Math.Round((double)count / (double)widthCount);
                            }

                            if (heightCount * widthCount < count)
                            {
                                heightCount++;
                            }
                        }

                        height += itemHeight * heightCount;
                    }
                }
            }

            Size size = new Size(availableSize.Width, height);
            return size;
        }

        /// <summary>
        /// When overridden in a derived class, positions child elements and determines a size for a <see cref="T:System.Windows.FrameworkElement"/> derived class.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should use to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            ObservableCollection<RibbonGalleryGroup> groups = m_ribbonGallery.GalleryGroups;
            double itemWidth = m_ribbonGallery.ItemWidth;
            double itemHeight = m_ribbonGallery.ItemHeight;
            double y = 0;
            UIElementCollection children = InternalChildren;

            if (children.Count > 0)
            {
                foreach (RibbonGalleryGroup group in groups)
                {
                    bool bArrange = true;

                    if (m_ribbonGallery.HasFilters)
                    {
                        RibbonGalleryFilter currentFilter = m_ribbonGallery.CurrentFilter;

                        int currentFilterIndex = -1;

                        if (currentFilter != null)
                        {
                            currentFilterIndex = m_ribbonGallery.GalleryFilters.IndexOf(currentFilter);
                        }

                        Int32Collection filterIndexes = RibbonGallery.GetFilterIndexes(group);

                        if (filterIndexes.Contains(currentFilterIndex) == false)
                        {
                            bArrange = false;
                        }
                    }

                    int start = m_ribbonGallery.Items.IndexOf(group.Items[0]);
                    int end = start + group.Items.Count;

                    if (bArrange)
                    {
                        group.Visibility = Visibility.Visible;

                        for (int i = start; i < end; i++)
                        {
                            children[i].Visibility = Visibility.Visible;
                        }

                        group.InvalidateArrange();
                        group.Arrange(new Rect(0, y, finalSize.Width, group.DesiredSize.Height));
                        y += group.DesiredSize.Height + 1;
                        double d = ArrangeGroup(0, y, finalSize.Width, start, end, children, itemWidth, itemHeight);
                        y = d;
                    }
                    else
                    {
                        group.Visibility = Visibility.Collapsed;
                    }
                }
            }

            return new Size(finalSize.Width, y);
        }

        /// <summary>
        /// Arranges the group.
        /// </summary>
        /// <param name="x">The x param value.</param>
        /// <param name="y">The y param value.</param>
        /// <param name="availableWidth">Width of the available.</param>
        /// <param name="start">The start param value.</param>
        /// <param name="end">The end param value.</param>
        /// <param name="collection">The collection.</param>
        /// <param name="itemWidth">Width of the item.</param>
        /// <param name="itemHeight">Height of the item.</param>
        /// <returns>Returns the y value</returns>
        private double ArrangeGroup(double x, double y, double availableWidth, int start, int end, UIElementCollection collection, double itemWidth, double itemHeight)
        {
            for (int i = start; i < end; i++)
            {
                if (availableWidth - x < itemWidth)
                {
                    x = 0;
                    y += itemHeight;
                }

                if(collection[i] is RibbonGalleryItem)
                {
                    if ((collection[i] as RibbonGalleryItem).IsChecked == true)
                    {
                        (collection[i] as RibbonGalleryItem).IsChecked = false;
                        (collection[i] as RibbonGalleryItem).ApplyTemplate();
                        (collection[i] as RibbonGalleryItem).IsChecked = true;
                    }
                }
                collection[i].Arrange(new Rect(x, y, itemWidth, itemHeight));

                x += itemWidth;
            }

            return y + itemHeight;
        }

        /// <summary>
        /// Gets the number of child <see cref="T:System.Windows.Media.Visual"/> objects in this instance of <see cref="T:System.Windows.Controls.Panel"/>.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The number of child <see cref="T:System.Windows.Media.Visual"/> objects.
        /// </returns>
        protected override int VisualChildrenCount
        {
            get
            {
                int collCount = base.VisualChildrenCount;
                if (collCount == 0)
                {
                    return collCount;
                }
                
                if (m_bInitialized == false)
                {
                    AttachGalleryGroups(m_ribbonGallery);
                }

                return collCount + m_iGroupCount;
            }
        }

        /// <summary>
        /// Gets a <see cref="T:System.Windows.Media.Visual"/> child of this <see cref="T:System.Windows.Controls.Panel"/> at the specified index position.
        /// </summary>
        /// <param name="index">The index position of the <see cref="T:System.Windows.Media.Visual"/> child.</param>
        /// <returns>
        /// A <see cref="T:System.Windows.Media.Visual"/> child of the parent <see cref="T:System.Windows.Controls.Panel"/> element.
        /// </returns>
        protected override Visual GetVisualChild(int index)
        {
            int collCount = base.VisualChildrenCount;

            if (index >= collCount)
            {
                index -= collCount;

                return m_ribbonGallery.GalleryGroups[index];
            }

            return base.GetVisualChild(index);
        }
        #endregion
    }
}
