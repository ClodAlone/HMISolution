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
  /// Summary description for SerieDataLabelsImpl.
  /// </summary>
  public class SerieDataLabelsImpl
    : CommonObject
    , ISerieDataLabels
  {
    #region Class Members
    /// <summary>
    /// 
    /// </summary>
    private bool m_bSeriesName;
    /// <summary>
    /// 
    /// </summary>
    private bool m_bCategoryName;
    /// <summary>
    /// 
    /// </summary>
    private bool m_bValue;
    /// <summary>
    /// 
    /// </summary>
    private bool m_bPercentage;
    /// <summary>
    /// 
    /// </summary>
    private bool m_bBubbleSize;
    /// <summary>
    /// 
    /// </summary>
    private bool m_bLegendKey;
    /// <summary>
    /// 
    /// </summary>
    private char m_cSeparator;
    #endregion

    #region Class constructors
    /// <summary>
    /// 
    /// </summary>
    /// <param name="application"></param>
    /// <param name="parent"></param>
    public SerieDataLabelsImpl( IApplication application, object parent )
      : base( application, parent )
    {
    }
    #endregion

    #region ISerieDataLabels Members
    /// <summary>
    /// 
    /// </summary>
    public bool IsSeriesName
    {
      get
      {
        return m_bSeriesName;
      }
      set
      {
        m_bSeriesName = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool IsCategoryName
    {
      get
      {
        return m_bCategoryName;
      }
      set
      {
        m_bCategoryName = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool IsValue
    {
      get
      {
        return m_bValue;
      }
      set
      {
        m_bValue = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool IsPercentage
    {
      get
      {
        return m_bPercentage;
      }
      set
      {
        m_bPercentage = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool IsBubbleSize
    {
      get
      {
        return m_bBubbleSize;
      }
      set
      {
        m_bBubbleSize = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool IsLegendKey
    {
      get
      {
        return m_bLegendKey;
      }
      set
      {
        m_bLegendKey = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public char Separator
    {
      get
      {
        return m_cSeparator;
      }
      set
      {
        m_cSeparator = value;
      }
    }

    #endregion
  }
}
