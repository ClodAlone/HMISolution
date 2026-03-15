#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Windows;

#if !WinRT
using Syncfusion.Windows.GridCommon;

namespace Syncfusion.Windows.Controls.Cells
#else
using Syncfusion.WinRT.GridCommon;

namespace Syncfusion.WinRT.Controls.Cells
#endif
{

    /// <summary>
    /// This class maintains the state of visible cell span background and lets you
    /// also access the underlying <see cref="CellSpanBackground"/> for which this
    /// visible cell span background was created and the row and column section
    /// this span belongs to.
    /// </summary>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class VisibleCellSpanBackgroundInfo : VisibleCellSpanInfo
    {
        int rowSection;
        int columnSection;

        /// <summary>
        /// Initializes a new instance of the <see cref="VisibleCellSpanBackgroundInfo"/> class.
        /// </summary>
        public VisibleCellSpanBackgroundInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VisibleCellSpanBackgroundInfo"/> class.
        /// </summary>
        /// <param name="top">The visible top row index.</param>
        /// <param name="left">The visible left column index.</param>
        /// <param name="cellSpanBackground">The cell span background.</param>
        public VisibleCellSpanBackgroundInfo(int top, int left, CellSpanBackgroundInfo cellSpanBackground)
            : base(top, left, cellSpanBackground)
        {
        }

        /// <summary>
        /// Gets the cell span background.
        /// </summary>
        /// <value>The cell span background.</value>
        public CellSpanBackgroundInfo CellSpanBackground
        {
            get { return (CellSpanBackgroundInfo) cellSpan; }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents the current <see cref="System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return String.Format("CoveredCellSpan: {0},{1},{2},{3},{4}", top, left, bottom, right, cellSpan);
        }

        /// <summary>
        /// Gets or sets the row section.
        /// </summary>
        /// <value>The row section.</value>
        public int RowSection
        {
            get { return rowSection; }
            set { rowSection = value; }
        }

        /// <summary>
        /// Gets or sets the column section.
        /// </summary>
        /// <value>The column section.</value>
        public int ColumnSection
        {
            get { return columnSection; }
            set { columnSection = value; }
        }
    }

}
