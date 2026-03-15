#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Text;

using Syncfusion.XlsIO;
using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser.Biff_Records.Charts;
using System.Collections.Generic;

namespace Syncfusion.XlsIO.Implementation
{
  /// <summary>
  /// Class allows users to configure Print setting of chart.
  /// </summary>
  public class ChartPageSetupImpl
    : PageSetupBaseImpl
    , IChartPageSetup
  {
    #region Class members
    /// <summary>
    /// Informs how chart should be printed.
    /// </summary>
    private PrintedChartSizeRecord m_chartSize = ( PrintedChartSizeRecord )BiffRecordFactory.GetRecord(
      TBIFFRecord.PrintedChartSize );
    #endregion

    #region IPageSetup Members
    /// <summary>
    /// Returns or sets the number of pages tall the worksheet will be scaled
    /// to when it is printed. Applies only to worksheets. Read / write Boolean.
    /// </summary>
    public bool   FitToPagesTall
    {
      get
      {
        return ( m_setup.FitHeight != 0 );
      }
      set
      {
        ushort newValue = value ? (ushort) 1 : (ushort) 0;
        if( m_setup.FitHeight != newValue )
        {
          m_setup.FitHeight = newValue;
          SetChanged();
        }
      }
    }
    /// <summary>
    /// Returns or sets the number of pages wide the worksheet will be scaled
    /// to when it is printed. Applies only to worksheets. Read / write Boolean.
    /// </summary>
    public bool   FitToPagesWide
    {
      get
      {
        return ( m_setup.FitWidth != 0 );
      }
      set
      {
        ushort newValue = value ? (ushort) 1 : (ushort) 0;
        if( m_setup.FitWidth != newValue )
        {
          m_setup.FitWidth = newValue;
          SetChanged();
        }
      }
    }

    #endregion

    #region Class Initialize methods
    /// <summary>
    /// Sets application and parent fields.
    /// </summary>
    /// <param name="application">Application object for the page setup.</param>
    /// <param name="parent">Parent object for the page setup.</param>
    public ChartPageSetupImpl( IApplication application, object parent )
      : base( application, parent )
    {
      FindParents();
    }
    /// <summary>
    /// Recovers page setup from the stream and sets its application and parent fields.
    /// The current record in the stream must be the PrintHeadersRecord.
    /// </summary>
    /// <param name="application">Application object for the page setup.</param>
    /// <param name="parent">Parent object for the page setup.</param>
    /// <param name="reader">BiffReader that contains page setup records.</param>
    [ CLSCompliant( false ) ]
    public ChartPageSetupImpl( IApplication application, object parent
      , BiffReader reader )
      : base( application, parent )
    {
      FindParents();
      Parse( reader );
    }
    /// <summary>
    /// Recovers Page setup from the Biff Records array starting from position.
    /// </summary>
    /// <param name="application">Application object for the page setup.</param>
    /// <param name="parent">Parent object for the page setup.</param>
    /// <param name="data">Array of Biff Records that contains all needed records.</param>
    /// <param name="position">Position of PrintHeadersRecord in the array.</param>
    [ CLSCompliant( false ) ]
    public ChartPageSetupImpl( IApplication application, object parent
      , IList<BiffRecordRaw> data, ref int position )
      : base( application, parent )
    {
      FindParents();
      position = Parse( data, position );
    }
    /// <summary>
    /// Recovers Page setup from the Biff Records list starting from position.
    /// </summary>
    /// <param name="application">Application object for the page setup.</param>
    /// <param name="parent">Parent object for the page setup.</param>
    /// <param name="data">List which contains Biff Records.</param>
    /// <param name="position">Position of PrintHeadersRecord in the array.</param>
    public ChartPageSetupImpl( IApplication application, object parent,
      List<BiffRecordRaw> data, ref int position )
      : base( application, parent )
    {
      FindParents();
      position = Parse( data, position );
    }
    #endregion

    #region Class reader methods
    /// <summary>
    /// Parses record.
    /// </summary>
    /// <param name="record">Record to parse.</param>
    /// <returns>True if record was successfully parsed, false otherwise.</returns>
    [ CLSCompliant( false ) ]
    protected override bool ParseRecord( BiffRecordRaw record )
    {
      bool bResult = base.ParseRecord( record );

      if( !bResult )
      {
        bResult = true;

        switch( record.TypeCode )
        {
          case TBIFFRecord.PrintedChartSize:
            m_chartSize = ( PrintedChartSizeRecord )record;
            break;

          default:
            bResult = false;
            break;
        }
      }

      return bResult;
    }


    /// <summary>
    /// Recovers page setup from the stream, first record must be PrintHeadersRecord.
    /// </summary>
    /// <param name="reader">Stream that contains all needed records.</param>
    [ CLSCompliant( false ) ]
    public void Parse( BiffReader reader )
    {
      throw new NotImplementedException();
    }
    #endregion

    #region Class serialize methods
    /// <summary>
    /// Serializes some records after main page setup block.
    /// </summary>
    /// <param name="records">OffsetArrayList to serialize into.</param>
    [ CLSCompliant( false ) ]
    protected override void SerializeEndRecords( OffsetArrayList records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      if( m_chartSize == null )
        throw new ArgumentNullException( "m_chartSize" );

      base.SerializeEndRecords( records );
      records.Add( ( BiffRecordRaw )m_chartSize.Clone() );
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// Clone current Record.
    /// </summary>
    /// <param name="parent">Parent object for create new instance.</param>
    /// <returns>Returns clone of current object.</returns>
    public ChartPageSetupImpl Clone( object parent )
    {
      ChartPageSetupImpl result = ( ChartPageSetupImpl )MemberwiseClone();
      result.SetParent( parent );
      result.FindParents();

      m_arrFooters = CloneUtils.CloneStringArray( m_arrFooters );
      m_arrHeaders = CloneUtils.CloneStringArray( m_arrHeaders );
      m_chartSize = ( PrintedChartSizeRecord )CloneUtils.CloneCloneable( m_chartSize );
      m_setup = ( PrintSetupRecord )CloneUtils.CloneCloneable( m_setup );
      m_unknown = ( PrinterSettingsRecord )CloneUtils.CloneCloneable( m_unknown );

      return result;
    }
    #endregion
  }
}
