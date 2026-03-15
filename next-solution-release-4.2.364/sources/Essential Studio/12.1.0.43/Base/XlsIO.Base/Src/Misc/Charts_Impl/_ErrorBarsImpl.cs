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
  /// Summary description for ErrorBarsImpl.
  /// </summary>
  public class ErrorBarsImpl : CommonObject, IChartErrorBars
  {
    #region Class members
    /// <summary>
    /// 
    /// </summary>
    private bool m_bFixedValue;
    /// <summary>
    /// 
    /// </summary>
    private bool m_bPercentage;
    /// <summary>
    /// 
    /// </summary>
    private bool m_bStandardDeviation;
    /// <summary>
    /// 
    /// </summary>
    private bool m_bStandardError;
    /// <summary>
    /// 
    /// </summary>
    private bool m_bCustom;
    /// <summary>
    /// 
    /// </summary>
    private double m_FixedValue;
    /// <summary>
    /// 
    /// </summary>
    private double m_Percentage;
    /// <summary>
    /// 
    /// </summary>
    private double m_StandardDeviation;
    /// <summary>
    /// 
    /// </summary>
    private string m_strFormulaPlus;
    /// <summary>
    /// 
    /// </summary>
    private string m_strFormulaMinus;
    /// <summary>
    /// 
    /// </summary>
    private LineStyleImpl m_lineStyle;
    #endregion

    #region Class constructors
    /// <summary>
    /// 
    /// </summary>
    /// <param name="application"></param>
    /// <param name="parent"></param>
    public ErrorBarsImpl( IApplication application, object parent )
      : base( application, parent )
    {
      //
      // TODO: Add constructor logic here
      //
    }
    #endregion

    #region IChartErrorBars Members
    /// <summary>
    /// 
    /// </summary>
    public bool IsFixedValue
    {
      get
      {
        return m_bFixedValue;
      }
      set
      {
        if( m_bFixedValue != value )
        {
          m_bFixedValue = value;
        }
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
    public bool IsStandardDeviation
    {
      get
      {
        return m_bStandardDeviation;
      }
      set
      {
        m_bStandardDeviation = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool IsStandardError
    {
      get
      {
        return m_bStandardError;
      }
      set
      {
        m_bStandardError = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool IsCustom
    {
      get
      {
        return m_bCustom;
      }
      set
      {
        m_bCustom = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public double FixedValue
    {
      get
      {
        return m_FixedValue;
      }
      set
      {
        m_FixedValue = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public double Percentage
    {
      get
      {
        return m_Percentage;
      }
      set
      {
        m_Percentage = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public double StandardDeviation
    {
      get
      {
        return m_StandardDeviation;
      }
      set
      {
        m_StandardDeviation = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public string FormulaPlus
    {
      get
      {
        return m_strFormulaPlus;
      }
      set
      {
        m_strFormulaPlus = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public string FormulaMinus
    {
      get
      {
        return m_strFormulaMinus;
      }
      set
      {
        m_strFormulaMinus = value;
      }
    }

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
        // TODO:  Add ErrorBarsImpl.LineStyle setter implementation
      }
    }

    #endregion
  }
}
