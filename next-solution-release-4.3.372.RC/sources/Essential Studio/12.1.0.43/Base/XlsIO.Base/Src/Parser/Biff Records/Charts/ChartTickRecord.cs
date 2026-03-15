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
  /// This record defines tick mark and tick label formatting.
  /// </summary>
  [ Biff( TBIFFRecord.ChartTick ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class ChartTickRecord : BiffRecordRaw
  {
    #region Class constants
    /// <summary>
    /// Mask for the rotation value.
    /// </summary>
    public const ushort DEF_ROTATION_MASK = 0x1C;
    /// <summary>
    /// First bit of the rotation value.
    /// </summary>
    public const ushort DEF_FIRST_ROTATION_BIT = 2;
    /// <summary>
    /// Correct size of the record.
    /// </summary>
    public const int    DEF_RECORD_SIZE = 30;
    /// <summary>
    /// Maximum possible angle.
    /// </summary>
    private const int DEF_MAX_ANGLE = 90;
    /// <summary>
    /// Size of the first reserved field.
    /// </summary>
    private const int ReservedFieldSize = 16;
    #endregion

    #region Class members
    /// <summary>
    /// Type of major tick mark.
    /// </summary>
    [ BiffRecordPos( 0, 1 ) ]
    private byte m_MajorMark = ( byte )ExcelTickMark.TickMark_Outside;
    /// <summary>
    /// Type of minor tick mark.
    /// </summary>
    [ BiffRecordPos( 1, 1 ) ]
    private byte m_MinorMark;
    /// <summary>
    /// Tick label position relative to axis line.
    /// </summary>
    [ BiffRecordPos( 2, 1 ) ]
    private byte m_labelPos = ( byte )ExcelTickLabelPosition.TickLabelPosition_NextToAxis;
    /// <summary>
    /// Background mode.
    /// </summary>
    [ BiffRecordPos( 3, 1 ) ]
    private byte m_BackgroundMode = 1;
    /// <summary>
    /// Tick-label text color; RGB value, high byte = 0.
    /// </summary>
    [ BiffRecordPos( 4, 4 ) ]
    private uint m_uiTextColor;
    /// <summary>
    /// Automatic text color.
    /// </summary>
    [ BiffRecordPos( 24, 0, TFieldType.Bit ) ]
    private bool m_bAutoTextColor = true;
    /// <summary>
    /// Display flags.
    /// </summary>
    [ BiffRecordPos( 24, 2 ) ]
    private ushort m_usFlags;
    /// <summary>
    /// Automatic text background.
    /// </summary>
    [ BiffRecordPos( 24, 1, TFieldType.Bit ) ]
    private bool m_bAutoTextBack = true;
    /// <summary>
    /// Automatic rotation.
    /// </summary>
    [ BiffRecordPos( 24, 5, TFieldType.Bit ) ]
    private bool m_bAutoRotation = true;
    /// <summary>
    /// Index to color of tick label.
    /// </summary>
    [ BiffRecordPos( 26, 2 ) ]
    private ushort m_usTickColorIndex;
    /// <summary>
    /// Text rotation angle.
    /// </summary>
    [ BiffRecordPos( 28, 2, true ) ]
    private short m_sRotationAngle = 0;
    /// <summary>
    /// Indicates if axis label is left to right.
    /// </summary>
    [ BiffRecordPos( 25, 6, TFieldType.Bit ) ]
    private bool m_bIsLeftToRight = false;
    /// <summary>
    /// Indicates if axis label is right to left.
    /// </summary>
    [ BiffRecordPos( 25, 7, TFieldType.Bit ) ]
    private bool m_bIsRightToLeft = false;
    #endregion

    #region Class properties
    /// <summary>
    /// Type of major tick mark.
    /// </summary>
    public ExcelTickMark MajorMark
    {
      get
      {
        return ( ExcelTickMark )m_MajorMark;
      }
      set
      {
        m_MajorMark = ( byte )value;
      }
    }
    /// <summary>
    /// Type of minor tick mark.
    /// </summary>
    public ExcelTickMark MinorMark
    {
      get
      {
        return ( ExcelTickMark )m_MinorMark;
      }
      set
      {
        m_MinorMark = ( byte )value;
      }
    }

    /// <summary>
    /// Tick label position relative to axis line.
    /// </summary>
    public ExcelTickLabelPosition LabelPos
    {
      get
      {
        return ( ExcelTickLabelPosition )m_labelPos;
      }
      set
      {
        m_labelPos = ( byte )value;
      }
    }
    /// <summary>
    /// Background mode.
    /// </summary>
    public ExcelChartBackgroundMode BackgroundMode
    {
      get
      {
        return (ExcelChartBackgroundMode) m_BackgroundMode;
      }
      set
      {
        m_BackgroundMode = (byte) value;
      }
    }
    /// <summary>
    /// Tick-label text color; RGB value, high byte = 0.
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
    /// Display flags. Read-only.
    /// </summary>
    public ushort Flags
    {
      get
      {
        return m_usFlags;
      }
    }
    /// <summary>
    /// Index to color of tick label.
    /// </summary>
    public ushort TickColorIndex
    {
      get
      {
        return m_usTickColorIndex;
      }
      set
      {
        m_usTickColorIndex = value;
      }
    }
    /// <summary>
    /// Reserved; must be zero.
    /// </summary>
    public short RotationAngle
    {
      get
      {
        if( m_sRotationAngle > DEF_MAX_ANGLE )
        {
          return ( short ) ( DEF_MAX_ANGLE - m_sRotationAngle );
        }
        else
        {
          return  m_sRotationAngle;
        }
      }
      set
      {
        if( value < -90 || value > DEF_MAX_ANGLE )
          throw new ArgumentOutOfRangeException( "value", "Value cannot be less -90 and greater than 90" );

        if( value >= 0 )
        {
          m_sRotationAngle = value;
        }
        else
        {
          m_sRotationAngle = ( short )( DEF_MAX_ANGLE - value );
        }
      }
    }

    /// <summary>
    /// Automatic text color.
    /// </summary>
    public bool IsAutoTextColor
    {
      get
      {
        return m_bAutoTextColor;
      }
      set
      {
        m_bAutoTextColor = value;
      }
    }
    /// <summary>
    /// Automatic text background.
    /// </summary>
    public bool IsAutoTextBack
    {
      get
      {
        return m_bAutoTextBack;
      }
      set
      {
        m_bAutoTextBack = value;
      }
    }
    /// <summary>
    /// Automatic rotation.
    /// </summary>
    public bool IsAutoRotation
    {
      get
      {
        return m_bAutoRotation;
      }
      set
      {
        m_bAutoRotation = value;
      }
    }
    /// <summary>
    /// Indicates is axis label is left to right.
    /// </summary>
    public bool IsLeftToRight
    {
      get
      {
        return m_bIsLeftToRight;
      }
      set
      {
        m_bIsLeftToRight = value;
      }
    }
    /// <summary>
    /// Indicates is axis label is right to left.
    /// </summary>
    public bool IsRightToLeft
    {
      get
      {
        return m_bIsRightToLeft;
      }
      set
      {
        m_bIsRightToLeft = value;
      }
    }
    /// <summary>
    /// Rotation.
    /// </summary>
    public TRotation Rotation
    {
      get
      {
        return (TRotation) ( GetUInt16BitsByMask( m_usFlags, DEF_ROTATION_MASK )
          >> DEF_FIRST_ROTATION_BIT );
      }
      set
      {
        SetUInt16BitsByMask( ref m_usFlags, DEF_ROTATION_MASK, 
          ( ushort ) ( (ushort) value << DEF_FIRST_ROTATION_BIT ) );
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
    public  ChartTickRecord()
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
    public  ChartTickRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }
    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  ChartTickRecord( int iReserve )
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
      // TODO: check correctness of data

      m_MajorMark = provider.ReadByte( iOffset );
      iOffset++;

      m_MinorMark = provider.ReadByte( iOffset );
      iOffset++;

      m_labelPos = provider.ReadByte( iOffset );
      iOffset++;

      m_BackgroundMode = provider.ReadByte( iOffset );
      iOffset++;

      m_uiTextColor = provider.ReadUInt32( iOffset );
      iOffset += 4;

      for( int i = 0; i < ReservedFieldSize; i++, iOffset++ )
        provider.WriteByte( iOffset, 0 );
      // Reserved field.
      //iOffset += ReservedFieldSize;


      m_usFlags = provider.ReadUInt16( iOffset );
      m_bAutoTextColor = provider.ReadBit( iOffset, 0 );
      m_bAutoTextBack = provider.ReadBit( iOffset, 1 );
      m_bAutoRotation = provider.ReadBit( iOffset, 5 );
      iOffset++;
  
      m_bIsLeftToRight = provider.ReadBit( iOffset, 6 );
      m_bIsRightToLeft = provider.ReadBit( iOffset, 7 );
      iOffset++;

      m_usTickColorIndex = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_sRotationAngle = provider.ReadInt16( iOffset );
      if (m_sRotationAngle != 0)
          m_bAutoRotation = false;
      iOffset += 2;
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

      provider.WriteByte( iOffset, m_MajorMark );
      iOffset++;

      provider.WriteByte( iOffset, m_MinorMark );
      iOffset++;

      provider.WriteByte( iOffset, m_labelPos );
      iOffset++;

      provider.WriteByte( iOffset, m_BackgroundMode );
      iOffset++;

      provider.WriteUInt32( iOffset, m_uiTextColor );
      iOffset += 4;

      // Reserved field.
      iOffset += ReservedFieldSize;


      provider.WriteUInt16( iOffset, m_usFlags );
      provider.WriteBit( iOffset, m_bAutoTextColor, 0 );
      provider.WriteBit( iOffset, m_bAutoTextBack, 1 );
      provider.WriteBit( iOffset, m_bAutoRotation, 5 );
      iOffset++;
  
      provider.WriteBit( iOffset, m_bIsLeftToRight, 6 );
      provider.WriteBit( iOffset, m_bIsRightToLeft, 7 );
      iOffset++;

      provider.WriteUInt16( iOffset, m_usTickColorIndex );
      iOffset += 2;

      provider.WriteInt16( iOffset, m_sRotationAngle );
      iOffset += 2;
    }
    /// <summary>
    /// 
    /// </summary>
    public override int GetStoreSize( ExcelVersion version )
    {
      return DEF_RECORD_SIZE;
    }
    #endregion
  }
}
