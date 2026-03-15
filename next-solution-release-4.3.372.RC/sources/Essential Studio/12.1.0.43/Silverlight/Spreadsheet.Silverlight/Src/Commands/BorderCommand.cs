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
    public class BorderCommand : CommandBase
    {
        public BorderCommand(SpreadsheetControl excelEditorControl)
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
                switch (parameter.ToString())
                {
                    case "BottomBorder":
                        this.AssociatedSpreadsheet.ChangeBorder(ExcelBorderStyle.BottomBorder);
                        break;
                    case "TopBorder":
                        this.AssociatedSpreadsheet.ChangeBorder(ExcelBorderStyle.TopBorder);
                        break;
                    case "LeftBorder":
                        this.AssociatedSpreadsheet.ChangeBorder(ExcelBorderStyle.LeftBorder);
                        break;
                    case "RightBorder":
                        this.AssociatedSpreadsheet.ChangeBorder(ExcelBorderStyle.RightBorder);
                        break;
                    case "AllBorder":
                        this.AssociatedSpreadsheet.ChangeBorder(ExcelBorderStyle.AllBorder);
                        break;
                    case "OutSideBorder":
                        this.AssociatedSpreadsheet.ChangeBorder(ExcelBorderStyle.OutSideBorder);
                        break;
                    case "ThickBoxBorder":
                        this.AssociatedSpreadsheet.ChangeBorder(ExcelBorderStyle.ThickBoxBorder);
                        break;
                    case "ThickBottomBorder":
                        this.AssociatedSpreadsheet.ChangeBorder(ExcelBorderStyle.ThickBottomBorder);
                        break;
                    case "TopAndBottomBorder":
                        this.AssociatedSpreadsheet.ChangeBorder(ExcelBorderStyle.TopAndBottomBorder);
                        break;
                    case "TopAndThickBottomBorder":
                        this.AssociatedSpreadsheet.ChangeBorder(ExcelBorderStyle.TopAndThickBottomBorder);
                        break;
                    case "OutsideBorder":
                        this.AssociatedSpreadsheet.ChangeBorder(ExcelBorderStyle.OutSideBorder);
                        break;
                    default:
                        this.AssociatedSpreadsheet.ChangeBorder(ExcelBorderStyle.NoBorder);
                        break;
                }
            }
            base.ExecuteCommand(parameter);
        }
    }

}
