#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if !NETFX_CORE && !WP
using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.IO;
using Syncfusion.Compression;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Interactive;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;
using Syncfusion.Pdf.Security;

/// <summary>
/// The Syncfusion.Pdf.Tables namespace contains classes for creating tables.
/// </summary>
namespace Syncfusion.Pdf.Tables
{
    /// <summary>
    /// Represents DataSource for tables
    /// </summary>
    internal class PdfDataSource
    {
        #region Fields
        /// <summary>
        /// Data table 
        /// </summary>
        private DataTable m_dataTable;
        /// <summary>
        /// Rows count
        /// </summary>
        private int m_rowCount = 0;
        /// <summary>
        /// Columns count
        /// </summary>
        private int m_colCount = 0;
        /// <summary>
        /// Data column
        /// </summary>
        private DataColumn m_dataColumn;
        /// <summary>
        /// An array
        /// </summary>
        private Array m_array;
        /// <summary>
        /// Use sorting data
        /// </summary>
        private bool m_useSorting = true;
        /// <summary>
        /// Cached rows
        /// </summary>
        private DataRow[] m_cachRows;
        #endregion

        #region Properties
        /// <summary>
        /// True if use data sorting, otherwise false
        /// </summary>
        internal bool UseSorting
        {
            get
            {
                return m_useSorting;
            }
            set
            {
                m_useSorting = value;
            }
        }

        /// <summary>
        /// Gets rows counts
        /// </summary>
        public int RowCount
        {
            get
            {
                return m_rowCount;
            }
        }

        /// <summary>
        /// Gets column count
        /// </summary>
        public int ColumnCount
        {
            get
            {
                return GetVisibleColCount();
            }
        }

        /// <summary>
        /// Gets visible columns names
        /// </summary>
        public string[] ColumnNames
        {
            get
            {
                return GetColumnsNames();
            }
        }

