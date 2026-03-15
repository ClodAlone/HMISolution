#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

#if !SILVERLIGHT

namespace Syncfusion.Layouting
{
    /// <summary>
    /// Summary description for TableLayoutInfo.
    /// </summary>
    internal class TableLayoutInfo : ParagraphLayoutInfo
    {
        #region Fields
        /// <summary>
        /// 
        /// </summary>
        private byte m_verticalAlignment;
        /// <summary>
        /// 
        /// </summary>
        private double m_rowHeight;
        /// <summary>
        /// 
        /// </summary>
        private double m_cellWidth;
        /// <summary>
        /// 
        /// </summary>
        private double m_verticalCellWidth;
        /// <summary>
        /// 
        /// </summary>
        private double m_cellHeight;
        /// <summary>
        /// 
        /// </summary>
        private bool m_bIsRowMergeContinue = false;
        /// <summary>
        /// /
        /// </summary>
        private bool m_bIsRowMergeStart = false;
        /// <summary>
        /// 
        /// </summary>
        private bool m_bIsRowSplitted = false;
        /// <summary>
        /// 
        /// </summary>
        private bool m_bIsExactlyRowHeight = false;
        /// <summary>
        /// 
        /// </summary>
        private bool m_bIsColumnMergeContinue = false;
        /// <summary>
        /// 
        /// </summary>
        private bool m_bIsColumnMergeStart = false;
        /// <summary>
        /// 
        /// </summary>
        private float m_tableCellLeftMargin;
        /// <summary>
        /// 
        /// </summary>
        private float m_tableCellTopMargin;
        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether this instance is column merge start.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is column merge start; otherwise, <c>false</c>.
        /// </value>
        public bool IsColumnMergeStart
        {
            get
            {
                return m_bIsColumnMergeStart;
            }
            set
            {
                m_bIsColumnMergeStart = value;
            }
        }
        /// <summary>
        /// Gets a value indicating whether this instance is column merge continue.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is column merge continue; otherwise, <c>false</c>.
        /// </value>
        public bool IsColumnMergeContinue
        {
            get
            {
                return m_bIsColumnMergeContinue;
            }
            set
            {
                m_bIsColumnMergeContinue = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is exactly row height.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is exactly row height; otherwise, <c>false</c>.
        /// </value>
        public bool IsExactlyRowHeight
        {
            get
            {
                return m_bIsExactlyRowHeight;
            }
            set
            {
                m_bIsExactlyRowHeight = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is row splitted.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is row splitted; otherwise, <c>false</c>.
        /// </value>
        public bool IsRowSplitted
        {
            get
            {
                return m_bIsRowSplitted;
            }
            set
            {
                m_bIsRowSplitted = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is row merge start.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is row merge start; otherwise, <c>false</c>.
        /// </value>
        public bool IsRowMergeStart
        {
            get
            {
                return m_bIsRowMergeStart;
            }
            set
            {
                m_bIsRowMergeStart = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is row merge continue.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is row merge continue; otherwise, <c>false</c>.
        /// </value>
        public bool IsRowMergeContinue
        {
            get
            {
                return m_bIsRowMergeContinue;
            }
            set
            {
                m_bIsRowMergeContinue = value;
            }
        }

        /// <summary>
        /// Gets or sets the TableCell LeftMargin.
        /// </summary>
        /// <value>The TableCell LeftMargin.</value>
        public float TableCellLeftMargin
        {
            get
            {
                return m_tableCellLeftMargin;
            }
            set
            {
                m_tableCellLeftMargin = value;
            }
        }

        /// <summary>
        /// Gets or sets the TableCell TopMargin.
        /// </summary>
        /// <value>The TableCell TopMargin.</value>
        public float TableCellTopMargin
        {
            get
            {
                return m_tableCellTopMargin;
            }
            set
            {
                m_tableCellTopMargin = value;
            }
        }
        /// <summary>
        /// Gets or sets the width of the table cell
        /// </summary>
        /// <value>The Width of the cell</value>
        public double CellWidth
        {
            get
            {
                return m_cellWidth;
            }
            set
            {
                m_cellWidth= value;
            }
        }
        /// <summary>
        /// Gets or sets the width of the cell with Text Direction as Vertical
        /// </summary>
        /// <value>The Width of the cell</value>
        public double VerticalCellWidth
        {
            get
            {
                return m_verticalCellWidth;
            }
            set
            {
                m_verticalCellWidth = value;
            }
        }
        /// <summary>
        /// Gets or sets the height of the cell with Text Direction as Vertical
        /// </summary>
        /// <value>The height of the cell</value>
        public double CellHeight
        {
            get
            {
                return m_cellHeight;
            }
            set
            {
                m_cellHeight = value;
            }
        }
        /// <summary>
        /// Gets or sets the height of the row.
        /// </summary>
        /// <value>The height of the row.</value>
        public double RowHeight
        {
            get
            {
                if (m_rowHeight < 0)
                    m_rowHeight = -m_rowHeight;
                return m_rowHeight;
            }
        }

        /// <summary>
        /// Gets the vertical alignment.
        /// </summary>
        /// <value>The vertical alignment.</value>
        public byte VerticalAlignment
        {
            get
            {
                return m_verticalAlignment;
            }
            set
            {
                m_verticalAlignment = value;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutTableInfo"/> class.
        /// </summary>
        /// <param name="childLayoutDirection">The child layout direction.</param>
        public TableLayoutInfo(ChildrenLayoutDirection childLayoutDirection)
            : base(childLayoutDirection)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutTableInfo"/> class.
        /// </summary>
        /// <param name="isExactlyRow">if set to <c>true</c> [is exactly row].</param>
        /// <param name="rowHeight">Height of the row.</param>
        public TableLayoutInfo(bool isExactlyRow, float rowHeight)
            : base()
        {
            m_bIsExactlyRowHeight = isExactlyRow;
            m_rowHeight = rowHeight;
        }
        #endregion
    }
}

#endif
