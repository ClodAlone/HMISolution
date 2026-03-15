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
using System.Text;

using Syncfusion.Pdf.Graphics;

namespace Syncfusion.Pdf.Grid
{
    public class PdfGridColumn
    {
        #region Fields
        private PdfGrid m_grid;
        private float m_width = float.MinValue;
        private PdfStringFormat m_format;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the width.
        /// </summary>
        /// <value>The width.</value>
        public float Width
        {
            get
            {
                return m_width;
            }
            set
            {
                m_width = value;
            }
        }

        /// <summary>
        /// Gets or sets the format.
        /// </summary>
        /// <value>The format.</value>
        public PdfStringFormat Format
        {
            get
            {
                if (m_format == null)
                    m_format = new PdfStringFormat();//GetDefaultFormat();

                return m_format;
            }
            set
            {
                m_format = value;
            }
        }

        /// <summary>
        /// Gets the grid.
        /// </summary>
        /// <value>The grid.</value>
        public PdfGrid Grid
        {
            get
            {
                return m_grid;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfGridColumn"/> class.
        /// </summary>
        /// <param name="grid">The grid.</param>
        public PdfGridColumn(PdfGrid grid)
        {
            m_grid = grid;
        }
        #endregion

        #region Helper Methods
        private PdfStringFormat GetDefaultFormat()
        {
            PdfStringFormat format = new PdfStringFormat();
            format.LineAlignment = PdfVerticalAlignment.Middle;
            format.Alignment = PdfTextAlignment.Left;
            return format;
        }
        #endregion
    }

    public class PdfGridColumnCollection : IEnumerable
    {
        #region Fields
        private PdfGrid m_grid;
        private List<PdfGridColumn> m_columns = new List<PdfGridColumn>();
        private float m_width = float.MinValue;
        private float m_previousCellsCount;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the <see cref="Syncfusion.Pdf.Grid.PdfGridColumn"/> at the specified index.
        /// </summary>
        /// <value></value>
        public PdfGridColumn this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                    throw new IndexOutOfRangeException();

                return (m_columns[index]);
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
                return m_columns.Count;
            }
        }

        /// <summary>
        /// Gets the widths.
        /// </summary>
        /// <value>The widths.</value>
        internal float Width
        {
            get
            {
                if (m_width == float.MinValue)
                    m_width = MeasureColumnsWidth();

                return m_width;
            }
        }
        #endregion

        #region Constructor
        public PdfGridColumnCollection(PdfGrid grid)
        {
            m_grid = grid;
            m_columns = new List<PdfGridColumn>();
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Clears this instance.
        /// </summary>
        internal void Clear()
        {
            m_columns.Clear();
        }

        /// <summary>
        /// Adds this instance.
        /// </summary>
        /// <returns></returns>
        public PdfGridColumn Add()
        {
            PdfGridColumn column = new PdfGridColumn(m_grid);
            m_columns.Add(column);
            return column;
        }

        /// <summary>
        /// Adds the specified count.
        /// </summary>
        /// <param name="count">The count.</param>
        public void Add(int count)
        {
            for (int i = 0; i < count; i++)
            {
                m_columns.Add(new PdfGridColumn(m_grid));

                foreach (PdfGridRow row in m_grid.Rows)
                {
                    PdfGridCell cell = new PdfGridCell();
                    cell.Value = "";
                    row.Cells.Add(cell);
                }
            }
        }

        /// <summary>
        /// Adds the specified column.
        /// </summary>
        /// <param name="column">The column.</param>
        public void Add(PdfGridColumn column)
        {
            if (column == null)
                throw new ArgumentNullException("column");

            m_columns.Add(column);
        }

        /// <summary>
        /// Adds the columns.
        /// </summary>
        /// <param name="count">The count.</param>
        internal void AddColumns(int count)
        {
            if (m_previousCellsCount == count)
                return;

            PdfGridColumn column;
            for (int i = count-1; i < count; i++)
            {
                column = new PdfGridColumn(m_grid);
                m_columns.Add(column);
            }

            m_previousCellsCount = count;
        }

        /// <summary>
        /// Calculates the column widths.
        /// </summary>
        internal float MeasureColumnsWidth()
        {
            float totalWidth = 0;

            m_grid.MeasureColumnsWidth();
          
            for (int i = 0, Count = m_columns.Count; i < Count; i++)
            {
                totalWidth += m_columns[i].Width;
            }

            return totalWidth;
        }

        /// <summary>
        /// Gets the widths of the columns.
        /// </summary>
        /// <param name="totalWidth">The total width.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="endColumn">The end column.</param>
        /// <returns>An array containing widths.</returns>
        internal float[] GetDefaultWidths(float totalWidth)
        {
            float[] widths = new float[Count];
            float summ = 0.0f;
            int subFactor = Count; 

            for (int i = 0; i < Count; i++)
            {
                widths[i] = m_columns[i].Width;
                if (m_columns[i].Width > 0)
                {
                    totalWidth -= m_columns[i].Width;
                    subFactor--;
                }
            }

            for (int i = 0; i < Count; i++)
            {
                float width = totalWidth / subFactor;
                if(widths[i] <= 0)
                    widths[i] = width;
            }

            return widths;
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
            return new PdfGridColumnEnumerator(this);
        }
        #endregion

        #region Internals
        /// <summary>
        /// Column collection enumerator.
        /// </summary>
        private struct PdfGridColumnEnumerator : IEnumerator
        {
            #region Fields
            private PdfGridColumnCollection m_columnCollection;
            private int m_currentIndex;
            #endregion

            #region Constructors
            /// <summary>
            /// Initializes a new instance of the <see cref="PdfGridColumnEnumerator"/> struct.
            /// </summary>
            /// <param name="columnCollection">The column collection.</param>
            internal PdfGridColumnEnumerator(PdfGridColumnCollection columnCollection)
            {
                if (columnCollection == null)
                    throw new ArgumentNullException("columnCollection");

                m_columnCollection = columnCollection;
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
                    return m_columnCollection[m_currentIndex];
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

                return (m_currentIndex < m_columnCollection.Count);
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
                if (m_currentIndex < 0 || m_currentIndex >= m_columnCollection.Count)
                    throw new IndexOutOfRangeException();
            }
            #endregion
        }
        #endregion
    }
}
