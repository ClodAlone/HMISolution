#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using Syncfusion.XlsIO.Interfaces;

#if ( WINRT )
using Windows.UI;
using Rectangle = Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
#endif

#if  (SILVERLIGHT)
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.Silverlight.Rectangle;
#elif WP
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.WP.Rectangle;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;


#endif


namespace Syncfusion.XlsIO.Implementation
{
  /// <summary>
  /// Represents a wrapper over data bar conditional formatting rule. Applying
  /// a data bar to a range helps you see the value of a cell relative to other cells.
  /// </summary>
  class DataBarWrapper :
    IDataBar,
    IOptimizedUpdate
  {
    #region Members
    /// <summary>
    /// Wrapped data bar object.
    /// </summary>
    private DataBarImpl m_wrapped;
    /// <summary>
    /// Parent conditional format wrapper.
    /// </summary>
    private ConditionalFormatWrapper m_format;
    private ConditionValueWrapper m_minPoint;
    private ConditionValueWrapper m_maxPoint;
    #endregion

    #region IDataBar Members
    /// <summary>
    /// Returns a ConditionValue object which specifies how the shortest bar is evaluated
    /// for a data bar conditional format.
    /// </summary>
    public IConditionValue MinPoint
    {
      get
      {
        return m_minPoint;//m_wrapped.MinPoint;
      }
      //set
      //{
      //  BeginUpdate();
      //  m_wrapped.MinPoint = value;
      //  EndUpdate();
      //}
    }
    /// <summary>
    /// Returns a ConditionValue object which specifies how the longest bar is evaluated
    /// for a data bar conditional format.
    /// </summary>
    public IConditionValue MaxPoint
    {
      get
      {
        return m_maxPoint;//m_wrapped.MaxPoint;
      }
      //set
      //{
      //  BeginUpdate();
      //  m_wrapped.MaxPoint = value;
      //  EndUpdate();
      //}
    }
    /// <summary>
    /// Gets/sets the color of the bars in a data bar conditional format.
    /// </summary>
    public Color BarColor
    {
      get
      {
        return m_wrapped.BarColor;
      }
      set
      {
        BeginUpdate();
        m_wrapped.BarColor = value;
        EndUpdate();
      }
    }
    /// <summary>
    /// Returns or sets a value that specifies the length of the longest
    /// data bar as a percentage of cell width.
    /// </summary>
    public int PercentMax
    {
      get
      {
        return m_wrapped.PercentMax;
      }
      set
      {
        BeginUpdate();
        m_wrapped.PercentMax = value;
        EndUpdate();
      }
    }
    /// <summary>
    /// Returns or sets a value that specifies the length of the shortest
    /// data bar as a percentage of cell width.
    /// </summary>
    public int PercentMin
    {
      get
      {
        return m_wrapped.PercentMin;
      }
      set
      {
        BeginUpdate();
        m_wrapped.PercentMin = value;
        EndUpdate();
      }
    }
    /// <summary>
    /// Returns or sets a Boolean value that specifies if the value in the cell
    /// is displayed if the data bar conditional format is applied to the range.
    /// </summary>
    public bool ShowValue
    {
      get
      {
        return m_wrapped.ShowValue;
      }
      set
      {
        BeginUpdate();
        m_wrapped.ShowValue = value;
        EndUpdate();
      }
    }
    /// <summary>
    /// Gets or sets the axis color of the data bar. 
    /// This element MUST exist if and only if axisPosition does not equal "none".
    /// </summary>
    public Color BarAxisColor
    {
        get
        {
            return m_wrapped.BarAxisColor;
        }
        set
        {
            BeginUpdate();
            m_wrapped.BarAxisColor = value;
            EndUpdate();
        }
    }
    /// <summary>
    /// Gets or sets the border color of the data bar. 
    /// This element MUST exist if and only if border equals "true".
    /// </summary>
    public Color BorderColor
    {
        get
        {
            return m_wrapped.BorderColor;
        }
        set
        {
            BeginUpdate();
            m_wrapped.BorderColor = value;
            EndUpdate();
        }
    }
    /// <summary>
    /// Gets whether the data bar has a border
    /// </summary>
    public bool HasBorder
    {
        get
        {
            return m_wrapped.HasBorder;
        }
    }
    /// <summary>
    /// Gets or sets whether the data bar has a gradient fill.
    /// </summary>
    public bool HasGradientFill
    {
        get
        {
            return m_wrapped.HasGradientFill;
        }
        set
        {
            m_wrapped.HasGradientFill = value;
        }
    }
    /// <summary>
    /// Gets or sets the direction of the data bar.
    /// </summary>
    public DataBarDirection DataBarDirection
    {
        get
        {
            return m_wrapped.DataBarDirection;
        }
        set
        {
            BeginUpdate();
            m_wrapped.DataBarDirection = value;
            EndUpdate();
        }
    }
    /// <summary>
    /// Gets or sets the negative border color of the data bar. 
    /// This element MUST exist if and only if negativeBarBorderColorSameAsPositive equals "false" and border equals "true".
    /// </summary>
    public Color NegativeBorderColor
    {
        get
        {
            return m_wrapped.NegativeBorderColor;
        }
        set
        {
            BeginUpdate();
            m_wrapped.NegativeBorderColor = value;
            EndUpdate();
        }
    }
    /// <summary>
    /// Gest or sests the negative fill color of the data bar. 
    /// This element MUST exist if and only if negativeBarColorSameAsPositive equals "false".
    /// </summary>
    public Color NegativeFillColor
    {
        get
        {
            return m_wrapped.NegativeFillColor;
        }
        set
        {
            BeginUpdate();
            m_wrapped.NegativeFillColor = value;
            EndUpdate();
        }
    }
    /// <summary>
    /// Gets or sets the axis position for the data bar
    /// </summary>
    public DataBarAxisPosition DataBarAxisPosition
    {
        get
        {
            return m_wrapped.DataBarAxisPosition;
        }
        set
        {
            BeginUpdate();
            m_wrapped.DataBarAxisPosition = value;
            EndUpdate();
        }
    }
    #endregion

    #region IOptimizedUpdate Members
    /// <summary>
    /// This method should be called before several updates to the object will take place.
    /// </summary>
    public void BeginUpdate()
    {
      m_format.BeginUpdate();
      m_wrapped = m_format.GetCondition().DataBar as DataBarImpl;
    }
    /// <summary>
    /// This method should be called after several updates to the object took place.
    /// </summary>
    public void EndUpdate()
    {
      m_format.EndUpdate();
    }
    #endregion

    #region Methods
    /// <summary>
    /// Initializes new instance of the wrapper.
    /// </summary>
    /// <param name="dataBar">Data bar object to wrap.</param>
    /// <param name="format">Parent conditional format wrapper.</param>
    public DataBarWrapper( DataBarImpl dataBar, ConditionalFormatWrapper format )
    {
      m_wrapped = dataBar;
      m_format = format;
      m_minPoint = new ConditionValueWrapper( dataBar.MinPoint, format );
      m_maxPoint = new ConditionValueWrapper( dataBar.MaxPoint, format );
    }
    #endregion
  }
}
