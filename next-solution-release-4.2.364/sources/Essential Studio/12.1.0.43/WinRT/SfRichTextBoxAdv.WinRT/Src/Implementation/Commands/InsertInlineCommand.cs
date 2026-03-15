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
    internal class InsertInlineCommand : CommandBase
    {
        public InsertInlineCommand(SfRichTextBoxAdv rich)
            : base(rich)
        {

        }

        protected override void ExecuteCommand(object parameter)
        {
            if(OwnerControl != null)
                OwnerControl.InsertInlineInParagraph(parameter as Inline);
            OwnerControl.Focus(FocusState.Programmatic);
            base.ExecuteCommand(parameter);
        }

    }
}
