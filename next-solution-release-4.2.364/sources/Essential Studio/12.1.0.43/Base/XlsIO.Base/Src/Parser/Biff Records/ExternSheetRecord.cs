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
using System.Collections.Generic;
using Syncfusion.XlsIO.Implementation.Exceptions;


namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  /// <summary>
  /// The record stores a list with indexes to SUPBOOK records.
  /// </summary>
  [ Biff( TBIFFRecord.ExternSheet ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class ExternSheetRecord : BiffRecordRawWithArray
  {
    #region Class consntants
    /// <summary>
    /// Size of the fixed part.
    /// </summary>
    private const int DEF_FIXED_PART_SIZE = 2;
    /// <summary>
    /// Maximum references in one record.
    /// </summary>
    public const int MaximumRefsCount = ( BiffRecordRaw.DEF_RECORD_MAX_SIZE - DEF_FIXED_PART_SIZE ) / TREF.DEF_TREF_SIZE;
    #endregion

    #region Internal classes
    /// <summary>
    /// Stores index to SUPBOOK record.
    /// </summary>
    public class TREF
    {
      #region Class constants
      /// <summary>
      /// Size of the TREF.
      /// </summary>
      public const int DEF_TREF_SIZE = 6;
      #endregion

      #region Class members
      /// <summary>
      /// Index to SUPBOOK record.
      /// </summary>
      private ushort m_usSupBookIndex;

      /// <summary>
      /// Index to first SUPBOOK sheet.
      /// </summary>
      private ushort m_usFirstSheet;

      /// <summary>
      /// Index to last SUPBOOK sheet.
      /// </summary>
      private ushort m_usLastSheet;
      #endregion

      #region Class properties
      /// <summary>
      /// Index to SUPBOOK record.
      /// </summary>
      public ushort SupBookIndex
      {
        get
        {
          return m_usSupBookIndex;
        }
        set
        {
          m_usSupBookIndex = value;
        }
      }

      /// <summary>
      /// Index to first SUPBOOK sheet.
      /// </summary>
      public ushort FirstSheet
      {
        get
        {
          return m_usFirstSheet;
        }
        set
        {
          m_usFirstSheet = value;
        }
      }

      /// <summary>
      /// Index to last SUPBOOK sheet.
      /// </summary>
      public ushort LastSheet
      {
        get
        {
          return m_usLastSheet;
        }
        set
        {
          m_usLastSheet = value;
        }
      }
      #endregion

      #region Class constructor
      /// <summary>
      /// Constructs reference by SUPBOOK index and its start and end sheet.
      /// </summary>
      /// <param name="supIndex">SUPBOOK index.</param>
      /// <param name="firstSheet">Index to first SUPBOOK sheet.</param>
      /// <param name="lastSheet">Index to last SUPBOOK sheet.</param>
      public TREF( int supIndex, int firstSheet, int lastSheet )
      {
        this.FirstSheet   = (ushort) firstSheet;
        this.LastSheet    = (ushort) lastSheet;
        this.SupBookIndex = (ushort) supIndex;
      }
      #endregion
    }
    #endregion

    #region Class members
    /// <summary>
    /// Number of following REF structures.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usRefCount = 0;
    /// <summary>
    /// List of REF structures.
    /// </summary>
    private List<TREF> m_arrRef;
    /// <summary>
    /// Represents the number of elements in rgXTI array
    /// </summary>
    private ushort m_cXTI;
    #endregion

    #region Class properties
    /// <summary>
    /// Number of following REF structures.
    /// </summary>
    public ushort RefCount
    {
      get
      {
        return m_usRefCount;
      }
      set
      {
        m_usRefCount = value;
      }
    }

    /// <summary>
    /// List of REF structures.
    /// </summary>
    public TREF[] Refs
    {
      get
      {
        return ( m_arrRef != null ) ? m_arrRef.ToArray() : null;
      }
      set
      {
        m_arrRef = new List<TREF>();
        m_arrRef.AddRange( value );
        m_usRefCount = ( ushort )m_arrRef.Count;
      }
    }
    /// <summary>
    /// Gets list of references.
    /// </summary>
    public List<TREF> RefList
    {
      get
      {
        return m_arrRef;
      }
    }

    /// <summary>
    /// Read-only. Minimum possible size of the record.
    /// </summary>
    public override int MinimumRecordSize
    {
      get
      {
        return 2;
      }
    }

    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor.
    /// </summary>
    public  ExternSheetRecord()
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
    public  ExternSheetRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }

    /// <summary>
    /// Reserves for record's internal data array iReserve bytes.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// If amount of bytes requested is less than zero.
    /// </exception>
    public  ExternSheetRecord( int iReserve )
      : base( iReserve )
    {
    }

    #endregion

    #region Class methods
    /// <summary>
    /// This method adds one TREF structure to the list.
    /// </summary>
    /// <param name="supIndex">SUPBOOK index.</param>
    /// <param name="firstSheet">Index to first SUPBOOK sheet.</param>
    /// <param name="lastSheet">Index to last SUPBOOK sheet.</param>
    /// <returns>
    /// Index of the old REF structure (if there was one)
    /// or new REF structure.
    /// </returns>
    public int AddReference( int supIndex, int firstSheet, int lastSheet )
    {
      int len;
      if( m_arrRef != null )
      {
        len = m_arrRef.Count;

        for( int i = 0; i < len; i++ )
        {
          TREF refer = m_arrRef[ i ];

          if( refer.SupBookIndex == supIndex && refer.FirstSheet == firstSheet
            && refer.LastSheet == lastSheet )
          {
            return i;
          }
        }
      }

      // reference was not found that we should create it
      TREF reference = new TREF( supIndex, firstSheet, lastSheet );
      AppendReference( reference );
      return m_usRefCount - 1;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="iBookIndex"></param>
    /// <returns></returns>
    public int GetBookReference( int iBookIndex )
    {
      for( int i = 0, len = m_arrRef.Count; i < len; i++ )
      {
        if( m_arrRef[ i ].SupBookIndex == iBookIndex )
        {
          return i;
        }
      }

      return -1;
    }
    public void AppendReference( TREF reference )
    {
      if( m_arrRef == null )
        m_arrRef = new List<TREF>();

      m_arrRef.Add( reference );
      m_usRefCount++;
    }
    public void AppendReferences( IList<TREF> refs )
    {
      m_arrRef.AddRange( refs );
      m_usRefCount += ( ushort )refs.Count;
    }
    public void PrependReferences( IList<TREF> refs )
    {
      m_arrRef.InsertRange( 0, refs );
      m_usRefCount += ( ushort )refs.Count;
    }
    public override object Clone()
    {
      ExternSheetRecord result = ( ExternSheetRecord )base.Clone();
      result.m_arrRef = CloneUtils.CloneCloneable( m_arrRef );
      return result;
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
      m_usRefCount = BitConverter.ToUInt16( m_data, 0 );
      if( m_iLength != m_usRefCount * 6 + 2 )
      {
        //throw new WrongBiffRecordDataException();
        //m_usRefCount = (ushort)((m_data.Length - 2) / 6); //Step to resize the reference count
      }

      m_arrRef = new List<TREF>( m_usRefCount );

      for( int i = 0, offset = 2; i < m_usRefCount; i++, offset += 6 )
      {
          if (offset >= m_data.Length)
          {
              m_cXTI = m_usRefCount;
              m_usRefCount = (ushort)i;
              break;
          }

        TREF reference = new TREF(
          GetUInt16( offset ),
          GetUInt16( offset + 2 ),
          GetUInt16( offset + 4 ) );

        m_arrRef.Add( reference );
      }
    }

    /// <summary>
    /// In this method, a class must pack all of its properties into an
    /// internal data array, m_data. This method is called by
    /// FillStream, when the record must be serialized into a stream.
    /// </summary>
    public override void InfillInternalData( ExcelVersion version )
    {
      m_iLength = GetStoreSize( ExcelVersion.Excel97to2003 );
      m_data = new byte[ m_iLength ];
      if (m_cXTI != 0)
          SetUInt16(0, m_cXTI);
      else
          SetUInt16(0, m_usRefCount);

      if( m_arrRef == null ) return;

      for( int i = 0, iOffset = DEF_FIXED_PART_SIZE, len = m_arrRef.Count; i < len; i++, iOffset += TREF.DEF_TREF_SIZE )
      {
        SetUInt16( iOffset    , m_arrRef[ i ].SupBookIndex );
        SetUInt16( iOffset + 2, m_arrRef[ i ].FirstSheet );
        SetUInt16( iOffset + 4, m_arrRef[ i ].LastSheet );
      }
    }

    /// <summary>
    /// Size of the required storage space. Read-only.
    /// </summary>
    public override int GetStoreSize( ExcelVersion version )
    {
      int iVariableSize = ( m_arrRef == null )
        ? 0
        : m_arrRef.Count * TREF.DEF_TREF_SIZE;

      return DEF_FIXED_PART_SIZE + iVariableSize;
    }
    #endregion
  }
}
