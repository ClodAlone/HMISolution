#region Copyright

//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.

#endregion Copyright

using System;
using System.IO;

namespace Syncfusion.XlsIO.Parser.Biff_Records.PivotTable
{
  /// <summary>
  /// This record follows the SXVIEW record and contains information about
  /// additional features added to PivotTables in Excel.
  /// </summary>
  [ Biff( TBIFFRecord.ViewExtendedInfo ) ]
  [ CLSCompliant( false ) ]
  public class ViewExtendedInfoRecord : BiffRecordRawWithArray
  {
    #region Class constants
    /// <summary>
    /// Bit mask for WrapPage property value.
    /// </summary>
    private const ushort DEF_WRAP_PAGE_MASK = 0x1E;
    /// <summary>
    /// Start bit for WrapPage property value.
    /// </summary>
    private const int DEF_WRAP_PAGE_START_BIT = 1;
    /// <summary>
    /// Maximum value for WrapPage property.
    /// </summary>
    public const int DEF_WRAPPAGE_MAXVALUE = 15;
    /// <summary>
    /// Offset to the strings data.
    /// </summary>
    private const int FirstStringOffset = 24;
    #endregion

    #region Class members
    /// <summary>
    /// Number of SXFORMAT records to follow.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usFormat;
    /// <summary>
    /// Number of characters for DisplayErrorString string.
    /// </summary>
    [BiffRecordPos( 2, 2, true )]
    private short m_sErrorStringLength;
    /// <summary>
    /// Number of characters for DisplayNullString string.
    /// </summary>
    [BiffRecordPos( 4, 2, true )]
    private short m_sNullStringLength;
    /// <summary>
    /// Number of characters in Tag string.
    /// </summary>
    [BiffRecordPos( 6, 2, true )]
    private short m_sTagLength;
    /// <summary>
    /// Number of RTSXSELECT records to follow.
    /// </summary>
    [ BiffRecordPos( 8, 2 ) ]
    private ushort m_usSelectNumber;
    /// <summary>
    /// Number of page field per row.
    /// </summary>
    [ BiffRecordPos( 10, 2 ) ]
    private ushort m_usFieldPerRow;
    /// <summary>
    /// Number of page field per column.
    /// </summary>
    [ BiffRecordPos( 12, 2 ) ]
    private ushort m_usFieldPerColumn;

    /// <summary>
    /// Option flags.
    /// </summary>
    [ BiffRecordPos( 14, 2 ) ]
    private ushort m_usOptions1;
    /// <summary>
    /// 
    /// </summary>
    [ BiffRecordPos( 14, 0, TFieldType.Bit ) ]
    private bool m_bAcrossPageLay;
    /// <summary>
    /// 
    /// </summary>
    [ BiffRecordPos( 14, 5, TFieldType.Bit ) ]
    private bool m_bPreserveFormattingNow;
    /// <summary>
    /// 
    /// </summary>
    [ BiffRecordPos( 14, 6, TFieldType.Bit ) ]
    private bool m_bManualUpdate;

    /// <summary>
    /// Option flags.
    /// </summary>
    [ BiffRecordPos( 16, 2 ) ]
    private ushort m_usOptions2;
    /// <summary>
    /// 
    /// </summary>
    [ BiffRecordPos( 16, 0, TFieldType.Bit ) ]
    private bool m_bEnableWizard;
    /// <summary>
    /// 
    /// </summary>
    [ BiffRecordPos( 16, 1, TFieldType.Bit ) ]
    private bool m_bEnableDrilldown;
    /// <summary>
    /// 
    /// </summary>
    [ BiffRecordPos( 16, 2, TFieldType.Bit ) ]
    private bool m_bEnableFieldDialog;
    /// <summary>
    /// 
    /// </summary>
    [ BiffRecordPos( 16, 3, TFieldType.Bit ) ]
    private bool m_bPreserveFormatting;
    /// <summary>
    /// 
    /// </summary>
    [ BiffRecordPos( 16, 4, TFieldType.Bit ) ]
    private bool m_bMergeLabels;
    /// <summary>
    /// 
    /// </summary>
    [ BiffRecordPos( 16, 5, TFieldType.Bit ) ]
    private bool m_bDisplayErrorString;
    /// <summary>
    /// 
    /// </summary>
    [ BiffRecordPos( 16, 6, TFieldType.Bit ) ]
    private bool m_bDisplayNullString;
    /// <summary>
    /// 
    /// </summary>
    [ BiffRecordPos( 16, 7, TFieldType.Bit ) ]
    private bool m_bSubtotalHiddenPageItems;


