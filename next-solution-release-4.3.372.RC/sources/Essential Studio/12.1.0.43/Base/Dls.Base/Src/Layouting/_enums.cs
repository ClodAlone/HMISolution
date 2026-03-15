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

namespace Syncfusion.Layouting
{
  #region enum HorizonatalAlignment
  /// <summary>
  /// 
  /// </summary>
  public enum HorizontalAlignment
  {
    /// <summary>
    /// 
    /// </summary>
    Left,
    /// <summary>
    /// 
    /// </summary>
    Center,
    /// <summary>
    /// 
    /// </summary>
    Right,
    /// <summary>
    /// 
    /// </summary>
    Justify
  }
  #endregion
  
  #region enum LayoutState
  /// <summary>
  /// Represents state information of LayoutContext object
  /// </summary>
  public enum LayoutState
  {
    /// <summary>
    /// Unknown state, set by itialization of LayoutContext
    /// <remarks>Cann't returned to parent context</remarks>
    /// </summary>
    Unknown,
    /// <summary>
    /// Current context contains at last one child. Layouting process on 
    /// current level not finished. 
    /// <remarks>Cann't returned to parent context</remarks>
    /// </summary>
    //Fitting,
    /// <summary>
    /// Current context can not contains childs widgets or children contexts.
    /// <remarks>Widget of current context didn't fit</remarks>
    /// </summary>
    NotFitted,
    /// <summary>
    /// Current context contains at last one child. Last widget 
    /// ( or child context ) was splitted.
    /// </summary>
    Splitted,
    /// <summary>
    /// Current context contains at last one child. Layouting process 
    /// finished. 
    /// </summary>
    Fitted,
    /// <summary>
    /// Current context contains at last one child. Layouting process 
    /// breaked. 
    /// </summary>
    Breaked
  }
  #endregion
  
  #region enum TabJustification
  /// <summary>
  /// 
  /// </summary>
  public enum TabJustification
  {
    /// <summary>
    /// Left tab.
    /// </summary>
    Left = 0,
    /// <summary>
    /// Centered tab.
    /// </summary>
    Centered = 1,
    /// <summary>
    /// Right tab.
    /// </summary>
    Right = 2,
    /// <summary>
    /// Decimal tab.
    /// </summary>
    Decimal = 3,
    /// <summary>
    /// Bar.
    /// </summary>
    Bar = 4
  }
  #endregion
  
  #region enum TabLeader
  /// <summary>
  /// 
  /// </summary>
  public enum TabLeader
  {
    /// <summary>
    /// No leader.
    /// </summary>
    NoLeader = 0,
    /// <summary>
    /// Dotted leader.
    /// </summary>
    Dotted = 1,
    /// <summary>
    /// Hyphenated leader.
    /// </summary>
    Hyphenated = 2,
    /// <summary>
    /// Single line leader. 
    /// </summary>
    Single = 3,
    /// <summary>
    /// Heavy line leader.
    /// </summary>
    Heavy = 4
  }
  #endregion
  
  #region enum ChildrenLayoutDirection
  /// <summary>
  /// 
  /// </summary>
  public enum ChildrenLayoutDirection
  {
    /// <summary>
    /// 
    /// </summary>
    Horizontal,
    /// <summary>
    /// 
    /// </summary>
    Vertical
  }
  #endregion
}