// <copyright file="RibbonWindowPanel.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the Ribbon window panel of the Ribbon
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class RibbonWindowPanel : Panel
    {
        #region Implementation
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
            int count = VisualChildrenCount;
            double x = 0;

            for (int i = 0; i < count; i++)
            {
                UIElement child = GetVisualChild(i) as UIElement;
                double width = child.DesiredSize.Width;
                double height = child.DesiredSize.Height;

                child.Arrange(new Rect(0, 0, width, height));

                x += width;
            }

            return finalSize;
        }

        /// <summary>
        /// Provides a required override for the MeasureOverride method.
        /// </summary>
        /// <param name="availableSize">The available size that this
        /// element can give to child
        /// elements. Infinity can be
        /// specified as a value to indicate
        /// that the element will size to
        /// whatever content is available.</param>
        /// <returns>
        /// The size that this element determines it needs during layout,
        /// based on its calculations of child element sizes.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            Size size = new Size();
            UIElementCollection children = InternalChildren;
            bool isInDesignMode = (bool) DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue;

            foreach (UIElement child in Children)
            {
                child.Measure(availableSize);

                size.Height = Math.Max(size.Height, child.DesiredSize.Height);
                size.Width = Math.Max(size.Width, child.DesiredSize.Width);
            }

            if (!isInDesignMode)
            {
                if (!double.IsInfinity(availableSize.Width))
                    size.Width = availableSize.Width;
            }

            return size;
        }
        #endregion
    }
}
