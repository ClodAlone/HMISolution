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
    internal class InsertHyperlinkCommand : CommandBase 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InsertHyperlinkCommand"/> class.
        /// </summary>
        /// <param name="richTextBoxAdv">The rich text box adv.</param>
        public InsertHyperlinkCommand(SfRichTextBoxAdv richTextBoxAdv)
            : base(richTextBoxAdv)
        {
        }
        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        protected override void ExecuteCommand(object parameter)
        {
            if (OwnerControl.IsReadOnlyMode || !OwnerControl.IsDocumentLoaded)
                return;
            OwnerControl.ShowHyperlinkUI();
            OwnerControl.Focus(FocusState.Pointer);
            base.ExecuteCommand(parameter);
        }
    }
}
