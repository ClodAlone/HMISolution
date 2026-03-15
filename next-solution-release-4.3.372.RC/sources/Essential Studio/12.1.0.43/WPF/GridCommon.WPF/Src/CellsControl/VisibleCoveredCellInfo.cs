#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Windows;
using Syncfusion.Windows.GridCommon;

namespace Syncfusion.Windows.Controls.Cells
{
    /// <summary>
    /// This class maintains the state of a visible covered cell and lets you
    /// also access the underlying <see cref="CoveredCell"/> for which this
    /// visible covered cell was created.
    /// </summary>
    public class VisibleCoveredCellInfo : VisibleCellSpanInfo
    {
        internal int arrangeId;

        /// <summary>
        /// Initializes a new instance of the <see cref="VisibleCoveredCellInfo"/> class.
        /// </summary>
        public VisibleCoveredCellInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VisibleCoveredCellInfo"/> class.
        /// </summary>
        /// <param name="top">The visible top row index.</param>
        /// <param name="left">The visible left column index.</param>
        /// <param name="coveredCell">The covered cell.</param>
        public VisibleCoveredCellInfo(int top, int left, CoveredCellInfo coveredCell)
            : base(top, left, coveredCell)
        {
        }

        /// <summary>
        /// Gets the covered cell.
        /// </summary>
        /// <value>The covered cell.</value>
        public CoveredCellInfo CoveredCell
        {
            get { return (CoveredCellInfo) cellSpan; }
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
    }

}
