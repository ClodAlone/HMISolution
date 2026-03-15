#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

using System;
using System.Collections;

using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser.Biff_Records.Charts;
using System.Collections.Generic;

namespace Syncfusion.XlsIO.Implementation.Charts
{
  /// <summary>
  /// Represents frame format each record is wrapped by ChartWrapperRecord.
  /// </summary>
  public class ChartWrappedFrameFormatImpl : ChartFrameFormatImpl
  {
    #region Class Initialize/Finalize methods
    /// <summary>
    /// Initializes new instance of ChartWrappedFrameFormat.
    /// </summary>
    /// <param name="application">Application object.</param>
    /// <param name="parent">Parent object.</param>
    public ChartWrappedFrameFormatImpl( IApplication application, object parent )
      : base( application, parent )
    {
    }
    #endregion

    #region Class parse / serialization methods
    /// <summary>
    /// Checks whether specified record is begin.
    /// </summary>
    /// <param name="record">Record to check.</param>
    /// <returns>True if this is begin; false otherwise.</returns>
    [ CLSCompliant( false ) ]
    protected override bool CheckBegin( BiffRecordRaw record )
    {
      record = UnwrapRecord( record );
      return base.CheckBegin( record );
    }

    /// <summary>
    /// Parses single record.
    /// </summary>
    /// <param name="record">Record to parse.</param>
    /// <param name="iBeginCounter">Number of not closed begin record.</param>
    [ CLSCompliant( false ) ]
    protected override void ParseRecord( BiffRecordRaw record, ref int iBeginCounter )
    {
      record = UnwrapRecord( record );
      base.ParseRecord( record, ref iBeginCounter );
    }

    /// <summary>
    /// Unwraps record if necessary.
    /// </summary>
    /// <param name="record">Record to unwrap.</param>
    /// <returns>Unwrapped record.</returns>
    [ CLSCompliant( false ) ]
    protected override BiffRecordRaw UnwrapRecord( BiffRecordRaw record )
    {
      if( record == null )
        throw new ArgumentNullException( "record" );

      if( record.TypeCode == TBIFFRecord.ChartWrapper )
      {
        ChartWrapperRecord wrapper = ( ChartWrapperRecord )record;
        return wrapper.Record;
      }

      return record;
    }
    /// <summary>
    /// Serializes single record.
    /// </summary>
    /// <param name="list">OffsetArrayList that will get biff records.</param>
    /// <param name="record">Record to serialize.</param>
    [ CLSCompliant( false ) ]
    protected override void SerializeRecord( IList<IBiffStorage> list, BiffRecordRaw record )
    {
      if( list == null )
        throw new ArgumentNullException( "list" );

      if( record == null )
        throw new ArgumentNullException( "record" );

      ChartWrapperRecord wrapper = ( ChartWrapperRecord )
        BiffRecordFactory.GetRecord( TBIFFRecord.ChartWrapper );

      wrapper.Record = record;
      list.Add( ( BiffRecordRaw )wrapper.Clone() );
    }
    #endregion
  }
}
