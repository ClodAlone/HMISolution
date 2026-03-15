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
  /// Summary description for AxisGridlines.
  /// </summary>
  public class AxisGridlinesImpl
    : CommonObject
    , IAxisGridlines
  {
    #region Class members
    /// <summary>
    /// 
    /// </summary>
    private LineStyleImpl m_lineStyle;
    /// <summary>
    /// 
    /// </summary>
    private bool m_bAuto;
    /// <summary>
    /// 
    /// </summary>
    private bool m_bMinimum;
    /// <summary>
    /// 
    /// </summary>
    private bool m_bMaximum;
    /// <summary>
    /// 
    /// </summary>
    private bool m_bMajorUnit;
    /// <summary>
    /// 
    /// </summary>
    private bool m_bMinorUnit;
    /// <summary>
    /// 
    /// </summary>
    private bool m_bCrossCategory;
    /// <summary>
    /// 
    /// </summary>
    private int  m_iDisplayUnits;
    /// <summary>
    /// 
    /// </summary>
    private bool m_bLogarithmicScale;
    /// <summary>
    /// 
    /// </summary>
    private bool m_bValuesReserved;
    /// <summary>
    /// 
    /// </summary>
    private bool m_bCatCrossedAtMaxVal;
    /// <summary>
    /// 
    /// </summary>
    private int  m_iValueAxisCross;
    /// <summary>
    /// 
    /// </summary>
    private int  m_iCategoryNumberBetweenTickMarksLabels;
    /// <summary>
    /// 
    /// </summary>
    private int  m_iCategoryNumberBetweenTickMarks;
    /// <summary>
    /// 
    /// </summary>
    private bool m_bValueAxisCrossesBetween;
    /// <summary>
    /// 
    /// </summary>
    private bool m_bValueAxisCrossesAtMaximum;
    /// <summary>
    /// 
    /// </summary>
    private bool m_bCategoriesReversed;
    #endregion

    #region Class constructors
    /// <summary>
    /// 
    /// </summary>
    public AxisGridlinesImpl( IApplication application, object parent )
      : base( application, parent )
    {
      //
      // TODO: Add constructor logic here.
      //
    }
    #endregion

    #region IAxisGridlines Members
    /// <summary>
    /// 
    /// </summary>
    public ILineStyle LineStyle
    {
      get
      {
        return m_lineStyle;
      }
      set
      {
        // TODO:  Add AxisGridlines.LineStyle setter implementation.
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool IsAuto
    {
      get
      {
        return m_bAuto;
      }
      set
      {
        m_bAuto = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool IsMinimum
    {
      get
      {
        return m_bMinimum;
      }
      set
      {
        m_bMinimum = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool IsMaximum
    {
      get
      {
        return m_bMaximum;
      }
      set
      {
        m_bMaximum = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool IsMajorUnit
    {
      get
      {
        return m_bMajorUnit;
      }
      set
      {
        m_bMajorUnit = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool IsMinorUnit
    {
      get
      {
        return m_bMinorUnit;
      }
      set
      {
        m_bMinorUnit = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool IsCrossCategory
    {
      get
      {
        return m_bCrossCategory;
      }
      set
      {
        m_bCrossCategory = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public int DisplayUnits
    {
      get
      {
        return m_iDisplayUnits;
      }
      set
      {
        m_iDisplayUnits = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool IsLogarithmicScale
    {
      get
      {
        return m_bLogarithmicScale;
      }
      set
      {
        m_bLogarithmicScale = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool AreValuesReversed
    {
      get
      {
        return m_bValuesReserved;
      }
      set
      {
        m_bValuesReserved = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool IsCatCrossedAtMaxVal
    {
      get
      {
        return m_bCatCrossedAtMaxVal;
      }
      set
      {
        m_bCatCrossedAtMaxVal = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public int ValueAxisCross
    {
      get
      {
        return m_iValueAxisCross;
      }
      set
      {
        m_iValueAxisCross = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public int CategoryNumberBetweenTickMarksLabels
    {
      get
      {
        return m_iCategoryNumberBetweenTickMarksLabels;
      }
      set
      {
        m_iCategoryNumberBetweenTickMarksLabels = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public int CategoryNumberBetweenTickMarks
    {
      get
      {
        return m_iCategoryNumberBetweenTickMarks;
      }
      set
      {
        m_iCategoryNumberBetweenTickMarks = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool IsValueAxisCrossesBetween
    {
      get
      {
        return m_bValueAxisCrossesBetween;
      }
      set
      {
        m_bValueAxisCrossesBetween = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool IsValueAxisCrossesAtMaximum
    {
      get
      {
        return m_bValueAxisCrossesAtMaximum;
      }
      set
      {
        m_bValueAxisCrossesAtMaximum = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool AreCategoriesReversed
    {
      get
      {
        return m_bCategoriesReversed;
      }
      set
      {
        m_bCategoriesReversed = value;
      }
    }

    #endregion
  }
}
