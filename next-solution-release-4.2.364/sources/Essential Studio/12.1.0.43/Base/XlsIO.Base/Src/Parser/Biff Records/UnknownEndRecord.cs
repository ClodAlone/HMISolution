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
  
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class UnknownEndRecord  : BiffRecordRaw
  {
    #region Class constants
    private const int DEF_UNKNOWN1 = 449;     // 0x1c1
    private const int DEF_UNKNOWN2 = 144525;  // 0x018d54
    /// <summary>
    /// Default record size.
    /// </summary>
    private const int DefaultRecordSize = 8;
    #endregion

    #region Class members

    [ BiffRecordPos( 0, 4, true ) ]
    private int m_iUnknown1;

    [ BiffRecordPos( 4, 4, true ) ]
    private int m_iUnknown2;
    #endregion

    #region Class properties

    /// <summary>
    /// Gets / sets first unknown int value.
    /// </summary>
    public int Unknown1
    {
      get
      {
        return m_iUnknown1;
      }
      set
      {
        m_iUnknown1 = value;
      }
    }

    /// <summary>
    /// Gets / sets second unknown int value.
    /// </summary>
    public int Unknown2
    {
      get
      {
        return m_iUnknown2;
      }
      set
      {
        m_iUnknown2 = value;
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
    public  UnknownEndRecord()
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
    public  UnknownEndRecord( Stream stream, out int itemSize )
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
    public  UnknownEndRecord( int iReserve )
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
      m_iUnknown1 = provider.ReadInt32( iOffset );
      iOffset += ExcelConstants.IntSize;

      m_iUnknown2 = provider.ReadInt32( iOffset );
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
      // hardcode part
      m_iUnknown1 = DEF_UNKNOWN1;
      m_iUnknown2 = DEF_UNKNOWN2;

      m_iLength = DefaultRecordSize;
      provider.WriteInt32( iOffset, m_iUnknown1 );
      provider.WriteInt32( iOffset + ExcelConstants.IntSize, m_iUnknown2 );
    }
    #endregion
  }
}
