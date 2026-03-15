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
using Syncfusion.XlsIO.Interfaces;

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  /// <summary>
  /// This record describes all unknown information in records.
  /// </summary>
  [ Biff( TBIFFRecord.Unknown ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class UnknownRecord
    : BiffRecordRawWithArray
    , ICloneable
  {
    #region Class static members
    /// <summary>
    /// 
    /// </summary>
    private static UnknownRecord _empty = new UnknownRecord();
    #endregion

    #region Class static properties
    /// <summary>
    /// 
    /// </summary>
    public static BiffRecordRaw Empty
    {
      get
      {
        return _empty;
      }
    }
    #endregion

    #region Class members
    /// <summary>
    /// 
    /// </summary>
    private byte[] m_tempData;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor
    /// </summary>
    public  UnknownRecord()
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
    public  UnknownRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }
    /// <summary>
    /// Read / initialize constructor.
    /// </summary>
    /// <param name="reader">Reader from which record data should be read.</param>
    /// <param name="itemSize">Size of read item.</param>
    /// <exception cref="System.ArgumentNullException">
    /// If stream is not specified.
    /// </exception>
    /// <exception cref="System.ApplicationException">
    /// If stream does not support read or seek operations.
    /// </exception>
    public UnknownRecord( BinaryReader reader, out int itemSize )
      : base()// reader, out itemSize )
    {
      m_iCode = reader.ReadInt16();
      m_iLength = reader.ReadInt16();
      m_data = new byte[ m_iLength ];

      reader.BaseStream.Read( m_data, 0, m_iLength );
      itemSize = m_iLength;
    }
    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// If amount of bytes requested is less than zero.
    /// </exception>
    public  UnknownRecord( int iReserve )
      : base( iReserve )
    {
    }
    #endregion

    #region Class properties
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

    /// <summary>
    /// 
    /// </summary>
    new public int RecordCode
    {
      get
      {
        return base.RecordCode;
      }
      set
      {
        m_iCode = value;
      }
    }
    /// <summary>
    /// Gets / sets length of the data array.
    /// </summary>
    public int DataLen
    {
      get
      {
        return m_iLength;
      }
      set
      {
        m_iLength = value;
      }
    }
    #endregion

    #region Record Serialization
    /// <summary>
    /// Parse structure of record. Converts data buffer to special
    /// values according to record specification.
    /// </summary>
    public override void ParseStructure()
    {
      //AutoExtractFields();
      m_tempData = new byte[ m_data.Length ];
      m_data.CopyTo( m_tempData, 0 );
    }
    /// <summary>
    /// In this method, a class must pack all of its properties into
    /// an internal data array, m_data. This method is called by
    /// FillStream, when the record must be serialized into a stream.
    /// </summary>
    public override void InfillInternalData( ExcelVersion version )
    {
//      m_iLength = m_tempData.Length;
//      SetBytes( 0, m_tempData, 0, m_tempData.Length );
    }
    /// <summary>
    /// Size of the required storage space. Read-only.
    /// </summary>
    public override int GetStoreSize( ExcelVersion version )
    {
      return m_iLength;
    }
    #endregion

    #region ICloneable Method
    /// <summary>
    /// Clones current instance.
    /// </summary>
    /// <returns>Returns cloned object.</returns>
    new public object Clone()
    { 
      UnknownRecord result = ( UnknownRecord )base.Clone();

      if( m_tempData != null )
      {
        result.m_tempData = CloneUtils.CloneByteArray( m_tempData );
      }

      return result;
    }
    #endregion
  }
}
