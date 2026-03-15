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
using Syncfusion.XlsIO.Interfaces;
#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  /// <summary>
  /// Contains a list of explicit column page breaks.
  /// </summary>
  [ Biff( TBIFFRecord.VerticalPageBreaks ) ]
  [ CLSCompliant( false ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  public class VerticalPageBreaksRecord : BiffRecordRaw
  {
    #region Class constants
    /// <summary>
    /// Size of the fixed part.
    /// </summary>
    internal const int DEF_FIXED_PART_SIZE = 2;
    /// <summary>
    /// Size of the subitem.
    /// </summary>
    internal const int DEF_SUBITEM_SIZE = 6;
    #endregion

    #region Internal classes
    /// <summary>
    /// Structure that contains information about a vertical page break.
    /// </summary>
    public class TVPageBreak : ICloneable
    {
      #region TVPageBreak members
      /// <summary>
      /// The column of the break.
      /// </summary>
      private ushort m_usCol = 0;
      /// <summary>
      /// The starting row of the break.
      /// </summary>
      private uint m_uiStartRow = 0;
      /// <summary>
      /// The ending row of the break.
      /// </summary>
      private uint m_uiEndRow = 0;
      #endregion

      #region TVPageBreak constructors
      /// <summary>
      /// Default constructor
      /// </summary>
      public TVPageBreak()
      {
      }

      /// <summary>
      /// Constructs class instance and fills it with specified values.
      /// </summary>
      /// <param name="Col">Column of the break.</param>
      /// <param name="StartRow">Starting row of the break.</param>
      /// <param name="EndRow">Ending row of the break.</param>
      public TVPageBreak( ushort Col, ushort StartRow, ushort EndRow )
      {
        m_usCol = Col;
        m_uiStartRow = StartRow;
        m_uiEndRow = EndRow;
      }
      #endregion

      #region TVPageBreak properties
      /// <summary>
      /// The column of the break.
      /// </summary>
      public ushort Column
      {
        get
        {
          return m_usCol;
        }
        set
        {
          m_usCol = value;
        }
      }
      /// <summary>
      /// The starting row of the break.
      /// </summary>
      public uint StartRow
      {
        get
        {
          return m_uiStartRow;
        }
        set
        {
          m_uiStartRow = value;
        }
      }
      /// <summary>
      /// The ending row of the break.
      /// </summary>
      public uint EndRow
      {
        get
        {
          return m_uiEndRow;
        }
        set
        {
          m_uiEndRow = value;
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
    /// Array of VerticalPageBreaks.
    /// </summary>
    private TVPageBreak[] m_arrPageBreaks = null;
    #endregion

    #region Class properties
    /// <summary>
    /// Array of VerticalPageBreaks.
    /// </summary>
    public TVPageBreak[] PageBreaks
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
    /// Read-only. Maximum possible size of the record.
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
    /// Default constructor, sets all fields default values.
    /// </summary>
    public  VerticalPageBreaksRecord()
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
    public  VerticalPageBreaksRecord( Stream stream, out int itemSize )
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
    public  VerticalPageBreaksRecord( int iReserve )
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
      m_usBreaksCount = provider.ReadUInt16( iOffset + 0 );
      m_arrPageBreaks = new TVPageBreak[ m_usBreaksCount ];

      ushort col, sRow, eRow;

      iOffset += 2;
      for( int i = 0; i < m_usBreaksCount; i++, iOffset += 6 )
      {
        col  = provider.ReadUInt16( iOffset );
        sRow = provider.ReadUInt16( iOffset + 2 );
        eRow = provider.ReadUInt16( iOffset + 4 );

        m_arrPageBreaks[ i ] = new TVPageBreak( col, sRow, eRow );
      }
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
      provider.WriteUInt16( iOffset + 0, m_usBreaksCount );
      m_iLength = ExcelConstants.ShortSize;

      for( int i = 0; i < m_usBreaksCount; i++, m_iLength += 6 )
      {
        provider.WriteUInt16( iOffset + m_iLength, m_arrPageBreaks[ i ].Column );
        provider.WriteUInt16( iOffset + m_iLength + 2, ( ushort )m_arrPageBreaks[ i ].StartRow );
        provider.WriteUInt16( iOffset + m_iLength + 4, ( ushort )m_arrPageBreaks[ i ].EndRow );
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
