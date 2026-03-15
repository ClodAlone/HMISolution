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

using Syncfusion.Windows.Forms.Edit;


using Syncfusion.Windows.Forms.Edit.Implementation;
using Syncfusion.Windows.Forms.Edit.Implementation.Config;
using Syncfusion.Windows.Forms.Edit.Interfaces;
using Syncfusion.Windows.Forms.Edit.Enums;
#endregion

namespace Syncfusion.Windows.Forms.Edit.Interfaces
{
  /// <summary>
  /// This is lexem which control parsing of input stream
  /// </summary>
	public interface IConfigLexem
	{
		#region Interface properties
		/// <summary>
		/// Gets name of the format to be used in collapsed state.
		/// </summary>
		string TypeCollapsed{ get; }
		/// <summary>
		/// Begin symbol or word for lexem.
		/// </summary>
		string      BeginBlock{ get; }
		/// <summary>
		/// If lexem has begin symbol and end symbol then use this property for
		/// setting end symbol. if lemex is "keyword" then this property must be
		/// set to null value.
		/// </summary>
		string      EndBlock{ get; }
		/// <summary>
		/// If lexem can be divided on multi lines or has some special rules
		/// which can continue lexem then us this setting.
		/// </summary>
		string      ContinueBlock{ get; }
		/// <summary>
		/// If many lexems has the same begin string then on parsing
		/// must be controlled order in which lexem parser will try to
		/// interpret input as lexem...
		/// </summary>
		int         Priority{ get; }
		/// <summary>
		/// Format which must be used for coloring. If format is Custom then
		/// used FormatName property for format identification
		/// </summary>
		FormatType  Type{ get; }
		/// <summary>
		/// FormatName which must be used for coloring.
		/// </summary>
		string      FormatName{ get; }
		/// <summary>
		/// Is BeginBlock property contains Regular expression or not
		/// </summary>
		bool        IsBeginRegex{ get; }
		/// <summary>
		/// Is EndBlock property contains Regular expression or not
		/// </summary>
		bool        IsEndRegex{ get; }
		/// <summary>
		/// Is ContinueBlock property contains Regular expression or not
		/// </summary>
		bool        IsContinueRegex{ get; }
		/// <summary>
		/// Is the end-block just the way to exit higher by stack, or it is real ending of lexem.
		/// </summary>
		bool        IsPseudoEnd{ get; }
		/// <summary>
		/// this flag indicate must parser parse lexem internals or not. For
		/// complex constructions data between begin and end blocks can have own
		/// formats.
		/// </summary>
		bool        IsComplex{ get; }
		/// <summary>
		/// Specific sublexems for complex lexem.
		/// </summary>
		ArrayList SubLexems{ get; }
		/// <summary>
		/// Link to the parent, who keeps current lexem.
		/// </summary>
		IConfigLexem ParentConfig{ get; }
		/// <summary>
		/// This flag indicates, whether parser should look for lexem`s config just in local array,
		/// or also look in parents
		/// </summary>
		bool OnlyLocalSublexems{ get; }
		/// <summary>
		/// Checks string to the equalization to end block.
		/// If end block is regular expression, input string will be checked by RegExp
		/// </summary>
		/// <param name="str">String to be checked</param>
		/// <returns>True if it can be treated as end block.</returns>
		bool IsEqualToEnd( string str );
		/// <summary>
		/// Checks string to the equalization to continue block.
		/// If continue block is regular expression, input string will be checked by RegExp
		/// </summary>
		/// <param name="str">String to be checked</param>
		/// <returns>True if it can be treated as continue block.</returns>
		bool IsEqualToContinue( string str );
		/// <summary>
		/// Checks string to the equalization to begin block.
		/// If begin block is regular expression, input string will be checked by RegExp
		/// </summary>
		/// <param name="str">String to be checked</param>
		/// <returns>True if it can be treated as begin block.</returns>
		bool IsEqualToBegin( string str );
		/// <summary>
		/// Searches for configs in sub-lexems.
		/// Current lexem config is not tested for equalization.
		/// If config was not found in sub-lexems,
		/// it will be searched in parent.
		/// </summary>
		/// <param name="str">String to find.</param>
		/// <returns>List of config lexems.</returns>
		IList FindConfigs( string str );
		/// <summary>
		/// GET format by Type and FormatName.
		/// </summary>
		ISnippetFormat Format{ get; }
		/// <summary>
		/// GET link to the virtual config for current lexem.
		/// </summary>
		/// <remarks>
		/// Virtual configs does not support collapsed state.
		/// </remarks>
		IConfigLexem VirtualConfig{ get; }
		/// <summary>
		/// Condition, needed to pass check. Format: name=ON|OFF
		/// </summary>
		string Condition{ get; }
		/// <summary>
		/// List of references.
		/// </summary>
		ArrayList References{ get; }
		/// <summary>
		/// Static unique ID of configuration node.
		/// </summary>
		int ID{ get; }
		/// <summary>
		/// Language, lexem belongs to.
		/// </summary>
		IConfigLanguage Language{ get; }
		/// <summary>
		/// GET sign of auto-indenting after lexem with such config.
		/// </summary>
		bool Indent{ get; }
		/// <summary>
		/// GET ID of the lexem configuration, that follows right after current 
		/// one is parsed. Such lexem must be complex and "OnlyLocalSublexems",
		/// without beginblock and with endblock.
		/// </summary>
		int NextID{ get; }
		/// <summary>
		/// GET sign of dropping down context choice list after entering text of the current lexem.
		/// </summary>
		/// <remarks>
		/// Can be set only on non-complex lexems.
		/// </remarks>
		bool DropContextChoiceList{ get; }
		/// <summary>
		/// Get value, that shows whether context prompt should be shown after typing text of the current lexem.
		/// </summary>
		/// <remarks>
		/// Can be set only on non-complex lexems.
		/// </remarks>
		bool DropContextPrompt{ get; }
		/// <summary>
		/// Gets whether content divider should be shown below lexem.
		/// </summary>
		bool ContentDivider{ get; }
		/// <summary>
		/// Gets whether IndentationGuideline should be shown.
		/// </summary>
		bool IndentationGuideline{ get; }
		/// <summary>
		/// Gets value indicating if lexem should be used if there are more than one config found on one priority level.
		/// </summary>
		bool DefaultInGroup{ get; }
		/// <summary>
		/// Gets value indicating if custom control should be used instead of the simple lexem rendering.
		/// </summary>
		bool UseCustomControl{ get; }
		/// <summary>
		/// Gets value indicating whether autoreplace triggers can be used.
		/// </summary>
		bool AllowTriggers{ get; }
		#endregion
	}
}