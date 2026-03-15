#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;

namespace Syncfusion.Windows.Edit
{
    /// <summary>
    /// Specifies the types of action supported by Undo manager
    /// </summary>
    /// <remarks>
    /// <para></para>
    /// <list type="table">
    /// <listheader>
    /// <term>Value</term>
    /// <description>Description</description></listheader>
    /// <item>
    /// <term>Delete</term>
    /// <description>Specifies Delete action type in Undo Manager.</description></item>
    /// <item>
    /// <term>Backspace</term>
    /// <description>Specifies Backspace action type in Undo Manager.</description></item>
    /// <item>
    /// <term>Cut</term>
    /// <description>Specifies Cut action type in Undo Manager.</description></item>
    /// <item>
    /// <term>Paste</term>
    /// <description>Specifies Paste action type in Undo Manager.</description></item>
    /// <item>
    /// <term>Enter</term>
    /// <description>Specifies Enter action type in Undo Manager.</description></item>
    /// <item>
    /// <term>Type</term>
    /// <description>Specified Type action supported by Undo manager.</description></item>
    /// </list>
    /// </remarks>
    internal enum ActionType
    {
        /// <summary>
        /// Specifies Type action supported by Undo manager.
        /// </summary>
        Type,

        /// <summary>
        /// Specifies Delete action supported by Undo manager.
        /// </summary>
        Delete,

        /// <summary>
        /// Specifies Backspace action supported by Undo manager.
        /// </summary>
        Backspace,

        /// <summary>
        /// Specifies Cut action supported by Undo manager.
        /// </summary>
        Cut,

        /// <summary>
        /// Specifies Paste action supported by Undo manager.
        /// </summary>
        Paste,

        /// <summary>
        /// Specifies Enter action supported by Undo manager.
        /// </summary>
        Enter,

        /// <summary>
        /// Specifies Tab action supported by Undo manager.
        /// </summary>
        Tab,

        /// <summary>
        /// Specifies Replace action supported by Undo manager.
        /// </summary>
        Replace,

        /// <summary>
        /// Specifies Replace All action supported by Undo manager.
        /// </summary>
        ReplaceAll,

        /// <summary>
        /// Specified Increase Indent action supported by Undo manager.
        /// </summary>
        IncreaseIndent,

        /// <summary>
        /// Specified Decrease Indent action supported by Undo manager.
        /// </summary>
        DecreaseIndent,

        /// <summary>
        /// Specified Comment Selection action supported by Undo manager.
        /// </summary>
        CommentSelection,

        /// <summary>
        /// Specified UnComment Selection action supported by Undo manager.
        /// </summary>
        UncommentSelection
    }

    /// <summary>
    /// Displays list of Languages supported by Essential Edit WPF
    /// </summary>
    /// <remarks>
    /// <para></para>
    /// <para></para>
    /// <para></para>
    /// <para></para>
    /// <list type="table">
    /// <listheader>
    /// <term>Value</term>
    /// <description>Description</description></listheader>
    /// <item>
    /// <term>Text</term>
    /// <description>Represents plain text and syntax highlighting will not be applied
    /// the text in the EditControl.</description></item>
    /// <item>
    /// <term>CSharp</term>
    /// <description>Represents C# files and applies syntax highlighting and outlining
    /// based on the built - in language specification for C#.</description></item>
    /// <item>
    /// <term>VisualBasic</term>
    /// <description>Represents Visual Basic files and applies syntax highlighting and
    /// outlining based on the built - in language specification for Visual
    /// Basic.</description></item>
    /// <item>
    /// <term>XAML</term>
    /// <description>Represents XAML files and applies syntax highlighting based on the
    /// built - in language specification for XAML.</description></item>
    /// <item>
    /// <term>XML</term>
    /// <description>Represents XML files and applies syntax highlighting based on the
    /// built - in language specification for XML.</description></item>
    /// <item>
    /// <term>Custom</term>
    /// <description>Represents user defined files and applies syntax highlighting based
    /// on the user defined language configurations applied using CustomLanguage
    /// property of EditControl class.</description></item></list>
    /// </remarks>
    public enum Languages
    {
        /// <summary>
        /// Represents plain Text and syntax highlighting will not be applied
        /// the text in the EditControl.
        /// </summary>
        Text,

        /// <summary>
        /// Represents C# files and applies syntax highlighting and outlining
        /// based on the built - in language specification for C#.
        /// </summary>
        CSharp,

        /// <summary>
        /// Represents user defined files and applies syntax highlighting based
        /// on the user defined language configurations applied using CustomLanguage
        /// property of EditControl class.
        /// </summary>
        Custom,

        /// <summary>
        /// Represents Visual Basic files and applies syntax highlighting and
        /// outlining based on the built - in language specification for Visual
        /// Basic.
        /// </summary>
        VisualBasic,

        /// <summary>
        /// Represents XML files and applies syntax highlighting based on the
        /// built - in language specification for XML.
        /// </summary>
        XML,

        /// <summary>
        /// Represents XAML files and applies syntax highlighting based on the
        /// built - in language specification for XAML.
        /// </summary>
        XAML,

        /// <summary>
        /// Represents XAML files and applies syntax highlighting based on the
        /// built - in language specification for XAML.
        /// </summary>
        SQL
    }

