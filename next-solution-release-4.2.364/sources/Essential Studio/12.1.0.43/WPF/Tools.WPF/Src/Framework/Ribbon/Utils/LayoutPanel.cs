// <copyright file="LayoutPanel.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <property name="flag" value="Finished" />
    /// <summary>
    /// This class is responsible for layout of ribbon bars.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class LayoutPanel : Panel
    {
        #region Initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutPanel"/> class.
        /// </summary>
        public LayoutPanel()
            : base()
        {
        }
        #endregion

        #region Implementation
        /// <property name="flag" value="Finished" />
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
            Size childSize = availableSize;

            foreach (UIElement child in InternalChildren)
            {
                child.Measure(childSize);
            }

            if (double.IsInfinity(availableSize.Height) || double.IsInfinity(availableSize.Width))
            {
                availableSize = InternalChildren[0].DesiredSize;
            }

            return availableSize;
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Provide a required override for the ArrangeOverride method.
        /// </summary>
        /// <param name="finalSize">The final area within the parent
        /// that this element should use to
        /// arrange itself and its children</param>
        /// <returns>
        /// The actual size used.
        /// </returns>
        protected override System.Windows.Size ArrangeOverride(System.Windows.Size finalSize)
        {
            foreach (UIElement child in InternalChildren)
            {
                child.Arrange(new Rect(new Point(0, 0), finalSize));
            }

            return finalSize;

        }
        #endregion
    }
}