    /// <summary>
    /// Number of characters for page field style string.
    /// </summary>
    [ BiffRecordPos( 18, 2, true ) ]
    private short m_sPageFieldStyleLength;
    /// <summary>
    /// Number of characters for table style string.
    /// </summary>
    [BiffRecordPos( 20, 2, true )]
    private short m_sTableStyleLength;
    /// <summary>
    /// Number of characters for vacate style string.
    /// </summary>
    [BiffRecordPos( 22, 2, true )]
    private short m_sVacateStyleLength;

    /// <summary>
    /// ErrorString.
    /// </summary>
    private string m_strErrorString;
    /// <summary>
    /// NullString.
    /// </summary>
    private string m_strNullString;
    /// <summary>
    /// Tag.
    /// </summary>
    private string m_strTag;
    /// <summary>
    /// Page field style.
    /// </summary>
    private string m_strPageFieldStyle;
    /// <summary>
    /// Table style.
    /// </summary>
    private string m_strTableStyle;
    /// <summary>
    /// Vacate style.
    /// </summary>
    private string m_strVacateStyle;
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor.
    /// </summary>
    public  ViewExtendedInfoRecord()
      : base()
    {
    }

    /// <summary>
    /// Read / initialize constructor.
    /// </summary>
    /// <param name="stream">Stream from which record data should be read.</param>
    /// <param name="itemSize">Size of read item.</param>
    /// <exception cref="System.ArgumentNullException">When stream is not specified.</exception>
    /// <exception cref="System.ApplicationException">When stream does not support read or seek operations.</exception>
    public  ViewExtendedInfoRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }

    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for the data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  ViewExtendedInfoRecord( int iReserve )
      : base( iReserve )
    {
    }

    #endregion

