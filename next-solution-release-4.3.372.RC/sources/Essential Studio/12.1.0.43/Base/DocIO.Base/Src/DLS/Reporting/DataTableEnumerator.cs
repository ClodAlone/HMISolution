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
#if !(SILVERLIGHT || WP)
using System.Data;
#endif
#endregion

using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;

namespace Syncfusion.DocIO.DLS
{
#if SILVERLIGHT || WP
    /// <summary>
    /// Summary description for DataTableEnumerator.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class DataTableEnumerator : IRowsEnumerator
    {
        #region Class members
        private IEnumerator m_table = null;
        private object m_row = null;
        private int m_currRowIndex = -1;
        private string[] m_columnsNames = null;
        private string m_tableName = string.Empty;
        private int m_rowCount;
        private Type m_userClassType = null;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets the current row index value.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        public int CurrentRowIndex
        {
            get
            {
                return m_currRowIndex;
            }
        }
        /// <summary>
        /// Gets the total row count.
        /// </summary>
        public int RowsCount
        {
            get
            {
              return m_rowCount;
            }
        }
        /// <summary>
        /// Gets the table name.
        /// </summary>
        public string TableName
        {
            get
            {
              return m_tableName;
            }
        }
        /// <summary>
        /// Check whether end of the row is reached.
        /// </summary>
        /// <value>Return true if its end of the row. otherwise, false.</value>
        public bool IsEnd
        {
            get
            {
                return !(m_currRowIndex < RowsCount);
            }
        }
        /// <summary>
        /// Check whether end of the row is reached.
        /// </summary>
        /// <value>Return true if its end of the row. otherwise, false.</value>
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
        protected object CurrentRow
        {
          get
          {
            return ( m_table != null ) ? m_table.Current : null;
          }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="DataTableEnumerator"/> class.
        /// </summary>
        /// <param name="table">The table.</param>
        public DataTableEnumerator( MailMergeDataTable table )
        {
          m_tableName = table.GroupName;
          table.SourceData.Reset();
          table.SourceData.MoveNext();
          m_table = table.SourceData;
          try
          {
              m_userClassType = m_table.Current.GetType();
              ReadColumnNames(m_table);
          }
          catch
          {
              m_userClassType = null;
              m_columnsNames = null;
          }
          CalculRowCount();
        }
        #endregion

        #region Class public methods
        /// <summary>
        /// Reset the row index.
        /// </summary>
        public void Reset()
        {
          m_table.Reset();
          m_currRowIndex = -1;
        }
        /// <summary>
        /// Points to next row.
        /// </summary>
        /// <returns></returns>
        public bool NextRow()
        {
            if (m_currRowIndex < RowsCount)
            {
                m_currRowIndex++;
            }

            return !IsEnd;
        }
        /// <summary>
        /// Retrieve the cell value for a specified column.
        /// </summary>
        /// <param name="columnName">Column Name.</param>
        /// <returns>Cell Value.</returns>
        public object GetCellValue(string columnName)
        {
          m_table.Reset();
          for( int i = 0; i <= m_currRowIndex; i++ )
          {
            m_table.MoveNext();
          }

#if WINRT
          IEnumerable<PropertyInfo> propInfo = m_userClassType.GetRuntimeProperties();
          foreach(PropertyInfo info in propInfo)
          {
#else
          PropertyInfo[] propInfo = m_userClassType.GetProperties();
          for( int i = 0, cnt = propInfo.Length; i < cnt; i++ )
          {
            PropertyInfo info = propInfo[i];
#endif
            if( info.Name == columnName )
            {
              return info.GetValue( m_table.Current, null );
            }
          }
          return null;
        }
        /// <summary>
        /// Gets the columnnames of the table.
        /// </summary>
        /// <value>retruns column names as string array.</value>
        public string[] ColumnNames
        {
            get
            {
                return m_columnsNames;
            }
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// 
        /// </summary>
        private void ReadColumnNames( IEnumerator table )
        {
          List<string> colNames = new List<string>();
#if WINRT
         IEnumerable<PropertyInfo> porpInfo = m_userClassType.GetRuntimeProperties();
         foreach (PropertyInfo info in porpInfo)
          {
              colNames.Add(info.Name);
          }
#else
          PropertyInfo[] porpInfo = m_userClassType.GetProperties();
          for ( int i = 0, cnt = porpInfo.Length; i < cnt; i++ )
          {
            colNames.Add( porpInfo[ i ].Name );
          }
#endif

          m_columnsNames = colNames.ToArray();
        }
        /// <summary>
        /// 
        /// </summary>
        private void CalculRowCount()
        {
            m_table.Reset();
            while (m_table.MoveNext())
            {
                m_rowCount++;
            }
            m_table.Reset();
        }
        #endregion
    }
#else
    /// <summary>
    /// Summary description for DataTableEnumerator.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class DataTableEnumerator : IRowsEnumerator
    {
        #region Class members
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
        private string[] m_columnsNames = null;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets the current row index value.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        public int CurrentRowIndex
        {
            get
            {
                return m_currRowIndex;
            }
        }
        /// <summary>
        /// Gets the total row count.
        /// </summary>
        public int RowsCount
        {
            get
            {
                if (m_table != null)
                    return m_table.Rows.Count;
                //If m_rowCount is zero check for m_row object for mail merge Data
                if (m_rowCount == 0 && this.m_row != null &&
                    this.m_row.ItemArray != null && this.m_row.ItemArray.Length > 0)
                    return 1;
                return m_rowCount;
            }
        }
        /// <summary>
        /// Gets the table name.
        /// </summary>
        public string TableName
        {
            get
            {
                if (m_table != null)
                    return m_table.TableName;
                return m_tableName;
            }
        }
        /// <summary>
        /// Check whether end of the row is reached.
        /// </summary>
        /// <value>Return true if its end of the row. otherwise, false.</value>
        public bool IsEnd
        {
            get
            {
                return !(m_currRowIndex < RowsCount);
            }
        }
        /// <summary>
        /// Check whether end of the row is reached.
        /// </summary>
        /// <value>Return true if its end of the row. otherwise, false.</value>
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
        protected object CurrentRow
        {
            get
            {
                if (m_currRowIndex < RowsCount)
                {
                    if (m_MMtable != null)
                    {
                        m_MMtable.Reset();
                        for (int i = 0; i <= m_currRowIndex; i++)
                        {
                            m_MMtable.MoveNext();
                        }
                        return m_MMtable.Current;
                    }
                    else if (m_table != null)
                    {
                        return m_table.Rows[m_currRowIndex];
                    }
                    return m_row;
                }

                return null;
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="DataTableEnumerator"/> class.
        /// </summary>
        /// <param name="table">The table.</param>
        public DataTableEnumerator(DataTable table)
        {
            m_table = table;
            ReadColumnNames(m_table);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="DataTableEnumerator"/> class.
        /// </summary>
        /// <param name="row">The row.</param>
        public DataTableEnumerator(DataRow row)
        {
            m_row = row;
            ReadColumnNames(row.Table);
        }
        #endregion

        #region Class public methods
        /// <summary>
        /// Reset the row index.
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
            {
                m_currRowIndex++;
            }

            return !IsEnd;
        }
        /// <summary>
        /// Retrieve the cell value for a specified column.
        /// </summary>
        /// <param name="columnName">Column Name.</param>
        /// <returns>Cell Value.</returns>
        public object GetCellValue(string columnName)
        {
            if (CurrentRow is DataRow)
            {
                return (CurrentRow as DataRow)[columnName];
            }
            else if (CurrentRow != null)
            {
                PropertyInfo[] propInfo = m_userClassType.GetProperties();
                for (int i = 0, cnt = propInfo.Length; i < cnt; i++)
                {
                    PropertyInfo info = propInfo[i];
                    if (info.Name == columnName)
                    {
                        return info.GetValue(CurrentRow, null);
                    }
                }
            }

            return null;
        }
        /// <summary>
        /// Gets the columnnames of the table.
        /// </summary>
        /// <value>retruns column names as string array.</value>
        public string[] ColumnNames
        {
            get
            {
                return m_columnsNames;
            }
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// 
        /// </summary>
        private void ReadColumnNames(DataTable table)
        {
            m_columnsNames = new string[table.Columns.Count];

            for (int i = 0; i < m_columnsNames.Length; i++)
            {
                m_columnsNames[i] = table.Columns[i].ColumnName;
            }
        }
        #endregion

        #region From Silverlight

        #region Members
        /// <summary>
        /// 
        /// </summary>
        private string m_tableName;
        /// <summary>
        /// 
        /// </summary>
        private IEnumerator m_MMtable = null;
        /// <summary>
        /// 
        /// </summary>
        private Type m_userClassType = null;
        /// <summary>
        /// 
        /// </summary>
        private int m_rowCount;
        #endregion

        #region Properties
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="DataTableEnumerator"/> class.
        /// </summary>
        /// <param name="table">The table.</param>
        public DataTableEnumerator(MailMergeDataTable table)
        {
            m_tableName = table.GroupName;
            table.SourceData.Reset();
            table.SourceData.MoveNext();
            m_MMtable = table.SourceData;
            try
            {
                 m_userClassType = m_MMtable.Current.GetType();
                 ReadColumnNames(m_MMtable);
            }
            catch
            {
                m_userClassType = null;
                m_columnsNames = null;
            }
            CalculRowCount();
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// 
        /// </summary>
        private void ReadColumnNames(IEnumerator table)
        {
            List<string> colNames = new List<string>();
            PropertyInfo[] porpInfo = m_userClassType.GetProperties();
            for (int i = 0, cnt = porpInfo.Length; i < cnt; i++)
            {
                colNames.Add(porpInfo[i].Name);
            }
            m_columnsNames = colNames.ToArray();
        }
        /// <summary>
        /// 
        /// </summary>
        private void CalculRowCount()
        {
            m_MMtable.Reset();
            while (m_MMtable.MoveNext())
            {
                m_rowCount++;
            }
            m_MMtable.Reset();
        }
        #endregion
        #endregion
    }
#endif
}