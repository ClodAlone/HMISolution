// <copyright file="TileViewPanel.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if !WINDOWS_PHONE_7
using System.Threading.Tasks;
#endif
#if WINDOWS_PHONE||WINDOWS_PHONE_7
using System.Windows;
using System.Windows.Controls;
#else
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif
#if WINDOWS_PHONE||WINDOWS_PHONE_7
namespace Syncfusion.WP.Controls.Layout
#else
namespace Syncfusion.UI.Xaml.Controls.Layout
#endif
{
    /// <summary>
    /// Used to group and arrange the collections of <see
    /// cref="T:Syncfusion.UI.Xaml.Controls.Layout.TileViewItem"/>.
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public class TileViewPanel : Panel
    {
        #region Variables

        internal double totalWidth = 0;

        internal double totalHeight = 0;

        private TileItemsControl parentItemsControl;

        internal TileItemsControl ParentItemsControl
        {
            get
            {
                if (parentItemsControl == null)
                {
                    parentItemsControl = ItemsControl.GetItemsOwner(this) as TileItemsControl;
                }
                return parentItemsControl;
            }
        }

        #endregion

        #region Override Methods
        /// <summary>
        /// Calculates the size for the UIElements
        /// </summary>
        /// <param name="availableSize"></param>
        /// <returns></returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            double totalwidth = 0.0;

            double totalheight = 0.0;

            double width = 0.0;

            double height = 0.0;

            int columncount = 1;

            int rowcount = 1;

            double top = 0.0;
            double left = 0.0;
            if (ParentItemsControl != null)
            {
                if (ParentItemsControl.tileview != null)
                    ParentItemsControl.tileview.tileViewPanel = this;
                if (ParentItemsControl.tileview != null && ParentItemsControl.tileview.MaximizedItem == null)
                {
                    if (ParentItemsControl.Orientation == Orientation.Horizontal)
                    {
                        if (double.IsInfinity(availableSize.Width))
                            availableSize.Width = ParentItemsControl.ActualWidth;
                        foreach (UIElement element in Children)
                        {
                            element.Measure(availableSize);
                            //Virtually measure the height of the viewport based on arrange override for an issue.
                            
                            left += ParentItemsControl.ItemWidth +
                                ((FrameworkElement)element).Margin.Left +
                                ((FrameworkElement)element).Margin.Right;
                            
                            if (Children[Children.Count - 1] != element)
                                width = left + ParentItemsControl.ItemWidth;
                            else
                                width = left;

                            if (width > availableSize.Width)
                            {
                                totalwidth = Math.Min(availableSize.Width, Math.Max(totalwidth, width));
                                left = 0.0;
                                width = 0.0;
                                rowcount++;
                            }
                        }
                        return new Size(Math.Max(totalwidth, width), rowcount * ParentItemsControl.ItemHeight);
                    }
                    else
                    {
                        if (double.IsInfinity(availableSize.Height))
                            availableSize.Height = ParentItemsControl.ActualHeight;
                        foreach (UIElement element in Children)
                        {
                            element.Measure(availableSize);
                           //Virtually measure the width of the viewport based on arrange override for an issue.
                            top += ParentItemsControl.ItemHeight +
                              ((FrameworkElement)element).Margin.Top +
                              ((FrameworkElement)element).Margin.Bottom;

                            if (Children[Children.Count - 1] != element)
                                height = top + ParentItemsControl.ItemHeight;
                            else
                                height = top;

                            if (height > availableSize.Height)
                            {
                                totalheight = Math.Min(availableSize.Height,Math.Max(totalheight, height));
                                top = 0.0;
                                height = 0.0;
                                columncount++;
                            }
                        }
                        return new Size(columncount * ParentItemsControl.ItemWidth, Math.Max(totalheight, height));
                    }
                }
                else
                {
                    if (ParentItemsControl.tileview != null && (ParentItemsControl.tileview.MinimizedItemsOrientation == MinimizedItemsOrientation.Right
                                                                || ParentItemsControl.tileview.MinimizedItemsOrientation == MinimizedItemsOrientation.Left))
                    {
                        double _height = ParentItemsControl.ItemHeight;

                        foreach (UIElement element in Children)
                        {
                            element.Measure(availableSize);
                            _height += ParentItemsControl.ItemHeight;
                        }
                        return new Size(ParentItemsControl.ItemWidth, _height);
                    }
                    else
                    {
                        double _width = ParentItemsControl.ItemWidth;

                        foreach (UIElement element in Children)
                        {
                            element.Measure(availableSize);
                            _width += ParentItemsControl.ItemWidth;
                        }
                        return new Size(_width, ParentItemsControl.ItemHeight);
                    }
                }
            }
            return base.MeasureOverride(availableSize);
        }

        /// <summary>
        /// Arranges the overrided elements
        /// </summary>
        /// <param name="finalSize"></param>
        /// <returns></returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            double top = 0.0;
            double left = 0.0;
            int rowCount = 1;
            int columnCount = 1;
            if (ParentItemsControl != null)
            {
                foreach (UIElement element in Children)
                {
                    SfTileViewItem tileItem = element as SfTileViewItem;
                    if (tileItem != null && tileItem.State == TileViewItemState.Maximized)
                    {
                        continue;
                    }

                    Rect rect = new Rect(left, top, ParentItemsControl.ItemWidth, ParentItemsControl.ItemHeight);
                    element.Arrange(rect);
                    if (ParentItemsControl.tileview.MaximizedItem == null)
                    {
                        if (ParentItemsControl.Orientation == Orientation.Horizontal)
                        {
                            left += ParentItemsControl.ItemWidth +
                                ((FrameworkElement)element).Margin.Left +
                                ((FrameworkElement)element).Margin.Right;

                            if (left + ParentItemsControl.ItemWidth > finalSize.Width)
                            {
                                left = 0.0;
                                top += ParentItemsControl.ItemHeight;
                                rowCount++;
                            }
                        }
                        else
                        {
                            top += ParentItemsControl.ItemHeight +
                               ((FrameworkElement)element).Margin.Top +
                               ((FrameworkElement)element).Margin.Bottom;

                            if (top + ParentItemsControl.ItemHeight > finalSize.Height)
                            {
                                top = 0.0;
                                left += ParentItemsControl.ItemWidth;
                                columnCount++;
                            }
                        }
                    }
                    else
                    {
                        if (ParentItemsControl.tileview.MinimizedItemsOrientation == MinimizedItemsOrientation.Left ||
                            ParentItemsControl.tileview.MinimizedItemsOrientation == MinimizedItemsOrientation.Right)
                        {

                            top += ParentItemsControl.ItemHeight;
                            rowCount++;
                            left = 0;
                        }
                        else
                        {
                            top = 0.0;
                            left += ParentItemsControl.ItemWidth;
                            columnCount++;
                        }
                    }
                }
            }
            return finalSize;
        }

        #endregion
    }
}
