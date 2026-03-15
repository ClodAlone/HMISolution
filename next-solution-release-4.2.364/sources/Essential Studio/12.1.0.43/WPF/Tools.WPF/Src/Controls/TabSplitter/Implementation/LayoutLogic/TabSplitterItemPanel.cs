// <copyright file="TabSplitterItemPanel.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the class for the TabSplitter Item panel
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class TabSplitterItemPanel : Panel
    {
        #region Private members
        /// <summary>
        /// Presents the row height
        /// </summary>
        private double m_rowHeight;
        #endregion

        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="TabSplitterItemPanel"/> class.
        /// </summary>
        static TabSplitterItemPanel()
        {
            KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(TabSplitterItemPanel), new FrameworkPropertyMetadata(KeyboardNavigationMode.Once));
            KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(typeof(TabSplitterItemPanel), new FrameworkPropertyMetadata(KeyboardNavigationMode.Cycle));
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="TabSplitterItemPanel"/> class.
        /// </summary>
        public TabSplitterItemPanel()
        {
            ClipToBounds = true;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Calculates the height of the max row.
        /// </summary>
        /// <param name="availableSize">Size of the available.</param>
        private void CalculateMaxRowHeight(Size availableSize)
        {
            m_rowHeight = 0;

            foreach (UIElement element in InternalChildren)
            {
                if (element.Visibility == Visibility.Collapsed)
                {
                    continue;
                }

                element.Measure(availableSize);
                double currentHeight = element.DesiredSize.Height;

                if (m_rowHeight < currentHeight)
                {
                    m_rowHeight = currentHeight;
                }
            }
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Returns an alternative clipping geometry that represents the region that would be clipped if ClipToBounds were set to true. 
        /// </summary>
        /// <param name="layoutSlotSize">The available size provided by the element.</param>
        /// <returns>The potential clipping geometry.</returns>
        protected override Geometry GetLayoutClip(Size layoutSlotSize)
        {
            if (ClipToBounds)
            {
                return new RectangleGeometry(new Rect(-2, -2, RenderSize.Width, 2 * RenderSize.Height));
            }

            return null;
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
            CalculateMaxRowHeight(availableSize);
            if (double.IsInfinity(availableSize.Width))
            {
                return base.MeasureOverride(new Size(availableSize.Width,m_rowHeight));
            }
            return new Size(availableSize.Width, m_rowHeight);
        }

        /// <summary>
        /// When overridden in a derived class, positions child elements and determines a size for a <see cref="T:System.Windows.FrameworkElement"/> derived class.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should use to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            double currentWidth = 0;

            foreach (UIElement child in InternalChildren)
            {
                if (currentWidth + child.DesiredSize.Width <= finalSize.Width)
                {
                    child.Arrange(new Rect(currentWidth, 0, child.DesiredSize.Width, m_rowHeight));
                    currentWidth += child.DesiredSize.Width;
                }
                else
                {
                    child.Arrange(new Rect(currentWidth, 0, 0, 0));
                }
            }

            return new Size(finalSize.Width, m_rowHeight);
        }
        #endregion
    }
}