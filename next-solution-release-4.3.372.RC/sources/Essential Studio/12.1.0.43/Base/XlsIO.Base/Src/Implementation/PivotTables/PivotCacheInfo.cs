#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser.Biff_Records.PivotTable;

#if  (SILVERLIGHT) || ( WINRT ) || (WP)
using Syncfusion.XlsIO.Interfaces;
#endif


namespace Syncfusion.XlsIO.Implementation.PivotTables
{
  class PivotCacheInfo : ICloneable
  {
    #region Members
    /// <summary>
    /// List of info records.
    /// </summary>
    private List<BiffRecordRaw> m_records;
    #endregion

    #region Properties
    /// <summary>
    /// Gets stream id record.
    /// </summary>
    private StreamIdRecord StreamIdRecord
    {
      get
      {
        return ( m_records != null ) ?
          m_records[ 0 ] as StreamIdRecord :
          null;
      }
    }
    /// <summary>
    /// Gets or sets stream id described by this pivot cache info.
    /// </summary>
    public int StreamId
    {
      get
      {
        StreamIdRecord record = StreamIdRecord;

        return ( record != null ) ? record.StreamId : -1;
      }
      set
      {
        StreamIdRecord.StreamId = ( ushort )value;
      }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Default constructor.
    /// </summary>
    public PivotCacheInfo()
    {
    }
    /// <summary>
    /// Extracts pivot cache data from the records array.
    /// </summary>
    /// <param name="data">List of records to get data from.</param>
    /// <param name="startIndex">Starting index.</param>
    public PivotCacheInfo( IList<BiffRecordRaw> data, int startIndex )
    {
      Parse( data, startIndex );
    }
    /// <summary>
    /// Extracts required data from the records list.
    /// </summary>
    /// <param name="data">List of records to get data from.</param>
    /// <param name="startIndex">Starting index.</param>
    public int Parse( IList<BiffRecordRaw> data, int startIndex )
    {
      m_records = new List<BiffRecordRaw>();
      BiffRecordRaw record = data[ startIndex ];

      if( record.TypeCode != TBIFFRecord.StreamId )
        throw new ArgumentOutOfRangeException();

      int iCount = data.Count;

      do
      {
        m_records.Add( record );
        startIndex++;

        if( startIndex >= iCount )
          break;

        record = data[ startIndex ];
      }
      while( record.TypeCode != TBIFFRecord.StreamId );

      return startIndex;
    }
    /// <summary>
    /// Saves pivot cache info into OffsetArrayList.
    /// </summary>
    /// <param name="records">OffsetArrayList to save data into.</param>
    public void Serialize( OffsetArrayList records )
    {
      if( m_records != null && m_records.Count > 0 )
        records.AddList( m_records );
    }
    #endregion

    #region ICloneable Members
    /// <summary>
    /// Creates a copy of the current object.
    /// </summary>
    /// <returns>A copy of the current object.</returns>
    public object Clone()
    {
      PivotCacheInfo result = ( PivotCacheInfo )MemberwiseClone();
      result.m_records = CloneUtils.CloneCloneable<BiffRecordRaw>( m_records );
      return result;
    }

    #endregion
  }
}
