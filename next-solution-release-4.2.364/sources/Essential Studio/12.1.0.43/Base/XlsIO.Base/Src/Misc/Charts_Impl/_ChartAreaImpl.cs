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
  /// Summary description for ChartAreaImpl.
  /// </summary>
  public class ChartAreaImpl : CommonObject, IChartArea
  {
    #region Class members
    private LineStyleImpl m_lineStyle;
    private AreaImpl m_area;
    private ChartFontImpl m_font;
    #endregion

    #region Class constructors
    /// <summary>
    /// 
    /// </summary>
    /// <param name="application"></param>
    /// <param name="parent"></param>
    public ChartAreaImpl( IApplication application, object parent )
      : base( application, parent )
    {
    }
    #endregion

    #region IChartArea Members
    /// <summary>
    /// 
    /// </summary>
    public ILineStyle Border
    {
      get
      {
        return m_lineStyle;
      }
      set
      {
        // TODO:  Add ChartAreaImpl.Border setter implementation
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
        // TODO:  Add ChartAreaImpl.Area setter implementation
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public IChartFont Font
    {
      get
      {
        return m_font;
      }
      set
      {
        // TODO:  Add ChartAreaImpl.Font setter implementation
      }
    }

    #endregion
  }
}
