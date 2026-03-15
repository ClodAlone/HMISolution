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
using Syncfusion.XlsIO.Implementation.Exceptions;
using System.Collections.Generic;
#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  /// <summary>
  /// This record stores the URL of an external document
  /// and a list of sheet names inside this document. Furthermore,
  /// it is used to store DDE and OLE object links or to indicate
  /// an internal 3D reference or an add-in function.
  /// </summary>
  [ Biff( TBIFFRecord.SupBook ) ]
  [ CLSCompliant( false ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  public class SupBookRecord : BiffRecordWithContinue
  {
    #region Class constants
    /// <summary>
    /// This constant indicates that the sup book record contains internal references.
    /// </summary>
    public const ushort INTERNAL_REFERENCE = 0x0401;
    /// <summary>
    /// This constant indicates that the sup book record contains add-in functions.
    /// </summary>
    public const ushort ADDIN_FUNCTION = 0x3A01;
    #endregion

    #region Class members
    /// <summary>
    /// Indicates whether this record is used for internal references or external references.
    /// </summary>
    bool m_bIsInternal;
    /// <summary>
    /// Indicates whether add-in function names are stored 
    /// in EXTERNNAME records following this SUPBOOK record.
    /// </summary>
    bool m_bIsAddInFunction;
    /// <summary>
    /// Number of sheet names (if external references) or
    /// number of sheets in this document (if internal references).
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usSheetNumber = 0;
    /// <summary>
    /// Length of encoded URL without sheet name (if external references).
    /// 0401h (if internal references)
    /// </summary>
    [ BiffRecordPos( 2, 2 ) ]
    private ushort m_usUrlLength = 0;

    /// <summary>
    /// Encoded URL without sheet name.
    /// </summary>
    private string m_strUrl = null;

    /// <summary>
    /// List of sheet names.
    /// </summary>
    private List<string> m_arrSheetNames;
    /// <summary>
    /// Original URL value.
    /// </summary>
    private string m_strOriginalURL;
    #endregion

    #region Class properties
    /// <summary>
    /// Whether the record is used for internal references or external references.
    /// </summary>
    public bool IsInternalReference
    {
      get
      {
        return m_bIsInternal;
      }
      set
      {
        m_bIsInternal = value;
      }
    }

    /// <summary>
    /// Read-only. Minimum possible size of the record.
    /// </summary>
    public override int MinimumRecordSize
    {
      get
      {
        return 4;
      }
    }

    /// <summary>
    /// Read-only. Maximum possible size of the record.
    /// </summary>
    public override int MaximumRecordSize
    {
      get
      {
        if( this.m_bIsInternal ) return 4;
        return base.MaximumRecordSize;
      }
    }

    /// <summary>
    /// Encoded URL without sheet name (for external references).
    /// </summary>
    public string URL
    {
      get
      {
        return m_strUrl;
      }
      set
      {
        m_strUrl = value;
        m_usUrlLength = ( m_strUrl != null ) ? ( ushort ) value.Length : ( ushort ) 0;
      }
    }
    /// <summary>
    /// Gets / sets original url value.
    /// </summary>
    public string OriginalURL
    {
      get
      {
        return m_strOriginalURL;
      }
      set
      {
        m_strOriginalURL = value;
      }
    }

    /// <summary>
    /// List of sheet names.
    /// </summary>
    public List<string> SheetNames
    {
      get
      {
        return m_arrSheetNames;
      }
      set
      {
        m_arrSheetNames = value;
        //m_usSheetNumber = ( ushort )( ( m_arrSheetNames != null ) ? m_arrSheetNames.Count : 0 );
      }
    }

    /// <summary>
    /// Number of sheet names (if external references) or
    /// number of sheets in this document (if internal references).
    /// </summary>
    public ushort SheetNumber
    {
      get
      {
        if( !IsInternalReference )
        {
          m_usSheetNumber = ( m_arrSheetNames != null ) ?
            ( ushort )m_arrSheetNames.Count : ( ushort )0;
        }

        return m_usSheetNumber;
      }
      set
      {
        m_usSheetNumber = value;
        if( !IsInternalReference )
        {
          m_usSheetNumber = ( m_arrSheetNames != null ) ?
            ( ushort )m_arrSheetNames.Count : ( ushort )0;
        }
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public bool IsAddInFunctions
    {
      get
      {
        return m_bIsAddInFunction;
      }
      set
      {
        m_bIsAddInFunction = value;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor
    /// </summary>
    public  SupBookRecord()
      : base()
    {
    }

    #endregion

    #region Record Serialization
    /// <summary>
    /// Parse structure of record. Converts data buffer to special
    /// values according to record specification.
    /// </summary>
    /// <exception cref="Syncfusion.XlsIO.Implementation.Exceptions.WrongBiffRecordDataException">
    /// When string's length does not fit to internal data length or
    /// when last string ends before data (some extra data at the
    /// end of m_data array).
    /// </exception>
    public override void ParseStructure()
    {
      m_usSheetNumber = m_provider.ReadUInt16( 0 );
      m_usUrlLength = m_provider.ReadUInt16( 2 );

      m_bIsInternal = ( m_iLength == 4 && m_usUrlLength == INTERNAL_REFERENCE );
      if( m_bIsInternal ) return;

      if( ( m_bIsAddInFunction = ( m_iLength == 4 && m_usUrlLength == ADDIN_FUNCTION ) ) )
        return;

      int offset = 2;
      int iStringLength;
      m_strOriginalURL = m_strUrl = m_provider.ReadString16Bit( offset, out iStringLength );//GetString16BitUpdateOffset( ref offset );
      offset += iStringLength;

      //System.Diagnostics.Debug.WriteLineIf( Syncfusion.XlsIO.Implementation.ApplicationImpl.IsDebugInfoEnabled,
      //  m_strUrl, "Supbook url" );

      m_arrSheetNames = new List<string>( m_usSheetNumber );

      for( int i = 0; i < m_usSheetNumber; i++ )
      {
        //System.Diagnostics.Debug.WriteLine( offset, "Offset" );
        string strSheetName = m_provider.ReadString16BitUpdateOffset( ref offset );
        m_arrSheetNames.Add( strSheetName );

        if( offset > m_iLength )
        {
          throw new WrongBiffRecordDataException( );
        }
      }

      if( offset != m_iLength )
      {
        throw new WrongBiffRecordDataException( );
      }
    }

    /// <summary>
    /// In this method, a class must pack all of its properties into
    /// an internal data array, m_data. This method is called by
    /// FillStream, when the record must be serialized into a stream.
    /// </summary>
    public override void InfillInternalData( ExcelVersion version )
    {
      PrognoseRecordSize();
      m_arrContinuePos.Clear();

      if( m_bIsInternal )
      {
        m_usUrlLength = INTERNAL_REFERENCE;
      }
      else if( m_bIsAddInFunction )
      {
        m_usUrlLength = ADDIN_FUNCTION;
      }
      else if( m_strOriginalURL != null )
      {
        m_usUrlLength = ( ushort )m_strOriginalURL.Length;
      }

      m_iLength = 0;
      IntPtrContinueRecordBuilder builder = new IntPtrContinueRecordBuilder( this, DEF_HEADER_SIZE );
      //m_usSheetNumber = ( ushort )( ( m_arrSheetNames != null ) ? m_arrSheetNames.Count : 0 );
      builder.AppendUInt16( SheetNumber );
      builder.AppendUInt16( m_usUrlLength );

      if( !m_bIsInternal && !m_bIsAddInFunction )
      {
        byte[] arrBuffer = null;
        int iFullLength = m_usUrlLength * 2 + 3;
        SSTRecord.EnsureSize( ref arrBuffer, iFullLength );
        int iOffset = 0;

        string strUrl = ( m_strOriginalURL != null ) ? m_strOriginalURL : m_strUrl;
        BiffRecordRaw.SetString16BitUpdateOffset( arrBuffer, ref iOffset, strUrl );
        builder.AppendBytes( arrBuffer, 2, iFullLength - 2 );

        if( m_arrSheetNames != null )
        {
          for( int i = 0, len = m_arrSheetNames.Count; i < len; i++ )
          {
            string strValue = m_arrSheetNames[ i ];
            int iLength = strValue.Length;
            iFullLength = iLength * 2 + 3;

            SSTRecord.EnsureSize( ref arrBuffer, iFullLength );
            iOffset = 0;
            BiffRecordRaw.SetString16BitUpdateOffset( arrBuffer, ref iOffset, strValue );

            if( builder.FreeSpace < iFullLength )
            {
              builder.StartContinueRecord();
            }

            builder.AppendBytes( arrBuffer, 0, iFullLength );
          }
        }
      }

      m_iLength = builder.Total;
      m_iFirstLength = builder.FirstRecordLength;
      builder.Dispose();
      builder = null;
    }

    /// <summary>
    /// 
    /// </summary>
    private void PrognoseRecordSize()
    {
      int iSize = 4;

      if( !m_bIsInternal && !m_bIsAddInFunction )
      {
        iSize += 3 + m_usUrlLength * 2;
        int iPartSize = iSize;

        if( m_arrSheetNames != null )
        {
          for( int i = 0, len = m_arrSheetNames.Count; i < len; i++ )
          {
            string value = m_arrSheetNames[ i ];
            int iStringSize = value.Length * 2 + 3;

            if( iPartSize + iStringSize > BiffRecordRaw.DEF_RECORD_MAX_SIZE )
            {
              iSize += BiffRecordRaw.DEF_HEADER_SIZE;
              iPartSize = 0;
            }

            iSize += iStringSize;
            iPartSize += iStringSize;
          }
        }
      }

      m_provider.EnsureCapacity( iSize );
      //throw new NotImplementedException();
    }
    /// <summary>
    /// Size of the required storage space. Read-only.
    /// </summary>
    public override int GetStoreSize( ExcelVersion version )
    {
      if( NeedInfill )
      {
        InfillInternalData( version );
        NeedInfill = false;
      }

      return m_iLength;
    }
    #endregion
  }
}
