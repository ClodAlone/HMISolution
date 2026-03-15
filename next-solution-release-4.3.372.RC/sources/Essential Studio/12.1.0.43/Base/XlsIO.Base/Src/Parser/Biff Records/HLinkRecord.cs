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
  /// The record represents an empty cell.
  /// It contains the cell address and formatting information.
  /// </summary>
  [ Biff( TBIFFRecord.HLink ) ]
  [ CLSCompliant( false ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  public class HLinkRecord
    : BiffRecordRawWithArray
  {
    #region Class constants
    /// <summary>
    /// Standard link GUID.
    /// </summary>
    public static readonly Guid GUID_STDLINK     = new Guid( "79EAC9D0-BAF9-11CE-8C82-00AA004BA90B" );
    /// <summary>
    /// URL moniker GUID.
    /// </summary>
    public static readonly Guid GUID_URLMONIKER  = new Guid( "79EAC9E0-BAF9-11CE-8C82-00AA004BA90B" );
    /// <summary>
    /// File moniker GUID.
    /// </summary>
    public static readonly Guid GUID_FILEMONIKER = new Guid( "00000303-0000-0000-C000-000000000046" );

    /// <summary>
    /// Standard link GUID bytes.
    /// </summary>
    public static readonly byte[] GUID_STDLINK_BYTES = GUID_STDLINK.ToByteArray();
    /// <summary>
    /// URL moniker GUID bytes.
    /// </summary>
    public static readonly byte[] GUID_URLMONIKER_BYTES = GUID_URLMONIKER.ToByteArray();
    /// <summary>
    /// File moniker GUID bytes.
    /// </summary>
    public static readonly byte[] GUID_FILEMONIKER_BYTES = GUID_FILEMONIKER.ToByteArray();
    /// <summary>
    /// Unknown block in the file link.
    /// </summary>
    public static readonly byte[] FILE_UNKNOWN = new byte[]
    {
      0xFF, 0xFF, 0xAD, 0xDE, 0x00, 0x00, 0x00, 0x00,
      0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
      0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
    };
    /// <summary>
    /// Another unknown block in the file link.
    /// </summary>
    public static readonly byte[] FILE_UNKNOWN2 = new byte[]{ 0x03, 0x00 };

    /// <summary>
    /// GUID size.
    /// </summary>
    public const int GUID_LENGTH = 16;
    /// <summary>
    /// Start byte of the standard link GUID.
    /// </summary>
    public const int STDLINK_START_BYTE = 8;
    /// <summary>
    /// Start byte of the URL moniker GUID.
    /// </summary>
    public const int URLMONIKER_START_BYTE = 0;
    /// <summary>
    /// Start byte of the file moniker GUID.
    /// </summary>
    public const int FILEMONIKER_START_BYTE = 0;
    #endregion

    #region Class members
    /// <summary>
    /// Index to first row.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private uint m_usFirstRow;
    /// <summary>
    /// Index to last row.
    /// </summary>
    [ BiffRecordPos( 2, 2 ) ]
    private uint m_usLastRow;
    /// <summary>
    /// Index to first column.
    /// </summary>
    [ BiffRecordPos( 4, 2 ) ]
    private uint m_usFirstColumn;
    /// <summary>
    /// Index to last column.
    /// </summary>
    [ BiffRecordPos( 6, 2 ) ]
    private uint m_usLastColumn;
    /// <summary>
    /// Unknown value: 00000002H.
    /// </summary>
    [ BiffRecordPos( 24, 4 ) ]
    private uint m_uiUnknown = 0x2;
    /// <summary>
    /// Option flags.
    /// </summary>
    [ BiffRecordPos( 28, 4 ) ]
    private uint m_uiOptions;

    /// <summary>
    /// True if file link or URL.
    /// False if no link.
    /// </summary>
    [ BiffRecordPos( 28, 0, TFieldType.Bit ) ]
    private bool m_bFileOrUrl;
    /// <summary>
    /// True if absolute path or URL.
    /// False if relative file path.
    /// </summary>
    [ BiffRecordPos( 28, 1, TFieldType.Bit ) ]
    private bool m_bAbsolutePathOrUrl;
    /// <summary>
    /// If this field and other description bits are
    /// True, then there is a description.
    /// False if there is no description.
    /// </summary>
    [ BiffRecordPos( 28, 2, TFieldType.Bit ) ]
    private bool m_bDescription1;
    /// <summary>
    /// True if there is a text mark.
    /// False if there is no text mark.
    /// </summary>
    [ BiffRecordPos( 28, 3, TFieldType.Bit ) ]
    private bool m_bTextMark;
    /// <summary>
    /// If this field and other description bits are
    /// True, then there is description.
    /// False if there is no description.
    /// </summary>
    [ BiffRecordPos( 28, 4, TFieldType.Bit ) ]
    private bool m_bDescription2;
    /// <summary>
    /// True if there is a target frame.
    /// False if there is no target frame.
    /// </summary>
    [ BiffRecordPos( 28, 7, TFieldType.Bit ) ]
    private bool m_bTargetFrame;
    /// <summary>
    /// True if UNC path (incl. server name).
    /// False if file link or URL.
    /// </summary>
    [ BiffRecordPos( 29, 0, TFieldType.Bit ) ]
    private bool m_bUncPath;

    /// <summary>
    /// (optional, see option flags) Character count of description text,
    /// including trailing zero word.
    /// </summary>
    private uint m_uiDescriptionLen;
    /// <summary>
    /// (optional, see option flags) Character array of description text,
    /// no Unicode string header, always 16-bit characters, zero-terminated.
    /// </summary>
    private string m_strDescription = string.Empty;
    /// <summary>
    /// (optional, see option flags) Character count of target frame,
    /// including trailing zero word.
    /// </summary>
    private uint m_uiTargetFrameLen;
    /// <summary>
    /// (optional, see option flags) Character array of target frame,
    /// no Unicode string header, always 16-bit characters, zero-terminated.
    /// </summary>
    private string m_strTargetFrame = string.Empty;
    /// <summary>
    /// (optional, see option flags) Character count of the text mark,
    /// including trailing zero word.
    /// </summary>
    private uint m_uiTextMarkLen;
    /// <summary>
    /// (optional, see option flags) Character array of the text
    /// mark without "#" sign, no Unicode string header, always
    /// 16-bit characters, zero-terminated.
    /// </summary>
    private string m_strTextMark = string.Empty;

    /// <summary>
    /// HyperLink type.
    /// </summary>
    private ExcelHyperLinkType m_LinkType;
    #endregion

    #region Url members
    /// <summary>
    /// URL length.
    /// </summary>
    private uint m_uiUrlLen;
    /// <summary>
    /// URL string.
    /// </summary>
    private string m_strUrl = string.Empty;
    #endregion

    #region File members
    /// <summary>
    /// 
    /// </summary>
    private ushort m_usDirUpLevel;
    /// <summary>
    /// 
    /// </summary>
    private uint m_uiFileNameLen;
    /// <summary>
    /// 
    /// </summary>
    private string m_strFileName = string.Empty;
    /// <summary>
    /// 
    /// </summary>
    private uint   m_uiFollowSize;
    /// <summary>
    /// 
    /// </summary>
    private uint   m_uiXFilePathLen;
    /// <summary>
    /// 
    /// </summary>
    private string m_strXFilePath;
    #endregion

    #region Unc members
    /// <summary>
    /// 
    /// </summary>
    private uint m_uiUncLen;
    /// <summary>
    /// 
    /// </summary>
    private string m_strUnc;
    #endregion

    #region Class Properties
    /// <summary>
    /// Index to first row.
    /// </summary>
    public uint FirstRow
    {
      get
      {
        return m_usFirstRow;
      }
      set
      {
        m_usFirstRow = value;
      }
    }
    /// <summary>
    /// Index to first column.
    /// </summary>
    public uint FirstColumn
    {
      get
      {
        return m_usFirstColumn;
      }
      set
      {
        m_usFirstColumn = value;
      }
    }
    /// <summary>
    /// Index to last row.
    /// </summary>
    public uint LastRow
    {
      get
      {
        return m_usLastRow;
      }
      set
      {
        m_usLastRow = value;
      }
    }
    /// <summary>
    /// Index to last column.
    /// </summary>
    public uint LastColumn
    {
      get
      {
        return m_usLastColumn;
      }
      set
      {
        m_usLastColumn = value;
      }
    }

    /// <summary>
    /// Unknown value: 00000002H. Read-only.
    /// </summary>
    public uint   Unknown
    {
      get
      {
        return m_uiUnknown;
      }
    }
    /// <summary>
    /// Option flags. Read-only.
    /// </summary>
    public uint   Options
    {
      get
      {
        return m_uiOptions;
      }
#if DEBUG
      set
      {
        m_uiOptions = value;
      }
#endif
    }


    /// <summary>
    /// True if file link or URL.
    /// False if no link.
    /// </summary>
    public bool   IsFileOrUrl
    {
      get
      {
        return m_bFileOrUrl;
      }
      set
      {
        m_bFileOrUrl = value;
      }
    }

    /// <summary>
    /// True if absolute path or URL.
    /// False if relative file path.
    /// </summary>
    public bool   IsAbsolutePathOrUrl
    {
      get
      {
        return m_bAbsolutePathOrUrl;
      }
      set
      {
        m_bAbsolutePathOrUrl = value;
      }
    }

    /// <summary>
    /// True if there is description.
    /// False if there is no description.
    /// </summary>
    public bool   IsDescription
    {
      get
      {
        return m_bDescription1 && m_bDescription2;
      }
      set
      {
        m_bDescription1 = value;
        m_bDescription2 = value;
      }
    }

    /// <summary>
    /// True if there is a text mark.
    /// False if there is no text mark.
    /// </summary>
    public bool   IsTextMark
    {
      get
      {
        return m_bTextMark;
      }
      set
      {
        m_bTextMark = value;
      }
    }

    /// <summary>
    /// True if there is a target frame.
    /// False if there is no target frame.
    /// </summary>
    public bool   IsTargetFrame
    {
      get
      {
        return m_bTargetFrame;
      }
      set
      {
        m_bTargetFrame = value;
      }
    }

    /// <summary>
    /// True if UNC path (incl. server name).
    /// False if file link or URL.
    /// </summary>
    public bool   IsUncPath
    {
      get
      {
        return m_bUncPath;
      }
      set
      {
        m_bUncPath = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool   CanBeUrl
    {
      get
      {
        return IsFileOrUrl && IsAbsolutePathOrUrl && !IsUncPath;
      }
      set
      {
        IsFileOrUrl         = value;
        IsAbsolutePathOrUrl = value;
        IsUncPath           = !value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool   CanBeFile
    {
      get
      {
        return IsFileOrUrl && !IsUncPath;
      }
      set
      {
        IsFileOrUrl = value;
        IsUncPath   = !value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool   CanBeUnc
    {
      get
      {
        return IsFileOrUrl && IsAbsolutePathOrUrl && IsUncPath;
      }
      set
      {
        if( value )
        {
          IsFileOrUrl = value;
          IsAbsolutePathOrUrl = value;
          IsUncPath = value;
        }
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public bool   CanBeWorkbook
    {
      get
      {
        return !IsFileOrUrl && !IsAbsolutePathOrUrl && IsTextMark && !IsUncPath;
      }
      set
      {
        if( value )
        {
          IsFileOrUrl = false;
          IsAbsolutePathOrUrl = false;
          IsTextMark = true;
          IsUncPath = false;
        }
      }
    }
    /// <summary>
    /// (optional, see option flags) Character count of description text,
    /// including trailing zero word. Read-only.
    /// </summary>
    public uint   DescriptionLen
    {
      get
      {
        return m_uiDescriptionLen;
      }
    }

    /// <summary>
    /// (optional, see option flags) Character array of description text,
    /// no Unicode string header, always 16-bit characters, zero-terminated.
    /// </summary>
    public string Description
    {
      get
      {
        return m_strDescription;
      }
      set
      {
        if( m_strDescription != value )
        {
          m_strDescription = ( value != null ) ? value: string.Empty;
          
          m_uiDescriptionLen = ( uint )( ( value == null )
            ? 0
            : ( m_strDescription.Length + 1 ) );

          IsDescription = true;
        }
      }
    }

    /// <summary>
    /// (optional, see option flags) Character count of target frame,
    /// including trailing zero word. Read-only.
    /// </summary>
    public uint   TargetFrameLen
    {
      get
      {
        return m_uiTargetFrameLen;
      }
    }
    /// <summary>
    /// (optional, see option flags) Character array of target frame,
    /// no Unicode string header, always 16-bit characters, zero-terminated.
    /// </summary>
    public string TargetFrame
    {
      get
      {
        return m_strTargetFrame;
      }
      set
      {
        if( m_strTargetFrame != value )
        {
          m_strTargetFrame = (value!= null) ? value: string.Empty;
          
          m_uiTargetFrameLen = (uint) ( ( value == null )
            ? 0
            : ( m_strTargetFrame.Length + 1 ) );
        }
      }
    }
    /// <summary>
    /// (optional, see option flags) Character count of the text mark,
    /// including trailing zero word.
    /// </summary>
    public uint   TextMarkLen
    {
      get
      {
        return m_uiTextMarkLen;
      }
    }
    /// <summary>
    /// (optional, see option flags) Character array of the text
    /// mark without the "#" sign, no Unicode string header, always
    /// 16-bit characters, zero-terminated.
    /// </summary>
    public string TextMark
    {
      get
      {
        return m_strTextMark;
      }
      set
      {
        if( m_strTextMark != value )
        {
          m_strTextMark = (value!= null) ? value : string.Empty;
          
          m_uiTextMarkLen = (uint) ( ( value == null )
            ? 0
            : ( m_strTextMark.Length + 1 ) );

          m_bTextMark = true;
        }
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public ExcelHyperLinkType  LinkType
    {
      get
      {
        return m_LinkType;
      }
      set
      {
        switch( value )
        {
          case ExcelHyperLinkType.File:
            CanBeFile = true;
            break;

          case ExcelHyperLinkType.Unc:
            CanBeUnc = true;
            break;

          case ExcelHyperLinkType.Url:
            CanBeUrl = true;
            break;

          case ExcelHyperLinkType.Workbook:
            CanBeWorkbook = true;
            break;
        }

        m_LinkType = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public bool   IsUrl
    {
      get
      {
        return ( m_LinkType == ExcelHyperLinkType.Url );
      }
      set
      {
        if( value )
        {
          m_LinkType = ExcelHyperLinkType.Url;
          CanBeUrl = true;
        }
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool   IsFileName
    {
      get
      {
        return m_LinkType == ExcelHyperLinkType.File;
      }
      set
      {
        if( value )
        {
          m_LinkType = ExcelHyperLinkType.File;
          CanBeFile = true;
        }
      }
    }


    #region /* comment */
//    /// <summary>
//    /// Read-only. Returns minimum possible size of record's
//    /// internal data array.
//    /// </summary>
//    override public int MinimumRecordSize
//    {
//      get
//      {
//        return 6;
//      }
//    }
//
//    /// <summary>
//    /// Read-only. Returns maximum possible size of record's
//    /// internal data array.
//    /// </summary>
//    override public int MaximumRecordSize
//    {
//      get
//      {
//        return 6;
//      }
//    }
    #endregion

    #endregion

    #region Url Properties
    /// <summary>
    /// 
    /// </summary>
    public uint   UrlLen
    {
      get
      {
        return m_uiUrlLen;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public string Url
    {
      get
      {
        return m_strUrl;
      }
      set
      {
        if( m_strUrl != value )
        {
          m_strUrl = value;
          m_uiUrlLen = (uint) ( (value != null )
            ? value.Length * 2 + 2
            : 2 );
          IsUrl = true;
        }
      }
    }
    #endregion

    #region File properties
    /// <summary>
    /// 
    /// </summary>
    public ushort DirUpLevel
    {
      get
      {
        return m_usDirUpLevel;
      }
      set
      {
        m_usDirUpLevel = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public uint FileNameLen
    {
      get
      {
        return m_uiFileNameLen;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public string FileName
    {
      get
      {
        return m_strFileName;
      }
      set
      {
        m_strFileName = ( value != null ) ? value : string.Empty;
        //m_uiFileNameLen = (uint) m_strFileName.Length * 2 + 2;
        m_uiFileNameLen = ( uint )m_strFileName.Length;
        IsFileName = true;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public uint   FollowSize
    {
      get
      {
        return m_uiFollowSize;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public uint   XFilePathLen
    {
      get
      {
        return m_uiXFilePathLen;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public string XFilePath
    {
      get
      {
        return m_strXFilePath;
      }
      set
      {
        m_strXFilePath = (value != null ) ? value : string.Empty;
        m_uiXFilePathLen = (uint) m_strXFilePath.Length * 2 + 2;
      }
    }
    #endregion

    #region Unc Properties
    /// <summary>
    /// Read-only.
    /// </summary>
    public uint UncLen
    {
      get
      {
        return m_uiUncLen;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public string UncPath
    {
      get
      {
        return m_strUnc;
      }
      set
      {
        m_strUnc = value;
        m_uiUncLen = (uint)m_strUnc.Length + 1;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor fills all data with default values.
    /// </summary>
    public  HLinkRecord()
      : base()
    {
    }

    /// <summary>
    /// Read / initialize constructor.
    /// </summary>
    /// <param name="stream">Stream from which record data should be read.</param>
    /// <param name="itemSize">Size of read item.</param>
    /// <exception cref="System.ArgumentNullException">If stream is not specified.</exception>
    /// <exception cref="System.ApplicationException">If stream does not support read or seek operations.</exception>
    public  HLinkRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }

    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  HLinkRecord( int iReserve )
      : base( iReserve )
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
      m_usFirstRow = GetUInt16( 0 );
      m_usLastRow = GetUInt16( 2 );
      m_usFirstColumn = GetUInt16( 4 );
      m_usLastColumn = GetUInt16( 6 );
      m_uiUnknown = GetUInt32( 24 );
      m_uiOptions = GetUInt32( 28 );
      m_bFileOrUrl = GetBit( 28, 0 );
      m_bAbsolutePathOrUrl = GetBit( 28, 1 );
      m_bDescription1 = GetBit( 28, 2 );
      m_bTextMark = GetBit( 28, 3 );
      m_bDescription2 = GetBit( 28, 4 );
      m_bTargetFrame = GetBit( 28, 7 );
      m_bUncPath = GetBit( 29, 0 );
      
      int iOffset = 32;
      
      if( IsDescription ) ParseDescription( ref iOffset );
      if( IsTargetFrame ) ParseTargetFrame( ref iOffset );

      ParseSpecialData( ref iOffset );

      if( IsTextMark )
        ParseTextMark( ref iOffset );
    }

    /// <summary>
    /// In this method, a class must pack all of its properties into
    /// an internal data array, m_data. This method is called by
    /// FillStream, when the record must be serialized into a stream.
    /// </summary>
    public override void InfillInternalData( ExcelVersion version )
    {
      SetOptionFlags();
      m_uiUnknown = 0x2;

      AutoGrowData = true;
      SetUInt16( 0, (ushort) m_usFirstRow );
      SetUInt16( 2, (ushort) m_usLastRow );
      SetUInt16( 4, (ushort) m_usFirstColumn );
      SetUInt16( 6, (ushort) m_usLastColumn );
      SetUInt32( 24, m_uiUnknown );
      SetUInt32( 28, m_uiOptions );
      SetBit( 28, m_bFileOrUrl, 0 );
      SetBit( 28, m_bAbsolutePathOrUrl, 1 );
      SetBit( 28, m_bDescription1, 2 );
      SetBit( 28, m_bTextMark, 3 );
      SetBit( 28, m_bDescription2, 4 );
      SetBit( 28, m_bTargetFrame, 7 );
      SetBit( 29, m_bUncPath, 0 );
      m_iLength = 32;

      SetBytes( 8, GUID_STDLINK_BYTES, 0, GUID_STDLINK_BYTES.Length );
      //m_iLength += 8;

      if( IsDescription )
        InfillLenAndString( ref m_uiDescriptionLen, ref m_strDescription, false );

      if( IsTargetFrame )
        InfillLenAndString( ref m_uiTargetFrameLen, ref m_strTargetFrame, false );

      InfillSpecialData();

      if( IsTextMark )
        InfillLenAndString( ref m_uiTextMarkLen, ref m_strTextMark, false );
    }

    #endregion

    #region Class parse helper methods
    /// <summary>
    /// Parses description.
    /// </summary>
    /// <param name="iOffset">Offset to the description data.</param>
    private void ParseDescription( ref int iOffset )
    {
      if( !IsDescription )
        throw new ArgumentException( "There is no description." );

      m_uiDescriptionLen = GetUInt32( iOffset );
      iOffset += 4;

      if( iOffset + m_uiDescriptionLen * 2 > m_iLength )
        throw new WrongBiffRecordDataException( "Description" );

      m_strDescription = Encoding.Unicode.GetString( 
        GetBytes( iOffset, (int) m_uiDescriptionLen * 2 ), 0,
        (int) m_uiDescriptionLen * 2 - 2 );

      iOffset += (int) m_uiDescriptionLen * 2;
    }
    /// <summary>
    /// Parses target frame.
    /// </summary>
    /// <param name="iOffset">Offset to the target frame data.</param>
    private void ParseTargetFrame( ref int iOffset )
    {
      if( !IsTargetFrame )
        throw new ArgumentException( "There is no target frame." );

      m_uiTargetFrameLen = GetUInt32( iOffset );
      iOffset += 4;

      m_strTargetFrame = Encoding.Unicode.GetString( 
        GetBytes( iOffset, (int) m_uiTargetFrameLen * 2 ), 0,
        (int) m_uiTargetFrameLen * 2 - 2 );

      iOffset += (int) m_uiTargetFrameLen * 2;
    }
    /// <summary>
    /// Parses special data block.
    /// </summary>
    /// <param name="iOffset">Offset to the data.</param>
    private void ParseSpecialData( ref int iOffset )
    {
      if( CheckUrl( ref iOffset ) )
      {
        LinkType = ExcelHyperLinkType.Url;
        ParseUrl( ref iOffset );
      }
      else if ( CheckLocalFile( ref iOffset ) )
      {
        LinkType = ExcelHyperLinkType.File;
        ParseFile( ref iOffset );
      }
      else if( CheckUnc( ref iOffset ) )
      {
        LinkType = ExcelHyperLinkType.Unc;
        ParseUnc( ref iOffset );
      }
      else //if( CanBeFile )
      {
        LinkType = ExcelHyperLinkType.Workbook;
        ParseWorkbook( ref iOffset );
      }
    }
    /// <summary>
    /// Parses text mark.
    /// </summary>
    /// <param name="iOffset">Offset to the data.</param>
    private void ParseTextMark( ref int iOffset )
    {
      if( !IsTextMark )
        throw new ArgumentException( "There is no text mark." );

      if( iOffset < m_iLength )
      {
        m_uiTextMarkLen = GetUInt32( iOffset );
        iOffset += 4;

        m_strTextMark = Encoding.Unicode.GetString( m_data, iOffset, ( int )( m_uiTextMarkLen - 1 ) * 2 );

        iOffset += (int) m_uiTextMarkLen * 2;
      }
    }
    /// <summary>
    /// Checks whether it is ULR link.
    /// </summary>
    /// <param name="iOffset">Offset to the GUID to check.</param>
    /// <returns>True if it is URL link.</returns>
    private bool CheckUrl( ref int iOffset )
    {
      bool result = /*CanBeUrl &&*/ CompareArrays( m_data, iOffset, GUID_URLMONIKER_BYTES,
        0, GUID_LENGTH );

      if( result ) iOffset += GUID_LENGTH;

      return result;
    }
    /// <summary>
    /// Checks whether it is link to a local file.
    /// </summary>
    /// <param name="iOffset">Offset to the GUID to check.</param>
    /// <returns>True if it is link to the local file.</returns>
    private bool CheckLocalFile( ref int iOffset )
    {
      bool result = CanBeFile && CompareArrays( m_data, iOffset, GUID_FILEMONIKER_BYTES,
        0, GUID_LENGTH );

      if( result ) iOffset += GUID_LENGTH;

      return result;
    }
    /// <summary>
    /// Checks whether it is link to a UNC.
    /// </summary>
    /// <param name="iOffset">Offset to the GUID to check.</param>
    /// <returns>True if it is link to the UNC.</returns>
    private bool CheckUnc( ref int iOffset )
    {
      return CanBeUnc;
    }
    /// <summary>
    /// Parses URL.
    /// </summary>
    /// <param name="iOffset">Offset to the URL data.</param>
    private void ParseUrl( ref int iOffset )
    {
      m_uiUrlLen = GetUInt32( iOffset );
      iOffset += 4;

      m_strUrl = Encoding.Unicode.GetString( m_data, iOffset, ( int )m_uiUrlLen);
      
      int iLen = m_strUrl.IndexOf( '\0' );
      
      if ( iLen != -1 )
        m_strUrl = m_strUrl.Substring( 0, iLen );

      iOffset += ( int )m_uiUrlLen;
    }
    /// <summary>
    /// Parses link to a file.
    /// </summary>
    /// <param name="iOffset">Offset to the data.</param>
    private void ParseFile( ref int iOffset )
    {
      m_usDirUpLevel = GetUInt16( iOffset );
      iOffset += 2;

      m_uiFileNameLen = GetUInt32( iOffset );
      iOffset += 4;

      m_strFileName = BiffRecordRaw.LatinEncoding.GetString( m_data, iOffset, ( int )m_uiFileNameLen - 1 );
      iOffset += ( int )m_uiFileNameLen;
      m_uiFileNameLen--;

      // SkipUnknown bytes
      iOffset += FILE_UNKNOWN.Length;

      m_uiFollowSize = GetUInt32( iOffset );
      iOffset += 4;

      if( m_uiFollowSize > 0 )
      {
        int iBytesCount = GetInt32( iOffset );
        iOffset += 4;

        iOffset += 2; // two unknown bytes
        m_strXFilePath = Encoding.Unicode.GetString( m_data, iOffset, iBytesCount );
        iOffset += iBytesCount;
      }
    }
    /// <summary>
    /// Parses UNC link.
    /// </summary>
    /// <param name="iOffset">Offset to the data.</param>
    private void ParseUnc( ref int iOffset )
    {
      m_uiUncLen = GetUInt32( iOffset );
      iOffset += 4;
      m_strUnc = Encoding.Unicode.GetString( m_data, iOffset, ( int )m_uiUncLen * 2 - 2 );
      iOffset += (int) m_uiUncLen * 2;
    }
    /// <summary>
    /// Parses link to the workbook.
    /// </summary>
    /// <param name="iOffset">Offset to the data.</param>
    private void ParseWorkbook( ref int iOffset )
    {
    }
    #endregion

    #region Class infill helper methods
    /// <summary>
    /// Infill's length and string value.
    /// </summary>
    /// <param name="uiLen">String length.</param>
    /// <param name="strValue">String value.</param>
    /// <param name="bBytesCount">Indicates whether length is one byte.</param>
    private void InfillLenAndString( ref uint uiLen, ref string strValue, bool bBytesCount )
    {
      if( strValue == null || strValue.Length == 0 ) strValue = "\0";

      uiLen = (uint) strValue.Length;
      
      if( strValue[ (int) uiLen - 1 ] != '\0' )
      {
        strValue += '\0';
        uiLen++;
      }

      if( bBytesCount ) uiLen *= 2;

      SetUInt32( m_iLength, uiLen );
      m_iLength += 4;

//      SetStringNoLen( m_iLength, strValue );
      byte[] arrData = Encoding.Unicode.GetBytes( strValue );
      SetBytes( m_iLength, arrData );
      m_iLength += arrData.Length;

    }
    /// <summary>
    /// Infills special data.
    /// </summary>
    private void InfillSpecialData()
    {
      switch( LinkType )
      {
        case ExcelHyperLinkType.File:
          InfillFileSpecialData();
          break;
        
        case ExcelHyperLinkType.Unc:
          InfillUncSpecialData();
          break;
        
        case ExcelHyperLinkType.Url:
          InfillUrlSpecialData();
          break;
        
        case ExcelHyperLinkType.Workbook:
          InfillWorkbookSpecialData();
          break;
      }
    }
    /// <summary>
    /// Infills file special data.
    /// </summary>
    private void InfillFileSpecialData()
    {
      SetBytes( m_iLength, GUID_FILEMONIKER_BYTES );
      m_iLength += GUID_FILEMONIKER_BYTES.Length;

      SetUInt16( m_iLength, m_usDirUpLevel );
      m_iLength += 2;

      if( m_strFileName[ ( int )m_uiFileNameLen - 1 ] != '\0' )
      {
        m_uiFileNameLen++;
        m_strFileName += '\0';
      }

      SetUInt32( m_iLength, m_uiFileNameLen );
      m_iLength += 4;

      byte[] buffer = BiffRecordRaw.LatinEncoding.GetBytes( m_strFileName );
      SetBytes( m_iLength, buffer );
      m_iLength += buffer.Length;

      SetBytes( m_iLength, FILE_UNKNOWN );
      m_iLength += FILE_UNKNOWN.Length;

      if( m_strXFilePath != null && m_strXFilePath.Length != 0 )
      {
        m_uiFollowSize = (uint) ( 4 + 2  + m_strXFilePath.Length * 2 );
      }
      else
      {
        m_uiFollowSize = 0;
      }
      
      SetUInt32( m_iLength, m_uiFollowSize );
      m_iLength += 4;

      if( m_uiFollowSize != 0 )
      {
        m_uiXFilePathLen = (uint) m_strXFilePath.Length * 2;
        SetUInt32( m_iLength, m_uiXFilePathLen );
        m_iLength += 4;

        SetBytes( m_iLength, FILE_UNKNOWN2 );
        m_iLength += FILE_UNKNOWN2.Length;

        //SetStringNoLen( m_iLength, m_strXFilePath );
        byte[] arrBytes = Encoding.Unicode.GetBytes( m_strXFilePath );
        SetBytes( m_iLength, arrBytes );
        m_iLength += arrBytes.Length;
      }
    }
    /// <summary>
    /// Infills UNC special data.
    /// </summary>
    private void InfillUncSpecialData()
    {
      CanBeUnc = true;
      InfillLenAndString( ref m_uiUncLen, ref m_strUnc, false );
    }
    /// <summary>
    /// Infills URL special data.
    /// </summary>
    private void InfillUrlSpecialData()
    {
      CanBeUrl = true;

      SetBytes( m_iLength, GUID_URLMONIKER_BYTES );
      m_iLength += GUID_URLMONIKER_BYTES.Length;

      InfillLenAndString( ref m_uiUrlLen, ref m_strUrl, true );
    }
    /// <summary>
    /// Infills workbook special data.
    /// </summary>
    private void InfillWorkbookSpecialData()
    {
      CanBeWorkbook = true;
    }
    /// <summary>
    /// Sets options according to the link type.
    /// </summary>
    private void SetOptionFlags()
    {
      switch( LinkType )
      {
        case ExcelHyperLinkType.File:
          CanBeFile = true;
          break;
        
        case ExcelHyperLinkType.Unc:
          CanBeUnc = true;
          break;

        case ExcelHyperLinkType.Url:
          CanBeUrl = true;
          break;

        case ExcelHyperLinkType.Workbook:
          CanBeWorkbook = true;
          break;
      }
    }
    #endregion
  }
}