    /// <summary>
    /// Specifies types of tokens supported by EditLanguage lexems
    /// </summary>
    /// <remarks>
    /// <para></para>
    /// <para></para>
    /// <list type="table">
    /// <listheader>
    /// <term>Value</term>
    /// <description>Description</description></listheader>
    /// <item>
    /// <term>Literals</term>
    /// <description>Represents the SyntaxHighlighting to the text. The Literal format
    /// color is applies to the text, which is located between start and end
    /// Text.</description></item>
    /// <item>
    /// <term>Keyword</term>
    /// <description>Represents the SyntaxHighlighting to the text. The Keyword format
    /// color is applies to the text, which is located between start and end
    /// Text.</description></item>
    /// <item>
    /// <term>Comment</term>
    /// <description>Represents the SyntaxHighlighting to the text. The Comment format
    /// color is applies to the text, which is located between start and end
    /// Text.</description></item>
    /// <item>
    /// <term>Preprocessor</term>
    /// <description>Represents the SyntaxHighlighting to the text. The Preprocessor
    /// format color is applies to the text, which is located between start and end
    /// Text.</description></item>
    /// <item>
    /// <term>Custom</term>
    /// <description>Represents user defined format and applies syntax highlighting based
    /// on the user defined format color.</description></item></list>
    /// </remarks>
    public enum EditTokenType
    {
        /// <summary>
        /// Specified the Literal Token
        /// </summary>
        Literals,

        /// <summary>
        /// Specified the Keyword Token
        /// </summary>
        Keyword,

        /// <summary>
        /// Specified the Comment Token
        /// </summary>
        Comment,

        /// <summary>
        /// Specified the Operator Token
        /// </summary>
        Operator,

        /// <summary>
        /// Specified the Preprocessor Token
        /// </summary>
        Preprocessor,

        /// <summary>
        /// Specified the Custom Token
        /// </summary>
        Custom,

        /// <summary>
        /// Specified the CodeSnippet Token
        /// </summary>
        CodeSnippet,

        /// <summary>
        /// Specified the Property Token
        /// </summary>
        Property,

        /// <summary>
        /// Specified the NamespaceDeclaration Token
        /// </summary>
        NamespaceDeclaration
    }

    /// <summary>
    /// Tabs present in Find and Replace Window
    /// </summary>
    public enum Tabs
    {
        /// <summary>
        /// Find Tab of Find And Replace Window
        /// </summary>
        FindTab,

        /// <summary>
        /// Replace Tab of Find And Replace Window
        /// </summary>
        ReplaceTab,

        /// <summary>
        /// Find Symbol Tab of Find And Replace Window
        /// </summary>
        FindSymbolTab
    }

    /// <summary>
    /// Specifies the visibility of Find results tab.
    /// </summary>
    public enum TabVisibility
    {
        /// <summary>
        /// Auto  - FindResults Tab will be whenever Find Symbol options is used and will be hidden when the Window is Closed explicitly or when FindReplace Window is closed.
        /// </summary>
        Auto,

        /// <summary>
        /// Visible - FindResults Tab will always be visible
        /// </summary>
        Visible,

        /// <summary>
        /// Collapsed - FindResults tab will always be collapsed even when the Find Symbol options is used.
        /// </summary>
        Collapsed
    }

    /// <summary>
    /// Specifies the types of languages
    /// </summary>
    /// <remarks>
    /// <para></para>
    /// <list type="table">
    /// <listheader>
    /// <term>Value</term>
    /// <description>Description</description></listheader>
    /// <item>
    /// <term>Procedural</term>
    /// <description>Represents the Procedural language like c, c++ etc.</description></item>
    /// <item>
    /// <term>Markup</term>
    /// <description>Represents the Markup language like XML and XAML.</description></item>
    /// </list>
    /// </remarks>
    [Obsolete("EditLanguage contains functionalities for procedural languages, for markup language use EditMarkupLanguage class")]
    public enum EditLanguageType
    {
        /// <summary>
        /// Specified the Procedural Language
        /// </summary>
        Procedural,

        /// <summary>
        /// Specified the Markup Language
        /// </summary>
        Markup
    }

    /// <summary>
    ///
    /// </summary>
    public enum ScopeLevel
    {
        /// <summary>
        ///
        /// </summary>
        None,

        /// <summary>
        ///
        /// </summary>
        Namespace,

        /// <summary>
        ///
        /// </summary>
        Class,

        /// <summary>
        ///
        /// </summary>
        Member,

        /// <summary>
        ///
        /// </summary>
        StaticMember,

        /// <summary>
        ///
        /// </summary>
        All
    }

    /// <summary>
    ///
    /// </summary>
    public enum IntellisenseMode
    {
        /// <summary>
        ///
        /// </summary>
        Auto,

        /// <summary>
        ///
        /// </summary>
        Custom
    }

    /// <summary>
    ///
    /// </summary>
    public enum LineModificationState
    {
        /// <summary>
        ///
        /// </summary>
        Unchanged,

        /// <summary>
        ///
        /// </summary>
        Modified,

        /// <summary>
        ///
        /// </summary>
        Saved
    }

    /// <summary>
    ///
    /// </summary>
    public enum IndentingOptions
    {
        /// <summary>
        ///
        /// </summary>
        None,

        /// <summary>
        ///
        /// </summary>
        Block,

        /// <summary>
        ///
        /// </summary>
        Smart
    }
}