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
    /// This class maintains the state of a visible Overlapping Cell and lets you
    /// also access the underlying <see cref="OverlappingCell"/> for which this
    /// visible Overlapping Cell was created.
    /// </summary>
    public class VisibleOverlappingCellInfo: VisibleCellSpanInfo
    {
        internal int arrangeId;

        /// <summary>
        /// Initializes a new instance of the <see cref="VisibleOverlappingCellInfo"/> class.
        /// </summary>
        public VisibleOverlappingCellInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VisibleOverlappingCellInfo"/> class.
        /// </summary>
        /// <param name="top">The Top.</param>
        /// <param name="left">The Left.</param>
        /// <param name="coveredCell">The Image cell.</param>
        public VisibleOverlappingCellInfo(int top, int left, OverlappingCellInfo overlappingCell)
            : base(top, left, overlappingCell)
        {
        }

        /// <summary>
        /// Gets the Overlapping Cell.
        /// </summary>
        /// <value>The Overlapping Cell.</value>
        public OverlappingCellInfo OverlappingCell
        {
            get { return (OverlappingCellInfo)cellSpan; }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return String.Format("OverlappingCellSpan: {0},{1},{2},{3},{4}", top, left, bottom, right, cellSpan);
        }
    }
}
