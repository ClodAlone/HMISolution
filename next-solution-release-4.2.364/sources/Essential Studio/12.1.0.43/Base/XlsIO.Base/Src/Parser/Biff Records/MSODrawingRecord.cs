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
using System.Collections;

using Syncfusion.XlsIO.Parser.Biff_Records.MsoDrawing;
using Syncfusion.XlsIO.Interfaces;
#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records
{

  /// <summary>
  /// This record contains a drawing object provided by the Microsoft
  /// Office Drawing tool.
  /// </summary>
  [ Biff( TBIFFRecord.MSODrawing ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class MSODrawingRecord
    : BiffRecordRawWithArray
    , ICloneable
    , ILengthSetter
  {
    #region Class Properties
    /// <summary>
    /// 
    /// </summary>
    public int RecordLength
    {
      get
      {
        return Length;
      }
      set
      {
        if( value < 0 && value > m_data.Length )
          throw new ArgumentOutOfRangeException( "RecordLength" );

        m_iLength = value;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor fills all data with default values.
    /// </summary>
    public  MSODrawingRecord()
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
    public  MSODrawingRecord( Stream stream, out int itemSize )
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
    public  MSODrawingRecord( int iReserve )
      : base( iReserve )
    {
    }

    #endregion

    #region Record Serialization
    /// <summary>
    /// Parse structure of record. Converts data buffer to special
    /// values according to record specification.
    /// </summary>
    public override void ParseStructure()
    {
    }

    /// <summary>
    /// In this method, a class must pack all of its properties into
    /// an internal data array, m_data. This method is called by
    /// FillStream, when the record must be serialized into a stream.
    /// </summary>
    public override void InfillInternalData( ExcelVersion version )
    {
      m_iLength = m_data.Length;
    }

    /// <summary>
    /// 
    /// </summary>
    public override bool NeedDataArray
    {
      get
      {
        return true;
      }
    }

    #endregion

    #region Class methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="length"></param>
    /// <param name="data"></param>
    public void SetData( int length, byte[] data )
    {
      if( length < 0 || data.Length < length )
        throw new ArgumentOutOfRangeException( "length" );

      m_data = new byte[ length ];
      Array.Copy( data, 0, m_data, 0, length );
    }
    /// <summary>
    /// Sets length of the internal data.
    /// </summary>
    /// <param name="iLength">Length to set.</param>
    public void SetLength( int iLength )
    {
      m_iLength = iLength;
    }
    /// <summary>
    /// Size of the required storage space. Read-only.
    /// </summary>
    public override int GetStoreSize( ExcelVersion version )
    {
      return m_data.Length;
    }
    #endregion
  }
}
