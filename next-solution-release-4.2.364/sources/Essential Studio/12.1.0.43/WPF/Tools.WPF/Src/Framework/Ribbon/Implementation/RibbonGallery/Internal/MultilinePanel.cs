// <copyright file="MultilinePanel.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents panel for multiline layout inside RibbonGallery.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class MultilinePanel : Panel
    {
        #region Private members
        /// <summary>
        /// Represents the integer m_center
        /// </summary>
        private int m_center;

        /// <summary>
        /// Represents the FirstLineWidth
        /// </summary>
        private double m_firstLineWidth;

        /// <summary>
        /// Represents the SecondLineWidth
        /// </summary>
        private double m_secondLineWidth;
        #endregion

        /// <summary>
        /// Gets or sets the alignment.
        /// </summary>
        /// <value>The alignment.</value>
        public TextAlignment Alignment
        {
            get { return (TextAlignment)GetValue(AlignmentProperty); }

            set { SetValue(AlignmentProperty, value); }
        }

        #region Dependency properties
        /// <summary>
        /// Defines alignment of the panel.
        /// </summary>
        public static readonly DependencyProperty AlignmentProperty =
            DependencyProperty.Register("Alignment", typeof(TextAlignment), typeof(MultilinePanel), new UIPropertyMetadata(TextAlignment.Center));

        #endregion
        #region Implementation

        /// <summary>
        /// When overridden in a derived class, measures the size in layout required for child elements and determines a size for the <see cref="T:System.Windows.FrameworkElement"/>-derived class.
        /// </summary>
        /// <param name="availableSize">The available size that this element can give to child elements. Infinity can be specified as a value to indicate that the element will size to whatever content is available.</param>
        /// <returns>
        /// The size that this element determines it needs during layout, based on its calculations of child element sizes.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            UIElementCollection children = InternalChildren;
            int childrenCount = children.Count;

            if (childrenCount == 0)
            {
                return new Size(0, 0);
            }

            double maxLineWidth1, maxLineWidth2;
            m_center = 1;
            m_firstLineWidth = 0;
            m_secondLineWidth = 0;
            List<double> childrenWidth = new List<double>();

            foreach (UIElement child in children)
            {
                child.Measure(availableSize);
                double currentChildWidth = child.DesiredSize.Width;
                childrenWidth.Add(currentChildWidth);
            }

            m_firstLineWidth = childrenWidth[0];

            for (int i = m_center; i < childrenCount; i++)
            {
                m_secondLineWidth += childrenWidth[i];
            }

            maxLineWidth1 = Math.Max(m_firstLineWidth, m_secondLineWidth);

            for (int i = m_center; i < childrenCount; i++)
            {
                maxLineWidth2 = Math.Max(m_firstLineWidth + childrenWidth[i], m_secondLineWidth - childrenWidth[i]);

                if (maxLineWidth2 < maxLineWidth1)
                {
                    m_firstLineWidth += childrenWidth[i];
                    m_secondLineWidth -= childrenWidth[i];
                    maxLineWidth1 = maxLineWidth2;
                    m_center = i + 1;
                }
                else
                {
                    break;
                }
            }

            if (availableSize.Height == double.PositiveInfinity)
            {
                availableSize.Height = 2 * children[0].DesiredSize.Height;
            }

            //LargeButtonPanel panel = (LargeButtonPanel)VisualUtils.FindAncestor(this, typeof(LargeButtonPanel));

            //if (panel != null && panel.Children.Count == 1 && panel.MaxWidth != double.PositiveInfinity)
            //{
            //    availableSize.Width = Math.Max(maxLineWidth1, panel.MaxWidth);
            //}
            //else
            //{
            availableSize.Width = maxLineWidth1;
            //}

            return availableSize;
        }

        /// <summary>
        /// When overridden in a derived class, positions child elements and determines a size for a <see cref="T:System.Windows.FrameworkElement"/> derived class.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should use to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            UIElementCollection children = InternalChildren;
            double x = 1.5, y = 0;
            double width, height;

            if (Alignment != TextAlignment.Left)
            {
                x += (finalSize.Width - m_firstLineWidth) / 2;
            }

            for (int i = 0; i < children.Count; i++)
            {
                width = children[i].DesiredSize.Width;
                height = children[i].DesiredSize.Height;

                if (i < m_center)
                {
                    children[i].Arrange(new Rect(x, y, width, height));
                    x += width;
                }
                else
                {
                    if (i == m_center)
                    {
                        y = children[0].DesiredSize.Height;
                        if (Alignment != TextAlignment.Left)
                        {
                            x = (finalSize.Width - m_secondLineWidth) / 2;
                        }
                        else
                        {
                            x = 1.5;
                        }
                    }

                    children[i].Arrange(new Rect(x, y, width, height));
                    x += width;
                }
            }

            return finalSize;
        }
        #endregion
    }
}
