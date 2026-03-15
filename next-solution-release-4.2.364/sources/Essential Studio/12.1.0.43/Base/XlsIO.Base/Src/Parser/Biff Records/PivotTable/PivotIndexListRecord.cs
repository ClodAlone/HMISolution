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
  /// Summary description for PivotIndexListRecord.
  /// </summary>
  [ Biff( TBIFFRecord.PivotIndexList ) ]
  [ CLSCompliant( false ) ]
  public class PivotIndexListRecord : BiffRecordRawWithArray
  {
    #region Class members
    /// <summary>
    /// Index list.
    /// </summary>
    private byte[] m_arrIndexes;
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor.
    /// </summary>
    public  PivotIndexListRecord()
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
    public  PivotIndexListRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }

    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for the data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  PivotIndexListRecord( int iReserve )
      : base( iReserve )
    {
    }

    #endregion

    #region Class properties
    /// <summary>
    /// Index list.
    /// </summary>
    public byte[] Indexes
    {
      get
      {
        return m_arrIndexes;
      }
      set
      {
        if( value == null )
          throw new ArgumentNullException( "value" );

        m_arrIndexes = value;
      }
    }
    #endregion

    #region Record Serialization
    /// <summary>
    /// Parse structure of record. Convert Data buffer to special
    /// values according to record specification.
    /// </summary>
    public override void ParseStructure()
    {
      m_arrIndexes = new byte[ m_iLength ];
      Array.Copy( m_data, 0, m_arrIndexes, 0, m_iLength );
    }

    /// <summary>
    /// In this method, the class must pack all of its properties into an
    /// internal data array, m_data. This method is called by
    /// FillStream when the record must be serialized into a stream.
    /// </summary>
    public override void InfillInternalData( ExcelVersion version )
    {
      bool bAutoGrow = AutoGrowData;
      AutoGrowData = true;

      m_iLength = m_arrIndexes.Length;
      SetBytes( 0, m_arrIndexes, 0, m_iLength );

      AutoGrowData = bAutoGrow;
    }

    #endregion

  }
}
