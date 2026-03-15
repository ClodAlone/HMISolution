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
using System.Text;


namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  /// <summary>
  /// The NOTE record specifies a comment associated with a particular cell.
  /// </summary>
  [ Biff( TBIFFRecord.Note ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class NoteRecord : BiffRecordRaw
  {
    #region Class constants
    /// <summary>
    /// Size of the fixed part of the record.
    /// </summary>
    private const int DEF_FIXED_PART_SIZE = 10;
    #endregion

    #region Class members
    /// <summary>
    /// Row of the comment.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usRow;

    /// <summary>
    /// Column of the comment.
    /// </summary>
    [ BiffRecordPos( 2, 2 ) ]
    private ushort m_usColumn;

    /// <summary>
    /// Options flag.
    /// </summary>
    [ BiffRecordPos( 4, 2 ) ]
    private ushort m_usOptions = 0;

    /// <summary>
    /// Whether the comment is visible.
    /// </summary>
    [ BiffRecordPos( 4, 1, TFieldType.Bit ) ]
    private bool m_bShow = false;

    /// <summary>
    /// Object ID for OBJ record that contains the comment.
    /// </summary>
    [ BiffRecordPos( 6, 2 ) ]
    private ushort m_usObjId = 0;

    /// <summary>
    /// Length of the name of the original comment author.
    /// </summary>
    [ BiffRecordPos( 8, 2 ) ]
    private ushort m_usAuthorNameLen = 0;

    /// <summary>
    /// Name of the original comment author.
    /// </summary>
    private string m_strAuthorName = string.Empty;
    #endregion

    #region Class Properties
    /// <summary>
    /// Index to row.
    /// </summary>
    public ushort Row
    {
      get
      {
        return m_usRow;
      }
      set
      {
        m_usRow = value;
      }
    }

    /// <summary>
    /// Index to column.
    /// </summary>
    public ushort Column
    {
      get
      {
        return m_usColumn;
      }
      set
      {
        m_usColumn = value;
      }
    }

    /// <summary>
    /// Name of the original comment author.
    /// </summary>
    public string AuthorName
    {
      get
      {
        return m_strAuthorName;
      }
      set
      {
        m_strAuthorName = value;
        m_usAuthorNameLen = ( value != null )
          ? ( ushort )m_strAuthorName.Length
          : ( ushort ) 0;
      }
    }

    /// <summary>
    /// Object ID for OBJ record that contains the comment.
    /// </summary>
    public ushort ObjId
    {
      get
      {
        return m_usObjId;
      }
      set
      {
        m_usObjId = value;
      }
    }
    /// <summary>
    /// Indicates whether the comment is visible.
    /// Changes one bit in m_usOptions field.
    /// </summary>
    public bool   IsVisible
    {
      get
      {
        return m_bShow;
      }
      set
      {
        m_bShow = value;
      }
    }
    /// <summary>
    /// Read-only. Reserved.
    /// </summary>
    public ushort Reserved
    {
      get
      {
        return m_usOptions;
      }
    }
    /// <summary>
    /// Read-only. Returns minimum possible size of record's
    /// internal data array.
    /// </summary>
    public override int MinimumRecordSize
    {
      get
      {
        return 8;
      }
    }

    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor fills all data with default values.
    /// </summary>
    public  NoteRecord()
      : base()
    {
      m_bShow = false;
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
    public  NoteRecord( Stream stream, out int itemSize )
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
    public  NoteRecord( int iReserve )
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
      m_usRow = provider.ReadUInt16( iOffset + 0 );
      m_usColumn = provider.ReadUInt16( iOffset + 2 );
      m_usOptions = provider.ReadUInt16( iOffset + 4 );
      m_bShow = provider.ReadBit( iOffset + 4, 1 );
      m_usObjId = provider.ReadUInt16( iOffset + 6 );
      m_usAuthorNameLen = provider.ReadUInt16( iOffset + 8 );

      int iBytes;

      if( m_usAuthorNameLen > 0 )
        m_strAuthorName = provider.ReadString( 10, m_usAuthorNameLen, out iBytes, false );

      //if( iBytes + 12 != m_iLength )
      //  throw new WrongBiffRecordDataException( "String and m_data array do not fit each other." );
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
      int iStoreSize = GetStoreSize( ExcelVersion.Excel97to2003 );
      m_iLength = 0;

      int iStartOffset = iOffset;
      provider.WriteUInt16( iOffset + 0, m_usRow );
      provider.WriteUInt16( iOffset + 2, m_usColumn );
      provider.WriteUInt16( iOffset + 4, m_usOptions );
      provider.WriteBit( iOffset + 4, m_bShow, 1 );
      provider.WriteUInt16( iOffset + 6, m_usObjId );
      provider.WriteUInt16( iOffset + 8, m_usAuthorNameLen );
      iOffset += 10;

      if( m_usAuthorNameLen > 0 )
      {
        provider.WriteStringNoLenUpdateOffset( ref iOffset, m_strAuthorName );
        m_iLength = iOffset - iStartOffset;
      
        if( m_iLength % 2 != 0 )
        {
          provider.WriteByte( iOffset, 0 );
          m_iLength++;
        }
      }
      else
      {
        provider.WriteByte( iOffset++, 0 );
        provider.WriteByte( iOffset++, 0 );
        m_iLength = iOffset - iStartOffset;
      }

      //System.Diagnostics.Debug.WriteLineIf( Syncfusion.XlsIO.Implementation.ApplicationImpl.IsDebugInfoEnabled,
      //  m_iLength, "Note data len" );
    }

    /// <summary>
    /// Size of the required storage space. Read-only.
    /// </summary>
    public override int GetStoreSize( ExcelVersion version )
    {
      int iStringLen;
        
      if( m_usAuthorNameLen > 0 )
      {
        iStringLen = Encoding.Unicode.GetByteCount( m_strAuthorName ) + 1;

        if( iStringLen % 2 != 0 ) iStringLen++;
      }
      else
      {
        iStringLen = 2;
      }

      return DEF_FIXED_PART_SIZE + iStringLen;
    }
    #endregion
  }
}