    #region Class properties
    /// <summary>
    /// Number of SXFORMAT records to follow.
    /// </summary>
    public ushort Format
    {
      get
      {
        return m_usFormat;
      }
      set
      {
        m_usFormat = value;
      }
    }
    /// <summary>
    /// Number of characters for DisplayErrorString string. Read-only.
    /// </summary>
    public short ErrorStringLength
    {
      get
      {
        return m_sErrorStringLength;
      }
    }
    /// <summary>
    /// Number of characters for DisplayNullString string. Read-only.
    /// </summary>
    public short NullStringLength
    {
      get
      {
        return m_sNullStringLength;
      }
    }
    /// <summary>
    /// Number of characters in Tag string. Read-only.
    /// </summary>
    public short TagLength
    {
      get
      {
        return m_sTagLength;
      }
    }
    /// <summary>
    /// Number of RTSXSELECT records to follow.
    /// </summary>
    public ushort SelectNumber
    {
      get
      {
        return m_usSelectNumber;
      }
      set
      {
        m_usSelectNumber = value;
      }
    }
    /// <summary>
    /// Number of page field per row.
    /// </summary>
    public ushort FieldPerRow
    {
      get
      {
        return m_usFieldPerRow;
      }
      set
      {
        m_usFieldPerRow = value;
      }
    }
    /// <summary>
    /// Number of page field per column.
    /// </summary>
    public ushort FieldPerColumn
    {
      get
      {
        return m_usFieldPerColumn;
      }
      set
      {
        m_usFieldPerColumn = value;
      }
    }
    /// <summary>
    /// Option flags. Read-only.
    /// </summary>
    public ushort Options1
    {
      get
      {
        return m_usOptions1;
      }
#if DEBUG
      set
      {
        m_usOptions1 = value;
      }
#endif
    }
    /// <summary>
    /// 
    /// </summary>
    public bool IsAcrossPageLay
    {
      get
      {
        return m_bAcrossPageLay;
      }
      set
      {
        m_bAcrossPageLay = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public bool IsPreserveFormattingNow
    {
      get
      {
        return m_bPreserveFormattingNow;
      }
      set
      {
        m_bPreserveFormattingNow = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public bool IsManualUpdate
    {
      get
      {
        return m_bManualUpdate;
      }
      set
      {
        m_bManualUpdate = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public ushort WrapPage
    {
      get
      {
        return ( ushort )( GetUInt16BitsByMask( m_usOptions1, DEF_WRAP_PAGE_MASK )
          >> DEF_WRAP_PAGE_START_BIT );
      }
      set
      {
        SetUInt16BitsByMask( ref m_usOptions1, DEF_WRAP_PAGE_MASK,
          ( ushort )( value << DEF_WRAP_PAGE_START_BIT ) );
      }
    }
    /// <summary>
    /// Option flags.
    /// </summary>
    public ushort Options2
    {
      get
      {
        return m_usOptions2;
      }
      set
      {
        m_usOptions2 = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public bool IsEnableWizard
    {
      get
      {
        return m_bEnableWizard;
      }
      set
      {
        m_bEnableWizard = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public bool IsEnableDrilldown
    {
      get
      {
        return m_bEnableDrilldown;
      }
      set
      {
        m_bEnableDrilldown = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public bool IsEnableFieldDialog
    {
      get
      {
        return m_bEnableFieldDialog;
      }
      set
      {
        m_bEnableFieldDialog = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public bool IsPreserveFormatting
    {
      get
      {
        return m_bPreserveFormatting;
      }
      set
      {
        m_bPreserveFormatting = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public bool IsMergeLabels
    {
      get
      {
        return m_bMergeLabels;
      }
      set
      {
        m_bMergeLabels = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public bool IsDisplayErrorString
    {
      get
      {
        return m_bDisplayErrorString;
      }
      set
      {
        m_bDisplayErrorString = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public bool IsDisplayNullString
    {
      get
      {
        return m_bDisplayNullString;
      }
      set
      {
        m_bDisplayNullString = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public bool IsSubtotalHiddenPageItems
    {
      get
      {
        return m_bSubtotalHiddenPageItems;
      }
      set
      {
        m_bSubtotalHiddenPageItems = value;
      }
    }
    /// <summary>
    /// Number of characters for page field style string. Read-only.
    /// </summary>
    public short PageFieldStyleLength
    {
      get
      {
        return m_sPageFieldStyleLength;
      }
    }
    /// <summary>
    /// Number of characters for table style string. Read-only.
    /// </summary>
    public short TableStyleLength
    {
      get
      {
        return m_sTableStyleLength;
      }
    }
    /// <summary>
    /// Number of characters for vacate style string. Read-only.
    /// </summary>
    public short VacateStyleLength
    {
      get
      {
        return m_sVacateStyleLength;
      }
    }
    /// <summary>
    /// ErrorString.
    /// </summary>
    public string ErrorString
    {
      get
      {
        return m_strErrorString;
      }
      set
      {
        if( value == null )
          throw new ArgumentNullException( "value" );

        m_strErrorString = value;
        m_sErrorStringLength = ( short )value.Length;
      }
    }
    /// <summary>
    /// NullString.
    /// </summary>
    public string NullString
    {
      get
      {
        return m_strNullString;
      }
      set
      {
        if( value == null )
          throw new ArgumentNullException( "value" );

        m_strNullString = value;
        m_sNullStringLength = ( short )value.Length;
      }
    }
    /// <summary>
    /// Tag.
    /// </summary>
    public string Tag
    {
      get
      {
        return m_strTag;
      }
      set
      {
        if( value == null )
          throw new ArgumentNullException( "value" );

        m_strTag = value;
        m_sTagLength = ( short )value.Length;
      }
    }
    /// <summary>
    /// Page field style.
    /// </summary>
    public string PageFieldStyle
    {
      get
      {
        return m_strPageFieldStyle;
      }
      set
      {
        if( value == null )
          throw new ArgumentNullException( "value" );

        m_strPageFieldStyle = value;
        m_sPageFieldStyleLength = ( short )value.Length;
      }
    }
    /// <summary>
    /// Table style.
    /// </summary>
    public string TableStyle
    {
      get
      {
        return m_strTableStyle;
      }
      set
      {
        if( value == null )
          throw new ArgumentNullException( "value" );

        m_strTableStyle = value;
        m_sTableStyleLength = ( short )value.Length;
      }
    }
    /// <summary>
    /// Vacate style.
    /// </summary>
    public string VacateStyle
    {
      get
      {
        return m_strVacateStyle;
      }
      set
      {
        if( value == null )
          throw new ArgumentNullException( "value" );

        m_strVacateStyle = value;
        m_sVacateStyleLength = ( short )value.Length;
      }
    }
    #endregion

    #region Record Serialization
    /// <summary>
    /// Parse structure of record. Convert Data buffer to special
    /// values according to record specification.
    /// </summary>
    public override void ParseStructure()
    {
      m_usFormat = GetUInt16( 0 );
      m_sErrorStringLength = GetInt16( 2 );
      m_sNullStringLength = GetInt16( 4 );
      m_sTagLength = GetInt16( 6 );
      m_usSelectNumber = GetUInt16( 8 );
      m_usFieldPerRow = GetUInt16( 10 );
      m_usFieldPerColumn = GetUInt16( 12 );
      m_usOptions1 = GetUInt16( 14 );
      m_bAcrossPageLay = GetBit( 14, 0 );
      m_bPreserveFormattingNow = GetBit( 14, 5 );
      m_bManualUpdate = GetBit( 14, 6 );
      m_usOptions2 = GetUInt16( 16 );
      m_bEnableWizard = GetBit( 16, 0 );
      m_bEnableDrilldown = GetBit( 16, 1 );
      m_bEnableFieldDialog = GetBit( 16, 2 );
      m_bPreserveFormatting = GetBit( 16, 3 );
      m_bMergeLabels = GetBit( 16, 4 );
      m_bDisplayErrorString = GetBit( 16, 5 );
      m_bDisplayNullString = GetBit( 16, 6 );
      m_bSubtotalHiddenPageItems = GetBit( 16, 7 );
      m_sPageFieldStyleLength = GetInt16( 18 );
      m_sTableStyleLength = GetInt16( 20 );
      m_sVacateStyleLength = GetInt16( 22 );

      // TODO: add parsing of strings.
      if( m_iLength > FirstStringOffset )
      {
        int iOffset = FirstStringOffset;

        if( m_sErrorStringLength > 0 )
          m_strErrorString = GetStringUpdateOffset( ref iOffset, m_sErrorStringLength );

        if( m_sNullStringLength > 0 )
          m_strNullString = GetStringUpdateOffset( ref iOffset, m_sNullStringLength );

        if( m_sTagLength > 0 )
          m_strTag = GetStringUpdateOffset( ref iOffset, m_sTagLength );

        if( m_sPageFieldStyleLength > 0 )
          m_strPageFieldStyle = GetStringUpdateOffset( ref iOffset, m_sPageFieldStyleLength );

        if( m_sTableStyleLength > 0 )
          m_strTableStyle = GetStringUpdateOffset( ref iOffset, m_sTableStyleLength );

        if( m_sVacateStyleLength > 0 )
          m_strVacateStyle = GetStringUpdateOffset( ref iOffset, m_sVacateStyleLength );
      }
    }
    /// <summary>
    /// In this method, the class must pack all of its properties into an
    /// internal data array, m_data. This method is called by
    /// FillStream when the record must be serialized into a stream.
    /// </summary>
    public override void InfillInternalData( ExcelVersion version )
    {
      SetUInt16( 0, m_usFormat );
      SetInt16( 2, m_sErrorStringLength );
      SetInt16( 4, m_sNullStringLength );
      SetInt16( 6, m_sTagLength );
      SetUInt16( 8, m_usSelectNumber );
      SetUInt16( 10, m_usFieldPerRow );
      SetUInt16( 12, m_usFieldPerColumn );
      SetUInt16( 14, m_usOptions1 );
      SetBit( 14, m_bAcrossPageLay, 0 );
      SetBit( 14, m_bPreserveFormattingNow, 5 );
      SetBit( 14, m_bManualUpdate, 6 );
      SetUInt16( 16, m_usOptions2 );
      SetBit( 16, m_bEnableWizard, 0 );
      SetBit( 16, m_bEnableDrilldown, 1 );
      SetBit( 16, m_bEnableFieldDialog, 2 );
      SetBit( 16, m_bPreserveFormatting, 3 );
      SetBit( 16, m_bMergeLabels, 4 );
      SetBit( 16, m_bDisplayErrorString, 5 );
      SetBit( 16, m_bDisplayNullString, 6 );
      SetBit( 16, m_bSubtotalHiddenPageItems, 7 );
      SetInt16( 18, m_sPageFieldStyleLength );
      SetInt16( 20, m_sTableStyleLength );
      SetInt16( 22, m_sVacateStyleLength );

      m_iLength = 24;
      AutoGrowData = true;

      if( m_sErrorStringLength > 0 )
        m_iLength += SetStringNoLen( m_iLength, m_strErrorString );

      if( m_sNullStringLength > 0 )
        m_iLength += SetStringNoLen( m_iLength, m_strNullString );

      if( m_sTagLength > 0 )
        m_iLength += SetStringNoLen( m_iLength, m_strTag );

      if( m_sPageFieldStyleLength > 0 )
        m_iLength += SetStringNoLen( m_iLength, m_strPageFieldStyle );

      if( m_sTableStyleLength > 0 )
        m_iLength += SetStringNoLen( m_iLength, m_strTableStyle );

      if( m_sVacateStyleLength > 0 )
        m_iLength += SetStringNoLen( m_iLength, m_strVacateStyle );
    }
    #endregion
  }
}
