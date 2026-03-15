#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

using System;
using System.Collections;
using System.Diagnostics;

using Syncfusion.XlsIO;
using Syncfusion.XlsIO.Implementation.Charts;
using Syncfusion.XlsIO.Implementation.Security;
using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser.Biff_Records.Charts;
using System.Collections.Generic;

namespace Syncfusion.XlsIO.Implementation.Collections
{
  /// <summary>
  /// Collection of the Charts object.
  /// </summary>
  public class ChartsCollection
    : CollectionBaseEx<IChart>
    , ICharts
  {
    #region Class constants
    /// <summary>
    /// Default start of the chart name.
    /// </summary>
    public const string DEF_CHART_NAME_START = "Chart";
    #endregion

    #region Class members
    /// <summary>
    /// Name-to-Chart dictionary.
    /// </summary>
    private Dictionary<string, IChart> m_hashNames = new Dictionary<string, IChart>( System.StringComparer.CurrentCultureIgnoreCase );
    /// <summary>
    /// Parent workbook.
    /// </summary>
    private WorkbookImpl m_book;
    #endregion

    #region Class constructor
    /// <summary>
    /// Creates chart collection.
    /// </summary>
    /// <param name="application">Application object for the collection.</param>
    /// <param name="parent">Parent object for the collection.</param>
    public ChartsCollection( IApplication application, object parent )
      : base( application, parent )
    {
      SetParents();
      m_book.Objects.TabSheetMoved += new TabSheetMovedEventHandler(Objects_TabSheetMoved);
    }
    #endregion

    #region ICharts Members
    /// <summary>
    /// Returns a single Chart object from a Charts collection.
    /// </summary>
    public IChart this[ string name ]
    {
      get
      {
        return m_hashNames[ name ];
      }
    }

    #endregion

    #region ICharts Methods
    /// <summary>
    /// Creates a new chart.
    /// </summary>
    /// <returns>Newly created chart object.</returns>
    public IChart Add()
    {
      ChartImpl chart = new ChartImpl( Application, this );
      chart.Name = GenerateDefaultName( List, DEF_CHART_NAME_START );

      return Add( chart );
    }

    /// <summary>
    /// Defines a new name.
    /// </summary>
    /// <param name="name">Name of the new chart's sheet.</param>
    /// <returns>Newly created chart object.</returns>
    public IChart Add(string name)
    {
      ChartImpl chart = new ChartImpl( Application, this );
      chart.Name = name;
      
      return Add( chart );
    }
    /// <summary>
    /// Removes Chart object from the collection.
    /// </summary>
    /// <param name="name">Name of the object to remove from the collection.</param>
    public IChart Remove( string name )
    {
      if( m_hashNames.ContainsKey( name ) )
      {
        if( m_book.ObjectCount == 1 )
          throw new ArgumentException( "Can't remove last worksheet from the workbook." );

        IChart result = m_hashNames[ name ];
        base.Remove( result );
        m_book.Objects.RemoveAt( ( ( ISerializableNamedObject ) result ).RealIndex );
      
        return result;
      }

      return null;
    }

    #endregion

    #region Class methods
    /// <summary>
    /// Adds specified chart to the collection.
    /// </summary>
    /// <param name="chartToAdd">Chart that should be added to the collection.</param>
    /// <returns>Added chart object.</returns>
    public IChart Add( IChart chartToAdd )
    {
      AddInternal( chartToAdd );
      m_book.Objects.Add( ( ISerializableNamedObject )chartToAdd );
      return chartToAdd;
    }

    /// <summary>
    /// Extracts chart object from the BiffReader.
    /// </summary>
    /// <param name="reader">BiffReader that contains chart data.</param>
    /// <returns>Extracted chart.</returns>
    [ CLSCompliant( false ) ]
    public IChart Add( BiffReader reader )
    {
      return Add( reader, ExcelParseOptions.Default, false, null, null );
    }

    /// <summary>
    /// Extracts chart object from the BiffReader.
    /// </summary>
    /// <param name="reader">BiffReader that contains chart data.</param>
    /// <param name="options">Parse options.</param>
    /// <param name="bSkip">Indicates whether skip parsing.</param>
    /// <param name="hashNewXFormatIndexes">
    /// Dictionary with new extended format indexes for ignore styles mode.
    /// </param>
    /// <param name="decryptor">Object used to decrypt encrypted records.</param>
    /// <returns>Extracted chart.</returns>
    [ CLSCompliant( false ) ]
    public IChart Add( BiffReader reader, ExcelParseOptions options, bool bSkip,
      Dictionary<int, int> hashNewXFormatIndexes, IDecryptor decryptor )
    {
      ChartImpl chart = new ChartImpl( Application, this, reader,
        options, bSkip, hashNewXFormatIndexes, decryptor );

      return Add( chart );
    }
    /// <summary>
    /// Sets all parents.
    /// </summary>
    /// <exception cref="System.ArgumentNullException">
    /// If one of the parent objects cannot be found.
    /// </exception>
    private void SetParents()
    {
      object parent = FindParent( typeof( WorkbookImpl ) );
      
      if( parent == null )
        throw new ArgumentNullException( "Can't find parent workbook." );

      m_book = (WorkbookImpl) parent;
    }
    /// <summary>
    /// Moves worksheet inside this collection only.
    /// </summary>
    /// <param name="iOldIndex">Old index in the collection.</param>
    /// <param name="iNewIndex">New index in the collection.</param>
    public void Move( int iOldIndex, int iNewIndex )
    {
      if( iOldIndex == iNewIndex ) return ;

      int iCount = InnerList.Count;

      if( iOldIndex < 0 || iOldIndex >= iCount )
        throw new ArgumentOutOfRangeException( "iOldIndex" );

      if( iNewIndex < 0 || iNewIndex >= iCount )
        throw new ArgumentOutOfRangeException( "iNewIndex" );

      ChartImpl toMove = this[ iOldIndex ] as ChartImpl;
      InnerList.RemoveAt( iOldIndex );
      InnerList.Insert( iNewIndex, toMove );
    }

    /// <summary>
    /// Event handler for series NameChanged event
    /// </summary>
    /// <param name="sender">Sender of the event.</param>
    /// <param name="e">Event arguments.</param>
    private void ChartsCollection_NameChanged( object sender, ValueChangedEventArgs e )
    {
      ChangeName( m_hashNames, e );
    }
    /// <summary>
    /// Performs additional operations before Clear method execution.
    /// </summary>
    protected override void OnClear()
    {
      base.OnClear ();
      m_hashNames.Clear();
    }

    /// <summary>
    /// Updates chart index after move/insert operation.
    /// </summary>
    /// <param name="chart">Chart that was changed.</param>
    /// <param name="iOldRealIndex">Old sheet index in the TabSheets collection.</param>
    private void UpdateSheetIndex( ChartImpl chart, int iOldRealIndex )
    {
      if( chart == null )
        throw new ArgumentNullException( "chart" );

      int iNewRealIndex = chart.RealIndex;
      int iIncrement = 0;
      int iStartIndex = -1;
      ITabSheets arrTabSheets = m_book.TabSheets;
      int iEndIndex = iOldRealIndex;

      if( iOldRealIndex > iNewRealIndex )
      {
        iStartIndex = iNewRealIndex + 1;
        iIncrement = 1;
      }
      else if( iOldRealIndex < iNewRealIndex )
      {
        iStartIndex = iNewRealIndex - 1;
        iIncrement = -1;
      }
      else
      {
        throw new NotImplementedException( "Chart wasn't moved at all" );
      }

      ITabSheet minTabSheet = null;

      for( int i = iStartIndex; i <= iEndIndex; i += iIncrement )
      {
        ITabSheet tabSheet = arrTabSheets[ i ];

        if( tabSheet is ChartImpl )
        {
          minTabSheet = tabSheet;
          break;
        }
      }

      if( minTabSheet != null )
      {
        ChartImpl chartLocated = ( ChartImpl )minTabSheet;
        int iOldSheetIndex = chart.Index;
        int iNewSheetIndex = chartLocated.Index;

        MoveInternal( iOldSheetIndex, iNewSheetIndex );
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="iOldSheetIndex"></param>
    /// <param name="iNewSheetIndex"></param>
    private void MoveInternal( int iOldSheetIndex, int iNewSheetIndex )
    {
      if( iOldSheetIndex == iNewSheetIndex ) return;

      int iCount = InnerList.Count;

      if( iOldSheetIndex < 0 || iOldSheetIndex >= iCount )
        throw new ArgumentOutOfRangeException( "iOldIndex" );

      if( iNewSheetIndex < 0 || iNewSheetIndex >= iCount )
        throw new ArgumentOutOfRangeException( "iNewIndex" );

      ChartImpl toMove = this[ iOldSheetIndex ] as ChartImpl;
      InnerList.RemoveAt( iOldSheetIndex );
      InnerList.Insert( iNewSheetIndex, toMove );

      int iMin = Math.Min( iNewSheetIndex, iOldSheetIndex );
      int iMax = Math.Max( iNewSheetIndex, iOldSheetIndex );

      for( int i = iMin; i <= iMax; i++ )
      {
        toMove = this[ i ] as ChartImpl;
        toMove.Index = i;
      }
    }
    /// <summary>
    /// Adds chart into internal collections.
    /// </summary>
    /// <param name="chartToAdd">Chart to add.</param>
    public void AddInternal( IChart chartToAdd )
    {
      if( chartToAdd == null )
        throw new ArgumentNullException( "chartToAdd" );

      if( chartToAdd.Name == null || chartToAdd.Name.Length == 0 )
        chartToAdd.Name = GenerateDefaultName( List, DEF_CHART_NAME_START );

      m_hashNames.Add( chartToAdd.Name, chartToAdd );

      ChartImpl chart = chartToAdd as ChartImpl;
      chart.Index = Count;
      chart.NameChanged += ChartsCollection_NameChanged;

      base.Add( chartToAdd );
      
      //      ( (ISerializableNamedObject) chartToAdd ).RealIndex = m_book.Objects.Count;
    }
    /// <summary>
    /// This method is called after tabsheet was moved.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void Objects_TabSheetMoved(object sender, TabSheetMovedEventArgs args)
    {
      ITabSheets tabSheets = ( ITabSheets )sender;
      int iNewIndex = args.NewIndex;

      ChartImpl chart = tabSheets[ iNewIndex ] as ChartImpl;

      if( chart != null )
      {
        int iOldIndex = args.OldIndex;
        UpdateSheetIndex( chart, iOldIndex );
      }
    }
    /// <summary>
    /// Adds copy of the specified chart to the collection.
    /// </summary>
    /// <param name="chartToCopy">Chart to copy.</param>
    public void AddCopy( IChart chartToCopy )
    {
      if( chartToCopy == null )
        throw new ArgumentNullException( "chartToCopy" );

      ChartImpl newChart = ( ChartImpl )( chartToCopy as ChartImpl ).Clone( null, this, null );
      newChart.ClearEvents();
      newChart.Name = CollectionBaseEx<object>.GenerateDefaultName( m_book.Objects, chartToCopy.Name );
      //newChart.NameChanged += ChartsCollection_NameChanged;
      //m_book.Objects.Add( newChart );
      ((WorksheetBaseImpl)newChart).m_dataHolder = null;
      m_book.InnerCharts.Add( newChart );
    }
    #endregion
  }
}
