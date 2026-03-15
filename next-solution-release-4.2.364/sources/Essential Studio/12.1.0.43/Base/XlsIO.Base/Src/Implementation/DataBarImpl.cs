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
using Syncfusion.XlsIO.Implementation.XmlSerialization;
using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO;

#if ( WINRT )
using Windows.UI;
using Rectangle = Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
#endif

#if  (SILVERLIGHT || WP)
using System.Windows.Media;
#elif ( WINRT )
#else
using System.Drawing;


#endif

namespace Syncfusion.XlsIO.Implementation
{
  /// <summary>
  /// Represents a data bar conditional formatting rule. Applying a data bar to a
  /// range helps you see the value of a cell relative to other cells.
  /// </summary>
  class DataBarImpl :
    IDataBar,
    ICloneable
  {
    #region Constants
    /// <summary>
    /// Default data bar color.
    /// </summary>
    private static readonly Color DefaultColor = Color.FromArgb( 0xFF, 0x63, 0x8E, 0xC6 );
    /// <summary>
    /// Default axis position of data bar
    /// </summary>
    private static readonly DataBarAxisPosition DefaultAxisPosition = DataBarAxisPosition.none;
    /// <summary>
    /// Default data bar direction
    /// </summary>
    private static readonly DataBarDirection DefaultDataBarDirection = DataBarDirection.context;
    #endregion

    #region Members
    /// <summary>
    /// A ConditionValue object which specifies how the shortest bar is evaluated
    /// for a data bar conditional format.
    /// </summary>
    private IConditionValue m_minPoint = new ConditionValue( ConditionValueType.LowestValue, "0" );
    /// <summary>
    /// A ConditionValue object which specifies how the longest bar is evaluated
    /// for a data bar conditional format.
    /// </summary>
    private IConditionValue m_maxPoint = new ConditionValue( ConditionValueType.HighestValue, "0" );
    /// <summary>
    /// The color of the bars in a data bar conditional format.
    /// </summary>
    private Color m_barColor = DefaultColor;
    /// <summary>
    /// A value that specifies the length of the longest
    /// data bar as a percentage of cell width.
    /// </summary>
    private int m_iPercentMax = CF.DefaultDataBarMaxLength;
    /// <summary>
    /// A value that specifies the length of the shortest
    /// data bar as a percentage of cell width.
    /// </summary>
    private int m_iPercentMin = CF.DefaultDataBarMinLength;
    /// <summary>
    /// Returns or sets a Boolean value that specifies if the value in the cell
    /// is displayed if the data bar conditional format is applied to the range.
    /// </summary>
    private bool m_bShowValue = true;
    /// <summary>
    /// Represents the axis color of the data bar. 
    /// </summary>
    private Color m_axisColor;
    /// <summary>
    /// Represents the border color of the data bar. 
    /// </summary>
    private Color m_borderColor;
    /// <summary>
    /// Represents the negative border color of the data bar. 
    /// </summary>
    private Color m_negativeBorderColor;
    /// <summary>
    /// Represents the negative fill color of the data bar.
    /// </summary>
    private Color m_negativeFillColor;
    /// <summary>
    /// Represents whether the data bar has a border.
    /// </summary>
    private bool m_bHasBorder = false;
    /// <summary>
    /// Represents the direction of the data bar.
    /// </summary>
    private DataBarDirection m_direction = DefaultDataBarDirection;
    /// <summary>
    /// Represents whether the data bar has a gradient fill.
    /// </summary>
    private bool m_bHasGradientFill = true;
    /// <summary>
    /// Represents whether the data bar has a negative bar color 
    /// that is different from the positive bar color.
    /// </summary>
    private bool m_bHasDiffNegativeBarColor = false;
    /// <summary>
    /// Represents whether the data bar has a negative border color 
    /// that is different from the positive border color.
    /// </summary>
    private bool m_bHasDiffNegativeBarBorderColor = false;
    /// <summary>
    /// Represents the axis position for the data bar
    /// </summary>
    private DataBarAxisPosition m_axisPosition = DefaultAxisPosition;
    /// <summary>
    /// Represents whether the data bar has extension list or not
    /// </summary>
    private bool m_hasExtensionList;
    /// <summary>
    /// Represents the GUID for the data bar extension list
    /// </summary>
    internal string ST_GUID;
    #endregion

    #region Class Properties
    /// <summary>
    /// Returns a ConditionValue object which specifies how the shortest bar is evaluated
    /// for a data bar conditional format.
    /// </summary>
    public IConditionValue MinPoint
    {
      get
      {
        return m_minPoint;
      }
      set
      {
        m_minPoint = value;
      }
    }
    /// <summary>
    /// Returns a ConditionValue object which specifies how the longest bar is evaluated
    /// for a data bar conditional format.
    /// </summary>
    public IConditionValue MaxPoint
    {
      get
      {
        return m_maxPoint;
      }
      set
      {
        m_maxPoint = value;
      }
    }
    /// <summary>
    /// Gets/sets the color of the bars in a data bar conditional format.
    /// </summary>
    public Color BarColor
    {
      get
      {
        //throw new Exception( "The method or operation is not implemented." );
        return m_barColor;
      }
      set
      {
        m_barColor = value;
        //throw new Exception( "The method or operation is not implemented." );
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
        return m_iPercentMax;
      }
      set
      {
        m_iPercentMax = value;
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
        return m_iPercentMin;
      }
      set
      {
        m_iPercentMin = value;
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
        return m_bShowValue;
      }
      set
      {
        m_bShowValue = value;
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
            return m_axisColor;
        }
        set
        {
            m_axisColor = value;
            HasExtensionList = true;
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
            return m_borderColor;
        }
        set
        {
            m_borderColor = value;
            m_bHasBorder = true;            
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
            return m_negativeBorderColor;
        }
        set
        {
            m_negativeBorderColor = value;
            HasExtensionList = true;
            HasDiffNegativeBarBorderColor = true;
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
            return m_negativeFillColor;
        }
        set
        {
            m_negativeFillColor = value;
            HasExtensionList = true;
            HasDiffNegativeBarColor = true;
        }
    }
    /// <summary>
    /// Gets whether the data bar has a border
    /// </summary>
    public bool HasBorder
    {
        get
        {
            return m_bHasBorder;
        }
    }
    /// <summary>
    /// Gets or sets the direction of the data bar.
    /// </summary>
    public DataBarDirection DataBarDirection
    {
        get
        {
            return m_direction;
        }
        set
        {
            m_direction = value;
            HasExtensionList = true;
        }
    }
    /// <summary>
    /// Gets or sets whether the data bar has a gradient fill.
    /// </summary>
    public bool HasGradientFill
    {
        get
        {
            return m_bHasGradientFill;
        }
        set
        {
            m_bHasGradientFill = value;
            HasExtensionList = true;
        }
    }
    /// <summary>
    /// Represents whether the data bar has a negative bar color 
    /// that is different from the positive bar color.
    /// </summary>
    internal bool HasDiffNegativeBarColor
    {
        get
        {
            return m_bHasDiffNegativeBarColor;
        }
        set
        {
            m_bHasDiffNegativeBarColor = value;
        }
    }
    /// <summary>
    /// Represents whether the data bar has a negative border color 
    /// that is different from the positive border color.
    /// </summary>
    internal bool HasDiffNegativeBarBorderColor
    {
        get
        {
            return m_bHasDiffNegativeBarBorderColor;
        }
        set
        {
            m_bHasDiffNegativeBarBorderColor = value;
        }
    }
    /// <summary>
    /// Gets or sets the axis position for the data bar
    /// </summary>
    public DataBarAxisPosition DataBarAxisPosition
    {
        get
        {
            return m_axisPosition;
        }
        set
        {
            m_axisPosition = value;
            HasExtensionList = true;
        }
    }
    /// <summary>
    /// Gets or sets the value whether the data bar has extension list or not
    /// </summary>
    internal bool HasExtensionList
    {
        get
        {
            return m_hasExtensionList;
        }
        set
        {
            m_hasExtensionList = value;

            if (ST_GUID == null)
            {
                ST_GUID = "{" + Guid.NewGuid().ToString() + "}";
            }
        }
    }
    #endregion

    #region Clonable Methods
    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    internal DataBarImpl Clone()
    {
      return ( DataBarImpl )MemberwiseClone();
      //throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    object ICloneable.Clone()
    {
      return Clone();
    }
    #endregion

    #region Methods
    /// <summary>
    /// Serves as a hash function for a particular type. GetHashCode() is suitable
    /// for use in hashing algorithms and data structures like a hash table.
    /// </summary>
    /// <returns>A hash code for the current object.</returns>
    public override int GetHashCode()
    {
      return base.GetHashCode() ^
        m_minPoint.GetHashCode() ^
        m_maxPoint.GetHashCode() ^
        m_barColor.GetHashCode() ^
        //m_strFormula.GetHashCode() ^
        m_iPercentMax.GetHashCode() ^
        m_iPercentMin.GetHashCode() ^
        //m_iPriority.GetHashCode() ^
        m_bShowValue.GetHashCode();
    }
    /// <summary>
    /// A hash code for the current Object without taking cell list into account.
    /// </summary>
    /// <param name="obj">The Object to compare with the current Object.</param>
    /// <returns></returns>
    public override bool Equals( object obj )
    {
      DataBarImpl dataBar = obj as DataBarImpl;

      if( dataBar == null )
        return false;

      return this == dataBar;
    }
    /// <summary>
    /// Compares two instances of the DataBarImpl.
    /// </summary>
    /// <param name="first">First object to compare.</param>
    /// <param name="second">Second object to compare.</param>
    /// <returns>True if objects are equal.</returns>
    public static bool operator==( DataBarImpl first, DataBarImpl second )
    {
      if( ( object )first == null && ( object )second == null )
        return true;

      if( ( object )first == null || ( object )second == null )
        return false;

      return first.m_minPoint == second.m_minPoint &&
        first.m_maxPoint == second.m_maxPoint &&
        first.m_barColor.ToArgb() == second.m_barColor.ToArgb() &&
        //first.m_strFormula == second.m_strFormula &&
        first.m_iPercentMax == second.m_iPercentMax &&
        first.m_iPercentMin == second.m_iPercentMin &&
        //first.m_iPriority == second.m_iPriority &&
        first.m_bShowValue == second.m_bShowValue;
    }
    /// <summary>
    /// Checks whether objects are different.
    /// </summary>
    /// <param name="first">First object to compare.</param>
    /// <param name="second">Second object to compare.</param>
    /// <returns>True if objects are not equal.</returns>
    public static bool operator !=( DataBarImpl first, DataBarImpl second )
    {
      return !( first == second );
    }
    #endregion
  }
}
