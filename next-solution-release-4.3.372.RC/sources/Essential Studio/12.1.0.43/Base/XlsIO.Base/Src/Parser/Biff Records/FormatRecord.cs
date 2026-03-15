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
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Collections;

using Syncfusion.XlsIO.Implementation;
using Syncfusion.XlsIO.Implementation.Exceptions;
#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  /// <summary>
  /// Record contains information about a number format.
  /// </summary>
  [ Biff( TBIFFRecord.Format ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class FormatRecord  : BiffRecordRaw
  {
    #region Class members

    /// <summary>
    /// Format index used in other records:
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usIndex = 0;

    /// <summary>
    /// Length of format string:
    /// </summary>
    [ BiffRecordPos( 2, 2 ) ]
    private ushort m_usFormatStringLen = 0;

    /// <summary>
    /// Format string:
    /// </summary>
    private string m_strFormatString = string.Empty;
    #endregion

    #region Class properties
    /// <summary>
    /// Format index used in other records:
    /// </summary>
    public int Index
    {
      get
      {
        return m_usIndex;
      }
      set
      {
        m_usIndex = (ushort) value;
      }
    }

    /// <summary>
    /// Length of format string:
    /// </summary>
    public string FormatString
    {
      get
      {
        return m_strFormatString;
      }
      set
      {
        if( m_strFormatString != value )
        {
          m_strFormatString = value;
          m_usFormatStringLen = ( ushort )m_strFormatString.Length;
        }
      }
    }

    /// <summary>
    /// Read-only. Minimum possible size of record.
    /// </summary>
    override public int MinimumRecordSize
    {
      get
      {
        return 5;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor
    /// </summary>
    public  FormatRecord()
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
    public  FormatRecord( Stream stream, out int itemSize )
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
    public  FormatRecord( int iReserve )
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
    /// <exception cref="Syncfusion.XlsIO.Implementation.Exceptions.WrongBiffRecordDataException">
    /// If there is any internal error.
    /// </exception>
    public override void ParseStructure( DataProvider provider, int iOffset, int iLength, ExcelVersion version )
    {
      m_usIndex = provider.ReadUInt16( iOffset );
      m_usFormatStringLen = provider.ReadUInt16( iOffset + 2 );

      int iBytes;
      FormatString = provider.ReadString( iOffset + 4, m_usFormatStringLen, out iBytes, false );

      if( m_iLength != 5 + iBytes )
      {
        throw new WrongBiffRecordDataException
          ( "m_iLength and String length do not fit each other." );
      }
    }

    /// <summary>
    /// In this method, class must pack all of its properties into
    /// an internal data array, m_data. This method is called by
    /// FillStream, when the record must be serialized into a stream.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">Offset in the buffer.</param>
    /// <param name="version">Excel version used for infill.</param>
    public override void InfillInternalData( DataProvider provider, int iOffset, ExcelVersion version )
    {
      m_iLength = GetStoreSize( version );

      provider.WriteUInt16( iOffset, m_usIndex );
      provider.WriteUInt16( iOffset + 2, m_usFormatStringLen );
      
      provider.WriteByte( iOffset + 4, 1 );
      provider.WriteBytes( iOffset + 5, Encoding.Unicode.GetBytes( m_strFormatString ), 0,
        m_usFormatStringLen * 2 );
    }

    /// <summary>
    /// Size of the required storage space. Read-only.
    /// </summary>
    public override int GetStoreSize( ExcelVersion version )
    {
      return 5 + m_usFormatStringLen * 2;
    }
    #endregion
  }
}