        /// <summary>
        /// Gets visible columns captions
        /// </summary>
        public string[] ColumnCaptions
        {
            get
            {
                return GetColumnsCaptions();
            }

        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the PdfDataSource class
        /// </summary>
        private PdfDataSource()
        {
        }

        /// <summary>
        /// Initializes a new instance of the PdfDataSource class using DataTable
        /// </summary>
        /// <param name="table">Data table</param>
        public PdfDataSource(DataTable table)
        {
            if (table == null)
            {
                throw new ArgumentNullException("Data table can't be null", "table");
            }

            SetTable(table);
        }

        /// <summary>
        /// Initializes a new instance of the PdfDataSource class using DataSet
        /// </summary>
        /// <param name="dataSet">Data set</param>
        /// <param name="tableName">Table name</param>
        public PdfDataSource(DataSet dataSet, string tableName)
            : this(GetTableFromDataSet(dataSet, tableName))
        {
        }

        /// <summary>
        ///  Initializes a new instance of the PdfDataSource class using DataView
        /// </summary>
        /// <param name="view"> Data View</param>
        public PdfDataSource(DataView view)
            : this(GetTableFromDataView(view))
        {
        }

        /// <summary>
        /// Initializes a new instance of the PdfDataSource class using DataColumn
        /// </summary>
        /// <param name="column">Data column</param>
        public PdfDataSource(DataColumn column)
        {
            if (column == null)
            {
                throw new ArgumentNullException("Column can't be null", "column");
            }

            if (column.Table == null)
            {
                throw new ArgumentNullException("Data column must belong to some table", "column");
            }

            m_dataColumn = column;
            m_colCount = 1;
            m_rowCount = m_dataColumn.Table.Rows.Count;
        }

        /// <summary>
        /// Initializes a new instance of the PdfDataSource class using array
        /// </summary>
        /// <param name="array">Source array</param>
        public PdfDataSource(Array array)
        {
            if (array == null)
            {
                throw new ArgumentException("Array can'n be null", "array");
            }

            if (!IsArrayValid(array, ref m_colCount))
            {
                throw new ArgumentException("We don't suuport more than one or two dimensions" +
                    " arrays in this context or you array has diiferent length", "array");
            }

            m_array = array;
            m_rowCount = m_array.GetLength(0);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets row of data for visible columns
        /// </summary>
        /// <param name="index">The index</param>
        /// <returns>An array of text values or null indicating reaching the end.</returns>
        public string[] GetRow(ref int index)
        {
            if (index < 0)
            {
                throw new IndexOutOfRangeException("The index must be less than rows count or" +
                        "more or equels than zero");
            }

            string[] values = null;

            // Indecates is there more rows.
            if (index < m_rowCount)
            {
                // Get row for sources implemented with contructors that have
                // DataTable,DataView, DataSet as parameter
                if (m_dataTable != null)
                {
                    values = GetRowFromTable(m_dataTable, ref index);
                }

                // Get row for sources implemented with contructors that have
                // DataColumn as parameter
                if (m_dataColumn != null)
                {
                    values = GetRowFromColumn(m_dataColumn, ref index);
                }

                // Get row for sources implemented with contructors that have
                // array as parameter
                if (m_array != null)
                {
                    values = GetRowFromArray(m_array, ref index);
                }
            }
            return values;
        }

        /// <summary>
        /// Verify is the column read only
        /// </summary>
        /// <param name="index">The index</param>
        /// <returns>Is current column read only</returns>
        public bool IsColumnReadOnly(int index)
        {
            if (index < 0 || index >= GetVisibleColCount())
            {
                throw new IndexOutOfRangeException("The index must be less than columns count or" +
                    "more than or equal to zero.");
            }

            bool readOnly = false;

            if (m_dataTable != null)
            {
                int i = GetVisibleIndex(index);
                readOnly = m_dataTable.Columns[i].ReadOnly;
            }

            if (m_dataColumn != null)
            {
                readOnly = m_dataColumn.ReadOnly;
            }

            if (m_array != null)
            {
                readOnly = m_array.IsReadOnly;
            }

            return readOnly;
        }

        /// <summary>
        /// Gets Column Mapping type
        /// </summary>
        /// <param name="index">The index</param>
        /// <returns>Column mapping type</returns>
        public MappingType GetColumnMappingType(int index)
        {
            if (index < 0 || index >= GetVisibleColCount())
            {
                throw new IndexOutOfRangeException("The index must be less than columns count or" +
                    "more or equels than zero");
            }

            MappingType type = MappingType.Hidden;

            if (m_dataTable != null)
            {
                int i = GetVisibleIndex(index);
                type = m_dataTable.Columns[i].ColumnMapping;
            }

            if (m_dataColumn != null)
            {
                type = m_dataColumn.ColumnMapping;
            }

            if (m_array != null)
            {
                throw new ArgumentException("Array does not have mapping type propety");
            }

            return type;
        }

        /// <summary>
        /// Gets Column Data type
        /// </summary>
        /// <param name="index">The index</param>
        /// <returns>Column data type</returns>
        public Type GetColumnDataType(int index)
        {
            if (index < 0 || index >= GetVisibleColCount())
            {
                throw new IndexOutOfRangeException("The index must be less than columns count or" +
                    "more or equels than zero");
            }

            Type type = null;

            if (m_dataTable != null)
            {
                int i = GetVisibleIndex(index);
                type = m_dataTable.Columns[i].DataType;
            }

            if (m_dataColumn != null)
            {
                type = m_dataColumn.DataType;
            }

            if (m_array != null)
            {
                type = GetTypeOfArray(m_array);
            }

            return type;
        }

        /// <summary>
        /// Get column default value
        /// </summary>
        /// <param name="index">The index</param>
        /// <returns>Column default value</returns>
        public object GetColumnDefaultValue(int index)
        {
            if (index < 0 || index >= GetVisibleColCount())
            {
                throw new IndexOutOfRangeException("The index must be less than columns count or" +
                    "more or equels than zero");
            }

            object value = null;

            if (m_dataTable != null)
            {
                int i = GetVisibleIndex(index);
                value = m_dataTable.Columns[i].DefaultValue;
            }

            if (m_dataColumn != null)
            {
                value = m_dataColumn.DefaultValue;
            }

            if (m_array != null)
            {
                throw new ArgumentException("Array does not have default value propety");
            }

            return value;
        }

        /// <summary>
        /// Gets whether column allow DBNull
        /// </summary>
        /// <param name="index">The index</param>
        /// <returns>Allow DBNull</returns>
        public bool AllowDBNull(int index)
        {
            if (index < 0 || index >= GetVisibleColCount())
            {
                throw new IndexOutOfRangeException("The index must be less than columns count or" +
                    "more or equels than zero");
            }

            bool allowNull = false;

            if (m_dataTable != null)
            {
                int i = GetVisibleIndex(index);
                allowNull = m_dataTable.Columns[i].AllowDBNull;
            }

            if (m_dataColumn != null)
            {
                allowNull = m_dataColumn.AllowDBNull;
            }

            if (m_array != null)
            {
                throw new ArgumentException("Array does not have allowDBNull propety");
            }

            return allowNull;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Gets array base type
        /// </summary>
        /// <param name="array">The array</param>
        /// <returns>Base type for array</returns>
        private Type GetTypeOfArray(Array array)
        {
            Type type = null;

            switch (array.Rank)
            {
                case 1:
                    Array nestedArray = array.GetValue(0) as Array;

                    if (nestedArray != null)
                    {
                        type = nestedArray.GetValue(0).GetType();
                    }
                    else
                    {
                        type = array.GetValue(0).GetType();
                    }
                    break;

                case 2:
                    type = array.GetValue(0, 0).GetType();
                    break;
            }

            return type;
        }

        /// <summary>
        /// Gets columns names
        /// </summary>
        /// <returns>Columns names</returns>
        private string[] GetColumnsNames()
        {
            string[] names = null;

            if (m_dataTable != null)
            {
                ArrayList visibleNames = new ArrayList();

                for (int i = 0; i < m_colCount; i++)
                {
                    DataColumn column = m_dataTable.Columns[i];

                    if (column.ColumnMapping != MappingType.Hidden)
                    {
                        visibleNames.Add(column.ColumnName);
                    }
                }
                names = visibleNames.ToArray(typeof(string)) as string[];
            }

            if (m_dataColumn != null)
            {
                if (m_dataColumn.ColumnMapping != MappingType.Hidden)
                {
                    names = new string[] { m_dataColumn.ColumnName };
                }
            }

            return names;
        }

        /// <summary>
        /// Gets columns captions.
        /// </summary>
        /// <returns>Columns captions</returns>
        private string[] GetColumnsCaptions()
        {
            string[] captions = null;

            if (m_dataTable != null)
            {
                ArrayList visibleCaptions = new ArrayList();

                for (int i = 0; i < m_colCount; i++)
                {
                    DataColumn column = m_dataTable.Columns[i];

                    if (column.ColumnMapping != MappingType.Hidden)
                    {
                        if (column.Caption != String.Empty)
                        {
                            visibleCaptions.Add(column.Caption);
                        }
                        else
                        {
                            visibleCaptions.Add(column.ColumnName);
                        }
                    }
                }
                captions = visibleCaptions.ToArray(typeof(string)) as string[];
            }

            if (m_dataColumn != null)
            {
                if (m_dataColumn.ColumnMapping != MappingType.Hidden)
                {
                    if (m_dataColumn.Caption != String.Empty)
                    {
                        captions = new string[] { m_dataColumn.Caption };
                    }
                    else
                    {
                        captions = new string[] { m_dataColumn.ColumnName };
                    }
                }
            }

            return captions;
        }

        /// <summary>
        /// Verify is the input array valid
        /// </summary>
        /// <param name="array">Array to check</param>
        /// <param name="count">Columns count</param>
        /// <returns>Is array valid</returns>
        private bool IsArrayValid(Array array, ref int count)
        {
            bool isValid = false;

            switch (array.Rank)
            {
                case 1:
                    int k = 0;

                    Array nestedArray = array.GetValue(0) as Array;

                    if (nestedArray != null)
                    {
                        //More than two dimension array
                        if (nestedArray.Rank > 1)
                        {
                            isValid = false;
                            break;
                        }

                        k = nestedArray.GetLength(0);

                        for (int i = 1, l = array.Length; i < l; i++)
                        {
                            int j = 0;

                            nestedArray = array.GetValue(i) as Array;

                            if (nestedArray != null)
                            {
                                if (nestedArray.Rank > 1)
                                {
                                    isValid = false;
                                    break;
                                }

                                j = nestedArray.GetLength(0);
                            }

                            if (k != j)
                            {
                                isValid = false;
                                break;
                            }
                            else
                            {
                                isValid = true;
                                count = k;
                            }
                        }
                    }
                    else//One dimension array
                    {
                        count = ++k;
                        isValid = true;
                    }
                    break;

                //Two dimension array
                case 2:
                    count = array.GetLength(1);
                    isValid = true;
                    break;

                //More than two dimension array
                default:
                    isValid = false;
                    break;
            }

            return isValid;
        }

        /// <summary>
        /// Set data Table
        /// </summary>
        /// <param name="table">Data table</param>
        private void SetTable(DataTable table)
        {

            if (table.Columns.Count == 0)
                table.Columns.Add("Col0");

            m_dataTable = table;
            m_colCount = m_dataTable.Columns.Count;
            m_rowCount = m_dataTable.Rows.Count;

            m_dataTable.ColumnChanged += new DataColumnChangeEventHandler(dataTable_ColumnChanged);
            m_dataTable.RowChanged += new DataRowChangeEventHandler(dataTable_RowChanged);
            m_dataTable.RowDeleted += new DataRowChangeEventHandler(dataTable_RowDeleted);
        }

        /// <summary>
        /// Handles the RowDeleted event of the dataTable control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Data.DataRowChangeEventArgs"/> instance containing the event data.</param>
        void dataTable_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            RefreshCache();
        }

        /// <summary>
        /// Handles the RowChanged event of the dataTable control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Data.DataRowChangeEventArgs"/> instance containing the event data.</param>
        void dataTable_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            RefreshCache();
        }

        /// <summary>
        /// Handles the ColumnChanged event of the dataTable control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Data.DataColumnChangeEventArgs"/> instance containing the event data.</param>
        void dataTable_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            RefreshCache();
        }

