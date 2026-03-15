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

using Syncfusion.Windows.Forms.Edit.Enums;

namespace Syncfusion.Windows.Forms.Edit.Interfaces
{
  /// <summary>
  /// Formats Manager declaration interface.
  /// </summary>
  public interface IFormatManager
  {
    #region Properties
    /// <summary>
    /// Gets format by name.
    /// </summary>
    ISnippetFormat this[ string name ]{ get; }
    /// <summary>
    /// Gets format by type, does not work for FormatType.Custom.
    /// </summary>
    ISnippetFormat this[ FormatType type ]{ get; }
    /// <summary>
    /// Gets format by indexs
    /// </summary>
    ISnippetFormat this[ int index ]{ get; }
    /// <summary>
    /// This is calculated value and it hold maximum heigh of line according to
    /// known to object formats.
    /// </summary>
    int MaxLineHeight{ get; }
    /// <summary>
    /// This is calculated value an it hold minimal size of line according to
    /// known for object formats.
    /// </summary>
    int MinLineHeight{ get; }
    /// <summary>
    /// This is calculated value and it hold maximum char width according to
    /// known for object formats.
    /// </summary>
    int MaxCharWidth{ get; }
    /// <summary>
    /// This is calcualted value and it hold minimum char width according to
    /// formats known by object.
    /// </summary>
    int MinCharWidth{ get; }
    /// <summary>
    /// String, to be placed instead of tab.
    /// </summary>
    string TabReplaceString{ get; }
    /// <summary>
    /// Gets or sets sing of showing whitespaces as bullets.
    /// </summary>
    bool ShowWhiteSpaces{ get; set; }
    #endregion

    #region Methods
    /// <summary>
    /// Create new format with unique name. FormatType will be set to
    /// Custom value.
    /// </summary>
    /// <param name="formatName">new format unique name</param>
    /// <returns>Created and added into collection format reference</returns>
    ISnippetFormat Add( string formatName );
    /// <summary>
    /// Create new format inherited from source format
    /// </summary>
    /// <param name="formatName">unique format name</param>
    /// <param name="source">format which setting must be inherited</param>
    /// <returns>Created and added into collection format reference</returns>
    ISnippetFormat Add( string formatName, ISnippetFormat source );
    /// <summary>
    /// Create new format inherited from source format
    /// </summary>
    /// <param name="formatName">unique format name</param>
    /// <param name="sourceName">Get source format by it unique name</param>
    /// <returns>Created and added into collection format reference</returns>
    ISnippetFormat Add( string formatName, string sourceName );
    /// <summary>
    /// Remove formmat from collection by it reference
    /// </summary>
    /// <param name="format">reference on format</param>
    void Remove( ISnippetFormat format );
    /// <summary>
    /// Remove format from collection by it unique name
    /// </summary>
    /// <param name="formatName">unique format name</param>
    void Remove( string formatName );
    #endregion
  }
}