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

using Syncfusion.XlsIO.Interfaces.Charts;
#endregion

namespace Syncfusion.XlsIO.Implementation.Charts
{
  /// <summary>
  /// Summary description for PlotAreaImpl.
  /// </summary>
  public class PlotAreaImpl
    : CommonObject
    , IChartPlotArea
  {
    #region Class members;
    /// <summary>
    /// 
    /// </summary>
    private LineStyleImpl m_border;
    /// <summary>
    /// 
    /// </summary>
    private AreaImpl m_area;
    #endregion

    #region Class constructor
    public PlotAreaImpl( IApplication application, object parent )
      : base( application, parent )
    {
      //
      // TODO: Add constructor logic here
      //
    }
    #endregion

    #region IChartPlotArea Members
    /// <summary>
    /// 
    /// </summary>
    public ILineStyle Border
    {
      get
      {
        return m_border;
      }
      set
      {
        // TODO:  Add PlotAreaImpl.Border setter implementation
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public IArea Area
    {
      get
      {
        return m_area;
      }
      set
      {
        // TODO:  Add PlotAreaImpl.Area setter implementation
      }
    }

    #endregion
  }
}
