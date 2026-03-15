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
using Syncfusion.XlsIO.Implementation;
using Syncfusion.XlsIO.Interfaces;

namespace Syncfusion.XlsIO
{
  /// <summary>
  /// Represents single condition value for iconset, databar, colorscale conditions.
  /// </summary>
  public interface IConditionValue
  {
    /// <summary>
    /// Returns one of the constants of the XlConditionValueTypes enumeration,
    /// which specifies how the threshold values for a data bar, color scale,
    /// or icon set conditional format are determined. Read-only.
    /// </summary>
    ConditionValueType Type { get; set; }
    /// <summary>
    /// Returns or sets the shortest bar or longest bar threshold value for a data
    /// bar conditional format.
    /// </summary>
    string Value { get; set; }
    /// <summary>
    /// Returns or sets one of the constants of the ConditionalFormatOperator enumeration, 
    /// which specifes if the threshold is "greater than" or "greater than or equal to" the threshold value.
    /// </summary>
    ConditionalFormatOperator Operator { get; set; }

  }
  /// <summary>
  /// Represents implementation of single condition value for iconset, databar, colorscale conditions.
  /// </summary>
  public class ConditionValue : IConditionValue
  {
    #region Members
    /// <summary>
    /// One of the constants of the XlConditionValueTypes enumeration,
    /// which specifies how the threshold values for a data bar, color scale,
    /// or icon set conditional format are determined. Read-only.
    /// </summary>
    private ConditionValueType m_type;
    /// <summary>
    /// Returns or sets the shortest bar or longest bar threshold value for a data
    /// bar conditional format.
    /// </summary>
    private string m_value;
    /// <summary>
    /// Returns or sets the condition value for a Icon set
    /// conditional format.
    /// </summary>
    private ConditionalFormatOperator m_condition = ConditionalFormatOperator.GreaterThanorEqualTo;
    #endregion

    #region Properties
    /// <summary>
    /// Gets/sets one of the constants of the ConditionValueTypes enumeration,
    /// which specifies how the threshold values for a data bar, color scale,
    /// or icon set conditional format are determined.
    /// </summary>
    public ConditionValueType Type
    {
      get
      {
        return m_type;
      }
      set
      {
        m_type = value;
      }
    }
    /// <summary>
    /// Returns or sets the shortest bar or longest bar threshold value for a data
    /// bar conditional format.
    /// </summary>
    public string Value
    {
      get
      {
        return m_value;
      }
      set
      {
        m_value = value;
      }
    }

    /// <summary>
    /// Returns or sets one of the constants of the ConditionalFormatOperator enumeration, 
    /// which specifes if the threshold is "greater than" or "greater than or equal to" the threshold value.
    /// </summary>
    public ConditionalFormatOperator Operator
    {
        get
        {
          return m_condition;
        }
        set
        {          
          m_condition = value;
        }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Equality comparison.
    /// </summary>
    /// <param name="first">The first condition to compare.</param>
    /// <param name="second">The second condition to compare.</param>
    /// <returns>true if conditions are equal.</returns>
    public static bool operator ==( ConditionValue first, ConditionValue second )
    {
      return first.m_type == second.m_type &&
        first.m_value == second.m_value;
    }
    /// <summary>
    /// Checks whether conditions are not equal.
    /// </summary>
    /// <param name="first">The first condition to compare.</param>
    /// <param name="second">The second condition to compare.</param>
    /// <returns>true if conditions are not equal.</returns>
    public static bool operator !=( ConditionValue first, ConditionValue second )
    {
      return !( first == second );
    }
    /// <summary>
    /// Initializes new instance of the class.
    /// </summary>
    /// <param name="type">Condition type.</param>
    /// <param name="value">Condition value.</param>
    public ConditionValue( ConditionValueType type, string value )
    {
      m_type = type;
      m_value = value;
    }
    /// <summary>
    /// Default constructor.
    /// </summary>
    internal ConditionValue()
    {
    }
    /// <summary>
    /// Creates copy of the condition.
    /// </summary>
    /// <returns>Copy of the current object.</returns>
    internal ConditionValue Clone()
    {
      return ( ConditionValue )MemberwiseClone();
    }
    /// <summary>
    /// Determines whether the specified Object is equal to the current Object.
    /// </summary>
    /// <param name="obj">The Object to compare with the current Object.</param>
    /// <returns>true if the specified Object is equal to the current Object; otherwise, false.</returns>
    public override bool Equals( object obj )
    {
      ConditionValue toCheck = obj as ConditionValue;

      return ( obj != null ) ? ( toCheck == this ) : false;
    }
    /// <summary>
    /// Returns the hash code for this instance.
    /// </summary>
    /// <returns>A 32-bit signed integer hash code.</returns>
    public override int GetHashCode()
    {
      return m_type.GetHashCode() ^ m_value.GetHashCode();
    }
    #endregion
  }
  /// <summary>
  /// Specifies the types of condition values that can be used.
  /// </summary>
  public enum ConditionValueType
  {
    /// <summary>
    /// No conditional value.
    /// </summary>
    None = -1,
    /// <summary>
    /// Number is used.
    /// </summary>
    Number = 1,
    /// <summary>
    /// Lowest value from the list of values.
    /// </summary>
    LowestValue = 2,
    /// <summary>
    /// Highest value from the list of values.
    /// </summary>
    HighestValue = 3,
    /// <summary>
    /// Percentage is used. 
    /// </summary>
    Percent = 4,
    /// <summary>
    /// Formula is used.
    /// </summary>
    Formula = 7,
    /// <summary>
    /// Percentile is used.
    /// </summary>
    Percentile = 5,
  }
  /// <summary>
  /// Specifies the type of conditions that can be used for Icon sets
  /// </summary>
  public enum ConditionalFormatOperator
  {
      /// <summary>
      /// Greater than condition is used [>].
      /// </summary>
      GreaterThan=0,
      /// <summary>
      /// Greater Than or Equal to condition is used [>=].
      /// </summary>
      GreaterThanorEqualTo=1,
  }
   
}
