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
using System.Collections.Specialized;
using System.IO;
using System.Diagnostics;
using System.Text;

using Syncfusion.XlsIO.Implementation;
using Syncfusion.XlsIO.Implementation.Shapes;
//using Syncfusion.XlsIO.Interfaces.Collections;
#endregion

namespace Syncfusion.XlsIO.Implementation.Collections
{
  /// <summary>
  ///
  /// </summary>
  public class WorksheetChartsCollection
    : CollectionBaseEx<object>
    , IChartShapes
  {
    #region Class members
    /// <summary>
    /// Parent worksheet.
    /// </summary>
    private WorksheetBaseImpl m_sheet;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="application"></param>
    /// <param name="parent"></param>
    public WorksheetChartsCollection( IApplication application, object parent )
      : base( application, parent )
    {
      SetParents();
    }
    #endregion

    #region Class Methods
    /// <summary>
    /// Adds chart.
    /// </summary>
    /// <returns>Added chart.</returns>
    public IChart AddChart()
    {
      IChart chart = m_sheet.Shapes.AddChart();
      InnerList.Add( chart );

      return chart;
    }

    /// <summary>
    /// Adds new chart to the collection (doesn't add it to the shapes collection).
    /// </summary>
    /// <param name="chart">Chart to add.</param>
    /// <returns>Added chart.</returns>
    protected internal IChartShape InnerAddChart( IChartShape chart )
    {
      if( chart == null )
        throw new ArgumentNullException( "chart" );

      InnerList.Add( chart );
      return chart;
    }
    /// <summary>
    /// Adds new chart to the collection.
    /// </summary>
    /// <param name="chart">Chart to add.</param>
    /// <returns>Added chart.</returns>
    protected internal IChartShape AddChart( IChartShape chart )
    {
      if( chart == null )
        throw new ArgumentNullException( "chart" );

      InnerAddChart( chart );
      m_sheet.InnerShapes.AddShape( ( ShapeImpl )chart );

      return chart;
    }
    /// <summary>
    /// Searched for all necessary parents.
    /// </summary>
    /// <exception cref="System.ArgumentNullException">
    /// If can't find parent worksheet.
    /// </exception>
    private void SetParents()
    {
      m_sheet = FindParent( typeof( WorksheetBaseImpl ), true ) as WorksheetBaseImpl;

      if( m_sheet == null )
        throw new ArgumentNullException( "Can't find parent worksheet." );
    }
    #endregion

    #region ICharts Members
    /// <summary>
    /// Returns a single Chart object from a Charts collection.
    /// </summary>
    public IChartShape this[ int index ]
    {
      get
      {
        if( index < 0 || index >= InnerList.Count )
          throw new ArgumentOutOfRangeException( "Chart index" );

        return InnerList[ index ] as IChartShape;
      }
    }

    /// <summary>
    /// Creates a new chart.
    /// </summary>
    /// <returns>Newly created chart object.</returns>
    public IChartShape Add()
    {
      IChartShape chart = m_sheet.Shapes.AddChart();
      return chart;//InnerAddChart( chart );
    }

    /// <summary>
    ///   Remove chart with specified index.
    /// </summary>
    /// <param name="index">Index of chart to remove.</param>
    new public void RemoveAt( int index )
    {
      if( index < 0 || index >= Count )
        throw new ArgumentOutOfRangeException( "index" );

      IChartShape chart = InnerList[ index ] as IChartShape;

      InnerList.RemoveAt( index );
      chart.Remove();
    }

    #endregion
  }
}
