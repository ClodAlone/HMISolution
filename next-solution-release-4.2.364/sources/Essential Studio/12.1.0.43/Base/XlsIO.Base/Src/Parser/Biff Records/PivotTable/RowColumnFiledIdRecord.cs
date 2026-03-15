#region Copyright

//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.

#endregion Copyright

using System;
using System.IO;

namespace Syncfusion.XlsIO.Parser.Biff_Records.PivotTable
{
  /// <summary>
  /// This record stores an array of field ID numbers (2-byte integers) for
  /// the row fields and column fields in a PivotTable. Two RowColumnFieldIdRecords
  /// appear in the file: the first contains the array of row field IDs, and
  /// the second contains the array of column field IDs.
  /// </summary>
  [ Biff( TBIFFRecord.RowColumnFieldId ) ]
  [ CLSCompliant( false ) ]
  public class RowColumnFiledIdRecord : BiffRecordRaw
  {
    #region Class members
    /// <summary>
    /// Array of 2-byte integers; contains either row field IDs or column field IDs.
    /// </summary>
    private ushort[] m_arrFieldId;
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor.
    /// </summary>
    public  RowColumnFiledIdRecord()
      : base()
    {
    }

    /// <summary>
    /// Read / initialize constructor.
    /// </summary>
    /// <param name="stream">Stream from which record data should be read.</param>
    /// <param name="itemSize">Size of read item.</param>
    /// <exception cref="System.ArgumentNullException">When stream is not specified.</exception>
    /// <exception cref="System.ApplicationException">When stream does not support read or seek operations.</exception>
    public  RowColumnFiledIdRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }

    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for the data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  RowColumnFiledIdRecord( int iReserve )
      : base( iReserve )
    {
    }

    #endregion

    #region Class properties
    /// <summary>
    /// Array of 2-byte integers; contains either row field IDs or column field IDs.
    /// </summary>
    public ushort[] FieldIds
    {
      get
      {
        return m_arrFieldId;
      }
      set
      {
        if( value == null )
          throw new ArgumentNullException( "value" );

        m_arrFieldId = value;
      }
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
      int iArrayLen = m_iLength / 2;
      m_arrFieldId = new ushort[ iArrayLen ];
      for( int i = 0; i < iArrayLen; i++, iOffset += ExcelConstants.ShortSize )
      {
        m_arrFieldId[ i ] = provider.ReadUInt16( iOffset );
      }
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
      m_iLength = m_arrFieldId.Length * 2;
      provider.WriteByte( iOffset + m_iLength - 1, 0 );

      for( int i = 0, len = m_arrFieldId.Length; i < len; i++, iOffset += ExcelConstants.ShortSize )
      {
        provider.WriteUInt16( iOffset, m_arrFieldId[ i ] );
      }
    }
    /// <summary>
    /// Evaluates size of the required storage space.
    /// </summary>
    /// <returns>Size of the required storage space.</returns>
    public override int GetStoreSize( ExcelVersion version )
    {
      return m_arrFieldId.Length * ExcelConstants.ShortSize;
    }
    #endregion

  }
}
