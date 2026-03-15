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
    #region Undo Command
    public class UndoCommand : CommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UndoCommand"/> class.
        /// </summary>
        /// <param name="richTextBoxAdv">The rich text box adv.</param>
        public UndoCommand(SfRichTextBoxAdv richTextBoxAdv)
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
                return OwnerControl.History.UndoStack.Count > 0;

            return false;
        }
        /// <summary>
        /// Executes the undo command.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        protected override void ExecuteCommand(object parameter)
        {
            OwnerControl.History.Undo();
            OwnerControl.Focus(FocusState.Pointer);
            base.ExecuteCommand(parameter);
        }
    }
    #endregion

    #region Redo Command
    public class RedoCommand : CommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RedoCommand"/> class.
        /// </summary>
        /// <param name="richTextBoxAdv">The rich text box adv.</param>
        public RedoCommand(SfRichTextBoxAdv richTextBoxAdv)
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
                return OwnerControl.History.RedoStack.Count > 0;

            return false;
        }
        /// <summary>
        /// Executes the redo command.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        protected override void ExecuteCommand(object parameter)
        {
            OwnerControl.History.Redo();
            OwnerControl.Focus(FocusState.Pointer);
            base.ExecuteCommand(parameter);
        }
    }
    #endregion
}
