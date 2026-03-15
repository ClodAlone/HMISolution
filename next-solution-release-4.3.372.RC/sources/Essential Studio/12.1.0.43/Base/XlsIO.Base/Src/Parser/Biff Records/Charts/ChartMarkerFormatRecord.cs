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
using Syncfusion.XlsIO.Implementation.Shapes;
#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records.Charts
{

  /// <summary>
  /// This record defines the color and shape of the line
  /// markers that appear on scatter and line charts.
  /// </summary>
  [ Biff( TBIFFRecord.ChartMarkerFormat ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class ChartMarkerFormatRecord : BiffRecordRaw
  {
    #region Class constants
    /// <summary>
    /// Type of marker.
    /// </summary>
    public enum TMarker : int
    {
      /// <summary>
      /// Represents the NoMarker marker type.
      /// </summary>
      NoMarker          = 0,
      /// <summary>
      /// Represents the Square marker type.
      /// </summary>
      Square            = 1,
      /// <summary>
      /// Represents the Diamond marker type.
      /// </summary>
      Diamond           = 2,
      /// <summary>
      /// Represents the Triangle marker type.
      /// </summary>
      Triangle          = 3,
      /// <summary>
      /// Represents the X marker type.
      /// </summary>
      X                 = 4,
      /// <summary>
      /// Represents the Star marker type.
      /// </summary>
      Star              = 5,
      /// <summary>
      /// Represents the DowJones marker type.
      /// </summary>
      DowJones          = 6,
      /// <summary>
      /// Represents the StandardDeviation marker type.
      /// </summary>
      StandardDeviation = 7,
      /// <summary>
      /// Represents the Circle marker type.
      /// </summary>
      Circle            = 8,
      /// <summary>
      /// Represents the PlusSign marker type.
      /// </summary>
      PlusSign          = 9,
    }
    /// <summary>
    /// Correct size of the record.
    /// </summary>
    public const int DEF_RECORD_SIZE = 20;
    #endregion

    #region Class members
    /// <summary>
    /// Foreground color: RGB value (high byte = 0).
    /// </summary>
    [ BiffRecordPos( 0, 4, true ) ]
    private int m_iForeColor;
    /// <summary>
    /// Background color: RGB value (high byte = 0).
    /// </summary>
    [ BiffRecordPos( 4, 4, true ) ]
    private int m_iBackColor;
    /// <summary>
    /// Type of marker.
    /// </summary>
    [ BiffRecordPos( 8, 2 ) ]
    private ushort m_usMarkerType = 1;
    /// <summary>
    /// Format flags.
    /// </summary>
    [ BiffRecordPos( 10, 2 ) ]
    private ushort m_usOptions = 0;
    /// <summary>
    /// Index to color of marker border.
    /// </summary>
    [ BiffRecordPos( 12, 2 ) ]
    private ushort m_usBorderColorIndex;
    /// <summary>
    /// Index to color of marker fill.
    /// </summary>
    [ BiffRecordPos( 14, 2 ) ]
    private ushort m_usFillColorIndex;
    /// <summary>
    /// Size of line markers.
    /// </summary>
    [ BiffRecordPos( 16, 4, true ) ]
    private int m_iLineSize = 100;
    /// <summary>
    /// Automatic color.
    /// </summary>
    [ BiffRecordPos( 10, 0, TFieldType.Bit ) ]
    private bool m_bAutoColor = true;
    /// <summary>
    /// True = "background = none".
    /// </summary>
    [ BiffRecordPos( 10, 4, TFieldType.Bit ) ]
    private bool m_bNotShowInt;
    /// <summary>
    /// True = "foreground = none".
    /// </summary>
    [ BiffRecordPos( 10, 5, TFieldType.Bit ) ]
    private bool m_bNotShowBrd;
    private bool m_blineProperties;
    #endregion

    #region Class properties
    /// <summary>
    /// Foreground color: RGB value (high byte = 0).
    /// </summary>
    public int ForeColor
    {
      get
      {
        return m_iForeColor;
      }
      set
      {
        m_iForeColor = value;
      }
    }
    /// <summary>
    /// Background color: RGB value (high byte = 0).
    /// </summary>
    public int BackColor
    {
      get
      {
        return m_iBackColor;
      }
      set
      {
        m_iBackColor = value;
      }
    }
    /// <summary>
    /// Type of marker.
    /// </summary>
    public ExcelChartMarkerType MarkerType
    {
      get
      {
        return (ExcelChartMarkerType) m_usMarkerType;
      }
      set
      {
        m_usMarkerType = (ushort) value;
      }
    }
    /// <summary>
    /// Format flags. Read-only.
    /// </summary>
    public ushort Options
    {
      get
      {
        return m_usOptions;
      }
    }
    /// <summary>
    /// Index to color of marker border.
    /// </summary>
    public ushort BorderColorIndex
    {
      get
      {
        return m_usBorderColorIndex;
      }
      set
      {
        m_usBorderColorIndex = value;
      }
    }
    /// <summary>
    /// Index to color of marker fill.
    /// </summary>
    public ushort FillColorIndex
    {
      get
      {
        return m_usFillColorIndex;
      }
      set
      {
        m_usFillColorIndex = value;
      }
    }
    /// <summary>
    /// Size of line markers.
    /// </summary>
    public int LineSize
    {
      get
      {
        return m_iLineSize;
      }
      set
      {
        m_iLineSize = value;
      }
    }
    /// <summary>
    /// Automatic color.
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
    /// True = "background = none".
    /// </summary>
    public bool IsNotShowInt
    {
      get
      {
        return m_bNotShowInt;
      }
      set
      {
        m_bNotShowInt = value;
      }
    }
    /// <summary>
    /// True = "foreground = none".
    /// </summary>
    public bool IsNotShowBrd
    {
      get
      {
        return m_bNotShowBrd;
      }
      set
      {
        m_bNotShowBrd = value;
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
    /// <summary>
    /// Indicates whether the Marker has the line properties.
    /// </summary>
    internal bool HasLineProperties
    {
        get
        {
            return m_blineProperties;
        }
        set
        {
            m_blineProperties = value;
        }
    }
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor, initializes all fields with default values.
    /// </summary>
    public  ChartMarkerFormatRecord()
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
    public  ChartMarkerFormatRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }
    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  ChartMarkerFormatRecord( int iReserve )
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

      m_iForeColor = provider.ReadInt32( iOffset );
      iOffset += 4;

      m_iBackColor = provider.ReadInt32( iOffset );
      iOffset += 4;

      m_usMarkerType = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_usOptions = provider.ReadUInt16( iOffset );
      m_bAutoColor = provider.ReadBit( iOffset, 0 );
      m_bNotShowInt = provider.ReadBit( iOffset, 4 );
      m_bNotShowBrd = provider.ReadBit( iOffset, 5 );
      iOffset += 2;

      m_usBorderColorIndex = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_usFillColorIndex = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_iLineSize = provider.ReadInt32( iOffset );
      //iOffset += 4;
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

      provider.WriteInt32( iOffset, m_iForeColor );
      iOffset += 4;

      provider.WriteInt32( iOffset, m_iBackColor );
      iOffset += 4;

      provider.WriteUInt16( iOffset, m_usMarkerType );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usOptions );
      provider.WriteBit( iOffset, m_bAutoColor, 0 );
      provider.WriteBit( iOffset, m_bNotShowInt, 4 );
      provider.WriteBit( iOffset, m_bNotShowBrd, 5 );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usBorderColorIndex );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usFillColorIndex );
      iOffset += 2;

      provider.WriteInt32( iOffset, m_iLineSize );
    }
    #endregion
  }
}
