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
  /// This record stores a PivotTable formula.
  /// </summary>
  [ Biff( TBIFFRecord.PivotFormula ) ]
  [ CLSCompliant( false ) ]
  public class PivotFormulaRecord : BiffRecordRaw
  {
    #region Constants
    /// <summary>
    /// Default record size.
    /// </summary>
    private const int DefaultRecordSize = 4;
    #endregion

    #region Class members
    /// <summary>
    /// Reserved. Should be set to zero.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usReserved;
    /// <summary>
    /// -1 if the calculated item formula applies to all fields, or,
    ///  if positive, the field that this calculated item formulas applies to.
    /// </summary>
    [ BiffRecordPos( 2, 2, true ) ]
    private short m_usAppliedField;
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor.
    /// </summary>
    public  PivotFormulaRecord()
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
    public  PivotFormulaRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }

    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for the data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  PivotFormulaRecord( int iReserve )
      : base( iReserve )
    {
    }

    #endregion

    #region Class properties
    /// <summary>
    /// Reserved. Should be set to zero. Read-only.
    /// </summary>
    public ushort Reserved
    {
      get
      {
        return m_usReserved;
      }
#if DEBUG
      set
      {
        m_usReserved = value;
      }
#endif
    }
    /// <summary>
    /// -1 if the calculated item formula applies to all fields, or,
    ///  if positive, the field that this calculated item formulas applies to.
    /// </summary>
    public short AppliedField
    {
      get
      {
        return m_usAppliedField;
      }
      set
      {
        m_usAppliedField = value;
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
      m_usReserved = provider.ReadUInt16( iOffset + 0 );
      m_usAppliedField = provider.ReadInt16( iOffset + 2 );
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
      provider.WriteUInt16( iOffset + 0, m_usReserved );
      provider.WriteInt16( iOffset + 2, m_usAppliedField );
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
