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

namespace Syncfusion.XlsIO.Implementation
{
  /// <summary>
  /// This object wraps IConditionValue object to ensure correct parent object update.
  /// </summary>
  public class ConditionValueWrapper : IConditionValue
  {
    #region Members
    /// <summary>
    /// Wrapped item.
    /// </summary>
    private IConditionValue m_wrapped;
    /// <summary>
    /// Parent object.
    /// </summary>
    private IOptimizedUpdate m_parent;
    #endregion

    #region IConditionValue Members
    /// <summary>
    /// Returns one of the constants of the XlConditionValueTypes enumeration,
    /// which specifies how the threshold values for a data bar, color scale,
    /// or icon set conditional format are determined. Read-only.
    /// </summary>
    public ConditionValueType Type
    {
      get
      {
        return m_wrapped.Type;
      }
      set
      {
        BeginUpdate();
        m_wrapped.Type = value;
        EndUpdate();
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
        return m_wrapped.Value;
      }
      set
      {
        BeginUpdate();
        m_wrapped.Value = value;
        EndUpdate();
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
        return m_wrapped.Operator;
     }
     set
     {
        BeginUpdate();
        m_wrapped.Operator = value;
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
      m_parent.BeginUpdate();
    }
    /// <summary>
    /// This method should be called after several updates to the object took place.
    /// </summary>
    public void EndUpdate()
    {
      m_parent.EndUpdate();
    }
    #endregion

    #region Properties
    /// <summary>
    /// Gets / sets wrapped object.
    /// </summary>
    internal IConditionValue Wrapped
    {
      get
      {
        return m_wrapped;
      }
      set
      {
        m_wrapped = value;
      }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Initializes new instance of the wrapped.
    /// </summary>
    /// <param name="value">Object to wrap.</param>
    /// <param name="parent">Parent object.</param>
    public ConditionValueWrapper( IConditionValue value, IOptimizedUpdate parent )
    {
      m_wrapped = value;
      m_parent = parent;
    }
    #endregion
  }
}
