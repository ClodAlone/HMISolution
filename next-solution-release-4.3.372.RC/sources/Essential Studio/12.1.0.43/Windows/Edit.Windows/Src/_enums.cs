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

namespace Syncfusion.Windows.Forms.Edit.Enums
{
	/// <summary>
	/// List all types of meanings of the lexem in the meaning of collapsing.
	/// </summary>
	public enum LexemCollapsingType
	{
		/// <summary>
		/// Lexem is a collapsed region.
		/// </summary>
		CollapsedLexem,
		/// <summary>
		/// Lexem is the first lexem of the collapsible region.
		/// </summary>
		RegionStart,
		/// <summary>
		/// Lexem is the last region of the collapsible region.
		/// </summary>
		RegionEnd,
		/// <summary>
		/// Lexem is not related to the collapsible regions.
		/// </summary>
		None
	}

	/// <summary>
	/// Specifies text parsing mode. You can switch between High speed and high accuracy modes.
	/// </summary>
	public enum TextParsingMode
	{
		/// <summary>
		/// Slow parsing speed, all text is parsed and colored with 100% accuracy.
		/// </summary>
		FullParsing,
		/// <summary>
		/// High parsing speed, some text can be colored incorrectly.
		/// </summary>
		PartialParsingNoFallback,
		/// <summary>
		/// High parsing speed, the accuracy of which can not be guaranteed, is colored as simple text without syntax highlighting.
		/// </summary>
		PartialParsingWithFallback
	}

	/// <summary>
	/// Type of the appending of new configuration with duplicated items to the existing one.
	/// </summary>
	public enum DuplicatesOptions
	{
		/// <summary>
		/// If language configuration exists in current configuration, new configuration will not be added.
		/// </summary>
		SkipDuplicates,
		/// <summary>
		/// If language configuration exists in current configuration, it will be deleted and new configuration will be added.
		/// </summary>
		OverwriteDuplicates,
		/// <summary>
		/// If language configuration exists in current configuration, exception will be raised.
		/// </summary>
		DuplicatesNotAllowed,
		/// <summary>
		/// If language configuration exists in current configuration, new configuration will be merged with it.
		/// New configuration has higher priority and will overwrite old settings. Useful to redefine coloring.
		/// </summary>
		MergeDuplicates
	}

	/// <summary>
	/// Action to be executed on the modified file.
	/// </summary>
	public enum SaveChangesAction
	{
		/// <summary>
		/// Changes should be saved by edit control.
		/// </summary>
		Save,
		/// <summary>
		/// Discards changes without saving them.
		/// </summary>
		Discard,
		/// <summary>
		/// Cancels current operation. Used in form's closure processing to cancel it's closure.
		/// </summary>
		Cancel,
		/// <summary>
		/// Shows standard dialog that prompts user to save changes or cancel the operation.
		/// </summary>
		ShowDialog
	}

	/// <summary>
	/// Enumeration of the states of modifier key.
	/// </summary>
	public enum ModifierKeyState
	{
		/// <summary>
		/// State of the modifier key is not checked.
		/// </summary>
		Indifferent,
		/// <summary>
		/// Modifier key must be pressed.
		/// </summary>
		Pressed,
		/// <summary>
		/// Modifier key must be not pressed.
		/// </summary>
		Unpressed
	}

	/// <summary>
	/// Underline styles of snippet
	/// </summary>
	public enum UnderlineStyle
	{
		/// <summary>
		/// No underline
		/// </summary>
		None,
		/// <summary>
		/// One line under snippet text
		/// </summary>
		Solid,
		/// <summary>
		/// Dash-dot line style for snippet text
		/// </summary>
		DashDot,
		/// <summary>
		/// Dot line style for snippet text
		/// </summary>
		Dot,
		/// <summary>
		/// Dash line style for snippet text
		/// </summary>
		Dash,
		/// <summary>
		/// Wave line style for snippet text
		/// </summary>
		Wave,
	}

	/// <summary>
	/// Weight of the underline.
	/// </summary>
	public enum UnderlineWeight
	{
		/// <summary>
		/// 1px line
		/// </summary>
		Thin,
		/// <summary>
		/// 2px line
		/// </summary>
		Bold,
		/// <summary>
		/// Drawing two lines in 1px weight with background color delimiter in 1px. Line needs 3px for drawing.
		/// </summary>
		Double,
		/// <summary>
		/// Each line takes 2 px. Lines needs 6px for drawing.
		/// </summary>
		DoubleBold
	}

