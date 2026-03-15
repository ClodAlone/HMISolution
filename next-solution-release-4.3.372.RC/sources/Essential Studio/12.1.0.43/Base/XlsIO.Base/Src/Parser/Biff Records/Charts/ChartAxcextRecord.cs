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
  /// This record defines axis options.
  /// </summary>
  [ Biff( TBIFFRecord.ChartAxcext ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class ChartAxcextRecord : BiffRecordRaw
  {
    #region Class constants
    /// <summary>
    /// Correct record size.
    /// </summary>
    private const int DEF_RECORD_SIZE = 18;
    /// <summary>
    /// Option flags.
    /// </summary>
    [ Flags ]
    private enum OptionFlags
    {
      /// <summary>
      /// None.
      /// </summary>
      None = 0,
      /// <summary>
      /// Use default minimum.
      /// </summary>
      DefaultMinimum = 1,
      /// <summary>
      /// Use default maximum.
      /// </summary>
      DefaultMaximum = 2,
      /// <summary>
      /// Use default major unit.
      /// </summary>
      DefaultMajorUnits = 4,
      /// <summary>
      /// Use default minor unit.
      /// </summary>
      DefaultMinorUnits = 8,
      /// <summary>
      /// This a date axis.
      /// </summary>
      DateAxis = 16,
      /// <summary>
      /// Use default base.
      /// </summary>
      DefaultBaseUnits = 32,
      /// <summary>
      /// Use default crossing point.
      /// </summary>
      DefaultCrossPoint = 64,
      /// <summary>
      /// Use default date settings for axis.
      /// </summary>
      DefaultDateSettings = 128,
    }
    #endregion

    #region Class members
    /// <summary>
    /// Minimum category on axis.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usMinCategoryAxis;
    /// <summary>
    /// Maximum category on axis.
    /// </summary>
    [ BiffRecordPos( 2, 2 ) ]
    private ushort m_usMaxCategoryAxis;
    /// <summary>
    /// Value of major unit.
    /// </summary>
    [ BiffRecordPos( 4, 2 ) ]
    private ushort m_usMajor = 1;
    /// <summary>
    /// Units of major unit.
    /// </summary>
    [ BiffRecordPos( 6, 2 ) ]
    private ushort m_usMajorUnits;
    /// <summary>
    /// Value of minor unit.
    /// </summary>
    [ BiffRecordPos( 8, 2 ) ]
    private ushort m_usMinor = 1;
    /// <summary>
    /// Units of minor unit.
    /// </summary>
    [ BiffRecordPos( 10, 2 ) ]
    private ushort m_usMinorUnits;
    /// <summary>
    /// Base unit of axis.
    /// </summary>
    [ BiffRecordPos( 12, 2 ) ]
    private ushort m_usBaseUnits;
    /// <summary>
    /// Crossing point of value axis (date).
    /// </summary>
    [ BiffRecordPos( 14, 2 ) ]
    private ushort m_usCrossingPoint;
    /// <summary>
    /// Option flags.
    /// </summary>
    [ BiffRecordPos( 16, 2 ) ]
    private OptionFlags m_options = ( OptionFlags )127;
    #endregion

    #region Class properties
    /// <summary>
    /// Minimum category on axis.
    /// </summary>
    public ushort MinCategoryOnAxis
    {
      get
      {
        return m_usMinCategoryAxis;
      }
      set
      {
        if( value != m_usMinCategoryAxis )
        {
          m_usMinCategoryAxis = value;
        }
      }
    }

    /// <summary>
    /// Maximum category on axis.
    /// </summary>
    public ushort MaxCategoryOnAxis
    {
      get
      {
        return m_usMaxCategoryAxis;
      }
      set
      {
        if( value != m_usMaxCategoryAxis )
        {
          m_usMaxCategoryAxis = value;
        }
      }
    }
    
    /// <summary>
    /// Value of major unit.
    /// </summary>
    public ushort Major
    {
      get
      {
        return m_usMajor;
      }
      set
      {
        if( value != m_usMajor )
        {
          m_usMajor = value;
        }
      }
    }

    /// <summary>
    /// Units of major unit.
    /// </summary>
    public ExcelChartBaseUnit MajorUnits
    {
      get
      {
        return ( ExcelChartBaseUnit )m_usMajorUnits;
      }
      set
      {
        m_usMajorUnits = ( ushort )value;
      }
    }

    /// <summary>
    /// Value of minor unit.
    /// </summary>
    public ushort Minor
    {
      get
      {
        return m_usMinor;
      }
      set
      {
        if( value != m_usMinor )
        {
          m_usMinor = value;
        }
      }
    }

    /// <summary>
    /// Units of minor unit.
    /// </summary>
    public ExcelChartBaseUnit MinorUnits
    {
      get
      {
        return ( ExcelChartBaseUnit )m_usMinorUnits;
      }
      set
      {
        m_usMinorUnits = ( ushort )value;
      }
    }

    /// <summary>
    /// Base unit of axis.
    /// </summary>
    public ExcelChartBaseUnit BaseUnits
    {
      get
      {
        return ( ExcelChartBaseUnit )m_usBaseUnits;
      }
      set
      {
        m_usBaseUnits = ( ushort )value;
      }
    }

    /// <summary>
    /// Crossing point of value axis (date).
    /// </summary>
    public ushort CrossingPoint
    {
      get
      {
        return m_usCrossingPoint;
      }
      set
      {
        if( value != m_usCrossingPoint )
        {
          m_usCrossingPoint = value;
        }
      }
    }
    /// <summary>
    /// Holder of record flags.
    /// </summary>
    public ushort Options
    {
      get
      {
        return ( ushort )m_options;
      }
    }
    /// <summary>
    /// Use default minimum.
    /// </summary>
    public bool UseDefaultMinimum
    {
      get
      {
        return ( m_options & OptionFlags.DefaultMinimum ) != 0;
      }
      set
      {
        if( value )
        {
          m_options |= OptionFlags.DefaultMinimum;
        }
        else
        {
          m_options &= ~OptionFlags.DefaultMinimum;
        }
      }
    }

    /// <summary>
    /// Use default maximum.
    /// </summary>
    public bool UseDefaultMaximum
    {
      get
      {
        return ( m_options & OptionFlags.DefaultMaximum ) != 0;
      }
      set
      {
        if( value )
        {
          m_options |= OptionFlags.DefaultMaximum;
        }
        else
        {
          m_options &= ~OptionFlags.DefaultMaximum;
        }
      }
    }

    /// <summary>
    /// Use default major unit.
    /// </summary>
    public bool UseDefaultMajorUnits
    {
      get
      {
        return ( m_options & OptionFlags.DefaultMajorUnits ) != 0;
      }
      set
      {
        if( value )
        {
          m_options |= OptionFlags.DefaultMajorUnits;
        }
        else
        {
          m_options &= ~OptionFlags.DefaultMajorUnits;
        }
      }
    }

    /// <summary>
    /// Use default minor unit.
    /// </summary>
    public bool UseDefaultMinorUnits
    {
      get
      {
        return ( m_options & OptionFlags.DefaultMinorUnits ) != 0;
      }
      set
      {
        if( value )
        {
          m_options |= OptionFlags.DefaultMinorUnits;
        }
        else
        {
          m_options &= ~OptionFlags.DefaultMinorUnits;
        }
      }
    }

    /// <summary>
    /// Date axis.
    /// </summary>
    public bool IsDateAxis
    {
      get
      {
        return ( m_options & OptionFlags.DateAxis ) != 0;
      }
      set
      {
        if( value )
        {
          m_options |= OptionFlags.DateAxis;
        }
        else
        {
          m_options &= ~OptionFlags.DateAxis;
        }
      }
    }

    /// <summary>
    /// Use default base.
    /// </summary>
    public bool UseDefaultBaseUnits
    {
      get
      {
        return ( m_options & OptionFlags.DefaultBaseUnits ) != 0;
      }
      set
      {
        if( value )
        {
          m_options |= OptionFlags.DefaultBaseUnits;
        }
        else
        {
          m_options &= ~OptionFlags.DefaultBaseUnits;
        }
      }
    }

    /// <summary>
    /// Use default crossing point.
    /// </summary>
    public bool UseDefaultCrossPoint
    {
      get
      {
        return ( m_options & OptionFlags.DefaultCrossPoint ) != 0;
      }
      set
      {
        if( value )
        {
          m_options |= OptionFlags.DefaultCrossPoint;
        }
        else
        {
          m_options &= ~OptionFlags.DefaultCrossPoint;
        }
      }
    }

    /// <summary>
    /// Use default date settings for axis.
    /// </summary>
    public bool UseDefaultDateSettings
    {
      get
      {
        return ( m_options & OptionFlags.DefaultDateSettings ) != 0;
      }
      set
      {
        if( value )
        {
          m_options |= OptionFlags.DefaultDateSettings;
        }
        else
        {
          m_options &= ~OptionFlags.DefaultDateSettings;
        }
      }
    }

    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor, initializes all fields with default values.
    /// </summary>
    public  ChartAxcextRecord()
      : base()
    {
    }
    /// <summary>
    /// Read / initialize constructor.
    /// </summary>
    /// <param name="stream">Stream from which record data should be read.</param>
    /// <param name="itemSize">size of read item</param>
    /// <exception cref="System.ArgumentNullException">If stream is not specified.</exception>
    /// <exception cref="System.ApplicationException">If stream does not support read or seek operations.</exception>
    public  ChartAxcextRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }
    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  ChartAxcextRecord( int iReserve )
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

      m_usMinCategoryAxis = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_usMaxCategoryAxis = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_usMajor = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_usMajorUnits = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_usMinor = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_usMinorUnits = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_usBaseUnits = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_usCrossingPoint = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_options = ( OptionFlags )provider.ReadUInt16( iOffset );

      if (m_usMajor > 1)
          m_options &= ~OptionFlags.DefaultMajorUnits;
      //iOffset += 2;
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
      //m_options &= 0xff;

      m_iLength = GetStoreSize( version );

      provider.WriteUInt16( iOffset, m_usMinCategoryAxis );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usMaxCategoryAxis );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usMajor );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usMajorUnits );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usMinor );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usMinorUnits );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usBaseUnits );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usCrossingPoint );
      iOffset += 2;

      provider.WriteUInt16( iOffset, ( ushort )m_options );
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