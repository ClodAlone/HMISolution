#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;

#if ( WINRT )
using Windows.UI;
#endif

#if  (SILVERLIGHT || WP)
using System.Windows.Media;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;

#endif

namespace Syncfusion.XlsIO.Implementation.Sorting
{
    /// <summary>
    /// This class used to Sort the Range.
    /// </summary>
    class DataSorter : IDataSort
    {
        #region Members
        /// <summary>
        /// Indicates whether to perform case sensitive sort.
        /// </summary>
        private bool m_bIsCaseSensitive;
        /// <summary>
        /// Indicates whether the range has header.
        /// </summary>
        private bool m_bHasHeader;
        /// <summary>
        /// Represents the sort orientation.
        /// </summary>
        private SortOrientation m_orientation;
        /// <summary>
        /// Represents the SortFields Collection.
        /// </summary>
        private ISortFields m_sortFields;
        /// <summary>
        /// Represents	 the sort range.
        /// </summary>
        private IRange m_sortRange;
        /// <summary>
        /// Represents the algorithm to sort.
        /// </summary>
        private SortingAlgorithms m_algorithm;
        /// <summary>
        /// Represents the parent object.
        /// </summary>
        private IWorkbook m_parentSheet;
        /// <summary>
        /// Represents the instance of the ISortingAlorithm type.
        /// </summary>
        private ISortingAlgorithm m_customAlgorithm;
        #endregion

        #region Properties
        /// <summary>
        /// Represents the instance of the ISortingAlorithm type.
        /// </summary>
        private ISortingAlgorithm CustomAlgorithm
        {
            get
            {
                return m_customAlgorithm;
            }
            set
            {
                m_customAlgorithm = value;
            }
        }
        /// <summary>
        /// Indicates whether to perform case sensitive sort.
        /// </summary>
        public bool IsCaseSensitive
        {
            get
            {
                return m_bIsCaseSensitive;
            }
            set
            {
                m_bIsCaseSensitive = value;
            }
        }
        /// <summary>
        /// Indicates whether the range has header.
        /// </summary>
        public bool HasHeader
        {
            get
            {
                return m_bHasHeader;
            }
            set
            {
                m_bHasHeader = value;
            }
        }
        /// <summary>
        /// Represents the sort orientation.
        /// </summary>
        public SortOrientation Orientation
        {
            get
            {
                return m_orientation;
            }
            set
            {
                m_orientation = value;
            }
        }
        /// <summary>
        /// Represents the SortFields Collection.
        /// </summary>
        public ISortFields SortFields
        {
            get
            {
                return m_sortFields;
            }
            set
            {
                m_sortFields = value;
            }
        }
        /// <summary>
        /// Represents	 the sort range.
        /// </summary>
        public IRange SortRange
        {
            get
            {
                return m_sortRange;
            }
            set
            {
                m_sortRange = value;
            }
        }
        /// <summary>
        /// Represents the algorithm to sort.
        /// </summary>
        public SortingAlgorithms Algorithm
        {
            get
            {
                return m_algorithm;
            }
            set
            {
                m_algorithm = value;
            }
        }
        #endregion

        #region Intialization
        /// <summary>
        /// Initialization.
        /// </summary>
        /// <param name="parentObject">Represents the parent object.</param>
        internal DataSorter(IWorkbook parentObject)
        {
            m_parentSheet = parentObject;
            this.m_sortFields = new SortFields(parentObject.Application, parentObject);
            this.IsCaseSensitive = false;
            this.HasHeader = true;
            this.Orientation = SortOrientation.TopToBottom;
            this.Algorithm = SortingAlgorithms.QuickSort;

        }
        #endregion

