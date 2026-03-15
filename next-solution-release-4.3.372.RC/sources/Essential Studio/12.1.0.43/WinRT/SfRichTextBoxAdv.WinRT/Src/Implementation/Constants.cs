#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if WPF
namespace Syncfusion.Windows.Tools.RichTextBoxAdv
#else
namespace Syncfusion.UI.Xaml.RichTextBoxAdv
#endif
{
    /// Operators used in the HTML
    /// </summary>
    class OperatorConstants
    {
        public const string Dot = ".";
        public const string OpenBrace = "{";
        public const string CloseBrace = "}";
        public const string Colon = ":";
        public const string SemiColon = ";";
        public const string Equal = "=";
        public const string Quotes = "\"";
        public const string LessthanSymbol = "<";
        public const string GreaterthanSymbol = ">";
        public const string Endslash = "/";
    }

    class InlineProperties
    {
        public const string UnderLine = "Underline";
        public const string FontFamily = "FontFamily";
        public const string FontSize = "FontSize";
        public const string ItalicStyle = "Italic";
        public const string BoldStyle = "Bold";
        public const string HighlightColor = "HighlightColor";
        public const string BaseLineAlignment = "BaselineAlignment";
        public const string StrikeThrough = "StrikeThrough";
        public const string Foreground = "Foreground";
        public const string Text = "Text";
        public const string NavigationUrl = "NavigationUrl";
        public const string NavigationText = "Text";
        public const string TargetType = "TargetType";
        public const string ImageString = "ImageString";
        public const string ImageSource = "ImageSource";
        public const string Width = "Width";
        public const string Height = "Height";
        public const string UiElement = "uielement";
    }


    class ParagraphProperties
    {
        public const string TextAlignment = "TextAlignment";
        public const string LeftIndent = "LeftIndent";
        public const string RightIndent = "RightIndent";
        public const string BeforeSpacing = "BeforeSpacing";
        public const string AfterSpacing = "AfterSpacing";
        public const string ListType = "ListType";
    }

    class RichTextBoxConstants
    {
        public const string Paragraph = "ParagraphAdv";
        public const string Span = "SpanAdv";
        public const string HyperLink = "HyperlinkAdv";
        public const string Section = "SectionAdv";
        public const string Document = "DocumentAdv";
        public const string ImageContainer = "ImageContainerAdv";
        public const string UiContainerAdv = "UIContainerAdv";
        public const string TableAdv = "TableAdv";
        public const string TableRowAdv = "TableRowAdv";
        public const string TableCellAdv = "TableCellAdv";
    }

    class CellProperties
    {
        public const string DesiredWidth = "DesiredWidth";
        public const string RowSpan = "RowSpan";
        public const string ColumnSpan = "ColumnSpan";
        public const string Background = "Background";
        public const string TableCellMode = "TableCellMode";
    }

}
