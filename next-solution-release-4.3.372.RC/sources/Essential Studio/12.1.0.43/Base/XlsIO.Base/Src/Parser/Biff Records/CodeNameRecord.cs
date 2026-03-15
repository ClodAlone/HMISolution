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
using Syncfusion.XlsIO.Implementation.Exceptions;

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  /// <summary>
  /// Summary description for CodeNameRecord.
  /// </summary>
  [Syncfusion.Documentation.DocumentationExclude()]
  [ Biff( TBIFFRecord.CodeName ) ]
  [ CLSCompliant( false ) ]
  public class CodeNameRecord : BiffRecordRawWithArray
  {
    #region Class members
    /// <summary>
    /// 
    /// </summary>
    private string m_strName;
    #endregion

    #region Class Properties
    /// <summary>
    /// 
    /// </summary>
    public string CodeName
    {
      get
      {
        return m_strName;
      }
      set
      {
        m_strName = value;
      }
    }

    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor
    /// </summary>
    public  CodeNameRecord()
      : base()
    {
    }

    /// <summary>
    /// Read / initialize constructor
    /// </summary>
    /// <param name="stream">Stream from which record data should be read.</param>
    /// <param name="itemSize">Size of read item.</param>
    /// <exception cref="System.ArgumentNullException">If stream is not specified.</exception>
    /// <exception cref="System.ApplicationException">If stream does not support read or seek operations.</exception>
    public  CodeNameRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }

    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  CodeNameRecord( int iReserve )
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
      if( this.Length > 0 )
      {
        AutoExtractFields();

        int iLen = GetUInt16( 0 );
        int iBytes;
        m_strName = GetString( 2, iLen, out iBytes );
        
        if( 3 + iBytes != Length )
        {
          throw new WrongBiffRecordDataException( "Wrong string or data length.");
        }
      }
    }

    /// <summary>
    /// In this method, the class must pack all of its properties into an
    /// internal data array, m_data. This method is called by
    /// FillStream when the record must be serialized into a stream.
    /// </summary>
    public override void InfillInternalData( ExcelVersion version )
    {
      m_iLength = GetStoreSize( ExcelVersion.Excel97to2003 );

      if( m_iLength > 0 )
      {
        m_data = new byte[ m_iLength ];
        SetString16BitLen( 0, m_strName );
      }
    }
    /// <summary>
    /// Size of the required storage space. Read-only.
    /// </summary>
    public override int GetStoreSize( ExcelVersion version )
    {
      int iLength = ( m_strName != null )
        ? m_strName.Length
        : 0;

      return ( iLength != 0 )
        ? 3 + m_strName.Length * 2
        : 0;
    }
    #endregion

  }
}
