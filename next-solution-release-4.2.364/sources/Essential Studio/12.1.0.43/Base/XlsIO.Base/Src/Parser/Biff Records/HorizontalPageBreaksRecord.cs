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
using Syncfusion.XlsIO.Interfaces;

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  /// <summary>
  /// Contains a list of explicit column page breaks.
  /// </summary>
  [ Biff( TBIFFRecord.HorizontalPageBreaks ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class HorizontalPageBreaksRecord : BiffRecordRawWithArray
  {
    #region Class constants
    /// <summary>
    /// Size of the fixed part.
    /// </summary>
    private const int DEF_FIXED_PART_SIZE = 2;
    /// <summary>
    /// Size of the subitem.
    /// </summary>
    internal const int DEF_SUBITEM_SIZE = 6;
    /// <summary>
    /// Size of the fixed part.
    /// </summary>
    internal const int FixedSize = 2;
    #endregion

    #region Internal classes
    /// <summary>
    /// This class contains information about single page break.
    /// </summary>
    public class THPageBreak : ICloneable
    {
      #region THPageBreak members
      /// <summary>
      /// The row of the break.
      /// </summary>
      private ushort m_usRow = 0;
      /// <summary>
      /// The starting column of the break.
      /// </summary>
      private ushort m_usStartCol = 0;
      /// <summary>
      /// The ending column of the break.
      /// </summary>
      private ushort m_usEndCol = 0;
      #endregion

      #region THPageBreak constructors
      /// <summary>
      /// Default constructor
      /// </summary>
      public THPageBreak()
      {
      }

      /// <summary>
      /// Constructs class instance and fills fields with values.
      /// </summary>
      /// <param name="Row">Row of the break.</param>
      /// <param name="StartCol">Starting column of the break.</param>
      /// <param name="EndCol">Ending column of the break.</param>
      public THPageBreak( ushort Row, ushort StartCol, ushort EndCol )
      {
        m_usRow = Row;
        m_usStartCol = StartCol;
        m_usEndCol = EndCol;
      }
      #endregion

      #region THPageBreak properties
      /// <summary>
      /// The row of the break.
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
      /// The starting column of the break.
      /// </summary>
      public ushort StartColumn
      {
        get
        {
          return m_usStartCol;
        }
        set
        {
          m_usStartCol = value;
        }
      }
      /// <summary>
      /// The ending column of the break.
      /// </summary>
      public ushort EndColumn
      {
        get
        {
          return m_usEndCol;
        }
        set
        {
          m_usEndCol = value;
        }
      }
      #endregion

      #region ICloneable Members
      /// <summary>
      /// Creates a new object that is a copy of the current instance.
      /// </summary>
      /// <returns>A new object that is a copy of this instance.</returns>
      public object Clone()
      {
        return MemberwiseClone();
      }

      #endregion
    }
    #endregion

    #region Class members
    /// <summary>
    /// Number of page breaks.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usBreaksCount = 0;

    /// <summary>
    /// Array of HorizontalPageBreaks.
    /// </summary>
    private THPageBreak[] m_arrPageBreaks = null;
    #endregion

    #region Class properties
    /// <summary>
    /// Array of HorizontalPageBreaks.
    /// </summary>
    public THPageBreak[] PageBreaks
    {
      get
      {
        return m_arrPageBreaks;
      }
      set
      {
        m_arrPageBreaks = value;
        m_usBreaksCount = ( value != null ) ? (ushort) value.Length : (ushort) 0;
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
    /// Default constructor, sets all fields' default values.
    /// </summary>
    public  HorizontalPageBreaksRecord()
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
    public  HorizontalPageBreaksRecord( Stream stream, out int itemSize )
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
    public  HorizontalPageBreaksRecord( int iReserve )
      : base( iReserve )
    {
    }
    #endregion

    #region Record Serialization
    /// <summary>
    /// Parse structure of record. Converts data buffer to special
    /// values according to record specification.
    /// </summary>
    /// <exception cref="Syncfusion.XlsIO.Implementation.Exceptions.WrongBiffRecordDataException">
    /// When data array does not fit to the page breaks array.
    /// </exception>
    public override void ParseStructure()
    {
      m_usBreaksCount = GetUInt16( 0 );
      m_arrPageBreaks = new THPageBreak[ m_usBreaksCount ];

      ushort row, sCol, eCol;

      int offset = 2;
      for( int i = 0; i < m_usBreaksCount; i++, offset += 6 )
      {
        row   = GetUInt16( offset );
        sCol  = GetUInt16( offset + 2 );
        eCol  = GetUInt16( offset + 4 );

        m_arrPageBreaks[ i ] = new THPageBreak( row, sCol, eCol );
      }

      if( offset != m_iLength )
        throw new WrongBiffRecordDataException();
    }
    /// <summary>
    /// In this method, a class must pack all of its properties into
    /// an internal data array, m_data. This method is called by
    /// FillStream, when the record must be serialized into a stream.
    /// </summary>
    public override void InfillInternalData( ExcelVersion version )
    {
      m_data = new byte[ GetStoreSize( ExcelVersion.Excel97to2003 ) ];
      SetUInt16( 0, m_usBreaksCount );
      m_iLength = 2;

      for( int i = 0; i < m_usBreaksCount; i++, m_iLength += 6 )
      {
        SetUInt16( m_iLength    , m_arrPageBreaks[ i ].Row );
        SetUInt16( m_iLength + 2, m_arrPageBreaks[ i ].StartColumn );
        SetUInt16( m_iLength + 4, m_arrPageBreaks[ i ].EndColumn );
      }
    }

    /// <summary>
    /// Size of the required storage space. Read-only.
    /// </summary>
    public override int GetStoreSize( ExcelVersion version )
    {
      return DEF_FIXED_PART_SIZE + DEF_SUBITEM_SIZE * m_usBreaksCount;
    }
    #endregion
  }
}
