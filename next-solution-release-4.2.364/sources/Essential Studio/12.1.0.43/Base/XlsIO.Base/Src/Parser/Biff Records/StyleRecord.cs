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

using Syncfusion.XlsIO.Implementation.Exceptions;
using Syncfusion.XlsIO.Interfaces;
#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  /// <summary>
  /// Style Record:
  /// Describes a built-in style in the GUI or user defined style.
  /// </summary>
  [ Biff( TBIFFRecord.Style ) ]
  [ CLSCompliant( false ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  public class StyleRecord
    : BiffRecordRaw
    , INamedObject
  {
    #region Class constants
    /// <summary>
    /// Extended format index bit mask.
    /// </summary>
    private const ushort DEF_XF_INDEX_BIT_MASK = 0x0FFF;
    #endregion

    #region Class members
    /// <summary>
    /// The actual index of the style extended format record.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usExtFormatIndex = 0;
    /// <summary>
    /// Whether the style is built in or user-defined.
    /// </summary>
    [ BiffRecordPos( 1, 7, TFieldType.Bit ) ]
    private bool   m_bIsBuildIn = true;
    /// <summary>
    /// If this is a built in style, then it is the number of the built in style.
    /// If this is user-defined style, then it is length of style's name.
    /// </summary>
    [ BiffRecordPos( 2, 1 ) ]
    private byte   m_BuildInOrNameLen = 0;
    /// <summary>
    /// The row or column level of the style.
    /// </summary>
    private byte   m_OutlineStyleLevel = 0xFF;
    /// <summary>
    /// The style's name (if user-defined).
    /// </summary>
    private string m_strName = null;
    /// <summary>
    /// Represents the default external format index based on workbook version
    /// </summary>
    private ushort m_DefXFIndex;
    private bool m_isBuiltInCustomized;
    #endregion

    #region Class properties
    /// <summary>
    /// Whether the style is built in or user-defined.
    /// </summary>
    public bool   IsBuildInStyle
    {
      get
      {
        return m_bIsBuildIn;
      }
      set
      {
        m_bIsBuildIn = value;
      }
    }

    /// <summary>
    /// The actual index of the style extended format record.
    /// </summary>
    public ushort ExtendedFormatIndex
    {
      get
      {
        if ( DefXFIndex > 0 )
            return ( ushort )( m_usExtFormatIndex & DefXFIndex );
        else
            return ( ushort )( m_usExtFormatIndex & DEF_XF_INDEX_BIT_MASK );
      }
      set
      {
        m_usExtFormatIndex = value;
        //SetUInt16BitsByMask( ref m_usExtFormatIndex, DEF_XF_INDEX_BIT_MASK, value );
      }
    }

    /// <summary>
    /// The row or column level of the style.
    /// </summary>
    public byte   BuildInOrNameLen
    {
      get
      {
        return m_BuildInOrNameLen;
      }
      set
      {
        m_BuildInOrNameLen = value;
      }
    }

    /// <summary>
    /// The row or column level of the style.
    /// </summary>
    public byte OutlineStyleLevel
    {
      get
      {
        return m_OutlineStyleLevel;
      }
      set
      {
        m_OutlineStyleLevel = value;
      }
    }
    /// <summary>
    /// The style's name (if user-defined).
    /// </summary>
    public string StyleName
    {
      get
      {
        return m_strName;
      }
      set
      {
        if( value == null )
          throw new ArgumentNullException( "StyleName" );

        if( value.Length == 0 )
          throw new ArgumentException( "StyleName - string cannot be empty." );

        if( value.Length > 255 )
          throw new ArgumentOutOfRangeException( "StyleName", "Style name cannot be larger 255 symbols." );

        m_strName = value;
        m_bIsBuildIn = false;

        // Line length cannot be more than 255 symbols.
        checked
        {
          m_BuildInOrNameLen = ( byte )m_strName.Length;
        }
      }
    }
    public bool IsBuiltIncustomized
    {
        get
        {
            return m_isBuiltInCustomized;
        }
        set
        {
            m_isBuiltInCustomized = value;
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

    /// <summary>
    /// Returns style name. Read-only.
    /// </summary>
    public string Name
    {
      get
      {
        return m_strName;
      }
    }
    /// <summary>
    /// Gets or sets the default external format index based on workbook version
    /// </summary>
    internal ushort DefXFIndex
    {
        get
        {
            return m_DefXFIndex;
        }
        set
        {
            m_DefXFIndex = value;
        }
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor
    /// </summary>
    public  StyleRecord()
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
    public  StyleRecord( Stream stream, out int itemSize )
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
    public  StyleRecord( int iReserve )
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
    /// <exception cref="WrongBiffRecordDataException">
    /// If there is any internal error.
    /// </exception>
    public override void ParseStructure( DataProvider provider, int iOffset, int iLength, ExcelVersion version )
    {
      m_iLength = iLength;
      m_usExtFormatIndex = provider.ReadUInt16( iOffset );
      m_bIsBuildIn = provider.ReadBit( iOffset + 1, 7 );
      m_BuildInOrNameLen = provider.ReadByte( iOffset + 2 );

      m_usExtFormatIndex &= ( ushort )0x0fff;

      if( m_bIsBuildIn )
      {
        if( iLength > 4 )
          throw new LargeBiffRecordDataException();
        
        m_OutlineStyleLevel = provider.ReadByte( iOffset + 3 );
      }
      else
      {
        bool bUnicode = provider.ReadByte( iOffset + 4 ) != 0;
        int iLen = /*( bUnicode )
          ? iLength - iOffset - 4
          : */m_BuildInOrNameLen;
        int iBytes;

        m_strName = provider.ReadString( iOffset + 4, iLen, out iBytes, false );
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
      if( m_usExtFormatIndex > 0x0fff )
        throw new ArgumentOutOfRangeException();

      m_iLength = GetStoreSize( version );

      provider.WriteUInt16( iOffset, m_usExtFormatIndex );
      provider.WriteBit( iOffset + 1, m_bIsBuildIn, 7 );
      provider.WriteByte( iOffset + 2, m_BuildInOrNameLen );

      if( m_bIsBuildIn )
      {
        provider.WriteByte( iOffset + 3, m_OutlineStyleLevel );
      }
      else
      {
        provider.WriteByte( iOffset + 2, ( byte )m_strName.Length );
        provider.WriteByte( iOffset + 3, 0 );

        byte[] arrStringBuffer = Encoding.Unicode.GetBytes( m_strName );

        provider.WriteByte( iOffset + 4, 1 );
        provider.WriteBytes( iOffset + 5, arrStringBuffer, 0, arrStringBuffer.Length );
      }
    }
    /// <summary>
    /// Size of the required storage space. Read-only.
    /// </summary>
    public override int GetStoreSize( ExcelVersion version )
    {
      if( m_bIsBuildIn )
      {
        return 4;
      }
      else
      {
        int iLength = Encoding.Unicode.GetByteCount( m_strName );
        return 5 + iLength;
      }
    }
    #endregion

    #region Class Overrides
    /// <summary>
    /// Copies this record into another StyleRecord.
    /// </summary>
    /// <param name="raw"></param>
    public override void CopyTo( BiffRecordRaw raw )
    {
      StyleRecord twin = raw as StyleRecord;

      if( twin == null )
        throw new ArgumentNullException( "Wrong BiffRecord type" );

      twin.m_usExtFormatIndex = m_usExtFormatIndex;
      twin.m_bIsBuildIn = m_bIsBuildIn;
      twin.m_BuildInOrNameLen = m_BuildInOrNameLen;
      twin.m_OutlineStyleLevel = m_OutlineStyleLevel;
      twin.m_strName = m_strName;
    }

    #endregion
  }
}