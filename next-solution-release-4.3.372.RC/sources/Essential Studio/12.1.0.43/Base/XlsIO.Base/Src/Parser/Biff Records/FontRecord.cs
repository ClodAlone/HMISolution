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
#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  /// <summary>
  /// Font Record describes a font in the workbook (index = 0-3,5-infinity - skip 4)
  /// An element in the Font Table contains information about a used font,
  /// including character formatting.
  /// </summary>
  [ Biff( TBIFFRecord.Font ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class FontRecord  : BiffRecordRaw
  {
    #region Class constants
    /// <summary>
    /// Font attributes.
    /// </summary>
    [ Flags ]
    private enum FontAttributes : ushort
    {
      /// <summary>
      /// Indicates whether this font is in italics.
      /// </summary>
      Italic = 2,
      /// <summary>
      /// Indicates whether characters are strikeout.
      /// </summary>
      Strikeout  = 8,
      /// <summary>
      /// Whether to use the Mac outline font style (Mac only).
      /// </summary>
      MacOutline = 16,
      /// <summary>
      /// Whether to use the Mac shadow font style (Mac only).
      /// </summary>
      MacShadow  = 32,

      /// <summary>
      /// All known flags.
      /// </summary>
      AllKnown = Italic | Strikeout | MacOutline | MacShadow,
    }
    /// <summary>
    /// Incorrect hash value.
    /// </summary>
    private const int DEF_INCORRECT_HASH = -1;
    /// <summary>
    /// Offset to the byte that indicates whether string is unicode.
    /// </summary>
    private const int DEF_STRING_TYPE_OFFSET = 15;
    /// <summary>
    /// Default font color.
    /// </summary>
    public const int DefaultFontColor = 32767;
    #endregion

    #region Class members
    /// <summary>
    /// Height of the font (in twips = 1/20 of a point).
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usFontHeight = 200;
    /// <summary>
    /// Font attributes.
    /// </summary>
    [ BiffRecordPos( 2, 2 ) ]
    private FontAttributes m_attributes = 0;

    /// <summary>
    /// Palette color index.
    /// </summary>
    [ BiffRecordPos( 4, 2 ) ]
    private ushort m_usPaletteColorIndex = DefaultFontColor;

    /// <summary>
    /// Boldness (100-1000). Standard values are 0190H (400) for normal text
    /// and 02BCH (700) for bold text.
    /// </summary>
    [ BiffRecordPos( 6, 2 ) ]
    private ushort m_usBoldWeight = 400;

    /// <summary>
    /// Escapement:
    /// </summary>
    [ BiffRecordPos( 8, 2 ) ]
    private ushort m_SuperSubscript = 0;

    /// <summary>
    /// Underline type:
    /// </summary>
    [ BiffRecordPos( 10, 1 ) ]
    private byte  m_Underline = 0;

    /// <summary>
    /// Font family:
    /// </summary>
    [ BiffRecordPos( 11, 1 ) ]
    private byte   m_Family = 0;

    /// <summary>
    /// Character set:
    /// </summary>
    [ BiffRecordPos( 12, 1 ) ]
    private byte   m_Charset = 0;

    /// <summary>
    /// Not used.
    /// </summary>
    [ BiffRecordPos( 13, 1 ) ]
    private byte   m_Reserved = 0;

    /// <summary>
    /// Font name: Unicode string, 8-bit string length.
    /// </summary>
    [ BiffRecordPos( 14, TFieldType.String ) ]
    private string m_strFontName = "Arial";

    /// <summary>
    /// Cached hash code value.
    /// </summary>
    private int m_iHashCode = DEF_INCORRECT_HASH;
    /// <summary>
    /// Represents the baseline value which indicates whether superscript or subscript
    /// </summary>
    private int m_baseLine;
    #endregion

    #region Class properties
    /// <summary>
    /// Read-only. Font attributes.
    /// </summary>
    public ushort Attributes
    {
      get
      {
        return ( ushort )m_attributes;
      }
    }
    /// <summary>
    /// Height of the font (in twips = 1/20 of a point).
    /// </summary>
    public ushort FontHeight
    {
      get
      {
        return m_usFontHeight;
      }
      set
      {
        if( m_usFontHeight != value )
        {
          m_usFontHeight = value;
          m_iHashCode = DEF_INCORRECT_HASH;
        }
      }
    }

    /// <summary>
    /// Palette color index.
    /// </summary>
    public ushort PaletteColorIndex
    {
      get
      {
        return m_usPaletteColorIndex;
      }
      set
      {
        if( m_usPaletteColorIndex != value )
        {
          m_usPaletteColorIndex = value;
          m_iHashCode = DEF_INCORRECT_HASH;
        }
      }
    }

    /// <summary>
    /// Boldness (100-1000). Standard values are 0190H (400) for normal text
    /// and 02BCH (700) for bold text.
    /// </summary>
    public ushort BoldWeight
    {
      get
      {
        return m_usBoldWeight;
      }
      set
      {
        if( m_usBoldWeight != value )
        {
          m_usBoldWeight = value;
          m_iHashCode = DEF_INCORRECT_HASH;
        }
      }
    }

    /// <summary>
    /// Escapement:
    /// </summary>
    public ExcelFontVertialAlignment SuperSubscript
    {
      get
      {
        return ( ExcelFontVertialAlignment )m_SuperSubscript;
      }
      set
      {
        if( m_SuperSubscript != ( ushort )value )
        {
          m_SuperSubscript = ( ushort )value;
          m_iHashCode = DEF_INCORRECT_HASH;
        }
      }
    }
    /// <summary>
    /// Gets or sets the baseline value which indicates whether superscript or subscript
    /// </summary>
    public int Baseline
    {
        get
        {
            return m_baseLine;
        }
        set
        {
            m_baseLine = value;
        }
    }
    /// <summary>
    /// Underline type:
    /// </summary>
    public ExcelUnderline  Underline
    {
      get
      {
        return ( ExcelUnderline ) m_Underline;
      }
      set
      {
        if( m_Underline != ( byte )value )
        {
          m_Underline = ( byte ) value;
          m_iHashCode = DEF_INCORRECT_HASH;
        }
      }
    }

    /// <summary>
    /// Font family:
    /// </summary>
    public byte   Family
    {
      get
      {
        return m_Family;
      }
      set
      {
        if( m_Family != value )
        {
          m_Family = value;
          m_iHashCode = DEF_INCORRECT_HASH;
        }
      }
    }

    /// <summary>
    /// Character set:
    /// </summary>
    public byte   Charset
    {
      get
      {
        return m_Charset;
      }
      set
      {
        if( m_Charset != value )
        {
          m_Charset = value;
          m_iHashCode = DEF_INCORRECT_HASH;
        }
      }
    }

    /// <summary>
    /// Font name: Unicode string, 8-bit string length.
    /// </summary>
    public string FontName
    {
      get
      {
        return m_strFontName;
      }
      set
      {
        if( m_strFontName != value )
        {
          m_strFontName = value;
          m_iHashCode = DEF_INCORRECT_HASH;
        }
      }
    }
    /// <summary>
    /// True if characters are italic.
    /// </summary>
    public bool   IsItalic
    {
      get
      {
        return ( m_attributes & FontAttributes.Italic ) != 0;
      }
      set
      {
        if( value )
        {
          m_attributes |= FontAttributes.Italic;
        }
        else
        {
          m_attributes &= ~FontAttributes.Italic;
        }

        m_iHashCode = DEF_INCORRECT_HASH;
      }
    }
    /// <summary>
    /// True if characters are strikeout.
    /// </summary>
    public bool   IsStrikeout
    {
      get
      {
        return ( m_attributes & FontAttributes.Strikeout ) != 0;
      }
      set
      {
        if( value )
        {
          m_attributes |= FontAttributes.Strikeout;
        }
        else
        {
          m_attributes &= ~FontAttributes.Strikeout;
        }

        m_iHashCode = DEF_INCORRECT_HASH;
      }
    }

    /// <summary>
    /// Whether to use the Mac outline font style (Mac only).
    /// </summary>
    public bool   IsMacOutline
    {
      get
      {
        return ( m_attributes & FontAttributes.MacOutline ) != 0;
      }
      set
      {
        if( value )
        {
          m_attributes |= FontAttributes.MacOutline;
        }
        else
        {
          m_attributes &= ~FontAttributes.MacOutline;
        }

        m_iHashCode = DEF_INCORRECT_HASH;
      }
    }

    /// <summary>
    /// Whether to use the Mac shadow font style thing (Mac only).
    /// </summary>
    public bool   IsMacShadow
    {
      get
      {
        return ( m_attributes & FontAttributes.MacShadow ) != 0;
      }
      set
      {
        if( value )
        {
          m_attributes |= FontAttributes.MacShadow;
        }
        else
        {
          m_attributes &= ~FontAttributes.MacShadow;
        }

        m_iHashCode = DEF_INCORRECT_HASH;
      }
    }
    /// <summary>
    /// Not used.
    /// </summary>
    public byte   Reserved
    {
      get
      {
        return m_Reserved;
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
        return 16;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor
    /// </summary>
    public  FontRecord()
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
    public  FontRecord( Stream stream, out int itemSize )
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
    public  FontRecord( int iReserve )
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
      m_usFontHeight = provider.ReadUInt16( iOffset + 0 );
      m_attributes = ( FontAttributes )provider.ReadUInt16( iOffset + 2 );
      m_usPaletteColorIndex = provider.ReadUInt16( iOffset + 4 );
      m_usBoldWeight = provider.ReadUInt16( iOffset + 6 );
      m_SuperSubscript = provider.ReadUInt16( iOffset + 8 );
      m_Underline = provider.ReadByte( iOffset + 10 );
      m_Family = provider.ReadByte( iOffset + 11 );
      m_Charset = provider.ReadByte( iOffset + 12 );
      m_Reserved = provider.ReadByte( iOffset + 13 );
      m_strFontName = provider.ReadString8Bit( iOffset + 14, out iLength );
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

      provider.WriteUInt16( iOffset, m_usFontHeight );
      iOffset += 2;

      provider.WriteUInt16( iOffset, ( ushort )m_attributes );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usPaletteColorIndex );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usBoldWeight );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_SuperSubscript );
      iOffset += 2;

      provider.WriteByte( iOffset, m_Underline );
      iOffset++;

      provider.WriteByte( iOffset, m_Family );
      iOffset++;

      provider.WriteByte( iOffset, m_Charset );
      iOffset++;

      provider.WriteByte( iOffset, m_Reserved );
      iOffset++;


      int iStringLen = m_strFontName.Length;
      provider.WriteByte( iOffset, ( byte )iStringLen );
      iOffset++;

      if( iStringLen > 0 )
        provider.WriteStringNoLenUpdateOffset( ref iOffset, m_strFontName );
    }
    /// <summary>
    /// Size of the required storage space. Read-only.
    /// </summary>
    public override int GetStoreSize( ExcelVersion version )
    {
      return 16 + m_strFontName.Length * 2;
    }
    #endregion

    #region Class Overrides
    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns>
    /// True if the specified object is equal to the current object;
    /// otherwise False.
    /// </returns>
    public override bool Equals(object obj)
    {
      FontRecord font = obj as FontRecord;

      bool bResult =
           font.m_strFontName         == m_strFontName
        && font.m_usFontHeight        == m_usFontHeight
        && font.m_usPaletteColorIndex == m_usPaletteColorIndex
        && font.m_usBoldWeight        == m_usBoldWeight
        && font.m_Underline           == m_Underline
        && font.m_SuperSubscript      == m_SuperSubscript
        && font.m_Family              == m_Family
        && font.m_Charset             == m_Charset
        && ( ( font.m_attributes & FontAttributes.AllKnown ) == ( m_attributes & FontAttributes.AllKnown ) );

      return bResult;
    }

    /// <summary>
    /// Serves as a hash function for a particular type, suitable for
    /// use in hashing algorithms and data structures like a hash table.
    /// </summary>
    /// <returns>A hash code for the current object.</returns>
    public override int GetHashCode()
    {
      if( m_iHashCode == DEF_INCORRECT_HASH )
      {
        m_iHashCode = m_usFontHeight.GetHashCode()
          + ( m_attributes & FontAttributes.AllKnown ).GetHashCode()
          + m_usPaletteColorIndex.GetHashCode()
          + m_usBoldWeight.GetHashCode()
          + m_SuperSubscript.GetHashCode()
          + m_Underline.GetHashCode()
          + m_Family.GetHashCode()
          + m_Charset.GetHashCode()
          + m_strFontName.GetHashCode();
      }

      return m_iHashCode;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="raw"></param>
    public override void CopyTo(BiffRecordRaw raw)
    {
      FontRecord twin = raw as FontRecord;

      if( twin == null )
        throw new ArgumentNullException( "twin" );

      twin.m_usFontHeight = m_usFontHeight;
      twin.m_attributes = m_attributes;
      twin.m_usPaletteColorIndex = m_usPaletteColorIndex;
      twin.m_usBoldWeight = m_usBoldWeight;
      twin.m_SuperSubscript = m_SuperSubscript;
      twin.m_Underline    = m_Underline;
      twin.m_Family       = m_Family;
      twin.m_Charset      = m_Charset;
      twin.m_strFontName  = m_strFontName;
      twin.m_iHashCode    = m_iHashCode;
    }

    #endregion

    #region Class Public Methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="record"></param>
    /// <returns></returns>
    public int CompareTo( FontRecord record )
    {
      if( record == null ) return 1;

//      int result;
      int result = GetHashCode() - record.GetHashCode();

      if( result != 0 ) return result;

      result = m_usFontHeight - record.m_usFontHeight;
      if( result != 0 ) return result;

      result = m_strFontName.CompareTo( record.m_strFontName );
      if( result != 0 ) return result;

      result = ( m_attributes & FontAttributes.AllKnown ).CompareTo(
        record.m_attributes & FontAttributes.AllKnown );

      if( result != 0 ) return result;

      result = m_usPaletteColorIndex - record.m_usPaletteColorIndex;
      if( result != 0 ) return result;

      result = m_usBoldWeight - record.m_usBoldWeight;
      if( result != 0 ) return result;

      result = m_SuperSubscript - record.m_SuperSubscript;
      if( result != 0 ) return result;

      result = m_Underline - record.m_Underline;
      if( result != 0 ) return result;

      result = m_Family - record.m_Family;
      if( result != 0 ) return result;

      result = m_Charset - record.m_Charset;
      if( result != 0 ) return result;

      return 0;
    }
    #endregion
  }
}
