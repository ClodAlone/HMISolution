#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;

namespace Syncfusion.DocIO.DLS
{
  public class MailMergeDataTable
  {
    #region Members
    /// <summary>
    /// 
    /// </summary>
    private string m_groupName;
    /// <summary>
    /// 
    /// </summary>
    private IEnumerator m_sourceData;
    #endregion

    #region Properties
    /// <summary>
    /// Get group name
    /// </summary>
    public string GroupName
    {
      get
      {
        return m_groupName;
      }
    }
    /// <summary>
    /// Get sourse data
    /// </summary>
    public IEnumerator SourceData
    {
      get
      {
        return m_sourceData;
      }
    }
    #endregion

    #region Constructors
    /// <summary>
    /// Create new instance of object
    /// </summary>
    /// <param name="groupName">Group Name</param>
    /// <param name="sourceData">Sourse data Dictionary</param>
    public MailMergeDataTable( string groupName, IEnumerable enumerable )
    {
      m_groupName = groupName;
      m_sourceData = enumerable.GetEnumerator();
    }
    /// <summary>
    /// Create new instance of object
    /// </summary>
    /// <param name="groupName">Group Name</param>
    /// <param name="sourceData">Sourse data Dictionary</param>
    internal MailMergeDataTable( string groupName, IEnumerator enumerator )
    {
      m_groupName = groupName;
      m_sourceData = enumerator;
    }
    #endregion

    #region Helper methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="command">Commands of formulation of new tablet</param>
    /// <returns>Return new instance of MailMergeDataTable from the chosen rows of current MailMergeDataTable</returns>
    internal MailMergeDataTable Select( string command )
    {
      string[] subStr = command.Split( new char[ 1 ] { ' ' } );
      string propertyName = subStr[ 0 ];
      string propertyValue = subStr[ 2 ];

      MailMergeDataTable newDataTable = null;
      List<object> rowList = new List<object>();
      m_sourceData.Reset();
      while( m_sourceData.MoveNext() )
      {

#if WINRT
          PropertyInfo info = m_sourceData.Current.GetType().GetRuntimeProperty(propertyName);
#else
          PropertyInfo info = m_sourceData.Current.GetType().GetProperty( propertyName );
#endif
          object value = info.GetValue( m_sourceData.Current, null );
        if( propertyValue == value.ToString() )
        {
          rowList.Add( m_sourceData.Current );
        }
      }

      if( rowList.Count > 0 )
      {
        newDataTable = new MailMergeDataTable( GroupName, rowList.GetEnumerator() );
      }

      return newDataTable;
    }
    #endregion
  }
}
