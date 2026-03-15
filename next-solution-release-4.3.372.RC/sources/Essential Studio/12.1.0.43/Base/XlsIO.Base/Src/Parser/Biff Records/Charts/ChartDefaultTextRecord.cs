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
#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records.Charts
{
  /// <summary>
  /// The DEFAULTTEXT record precedes a TEXT record to identify the text 
  /// defined in the TEXT record as the default properties for certain chart items.
  /// </summary>
  [ Biff( TBIFFRecord.ChartDefaultText ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class ChartDefaultTextRecord : BiffRecordRaw
  {
    #region Class constants
    /// <summary>
    /// Correct record size.
    /// </summary>
    private const int DEF_RECORD_SIZE = 2;
    /// <summary>
    /// Represents the text defaults options.
    /// </summary>
    public enum TextDefaults
    {
      /// <summary>
      /// Represents the ShowLabels text defaults option.
      /// </summary>
      ShowLabels = 0,
      /// <summary>
      /// Represents the ValueAndPercents text defaults option.
      /// </summary>
      ValueAndPercents = 1,
      /// <summary>
      /// Represents the All text defaults option.
      /// </summary>
      All = 2
    }
    #endregion

    #region Class members
    /// <summary>
    /// Object identifier for the text:
    /// 0 = default text characteristics for "show labels" data labels
    /// 1 = default text characteristics for value and percentage data labels
    /// 2 = default text characteristics for all text in the chart
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usObjectIdentifier;
    #endregion

    #region Class properties
    /// <summary>
    /// Object identifier for the text:
    /// TextDefaults.ShowLabels = default text characteristics for "show labels" data labels.
    /// TextDefaults.ValueAndPercents = default text characteristics for value and percentage data labels.
    /// TextDefaults.All = default text characteristics for all text in the chart.
    /// </summary>
    public TextDefaults TextCharacteristics
    {
      get
      {
        return ( TextDefaults )m_usObjectIdentifier;
      }
      set
      {
        m_usObjectIdentifier = ( ushort )value;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor, initializes all fields with default values.
    /// </summary>
    public  ChartDefaultTextRecord()
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
    public  ChartDefaultTextRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }
    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  ChartDefaultTextRecord( int iReserve )
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
      // TODO: check correctness of data

      m_usObjectIdentifier = provider.ReadUInt16( iOffset );
    }
    /// <summary>
    /// In this method, class must pack all of its properties into
    /// an internal data array, m_data. This method is called by
    /// FillStream, when the record must be serialized into a stream.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">Offset in the buffer.</param>
    /// <param name="version">Excel version used for infill.</param>
    public override void InfillInternalData( DataProvider provider, int iOffset, ExcelVersion version )
    {
      m_iLength = GetStoreSize( version );
      provider.WriteUInt16( iOffset, m_usObjectIdentifier );
    }
    /// <summary>
    /// 
    /// </summary>
    public override int GetStoreSize( ExcelVersion version )
    {
      return DEF_RECORD_SIZE;
    }
    #endregion
  }
}