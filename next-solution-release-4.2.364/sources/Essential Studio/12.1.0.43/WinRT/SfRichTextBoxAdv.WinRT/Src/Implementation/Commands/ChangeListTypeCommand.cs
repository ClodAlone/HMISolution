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
    internal class ChangeListTypeCommand : CommandBase
    {
        internal ChangeListTypeCommand(SfRichTextBoxAdv richTextBoxAdv)
            : base(richTextBoxAdv)
        {
        }

       protected override void  ExecuteCommand(object parameter)
       {
           string listType = parameter as string;
           OwnerControl.ChangeListType(listType);
           OwnerControl.Focus(FocusState.Pointer);
     	   base.ExecuteCommand(parameter);
       }
    }
}
