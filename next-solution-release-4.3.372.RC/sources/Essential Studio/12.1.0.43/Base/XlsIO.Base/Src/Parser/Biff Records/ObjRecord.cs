#region Copyright

//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.

#endregion Copyright

using System;
using System.Collections;
using System.IO;

using Syncfusion.XlsIO.Implementation;
using Syncfusion.XlsIO.Parser.Biff_Records.ObjRecords;
using System.Collections.Generic;
using Syncfusion.XlsIO.Interfaces;

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  /// <summary>
  /// The OBJ record contains a partial description of a drawing object
  /// and the MSODRAWING, MSODRAWINGGROUP, and MSODRAWINGSELECTION records contain
  /// the remaining drawing object data.
  /// </summary>
  [Syncfusion.Documentation.DocumentationExclude()]
  [ Biff( TBIFFRecord.OBJ ) ]
  [ CLSCompliant( false ) ]
  public class OBJRecord : BiffRecordRaw, ICloneable
  {
    #region Class constants
    #endregion

    #region Class members
    /// <summary>
    /// Array that contain all subrecords.
    /// </summary>
    private List<ObjSubRecord> m_records = new List<ObjSubRecord>();
    #endregion

    #region Class Properties
    /// <summary>
    /// Read-only. Returns array of subrecords.
    /// </summary>
    public ObjSubRecord[] Records
    {
      get
      {
        return m_records.ToArray();
      }
    }
    /// <summary>
    /// Read-only. Returns array of subrecords.
    /// </summary>
    public List<ObjSubRecord> RecordsList
    {
      get
      {
        return m_records;
      }
    }
    /// <summary>
    /// Indicates whether this record needs a data array after parsing
    /// is complete. Read-only.
    /// </summary>
    public override bool NeedDataArray
    {
      get
      {
        return true;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor fills all data with default values.
    /// </summary>
    public  OBJRecord()
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
    public  OBJRecord( Stream stream, out int itemSize )
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
    public  OBJRecord( int iReserve )
      : base( iReserve )
    {
    }

    #endregion

    #region Record Serialization
    /// <summary>
    /// Parse structure of record. Convert Data buffer to special
    /// values according to record specification.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">Offset to the record's data.</param>
    /// <param name="iLength">Length of the record's data.</param>
    /// <param name="version">Excel version used for infill.</param>
    public override void ParseStructure( DataProvider provider, int iOffset, int iLength, ExcelVersion version )
    {
      int iStartOffset = iOffset;
      int iFinalOffset = iOffset + m_iLength;
      TObjType objectType = ( TObjType )0;
      do
      {
        ObjSubRecord item = GetSubRecord( provider, iOffset, iStartOffset, objectType );
        m_records.Add( item );

        if( item.Type == TObjSubRecordType.ftCmo )
        {
          ftCmo cmo = ( ftCmo )item;
          objectType = cmo.ObjectType;
        }

        iOffset += item.Length + 4;
      }
      while( iOffset < iFinalOffset );
    }

    /// <summary>
    /// In this method, a class must pack all of its properties into
    /// an internal data array, m_data. This method is called by
    /// FillStream, when the record must be serialized into a stream.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">Offset in the buffer.</param>
    /// <param name="version">Excel version used for infill.</param>
    public override void InfillInternalData( DataProvider provider, int iOffset, ExcelVersion version )
    {
      m_iLength = 0;
      //m_data = new byte[ StoreSize ];

      for( int i = 0, len = m_records.Count; i < len; i++ )
      {
        // Use 'as' to increase performance.
        ObjSubRecord record = m_records[ i ] as ObjSubRecord;
        record.FillArray( provider, iOffset );
        int iStoreSize = record.GetStoreSize( version );
        m_iLength += iStoreSize;
        iOffset += iStoreSize;
      }
    }

    /// <summary>
    /// Size of the required storage space. Read-only.
    /// </summary>
    public override int GetStoreSize( ExcelVersion version )
    {
      int iResult = 0;

      for( int i = 0, len = m_records.Count; i < len; i++ )
      {
        // Use 'as' to increase performance.
        ObjSubRecord record = m_records[ i ] as ObjSubRecord;
        iResult += record.GetStoreSize( version );
      }

      return iResult;
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// Get subrecord by its offset.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="offset">Offset to the subrecord.</param>
    /// <param name="iStartOffset">Start offset of the OBJRecord.</param>
    /// <returns>Parsed subrecord from internal data array.</returns>
    protected ObjSubRecord GetSubRecord( DataProvider provider, int offset, int iStartOffset, TObjType objectType )
    {
      TObjSubRecordType type = ( TObjSubRecordType )provider.ReadInt16( offset );
      ushort length = provider.ReadUInt16( offset + 2 );

      if( type == TObjSubRecordType.ftLbsData || length + offset + 4 > Length )
      {
        // first (-4) - size of the subrecord header.
        int iDecrease = 4;

        // second (-4) - size of the ftEnd record.
        int iEndOffset = Length - 4;

        if( provider.ReadInt16( iEndOffset ) == 0 )
          iDecrease += 4;

        length = ( ushort )( Length - offset - iDecrease + iStartOffset );
      }

      // This was added to fix defect 1811
      if( type == TObjSubRecordType.ftEnd ) length = 0;

      byte[] buffer = new byte[ length ];
      provider.ReadArray( offset + 4, buffer );

      switch( type )
      {
        case TObjSubRecordType.ftCmo:
          return new ftCmo( type, length, buffer );

        case TObjSubRecordType.ftEnd:
          return new ftEnd( type, length, buffer );

        case TObjSubRecordType.ftNts:
          return new ftNts( type, length, buffer );

        //case TObjSubRecordType.ftPictFmla:
        //  return new ftPictFmla( type, length, buffer );

        case TObjSubRecordType.ftSbs:
          return new ftSbs( type, length, buffer );

        case TObjSubRecordType.ftSbsFormula:
          return new ftSbsFormula( type, length, buffer );

        case TObjSubRecordType.ftLbsData:
          return new ftLbsData( type, length, buffer, objectType );

        case TObjSubRecordType.ftCbls:
          return new ftCbls( type, length, buffer );

        case TObjSubRecordType.ftCblsData:
          return new ftCblsData( type, length, buffer );

        case TObjSubRecordType.ftCblsFmla:
          return new ftCblsFmla(type, length, buffer);

        case TObjSubRecordType.ftMacro:
          return new ftMacro( length, buffer );

        case TObjSubRecordType.ftRbo:
          return new ftRbo( length, buffer );

        case TObjSubRecordType.ftRboData:
          return new ftRboData( length, buffer );

        case TObjSubRecordType.ftCf:
          return new ftCf(TObjSubRecordType.ftCf, length, buffer);
              
          case TObjSubRecordType.ftPioGrbit:
          return new ftPioGrbit(TObjSubRecordType.ftPioGrbit, length, buffer);

        default:
          return new ftUnknown( type, length, buffer );
      }
    }
    /// <summary>
    /// Adds new subrecord.
    /// </summary>
    /// <param name="record">Record to add.</param>
    public void AddSubRecord( ObjSubRecord record )
    {
      m_records.Add( record );
    }
    /// <summary>
    /// Searches for the subrecord of the specified type.
    /// </summary>
    /// <param name="recordType">Record type to search for.</param>
    /// <returns>Found subrecord or null if not found.</returns>
    public ObjSubRecord FindSubRecord( TObjSubRecordType recordType )
    {
      int index = FindSubRecordIndex( recordType );

      return ( index >= 0 ) ?
        m_records[ index ] :
        null;
    }
    /// <summary>
    /// Searches for the index of the subrecord with the specified type.
    /// </summary>
    /// <param name="recordType">Record type to search for.</param>
    /// <returns>Index of the found subrecord or -1 if not found.</returns>
    public int FindSubRecordIndex( TObjSubRecordType recordType )
    {
      int result = -1;

      for( int i = 0, len = m_records.Count; i < len; i++ )
      {
        ObjSubRecord current = m_records[ i ];

        if( current.Type == recordType )
        {
          result = i;
          break;
        }
      }

      return result;
    }
    #endregion

    #region ICloneable members
    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>
    /// A new object that is a copy of this instance.</returns>
    new public object Clone()
    {
      OBJRecord result = ( OBJRecord )base.Clone();

      result.m_records = CloneUtils.CloneCloneable( m_records );

      return result;
    }
    #endregion
  }
}
