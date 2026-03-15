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

namespace Syncfusion.XlsIO.Parser.Biff_Records.Charts
{
  /// <summary>
  /// This record is used in conjunction with several child records
  /// (which further define the text displayed on the chart) to define
  /// the alignment, color, position, size, and so on of text fields
  /// that appear on the chart. The fields in this record have meaning
  /// according to the TEXT record's parent (CHART, LEGEND, or DEFAULTTEXT).
  /// </summary>
  [ Biff( TBIFFRecord.ChartText ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class ChartTextRecord : BiffRecordRaw
  {
    #region Class constants
    /// <summary>
    /// Mask for the rotation value.
    /// </summary>
    public const int DEF_ROTATION_MASK = 0x0700;
    /// <summary>
    /// First bit of the rotation value.
    /// </summary>
    public const int DEF_FIRST_ROTATION_BIT = 8;
    /// <summary>
    /// Mask for data label placement.
    /// </summary>
    public const int DEF_DATA_LABEL_MASK = 0x000F;
    /// <summary>
    /// First bit of the data label placement value.
    /// </summary>
    public const int DEF_DATA_LABEL_FIRST_BIT = 0;
    /// <summary>
    /// Correct size of the record.
    /// </summary>
    public const int DEF_RECORD_SIZE = 32;
    #endregion

    #region Class members
    /// <summary>
    /// Horizontal alignment of the text.
    /// </summary>
    [ BiffRecordPos( 0, 1 ) ]
    private byte m_HorzAlign = 1;
    /// <summary>
    /// Vertical alignment of the text.
    /// </summary>
    [ BiffRecordPos( 1, 1 ) ]
    private byte m_VertAlign = 1;
    /// <summary>
    /// Display mode of the background.
    /// </summary>
    [ BiffRecordPos( 2, 2 ) ]
    private ushort m_usBkgMode = 1;
    /// <summary>
    /// Text color.
    /// </summary>
    [ BiffRecordPos( 4, 4 ) ]
    private uint m_uiTextColor;
    /// <summary>
    /// X-position of the text in 1/4000 of chart area.
    /// </summary>
    [ BiffRecordPos( 8, 4 ) ]
    private uint m_uiXPos;
    /// <summary>
    /// Y-position of the text in 1/4000 of chart area.
    /// </summary>
    [ BiffRecordPos( 12, 4 ) ]
    private uint m_uiYPos;
    /// <summary>
    /// X-size of the text in 1/4000 of chart area.
    /// </summary>
    [ BiffRecordPos( 16, 4 ) ]
    private uint m_uiXSize;
    /// <summary>
    /// Y-size of the text in 1/4000 of chart area.
    /// </summary>
    [ BiffRecordPos( 20, 4 ) ]
    private uint m_uiYSize;
    /// <summary>
    /// Option flags.
    /// </summary>
    [ BiffRecordPos( 24, 2 ) ]
    private ushort m_usOptions;
    /// <summary>
    /// Index to color value of text.
    /// </summary>
    [ BiffRecordPos( 26, 2 ) ]
    private ushort m_usColorIndex;
    /// <summary>
    /// Option flags.
    /// </summary>
    [ BiffRecordPos( 28, 2 ) ]
    private ushort m_usOptions2;
    /// <summary>
    /// True for automatic color; False for user-selected color.
    /// </summary>
    [ BiffRecordPos( 24, 0, TFieldType.Bit ) ]
    private bool m_bAutoColor = true;
    /// <summary>
    /// If text is an attached data label: 
    /// True to draw legend key with data label;
    /// False for no legend key.
    /// </summary>
    [ BiffRecordPos( 24, 1, TFieldType.Bit ) ]
    private bool m_bShowKey;
    /// <summary>
    /// True if text of label is the value of the data point;
    /// False if text is the category label.
    /// </summary>
    [ BiffRecordPos( 24, 2, TFieldType.Bit ) ]
    private bool m_bShowValue;
    /// <summary>
    /// True if text is not horizontal;
    /// False if text is horizontal.
    /// </summary>
    [ BiffRecordPos( 24, 3, TFieldType.Bit ) ]
    private bool m_bVertical;
    /// <summary>
    /// True to use automatically generated text string;
    /// False to use user-created text string.
    /// Must be one for fShowValue to be meaningful.
    /// </summary>
    [ BiffRecordPos( 24, 4, TFieldType.Bit ) ]
    private bool m_bAutoText = true;
    /// <summary>
    /// True if default or unmodified;
    /// False if modified.
    /// </summary>
    [ BiffRecordPos( 24, 5, TFieldType.Bit ) ]
    private bool m_bGenerated = true;
    /// <summary>
    /// True if an automatic text label has been deleted by the user.
    /// </summary>
    [ BiffRecordPos( 24, 6, TFieldType.Bit ) ]
    private bool m_bDeleted;
    /// <summary>
    /// True if background is set to automatic.
    /// </summary>
    [ BiffRecordPos( 24, 7, TFieldType.Bit ) ]
    private bool m_bAutoMode = true;
    /// <summary>
    /// True to show category label and value as a percentage.
    /// (pie charts only)
    /// </summary>
    [ BiffRecordPos( 25, 3, TFieldType.Bit ) ]
    private bool m_bShowLabelPercent;
    /// <summary>
    /// True to show value as a percent.
    /// This bit applies only to pie charts.
    /// </summary>
    [ BiffRecordPos( 25, 4, TFieldType.Bit ) ]
    private bool m_bShowPercent;
    /// <summary>
    /// True to show bubble sizes.
    /// </summary>
    [ BiffRecordPos( 25, 5, TFieldType.Bit ) ]
    private bool m_bShowBubbleSizes;
    /// <summary>
    /// True to show label.
    /// </summary>
    [ BiffRecordPos( 25, 6, TFieldType.Bit ) ]
    private bool m_bShowLabel;
    /// <summary>
    /// Text rotation.
    /// </summary>
    [ BiffRecordPos( 30, 2, true ) ]
    private short? m_sRotation;
    #endregion

    #region Class properties
    /// <summary>
    /// Horizontal alignment of the text.
    /// </summary>
    public ExcelChartHorzAlignment HorzAlign
    {
      get
      {
        return (ExcelChartHorzAlignment) m_HorzAlign;
      }
      set
      {
        m_HorzAlign = (byte) value;
      }
    }
    /// <summary>
    /// Vertical alignment of the text.
    /// </summary>
    public ExcelChartVertAlignment VertAlign
    {
      get
      {
        return (ExcelChartVertAlignment) m_VertAlign;
      }
      set
      {
        m_VertAlign = (byte) value;
      }
    }
    /// <summary>
    /// Display mode of the background.
    /// </summary>
    public ExcelChartBackgroundMode BackgroundMode
    {
      get
      {
        return (ExcelChartBackgroundMode) m_usBkgMode;
      }
      set
      {
        m_usBkgMode = (ushort) value;
      }
    }
    /// <summary>
    /// Text color.
    /// </summary>
    public uint TextColor
    {
      get
      {
        return m_uiTextColor;
      }
      set
      {
        m_uiTextColor = value;
      }
    }
    /// <summary>
    /// X-position of the text in 1/4000 of chart area.
    /// </summary>
    public uint XPos
    {
      get
      {
        return m_uiXPos;
      }
      set
      {
        m_uiXPos = value;
      }
    }
    /// <summary>
    /// Y-position of the text in 1/4000 of chart area.
    /// </summary>
    public uint YPos
    {
      get
      {
        return m_uiYPos;
      }
      set
      {
        m_uiYPos = value;
      }
    }
    /// <summary>
    /// X-size of the text in 1/4000 of chart area.
    /// </summary>
    public uint XSize
    {
      get
      {
        return m_uiXSize;
      }
      set
      {
        m_uiXSize = value;
      }
    }
    /// <summary>
    /// Y-size of the text in 1/4000 of chart area.
    /// </summary>
    public uint YSize
    {
      get
      {
        return m_uiYSize;
      }
      set
      {
        m_uiYSize = value;
      }
    }
    /// <summary>
    /// Option flags. Read-only.
    /// </summary>
    public ushort Options
    {
      get
      {
        return m_usOptions;
      }
    }
    /// <summary>
    /// Index to color value of text.
    /// </summary>
    public ExcelKnownColors ColorIndex
    {
      get
      {
        return ( ExcelKnownColors )m_usColorIndex;
      }
      set
      {
        m_usColorIndex = ( ushort )value;
      }
    }
    /// <summary>
    /// Option flags. Read-only.
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
    /// True if automatic color; False if user-selected color.
    /// </summary>
    public bool IsAutoColor
    {
      get
      {
        return m_bAutoColor;
      }
      set
      {
        m_bAutoColor = value;
      }
    }
    /// <summary>
    /// If text is an attached data label: 
    /// True to draw legend key with data label;
    /// False if no legend key.
    /// </summary>
    public bool IsShowKey
    {
      get
      {
        return m_bShowKey;
      }
      set
      {
        m_bShowKey = value;
      }
    }
    /// <summary>
    /// True if text of label is the value of the data point;
    /// False if text is the category label.
    /// </summary>
    public bool IsShowValue
    {
      get
      {
        return m_bShowValue;
      }
      set
      {
        m_bShowValue = value;
      }
    }
    /// <summary>
    /// True if text is not horizontal;
    /// False if text is horizontal.
    /// </summary>
    public bool IsVertical
    {
      get
      {
        return m_bVertical;
      }
      set
      {
        m_bVertical = value;
      }
    }
    /// <summary>
    /// True to use automatically generated text string;
    /// False to use user-created text string.
    /// Must be one for fShowValue to be meaningful.
    /// </summary>
    public bool IsAutoText
    {
      get
      {
        return m_bAutoText;
      }
      set
      {
        m_bAutoText = value;
      }
    }
    /// <summary>
    /// True if default or unmodified;
    /// False if modified.
    /// </summary>
    public bool IsGenerated
    {
      get
      {
        return m_bGenerated;
      }
      set
      {
        m_bGenerated = value;
      }
    }
    /// <summary>
    /// True if an automatic text label has been deleted by the user.
    /// </summary>
    public bool IsDeleted
    {
      get
      {
        return m_bDeleted;
      }
      set
      {
        m_bDeleted = value;
      }
    }
    /// <summary>
    /// True if background is set to automatic.
    /// </summary>
    public bool IsAutoMode
    {
      get
      {
        return m_bAutoMode;
      }
      set
      {
        m_bAutoMode = value;
      }
    }
    /// <summary>
    /// True to show category label and value as a percentage
    /// (pie charts only).
    /// </summary>
    public bool IsShowLabelPercent
    {
      get
      {
        return m_bShowLabelPercent;
      }
      set
      {
        m_bShowLabelPercent = value;
      }
    }
    /// <summary>
    /// True to show value as a percent.
    /// This bit applies only to pie charts.
    /// </summary>
    public bool IsShowPercent
    {
      get
      {
        return m_bShowPercent;
      }
      set
      {
        m_bShowPercent = value;
      }
    }
    /// <summary>
    /// True to show bubble sizes.
    /// </summary>
    public bool IsShowBubbleSizes
    {
      get
      {
        return m_bShowBubbleSizes;
      }
      set
      {
        m_bShowBubbleSizes = value;
      }
    }
    /// <summary>
    /// True to show label.
    /// </summary>
    public bool IsShowLabel
    {
      get
      {
        return m_bShowLabel;
      }
      set
      {
        m_bShowLabel = value;
      }
    }
    /// <summary>
    /// Rotation.
    /// </summary>
    public TRotation Rotation
    {
      get
      {
        return (TRotation) ( GetUInt16BitsByMask( m_usOptions, DEF_ROTATION_MASK )
          >> DEF_FIRST_ROTATION_BIT );
      }
      set
      {
        SetUInt16BitsByMask( ref m_usOptions, DEF_ROTATION_MASK, 
          ( ushort ) ( (ushort) value << DEF_FIRST_ROTATION_BIT ) );
      }
    }
    /// <summary>
    /// Data label placement.
    /// </summary>
    public ExcelDataLabelPosition DataLabelPlacement
    {
      get
      {
        return ( ExcelDataLabelPosition )( GetUInt16BitsByMask( m_usOptions2,
          DEF_DATA_LABEL_MASK ) >> DEF_DATA_LABEL_FIRST_BIT );
      }
      set
      {
        SetUInt16BitsByMask( ref m_usOptions2, DEF_DATA_LABEL_MASK, 
          ( ushort ) ( (ushort) value << DEF_DATA_LABEL_FIRST_BIT ) );
      }
    }

    /// <summary>
    /// Text rotation.
    /// </summary>
    public short TextRotation
    {
      get
      {
        return m_sRotation ?? 0;
      }
      set
      {
        m_sRotation = value;
      }
    }
    /// <summary>
    /// Text rotation.
    /// </summary>
    public short? TextRotationOrNull
    {
      get
      {
        return m_sRotation;
      }
      set
      {
        m_sRotation = value;
      }
    }

    /// <summary>
    /// Minimum possible size of the record.
    /// </summary>
    public override int MinimumRecordSize
    {
      get
      {
        return DEF_RECORD_SIZE;
      }
    }

    /// <summary>
    /// Maximum possible size of the record.
    /// </summary>
    public override int MaximumRecordSize
    {
      get
      {
        return DEF_RECORD_SIZE;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor, initializes all fields with default values.
    /// </summary>
    public  ChartTextRecord()
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
    public  ChartTextRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }
    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  ChartTextRecord( int iReserve )
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
      m_HorzAlign = provider.ReadByte( iOffset );
      iOffset++;
      m_VertAlign = provider.ReadByte( iOffset );
      iOffset++;
      m_usBkgMode = provider.ReadUInt16( iOffset );
      iOffset += 2;
      m_uiTextColor = provider.ReadUInt32( iOffset );
      iOffset += 4;
      m_uiXPos = provider.ReadUInt32( iOffset );
      iOffset += 4;
      m_uiYPos = provider.ReadUInt32( iOffset );
      iOffset += 4;
      m_uiXSize = provider.ReadUInt32( iOffset );
      iOffset += 4;
      m_uiYSize = provider.ReadUInt32( iOffset );
      iOffset += 4;
      m_usOptions = provider.ReadUInt16( iOffset );
      m_bAutoColor = provider.ReadBit( iOffset, 0 );
      m_bShowKey = provider.ReadBit( iOffset, 1 );
      m_bShowValue = provider.ReadBit( iOffset, 2 );
      m_bVertical = provider.ReadBit( iOffset, 3 );
      m_bAutoText = provider.ReadBit( iOffset, 4 );
      m_bGenerated = provider.ReadBit( iOffset, 5 );
      m_bDeleted = provider.ReadBit( iOffset, 6 );
      m_bAutoMode = provider.ReadBit( iOffset, 7 );
      iOffset++;
      m_bShowLabelPercent = provider.ReadBit( iOffset, 3 );
      m_bShowPercent = provider.ReadBit( iOffset, 4 );
      m_bShowBubbleSizes = provider.ReadBit( iOffset, 5 );
      m_bShowLabel = provider.ReadBit( iOffset, 6 );
      iOffset++;
      m_usColorIndex = provider.ReadUInt16( iOffset );
      iOffset += 2;
      m_usOptions2 = provider.ReadUInt16( iOffset );
      iOffset += 2;
      m_sRotation = provider.ReadInt16( iOffset );
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
      provider.WriteByte( iOffset + 0, m_HorzAlign );
      provider.WriteByte( iOffset + 1, m_VertAlign );
      provider.WriteUInt16( iOffset + 2, m_usBkgMode );
      provider.WriteUInt32( iOffset + 4, m_uiTextColor );
      provider.WriteUInt32( iOffset + 8, m_uiXPos );
      provider.WriteUInt32( iOffset + 12, m_uiYPos );
      provider.WriteUInt32( iOffset + 16, m_uiXSize );
      provider.WriteUInt32( iOffset + 20, m_uiYSize );
      provider.WriteUInt16( iOffset + 24, m_usOptions );
      provider.WriteBit( iOffset + 24, m_bAutoColor, 0 );
      provider.WriteBit( iOffset + 24, m_bShowKey, 1 );
      provider.WriteBit( iOffset + 24, m_bShowValue, 2 );
      provider.WriteBit( iOffset + 24, m_bVertical, 3 );
      provider.WriteBit( iOffset + 24, m_bAutoText, 4 );
      provider.WriteBit( iOffset + 24, m_bGenerated, 5 );
      provider.WriteBit( iOffset + 24, m_bDeleted, 6 );
      provider.WriteBit( iOffset + 24, m_bAutoMode, 7 );
      provider.WriteBit( iOffset + 25, m_bShowLabelPercent, 3 );
      provider.WriteBit( iOffset + 25, m_bShowPercent, 4 );
      provider.WriteBit( iOffset + 25, m_bShowBubbleSizes, 5 );
      provider.WriteBit( iOffset + 25, m_bShowLabel, 6 );
      provider.WriteUInt16( iOffset + 26, m_usColorIndex );
      provider.WriteUInt16( iOffset + 28, m_usOptions2 );
      provider.WriteInt16( iOffset + 30, TextRotation );

      m_iLength = DEF_RECORD_SIZE;
    }
    #endregion
  }
}
