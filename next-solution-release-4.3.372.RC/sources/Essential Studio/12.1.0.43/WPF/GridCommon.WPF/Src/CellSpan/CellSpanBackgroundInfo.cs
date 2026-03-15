#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Windows.Media;

namespace Syncfusion.Windows.Controls.Cells
{
    /// <summary>
    /// Holds the range and options for a cell span background. A cell span background
    /// tells the cells control to draw the background of one cell across multiple
    /// neighbouring cells. For example you can have a picture as background and
    /// this picture will 
    /// be drawn across the neighbouring cells. These cells are still independent
    /// single cells, they only share the background.
    /// </summary>
    public class CellSpanBackgroundInfo : CellSpanInfo
    {
        Brush background;
        Pen pen;

        /// <summary>
        /// Initializes a new instance of the <see cref="CellSpanBackgroundInfo"/> class.
        /// </summary>
        public CellSpanBackgroundInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CellSpanBackgroundInfo"/> class.
        /// </summary>
        /// <param name="top">The top row index.</param>
        /// <param name="left">The left column index.</param>
        /// <param name="bottom">The bottom row index.</param>
        /// <param name="right">The right column index.</param>
        /// <param name="clipRows">if set to <c>true</c> allow estimates for out of view rows.</param>
        /// <param name="clipColumns">if set to <c>true</c> allow estimates for out of view columns.</param>
        /// <param name="background">The background.</param>
        /// <param name="pen">The pen.</param>
        public CellSpanBackgroundInfo(int top, int left, int bottom, int right, bool clipRows, bool clipColumns, Brush background, Pen pen)
            : base(top, left, bottom, right, clipRows, clipColumns)
        {
            this.background = background;
            this.pen = pen;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CellSpanBackgroundInfo"/> class.
        /// </summary>
        /// <param name="top">The top row index.</param>
        /// <param name="left">The left column index.</param>
        /// <param name="bottom">The bottom row index.</param>
        /// <param name="right">The right column index.</param>
        public CellSpanBackgroundInfo(int top, int left, int bottom, int right)
            : base(top, left, bottom, right)
        {
        }

        /// <summary>
        /// Gets or sets the border.
        /// </summary>
        /// <value>The border.</value>
        public Pen Border
        {
            get { return pen; }
            set { pen = value; }
        }

        /// <summary>
        /// Gets or sets the background.
        /// </summary>
        /// <value>The background.</value>
        public Brush Background
        {
            get { return background; }
            set { background = value; }
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

            CellSpanBackgroundInfo other = obj as CellSpanBackgroundInfo;
            if (other == null)
                return false;

            return other.pen == pen
                && other.background == background;
        }

        /// <summary>
        /// Returns a string describing the state of the object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0} ( Top = {1} Left = {2} Bottom = {3} Right = {4} Background = {5} Border = {6} )",
               GetType().Name, Top, Left, Bottom, Right, Background, Border);
        }

        public override void Dispose()
        {
            background = null;
            Background = null;
            pen = null;
            Border = null;
        }
    }
}
