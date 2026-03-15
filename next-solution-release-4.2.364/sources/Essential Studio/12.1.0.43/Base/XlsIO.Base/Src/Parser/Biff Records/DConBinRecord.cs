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
#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  /// <summary>
  /// This record stores a data consolidation reference. DConBin is identical to 
  /// DConName, except that DConBin is used when the data consolidation reference
  ///  refers to a built-in name (as described by a Name record).
  /// </summary>
  [ Biff( TBIFFRecord.DCONBIN ) ]
  [ CLSCompliant( false ) ]
  public class DConBinRecord : BiffRecordRawWithArray
  {
    #region Class members
    /// <summary>
    /// Named range of the source area for consolidation.
    /// </summary>
    [ BiffRecordPos( 0, TFieldType.String16Bit ) ]
    private string m_strName;
    /// <summary>
    /// Workbook name.
    /// </summary>
    private string m_strWorkbookName;
    /// <summary>
    /// Used for preserving the Record.
    /// </summary>
    private byte[] arrdata;
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor fills all data with default values.
    /// </summary>
    public  DConBinRecord()
      : base()
    {
    }

    /// <summary>
    /// Read / initialize constructor
    /// </summary>
    /// <param name="stream">Stream from which record data should be read.</param>
    /// <param name="itemSize">Size of read item.</param>
    /// <exception cref="System.ArgumentNullException">
    /// If stream is not specified.
    /// </exception>
    /// <exception cref="System.ApplicationException">
    /// If stream does not support read or seek operations.
    /// </exception>
    public  DConBinRecord( Stream stream, out int itemSize )
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
    public  DConBinRecord( int iReserve )
      : base( iReserve )
    {
    }

    #endregion

    #region Class properties
    /// <summary>
    /// Named range of the source area for consolidation.
    /// </summary>
    public string Name
    {
      get
      {
        return m_strName;
      }
      set
      {
        if( value == null )
          throw new ArgumentNullException( "value" );

        m_strName = value;
      }
    }
    /// <summary>
    /// Workbook name.
    /// </summary>
    public string WorkbookName
    {
      get
      {
        return m_strWorkbookName;
      }
      set
      {
        if( value == null )
          throw new ArgumentNullException( "value" );

        m_strWorkbookName = value;
      }
    }
    #endregion

    #region Record Serialization
    /// <summary>
    /// Parse structure of record. Converts data buffer to special
    /// values according to record specification.
    /// </summary>
    public override void ParseStructure()
    {
      int iOffset = 0;
      arrdata = m_data;
      
      //TODO: Complete the DconBinRecord implementation

      //m_strName = GetString16BitUpdateOffset( ref iOffset );
      //m_strWorkbookName = GetString16BitUpdateOffset( ref iOffset );
    }

    /// <summary>
    /// In this method, a class must pack its own properties into
    /// an internal data array, m_data. This method is called by
    /// FillStream when the record must be serialized into a stream.
    /// </summary>
    public override void InfillInternalData( ExcelVersion version )
    {
      AutoGrowData = true;
      SetBytes(0, arrdata);

      //TODO: Complete the DconBinRecord implementation

      //bool bAscii = IsAsciiString( m_strName );
      //int iOffset = SetString16BitLen( 0, m_strName, true, bAscii );

      //bAscii = IsAsciiString( m_strWorkbookName );
      //m_iLength = iOffset + SetString16BitLen( iOffset, m_strWorkbookName, false, bAscii );
    }

    #endregion
  }
}
