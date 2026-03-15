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
    /// This a base class for spanned ranges such as covered cells and
    /// cell spanned backgrounds. It contains Top, Left, Bottom and
    /// Right row and column index for the spanned range.
    /// </summary>
    public class CellSpanInfo : CellSpanInfoBase
    {
        bool clipRows;
        bool clipColumns;

        /// <summary>
        /// Initializes a new instance of the <see cref="CellSpanInfo"/> class.
        /// </summary>
        public CellSpanInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CellSpanInfo"/> class.
        /// </summary>
        /// <param name="top">The top row index.</param>
        /// <param name="left">The left column index.</param>
        /// <param name="bottom">The bottom row index.</param>
        /// <param name="right">The right column index.</param>
        /// <param name="clipRows">if set to <c>true</c> allow estimates for out of view rows.</param>
        /// <param name="clipColumns">if set to <c>true</c> allow estimates for out of view columns.</param>
        public CellSpanInfo(int top, int left, int bottom, int right, bool clipRows, bool clipColumns)
            : base(top, left, bottom, right)
        {
            this.clipRows = clipRows;
            this.clipColumns = clipColumns;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CellSpanInfo"/> class.
        /// </summary>
        /// <param name="top">The top row index.</param>
        /// <param name="left">The left column index.</param>
        /// <param name="bottom">The bottom row index.</param>
        /// <param name="right">The right column index.</param>
        public CellSpanInfo(int top, int left, int bottom, int right)
            : base(top, left, bottom, right)
        {
            this.clipRows = false;
            this.clipColumns = false;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to allow estimates for out of view rows when calculating
        /// the height in points for the spanned range.
        /// </summary>
        /// <value><c>true</c> if estimates are allowed for out of view rows; otherwise, <c>false</c>.</value>
        public bool ClipRows
        {
            get { return clipRows; }
            set { clipRows = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to allow estimates for out of view columns when calculating
        /// the height in points for the spanned range.
        /// </summary>
        /// <value><c>true</c> if estimates are allowed for out of view columns; otherwise, <c>false</c>.</value>
        public bool ClipColumns
        {
            get { return clipColumns; }
            set { clipColumns = value; }
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="System.Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="System.Object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="System.Object"/>.</param>
        /// <returns>
        /// true if the specified <see cref="System.Object"/> is equal to the current <see cref="System.Object"/>; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;

            CellSpanInfo other = obj as CellSpanInfo;
            if (other == null)
                return false;

            return other.clipColumns == clipColumns
                && other.clipRows == clipRows;
        }

        /// <summary>
        /// Returns a string describing the state of the object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0} ( Top = {1} Left = {2} Bottom = {3} Right = {4} ClipRows = {5} ClipColumns = {6} )",
               GetType().Name, Top, Left, Bottom, Right, ClipRows, ClipColumns);
        }
    }

}
