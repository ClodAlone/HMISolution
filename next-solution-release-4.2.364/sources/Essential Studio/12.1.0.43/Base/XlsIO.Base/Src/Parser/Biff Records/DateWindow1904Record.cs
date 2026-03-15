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
  /// This record specifies the base date for displaying date values.
  /// All dates are stored as count of days past this base date.
  /// </summary>
  [ Biff( TBIFFRecord.DateWindow1904 ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class DateWindow1904Record  : BiffRecordRaw
  {
    #region Class members
    /// <summary>
    /// Two bytes which used for storing boolean value:
    /// 0 = Base date is 1899-Dec-31 (the cell value 1 represents 1900-Jan-01)
    /// 1 = Base date is 1904-Jan-01 (the cell value 1 represents 1904-Jan-0)
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usWindow = 0;
    /// <summary>
    /// Use first bit of m_usWindow to store boolean flag:
    /// False = Base date is 1899-Dec-31 (the cell value 1 represents 1900-Jan-01)
    /// True = Base date is 1904-Jan-01 (the cell value 1 represents 1904-Jan-0)
    /// </summary>
    [ BiffRecordPos( 0, 0, TFieldType.Bit ) ]
    private bool m_bIs1904Windowing = false;
    #endregion

    #region Class properties
    /// <summary>
    /// Two bytes which are used for storing boolean value.
    /// </summary>
    public ushort Windowing
    {
      get
      {
        return m_usWindow;
      }
      set
      {
        m_usWindow = value;
      }
    }
    /// <summary>
    /// Boolean value specifying whether 1904 date windowing is used.
    /// </summary>
    public bool   Is1904Windowing
    {
      get
      {
        return m_bIs1904Windowing;
      }
      set
      {
        m_bIs1904Windowing = value;
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
    public  DateWindow1904Record()
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
    public  DateWindow1904Record( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }

    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  DateWindow1904Record( int iReserve )
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
      m_usWindow = provider.ReadUInt16( iOffset + 0 );
      m_bIs1904Windowing = provider.ReadBit( iOffset + 0, 0 );
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
      provider.WriteUInt16( iOffset + 0, m_usWindow );
      provider.WriteBit( iOffset + 0, m_bIs1904Windowing, 0 );
      m_iLength = ExcelConstants.ShortSize;
    }

    #endregion
  }
}
