#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Drawing;

using Syncfusion.Pdf.Graphics;

namespace Syncfusion.Pdf.Grid
{
    /// <summary>
    /// Arguments of BeginPageLayoutEvent.
    /// </summary>
    /// <seealso cref="BeginPageLayoutEventArgs"/> Class    
    public class PdfGridBeginPageLayoutEventArgs : BeginPageLayoutEventArgs
    {
        #region Fields
        private int m_startRow;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the start row.
        /// </summary>
        /// <value>The start row.</value>
        public int StartRowIndex
        {
            get
            {
                return m_startRow;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfGridBeginPageLayoutEventArgs"/> class.
        /// </summary>
        /// <param name="bounds">The bounds.</param>
        /// <param name="page">The page.</param>
        /// <param name="startRow">The start row.</param>
        internal PdfGridBeginPageLayoutEventArgs(RectangleF bounds, PdfPage page, int startRow)
            : base(bounds, page)
        {
            m_startRow = startRow;
        }

        #endregion
    }

    /// <summary>
    /// Arguments of EndPageLayoutEvent.
    /// </summary>
    /// <seealso cref="EndPageLayoutEventArgs"/> Class    
    public class PdfGridEndPageLayoutEventArgs : EndPageLayoutEventArgs
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfGridEndPageLayoutEventArgs"/> class.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="endRow">The end row.</param>
        internal PdfGridEndPageLayoutEventArgs(PdfLayoutResult result)
            : base(result)
        {
        }
        #endregion
    }
}
