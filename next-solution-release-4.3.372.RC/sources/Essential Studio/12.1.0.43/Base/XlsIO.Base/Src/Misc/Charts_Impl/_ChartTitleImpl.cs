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
  /// Summary description for ChartTitleImpl.
  /// </summary>
  public class ChartTitleImpl
    : CommonObject
    , IChartTitle
  {
    #region Class members
    /// <summary>
    /// 
    /// </summary>
    private int m_iXPos;
    /// <summary>
    /// 
    /// </summary>
    private int m_iYPos;
    /// <summary>
    /// 
    /// </summary>
    private string m_strText;
    /// <summary>
    /// 
    /// </summary>
    private AreaImpl m_Area;
    /// <summary>
    /// 
    /// </summary>
    private LineStyleImpl m_Border;
    /// <summary>
    /// 
    /// </summary>
    private ChartFontImpl m_Font;
    /// <summary>
    /// 
    /// </summary>
    private ExcelChartHorzAlignment m_HAlign;
    /// <summary>
    /// 
    /// </summary>
    private ExcelChartVertAlignment m_VAlign;
    /// <summary>
    /// 
    /// </summary>
    private int m_iTextDirection;
    /// <summary>
    /// 
    /// </summary>
    private int m_iTextOrientation;
    #endregion

    #region Class constructors
    /// <summary>
    /// 
    /// </summary>
    /// <param name="application"></param>
    /// <param name="parent"></param>
    public ChartTitleImpl( IApplication application, object parent )
      : base( application, parent )
    {
    }
    #endregion

    #region IChartTitle Members
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
    public string Text
    {
      get
      {
        return m_strText;
      }
      set
      {
        m_strText = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public IArea Area
    {
      get
      {
        return m_Area;
      }
      set
      {
        // TODO:  Add ChartTitleImpl.Area setter implementation
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public ILineStyle Border
    {
      get
      {
        return m_Border;
      }
      set
      {
        // TODO:  Add ChartTitleImpl.Border setter implementation
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public IChartFont Font
    {
      get
      {
        return m_Font;
      }
      set
      {
        // TODO:  Add ChartTitleImpl.Font setter implementation
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public Syncfusion.XlsIO.ExcelChartHorzAlignment HAlign
    {
      get
      {
        return m_HAlign;
      }
      set
      {
        m_HAlign = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public Syncfusion.XlsIO.ExcelChartVertAlignment VAlign
    {
      get
      {
        return m_VAlign;
      }
      set
      {
        m_VAlign = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public int TextDirection
    {
      get
      {
        return m_iTextDirection;
      }
      set
      {
        m_iTextDirection = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public int TextOrientation
    {
      get
      {
        return m_iTextOrientation;
      }
      set
      {
        m_iTextOrientation = value;
      }
    }

    #endregion
  }
}
