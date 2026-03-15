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

using Syncfusion.XlsIO.Parser.Biff_Records.MsoDrawing;
using System.Collections.Generic;
using Syncfusion.XlsIO.Interfaces;
#if ( WINRT )
using Syncfusion.XlsIO.Implementation;
#endif
#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  /// <summary>
  /// This record contains a drawing object provided by the Drawing tool.
  /// </summary>
  [ Biff( TBIFFRecord.MSODrawingGroup ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class MSODrawingGroupRecord
    //: BiffRecordWithContinue
    : BiffContinueRecordRaw
    , ICloneable,IDisposable
  {
    #region Class constants
    /// <summary>
    /// Data offset.
    /// </summary>
    private const int DEF_DATA_OFFSET = 0;
    #endregion

    #region Class members
    /// <summary>
    /// 
    /// </summary>
    protected byte[] m_tempData;
    /// <summary>
    /// 
    /// </summary>
    protected List<MsoBase> m_arrStructures = new List<MsoBase>();
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor
    /// </summary>
    public  MSODrawingGroupRecord()
      : base()
    {
    }

    protected override void OnDispose()
    {
        m_tempData = null;
        for (int i = 0; i < m_arrStructures.Count; i++)
        {
            m_arrStructures [i].Dispose ();
          
        }
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
    public  MSODrawingGroupRecord( Stream stream, out int itemSize )
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
    public  MSODrawingGroupRecord( int iReserve )
      : base( iReserve )
    {
    }
    #endregion
  
    #region Class properties
    /// <summary>
    /// Returns array with all mso structures. Read-only.
    /// </summary>
    public MsoBase[] Structures
    {
      get
      {
        return m_arrStructures.ToArray();
      }
    }
    /// <summary>
    /// Returns List with all mso structures. Read-only.
    /// </summary>
    public List<MsoBase> StructuresList
    {
      get
      {
        return m_arrStructures;
      }
    }
    /// <summary>
    /// Indicates whether record needs internal data array
    /// or if it can be cleaned. Read-only.
    /// </summary>
    public override bool NeedDataArray
    {
      get
      {
        return true;
      }
    }
    /// <summary>
    /// Offset to the structures data.
    /// </summary>
    protected virtual int StructuresOffset
    {
      get
      {
        return DEF_DATA_OFFSET;
      }
    }

//    /// <summary>
//    /// Type of the first continue record. Read-only.
//    /// </summary>
//    public override TBIFFRecord FirstContinueType
//    {
//      get
//      {
//        return TBIFFRecord.MSODrawingGroup;
//      }
//    }
    #endregion

    #region Record Serialization
    /// <summary>
    /// 
    /// </summary>
    public override void ParseStructure()
    {
      // Call to extract Continue Records after this record until other 
      // records in stream.
      //base.AddContinueRecordType( TBIFFRecord.MSODrawingGroup );
      if( m_extractor != null )
      {
        base.AddContinueRecordType( TypeCode );
        base.ParseStructure();

        m_tempData = new byte[ m_data.Length ];
        m_data.CopyTo( m_tempData, 0 );
        ParseData();
      }
    }
    /// <summary>
    /// Parses data.
    /// </summary>
    protected virtual void ParseData()
    {
      //int iOffset = StructuresOffset;
      MemoryStream stream = new MemoryStream( m_data );
      stream.Position = StructuresOffset;
      int iLength = m_data.Length;

      while( stream.Position < iLength )
      {
        MsoBase record = MsoFactory.CreateMsoRecord( null, stream );
        m_arrStructures.Add( record );
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public override void InfillInternalData( ExcelVersion version )
    {
      // If length is small enough this means that structures where set,
      // otherwise serialization has been already done.
      //if( m_iLength <= DEF_HEADER_SIZE && m_arrStructures.Count > 0 )
      if( m_arrStructures.Count > 0 )
      {
        m_arrContinuePos.Clear();
        int iStartIndex;

        Stream streamData = CreateDataList( out iStartIndex );
        FillDataList( streamData, iStartIndex );

        int iTempSize = ( int )streamData.Length;
        //m_tempData = CombineArrays( iDataSize, arrData );
        m_tempData = ( ( MemoryStream )streamData ).GetBuffer();

        // We must set Length property equal only to size of this record.
        m_iLength = ( iTempSize > MaximumRecordSize )
          ? MaximumRecordSize
          : iTempSize;

        AutoGrowData = true;
        SetBytes( 0, m_tempData, 0, m_iLength );

        // NOTE: Call to create Continue Builder. Continue Builder will start to
        // put data after this Record Position + MaximumRecordSize... that is why 
        // m_data must be filled first by record data and only after that we can 
        // add continue records.
        base.InfillInternalData( version );

        if( iTempSize > MaximumRecordSize )
        {
          int iPos = m_iLength;
          int iLen = iTempSize - iPos;
        
          Builder.AppendBytes( m_tempData, iPos, iLen );
          m_iLength = Builder.Total;
        }
      }
    }
    /// <summary>
    /// Creates List for all data.
    /// </summary>
    /// <param name="iStartIndex">First free index in the resulting List.</param>
    /// <returns>Created List.</returns>
    protected virtual Stream CreateDataList( out int iStartIndex )
    {
      //int iCount = m_arrStructures.Count;
      iStartIndex = 0;
      return new MemoryStream();
    }
    /// <summary>
    /// Fills array list with structures data.
    /// </summary>
    /// <param name="stream">Stream to put resulting data into.</param>
    /// <param name="iStartIndex">Start index in List.</param>
    protected void FillDataList( Stream stream, int iStartIndex )
    {
      int iCount = m_arrStructures.Count;

      for( int i = 0; i < iCount; i++ )
      {
        // Use 'as' to increase performance.
        MsoBase record = m_arrStructures[ i ];
        record.FillArray( stream );
      }
    }
    /// <summary>
    /// Size of the required storage space. Read-only.
    /// </summary>
    public override int GetStoreSize( ExcelVersion version )
    {
      // TODO: optimize
      if( NeedInfill )
      {
        InfillInternalData( version );
        NeedInfill = false;
      }

      return m_iLength;
    }
    #endregion

    #region Class Public Methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="item"></param>
    public void AddStructure( MsoBase item )
    {
      m_arrStructures.Add( item );
    }
    protected override ContinueRecordBuilder CreateBuilder()
    {
      ContinueRecordBuilder builder = base.CreateBuilder();
      builder.FirstContinueType = TBIFFRecord.MSODrawingGroup;
      return builder;
    }
    #endregion

    #region ICloneable Members
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    new public object Clone()
    {
      MSODrawingGroupRecord result = ( MSODrawingGroupRecord )base.Clone();

      this.m_arrStructures = new List<MsoBase>( result.m_arrStructures );
      return result;
    }

    #endregion
  }
}
