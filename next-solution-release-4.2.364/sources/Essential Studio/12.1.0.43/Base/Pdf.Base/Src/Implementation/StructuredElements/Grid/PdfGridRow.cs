#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

using Syncfusion.Pdf.Graphics;

namespace Syncfusion.Pdf.Grid
{
    public class PdfGridRow
    {
        #region Fields
        private PdfGridCellCollection m_cells;
        private PdfGrid m_grid;
        private PdfGridRowStyle m_style;
        private float m_height = float.MinValue;
        private float m_width = float.MinValue;
        private bool m_bRowSpanExists;
        private bool m_bColumnSpanExists;
        private float m_rowBreakHeight;
        private int m_rowOverflowIndex;
        private PdfLayoutResult m_gridResult;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the cells.
        /// </summary>
        /// <value>The cells.</value>
        public PdfGridCellCollection Cells
        {
            get
            {
                if (m_cells == null)
                    m_cells = new PdfGridCellCollection(this);

                return m_cells;
            }
        }

        /// <summary>
        /// Gets or sets the parent grid.
        /// </summary>
        /// <value>The parent grid.</value>
        internal PdfGrid Grid
        {
            get
            {
                return m_grid;
            }
            set
            {
                m_grid = value;
            }
        }

        /// <summary>
        /// Gets or sets the row style.
        /// </summary>
        /// <value>The row style.</value>
        public PdfGridRowStyle Style
        {
            get
            {
                if (m_style == null)
                    m_style = new PdfGridRowStyle();

                return m_style;
            }
            set
            {
                m_style = value;
            }
        }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        public float Height
        {
            get
            {
                if (m_height == float.MinValue)
                    m_height = MeasureHeight();

                return m_height;
            }
            set
            {
                m_height = value;
            }
        }

