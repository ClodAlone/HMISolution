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
  /// This record defines a surface chart group.
  /// </summary>
  [ Biff( TBIFFRecord.ChartSurface ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class ChartSurfaceRecord : BiffRecordRaw
  {
    #region Class constants
    /// <summary>
    /// Correct size of the record.
    /// </summary>
    public const int DEF_RECORD_SIZE = 2;
    #endregion

    #region Class members
    /// <summary>
    /// Option flags.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usOptions = 0;
    /// <summary>
    /// True if chart contains color fill for surface.
    /// </summary>
    [ BiffRecordPos( 0, 0, TFieldType.Bit ) ]
    private bool m_bFillSurface;
    /// <summary>
    /// True if this surface chart has shading.
    /// </summary>
    [ BiffRecordPos( 0, 1, TFieldType.Bit ) ]
    private bool m_b3DPhongShade;
    #endregion

    #region Class properties
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
    /// True if chart contains color fill for surface.
    /// </summary>
    public bool IsFillSurface
    {
      get
      {
        return m_bFillSurface;
      }
      set
      {
        m_bFillSurface = value;
      }
    }
    /// <summary>
    /// True if this surface chart has shading.
    /// </summary>
    public bool Is3DPhongShade
    {
      get
      {
        return m_b3DPhongShade;
      }
      set
      {
        m_b3DPhongShade = value;
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
    public  ChartSurfaceRecord()
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
    public  ChartSurfaceRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }
    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  ChartSurfaceRecord( int iReserve )
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
      m_usOptions = provider.ReadUInt16( iOffset + 0 );
      m_bFillSurface = provider.ReadBit( iOffset + 0, 0 );
      m_b3DPhongShade = provider.ReadBit( iOffset + 0, 1 );
    }
    /// <summary>
    /// In this method, a class must pack all of its properties into
    /// an internal data array, m_data. This method is called by
    /// FillStream, when the record must be serialized into a stream.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">Offset in the buffer.</param>
    /// <param name="version">Excel version used for infill.</param>
    /// <returns>Size of the record data.</returns>
    public override void InfillInternalData( DataProvider provider, int iOffset, ExcelVersion version )
    {
      provider.WriteUInt16( iOffset + 0, m_usOptions );
      provider.WriteBit( iOffset + 0, m_bFillSurface, 0 );
      provider.WriteBit( iOffset + 0, m_b3DPhongShade, 1 );
      m_iLength = ExcelConstants.ShortSize;
    }
    #endregion
  }
}