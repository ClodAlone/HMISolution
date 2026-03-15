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

using System;

namespace Syncfusion.XlsIO.Parser.Biff_Records.MsoDrawing
{
  /// <summary>
  /// Summary description for Attributes.
  /// </summary>
  [Syncfusion.Documentation.DocumentationExclude()]
  public class MsoDrawingAttribute : Attribute
  {
    #region Class members
    /// <summary>
    /// 
    /// </summary>
    private MsoRecords m_recordType;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// 
    /// </summary>
    private MsoDrawingAttribute()
    {
    }
    /// <summary>
    /// Creates new instance.
    /// </summary>
    /// <param name="recordType">Type of new instance</param>
    public MsoDrawingAttribute( MsoRecords recordType )
    {
      m_recordType = recordType;
    }
    #endregion

    #region Class properties
    /// <summary>
    /// 
    /// </summary>
    public MsoRecords RecordType
    {
      get
      {
        return m_recordType;
      }
    }
    #endregion
  }
}