        /// <summary>
        /// Gets the width.
        /// </summary>
        /// <value>The width.</value>
        internal float Width
        {
            get
            {
                if (m_width == float.MinValue)
                    m_width = MeasureWidth();

                return m_width;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [row span exists].
        /// </summary>
        /// <value><c>true</c> if [row span exists]; otherwise, <c>false</c>.</value>
        internal bool RowSpanExists
        {
            get
            {
                return m_bRowSpanExists;
            }
            set
            {
                m_bRowSpanExists = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [column span exists].
        /// </summary>
        /// <value><c>true</c> if [column span exists]; otherwise, <c>false</c>.</value>
        internal bool ColumnSpanExists
        {
            get
            {
                return m_bColumnSpanExists;
            }
            set
            {
                m_bColumnSpanExists = value;
            }
        }

        /// <summary>
        /// Height of the row yet to be drawn after split.
        /// </summary>
        internal float RowBreakHeight
        {
            get
            {
                return m_rowBreakHeight;
            }
            set
            {
                m_rowBreakHeight = value;
            }
        }

        /// <summary>
        /// Gets or sets the index of the cell at which the row breaks when AllowHorizonalOverflow is true.
        /// </summary>
        internal int RowOverflowIndex
        {
            get
            {
                return m_rowOverflowIndex;
            }
            set
            {
                m_rowOverflowIndex = value;
            }
        }

        /// <summary>
        /// Holds the result of nested grid.
        /// </summary>
        internal PdfLayoutResult NestedGridLayoutResult
        {
            get
            {
                return m_gridResult;
            }
            set
            {
                m_gridResult = value;
            }
        }

        /// <summary>
        /// Returns index of the row.
        /// </summary>
        internal int RowIndex
        {
            get
            {
                return Grid.Rows.IndexOf(this);
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfGridRow"/> class.
        /// </summary>
        /// <param name="parentGrid">The parent grid.</param>
        public PdfGridRow(PdfGrid grid)
        {
            m_grid = grid;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Calculates the height.
        /// </summary>
        /// <returns></returns>
        private float MeasureHeight()
        {
            float rowHeight = Cells[0].Height;

            foreach (PdfGridCell cell in Cells)
            {
                if (cell.ColumnSpan == 1 || cell.RowSpan == 1)
                    rowHeight = Math.Max(rowHeight, cell.Height);
                else
                    rowHeight = Math.Min(rowHeight, cell.Height);

                cell.Height = rowHeight;
            }
            return rowHeight;
        }

        /// <summary>
        /// Measures the width.
        /// </summary>
        /// <returns></returns>
        private float MeasureWidth()
        {
            float width = 0;

            foreach (PdfGridColumn column in Grid.Columns)
            {
                width += column.Width;
            }

            return width;
        }

        /// <summary>
        /// Applies the cell style to all the cells present in a row.
        /// </summary>
        /// <param name="cellStyle">The cell style.</param>
        public void ApplyStyle(PdfGridCellStyle cellStyle)
        {
            foreach (PdfGridCell cell in Cells)
            {
                cell.Style = cellStyle;
            }
        }
        #endregion
       
    }

    public class PdfGridRowCollection : List<PdfGridRow>
    {
        #region Fields
        private PdfGrid m_grid;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfGridRowCollection"/> class.
        /// </summary>
        internal PdfGridRowCollection(PdfGrid grid)
        {
            m_grid = grid;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Adds this instance.
        /// </summary>
        /// <returns></returns>
        public PdfGridRow Add()
        {
            PdfGridRow row = new PdfGridRow(m_grid);
            
            Add(row);
            return row;
        }

        /// <summary>
        /// Adds this instance.
        /// </summary>
        public void Add(PdfGridRow row)
        {
            row.Style = ((PdfGridStyleBase)m_grid.Style as PdfGridRowStyle);

            if (row.Cells.Count == 0)
            {
                for (int i = 0; i < m_grid.Columns.Count; i++)
                    row.Cells.Add(new PdfGridCell());
            }

            base.Add(row);
        }

        /// <summary>
        /// Sets the span.
        /// </summary>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="cellIndex">Index of the cell.</param>
        /// <param name="rowSpan">The row span.</param>
        /// <param name="colSpan">The col span.</param>
        public void SetSpan(int rowIndex, int cellIndex, int rowSpan, int colSpan)
        {
            if (rowIndex > m_grid.Rows.Count)
                throw new IndexOutOfRangeException("rowIndex");

            if (cellIndex > m_grid.Columns.Count)
                throw new IndexOutOfRangeException("cellIndex");

            m_grid.Rows[rowIndex].Cells[cellIndex].RowSpan = rowSpan;
            m_grid.Rows[rowIndex].Cells[cellIndex].ColumnSpan = colSpan;
        }

        /// <summary>
        /// Applies the style.
        /// </summary>
        /// <param name="style">The style.</param>
        public void ApplyStyle(PdfGridStyleBase style)
        {
            if (style is PdfGridCellStyle)
            {
                foreach (PdfGridRow row in m_grid.Rows)
                {
                    foreach (PdfGridCell cell in row.Cells)
                    {
                        cell.Style = (style as PdfGridCellStyle);
                    }
                }
            }
            else if (style is PdfGridRowStyle)
            {
                foreach (PdfGridRow row in m_grid.Rows)
                {
                    row.Style = (style as PdfGridRowStyle);
                }
            }
        }
        #endregion
    }

    public class PdfGridHeaderCollection : IEnumerable
    {
        #region Fields
        private PdfGrid m_grid;
        private List<PdfGridRow> m_rows = new List<PdfGridRow>();
        #endregion

        #region Properties
        /// <summary>
        /// Gets the <see cref="Syncfusion.Pdf.Grid.PdfGridColumn"/> at the specified index.
        /// </summary>
        /// <value></value>
        public PdfGridRow this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                    throw new IndexOutOfRangeException();

                return (m_rows[index]);
            }
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        public int Count
        {
            get
            {
                return m_rows.Count;
            }
        }
        #endregion

        #region Constructor
        public PdfGridHeaderCollection(PdfGrid grid)
        {
            m_grid = grid;
            m_rows = new List<PdfGridRow>();
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Adds the specified row.
        /// </summary>
        /// <param name="row">The row.</param>
        internal void Add(PdfGridRow row)
        {
            m_rows.Add(row);
        }

        /// <summary>
        /// Adds the specified count.
        /// </summary>
        /// <param name="count">The count.</param>
        public PdfGridRow[] Add(int count)
        {
            PdfGridRow row;
            for (int i = 0; i < count; i++)
            {
                row = new PdfGridRow(m_grid);

                for (int j = 0; j < m_grid.Columns.Count; j++)
                {
                    row.Cells.Add(new PdfGridCell());
                }

                m_rows.Add(row);
            }

            return m_rows.ToArray();
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            m_rows.Clear();
        }

        /// <summary>
        /// Applies the style.
        /// </summary>
        /// <param name="style">The style.</param>
        public void ApplyStyle(PdfGridStyleBase style)
        {
            if (style is PdfGridCellStyle)
            {
                foreach (PdfGridRow row in m_grid.Headers)
                {
                    foreach (PdfGridCell cell in row.Cells)
                    {
                        cell.Style = (style as PdfGridCellStyle);
                    }
                }
            }
            else if (style is PdfGridRowStyle)
            {
                foreach (PdfGridRow row in m_grid.Headers)
                {
                    row.Style = (style as PdfGridRowStyle);
                }
            }
        }

        internal int IndexOf(PdfGridRow row)
        {
            return m_rows.IndexOf(row);
        }
        #endregion

        #region IEnumerable Members
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator GetEnumerator()
        {
            return new PdfGridHeaderRowEnumerator(this);
        }
        #endregion

        #region Internals
        /// <summary>
        /// Column collection enumerator.
        /// </summary>
        private struct PdfGridHeaderRowEnumerator : IEnumerator
        {
            #region Fields
            private PdfGridHeaderCollection m_headerRowCollection;
            private int m_currentIndex;
            #endregion

            #region Constructors
            /// <summary>
            /// Initializes a new instance of the <see cref="PdfGridRowEnumerator"/> struct.
            /// </summary>
            /// <param name="rowCollection">The row collection.</param>
            internal PdfGridHeaderRowEnumerator(PdfGridHeaderCollection rowCollection)
            {
                if (rowCollection == null)
                    throw new ArgumentNullException("rowCollection");

                m_headerRowCollection = rowCollection;
                m_currentIndex = -1;
            }
            #endregion

            #region IEnumerator Members
            /// <summary>
            /// Gets the current.
            /// </summary>
            /// <value>The current.</value>
            public object Current
            {
                get
                {
                    CheckIndex();
                    return m_headerRowCollection[m_currentIndex];
                }
            }

            /// <summary>
            /// Advances the enumerator to the next element of the collection.
            /// </summary>
            /// <returns>
            /// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
            /// </returns>
            /// <exception cref="T:System.InvalidOperationException">
            /// The collection was modified after the enumerator was created.
            /// </exception>
            public bool MoveNext()
            {
                ++m_currentIndex;

                return (m_currentIndex < m_headerRowCollection.Count);
            }

            /// <summary>
            /// Sets the enumerator to its initial position, which is before the first element in the collection.
            /// </summary>
            /// <exception cref="T:System.InvalidOperationException">
            /// The collection was modified after the enumerator was created.
            /// </exception>
            public void Reset()
            {
                m_currentIndex = -1;
            }
            #endregion

            #region Helper methods
            /// <summary>
            /// Checks the index.
            /// </summary>
            private void CheckIndex()
            {
                if (m_currentIndex < 0 || m_currentIndex >= m_headerRowCollection.Count)
                    throw new IndexOutOfRangeException();
            }
            #endregion
        }
        #endregion
    }
}
