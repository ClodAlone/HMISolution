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
  /// Summary description for AxisImpl.
  /// </summary>
  public class AxisImpl
    : CommonObject
    , IAxis
  {
    #region Class members
    /// <summary>
    /// 
    /// </summary>
    private LineStyleImpl m_LineStyle;
    /// <summary>
    /// 
    /// </summary>
    private ChartFontImpl m_Font;
    /// <summary>
    /// 
    /// </summary>
    private AxisGridlinesImpl m_MajorGridlines;
    /// <summary>
    /// 
    /// </summary>
    private AxisGridlinesImpl m_MinorGridlines;
    /// <summary>
    /// 
    /// </summary>
    private string m_strNumberFormat;
    /// <summary>
    /// 
    /// </summary>
    private int m_iMajorTickMark;
    /// <summary>
    /// 
    /// </summary>
    private int m_iMinorTickMark;
    /// <summary>
    /// 
    /// </summary>
    private int m_iTickMarkLabels;
    /// <summary>
    /// 
    /// </summary>
    private int m_iTextOrientation;
    /// <summary>
    /// 
    /// </summary>
    private int m_iTextDirection;
    /// <summary>
    /// 
    /// </summary>
    private int m_iAxisType;

    #endregion

    #region Class constructors
    public AxisImpl( IApplication application, object parent )
      : base( application, parent )
    {
      //
      // TODO: Add constructor logic here.
      //
    }
    #endregion

    #region IAxis Members
    /// <summary>
    /// 
    /// </summary>
    public ILineStyle LineStyle
    {
      get
      {
        return m_LineStyle;
      }
      set
      {
        // TODO:  Add AxisImpl.LineStyle setter implementation.
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
        // TODO:  Add AxisImpl.Font setter implementation.
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public IAxisGridlines MajorGridlines
    {
      get
      {
        return m_MajorGridlines;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public IAxisGridlines MinorGridlines
    {
      get
      {
        return m_MinorGridlines;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public string NumberFormat
    {
      get
      {
        return m_strNumberFormat;
      }
      set
      {
        m_strNumberFormat = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public int MajorTickMark
    {
      get
      {
        return m_iMajorTickMark;
      }
      set
      {
        m_iMajorTickMark = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public int MinorTickMark
    {
      get
      {
        return m_iMinorTickMark;
      }
      set
      {
        m_iMinorTickMark = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public int TickMarkLabels
    {
      get
      {
        return m_iTickMarkLabels;
      }
      set
      {
        m_iTickMarkLabels = value;
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
    public int AxisType
    {
      get
      {
        return m_iAxisType;
      }
    }

    #endregion
  }
}
