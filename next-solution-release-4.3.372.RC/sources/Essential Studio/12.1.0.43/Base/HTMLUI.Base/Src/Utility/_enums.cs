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

using System;

namespace Syncfusion.HTMLUI.Base
{
  /// <summary>
  /// Enumerator which indicates where destination document is.
  /// </summary>
  public enum ResourceType
  {
    #region Enum Flags
    /// <summary>
    /// Unknown type.
    /// </summary>
    Unknown,
    /// <summary>
    /// File is local.
    /// </summary>
    LocalResource,
    /// <summary>
    /// Document is remote.
    /// </summary>
    RemoteResource
    #endregion
  }

  /// <summary>
  /// Enumerator declares the types of documents supported by HTMLUI.
  /// </summary>
  public enum DataFormat
  {
    #region Enum Flags
    /// <summary>
    /// Document is in HTML format.
    /// </summary>
    Html,
    /// <summary>
    /// Document is in Mht format.
    /// </summary>
    Mht
    #endregion
  }
}