        /// <summary>
        /// Refreshes the cache.
        /// </summary>
        void RefreshCache()
        {
            m_cachRows = null;
        }

        /// <summary>
        /// Gets visible columns count
        /// </summary>
        /// <returns>Returns visible columns count</returns>
        private int GetVisibleColCount()
        {
            int count = 0;

            if (m_dataTable != null)
            {
                for (int i = 0; i < m_colCount; i++)
                {
                    DataColumn column = m_dataTable.Columns[i];

                    if (column.ColumnMapping != MappingType.Hidden)
                    {
                        count++;
                    }
                }
            }

            if (m_dataColumn != null)
            {
                if (m_dataColumn.ColumnMapping == MappingType.Hidden)
                {
                    count = 0;
                }
                else
                {
                    count = 1;
                }
            }

            if (m_array != null)
            {
                count = m_colCount;
            }

            return count;
        }

        /// <summary>
        /// Convert input index to real index
        /// </summary>
        /// <param name="index">The index</param>
        /// <returns>Gets real table index</returns>
        private int GetVisibleIndex(int index)
        {
            if (index < 0 || index >= GetVisibleColCount())
            {
                throw new IndexOutOfRangeException("The index must be less than columns count or" +
                    "more than or equel to zero");
            }

            int resultIndex = 0;

            if (m_dataTable != null)
            {
                int i = index;

                int k = 0;

                while (i > -1)
                {
                    DataColumn column = m_dataTable.Columns[k];

                    if (column.ColumnMapping == MappingType.Hidden)
                    {
                        k++;
                    }
                    else
                    {
                        i--;
                        k++;
                    }
                }

                resultIndex = k - 1;
            }

            if (m_dataColumn != null)
            {
                if (m_dataColumn.ColumnMapping == MappingType.Hidden)
                {
                    throw new ArgumentException("The source is DataColumn, but this column is hidden");
                }
                else
                {
                    resultIndex = 0;
                }
            }

            if (m_array != null)
            {
                resultIndex = index;
            }

            return resultIndex;
        }

