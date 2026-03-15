#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
#if WINRT
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#else
using System.Windows;
using System.Windows.Controls;
#endif

namespace Syncfusion.UI.Xaml.TreeMap
{
    public class TreeMapLegendPanel : Panel
    {
        #region CLR Properties

        private TreeMapLegend treeMapLegend;

        internal TreeMapLegend TreeMapLegend
        {
            get
            {
                if (treeMapLegend == null)
                {
                    treeMapLegend = ItemsControl.GetItemsOwner(this) as TreeMapLegend;
                    if (treeMapLegend != null)
                        treeMapLegend.legendPanel = this;
                }
                return treeMapLegend;
            }
        }

        #endregion

        #region Private Members
        private double itemWidth, itemHeight;
        #endregion

        #region Override Methods

        protected override Size MeasureOverride(Size availableSize)
        {
            double maxWidth = 0d, maxHeight = 0d, minWidth = 0d, minHeight = 0d;
            int colCount = 1, rowCount = 1;
            double top = 0d, left = 0d;
            bool hasItemWidth = false, hasItemHeight = false;

            if (TreeMapLegend != null)
            {
                if (TreeMapLegend.Orientation == Orientation.Horizontal)
                {
                    availableSize.Width = (double.IsInfinity(availableSize.Width)) ? TreeMapLegend.TreeMap.ActualWidth : availableSize.Width;
                    if (TreeMapLegend.Margin != new Thickness())
                    {
                        availableSize.Width -= (TreeMapLegend.Margin.Left + TreeMapLegend.Margin.Right);
                    }
                    MeasureItemWidthandHeight(ref availableSize, ref hasItemWidth, ref hasItemHeight);

                    foreach (UIElement element in Children)
                    {
                        left += itemWidth;

                        if (!Equals(Children[Children.Count - 1], element))
                            minWidth = left + itemWidth;
                        else
                            minWidth = left;

                        if (minWidth > availableSize.Width)
                        {
                            maxWidth = Math.Min(availableSize.Width, Math.Max(maxWidth, minWidth));
                            left = 0;
                            minWidth = 0;
                            rowCount++;
                        }
                    }
                    return new Size(Math.Max(maxWidth, minWidth), rowCount * itemHeight);
                }
                availableSize.Height = (double.IsInfinity(availableSize.Height)) ? TreeMapLegend.TreeMap.ActualHeight : availableSize.Height;
                if (TreeMapLegend.legendHeaderPresenter != null)
                {
                    availableSize.Height -= (TreeMapLegend.legendHeaderPresenter.DesiredSize.Height + TreeMapLegend.Margin.Top + TreeMapLegend.Margin.Bottom);
                }
                MeasureItemWidthandHeight(ref availableSize, ref hasItemWidth, ref hasItemHeight);

                foreach (UIElement element in Children)
                {
                    top += itemHeight;

                    if (!Equals(Children[Children.Count - 1], element))
                        minHeight = top + itemHeight;
                    else
                        minHeight = top;

                    if (minHeight > availableSize.Height)
                    {
                        maxHeight = Math.Min(availableSize.Height, Math.Max(maxHeight, minHeight));
                        top = 0;
                        minHeight = 0;
                        colCount++;
                    }
                }
                return new Size(colCount * itemWidth, Math.Max(maxHeight, minHeight));
            }
            return base.MeasureOverride(availableSize);
        }

        private void MeasureItemWidthandHeight(ref Size availableSize, ref bool hasItemWidth, ref bool hasItemHeight)
        {
            foreach (UIElement element in Children)
            {
                element.Measure(availableSize);

                if (TreeMapLegend.LegendItemWidth.Equals(0d))
                {
                    itemWidth = Math.Max(itemWidth, element.DesiredSize.Width);
                }
                else if (!hasItemWidth)
                {
                    itemWidth = TreeMapLegend.LegendItemWidth;
                    hasItemWidth = true;
                }
                if (TreeMapLegend.LegendItemHeight.Equals(0d))
                {
                    itemHeight = Math.Max(itemHeight, element.DesiredSize.Height);
                }
                else if (!hasItemHeight)
                {
                    itemHeight = TreeMapLegend.LegendItemHeight;
                    hasItemHeight = true;
                }
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double top = 0d, left = 0d;
            int rowCount = 1, columnCount = 1;

            if (TreeMapLegend != null)
            {
                foreach (UIElement element in Children)
                {
                    var rect = new Rect(left, top, itemWidth, itemHeight);
                    element.Arrange(rect);

                    if (TreeMapLegend.Orientation == Orientation.Horizontal)
                    {
                        left += itemWidth;

                        if (left + itemWidth > finalSize.Width)
                        {
                            left = 0;
                            top += itemHeight;
                            rowCount++;
                        }
                    }
                    else
                    {
                        top += itemHeight;

                        if (top + itemHeight > finalSize.Height)
                        {
                            top = 0;
                            left += itemWidth;
                            columnCount++;
                        }
                    }
                }
                if (TreeMapLegend.Orientation == Orientation.Horizontal)
                {
                    finalSize.Height = rowCount * itemHeight;
                }
                else
                {
                    finalSize.Width = columnCount * itemWidth;
                }
            }
            return finalSize;
        }

        #endregion
    }
}
