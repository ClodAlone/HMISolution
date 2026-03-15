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

#if ( WINRT )
using Windows.UI;
using Syncfusion.XlsIO.Implementation.WINRT;
using Syncfusion.XlsIO.Implementation;
#endif

#if  SILVERLIGHT
using System.Windows.Media;
using Syncfusion.XlsIO.Implementation.Silverlight;
using Syncfusion.XlsIO.Implementation;
#elif WP
using System.Windows.Media;
using Syncfusion.XlsIO.Implementation.WP;
using Syncfusion.XlsIO.Implementation;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;
using Syncfusion.XlsIO.Implementation;
#endif
#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records.Charts
{
  /// <summary>
  /// This record describes the patterns and colors used in a filled area.
  /// </summary>
  [ Biff( TBIFFRecord.ChartAreaFormat ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class ChartAreaFormatRecord : BiffRecordRaw
  {
    #region Class constants
    /// <summary>
    /// Correct size of the record.
    /// </summary>
    public const int DEF_RECORD_SIZE = 16;
    #endregion

    #region Class members
    /// <summary>
    /// Foreground color (RGB).
    /// </summary>
    [ BiffRecordPos( 0, 4, true ) ]
    private int    m_iForeground;
    /// <summary>
    /// Background color (RGB).
    /// </summary>
    [ BiffRecordPos( 4, 4, true ) ]
    private int    m_iBackground;
    /// <summary>
    /// Pattern.
    /// </summary>
    [ BiffRecordPos( 8, 2 ) ]
    private ushort m_usPattern;
    /// <summary>
    /// Storage for record bit flags.
    /// </summary>
    [ BiffRecordPos( 10, 2 ) ]
    private ushort m_usOptions;
    /// <summary>
    /// Automatic format.
    /// </summary>
    [ BiffRecordPos( 10, 0, TFieldType.Bit ) ]
    private bool   m_bAutomaticFormat = true;
    /// <summary>
    /// Foreground and background are swapped when the data value is negative.
    /// </summary>
    [ BiffRecordPos( 10, 1, TFieldType.Bit ) ]
    private bool   m_bSwapColorsOnNegative;// = true;
    /// <summary>
    /// Index of foreground color.
    /// </summary>
    [ BiffRecordPos( 12, 2 ) ]
    private ushort m_usForegroundIndex;
    /// <summary>
    /// Background color index.
    /// </summary>
    [ BiffRecordPos( 14, 2 ) ]
    private ushort m_usBackgroundIndex;
    #endregion

    #region Class properties
    /// <summary>
    /// Foreground color (RGB).
    /// </summary>
    public int   ForegroundColor
    {
      get
      {
        return m_iForeground;
      }
      set
      {
        if( value != m_iForeground )
        {
          m_iForeground = value;
        }
      }
    }
    /// <summary>
    /// Background color (RGB).
    /// </summary>
    public Color    BackgroundColor
    {
      get
      {
        return ColorExtension.FromArgb( m_iBackground );
      }
      set
      {
        int iValue = ( value.ToArgb() & 0xffffff );

        if( iValue != m_iBackground )
        {
          m_iBackground = iValue;
        }
      }
    }
    /// <summary>
    /// Pattern.
    /// </summary>
    public ExcelPattern Pattern
    {
      get
      {
        return ( ExcelPattern )m_usPattern;
      }
      set
      {
        m_usPattern = ( ushort )value;
      }
    }
    /// <summary>
    /// Holder of record flags.
    /// </summary>
    public ushort Options
    {
      get
      {
        return m_usOptions;
      }
    }
    /// <summary>
    /// Index of foreground color.
    /// </summary>
    public ExcelKnownColors ForegroundColorIndex
    {
      get
      {
        return ( ExcelKnownColors )m_usForegroundIndex;
      }
      set
      {
        ushort usValue = ( ushort )value;

        if( usValue != m_usForegroundIndex )
        {
          m_usForegroundIndex = usValue;
        }
      }
    }
    /// <summary>
    /// Background color index.
    /// </summary>
    public ExcelKnownColors BackgroundColorIndex
    {
      get
      {
        return ( ExcelKnownColors )m_usBackgroundIndex;
      }
      set
      {
        ushort usValue = ( ushort )value;

        if( usValue != m_usBackgroundIndex )
        {
          m_usBackgroundIndex = usValue;
        }
      }
    }
    /// <summary>
    /// Automatic format or not.
    /// </summary>
    public bool   UseAutomaticFormat
    {
      get
      {
        return m_bAutomaticFormat;
      }
      set
      {
        m_bAutomaticFormat = value;
      }
    }
    /// <summary>
    /// Foreground and background are swapped when the data value is negative.
    /// </summary>
    public bool   SwapColorsOnNegative
    {
      get
      {
        return m_bSwapColorsOnNegative;
      }
      set
      {
        m_bSwapColorsOnNegative = value;
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
        return DEF_RECORD_SIZE;
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
        return DEF_RECORD_SIZE;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor, initializes all fields with default values.
    /// </summary>
    public  ChartAreaFormatRecord()
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
    public  ChartAreaFormatRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }
    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  ChartAreaFormatRecord( int iReserve )
      : base( iReserve )
    {
    }
    #endregion

    #region Record Serialization
    /// <summary>
    /// 
    /// </summary>
    public override int GetStoreSize( ExcelVersion version )
    {
      return DEF_RECORD_SIZE;
    }
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

      //AutoExtractFields();
      //m_iForeground = provider.ReadInt32( iOffset );
      m_iForeground = ReadColor( provider, ref iOffset );
      //iOffset += 4;

      m_iBackground = ReadColor( provider, ref iOffset );//provider.ReadInt32( iOffset );
      //iOffset += 4;

      m_usPattern = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_usOptions = provider.ReadUInt16( iOffset );
      m_bAutomaticFormat = provider.ReadBit( iOffset, 0 );
      m_bSwapColorsOnNegative = provider.ReadBit( iOffset, 1 );
      iOffset += 2;

      m_usForegroundIndex = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_usBackgroundIndex = provider.ReadUInt16( iOffset );
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
      m_usOptions &= 0x03;
      m_iLength = GetStoreSize( version );

      //provider.WriteInt32( iOffset, m_iForeground );
      //iOffset += 4;
      WriteColor( provider, ref iOffset, m_iForeground );

      //provider.WriteInt32( iOffset, m_iBackground );
      //iOffset += 4;
      WriteColor( provider, ref iOffset, m_iBackground );

      provider.WriteUInt16( iOffset, m_usPattern );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usOptions );
      provider.WriteBit( iOffset, m_bAutomaticFormat, 0 );
      provider.WriteBit( iOffset, m_bSwapColorsOnNegative, 1 );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usForegroundIndex );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usBackgroundIndex );
    }
    private int ReadColor( DataProvider provider, ref int iOffset )
    {
      //int iAlpha = provider.ReadByte( iOffset++ );
      byte btR = provider.ReadByte( iOffset++ );
      byte btG = provider.ReadByte( iOffset++ );
      byte btB = provider.ReadByte( iOffset++ );
      iOffset++;
      return Color.FromArgb( 255, btR, btG, btB ).ToArgb();
    }
    private void WriteColor( DataProvider provider, ref int iOffset, int iColor )
    {
      Color color = ColorExtension.FromArgb( iColor );
      //provider.WriteByte( iOffset++, 0 ); // alpha
      provider.WriteByte( iOffset++, color.R );
      provider.WriteByte( iOffset++, color.G );
      provider.WriteByte( iOffset++, color.B );
      provider.WriteByte( iOffset++, 0 );
    }
    #endregion
  }
}