        /// <summary>
        /// Gets row from array
        /// </summary>
        /// <param name="array">The array</param>
        /// <param name="index">The index</param>
        /// <returns>Returns array of values</returns>
        private string[] GetRowFromArray(Array array, ref int index)
        {
            string[] values = null;

            switch (array.Rank)
            {
                case 1:
                    Array nestedArray = array.GetValue(0) as Array;

                    //One dimension array
                    if (nestedArray == null)
                    {
                        values = new string[m_colCount];
                        for (int i = 0; i < m_colCount; i++)
                        {
                            object value = array.GetValue(index);
                            values[i] = Convert.ToString(value);
                        }
                    }
                    else //Two dimension array 
                    {
                        values = new string[m_colCount];

                        nestedArray = array.GetValue(index) as Array;

                        for (int i = 0; i < m_colCount; i++)
                        {
                            object value = nestedArray.GetValue(i);
                            values[i] = Convert.ToString(value);
                        }
                    }
                    break;

                //Two dimension array 
                case 2:
                    values = new string[m_colCount];

                    for (int i = 0; i < m_colCount; i++)
                    {
                        object value = array.GetValue(index, i);

                        values[i] = Convert.ToString(value);
                    }
                    break;

                //More than one dimension array
                default:
                    throw new ArgumentException("We don't suuport more than one or two dimensions" +
                        " arrays in this context or you array has diiferent length", "array");
            }

            ++index;

            return values;
        }

