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
using System.Drawing;

using Syncfusion.XlsIO.Interfaces.Charts;
#endregion

namespace Syncfusion.XlsIO.Implementation.Charts
{
  /// <summary>
  /// Summary description for LineStyleImpl.
  /// </summary>
  public class LineStyleImpl : CommonObject, ILineStyle
  {
    #region Class members
    /// <summary>
    /// 
    /// </summary>
    private ExcelAutoType m_bAutoType = ExcelAutoType.Auto;
    /// <summary>
    /// 
    /// </summary>
    private ExcelLineStyle m_Style = ExcelLineStyle.None;
    /// <summary>
    /// 
    /// </summary>
    private Color m_Color = Color.Black;
    /// <summary>
    /// 
    /// </summary>
    private ExcelChartLineWeight m_Weight = ExcelChartLineWeight.Hairline;
    #endregion

    #region Class constructors
    /// <summary>
    /// 
    /// </summary>
    /// <param name="application"></param>
    /// <param name="parent"></param>
    public LineStyleImpl( IApplication application, object parent )
      : base( application, parent )
    {
    }
    #endregion

    #region ILineStyle Members
    /// <summary>
    /// 
    /// </summary>
    public Syncfusion.XlsIO.ExcelAutoType LineType
    {
      get
      {
        return m_bAutoType;
      }
      set
      {
        m_bAutoType = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public Syncfusion.XlsIO.ExcelLineStyle Style
    {
      get
      {
        return m_Style;
      }
      set
      {
        m_Style = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public System.Drawing.Color Color
    {
      get
      {
        return m_Color;
      }
      set
      {
        m_Color = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public Syncfusion.XlsIO.ExcelChartLineWeight Weight
    {
      get
      {
        return m_Weight;
      }
      set
      {
        m_Weight = value;
      }
    }

    #endregion
  }
}
