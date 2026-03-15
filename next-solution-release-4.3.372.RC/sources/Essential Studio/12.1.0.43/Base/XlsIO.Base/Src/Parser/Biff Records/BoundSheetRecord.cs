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
using System.Text;

using Syncfusion.XlsIO;
using Syncfusion.XlsIO.Implementation.Exceptions;
using Syncfusion.XlsIO.Implementation.Security;
#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  ///<exclude/>
  /// <summary>
  /// Summary description for BoundSheetRecord.
  /// This record is located in the workbook globals area and represents a sheet inside
  /// of the workbook. A record is written for each sheet.  It stores the sheet name and
  /// a stream offset to the BOF record within the workbook stream.
  /// </summary>
  [ Biff( TBIFFRecord.BoundSheet ) ]
  //[ BiffOffsetsRecords( TBIFFRecord.BOF ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class BoundSheetRecord : BiffRecordRaw
  {
    #region Class constants
    /// <summary>
    /// Possible sheet types.
    /// </summary>
    public enum SheetType
    {
      /// <summary>
      /// Represents the Worksheet sheet type.
      /// </summary>
      Worksheet = 0,
      /// <summary>
      /// Represents the Chart sheet type.
      /// </summary>
      Chart = 2,
      /// <summary>
      /// Represents the VisualBasicModule sheet type.
      /// </summary>
      VisualBasicModule = 6
    }
    #endregion

    #region Class members
    /// <summary>
    /// Absolute stream position of the BOF record of the sheet represented by this record.
    /// </summary>
    [ BiffRecordPos( 0, 4, true ) ]
    private int     m_iBOFPosition = 1394;
    /// <summary>
    /// Options: Visibility and sheet type.
    /// </summary>
    [ BiffRecordPos( 4, 1, true ) ]
    private byte    m_Visibility = 0;
    /// <summary>
    /// Sheet type: 
    /// 0x00 = Worksheet
    /// 0x02 = Chart
    /// 0x06 = Visual Basic module
    /// </summary>
    [ BiffRecordPos( 5, 1, true ) ]
    private byte    m_SheetType = 0;
    /// <summary>
    /// Sheet name.
    /// </summary>
    [ BiffRecordPos( 6, TFieldType.String ) ]
    private string  m_strSheetName = "Sheet1";
    /// <summary>
    /// Helper field of record. Value is not stored as a record structure field.
    /// This index is used by the UpdateOffsets method for detecting the corresponding BOF
    /// field.
    /// </summary>
    private int     m_iSheetIndex = -1;
    /// <summary>
    /// BOF record that should be referenced.
    /// </summary>
    private BOFRecord m_bof;
    #endregion

    #region Class Properties
    /// <summary>
    /// Absolute stream position of the BOF record of the sheet represented by this record.
    /// </summary>
    public int      BOFPosition
    {
      get
      {
        return m_iBOFPosition;
      }
      set
      {
        m_iBOFPosition = value;
      }
    }

    /// <summary>
    /// Sheet name as a unicode string.
    /// </summary>
    public string   SheetName
    {
      get
      {
        return m_strSheetName;
      }
      set
      {
        m_strSheetName = value;
      }
    }
    /// <summary>
    /// Helper field of record. Value is not stored as a record structure field.
    /// This index is used by the UpdateOffsets method for detecting the corresponding BOF
    /// field.
    /// </summary>
    public int      SheetIndex
    {
      get
      {
        return m_iSheetIndex;
      }
      set
      {
        m_iSheetIndex = value;
      }
    }
    /// <summary>
    /// Sheet type.
    /// </summary>
    public SheetType BoundSheetType
    {
      get
      {
        return ( SheetType )m_SheetType;
      }
      set
      {
        m_SheetType = ( byte )value;
      }
    }
    /// <summary>
    /// Visibility type of Bound.
    /// </summary>
    public WorksheetVisibility Visibility
    {
      get
      {
        return ( WorksheetVisibility )m_Visibility;
      }
      set
      {
        m_Visibility = ( byte )value;
      }
    }
    /// <summary>
    /// Read-only. Returns minimum possible size of the record's
    /// internal data array.
    /// </summary>
    public override int MinimumRecordSize
    {
      get
      {
        return 8;
      }
    }
    /// <summary>
    /// Gets / sets BOF record that should be referenced.
    /// </summary>
    public BOFRecord BOF
    {
      get
      {
        return m_bof;
      }
      set
      {
        m_bof = value;
      }
    }

    /// <summary>
    /// Returns offset in the data array where encoded/decoded data should start. Read-only.
    /// </summary>
    public override int StartDecodingOffset
    {
      get
      {
        return 4;
      }
    }

    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor
    /// </summary>
    public  BoundSheetRecord()
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
    public  BoundSheetRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }

    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  BoundSheetRecord( int iReserve )
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
      m_iBOFPosition = provider.ReadInt32( iOffset + 0 );
      m_Visibility = provider.ReadByte( iOffset + 4 );
      m_SheetType = provider.ReadByte( iOffset + 5 );
      int iFullLength;
      m_strSheetName = provider.ReadString8Bit( iOffset + 6, out iFullLength );

      InternalDataIntegrityCheck();
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
      int iStartOffset = iOffset;
      provider.WriteInt32( iOffset + 0, m_iBOFPosition );
      provider.WriteByte( iOffset + 4, m_Visibility );
      provider.WriteByte( iOffset + 5, m_SheetType );
      iOffset += 6;
      provider.WriteString8BitUpdateOffset( ref iOffset, m_strSheetName );
      m_iLength = iOffset - iStartOffset;
    }

    /// <summary>
    /// This method checks the record's internal data array for integrity.
    /// </summary>
    /// <exception cref="Syncfusion.XlsIO.Implementation.Exceptions.WrongBiffRecordDataException">
    /// If there is any internal error.
    /// </exception>
    private void InternalDataIntegrityCheck()
    {
      //// First we should check if string's last byte is last byte of the record.
      //int rightLen = 8 + (( m_data[ 7 ] == 0 ) ?
      //  m_strSheetName.Length : m_strSheetName.Length * 2);

      //if( m_iLength != rightLen )
      //  throw new WrongBiffRecordDataException( "Wrong string length or record's data length." );
    }
    ///// <summary>
    ///// Method updates fields of record which must contain stream offset
    ///// or other data. This method must be called before save operation
    ///// when all records are placed in an array.
    ///// </summary>
    ///// <param name="records">Array with all records.</param>
    //public override void UpdateOffsets( List<IBiffStorage> records )
    //{
    //  if( SheetIndex < 0 )
    //    throw new ApplicationException( "SheetIndex property not set to corresponding Worksheet index." );

    //  int iPos = -1;

    //  for( int i = 0, len = records.Count; i < len; i++ )
    //  {
    //    BOFRecord bof = ( BOFRecord )records[ i ];

    //    if( bof.IsNested ) continue;

    //    if( bof.Type != BOFRecord.TType.TYPE_WORKBOOK ) iPos++;
        
    //    if( iPos == SheetIndex )
    //    {
    //      BOFPosition = ( int )bof.StreamPos;
    //      return;
    //    }
    //  }

    //  throw new ArgumentException( "SheetIndex property has bad value or not all sheets saved to storage." );
    //}
    /// <summary>
    /// 
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="encryptor">Object to encrypt data.</param>
    /// <param name="streamPosition">Position in the output stream. Used to increase performance.</param>
    /// <returns></returns>
    public override int FillStream( BinaryWriter writer, DataProvider provider,
      IEncryptor encryptor, int streamPosition )
    {
      m_iBOFPosition = ( int )m_bof.StreamPos;
      //SetInt32( 0, m_iBOFPosition );
      return base.FillStream( writer, provider, encryptor, streamPosition );
    }
    /// <summary>
    /// Size of the required storage space. Read-only.
    /// </summary>
    public override int GetStoreSize( ExcelVersion version )
    {
      return 8 + Encoding.Unicode.GetByteCount( m_strSheetName );
    }
    #endregion
  }
}
