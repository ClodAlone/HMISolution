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
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml;

namespace Syncfusion.UI.Xaml.RichTextBoxAdv
{
    #region Copy Command
    public class CopyCommand : CommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CopyCommand"/> class.
        /// </summary>
        /// <param name="richTextBoxAdv">The rich text box adv.</param>
        public CopyCommand(SfRichTextBoxAdv richTextBoxAdv)
            : base(richTextBoxAdv)
        {
        }
        /// <summary>
        /// Determines whether this instance can execute copy command with the specified parameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>
        ///   <c>true</c> if this instance can execute copy command with the specified parameter; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanExecuteCommand(object parameter)
        {
            if (OwnerControl != null)
                return !OwnerControl.Selection.IsEmpty;
            return false;
        }
        /// <summary>
        /// Executes the copy command.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        protected override void ExecuteCommand(object parameter)
        {
            OwnerControl.Selection.Copy();
            OwnerControl.Focus(FocusState.Pointer);
            base.ExecuteCommand(parameter);
        }
    }
    #endregion

    #region Cut Command
    public class CutCommand : CommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CutCommand"/> class.
        /// </summary>
        /// <param name="richTextBoxAdv">The rich text box adv.</param>
        public CutCommand(SfRichTextBoxAdv richTextBoxAdv)
            : base(richTextBoxAdv)
        {
        }
        /// <summary>
        /// Determines whether this instance can execute cut command with the specified parameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>
        ///   <c>true</c> if this instance can execute cut command with the specified parameter; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanExecuteCommand(object parameter)
        {
            if (OwnerControl != null && !OwnerControl.IsReadOnlyMode)
                return !OwnerControl.Selection.IsEmpty;
            return false;
        }
        /// <summary>
        /// Executes the cut command.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        protected override void ExecuteCommand(object parameter)
        {
            if (OwnerControl.IsReadOnlyMode || !OwnerControl.IsDocumentLoaded)
                return;
            OwnerControl.Selection.Cut();
            OwnerControl.Focus(FocusState.Pointer);
            base.ExecuteCommand(parameter);
        }
    }
    #endregion

    #region Paste Command
    public class PasteCommand : CommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PasteCommand"/> class.
        /// </summary>
        /// <param name="richTextBoxAdv">The rich text box adv.</param>
        public PasteCommand(SfRichTextBoxAdv richTextBoxAdv)
            : base(richTextBoxAdv)
        {
        }
        /// <summary>
        /// Determines whether this instance can execute paste command with the specified parameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>
        ///   <c>true</c> if this instance can execute paste command with the specified parameter; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanExecuteCommand(object parameter)
        {
            if (OwnerControl != null)
            {
                DataPackageView dataView = Clipboard.GetContent();
                return dataView.Contains(StandardDataFormats.Text) || dataView.Contains(StandardDataFormats.Rtf) || dataView.Contains(StandardDataFormats.Bitmap);
            }
            return false;
        }
        /// <summary>
        /// Executes the paste command.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        protected override void ExecuteCommand(object parameter)
        {
            if (OwnerControl.IsReadOnlyMode || !OwnerControl.IsDocumentLoaded)
                return;
            OwnerControl.Selection.Paste();
            OwnerControl.Focus(FocusState.Pointer);
            base.ExecuteCommand(parameter);
        }
    }
    #endregion
}
