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
    internal class FontDialogCommand : CommandBase
    {
        public FontDialogCommand(SfRichTextBoxAdv richText):base(richText)
        {

        }

        protected override void ExecuteCommand(object parameter)
        {
            if (OwnerControl != null)
            {
                //FontDialog fontDialog = new FontDialog(AssociatedRichEditor);
                //fontDialog.ShowDialog();
                //AssociatedRichEditor.FontWindow.ShowDialog();
            }
            base.ExecuteCommand(parameter);
        }
    }
}
