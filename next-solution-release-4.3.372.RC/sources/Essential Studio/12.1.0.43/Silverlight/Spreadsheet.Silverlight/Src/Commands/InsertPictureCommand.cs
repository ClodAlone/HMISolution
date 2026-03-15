#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Windows.Controls;
using System.IO;
using Syncfusion.Windows.Controls.Grid.Converter;
using Syncfusion.XlsIO;
using Syncfusion.Windows.Controls.Grid;
#if !SILVERLIGHT
using Microsoft.Win32;
using System;
using System.Windows;
#endif

namespace Syncfusion.Windows.Controls.Spreadsheet.Commands
{
    public class InsertPictureCommand : CommandBase
    {
        public InsertPictureCommand(SpreadsheetControl excelEditorControl)
            : base(excelEditorControl)
        {

        }

        public override bool CanExcuteCommand(object parameter)
        {
            if (this.AssociatedSpreadsheet != null && this.AssociatedSpreadsheet.GridProperties != null 
                && !this.AssociatedSpreadsheet.GridProperties.IsEditing && !this.AssociatedSpreadsheet.GridProperties.IsDisableMode)
                return true;
            else
                return false;
            //return base.CanExcuteCommand(parameter);
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
            openFileDialog.Filter = "JPEG File Interchange Format(*.jpg)|*.jpg;*.jpeg;*.jpe;*.jfif|Graphics Interchange Format(*.gif)|*.gif|Windows Bitmap(*.bmp,*.dip)|*.bmp;*.dip|Portable Network Graphics (*.png)|*.png";
                
            
#if !SILVERLIGHT
            if ((bool)openFileDialog.ShowDialog())
            {
                try
                {
                    if ((stream = openFileDialog.OpenFile()) != null)
                    {
                        file = new byte[stream.Length];
                        stream.Read(file, 0, (int)stream.Length);
                    }
                    if (stream != null) stream.Close();
                    if (file != null)
                    {
                        var stream1 = new MemoryStream(file);
                              
                        IWorksheet worksheet = AssociatedSpreadsheet.ExcelProperties.WorkBook.Worksheets[AssociatedSpreadsheet.GridProperties.CurrentSheetName] as IWorksheet;                        
                        worksheet.Pictures.AddPicture(this.AssociatedSpreadsheet.GridProperties.CurrentCell.RowIndex, this.AssociatedSpreadsheet.GridProperties.CurrentCell.ColumnIndex,stream1);
                        if (AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.CommandStack.ShouldGenerateUndoInfo)
                        {
                            GridStyleInfo[] cellsInfo = new GridStyleInfo[] { };
                            GridRangeInfo range = GridRangeInfo.Cell(this.AssociatedSpreadsheet.GridProperties.CurrentCell.RowIndex, this.AssociatedSpreadsheet.GridProperties.CurrentCell.ColumnIndex);
                            SpreadsheetPictureCommand pictureCommand = new SpreadsheetPictureCommand(AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel, range, cellsInfo, Styles.StyleModifyType.Copy, stream1, true);
                            AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.CommandStack.Push(pictureCommand);
                        }
                        AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.InvalidateCell(GridRangeInfo.Cell(this.AssociatedSpreadsheet.GridProperties.CurrentCell.RowIndex, this.AssociatedSpreadsheet.GridProperties.CurrentCell.ColumnIndex));
                        ExcelGridModelImportExtensions.CopyImageToGrid(worksheet, AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel);
                        AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.InvalidateVisual();
                    }
                }
                catch (Exception)
                {

                    MessageBox.Show("Error:Please Select Cell in Which you have to insert Picture: ","Error" );
                }
           }
            //else
            //{
            //    MessageBox.Show("Problem occured, try again later");
            //}
#else
            openFileDialog.ShowDialog();
            if (openFileDialog.File != null)
            {
                if (openFileDialog.File.Exists)
                {                   
                    stream = openFileDialog.File.OpenRead();
                    file = new byte[stream.Length];
                    stream.Read(file, 0, (int)stream.Length);
                }

                if (stream != null) stream.Close();

                if (file != null)
                {
                    var stream1 = new MemoryStream(file);
                    System.Drawing.Image img1 = new System.Drawing.Image(stream1);
                    IWorksheet worksheet = AssociatedSpreadsheet.ExcelProperties.WorkBook.Worksheets[AssociatedSpreadsheet.GridProperties.CurrentSheetName] as IWorksheet;
                    worksheet.Pictures.AddPicture(this.AssociatedSpreadsheet.GridProperties.CurrentCell.RowIndex, this.AssociatedSpreadsheet.GridProperties.CurrentCell.ColumnIndex,img1);
                    AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.InvalidateCell(GridRangeInfo.Cell(this.AssociatedSpreadsheet.GridProperties.CurrentCell.RowIndex, this.AssociatedSpreadsheet.GridProperties.CurrentCell.ColumnIndex));
                    AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.CoveredCells.Clear();
                    ExcelGridModelImportExtensions.CopyImageToGrid(worksheet, AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel);
                    AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.InvalidateVisual();
                }
            }
#endif
            base.ExecuteCommand(parameter);
        }
    }
}
    
