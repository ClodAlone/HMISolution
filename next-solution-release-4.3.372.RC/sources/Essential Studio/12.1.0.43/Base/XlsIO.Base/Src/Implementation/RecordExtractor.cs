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

using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;

namespace Syncfusion.XlsIO.Implementation
{
  /// <summary>
  /// Stores created Biff records and allows to get specific record.
  /// </summary>
  public class RecordExtractor
  {
    #region Class members
    /// <summary>
    /// Dictionary with Biff records. Key - Biff record type value.
    /// </summary>
    private Dictionary< int, BiffRecordRaw > m_dictRecords;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Initializes new instance.
    /// </summary>
    public RecordExtractor()
    {
      m_dictRecords = new Dictionary< int, BiffRecordRaw >();
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Returns Biff record filled with speciffic values.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">Offset to the record's start.</param>
    /// <param name="version">Excel version used for infill.</param>
    /// <returns>Extracted record.</returns>
    public BiffRecordRaw GetRecord( DataProvider provider, int iOffset, ExcelVersion version )
    {
      if( provider == null )
        throw new ArgumentNullException( "provider" );

      int recordType = provider.ReadInt16( iOffset );
      iOffset += 2;

      BiffRecordRaw record = GetRecord( recordType );

      int iLength = provider.ReadInt16( iOffset );
      record.Length = iLength;
      iOffset += 2;

      record.ParseStructure( provider, iOffset, iLength, version );
      return record;
    }
    /// <summary>
    /// Returns empty Biff record.
    /// </summary>
    /// <param name="recordType">Biff record type.</param>
    /// <returns>Extracted record.</returns>
    public BiffRecordRaw GetRecord( int recordType )
    {
      BiffRecordRaw record;

      if( !m_dictRecords.TryGetValue( recordType, out record ) )
      {
        record = BiffRecordFactory.GetRecord( recordType );
        m_dictRecords.Add( recordType, record );
      }

      return record;
    }
    #endregion
  }
}