#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Windows;
using System;

#if !WinRT
using Syncfusion.Windows.GridCommon;

namespace Syncfusion.Windows.Controls.Cells
#else
using Syncfusion.WinRT.GridCommon;
using Windows.Foundation;

namespace Syncfusion.WinRT.Controls.Cells
#endif
{
    /// <summary>
    /// This class maintains the state of visible cell span and lets you
    /// also access the underlying <see cref="CellSpan"/> for which this
    /// visible cell span was created. 
    /// </summary>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class VisibleCellSpanInfo : IComparable
    {
        internal int top;
        internal int left;
        internal int bottom;
        internal int right;
        internal CellSpanInfo cellSpan;
        internal Rect exactBounds;
        internal Rect clippedBounds;
        internal bool spansMultipleHorizontalSections = false;
        internal bool spansMultipleVerticalSections = false;
        internal bool isAmbiguousSection;
        internal bool forceClipping;

        /// <summary>
        /// Initializes a new instance of the <see cref="VisibleCellSpanInfo"/> class.
        /// </summary>
        public VisibleCellSpanInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VisibleCellSpanInfo"/> class.
        /// </summary>
        /// <param name="top">The top visible row index.</param>
        /// <param name="left">The left visible column index.</param>
        public VisibleCellSpanInfo(int top, int left)
        {
            this.top = this.bottom = top;
            this.left = this.right = left;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VisibleCellSpanInfo"/> class.
        /// </summary>
        /// <param name="top">The top visible row index.</param>
        /// <param name="left">The left visible column index.</param>
        /// <param name="cellSpan">The cell span.</param>
        public VisibleCellSpanInfo(int top, int left, CellSpanInfo cellSpan)
        {
            this.top = this.bottom = top;
            this.left = this.right = left;
            this.cellSpan = cellSpan;
        }

        /// <summary>
        /// Gets the underling cell span.
        /// </summary>
        /// <value>The cell span.</value>
        public CellSpanInfo CellSpan
        {
            get { return cellSpan; }
        }

        // Visible row and column index. Not absolute indexes.
        /// <summary>
        /// Gets the visible top row index.
        /// </summary>
        /// <value>The top.</value>
        public int Top
        {
            get { return top; }
        }

        /// <summary>
        /// Gets the visible left column index.
        /// </summary>
        /// <value>The left.</value>
        public int Left
        {
            get { return left; }
        }

        /// <summary>
        /// Gets the visible bottom row index.
        /// </summary>
        /// <value>The bottom.</value>
        public int Bottom
        {
            get { return bottom; }
        }

        /// <summary>
        /// Gets the visible right column index.
        /// </summary>
        /// <value>The right.</value>
        public int Right
        {
            get { return right; }
        }

        internal bool IsAmbiguousSection
        {
            get { return isAmbiguousSection; }
        }

        /// <summary>
        /// Gets the exact bounds.
        /// </summary>
        /// <value>The exact bounds.</value>
        public Rect ExactBounds
        {
            get { return exactBounds; }
        }

        /// <summary>
        /// Gets the clipped bounds.
        /// </summary>
        /// <value>The clipped bounds.</value>
        public Rect ClippedBounds
        {
            get { return clippedBounds; }
        }

        #region IComparable Members

        /// <summary>
        /// Compares the current instance with another object of the same type.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance is less than <paramref name="obj"/>. Zero This instance is equal to <paramref name="obj"/>. Greater than zero This instance is greater than <paramref name="obj"/>.
        /// </returns>
        public int CompareTo(object obj)
        {
            CellSpanInfo other = ((VisibleCellSpanBackgroundInfo)obj).CellSpan;
            return CellSpan.CompareTo(other);
        }

        #endregion
    }

}
