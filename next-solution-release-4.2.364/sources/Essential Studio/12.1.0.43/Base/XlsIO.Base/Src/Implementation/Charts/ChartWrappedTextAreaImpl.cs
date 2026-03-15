#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

using System;
using System.Collections;

using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser.Biff_Records.Charts;
using System.Collections.Generic;

namespace Syncfusion.XlsIO.Implementation.Charts
{
	/// <summary>
	/// Represents chart text area, each record is wrapped into ChartWrappedRecord.
	/// </summary>
  public class ChartWrappedTextAreaImpl
    : ChartTextAreaImpl
    , ISerializable
  {
    #region Class constants
    /// <summary>
    /// Unknown bytes. Needed for data labels serialization.
    /// </summary>
    private static readonly byte[][] DEF_UNKNOWN_START = new byte[][]
    {
      new byte[]
      {
        0x50, 0x08, 0x00, 0x00, 0x0A, 0x0A, 0x03, 0x00, 0x50, 0x08, 0x5A, 0x08, 0x61, 0x08, 0x61, 0x08,
        0x6A, 0x08, 0x6B, 0x08                                    
      },
      new byte[]
      {
        0x52, 0x08, 0x00, 0x00, 0x0D, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
      },
      new byte[]
      {
        0x6A, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
      },
      new byte[]
      {
        0x54, 0x08, 0x00, 0x00, 0x12, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
      },
    };
    /// <summary>
    /// Unknown end.
    /// </summary>
    private static readonly byte[][] DEF_UNKNOWN_END = new byte[][]
    {
      new byte[]
      {
        0x55, 0x08, 0x00, 0x00, 0x12, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00
      },
      new byte[]
      {
        0x53, 0x08, 0x00, 0x00, 0x0D, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00
      },
    };
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Initializes new instance.
    /// </summary>
    /// <param name="application">Application object.</param>
    /// <param name="parent">Parent object.</param>
    public ChartWrappedTextAreaImpl( IApplication application, object parent )
      : base( application, parent )
    {
    }
    /// <summary>
    /// Creates objects sets its Application and Parent properties to specified
    /// values and parses object data.
    /// </summary>
    /// <param name="application">Application object.</param>
    /// <param name="parent">Parent object.</param>
    /// <param name="data">Array with object's records.</param>
    /// <param name="iPos">Position of the first object's record in the data array.</param>
    public ChartWrappedTextAreaImpl( IApplication application, object parent,
      IList<BiffRecordRaw> data, ref int iPos )
      : base( application, parent, data, ref iPos )
    {
    }
    /// <summary>
    /// Creates objects sets its Application and Parent properties to specified values.
    /// </summary>
    /// <param name="application">Application object.</param>
    /// <param name="parent">Parent object.</param>
    /// <param name="textLink">Text link.</param>
    [ CLSCompliant( false ) ]
    public ChartWrappedTextAreaImpl( IApplication application, object parent, ExcelObjectTextLink textLink )
      : base( application, parent, textLink )
    {
    }
    #endregion

    #region Class overrides
    /// <summary>
    /// Creates frame format.
    /// </summary>
    /// <returns>Newly created frame format.</returns>
    protected override ChartFrameFormatImpl CreateFrameFormat()
    {
      return new ChartWrappedFrameFormatImpl( Application, this );
    }

    #endregion

    #region Class serialize methods
    /// <summary>
    /// Saves single record into list of biff records.
    /// </summary>
    /// <param name="records">OffsetArrayList that will receive necessary records.</param>
    /// <param name="record">Record to serialize.</param>
    [ CLSCompliant( false ) ]
    protected override void SerializeRecord( IList<IBiffStorage> records, BiffRecordRaw record )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      if( record == null )
        throw new ArgumentNullException( "record" );

      if( record.TypeCode == TBIFFRecord.ChartDataLabels )
      {
        base.SerializeRecord( records, record );
      }
      else
      {
        ChartWrapperRecord wrapper = ( ChartWrapperRecord )
          BiffRecordFactory.GetRecord( TBIFFRecord.ChartWrapper );

        wrapper.Record = ( BiffRecordRaw )record.Clone();

        records.Add( wrapper );
      }
    }
    /// <summary>
    /// Indicates whether object should be serialized. Read-only.
    /// </summary>
    protected override bool ShouldSerialize
    {
      get
      {
        return true;
      }
    }
//    /// <summary>
//    /// Saves chart text area into OffsetArrayList.
//    /// </summary>
//    /// <param name="records">
//    /// OffsetArrayList that will receive all chart's records.
//    /// </param>
//    /// <exception cref="System.ArgumentNullException">
//    /// When specified OffsetArrayList is NULL.
//    /// </exception>
//    [ CLSCompliant( false ) ]
//    public override void Serialize( OffsetArrayList records )
//    {
//      //SerializeUnknown( records, DEF_UNKNOWN_START );
//      base.Serialize( records );
//      //SerializeUnknown( records, DEF_UNKNOWN_END );
//    }

    /// <summary>
    /// Serializes unknown section.
    /// </summary>
    /// <param name="records">
    /// OffsetArrayList that will receive all chart's records.
    /// </param>
    /// <param name="arrUnknown">Unknown bytes to serialize.</param>
    private void SerializeUnknown( OffsetArrayList records, byte[][] arrUnknown )
    {
      if( arrUnknown == null )
        throw new ArgumentNullException( "arrUnknown" );

      if( records == null )
        throw new ArgumentNullException( "records" );

      for( int i = 0, len = arrUnknown.Length; i < len; i++ )
      {
        byte[] arrData = arrUnknown[ i ];

        if( arrData == null )
          throw new ArgumentNullException( "arrData" );

        int iDataLen = arrData.Length;
        UnknownRecord unknown = ( UnknownRecord )BiffRecordFactory.GetRecord( TBIFFRecord.Unknown );
        unknown.RecordCode = BitConverter.ToUInt16( arrData, 0 );
        unknown.m_data = new byte[ iDataLen ];
        unknown.DataLen = iDataLen;
        arrData.CopyTo( unknown.m_data, 0 );

        records.Add( unknown );
      }
    }
    #endregion
  }
}
