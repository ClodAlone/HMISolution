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
using System.ComponentModel;
using System.Drawing;
using System.Text;
#if !SILVERLIGHT && !NETFX_CORE && !WP
using System.Data;
#endif

using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Tables;
using System.Reflection;


namespace Syncfusion.Pdf.Grid
{
    public class PdfGrid : PdfLayoutElement
    {
        #region Fields
        private PdfGridHeaderCollection m_headers;
        private PdfGridRowCollection m_rows;
        private object m_dataSource;
        private string m_dataMember;
#if !SILVERLIGHT && !NETFX_CORE && !WP
        private PdfDataSource m_dsParser;
#endif
        private PdfGridStyle m_style;
        private PdfGridColumnCollection m_columns;
        private bool m_bRepeatHeader;
        private SizeF m_size = SizeF.Empty;
        private bool m_breakRow = true;
        private bool m_isChildGrid;
        private PdfGridCell m_parentCell;
        private float initialWidth = 0.0f;
        /// <summary>
        /// Internal variable to store layout format.
        /// </summary>
        private PdfLayoutFormat m_layoutFormat;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the headers.
        /// </summary>
        /// <value>The headers.</value>
        public PdfGridHeaderCollection Headers
        {
            get
            {
                if (m_headers == null)
                    m_headers = new PdfGridHeaderCollection(this);

                return m_headers;
            }
        }

        /// <summary>
        /// Gets the rows.
        /// </summary>
        /// <value>The rows.</value>
        public PdfGridRowCollection Rows
        {
            get
            {
                if (m_rows == null)
                    m_rows = new PdfGridRowCollection(this);

                return m_rows;
            }
        }

        /// <summary>
        /// Gets or sets the data source.
        /// </summary>
        /// <value>The data source.</value>
        public Object DataSource
        {
            get
            {
                return m_dataSource;
            }
            set
            {
                //if (((value != null) && !(value is IList)))
                //    throw new ArgumentException("Data source not supported.");

                if (value != null && value != m_dataSource)
                {
                    m_dataSource = value;
                    Columns.Clear();
                    SetDataSource();
                }
            }
        }

        /// <summary>
        /// Gets or sets the data member.
        /// </summary>
        /// <value>The data member.</value>
        public string DataMember
        {
            get
            {
                return m_dataMember;
            }
            set
            {
                if (value != null || m_dataMember != value)
                {
                    m_dataMember = value;
                    SetDataSource();
                }
            }
        }

        /// <summary>
        /// Gets or sets the style.
        /// </summary>
        /// <value>The style.</value>
        public PdfGridStyle Style
        {
            get
            {
                if (m_style == null)
                    m_style = new PdfGridStyle();

                return m_style;
            }
            set
            {
                m_style = value;
            }
        }

        /// <summary>
        /// Gets the first row.
        /// </summary>
        /// <value>The first row.</value>
        internal PdfGridRow LastRow
        {
            get
            {
                if (Rows.Count > 0)
                    return Rows[Rows.Count-1];
                else
                    return null;
            }
        }

