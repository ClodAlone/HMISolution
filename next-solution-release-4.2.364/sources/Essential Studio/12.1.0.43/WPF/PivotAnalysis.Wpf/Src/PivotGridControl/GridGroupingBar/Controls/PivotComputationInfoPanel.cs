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
using System.Windows.Controls;
using System.Windows;
using Syncfusion.PivotAnalysis.Base;
using System.Windows.Controls.Primitives;

namespace Syncfusion.Windows.Controls.PivotGrid
{
    /// <summary>
    /// This class represents the PivotComputationInfo StackPanel represented as a ItemsPanel for ComputationInfo Groupingbar
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class PivotComputationInfoPanel : StackPanel
    {
        #region [ Overrides ]
        /// <summary>
        /// Updates the Background for child elements when the items are set to AllowRunTimeGroupByField property false.
        /// </summary>
        protected override void OnRender(System.Windows.Media.DrawingContext dc)
        {
            PivotGridControl grid = Common.GetParentElement<PivotGridControl>(this) as PivotGridControl;
            foreach (ListBoxItem item in this.Children)
            {
                PivotGridGroupingBar.SetDisabled(item, grid);
            }
            base.OnRender(dc);
        }

        /// <summary>
        /// Invalidates the visual once the ListBoxItems are added.
        /// </summary>
        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
            this.InvalidateVisual();
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
            PivotGridControl grid = Common.GetParentElement<PivotGridControl>(this) as PivotGridControl;           
            int count = this.Children.Count;
            double desiredWidth = 0d;
            foreach (ListBoxItem item in this.Children)
            {
                PivotComputationInfo compInfo = item.Content as PivotComputationInfo;
                if (compInfo != null)
                {
                    double width = Common.GetTextSize(compInfo.FieldHeader).Width + 25;
                    desiredWidth += width;
                    item.Width = width;
                }
            }
            if (desiredWidth > availableSize.Width)
            {
                foreach (UIElement item in this.Children)
                {
                    ListBoxItem lstItem = item as ListBoxItem;
                    if (lstItem != null)
                    {
                        if (((availableSize.Width / count)) >= 2)
                        {
                          if(grid.AllowRowHeaderAreaAutoSizing)
                              lstItem.Width = (availableSize.Width / count) - 2;
                        }
                    }
                }
            }
            return base.MeasureOverride(availableSize);
        }

        #endregion
    }
}
