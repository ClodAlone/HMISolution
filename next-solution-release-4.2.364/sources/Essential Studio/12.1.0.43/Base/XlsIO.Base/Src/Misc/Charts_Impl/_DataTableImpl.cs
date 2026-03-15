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
  /// Summary description for DataTableImpl.
  /// </summary>
  public class DataTableImpl : IDataTable
  {
    #region Class members
    private LineStyleImpl m_LineStyle;
    private bool m_bShowLegendKeys = false;
    private bool m_bHasHLines = true;
    private bool m_bHasVLines = true;
    private bool m_bHasOutlineLines = true;
    #endregion

    #region Class constructors
    /// <summary>
    /// 
    /// </summary>
    /// <param name="application"></param>
    /// <param name="parent"></param>
    public DataTableImpl( IApplication application, object parent )
    {
      //
      // TODO: Add constructor logic here
      //
    }
    #endregion

    #region IDataTable Members
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
        // TODO: implementation of LineStyle setter
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool IsShowLegendKeys
    {
      get
      {
        return m_bShowLegendKeys;
      }
      set
      {
        m_bShowLegendKeys = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool HasHorizontalLines
    {
      get
      {
        return m_bHasHLines;
      }
      set
      {
        m_bHasHLines = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool HasVerticalLines
    {
      get
      {
        return m_bHasVLines;
      }
      set
      {
        m_bHasVLines = false;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool HasOutlineLines
    {
      get
      {
        return m_bHasOutlineLines;
      }
      set
      {
        m_bHasOutlineLines = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public IChartFont Font
    {
      get
      {
        // TODO:  Add DataTableImpl.Font getter implementation
        return null;
      }
      set
      {
        // TODO:  Add DataTableImpl.Font setter implementation
      }
    }

    #endregion
  }
}
