#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Win32;
using Syncfusion.XlsIO;

namespace Syncfusion.Windows.Controls.Spreadsheet.Commands
{
    public class SaveCommand : CommandBase
    {
        public SaveCommand(SpreadsheetControl excelEditorControl): base(excelEditorControl)
        {

        }

        protected override void ExecuteCommand(object parameter)
        {
            try
            {
                if (AssociatedSpreadsheet.ExcelProperties.FileName != null && AssociatedSpreadsheet.ExcelProperties.FileName != string.Empty)
                {
                    string tmpFullPath = Path.GetFullPath(AssociatedSpreadsheet.ExcelProperties.FileName);
                    AssociatedSpreadsheet.ExcelProperties.WorkBook.SaveAs(tmpFullPath);
                }
                else
                {
                    SaveFileDialog sfd = new SaveFileDialog
                    {
                        FilterIndex = 2,
                        Filter = "Excel 97 to 2003 Files(*.xls)|*.xls|Excel 2007 to 2010 Files(*.xlsx)|*.xlsx"
                    };

                    if (sfd.ShowDialog() == true)
                    {
                        using (Stream stream = sfd.OpenFile())
                        {
                            if (sfd.FilterIndex == 1)
                                AssociatedSpreadsheet.ExcelProperties.WorkBook.Version = ExcelVersion.Excel97to2003;
                            else
                                AssociatedSpreadsheet.ExcelProperties.WorkBook.Version = ExcelVersion.Excel2010;
                            AssociatedSpreadsheet.ExcelProperties.WorkBook.SaveAs(stream);
                            AssociatedSpreadsheet.ExcelProperties.FileName = sfd.FileName;
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
            base.ExecuteCommand(parameter);
        }
    }
}
