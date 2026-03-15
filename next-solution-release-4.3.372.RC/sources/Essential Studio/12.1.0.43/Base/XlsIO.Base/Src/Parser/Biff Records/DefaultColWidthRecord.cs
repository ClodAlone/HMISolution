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
  /// Specifies the default width for columns that have no specific width set.
  /// </summary>
  [ Biff( TBIFFRecord.DefaultColWidth ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class DefaultColWidthRecord  : BiffRecordRaw
  {
    #region Class constants
    /// <summary>
    /// Correct size of the record.
    /// </summary>
    private const int DefaultRecordSize = 2;
    #endregion

    #region Class members

    /// <summary>
    /// Column width in characters, using the width of the zero character
    /// with default font (first font record in the file).
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usColWidth = 8;
    #endregion

    #region Class properties

    /// <summary>
    /// Column width in characters, using the width of the zero character
    /// with default font (first font record in the file).
    /// </summary>
    public ushort Width
    {
      get
      {
        return m_usColWidth;
      }
      set
      {
        m_usColWidth = value;
      }
    }

    /// <summary>
    /// Read-only. Returns minimum possible size of record's
    /// internal data array.
    /// </summary>
    override public int MinimumRecordSize
    {
      get
      {
        return 2;
      }
    }

    /// <summary>
    /// Read-only. Returns maximum possible size of record's
    /// internal data array.
    /// </summary>
    override public int MaximumRecordSize
    {
      get
      {
        return 2;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor
    /// </summary>
    public  DefaultColWidthRecord()
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
    public  DefaultColWidthRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }

    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  DefaultColWidthRecord( int iReserve )
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
      m_usColWidth = provider.ReadUInt16( iOffset + 0 );
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
      provider.WriteUInt16( iOffset + 0, m_usColWidth );
      m_iLength = DefaultRecordSize;
    }
    /// <summary>
    /// Evaluates size of the required storage space.
    /// </summary>
    /// <returns>Size of the required storage space.</returns>
    public override int GetStoreSize( ExcelVersion version )
    {
      return DefaultRecordSize;
    }
    #endregion
  }
}
