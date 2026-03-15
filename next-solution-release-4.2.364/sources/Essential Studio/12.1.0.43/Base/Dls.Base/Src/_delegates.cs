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
#endregion

namespace Syncfusion.DLS
{
  /// <summary>
  /// Rpresents ValueChangedEventArgs
  /// </summary>
  public class ValueChangedEventArgs : EventArgs
  {
    #region Class members
    /// <summary>
    ///
    /// </summary>
    private object m_old;
    /// <summary>
    ///
    /// </summary>
    private object m_new;
    /// <summary>
    /// 
    /// </summary>
    private string m_strProperty;
    #endregion

    #region Class properties
    /// <summary>
    /// Represents an event with no event data.
    /// </summary>
    new public static ValueChangedEventArgs Empty
    {
      get
      {
        return ( EventArgs.Empty as ValueChangedEventArgs );
      }
    }
    /// <summary>
    /// Represents new value.   
    /// </summary>
    public object NewValue
    {
      get
      {
        return m_new;
      }
    }
    /// <summary>
    /// Represents old value.   
    /// </summary>
    public object OldValue
    {
      get
      {
        return m_old;
      }
    }
    /// <summary>
    /// Gets property name.
    /// </summary>
    public string PropertyName
    {
      get
      {
        return m_strProperty;
      }
    }
    #endregion

    #region Class initialize/finalize methods
    /// <summary>
    ///
    /// </summary>
    private ValueChangedEventArgs()
    {
    }
    /// <summary>
    ///
    /// </summary>
    /// <param name="old"></param>
    /// <param name="newValue"></param>
    public ValueChangedEventArgs( object old, object newValue )
      : this( string.Empty, old, newValue )
    {
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="old"></param>
    /// <param name="newValue"></param>
    public ValueChangedEventArgs( string name, object old, object newValue )
    {
      m_strProperty = name;
      m_old = old;
      m_new = newValue;
    }
    #endregion
  }

  /// <summary>
  /// Represents a method used for handling Changed event.
  /// </summary>
  public delegate void ValueChangedEventHandler( object sender, ValueChangedEventArgs e );
}