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

namespace Syncfusion.XlsIO
{
  /// <summary>
  /// Summary description for ITemplateMarkers.
  /// </summary>
  public interface ITemplateMarkersProcessor
  {
    #region Interface methods
    /// <summary>
    /// Applies markers to the parent object.
    /// </summary>
    void ApplyMarkers();
    /// <summary>
    /// Applies markers to the parent object.
    /// </summary>
    void ApplyMarkers( UnknownVariableAction action );
    /// <summary>
    /// Adds new variable to the collection.
    /// </summary>
    /// <param name="strName">Name of the new variable.</param>
    /// <param name="variable">Variable value.</param>
    void AddVariable( string strName, object variable );
    /// <summary>
    /// Adds new variable to the collection.
    /// </summary>
    /// <param name="strName">Name of the new variable.</param>
    /// <param name="variable">Variable value.</param>
    /// <param name="variableTypeAction">Marker variable Type action.</param>
    void AddVariable(string strName, object variable,VariableTypeAction variableTypeAction);
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
	/// <summary>
    /// Adds conditional format to the Template Marker.
    /// </summary>
    /// <param name="range">Represents the range where the conditional format to be applied.</param>
    /// <remarks>The conditional format range should be within the tempalte marker range.</remarks>
    IConditionalFormats CreateConditionalFormats(IRange range);
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
