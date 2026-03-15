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

using System.Collections;

using Syncfusion.Windows.Forms.Edit.Utils.CodeSnippets;

namespace Syncfusion.Windows.Forms.Edit.Interfaces
{
  /// <summary>
  /// This interface publish lexem-parser configuration functionality.
  /// </summary>
	public interface IConfigLanguage
		: IFormatManager
	{
		#region Interface Properties
		/// <summary>
		/// Language friendly name
		/// </summary>
		string    Language{ get; }
		/// <summary>
		/// GET, SET commaon splitters.
		/// </summary>
		string    OneCharTokenSplits{ get; set; }
		/// <summary>
		/// GET list of lexem configurations.
		/// </summary>
		ArrayList Lexems{ get; }
		/// <summary>
		/// Gets container of code snippets.
		/// </summary>
		CodeSnippetsContainer SnippetsContainer{ get; }
		/// <summary>
		/// Gets list of autoreplace triggers.
		/// </summary>
		ArrayList AutoReplaceTriggers{ get; }
		/// <summary>
		/// Array of multichar splitters (ex. ++, --, /*, */, //, /// )
		/// </summary>
		ArrayList Splits{ get; set; }
		/// <summary>
		/// Known for object formats. Here is list of defined in config file
		/// formats. If format not defined, but it belong to default formats
		/// specified by FormatType enum then will be used default configuration
		/// for it. Each string hold one extension.
		/// </summary>
		ArrayList  KnownFormats{ get; }
		/// <summary>
		/// file extensions by which this language can be automatically linked
		/// to source file.
		/// </summary>
		ArrayList  Extensions{ get; set; }
		/// <summary>
		/// GET lexem configuration by it`s ID.
		/// </summary>
		IConfigLexem FindConfig( int iConfigID );
		/// <summary>
		/// Case insensitivity of the language.
		/// </summary>
		bool CaseInsensitive{ get; set; }
		/// <summary>
		/// Gets or sets string representing beginning of comment for this language.
		/// If EndComment is empty string, BeginComment is inserted in each of the commented lines.
		/// </summary>
		string StartComment{ get; set; }
		/// <summary>
		/// Gets or sets string representing end of comment for this language.
		/// If EndComment is empty string, BeginComment is inserted in each of the commented lines.
		/// </summary>
		string EndComment{ get; set; }
        /// <summary>
        /// Gets or sets value for cached
        /// </summary>
        bool Cached { get; set; }
		/// <summary>
		/// Gets or sets array of autoreplace triggers activators.
		/// </summary>
		char[] TriggersActivators{ get; }
		/// <summary>
		/// Gets string of triggers activators.
		/// </summary>
		string TriggersActivatorsString{ get; }
		#endregion

		#region Interface Methods
		/// <summary>
		/// Adds new code snippet to the language.
		/// </summary>
		/// <param name="title">Title of code snippet.</param>
		/// <param name="literals">List of literals.</param>
		/// <param name="code">Code for code snippet.</param>
		void AddCodeSnippet( string title, ArrayList literals, string code );
		/// <summary>
		/// Adds code snippet to the language.
		/// </summary>
		/// <param name="snippet">Code snippet to be added.</param>
		void AddCodeSnippet( CodeSnippet snippet );
		/// <summary>
		/// Adds new code snippets container to the language.
		/// </summary>
		/// <param name="container">Code snippets container to be added.</param>
		void AddCodeSnippetsContainer( CodeSnippetsContainer container );
		/// <summary>
		/// Resets all cached data. Must be called after every change of the configuration inside the language.
		/// </summary>
		void ResetCaches();
		#endregion
  }
}