        #region Methods
        /// <summary>
        /// Sorts the range based on the sort fields.
        /// </summary>
        public void Sort()
        {
            if (SortRange == null)
                throw new ArgumentNullException("Sort Range");


            int[] iColumns = new int[m_sortFields.Count];
            OrderBy[] orderBy = new OrderBy[iColumns.Length];
            Color[] colors = new Color[iColumns.Length];
            int i = 0;
            foreach (ISortField sortField in m_sortFields)
            {
                if (sortField.SortOn == SortOn.CellColor || sortField.SortOn == SortOn.FontColor)
                    colors[i] = sortField.Color;
                iColumns[i] = sortField.Key;
                orderBy[i] = sortField.Order;
                i++;

            }
            SortBy(iColumns, orderBy, colors);
        }
        /// <summary>
        /// Sorts the range with the given parameter.
        /// </summary>
        /// <param name="iColumns">Column indexes to sort.</param>
        /// <param name="orderBy">Represents the sort order.</param>
        /// <param name="colors">Colors to sort by.</param>
        public void SortBy(int[] iColumns, OrderBy[] orderBy, Color[] colors)
        {
            Type[] columnTypes = new Type[iColumns.Length];

            object[][] sortableData = null;

            if (this.Orientation == SortOrientation.TopToBottom)
                sortableData = SortableData(SortRange, columnTypes, iColumns);
            else
                sortableData = SortableDataColumn(SortRange, columnTypes, iColumns);

            SortingAlgorithm sorter;

            if (m_sortFields[0].SortOn == SortOn.Values)
            {
                bool isAscending = false, isTopToBottom = false;
                if (m_orientation == SortOrientation.TopToBottom)
                    isTopToBottom = true;
                switch (Algorithm)
                {
                    case SortingAlgorithms.QuickSort:
                        sorter = new QuickSort3Impl(sortableData, columnTypes, orderBy, colors);
                        sorter.Sort(0, sortableData.Length - 1, 1);

                        if (m_orientation == SortOrientation.TopToBottom)
                            SwapManager(SortRange, sorter.Data);
                        else
                            SwapManagerColumnWise(SortRange, sorter.Data);
                        break;

                    case SortingAlgorithms.InsertionSort:
                        sorter = new InsertionSortImpl(sortableData, columnTypes, orderBy, colors);
                        sorter.Sort(0, sortableData.Length - 1, 1);
                        if (m_orientation == SortOrientation.TopToBottom)
                            SwapManager(SortRange, sorter.Data);
                        else
                            SwapManagerColumnWise(SortRange, sorter.Data);
                        break;

                    case SortingAlgorithms.MergeSort:
                        sorter = new MergeSortImpl(sortableData, columnTypes, orderBy, colors);
                        sorter.Sort(0, sorter.Data.Length - 1, 1);
                        if (m_orientation == SortOrientation.TopToBottom)
                            SwapManager(SortRange, sorter.Data);
                        else
                            SwapManagerColumnWise(SortRange, sorter.Data);
                        break;

                   
                    case SortingAlgorithms.HeapSort:
                        sorter = new HeapSortImpl(sortableData, columnTypes, orderBy, colors);
                        sorter.Sort(0, sorter.Data.Length, 1);
                        if (columnTypes.Length > 1)
                        {
                            sorter = new MergeSortImpl(sorter.Data, columnTypes, orderBy, colors);
                            sorter.Sort(0, sorter.Data.Length - 1, 1);
                        }
                        if (m_orientation == SortOrientation.TopToBottom)
                            SwapManager(SortRange, sorter.Data);
                        else
                            SwapManagerColumnWise(SortRange, sorter.Data);
                        break;
                }
            }
            else
            {
                sorter = new StyleSorting(sortableData, columnTypes, orderBy, colors);
                sorter.Sort(0, sorter.Data.Length - 1, 1);

                for (int i = 1; i < m_sortFields.Count; i++)
                    if (m_sortFields[i].SortOn != SortOn.Values)
                        sorter.Sort(0, sorter.Data.Length - 1, i + 1);

                if (m_orientation == SortOrientation.TopToBottom)
                    SwapManager(SortRange, sorter.Data);
                else
                    SwapManagerColumnWise(SortRange, sorter.Data);
            }
        }
        /// <summary>
        /// Gets the data from the range to sort.
        /// </summary>
        /// <param name="range">Range to sort.</param>
        /// <param name="columnTypes">Represents the column Data Type</param>
        /// <param name="iColumns">Columns index to sort.</param>
        /// <returns>Data in object array to sort.</returns>
        internal object[][] SortableData(IRange range, Type[] columnTypes, int[] iColumns)
        {

            int row = range.Row;
            if (HasHeader)
                row = range.Row + 1;
            int lastRow = range.LastRow;
            int column = range.Column;
            int lastColumn = range.LastColumn;
            int rowCount = (lastRow - row) + 1;
            int columnCount = iColumns.Length;
            object[][] sortable = new object[rowCount][];
            int sortRow = 0, sortColumn = 0;
            int[] types = new int[columnTypes.Length];
            foreach (int iColumn in iColumns)
            {
                int type;
                columnTypes[sortColumn] = GetColumnType(range[row, iColumn + 1], out type);
                types[sortColumn++] = type;
            }
            SortFields sortFieldsImpl = m_sortFields as SortFields;

