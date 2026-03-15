#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Windows;
using Syncfusion.Windows.Controls.Spreadsheet.CommandExtensions;

namespace Syncfusion.Windows.Controls.Spreadsheet.Commands
{
    public class HorizontalAlignmentCommand:CommandBase
    {
        public HorizontalAlignmentCommand(SpreadsheetControl excelEditorControl) : base(excelEditorControl)
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
            if (parameter != null)
            {
                var horizontalAlignment = parameter.ToString();
                switch (horizontalAlignment)
                {
                    case "HorizontalAlignment.Center":
                        AssociatedSpreadsheet.SetHorizontalAlignment(HorizontalAlignment.Center);
                        break;
                    case "HorizontalAlignment.Right":
                        AssociatedSpreadsheet.SetHorizontalAlignment(HorizontalAlignment.Right);
                        break;
                    default:
                        AssociatedSpreadsheet.SetHorizontalAlignment(HorizontalAlignment.Left);
                        break;
                }
                AssociatedSpreadsheet.GridProperties.RefreshGridProperties("HorizontalAlignment");
                AssociatedSpreadsheet.Focus();
                base.ExecuteCommand(parameter);
            }
        }
    }
}
