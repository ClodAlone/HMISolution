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
using System.Text;
#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records.PivotTable
{
  /// <summary>
  /// This record contains an SQL query string, an SQL server connection string
  /// or page item name from a multiple-consolidation PivotTable.
  /// </summary>
  [ Biff( TBIFFRecord.PivotString ) ]
  [ CLSCompliant( false ) ]
  public class PivotStringRecord
    : BiffRecordRaw
    , IValueHolder
  {
    #region Class members
    /// <summary>
    /// String.
    /// </summary>
    [ BiffRecordPos( 0, TFieldType.String16Bit ) ]
    private string m_strString;
    ///// <summary>
    ///// Indicates whether record was changed since parsing. If not then we will
    ///// use original data because it can modify some offset and make resulting
    ///// file unreadable for MS Excel.
    ///// </summary>
    //private bool m_bChanged = true;
    private bool m_b8Bit = false;
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor.
    /// </summary>
    public  PivotStringRecord()
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
    public  PivotStringRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }

    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for the data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  PivotStringRecord( int iReserve )
      : base( iReserve )
    {
    }

    #endregion

    #region Class properties
    /// <summary>
    /// String value.
    /// </summary>
    public string String
    {
      get
      {
        return m_strString;
      }
      set
      {
        if( value == null )
          throw new ArgumentNullException( "value" );

        m_strString = value;
        m_b8Bit = BiffRecordRawWithArray.IsAsciiString( value );
      }
    }
    /// <summary>
    /// Indicates whether data array is required by this record. Read-only.
    /// </summary>
    public override bool NeedDataArray
    {
      get
      {
        return true;
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
      int iFullLength;
      m_strString = provider.ReadString16Bit( iOffset + 0, out iFullLength );

      if( m_strString.Length * 2 + 3 < iFullLength )
        m_b8Bit = true;
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
      if( m_b8Bit )
      {
        int iStartOffset = iOffset;
        //provider.WriteString8BitUpdateOffset( ref iOffset, m_strString );
        provider.WriteString16BitUpdateOffset( ref iOffset, m_strString, false );
        m_iLength = iOffset - iStartOffset;
      }
      else
      {
        m_iLength = provider.WriteString16Bit( iOffset + 0, m_strString );
      }
    }
    /// <summary>
    /// Evaluates size of the required storage space.
    /// </summary>
    /// <returns>Size of the required storage space.</returns>
    public override int GetStoreSize( ExcelVersion version )
    {
      return 3 + ( ( m_b8Bit ) ?
        m_strString.Length :
        Encoding.Unicode.GetByteCount( m_strString ) );
    }
    #endregion

    #region IValueHolder Members
    /// <summary>
    /// Value of the record.
    /// </summary>
    object IValueHolder.Value
    {
      get
      {
        return String;
      }
      set
      {
        String = ( string )value;
      }
    }

    #endregion
  }
}
