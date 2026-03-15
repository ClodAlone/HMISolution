// <copyright file="ChartPanel.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

namespace Syncfusion.Windows.Chart
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    /// <summary>
    /// Represents chart layout panel.
    /// </summary>
    #if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
    #endif
    public class ChartPanel : Panel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartPanel"/> class.
        /// </summary>
        public ChartPanel()
        {
            this.Background = Brushes.Transparent;
        }
        #region Implementation
        /// <summary>
        /// When overridden in a derived class, positions child elements and determines a size for a <see cref="T:System.Windows.FrameworkElement"></see> derived class.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should use to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            Rect rect = new Rect(finalSize);

            for (int i = this.InternalChildren.Count - 1; i >= 0; i--)
            {
                this.InternalChildren[i].Arrange(rect);
            }

            return base.ArrangeOverride(finalSize);
        }

        /// <summary>
        /// When overridden in a derived class, measures the size in layout required for child elements and determines a size for the <see cref="T:System.Windows.FrameworkElement"></see>-derived class.
        /// </summary>
        /// <param name="availableSize">The available size that this element can give to child elements. Infinity can be specified as a value to indicate that the element will size to whatever content is available.</param>
        /// <returns>
        /// The size that this element determines it needs during layout, based on its calculations of child element sizes.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            for (int i = this.InternalChildren.Count - 1; i >= 0; i--)
            {
                this.InternalChildren[i].Measure(availableSize);
            }

            return ChartLayoutUtils.CheckSize(availableSize);
        }
        #endregion
    }
}