            for (int i = row, counter = 0; i <= lastRow; i++, sortRow++, counter++)
            {
                sortColumn = 1;
                sortable[sortRow] = new object[iColumns.Length + 1];
                sortable[sortRow][0] = counter;
                foreach (int iColumnT in iColumns)
                {
                    switch (m_sortFields[sortFieldsImpl.FindByKey(iColumnT)].SortOn)
                    {
                        case SortOn.Values:
                            sortable[sortRow][sortColumn] = GetValue(range[i, iColumnT + 1], types[sortColumn - 1]);
                            break;
                        case SortOn.CellColor:
                            sortable[sortRow][sortColumn] = range[i, iColumnT + 1].CellStyle.Color;
                            break;
                        case SortOn.FontColor:
                            sortable[sortRow][sortColumn] = range[i, iColumnT + 1].CellStyle.Font.RGBColor;
                            break;
                    }

                    sortColumn++;
                }
            }
            return sortable;
        }
        /// <summary>
        /// Gets the data from the range to sort in column wise.
        /// </summary>
        /// <param name="range">Range to sort.</param>
        /// <param name="rowTypes">Represents the row datatypes.</param>
        /// <param name="iRows">Row indexes to sort.</param>
        /// <returns>data in object array to sort.</returns>
        internal object[][] SortableDataColumn(IRange range, Type[] rowTypes, int[] iRows)
        {

            int row = range.Row;
            int lastRow = range.LastRow;
            int column = range.Column;
            if (HasHeader)
                column++;
            int lastColumn = range.LastColumn;
            int columnCount = (lastColumn - column) + 1;
            int rowCount = iRows.Length;
            object[][] sortable = new object[columnCount][];
            int sortRow = 0, sortColumn = 0;
            int[] types = new int[rowTypes.Length];
            foreach (int iRow in iRows)
            {
                int type;
                rowTypes[sortColumn] = GetColumnType(range[iRow + 1, column], out type);
                types[sortColumn++] = type;
            }

            int counter = 0;
            int valueRowIndex = 0;

            for (int i = 0; i < rowCount; i++)
            {
                int startColumn = column;


                valueRowIndex = iRows[i];
                for (int j = 0; j < columnCount; j++)
                {


                    sortable[j] = new object[rowCount + 1];
                    sortable[j][i] = counter++;
                    switch (m_sortFields[i].SortOn)
                    {
                        case SortOn.Values:
                            sortable[j][i + 1] = GetValue(range[valueRowIndex + 1, startColumn++], types[i]);
                            break;
                        case SortOn.CellColor:
                            sortable[j][i + 1] = range[valueRowIndex + 1, startColumn++].CellStyle.Color;
                            break;
                        case SortOn.FontColor:
                            sortable[j][i + 1] = range[valueRowIndex + 1, startColumn++].CellStyle.Font.RGBColor;
                            break;
                    }

                    sortColumn++;

                }
            }