	/// <summary>
	/// Style of border line.
	/// </summary>
	public enum FrameBorderStyle
	{
		/// <summary>
		/// No border.
		/// </summary>
		None,
		/// <summary>
		/// One simple line.
		/// </summary>
		Solid,
		/// <summary>
		/// Dash-dot line.
		/// </summary>
		DashDot,
		/// <summary>
		/// Dot line.
		/// </summary>
		Dot,
		/// <summary>
		/// Dash line.
		/// </summary>
		Dash,
		/// <summary>
		/// Wave line.
		/// </summary>
		Wave,
	}

	/// <summary>
	/// Weight of border line.
	/// </summary>
	[Flags]
	public enum BorderWeight
	{
		/// <summary>
		/// 1px line.
		/// </summary>
		Thin = 1,
		/// <summary>
		/// 2px line.
		/// </summary>
		Bold = 3,
		/// <summary>
		/// Two 1px lines with delimiter in 1px.
		/// </summary>
		Double = 4,
	}

	/// <summary>
	/// Default types supported by control render.
	/// </summary>
	public enum FormatType
	{
		/// <summary>
		/// Default text drawing format.
		/// </summary>
		Text,
		/// <summary>
		/// Selected text format.
		/// </summary>
		SelectedText,
		/// <summary>
		/// Selected text drawing when window which holds the control loses focus.
		/// </summary>
		InactiveSelectedText,
		/// <summary>
		/// Display part of text in error color.
		/// </summary>
		Error,
		/// <summary>
		/// Special bookmark formatting.
		/// </summary>
		Bookmark,
		/// <summary>
		/// Display line of code as is on it set breakpoint.
		/// </summary>
		EnabledBreakPoint,
		/// <summary>
		/// Disabled breakpoint look and feel.
		/// </summary>
		DisabledBreakPoint,
		/// <summary>
		/// Breakpoint placed in wrong location.
		/// </summary>
		WrongBreakPoint,
		/// <summary>
		/// Current cursor position.
		/// </summary>
		CurrentStatement,
		/// <summary>
		/// Text of collapsed region caption.
		/// </summary>
		CollapsedText,
		/// <summary>
		/// Read only parts of text. Text marked in colors which say to user that code can not be edited.
		/// </summary>
		ReadOnlyRegion,
		/// <summary>
		/// Special code which generated automatically by environment can be assigned to this format.
		/// </summary>
		WizardCode,
		/// <summary>
		/// Comment in parsed language.
		/// </summary>
		Comment,
		/// <summary>
		/// Operators and punctuators symbols.
		/// </summary>
		Operator,
		/// <summary>
		/// Keyword of language.
		/// </summary>
		KeyWord,
		/// <summary>
		/// Keyword which does not belong to language directly, and used by pre-processing.
		/// </summary>
		PreprocessorKeyword,
		/// <summary>
		/// Strings - "this is string".
		/// </summary>
		String,
		/// <summary>
		/// One character symbols.
		/// </summary>
		SingleCharacter,
		/// <summary>
		/// Unique resource identifier, mostly used for web URL and e-mails.
		/// </summary>
		URI,
		/// <summary>
		/// Number value in integer or float format.
		/// </summary>
		Number,
		/// <summary>
		/// Whitespace and tabs.
		/// </summary>
		Whitespace,
		/// <summary>
		/// All other formats which can not be identified directly by control.
		/// </summary>
		Custom
	}

	/// <summary>
	/// Enumeration of the known languages.
	/// </summary>
	public enum KnownLanguages
	{
		/// <summary>
		/// Undefined language.
		/// </summary>
		Undefined,
		/// <summary>
		/// Plain text.
		/// </summary>
		Text,
		/// <summary>
		/// C#.
		/// </summary>
		CSharp,
		/// <summary>
		/// Delphi.
		/// </summary>
		Delphi,
		/// <summary>
		/// Xml.
		/// </summary>
		XML,
		/// <summary>
		/// Html.
		/// </summary>
		HTML,
		/// <summary>
		/// VB.NET
		/// </summary>
		VBNET,
		/// <summary>
		/// Sql.
		/// </summary>
		SQL,
		/// <summary>
		/// Java.
		/// </summary>
		Java,
		/// <summary>
		/// VBScript.
		/// </summary>
		VBScript,
		/// <summary>
		/// JScript.
		/// </summary>
		JScript,
		/// <summary>
		/// C.
		/// </summary>
		C
	}

