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
using System.IO;
using Syncfusion.XlsIO.Implementation.Exceptions;
#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  /// <summary>
  /// Sheet Tab Index Array Record:
  /// Contains an array of sheet ID's. Sheets always keep their ID
  /// regardless of what their name is.
  /// </summary>
  [ Biff( TBIFFRecord.TabId ) ]
  [ CLSCompliant( false ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  public class TabIdRecord  : BiffRecordRaw
  {
    #region Class members
    /// <summary>
    /// Array of tab IDs.
    /// </summary>
    private ushort[] m_arrTabIds = new ushort[]{ 1 };
    #endregion

    #region Class properties
    /// <summary>
    /// Array of tab IDs.
    /// </summary>
    public ushort[] TabIds
    {
      get
      {
        return m_arrTabIds;
      }
      set
      {
        m_arrTabIds = value;
      }
    }

    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor
    /// </summary>
    public  TabIdRecord()
      : base()
    {
    }
    /// <summary>
    /// Read / initialize constructor.
    /// </summary>
    /// <param name="stream">Stream from which record data should be read.</param>
    /// <param name="itemSize">Size of read item.</param>
    /// <exception cref="System.ArgumentNullException">
    /// If stream is not specified.
    /// </exception>
    /// <exception cref="System.ApplicationException">
    /// If stream does not support read or seek operations.
    /// </exception>
    public  TabIdRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }
    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// If amount of bytes requested is less than zero.
    /// </exception>
    public  TabIdRecord( int iReserve )
      : base( iReserve )
    {
    }
    #endregion

    #region Record Serialization
    /// <summary>
    /// Parse structure of record. Converts data buffer to special
    /// values according to record specification.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">Offset to the record's data.</param>
    /// <param name="iLength">Length of the record's data.</param>
    /// <param name="version">Excel version used for infill.</param>
    public override void ParseStructure( DataProvider provider, int iOffset, int iLength, ExcelVersion version )
    {
      //AutoExtractFields();
      InternalDataIntegrityCheck();

      m_arrTabIds = new ushort[ Length / 2 ];

      for( int j = 0, iLast = m_iLength + iOffset; iOffset < iLast; j++, iOffset += ExcelConstants.ShortSize )
      {
        m_arrTabIds[ j ] = provider.ReadUInt16( iOffset );
      }
    }

    /// <summary>
    /// In this method, a class must pack all of its properties into
    /// an internal data array, m_data. This method is called by
    /// FillStream, when the record must be serialized into a stream.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">Offset in the buffer.</param>
    /// <param name="version">Excel version used for infill.</param>
    /// <returns>Size of the record data.</returns>
    public override void InfillInternalData( DataProvider provider, int iOffset, ExcelVersion version )
    {
      m_iLength = GetStoreSize( ExcelVersion.Excel97to2003 );

      for( int j = 0, len = m_arrTabIds.Length; j < len; iOffset += ExcelConstants.ShortSize, j++ )
      {
        provider.WriteUInt16( iOffset, m_arrTabIds[ j ] );
      }
    }

    /// <summary>
    /// This method checks a record's internal data array for integrity.
    /// </summary>
    /// <exception cref="WrongBiffRecordDataException">
    /// If there is any internal error.
    /// </exception>
    private void InternalDataIntegrityCheck()
    {
      if( m_iLength % 2 != 0 )
        throw new WrongBiffRecordDataException( "MergeCellsRecord" );
    }
    /// <summary>
    /// Size of the required storage space. Read-only.
    /// </summary>
    public override int GetStoreSize( ExcelVersion version )
    {
      return m_arrTabIds.Length * 2;
    }
    #endregion
  }
}