            return sortable;
        }
        /// <summary>
        /// Gets the value based on the column type.
        /// </summary>
        /// <param name="range">Cell range to get the value.</param>
        /// <param name="type">Type of the column.</param>
        /// <returns>value based on the column type.</returns>
        internal object GetValue(IRange range, int type)
        {
            switch (type)
            {
                case 1:
                    return range.FormulaDateTime;
                case 2:
                    return range.FormulaNumberValue;
                case 3:
                    return range.FormulaStringValue;
                case 4:
                    return range.Number;
                case 5:
                    return range.DateTime;
                case 6:
                    return range.DisplayText;
            }
            return range.Value;
        }
        /// <summary>
        /// Swaps the Range based on the List.
        /// </summary>
        /// <param name="range">Range to sort.</param>
        /// <param name="sortedResult">Represents the sorted data.</param>
        internal void SwapManager(IRange range, object[][] sortedResult)
        {
            int startRow = 0;

            if ((int)sortedResult[0][0] != -1)
                SwapManager(range, sortedResult, startRow);
            for (int i = 1; i < sortedResult.Length; i++)
            {
                if (sortedResult[i] !=null && (int)sortedResult[i][0] != -1)
                {
                    SwapManager(range, sortedResult, i);
                }
            }

        }
        /// <summary>
        /// Swaps the Range based on the List.
        /// </summary>
        /// <param name="range">Range to sort.</param>
        /// <param name="sortedResult">Represents the sorted data.</param>
        internal void SwapManagerColumnWise(IRange range, object[][] sortedResult)
        {
            int startRow = 0;

            if ((int)sortedResult[0][0] != -1)
                SwapManagerColumnWise(range, sortedResult, startRow);
            for (int i = 0; i < sortedResult.Length; i++)
            {
                if ((int)sortedResult[i][0] != -1)
                {
                    SwapManagerColumnWise(range, sortedResult, i);
                }
            }

        }
        /// <summary>
        /// Swaps the Range based on the List.
        /// </summary>
        /// <param name="range">Range to sort.</param>
        /// <param name="sortedResult">Represents the sorted data.</param>
        /// <param name="startRow">Start row to swap the range data.</param>
        internal void SwapManagerColumnWise(IRange range, object[][] sortedResult, int startRow)
        {

            int row = range.Row;
            int lastRow = range.LastRow;
            int column = range.Column;
            if (HasHeader)
                column++;
            int lastColumn = range.LastColumn;
            IWorksheet sheet = range.Worksheet;
            int tmpColumn = lastColumn + 1;
            sheet.InsertColumn(tmpColumn);
            IRange tmpRange = sheet[row, tmpColumn, lastRow, tmpColumn];
            sheet[row, column + startRow, lastRow, column + startRow].CopyTo(tmpRange, ExcelCopyRangeOptions.All);
            int index = 0;
            int i = startRow;
            do
            {
                index = (int)sortedResult[i][0];
                if (index == startRow || index == -1)
                    break;
                sheet[row, column + index, lastRow, column + index].CopyTo(sheet[row, column + i, lastRow, column + i], ExcelCopyRangeOptions.All);
                sortedResult[i][0] = -1;
                i = index;
            } while (index != startRow);
            tmpRange.CopyTo(sheet[row, column + i, lastRow, column + i]);
            sheet.DeleteColumn(tmpColumn);
            sortedResult[i][0] = -1;
        }
        /// <summary>
        /// Swaps the Range based on the List.
        /// </summary>
        /// <param name="range">Range to sort.</param>
        /// <param name="sortedResult">Represents the sorted data.</param>
        /// <param name="startRow">Start row to swap the range data.</param>
        internal void SwapManager(IRange range, object[][] sortedResult, int startRow)
        {

            int row = range.Row;
            if (HasHeader)
                row = range.Row + 1;
            int lastRow = range.LastRow;
            int column = range.Column;
            int lastColumn = range.LastColumn;
            int rowCount = (lastRow - row) + 1;
            IWorksheet sheet = range.Worksheet;
            int tmpRowIndex = range.LastRow + 1;
            IRange tmpRange = sheet[tmpRowIndex, column, tmpRowIndex, lastColumn];
            sheet.InsertRow(tmpRowIndex);
            sheet[row + startRow, column, row + startRow, lastColumn].CopyTo(tmpRange, ExcelCopyRangeOptions.All);
            int i = startRow;
            int rowIndex = i;
            do
            {
                if (sortedResult[i] == null)
                    break;
                    rowIndex = (int)sortedResult[i][0];
                    if (rowIndex == startRow || rowIndex == -1)
                        break;
                    sheet[rowIndex + row, column, rowIndex + row, lastColumn].CopyTo(sheet[i + row, column, i + row, lastColumn], ExcelCopyRangeOptions.All);
                    sortedResult[i][0] = -1;
                    i = rowIndex;
                
            } while (rowIndex != startRow);
            tmpRange.CopyTo(sheet[i + row, column, i + row, lastColumn], ExcelCopyRangeOptions.All);
            sheet.DeleteRow(tmpRange.Row);
            if(sortedResult[i] !=null)
            sortedResult[i][0] = -1;
        }
        /// <summary>
        /// Gets the cell type.
        /// </summary>
        /// <param name="range">Range to get the cell type.</param>
        /// <returns>Type of the cell.</returns>
        internal Type GetColumnType(IRange range, out int iType)
        {

            if (range.HasFormula)
            {
                if (range.HasFormulaDateTime)
                {
                    iType = 1;
                    return typeof(DateTime);
                }
                if (range.HasFormulaStringValue)
                {
                    iType = 3;
                    return typeof(string);
                }
                if (range.HasFormulaNumberValue)
                {
                    iType = 2;
                    return typeof(double);
                }

            }

            if (range.HasNumber)
            {
                iType = 4;
                return typeof(double);
            }
            if (range.HasString)
            {
                iType = 6;
                return typeof(string);
            }
            if (range.HasDateTime)
            {
                iType = 5;
                return typeof(DateTime);
            }
            iType = 6;
            return typeof(string);
        }
        #endregion
    }
}
