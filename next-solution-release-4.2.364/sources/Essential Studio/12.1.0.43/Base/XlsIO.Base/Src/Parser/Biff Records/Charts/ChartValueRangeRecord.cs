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
  /// This record defines the value axis.
  /// </summary>
  [ Biff( TBIFFRecord.ChartValueRange ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class ChartValueRangeRecord
    : BiffRecordRaw
    , IMaxCross
  {
    #region Class constants
    /// <summary>
    /// Record size constant.
    /// </summary>
    public const int DEF_RECORD_SIZE = 42;
    #endregion

    #region Class members
    /// <summary>
    /// Maximum value on axis.
    /// </summary>
    [ BiffRecordPos( 0, 8, TFieldType.Float ) ]
    private double m_dNumMin;
    /// <summary>
    /// Maximum value on axis.
    /// </summary>
    [ BiffRecordPos( 8, 8, TFieldType.Float ) ]
    private double m_dNumMax;
    /// <summary>
    /// Value of major increment.
    /// </summary>
    [ BiffRecordPos( 16, 8, TFieldType.Float ) ]
    private double m_dNumMajor;
    /// <summary>
    /// Value of minor increment.
    /// </summary>
    [ BiffRecordPos( 24, 8, TFieldType.Float ) ]
    private double m_dNumMinor;
    /// <summary>
    /// Value where category axis crosses.
    /// </summary>
    [ BiffRecordPos( 32, 8, TFieldType.Float ) ]
    private double m_dNumCross;
    /// <summary>
    /// Format flags.
    /// </summary>
    [ BiffRecordPos( 40, 2 ) ]
    private ushort m_usFormatFlags = 0;
    /// <summary>
    /// Automatic minimum selected.
    /// </summary>
    [ BiffRecordPos( 40, 0, TFieldType.Bit ) ]
    private bool m_bAutoMin = true;
    /// <summary>
    /// Automatic maximum selected.
    /// </summary>
    [ BiffRecordPos( 40, 1, TFieldType.Bit ) ]
    private bool m_bAutoMax = true;
    /// <summary>
    /// Automatic major selected.
    /// </summary>
    [ BiffRecordPos( 40, 2, TFieldType.Bit ) ]
    private bool m_bAutoMajor = true;
    /// <summary>
    /// Automatic minor selected.
    /// </summary>
    [ BiffRecordPos( 40, 3, TFieldType.Bit ) ]
    private bool m_bAutoMinor = true;
    /// <summary>
    /// Automatic category crossing point selected.
    /// </summary>
    [ BiffRecordPos( 40, 4, TFieldType.Bit ) ]
    private bool m_bAutoCross = true;
    /// <summary>
    /// Logarithmic scale.
    /// </summary>
    [ BiffRecordPos( 40, 5, TFieldType.Bit ) ]
    private bool m_bLogScale;
    /// <summary>
    /// Values in reverse order.
    /// </summary>
    [ BiffRecordPos( 40, 6, TFieldType.Bit ) ]
    private bool m_bReverse;
    /// <summary>
    /// Category axis to cross at maximum value.
    /// </summary>
    [ BiffRecordPos( 40, 7, TFieldType.Bit ) ]
    private bool m_bMaxCross;
    #endregion

    #region Class properties
    /// <summary>
    /// Maximum value on axis.
    /// </summary>
    public double NumMin
    {
      get
      {
        return m_dNumMin;
      }
      set
      {
        m_dNumMin = value;
      }
    }

    /// <summary>
    /// Maximum value on axis.
    /// </summary>
    public double NumMax
    {
      get
      {
        return m_dNumMax;
      }
      set
      {
        m_dNumMax = value;
      }
    }

    /// <summary>
    /// Value of major increment.
    /// </summary>
    public double NumMajor
    {
      get
      {
        return m_dNumMajor;
      }
      set
      {
        m_dNumMajor = value;
      }
    }

    /// <summary>
    /// Value of minor increment.
    /// </summary>
    public double NumMinor
    {
      get
      {
        return m_dNumMinor;
      }
      set
      {
        m_dNumMinor = value;
      }
    }

    /// <summary>
    /// Value where category axis crosses.
    /// </summary>
    public double NumCross
    {
      get
      {
        return m_dNumCross;
      }
      set
      {
        m_dNumCross = value;
      }
    }

    /// <summary>
    /// Format flags. Read-only.
    /// </summary>
    public ushort FormatFlags
    {
      get
      {
        return m_usFormatFlags;
      }
#if DEBUG
      set
      {
        m_usFormatFlags = value;
      }
#endif
    }

    /// <summary>
    /// Automatic minimum selected.
    /// </summary>
    public bool IsAutoMin
    {
      get
      {
        return m_bAutoMin;
      }
      set
      {
        m_bAutoMin = value;
      }
    }
    /// <summary>
    /// Automatic maximum selected.
    /// </summary>
    public bool IsAutoMax
    {
      get
      {
        return m_bAutoMax;
      }
      set
      {
        m_bAutoMax = value;
      }
    }
    /// <summary>
    /// Automatic major selected.
    /// </summary>
    public bool IsAutoMajor
    {
      get
      {
        return m_bAutoMajor;
      }
      set
      {
        m_bAutoMajor = value;
      }
    }
    /// <summary>
    /// Automatic minor selected.
    /// </summary>
    public bool IsAutoMinor
    {
      get
      {
        return m_bAutoMinor;
      }
      set
      {
        m_bAutoMinor = value;
      }
    }
    /// <summary>
    /// Automatic category crossing point selected.
    /// </summary>
    public bool IsAutoCross
    {
      get
      {
        return m_bAutoCross;
      }
      set
      {
        m_bAutoCross = value;
      }
    }
    /// <summary>
    /// Logarithmic scale.
    /// </summary>
    public bool IsLogScale
    {
      get
      {
        return m_bLogScale;
      }
      set
      {
        m_bLogScale = value;
      }
    }
    /// <summary>
    /// Values in reverse order.
    /// </summary>
    public bool IsReverse
    {
      get
      {
        return m_bReverse;
      }
      set
      {
        m_bReverse = value;
      }
    }
    /// <summary>
    /// Category axis to cross at maximum value.
    /// </summary>
    public bool IsMaxCross
    {
      get
      {
        return m_bMaxCross;
      }
      set
      {
        m_bMaxCross = value;
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
    public  ChartValueRangeRecord()
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
    public  ChartValueRangeRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }
    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  ChartValueRangeRecord( int iReserve )
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

      m_dNumMin = provider.ReadDouble( iOffset );
      iOffset += 8;

      m_dNumMax = provider.ReadDouble( iOffset );
      iOffset += 8;

      m_dNumMajor = provider.ReadDouble( iOffset );
      iOffset += 8;

      m_dNumMinor = provider.ReadDouble( iOffset );
      iOffset += 8;

      m_dNumCross = provider.ReadDouble( iOffset );
      iOffset += 8;

      m_usFormatFlags = provider.ReadUInt16( iOffset );
      m_bAutoMin = provider.ReadBit( iOffset, 0 );
      m_bAutoMax = provider.ReadBit( iOffset, 1 );
      m_bAutoMajor = provider.ReadBit( iOffset, 2 );
      m_bAutoMinor = provider.ReadBit( iOffset, 3 );
      m_bAutoCross = provider.ReadBit( iOffset, 4 );
      m_bLogScale = provider.ReadBit( iOffset, 5 );
      m_bReverse = provider.ReadBit( iOffset, 6 );
      m_bMaxCross = provider.ReadBit( iOffset, 7 );

      // iOffset += 2;
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

      provider.WriteDouble( iOffset, m_dNumMin );
      iOffset += 8;

      provider.WriteDouble( iOffset, m_dNumMax );
      iOffset += 8;

      provider.WriteDouble( iOffset, m_dNumMajor );
      iOffset += 8;

      provider.WriteDouble( iOffset, m_dNumMinor );
      iOffset += 8;

      provider.WriteDouble( iOffset, m_dNumCross );
      iOffset += 8;

      provider.WriteUInt16( iOffset, m_usFormatFlags );
      provider.WriteBit( iOffset, m_bAutoMin, 0 );
      provider.WriteBit( iOffset, m_bAutoMax, 1 );
      provider.WriteBit( iOffset, m_bAutoMajor, 2 );
      provider.WriteBit( iOffset, m_bAutoMinor, 3 );
      provider.WriteBit( iOffset, m_bAutoCross, 4 );
      provider.WriteBit( iOffset, m_bLogScale, 5 );
      provider.WriteBit( iOffset, m_bReverse, 6 );
      provider.WriteBit( iOffset, m_bMaxCross, 7 );
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