#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Input;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Syncfusion.UI.Xaml.RichTextBoxAdv
{
    #region BoldCommand
    public class BoldCommand : CommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BoldCommand"/> class.
        /// </summary>
        /// <param name="richTextBoxAdv">The rich text box adv.</param>
        public BoldCommand(SfRichTextBoxAdv richTextBoxAdv)
            : base(richTextBoxAdv)
        {
        }
        /// <summary>
        /// Executes the bold command.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        protected override void ExecuteCommand(object parameter)
        {
            if (OwnerControl.IsReadOnlyMode || !OwnerControl.IsDocumentLoaded)
                return;
            OwnerControl.Selection.OnBold();
            OwnerControl.Focus(FocusState.Pointer);
            base.ExecuteCommand(parameter);
        }
    }
    #endregion

    #region ItalicCommand
    public class ItalicCommand : CommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItalicCommand"/> class.
        /// </summary>
        /// <param name="richTextBoxAdv">The rich text box adv.</param>
        public ItalicCommand(SfRichTextBoxAdv richTextBoxAdv)
            : base(richTextBoxAdv)
        {
        }
        /// <summary>
        /// Executes the italic command.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        protected override void ExecuteCommand(object parameter)
        {
            if (OwnerControl.IsReadOnlyMode || !OwnerControl.IsDocumentLoaded)
                return;
            OwnerControl.Selection.OnItalic();
            OwnerControl.Focus(FocusState.Pointer);
            base.ExecuteCommand(parameter);
        }
    }
    #endregion

    #region FontColorCommand
    public class FontColorCommand : CommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FontColorCommand"/> class.
        /// </summary>
        /// <param name="richTextBoxAdv">The rich text box adv.</param>
        public FontColorCommand(SfRichTextBoxAdv richTextBoxAdv)
            : base(richTextBoxAdv)
        {
        }
        /// <summary>
        /// Executes the font color command.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        protected override void ExecuteCommand(object parameter)
        {
            if (OwnerControl.IsReadOnlyMode || !OwnerControl.IsDocumentLoaded)
                return;
            if (parameter is Color)
                OwnerControl.Selection.OnFontColor((Color)parameter);
            OwnerControl.Focus(FocusState.Pointer);
            base.ExecuteCommand(parameter);
        }
    }
    #endregion

    #region FontFamilyCommand
    public class FontFamilyCommand : CommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FontFamilyCommand"/> class.
        /// </summary>
        /// <param name="richTextBoxAdv">The rich text box adv.</param>
        public FontFamilyCommand(SfRichTextBoxAdv richTextBoxAdv)
            : base(richTextBoxAdv)
        {
        }
        /// <summary>
        /// Executes the  font family command.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        protected override void ExecuteCommand(object parameter)
        {
            if (OwnerControl.IsReadOnlyMode || !OwnerControl.IsDocumentLoaded)
                return;
            if (parameter is string)
            {
                string fontName = parameter.ToString();
                FontFamily fontFamily = new FontFamily(fontName);
                OwnerControl.Selection.OnFontFamily(fontFamily);
            }
            OwnerControl.Focus(FocusState.Pointer);
            base.ExecuteCommand(parameter);
        }
    }
    #endregion

    #region FontSizeCommand
    public class FontSizeCommand : CommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FontSizeCommand"/> class.
        /// </summary>
        /// <param name="richTextBoxAdv">The rich text box adv.</param>
        public FontSizeCommand(SfRichTextBoxAdv richTextBoxAdv)
            : base(richTextBoxAdv)
        {
        }
        /// <summary>
        /// Executes the font size command.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        protected override void ExecuteCommand(object parameter)
        {
            if (OwnerControl.IsReadOnlyMode || !OwnerControl.IsDocumentLoaded)
                return;
            if (parameter is double)
                OwnerControl.Selection.OnFontSize((double)parameter);
            else if (parameter is string)
                OwnerControl.Selection.OnFontSize(double.Parse(parameter.ToString()));
            OwnerControl.Focus(FocusState.Pointer);
            base.ExecuteCommand(parameter);
        }
    }
    #endregion

    #region HighlightColorCommand
    public class HighlightColorCommand : CommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HighlightColorCommand"/> class.
        /// </summary>
        /// <param name="richTextBoxAdv">The rich text box adv.</param>
        public HighlightColorCommand(SfRichTextBoxAdv richTextBoxAdv)
            : base(richTextBoxAdv)
        {
        }
        /// <summary>
        /// Executes the HighlightColor command.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        protected override void ExecuteCommand(object parameter)
        {
            if (OwnerControl.IsReadOnlyMode || !OwnerControl.IsDocumentLoaded)
                return;
            HighlightColor highlightColor;
            if (Enum.IsDefined(typeof(HighlightColor), parameter))
                OwnerControl.Selection.OnHighlightColor((HighlightColor)parameter);
            else if (parameter is string && Enum.TryParse(parameter.ToString(), true, out highlightColor))
                OwnerControl.Selection.OnHighlightColor(highlightColor);
            OwnerControl.Focus(FocusState.Pointer);
            base.ExecuteCommand(parameter);
        }
    }
    #endregion

    #region BaselineAlignmentCommand
    public class BaselineAlignmentCommand : CommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaselineAlignmentCommand"/> class.
        /// </summary>
        /// <param name="richTextBoxAdv">The rich text box adv.</param>
        public BaselineAlignmentCommand(SfRichTextBoxAdv richTextBoxAdv)
            : base(richTextBoxAdv)
        {
        }
        /// <summary>
        /// Executes the BaselineAlignment command.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        protected override void ExecuteCommand(object parameter)
        {
            if (OwnerControl.IsReadOnlyMode || !OwnerControl.IsDocumentLoaded)
                return;
            BaselineAlignment baselineAlignment;
            if (Enum.IsDefined(typeof(BaselineAlignment), parameter))
                OwnerControl.Selection.OnBaselineAlignment((BaselineAlignment)parameter);
            else if (parameter is string && Enum.TryParse(parameter.ToString(), true, out baselineAlignment))
                OwnerControl.Selection.OnBaselineAlignment(baselineAlignment);
            OwnerControl.Focus(FocusState.Pointer);
            base.ExecuteCommand(parameter);
        }
    }
    #endregion

    #region StrikeThroughCommand
    public class StrikeThroughCommand : CommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StrikeThroughCommand"/> class.
        /// </summary>
        /// <param name="richTextBoxAdv">The rich text box adv.</param>
        public StrikeThroughCommand(SfRichTextBoxAdv richTextBoxAdv)
            : base(richTextBoxAdv)
        {
        }
        /// <summary>
        /// Executes the StrikeThrough command.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        protected override void ExecuteCommand(object parameter)
        {
            if (OwnerControl.IsReadOnlyMode || !OwnerControl.IsDocumentLoaded)
                return;
            StrikeThrough strikeThrough;
            if (Enum.IsDefined(typeof(StrikeThrough), parameter))
                OwnerControl.Selection.OnStrikeThrough((StrikeThrough)parameter);
            else if (parameter is string && Enum.TryParse(parameter.ToString(), true, out strikeThrough))
                OwnerControl.Selection.OnStrikeThrough(strikeThrough);
            OwnerControl.Focus(FocusState.Pointer);
            base.ExecuteCommand(parameter);
        }
    }
    #endregion

    #region UnderlineCommand
    public class UnderlineCommand : CommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnderlineCommand"/> class.
        /// </summary>
        /// <param name="richTextBoxAdv">The rich text box adv.</param>
        public UnderlineCommand(SfRichTextBoxAdv richTextBoxAdv)
            : base(richTextBoxAdv)
        {
        }
        /// <summary>
        /// Executes the underline command.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        protected override void ExecuteCommand(object parameter)
        {
            if (OwnerControl.IsReadOnlyMode || !OwnerControl.IsDocumentLoaded)
                return;
            Underline underline;
            if (parameter != null && Enum.IsDefined(typeof(Underline), parameter))
                OwnerControl.Selection.OnUnderline((Underline)parameter);
            else if (parameter is string && Enum.TryParse(parameter.ToString(), true, out underline))
                OwnerControl.Selection.OnUnderline(underline);
            else
                OwnerControl.Selection.ToggleUnderline();
            OwnerControl.Focus(FocusState.Pointer);
            base.ExecuteCommand(parameter);
        }
    }
    #endregion
}
