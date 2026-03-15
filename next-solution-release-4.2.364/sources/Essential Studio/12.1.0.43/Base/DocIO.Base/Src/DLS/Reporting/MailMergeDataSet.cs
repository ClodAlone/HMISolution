#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Windows;
using Syncfusion.DocIO.DLS;
using System.Collections.Generic;
using System.Reflection;
using System.Collections;

namespace Syncfusion.DocIO.DLS
{
  public class MailMergeDataSet
  {
    #region Constants
    private string DEF_GROUPNAME_PROPERTY = "GroupName";
    private string DEF_SOURCEDATA_PROPERTY = "SourceData";
    #endregion

    #region Members
    private List<object> m_dataSet;
    #endregion

    #region Properties
    /// <summary>
    /// Gets list of MailMergeDataTables
    /// </summary>
    public List<object> DataSet
    {
      get
      {
        return m_dataSet;
      }
    }
    #endregion

    #region Constructor
    /// <summary>
    /// Create new instance of MailMergeDataSet
    /// </summary>
    /// <param name="dataSet">list of MailMergeDataTables</param>
    public MailMergeDataSet()
    {
      m_dataSet  = new List<object>();
    }
    #endregion

    #region Helper methods
    /// <summary>
    /// Append new MailMergeDataTable to MailMergeDataSet
    /// </summary>
    /// <param name="dataTable">MailMergeDataTable</param>
    public void Add( object dataTable )
    {
      m_dataSet.Add( dataTable );
    }
    /// <summary>
    /// Cleans MailMergeDataSet of any data by removing all MaillMergeDataTable
    /// </summary>
    public void Clear()
    {
      m_dataSet.Clear();
      m_dataSet = null;
    }
    /// <summary>
    /// Gets MailMergeDataTable with necessary table name
    /// </summary>
    /// <typeparam name="T">MailMergeDataTable</typeparam>
    /// <param name="tableName">Name of table</param>
    /// <returns></returns>
    internal MailMergeDataTable GetDataTable( string tableName )
    {
      foreach( object obj in m_dataSet )
      {
        Type type = obj.GetType();

#if WINRT
        PropertyInfo nameInfo = type.GetRuntimeProperty(DEF_GROUPNAME_PROPERTY);
#else
        PropertyInfo nameInfo = type.GetProperty( DEF_GROUPNAME_PROPERTY );
#endif
        string value = nameInfo.GetValue( obj, null ).ToString();
        if( !string.IsNullOrEmpty(value) && value == tableName )
        {
#if WINRT
            PropertyInfo dataInfo = type.GetRuntimeProperty(DEF_SOURCEDATA_PROPERTY);
#else
            PropertyInfo dataInfo = type.GetProperty(DEF_SOURCEDATA_PROPERTY);
#endif
            IEnumerator data = dataInfo.GetValue(obj, null) as IEnumerator;
          MailMergeDataTable dataTable = null;
          if( data != null )
          {
            dataTable = new MailMergeDataTable( value, data );
          }
          return dataTable;
        }
      }
      return null;
    }
    /// <summary>
    /// Remove MailMergeDataTable with necessary table name
    /// </summary>
    /// <param name="tableName">Name of table</param>
    internal void RemoveDataTable( string tableName )
    {
      foreach( object obj in m_dataSet )
      {

#if WINRT
        PropertyInfo info = obj.GetType().GetRuntimeProperty(DEF_GROUPNAME_PROPERTY);
#else
        PropertyInfo info = obj.GetType().GetProperty( DEF_GROUPNAME_PROPERTY );
#endif
        string value = info.GetValue( obj, null ).ToString();
        if( !string.IsNullOrEmpty( value ) && value == tableName )
        {
          m_dataSet.Remove( obj );
          break;
        }
      }
    }
    #endregion
  }
}  
