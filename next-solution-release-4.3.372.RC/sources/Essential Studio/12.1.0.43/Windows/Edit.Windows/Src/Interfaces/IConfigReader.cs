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
using System.IO;
using System.Xml;
using System.Drawing;
using System.Collections;
using  Syncfusion.Windows.Forms.Edit.Enums;
using Syncfusion.Windows.Forms.Edit;

using Syncfusion.Windows.Forms.Edit.Implementation;
using Syncfusion.Windows.Forms.Edit.Interfaces;
#endregion

namespace Syncfusion.Windows.Forms.Edit.Interfaces
{
  /// <summary>
  /// Interface provide base functionality for reading and writting configuration
  /// needed for coloring.
  /// </summary>
  public interface IConfig
  {
    #region Access configuration languages objects
    /// <summary>
    /// Get configuration for language by index
    /// </summary>
    IConfigLanguage this[ int index ]{ get; }
    /// <summary>
    /// Get configuration for language by specified name
    /// </summary>
    IConfigLanguage this[ string name ]{ get; }
    /// <summary>
    /// Get list of known to configuration languages
    /// </summary>
    ArrayList KnownLanguages{ get; }
    /// <summary>
    /// Creates new language configuration and adds it to the configurations list.
    /// </summary>
    /// <param name="name">Name of the new language.</param>
    /// <returns>New instance of language configuration.</returns>
    IConfigLanguage CreateLanguageConfiguration( string name );
    /// <summary>
    /// GET default language configuration that is stored in embebbed resource.
    /// </summary>
    IConfigLanguage DefaultLanguage{ get; }
    #endregion

    #region Open/Append/Save file operations
    /// <summary>
    /// Open config file by it file path
    /// </summary>
    /// <param name="configFile">file path to config file</param>
    void Open( string configFile );
    /// <summary>
    /// Read config from stream
    /// </summary>
    /// <param name="configFile">stream which contains configuration for
    /// parsers</param>
    void Open( Stream configFile );
    /// <summary>
    /// XML Document which contains configuration rules for parsers
    /// </summary>
    /// <param name="configFile">XML Document</param>
    void Open( XmlDocument configFile );
    /// <summary>
    /// If config file divided by user on many small one for each language then
    /// this methods will help him to load them in one call.
    /// </summary>
    /// <param name="configFile">file path to config file</param>
    void AppendConfig( string configFile );
    /// <summary>
    /// If config file divided by user on many small one for each language then
    /// this methods will help him to load them in one call.
    /// </summary>
    /// <param name="configFile">Stream with XML config</param>
    void AppendConfig( Stream configFile );
    /// <summary>
    /// If config file divided by user on many small one for each language then
    /// this methods will help him to load them in one call.
    /// </summary>
    /// <param name="configFile">XML document which contains formatting data</param>
    void AppendConfig( XmlDocument configFile );
    /// <summary>
    /// Reset to default configuration
    /// </summary>
    void Reset();
    /// <summary>
    /// Save current configuration to file
    /// </summary>
    /// <param name="fileName">output file name</param>
    void Save( string fileName );
    /// <summary>
    /// Save configuration to output stream
    /// </summary>
    /// <param name="config">stream for config saving</param>
    void Save( Stream config );
    /// <summary>
    /// Save configuration to XML Document
    /// </summary>
    /// <param name="config">XML document to which data must be saved</param>
    void Save( XmlDocument config );
    #endregion

    #region Events
    /// <summary>
    /// Event, that is raised when 
    /// </summary>
    event EventHandler ConfigurationChanged;
    #endregion
  }
}