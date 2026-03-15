#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Windows.Controls;
using System.IO;
using Syncfusion.XlsIO;
using Syncfusion.Compression.Zip;

#if !SILVERLIGHT
using Microsoft.Win32;
#endif

namespace Syncfusion.Windows.Controls.Spreadsheet.Commands
{
    public class ExportToExcelCommand : CommandBase
    {
        public ExportToExcelCommand(SpreadsheetControl excelEditorControl) : base(excelEditorControl)
        {

        }

        protected override void ExecuteCommand(object parameter)
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
                    string extension = sfd.SafeFileName.Split('.')[1];
                    
                    if (extension == "xls")
                        this.AssociatedSpreadsheet.ExcelProperties.WorkBook.Version = ExcelVersion.Excel97to2003;
                    else
                        this.AssociatedSpreadsheet.ExcelProperties.WorkBook.Version = ExcelVersion.Excel2010;
                    AssociatedSpreadsheet.ExcelProperties.WorkBook.SaveAs(stream);

                    if (AssociatedSpreadsheet.ExcelProperties.WorkBook.Version != ExcelVersion.Excel97to2003)
                    {
                        Stream xdoc = AssociatedSpreadsheet.GridProperties.ActiveSpreadsheetGrid.CreateXMLFile();
                        ZipArchive archive = new ZipArchive();
                        archive.Open(stream, true);
                        if (archive["Syncfusion"] == null)
                            archive.AddItem("Syncfusion", null, false, FileAttributes.Directory);
                        if (archive["Syncfusion/Custom.xml"] != null)
                            archive.UpdateItem("Syncfusion/Custom.xml", xdoc, false);
                        else
                            archive.AddItem("Syncfusion/Custom.xml", xdoc, true, FileAttributes.Normal);
                        archive.Save(stream, false);
                        archive.Close();
                    }
                }
            }
            base.ExecuteCommand(parameter);
        }
    }
}
