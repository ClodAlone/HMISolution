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
  /// Summary description for WallsImpl.
  /// </summary>
  public class WallsImpl : CommonObject, IChartWalls
  {
    #region Class members
    /// <summary>
    /// 
    /// </summary>
    private LineStyleImpl m_border;
    /// <summary>
    /// 
    /// </summary>
    private AreaImpl m_area;
    #endregion

    #region Class constructors
    /// <summary>
    /// 
    /// </summary>
    /// <param name="application"></param>
    /// <param name="parent"></param>
    public WallsImpl( IApplication application, object parent )
      : base( application, parent )
    {
      //
      // TODO: Add constructor logic here
      //
    }
    #endregion

    #region IChartWalls Members
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
        // TODO:  Add WallsImpl.Border setter implementation
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
        // TODO:  Add WallsImpl.Area setter implementation
      }
    }

    #endregion
  }
}
