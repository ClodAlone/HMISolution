// <copyright file="DayCellPanel.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System.Windows;
using System.Windows.Controls;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// Represents day cell wrap panel.
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class DayCellPanel : Panel
    {
        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="DayCellPanel"/> class.
        /// </summary>
        public DayCellPanel()
            : base()
        {
        }

        #endregion Initialization

        #region Implementation

        /// <summary>
        /// Measures the size in layout required for child elements and
        /// determines a size for the control.
        /// </summary>
        /// <param name="availableSize">The available size that this
        /// element can give to child elements. Infinity can be
        /// specified as a value to indicate that the element will size to
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

            return availableSize;
        }

        /// <summary>
        /// Positions child elements and determines a size for the control.
        /// </summary>
        /// <param name="finalSize">The final area within the parent
        /// that this element should use to arrange itself and its children.</param>
        /// <returns>
        /// The actual size used.
        /// </returns>
        protected override System.Windows.Size ArrangeOverride(System.Windows.Size finalSize)
        {
            foreach (UIElement child in InternalChildren)
            {
                child.Arrange(new Rect(new Point(0, 0), this.DesiredSize));
            }

            return finalSize;
        }

        #endregion Implementation
    }
}