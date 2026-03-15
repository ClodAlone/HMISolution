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

#region file using directives
using System;
#if !SILVERLIGHT || SkipSilverlightNamespaces
using System.Data;
#endif
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Summary description for DataViewEnumerator.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class DataViewEnumerator : IRowsEnumerator
    {
        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private DataView m_dataView;
        /// <summary>
        /// 
        /// </summary>
        private DataTable m_table = null;
        /// <summary>
        /// 
        /// </summary>
        private DataRow m_row = null;
        /// <summary>
        /// 
        /// </summary>
        private int m_currRowIndex = -1;
        /// <summary>
        /// 
        /// </summary>
        private string[] m_columnNames = null;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataView"></param>
        public DataViewEnumerator(DataView dataView)
        {
            m_dataView = dataView;
            m_table = dataView.Table;

            ReadColumnNames(m_table);
        }
        #endregion

        #region Class properties
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int CurrentRowIndex
        {
            get
            {
                return m_currRowIndex;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public int RowsCount
        {
            get
            {
                return (m_dataView != null) ? m_dataView.Count : 1;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public string TableName
        {
            get
            {
                return (m_dataView != null) ? m_dataView.Table.TableName : "";
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool IsEnd
        {
            get
            {
                return !(m_currRowIndex < RowsCount);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool IsLast
        {
            get
            {
                return !(m_currRowIndex < RowsCount - 1);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected DataRow CurrentRow
        {
            get
            {
                if (m_currRowIndex < RowsCount)
                {
                    return (DataRow)((m_dataView != null) ? m_dataView[m_currRowIndex].Row : m_row);
                }

                return null;
            }
        }
        #endregion

        #region Class public methods
        /// <summary>
        /// Resets the row index.
        /// </summary>
        public void Reset()
        {
            m_currRowIndex = -1;
        }
        /// <summary>
        /// Points to next row.
        /// </summary>
        /// <returns></returns>
        public bool NextRow()
        {
            if (m_currRowIndex < RowsCount)
                m_currRowIndex++;

            return !IsEnd;
        }
        /// <summary>
        /// Returns the cell value for a specified column name.
        /// </summary>
        /// <param name="columnName">column name.</param>
        /// <returns>returns the cell value.</returns>
        public object GetCellValue(string columnName)
        {
            DataRow row = CurrentRow;
            return (row != null) ? row[columnName] : null;
        }
        /// <summary>
        /// Gets the column name of the table.
        /// </summary>
        /// <value>retruns the column value as string array.</value>
        public string[] ColumnNames
        {
            get
            {
                return m_columnNames;
            }
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// 
        /// </summary>
        private void ReadColumnNames(DataTable dataTable)
        {
            m_columnNames = new string[dataTable.Columns.Count];

            for (int i = 0; i < m_columnNames.Length; i++)
            {
                m_columnNames[i] = m_table.Columns[i].ColumnName;
            }
        }
        #endregion
    }
}
