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
  /// Summary description for BookBoolRecord.
  /// Saves External Links record.
  /// Contains a flag specifying whether the GUI should save externally
  /// linked values from other workbooks.
  /// </summary>
  [ Biff( TBIFFRecord.BookBool ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class BookBoolRecord : BiffRecordRaw
  {
    #region Class members
    /// <summary>
    /// 0 = Save external linked values. 1 = Do not save external linked values.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usSaveLinkValue = 0;
    #endregion

    #region Class properties
    /// <summary>
    ///If zero, then save external linked values.
    /// </summary>
    public ushort SaveLinkValue
    {
      get
      {
        return m_usSaveLinkValue;
      }
      set
      {
        m_usSaveLinkValue = value;
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
    /// Read-only. Returns maximum possible size of record's internal
    /// data array.
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
    /// Default Constructor
    /// </summary>
    public  BookBoolRecord()
      : base()
    {
    }

    /// <summary>
    /// Read / initialize Constructor.
    /// </summary>
    /// <param name="stream">Stream from which record data should be read.</param>
    /// <param name="itemSize">Size of read item.</param>
    /// <exception cref="System.ArgumentNullException">If stream is not specified.</exception>
    /// <exception cref="System.ApplicationException">If stream does not support read or seek operations.</exception>
    public  BookBoolRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }

    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  BookBoolRecord( int iReserve )
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
      m_usSaveLinkValue = provider.ReadUInt16( iOffset + 0 );
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
      provider.WriteUInt16( iOffset + 0, m_usSaveLinkValue );
      m_iLength = ExcelConstants.ShortSize;
    }

    #endregion
  }
}
