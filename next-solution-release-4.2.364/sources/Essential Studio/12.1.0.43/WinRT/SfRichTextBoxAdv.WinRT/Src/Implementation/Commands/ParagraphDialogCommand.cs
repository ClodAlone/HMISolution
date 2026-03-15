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

namespace Syncfusion.UI.Xaml.RichTextBoxAdv
{
    internal class ParagraphDialogCommand : CommandBase 
    {
        public ParagraphDialogCommand(SfRichTextBoxAdv richTextBox):base(richTextBox)
        {

        }

        protected override void ExecuteCommand(object parameter)
        {
            if (OwnerControl != null)
            {
                //ParagraphDialog paragraphDialog = new ParagraphDialog(AssociatedRichEditor);
                //paragraphDialog.ShowDialog();
                //AssociatedRichEditor.ParagraphWindow.ShowDialog();
            }
            base.ExecuteCommand(parameter);
        }
    }
}