        /// <summary>
        /// Gets row from data column
        /// </summary>
        /// <param name="dataColumn">The column</param>
        /// <param name="index">The index</param>
        /// <returns>Returns array of values</returns>
        private string[] GetRowFromColumn(DataColumn dataColumn, ref int index)
        {
            if (dataColumn.ColumnMapping == MappingType.Hidden)
            {
                throw new ArgumentException("The source is DataColumn, but this column is hidden");
            }

            string[] values = null;

            //Display sorted data
            if (m_useSorting)
            {
                if (m_cachRows == null)
                {
                    m_cachRows = dataColumn.Table.Select();
                }

                object value = m_cachRows[index][dataColumn.ColumnName];

                values = new string[]{
					Convert.ToString(value )
				};
            }
            else //Display unsorted data
            {
                object value = dataColumn.Table.Rows[index][dataColumn.ColumnName];

                values = new string[]{
					Convert.ToString(value)
				};
            }

            ++index;

            return values;
        }

        /// <summary>
        /// Gets row from table
        /// </summary>
        /// <param name="dataTable">The Data Table</param>
        /// <param name="index">The index</param>
        /// <returns>Returns array of values</returns>
        private string[] GetRowFromTable(DataTable dataTable, ref int index)
        {
            if (dataTable.Rows.Count <= 0)
            {
                throw new ArgumentException("There is no rows in data source");
            }

            if (index < 0 || index >= dataTable.Rows.Count)
            {
                throw new IndexOutOfRangeException("The index must be less than rows count or" +
                    "more or equels than zero");
            }

            object[] array = null;

            //Display sorted data
            if (m_useSorting)
            {
                if (m_cachRows == null)
                {
                    m_cachRows = dataTable.Select();
                }

                array = m_cachRows[index].ItemArray;
            }
            else //Display unsorted data
            {
                array = dataTable.Rows[index].ItemArray;
            }

            ArrayList strings = new ArrayList();

            for (int i = 0, l = array.Length; i < l; i++)
            {
                if (dataTable.Columns[i].ColumnMapping != MappingType.Hidden)
                {
                    strings.Add(Convert.ToString(array[i]));
                }
            }

            ++index;

            string[] values = strings.ToArray(typeof(string)) as string[];

            return values;
        }

        /// <summary>
        /// Gets table form DataSet
        /// </summary>
        /// <param name="dataSet">The DataSet</param>
        /// <param name="tableName">Table name</param>
        /// <returns>Returns table</returns>
        private static DataTable GetTableFromDataSet(DataSet dataSet, string tableName)
        {
            if (dataSet == null)
            {
                throw new ArgumentNullException("Data Set can't be null", "dataSet");
            }

            if (dataSet.Tables.Count <= 0)
            {
                throw new ArgumentException("The data set should contain at least one data table", "dataSet");
            }

            DataTable dt = null;

            if (tableName != null && tableName != String.Empty)
            {
                if (!dataSet.Tables.Contains(tableName))
                {
                    throw new ArgumentNullException("The data set should contain a table with specified table name", tableName);
                }

                dt = dataSet.Tables[tableName];
            }
            else
            {
                dt = dataSet.Tables[0];
            }

            return dt;
        }

        /// <summary>
        /// Gets table from DataView
        /// </summary>
        /// <param name="view">The DataView</param>
        /// <returns>Returns table</returns>
        private static DataTable GetTableFromDataView(DataView view)
        {
            if (view == null)
            {
                throw new ArgumentNullException("Data view", "view");
            }

            return view.Table;
        }
        #endregion
    }
}
#endif