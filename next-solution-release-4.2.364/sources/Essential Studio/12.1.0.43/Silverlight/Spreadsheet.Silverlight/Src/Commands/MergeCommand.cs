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
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Syncfusion.Windows.Controls.Spreadsheet.CommandExtensions;

namespace Syncfusion.Windows.Controls.Spreadsheet.Commands
{
     public class MergeCommand:CommandBase
    {
         public MergeCommand(SpreadsheetControl excelEditorControl)
             : base(excelEditorControl)
        {
        }

        public override bool CanExcuteCommand(object parameter)
        {
            if (this.AssociatedSpreadsheet != null && this.AssociatedSpreadsheet.GridProperties != null && !this.AssociatedSpreadsheet.GridProperties.IsEditing)
                return true;
            else
                return false;
            //return base.CanExcuteCommand(parameter);
        }

        protected override void ExecuteCommand(object parameter)
        {
            if (parameter != null && !string.IsNullOrEmpty(parameter.ToString()))
            {
                switch(parameter.ToString())
                {
                    case "MergeAndCenter":
                        this.AssociatedSpreadsheet.MergeCells(true);
                        break;
                    default:
                        this.AssociatedSpreadsheet.MergeCells(false);
                        break;
                }
            }
            base.ExecuteCommand(parameter);
        }
    }
}
