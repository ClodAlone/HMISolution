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
  /// Stores the Username of the owner of the spreadsheet generator
  /// (on UNIX, it's the user's login; on Windows, it is the name you typed during
  /// installation).
  /// </summary>
  [ Biff( TBIFFRecord.WriteAccess ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class WriteAccessRecord : BiffRecordRaw
  {
    #region Class constants
    /// <summary>
    /// Default user name.
    /// </summary>
    private const string DEF_USER_NAME = "User";
    /// <summary>
    /// Minimum record size.
    /// </summary>
    private const int DEF_MIN_SIZE = 112;
    /// <summary>
    /// Maximum record size.
    /// </summary>
    private const int DEF_MAX_SIZE = 112;
    /// <summary>
    /// Space character value.
    /// </summary>
    private const byte DEF_SPACE = 0x20;
    #endregion

    #region Class members
    /// <summary>
    /// User name, Unicode string, 16-bit string length, 109 characters.
    /// </summary>
    private string m_strUserName = DEF_USER_NAME;
    #endregion

    #region Class properties
    /// <summary>
    /// User name, Unicode string, 16-bit string length, 109 characters.
    /// </summary>
    public string UserName
    {
      get
      {
        return m_strUserName;
      }
      set
      {
        m_strUserName = value;
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
        return DEF_MIN_SIZE;
      }
    }

    /// <summary>
    /// Read-only. Returns maximum possible size of record's
    /// internal data array.
    /// </summary>
    public override int MaximumRecordSize
    {
      get
      {
        return DEF_MAX_SIZE;
      }
    }
    /// <summary>
    /// Indicates whether record allows shorter data. Read-only.
    /// </summary>
    public override bool IsAllowShortData
    {
      get
      {
        return true;
      }
    }

    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor
    /// </summary>
    public  WriteAccessRecord()
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
    public  WriteAccessRecord( Stream stream, out int itemSize )
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
    public  WriteAccessRecord( int iReserve )
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
      int iFullLength;
      uint uiStringLen = provider.ReadUInt16( iOffset );

      if( uiStringLen < iLength )
      {
        m_strUserName = provider.ReadString16Bit( iOffset, out iFullLength );
      }
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
      m_iLength = iOffset;

      // If user name is not set, then apply default name.
      if( m_strUserName == null ) m_strUserName = DEF_USER_NAME;

      //provider.WriteString16BitUpdateOffset( ref iOffset, m_strUserName );
      provider.WriteUInt16( iOffset, ( ushort )m_strUserName.Length );
      iOffset += 2;

      provider.WriteStringNoLenUpdateOffset( ref iOffset, m_strUserName, false );

      if( iOffset - m_iLength < DEF_MIN_SIZE )
      {
        for( int i = 0, len = m_iLength - iOffset; i < len; i++, iOffset++ )
        {
          provider.WriteByte( iOffset, DEF_SPACE );
        }
      }

      m_iLength = DEF_MIN_SIZE;
    }
    #endregion
  }
}