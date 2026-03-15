// <copyright file="TwoItemsLayoutPanelEx.cs" company="Syncfusion">
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

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Panel with two columns.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class TwoItemsLayoutPanelEx : Panel
    {
        #region Properties
        /// <summary>
        /// Gets or sets the minimum right column width.
        /// </summary>
        public double MinRightWidth
        {
            get
            {
                return (double)GetValue(MinRightWidthProperty);
            }

            set
            {
                SetValue(MinRightWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the maximum right column width.
        /// </summary>
        public double MaxRightWidth
        {
            get
            {
                return (double)GetValue(MaxRightWidthProperty);
            }

            set
            {
                SetValue(MaxRightWidthProperty, value);
            }
        }
        #endregion

        #region Dependency Properties
        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Defines minimal right column width.
        /// </summary>
        public static readonly DependencyProperty MinRightWidthProperty =
            DependencyProperty.Register("MinRightWidth", typeof(double), typeof(TwoItemsLayoutPanelEx), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Defines maximum right column width.
        /// </summary>
        public static readonly DependencyProperty MaxRightWidthProperty =
            DependencyProperty.Register("MaxRightWidth", typeof(double), typeof(TwoItemsLayoutPanelEx), new UIPropertyMetadata(0d));
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
            if (Children.Count != 2)
            {
                throw new InvalidOperationException();
            }

            double minWidth = MinRightWidth;

            UIElement elementLeft = Children[0];
            UIElement elementRight = Children[1];
            elementLeft.Measure(new Size(Math.Max(0, availableSize.Width - minWidth), availableSize.Height));
            elementRight.Measure(new Size(Math.Min(MaxRightWidth, availableSize.Width - elementLeft.DesiredSize.Width), availableSize.Height));

            Size result = new Size(Math.Min(availableSize.Width, elementLeft.DesiredSize.Width + elementRight.DesiredSize.Width), availableSize.Height);

            return result;
        }

        /// <summary>
        /// Arranges children.
        /// </summary>
        /// <param name="finalSize">The final area within the parent
        /// that this element should use to
        /// arrange itself and its children.</param>
        /// <returns>
        /// The actual size used.
        /// </returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            UIElement elementLeft = Children[0];
            UIElement elementRight = Children[1];

            elementLeft.Arrange(new Rect(0, 0, elementLeft.DesiredSize.Width, finalSize.Height));
            elementRight.Arrange(new Rect(elementLeft.DesiredSize.Width, 0, finalSize.Width - elementLeft.DesiredSize.Width, finalSize.Height));
            return finalSize;
        }
        #endregion
    }
}
