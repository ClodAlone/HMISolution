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

//#define USE_INTPTR

using System;
using System.IO;

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  /// <summary>
  /// Whenever the content of the record exceeds the given limits,
  /// the record must be split. Several Continue Records containing
  /// the additional data are added after the parent record.
  /// </summary>
  [ Biff( TBIFFRecord.Continue ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class ContinueRecord
#if USE_INTPTR
    : BiffRecordRawWithDataProvider
#else
    : BiffRecordRawWithArray
#endif
    , ILengthSetter
  {
    #region Class properties
    /// <summary>
    /// 
    /// </summary>
    public override bool NeedDataArray
    {
      get
      {
        return true;
      }
    }

    #endregion

    #region Class methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="len"></param>
    public void SetLength( int len )
    {
      m_iLength = len;
#if USE_INTPTR
      m_provider.EnsureCapacity( len );
#endif
    }
    /// <summary>
    /// Sets internal data array.
    /// </summary>
    /// <param name="arrData">Data array to set.</param>
    public void SetData( byte[] arrData )
    {
      m_data = arrData;
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor
    /// </summary>
    public  ContinueRecord()
      : base()
    {
    }

    #endregion

    #region Record Serialization
    /// <summary>
    /// Parse structure of record. Converts data buffer to special
    /// values according to record specification.
    /// </summary>
    public override void ParseStructure()
    {
    }

    /// <summary>
    /// In this method, a class must pack its own properties into
    /// an internal data array, m_data. This method is called by
    /// FillStream when the record must be serialized into a stream.
    /// </summary>
    public override void InfillInternalData( ExcelVersion version )
    {
    }
    /// <summary>
    /// Size of the required storage space. Read-only.
    /// </summary>
    public override int GetStoreSize( ExcelVersion version )
    {
      return m_iLength;
    }
    #endregion
  }
}
