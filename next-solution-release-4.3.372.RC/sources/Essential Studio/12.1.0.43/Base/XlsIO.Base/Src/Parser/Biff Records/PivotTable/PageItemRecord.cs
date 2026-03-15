#region Copyright

//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.

#endregion Copyright

using System;
using System.Collections.Generic;
using System.IO;

namespace Syncfusion.XlsIO.Parser.Biff_Records.PivotTable
{
  /// <summary>
  /// This record contains information about the PivotTable page item.
  /// </summary>
  [ Biff( TBIFFRecord.PageItem ) ]
  [ CLSCompliant( false ) ]
  public class PageItemRecord : BiffRecordRawWithArray
  {
    #region Class members
    /// <summary>
    /// This class contains information about field
    /// </summary>
    public class FieldInfo
    {
      /// <summary>
      /// Index to the ViewItemRecord for the page item.
      /// </summary>
      [BiffRecordPos( 0, 2 )]
      public ushort ViewItemIndex;
      /// <summary>
      /// Index to the ViewFieldsRecord for the page item.
      /// </summary>
      [BiffRecordPos( 2, 2 )]
      public ushort ViewFieldIndex;
      /// <summary>
      /// Object ID for the page item drop-down arrows.
      /// </summary>
      [BiffRecordPos( 4, 2 )]
      public ushort ObjectId;
      /// <summary>
      /// Creates copy of the current object.
      /// </summary>
      /// <returns>Create copy.</returns>
      internal FieldInfo Clone()
      {
        return ( FieldInfo )MemberwiseClone();
      }
    }
    /// <summary>
    /// List with page item description for each filter applied.
    /// </summary>
    private List<FieldInfo> m_arrFieldItems = new List<FieldInfo>();
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor.
    /// </summary>
    public  PageItemRecord()
      : base()
    {
    }

    /// <summary>
    /// Read / initialize constructor.
    /// </summary>
    /// <param name="stream">Stream from which record data should be read.</param>
    /// <param name="itemSize">Size of read item.</param>
    /// <exception cref="System.ArgumentNullException">When stream is not specified.</exception>
    /// <exception cref="System.ApplicationException">When stream does not support read or seek operations.</exception>
    public  PageItemRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }

    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for the data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  PageItemRecord( int iReserve )
      : base( iReserve )
    {
    }

    #endregion

    #region Class properties
    /// <summary>
    /// Returns field items. Read-only.
    /// </summary>
    public List<FieldInfo> Items
    {
      get
      {
        return m_arrFieldItems;
      }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Parse structure of record. Convert Data buffer to special
    /// values according to record specification.
    /// </summary>
    public override void ParseStructure()
    {
      int iOffset = 0;

      while( iOffset < m_iLength )
      {
        FieldInfo info = new FieldInfo();
        info.ViewItemIndex = GetUInt16( iOffset );
        iOffset += 2;

        info.ViewFieldIndex = GetUInt16( iOffset );
        iOffset += 2;

        info.ObjectId = GetUInt16( iOffset );
        iOffset += 2;

        m_arrFieldItems.Add( info );
      }
    }
    /// <summary>
    /// In this method, the class must pack all of its properties into an
    /// internal data array, m_data. This method is called by
    /// FillStream when the record must be serialized into a stream.
    /// </summary>
    public override void InfillInternalData( ExcelVersion version )
    {
      int iCount = m_arrFieldItems.Count;
      m_iLength =  0;
      m_data = new byte[ iCount * 6 ];

      for( int i = 0; i < iCount; i++ )
      {
        FieldInfo info = m_arrFieldItems[ i ];
        SetUInt16( m_iLength, info.ViewItemIndex );
        m_iLength += 2;

        SetUInt16( m_iLength, info.ViewFieldIndex );
        m_iLength += 2;

        SetUInt16( m_iLength, info.ObjectId );
        m_iLength += 2;
      }
    }
    /// <summary>
    /// Creates copy of the current object.
    /// </summary>
    /// <returns>Copy of the current object.</returns>
    public override object Clone()
    {
      PageItemRecord result = ( PageItemRecord )base.Clone();
      result.m_arrFieldItems = new List<FieldInfo>();

      for( int i = 0, len = m_arrFieldItems.Count; i < len; i++ )
      {
        result.m_arrFieldItems[ i ] = m_arrFieldItems[ i ].Clone();
      }

      return result;
    }
    #endregion
  }
}
