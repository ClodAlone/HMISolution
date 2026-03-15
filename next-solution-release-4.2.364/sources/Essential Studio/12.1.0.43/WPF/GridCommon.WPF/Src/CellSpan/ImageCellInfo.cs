#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;

namespace Syncfusion.Windows.Controls.Cells
{
    /// <summary>
    /// Holds the range and options for a Image cell. A Image cell is a cell that
    /// spans over neighbouring cells. All cells in this range are treated as one single cell.
    /// </summary>
    public class OverlappingCellInfo : CellSpanInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OverlappingCellInfo"/> class.
        /// </summary>
        public OverlappingCellInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OverlappingCellInfo"/> class.
        /// </summary>
        /// <param name="top">The top.</param>
        /// <param name="left">The left.</param>
        /// <param name="bottom">The bottom.</param>
        /// <param name="right">The right.</param>
        public OverlappingCellInfo(int top, int left, int bottom, int right)
            : base(top, left, bottom, right)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OverlappingCellInfo"/> class.
        /// </summary>
        /// <param name="top">The top.</param>
        /// <param name="left">The left.</param>
        /// <param name="bottom">The bottom.</param>
        /// <param name="right">The right.</param>
        /// <param name="clipRows">if set to <c>true</c> allow estimates for out of view rows.</param>
        /// <param name="clipColumns">if set to <c>true</c> allow estimates for out of view columns.</param>
        public OverlappingCellInfo(int top, int left, int bottom, int right, bool clipRows, bool clipColumns)
            : base(top, left, bottom, right, clipRows, clipColumns)
        {

        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return base.ToString();
        }
    }
}
