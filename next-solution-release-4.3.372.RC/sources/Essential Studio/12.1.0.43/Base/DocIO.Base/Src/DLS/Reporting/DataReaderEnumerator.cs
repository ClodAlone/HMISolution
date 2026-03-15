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
using System.Collections;
#if !SILVERLIGHT || SkipSilverlightNamespaces
using System.Data;
#endif
using System;
using System.Collections.Generic;
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Summary description for DataReaderEnumerator.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class DataReaderEnumerator : IRowsEnumerator
    {
        #region Class members
        private IDataReader m_dataReader;
        private List<List<String>> m_rows;
        private int m_currRowIndex = -1;
        private string[] m_columnNames = null;
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
                return m_rows.Count;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public string TableName
        {
            get
            {
                return m_dataReader.GetSchemaTable().TableName;
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
        protected List<String> CurrentRow
        {
            get
            {
                if (m_currRowIndex < RowsCount)
                {
                    return (m_rows != null) ? m_rows[m_currRowIndex] : null;
                }

                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public string[] ColumnNames
        {
            get
            {
                return m_columnNames;
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="DataReaderEnumerator"/> class.
        /// </summary>
        /// <param name="dataReader">The data reader.</param>
        public DataReaderEnumerator(IDataReader dataReader)
        {
            m_dataReader = dataReader;
            m_rows = new List<List<String>>();
            m_columnNames = new string[m_dataReader.FieldCount];

            for (int i = 0; i < m_dataReader.FieldCount; i++)
            {
                m_columnNames[i] = m_dataReader.GetName(i);
            }

            while (m_dataReader.Read())
            {
                List<String> row = new List<String>();

                for (int i = 0; i < m_dataReader.FieldCount; i++)
                {
                    row.Add(m_dataReader[i].ToString());
                }

                m_rows.Add(row);
            }
        }
        #endregion

        #region Class public methods
        /// <summary>
        /// Reset row index.
        /// </summary>
        public void Reset()
        {
            m_currRowIndex = -1;
        }
        /// <summary>
        /// Increment the RowIndex value.
        /// </summary>
        /// <returns></returns>
        public bool NextRow()
        {
            if (m_currRowIndex < RowsCount)
                m_currRowIndex++;

            return !IsEnd;
        }
        /// <summary>
        /// Retrieve the cell value for a specified column.
        /// </summary>
        /// <param name="columnName">Column name.</param>
        /// <returns>Returns the cell value.</returns>
        public object GetCellValue(string columnName)
        {
            List<String> row = CurrentRow;
            return (row != null) ? row[m_dataReader.GetOrdinal(columnName)] : null;
        }
        #endregion
    }
}
