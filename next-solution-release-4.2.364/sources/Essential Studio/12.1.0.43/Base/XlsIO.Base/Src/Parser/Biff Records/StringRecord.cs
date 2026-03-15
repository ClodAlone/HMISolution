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
#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  /// <summary>
  /// This record stores the result of a string formula.
  /// It occurs directly after a string formula.
  /// </summary>
  [ Biff( TBIFFRecord.String ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class StringRecord  : BiffRecordRaw
  {
    #region Class constants
    /// <summary>
    /// Size of the fixed part.
    /// </summary>
    private const int DEF_FIXED_SIZE = 3;
    #endregion

    #region Class members
    /// <summary>
    /// String length.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usStringLength = 0;

    /// <summary>
    /// Non-empty Unicode string.
    /// </summary>
    private string m_strValue = null;
    private bool m_bIsUnicode = false;
    #endregion

    #region Class properties
    /// <summary>
    /// Non-empty Unicode string.
    /// </summary>
    public string Value
    {
      get
      {
        return m_strValue;
      }
      set
      {
        m_strValue = value;
        m_usStringLength = ( value != null ) ? ( ushort ) value.Length : ( ushort ) 0;
        m_bIsUnicode = !BiffRecordRawWithArray.IsAsciiString( value );
      }
    }
    /// <summary>
    /// Read-only. Returns minimum possible size of record's
    /// internal data array.
    /// </summary>
    public override int MinimumRecordSize
    {
      get
      {
        return 4;
      }
    }

    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor
    /// </summary>
    public  StringRecord()
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
    public  StringRecord( Stream stream, out int itemSize )
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
    public  StringRecord( int iReserve )
      : base( iReserve )
    {
    }

    #endregion

    #region Record Serialization
    /// <summary>
    /// Parse structure of record. Convert Data buffer to special
    /// values according to record specification.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">Offset to the record's data.</param>
    /// <param name="iLength">Length of the record's data.</param>
    /// <param name="version">Excel version used for infill.</param>
    public override void ParseStructure( DataProvider provider, int iOffset, int iLength, ExcelVersion version )
    {
      m_usStringLength = provider.ReadUInt16( iOffset );
      int iBytes;
      m_strValue = provider.ReadString( iOffset + 2, m_usStringLength, out iBytes, false );
      m_bIsUnicode = ( m_strValue.Length * 2 > iBytes );
    }

    /// <summary>
    /// In this method, a class must pack all of its properties into
    /// an internal data array, m_data. This method is called by
    /// FillStream, when the record must be serialized into a stream.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <returns>Size of the record data.</returns>
    /// <param name="iOffset">Offset in the buffer.</param>
    /// <param name="version">Excel version used for infill.</param>
    public override void InfillInternalData( DataProvider provider, int iOffset, ExcelVersion version )
    {
      m_iLength = GetStoreSize( version );
      provider.WriteUInt16( iOffset, m_usStringLength );

      int iCurOffset = iOffset + 2;
      provider.WriteStringNoLenUpdateOffset( ref iCurOffset, m_strValue, m_bIsUnicode );
    }
    /// <summary>
    /// Size of the required storage space. Read-only.
    /// </summary>
    public override int GetStoreSize( ExcelVersion version )
    {
      int stringSize =         ( m_bIsUnicode ) ?
        Encoding.Unicode.GetByteCount( m_strValue ) :
        m_strValue.Length;

      return DEF_FIXED_SIZE + stringSize;
    }
    #endregion
  }
}
  