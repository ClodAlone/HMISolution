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
using Windows.UI.Xaml;

namespace Syncfusion.UI.Xaml.RichTextBoxAdv
{
    #region AfterSpacingCommand
    public class AfterSpacingCommand : CommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AfterSpacingCommand"/> class.
        /// </summary>
        /// <param name="richTextBoxAdv">The rich text box adv.</param>
        public AfterSpacingCommand(SfRichTextBoxAdv richTextBoxAdv)
            : base(richTextBoxAdv)
        {
        }
        /// <summary>
        /// Executes the after space command.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        protected override void ExecuteCommand(object parameter)
        {
            if (OwnerControl.IsReadOnlyMode || !OwnerControl.IsDocumentLoaded)
                return;
            if (parameter is double)
                OwnerControl.Selection.OnAfterSpacing((double)parameter);
            else if (parameter is string)
                OwnerControl.Selection.OnAfterSpacing(double.Parse(parameter.ToString()));
            OwnerControl.Focus(FocusState.Pointer);
            base.ExecuteCommand(parameter);
        }
    }
    #endregion

    #region BeforeSpacingCommand
    public class BeforeSpacingCommand : CommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BeforeSpacingCommand"/> class.
        /// </summary>
        /// <param name="richTextBoxAdv">The rich text box adv.</param>
        public BeforeSpacingCommand(SfRichTextBoxAdv richTextBoxAdv)
            : base(richTextBoxAdv)
        {
        }
        /// <summary>
        /// Executes the before space command.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        protected override void ExecuteCommand(object parameter)
        {
            if (OwnerControl.IsReadOnlyMode || !OwnerControl.IsDocumentLoaded)
                return;
            if (parameter is double)
                OwnerControl.Selection.OnBeforeSpacing((double)parameter);
            else if (parameter is string)
                OwnerControl.Selection.OnBeforeSpacing(double.Parse(parameter.ToString()));
            OwnerControl.Focus(FocusState.Pointer);
            base.ExecuteCommand(parameter);
        }
    }
    #endregion

    #region LeftIndentCommand
    public class LeftIndentCommand : CommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LeftIndentCommand"/> class.
        /// </summary>
        /// <param name="richTextBoxAdv">The rich text box adv.</param>
        public LeftIndentCommand(SfRichTextBoxAdv richTextBoxAdv)
            : base(richTextBoxAdv)
        {
        }
        /// <summary>
        /// Executes the left indent command.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        protected override void ExecuteCommand(object parameter)
        {
            if (OwnerControl.IsReadOnlyMode || !OwnerControl.IsDocumentLoaded)
                return;
            if (parameter is double)
                OwnerControl.Selection.OnLeftIndent((double)parameter);
            else if (parameter is string)
                OwnerControl.Selection.OnLeftIndent(double.Parse(parameter.ToString()));
            OwnerControl.Focus(FocusState.Pointer);
            base.ExecuteCommand(parameter);
        }
    }
    #endregion

    #region RightIndentCommand
    public class RightIndentCommand : CommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RightIndentCommand"/> class.
        /// </summary>
        /// <param name="richTextBoxAdv">The rich text box adv.</param>
        public RightIndentCommand(SfRichTextBoxAdv richTextBoxAdv)
            : base(richTextBoxAdv)
        {
        }
        /// <summary>
        /// Executes the right indent command.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        protected override void ExecuteCommand(object parameter)
        {
            if (OwnerControl.IsReadOnlyMode || !OwnerControl.IsDocumentLoaded)
                return;
            if (parameter is double)
                OwnerControl.Selection.OnRightIndent((double)parameter);
            else if (parameter is string)
                OwnerControl.Selection.OnRightIndent(double.Parse(parameter.ToString()));
            OwnerControl.Focus(FocusState.Pointer);
            base.ExecuteCommand(parameter);
        }
    }
    #endregion

    #region FirstLineIndentCommand
    public class FirstLineIndentCommand : CommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FirstLineIndentCommand"/> class.
        /// </summary>
        /// <param name="richTextBoxAdv">The rich text box adv.</param>
        public FirstLineIndentCommand(SfRichTextBoxAdv richTextBoxAdv)
            : base(richTextBoxAdv)
        {
        }
        /// <summary>
        /// Executes the first line indent command.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        protected override void ExecuteCommand(object parameter)
        {
            if (OwnerControl.IsReadOnlyMode || !OwnerControl.IsDocumentLoaded)
                return;
            if (parameter is double)
                OwnerControl.Selection.OnFirstLineIndent((double)parameter);
            else if (parameter is string)
                OwnerControl.Selection.OnFirstLineIndent(double.Parse(parameter.ToString()));
            OwnerControl.Focus(FocusState.Pointer);
            base.ExecuteCommand(parameter);
        }
    }
    #endregion

    #region LineSpacingTypeCommand
    public class LineSpacingTypeCommand : CommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LineSpacingTypeCommand"/> class.
        /// </summary>
        /// <param name="richTextBoxAdv">The rich text box adv.</param>
        public LineSpacingTypeCommand(SfRichTextBoxAdv richTextBoxAdv)
            : base(richTextBoxAdv)
        {
        }
        /// <summary>
        /// Executes the line space type command.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        protected override void ExecuteCommand(object parameter)
        {
            if (OwnerControl.IsReadOnlyMode || !OwnerControl.IsDocumentLoaded)
                return;
            LineSpacingType lineSpacingType;
            if (Enum.IsDefined(typeof(LineSpacingType), parameter))
                OwnerControl.Selection.OnLineSpacingType((LineSpacingType)parameter);
            else if (parameter is string && Enum.TryParse(parameter.ToString(), true, out lineSpacingType))
                OwnerControl.Selection.OnLineSpacingType(lineSpacingType);
            OwnerControl.Focus(FocusState.Pointer);
            base.ExecuteCommand(parameter);
        }
    }
    #endregion

    #region LineSpacingCommand
    public class LineSpacingCommand : CommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LineSpacingCommand"/> class.
        /// </summary>
        /// <param name="richTextBoxAdv">The rich text box adv.</param>
        public LineSpacingCommand(SfRichTextBoxAdv richTextBoxAdv)
            : base(richTextBoxAdv)
        {
        }
        /// <summary>
        /// Executes the line space command.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        protected override void ExecuteCommand(object parameter)
        {
            if (OwnerControl.IsReadOnlyMode || !OwnerControl.IsDocumentLoaded)
                return;
            if (parameter is double)
                OwnerControl.Selection.OnLineSpacing((double)parameter);
            else if (parameter is string)
                OwnerControl.Selection.OnLineSpacing(double.Parse(parameter.ToString()));
            OwnerControl.Focus(FocusState.Pointer);
            base.ExecuteCommand(parameter);
        }
    }
    #endregion

    #region TextAlignmentCommand
    public class TextAlignmentCommand : CommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextAlignmentCommand"/> class.
        /// </summary>
        /// <param name="richTextBoxAdv">The rich text box adv.</param>
        public TextAlignmentCommand(SfRichTextBoxAdv richTextBoxAdv)
            : base(richTextBoxAdv)
        {
        }
        /// <summary>
        /// Executes the text alignment command.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        protected override void ExecuteCommand(object parameter)
        {
            if (OwnerControl.IsReadOnlyMode || !OwnerControl.IsDocumentLoaded)
                return;
            TextAlignment textAlignment;
            if (Enum.IsDefined(typeof(TextAlignment), parameter))
                OwnerControl.Selection.OnTextAlignment((TextAlignment)parameter);
            else if (parameter is string && Enum.TryParse(parameter.ToString(), true, out textAlignment))
                OwnerControl.Selection.OnTextAlignment(textAlignment);
            OwnerControl.Focus(FocusState.Pointer);
            base.ExecuteCommand(parameter);
        }
    }
    #endregion
}
