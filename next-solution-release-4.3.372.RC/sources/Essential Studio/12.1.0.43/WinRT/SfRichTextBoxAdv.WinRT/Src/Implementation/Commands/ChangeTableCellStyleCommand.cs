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

namespace Syncfusion.UI.Xaml.RichTextBoxAdv
{
    internal class ChangeTableCellStyleCommand : CommandBase
    {
        public ChangeTableCellStyleCommand(SfRichTextBoxAdv rich):base(rich)
        {

        }

        protected override void  ExecuteCommand(object parameter)
        {
           if(parameter !=null)
           {
               if (OwnerControl != null)
               {
                   OwnerControl.ChangeTableCellBackground((Color)parameter);
               }
 	           base.ExecuteCommand(parameter);
           }
        }

    }
}