	/// <summary>
	/// Indicates different additional features for searching text.
	/// </summary>
	[Flags]
	public enum SearchAttributes : short
	{
		/// <summary>
		/// Unknown feature.
		/// </summary>
		Unknown = 0x0000,
		/// <summary>
		/// Matches case in searching.
		/// </summary>
		MatchCase = 0x0001,
		/// <summary>
		/// Finds only whole word.
		/// </summary>
		MatchWholeWord = 0x0002,
		/// <summary>
		/// Searches in hidden text.
		/// </summary>
		SearchHidden = 0x0004,
		/// <summary>
		/// Searches in up direction.
		/// </summary>
		SearchUp = 0x0008,
		/// <summary>
		/// Uses Regular expressions for searching.
		/// </summary>
		UseRegexp = 0x0010
	}

	/// <summary>
	/// Indicates type of searching.
	/// </summary>
	public enum SearchType
	{
		/// <summary>
		/// Unknown type of searching.
		/// </summary>
		Unknown,
		/// <summary>
		/// Find next word.
		/// </summary>
		FindNext,
		/// <summary>
		/// Mark all found words.
		/// </summary>
		MarkAll
	}

	/// <summary>
	/// Type of the action on text.
	/// </summary>
	public enum TextChange
	{
		/// <summary>
		/// Text was inserted.
		/// </summary>
		Inserted,
		/// <summary>
		/// Text was deleted.
		/// </summary>
		Deleted,
		/// <summary>
		/// Text was changed.
		/// </summary>
        Changed,
        /// <summary>
        /// Perform Undo Operation.
        /// </summary>
        Undo,
        /// <summary>
        /// Perform Redo Operation.
        /// </summary>
        Redo      
	}

	/// <summary>
	/// Type of word wrapping.
	/// </summary>
	public enum WordWrapType
	{
		/// <summary>
		/// Wrapping by lexem.
		/// </summary>
		WrapByWord,
		/// <summary>
		/// Wrapping by char.
		/// </summary>
		WrapByChar
	}

	/// <summary>
	/// Mode of word wrapping.
	/// </summary>
	public enum WordWrapMode
	{
		/// <summary>
		/// Text is wrapped in limit of control width.
		/// </summary>
		Control,
		/// <summary>
		/// Text is wrapped in limit of text area.
		/// </summary>
		WordWrapMargin,
		/// <summary>
		/// Text is wrapped in limit of specified column.
		/// </summary>
		SpecifiedColumn
	}

	/// <summary>
	/// Mode of auto indenting.
	/// </summary>
	public enum AutoIndentMode
	{
		/// <summary>
		/// Auto indent os off.
		/// </summary>
		None,
		/// <summary>
		/// Block mode. Next line begins in the same place as the previous.
		/// </summary>
		Block,
		/// <summary>
		/// Smart mode. Start column of the next line depends on indentation properties of lexems.
		/// </summary>
		Smart,
	}

	/// <summary>
	/// Mode of showing outlining tooltips.
	/// </summary>
	public enum OutliningTooltipShowMode
	{
		/// <summary>
		/// No tooltips are shown.
		/// </summary>
		Off,
		/// <summary>
		/// Simple (not outlining) tootlip is shown.
		/// </summary>
		SimpleTooltip,
		/// <summary>
		/// Outlining tooltip is shown.
		/// </summary>
		On
	}

	/// <summary>
	/// Indicates the visibility of status bar sizing grip.
	/// </summary>
	public enum SizingGripVisibility
	{
		/// <summary>
		/// Sizing grip is always shown.
		/// </summary>
		Visible,
		/// <summary>
		/// Sizing grip is never shown.
		/// </summary>
		Hidden
	}

	/// <summary>
	/// Types of context choice items.
	/// </summary>
	public enum ContextChoiceItemType
	{
		/// <summary>
		/// Default context choice item.
		/// </summary>
		Default,
		/// <summary>
		/// Container of code snippets.
		/// </summary>
		CodeSnippetsContainer,
		/// <summary>
		/// Code snippet.
		/// </summary>
		CodeSnippet
	}
	/// <summary>
	/// Alignment of the line numbers area.
	/// </summary>
	public enum LineNumberAlignment
	{
		/// <summary>
		/// Left alignment.
		/// </summary>
		Right,
		/// <summary>
		/// Right alignment.
		/// </summary>
		Left
	}

	/// <summary>
	/// Placement of the margin.
	/// </summary>
	public enum MarginPlacement
	{
		/// <summary>
		/// Margin is situated on the left.
		/// </summary>
		Left,
		/// <summary>
		/// Margin is situated on the right.
		/// </summary>
		Right
	}
}