        /// <summary>
        /// Gets the columns.
        /// </summary>
        /// <value>The columns.</value>
        public PdfGridColumnCollection Columns
        {
            get
            {
                if (m_columns == null)
                    m_columns = new PdfGridColumnCollection(this);

                return m_columns;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [repeat header].
        /// </summary>
        /// <value><c>true</c> if [repeat header]; otherwise, <c>false</c>.</value>
        public bool RepeatHeader
        {
            get
            {
                return m_bRepeatHeader;
            }
            set
            {
                m_bRepeatHeader = value;
            }
        }

        /// <summary>
        /// Gets the size.
        /// </summary>
        /// <value>The size.</value>
        internal SizeF Size
        {
            get
            {
                if (m_size == SizeF.Empty)
                    m_size = Measure();

                return m_size;
            }
        }

        /// <summary>
        /// Gets or sets whether to split or cut rows that overflow a page.
        /// </summary>
        public bool AllowRowBreakAcrossPages
        {
            get
            {
                return m_breakRow;
            }
            set
            {
                m_breakRow = value;
            }
        }

        /// <summary>
        /// Gets or set if grid is nested grid.
        /// </summary>
        internal bool IsChildGrid
        {
            get
            {
                return m_isChildGrid;
            }
            set
            {
                m_isChildGrid = value;
            }
        }

        /// <summary>
        /// Gets or sets the parent cell of the nested grid.
        /// </summary>
        internal PdfGridCell ParentCell
        {
            get
            {
                return m_parentCell;
            }
            set
            {
                m_parentCell = value;
            }
        }

        /// <summary>
        /// Gets layout format of the grid.
        /// </summary>
        internal PdfLayoutFormat LayoutFormat
        {
            get
            {
                return m_layoutFormat;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfGrid"/> class.
        /// </summary>
        public PdfGrid()
        {
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Draws the specified graphics.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <param name="location">The location.</param>
        /// <param name="width">The width.</param>
        public void Draw(PdfGraphics graphics, PointF location, float width)
        {
            Draw(graphics, location.X, location.Y, width);
        }

        /// <summary>
        /// Draws the specified graphics.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        public void Draw(PdfGraphics graphics, float x, float y, float width)
        {
            initialWidth = width;
            RectangleF boundaries = new RectangleF(x, y, width, 0);
            Draw(graphics, boundaries);
        }

        /// <summary>
        /// Draws the specified graphics.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <param name="bounds">The bounds.</param>
        public void Draw(PdfGraphics graphics, RectangleF bounds)
        {
            SetSpan();
            initialWidth = bounds.Width;
            PdfGridLayouter layouter = new PdfGridLayouter(this);
            layouter.Layout(graphics, bounds);
        }

        /// <summary>
        /// Draws the specified page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="location">The location.</param>
        /// <returns></returns>
        new public PdfGridLayoutResult Draw(PdfPage page, PointF location)
        {
            initialWidth = page.Graphics.ClientSize.Width;
            PdfLayoutResult lr = base.Draw(page, location);
            return (PdfGridLayoutResult)lr;
        }

        /// <summary>
        /// Draws the specified page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="location">The location.</param>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        public PdfGridLayoutResult Draw(PdfPage page, PointF location,
            PdfGridLayoutFormat format)
        {
            initialWidth = page.Graphics.ClientSize.Width;
            PdfLayoutResult lr = base.Draw(page, location, format);
            return (PdfGridLayoutResult)lr;
        }

        /// <summary>
        /// Draws the specified page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="bounds">The bounds.</param>
        /// <returns></returns>
        new public PdfGridLayoutResult Draw(PdfPage page, RectangleF bounds)
        {
            initialWidth = bounds.Width;
            PdfLayoutResult lr = base.Draw(page, bounds);
            return (PdfGridLayoutResult)lr;
        }

        /// <summary>
        /// Draws the specified page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        public PdfGridLayoutResult Draw(PdfPage page, RectangleF bounds,
            PdfGridLayoutFormat format)
        {
            initialWidth = bounds.Width;
            PdfLayoutResult lr = base.Draw(page, bounds, format);
            return (PdfGridLayoutResult)lr;
        }

        /// <summary>
        /// Draws the specified page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        new public PdfGridLayoutResult Draw(PdfPage page, float x, float y)
        {
            initialWidth = page.Graphics.ClientSize.Width;
            PdfLayoutResult lr = base.Draw(page, x, y);
            return (PdfGridLayoutResult)lr;
        }

        /// <summary>
        /// Draws the specified page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        public PdfGridLayoutResult Draw(PdfPage page,
            float x, float y, PdfGridLayoutFormat format)
        {
            initialWidth = page.Graphics.ClientSize.Width;
            PdfLayoutResult lr = base.Draw(page, x, y, format);
            return (PdfGridLayoutResult)lr;
        }

        /// <summary>
        /// Draws the specified page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <returns></returns>
        public PdfGridLayoutResult Draw(PdfPage page,
            float x, float y, float width)
        {
            return Draw(page, x, y, width, null);
        }

        /// <summary>
        /// Draws the specified page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        public PdfGridLayoutResult Draw(PdfPage page,
            float x, float y, float width, PdfGridLayoutFormat format)
        {
            RectangleF rect = new RectangleF(x, y, width + x, 0);
            initialWidth = rect.Width;
            PdfLayoutResult lr = base.Draw(page, rect, format);
            return (PdfGridLayoutResult)lr;
        }

        /// <summary>
        /// Layouts the element.
        /// </summary>
        /// <param name="param">Layout parameters.</param>
        /// <returns>Returns the results of layout.</returns>
        protected override PdfLayoutResult Layout(PdfLayoutParams param)
        {
            if (param.Bounds.Width < 0)
                throw new ArgumentOutOfRangeException("Width");

            SetSpan();
            m_layoutFormat = param.Format;
            PdfGridLayouter layouter = new PdfGridLayouter(this);
            PdfGridLayoutResult result = (PdfGridLayoutResult)layouter.Layout(param);

            return result;
        }

        /// <summary>
        /// Draws an element on the Graphics.
        /// </summary>
        /// <param name="graphics">Graphics context where the element should be printed.</param>
        protected override void DrawInternal(PdfGraphics graphics)
        {
            SetSpan();

            PdfGridLayouter layouter = new PdfGridLayouter(this);
            layouter.Layout(graphics, PointF.Empty);
        }
       
        /// <summary>
        /// Applies the span.
        /// </summary>
        private void SetSpan()
        {
            int colSpan, rowSpan = 1;
            int currentCellIndex, currentRowIndex = 0;

            for (int i = 0, RowCount = Headers.Count; i < RowCount; i++)
            {
                PdfGridRow row = Headers[i];
                for (int j = 0, ColCount = row.Cells.Count; j < ColCount; j++)
                {
                    PdfGridCell cell = row.Cells[j];

                    //Skip setting span map for already coverted rows/columns.
                    if (!cell.IsCellMergeContinue && !cell.IsRowMergeContinue
                        && (cell.ColumnSpan > 1 || cell.RowSpan > 1))
                    {
                        if (cell.ColumnSpan + j > row.Cells.Count)
                            throw new ArgumentException(string.Format("Invalid span specified at row {0} column {1}", j.ToString(), i.ToString()));

                        if (cell.RowSpan + i > Rows.Count)
                            throw new ArgumentException(string.Format("Invalid span specified at row {0} column {1}", j.ToString(), i.ToString()));

                        if (cell.ColumnSpan > 1 && cell.RowSpan > 1)
                        {
                            colSpan = cell.ColumnSpan;
                            rowSpan = cell.RowSpan;

                            currentCellIndex = j;
                            currentRowIndex = i;

                            cell.IsCellMergeStart = true;
                            cell.IsRowMergeStart = true;

                            //Set Column merges for first row
                            while (colSpan > 1)
                            {
                                currentCellIndex++;
                                row.Cells[currentCellIndex].IsCellMergeContinue = true;
                                colSpan--;
                            }

                            currentCellIndex = j;
                            colSpan = cell.ColumnSpan;

                            //Set Row Merges and column merges foreach subsequent rows.
                            while (rowSpan > 1)
                            {
                                currentRowIndex++;
                                Headers[currentRowIndex].Cells[j].IsRowMergeContinue = true;
                                rowSpan--;

                                while (colSpan > 1)
                                {
                                    currentCellIndex++;
                                    Headers[currentRowIndex].Cells[currentCellIndex].IsCellMergeContinue = true;
                                    colSpan--;
                                }
                                colSpan = cell.ColumnSpan;
                                currentCellIndex = j;
                            }
                        }
                        else if (cell.ColumnSpan > 1 && cell.RowSpan == 1)
                        {
                            colSpan = cell.ColumnSpan;
                            currentCellIndex = j;
                            cell.IsCellMergeStart = true;

                            //Set Column merges.
                            while (colSpan > 1)
                            {
                                currentCellIndex++;
                                row.Cells[currentCellIndex].IsCellMergeContinue = true;
                                colSpan--;
                            }
                        }
                        else if (cell.ColumnSpan == 1 && cell.RowSpan > 1)
                        {
                            rowSpan = cell.RowSpan;
                            currentRowIndex = i;

                            //Set row Merges.
                            while (rowSpan > 1)
                            {
                                currentRowIndex++;
                                Headers[currentRowIndex].Cells[j].IsRowMergeContinue = true;
                                rowSpan--;
                            }
                        }
                    }

                }
            }

            colSpan = rowSpan = 1;
            currentCellIndex = currentRowIndex = 0;

            for (int i = 0, RowCount = Rows.Count; i < RowCount; i++)
            {
                PdfGridRow row = Rows[i];
                for (int j = 0, ColCount = row.Cells.Count; j < ColCount; j++)
                {
                    PdfGridCell cell = row.Cells[j];

                    //Skip setting span map for already coverted rows/columns.
                    if (!cell.IsCellMergeContinue && !cell.IsRowMergeContinue
                        && (cell.ColumnSpan > 1 || cell.RowSpan > 1))
                    {
                        if (cell.ColumnSpan + j > row.Cells.Count)
                            throw new ArgumentException(string.Format("Invalid span specified at row {0} column {1}", j.ToString(), i.ToString()));

                        if (cell.RowSpan + i > Rows.Count)
                            throw new ArgumentException(string.Format("Invalid span specified at row {0} column {1}", j.ToString(), i.ToString()));

                        if (cell.ColumnSpan > 1 && cell.RowSpan > 1)
                        {
                            colSpan = cell.ColumnSpan;
                            rowSpan = cell.RowSpan;

                            currentCellIndex = j;
                            currentRowIndex = i;

                            cell.IsCellMergeStart = true;
                            cell.IsRowMergeStart = true;

                            //Set Column merges for first row
                            while (colSpan > 1)
                            {
                                currentCellIndex++;
                                row.Cells[currentCellIndex].IsCellMergeContinue = true;
                                colSpan--;
                            }

                            currentCellIndex = j;
                            colSpan = cell.ColumnSpan;

                            //Set Row Merges and column merges foreach subsequent rows.
                            while (rowSpan > 1)
                            {
                                currentRowIndex++;
                                Rows[currentRowIndex].Cells[j].IsRowMergeContinue = true;
                                rowSpan--;

                                while (colSpan > 1)
                                {
                                    currentCellIndex++;
                                    Rows[currentRowIndex].Cells[currentCellIndex].IsCellMergeContinue = true;
                                    colSpan--;
                                }
                                colSpan = cell.ColumnSpan;
                                currentCellIndex = j;
                            }
                        }
                        else if (cell.ColumnSpan > 1 && cell.RowSpan == 1)
                        {
                            colSpan = cell.ColumnSpan;
                            currentCellIndex = j;
                            cell.IsCellMergeStart = true;

                            //Set Column merges.
                            while (colSpan > 1)
                            {
                                currentCellIndex++;
                                row.Cells[currentCellIndex].IsCellMergeContinue = true;
                                colSpan--;
                            }
                        }
                        else if (cell.ColumnSpan == 1 && cell.RowSpan > 1)
                        {
                            rowSpan = cell.RowSpan;
                            currentRowIndex = i;

                            //Set row Merges.
                            while (rowSpan > 1)
                            {
                                currentRowIndex++;
                                Rows[currentRowIndex].Cells[j].IsRowMergeContinue = true;
                                rowSpan--;
                            }
                        }
                    }

                }
            }
        }

        /// <summary>
        /// Measures this instance.
        /// </summary>
        /// <returns></returns>
        private SizeF Measure()
        {
            float height = 0;
            float width = Columns.Width;

             foreach (PdfGridRow row in Headers)
            {
                height += row.Height;
            }

            foreach (PdfGridRow row in Rows)
            {
                height += row.Height;
            }

            return new SizeF(width, height);
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Sets the data source.
        /// </summary>
        private void SetDataSource()
        {
#if !SILVERLIGHT && !NETFX_CORE && !WP
            Array array = m_dataSource as Array;
            DataSet dataSet = m_dataSource as DataSet;
            DataColumn dataColumn = m_dataSource as DataColumn;
            DataTable dataTable = m_dataSource as DataTable;
            DataView dataView = m_dataSource as DataView;

            PdfDataSource ds = null;

            if (array != null)
            {
                ds = new PdfDataSource(array);
            }
            else if (dataColumn != null)
            {
                ds = new PdfDataSource(dataColumn);
            }
            else if (dataTable != null)
            {
                ds = new PdfDataSource(dataTable);
            }
            else if (dataView != null)
            {
                ds = new PdfDataSource(dataView);
            }
            else if (dataSet != null)
            {
                ds = new PdfDataSource(dataSet, m_dataMember);
            }

            m_dsParser = ds;

            PopulateHeader();
            PopulateGrid();
#else
            PopulateGrid();
#endif
        }

#if SILVERLIGHT || NETFX_CORE || WP
        private void PopulateGrid()
        {
            if (m_dataSource is IEnumerable)
            {
                PdfGridRow row;
                PropertyInfo[] props = null;

                foreach (object obj in (m_dataSource as IEnumerable))
                {
                    if (obj != null)
                    {
#if NETFX_CORE || WP
                        IEnumerable<PropertyInfo> enumerable = obj.GetType().GetRuntimeProperties();
                        props = new List<PropertyInfo>(enumerable).ToArray();
#else
                        props = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
#endif
                        this.Columns.Add(props.Length);
                        row = new PdfGridRow(this);
                        foreach (var prop in props)
                        {
                            PdfGridCell cell = new PdfGridCell();
                            cell.Value = prop.Name;
                            row.Cells.Add(cell);
                        }
                        this.Headers.Add(row);
                        
                        break;
                    }
                }

                foreach (var item in (m_dataSource as IEnumerable))
                {
                    row = new PdfGridRow(this);
                    foreach (var prop in props)
                    {
                        Type currentRecordType = item.GetType();
#if NETFX_CORE || WP
                        PropertyInfo property = currentRecordType.GetRuntimeProperty(prop.Name);
#else
                        PropertyInfo property = currentRecordType.GetProperty(prop.Name);
#endif
                        PdfGridCell cell = new PdfGridCell(row);
                        cell.Value = Convert.ToString(property.GetValue(item, null));
                        row.Cells.Add(cell);
                    }
                    Rows.Add(row);
                }
            }
        }
#else
        /// <summary>
        /// Populates the grid.
        /// </summary>
        private void PopulateGrid()
        {
            if (m_dsParser != null)
            {
                int i = 0;
                Rows.Clear();
                while(i < m_dsParser.RowCount)
                {
                    PdfGridRow row = new PdfGridRow(this);
                    string[] rowValues = m_dsParser.GetRow(ref i);
                    for (int j = 0; j < m_dsParser.ColumnCount; j++)
                    {
                        PdfGridCell cell = new PdfGridCell(row);
                        cell.Value = rowValues[j];
                        row.Cells.Add(cell);
                    }
                    Rows.Add(row);
                }
            }
            
            for (int i = 0; i < m_dsParser.ColumnCount; i++)
            {
                Columns.Add(new PdfGridColumn(this));
            }
        }

        /// <summary>
        /// Populates the header row.
        /// </summary>
        private void PopulateHeader()
        {

            Headers.Clear();
            string[] columnNames = m_dsParser.ColumnCaptions;

            if (columnNames == null)
                return;

            PdfGridRow row = new PdfGridRow(this);
            for (int j = 0; j < m_dsParser.ColumnCount; j++)
            {
                PdfGridCell cell = new PdfGridCell(row);
                cell.Value = columnNames[j];
                row.Cells.Add(cell);
            }
            Headers.Add(row);
        }
#endif
        /// <summary>
        /// Calculates the column widths.
        /// </summary>
        internal void MeasureColumnsWidth()
        {
            float[] widths = new float[Columns.Count];
            float cellWidth = 0;

            if (Headers.Count > 0)
            {
                for (int i = 0, ColCount = Headers[0].Cells.Count; i < ColCount; i++)
                {
                    for (int j = 0, RowCount = Headers.Count; j < RowCount; j++)
                    {
                        float rowWidth = initialWidth > 0.0 ? Math.Min(initialWidth, Headers[j].Cells[i].Width) : Headers[j].Cells[i].Width;
                        cellWidth = Math.Max(cellWidth, rowWidth);
                    }
                    widths[i] = cellWidth;
                }
            }

            for (int i = 0, ColCount = Columns.Count; i < ColCount; i++)
            {
                for (int j = 0, RowCount = Rows.Count; j < RowCount; j++)
                {
                    float rowWidth = initialWidth > 0.0 ? Math.Min(initialWidth, Rows[j].Cells[i].Width) : Rows[j].Cells[i].Width;
                    cellWidth =  Math.Max(widths[i], Math.Max(cellWidth, rowWidth));
                    cellWidth = Math.Max(Columns[i].Width, cellWidth);
                }
                widths[i] = cellWidth;
                cellWidth = 0;
            }

            for (int i = 0, Count = Columns.Count; i < Count; i++)
            {
                if (Columns[i].Width < 0)
                    Columns[i].Width = widths[i];
            }
        }

        /// <summary>
        /// Calculates the width of the columns.
        /// </summary>
        /// <param name="bounds">The bounds.</param>
        internal void MeasureColumnsWidth(RectangleF bounds)
        {
            float[] widths = Columns.GetDefaultWidths(bounds.Width - bounds.X);

            for (int i = 0, Count = Columns.Count; i < Count; i++)
            {
                if (Columns[i].Width < 0)
                    Columns[i].Width = widths[i];
            }
        }
        #endregion
    }
}
