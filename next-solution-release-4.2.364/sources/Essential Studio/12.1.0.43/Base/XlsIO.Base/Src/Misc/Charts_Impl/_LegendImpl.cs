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
  /// Summary description for LegendImpl.
  /// </summary>
  public class LegendImpl : IChartLegend
  {
    #region Class members
    private LineStyleImpl m_border;
    private AreaImpl m_area;
    private ExcelLegendPosition m_placement;
    private int m_iXPos;
    private int m_iYPos;
    private int m_iWidth;
    private int m_iHeight;
    #endregion

    #region Class constructors
    public LegendImpl()
    {
      //
      // TODO: Add constructor logic here
      //
    }
    #endregion

    #region IChartLegend Members
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
        // TODO:  Add LegendImpl.Border setter implementation
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
        // TODO:  Add LegendImpl.Area setter implementation
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public IChartFont Font
    {
      get
      {
        // TODO:  Add LegendImpl.Font getter implementation
        return null;
      }
      set
      {
        // TODO:  Add LegendImpl.Font setter implementation
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public Syncfusion.XlsIO.ExcelLegendPosition Placement
    {
      get
      {
        return m_placement;
      }
      set
      {
        m_placement = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public int XPos
    {
      get
      {
        return m_iXPos;
      }
      set
      {
        m_iXPos = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public int YPos
    {
      get
      {
        return m_iYPos;
      }
      set
      {
        m_iYPos = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public int Width
    {
      get
      {
        return m_iWidth;
      }
      set
      {
        m_iWidth = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public int Height
    {
      get
      {
        return m_iHeight;
      }
      set
      {
        m_iHeight = value;
      }
    }
    #endregion
  }
}
