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
using System.Collections;

using System.IO;
#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records.Charts
{
  /// <summary>
  /// This record defines a 3D chart group and also contains generic formatting information.
  /// </summary>
  [ Biff( TBIFFRecord.Chart3D ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class Chart3DRecord : BiffRecordRaw
  {
    #region Class constants
    /// <summary>
    /// Correct record size.
    /// </summary>
    private const int DEF_RECORD_SIZE = 14;
    #endregion

    #region Class members
    /// <summary>
    /// Rotation angle (0 to 360 degrees).
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usRotationAngle = 20;
    /// <summary>
    /// Elevation angle (�90 to +90 degrees).
    /// </summary>
    [ BiffRecordPos( 2, 2, true ) ]
    private short  m_sElevationAngle = 15;
    /// <summary>
    /// Distance from eye to chart (0 to 100).
    /// </summary>
    [ BiffRecordPos( 4, 2 ) ]
    private ushort m_usDistance = 30;
    /// <summary>
    /// Height of plot volume relative to width and depth.
    /// </summary>
    [ BiffRecordPos( 6, 2 ) ]
    private ushort m_usHeight = 100;
    /// <summary>
    /// Depth of points relative to width.
    /// </summary>
    [ BiffRecordPos( 8, 2 ) ]
    private ushort m_usDepth = 100;
    /// <summary>
    /// Space between series.
    /// </summary>
    [ BiffRecordPos( 10, 2 ) ]
    private ushort m_usGap = 150;
    /// <summary>
    /// Option flags.
    /// </summary>
    [ BiffRecordPos( 12, 2 ) ]
    private ushort m_usOptions;
    /// <summary>
    /// Whether to use perspective transform.
    /// </summary>
    [ BiffRecordPos( 12, 0, TFieldType.Bit ) ]
    private bool   m_bPerspective;
    /// <summary>
    /// 3-D columns are clustered or stacked.
    /// </summary>
    [ BiffRecordPos( 12, 1, TFieldType.Bit ) ]
    private bool   m_bClustered;
    /// <summary>
    /// Use auto scaling.
    /// </summary>
    [ BiffRecordPos( 12, 2, TFieldType.Bit ) ]
    private bool   m_bAutoScaling = true;
    /// <summary>
    /// Reserved; must be one.
    /// </summary>
    [ BiffRecordPos( 12, 4, TFieldType.Bit ) ]
    private bool   m_bReserved = true;
    /// <summary>
    /// Use 2D walls and gridlines.
    /// </summary>
    [ BiffRecordPos( 12, 5, TFieldType.Bit ) ]
    private bool   m_b2DWalls;
    /// <summary>
    /// Indicates whether elevation has default value.
    /// </summary>
    private bool m_bDefaultElevation = true;
    /// <summary>
    /// Indicates whether rotation has default value.
    /// </summary>
    private bool m_bDefaultRotation = true;
    #endregion

    #region Class properties
    /// <summary>
    /// Rotation angle (0 to 360 degrees).
    /// </summary>
    public ushort RotationAngle
    {
      get
      {
        return m_usRotationAngle;
      }
      set
      {
        if( value < 0 || value > 360 )
          throw new ArgumentOutOfRangeException( "value", "Value cannot be less than 0 or greater than 360." );

        m_usRotationAngle = value;
        m_bDefaultRotation = false;
      }
    }
    /// <summary>
    /// Elevation angle (�90 to +90 degrees).
    /// </summary>
    public short  ElevationAngle
    {
      get
      {
        return m_sElevationAngle;
      }
      set
      {
        if( value < -90 || value > 90 )
          throw new ArgumentOutOfRangeException( "value", "Value cannot be less than -90 or greater than 90." );

        m_sElevationAngle = value;
        m_bDefaultElevation = false;
      }
    }
    /// <summary>
    /// Indicates whether rotation has default value.
    /// </summary>
    public bool IsDefaultRotation
    {
      get
      {
        return m_bDefaultRotation;
      }
      set
      {
        m_bDefaultRotation = value;
      }
    }
    /// <summary>
    /// Indicates whether elevation has default value.
    /// </summary>
    public bool IsDefaultElevation
    {
      get
      {
        return m_bDefaultElevation;
      }
      set
      {
        m_bDefaultElevation = value;
      }
    }
    /// <summary>
    /// Distance from eye to chart (0 to 100).
    /// </summary>
    public ushort DistanceFromEye
    {
      get
      {
        return m_usDistance;
      }
      set
      {
        if( value < 0 || value > 100 )
          throw new ArgumentOutOfRangeException( "value", "Value cannot be less than 0 or greater than 100." );

        m_usDistance = value;
      }
    }

    /// <summary>
    /// Height of plot volume relative to width and depth.
    /// </summary>
    public ushort Height
    {
      get
      {
        return m_usHeight;
      }
      set
      {
        m_usHeight = value;
      }
    }
    /// <summary>
    /// Depth of points relative to width.
    /// </summary>
    public ushort Depth
    {
      get
      {
        return m_usDepth;
      }
      set
      {
        if( value != m_usDepth )
        {
          m_usDepth = value;
        }
      }
    }
    /// <summary>
    /// Space between series.
    /// </summary>
    public ushort SeriesSpace
    {
      get
      {
        return m_usGap;
      }
      set
      {
        if( value != m_usGap )
        {
          m_usGap = value;
        }
      }
    }
    /// <summary>
    /// Holder of flags.
    /// </summary>
    public ushort Options
    {
      get
      {
        return m_usOptions;
      }
    }
    /// <summary>
    /// Whether to use perspective transform.
    /// </summary>
    public bool   IsPerspective
    {
      get
      {
        return m_bPerspective;
      }
      set
      {
        if( value != m_bPerspective )
        {
          m_bPerspective = value;
        }
      }
    }
    /// <summary>
    /// 3D columns are clustered or stacked.
    /// </summary>
    public bool   IsClustered
    {
      get
      {
        return m_bClustered;
      }
      set
      {
        if( value != m_bClustered )
        {
          m_bClustered = value;
        }
      }
    }
    /// <summary>
    /// Use auto scaling.
    /// </summary>
    public bool   IsAutoScaled
    {
      get
      {
        return m_bAutoScaling;
      }
      set
      {
        if( value != m_bAutoScaling )
        {
          m_bAutoScaling = value;
        }
      }
    }
    /// <summary>
    /// Use 2D walls and gridlines.
    /// </summary>
    public bool   Is2DWalls
    {
      get
      {
        return m_b2DWalls;
      }
      set
      {
        if( value != m_b2DWalls )
        {
          m_b2DWalls = value;
        }
      }
    }

#if DEBUG
    /// <summary>
    /// Gets or Sets reserved. Must be one.
    /// </summary>
    public bool Reserved
    {
      get
      {
        return m_bReserved;
      }
      set
      {
        m_bReserved = value;
      }
    }
#endif
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor, sets all fields with default values.
    /// </summary>
    public  Chart3DRecord()
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
    public  Chart3DRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }
    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  Chart3DRecord( int iReserve )
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

      m_usRotationAngle = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_sElevationAngle = provider.ReadInt16( iOffset );
      iOffset += 2;

      m_usDistance = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_usHeight = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_usDepth = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_usGap = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_usOptions = provider.ReadUInt16( iOffset );
      m_bPerspective = provider.ReadBit( iOffset, 0 );
      m_bClustered = provider.ReadBit( iOffset, 1 );
      m_bAutoScaling = provider.ReadBit( iOffset, 2 );
      m_bReserved = provider.ReadBit( iOffset, 4 );
      m_b2DWalls = provider.ReadBit( iOffset, 5 );
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
      m_usOptions &= 0x37;

      m_iLength = GetStoreSize( version );

      provider.WriteUInt16( iOffset, m_usRotationAngle );
      iOffset += 2;

      provider.WriteInt16( iOffset, m_sElevationAngle );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usDistance );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usHeight );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usDepth );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usGap );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usOptions );
      provider.WriteBit( iOffset, m_bPerspective, 0 );
      provider.WriteBit( iOffset, m_bClustered, 1 );
      provider.WriteBit( iOffset, m_bAutoScaling, 2 );
      provider.WriteBit( iOffset, m_bReserved, 4 );
      provider.WriteBit( iOffset, m_b2DWalls, 5 );
    }

    public static bool operator==( Chart3DRecord chart3D, Chart3DRecord chart3D2 )
    {
      if( object.Equals( chart3D, null ) && object.Equals( chart3D2, null ) )
        return true;

      if( object.Equals( chart3D, null ) || object.Equals( chart3D2, null ) )
        return false;

      return chart3D2.m_usRotationAngle == chart3D.m_usRotationAngle &&
        chart3D2.m_sElevationAngle == chart3D.m_sElevationAngle &&
        chart3D2.m_usDistance == chart3D.m_usDistance &&
        chart3D2.m_usHeight == chart3D.m_usHeight &&
        chart3D2.m_usDepth == chart3D.m_usDepth &&
        chart3D2.m_usGap == chart3D.m_usGap &&
        chart3D2.m_bPerspective == chart3D.m_bPerspective &&
        chart3D2.m_bClustered == chart3D.m_bClustered &&
        chart3D2.m_bAutoScaling == chart3D.m_bAutoScaling &&
        chart3D2.m_b2DWalls == chart3D.m_b2DWalls;
    }

    public static bool operator!=( Chart3DRecord chart3D, Chart3DRecord chart3D2 )
    {
      return !( chart3D == chart3D2 );
    }
    #endregion
  }
}
