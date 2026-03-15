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
    #region Delete Table Command
    public class DeleteTableCommand : CommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteTableCommand"/> class.
        /// </summary>
        /// <param name="richTextBoxAdv">The rich text box adv.</param>
        public DeleteTableCommand(SfRichTextBoxAdv richTextBoxAdv)
            : base(richTextBoxAdv)
        {
        }
        /// <summary>
        /// Determines whether this instance can execute command with the specified parameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>
        ///   <c>true</c> if this instance can execute command with the specified parameter; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanExecuteCommand(object parameter)
        {
            if (OwnerControl != null)
                return OwnerControl.Selection.Start.Paragraph.IsInsideTable;
            return false;
        }
        /// <summary>
        /// Executes the delete table command.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        protected override void ExecuteCommand(object parameter)
        {
            if (OwnerControl.IsReadOnlyMode || !OwnerControl.IsDocumentLoaded)
                return;
            OwnerControl.DeleteTableFromBlocks();
            OwnerControl.Focus(FocusState.Pointer);
            base.ExecuteCommand(parameter);
        }
    }
    #endregion

    #region Delete Row Command
    public class DeleteRowCommand : CommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteRowCommand"/> class.
        /// </summary>
        /// <param name="richTextBoxAdv">The rich text box adv.</param>
        public DeleteRowCommand(SfRichTextBoxAdv richTextBoxAdv)
            : base(richTextBoxAdv)
        {
        }
        /// <summary>
        /// Determines whether this instance can execute command with the specified parameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>
        ///   <c>true</c> if this instance can execute command with the specified parameter; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanExecuteCommand(object parameter)
        {
            if (OwnerControl != null)
                return OwnerControl.Selection.Start.Paragraph.IsInsideTable;
            return false;
        }
        /// <summary>
        /// Executes the delete row command.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        protected override void ExecuteCommand(object parameter)
        {
            if (OwnerControl.IsReadOnlyMode || !OwnerControl.IsDocumentLoaded)
                return;
            OwnerControl.DeleteRowFromTable();
            OwnerControl.Focus(FocusState.Pointer);
            base.ExecuteCommand(parameter);
        }
    }
    #endregion

    #region Delete Column Command
    public class DeleteColumnCommand : CommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteColumnCommand"/> class.
        /// </summary>
        /// <param name="richTextBoxAdv">The rich text box adv.</param>
        public DeleteColumnCommand(SfRichTextBoxAdv richTextBoxAdv)
            : base(richTextBoxAdv)
        {
        }
        /// <summary>
        /// Determines whether this instance can execute command with the specified parameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>
        ///   <c>true</c> if this instance can execute command with the specified parameter; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanExecuteCommand(object parameter)
        {
            if (OwnerControl != null)
                return OwnerControl.Selection.Start.Paragraph.IsInsideTable;
            return false;
        }
        /// <summary>
        /// Executes the delete column command.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        protected override void ExecuteCommand(object parameter)
        {
            if (OwnerControl.IsReadOnlyMode || !OwnerControl.IsDocumentLoaded)
                return;
            OwnerControl.DeleteColumnFromTable();
            OwnerControl.Focus(FocusState.Pointer);
            base.ExecuteCommand(parameter);
        }
    }
    #endregion
}
