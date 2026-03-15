#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System.Windows.Controls;
using System.IO;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.XlsIO;
using System.Xml.Linq;
using System;

#if !SILVERLIGHT
using Microsoft.Win32;
using System.Windows;
#endif

namespace Syncfusion.Windows.Controls.Spreadsheet.Commands
{
    public class ImportFromExcelCommand : CommandBase
    {
        public ImportFromExcelCommand(SpreadsheetControl excelEditorControl): base(excelEditorControl)
        {

        }

        protected override void ExecuteCommand(object parameter)
        {
            byte[] file = null;
#if !SILVERLIGHT
            Stream stream = null;
#else
            FileStream stream = null;
#endif


            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel 2007 - 2010 Files(*.xlsx)|*.xlsx|Excel 97 - 2003 Files(*.xls)|*.xls|All Files(*.*)|*.*";
#if !SILVERLIGHT
            if ((bool)openFileDialog.ShowDialog())
            {
                try
                {
                    if ((stream = openFileDialog.OpenFile()) != null)
                    {
                        file = new byte[stream.Length];
                        stream.Read(file, 0, (int)stream.Length);
                        AssociatedSpreadsheet.ExcelProperties.FileName = openFileDialog.FileName;
                    }
                    if (stream != null) stream.Close();
                    if (file != null)
                    {
                        var stream1 = new MemoryStream(file);
                        {
                            AssociatedSpreadsheet.ImportExcel(stream1);
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (!ex.Message.Equals("Workbook is protected and password wasn't specified."))
                        MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                    else
                        AssociatedSpreadsheet.New(3);
                }
                //catch (Exception ex)
                //{

                //}
            }
//            else
//            {
//                MessageBox.Show("Problem occured, try again later");
//            }
#else
            openFileDialog.ShowDialog();
            if (openFileDialog.File != null)
            {
                if (openFileDialog.File.Exists)
                {
                    stream = openFileDialog.File.OpenRead();
                    file = new byte[stream.Length];
                    stream.Read(file, 0, (int) stream.Length);
                    AssociatedSpreadsheet.ExcelProperties.FileName = openFileDialog.File.Name;
                }

                if (stream != null) stream.Close();
            }

            if (file != null)
            {
                var stream1 = new MemoryStream(file);
                {
                    try
                    {
                        AssociatedSpreadsheet.ImportFromExcel(stream1);

                        if (AssociatedSpreadsheet.ExcelProperties.WorkBook.Version != ExcelVersion.Excel97to2003)
                        {
                            AssociatedSpreadsheet.ExcelProperties.ChangedXMLCellList = new XElement("ChangedCells");
                            AssociatedSpreadsheet.ExcelProperties.ChangedCellList = AssociatedSpreadsheet.GridProperties.ActiveSpreadsheetGrid.ReadXMLFile(stream1);
                        }

                        if (AssociatedSpreadsheet.ExcelProperties.ChangedCellList == null)
                            AssociatedSpreadsheet.ExcelProperties.ChangedCellList = new BinaryList();
                        else
                            AssociatedSpreadsheet.GridProperties.ActiveSpreadsheetGrid.CreateXElement();

                    }
                    catch (Exception ex)
                    {
                        if (ex.Message == "Workbook is protected and password wasn't specified.")
                        {
                            AssociatedSpreadsheet.New(3);
                        }
                    }
                }
            }

#endif
            base.ExecuteCommand(parameter);
        }
    }
}
