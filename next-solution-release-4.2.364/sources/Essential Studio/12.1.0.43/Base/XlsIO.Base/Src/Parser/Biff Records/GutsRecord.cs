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

using System;
using System.IO;

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  /// <summary>
  /// Contains information about the layout of outline symbols.
  /// </summary>
  [ Biff( TBIFFRecord.Guts ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class GutsRecord  : BiffRecordRaw
  {
    #region Class members

    /// <summary>
    /// Width of the area to display row outlines (left of the sheet), in pixels.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usLeftRowGutter = 0;
    /// <summary>
    /// Height of the area to display column outlines (above the sheet), in pixels.
    /// </summary>
    [ BiffRecordPos( 2, 2 ) ]
    private ushort m_usTopColGutter = 0;
    /// <summary>
    /// Number of visible row outline levels (used row levels + 1;
    /// or 0, if not used).
    /// </summary>
    [ BiffRecordPos( 4, 2 ) ]
    private ushort m_usMaxRowLevel = 0;
    /// <summary>
    /// Number of visible column outline levels (used column levels + 1;
    ///  or 0, if not used).
    /// </summary>
    [ BiffRecordPos( 6, 2 ) ]
    private ushort m_usMaxColLevel = 0;
    #endregion

    #region Class properties

    /// <summary>
    /// Width of the area to display row outlines (left of the sheet), in pixels.
    /// </summary>
    public ushort LeftRowGutter
    {
      get
      {
        return m_usLeftRowGutter;
      }
      set
      {
        m_usLeftRowGutter = value;
      }
    }

    /// <summary>
    /// Height of the area to display column outlines (above the sheet), in pixels.
    /// </summary>
    public ushort TopColumnGutter
    {
      get
      {
        return m_usTopColGutter;
      }
      set
      {
        m_usTopColGutter = value;
      }
    }

    /// <summary>
    /// Number of visible row outline levels (used row levels + 1;
    /// or 0, if not used).
    /// </summary>
    public ushort MaxRowLevel
    {
      get
      {
        return m_usMaxRowLevel;
      }
      set
      {
        m_usMaxRowLevel = value;
      }
    }

    /// <summary>
    /// Number of visible column outline levels (used column levels + 1;
    /// or 0, if not used).
    /// </summary>
    public ushort MaxColumnLevel
    {
      get
      {
        return m_usMaxColLevel;
      }
      set
      {
        m_usMaxColLevel = value;
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
        return 8;
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
        return 8;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor
    /// </summary>
    public  GutsRecord()
      : base()
    {
    }

    /// <summary>
    /// Read / initialize constructor.
    /// </summary>
    /// <param name="stream">Stream from which record data should be read.</param>
    /// <param name="itemSize">Size of read item.</param>
    /// <exception cref="System.ArgumentNullException">
    /// If stream is not specified.
    /// </exception>
    /// <exception cref="System.ApplicationException">
    /// If stream does not support read or seek operations.
    /// </exception>
    public  GutsRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }

    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// If amount of bytes requested is less than zero.
    /// </exception>
    public  GutsRecord( int iReserve )
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
    /// <exception cref="Syncfusion.XlsIO.Implementation.Exceptions.WrongBiffRecordDataException">
    /// If there is any internal error.
    /// </exception>
    public override void ParseStructure( DataProvider provider, int iOffset, int iLength, ExcelVersion version )
    {
      m_usLeftRowGutter = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_usTopColGutter = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_usMaxRowLevel = provider.ReadUInt16( iOffset );
      iOffset += 2;

      m_usMaxColLevel = provider.ReadUInt16( iOffset );
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
      provider.WriteUInt16( iOffset, m_usLeftRowGutter );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usTopColGutter );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usMaxRowLevel );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usMaxColLevel );
      //iOffset += 2;
    }

    #endregion
  }
}
