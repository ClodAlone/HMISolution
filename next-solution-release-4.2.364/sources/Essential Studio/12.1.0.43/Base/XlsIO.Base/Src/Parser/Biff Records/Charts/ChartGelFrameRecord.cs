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
using System.Collections;
using System.IO;

using Syncfusion.XlsIO.Parser.Biff_Records.MsoDrawing;
using Syncfusion.XlsIO.Parser.Biff_Records;

using FOPTE = Syncfusion.XlsIO.Parser.Biff_Records.MsoDrawing.MsofbtOPT.FOPTE;
using System.Collections.Generic;

#if ( WINRT )
using Windows.UI;
using Rectangle = Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
#endif

#if  SILVERLIGHT
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.Silverlight.Rectangle;
#elif WP
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.WP.Rectangle;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;
#endif

#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records.Charts
{
  /// <summary>
  /// This record stores fill effects such as gradient fills, patterns, 
  /// textures, and so on. The record data is obtained from the 
  /// Microsoft Office Drawing DLL.
  /// </summary>
  [ Biff( TBIFFRecord.ChartGelFrame ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class ChartGelFrameRecord : BiffContinueRecordRaw
  {
    #region Class constants
    /// <summary>
    /// Represents first bytes in record.
    /// </summary>
    private readonly byte[] DEF_FIRST_BYTES = { 0xe3, 0x01, 0x0b, 0xf0 };
    /// <summary>
    /// Represents last bytes.
    /// </summary>
    private readonly byte[] DEF_LAST_BYTES = { 179, 0, 34, 241, 66, 0, 0, 0, 158, 1, 255, 255
      , 255, 255, 159, 1, 255, 255, 255, 255, 160, 1, 0, 0, 0, 32, 161, 193, 0, 0, 0, 0, 162
      , 1, 255, 255, 255, 255, 163, 1, 255, 255, 255, 255, 164, 1, 0, 0, 0, 32, 165, 193, 0, 0
      , 0, 0, 166, 1, 255, 255, 255, 255, 167, 1, 255, 255, 255, 255, 191, 1, 0, 0, 96, 0 };
    /// <summary>
    /// Represents start mso index.
    /// </summary>
    public const int DEF_START_MSO_INDEX = 384;
    /// <summary>
    /// Represents last mso index.
    /// </summary>
    public const int DEF_LAST_MSO_INDEX = 412;
    /// <summary>
    /// Represents default offset.
    /// </summary>
    public const int DEF_OFFSET = 8;
    #endregion

    #region Class members
    /// <summary>
    /// Represents option list;
    /// </summary>
    private List<FOPTE> m_list = new List<FOPTE>();
    #endregion

    #region Class properties
    /// <summary>
    /// Indicates whether record needs internal data array or if it can be cleaned.
    /// </summary>
    public override bool NeedDataArray
    {
      get
      {
        return true;
      }
    }

    /// <summary>
    /// Represents list of fill options.
    /// </summary>
    public List<FOPTE> OptionList
    {
      get
      {
        return m_list;
      }
      set
      {
        if( value == null )
          throw new ArgumentNullException( "OptionList" );

        m_list = value;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor, initializes all fields with default values.
    /// </summary>
    public  ChartGelFrameRecord()
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
    public  ChartGelFrameRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }
    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  ChartGelFrameRecord( int iReserve )
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
      if( m_extractor != null )
      {
        base.AddContinueRecordType( TypeCode );
        base.ParseStructure();

        ParseData();
        m_arrContinuePos.Clear();
      }
    }
    /// <summary>
    /// Parses data from stream.
    /// </summary>
    private void ParseData()
    {
      int iOffset = DEF_OFFSET;
      uint uLength = BitConverter.ToUInt32( m_data, 4 );

      uint iEndOffset = uLength + ( uint )iOffset;

      while( iOffset < iEndOffset )
      {
        FOPTE prop = new FOPTE( m_data, ref iOffset );
        m_list.Add( prop );

        if( prop.IsComplex )
        {
          iEndOffset -= prop.UInt32Value;
        }
      }

      for( int i = 0, len = m_list.Count; i < len; i++ )
      {
        FOPTE prop = m_list[ i ];
        prop.ReadComplexData( m_data, ref iOffset );
      }
    }
    /// <summary>
    /// In this method, a class must pack all of its properties into
    /// an internal data array, m_data. This method is called by
    /// FillStream when the record must be serialized into a stream.
    /// </summary>
    public override void InfillInternalData( ExcelVersion version )
    {
//      if( m_iLength < 0 )
//      {
//        FillDataList();
//
//        int iTempSize = m_iLength;
//        byte[] m_tempData = Data;
//
//        // We must set Length property equal only to size of this record.
//        m_iLength = ( iTempSize > MaximumRecordSize )
//          ? MaximumRecordSize
//          : iTempSize;
//
//        AutoGrowData = true;
//        SetBytes( 0, m_tempData, 0, m_iLength );
//
//        // NOTE: Call to create Continue Builder. Continue Builder will start to
//        // put data after this Record Position + MaximumRecordSize... that is why 
//        // m_data must be filled first by record data and only after that we can 
//        // add continue records.
//        base.InfillInternalData();
//        Builder.FirstContinueType = TBIFFRecord.ChartGelFrame;
//
//        if( iTempSize > MaximumRecordSize )
//        {
//          int iPos = m_iLength;
//          int iLen = iTempSize - iPos;
//        
//          Builder.AppendBytes( m_tempData, iPos, iLen );
//          m_iLength = Builder.Total;
//        }
//      }
    }
    /// <summary>
    /// Infill internal data.
    /// </summary>
    private void FillDataList()
    {
      m_iLength = 0;
      int iStartAdd = DEF_OFFSET;

      for( int i = 0, ilen = m_list.Count; i < ilen; i++ )
      {
        FOPTE rec = m_list[ i ];

        m_iLength += rec.MainData.Length;
        iStartAdd += rec.MainData.Length;

        if( rec.AdditionalData != null && rec.AdditionalData.Length > 0 )
          m_iLength += rec.AdditionalData.Length;
      }

      m_iLength += DEF_LAST_BYTES.Length + DEF_OFFSET;
      m_data = new byte[ m_iLength ];

      DEF_FIRST_BYTES.CopyTo( m_data, 0 );
      DEF_LAST_BYTES.CopyTo( m_data, m_iLength - DEF_LAST_BYTES.Length );
      Array.Copy( BitConverter.GetBytes( m_iLength - DEF_OFFSET - DEF_LAST_BYTES.Length ), 0, m_data, 4, 4 );

      int iIndex = DEF_OFFSET;

      for( int i = 0, ilen = m_list.Count; i < ilen; i++ )
      {
        FOPTE rec = m_list[ i ];

        rec.MainData.CopyTo( m_data, iIndex );
        iIndex += rec.MainData.Length;

        if( rec.AdditionalData != null && rec.AdditionalData.Length > 0 )
        {
          rec.AdditionalData.CopyTo( m_data, iStartAdd );
          iStartAdd += rec.AdditionalData.Length;
        }
      }
    }
    /// <summary>
    /// Updates gel record to adds to stream.
    /// </summary>
    /// <returns>Returns array list with updated gel frame records.</returns>
    public List<BiffRecordRaw> UpdatesToAddInStream()
    {
      FillDataList();

      List<BiffRecordRaw> result = new List<BiffRecordRaw>();
      int iLen = m_data.Length;
      BiffRecordRawWithArray record = this;
      int iIndex = 0;
      int iCount = 0;

      while( iLen > DEF_RECORD_MAX_SIZE )
      {
        TBIFFRecord id = ( iCount < 2 ) ? TBIFFRecord.ChartGelFrame : TBIFFRecord.Continue;
        record = ( BiffRecordRawWithArray )BiffRecordFactory.GetRecord( id );
        byte[] arr = new byte[ DEF_RECORD_MAX_SIZE ];

        Array.Copy( m_data, iIndex, arr, 0, DEF_RECORD_MAX_SIZE );
        record.SetInternalData( arr );
        record.Length = DEF_RECORD_MAX_SIZE;
        result.Add( record );

        iIndex += DEF_RECORD_MAX_SIZE;
        iCount++;
        iLen -= DEF_RECORD_MAX_SIZE;
      }

      if( iIndex == 0 )
      {
        result.Add( record );
      }
      else
      {
        TBIFFRecord id = ( iCount < 2 ) ? TBIFFRecord.ChartGelFrame : TBIFFRecord.Continue;
        record = ( BiffRecordRawWithArray )BiffRecordFactory.GetRecord( id );

        int iDataLen = m_data.Length - iIndex;
        byte[] data = new byte[ iDataLen ];

        Array.Copy( m_data, iIndex, data, 0, iDataLen );
        record.SetInternalData( data );
        record.Length = iDataLen;

        result.Add( record );
      }

      return result;
    }
    #endregion

    #region ICloneable methods
    /// <summary>
    /// Clones current record.
    /// </summary>
    /// <returns>Returns cloned object.</returns>
    public override object Clone()
    {
      ChartGelFrameRecord result = ( ChartGelFrameRecord )base.Clone();
      result.m_list = new List<FOPTE>();

      for( int i = 0, iLen = m_list.Count; i < iLen; i++ )
      {
        FOPTE fop = m_list[ i ];
        result.m_list.Add( ( FOPTE )fop.Clone() );
      }

      return result;
    }

    #endregion

    #region Class methods
    /// <summary>
    /// Updates record to serialize in biff stream.
    /// </summary>
    public void UpdateToSerialize()
    {
      //m_list.Clear();
      m_iLength = -1;
      int desiredIndex;

      for( int i = DEF_START_MSO_INDEX; i <= DEF_LAST_MSO_INDEX; i++ )
      {
        MsoOptions id = ( MsoOptions )i;

        if( !Contains( id, out desiredIndex ) )
        {
          FOPTE opt = new FOPTE();
          opt.Id = id;

          if( opt.Id == MsoOptions.Transparency || opt.Id == MsoOptions.GradientTransparency )
            opt.UInt32Value = ushort.MaxValue;

          if( opt.Id == MsoOptions.GradientColorType )
            opt.UInt32Value = 1;

          m_list.Insert( desiredIndex, opt );
        }
      }

      if( !Contains( MsoOptions.NoFillHitTest, out desiredIndex ) )
      {
        FOPTE fop = new FOPTE();
        fop.Id = MsoOptions.NoFillHitTest;

        m_list.Insert( desiredIndex, fop );
      }
    }
    private bool Contains( MsoOptions id, out int index )
    {
      index = 0;

      for( int len = m_list.Count; index < len; index++ )
      {
        if( m_list[ index ].Id == id )
          break;
      }

      return index < m_list.Count;
    }
    #endregion
  }
}
