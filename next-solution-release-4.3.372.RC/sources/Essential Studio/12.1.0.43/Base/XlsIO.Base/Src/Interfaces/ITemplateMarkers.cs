#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;

namespace Syncfusion.XlsIO
{
  /// <summary>
  /// Summary description for ITemplateMarkers.
  /// </summary>
  public interface ITemplateMarkers
  {
    #region Interface methods
    /// <summary>
    /// Applies markers to the parent object.
    /// </summary>
    void ApplyMarkers();
    /// <summary>
    /// Adds new variable to the collection.
    /// </summary>
    /// <param name="strName">Name of the new variable.</param>
    /// <param name="variable">Variable value.</param>
    void AddVariable( string strName, object variable );
    /// <summary>
    /// Removes variable from the collection.
    /// </summary>
    /// <param name="strName">Variable name.</param>
    void RemoveVariable( string strName );
    /// <summary>
    /// Checks whether template markers object contains variable with specified name.
    /// </summary>
    /// <param name="strName">Name to locate.</param>
    bool ContainsVariable( string strName );
    #endregion

    #region Interface properties
    /// <summary>
    /// Gets / sets marker prefix. String that indicates that cell contains marker.
    /// </summary>
    string MarkerPrefix { get; set; }
    /// <summary>
    /// Gets / sets arguments separator.
    /// </summary>
    char ArgumentSeparator { get; set; }
    #endregion
  }
}
